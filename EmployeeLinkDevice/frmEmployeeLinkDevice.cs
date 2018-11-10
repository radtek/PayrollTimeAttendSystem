using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InteractPayrollClient
{
    public partial class frmEmployeeLinkDevice : Form
    {
        clsISClientUtilities clsISClientUtilities;

        ToolStripMenuItem miLinkedMenuItem;

        private DataSet pvtDataSet;

        private Int64 pvtint64CompanyNo = -1;
        private int pvtintDeviceNo = -1;

        private DataView pvtEmployeeDataView;
        private DataView pvtPayCategoryDataView;
        private DataView pvtDepartmentDataView;
        private DataView pvtGroupDataView;

        private int pvtintDeviceDataGridViewRowIndex = -1;
        private int pvtintCompanyDataGridViewRowIndex = -1;

        private bool pvtblnCompanyDataGridViewLoaded = false;
        private bool pvtblnDeviceDataGridViewLoaded = false;

        public frmEmployeeLinkDevice()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;
                this.grbInfo.Height += 114;

                this.dgvEmployeeDataGridView.Height += 38;
                this.dgvEmployeeChosenDataGridView.Height += 38;
                this.btnAdd.Top += 29;
                this.btnAddAll.Top += 29;
                this.btnRemove.Top += 29;
                this.btnRemoveAll.Top += 29;

                this.lblCostCentre.Top += 38;
                this.lblLinkedCostCentre.Top += 38;
                this.dgvPayCategoryDataGridView.Top += 38;
                this.dgvPayCategoryChosenDataGridView.Top += 38;
                this.dgvPayCategoryDataGridView.Height += 38;
                this.dgvPayCategoryChosenDataGridView.Height += 38;
                this.btnPayCategoryAdd.Top += 67;
                this.btnPayCategoryRemove.Top += 67;

                this.lblDepartment.Top += 78;
                this.lblLinkedDepartment.Top += 78;
                this.dgvDepartmentDataGridView.Top += 78;
                this.dgvDepartmentChosenDataGridView.Top += 78;
                this.dgvDepartmentDataGridView.Height += 19;
                this.dgvDepartmentChosenDataGridView.Height += 19;
                this.btnDepartmentAdd.Top += 87;
                this.btnDepartmentRemove.Top += 87;

                this.lblLinkedGroup.Top += 95;
                this.lblGroup.Top += 95;
                this.dgvGroupChosenDataGridView.Top += 95;
                this.dgvGroupDataGridView.Top += 95;
                this.dgvGroupChosenDataGridView.Height += 19;
                this.dgvGroupDataGridView.Height += 19;
                this.btnGroupAdd.Top += 114;
                this.btnGroupRemove.Top += 114;
            }
        }

        private void frmEmployeeLinkDevice_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISClientUtilities = new clsISClientUtilities(this, "busEmployeeLinkDevice");

                this.lblCompany.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblClockingDevice.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblLinkedEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblLinkedCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                this.lblDepartment.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblLinkedDepartment.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                this.lblGroup.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblLinkedGroup.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

#if(DEBUG)
                int intWidth = this.dgvDeviceDataGridView.RowHeadersWidth;

                if (this.dgvDeviceDataGridView.ScrollBars == ScrollBars.Vertical
                    | this.dgvDeviceDataGridView.ScrollBars == ScrollBars.Both)
                {
                    intWidth += 19;
                }

                for (int intCol = 0; intCol < this.dgvDeviceDataGridView.ColumnCount; intCol++)
                {
                    if (this.dgvDeviceDataGridView.Columns[intCol].Visible == true)
                    {
                        intWidth += this.dgvDeviceDataGridView.Columns[intCol].Width;
                    }
                }

                if (intWidth != this.dgvDeviceDataGridView.Width)
                {
                    System.Windows.Forms.MessageBox.Show("Width should be " + intWidth.ToString());
                }

                int intHeight = this.dgvDeviceDataGridView.ColumnHeadersHeight + 2;
                int intNewHeight = this.dgvDeviceDataGridView.RowTemplate.Height / 2;

                for (int intRow = 0; intRow < 200; intRow++)
                {
                    intHeight += this.dgvDeviceDataGridView.RowTemplate.Height;

                    if (intHeight == this.dgvDeviceDataGridView.Height)
                    {
                        break;
                    }
                    else
                    {
                        if (intHeight > this.dgvDeviceDataGridView.Height)
                        {
                            System.Windows.Forms.MessageBox.Show("Height should be " + intHeight.ToString());
                            break;
                        }
                        else
                        {

                            if (intHeight + intNewHeight > this.dgvDeviceDataGridView.Height)
                            {
                                System.Windows.Forms.MessageBox.Show("Height should be " + intHeight.ToString());
                                break;
                            }
                        }
                    }
                }

                intWidth = this.dgvDepartmentDataGridView.RowHeadersWidth;

                if (this.dgvDepartmentDataGridView.ScrollBars == ScrollBars.Vertical
                    | this.dgvDepartmentDataGridView.ScrollBars == ScrollBars.Both)
                {
                    intWidth += 19;
                }

                for (int intCol = 0; intCol < this.dgvDepartmentDataGridView.ColumnCount; intCol++)
                {
                    if (this.dgvDepartmentDataGridView.Columns[intCol].Visible == true)
                    {
                        intWidth += this.dgvDepartmentDataGridView.Columns[intCol].Width;
                    }
                }

                if (intWidth != this.dgvDepartmentDataGridView.Width)
                {
                    System.Windows.Forms.MessageBox.Show("Employee Width should be " + intWidth.ToString());
                }

                intHeight = this.dgvDepartmentDataGridView.ColumnHeadersHeight + 2;
                intNewHeight = this.dgvDepartmentDataGridView.RowTemplate.Height / 2;

                for (int intRow = 0; intRow < 200; intRow++)
                {
                    intHeight += this.dgvDepartmentDataGridView.RowTemplate.Height;

                    if (intHeight == this.dgvDepartmentDataGridView.Height)
                    {
                        break;
                    }
                    else
                    {
                        if (intHeight > this.dgvDepartmentDataGridView.Height)
                        {
                            System.Windows.Forms.MessageBox.Show("Employee Height should be " + intHeight.ToString());
                            break;
                        }
                        else
                        {

                            if (intHeight + intNewHeight > this.dgvDepartmentDataGridView.Height)
                            {
                                System.Windows.Forms.MessageBox.Show("Employee Height should be " + intHeight.ToString());
                                break;
                            }
                        }
                    }
                }
#endif      
                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Form_Records", objParm,false);
                pvtDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                string strDeviceType = "";

                for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["Device"].Rows.Count; intRowCount++)
                {
                    if (pvtDataSet.Tables["Device"].Rows[intRowCount]["DEVICE_USAGE"].ToString() == "T")
                    {
                        strDeviceType = "Time & Attendance";
                    }
                    else
                    {
                        if (pvtDataSet.Tables["Device"].Rows[intRowCount]["DEVICE_USAGE"].ToString() == "A")
                        {
                            strDeviceType = "Access";
                        }
                        else
                        {
                            strDeviceType = "Access / Time & Attendance";
                        }
                    }

                    this.dgvDeviceDataGridView.Rows.Add(pvtDataSet.Tables["Device"].Rows[intRowCount]["DEVICE_DESC"].ToString(),strDeviceType,pvtDataSet.Tables["Device"].Rows[intRowCount]["DEVICE_NO"].ToString(),intRowCount.ToString());
                }

                pvtblnDeviceDataGridViewLoaded = true;

                if (pvtDataSet.Tables["Device"].Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvDeviceDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDeviceDataGridView));

                    Load_CurrentForm_Records();
                }
                else
                {
                    this.btnUpdate.Enabled = false;
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
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

            return intReturnIndex;
        }

        public void Set_DataGridView_SelectedRowIndex(DataGridView myDataGridView, int intRow)
        {
            //Fires DataGridView RowEnter Function
            if (myDataGridView.CurrentCell.RowIndex == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvCompanyDataGridView":

                        pvtintCompanyDataGridViewRowIndex = -1;
                        this.dgvCompanyDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDeviceDataGridView":

                        pvtintDeviceDataGridViewRowIndex = -1;
                        this.dgvDeviceDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        System.Windows.Forms.MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void Load_CurrentForm_Records()
        {
            int intCompanyRow = 0;

            if (pvtblnCompanyDataGridViewLoaded == true)
            {
                if (this.dgvCompanyDataGridView.Rows.Count > 0)
                {
                    intCompanyRow = this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView);
                }
            }

            pvtblnCompanyDataGridViewLoaded = false;

            this.Clear_DataGridView(dgvCompanyDataGridView);

            for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["Company"].Rows.Count; intRowCount++)
            {
                this.dgvCompanyDataGridView.Rows.Add(pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_DESC"].ToString(), intRowCount.ToString());
            }

            pvtblnCompanyDataGridViewLoaded = true;

            if (this.dgvCompanyDataGridView.Rows.Count > 0)
            {
                Set_Form_For_Read();

                this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
            }
            else
            {
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text = this.Text + " - Update";

            Set_Form_For_Edit();

            this.dgvCompanyDataGridView.Focus();
        }

        private void Set_Form_For_Edit()
        {
            this.dgvCompanyDataGridView.Enabled = false;
            this.dgvDeviceDataGridView.Enabled = false;

            this.picCompanyLock.Visible = true;
            this.picDeviceLock.Visible = true;

            this.rbnSurnameName.Enabled = false;
            this.rbnNameSurname.Enabled = false;

            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;
            this.btnPayCategoryAdd.Enabled = true;
            this.btnPayCategoryRemove.Enabled = true;
            this.btnDepartmentAdd.Enabled = true;
            this.btnDepartmentRemove.Enabled = true;
            this.btnGroupAdd.Enabled = true;
            this.btnGroupRemove.Enabled = true;

            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
        }

        private void Set_Form_For_Read()
        {
            this.dgvCompanyDataGridView.Enabled = true;
            this.dgvDeviceDataGridView.Enabled = true;

            this.picCompanyLock.Visible = false;
            this.picDeviceLock.Visible = false;

            this.rbnSurnameName.Enabled = true;
            this.rbnNameSurname.Enabled = true;

            if (this.dgvDeviceDataGridView.Rows.Count > 1)
            {
                this.btnUpdate.Enabled = true;

                if (this.dgvEmployeeChosenDataGridView.Rows.Count == 0
                    & this.dgvPayCategoryChosenDataGridView.Rows.Count == 0
                    & this.dgvDepartmentChosenDataGridView.Rows.Count == 0
                    & this.dgvGroupChosenDataGridView.Rows.Count == 0)
                {
                    this.btnDelete.Enabled = false;
                }
                else
                {
                    this.btnDelete.Enabled = true;
                }
            }
            else
            {
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }

            this.btnAdd.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;
            this.btnPayCategoryAdd.Enabled = false;
            this.btnPayCategoryRemove.Enabled = false;
            this.btnDepartmentAdd.Enabled = false;
            this.btnDepartmentRemove.Enabled = false;
            this.btnGroupAdd.Enabled = false;
            this.btnGroupRemove.Enabled = false;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            //Remove '- Update' from Header
            if (this.Text.LastIndexOf(" - ") != -1)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf(" - "));
            }

            Set_Form_For_Read();

            Load_CurrentForm_Records();
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                //Remove Chosen Records (On Database)
                DataView EmployeeChosenDataView = new DataView(pvtDataSet.Tables["EmployeeChosen"], "DEVICE_NO = " + this.pvtintDeviceNo + " AND COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < EmployeeChosenDataView.Count; intRow++)
                {
                    EmployeeChosenDataView[intRow].Delete();
                    intRow -= 1;
                }

                DataView PayCategoryChosenDataView = new DataView(pvtDataSet.Tables["PayCategoryChosen"], "DEVICE_NO = " + this.pvtintDeviceNo + " AND COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < PayCategoryChosenDataView.Count; intRow++)
                {
                    PayCategoryChosenDataView[intRow].Delete();
                    intRow -= 1;
                }

                DataView DepartmentChosenDataView = new DataView(pvtDataSet.Tables["DepartmentChosen"], "DEVICE_NO = " + this.pvtintDeviceNo + " AND COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < DepartmentChosenDataView.Count; intRow++)
                {
                    DepartmentChosenDataView[intRow].Delete();
                    intRow -= 1;
                }

                DataView GroupChosenDataView = new DataView(pvtDataSet.Tables["GroupChosen"], "DEVICE_NO = " + this.pvtintDeviceNo + " AND COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < GroupChosenDataView.Count; intRow++)
                {
                    GroupChosenDataView[intRow].Delete();
                    intRow -= 1;
                }

                pvtDataSet.AcceptChanges();
               
                DataSet TempDataSet = new DataSet();
               
                TempDataSet.Tables.Add(pvtDataSet.Tables["EmployeeChosen"].Clone());

                for (int intRowCount = 0; intRowCount < this.dgvEmployeeChosenDataGridView.Rows.Count; intRowCount++)
                {
                    DataRowView drvDataRowView = EmployeeChosenDataView.AddNew();

                    drvDataRowView["DEVICE_NO"] = pvtintDeviceNo;
                    drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                    drvDataRowView["EMPLOYEE_NO"] = Convert.ToInt32(this.dgvEmployeeChosenDataGridView[4,intRowCount].Value);
                    drvDataRowView["PAY_CATEGORY_NO"] = Convert.ToInt32(this.dgvEmployeeChosenDataGridView[5,intRowCount].Value);
                    drvDataRowView["PAY_CATEGORY_TYPE"] = this.dgvEmployeeChosenDataGridView[3,intRowCount].Value.ToString();
                  
                    drvDataRowView.EndEdit();

                    TempDataSet.Tables["EmployeeChosen"].ImportRow(drvDataRowView.Row);
                }

                TempDataSet.Tables.Add(pvtDataSet.Tables["PayCategoryChosen"].Clone());

                for (int intRowCount = 0; intRowCount < this.dgvPayCategoryChosenDataGridView.Rows.Count; intRowCount++)
                {
                    DataRowView drvDataRowView = PayCategoryChosenDataView.AddNew();

                    drvDataRowView["DEVICE_NO"] = pvtintDeviceNo;
                    drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                    drvDataRowView["PAY_CATEGORY_NO"] = Convert.ToInt32(this.dgvPayCategoryChosenDataGridView[2,intRowCount].Value);
                    drvDataRowView["PAY_CATEGORY_TYPE"] = this.dgvPayCategoryChosenDataGridView[1,intRowCount].Value.ToString();

                    drvDataRowView.EndEdit();

                    TempDataSet.Tables["PayCategoryChosen"].ImportRow(drvDataRowView.Row);
                }

                TempDataSet.Tables.Add(pvtDataSet.Tables["DepartmentChosen"].Clone());

                for (int intRowCount = 0; intRowCount < this.dgvDepartmentChosenDataGridView.Rows.Count; intRowCount++)
                {
                    DataRowView drvDataRowView = DepartmentChosenDataView.AddNew();

                    drvDataRowView["DEVICE_NO"] = pvtintDeviceNo;
                    drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                    drvDataRowView["PAY_CATEGORY_NO"] = this.dgvDepartmentChosenDataGridView[3,intRowCount].Value.ToString();
                    drvDataRowView["PAY_CATEGORY_TYPE"] = this.dgvDepartmentChosenDataGridView[2,intRowCount].Value.ToString();
                    drvDataRowView["DEPARTMENT_NO"] = Convert.ToInt32(this.dgvDepartmentChosenDataGridView[4,intRowCount].Value);

                    drvDataRowView.EndEdit();

                    TempDataSet.Tables["DepartmentChosen"].ImportRow(drvDataRowView.Row);
                }

                TempDataSet.Tables.Add(pvtDataSet.Tables["GroupChosen"].Clone());

                for (int intRowCount = 0; intRowCount < this.dgvGroupChosenDataGridView.Rows.Count; intRowCount++)
                {
                    DataRowView drvDataRowView = GroupChosenDataView.AddNew();

                    drvDataRowView["DEVICE_NO"] = pvtintDeviceNo;
                    drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                    drvDataRowView["GROUP_NO"] = Convert.ToInt32(this.dgvGroupChosenDataGridView[1,intRowCount].Value);

                    drvDataRowView.EndEdit();

                    TempDataSet.Tables["GroupChosen"].ImportRow(drvDataRowView.Row);
                }

                //Compress DataSet
                byte[] pvtbytCompress = clsISClientUtilities.Compress_DataSet(TempDataSet);

                object[] objParm = new object[3];
                objParm[0] = pvtint64CompanyNo;
                objParm[1] = pvtintDeviceNo;
                objParm[2] = pvtbytCompress;

                clsISClientUtilities.DynamicFunction("Update_Records", objParm,true);

                pvtDataSet.AcceptChanges();

                btnCancel_Click(sender, e);

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeeChosenDataGridView.Rows[this.dgvEmployeeChosenDataGridView.CurrentRow.Index];

                this.dgvEmployeeChosenDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvEmployeeDataGridView.CurrentCell = this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1];
            }
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeeDataGridView.Rows[this.dgvEmployeeDataGridView.CurrentRow.Index];

                this.dgvEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeChosenDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvEmployeeChosenDataGridView.CurrentCell = this.dgvEmployeeChosenDataGridView[0, this.dgvEmployeeChosenDataGridView.Rows.Count - 1];
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

            if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void btnGroupAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvGroupDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvGroupDataGridView.Rows[this.dgvGroupDataGridView.CurrentRow.Index];

                this.dgvGroupDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvGroupChosenDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvGroupChosenDataGridView.CurrentCell = this.dgvGroupChosenDataGridView[0, this.dgvGroupChosenDataGridView.Rows.Count - 1];
            }
        }

        private void btnGroupRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvGroupChosenDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvGroupChosenDataGridView.Rows[this.dgvGroupChosenDataGridView.CurrentRow.Index];

                this.dgvGroupChosenDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvGroupDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvGroupDataGridView.CurrentCell = this.dgvGroupDataGridView[0, this.dgvGroupDataGridView.Rows.Count - 1];
            }
        }

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomClientMessageBox.Show("Delete All Employees / Cost Centres / Departments / Groups Linked to Device '" + this.dgvDeviceDataGridView[0,this.dgvDeviceDataGridView.CurrentRow.Index].Value.ToString() + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    DataView EmployeeChosenDataView = new DataView(pvtDataSet.Tables["EmployeeChosen"], "DEVICE_NO = " + this.pvtintDeviceNo + " AND COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < EmployeeChosenDataView.Count; intRow++)
                    {
                        EmployeeChosenDataView[intRow].Delete();
                        intRow -= 1;
                    }

                    DataView PayCategoryChosenDataView = new DataView(pvtDataSet.Tables["PayCategoryChosen"], "DEVICE_NO = " + this.pvtintDeviceNo + " AND COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < PayCategoryChosenDataView.Count; intRow++)
                    {
                        PayCategoryChosenDataView[intRow].Delete();
                        intRow -= 1;
                    }

                    DataView DepartmentChosenDataView = new DataView(pvtDataSet.Tables["DepartmentChosen"], "DEVICE_NO = " + this.pvtintDeviceNo + " AND COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < DepartmentChosenDataView.Count; intRow++)
                    {
                        DepartmentChosenDataView[intRow].Delete();
                        intRow -= 1;
                    }

                    DataView GroupChosenDataView = new DataView(pvtDataSet.Tables["GroupChosen"], "DEVICE_NO = " + this.pvtintDeviceNo + " AND COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < GroupChosenDataView.Count; intRow++)
                    {
                        GroupChosenDataView[intRow].Delete();
                        intRow -= 1;
                    }

                    pvtDataSet.AcceptChanges();

                    object[] objParm = new object[2];
                    objParm[0] = pvtint64CompanyNo;
                    objParm[1] = pvtintDeviceNo;

                    clsISClientUtilities.DynamicFunction("Delete_Records", objParm,true);

                    this.Load_CurrentForm_Records();
                }
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
            }
        }
        private void btnPayCategoryAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvPayCategoryDataGridView.Rows[this.dgvPayCategoryDataGridView.CurrentRow.Index];

                this.dgvPayCategoryDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvPayCategoryChosenDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvPayCategoryChosenDataGridView.CurrentCell = this.dgvPayCategoryChosenDataGridView[0, this.dgvPayCategoryChosenDataGridView.Rows.Count - 1];
            }
        }

        private void btnPayCategoryRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvPayCategoryChosenDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvPayCategoryChosenDataGridView.Rows[this.dgvPayCategoryChosenDataGridView.CurrentRow.Index];

                this.dgvPayCategoryChosenDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvPayCategoryDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvPayCategoryDataGridView.CurrentCell = this.dgvPayCategoryDataGridView[0, this.dgvPayCategoryDataGridView.Rows.Count - 1];
            }
        }

        private void btnDepartmentAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvDepartmentDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvDepartmentDataGridView.Rows[this.dgvDepartmentDataGridView.CurrentRow.Index];

                this.dgvDepartmentDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvDepartmentChosenDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvDepartmentChosenDataGridView.CurrentCell = this.dgvDepartmentChosenDataGridView[0, this.dgvDepartmentChosenDataGridView.Rows.Count - 1];
            }
        }

        private void btnDepartmentRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvDepartmentChosenDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvDepartmentChosenDataGridView.Rows[this.dgvDepartmentChosenDataGridView.CurrentRow.Index];

                this.dgvDepartmentChosenDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvDepartmentDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvDepartmentDataGridView.CurrentCell = this.dgvDepartmentDataGridView[0, this.dgvDepartmentDataGridView.Rows.Count - 1];
            }
        }

        private void frmEmployeeLinkDevice_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }

        private void dgvCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (pvtblnCompanyDataGridViewLoaded == true)
                {
                    if (pvtintCompanyDataGridViewRowIndex != e.RowIndex)
                    {
                        pvtintCompanyDataGridViewRowIndex = e.RowIndex;

                        int intCompanyIndex = Convert.ToInt32(dgvCompanyDataGridView[1, e.RowIndex].Value);

                        pvtint64CompanyNo = Convert.ToInt64(pvtDataSet.Tables["Company"].Rows[intCompanyIndex]["COMPANY_NO"]);

                        DataView dtDataView = new DataView(pvtDataSet.Tables["Employee"], "COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                        if (dtDataView.Count == 0)
                        {
                            object[] objParm = new object[1];
                            objParm[0] = pvtint64CompanyNo;

                            byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Company_Records", objParm, false);
                            DataSet TempDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                            pvtDataSet.Merge(TempDataSet);
                        }

                        if (dtDataView.Count == 0)
                        {
                            this.btnUpdate.Enabled = false;
                            this.btnDelete.Enabled = false;
                        }
                        else
                        {
                            this.btnUpdate.Enabled = true;
                            this.btnDelete.Enabled = true;
                        }

                        this.Clear_DataGridView(dgvPayCategoryDataGridView);
                        this.Clear_DataGridView(dgvPayCategoryChosenDataGridView);

                        this.Clear_DataGridView(dgvDepartmentDataGridView);
                        this.Clear_DataGridView(dgvDepartmentChosenDataGridView);

                        this.Clear_DataGridView(dgvGroupDataGridView);
                        this.Clear_DataGridView(dgvGroupChosenDataGridView);

                        //Read Only
                        if (this.btnSave.Enabled == false)
                        {
                            Load_Employees();

                            pvtPayCategoryDataView = null;
                            pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"], "COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                            for (int intRowCount = 0; intRowCount < pvtPayCategoryDataView.Count; intRowCount++)
                            {
                                DataView DataView = new DataView(pvtDataSet.Tables["PayCategoryChosen"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString() + "'", "", DataViewRowState.CurrentRows);

                                if (DataView.Count == 0)
                                {
                                    this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                                                                             pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                                                                             pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_NO"].ToString());
                                }
                                else
                                {
                                    this.dgvPayCategoryChosenDataGridView.Rows.Add(pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                                                                             pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                                                                             pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_NO"].ToString());
                                }
                            }

                            pvtDepartmentDataView = null;
                            pvtDepartmentDataView = new DataView(pvtDataSet.Tables["Department"], "COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                            for (int intRowCount = 0; intRowCount < pvtDepartmentDataView.Count; intRowCount++)
                            {
                                DataView DataView = new DataView(pvtDataSet.Tables["DepartmentChosen"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + pvtDepartmentDataView[intRowCount]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtDepartmentDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString() + "' AND DEPARTMENT_NO = " + pvtDepartmentDataView[intRowCount]["DEPARTMENT_NO"].ToString(), "", DataViewRowState.CurrentRows);

                                if (DataView.Count == 0)
                                {
                                    dgvDepartmentDataGridView.Rows.Add(pvtDepartmentDataView[intRowCount]["DEPARTMENT_DESC"].ToString(),
                                                                       pvtDepartmentDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                                                                       pvtDepartmentDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                                                                       pvtDepartmentDataView[intRowCount]["PAY_CATEGORY_NO"].ToString(),
                                                                       pvtDepartmentDataView[intRowCount]["DEPARTMENT_NO"].ToString());
                                }
                                else
                                {
                                    dgvDepartmentChosenDataGridView.Rows.Add(pvtDepartmentDataView[intRowCount]["DEPARTMENT_DESC"].ToString(),
                                                                             pvtDepartmentDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                                                                             pvtDepartmentDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                                                                             pvtDepartmentDataView[intRowCount]["PAY_CATEGORY_NO"].ToString(),
                                                                             pvtDepartmentDataView[intRowCount]["DEPARTMENT_NO"].ToString());
                                }
                            }

                            pvtGroupDataView = null;
                            pvtGroupDataView = new DataView(pvtDataSet.Tables["Group"], "COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                            for (int intRowCount = 0; intRowCount < pvtGroupDataView.Count; intRowCount++)
                            {
                                DataView DataView = new DataView(pvtDataSet.Tables["GroupChosen"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + pvtint64CompanyNo + " AND GROUP_NO = " + pvtGroupDataView[intRowCount]["GROUP_NO"].ToString(), "", DataViewRowState.CurrentRows);

                                if (DataView.Count == 0)
                                {
                                    this.dgvGroupDataGridView.Rows.Add(pvtGroupDataView[intRowCount]["GROUP_DESC"].ToString(),
                                                                       pvtGroupDataView[intRowCount]["GROUP_NO"].ToString());
                                }
                                else
                                {
                                    this.dgvGroupChosenDataGridView.Rows.Add(pvtGroupDataView[intRowCount]["GROUP_DESC"].ToString(),
                                                                       pvtGroupDataView[intRowCount]["GROUP_NO"].ToString());
                                }
                            }

                            if (this.dgvEmployeeChosenDataGridView.Rows.Count == 0
                            & this.dgvPayCategoryChosenDataGridView.Rows.Count == 0
                            & this.dgvDepartmentChosenDataGridView.Rows.Count == 0
                            & this.dgvGroupChosenDataGridView.Rows.Count == 0)
                            {
                                this.btnDelete.Enabled = false;
                            }
                            else
                            {
                                this.btnDelete.Enabled = true;
                            }
                        }
                    }
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void Load_Employees()
        {
            this.Clear_DataGridView(dgvEmployeeDataGridView);
            this.Clear_DataGridView(dgvEmployeeChosenDataGridView);

            string strNames = "";

            pvtEmployeeDataView = null;
            pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"], "COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

            for (int intRowCount = 0; intRowCount < pvtEmployeeDataView.Count; intRowCount++)
            {
                DataView DataView = new DataView(pvtDataSet.Tables["EmployeeChosen"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[intRowCount]["EMPLOYEE_NO"].ToString() + " AND PAY_CATEGORY_NO = " + pvtEmployeeDataView[intRowCount]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtEmployeeDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString() + "'", "", DataViewRowState.CurrentRows);

                if (this.rbnSurnameName.Checked == true)
                {
                    strNames = pvtEmployeeDataView[intRowCount]["EMPLOYEE_SURNAME"].ToString() + " / " + pvtEmployeeDataView[intRowCount]["EMPLOYEE_NAME"].ToString();
                }
                else
                {
                    strNames = pvtEmployeeDataView[intRowCount]["EMPLOYEE_NAME"].ToString() + " / " + pvtEmployeeDataView[intRowCount]["EMPLOYEE_SURNAME"].ToString();
                }

                if (DataView.Count == 0)
                {
                    dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intRowCount]["EMPLOYEE_CODE"].ToString(),
                                                     strNames,
                                                     pvtEmployeeDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                                                     pvtEmployeeDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                                                     pvtEmployeeDataView[intRowCount]["EMPLOYEE_NO"].ToString(),
                                                     pvtEmployeeDataView[intRowCount]["PAY_CATEGORY_NO"].ToString());
                }
                else
                {
                    dgvEmployeeChosenDataGridView.Rows.Add(pvtEmployeeDataView[intRowCount]["EMPLOYEE_CODE"].ToString(),
                                                     strNames,
                                                     pvtEmployeeDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                                                     pvtEmployeeDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                                                     pvtEmployeeDataView[intRowCount]["EMPLOYEE_NO"].ToString(),
                                                     pvtEmployeeDataView[intRowCount]["PAY_CATEGORY_NO"].ToString());
                }
            }
        }

        private void dgvDeviceDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 2)
            {
                if (double.Parse(e.CellValue1.ToString()) > double.Parse(e.CellValue2.ToString()))
                {
                    e.SortResult = 1;
                }
                else if (double.Parse(e.CellValue1.ToString()) < double.Parse(e.CellValue2.ToString()))
                {
                    e.SortResult = -1;
                }
                else
                {
                    e.SortResult = 0;
                }

                e.Handled = true;
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

        private void dgvDeviceDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnDeviceDataGridViewLoaded == true)
            {
                if (pvtintDeviceDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintDeviceDataGridViewRowIndex = e.RowIndex;

                    pvtintDeviceNo = Convert.ToInt32(dgvDeviceDataGridView[2, e.RowIndex].Value);

                    if (this.dgvCompanyDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
                    }
                }
            }
        }

        private void RadioButton_Names_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            if (myRadioButton.Name == "rbnSurnameName")
            {
                this.dgvEmployeeDataGridView.Columns[1].HeaderText = "Surname / Name";
                this.dgvEmployeeChosenDataGridView.Columns[1].HeaderText = "Surname / Name";
            }
            else
            {
                this.dgvEmployeeDataGridView.Columns[1].HeaderText = "Name / Surname";
                this.dgvEmployeeChosenDataGridView.Columns[1].HeaderText = "Name / Surname";
            }

            //Not Empty
            if (pvtint64CompanyNo != -1)
            {
                Load_Employees();
            }
        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnAdd_Click(sender, e);
            }
        }

        private void dgvEmployeeChosenDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnRemove_Click(sender, e);
            }
        }

        private void dgvPayCategoryDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnPayCategoryAdd_Click(sender, e);
            }
        }

        private void dgvPayCategoryChosenDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnPayCategoryRemove_Click(sender, e);
            }
        }

        private void dgvDepartmentDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnDepartmentAdd_Click(sender, e);
            }
        }

        private void dgvDepartmentChosenDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnDepartmentRemove_Click(sender, e);
            }
        }

        private void dgvGroupDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnGroupAdd_Click(sender, e);
            }
        }

        private void dgvGroupChosenDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnGroupRemove_Click(sender, e);
            }
        }
    }
}
