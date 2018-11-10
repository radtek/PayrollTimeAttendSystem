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
    public partial class frmRptPaySlipSelection : Form
    {
        clsISUtilities clsISUtilities;

        bool pvtblnLoadedEmailButton = false;

        private byte[] pvtbytCompress;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintDateDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;

        private DataView pvtDateDataView;
        private DataView pvtEmployeeDataView;
        private DataView pvtEmployeePayCategoryDataView;
        private DataView pvtPayCategoryDataView;
        private DataView pvtPayCategoryDateDataView;
        private DataView pvtCostCentreDateDataView;

        DataGridViewCellStyle ErrorDataGridViewCellStyle;

        private string pvtstrPayrollType = "";

        private string pvtstrFilter = "";

        DateTime pvtdtWageDate = DateTime.Now.AddYears(-50);

        private int pvtintCodeColWidth = 0;
        private int pvtintCodeColOtherWidth = 0;
        
        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnDateDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;

        private int pvtintlblEmployeeTop = 0;
        private int pvtintdgvEmployeeDataGridViewTop = 0;
        private int pvtintdgvEmployeeDataGridViewHeight = 0;

        private int pvtintbtnEmployeeAddTop = 0;
        private int pvtintbtnEmployeeAddAllTop = 0;
        private int pvtintbtnEmployeeRemoveTop = 0;
        private int pvtintbtnEmployeeRemoveAllTop = 0;

        public frmRptPaySlipSelection()
        {
            InitializeComponent();
            
            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900)
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

        private void frmRptPaySlipSelection_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busRptPaySlip");

                ErrorDataGridViewCellStyle = new DataGridViewCellStyle();
                ErrorDataGridViewCellStyle.BackColor = Color.Red;
                ErrorDataGridViewCellStyle.SelectionBackColor = Color.Red;

                pvtintCodeColWidth = this.dgvEmployeeDataGridView.Columns[0].Width;
                pvtintCodeColOtherWidth = this.dgvEmployeeDataGridView.Columns[1].Width + this.dgvEmployeeDataGridView.Columns[2].Width;

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblRunDate.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayCategory.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");

                pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);

                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
            this.reportViewer.RefreshReport();
        }

        void Email_Click(object sender, EventArgs e)
        {
            try
            {


                if (this.reportViewer.LocalReport.DataSources.Count > 0)
                {
                    if (this.pvtTempDataSet.Tables["EmployeePayslip"] != null)
                    {
                        if (this.pvtTempDataSet.Tables["EmployeePayslip"].Rows.Count > 0)
                        {
                            frmEmployeePayslip frmEmployeePayslip = new frmEmployeePayslip();

                            frmEmployeePayslip.Icon = this.Icon;

                            frmEmployeePayslip.lblEmployeePayslip.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                            frmEmployeePayslip.lblEmployeeNoPayslip.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                            for (int intRow = 0; intRow < this.pvtTempDataSet.Tables["EmployeePayslip"].Rows.Count; intRow++)
                            {
                                frmEmployeePayslip.dgvEmployeePayslipDataGridView.Rows.Add("",
                                                                                           this.pvtTempDataSet.Tables["EmployeePayslip"].Rows[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                                           this.pvtTempDataSet.Tables["EmployeePayslip"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                                           this.pvtTempDataSet.Tables["EmployeePayslip"].Rows[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                                           this.pvtTempDataSet.Tables["EmployeePayslip"].Rows[intRow]["EMPLOYEE_EMAIL"].ToString());

                                if (this.pvtTempDataSet.Tables["EmployeePayslip"].Rows[intRow]["EMPLOYEE_EMAIL"].ToString() == "")
                                {
                                    frmEmployeePayslip.dgvEmployeePayslipDataGridView[0, frmEmployeePayslip.dgvEmployeePayslipDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;
                                }
                            }

                            for (int intRow = 0; intRow < this.pvtTempDataSet.Tables["EmployeeNoPayslip"].Rows.Count; intRow++)
                            {
                                frmEmployeePayslip.dgvEmployeeNoPayslipDataGridView.Rows.Add("",
                                                                                           this.pvtTempDataSet.Tables["EmployeeNoPayslip"].Rows[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                                           this.pvtTempDataSet.Tables["EmployeeNoPayslip"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                                           this.pvtTempDataSet.Tables["EmployeeNoPayslip"].Rows[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                                           this.pvtTempDataSet.Tables["EmployeeNoPayslip"].Rows[intRow]["EMPLOYEE_EMAIL"].ToString());
                            }

                            AppDomain.CurrentDomain.SetData("EmailPayslips", "N");

                            frmEmployeePayslip.ShowDialog();

                            if (AppDomain.CurrentDomain.GetData("EmailPayslips").ToString() == "Y")
                            {
                                object[] objParm = new object[3];
                                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                                
                                int intReturnCode = (int)clsISUtilities.DynamicFunction("Email_Payslips", objParm);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No Employees Setup to be Emailed.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
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

                    case "dgvDateDataGridView":

                        pvtintDateDataGridViewRowIndex = -1;
                        this.dgvDateDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void rbnAllEmployees_Click(object sender, System.EventArgs e)
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

        private void rbnSelectedEmployees_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (rbnSelectedEmployees.Checked == true
                    & this.dgvDateDataGridView.Rows.Count > 0)
                {
                    this.grbEmployeeSelection.Enabled = true;

                    this.Clear_DataGridView(this.dgvPayCategoryDataGridView);

                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                    this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

                    pvtblnPayCategoryDataGridViewLoaded = false;

                    pvtPayCategoryDataView = null;
                    pvtPayCategoryDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                       pvtstrFilter,
                       "PAY_CATEGORY_DESC",
                       DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
                    {
                        this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                 intRow.ToString());
                    }

                    pvtblnPayCategoryDataGridViewLoaded = true;

                    if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
                    }

                    tabControl.SelectedIndex = 0;
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
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
                    this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView,0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView,this.dgvChosenEmployeeDataGridView.Rows.Count - 1);
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                int intFindRow = -1;

                if (this.rbnSelectedEmployees.Checked == true)
                {
                    intFindRow = pvtEmployeePayCategoryDataView.Find(this.dgvChosenEmployeeDataGridView[3, this.Get_DataGridView_SelectedRowIndex(dgvChosenEmployeeDataGridView)].Value.ToString());
                }

                DataGridViewRow myDataGridViewRow = this.dgvChosenEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView)];

                this.dgvChosenEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                if (this.rbnSelectedEmployees.Checked == true)
                {
                    if (intFindRow > -1)
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

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (this.rbnSelectedEmployees.Checked == true)
                {
                    if (this.dgvChosenEmployeeDataGridView.Rows.Count == 0)
                    {
                        MessageBox.Show("Select Employee/s.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        return;
                    }
                }

                if (this.rbnSelectedPayCategory.Checked == true)
                {
                    if (this.dgvChosenCostCentreDataGridView.Rows.Count == 0)
                    {
                        MessageBox.Show("Select Cost Centre/s.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        return;
                    }
                }

                string strEmployeeNos = "";

                for (int intRow = 0; intRow < this.dgvChosenEmployeeDataGridView.Rows.Count; intRow++)
                {
                    if (intRow == 0)
                    {
                        strEmployeeNos = this.dgvChosenEmployeeDataGridView[3,intRow].Value.ToString();
                    }
                    else
                    {
                        strEmployeeNos += "," + this.dgvChosenEmployeeDataGridView[3,intRow].Value.ToString();
                    }
                }

                string strPayCategoryNos = "";

                for (int intRow = 0; intRow < this.dgvChosenCostCentreDataGridView.Rows.Count; intRow++)
                {
                    if (intRow == 0)
                    {
                        strPayCategoryNos = this.dgvChosenCostCentreDataGridView[1, intRow].Value.ToString();
                    }
                    else
                    {
                        strPayCategoryNos += "," + this.dgvChosenCostCentreDataGridView[1, intRow].Value.ToString();
                    }
                }
             
                string strPrintOrder = "";

                if (this.rbnPrintCode.Checked == true)
                {
                    strPrintOrder = "C";
                }
                else
                {
                    if (this.rbnPrintSurname.Checked == true)
                    {
                        strPrintOrder = "S";
                    }
                    else
                    {
                        //Name
                        strPrintOrder = "N";
                    }
                }

                if (pvtblnLoadedEmailButton == false)
                {
                    ToolStrip toolStrip = (ToolStrip)reportViewer.Controls.Find("toolStrip1", true)[0];

                    for (int intCount = 0; intCount < toolStrip.Items.Count; intCount++)
                    {
                        ToolStripItem tsiToolStripItem = toolStrip.Items[intCount];

                        if (tsiToolStripItem.Name == "export")
                        {
                            toolStrip.Items[intCount].Visible = false;
                        }
                    }

                    ToolStripMenuItem emailToolStripMenuItem = new ToolStripMenuItem("Email");

                    emailToolStripMenuItem.ToolTipText = "Email Payslips";

                    emailToolStripMenuItem.Click += Email_Click;
                    toolStrip.Items.Add(emailToolStripMenuItem);

                    pvtblnLoadedEmailButton = true;
                }

                this.reportViewer.Clear();
                this.reportViewer.Refresh();
                this.tabControlMain.SelectedIndex = 1;

                object[] objParm = new object[9];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[3] = pvtstrPayrollType;
                objParm[4] = pvtdtWageDate;
                objParm[5] = strEmployeeNos;
                objParm[6] = strPayCategoryNos;
                objParm[7] = strPrintOrder;
                objParm[8] = "C";

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_PaySlip", objParm);

                pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", pvtTempDataSet.Tables["Report"]);

                this.reportViewer.LocalReport.DataSources.Clear();
                this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                //Calculates Number of Pages in Report and Display in Viewer
                this.reportViewer.PageCountMode = Microsoft.Reporting.WinForms.PageCountMode.Actual;

                this.reportViewer.RefreshReport();
                this.reportViewer.Focus();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_CurrentForm_Records()
        {
            string strFilter = "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

            strFilter += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

            pvtDateDataView = null;
            pvtDateDataView = new DataView(this.pvtDataSet.Tables["Date"],
                strFilter,
                "",
                DataViewRowState.CurrentRows);

            this.Clear_DataGridView(this.dgvDateDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);
            this.Clear_DataGridView(this.dgvCostCentreDataGridView);

            this.pvtblnDateDataGridViewLoaded = false;

            for (int intRow = 0; intRow < this.pvtDateDataView.Count; intRow++)
            {
                this.dgvDateDataGridView.Rows.Add(Convert.ToDateTime(this.pvtDateDataView[intRow]["PAY_PERIOD_DATE"]).ToString("dd MMMM yyyy"),
                                                  Convert.ToDateTime(this.pvtDateDataView[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd"));
            }

            this.pvtblnDateDataGridViewLoaded = true;

            if (this.pvtDateDataView.Count > 0)
            {
                this.btnOK.Enabled = true;
                this.Set_DataGridView_SelectedRowIndex(this.dgvDateDataGridView, 0);
            }
            else
            {
                this.btnOK.Enabled = false;
            }
            
            rbnAllPayCategory_Click(null, null);
            rbnAllEmployees_Click(null, null);
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

                    pvtstrFilter = "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";

                    this.rbnAllEmployees.Checked = true;
                    this.rbnAllPayCategory.Checked = true;

                    if (pvtDataSet != null)
                    {
                        Load_CurrentForm_Records();
                    }
                }
            }
        }

        private void dgvDateDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnDateDataGridViewLoaded == true)
            {
                if (pvtintDateDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintDateDataGridViewRowIndex = e.RowIndex;
                    
                    pvtdtWageDate = DateTime.ParseExact(this.dgvDateDataGridView[1, e.RowIndex].Value.ToString(), "yyyy-MM-dd", null);
                    
                    this.rbnAllEmployees.Checked = true;

                    rbnAllEmployees_Click(null, null);
                }
            }
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnAdd_Click(sender, e);
        }

        private void dgvChosenEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnRemove_Click(sender, e);
        }

        private void dgvDateDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (double.Parse(dgvDateDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString().Replace("-", "")) > double.Parse(dgvDateDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString().Replace("-", "")))
            {
                e.SortResult = 1;
            }
            else
            {
                if (double.Parse(dgvDateDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString().Replace("-", "")) < double.Parse(dgvDateDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString().Replace("-", "")))
                {
                    e.SortResult = -1;
                }
                else
                {
                    e.SortResult = 0;
                }
            }
      
            e.Handled = true;
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    try
                    {
                        int intPayCategoryNo = Convert.ToInt32(pvtPayCategoryDataView[Convert.ToInt32(this.dgvPayCategoryDataGridView[1, e.RowIndex].Value)]["PAY_CATEGORY_NO"]);

                        pvtEmployeePayCategoryDataView = null;
                        pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategoryDate"],
                            pvtstrFilter + " AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'",
                            "EMPLOYEE_NO",
                            DataViewRowState.CurrentRows);

                        this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                        //this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

                        if (pvtEmployeePayCategoryDataView.Count == 0)
                        {
                            object[] objParm = new object[5];
                            objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                            objParm[1] = pvtstrPayrollType;
                            objParm[2] = pvtdtWageDate;
                            objParm[3] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                            objParm[4] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Employees_For_CostCentre_For_Date", objParm);

                            pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                            pvtDataSet.Merge(pvtTempDataSet);
                        }

                        pvtEmployeePayCategoryDataView.RowFilter += " AND PAY_CATEGORY_NO = " + intPayCategoryNo;

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

                        for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                        {
                            this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                  pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                  pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                  pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());
                        }

                        if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                        {
                            this.btnAdd.Enabled = true;
                            this.btnRemove.Enabled = true;
                            this.btnAddAll.Enabled = true;
                            this.btnRemoveAll.Enabled = true;

                            this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
                        }
                    }
                    catch (Exception eException)
                    {
                        clsISUtilities.ErrorHandler(eException);
                    }
                }
            }
        }

        private void rbnSelectedPayCategory_Click(object sender, EventArgs e)
        {
            try
            {
                pvtEmployeePayCategoryDataView = null;
                pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategoryDate"],
                    pvtstrFilter + " AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'",
                    "",
                    DataViewRowState.CurrentRows);

                if (pvtEmployeePayCategoryDataView.Count == 0)
                {
                    object[] objParm = new object[5];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtstrPayrollType;
                    objParm[2] = pvtdtWageDate;
                    objParm[3] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[4] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Employees_For_CostCentre_For_Date", objParm);

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                    pvtDataSet.Merge(pvtTempDataSet);
                }

                pvtPayCategoryDateDataView = null;
                pvtPayCategoryDateDataView = new DataView(this.pvtDataSet.Tables["PayCategoryDate"],
                    pvtstrFilter + " AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'",
                   "",
                   DataViewRowState.CurrentRows);

                //Creates Empty DataView 
                string strPayCategoryFilter = " AND PAY_CATEGORY_NO IN (-1";

                for (int intRow = 0; intRow < this.pvtPayCategoryDateDataView.Count; intRow++)
                {
                    if (intRow == 0)
                    {
                        strPayCategoryFilter = " AND PAY_CATEGORY_NO IN (" + pvtPayCategoryDateDataView[intRow]["PAY_CATEGORY_NO"].ToString();
                    }
                    else
                    {
                        strPayCategoryFilter += "," + pvtPayCategoryDateDataView[intRow]["PAY_CATEGORY_NO"].ToString();
                    }
                }

                strPayCategoryFilter += ")";

                pvtCostCentreDateDataView = null;
                pvtCostCentreDateDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                    pvtstrFilter + strPayCategoryFilter,
                    "PAY_CATEGORY_DESC",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtCostCentreDateDataView.Count; intRow++)
                {
                    this.dgvCostCentreDataGridView.Rows.Add(pvtCostCentreDateDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                            pvtCostCentreDateDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                }

                if (this.dgvCostCentreDataGridView.Rows.Count > 0)
                {
                    this.btnCostCentreAdd.Enabled = true;
                    this.btnCostCentreRemove.Enabled = true;
                    this.btnCostCentreAddAll.Enabled = true;
                    this.btnCostCentreRemoveAll.Enabled = true;

                    this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView, 0);

                    this.tabControl.SelectedIndex = 1;
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void dgvCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void rbnAllPayCategory_Click(object sender, EventArgs e)
        {
            this.Clear_DataGridView(this.dgvCostCentreDataGridView);
            this.Clear_DataGridView(this.dgvChosenCostCentreDataGridView);

            this.btnCostCentreAdd.Enabled = false;
            this.btnCostCentreRemove.Enabled = false;
            this.btnCostCentreAddAll.Enabled = false;
            this.btnCostCentreRemoveAll.Enabled = false;

            this.tabControl.SelectedIndex = 1;

        }

        private void dgvChosenCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmRptPaySlipSelection_KeyDown(object sender, KeyEventArgs e)
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

        private void tabControl_KeyDown(object sender, KeyEventArgs e)
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

                this.Clear_DataGridView(this.dgvEmployeeDataGridView);

                //2017-08-09 - From Here
                pvtEmployeePayCategoryDataView = null;
                pvtEmployeePayCategoryDataView = new DataView(this.pvtDataSet.Tables["EmployeePayCategoryDate"],
                    pvtstrFilter + " AND PAY_PERIOD_DATE = '" + pvtdtWageDate.ToString("yyyy-MM-dd") + "'",
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
           
                if (pvtEmployeePayCategoryDataView.Count == 0)
                {
                    object[] objParm = new object[5];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtstrPayrollType;
                    objParm[2] = pvtdtWageDate;
                    objParm[3] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[4] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Employees_For_CostCentre_For_Date", objParm);

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                    pvtDataSet.Merge(pvtTempDataSet);
                }
                
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

                tabControl.SelectedIndex = 0;
            }
        }
    }
}
