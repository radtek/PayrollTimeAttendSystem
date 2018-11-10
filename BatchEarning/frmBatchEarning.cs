using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace InteractPayroll
{
    public partial class frmBatchEarning : Form
    {
        clsISUtilities clsISUtilities;
        //Test
        //Test1
        private DataSet pvtDataSet;

        private DataView pvtEarningDeductionDataView;
        private DataView pvtEmployeeDataView;
        private DataView pvtEmployeeEarningDeductionAmountDataView;

        private string pvtstrPayrollType = "";

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnEarningDeductionDataGridViewLoaded = false;
       
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtinEarningDeductionDataGridViewRowIndex = -1;

        private int pvtintEarningDeductionNo = -1;
        private int pvtintDeductionSubAccountNo = -1;
        private int pvtintProcessNo = 99;

        private int pvtintGrbFilterEditHeight = 0; 
        private int pvtintGrbFilterNormalHeight = 86;

        private bool pvtblnDontFireEvents = false;
        private bool pvtblnImportFileSuccessful = false;

        private string pvtstrMenuId = "";
        private string pvtstrTableName = "";
        private string pvtstrTablePrefix = "";

        DataGridViewCellStyle LockedPayrollRunDataGridViewCellStyle;
        DataGridViewCellStyle ErrorDataGridViewCellStyle;
      
        public frmBatchEarning()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.dgvEarningDeductionDataGridView.Height += 114;

                this.dgvEmployeeDataGridView.Height += 114;
                this.dgvEmployeeSelectedDataGridView.Height += 114;

                this.grbSetProcessValues.Top += 114;
                this.grbLegend.Top += 114;
            }
        }

        private void frmBatchEarning_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busBatchEarning");

                LockedPayrollRunDataGridViewCellStyle = new DataGridViewCellStyle();
                LockedPayrollRunDataGridViewCellStyle.BackColor = Color.Magenta;
                LockedPayrollRunDataGridViewCellStyle.SelectionBackColor = Color.Magenta;

                ErrorDataGridViewCellStyle = new DataGridViewCellStyle();
                ErrorDataGridViewCellStyle.BackColor = Color.Red;
                ErrorDataGridViewCellStyle.SelectionBackColor = Color.Red;

                cboProcessValue.SelectedIndex = 0;

                this.txtAmount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);

                pvtstrMenuId = AppDomain.CurrentDomain.GetData("MenuId").ToString();

                if (pvtstrMenuId == "57")
                {
                    this.dgvEarningDeductionDataGridView.Columns[5].HeaderText = "Amount / Hour";

                    this.lblEarningDeductionDesc.Text = "Earning";
                    this.toolTip1.SetToolTip(this.btnEarningDeductionDeleteRec, "Delete Current Earning Spreadsheet Row");

                    this.grbLock.Text = "Earning Record Lock";

                    pvtstrTableName = "Earning";
                    pvtstrTablePrefix = "EARNING";
                }
                else
                {
                    this.lblEarningDeductionDesc.Text = "Deduction";
                    this.toolTip1.SetToolTip(this.btnEarningDeductionDeleteRec, "Delete Current Deduction Spreadsheet Row");

                    this.grbLock.Text = "Deduction Record Lock";

                    pvtstrTableName = "Deduction";
                    pvtstrTablePrefix = "DEDUCTION";
                }

                pvtintGrbFilterEditHeight = this.grbFilter.Height;
                this.grbFilter.Height = this.pvtintGrbFilterNormalHeight;

                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEarningDeductionDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                object[] objParm = new object[5];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[3] =  AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                objParm[4] = pvtstrMenuId;
               
                byte[] pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add("",
                                                             pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString());

                    if (((this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "Y"
                    | this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "N")
                    & pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString().Substring(0,1) == "W")
                    | (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y"
                    & pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString().Substring(0,1) == "S")
                    | (this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "Y"
                    & pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString().Substring(0,1) == "T"))
                    {
                        this.dgvPayrollTypeDataGridView[0,this.dgvPayrollTypeDataGridView.Rows.Count - 1].Style = this.LockedPayrollRunDataGridViewCellStyle;
                    }
                }

                pvtblnPayrollTypeDataGridViewLoaded = true;

                if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);
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
                        dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEarningDeductionDataGridView":

                        this.pvtinEarningDeductionDataGridViewRowIndex = -1;
                        this.dgvEarningDeductionDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeDataGridView":
                        
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

        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnPayrollTypeDataGridViewLoaded == true
            &  pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
            {
                pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                this.Cursor = Cursors.AppStarting;

                this.grbFilter.Height = this.pvtintGrbFilterNormalHeight;

                pvtstrPayrollType = this.dgvPayrollTypeDataGridView[1, e.RowIndex].Value.ToString().Substring(0, 1);

                if (pvtstrPayrollType == "W")
                {
                    this.lblErrors.Text = "Records are Locked Due to Current Wage Run";
                }
                else
                {
                    if (pvtstrPayrollType == "S")
                    {
                        this.lblErrors.Text = "Records are Locked Due to Current Salary Run";
                    }
                    else
                    {
                        this.lblErrors.Text = "Records are Locked Due to Current Time Attendance Run";
                    }
                }

                if (((this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "Y"
                | this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "N")
                & pvtstrPayrollType == "W")
                | (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y"
                & pvtstrPayrollType == "S")
                | (this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "Y"
                & pvtstrPayrollType == "T"))
                {
                    this.grbLock.Visible = true;
                }
                else
                {
                    this.grbLock.Visible = false;
                }

                pvtEmployeeDataView = null;
                pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                "PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                "EMPLOYEE_CODE",
                DataViewRowState.CurrentRows);

                pvtEarningDeductionDataView = null;
                pvtEarningDeductionDataView = new DataView(this.pvtDataSet.Tables[pvtstrTableName],
                "PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                "",
                DataViewRowState.CurrentRows);

                if (this.cboProcess.Items.Count != 3)
                {
                    this.cboProcess.Items.Clear();

                    this.cboProcess.Items.Add("All");
                    this.cboProcess.Items.Add("Pending");
                    this.cboProcess.Items.Add("Next Run");
                }

                this.cboProcess.SelectedIndex = 0;

                this.cboType.Items.Clear();

                this.cboType.Items.Add("All");

                for (int intRow = 0; intRow < pvtEarningDeductionDataView.Count; intRow++)
                {
                    this.cboType.Items.Add(pvtEarningDeductionDataView[intRow][pvtstrTablePrefix + "_DESC"].ToString());
                }

                if (cboType.Items.Count > 0)
                {
                    if (this.cboType.SelectedIndex == 0)
                    {
                        EventArgs ev = new EventArgs();
                        cboType_SelectedIndexChanged(sender, ev);
                    }
                    else
                    {
                        this.cboType.SelectedIndex = 0;
                    }
                }

                this.Cursor = Cursors.Default;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pvtblnDontFireEvents == false)
            {
                if (this.btnSave.Enabled == false)
                {
                    if (this.cboType.SelectedIndex > -1
                        & this.cboProcess.SelectedIndex > -1)
                    {
                        if (cboProcess.SelectedIndex == 0)
                        {
                            pvtintProcessNo = 99;
                        }
                        else
                        {
                            if (cboProcess.SelectedIndex == 1)
                            {
                                pvtintProcessNo = -1;
                            }
                            else
                            {
                                pvtintProcessNo = 0;
                            }
                        }

                        cboType_SelectedIndexChanged(sender, e);
                    }
                }
            }
        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pvtblnDontFireEvents == false)
            {
                if (this.btnSave.Enabled == false)
                {
                    if (this.cboType.SelectedIndex == 0)
                    {
                        pvtintEarningDeductionNo = -1;
                    }
                    else
                    {
                        pvtintEarningDeductionNo = Convert.ToInt32(pvtEarningDeductionDataView[this.cboType.SelectedIndex - 1][this.pvtstrTablePrefix + "_NO"]);

                        if (this.pvtstrMenuId == "58")
                        {
                            pvtintDeductionSubAccountNo = Convert.ToInt32(pvtEarningDeductionDataView[this.cboType.SelectedIndex - 1]["DEDUCTION_SUB_ACCOUNT_NO"]);
                        }
                    }

                    Load_Earning_Deduction();
                }
                else
                {
                    this.btnAdd.Enabled = true;
                    this.btnAddAll.Enabled = true;
                    this.btnRemove.Enabled = true;
                    this.btnRemoveAll.Enabled = true;

                    this.btnImportFile.Enabled = true;
                    this.txtAmount.Enabled = true;
                    
                    if (dgvEmployeeSelectedDataGridView.Columns[4].Visible == true)
                    {
                        dgvEmployeeSelectedDataGridView.Columns[2].Width += 42;
                        dgvEmployeeSelectedDataGridView.Columns[3].Width += 42;

                        //Width=84
                        dgvEmployeeSelectedDataGridView.Columns[4].Visible = false;
                    }

                    this.lblAmountHoursDesc.Text = "Amount";

                    //New Earnings / Deductions
                    pvtintEarningDeductionNo = Convert.ToInt32(pvtEarningDeductionDataView[this.cboType.SelectedIndex][this.pvtstrTablePrefix + "_NO"]);

                    string strFilter = "PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "' AND " + this.pvtstrTablePrefix + "_NO = " + pvtintEarningDeductionNo;

                    if (this.pvtstrMenuId == "58")
                    {
                        pvtintDeductionSubAccountNo = Convert.ToInt32(pvtEarningDeductionDataView[this.cboType.SelectedIndex]["DEDUCTION_SUB_ACCOUNT_NO"]);

                        strFilter += " AND DEDUCTION_SUB_ACCOUNT_NO = " + pvtintDeductionSubAccountNo;
                    }
                    else
                    {
                        if (pvtintEarningDeductionNo == 2
                            | pvtintEarningDeductionNo == 3
                            | pvtintEarningDeductionNo == 4
                            | pvtintEarningDeductionNo == 5)
                        {
                            this.lblAmountHoursDesc.Text = "Hours";
                        }
                    }

                    DataView EmployeeEarningDeductionDataView = new DataView(pvtDataSet.Tables["Employee" + pvtstrTableName],
                              strFilter,
                              "EMPLOYEE_NO",
                              DataViewRowState.CurrentRows);

                    DataView EmployeeEarningDeductionAmountDataView = new DataView(pvtDataSet.Tables["Employee" + pvtstrTableName + "Amount"],
                              strFilter,
                              "EMPLOYEE_NO",
                              DataViewRowState.CurrentRows);

                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeSelectedDataGridView);

                    int intFindRow = -1;

                    for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                    {
                        //Has Link
                        intFindRow = EmployeeEarningDeductionDataView.Find(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                        if (intFindRow != -1)
                        {
                            intFindRow = EmployeeEarningDeductionAmountDataView.Find(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                            if (intFindRow == -1)
                            {
                                this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                    pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                    pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                    "0.00",
                                                                    pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());
                            }
                        }
                    }
                }
            }
        }

        private void Load_Earning_Deduction()
        {
            if (this.cboType.SelectedIndex == 0
                & this.cboProcess.SelectedIndex == 0)
            {
                this.picPayrollTypeLock.Visible = false;
                this.picEarningDeductionFilter.Visible = false;
            }
            else
            {
                this.picPayrollTypeLock.Visible = true;
                this.picEarningDeductionFilter.Visible = true;
            }

            this.Clear_DataGridView(this.dgvEarningDeductionDataGridView);

            this.pvtblnEarningDeductionDataGridViewLoaded = false;

            string strFilter = "PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "'";

            if (cboType.SelectedIndex != 0)
            {
                strFilter += " AND " + pvtstrTablePrefix + "_NO = " + pvtEarningDeductionDataView[cboType.SelectedIndex - 1][pvtstrTablePrefix + "_NO"].ToString();
            }
        
            pvtEmployeeEarningDeductionAmountDataView = null;
            pvtEmployeeEarningDeductionAmountDataView = new DataView(pvtDataSet.Tables["Employee" + pvtstrTableName + "Amount"],
                      strFilter,
                      "EMPLOYEE_CODE",
                      DataViewRowState.CurrentRows  | DataViewRowState.Deleted);

            int intFindRow = 0;
            string strTypeDesc = "";
            string strProcessDesc = "";
            double dblAmount = 0;

            for (int intRow = 0; intRow < pvtEmployeeEarningDeductionAmountDataView.Count; intRow++)
            {
                if (pvtintProcessNo != 99)
                {
                    if (Convert.ToInt32(pvtEmployeeEarningDeductionAmountDataView[intRow]["PROCESS_NO"]) != pvtintProcessNo)
                    {
                        continue;
                    }
                }

                intFindRow = pvtEmployeeDataView.Find(pvtEmployeeEarningDeductionAmountDataView[intRow]["EMPLOYEE_CODE"].ToString());

                for (int intEarningRow = 0; intEarningRow < pvtEarningDeductionDataView.Count; intEarningRow++)
                {
                    if (pvtEarningDeductionDataView[intEarningRow][this.pvtstrTablePrefix + "_NO"].ToString() == pvtEmployeeEarningDeductionAmountDataView[intRow][this.pvtstrTablePrefix + "_NO"].ToString())
                    {
                        if (this.pvtstrMenuId == "58")
                        {
                            if (pvtEarningDeductionDataView[intEarningRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString() != pvtEmployeeEarningDeductionAmountDataView[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString())
                            {
                                continue;
                            }
                        }

                        strTypeDesc = pvtEarningDeductionDataView[intEarningRow][this.pvtstrTablePrefix +"_DESC"].ToString();
                        dblAmount = Convert.ToDouble(pvtEmployeeEarningDeductionAmountDataView[intRow]["AMOUNT"]);

                        if (Convert.ToInt32(pvtEmployeeEarningDeductionAmountDataView[intRow]["PROCESS_NO"]) == 0)
                        {
                            strProcessDesc = "Next Run";
                        }
                        else
                        {
                            strProcessDesc = "Pending";
                        }

                        this.dgvEarningDeductionDataGridView.Rows.Add("",
                                                                      pvtEmployeeDataView[intFindRow]["EMPLOYEE_CODE"].ToString(),
                                                                      pvtEmployeeDataView[intFindRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                      pvtEmployeeDataView[intFindRow]["EMPLOYEE_NAME"].ToString(),
                                                                      strTypeDesc,
                                                                      dblAmount.ToString("##########0.00"),
                                                                      strProcessDesc,
                                                                      intRow.ToString());

                        if (this.grbLock.Visible == true)
                        {
                            this.dgvEarningDeductionDataGridView[0,this.dgvEarningDeductionDataGridView.Rows.Count - 1].Style = this.LockedPayrollRunDataGridViewCellStyle;
                        }

                        break;
                    }
                }
            }

            this.pvtblnEarningDeductionDataGridViewLoaded = true;

            if (this.dgvEarningDeductionDataGridView.Rows.Count > 0)
            {
                if (((this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "Y"
                | this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "N")
                & pvtstrPayrollType == "W")
                | (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y"
                & pvtstrPayrollType == "S")
                | (this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "Y"
                & pvtstrPayrollType == "T"))
                {
                    this.btnNew.Enabled = false;
                    this.btnUpdate.Enabled = false;
                    this.btnDelete.Enabled = false;
                }
                else
                {
                    this.btnNew.Enabled = true;
                    this.btnUpdate.Enabled = true;
                    this.btnDelete.Enabled = true;
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvEarningDeductionDataGridView, 0);
            }
            else
            {
                if (((this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "Y"
                | this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "N")
                & pvtstrPayrollType == "W")
                | (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y"
                & pvtstrPayrollType == "S")
                | (this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "Y"
                & pvtstrPayrollType == "T"))
                {
                    this.btnNew.Enabled = false;
                }
                else
                {
                    this.btnNew.Enabled = true;
                }

                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)];
                
                this.dgvEmployeeSelectedDataGridView.Rows.Add("",
                                                              myDataGridViewRow.Cells[0].Value.ToString(),
                                                              myDataGridViewRow.Cells[1].Value.ToString(),
                                                              myDataGridViewRow.Cells[2].Value.ToString(),
                                                              myDataGridViewRow.Cells[3].Value.ToString(),
                                                              myDataGridViewRow.Cells[4].Value.ToString());

                this.dgvEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeSelectedDataGridView.CurrentCell = this.dgvEmployeeSelectedDataGridView[0, this.dgvEmployeeSelectedDataGridView.Rows.Count - 1];

                this.btnImportFile.Visible = false;

                this.txtAmount.Enabled = true;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvEmployeeSelectedDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeeSelectedDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeSelectedDataGridView)];

                this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow.Cells[1].Value.ToString(),
                                                      myDataGridViewRow.Cells[2].Value.ToString(),
                                                      myDataGridViewRow.Cells[3].Value.ToString(),
                                                      "0.00",
                                                      myDataGridViewRow.Cells[5].Value.ToString());

                this.dgvEmployeeSelectedDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeDataGridView.CurrentCell = this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1];

                if (this.dgvEmployeeSelectedDataGridView.Rows.Count == 0)
                {
                    this.btnImportFile.Visible = true;
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

            if (this.dgvEmployeeSelectedDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.Text = this.Text + " - New";

            pvtblnImportFileSuccessful = false;

            this.btnImportFile.Visible = true;

            if (dgvEmployeeSelectedDataGridView.Columns[4].Visible == true)
            {
                dgvEmployeeSelectedDataGridView.Columns[2].Width += 42;
                dgvEmployeeSelectedDataGridView.Columns[3].Width += 42;

                //Width=84
                dgvEmployeeSelectedDataGridView.Columns[4].Visible = false;
            }
            
            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;

            this.btnImportFile.Enabled = true;
            
            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.picPayrollTypeLock.Image = BatchEarning.Properties.Resources.NewLock16;
            this.picPayrollTypeLock.Visible = true;
            this.picEarningDeductionFilter.Visible = false;
            
            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.grbFilter.Height = this.pvtintGrbFilterEditHeight;

            this.lblEarningDeductionDesc.Visible = false;
            this.dgvEarningDeductionDataGridView.Visible = false;

            this.lblEmployee.Visible = true;
            this.dgvEmployeeDataGridView.Visible = true;

            this.lblSelectedEmployee.Visible = true;
            this.dgvEmployeeSelectedDataGridView.Visible = true;

            this.btnAdd.Visible = true;
            this.btnAddAll.Visible = true;

            this.btnRemove.Visible = true;
            this.btnRemoveAll.Visible = true;

            this.cboType.Items.Clear();

            for (int intRow = 0; intRow < pvtEarningDeductionDataView.Count; intRow++)
            {
                this.cboType.Items.Add(pvtEarningDeductionDataView[intRow][this.pvtstrTablePrefix + "_DESC"].ToString());
            }

            if (this.cboType.Items.Count > 0)
            {
                this.cboType.SelectedIndex = 0;
            }
           
            this.cboProcess.Items.Clear();

            this.cboProcess.Items.Add("Pending");
            this.cboProcess.Items.Add("Next Run");

            this.txtAmount.Enabled = true;
            this.txtAmount.Text = "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.pvtDataSet.RejectChanges();

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

            this.cboProcessValue.Enabled = false;
            this.btnSetValue.Enabled = false;

            this.btnImportFile.Visible = false;

            this.dgvEarningDeductionDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEarningDeductionDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.btnEarningDeductionDeleteRec.Visible = false;

            this.cboType.Enabled = true;
            this.cboProcess.Enabled = true;
            
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.picPayrollTypeLock.Visible = false;

            this.btnNew.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.grbFilter.Height = this.pvtintGrbFilterNormalHeight;

            this.lblEarningDeductionDesc.Visible = true;
            this.dgvEarningDeductionDataGridView.Visible = true;

            this.lblEmployee.Visible = false;
            this.dgvEmployeeDataGridView.Visible = false;

            this.lblSelectedEmployee.Visible = false;
            this.dgvEmployeeSelectedDataGridView.Visible = false;

            this.btnAdd.Visible = false;
            this.btnAddAll.Visible = false;

            this.btnRemove.Visible = false;
            this.btnRemoveAll.Visible = false;

            this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView,pvtintPayrollTypeDataGridViewRowIndex);
        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true
            && this.btnAdd.Enabled == true)
            {
                btnAdd_Click(sender, e);
            }
        }

        private void dgvEmployeeSelectedDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true
            && this.btnRemove.Enabled == true)
            {
                btnRemove_Click(sender, e);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet TempDataSet = null;

                if (this.Text.IndexOf("- New") > -1)
                {
                    Int16 int16ProcessNo;

                    if (this.cboProcess.SelectedIndex == -1)
                    {
                        CustomMessageBox.Show("Select Process.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.cboProcess.Focus();
                        return;
                    }
                    else
                    {
                        if (this.cboProcess.SelectedIndex == 0)
                        {
                            //Pending
                            int16ProcessNo = -1;
                        }
                        else
                        {
                            //Next Run
                            int16ProcessNo = 0;
                        }
                    }

                    if (pvtblnImportFileSuccessful == false)
                    {
                        if (this.txtAmount.Text.Replace(".", "") == "")
                        {
                            CustomMessageBox.Show("Enter Amount.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this.txtAmount.Focus();
                            return;
                        }
                        else
                        {
                            if (Convert.ToDouble(this.txtAmount.Text) == 0)
                            {
                                CustomMessageBox.Show("Amount Must be Greater than Zero.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                this.txtAmount.Focus();
                                return;
                            }
                        }
                    }

                    string strEmployeeNos = "";
                    string strEmployeeAmounts = "";

                    if (this.dgvEmployeeSelectedDataGridView.Rows.Count == 0)
                    {
                        CustomMessageBox.Show("Select Employee/s.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        for (int intRow = 0; intRow < this.dgvEmployeeSelectedDataGridView.Rows.Count; intRow++)
                        {
                            if (intRow == 0)
                            {
                                strEmployeeNos = this.dgvEmployeeSelectedDataGridView[5, intRow].Value.ToString();

                                if (pvtblnImportFileSuccessful == true)
                                {
                                    strEmployeeAmounts = this.dgvEmployeeSelectedDataGridView[4, intRow].Value.ToString();
                                }
                            }
                            else
                            {
                                strEmployeeNos += "," + this.dgvEmployeeSelectedDataGridView[5, intRow].Value.ToString();

                                if (pvtblnImportFileSuccessful == true)
                                {
                                    strEmployeeAmounts += "," + this.dgvEmployeeSelectedDataGridView[4, intRow].Value.ToString();
                                }
                            }
                        }
                    }

                    DataView UpdateDataView = new DataView(pvtDataSet.Tables["Employee" + this.pvtstrTableName + "Amount"],
                       "PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "'",
                       "",
                       DataViewRowState.CurrentRows);

                    //Remove All Records for PAY_CATEGORY_TYPE
                    for (int intRow = 0; intRow < UpdateDataView.Count; intRow++)
                    {
                        UpdateDataView[intRow].Delete();

                        intRow -= 1;
                    }

                    object[] objParm = new object[11];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = this.pvtstrPayrollType;
                    objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[3] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                    objParm[4] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                    objParm[5] = this.pvtintEarningDeductionNo;
                    objParm[6] = this.pvtintDeductionSubAccountNo;
                    objParm[7] = int16ProcessNo;

                    if (pvtblnImportFileSuccessful == true)
                    {
                        objParm[8] = strEmployeeNos;
                        objParm[9] = strEmployeeAmounts;
                    }
                    else
                    {
                        objParm[8] = Convert.ToDouble(this.txtAmount.Text);
                        objParm[9] = strEmployeeNos;
                    }

                    objParm[10] = this.pvtstrMenuId;

                    byte[] bytCompress = null;

                    if (pvtblnImportFileSuccessful == true)
                    {
                        bytCompress = (byte[])clsISUtilities.DynamicFunction("Insert_Records_From_File_Import", objParm, true);
                    }
                    else
                    {

                        bytCompress = (byte[])clsISUtilities.DynamicFunction("Insert_Records", objParm, true);
                    }

                    TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);
                }
                else
                {
                    DataView UpdateDataView = new DataView(pvtDataSet.Tables["Employee" + pvtstrTableName + "Amount"],
                        "PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "'",
                        "",
                        DataViewRowState.CurrentRows | DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent);
                   
                    TempDataSet = new DataSet();

                    DataTable myDataTable = this.pvtDataSet.Tables["Employee" + pvtstrTableName + "Amount"].Clone();

                    TempDataSet.Tables.Add(myDataTable);

                    for (int intRow = 0; intRow < UpdateDataView.Count; intRow++)
                    {
                        if (UpdateDataView[intRow].Row.RowState == DataRowState.Modified
                            | UpdateDataView[intRow].Row.RowState == DataRowState.Deleted)
                        {
                            TempDataSet.Tables["Employee" + pvtstrTableName + "Amount"].ImportRow(UpdateDataView[intRow].Row);
                        }

                        UpdateDataView[intRow].Delete();
                    }

                    if (TempDataSet.Tables["Employee" + pvtstrTableName + "Amount"].Rows.Count > 0)
                    {
                        //Compress DataSet
                        byte[] bytCompress = clsISUtilities.Compress_DataSet(TempDataSet);

                        object[] objParm = new object[7];
                        objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        objParm[1] = this.pvtstrPayrollType;
                        objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                        objParm[3] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                        objParm[4] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                        objParm[5] = pvtstrMenuId;
                        objParm[6] = bytCompress;

                        byte[] bytReturnCompress = (byte[])clsISUtilities.DynamicFunction("Update_Record", objParm, true);

                        TempDataSet = null;
                        TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytReturnCompress);
                    }
                    else
                    {
                        goto btnSave_Continue;
                    }
                }

                if (TempDataSet.Tables["CompanyCheck"].Rows[0]["RUN_IND"].ToString() == "")
                {
                    TempDataSet.Tables.Remove("CompanyCheck");

                    pvtDataSet.Merge(TempDataSet);
                }
                else
                {
                    CustomMessageBox.Show("Action Cancelled - Payroll Run in Progress.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (this.pvtstrPayrollType == "W")
                    {
                        this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"] = "Y";
                    }
                    else
                    {
                        if (this.pvtstrPayrollType == "S")
                        {
                            this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"] = "Y";
                        }
                        else
                        {
                            this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"] = "Y";
                        }
                    }

                    if (((this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "Y"
                        | this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "N")
                        & pvtstrPayrollType == "W")
                        | (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y"
                        & pvtstrPayrollType == "S")
                        | (this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "Y"
                        & pvtstrPayrollType == "T"))
                    {
                        this.dgvPayrollTypeDataGridView[0,pvtintPayrollTypeDataGridViewRowIndex].Style = this.LockedPayrollRunDataGridViewCellStyle;
                    }

                    this.pvtDataSet.Tables["Employee" + pvtstrTableName + "Amount"].RejectChanges();
                }

                this.pvtDataSet.AcceptChanges();

                btnSave_Continue:

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
               
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text = this.Text + " - Update";

            this.cboProcessValue.Enabled = true;
            this.btnSetValue.Enabled = true;

            this.btnEarningDeductionDeleteRec.Visible = true;

            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.cboType.Enabled = false;
            this.cboProcess.Enabled = false;
            
            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.picPayrollTypeLock.Image = BatchEarning.Properties.Resources.NewLock16;
            this.picPayrollTypeLock.Visible = true;
            
            this.dgvEarningDeductionDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvEarningDeductionDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

            if (this.dgvEarningDeductionDataGridView.Rows.Count > 0)
            {
                dgvEarningDeductionDataGridView.CurrentCell = dgvEarningDeductionDataGridView[5, 0];
                dgvEarningDeductionDataGridView.Focus();
            }
        }

        private void dgvEarningDeductionDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.Control is ComboBox)
                {
                    ComboBox myComboBox = (ComboBox)e.Control;

                    if (myComboBox != null)
                    {
                        myComboBox.SelectedIndexChanged -= new EventHandler(ComboBox_SelectedIndexChanged);
                        myComboBox.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
                    }
                }
                else
                {
                    if (e.Control is TextBox)
                    {
                        e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                        e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                    }
                }
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                ComboBox myComboBox = (ComboBox)sender;

                int intRow = Convert.ToInt32(this.dgvEarningDeductionDataGridView[7, pvtinEarningDeductionDataGridViewRowIndex].Value);

                int intProcessNo = myComboBox.SelectedIndex - 1;

                if (Convert.ToInt32(pvtEmployeeEarningDeductionAmountDataView[intRow]["PROCESS_NO"]) != intProcessNo)
                {
                    pvtEmployeeEarningDeductionAmountDataView[intRow]["PROCESS_NO"] = intProcessNo;
                }
            }
        }

        private void dgvEarningDeductionDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnEarningDeductionDataGridViewLoaded == true
            & pvtinEarningDeductionDataGridViewRowIndex != e.RowIndex)
            {
                pvtinEarningDeductionDataGridViewRowIndex = e.RowIndex;
            }
        }

        private void dgvEarningDeductionDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.ColumnIndex == 5)
                {
                    int intRow = Convert.ToInt32(this.dgvEarningDeductionDataGridView[7, pvtinEarningDeductionDataGridViewRowIndex].Value);

                    if (this.dgvEarningDeductionDataGridView[e.ColumnIndex, e.RowIndex].Value == null)
                    {
                        pvtEmployeeEarningDeductionAmountDataView[intRow]["AMOUNT"] = 0;
                    }
                    else
                    {
                        if (this.dgvEarningDeductionDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Replace(".", "") == "")
                        {
                            pvtEmployeeEarningDeductionAmountDataView[intRow]["AMOUNT"] = 0;
                        }
                        else
                        {
                            if (Convert.ToDouble(pvtEmployeeEarningDeductionAmountDataView[intRow]["AMOUNT"]) != Convert.ToDouble(this.dgvEarningDeductionDataGridView[e.ColumnIndex, e.RowIndex].Value))
                            {
                                pvtEmployeeEarningDeductionAmountDataView[intRow]["AMOUNT"] = Convert.ToDouble(this.dgvEarningDeductionDataGridView[e.ColumnIndex, e.RowIndex].Value);
                            }
                        }
                    }
                }
            }
        }

        private void btnEarningDeductionDeleteRec_Click(object sender, EventArgs e)
        {
            if (this.dgvEarningDeductionDataGridView.Rows.Count > 0)
            {
                int intRow = Convert.ToInt32(this.dgvEarningDeductionDataGridView[7, pvtinEarningDeductionDataGridViewRowIndex].Value);

                pvtEmployeeEarningDeductionAmountDataView[intRow].Delete();

                this.dgvEarningDeductionDataGridView.Rows.RemoveAt(this.Get_DataGridView_SelectedRowIndex(this.dgvEarningDeductionDataGridView));
            }
        }

        private void dgvEarningDeductionDataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.ColumnIndex >= 5)
                {
                    this.Cursor = Cursors.Default;
                }
                else
                {
                    this.Cursor = Cursors.No;
                }
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void dgvEarningDeductionDataGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomMessageBox.Show("Delete ALL Records in Spreadsheet?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dlgResult == DialogResult.Yes)
                {
                    DataSet TempDataSet = new DataSet();

                    DataTable myDataTable = this.pvtDataSet.Tables["Employee" + pvtstrTableName + "Amount"].Clone();

                    TempDataSet.Tables.Add(myDataTable);

                    int intDataViewRow = 0;

                    for (int intRow = 0; intRow < this.dgvEarningDeductionDataGridView.Rows.Count; intRow++)
                    {
                        intDataViewRow = Convert.ToInt32(this.dgvEarningDeductionDataGridView[7, intRow].Value);

                        TempDataSet.Tables["Employee" + pvtstrTableName + "Amount"].ImportRow(pvtEmployeeEarningDeductionAmountDataView[intDataViewRow].Row);
                    }

                    byte[] bytCompress = clsISUtilities.Compress_DataSet(TempDataSet);

                    DataView UpdateDataView = new DataView(pvtDataSet.Tables["Employee" + pvtstrTableName + "Amount"],
                        "PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "'",
                        "",
                        DataViewRowState.CurrentRows);

                    //Remove All Records for PAY_CATEGORY_TYPE
                    for (int intRow = 0; intRow < UpdateDataView.Count; intRow++)
                    {
                        UpdateDataView[intRow].Delete();

                        intRow -= 1;
                    }

                    object[] objParm = new object[7];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = this.pvtstrPayrollType;
                    objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[3] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                    objParm[4] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                    objParm[5] = this.pvtstrMenuId;
                    objParm[6] = bytCompress;

                    bytCompress = (byte[])clsISUtilities.DynamicFunction("Delete_Records", objParm, true);

                    TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                    if (TempDataSet.Tables["CompanyCheck"].Rows[0]["RUN_IND"].ToString() == "")
                    {
                        TempDataSet.Tables.Remove("CompanyCheck");

                        pvtDataSet.Merge(TempDataSet);
                    }
                    else
                    {
                        CustomMessageBox.Show("Action Cancelled - Payroll Run in Progress.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (this.pvtstrPayrollType == "W")
                        {
                            this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"] = "Y";
                        }
                        else
                        {
                            if (this.pvtstrPayrollType == "S")
                            {
                                this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"] = "Y";
                            }
                            else
                            {
                                this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"] = "Y";
                            }
                        }
                        
                        if (((this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "Y"
                        | this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_RUN_IND"].ToString() == "N")
                        & pvtstrPayrollType == "W")
                        | (this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_RUN_IND"].ToString() == "Y"
                        & pvtstrPayrollType == "S")
                        | (this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTENDANCE_RUN_IND"].ToString() == "Y"
                        & pvtstrPayrollType == "T"))
                        {
                            this.dgvPayrollTypeDataGridView[0,pvtintPayrollTypeDataGridViewRowIndex].Style = this.LockedPayrollRunDataGridViewCellStyle;
                        }

                        this.pvtDataSet.Tables["Employee" + pvtstrTableName + "Amount"].RejectChanges();
                    }

                    this.pvtDataSet.AcceptChanges();

                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnSetValue_Click(object sender, EventArgs e)
        {
            int intDataViewRow = 0;

            int intProcessNo = 0;

            if (this.cboProcessValue.SelectedIndex == 0)
            {
                intProcessNo = -1;
            }

            for (int intRow = 0; intRow < this.dgvEarningDeductionDataGridView.Rows.Count; intRow++)
            {
                if (this.dgvEarningDeductionDataGridView[6, intRow].Value.ToString() != this.cboProcessValue.SelectedItem.ToString())
                {
                    this.dgvEarningDeductionDataGridView[6, intRow].Value = this.cboProcessValue.SelectedItem.ToString();
                    
                    intDataViewRow = Convert.ToInt32(this.dgvEarningDeductionDataGridView[7, intRow].Value);

                    pvtEmployeeEarningDeductionAmountDataView[intDataViewRow]["PROCESS_NO"] = intProcessNo;
                }
            }
        }

        private void btnImportFile_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            bool blnEmployeeFound = false;
            bool blnEmployeeInFileNotFound = false;
            bool blnErrorInFile = false;
            decimal value;

            pvtblnImportFileSuccessful = false;

            if (result == DialogResult.OK) 
            {
                pvtblnImportFileSuccessful = true;
                List<EmployeeEarning> listEmployeeEarning = new List<EmployeeEarning>();

                using (var reader = new StreamReader(openFileDialog.FileName))
                {
                    while (!reader.EndOfStream)
                    {
                        blnEmployeeFound = false;

                        var line = reader.ReadLine();

                        if (line.Trim() == "")
                        {
                            continue;
                        }
                        
                        string[] strValues = line.Split(',');

                        if (strValues.Length == 2)
                        {
                            //Check for Valid Amount
                            if (Decimal.TryParse(strValues[1].Trim(), out value) == false)
                            {
                                blnErrorInFile = true;
                                break;
                            }

                            for (int intRow = 0; intRow < dgvEmployeeDataGridView.Rows.Count; intRow++)
                            {
                                if (dgvEmployeeDataGridView[0, intRow].Value.ToString() == strValues[0].Trim())
                                {
                                    blnEmployeeFound = true;

                                    EmployeeEarning employeeEarning = new EmployeeEarning();
                                    
                                    employeeEarning.EmployeeNo = dgvEmployeeDataGridView[4, intRow].Value.ToString();
                                    employeeEarning.EmployeeCode = dgvEmployeeDataGridView[0, intRow].Value.ToString();
                                    employeeEarning.EmployeeSurname = dgvEmployeeDataGridView[1, intRow].Value.ToString();
                                    employeeEarning.EmployeeName = dgvEmployeeDataGridView[2, intRow].Value.ToString();
                                    employeeEarning.EmployeeAmount = Convert.ToDecimal(strValues[1].Trim());

                                    listEmployeeEarning.Add(employeeEarning);

                                    break;
                                }
                            }

                            if (blnEmployeeFound == false)
                            {
                                blnEmployeeInFileNotFound = true;
                                pvtblnImportFileSuccessful = false;

                                EmployeeEarning employeeEarning = new EmployeeEarning();

                                employeeEarning.EmployeeNo = "";
                                employeeEarning.EmployeeCode = strValues[0].Trim();
                                employeeEarning.EmployeeSurname = "";
                                employeeEarning.EmployeeName = "";

                                employeeEarning.EmployeeAmount = Convert.ToDecimal(strValues[1].Trim());

                                listEmployeeEarning.Add(employeeEarning);
                            }
                        }
                        else
                        {
                            blnErrorInFile = true;
                            pvtblnImportFileSuccessful = false;

                            break;
                        }
                    }
                }

                if (blnErrorInFile == false)
                {
                    //Value Column
                    if (dgvEmployeeSelectedDataGridView.Columns[4].Visible == false)
                    {
                        dgvEmployeeSelectedDataGridView.Columns[2].Width -= 42;
                        dgvEmployeeSelectedDataGridView.Columns[3].Width -= 42;

                        //Width=84
                        dgvEmployeeSelectedDataGridView.Columns[4].Visible = true;
                    }
                    
                    foreach (var rec in listEmployeeEarning)
                    {
                        if (rec.EmployeeNo == "")
                        {
                            this.dgvEmployeeSelectedDataGridView.Rows.Add("",
                                                                          rec.EmployeeCode,
                                                                          rec.EmployeeName,
                                                                          rec.EmployeeSurname,
                                                                          rec.EmployeeAmount.ToString("########0.00"),
                                                                          "");

                            dgvEmployeeSelectedDataGridView[0, this.dgvEmployeeSelectedDataGridView.Rows.Count - 1].Style = this.ErrorDataGridViewCellStyle;
                        }
                        else
                        {
                            this.dgvEmployeeSelectedDataGridView.Rows.Add("",
                                                                          rec.EmployeeCode,
                                                                          rec.EmployeeName,
                                                                          rec.EmployeeSurname,
                                                                          rec.EmployeeAmount.ToString("########0.00"),
                                                                          rec.EmployeeNo);

                            if (blnEmployeeInFileNotFound == true)
                            {
                                this.btnSave.Enabled = false;
                            }
                            else
                            {
                                for (int intRow = 0; intRow < dgvEmployeeDataGridView.Rows.Count; intRow++)
                                {
                                    if (dgvEmployeeDataGridView[0, intRow].Value.ToString() == rec.EmployeeCode)
                                    {
                                        this.dgvEmployeeDataGridView.Rows.RemoveAt(intRow);

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    CustomMessageBox.Show("File NOT in Correct Format\n\nFile is Comma Delimited\nFile Must have NO Header Line\nEach Record has 2 Fields\n\n    1) Employee Code\n    2) " + cboType.SelectedItem.ToString() + " Amount", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (blnEmployeeInFileNotFound == true)
                {
                    CustomMessageBox.Show("There are Employee/s in File that are NOT relevant to the " + cboType.SelectedItem.ToString() + " Deduction\n\nFix File to Continue.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.btnSave.Enabled = false;
                }

                this.txtAmount.Text = "";
                this.txtAmount.Enabled = false;

                this.btnAdd.Enabled = false;
                this.btnAddAll.Enabled = false;
                this.btnRemove.Enabled = false;
                this.btnRemoveAll.Enabled = false;

                this.btnImportFile.Enabled = false;
            }
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
