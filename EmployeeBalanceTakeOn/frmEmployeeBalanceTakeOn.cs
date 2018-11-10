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
    public partial class frmEmployeeBalanceTakeOn : Form
    {
        clsISUtilities clsISUtilities;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;

        private DataView pvtEmployeeDataView;
        private DataView pvtCompanyDataView;

        private DataView pvtDeductionDescDataView;
        private DataView pvtEmployeeDeductionDataView;
        private DataView pvtPayCategoryDataView;
        private DataView pvtEarningDescDataView;
        private DataView pvtEmployeeEarningDataView;

        private int pvtintEmployeeDataGridViewRowIndex = -1;
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;

        private int pvtintEarningTotalCol = 2;
       
        double pvtdblEarningsTotal = 0;
        
        private byte[] pvtbytCompress;

        private int pvtintCurrEmployeeRecordIndex = 0;
       
        private object[] pvtobjKey2 = new object[2];
        private object[] pvtobjKey3 = new object[3];

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;
        private bool pvtblnEarningDataGridViewLoaded = false;
        private bool pvtblnDeductionsDataGridViewLoaded = false;

        public frmEmployeeBalanceTakeOn()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void frmEmployeeBalanceTakeOn_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busEmployeeBalanceTakeOn");

                this.dgvEarningsTotalDataGridView.Rows.Add("Earnings Total","0.00");

                this.dgvDeductionsTotalDataGridView.Rows.Add("Deductions Total", "0.00");
                this.dgvDeductionsTotalDataGridView.Rows.Add("Nett Total", "0.00");
                this.dgvDeductionsTotalDataGridView.Rows.Add("Earnings Total", "0.00");
               
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEarning.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblDeductionsDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
 
                object[] objParm = new object[1];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
               
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
              
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                if (this.pvtDataSet.Tables.Count > 0)
                {
                    for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                    {
                        this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString());
                    }

                    pvtblnPayrollTypeDataGridViewLoaded = true;

                    if (dgvPayrollTypeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);
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

                    case "dgvEmployeeDataGridView":

                        pvtintEmployeeDataGridViewRowIndex = -1;
                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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
            Clear_Controls();

            pvtCompanyDataView = null;
            pvtCompanyDataView = new DataView(this.pvtDataSet.Tables["Company"],
                "PAY_CATEGORY_TYPE  = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "'",
                "",
                DataViewRowState.CurrentRows);

            if (pvtCompanyDataView.Count == 0)
            {
                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1);

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Company_Records", objParm);

                pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                pvtDataSet.Merge(pvtTempDataSet);
            }
            
            if (pvtCompanyDataView.Count > 0)
            {
                pvtDeductionDescDataView = new DataView(this.pvtDataSet.Tables["DeductionDesc"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "'",
                    "DEDUCTION_NO,DEDUCTION_SUB_ACCOUNT_NO",
                    DataViewRowState.CurrentRows);

                pvtEarningDescDataView = new DataView(this.pvtDataSet.Tables["EarningDesc"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "'",
                    "EARNING_NO",
                    DataViewRowState.CurrentRows);

                pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "'",
                    "",
                    DataViewRowState.CurrentRows);

                this.Clear_DataGridView(this.dgvEmployeeDataGridView);

                this.pvtblnEmployeeDataGridViewLoaded = false;

                for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                {
                    this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                          pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                          pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                          Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_TAX_STARTDATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                          intRow.ToString());   
                }

                this.dgvEmployeeDataGridView.Refresh();

                this.pvtblnEmployeeDataGridViewLoaded = true;

                //Select First Row
                if (pvtEmployeeDataView.Count > 0)
                {
                    this.btnUpdate.Enabled = true;

                    this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, 0);
                }
                else
                {
                    this.btnUpdate.Enabled = false;
                }
            }
        }

        private void Load_Employee()
        {
            pvtEmployeeDeductionDataView = null;
            pvtEmployeeDeductionDataView = new DataView(this.pvtDataSet.Tables["EmployeeDeduction"],
            "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["EMPLOYEE_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "'",
            "DEDUCTION_NO,DEDUCTION_SUB_ACCOUNT_NO",
            DataViewRowState.CurrentRows);

            pvtEmployeeEarningDataView = null;
            pvtEmployeeEarningDataView = new DataView(this.pvtDataSet.Tables["EmployeeEarning"],
                "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[pvtintCurrEmployeeRecordIndex]["EMPLOYEE_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "'",
                "",
                DataViewRowState.CurrentRows);

            if (pvtEmployeeDataView.Count != 0)
            {
                Load_Earnings_And_Deductions();
            }
        }

        private void Load_Earnings_And_Deductions()
        {
            pvtdblEarningsTotal = 0;
            
            double dblDeductionsYTDTotal = 0;

            this.Clear_DataGridView(this.dgvEarningsDataGridView);
            this.Clear_DataGridView(this.dgvDeductionsDataGridView);

            this.pvtblnEarningDataGridViewLoaded = false;
            
            int intEarningDescRow = 0;
            string strEarningDesc = "";

            for (int intRow = 0; intRow < pvtEmployeeEarningDataView.Count; intRow++)
            {
                intEarningDescRow = pvtEarningDescDataView.Find(pvtEmployeeEarningDataView[intRow]["EARNING_NO"].ToString());
                
                strEarningDesc = pvtEarningDescDataView[intEarningDescRow]["EARNING_DESC"].ToString();
                
                this.dgvEarningsDataGridView.Rows.Add(pvtEarningDescDataView[intEarningDescRow]["IRP5_CODE"].ToString(),
                                                      strEarningDesc,
                                                      Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL"]).ToString("######0.00"),
                                                      intRow.ToString());
              
                pvtdblEarningsTotal += Convert.ToDouble(pvtEmployeeEarningDataView[intRow]["TOTAL"]);
            }

            this.pvtblnEarningDataGridViewLoaded = true;
           
            //Earnings Total
            this.dgvEarningsTotalDataGridView[1, 0].Value = pvtdblEarningsTotal.ToString("######0.00");
           
            //Deductions
            this.dgvDeductionsTotalDataGridView[1, 2].Value = pvtdblEarningsTotal.ToString("######0.00");

            pvtblnDeductionsDataGridViewLoaded = false;
            
            int intDeductionRow;
            string strDeductionDesc = "";

            for (int intRow = 0; intRow < pvtEmployeeDeductionDataView.Count; intRow++)
            {
                pvtobjKey2[0] = pvtEmployeeDeductionDataView[intRow]["DEDUCTION_NO"].ToString();
                pvtobjKey2[1] = pvtEmployeeDeductionDataView[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString();

                intDeductionRow = pvtDeductionDescDataView.Find(pvtobjKey2);

                if (Convert.ToInt32(pvtDeductionDescDataView[intDeductionRow]["DEDUCTION_SUB_ACCOUNT_COUNT"]) > 1)
                {
                    strDeductionDesc = pvtDeductionDescDataView[intDeductionRow]["DEDUCTION_DESC"].ToString() + " (" + pvtDeductionDescDataView[intDeductionRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString() + ")";
                }
                else
                {
                    strDeductionDesc = pvtDeductionDescDataView[intDeductionRow]["DEDUCTION_DESC"].ToString();
                }

                this.dgvDeductionsDataGridView.Rows.Add(pvtDeductionDescDataView[intDeductionRow]["IRP5_CODE"].ToString(),
                                                     strDeductionDesc,
                                                     Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL"]).ToString("######0.00"),
                                                     intRow.ToString());
               
                dblDeductionsYTDTotal += Convert.ToDouble(pvtEmployeeDeductionDataView[intRow]["TOTAL"]);
            }

            pvtblnDeductionsDataGridViewLoaded = true;

            this.dgvDeductionsTotalDataGridView[1, 0].Value = dblDeductionsYTDTotal.ToString("######0.00");
            this.dgvDeductionsTotalDataGridView[1, 1].Value = Convert.ToDouble(Convert.ToDouble(this.dgvDeductionsTotalDataGridView[1, 2].Value) - dblDeductionsYTDTotal).ToString("######0.00");
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            this.btnUpdate.Enabled = false;

            this.dgvEarningsDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvEarningsDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

            this.dgvDeductionsDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvDeductionsDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.picEmployeeLock.Visible = true;
            this.picPayrollTypeLock.Visible = true;
     
            this.dgvEmployeeDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            if (this.dgvEarningsDataGridView.Rows.Count > 0)
            {
                this.dgvEarningsDataGridView.CurrentCell = this.dgvEarningsDataGridView[2, this.Get_DataGridView_SelectedRowIndex(dgvEarningsDataGridView)];
            }

            if (this.dgvDeductionsDataGridView.Rows.Count > 0)
            {
                this.dgvDeductionsDataGridView.CurrentCell = this.dgvDeductionsDataGridView[2, this.Get_DataGridView_SelectedRowIndex(dgvDeductionsDataGridView)];
            }
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);

            this.btnUpdate.Enabled = true;

            this.dgvEarningsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEarningsDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.dgvDeductionsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvDeductionsDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
              
            this.picEmployeeLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;
                     
            this.dgvEmployeeDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.pvtDataSet.RejectChanges();

            Load_Employee();
        }

        private void Calculate_Earnings()
        {
            double pvtdblEarningsTotal = 0;
  
            for (int intRow = 0; intRow < this.dgvEarningsDataGridView.Rows.Count; intRow++)
            {
                pvtdblEarningsTotal += Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningTotalCol,intRow].Value);
            }

            this.dgvEarningsTotalDataGridView[1, 0].Value = pvtdblEarningsTotal.ToString("######0.00");

            //Deductions
            this.dgvDeductionsTotalDataGridView[1, 2].Value = pvtdblEarningsTotal.ToString("######0.00");
            
            Calculate_Deductions();
        }

        private void Calculate_Deductions()
        {
            double dblTotal = 0;
            
            for (int intRow = 0; intRow < pvtEmployeeDeductionDataView.Count; intRow++)
            {
                dblTotal += Convert.ToDouble(this.dgvDeductionsDataGridView[2,intRow].Value);
            }

            this.dgvDeductionsTotalDataGridView[1,0].Value = dblTotal.ToString("######0.00");

            this.dgvDeductionsTotalDataGridView[1, 1].Value = Convert.ToDouble((Convert.ToDouble(this.dgvDeductionsTotalDataGridView[1, 2].Value) - Convert.ToDouble(this.dgvDeductionsTotalDataGridView[1, 0].Value))).ToString("#####0.00"); 
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();
                DataTable myDataTable;
                
                myDataTable = this.pvtDataSet.Tables["EmployeeEarning"].Clone();

                pvtTempDataSet.Tables.Add(myDataTable);

                for (int intRow = 0; intRow < pvtEmployeeEarningDataView.Count; intRow++)
                {
                    if (pvtEmployeeEarningDataView[intRow].Row.RowState == DataRowState.Modified)
                    {
                        pvtTempDataSet.Tables["EmployeeEarning"].ImportRow(pvtEmployeeEarningDataView[intRow].Row);
                    }
                }

                myDataTable = this.pvtDataSet.Tables["EmployeeDeduction"].Clone();

                pvtTempDataSet.Tables.Add(myDataTable);

                for (int intRow = 0; intRow < pvtEmployeeDeductionDataView.Count; intRow++)
                {
                    if (pvtEmployeeDeductionDataView[intRow].Row.RowState == DataRowState.Modified)
                    {
                        pvtTempDataSet.Tables["EmployeeDeduction"].ImportRow(pvtEmployeeDeductionDataView[intRow].Row);
                    }
                }

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);
                
                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = pvtbytCompress;
                objParm[2] = this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1);

                clsISUtilities.DynamicFunction("Update_Records", objParm,true);
                
                this.pvtDataSet.AcceptChanges();

                btnCancel_Click(null, null);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Clear_Controls()
        {
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvEarningsDataGridView);
            this.Clear_DataGridView(this.dgvEarningsDataGridView);
            
            this.dgvEarningsTotalDataGridView[1, 0].Value = "0.00";
           
            this.dgvDeductionsTotalDataGridView[1,0].Value = "0.00";
            this.dgvDeductionsTotalDataGridView[1,1].Value = "0.00";
            this.dgvDeductionsTotalDataGridView[1,2].Value = "0.00";
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
                        this.btnUpdate.Enabled = false;

                        Load_CurrentForm_Records();
                    }
                }
            }
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnEmployeeDataGridViewLoaded == true)
            {
                if (pvtintEmployeeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintEmployeeDataGridViewRowIndex = e.RowIndex;

                    pvtintCurrEmployeeRecordIndex = Convert.ToInt32(this.dgvEmployeeDataGridView[4, e.RowIndex].Value);

                    Load_Employee();
                }
            }
        }

        private void dgvEarningsDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEarningsDataGridView.Rows.Count > 0
                & pvtblnEarningDataGridViewLoaded == true)
            {
            }
        }
       
        private void dgvEarningsDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
            e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
        }

        private void dgvDeductionsDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDeductionsDataGridView.Rows.Count > 0
                & pvtblnDeductionsDataGridViewLoaded == true)
            {
            }
        }

        private void dgvDeductionsDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
            e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
        }

        private void dgvEarningsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgvEarningsDataGridView[e.ColumnIndex, e.RowIndex].Value = Convert.ToDouble(dgvEarningsDataGridView[e.ColumnIndex, e.RowIndex].Value).ToString("######0.00");

            int intEarningRow = Convert.ToInt32(dgvEarningsDataGridView[3, e.RowIndex].Value);

            if (Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningTotalCol, e.RowIndex].Value) != Convert.ToDouble(this.pvtEmployeeEarningDataView[intEarningRow]["TOTAL"]))
            {
                this.pvtEmployeeEarningDataView[intEarningRow]["TOTAL"] = Convert.ToDouble(this.dgvEarningsDataGridView[pvtintEarningTotalCol, e.RowIndex].Value);
                
                Calculate_Earnings();
            }
        }

        private void dgvDeductionsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgvDeductionsDataGridView[e.ColumnIndex, e.RowIndex].Value = Convert.ToDouble(dgvDeductionsDataGridView[e.ColumnIndex, e.RowIndex].Value).ToString("######0.00");

            int intDeductionRow = Convert.ToInt32(dgvDeductionsDataGridView[3, e.RowIndex].Value);

            if (Convert.ToDouble(this.dgvDeductionsDataGridView[2, e.RowIndex].Value) != Convert.ToDouble(this.pvtEmployeeDeductionDataView[intDeductionRow]["TOTAL"]))
            {
                this.pvtEmployeeDeductionDataView[intDeductionRow]["TOTAL"] = Convert.ToDouble(this.dgvDeductionsDataGridView[2, e.RowIndex].Value);

                Calculate_Deductions();
            }
        }
    }
}
