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
    public partial class frmRptLeaveTimeSheet : Form
    {
        clsISUtilities clsISUtilities;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;

        DataSet pvtDataSet;
        DataSet pvtTempDataSet;
        DataView pvtLeaveTypeDataView;
        DataView pvtEmployeeDataView;
        DataView pvtPayCategoryDataView;
        DataView pvtEmployeePayCategoryDataView;
        DataView pvtDateDataView;
        DataView pvtReportDataView;

        private int pvtintlblEmployeeTop = 0;
        private int pvtintdgvEmployeeDataGridViewTop = 0;
        private int pvtintdgvEmployeeDataGridViewHeight = 0;

        private int pvtintbtnEmployeeAddTop = 0;
        private int pvtintbtnEmployeeAddAllTop = 0;
        private int pvtintbtnEmployeeRemoveTop = 0;
        private int pvtintbtnEmployeeRemoveAllTop = 0;

        byte[] pvtbytCompress;

        private string pvtstrPayrollType = "";

        private int pvtintTabControlCurrentSelectedIndex = 0;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnLeaveTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        
        DateTime pvtdtWageDate = DateTime.Now.AddYears(-50);

        public frmRptLeaveTimeSheet()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.tabControlMain.Height += 114;
                this.tabControl.Height += 114;
                this.tabPage1.Height += 114;
                this.tabPage2.Height += 114;

                this.grbEmployee.Top += 114;
                this.grbCostCentre.Top += 114;
               
                this.dgvPayCategoryDataGridView.Height += 38;
                this.dgvChosenEmployeeDataGridView.Height += 114;

                this.lblPayPeriod.Top += 114;
                this.dgvPayPeriodDataGridView.Top += 114;
                
                this.lblEmployee.Top += 38;
                this.dgvEmployeeDataGridView.Top += 38;
                this.dgvEmployeeDataGridView.Height += 76;

                this.btnAdd.Top += 86;
                this.btnAddAll.Top += 86;
                this.btnRemove.Top += 86;
                this.btnRemoveAll.Top += 86;

                this.dgvCostCentreDataGridView.Height += 114;
                this.dgvChosenCostCentreDataGridView.Height += 114;
                
                this.reportViewer.Height += 114;
            }

            pvtintlblEmployeeTop = this.lblEmployee.Top;
            pvtintdgvEmployeeDataGridViewTop = this.dgvEmployeeDataGridView.Top;
            pvtintdgvEmployeeDataGridViewHeight = this.dgvEmployeeDataGridView.Height;

            pvtintbtnEmployeeAddTop = this.btnAdd.Top;
            pvtintbtnEmployeeAddAllTop = this.btnAddAll.Top;
            pvtintbtnEmployeeRemoveTop = this.btnRemove.Top;
            pvtintbtnEmployeeRemoveAllTop = this.btnRemoveAll.Top;
        }

        private void frmRptLeaveTimeSheet_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busRptLeaveTimeSheet");

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayCategory.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayPeriod.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
             
                this.pvtblnPayrollTypeDataGridViewLoaded = true;

                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("MenuId").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[3] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
            this.reportViewer.RefreshReport();
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

                    case "dgvPayCategoryDataGridView":

                        pvtintPayCategoryDataGridViewRowIndex = -1;
                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

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

        private void rbnAllCostCentre_Click(object sender, EventArgs e)
        {
            this.Clear_DataGridView(this.dgvCostCentreDataGridView);
            this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);

            this.btnCostCentreAdd.Enabled = false;
            this.btnCostCentreAddAll.Enabled = false;
            this.btnCostCentreRemove.Enabled = false;
            this.btnCostCentreRemoveAll.Enabled = false;

            this.tabControl.SelectedIndex = 1;
        }

        private void rbnAllEmployees_Click(object sender, EventArgs e)
        {
            if (rbnAllEmployees.Checked == true)
            {
                this.grbEmployeeSelection.Enabled = false;
                this.rbnByPayCategory.Checked = true;

                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);
                this.Clear_DataGridView(this.dgvPayCategoryDataGridView);

                this.btnAdd.Enabled = false;
                this.btnRemove.Enabled = false;
                this.btnAddAll.Enabled = false;
                this.btnRemoveAll.Enabled = false;

                tabControl.SelectedIndex = 0;
            }
        }

        private void btnCostCentreAdd_Click(object sender, System.EventArgs e)
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

        private void btnCostCentreRemove_Click(object sender, System.EventArgs e)
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

        private void Clear_DataGridView(DataGridView myDataGridView)
        {
            myDataGridView.Rows.Clear();

            if (myDataGridView.SortedColumn != null)
            {
                myDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
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
                        pvtstrPayrollType = "W";
                    }
                    else
                    {
                        pvtstrPayrollType = "S";
                    }

                    Load_CurrentForm_Records();
                }
            }
        }

        private void Load_CurrentForm_Records()
        {
            //All Employees
            this.rbnAllEmployees.Checked = true;
            this.rbnAllCostCentre.Checked = true;

            this.Clear_DataGridView(this.dgvCostCentreDataGridView);
            this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);

            this.btnCostCentreAdd.Enabled = false;
            this.btnCostCentreAddAll.Enabled = false;
            this.btnCostCentreRemove.Enabled = false;
            this.btnCostCentreRemoveAll.Enabled = false;

            EventArgs e = new EventArgs();

            rbnAllEmployees_Click(null, e);

            this.Clear_DataGridView(this.dgvPayPeriodDataGridView);

            string strFilter = "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
            strFilter += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

            pvtDateDataView = null;
            pvtDateDataView = new DataView(this.pvtDataSet.Tables["Date"],
                strFilter,
                "PAY_PERIOD_DATE DESC",
                DataViewRowState.CurrentRows);

            //First Build Pay Category ListBox
            for (int intRow = 0; intRow < pvtDateDataView.Count; intRow++)
            {
                this.dgvPayPeriodDataGridView.Rows.Add(Convert.ToDateTime(pvtDateDataView[intRow]["PAY_PERIOD_DATE"]).ToString("dd MMMM yyyy"),
                                                  Convert.ToDateTime(pvtDateDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd"));
            }

            if (this.dgvPayPeriodDataGridView.Rows.Count > 0)
            {
                this.btnOK.Enabled = true;
            }
            else
            {
                this.btnOK.Enabled = false;
            }

            this.tabControl.SelectedIndex = 0;
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

        private void rbnSelectedCostCentre_Click(object sender, EventArgs e)
        {
            this.Clear_DataGridView(this.dgvCostCentreDataGridView);
            this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);

            pvtPayCategoryDataView = null;
            pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
            "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
            "PAY_CATEGORY_DESC",
            DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
            {
                this.dgvCostCentreDataGridView.Rows.Add(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                         pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());
            }


            if (this.dgvCostCentreDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView, 0);
            }

            this.btnCostCentreAdd.Enabled = true;
            this.btnCostCentreAddAll.Enabled = true;
            this.btnCostCentreRemove.Enabled = true;
            this.btnCostCentreRemoveAll.Enabled = true;

            this.tabControl.SelectedIndex = 1;
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);

                    bool blnFound = false;
                    int intFindRow = -1;
                    int intPayCategoryNo = Convert.ToInt32(dgvPayCategoryDataGridView[1, e.RowIndex].Value);

                    pvtEmployeePayCategoryDataView = null;
                    pvtEmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_NO = " + intPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'",
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

                    pvtEmployeeDataView = null;
                    pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")),
                    "EMPLOYEE_CODE",
                    DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                    {
                        intFindRow = pvtEmployeePayCategoryDataView.Find(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                        if (intFindRow == -1)
                        {
                            continue;
                        }
                        else
                        {
                            blnFound = false;

                            //Exclude These Employees 
                            for (int intSelectedRow = 0; intSelectedRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intSelectedRow++)
                            {
                                if (this.dgvChosenEmployeeDataGridView[3, intSelectedRow].Value.ToString() == pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString())
                                {
                                    blnFound = true;
                                    break;
                                }
                            }
                        }

                        if (blnFound == true)
                        {
                            continue;
                        }


                        this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());
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
            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                int intFindRow = pvtEmployeePayCategoryDataView.Find(this.dgvChosenEmployeeDataGridView[3, this.Get_DataGridView_SelectedRowIndex(dgvChosenEmployeeDataGridView)].Value.ToString());

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

        private void rbnSelectedEmployees_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbnSelectedEmployees.Checked == true
                    & this.dgvPayPeriodDataGridView.Rows.Count > 0)
                {
                    this.grbEmployeeSelection.Enabled = true;

                    this.Clear_DataGridView(this.dgvPayCategoryDataGridView);

                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                    this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

                    pvtblnPayCategoryDataGridViewLoaded = false;

                    pvtPayCategoryDataView = null;
                    pvtPayCategoryDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                       "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                       "PAY_CATEGORY_DESC",
                       DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
                    {
                        this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                 pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                    }

                    pvtblnPayCategoryDataGridViewLoaded = true;

                    if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
                    }

                    this.btnAdd.Enabled = true;
                    this.btnAddAll.Enabled = true;
                    this.btnRemove.Enabled = true;
                    this.btnRemoveAll.Enabled = true;

                    this.tabControl.SelectedIndex = 0;
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnAdd_Click(sender, e);
        }

        private void dgvChosenEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnRemove_Click(sender, e);
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

        private void dgvCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbnSelectedEmployees.Checked == true
                      & this.dgvChosenEmployeeDataGridView.Rows.Count == 0)
                {
                    this.tabControl.SelectedIndex = 0;

                    CustomMessageBox.Show("Select Employees.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    this.btnAdd.Focus();

                    return;
                }

                if (this.rbnSelectedCostCentre.Checked == true
                      & this.dgvChosenCostCentreDataGridView.Rows.Count == 0)
                {
                    this.tabControl.SelectedIndex = 1;

                    CustomMessageBox.Show("Select Cost Centres.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    this.btnAdd.Focus();

                    return;
                }

                string strEmployeeIn = "";
                string strPayCategoryNoIn = "";
                string strFromDate = "";
                
                strFromDate = this.dgvPayPeriodDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvPayPeriodDataGridView)].Value.ToString();
                
                if (rbnSelectedEmployees.Checked == true)
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

                if (this.rbnSelectedCostCentre.Checked == true)
                {
                    for (int intRow = 0; intRow < this.dgvChosenCostCentreDataGridView.Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            strPayCategoryNoIn = "(" + this.dgvChosenCostCentreDataGridView[1, intRow].Value.ToString();
                        }
                        else
                        {
                            strPayCategoryNoIn += "," + this.dgvChosenCostCentreDataGridView[1, intRow].Value.ToString();
                        }
                    }

                    strPayCategoryNoIn += ")";
                }

                if (pvtDataSet.Tables["Report"] != null)
                {
                    pvtDataSet.Tables.Remove("Report");
                }

                this.tabControlMain.SelectedIndex = 1;
                this.reportViewer.Clear();
                
                object[] objParm = new object[6];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = strEmployeeIn;
                objParm[2] = strPayCategoryNoIn;
                objParm[3] = strFromDate;
                objParm[4] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[5] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Report", objParm);

                pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                if (pvtTempDataSet.Tables["Report"].Rows.Count > 0)
                {
                    pvtTempDataSet.Tables["Report"].Columns.Add("COMPANY_DESC", typeof(String));
                    pvtTempDataSet.Tables["Report"].Columns.Add("REPORT_HEADER_DESC", typeof(String));
                    pvtTempDataSet.Tables["Report"].Columns.Add("REPORT_DATETIME", typeof(String));

                    pvtTempDataSet.Tables["Report"].Rows[0]["COMPANY_DESC"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["COMPANY_DESC"].ToString();
                    pvtTempDataSet.Tables["Report"].Rows[0]["REPORT_DATETIME"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"].ToString();

                    pvtTempDataSet.Tables["Report"].Rows[0]["REPORT_HEADER_DESC"] = "Pay Period Date - " + this.dgvPayPeriodDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvPayPeriodDataGridView)].Value.ToString();

                    pvtTempDataSet.Tables["Report"].Columns.Add("PAY_CATEGORY_DESC", typeof(String));
                    pvtTempDataSet.Tables["Report"].Columns.Add("EMPLOYEE_CODE", typeof(String));
                    pvtTempDataSet.Tables["Report"].Columns.Add("EMPLOYEE_SURNAME", typeof(String));
                    pvtTempDataSet.Tables["Report"].Columns.Add("EMPLOYEE_NAME", typeof(String));

                    pvtTempDataSet.Tables["Report"].Columns.Add("LEAVE_DAYS_DECIMAL", typeof(System.Double));
                    pvtTempDataSet.Tables["Report"].Columns.Add("NORMAL_LEAVE_ACCUM_DAYS", typeof(System.Double));
                    pvtTempDataSet.Tables["Report"].Columns.Add("SICK_LEAVE_ACCUM_DAYS", typeof(System.Double));
               
                    pvtEmployeeDataView = null;
                    pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) ,
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

                    pvtPayCategoryDataView = null;
                    pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                                                          "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                                                          "PAY_CATEGORY_NO",
                                                          DataViewRowState.CurrentRows);
                    int intFindRow = -1;

                    for (int intRow = 0; intRow < pvtTempDataSet.Tables["Report"].Rows.Count; intRow++)
                    {
                        intFindRow = pvtEmployeeDataView.Find(pvtTempDataSet.Tables["Report"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                        pvtTempDataSet.Tables["Report"].Rows[intRow]["EMPLOYEE_CODE"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_CODE"].ToString();
                        pvtTempDataSet.Tables["Report"].Rows[intRow]["EMPLOYEE_SURNAME"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_SURNAME"].ToString();
                        pvtTempDataSet.Tables["Report"].Rows[intRow]["EMPLOYEE_NAME"] = pvtEmployeeDataView[intFindRow]["EMPLOYEE_NAME"].ToString();

                        intFindRow = pvtPayCategoryDataView.Find(pvtTempDataSet.Tables["Report"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());

                        pvtTempDataSet.Tables["Report"].Rows[intRow]["PAY_CATEGORY_DESC"] = pvtPayCategoryDataView[intFindRow]["PAY_CATEGORY_DESC"].ToString();

                        pvtTempDataSet.Tables["Report"].Rows[intRow]["LEAVE_DAYS_DECIMAL"] = 0;
                        pvtTempDataSet.Tables["Report"].Rows[intRow]["NORMAL_LEAVE_ACCUM_DAYS"] = 0;
                        pvtTempDataSet.Tables["Report"].Rows[intRow]["SICK_LEAVE_ACCUM_DAYS"] = 0;
                    }

                    DateTime dtRunDateTime = DateTime.ParseExact(strFromDate, "yyyy-MM-dd", null);
                    DateTime dtBeginFinancialYear = DateTime.Now;
                    DateTime dtEndFinancialYear = DateTime.Now;

                    if (dtRunDateTime.Month >= Convert.ToInt32(pvtTempDataSet.Tables["ReportHeader"].Rows[0]["LEAVE_BEGIN_MONTH"]))
                    {
                        dtBeginFinancialYear = new DateTime(dtRunDateTime.Year, Convert.ToInt32(pvtTempDataSet.Tables["ReportHeader"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
                    }
                    else
                    {
                        dtBeginFinancialYear = new DateTime(dtRunDateTime.Year - 1, Convert.ToInt32(pvtTempDataSet.Tables["ReportHeader"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
                    }

                    //Last Day Of Fiscal Year
                    dtEndFinancialYear = dtBeginFinancialYear.AddYears(1).AddDays(-1);
                    
                    for (int intRow = 0; intRow < pvtTempDataSet.Tables["ReportLeave"].Rows.Count; intRow++)
                    {
                        pvtReportDataView = null;
                        pvtReportDataView = new DataView(pvtTempDataSet.Tables["Report"],
                        "EMPLOYEE_NO = " + pvtTempDataSet.Tables["ReportLeave"].Rows[intRow]["EMPLOYEE_NO"].ToString() + " AND LEAVE_TO_DATE = '" + Convert.ToDateTime(pvtTempDataSet.Tables["ReportLeave"].Rows[intRow]["LEAVE_TO_DATE"]).ToString("yyyyMMdd") + "'",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtReportDataView.Count > 0)
                        {
                            pvtReportDataView[0]["LEAVE_DAYS_DECIMAL"] = Convert.ToDouble(pvtTempDataSet.Tables["ReportLeave"].Rows[intRow]["LEAVE_DAYS_DECIMAL"]);

                            pvtReportDataView[0]["NORMAL_LEAVE_ACCUM_DAYS"] = Convert.ToDouble(pvtTempDataSet.Tables["ReportLeave"].Rows[intRow]["NORMAL_LEAVE_ACCUM_DAYS"]);
                            pvtReportDataView[0]["SICK_LEAVE_ACCUM_DAYS"] = Convert.ToDouble(pvtTempDataSet.Tables["ReportLeave"].Rows[intRow]["SICK_LEAVE_ACCUM_DAYS"]);
                        }
                    }

                    pvtTempDataSet.AcceptChanges();

                    Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", pvtTempDataSet.Tables["Report"]);

                    this.reportViewer.LocalReport.DataSources.Clear();
                    this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                    this.reportViewer.RefreshReport();
                    this.reportViewer.Focus();
                }
                else
                {
                    CustomMessageBox.Show("Empty DataSet.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void tabControlMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                //Cancel Button
                this.Close();
            }
        }

        private void frmRptLeaveTimeSheet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                //Cancel Button
                this.Close();
            }
        }

        private void reportViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                //Cancel Button
                this.Close();
            }
        }

        private void rbnByPayCategory_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnByPayCategory.Checked == true)
            {
                this.lblEmployee.Top = pvtintlblEmployeeTop;
                this.dgvEmployeeDataGridView.Top = pvtintdgvEmployeeDataGridViewTop;
                this.dgvEmployeeDataGridView.Height = pvtintdgvEmployeeDataGridViewHeight;

                this.lblEmployee.BringToFront();

                this.btnAdd.Top = pvtintbtnEmployeeAddTop;
                this.btnAddAll.Top = pvtintbtnEmployeeAddAllTop;
                this.btnRemove.Top = pvtintbtnEmployeeRemoveTop;
                this.btnRemoveAll.Top = pvtintbtnEmployeeRemoveAllTop;

                if (dgvPayCategoryDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
                }

                tabControl.SelectedIndex = 0;
            }
        }

        private void rbnEmployeeOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnEmployeeOnly.Checked == true)
            {
                this.lblEmployee.Top = this.lblCostCentre.Top;

                this.dgvEmployeeDataGridView.Top = this.dgvChosenEmployeeDataGridView.Top;
                this.dgvEmployeeDataGridView.Height = this.dgvChosenEmployeeDataGridView.Height;
                this.dgvEmployeeDataGridView.BringToFront();

                this.lblEmployee.BringToFront();

                this.btnAdd.Top = this.btnCostCentreAdd.Top;
                this.btnAddAll.Top = this.btnCostCentreAddAll.Top;
                this.btnRemove.Top = this.btnCostCentreRemove.Top;
                this.btnRemoveAll.Top = this.btnCostCentreRemoveAll.Top;

                bool blnFound = false;
                int intFindRow = -1;

                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                
                pvtEmployeePayCategoryDataView = null;
                pvtEmployeePayCategoryDataView = new DataView(pvtDataSet.Tables["EmployeePayCategory"],
                "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'",
                "EMPLOYEE_NO",
                DataViewRowState.CurrentRows);

                pvtEmployeeDataView = null;
                pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")),
                "EMPLOYEE_CODE",
                DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                {
                    intFindRow = pvtEmployeePayCategoryDataView.Find(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                    if (intFindRow == -1)
                    {
                        continue;
                    }
                    else
                    {
                        blnFound = false;

                        //Exclude These Employees 
                        for (int intSelectedRow = 0; intSelectedRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intSelectedRow++)
                        {
                            if (this.dgvChosenEmployeeDataGridView[3, intSelectedRow].Value.ToString() == pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString())
                            {
                                blnFound = true;
                                break;
                            }
                        }
                    }

                    if (blnFound == true)
                    {
                        continue;
                    }

                    this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                            pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                            pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                            pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                                            "-1");
                }

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
                }

                tabControl.SelectedIndex = 0;
            }
        }

        private void dgvPayPeriodDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            pvtdtWageDate = DateTime.ParseExact(this.dgvPayPeriodDataGridView[1, e.RowIndex].Value.ToString(), "yyyy-MM-dd", null);
            this.rbnAllEmployees.Checked = true;
            EventArgs ev = new EventArgs();
            rbnAllEmployees_Click(sender, ev);
        }
    }
}
