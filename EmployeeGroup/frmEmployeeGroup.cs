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
    public partial class frmEmployeeGroup : Form
    {
        clsISClientUtilities clsISClientUtilities;

        ToolStripMenuItem miLinkedMenuItem;

        private int pvtintPayCategoryTypeCol = 2;
        private int pvtintEmployeeNoCol = 4;
        private int pvtintPayCategoryNoCol = 5;

        private DataSet pvtDataSet;
        private DataView pvtGroupDataView;
        private DataView pvEmployeeDataView;
        
        private Int64 pvtint64CompanyNo = -1;
        private int pvtintCompanyDataTableRow = -1;

        private int pvtintGroupNo = -1;
        private int pvtintGroupDataViewRow = -1;

        private int pvtintCompanyDataGridViewRowIndex = -1;
        private int pvtintGroupDataGridViewRowIndex = -1;

        private bool pvtblnCompanyDataGridViewLoaded = false;
        private bool pvtblnGroupDataGridViewLoaded = false;

        public frmEmployeeGroup()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.dgvEmployeeDataGridView.Height += 114;
                this.dgvEmployeeLinkedDataGridView.Height += 114;

                this.btnAdd.Top += 57;
                this.btnAddAll.Top += 57;
                this.btnRemove.Top += 57;
                this.btnRemoveAll.Top += 57;
                
                this.grbGroup.Height += 114;
            }
        }

        private void frmEmployeeGroup_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISClientUtilities = new clsISClientUtilities(this, "busEmployeeGroup");

                this.lblCompany.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblGroup.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblSelectedEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

#if(DEBUG)
                int intWidth = this.dgvGroupDataGridView.RowHeadersWidth;

                if (this.dgvGroupDataGridView.ScrollBars == ScrollBars.Vertical
                    | this.dgvGroupDataGridView.ScrollBars == ScrollBars.Both)
                {
                    intWidth += 19;
                }

                for (int intCol = 0; intCol < this.dgvGroupDataGridView.ColumnCount; intCol++)
                {
                    if (this.dgvGroupDataGridView.Columns[intCol].Visible == true)
                    {
                        intWidth += this.dgvGroupDataGridView.Columns[intCol].Width;
                    }
                }

                if (intWidth != this.dgvGroupDataGridView.Width)
                {
                    System.Windows.Forms.MessageBox.Show("Width should be " + intWidth.ToString());
                }

                int intHeight = this.dgvGroupDataGridView.ColumnHeadersHeight + 2;
                int intNewHeight = this.dgvGroupDataGridView.RowTemplate.Height / 2;

                for (int intRow = 0; intRow < 200; intRow++)
                {
                    intHeight += this.dgvGroupDataGridView.RowTemplate.Height;

                    if (intHeight == this.dgvGroupDataGridView.Height)
                    {
                        break;
                    }
                    else
                    {
                        if (intHeight > this.dgvGroupDataGridView.Height)
                        {
                            System.Windows.Forms.MessageBox.Show("Height should be " + intHeight.ToString());
                            break;
                        }
                        else
                        {

                            if (intHeight + intNewHeight > this.dgvGroupDataGridView.Height)
                            {
                                System.Windows.Forms.MessageBox.Show("Height should be " + intHeight.ToString());
                                break;
                            }
                        }
                    }
                }

                intWidth = this.dgvEmployeeDataGridView.RowHeadersWidth;

                if (this.dgvEmployeeDataGridView.ScrollBars == ScrollBars.Vertical
                    | this.dgvEmployeeDataGridView.ScrollBars == ScrollBars.Both)
                {
                    intWidth += 19;
                }

                for (int intCol = 0; intCol < this.dgvEmployeeDataGridView.ColumnCount; intCol++)
                {
                    if (this.dgvEmployeeDataGridView.Columns[intCol].Visible == true)
                    {
                        intWidth += this.dgvEmployeeDataGridView.Columns[intCol].Width;
                    }
                }

                if (intWidth != this.dgvEmployeeDataGridView.Width)
                {
                    System.Windows.Forms.MessageBox.Show("Employee Width should be " + intWidth.ToString());
                }

                intHeight = this.dgvEmployeeDataGridView.ColumnHeadersHeight + 2;
                intNewHeight = this.dgvEmployeeDataGridView.RowTemplate.Height / 2;

                for (int intRow = 0; intRow < 200; intRow++)
                {
                    intHeight += this.dgvEmployeeDataGridView.RowTemplate.Height;

                    if (intHeight == this.dgvEmployeeDataGridView.Height)
                    {
                        break;
                    }
                    else
                    {
                        if (intHeight > this.dgvEmployeeDataGridView.Height)
                        {
                            System.Windows.Forms.MessageBox.Show("Employee Height should be " + intHeight.ToString());
                            break;
                        }
                        else
                        {

                            if (intHeight + intNewHeight > this.dgvEmployeeDataGridView.Height)
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

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void frmEmployeeGroup_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }

        private void Load_CurrentForm_Records()
        {
            int intCompanyRow = 0;

            this.txtGroup.Text = "";

            if (pvtblnCompanyDataGridViewLoaded == true)
            {
                if (this.dgvCompanyDataGridView.Rows.Count > 0)
                {
                    intCompanyRow = this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView);
                }
            }

            pvtblnCompanyDataGridViewLoaded = false;

            Clear_DataGridView(dgvCompanyDataGridView);
            Clear_DataGridView(dgvGroupDataGridView);
            Clear_DataGridView(dgvEmployeeDataGridView);
            Clear_DataGridView(dgvEmployeeLinkedDataGridView);

            for (int intRow = 0; intRow < this.pvtDataSet.Tables["Company"].Rows.Count; intRow++)
            {
                dgvCompanyDataGridView.Rows.Add(this.pvtDataSet.Tables["Company"].Rows[intRow]["COMPANY_DESC"].ToString(),
                                                intRow.ToString());
            }

            this.btnDelete.Enabled = false;
            this.btnUpdate.Enabled = false;

            pvtblnCompanyDataGridViewLoaded = true;

            if (dgvCompanyDataGridView.Rows.Count > 0)
            {
                this.btnNew.Enabled = true;

                this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, intCompanyRow);
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

                    case "dgvGroupDataGridView":

                        pvtintGroupDataGridViewRowIndex = -1;
                        this.dgvGroupDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (this.txtGroup.Text.Trim() == "")
                {
                    CustomClientMessageBox.Show("Enter Group Description.",this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.txtGroup.Focus();
                    return;
                }

                if (this.dgvEmployeeLinkedDataGridView.Rows.Count == 0)
                {
                    CustomClientMessageBox.Show("Select Employees.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.btnAdd.Focus();
                    return;
                }

                if (this.Text.IndexOf(" - New") > 0)
                {
                    pvtintGroupNo = 0;
                }

                //NB - New Will Not Remove Any Records
                DataView EmployeeLinkDataView = new DataView(pvtDataSet.Tables["EmployeeLink"], "GROUP_NO = " + pvtintGroupNo + " AND COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < EmployeeLinkDataView.Count; intRow++)
                {
                    EmployeeLinkDataView[intRow].Delete();
                    intRow -= 1;
                }

                pvtDataSet.AcceptChanges();

                for (int intRowCount = 0; intRowCount < this.dgvEmployeeLinkedDataGridView.Rows.Count; intRowCount++)
                {
                    DataRowView drvDataRowView = EmployeeLinkDataView.AddNew();

                    drvDataRowView["GROUP_NO"] = pvtintGroupNo;
                    drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                    drvDataRowView["EMPLOYEE_NO"] = Convert.ToInt32(this.dgvEmployeeLinkedDataGridView[pvtintEmployeeNoCol,intRowCount].Value);
                    drvDataRowView["PAY_CATEGORY_NO"] = Convert.ToInt32(this.dgvEmployeeLinkedDataGridView[pvtintPayCategoryNoCol, intRowCount].Value);
                    drvDataRowView["PAY_CATEGORY_TYPE"] = this.dgvEmployeeLinkedDataGridView[pvtintPayCategoryTypeCol, intRowCount].Value.ToString();

                    drvDataRowView.EndEdit();
                }



                DataSet TempDataSet = new DataSet();
                //Add EmployeePayCategory Table 
                TempDataSet.Tables.Add(pvtDataSet.Tables["EmployeeLink"].Clone());

                for (int intRow = 0; intRow < EmployeeLinkDataView.Count; intRow++)
                {
                    TempDataSet.Tables["EmployeeLink"].ImportRow(EmployeeLinkDataView[intRow].Row);
                }

                //Compress DataSet
                byte[] pvtbytCompress = clsISClientUtilities.Compress_DataSet(TempDataSet);
               
                this.Cursor = Cursors.WaitCursor;

                if (this.Text.IndexOf(" - New") > 0)
                {
                    object[] objParm = new object[3];
                    objParm[0] = pvtint64CompanyNo;
                    objParm[1] = this.txtGroup.Text.Trim();
                    objParm[2] = pvtbytCompress;

                    pvtintGroupNo = (int)clsISClientUtilities.DynamicFunction("Insert_Group", objParm,true);

                    for (int intRowCount = 0; intRowCount < EmployeeLinkDataView.Count; intRowCount++)
                    {
                        EmployeeLinkDataView[intRowCount]["GROUP_NO"] = pvtintGroupNo;

                        intRowCount -= 1;
                    }

                    DataRowView drvDataRowView = this.pvtGroupDataView.AddNew();
                    //Set Key for Find
                    drvDataRowView["COMPANY_NO"] = pvtDataSet.Tables["Company"].Rows[pvtintCompanyDataTableRow]["COMPANY_NO"].ToString();
                    drvDataRowView["GROUP_DESC"] = this.txtGroup.Text.Trim();
                    drvDataRowView["GROUP_NO"] = pvtintGroupNo;

                    drvDataRowView.EndEdit();
                }
                else
                {
                    object[] objParm = new object[4];
                    objParm[0] = Convert.ToInt64(pvtDataSet.Tables["Company"].Rows[pvtintCompanyDataTableRow]["COMPANY_NO"]);
                    objParm[1] = pvtintGroupNo;
                    objParm[2] = this.txtGroup.Text.Trim();
                    objParm[3] = pvtbytCompress;

                    clsISClientUtilities.DynamicFunction("Update_Group", objParm,true);

                    this.dgvGroupDataGridView[0,this.dgvGroupDataGridView.CurrentRow.Index].Value = this.txtGroup.Text.Trim();
                    pvtGroupDataView[pvtintGroupDataViewRow]["GROUP_DESC"] = this.txtGroup.Text.Trim();
                }

                pvtDataSet.AcceptChanges();

                if (this.Text.IndexOf(" - New") > 0)
                {
                    Load_CurrentForm_Records();
                }

                btnCancel_Click(sender, e);

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
            }
        }

        private void Set_Form_For_Edit()
        {
            this.dgvCompanyDataGridView.Enabled = false;
            this.dgvGroupDataGridView.Enabled = false;

            this.picGroupLock.Visible = true;

            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.rbnSurnameName.Enabled = false;
            this.rbnNameSurname.Enabled = false;

            this.picCompanyLock.Visible = true;
            this.picGroupLock.Visible = true;

            if (this.Text.IndexOf(" - New") > 0)
            {
                this.txtGroup.Text = "";

                Clear_DataGridView(dgvEmployeeLinkedDataGridView);
                Clear_DataGridView(dgvEmployeeDataGridView);
            }

            this.txtGroup.Enabled = true;

            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();

            this.btnAdd.Focus();
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (this.Text.LastIndexOf(" - ") != -1)
            {
                this.Text = this.Text.Substring(0, this.Text.LastIndexOf(" - "));
            }

            this.picGroupLock.Visible = false;

            this.rbnSurnameName.Enabled = true;
            this.rbnNameSurname.Enabled = true;

            this.btnNew.Enabled = true;
            
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.btnAdd.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;

            this.txtGroup.Enabled = false;
            this.txtGroup.Text = "";

            this.dgvCompanyDataGridView.Enabled = true;
            this.dgvGroupDataGridView.Enabled = true;

            Clear_DataGridView(dgvEmployeeDataGridView);
            Clear_DataGridView(dgvEmployeeLinkedDataGridView);

            this.picGroupLock.Visible = false;
            this.picCompanyLock.Visible = false;

            if (this.dgvGroupDataGridView.Rows.Count > 0)
            {
                this.btnUpdate.Enabled = true;
                this.btnDelete.Enabled = true;

                this.Set_DataGridView_SelectedRowIndex(dgvGroupDataGridView,this.Get_DataGridView_SelectedRowIndex(dgvGroupDataGridView));
            }
            else
            {
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;

                if (this.dgvGroupDataGridView.SortedColumn != null)
                {
                    dgvGroupDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
       }

        public void btnNew_Click(object sender, System.EventArgs e)
        {
            this.Text += " - New";
            string strNames = "";

            Set_Form_For_Edit();

            pvEmployeeDataView = null;
            pvEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"], "COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);
            
            for (int intRowCount = 0; intRowCount < pvEmployeeDataView.Count; intRowCount++)
            {
                if (this.rbnSurnameName.Checked == true)
                {
                    strNames = pvEmployeeDataView[intRowCount]["EMPLOYEE_SURNAME"].ToString() + " / " + pvEmployeeDataView[intRowCount]["EMPLOYEE_NAME"].ToString();
                }
                else
                {
                    strNames = pvEmployeeDataView[intRowCount]["EMPLOYEE_NAME"].ToString() + " / " + pvEmployeeDataView[intRowCount]["EMPLOYEE_SURNAME"].ToString();
                }

                this.dgvEmployeeDataGridView.Rows.Add(pvEmployeeDataView[intRowCount]["EMPLOYEE_CODE"].ToString(),
                                                        strNames,  
                                                        pvEmployeeDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                                                        pvEmployeeDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                                                        pvEmployeeDataView[intRowCount]["EMPLOYEE_NO"].ToString(),
                                                        pvEmployeeDataView[intRowCount]["PAY_CATEGORY_NO"].ToString());
            }
            
            this.txtGroup.Focus();
        }

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomClientMessageBox.Show("Delete Group '" + pvtGroupDataView[this.pvtintGroupDataViewRow]["GROUP_DESC"].ToString() + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[2];
                    objParm[0] = pvtint64CompanyNo;
                    objParm[1] = pvtintGroupNo;

                    clsISClientUtilities.DynamicFunction("Delete_Group", objParm,true);

                    pvtGroupDataView[pvtintGroupDataViewRow].Delete();

                    DataView EmployeeLinkDataView = new DataView(pvtDataSet.Tables["EmployeeLink"], "GROUP_NO = " + pvtintGroupNo + " AND COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < EmployeeLinkDataView.Count; intRow++)
                    {
                        EmployeeLinkDataView[intRow].Delete();
                        intRow -= 1;
                    }

                    this.pvtDataSet.AcceptChanges();

                    pvtintGroupNo = -1;

                    this.Load_CurrentForm_Records();
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeeDataGridView.Rows[this.dgvEmployeeDataGridView.CurrentRow.Index];

                this.dgvEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeLinkedDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvEmployeeLinkedDataGridView.CurrentCell = this.dgvEmployeeLinkedDataGridView[0, this.dgvEmployeeLinkedDataGridView.Rows.Count - 1];
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEmployeeLinkedDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeeLinkedDataGridView.Rows[this.dgvEmployeeLinkedDataGridView.CurrentRow.Index];

                this.dgvEmployeeLinkedDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvEmployeeDataGridView.CurrentCell = this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1];
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

            if (this.dgvEmployeeLinkedDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
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

                        pvtintCompanyDataTableRow = Convert.ToInt32(this.dgvCompanyDataGridView[1, e.RowIndex].Value);

                        pvtint64CompanyNo = Convert.ToInt64(pvtDataSet.Tables["Company"].Rows[pvtintCompanyDataTableRow]["COMPANY_NO"]);

                        DataView DataView = new DataView(pvtDataSet.Tables["Employee"],
                            "COMPANY_NO = " + pvtint64CompanyNo,
                            "",
                            DataViewRowState.CurrentRows);

                        if (DataView.Count == 0)
                        {
                            object[] objParm = new object[1];
                            objParm[0] = pvtint64CompanyNo;

                            byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Company_Records", objParm, false);
                            DataSet TempDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                            pvtDataSet.Merge(TempDataSet);
                        }

                        pvtGroupDataView = null;
                        pvtGroupDataView = new DataView(pvtDataSet.Tables["Group"],
                            "COMPANY_NO = " + pvtint64CompanyNo,
                            "GROUP_DESC",
                            DataViewRowState.CurrentRows);

                        this.txtGroup.Text = "";
                        int intGroupRow = 0;

                        pvtblnGroupDataGridViewLoaded = false;

                        Clear_DataGridView(dgvGroupDataGridView);
                        Clear_DataGridView(dgvEmployeeDataGridView);
                        Clear_DataGridView(dgvEmployeeLinkedDataGridView);

                        for (int intRow = 0; intRow < pvtGroupDataView.Count; intRow++)
                        {
                            this.dgvGroupDataGridView.Rows.Add(pvtGroupDataView[intRow]["GROUP_DESC"].ToString()
                                                              , intRow.ToString());

                            if (pvtintGroupNo == Convert.ToInt32(pvtGroupDataView[intRow]["GROUP_NO"]))
                            {
                                intGroupRow = intRow;
                            }
                        }

                        pvtblnGroupDataGridViewLoaded = true;

                        if (dgvGroupDataGridView.Rows.Count > 0)
                        {
                            this.btnDelete.Enabled = true;
                            this.btnUpdate.Enabled = true;

                            this.Set_DataGridView_SelectedRowIndex(dgvGroupDataGridView, intGroupRow);
                        }
                        else
                        {
                            this.btnDelete.Enabled = false;
                            this.btnUpdate.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
            
        }

        private void dgvGroupDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnGroupDataGridViewLoaded == true)
            {
                if (pvtintGroupDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintGroupDataGridViewRowIndex = e.RowIndex;

                    pvtintGroupDataViewRow = Convert.ToInt32(this.dgvGroupDataGridView[1, e.RowIndex].Value);

                    pvtintGroupNo = Convert.ToInt32(pvtGroupDataView[pvtintGroupDataViewRow]["GROUP_NO"]);

                    this.txtGroup.Text = pvtGroupDataView[pvtintGroupDataViewRow]["GROUP_DESC"].ToString();

                    Clear_DataGridView(dgvEmployeeDataGridView);
                    Clear_DataGridView(dgvEmployeeLinkedDataGridView);

                    string strNames = "";

                    pvEmployeeDataView = null;
                    pvEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"], "COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                    for (int intRowCount = 0; intRowCount < pvEmployeeDataView.Count; intRowCount++)
                    {
                        DataView DataView = new DataView(pvtDataSet.Tables["EmployeeLink"], "GROUP_NO = " + pvtintGroupNo + " AND COMPANY_NO = " + pvtint64CompanyNo + " AND EMPLOYEE_NO = " + pvEmployeeDataView[intRowCount]["EMPLOYEE_NO"].ToString() + " AND PAY_CATEGORY_NO = " + pvEmployeeDataView[intRowCount]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvEmployeeDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString() + "'", "", DataViewRowState.CurrentRows);

                        if (this.rbnSurnameName.Checked == true)
                        {
                            strNames = pvEmployeeDataView[intRowCount]["EMPLOYEE_SURNAME"].ToString() + " / " + pvEmployeeDataView[intRowCount]["EMPLOYEE_NAME"].ToString();
                        }
                        else
                        {
                            strNames = pvEmployeeDataView[intRowCount]["EMPLOYEE_NAME"].ToString() + " / " + pvEmployeeDataView[intRowCount]["EMPLOYEE_SURNAME"].ToString();
                        }

                        if (DataView.Count == 0)
                        {
                            this.dgvEmployeeDataGridView.Rows.Add(pvEmployeeDataView[intRowCount]["EMPLOYEE_CODE"].ToString(),
                            strNames,
                            pvEmployeeDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                            pvEmployeeDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                            pvEmployeeDataView[intRowCount]["EMPLOYEE_NO"].ToString(),
                            pvEmployeeDataView[intRowCount]["PAY_CATEGORY_NO"].ToString());
                        }
                        else
                        {
                            this.dgvEmployeeLinkedDataGridView.Rows.Add(pvEmployeeDataView[intRowCount]["EMPLOYEE_CODE"].ToString(),
                            strNames,
                            pvEmployeeDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString(),
                            pvEmployeeDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                            pvEmployeeDataView[intRowCount]["EMPLOYEE_NO"].ToString(),
                            pvEmployeeDataView[intRowCount]["PAY_CATEGORY_NO"].ToString());
                        }
                    }
                }
            }
        }

        private void DataGrid_Sorted(object sender, EventArgs e)
        {
            DataGridView myDataGridView = (DataGridView)sender;

            if (myDataGridView.Rows.Count > 0)
            {
                if (myDataGridView.SelectedRows.Count > 0)
                {
                    if (myDataGridView.SelectedRows[0].Selected == true)
                    {
                        myDataGridView.FirstDisplayedScrollingRowIndex = myDataGridView.SelectedRows[0].Index; ;
                    }
                }
            }
        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnAdd_Click(sender, e);
            }
        }

        private void dgvEmployeeLinkedDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnRemove_Click(sender, e);
            }
        }

        private void rbnSurnameOrNameOrder_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            if (myRadioButton.Name == "rbnSurnameName")
            {
                this.dgvEmployeeDataGridView.Columns[1].HeaderText = "Surname / Name";
                this.dgvEmployeeLinkedDataGridView.Columns[1].HeaderText = "Surname / Name";
            }
            else
            {
                this.dgvEmployeeDataGridView.Columns[1].HeaderText = "Name / Surname";
                this.dgvEmployeeLinkedDataGridView.Columns[1].HeaderText = "Name / Surname";
            }

            if (this.dgvGroupDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvGroupDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvGroupDataGridView));
            }
        }
    }
}
