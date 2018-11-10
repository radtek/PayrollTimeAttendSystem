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
    public partial class frmSalaryWageIncrease : Form
    {
        private clsISUtilities clsISUtilities;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnTimePeriodDataGridViewLoaded = false;
        private bool pvtblnPercentIncreaseDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;

        private bool pvtblnEmployeesLoaded = false;

        private bool pvtblnPercentIncreaseClicked = false;

        private int pvtintNumberDays = -1;
        private double pvtdblPercentIncrease = -1;

        private DataSet pvtDataSet;

        private string pvtstrPayrollType = "";

        private Int64 parInt64CompanyNo = -1;
        private int pvtintPayCategoryNo = -1;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintTimePeriodDataGridViewRowIndex = -1;
        private int pvtintPercentIncreaseDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;

        private int pvtintTimerCount = 0;
        
        System.Data.DataView pvtEmployeeDataView = null;
        System.Data.DataView pvtPayCategoryDataView = null;

        DataGridViewCellStyle LockedPayrollRunDataGridViewCellStyle;

        public frmSalaryWageIncrease()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.dgvEmployeeDataGridView.Height += 114;
                this.dgvEmployeeSelectedDataGridView.Height += 114;

                this.btnAdd.Top += 57;
                this.btnAddAll.Top += 57;
                this.btnRemove.Top += 57;
                this.btnRemoveAll.Top += 57;

                this.grbInfo.Top += 114;
                this.grbBackupInfo.Top += 114;
                this.grbLegend.Top += 114;

                this.lblEmployeeNotLinked.Top += 114;
                this.dgvEmployeeNotLinkedDataGridView.Top += 114;
            }
        }

        private void frmSalaryWageIncrease_Load(object sender, EventArgs e)
        {
            try
            {
                LockedPayrollRunDataGridViewCellStyle = new DataGridViewCellStyle();
                LockedPayrollRunDataGridViewCellStyle.BackColor = Color.Magenta;
                LockedPayrollRunDataGridViewCellStyle.SelectionBackColor = Color.Magenta;

                clsISUtilities = new clsISUtilities(this, "busSalaryWageIncrease");

                //Hard Coded - Needs to Change
                parInt64CompanyNo = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")); 
                
                this.txtIncrease.KeyPress += clsISUtilities.Numeric_2_Decimal_KeyPress;

                this.lblDescription.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPercentIncrease.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeNotLinked.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeLinked.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblListEmployees.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayCategory.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");

                this.pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);
               
                dgvTimePeriodDataGridView.Rows.Add("4 Weeks","28");
                dgvTimePeriodDataGridView.Rows.Add("3 Weeks", "21");
                dgvTimePeriodDataGridView.Rows.Add("2 Weeks","14");
                dgvTimePeriodDataGridView.Rows.Add("1 Week","7");
                dgvTimePeriodDataGridView.Rows.Add("6 Days", "6");
                dgvTimePeriodDataGridView.Rows.Add("5 Days", "5");
                dgvTimePeriodDataGridView.Rows.Add("4 Days", "4");
                dgvTimePeriodDataGridView.Rows.Add("3 Days", "3");
                dgvTimePeriodDataGridView.Rows.Add("2 Days", "2");
                dgvTimePeriodDataGridView.Rows.Add("1 Day", "1");

                pvtblnTimePeriodDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(dgvTimePeriodDataGridView, 0);
                               
                object[] objParm = new object[3];
                objParm[0] = parInt64CompanyNo;
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                Load_PayCategories();

                this.tmrTimer.Enabled = true;
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_PayCategories()
        {
            pvtblnPayCategoryDataGridViewLoaded = false;

            this.Clear_DataGridView(dgvPayCategoryDataGridView);

            pvtPayCategoryDataView = null;
            pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"], "PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "'", "", DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
            {
                dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                    intRow.ToString());
            }

            pvtblnPayCategoryDataGridViewLoaded = true;

            if (pvtPayCategoryDataView.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
            }
            else
            {
                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                this.Clear_DataGridView(this.dgvEmployeeSelectedDataGridView);
                this.Clear_DataGridView(this.dgvEmployeeNotLinkedDataGridView);

                this.Clear_DataGridView(this.dgvPercentIncreaseDataGridView);

                this.btnUpdate.Enabled = false;
            }
        }

        private void Load_Employees()
        {
            this.Clear_DataGridView(dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeSelectedDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeNotLinkedDataGridView);

            bool blnEmployeeIncluded = true;
            string strSalaryWageIncreaseDateTime = "";
            string strNames = "";
            string strPercent = "";

            pvtEmployeeDataView.Sort = "EMPLOYEE_CODE";

            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
            {
                blnEmployeeIncluded = true;

                if (pvtEmployeeDataView[intRow]["SALARY_WAGE_INCREASE"].ToString() == "")
                {
                    strSalaryWageIncreaseDateTime = "";
                }
                else
                {
                    strSalaryWageIncreaseDateTime = Convert.ToDateTime(pvtEmployeeDataView[intRow]["SALARY_WAGE_INCREASE_DATETIME"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString() + " HH:mm");
                }

                if (rbnSurnameName.Checked == true)
                {
                    strNames = pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString() + " / " + pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString();
                }
                else
                {
                    strNames = pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString() + " / " + pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString();
                }

                if (pvtEmployeeDataView[intRow]["SALARY_WAGE_INCREASE"].ToString() == "")
                {
                    strPercent = "";
                }
                else
                {
                    strPercent = Convert.ToDouble(pvtEmployeeDataView[intRow]["SALARY_WAGE_INCREASE"]).ToString("##0.00");
                }

                if (pvtdblPercentIncrease != -1)
                {
                    if (pvtEmployeeDataView[intRow]["SALARY_WAGE_INCREASE"].ToString() == "")
                    {
                        blnEmployeeIncluded = false;
                    }
                    else
                    {
                        if  (Convert.ToDouble(pvtEmployeeDataView[intRow]["SALARY_WAGE_INCREASE"]) != pvtdblPercentIncrease)
                        {
                            blnEmployeeIncluded = false;
                        }
                    }
                }

                if (blnEmployeeIncluded == true)
                {
                    if (pvtEmployeeDataView[intRow]["SALARY_WAGE_INCREASE_DATETIME"].ToString() == "")
                    {
                        if (this.rbnChanged.Checked == true)
                        {
                            blnEmployeeIncluded = false;
                        }
                    }
                    else
                    {
                        if (this.rbnChanged.Checked == true)
                        {
                            if (Convert.ToDateTime(pvtEmployeeDataView[intRow]["SALARY_WAGE_INCREASE_DATETIME"]).AddDays(pvtintNumberDays) < DateTime.Now)
                            {
                                blnEmployeeIncluded = false;
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(pvtEmployeeDataView[intRow]["SALARY_WAGE_INCREASE_DATETIME"]).AddDays(pvtintNumberDays) >= DateTime.Now)
                            {
                                blnEmployeeIncluded = false;
                            }
                        }
                    }
                }

                if (blnEmployeeIncluded == true)
                {
                    this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                          strNames,
                                                          Convert.ToDouble(pvtEmployeeDataView[intRow]["HOURLY_RATE"]).ToString("########0.00"),
                                                          "",
                                                          strPercent,
                                                          strSalaryWageIncreaseDateTime,
                                                          intRow.ToString());
                }
                else
                {
                    this.dgvEmployeeNotLinkedDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                   strNames,
                                                                   Convert.ToDouble(pvtEmployeeDataView[intRow]["HOURLY_RATE"]).ToString("########0.00"),
                                                                   strPercent,
                                                                   strSalaryWageIncreaseDateTime,
                                                                   intRow.ToString());
                }

                pvtblnEmployeesLoaded = true;

                if (pvtEmployeeDataView[intRow]["PAYROLL_LINK"] != System.DBNull.Value)
                {
                    if (blnEmployeeIncluded == true)
                    {
                        this.dgvEmployeeDataGridView.Rows[this.dgvEmployeeDataGridView.Rows.Count - 1].HeaderCell.Style = LockedPayrollRunDataGridViewCellStyle;
                    }
                    else
                    {
                        this.dgvEmployeeNotLinkedDataGridView.Rows[this.dgvEmployeeNotLinkedDataGridView.Rows.Count - 1].HeaderCell.Style = LockedPayrollRunDataGridViewCellStyle;
                    }
                }
            }

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, 0);
                this.btnUpdate.Enabled = true;
            }
            else
            {
                this.btnUpdate.Enabled = false;
            }

            if (this.dgvEmployeeNotLinkedDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvEmployeeNotLinkedDataGridView, 0);
            }
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

                    case "dgvTimePeriodDataGridView":

                        pvtintTimePeriodDataGridViewRowIndex = -1;
                        this.dgvTimePeriodDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPercentIncreaseDataGridView":

                        pvtintPercentIncreaseDataGridViewRowIndex = -1;
                        this.dgvPercentIncreaseDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeDataGridView":

                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void Clear_DataGridView(DataGridView myDataGridView)
        {
            myDataGridView.Rows.Clear();

            if (myDataGridView.SortedColumn != null)
            {
                myDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
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

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    pvtdblPercentIncrease = -1;

                    pvtstrPayrollType = this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                    if (pvtstrPayrollType == "W")
                    {
                        gbIncrease.Text = "Hourly Rate Percent Increase";
                    }
                    else
                    {
                        gbIncrease.Text = "Monthly Rate Percent Increase";
                    }

                    if (this.pvtDataSet != null)
                    {
                        Load_PayCategories();
                    }
                }
            }
        }

        private void dgvTimePeriodDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnTimePeriodDataGridViewLoaded == true)
            {
                if (pvtDataSet != null)
                {
                    if (pvtintTimePeriodDataGridViewRowIndex != e.RowIndex)
                    {
                        pvtintTimePeriodDataGridViewRowIndex = e.RowIndex;

                        pvtintNumberDays = Convert.ToInt32(dgvTimePeriodDataGridView[1, e.RowIndex].Value);

                        pvtblnEmployeesLoaded = false;

                        if (pvtblnPercentIncreaseClicked == false)
                        {
                            pvtblnPercentIncreaseDataGridViewLoaded = false;

                            this.Clear_DataGridView(dgvPercentIncreaseDataGridView);

                            double dblIncrease = -1;

                            pvtEmployeeDataView = null;
                            pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"], "PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo, "SALARY_WAGE_INCREASE DESC", DataViewRowState.CurrentRows);

                            int intIndex = 0;

                            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                            {
                                if (intRow == 0)
                                {
                                    dgvPercentIncreaseDataGridView.Rows.Add("All");
                                }

                                if (pvtEmployeeDataView[intRow]["SALARY_WAGE_INCREASE"].ToString() == "")
                                {
                                    break;
                                }
                                else
                                {
                                    if (dblIncrease != Convert.ToDouble(pvtEmployeeDataView[intRow]["SALARY_WAGE_INCREASE"]))
                                    {
                                        dblIncrease = Convert.ToDouble(pvtEmployeeDataView[intRow]["SALARY_WAGE_INCREASE"]);

                                        dgvPercentIncreaseDataGridView.Rows.Add(dblIncrease.ToString("###0.00"));

                                        if (pvtdblPercentIncrease == dblIncrease)
                                        {
                                            intIndex = this.dgvPercentIncreaseDataGridView.Rows.Count - 1;
                                        }
                                    }
                                }
                            }

                            pvtblnPercentIncreaseDataGridViewLoaded = true;

                            if (pvtEmployeeDataView.Count > 0)
                            {
                                this.Set_DataGridView_SelectedRowIndex(dgvPercentIncreaseDataGridView, intIndex);
                            }
                        }

                        Load_Employees();
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Names_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            int intIndex = 0;

            for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
            {
                intIndex = Convert.ToInt32(this.dgvEmployeeDataGridView[6,intRow].Value);

                if (myRadioButton.Name == "rbnSurnameName")
                {
                    this.dgvEmployeeDataGridView[1, intRow].Value = pvtEmployeeDataView[intIndex]["EMPLOYEE_SURNAME"].ToString() + " / " + pvtEmployeeDataView[intIndex]["EMPLOYEE_NAME"].ToString();
                }
                else
                {
                    this.dgvEmployeeDataGridView[1, intRow].Value = pvtEmployeeDataView[intIndex]["EMPLOYEE_NAME"].ToString() + " / " + pvtEmployeeDataView[intIndex]["EMPLOYEE_SURNAME"].ToString();
                }
            }
            
            for (int intRow = 0; intRow < this.dgvEmployeeNotLinkedDataGridView.Rows.Count; intRow++)
            {
                intIndex = Convert.ToInt32(this.dgvEmployeeNotLinkedDataGridView[5, intRow].Value);

                if (myRadioButton.Name == "rbnSurnameName")
                {
                    this.dgvEmployeeNotLinkedDataGridView[1, intRow].Value = pvtEmployeeDataView[intIndex]["EMPLOYEE_SURNAME"].ToString() + " / " + pvtEmployeeDataView[intIndex]["EMPLOYEE_NAME"].ToString();
                }
                else
                {
                    this.dgvEmployeeNotLinkedDataGridView[1, intRow].Value = pvtEmployeeDataView[intIndex]["EMPLOYEE_NAME"].ToString() + " / " + pvtEmployeeDataView[intIndex]["EMPLOYEE_SURNAME"].ToString();
                }
            }
        }

        private void txtIncrease_Leave(object sender, EventArgs e)
        {
            if (this.txtIncrease.Text.Trim().Replace(".","") != "")
            {
                double dblCurrentValue = 0;
                double dblNewValue = 0;

                this.txtIncrease.Text = Convert.ToDouble(this.txtIncrease.Text.Trim()).ToString("########0.00");

                for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
                {
                    dblCurrentValue = Convert.ToDouble(this.dgvEmployeeDataGridView[2, intRow].Value);
                    dblNewValue = dblCurrentValue + Math.Round((dblCurrentValue * Convert.ToDouble(this.txtIncrease.Text)) / 100, 2);

                    this.dgvEmployeeDataGridView[3, intRow].Value = dblNewValue.ToString("########0.00");
                }

                for (int intRow = 0; intRow < this.dgvEmployeeSelectedDataGridView.Rows.Count; intRow++)
                {
                    dblCurrentValue = Convert.ToDouble(this.dgvEmployeeSelectedDataGridView[2, intRow].Value);
                    dblNewValue = dblCurrentValue + Math.Round((dblCurrentValue * Convert.ToDouble(this.txtIncrease.Text)) / 100, 2);

                    this.dgvEmployeeSelectedDataGridView[3, intRow].Value = dblNewValue.ToString("########0.00");
                }
            }
            else
            {
                this.txtIncrease.Text = "";
            }
        }

        private void txtIncrease_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                EventArgs ev = new EventArgs();

                txtIncrease_Leave(sender, ev);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true
            && this.rbnPercent.Checked == true)
            {
                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    if (this.dgvEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)].HeaderCell.Style == LockedPayrollRunDataGridViewCellStyle)
                    {
                        CustomMessageBox.Show("Record for this Employee is Locked.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        DataGridViewRow myDataGridViewRow = this.dgvEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)];

                        this.dgvEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                        this.dgvEmployeeSelectedDataGridView.Rows.Add(myDataGridViewRow);

                        this.Set_DataGridView_SelectedRowIndex(dgvEmployeeSelectedDataGridView, dgvEmployeeSelectedDataGridView.Rows.Count - 1);
                    }
                }
            }
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            btnAddAll_Click_Continue:

            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                btnAdd_Click(sender, e);

                goto btnAddAll_Click_Continue;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.dgvEmployeeSelectedDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvEmployeeSelectedDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeSelectedDataGridView)];

                    this.dgvEmployeeSelectedDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                    this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, dgvEmployeeDataGridView.Rows.Count - 1);
                }
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            btnRemoveAll_Click_Continue:

            if (this.dgvEmployeeSelectedDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(sender, e);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnAdd_Click(sender, e);
        }

        private void dgvEmployeeSelectedDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnRemove_Click(sender, e);
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvEmployeeSelectedDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void rbnTimePeriod_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            if (myRadioButton.Name == "rbnChanged")
            {
                this.lblDescription.Text = "Changed in Time Period";
            }
            else
            {
                this.lblDescription.Text = "Not Changed in Time Period";
            }

            this.Set_DataGridView_SelectedRowIndex(dgvTimePeriodDataGridView, pvtintTimePeriodDataGridViewRowIndex);
        }

        private void dgvPercentIncreaseDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPercentIncreaseDataGridViewLoaded == true)
            {
                if (pvtintPercentIncreaseDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPercentIncreaseDataGridViewRowIndex = e.RowIndex;

                    if (dgvPercentIncreaseDataGridView[0, e.RowIndex].Value.ToString() == "All")
                    {
                        pvtdblPercentIncrease = -1;
                    }
                    else
                    {
                        pvtdblPercentIncrease = Convert.ToDouble(dgvPercentIncreaseDataGridView[0, e.RowIndex].Value);
                    }

                    pvtblnPercentIncreaseClicked = true;

                    if (pvtblnEmployeesLoaded == true)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvTimePeriodDataGridView, pvtintTimePeriodDataGridViewRowIndex);
                    }

                    pvtblnPercentIncreaseClicked = false;
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            this.btnUpdate.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.dgvPayCategoryDataGridView.Enabled = false;
            this.dgvTimePeriodDataGridView.Enabled = false;
            this.dgvPercentIncreaseDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            this.picPayCategoryLock.Visible = true;
            this.picPayrollTypeLock.Visible = true;

            if (this.rbnPercent.Checked == true)
            {
                this.picPercentageLock.Visible = true;
            }

            this.pictTimePeriodLock.Visible = true;

            this.rbnNotChanged.Enabled = false;
            this.rbnChanged.Enabled = false;

            this.rbnSurnameName.Enabled = false;
            this.rbnNameSurname.Enabled = false;

            this.rbnPercent.Enabled = false;
            this.rbnActual.Enabled = false;

            this.txtIncrease.Enabled = true;

            if (this.rbnPercent.Checked == true)
            {
                this.btnAdd.Enabled = true;
                this.btnAddAll.Enabled = true;
                this.btnRemove.Enabled = true;
                this.btnRemoveAll.Enabled = true;
            }
            else
            {
                this.dgvEmployeeDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
                this.dgvEmployeeDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

                dgvEmployeeDataGridView.CurrentCell = dgvEmployeeDataGridView[3, 0];
                dgvEmployeeDataGridView.Focus();

                this.grbEnterValueInfo.Visible = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);

            this.btnUpdate.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.dgvPayCategoryDataGridView.Enabled = true;
            this.dgvTimePeriodDataGridView.Enabled = true;
            this.dgvPercentIncreaseDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;

            this.picPayCategoryLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;
            this.picPercentageLock.Visible = false;
            this.pictTimePeriodLock.Visible = false;

            this.rbnNotChanged.Enabled = true;
            this.rbnChanged.Enabled = true;

            this.rbnSurnameName.Enabled = true;
            this.rbnNameSurname.Enabled = true;

            this.rbnPercent.Enabled = true;
            this.rbnActual.Enabled = true;

            this.txtIncrease.Enabled = false;
            this.txtIncrease.Text = "";

            this.btnAdd.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;

            this.dgvEmployeeDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployeeDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.grbEnterValueInfo.Visible = false;

            this.Set_DataGridView_SelectedRowIndex(dgvTimePeriodDataGridView, pvtintTimePeriodDataGridViewRowIndex);
        }

        private void dgvEmployeeNotLinkedDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int intIndex = 0;
                string strType = "P";
                string strEmployeeNos = "";
                DataSet DataSet = new System.Data.DataSet();
                DataSet.Tables.Add(pvtDataSet.Tables["Employee"].Clone());

                if (rbnPercent.Checked == true)
                {
                    if (this.txtIncrease.Text.Trim().Replace(".", "") == "")
                    {
                        this.txtIncrease.Text = "";

                        CustomMessageBox.Show("Enter Percent Increase / Decrease Value.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.txtIncrease.Focus();
                        return;
                    }
                    else
                    {
                        if (Convert.ToDouble(this.txtIncrease.Text.Trim()) == 0)
                        {
                            CustomMessageBox.Show("Percent Increase / Decrease Value Cannot be Zero.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.txtIncrease.Focus();
                            return;
                        }
                    }

                    if (dgvEmployeeSelectedDataGridView.Rows.Count == 0)
                    {
                        CustomMessageBox.Show("You need to Select at least 1 Employee.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.btnAdd.Focus();
                        return;
                    }

                    for (int intRow = 0; intRow < this.dgvEmployeeSelectedDataGridView.Rows.Count; intRow++)
                    {
                        intIndex = Convert.ToInt32(this.dgvEmployeeSelectedDataGridView[6, intRow].Value);

                        DataSet.Tables["Employee"].ImportRow(pvtEmployeeDataView[intIndex].Row);

                        if (intRow == 0)
                        {
                            strEmployeeNos = pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString();
                        }
                        else
                        {
                            strEmployeeNos += "," + pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString();
                        }
                    }
                }
                else
                {
                    strType = "A";

                    for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
                    {
                        if (this.dgvEmployeeDataGridView[3, intRow].Value != null)
                        {
                            if (this.dgvEmployeeDataGridView[3, intRow].Value.ToString() != "")
                            {
                                intIndex = Convert.ToInt32(this.dgvEmployeeDataGridView[6, intRow].Value);

                                pvtEmployeeDataView[intIndex]["HOURLY_RATE"] = Convert.ToDouble(this.dgvEmployeeDataGridView[3, intRow].Value);
                                pvtEmployeeDataView[intIndex]["SALARY_WAGE_INCREASE"] = Convert.ToDouble(this.dgvEmployeeDataGridView[4, intRow].Value);

                                DataSet.Tables["Employee"].ImportRow(pvtEmployeeDataView[intIndex].Row);

                                if (intRow == 0)
                                {
                                    strEmployeeNos = pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString();
                                }
                                else
                                {
                                    strEmployeeNos += "," + pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString();
                                }
                            }
                        }
                    }

                    if (strEmployeeNos == "")
                    {
                        CustomMessageBox.Show("Enter at least 1 Value into the 'New' Column in the 'List of Employees' Spreadsheet.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.dgvEmployeeDataGridView.Focus();
                        return;
                    }

                    this.txtIncrease.Text = "0";
                }

                pvtEmployeeDataView.RowFilter = pvtEmployeeDataView.RowFilter + " AND EMPLOYEE_NO IN (" + strEmployeeNos + ")";

                //Compress DataSet
                byte[] bytCompress = clsISUtilities.Compress_DataSet(DataSet);

                object[] objParm = new object[8];

                objParm[0] = parInt64CompanyNo;
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[3] = pvtintPayCategoryNo;
                objParm[4] = this.pvtstrPayrollType;
                objParm[5] = strType;
                objParm[6] = Convert.ToDouble(this.txtIncrease.Text);
                objParm[7] = bytCompress;

                bytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Records", objParm, true);

                DataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                //Remove Rows from DataTable
                for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                {
                    pvtEmployeeDataView[intRow].Delete();

                    intRow -= 1;
                }

                pvtDataSet.AcceptChanges();

                pvtDataSet.Merge(DataSet);

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    int intIndex = Convert.ToInt32(dgvPayCategoryDataGridView[1,e.RowIndex].Value);

                    pvtintPayCategoryNo = Convert.ToInt32(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"]);
                    
                    this.Set_DataGridView_SelectedRowIndex(dgvTimePeriodDataGridView,0);
                }
            }
        }

        private void Option_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            if (myRadioButton.Name == "rbnPercent")
            {
                lblPercentIncrease.Visible = true;
                this.dgvPercentIncreaseDataGridView.Visible = true;
                this.gbIncrease.Visible = true;

                this.btnAdd.Visible = true;
                this.btnAddAll.Visible = true;
                this.btnRemove.Visible = true;
                this.btnRemoveAll.Visible = true;

                this.lblEmployeeLinked.Visible = true;
                this.dgvEmployeeSelectedDataGridView.Visible = true;
            }
            else
            {
                lblPercentIncrease.Visible = false;
                this.dgvPercentIncreaseDataGridView.Visible = false;
                this.gbIncrease.Visible = false;


                this.btnAdd.Visible = false;
                this.btnAddAll.Visible = false;
                this.btnRemove.Visible = false;
                this.btnRemoveAll.Visible = false;

                this.lblEmployeeLinked.Visible = false;
                this.dgvEmployeeSelectedDataGridView.Visible = false;
            }

            this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView));
        }

        private void dgvEmployeeDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.Control is TextBox)
                {
                    e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);

                    e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                }
            }
        }

        private void dgvEmployeeDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEmployeeDataGridView[e.ColumnIndex, e.RowIndex].Value == null)
            {
            }
            else
            {
                if (dgvEmployeeDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Replace(".", "") == "")
                {
                    dgvEmployeeDataGridView[e.ColumnIndex, e.RowIndex].Value = "";
                }
                else
                {
                    if (Convert.ToDouble(dgvEmployeeDataGridView[e.ColumnIndex, e.RowIndex].Value) == 0)
                    {
                        dgvEmployeeDataGridView[e.ColumnIndex, e.RowIndex].Value = "";
                    }
                    else
                    {
                        double dblNewValue = Convert.ToDouble(dgvEmployeeDataGridView[e.ColumnIndex, e.RowIndex].Value);

                        dgvEmployeeDataGridView[e.ColumnIndex, e.RowIndex].Value = dblNewValue.ToString("#########0.00");

                        double dblPercent = (Math.Round(dblNewValue / Convert.ToDouble(dgvEmployeeDataGridView[e.ColumnIndex - 1, e.RowIndex].Value) - 1,2)) * 100;

                        dgvEmployeeDataGridView[e.ColumnIndex + 1, e.RowIndex].Value = dblPercent.ToString("###0.00");
                    }
                }
            }
        }

        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            if (pvtintTimerCount == 7)
            {
                this.tmrTimer.Enabled = false;
            }
            else
            {
                pvtintTimerCount += 1;

                switch (pvtintTimerCount)
                {
                    case 1:

                        this.picBackupInfo.Visible = false;
                       
                        break;

                    case 2:

                        this.picBackupInfo.Visible = true;

                        break;

                    case 3:

                        this.picBackupInfo.Visible = false;

                        break;

                    case 4:

                        this.picBackupInfo.Visible = true;

                        break;

                    case 5:

                        this.picBackupInfo.Visible = false;

                        break;

                    case 6:

                        this.picBackupInfo.Visible = true;

                        break;
                }
            }
        }

        private void picBackupInfo_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("MenuClick", "backupDatabaseToolStripMenuItem");

            Timer TimerMenuClick = (Timer)AppDomain.CurrentDomain.GetData("TimerMenuClick");

            TimerMenuClick.Enabled = true;
        }
    }
}
