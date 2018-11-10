using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

namespace InteractPayroll
{
    public partial class frmRptEti : Form
    {
        clsISUtilities clsISUtilities;

        DataSet pvtDataSet;
        DataView pvtEtiEmployeeDataView;
        DataView pvtEmployeeDataView;

        bool pvtblnDateDataGridViewLoaded = false;

        int pvtintDateDataGridViewRowIndex = -1;

        public frmRptEti()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.tabControlMain.Height += 114;
                this.tabPage1.Height += 114;
                this.tabPage2.Height += 114;

                this.grbEmployee.Top += 114;
              
                this.dgvEmployeeDataGridView.Height += 114;
                this.dgvChosenEmployeeDataGridView.Height += 114;
                
                this.btnAdd.Top += 76;
                this.btnAddAll.Top += 76;
                this.btnRemove.Top += 76;
                this.btnRemoveAll.Top += 76;

                this.reportViewer.Height += 114;
            }
        }

        private void frmRptEti_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busRptEti");

                this.lblRunDate.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);


                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                byte[] pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                this.pvtblnDateDataGridViewLoaded = false;

                for (int intRow = 0; intRow < this.pvtDataSet.Tables["EtiRunDate"].Rows.Count; intRow++)
                {
                    this.dgvDateDataGridView.Rows.Add(Convert.ToDateTime(this.pvtDataSet.Tables["EtiRunDate"].Rows[intRow]["ETI_RUN_DATE"]).ToString("MMMM yyyy"),
                                                      Convert.ToDateTime(this.pvtDataSet.Tables["EtiRunDate"].Rows[intRow]["ETI_RUN_DATE"]).ToString("yyyy-MM-dd"));
                }

                this.pvtblnDateDataGridViewLoaded = true;

                if (this.dgvDateDataGridView.Rows.Count > 0)
                {
                    this.btnOK.Enabled = true;
                    this.Set_DataGridView_SelectedRowIndex(this.dgvDateDataGridView, 0);
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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

        public void Set_DataGridView_SelectedRowIndex(DataGridView myDataGridView, int intRow)
        {
            //Fires DataGridView RowEnter Function
            if (this.Get_DataGridView_SelectedRowIndex(myDataGridView) == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
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

        private void Clear_DataGridView(DataGridView myDataGridView)
        {
            myDataGridView.Rows.Clear();

            if (myDataGridView.SortedColumn != null)
            {
                myDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
        }

        private void dgvDateDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnDateDataGridViewLoaded == true)
            {
                if (pvtintDateDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintDateDataGridViewRowIndex = e.RowIndex;

                    if (rbnSelectedEmployees.Checked == true)
                    {
                        EventArgs ev = new EventArgs();
                        rbnSelectedEmployees_Click(sender, ev);
                    }
                }
            }
        }

        private void rbnAllEmployees_Click(object sender, EventArgs e)
        {
            this.btnAdd.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemoveAll.Enabled = false;

            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);
        }

        private void rbnSelectedEmployees_Click(object sender, EventArgs e)
        {
            if (pvtintDateDataGridViewRowIndex > -1)
            {
                this.btnAdd.Enabled = true;
                this.btnRemove.Enabled = true;
                this.btnAddAll.Enabled = true;
                this.btnRemoveAll.Enabled = true;

                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

                pvtEmployeeDataView = null;
                pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                "",
                "EMPLOYEE_NO",
                DataViewRowState.CurrentRows);


                pvtEtiEmployeeDataView = null;
                pvtEtiEmployeeDataView = new DataView(this.pvtDataSet.Tables["EtiEmployee"],
                    "ETI_RUN_DATE = '" + dgvDateDataGridView[1, pvtintDateDataGridViewRowIndex].Value + "'",
                    "",
                    DataViewRowState.CurrentRows);
             
                for (int intRow = 0; intRow < this.pvtEtiEmployeeDataView.Count; intRow++)
                {
                    int intFindRow = pvtEmployeeDataView.Find(pvtEtiEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                    this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intFindRow]["EMPLOYEE_CODE"].ToString(),
                                                          pvtEmployeeDataView[intFindRow]["EMPLOYEE_SURNAME"].ToString(),
                                                          pvtEmployeeDataView[intFindRow]["EMPLOYEE_NAME"].ToString(),
                                                          pvtEmployeeDataView[intFindRow]["EMPLOYEE_NO"].ToString());
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

        private void btnRemove_Click(object sender, EventArgs e)
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

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

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
        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnAdd_Click(sender, e);
        }

        private void dgvChosenEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnRemove_Click(sender, e);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.rbnSelectedEmployees.Checked == true)
                {
                    if (this.dgvChosenEmployeeDataGridView.Rows.Count == 0)
                    {
                        CustomMessageBox.Show("Select Employee/s.",
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
                        strEmployeeNos = this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString();
                    }
                    else
                    {
                        strEmployeeNos += "," + this.dgvChosenEmployeeDataGridView[3, intRow].Value.ToString();
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

                //this.reportViewer.Clear();
                //this.reportViewer.Refresh();
                this.tabControlMain.SelectedIndex = 1;

                object[] objParm = new object[6];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[3] = dgvDateDataGridView[1, pvtintDateDataGridViewRowIndex].Value;
                objParm[4] = strEmployeeNos;
                objParm[5] = strPrintOrder;

                byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Print_EtiReport", objParm);

                DataSet TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                if (TempDataSet.Tables["Report"].Rows.Count > 0)
                {
                    TempDataSet.Tables["Report"].Columns.Add("COMPANY_DESC", typeof(String));
                    TempDataSet.Tables["Report"].Columns.Add("REPORT_HEADER_DESC", typeof(String));
                    TempDataSet.Tables["Report"].Columns.Add("REPORT_DATETIME", typeof(String));

                    TempDataSet.Tables["Report"].Rows[0]["COMPANY_DESC"] = TempDataSet.Tables["ReportHeader"].Rows[0]["COMPANY_DESC"].ToString();
                    TempDataSet.Tables["Report"].Rows[0]["REPORT_HEADER_DESC"] = dgvDateDataGridView[0, pvtintDateDataGridViewRowIndex].Value.ToString();
                    TempDataSet.Tables["Report"].Rows[0]["REPORT_DATETIME"] = TempDataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"].ToString();
                }

                Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", TempDataSet.Tables["Report"]);

                this.reportViewer.LocalReport.DataSources.Clear();
                this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                ////Calculates Number of Pages in Report and Display in Viewer
                this.reportViewer.PageCountMode = Microsoft.Reporting.WinForms.PageCountMode.Actual;

                this.reportViewer.RefreshReport();
                this.reportViewer.Focus();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
    }
}
