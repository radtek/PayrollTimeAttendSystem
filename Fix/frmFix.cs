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
    public partial class frmFix : Form
    {
        clsISUtilities clsISUtilities;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;

        private DataSet pvtDataSet;
        private byte[] pvtbytCompress;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;

        private DataView pvtPayCategoryDataView;
        private DataView pvtEmployeePayCategoryDataView;

        private string pvtstrPayrollType = "";
        private string pvtstrPayCategoryNo = "";

        DataGridViewCellStyle EmployeeNotLinkedDataGridViewCellStyle;

        public frmFix()
        {
            InitializeComponent();
        }

        private void frmFix_Load(object sender, EventArgs e)
        {
            clsISUtilities = new clsISUtilities(this, "busFix");

            Set_Form_For_Read();

            EmployeeNotLinkedDataGridViewCellStyle = new DataGridViewCellStyle();
            EmployeeNotLinkedDataGridViewCellStyle.BackColor = Color.Yellow;
            EmployeeNotLinkedDataGridViewCellStyle.SelectionBackColor = Color.Yellow;

            this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
            this.lblSelectedEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

            this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
            this.lblSelectedCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

            this.lblLeaveType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
            this.lblSelectedLeaveType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

            this.lblOccupation.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
            this.lblSelectedOccupation.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

            this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
            this.lblDescription.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
            this.lblListEmployees.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
            this.lblEmployeeLinked.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
            this.lblEmployeeNotLinked.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

            object[] objParm = new object[1];
            objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

            pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

            this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
            this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");

            pvtblnPayrollTypeDataGridViewLoaded = true;

            Load_CurrentForm_Records();
        }

        private void Load_CurrentForm_Records()
        {
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

            for (int intRow = 0; intRow < pvtDataSet.Tables["Employee"].Rows.Count; intRow++)
            {
                this.dgvEmployeeDataGridView.Rows.Add(pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString(),
                                                      pvtDataSet.Tables["Employee"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString(),
                                                      pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                      pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString(),
                                                      intRow.ToString());
            }

            this.Clear_DataGridView(this.dgvCostCentreDataGridView);
            this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);

            for (int intRow = 0; intRow < pvtDataSet.Tables["PayCategory"].Rows.Count; intRow++)
            {
                this.dgvCostCentreDataGridView.Rows.Add(pvtDataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString(),
                                                        pvtDataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                        intRow.ToString());
            }

            this.Clear_DataGridView(this.dgvDepartmentDataGridView);
            this.Clear_DataGridView(this.dgvChosenDepartmentDataGridView);

            for (int intRow = 0; intRow < pvtDataSet.Tables["Department"].Rows.Count; intRow++)
            {
                this.dgvDepartmentDataGridView.Rows.Add(pvtDataSet.Tables["Department"].Rows[intRow]["DEPARTMENT_DESC"].ToString(),
                                                      intRow.ToString());
            }

            this.Clear_DataGridView(this.dgvOccupationDataGridView);
            this.Clear_DataGridView(this.dgvChosenOccupationDataGridView);

            for (int intRow = 0; intRow < pvtDataSet.Tables["Occupation"].Rows.Count; intRow++)
            {
                this.dgvOccupationDataGridView.Rows.Add(pvtDataSet.Tables["Occupation"].Rows[intRow]["OCCUPATION_DESC"].ToString(),
                                                      intRow.ToString());
            }

            this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView));
        }

        private void Set_Form_For_Read()
        {
            this.picPayrollTypeLock.Visible = false;
            this.picPayCategoryLock.Visible = false;

            this.dgvPayrollTypeDataGridView.Enabled = true;
            this.dgvPayCategoryDataGridView.Enabled = true;

            this.btnUpdate.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.btnAdd.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemoveAll.Enabled = false;

            this.btnCostCentreAdd.Enabled = false;
            this.btnCostCentreRemove.Enabled = false;
            this.btnCostCentreAddAll.Enabled = false;
            this.btnCostCentreRemoveAll.Enabled = false;

            this.btnAddDepartment.Enabled = false;
            this.btnRemoveDepartment.Enabled = false;
            this.btnAddAllDepartment.Enabled = false;
            this.btnRemoveAllDepartment.Enabled = false;

            this.btnAddOccupation.Enabled = false;
            this.btnRemoveOccupation.Enabled = false;
            this.btnAddAllOccupation.Enabled = false;
            this.btnRemoveAllOccupation.Enabled = false;

            this.btnAddLink.Enabled = false;
            this.btnAddAllLink.Enabled = false;
            this.btnRemoveLink.Enabled = false;
            this.btnRemoveAllLink.Enabled = false;
        }

        private void Set_Form_For_Edit()
        {
            this.picPayrollTypeLock.Visible = true;
            this.picPayCategoryLock.Visible = true;

            this.dgvPayrollTypeDataGridView.Enabled = false;
            this.dgvPayCategoryDataGridView.Enabled = false;

            this.btnUpdate.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.btnAdd.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemoveAll.Enabled = true;

            this.btnCostCentreAdd.Enabled = true;
            this.btnCostCentreRemove.Enabled = true;
            this.btnCostCentreAddAll.Enabled = true;
            this.btnCostCentreRemoveAll.Enabled = true;

            this.btnAddDepartment.Enabled = true;
            this.btnRemoveDepartment.Enabled = true;
            this.btnAddAllDepartment.Enabled = true;
            this.btnRemoveAllDepartment.Enabled = true;

            this.btnAddOccupation.Enabled = true;
            this.btnRemoveOccupation.Enabled = true;
            this.btnAddAllOccupation.Enabled = true;
            this.btnRemoveAllOccupation.Enabled = true;

            this.btnAddLink.Enabled = true;
            this.btnAddAllLink.Enabled = true;
            this.btnRemoveLink.Enabled = true;
            this.btnRemoveAllLink.Enabled = true;
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)];

                    this.dgvEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvChosenEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                    if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
                    }

                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView, this.dgvChosenEmployeeDataGridView.Rows.Count - 1);
                }
            }
        }

        private void btnAddAll_Click(object sender, System.EventArgs e)
        {
        btnAddAll_Click_Continue:

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                btnAdd_Click(null, null);

                goto btnAddAll_Click_Continue;
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvChosenEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView)];

                    this.dgvChosenEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                    if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView, 0);
                    }

                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1);
                }
            }
        }

        private void btnRemoveAll_Click(object sender, System.EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void Clear_DataGridView(DataGridView myDataGridView)
        {
            myDataGridView.Rows.Clear();

            if (myDataGridView.SortedColumn != null)
            {
                myDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.None;
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
                    case "dgvEmployeeDataGridView":

                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenEmployeeDataGridView":

                        this.dgvChosenEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvCostCentreDataGridView":

                        this.dgvCostCentreDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenCostCentreDataGridView":

                        this.dgvChosenCostCentreDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDepartmentDataGridView":

                        this.dgvDepartmentDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenDepartmentDataGridView":

                        this.dgvChosenDepartmentDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvOccupationDataGridView":

                        this.dgvOccupationDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenOccupationDataGridView":

                        this.dgvChosenOccupationDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayrollTypeDataGridView":

                        pvtintPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        pvtintPayCategoryDataGridViewRowIndex = -1;
                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeListDataGridView":

                        this.dgvEmployeeListDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeSelectedDataGridView":

                        this.dgvEmployeeSelectedDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeNotLinkedDataGridView":

                        this.dgvEmployeeNotLinkedDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvDepartmentDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenDepartmentDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvOccupationDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenOccupationDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

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

        private void btnCostCentreAdd_Click(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvCostCentreDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvCostCentreDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView)];

                    this.dgvCostCentreDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvChosenCostCentreDataGridView.Rows.Add(myDataGridViewRow);

                    if (this.dgvCostCentreDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView, 0);
                    }

                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenCostCentreDataGridView, this.dgvChosenCostCentreDataGridView.Rows.Count - 1);
                }
            }
        }

        private void btnCostCentreRemove_Click(object sender, System.EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvChosenCostCentreDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvChosenCostCentreDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenCostCentreDataGridView)];

                    this.dgvChosenCostCentreDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvCostCentreDataGridView.Rows.Add(myDataGridViewRow);

                    if (this.dgvChosenCostCentreDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvChosenCostCentreDataGridView, 0);
                    }

                    this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView, this.dgvCostCentreDataGridView.Rows.Count - 1);
                }
            }
        }

        private void dgvCostCentreDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnCostCentreAdd_Click(sender, e);
        }

        private void dgvChosenCostCentreDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnCostCentreRemove_Click(sender, e);
        }

        private void btnCostCentreAddAll_Click(object sender, EventArgs e)
        {
        btnCostCentreAddAll_Click_Continue:

            if (this.dgvCostCentreDataGridView.Rows.Count > 0)
            {
                this.btnCostCentreAdd_Click(null, null);

                goto btnCostCentreAddAll_Click_Continue;
            }
        }

        private void btnCostCentreRemoveAll_Click(object sender, EventArgs e)
        {
        btnCostCentreRemoveAll_Click_Continue:

            if (this.dgvChosenCostCentreDataGridView.Rows.Count > 0)
            {
                this.btnCostCentreRemove_Click(null, null);

                goto btnCostCentreRemoveAll_Click_Continue;
            }
        }

        private void btnAddDepartment_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvDepartmentDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvDepartmentDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)];

                    this.dgvDepartmentDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvChosenDepartmentDataGridView.Rows.Add(myDataGridViewRow);

                    if (this.dgvDepartmentDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView, 0);
                    }

                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenDepartmentDataGridView, this.dgvChosenDepartmentDataGridView.Rows.Count - 1);
                }
            }
        }

        private void btnAddAllDepartment_Click(object sender, EventArgs e)
        {
        btnAddAllDepartment_Click_Continue:

            if (this.dgvDepartmentDataGridView.Rows.Count > 0)
            {
                this.btnAddDepartment_Click(null, null);

                goto btnAddAllDepartment_Click_Continue;
            }
        }

        private void btnRemoveDepartment_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvChosenDepartmentDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvChosenDepartmentDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenDepartmentDataGridView)];

                    this.dgvChosenDepartmentDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvDepartmentDataGridView.Rows.Add(myDataGridViewRow);

                    if (this.dgvChosenDepartmentDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvChosenDepartmentDataGridView, 0);
                    }

                    this.Set_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView, this.dgvDepartmentDataGridView.Rows.Count - 1);
                }
            }
        }

        private void btnRemoveAllDepartment_Click(object sender, EventArgs e)
        {
        btnRemoveAllDepartment_Click_Continue:

            if (this.dgvChosenDepartmentDataGridView.Rows.Count > 0)
            {
                this.btnRemoveDepartment_Click(null, null);

                goto btnRemoveAllDepartment_Click_Continue;
            }

        }

        private void btnAddOccupation_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvOccupationDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvOccupationDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvOccupationDataGridView)];

                    this.dgvOccupationDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvChosenOccupationDataGridView.Rows.Add(myDataGridViewRow);

                    if (this.dgvOccupationDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvOccupationDataGridView, 0);
                    }

                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenOccupationDataGridView, this.dgvChosenOccupationDataGridView.Rows.Count - 1);
                }
            }
        }

        private void btnAddAllOccupation_Click(object sender, EventArgs e)
        {
        btnAddAllOccupation_Click_Continue:

            if (this.dgvOccupationDataGridView.Rows.Count > 0)
            {
                this.btnAddOccupation_Click(null, null);

                goto btnAddAllOccupation_Click_Continue;
            }
        }

        private void btnRemoveOccupation_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvChosenOccupationDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvChosenOccupationDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenOccupationDataGridView)];

                    this.dgvChosenOccupationDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvOccupationDataGridView.Rows.Add(myDataGridViewRow);

                    if (this.dgvChosenOccupationDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvChosenOccupationDataGridView, 0);
                    }

                    this.Set_DataGridView_SelectedRowIndex(this.dgvOccupationDataGridView, this.dgvOccupationDataGridView.Rows.Count - 1);
                }
            }
        }

        private void btnRemoveAllOccupation_Click(object sender, EventArgs e)
        {
        btnRemoveAllOccupation_Click_Continue:

            if (this.dgvChosenOccupationDataGridView.Rows.Count > 0)
            {
                this.btnRemoveOccupation_Click(null, null);

                goto btnRemoveAllOccupation_Click_Continue;
            }

        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnAdd_Click(sender, e);
        }

        private void dgvChosenEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnRemove_Click(sender, e);
        }

        private void dgvDepartmentDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnAddDepartment_Click(sender, e);
        }

        private void dgvChosenDepartmentDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnRemoveDepartment_Click(sender, e);
        }

        private void dgvOccupationDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnAddOccupation_Click(sender, e);
        }

        private void dgvChosenOccupationDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnRemoveOccupation_Click(sender, e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Set_Form_For_Edit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Set_Form_For_Read();

            Load_CurrentForm_Records();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlgResult = MessageBox.Show("Are you Sure you want to Delete these Records?'",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    DataSet DataSet = new System.Data.DataSet();

                    int intTableRow = -1;

                    string strEmployeeWageNoIn = "";
                    string strEmployeeSalaryNoIn = "";

                    for (int intRow = 0; intRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intRow++)
                    {
                        intTableRow = Convert.ToInt32(this.dgvChosenEmployeeDataGridView[4, intRow].Value);

                        if (pvtDataSet.Tables["Employee"].Rows[intTableRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                        {
                            if (strEmployeeWageNoIn == "")
                            {
                                strEmployeeWageNoIn = "(" + pvtDataSet.Tables["Employee"].Rows[intTableRow]["EMPLOYEE_NO"].ToString();
                            }
                            else
                            {
                                strEmployeeWageNoIn += "," + pvtDataSet.Tables["Employee"].Rows[intTableRow]["EMPLOYEE_NO"].ToString();
                            }
                        }
                        else
                        {
                            if (strEmployeeSalaryNoIn == "")
                            {
                                strEmployeeSalaryNoIn = "(" + pvtDataSet.Tables["Employee"].Rows[intTableRow]["EMPLOYEE_NO"].ToString();
                            }
                            else
                            {
                                strEmployeeSalaryNoIn += "," + pvtDataSet.Tables["Employee"].Rows[intTableRow]["EMPLOYEE_NO"].ToString();
                            }
                        }
                    }

                    if (strEmployeeWageNoIn != "")
                    {
                        strEmployeeWageNoIn += ")";
                    }

                    if (strEmployeeSalaryNoIn != "")
                    {
                        strEmployeeSalaryNoIn += ")";
                    }
                   
                    string strPayCategoryWageNoIn = "";
                    string strPayCategorySalaryNoIn = "";

                    for (int intRow = 0; intRow < this.dgvChosenCostCentreDataGridView.Rows.Count; intRow++)
                    {
                        intTableRow = Convert.ToInt32(this.dgvChosenCostCentreDataGridView[2, intRow].Value);

                        if (pvtDataSet.Tables["PayCategory"].Rows[intTableRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                        {
                            if (strPayCategoryWageNoIn == "")
                            {
                                strPayCategoryWageNoIn = "(" + pvtDataSet.Tables["PayCategory"].Rows[intTableRow]["PAY_CATEGORY_NO"].ToString();
                            }
                            else
                            {
                                strPayCategoryWageNoIn += "," + pvtDataSet.Tables["PayCategory"].Rows[intTableRow]["PAY_CATEGORY_NO"].ToString();
                            }
                        }
                        else
                        {
                            if (strPayCategorySalaryNoIn == "")
                            {
                                strPayCategorySalaryNoIn = "(" + pvtDataSet.Tables["PayCategory"].Rows[intTableRow]["PAY_CATEGORY_NO"].ToString();
                            }
                            else
                            {
                                strPayCategorySalaryNoIn += "," + pvtDataSet.Tables["PayCategory"].Rows[intTableRow]["PAY_CATEGORY_NO"].ToString();
                            }
                        }
                    }

                    if (strPayCategoryWageNoIn != "")
                    {
                        strPayCategoryWageNoIn += ")";
                    }

                    if (strPayCategorySalaryNoIn != "")
                    {
                        strPayCategorySalaryNoIn += ")";
                    }

                    string strDepartmentNoIn = "";

                    for (int intRow = 0; intRow < this.dgvChosenDepartmentDataGridView.Rows.Count; intRow++)
                    {
                        intTableRow = Convert.ToInt32(this.dgvChosenDepartmentDataGridView[1, intRow].Value);

                         if (strDepartmentNoIn == "")
                        {
                            strDepartmentNoIn = "(" + pvtDataSet.Tables["Department"].Rows[intTableRow]["DEPARTMENT_NO"].ToString();
                        }
                        else
                        {
                            strDepartmentNoIn += "," + pvtDataSet.Tables["Department"].Rows[intTableRow]["DEPARTMENT_NO"].ToString();
                        }
                    }

                    if (strDepartmentNoIn != "")
                    {
                        strDepartmentNoIn += ")";
                    }

                    string strOccupationNoIn = "";

                    for (int intRow = 0; intRow < this.dgvChosenOccupationDataGridView.Rows.Count; intRow++)
                    {
                        intTableRow = Convert.ToInt32(this.dgvChosenOccupationDataGridView[1, intRow].Value);

                        if (strOccupationNoIn == "")
                        {
                            strOccupationNoIn = "(" + pvtDataSet.Tables["Occupation"].Rows[intTableRow]["OCCUPATION_NO"].ToString();
                        }
                        else
                        {
                            strOccupationNoIn += "," + pvtDataSet.Tables["Occupation"].Rows[intTableRow]["OCCUPATION_NO"].ToString();
                        }
                    }

                    if (strOccupationNoIn != "")
                    {
                        strOccupationNoIn += ")";
                    }

                    string strCostCentreEmployeeNoIN = "";

                    for (int intRow = 0; intRow < this.dgvEmployeeSelectedDataGridView.Rows.Count; intRow++)
                    {
                        intTableRow = Convert.ToInt32(this.dgvEmployeeSelectedDataGridView[3, intRow].Value);

                        if (strCostCentreEmployeeNoIN == "")
                        {
                            strCostCentreEmployeeNoIN = "(" + pvtDataSet.Tables["Employee"].Rows[intTableRow]["EMPLOYEE_NO"].ToString();
                        }
                        else
                        {
                            strCostCentreEmployeeNoIN += "," + pvtDataSet.Tables["Employee"].Rows[intTableRow]["EMPLOYEE_NO"].ToString();
                        }
                    }

                    if (strCostCentreEmployeeNoIN != "")
                    {
                        strCostCentreEmployeeNoIN += ")";
                    }

                    object[] objParm = new object[11];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = strEmployeeWageNoIn;
                    objParm[2] = strEmployeeSalaryNoIn;
                    objParm[3] = strPayCategoryWageNoIn;
                    objParm[4] = strPayCategorySalaryNoIn;
                    objParm[5] = strDepartmentNoIn;
                    objParm[6] = strOccupationNoIn;
                    objParm[7] = pvtstrPayCategoryNo;
                    objParm[8] = pvtstrPayrollType;
                    objParm[9] = strCostCentreEmployeeNoIN;
                    objParm[10] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Records", objParm);

                    pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                    MessageBox.Show("Update Successful.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                    Set_Form_For_Read();

                    Load_CurrentForm_Records();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update UNSUCCESSFUL - " + ex.Message,
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

            }
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    pvtstrPayrollType = this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                    pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"], "PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'", "", DataViewRowState.CurrentRows);

                    this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeListDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeSelectedDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeNotLinkedDataGridView);

                    pvtblnPayCategoryDataGridViewLoaded = false;

                    for (int intRow = 0; intRow < this.pvtPayCategoryDataView.Count; intRow++)
                    {
                        this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                 intRow.ToString());

                    }

                    pvtblnPayCategoryDataGridViewLoaded = true;

                    if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
                    }
                }
            }
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    int intDataViewIndex = Convert.ToInt32(dgvPayCategoryDataGridView[1, e.RowIndex].Value);

                    pvtstrPayCategoryNo = pvtPayCategoryDataView[intDataViewIndex]["PAY_CATEGORY_NO"].ToString();

                    this.Clear_DataGridView(this.dgvEmployeeListDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeSelectedDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeNotLinkedDataGridView);

                    for (int intRow = 0; intRow < pvtDataSet.Tables["Employee"].Rows.Count; intRow++)
                    {
                        if (pvtDataSet.Tables["Employee"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() != pvtstrPayrollType)
                        {
                            continue;
                        }

                        pvtEmployeePayCategoryDataView = null;
                        pvtEmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"], "EMPLOYEE_NO  = " + pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'", "", DataViewRowState.CurrentRows);

                        if (pvtEmployeePayCategoryDataView.Count == 0)
                        {
                            this.dgvEmployeeListDataGridView.Rows.Add(pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                              pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                              pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                              intRow.ToString());

                            this.dgvEmployeeListDataGridView.Rows[this.dgvEmployeeListDataGridView.Rows.Count - 1].HeaderCell.Style = this.EmployeeNotLinkedDataGridViewCellStyle;

                            this.dgvEmployeeNotLinkedDataGridView.Rows.Add(pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                           pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                           pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                            intRow.ToString());
                        }
                        else
                        {
                            if (pvtEmployeePayCategoryDataView[0]["PAY_CATEGORY_NO"].ToString() == pvtstrPayCategoryNo)
                            {
                                this.dgvEmployeeSelectedDataGridView.Rows.Add(pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                              pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                              pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                              intRow.ToString());


                            }
                            else
                            {
                                this.dgvEmployeeListDataGridView.Rows.Add(pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                              pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                              pvtDataSet.Tables["Employee"].Rows[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                              intRow.ToString());

                            }

                        }
                    }
                }
            }
        }

        private void dgvEmployeeListDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvEmployeeSelectedDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvEmployeeNotLinkedDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void btnAddLink_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvEmployeeListDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvEmployeeListDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeListDataGridView)];

                    this.dgvEmployeeListDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvEmployeeSelectedDataGridView.Rows.Add(myDataGridViewRow);

                    this.dgvEmployeeSelectedDataGridView.CurrentCell = this.dgvEmployeeSelectedDataGridView[0, this.dgvEmployeeSelectedDataGridView.Rows.Count - 1];
                }
            }
        }

        private void btnRemoveLink_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvEmployeeSelectedDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvEmployeeSelectedDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeSelectedDataGridView)];

                    this.dgvEmployeeSelectedDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvEmployeeListDataGridView.Rows.Add(myDataGridViewRow);

                    this.dgvEmployeeListDataGridView.Rows[this.dgvEmployeeListDataGridView.Rows.Count - 1].HeaderCell.Style = this.EmployeeNotLinkedDataGridViewCellStyle;

                    this.dgvEmployeeListDataGridView.CurrentCell = this.dgvEmployeeListDataGridView[0, this.dgvEmployeeListDataGridView.Rows.Count - 1];
                }
            }
        }

        private void btnAddAllLink_Click(object sender, EventArgs e)
        {
        btnAddAllLink_Click_Continue:

            if (this.dgvEmployeeListDataGridView.Rows.Count > 0)
            {
                btnAddLink_Click(null, null);

                goto btnAddAllLink_Click_Continue;
            }
        }

        private void btnRemoveAllLink_Click(object sender, EventArgs e)
        {
        btnRemoveAllLink_Click_Continue:

            if (this.dgvEmployeeSelectedDataGridView.Rows.Count > 0)
            {
                btnRemoveLink_Click(null, null);

                goto btnRemoveAllLink_Click_Continue;
            }

        }

        private void dgvEmployeeListDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnAddLink_Click(sender, e);
        }

        private void dgvEmployeeSelectedDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnRemoveLink_Click(sender, e);
        }
    }
}
