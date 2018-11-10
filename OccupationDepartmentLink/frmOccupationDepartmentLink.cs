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
    public partial class frmOccupationDepartmentLink : Form
    {
        clsISUtilities clsISUtilities;

        public DataSet pvtDataSet;
        public DataView pvtOccupationDepartmentDataView;
        public DataView pvtEmployeeDataView;
        public DataSet pvtTempDataSet;
       
        DataGridViewCellStyle EmployeeNotLinkedDataGridViewCellStyle;

        private int pvtintOccupationDepartmentNo = -1;
        
        private int pvtintOccupationDepartmentRowIndex = -1;
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;

        private string pvtstrMenuId = "";
        private string pvtstrTableName = "";

        private byte[] pvtbytCompress;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnOccupationDepartmentDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;
        
        public frmOccupationDepartmentLink()
        {
            InitializeComponent();
        }

        private void frmOccupationDepartment_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busOccupationDepartmentLink");
                
                this.lblDescription.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeNotLinked.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeLinked.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblListEmployees.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                EmployeeNotLinkedDataGridViewCellStyle = new DataGridViewCellStyle();
                EmployeeNotLinkedDataGridViewCellStyle.BackColor = Color.Yellow;
                EmployeeNotLinkedDataGridViewCellStyle.SelectionBackColor = Color.Yellow;

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add("Time Attendance");
                }
                else
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                    this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");
                }

                this.pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);

                //NB There are Comments to tell Programmer to Leave 3E for Department
                pvtstrMenuId = AppDomain.CurrentDomain.GetData("MenuId").ToString().Substring(0,2);

                if (pvtstrMenuId == "3E")
                {
                    pvtstrTableName = "DEPARTMENT";
                    this.lblEmployeeLinked.Text = "Employee/s Linked to Department";
                    this.lblEmployeeNotLinked.Text = "Employee/s NOT Linked to Any Department";

                    lblLegend.Text = "Employees Not linked to any Department";
                }
                else
                {
                    this.Name = "frmOccupationLink";
                    this.lblDescription.Text = "Occupation";
                    
                    pvtstrTableName = "OCCUPATION";

                    this.lblEmployeeLinked.Text = "Employee/s Linked to Occupation";
                    this.lblEmployeeNotLinked.Text = "Employee/s NOT Linked to Any Occupation";

                    lblLegend.Text = "Employees Not linked to any Occupation";
                }
               
                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = pvtstrMenuId;
                objParm[2] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                              
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

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

                        this.pvtintPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvOccupationDepartmentDataGridView":

                        this.pvtintOccupationDepartmentRowIndex = -1;
                        this.dgvOccupationDepartmentDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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
       
        public void Load_CurrentForm_Records()
        {
            this.btnUpdate.Enabled = false;

            this.Clear_DataGridView(this.dgvOccupationDepartmentDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeSelectedDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeNotLinkedDataGridView);

            int intOccupationDepartmentRow = 0;
           
            pvtOccupationDepartmentDataView = new DataView(pvtDataSet.Tables["OccupationDepartment"],
                "",
                pvtstrTableName + "_DESC",
                DataViewRowState.CurrentRows);

            pvtblnOccupationDepartmentDataGridViewLoaded = false;

            for (int intRow = 0; intRow < pvtOccupationDepartmentDataView.Count; intRow++)
            {
                this.dgvOccupationDepartmentDataGridView.Rows.Add(pvtOccupationDepartmentDataView[intRow][pvtstrTableName + "_DESC"].ToString(),
                                                                  intRow.ToString());
                
                if (Convert.ToInt32(pvtOccupationDepartmentDataView[intRow][pvtstrTableName + "_NO"]) == pvtintOccupationDepartmentNo)
                {
                    intOccupationDepartmentRow = intRow;
                }
            }

            pvtblnOccupationDepartmentDataGridViewLoaded = true;

            if (this.dgvOccupationDepartmentDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvOccupationDepartmentDataGridView, intOccupationDepartmentRow);
            }
        }

        public void btnClose_Click(object sender, EventArgs e)
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
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();
                DataTable DataTable = this.pvtDataSet.Tables["Employee"].Clone();

                pvtTempDataSet.Tables.Add(DataTable);

                pvtEmployeeDataView = null;

                pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                "PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0,1) + "'",
                    "EMPLOYEE_CODE",
                    DataViewRowState.ModifiedCurrent);

                for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                {
                    pvtTempDataSet.Tables["Employee"].ImportRow(pvtEmployeeDataView[intRow].Row);
                }

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = pvtstrMenuId;
                objParm[3] = pvtbytCompress;

                clsISUtilities.DynamicFunction("Update_Employee_Link", objParm,true);

                this.pvtDataSet.AcceptChanges();

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
       
        private void Set_Form_For_Edit()
        {
            if (this.Text.EndsWith(" - New") == true)
            {
                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                this.Clear_DataGridView(this.dgvEmployeeSelectedDataGridView);
                this.Clear_DataGridView(this.dgvEmployeeNotLinkedDataGridView);
            }
            else
            {
                this.btnAdd.Enabled = true;
                this.btnAddAll.Enabled = true;
                this.btnRemove.Enabled = true;
                this.btnRemoveAll.Enabled = true;
            }

            this.dgvOccupationDepartmentDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.picOccupationDepartmentLock.Visible = true;
            this.picPayrollTypeLock.Visible = true;

            this.btnUpdate.Enabled = false;
            
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
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

            this.dgvOccupationDepartmentDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.picOccupationDepartmentLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            if (this.pvtOccupationDepartmentDataView.Count > 0)
            {
                this.btnUpdate.Enabled = true;
            }
            else
            {
                this.btnUpdate.Enabled = false;
            }

            this.btnAdd.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;

            Load_CurrentForm_Records();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                "EMPLOYEE_NO = " + this.dgvEmployeeDataGridView[3,this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Value.ToString(),
                "",
                DataViewRowState.CurrentRows);

                pvtEmployeeDataView[0][pvtstrTableName + "_NO"] = pvtintOccupationDepartmentNo;

                DataGridViewRow myDataGridViewRow = this.dgvEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)];

                this.dgvEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeSelectedDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvEmployeeSelectedDataGridView.CurrentCell = this.dgvEmployeeSelectedDataGridView[0, this.dgvEmployeeSelectedDataGridView.Rows.Count - 1];

                Load_Unlinked_Employees();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvEmployeeSelectedDataGridView.Rows.Count > 0)
            {
                pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                  "EMPLOYEE_NO = " + this.dgvEmployeeSelectedDataGridView[3,this.Get_DataGridView_SelectedRowIndex(dgvEmployeeSelectedDataGridView)].Value.ToString(),
                  "",
                  DataViewRowState.CurrentRows);

                pvtEmployeeDataView[0][pvtstrTableName + "_NO"] = 0;

                DataGridViewRow myDataGridViewRow = this.dgvEmployeeSelectedDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeSelectedDataGridView)];

                this.dgvEmployeeSelectedDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvEmployeeDataGridView.Rows[this.dgvEmployeeDataGridView.Rows.Count - 1].HeaderCell.Style = this.EmployeeNotLinkedDataGridViewCellStyle;

                this.dgvEmployeeDataGridView.CurrentCell = this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1];

                Load_Unlinked_Employees();
            }
        }

        private void Load_Unlinked_Employees()
        {
            this.Clear_DataGridView(this.dgvEmployeeNotLinkedDataGridView);

            pvtEmployeeDataView = null;
            pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
            "PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0,1) + "' AND " + pvtstrTableName + "_NO = 0",
                "EMPLOYEE_CODE",
                DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
            {
                this.dgvEmployeeNotLinkedDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                          pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                          pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                          pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                this.dgvEmployeeNotLinkedDataGridView.Rows[this.dgvEmployeeNotLinkedDataGridView.Rows.Count - 1].HeaderCell.Style = this.EmployeeNotLinkedDataGridViewCellStyle;
            }
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
        btnAddAll_Click_Continue:

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                btnAdd_Click(null, null);

                goto btnAddAll_Click_Continue;
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.dgvEmployeeSelectedDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;

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

        private void dgvOccupationDepartmentDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnOccupationDepartmentDataGridViewLoaded == true)
            {
                if (pvtintOccupationDepartmentRowIndex != e.RowIndex)
                {
                    pvtintOccupationDepartmentRowIndex = Convert.ToInt32(this.dgvOccupationDepartmentDataGridView[1, e.RowIndex].Value);

                    pvtintOccupationDepartmentNo = Convert.ToInt32(pvtOccupationDepartmentDataView[pvtintOccupationDepartmentRowIndex][pvtstrTableName + "_NO"]);

                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeSelectedDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeNotLinkedDataGridView);

                    pvtblnEmployeeDataGridViewLoaded = false;

                    pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                    "PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "' AND " + pvtstrTableName + "_NO <> " + pvtintOccupationDepartmentNo,
                    "EMPLOYEE_CODE",
                    DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                    {
                        this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                        if (pvtEmployeeDataView[intRow][pvtstrTableName + "_NO"].ToString() == "0")
                        {
                            this.dgvEmployeeDataGridView.Rows[this.dgvEmployeeDataGridView.Rows.Count - 1].HeaderCell.Style = this.EmployeeNotLinkedDataGridViewCellStyle;
                        }
                    }

                    pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                    "PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "' AND " + pvtstrTableName + "_NO = " + pvtintOccupationDepartmentNo,
                    "EMPLOYEE_CODE",
                    DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                    {
                        this.dgvEmployeeSelectedDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());
                    }

                    if (this.dgvEmployeeDataGridView.Rows.Count > 0
                        | this.dgvEmployeeSelectedDataGridView.Rows.Count > 0)
                    {
                        this.btnUpdate.Enabled = true;
                    }
                    else
                    {
                        this.btnUpdate.Enabled = false;
                    }

                    Load_Unlinked_Employees();
                }
            }
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEmployeeDataGridView.Rows.Count > 0
                & pvtblnEmployeeDataGridViewLoaded == true)
            {
            }
        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    btnAdd_Click(sender, e);
                }
            }
        }

        private void dgvEmployeeSelectedDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (dgvEmployeeSelectedDataGridView.Rows.Count > 0)
                {
                    btnRemove_Click(sender, e);
                }
            }
        }
    }
}
