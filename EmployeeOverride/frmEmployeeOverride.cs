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
    public partial class frmEmployeeOverride : Form
    {
        InteractPayrollClient.clsISClientUtilities clsISClientUtilities;

        ToolStripMenuItem miLinkedMenuItem;

        private Int64 pvtint64CompanyNo;
        private Int64 pvtint64EmployeeNo;
        private string pvtstrPayCategoryType;

        private DataSet pvtDataSet;
        private DataView pvtDataViewEmployeeLink;
        private DataView pvtDataViewPeopleLink;
        private DataView pvtDataViewPayCategory;
        private DataView pvtDataViewPayCategoryLink;
        private DataView pvtDataViewDepartment;
        private DataView pvtDataViewDepartmentLink;
        private DataView pvtDataViewUser;
        private DataView pvtDataViewUserLink;

        private int pvtintCompanyDataGridViewRowIndex = -1;
        private int pvtintPersonLinkedDataGridViewRowIndex = -1;

        private bool pvtblnCompanyDataGridViewLoaded = false;
        private bool pvtblnPersonLinkedDataGridViewLoaded = false;

        DataGridViewCellStyle EmployeeOverrideDataGridViewCellStyle;

        public frmEmployeeOverride()
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
                this.lblSelectedCostCentre.Top += 38;
                this.dgvPayCategoryDataGridView.Top += 38;
                this.dgvPayCategoryChosenDataGridView.Top += 38;
                this.dgvPayCategoryDataGridView.Height += 38;
                this.dgvPayCategoryChosenDataGridView.Height += 38;
                this.btnPayCategoryAdd.Top += 57;
                this.btnPayCategoryRemove.Top += 57;

                this.lblDepartment.Top += 78;
                this.lblSelectedDepartment.Top += 78;
                this.dgvDepartmentDataGridView.Top += 78;
                this.dgvDepartmentChosenDataGridView.Top += 78;
                this.dgvDepartmentDataGridView.Height += 19;
                this.dgvDepartmentChosenDataGridView.Height += 19;
                this.btnDepartmentAdd.Top += 87;
                this.btnDepartmentRemove.Top += 87;

                this.lblSelectedUser.Top += 95;
                this.lblUser.Top += 95;
                this.dgvUserChosenDataGridView.Top += 95;
                this.dgvUserDataGridView.Top += 95;
                this.dgvUserChosenDataGridView.Height += 19;
                this.dgvUserDataGridView.Height += 19;
                this.btnUserAdd.Top += 114;
                this.btnUserRemove.Top += 114;
            }
        }

        private void frmEmployeeOverride_Load(object sender, EventArgs e)
        {
            EmployeeOverrideDataGridViewCellStyle = new DataGridViewCellStyle();
            EmployeeOverrideDataGridViewCellStyle.BackColor = Color.SeaGreen;
            EmployeeOverrideDataGridViewCellStyle.SelectionBackColor = Color.SeaGreen;
 
            pvtDataSet = new DataSet();

            miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

            clsISClientUtilities = new clsISClientUtilities(this, "busEmployeeOverride");

            this.lblCompany.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
            this.lblPeopleLink.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

            this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
            this.lblSelectedEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

            this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
            this.lblSelectedCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

            this.lblDepartment.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
            this.lblSelectedDepartment.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

            this.lblUser.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
            this.lblSelectedUser.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

#if(DEBUG)
            int intWidth = 0;
            int intHeight = 0;
            int intNewHeight = 0;

            DataGridView myDataGridView;

            foreach (Control myControl in this.Controls)
            {
                if (myControl is DataGridView)
                {
                    myDataGridView = null;
                    myDataGridView = (DataGridView)myControl;

                    intWidth = myDataGridView.RowHeadersWidth;

                    if (myDataGridView.ScrollBars == ScrollBars.Vertical
                        | myDataGridView.ScrollBars == ScrollBars.Both)
                    {
                        intWidth += 19;
                    }

                    for (int intCol = 0; intCol < myDataGridView.ColumnCount; intCol++)
                    {
                        if (myDataGridView.Columns[intCol].Visible == true)
                        {
                            intWidth += myDataGridView.Columns[intCol].Width;
                        }
                    }

                    if (intWidth != myDataGridView.Width)
                    {
                        System.Windows.Forms.MessageBox.Show(myDataGridView.Name + " Width should be " + intWidth.ToString());
                    }

                    intHeight = myDataGridView.ColumnHeadersHeight + 2;
                    intNewHeight = myDataGridView.RowTemplate.Height / 2;

                    for (int intRow = 0; intRow < 200; intRow++)
                    {
                        intHeight += myDataGridView.RowTemplate.Height;

                        if (intHeight == myDataGridView.Height)
                        {
                            break;
                        }
                        else
                        {
                            if (intHeight > myDataGridView.Height)
                            {
                                System.Windows.Forms.MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                break;
                            }
                            else
                            {

                                if (intHeight + intNewHeight > myDataGridView.Height)
                                {
                                    System.Windows.Forms.MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (myControl is GroupBox)
                    {
                        foreach (Control myControl1 in myControl.Controls)
                        {
                            if (myControl1 is DataGridView)
                            {
                                myDataGridView = null;
                                myDataGridView = (DataGridView)myControl1;

                                intWidth = myDataGridView.RowHeadersWidth;

                                if (myDataGridView.ScrollBars == ScrollBars.Vertical
                                    | myDataGridView.ScrollBars == ScrollBars.Both)
                                {
                                    intWidth += 19;
                                }

                                for (int intCol = 0; intCol < myDataGridView.ColumnCount; intCol++)
                                {
                                    if (myDataGridView.Columns[intCol].Visible == true)
                                    {
                                        intWidth += myDataGridView.Columns[intCol].Width;
                                    }
                                }

                                if (intWidth != myDataGridView.Width)
                                {
                                    System.Windows.Forms.MessageBox.Show(myDataGridView.Name + " Width should be " + intWidth.ToString());
                                }

                                intHeight = myDataGridView.ColumnHeadersHeight + 2;
                                intNewHeight = myDataGridView.RowTemplate.Height / 2;

                                for (int intRow = 0; intRow < 200; intRow++)
                                {
                                    intHeight += myDataGridView.RowTemplate.Height;

                                    if (intHeight == myDataGridView.Height)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (intHeight > myDataGridView.Height)
                                        {
                                            System.Windows.Forms.MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                            break;
                                        }
                                        else
                                        {

                                            if (intHeight + intNewHeight > myDataGridView.Height)
                                            {
                                                System.Windows.Forms.MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
#endif
            Load_CurrentForm_Records();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Load_CurrentForm_Records()
        {
            try
            {
                int intCompanyRow = 0;

                if (pvtblnCompanyDataGridViewLoaded == true)
                {
                    if (this.dgvCompanyDataGridView.CurrentRow.Index > -1)
                    {
                        intCompanyRow = this.dgvCompanyDataGridView.CurrentRow.Index;
                    }
                }

                pvtblnCompanyDataGridViewLoaded = false;

                this.Clear_DataGridView(dgvCompanyDataGridView);

                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Form_Records", objParm,false);
                pvtDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["Company"].Rows.Count; intRowCount++)
                {
                    this.dgvCompanyDataGridView.Rows.Add(pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_DESC"].ToString(),
                                                        pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_NO"].ToString());
                }

                pvtblnCompanyDataGridViewLoaded = true;

                if (this.dgvCompanyDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));

                    Set_Form_For_Read();
                }
                else
                {
                    this.btnUpdate.Enabled = false;
                    this.btnDelete.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
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

                    case "dgvPersonLinkedDataGridView":

                        pvtintPersonLinkedDataGridViewRowIndex = -1;
                        this.dgvPersonLinkedDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void Set_Form_For_Edit()
        {
            this.dgvCompanyDataGridView.Enabled = false;
            this.dgvPersonLinkedDataGridView.Enabled = false;

            this.picCompanyLock.Visible = true;
            this.picPeopleLinkLock.Visible = true;

            this.rbnAll.Enabled = false;
            this.rbnOverride.Enabled = false;

            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;

            this.btnPayCategoryAdd.Enabled = true;
            this.btnPayCategoryRemove.Enabled = true;

            this.btnDepartmentAdd.Enabled = true;
            this.btnDepartmentRemove.Enabled = true;

            this.btnUserAdd.Enabled = true;
            this.btnUserRemove.Enabled = true;
         
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
        }

        private void Set_Form_For_Read()
        {
            if (this.Text.LastIndexOf(" - ") != -1)
            {
                this.Text = this.Text.Substring(0, this.Text.LastIndexOf(" - "));
            }

            this.dgvCompanyDataGridView.Enabled = true;
            this.dgvPersonLinkedDataGridView.Enabled = true;

            this.rbnAll.Enabled = true;
            this.rbnOverride.Enabled = true;

            this.picCompanyLock.Visible = false;
            this.picPeopleLinkLock.Visible = false;

            if (this.dgvPersonLinkedDataGridView.Rows.Count > 0)
            {
                this.btnUpdate.Enabled = true;

                if (this.dgvEmployeeChosenDataGridView.Rows.Count == 0
                    & this.dgvPayCategoryChosenDataGridView.Rows.Count == 0
                    & this.dgvDepartmentChosenDataGridView.Rows.Count == 0
                    & this.dgvUserChosenDataGridView.Rows.Count == 0)
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

            this.btnUserAdd.Enabled = false;
            this.btnUserRemove.Enabled = false;
                           
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;
        }

        public void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";  

            this.Set_Form_For_Edit();
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

        public void btnCancel_Click(object sender, EventArgs e)
        {
            this.Set_Form_For_Read();

            if (this.dgvPersonLinkedDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPersonLinkedDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPersonLinkedDataGridView));
            }
        }

        public void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                pvtDataViewEmployeeLink = null;
                pvtDataViewEmployeeLink = new DataView(pvtDataSet.Tables["EmployeeLink"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo, "", DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtDataViewEmployeeLink.Count; intRow++)
                {
                    pvtDataViewEmployeeLink[intRow].Delete();
                    intRow -= 1;
                }

                pvtDataViewPayCategoryLink = null;
                pvtDataViewPayCategoryLink = new DataView(pvtDataSet.Tables["PayCategoryLink"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo, "", DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtDataViewPayCategoryLink.Count; intRow++)
                {
                    pvtDataViewPayCategoryLink[intRow].Delete();
                    intRow -= 1;
                }

                pvtDataViewDepartmentLink = null;
                pvtDataViewDepartmentLink = new DataView(pvtDataSet.Tables["DepartmentLink"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo, "", DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtDataViewDepartmentLink.Count; intRow++)
                {
                    pvtDataViewDepartmentLink[intRow].Delete();
                    intRow -= 1;
                }

                pvtDataViewUserLink = null;
                pvtDataViewUserLink = new DataView(pvtDataSet.Tables["UserLink"], "COMPANY_NO = " + pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo, "", DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtDataViewUserLink.Count; intRow++)
                {
                    pvtDataViewUserLink[intRow].Delete();
                    intRow -= 1;
                }

                pvtDataSet.AcceptChanges();

                DataSet TempDataSet = new DataSet();

                TempDataSet.Tables.Add(pvtDataSet.Tables["EmployeeLink"].Clone());

                for (int intRowCount = 0; intRowCount < this.dgvEmployeeChosenDataGridView.Rows.Count; intRowCount++)
                {
                    DataRowView drvDataRowView = pvtDataViewEmployeeLink.AddNew();

                    drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                    drvDataRowView["EMPLOYEE_NO"] = this.pvtint64EmployeeNo;
                    drvDataRowView["EMPLOYEE_NO_LINK"] = Convert.ToInt32(this.dgvEmployeeChosenDataGridView[4,intRowCount].Value);

                    drvDataRowView.EndEdit();

                    TempDataSet.Tables["EmployeeLink"].ImportRow(drvDataRowView.Row);
                }

                TempDataSet.Tables.Add(pvtDataSet.Tables["PayCategoryLink"].Clone());

                for (int intRowCount = 0; intRowCount < this.dgvPayCategoryChosenDataGridView.Rows.Count; intRowCount++)
                {
                    DataRowView drvDataRowView = pvtDataViewPayCategoryLink.AddNew();

                    drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                    drvDataRowView["EMPLOYEE_NO"] = this.pvtint64EmployeeNo;
                    drvDataRowView["PAY_CATEGORY_NO"] = Convert.ToInt32(this.dgvPayCategoryChosenDataGridView[2,intRowCount].Value);
                    drvDataRowView["PAY_CATEGORY_TYPE"] = this.dgvPayCategoryChosenDataGridView[1,intRowCount].Value.ToString();

                    drvDataRowView.EndEdit();

                    TempDataSet.Tables["PayCategoryLink"].ImportRow(drvDataRowView.Row);
                }

                TempDataSet.Tables.Add(pvtDataSet.Tables["DepartmentLink"].Clone());

                for (int intRowCount = 0; intRowCount < this.dgvDepartmentChosenDataGridView.Rows.Count; intRowCount++)
                {
                    DataRowView drvDataRowView = pvtDataViewDepartmentLink.AddNew();

                    drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                    drvDataRowView["EMPLOYEE_NO"] = this.pvtint64EmployeeNo;
                    drvDataRowView["PAY_CATEGORY_NO"] = Convert.ToInt32(this.dgvDepartmentChosenDataGridView[2,intRowCount].Value);
                    drvDataRowView["PAY_CATEGORY_TYPE"] = this.dgvDepartmentChosenDataGridView[3, intRowCount].Value.ToString();
                    drvDataRowView["DEPARTMENT_NO"] = Convert.ToInt32(this.dgvDepartmentChosenDataGridView[4, intRowCount].Value);

                    drvDataRowView.EndEdit();

                    TempDataSet.Tables["DepartmentLink"].ImportRow(drvDataRowView.Row);
                }

                TempDataSet.Tables.Add(pvtDataSet.Tables["UserLink"].Clone());

                for (int intRowCount = 0; intRowCount < this.dgvUserChosenDataGridView.Rows.Count; intRowCount++)
                {
                    DataRowView drvDataRowView = pvtDataViewUserLink.AddNew();

                    drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                    drvDataRowView["EMPLOYEE_NO"] = this.pvtint64EmployeeNo;
                    drvDataRowView["USER_NO"] = Convert.ToInt32(this.dgvUserChosenDataGridView[3,intRowCount].Value);

                    drvDataRowView.EndEdit();

                    TempDataSet.Tables["UserLink"].ImportRow(drvDataRowView.Row);
                }

                //Compress DataSet
                byte[] pvtbytCompress = clsISClientUtilities.Compress_DataSet(TempDataSet);

                object[] objParm = new object[3];
                objParm[0] = pvtint64CompanyNo;
                objParm[1] = pvtint64EmployeeNo;
                objParm[2] = pvtbytCompress;

                clsISClientUtilities.DynamicFunction("Update_Records", objParm,true);

                DataView DataView = new DataView(pvtDataSet.Tables["PeopleLink"], "COMPANY_NO = " + pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo, "", DataViewRowState.CurrentRows);

                if (pvtDataViewEmployeeLink.Count > 0
                | pvtDataViewPayCategoryLink.Count > 0
                | pvtDataViewDepartmentLink.Count > 0
                | pvtDataViewUserLink.Count > 0)
                {
                    DataView[0]["LINKED_IND"] = "Y";
                }
                else
                {
                    DataView[0]["LINKED_IND"] = "N";
                }

                pvtDataSet.AcceptChanges();

                btnCancel_Click(sender, e);

                if (this.dgvCompanyDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));

                    Set_Form_For_Read();
                }
                else
                {
                    this.btnUpdate.Enabled = false;
                    this.btnDelete.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
            }
        }

        public void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomClientMessageBox.Show("Delete All Employees/Cost Centres/Users Linked to Employee '" + this.dgvPersonLinkedDataGridView[2, this.dgvPersonLinkedDataGridView.CurrentRow.Index].Value.ToString() + " " + this.dgvPersonLinkedDataGridView[1, this.dgvPersonLinkedDataGridView.CurrentRow.Index].Value.ToString() + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {

                    DataSet TempDataSet = new DataSet();

                    TempDataSet.Tables.Add(pvtDataSet.Tables["EmployeeLink"].Clone());

                    pvtDataViewEmployeeLink = null;
                    pvtDataViewEmployeeLink = new DataView(pvtDataSet.Tables["EmployeeLink"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo, "", DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtDataViewEmployeeLink.Count; intRow++)
                    {
                        pvtDataViewEmployeeLink[intRow].Delete();
                        intRow -= 1;
                    }

                    TempDataSet.Tables.Add(pvtDataSet.Tables["PayCategoryLink"].Clone());

                    pvtDataViewPayCategoryLink = null;
                    pvtDataViewPayCategoryLink = new DataView(pvtDataSet.Tables["PayCategoryLink"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo, "", DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtDataViewPayCategoryLink.Count; intRow++)
                    {
                        pvtDataViewPayCategoryLink[intRow].Delete();
                        intRow -= 1;
                    }

                    TempDataSet.Tables.Add(pvtDataSet.Tables["DepartmentLink"].Clone());

                    pvtDataViewDepartmentLink = null;
                    pvtDataViewDepartmentLink = new DataView(pvtDataSet.Tables["DepartmentLink"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo, "", DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtDataViewDepartmentLink.Count; intRow++)
                    {
                        pvtDataViewDepartmentLink[intRow].Delete();
                        intRow -= 1;
                    }

                    TempDataSet.Tables.Add(pvtDataSet.Tables["UserLink"].Clone());

                    pvtDataViewUserLink = null;
                    pvtDataViewUserLink = new DataView(pvtDataSet.Tables["UserLink"], "COMPANY_NO = " + pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo, "", DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtDataViewUserLink.Count; intRow++)
                    {
                        pvtDataViewUserLink[intRow].Delete();
                        intRow -= 1;
                    }

                    pvtDataSet.AcceptChanges();

                    //Compress DataSet
                    byte[] pvtbytCompress = clsISClientUtilities.Compress_DataSet(TempDataSet);

                    object[] objParm = new object[3];
                    objParm[0] = pvtint64CompanyNo;
                    objParm[1] = pvtint64EmployeeNo;
                    objParm[2] = pvtbytCompress;

                    clsISClientUtilities.DynamicFunction("Update_Records", objParm,true);

                    DataView DataView = new DataView(pvtDataSet.Tables["PeopleLink"], "COMPANY_NO = " + pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo, "", DataViewRowState.CurrentRows);

                    DataView[0]["LINKED_IND"] = "N";

                    pvtDataSet.AcceptChanges();

                    btnCancel_Click(sender, e);

                    if (this.dgvCompanyDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
                    }
                }
            }
            catch(Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
            }
        }

        private void btnUserAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvUserDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvUserDataGridView.Rows[this.dgvUserDataGridView.CurrentRow.Index];

                this.dgvUserDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvUserChosenDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvUserChosenDataGridView.CurrentCell = this.dgvUserChosenDataGridView[0, this.dgvUserChosenDataGridView.Rows.Count - 1];
            }
        }

        private void btnUserRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvUserChosenDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvUserChosenDataGridView.Rows[this.dgvUserChosenDataGridView.CurrentRow.Index];

                this.dgvUserChosenDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvUserDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvUserDataGridView.CurrentCell = this.dgvUserDataGridView[0, this.dgvUserDataGridView.Rows.Count - 1];
            }
        }

        private void frmEmployeeOverride_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
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

        private void dgvCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (pvtblnCompanyDataGridViewLoaded == true)
                {
                    if (pvtintCompanyDataGridViewRowIndex != e.RowIndex)
                    {
                        pvtintCompanyDataGridViewRowIndex = e.RowIndex;

                        pvtint64CompanyNo = Convert.ToInt64(this.dgvCompanyDataGridView[1, e.RowIndex].Value);

                        Clear_DataGridView(dgvPersonLinkedDataGridView);

                        Clear_DataGridView(dgvEmployeeDataGridView);
                        Clear_DataGridView(dgvEmployeeChosenDataGridView);

                        Clear_DataGridView(dgvPayCategoryDataGridView);
                        Clear_DataGridView(dgvPayCategoryChosenDataGridView);

                        Clear_DataGridView(dgvDepartmentDataGridView);
                        Clear_DataGridView(dgvDepartmentChosenDataGridView);

                        Clear_DataGridView(dgvUserDataGridView);
                        Clear_DataGridView(dgvUserChosenDataGridView);

                        DataView DataView = new DataView(pvtDataSet.Tables["PeopleLink"],
                            "COMPANY_NO = " + pvtint64CompanyNo,
                            "",
                            DataViewRowState.CurrentRows);

                        if (DataView.Count == 0)
                        {
                            object[] objParm = new object[3];
                            objParm[0] = pvtint64CompanyNo;
                            objParm[1] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                            objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                            byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Company_Records", objParm, false);
                            DataSet TempDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                            pvtDataSet.Merge(TempDataSet);
                        }

                        pvtblnPersonLinkedDataGridViewLoaded = false;

                        string strExtraFilter = "";

                        if (this.rbnOverride.Checked == true)
                        {
                            strExtraFilter = " AND LINKED_IND = 'Y'";
                        }

                        pvtDataViewPeopleLink = null;
                        pvtDataViewPeopleLink = new DataView(pvtDataSet.Tables["PeopleLink"], "COMPANY_NO = " + this.pvtint64CompanyNo.ToString() + strExtraFilter, "", DataViewRowState.CurrentRows);

                        for (int intRowCount = 0; intRowCount < pvtDataViewPeopleLink.Count; intRowCount++)
                        {
                            this.dgvPersonLinkedDataGridView.Rows.Add(pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_CODE"].ToString(),
                                                                 pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_SURNAME"].ToString(),
                                                                 pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_NAME"].ToString(),
                                                                 pvtDataViewPeopleLink[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                                                                 pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_NO"].ToString());

                            if (pvtDataViewPeopleLink[intRowCount]["LINKED_IND"].ToString() == "Y")
                            {
                                dgvPersonLinkedDataGridView.Rows[intRowCount].HeaderCell.Style = EmployeeOverrideDataGridViewCellStyle;
                            }
                        }

                        pvtblnPersonLinkedDataGridViewLoaded = true;

                        if (this.dgvPersonLinkedDataGridView.Rows.Count > 0)
                        {
                            this.btnUpdate.Enabled = true;
                            this.btnDelete.Enabled = true;

                            this.Set_DataGridView_SelectedRowIndex(dgvPersonLinkedDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPersonLinkedDataGridView));
                        }
                        else
                        {
                            this.btnUpdate.Enabled = false;
                            this.btnDelete.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
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

        private void dgvPersonLinkedDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPersonLinkedDataGridViewLoaded == true)
            {
                if (pvtintPersonLinkedDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPersonLinkedDataGridViewRowIndex = e.RowIndex;

                    pvtint64EmployeeNo = Convert.ToInt64(this.dgvPersonLinkedDataGridView[4, e.RowIndex].Value);

                    pvtstrPayCategoryType = this.dgvPersonLinkedDataGridView[3, e.RowIndex].Value.ToString();

                    Clear_DataGridView(dgvEmployeeDataGridView);
                    Clear_DataGridView(dgvEmployeeChosenDataGridView);

                    Clear_DataGridView(dgvPayCategoryDataGridView);
                    Clear_DataGridView(dgvPayCategoryChosenDataGridView);

                    Clear_DataGridView(dgvDepartmentDataGridView);
                    Clear_DataGridView(dgvDepartmentChosenDataGridView);

                    Clear_DataGridView(dgvUserDataGridView);
                    Clear_DataGridView(dgvUserChosenDataGridView);

                    for (int intRowCount = 0; intRowCount < pvtDataViewPeopleLink.Count; intRowCount++)
                    {
                        if (pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_NO"].ToString() == pvtint64EmployeeNo.ToString())
                        {
                            continue;
                        }

                        pvtDataViewEmployeeLink = null;
                        pvtDataViewEmployeeLink = new DataView(pvtDataSet.Tables["EmployeeLink"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND EMPLOYEE_NO_LINK = " + pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_NO"].ToString(), "", DataViewRowState.CurrentRows);

                        if (pvtDataViewEmployeeLink.Count == 0)
                        {
                            dgvEmployeeDataGridView.Rows.Add(pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_CODE"].ToString(),
                                                             pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_SURNAME"].ToString(),
                                                             pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_NAME"].ToString(),
                                                             pvtDataViewPeopleLink[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                                                             pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_NO"].ToString());
                        }
                        else
                        {
                            dgvEmployeeChosenDataGridView.Rows.Add(pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_CODE"].ToString(),
                                                             pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_SURNAME"].ToString(),
                                                             pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_NAME"].ToString(),
                                                             pvtDataViewPeopleLink[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                                                             pvtDataViewPeopleLink[intRowCount]["EMPLOYEE_NO"].ToString());
                        }
                    }



                    pvtDataViewPayCategory = null;
                    pvtDataViewPayCategory = new DataView(pvtDataSet.Tables["PayCategory"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayCategoryType + "'", "", DataViewRowState.CurrentRows);

                    for (int intRowCount = 0; intRowCount < pvtDataViewPayCategory.Count; intRowCount++)
                    {
                        pvtDataViewPayCategoryLink = null;
                        pvtDataViewPayCategoryLink = new DataView(pvtDataSet.Tables["PayCategoryLink"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND PAY_CATEGORY_NO = " + pvtDataViewPayCategory[intRowCount]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayCategoryType + "'", "", DataViewRowState.CurrentRows);

                        if (pvtDataViewPayCategoryLink.Count == 0)
                        {
                            dgvPayCategoryDataGridView.Rows.Add(pvtDataViewPayCategory[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                            pvtDataViewPayCategory[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                            pvtDataViewPayCategory[intRowCount]["PAY_CATEGORY_NO"].ToString());
                        }
                        else
                        {
                            dgvPayCategoryChosenDataGridView.Rows.Add(pvtDataViewPayCategory[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                            pvtDataViewPayCategory[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                            pvtDataViewPayCategory[intRowCount]["PAY_CATEGORY_NO"].ToString());
                        }
                    }

                    pvtDataViewDepartment = null;
                    pvtDataViewDepartment = new DataView(pvtDataSet.Tables["Department"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayCategoryType + "'", "", DataViewRowState.CurrentRows);
                    
                    for (int intRowCount = 0; intRowCount < pvtDataViewDepartment.Count; intRowCount++)
                    {
                        pvtDataViewDepartmentLink = null;
                        pvtDataViewDepartmentLink = new DataView(pvtDataSet.Tables["DepartmentLink"], "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND DEPARTMENT_NO = " + pvtDataViewDepartment[intRowCount]["DEPARTMENT_NO"].ToString(), "", DataViewRowState.CurrentRows);
                        
                        if (pvtDataViewDepartmentLink.Count == 0)
                        {
                            dgvDepartmentDataGridView.Rows.Add(pvtDataViewDepartment[intRowCount]["DEPARTMENT_DESC"].ToString(),
                                                               pvtDataViewDepartment[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                                                               pvtDataViewDepartment[intRowCount]["PAY_CATEGORY_NO"].ToString(),
                                                               pvtDataViewDepartment[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                                                               pvtDataViewDepartment[intRowCount]["DEPARTMENT_NO"].ToString());
                        }
                        else
                        {
                            dgvDepartmentChosenDataGridView.Rows.Add(pvtDataViewDepartment[intRowCount]["DEPARTMENT_DESC"].ToString(),
                                                                     pvtDataViewDepartment[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                                                                     pvtDataViewDepartment[intRowCount]["PAY_CATEGORY_NO"].ToString(),
                                                                     pvtDataViewDepartment[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                                                                     pvtDataViewDepartment[intRowCount]["DEPARTMENT_NO"].ToString());
                        }
                    }

                    pvtDataViewUser = null;
                    pvtDataViewUser = new DataView(pvtDataSet.Tables["User"], "COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);


                    for (int intRowCount = 0; intRowCount < pvtDataViewUser.Count; intRowCount++)
                    {
                        pvtDataViewUserLink = null;
                        pvtDataViewUserLink = new DataView(pvtDataSet.Tables["UserLink"], "COMPANY_NO = " + pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvtint64EmployeeNo + " AND USER_NO = " + pvtDataViewUser[intRowCount]["USER_NO"].ToString(), "", DataViewRowState.CurrentRows);

                        if (pvtDataViewUserLink.Count == 0)
                        {
                            this.dgvUserDataGridView.Rows.Add(pvtDataViewUser[intRowCount]["USER_ID"].ToString(),
                                                              pvtDataViewUser[intRowCount]["SURNAME"].ToString(),
                                                              pvtDataViewUser[intRowCount]["FIRSTNAME"].ToString(),
                                                              pvtDataViewUser[intRowCount]["USER_NO"].ToString());
                        }
                        else
                        {
                            this.dgvUserChosenDataGridView.Rows.Add(pvtDataViewUser[intRowCount]["USER_ID"].ToString(),
                                                              pvtDataViewUser[intRowCount]["SURNAME"].ToString(),
                                                              pvtDataViewUser[intRowCount]["FIRSTNAME"].ToString(),
                                                              pvtDataViewUser[intRowCount]["USER_NO"].ToString());
                        }
                    }

                    if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0
                        | this.dgvPayCategoryChosenDataGridView.Rows.Count > 0
                        | this.dgvDepartmentChosenDataGridView.Rows.Count > 0
                        | this.dgvUserChosenDataGridView.Rows.Count > 0)
                    {
                        this.btnDelete.Enabled = true;
                    }
                    else
                    {
                        this.btnDelete.Enabled = false;
                    }
                }
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

        private void dgvUserDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnUserAdd_Click(sender, e);
            }
        }

        private void dgvUserChosenDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnUserRemove_Click(sender, e);
            }
        }

        private void rbnEmployeeView_Click(object sender, EventArgs e)
        {
            if (this.dgvCompanyDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
            }
        }
    }
}
