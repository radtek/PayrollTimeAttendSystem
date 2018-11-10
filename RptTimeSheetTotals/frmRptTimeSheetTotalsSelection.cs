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
    public partial class frmRptTimeSheetTotalsSelection : Form
    {
        clsISUtilities clsISUtilities;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        
        private DataView pvtEmployeeDataView;
        private DataView pvtPayPeriodDataView;
        private DataView pvtPayCategoryDataView;
        private DataView pvtEmployeePayCategoryDataView;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnCostCentreDataGridViewLoaded = false;
        private bool pvtblnRunDateDataGridViewLoaded = false;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;
        private int pvtintRunDateDataGridViewRowIndex = -1;

        private byte[] pvtbytCompress;

        private string pvtstrPayrollType = "";

        private int pvtintlblEmployeeTop = 0;
        private int pvtintdgvEmployeeDataGridViewTop = 0;
        private int pvtintdgvEmployeeDataGridViewHeight = 0;

        private int pvtintbtnEmployeeAddTop = 0;
        private int pvtintbtnEmployeeAddAllTop = 0;
        private int pvtintbtnEmployeeRemoveTop = 0;
        private int pvtintbtnEmployeeRemoveAllTop = 0;

        DateTime pvtdtWageDate = DateTime.Now.AddYears(-50);

        public frmRptTimeSheetTotalsSelection()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.tabMainControl.Height += 114;
                this.tabControl.Height += 114;
                this.tabPage1.Height += 114;
                this.tabPage2.Height += 114;

                this.grbEmployee.Top += 114;
                this.grbCostCentre.Top += 114;

                this.dgvPayCategoryDataGridView.Height += 38;
                this.dgvChosenEmployeeDataGridView.Height += 114;

                this.lblEmployee.Top += 38;
                this.dgvEmployeeDataGridView.Top += 38;
                this.dgvEmployeeDataGridView.Height += 76;

                this.btnEmployeeAdd.Top += 86;
                this.btnEmployeeAddAll.Top += 86;
                this.btnEmployeeRemove.Top += 86;
                this.btnEmployeeRemoveAll.Top += 86;

                this.dgvCostCentreDataGridView.Height += 114;
                this.dgvChosenCostCentreDataGridView.Height += 114;

                this.reportViewer.Height += 114;
            }

            pvtintlblEmployeeTop = this.lblEmployee.Top;
            pvtintdgvEmployeeDataGridViewTop = this.dgvEmployeeDataGridView.Top;
            pvtintdgvEmployeeDataGridViewHeight = this.dgvEmployeeDataGridView.Height;

            pvtintbtnEmployeeAddTop = this.btnEmployeeAdd.Top;
            pvtintbtnEmployeeAddAllTop = this.btnEmployeeAddAll.Top;
            pvtintbtnEmployeeRemoveTop = this.btnEmployeeRemove.Top;
            pvtintbtnEmployeeRemoveAllTop = this.btnEmployeeRemoveAll.Top;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmRptTimeSheetTotalsSelection_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busRptTimeSheetTotals");

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    lblPeriod.Text = "Time Attendance Run Date";
                }

                clsISUtilities.Set_Form_For_Edit(false);

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPeriod.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayCategory.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                pvtDataSet = new DataSet();

                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[3] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                DataView PayCategoryDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND PAY_CATEGORY_TYPE = 'W'",
                    "",
                    DataViewRowState.CurrentRows);

                if (PayCategoryDataView.Count > 0)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                }

                PayCategoryDataView = null;
                PayCategoryDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                   "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND PAY_CATEGORY_TYPE = 'S'",
                   "",
                   DataViewRowState.CurrentRows);

                if (PayCategoryDataView.Count > 0)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");
                }

                PayCategoryDataView = null;
                PayCategoryDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                   "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")).ToString() + " AND PAY_CATEGORY_TYPE = 'T'",
                   "",
                   DataViewRowState.CurrentRows);

                if (PayCategoryDataView.Count > 0)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add("Time Attendance");
                }

                pvtblnPayrollTypeDataGridViewLoaded = true;

                if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);
                }
                else
                {
                    this.btnOK.Enabled = false;
                }
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

                    case "dgvCostCentreDataGridView":

                        this.dgvCostCentreDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenCostCentreDataGridView":

                        this.dgvChosenCostCentreDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvRunDateDataGridView":

                        pvtintRunDateDataGridViewRowIndex = -1;
                        this.dgvRunDateDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void dgvRunDateDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnRunDateDataGridViewLoaded == true)
            {
                if (pvtintRunDateDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintRunDateDataGridViewRowIndex = e.RowIndex;

                    pvtdtWageDate = DateTime.ParseExact(this.dgvRunDateDataGridView[1, e.RowIndex].Value.ToString(), "yyyy-MM-dd", null);

                    this.rbnCostCentreAll.Checked = true;
                    Clear_CostCentre();

                    if (this.rbnEmployeeAll.Checked == true)
                    {
                        rbnEmployeeAll_CheckedChanged(sender, e);
                    }
                    else
                    {
                        this.rbnEmployeeAll.Checked = true;
                    }
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

                    this.Clear_DataGridView(this.dgvRunDateDataGridView);
                    pvtblnRunDateDataGridViewLoaded = false;

                    pvtstrPayrollType = dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                    string strFilter = "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

                    pvtPayPeriodDataView = null;
                    pvtPayPeriodDataView = new DataView(this.pvtDataSet.Tables["PayPeriodDate"],
                       strFilter,
                       "PAY_PERIOD_DATE DESC",
                       DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtPayPeriodDataView.Count; intRow++)
                    {
                        this.dgvRunDateDataGridView.Rows.Add(Convert.ToDateTime(pvtPayPeriodDataView[intRow]["PAY_PERIOD_DATE"]).ToString("dd MMMM yyyy"),
                                                          Convert.ToDateTime(pvtPayPeriodDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd"));
                    }

                    pvtEmployeeDataView = null;
                    pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                        strFilter,
                        "EMPLOYEE_CODE",
                        DataViewRowState.CurrentRows);

                    pvtPayCategoryDataView = null;
                    pvtPayCategoryDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                       strFilter,
                       "PAY_CATEGORY_DESC",
                       DataViewRowState.CurrentRows);

                    if (this.pvtDataSet != null)
                    {
                        this.rbnCostCentreAll.Checked = true;
                        Clear_CostCentre();

                        if (this.rbnEmployeeAll.Checked == true)
                        {
                            rbnEmployeeAll_CheckedChanged(sender, e);
                        }
                        else
                        {
                            this.rbnEmployeeAll.Checked = true;
                        }
                    }

                    pvtblnRunDateDataGridViewLoaded = true;

                    if (pvtPayPeriodDataView.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvRunDateDataGridView, 0);
                    }
                    else
                    {
                        this.btnOK.Enabled = false;
                    }
                }
            }
        }

        private void Clear_CostCentre()
        {
            this.Clear_DataGridView(this.dgvCostCentreDataGridView);
            this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);

            this.btnCostCentreAdd.Enabled = false;
            this.btnCostCentreAddAll.Enabled = false;
            this.btnCostCentreRemoveAll.Enabled = false;
            this.btnCostCentreRemove.Enabled = false;
        }

        private void btnEmployeeAdd_Click(object sender, System.EventArgs e)
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

        private void btnEmployeeRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvChosenEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView)];

                int intRowPayCategoryNo = Convert.ToInt32(myDataGridViewRow.Cells[4].Value);

                int intPayCategoryNo = Convert.ToInt32(dgvPayCategoryDataGridView[1, pvtintPayCategoryDataGridViewRowIndex].Value);

                this.dgvChosenEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                if (rbnByPayCategory.Checked == true)
                {
                    if (intRowPayCategoryNo == intPayCategoryNo)
                    {
                        this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);
                        this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1);
                    }
                }
                else
                {
                    this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);
                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, this.dgvEmployeeDataGridView.Rows.Count - 1);
                }

                if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView, 0);
                }
            }
        }

        private void btnEmployeeRemoveAll_Click(object sender, System.EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                btnEmployeeRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void btnEmployeeAddAll_Click(object sender, System.EventArgs e)
        {
        btnEmployeeAddAll_Click_Continue:

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                btnEmployeeAdd_Click(null, null);

                goto btnEmployeeAddAll_Click_Continue;
            }
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    int intPayCategoryNo = Convert.ToInt32(dgvPayCategoryDataGridView[1, e.RowIndex].Value);

                    string strFilter = "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    strFilter += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + intPayCategoryNo;
                    strFilter += " AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'";

                    pvtEmployeePayCategoryDataView = null;
                    pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategory"],
                        strFilter,
                        "EMPLOYEE_NO",
                        DataViewRowState.CurrentRows);

                    int intFindRow = -1;
                    bool blnFound = false;

                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);

                    //if (pvtEmployeePayCategoryDataView.Count == 0)
                    //{
                    //    object[] objParm = new object[5];
                    //    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    //    objParm[1] = pvtstrPayrollType;
                    //    objParm[2] = pvtdtWageDate;
                    //    objParm[3] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    //    objParm[4] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                    //    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Employees_For_CostCentre_For_Date", objParm);

                    //    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                    //    pvtDataSet.Merge(pvtTempDataSet);
                    //}

                    for (int intIndex = 0; intIndex < pvtEmployeeDataView.Count; intIndex++)
                    {
                        intFindRow = pvtEmployeePayCategoryDataView.Find(Convert.ToInt32(pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"]));

                        if (intFindRow == -1)
                        {
                            continue;
                        }
                        else
                        {
                            blnFound = false;

                            //Exclude These Employees 
                            for (int intRow = 0; intRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intRow++)
                            {
                                if (this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString() == pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString())
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

                        this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intIndex]["EMPLOYEE_CODE"].ToString(),
                                                              pvtEmployeeDataView[intIndex]["EMPLOYEE_SURNAME"].ToString(),
                                                              pvtEmployeeDataView[intIndex]["EMPLOYEE_NAME"].ToString(),
                                                              pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString(),
                                                              intPayCategoryNo.ToString());
                    }

                    if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
                    }
                }
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

        private void rbnEmployeeAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnEmployeeAll.Checked == true)
            {
                this.grbEmployeeSelection.Enabled = false;
                this.rbnByPayCategory.Checked = true;

                this.btnEmployeeAdd.Enabled = false;
                this.btnEmployeeAddAll.Enabled = false;
                this.btnEmployeeRemove.Enabled = false;
                this.btnEmployeeRemoveAll.Enabled = false;

                this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

                this.tabControl.SelectedIndex = 0;
            }
        }

        private void rbnEmployeeSelected_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rbnEmployeeSelected.Checked == true)
                {
                    pvtblnPayCategoryDataGridViewLoaded = false;

                    this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                    this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

                    this.btnEmployeeAdd.Enabled = true;
                    this.btnEmployeeAddAll.Enabled = true;
                    this.btnEmployeeRemoveAll.Enabled = true;
                    this.btnEmployeeRemove.Enabled = true;

                    this.grbEmployeeSelection.Enabled = true;
                    this.rbnByPayCategory.Checked = true;

                    for (int intIndex = 0; intIndex < pvtPayCategoryDataView.Count; intIndex++)
                    {
                        this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                                 pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());
                    }

                    pvtblnPayCategoryDataGridViewLoaded = true;

                    if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
                    }

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
            btnEmployeeAdd_Click(sender, e);
        }

        private void dgvChosenEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnEmployeeRemove_Click(sender, e);
        }

        private void btnCostCentreAdd_Click(object sender, EventArgs e)
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

        private void btnCostCentreRemove_Click(object sender, EventArgs e)
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

        private void btnCostCentreAddAll_Click(object sender, EventArgs e)
        {
        btnCostCentreAddAll_Click_Continue:

            if (this.dgvCostCentreDataGridView.Rows.Count > 0)
            {
                btnCostCentreAdd_Click(null, null);

                goto btnCostCentreAddAll_Click_Continue;
            }
        }

        private void btnCostCentreRemoveAll_Click(object sender, EventArgs e)
        {
        btnCostCentreRemoveAll_Click_Continue:

            for (int intRowCount = 0; intRowCount < this.dgvChosenCostCentreDataGridView.Rows.Count; intRowCount++)
            {
                btnCostCentreRemove_Click(sender, e);

                goto btnCostCentreRemoveAll_Click_Continue;
            }
        }

        private void dgvCostCentreDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnCostCentreAdd_Click(sender, e);
        }

        private void dgvChosenCostCentreDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnCostCentreRemove_Click(sender, e);
        }

        private void rbnCostCentreAll_Click(object sender, EventArgs e)
        {
            Clear_CostCentre();

            this.tabControl.SelectedIndex = 1;
        }

        private void rbnCostCentreSelected_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rbnCostCentreSelected.Checked == true)
                {
                    pvtblnCostCentreDataGridViewLoaded = false;

                    this.Clear_DataGridView(this.dgvCostCentreDataGridView);
                    this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);

                    this.btnCostCentreAdd.Enabled = true;
                    this.btnCostCentreAddAll.Enabled = true;
                    this.btnCostCentreRemoveAll.Enabled = true;
                    this.btnCostCentreRemove.Enabled = true;

                    //ELR 2014-05-10 (No Data)
                    if (pvtPayCategoryDataView != null)
                    {
                        for (int intIndex = 0; intIndex < pvtPayCategoryDataView.Count; intIndex++)
                        {
                            this.dgvCostCentreDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                                     pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());
                        }

                        pvtblnCostCentreDataGridViewLoaded = true;

                        if (this.dgvCostCentreDataGridView.Rows.Count > 0)
                        {
                            this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView, 0);
                        }
                    }

                    this.tabControl.SelectedIndex = 1;
                }

            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dtFromDate = DateTime.Now;
                DateTime dtToDate = DateTime.Now;
                string strReportType = "";
                string strEmployeeNoIN = "";
                string strPayCategoryNoIN = "";

                string strOrder = "E";
                string strGroupCostCentre = "N";
  
                if (this.rbnEmployeeSelected.Checked == true)
                {
                    if (this.dgvChosenEmployeeDataGridView.Rows.Count == 0)
                    {
                        this.tabControl.SelectedIndex = 0;

                        CustomMessageBox.Show("Select Employee/s.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        this.btnEmployeeAdd.Focus();

                        return;
                    }
                    else
                    {
                        for (int intRow = 0; intRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intRow++)
                        {
                            if (intRow == 0)
                            {
                                strEmployeeNoIN = "(" + this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString();
                            }
                            else
                            {
                                strEmployeeNoIN += "," + this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString();
                            }
                        }

                        strEmployeeNoIN += ") ";
                    }
                }

                if (this.rbnCostCentreSelected.Checked == true)
                {
                    if (this.dgvChosenCostCentreDataGridView.Rows.Count == 0)
                    {
                        this.tabControl.SelectedIndex = 1;

                        CustomMessageBox.Show("Select Cost Centre/s.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        this.btnEmployeeAdd.Focus();

                        return;
                    }
                    else
                    {
                        for (int intRow = 0; intRow < this.dgvChosenCostCentreDataGridView.Rows.Count; intRow++)
                        {
                            if (intRow == 0)
                            {
                                strPayCategoryNoIN = "(" + this.dgvChosenCostCentreDataGridView[1, intRow].Value.ToString();
                            }
                            else
                            {
                                strPayCategoryNoIN += "," + this.dgvChosenCostCentreDataGridView[1, intRow].Value.ToString();
                            }
                        }

                        strPayCategoryNoIN += ") ";
                    }
                }

                if (this.rbnPrintName.Checked == true)
                {
                    strOrder = "N";
                }
                else
                {
                    if (this.rbnPrintSurname.Checked == true)
                    {
                        strOrder = "S";
                    }
                }

                if (this.chkByPayParameter.Checked == true)
                {
                    strGroupCostCentre = "Y";
                }

                this.reportViewer.Clear();
                this.reportViewer.Refresh();
                this.tabMainControl.SelectedIndex = 1;

                object[] objParm = new object[11];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = pvtstrPayrollType;
                objParm[2] = strReportType;
                objParm[3] = strEmployeeNoIN;
                objParm[4] = strPayCategoryNoIN;
                objParm[5] = this.dgvRunDateDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvRunDateDataGridView)].Value.ToString();
                objParm[6] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                objParm[7] = strOrder;
                objParm[8] = strGroupCostCentre;
                objParm[9] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[10] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Report", objParm);

                pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                DataView EmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                    "",
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

                DataView PayCategoryDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                   "",
                   "PAY_CATEGORY_NO",
                   DataViewRowState.CurrentRows);

                if (pvtTempDataSet.Tables["Report"].Rows.Count == 0)
                {
                    CustomMessageBox.Show("Empty Result Set.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    this.tabPage2.Cursor = Cursors.WaitCursor;

                    pvtTempDataSet.Tables["Report"].Columns.Add("COMPANY_DESC", typeof(String));
                    pvtTempDataSet.Tables["Report"].Columns.Add("MAIN_REPORT_HEADER_DESC", typeof(String));
                    pvtTempDataSet.Tables["Report"].Columns.Add("REPORT_HEADER_DESC", typeof(String));
                    pvtTempDataSet.Tables["Report"].Columns.Add("REPORT_DATETIME", typeof(String));

                    pvtTempDataSet.Tables["Report"].Columns.Add("PAY_CATEGORY_DESC", typeof(String));
                    pvtTempDataSet.Tables["Report"].Columns.Add("EMPLOYEE_CODE", typeof(String));
                    pvtTempDataSet.Tables["Report"].Columns.Add("EMPLOYEE_SURNAME", typeof(String));
                    pvtTempDataSet.Tables["Report"].Columns.Add("EMPLOYEE_NAME", typeof(String));

                    int intFindRow = -1;

                    for (int intRow = 0; intRow < this.pvtTempDataSet.Tables["Report"].Rows.Count; intRow++)
                    {
                        intFindRow = EmployeeDataView.Find(pvtTempDataSet.Tables["Report"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                        pvtTempDataSet.Tables["Report"].Rows[intRow]["EMPLOYEE_CODE"] = EmployeeDataView[intFindRow]["EMPLOYEE_CODE"].ToString();
                        pvtTempDataSet.Tables["Report"].Rows[intRow]["EMPLOYEE_SURNAME"] = EmployeeDataView[intFindRow]["EMPLOYEE_SURNAME"].ToString();
                        pvtTempDataSet.Tables["Report"].Rows[intRow]["EMPLOYEE_NAME"] = EmployeeDataView[intFindRow]["EMPLOYEE_NAME"].ToString();

                        if (Convert.ToInt32(pvtTempDataSet.Tables["Report"].Rows[intRow]["PAY_CATEGORY_NO"]) != 0)
                        {
                            intFindRow = PayCategoryDataView.Find(this.pvtTempDataSet.Tables["Report"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                            pvtTempDataSet.Tables["Report"].Rows[intRow]["PAY_CATEGORY_DESC"] = PayCategoryDataView[intFindRow]["PAY_CATEGORY_DESC"].ToString();
                        }

                        if (intRow == 0)
                        {
                            pvtTempDataSet.Tables["Report"].Rows[0]["COMPANY_DESC"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["COMPANY_DESC"].ToString();

                            if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "P")
                            {
                                pvtTempDataSet.Tables["Report"].Rows[0]["MAIN_REPORT_HEADER_DESC"] = "Timesheet Totals";
                            }
                            else
                            {
                                pvtTempDataSet.Tables["Report"].Rows[0]["MAIN_REPORT_HEADER_DESC"] = "Timesheet Totals";
                            }

                            pvtTempDataSet.Tables["Report"].Rows[0]["REPORT_HEADER_DESC"] = "Pay Period Date - " + this.dgvRunDateDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvRunDateDataGridView)].Value.ToString();
                            pvtTempDataSet.Tables["Report"].Rows[0]["REPORT_DATETIME"] = pvtTempDataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"].ToString();

                            //pvtTempDataSet.Tables["Report"].Rows[intRow]["EMPLOYEE_CODE"] = "ZZZZZZZZ";
                            //pvtTempDataSet.Tables["Report"].Rows[intRow]["EMPLOYEE_SURNAME"] = "ZZZZZZZZZZZZZZZZZZZZ";
                            //pvtTempDataSet.Tables["Report"].Rows[intRow]["EMPLOYEE_NAME"] = "ZZZZZZZZZZZZZZZZZZZZ";
                        }
                    }

                    this.tabPage2.Cursor = Cursors.Default;

                    pvtTempDataSet.AcceptChanges();

                    Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", pvtTempDataSet.Tables["Report"]);

                    this.reportViewer.LocalReport.ReportEmbeddedResource = "RptTimeSheetTotals.Report.rdlc";
                   
                    this.reportViewer.LocalReport.DataSources.Clear();
                    this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                    this.tabMainControl.SelectedIndex = 1;

                    this.reportViewer.RefreshReport();
                    this.reportViewer.Focus();
                }
            }
            catch (Exception eException)
            {
                this.tabPage2.Cursor = Cursors.Default;
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void dgvRunDateDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (dgvRunDateDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString() == "")
            {
                e.SortResult = -1;
            }
            else
            {
                if (dgvRunDateDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString() == "")
                {
                    e.SortResult = 1;
                }
                else
                {
                    if (double.Parse(dgvRunDateDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString().Replace("-", "")) > double.Parse(dgvRunDateDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString().Replace("-", "")))
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvRunDateDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString().Replace("-", "")) < double.Parse(dgvRunDateDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString().Replace("-", "")))
                        {
                            e.SortResult = -1;
                        }
                        else
                        {
                            e.SortResult = 0;
                        }
                    }
                }
            }

            e.Handled = true;
        }

        private void frmRptTimeSheetTotalsSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                //Cancel Button
                this.Close();
            }
        }

        private void tabMainControl_KeyDown(object sender, KeyEventArgs e)
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

                this.btnEmployeeAdd.Top = pvtintbtnEmployeeAddTop;
                this.btnEmployeeAddAll.Top = pvtintbtnEmployeeAddAllTop;
                this.btnEmployeeRemove.Top = pvtintbtnEmployeeRemoveTop;
                this.btnEmployeeRemoveAll.Top = pvtintbtnEmployeeRemoveAllTop;

                if (dgvPayCategoryDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
                }

                this.tabControl.SelectedIndex = 0;
            }
        }

        private void rbnEmployeeOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnEmployeeOnly.Checked == true)
            {
                this.lblEmployee.Top = lblSelectedEmployee.Top;

                this.dgvEmployeeDataGridView.Top = this.dgvChosenEmployeeDataGridView.Top;
                this.dgvEmployeeDataGridView.Height = this.dgvChosenEmployeeDataGridView.Height;
                this.dgvEmployeeDataGridView.BringToFront();

                this.lblEmployee.BringToFront();

                this.btnEmployeeAdd.Top = this.btnCostCentreAdd.Top;
                this.btnEmployeeAddAll.Top = this.btnCostCentreAddAll.Top;
                this.btnEmployeeRemove.Top = this.btnCostCentreRemove.Top;
                this.btnEmployeeRemoveAll.Top = this.btnCostCentreRemoveAll.Top;

                bool blnFound = false;

                this.Clear_DataGridView(this.dgvEmployeeDataGridView);

                //2017-08-09 - From Here
                string strFilter = "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                strFilter += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";
                strFilter += " AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'";
                
                pvtEmployeePayCategoryDataView = null;
                pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategory"],
                    strFilter,
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                               
                //Creates Empty DataView 
                string strEmployeeFilter = " AND EMPLOYEE_NO IN (-1";

                for (int intRow = 0; intRow < this.pvtEmployeePayCategoryDataView.Count; intRow++)
                {
                    if (intRow == 0)
                    {
                        strEmployeeFilter = " AND EMPLOYEE_NO IN (" + pvtEmployeePayCategoryDataView[intRow]["EMPLOYEE_NO"].ToString();
                    }
                    else
                    {
                        strEmployeeFilter += "," + pvtEmployeePayCategoryDataView[intRow]["EMPLOYEE_NO"].ToString();
                    }
                }

                strEmployeeFilter += ")";

                //Exclude These Employees 
                for (int intRow = 0; intRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intRow++)
                {
                    if (intRow == 0)
                    {
                        strEmployeeFilter += " AND NOT EMPLOYEE_NO IN (" + this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString();
                    }
                    else
                    {
                        strEmployeeFilter += "," + this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString();
                    }
                }

                if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
                {
                    strEmployeeFilter += ")";
                }

                pvtEmployeeDataView = null;
                pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + strEmployeeFilter,
                    "EMPLOYEE_CODE",
                    DataViewRowState.CurrentRows);
                //2017-08-09 - To Here
                
                for (int intIndex = 0; intIndex < pvtEmployeeDataView.Count; intIndex++)
                {
                    blnFound = false;

                    //Exclude These Employees 
                    for (int intRow = 0; intRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intRow++)
                    {
                        if (this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString() == pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString())
                        {
                            blnFound = true;
                            break;
                        }
                    }

                    if (blnFound == true)
                    {
                        continue;
                    }

                    this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intIndex]["EMPLOYEE_CODE"].ToString(),
                                                            pvtEmployeeDataView[intIndex]["EMPLOYEE_SURNAME"].ToString(),
                                                            pvtEmployeeDataView[intIndex]["EMPLOYEE_NAME"].ToString(),
                                                            pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString(),
                                                            "-1");
                }

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
                }

                this.tabControl.SelectedIndex = 0;
            }
        }
    }
}
