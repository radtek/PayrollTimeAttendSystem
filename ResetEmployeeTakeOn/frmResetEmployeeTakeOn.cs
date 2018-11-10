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
    public partial class frmResetEmployeeTakeOn : Form
    {
        clsISUtilities clsISUtilities;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;

        private byte[] pvtbytCompress;
        private DataSet pvtDataSet;
        private DataView pvtPayCategoryDataView;
        private DataView pvtEmployeePayCategoryDataView;
        private DataView pvtEmployeeDataView;

        private Int64 pvtint64CompanyNo = -1;
        private string pvtstrPayrollType = "";

        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnPayrollTypeDataGridViewLoaded = false;

        public frmResetEmployeeTakeOn()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 118;

                this.dgvEmployeeDataGridView.Height += 114;
                this.dgvChosenEmployeeDataGridView.Height += 114;

                this.btnAdd.Top += 114;
                this.btnAddAll.Top += 114;
                this.btnRemove.Top += 114;
                this.btnRemoveAll.Top += 114;
            }
        }

        private void frmResetEmployeeTakeOn_Load(object sender, EventArgs e)
        {
            try
            {
                this.pvtint64CompanyNo = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")); 

                clsISUtilities = new clsISUtilities(this, "busResetEmployeeTakeOn");

                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayCategory.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                object[] objParm = new object[1];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");
                this.dgvPayrollTypeDataGridView.Rows.Add("Time Attendance");

                this.pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);
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

                    case "dgvEmployeeDataGridView":

                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenEmployeeDataGridView":

                        this.dgvChosenEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        pvtintPayCategoryDataGridViewRowIndex = -1;
                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnAdd_Click(sender, e);
            }
        }

        private void dgvChosenEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnRemove_Click(sender, e);
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

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                    this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

                    pvtstrPayrollType = this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                    pvtPayCategoryDataView = null;
                    pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                        "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "",
                        DataViewRowState.CurrentRows);

                    this.pvtblnPayCategoryDataGridViewLoaded = false;

                    for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
                    {
                        this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                 intRow.ToString());

                    }

                    this.pvtblnPayCategoryDataGridViewLoaded = true;

                    if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                    {
                        this.btnUpdate.Enabled = true;

                        this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
                    }
                    else
                    {
                        this.btnUpdate.Enabled = false;
                    }
                }
            }
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);

                    int intPayCategoryNo = Convert.ToInt32(pvtPayCategoryDataView[Convert.ToInt32(this.dgvPayCategoryDataGridView[1, e.RowIndex].Value)]["PAY_CATEGORY_NO"]);

                    pvtEmployeePayCategoryDataView = null;
                    pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategory"],
                        "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + intPayCategoryNo.ToString(),
                        "EMPLOYEE_NO",
                        DataViewRowState.CurrentRows);

                    pvtEmployeeDataView = null;
                    pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                        "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "EMPLOYEE_NO",
                        DataViewRowState.CurrentRows);

                    int intFindRow = -1;
                    bool blnFound = false;

                    for (int intRow = 0; intRow < pvtEmployeePayCategoryDataView.Count; intRow++)
                    {
                        blnFound = false;
                        intFindRow = pvtEmployeeDataView.Find(pvtEmployeePayCategoryDataView[intRow]["EMPLOYEE_NO"].ToString());

                        if (intFindRow > -1)
                        {
                            for (int intRow1 = 0; intRow1 < dgvChosenEmployeeDataGridView.Rows.Count; intRow1++)
                            {
                                if (this.dgvChosenEmployeeDataGridView[3, intRow1].Value.ToString() == pvtEmployeePayCategoryDataView[intRow]["EMPLOYEE_NO"].ToString())
                                {
                                    blnFound = true;
                                    break;
                                }
                            }

                            if (blnFound == true)
                            {
                                continue;
                            }

                            this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intFindRow]["EMPLOYEE_CODE"].ToString(),
                                                                  pvtEmployeeDataView[intFindRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                  pvtEmployeeDataView[intFindRow]["EMPLOYEE_NAME"].ToString(),
                                                                  pvtEmployeeDataView[intFindRow]["EMPLOYEE_NO"].ToString());
                        }
                    }

                    if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
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

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                int intFindRow = -1;

                intFindRow = pvtEmployeePayCategoryDataView.Find(this.dgvChosenEmployeeDataGridView[3, this.Get_DataGridView_SelectedRowIndex(dgvChosenEmployeeDataGridView)].Value.ToString());
                
                DataGridViewRow myDataGridViewRow = this.dgvChosenEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView)];

                this.dgvChosenEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                if (intFindRow > -1)
                {
                    this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);
                }
                
                if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView, 0);
                }

                if (intFindRow > -1)
                {
                    if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1);
                    }
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

        private void btnRemoveAll_Click(object sender, System.EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Set_Form_For_Edit();
        }

        private void Set_Form_For_Edit()
        {
            this.Text += " - Update";
            
            this.dgvPayCategoryDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.picCostCentreLock.Visible = true;
            this.picPayrollTypeLock.Visible = true;

            this.btnUpdate.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;
        }

        private void Set_Form_For_Read()
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);

            this.dgvPayCategoryDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.picCostCentreLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;
           
            this.btnAdd.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Set_Form_For_Read();

            this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string strEmployeeIn = "";

                if (this.dgvChosenEmployeeDataGridView.Rows.Count == 0)
                {
                    CustomMessageBox.Show("Select Employees.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                }
                else
                {
                    for (int intRow = 0; intRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            strEmployeeIn = "(" + this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString();
                        }
                        else
                        {
                            strEmployeeIn += "," + this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString();
                        }
                    }

                    strEmployeeIn += ")";
                }

                object[]  objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = pvtstrPayrollType;

                int intReturnCode = (int)clsISUtilities.DynamicFunction("Backup_DataBase", objParm);

                if (intReturnCode == 0)
                {
                    objParm = new object[3];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtstrPayrollType;
                    objParm[2] = strEmployeeIn;

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Reset_Employee_TakeOn", objParm,true);

                    pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                }
                else
                {
                    CustomMessageBox.Show("Backup of Database Failed.\n\nAction Cancelled.",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }

                this.btnCancel_Click(sender, e);

                //this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView));
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
    }
}
