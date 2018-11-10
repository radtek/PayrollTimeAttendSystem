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
    public partial class frmUserEmployeeLink : Form
    {

        clsISUtilities clsISUtilities;

        DataSet pvtDataSet;
        DataSet pvtTempDataSet;

        DataView pvtDataView;
        DataView pvtTempDataView;
        DataView pvtEmployeeDataView;
        DataView pvtPayCategoryDataView;
        DataView pvtDepartmentDataView;
        DataRowView pvtdrvDataRowView;

        DataView pvtUserEmployeeDataView;
        DataView pvtUserPayCategoryDataView;
        DataView pvtUserDepartmentDataView;

        private int pvtintReturnCode;
        private int pvtintUserNo = -1;
        private Int64 pvtint64CompanyNo = -1;
        private Int64 pvtint64CompanyNoSaved = -1;
        private int pvtintEmployeeNo = -1;

        byte[] pvtbytCompress;

        private bool pvtblnUserDataGridViewLoaded = false;
        private bool pvtblnCompanyDataGridViewLoaded = false;

        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnPayCategorySelectedDataGridViewLoaded = false;

        private bool pvtblnDepartmentDataGridViewLoaded = false;
        private bool pvtblnDepartmentSelectedDataGridViewLoaded = false;

        private bool pvtblnEmployeeDataGridViewLoaded = false;
        private bool pvtblnEmployeeSelectedDataGridViewLoaded = false;
        
        private bool pvtblnEmployeeNotLinkedDataGridViewLoaded = false;

        public frmUserEmployeeLink()
        {
            InitializeComponent();
        }

        private void frmUserEmployeeLink_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busUserEmployeeLink");

                this.lblUserSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCompanySpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblListCostCentreSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedCostCentreSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblListDepartmentsSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedDepartmentsSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblListEmployeesSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedEmployeesSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblNotLinkedEmployeesSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                Load_Users();
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
                    case "dgvUserDataGridView":

                        this.dgvUserDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvCompanyDataGridView":

                        this.dgvCompanyDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeNotLinkedDataGridView":

                        this.dgvEmployeeNotLinkedDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;


                    case "dgvEmployeeDataGridView":

                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeSelectedDataGridView":

                        this.dgvEmployeeSelectedDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDepartmentDataGridView":

                        this.dgvDepartmentDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDepartmentSelectedDataGridView":

                        this.dgvDepartmentSelectedDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategorySelectedDataGridView":

                        this.dgvPayCategorySelectedDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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


        private void Load_Users()
        {
            int intRowIndex = 0;
            int intUserNo = -1;

            pvtDataView = null;
            pvtDataView = new DataView(pvtDataSet.Tables["User"],
                "",
                "USER_ID",
                DataViewRowState.CurrentRows);

            this.pvtblnUserDataGridViewLoaded = false;

            this.Clear_DataGridView(this.dgvUserDataGridView);

            for (int intRow = 0; intRow < this.pvtDataView.Count; intRow++)
            {
                if (intUserNo != Convert.ToInt32(pvtDataView[intRow]["USER_NO"]))
                {
                    intUserNo = Convert.ToInt32(pvtDataView[intRow]["USER_NO"]);

                    this.dgvUserDataGridView.Rows.Add(pvtDataView[intRow]["USER_ID"].ToString(),
                                                      pvtDataView[intRow]["SURNAME"].ToString(),
                                                      pvtDataView[intRow]["FIRSTNAME"].ToString(),
                                                      intRow.ToString());

                    if (pvtintUserNo == Convert.ToInt32(pvtDataView[intRow]["USER_NO"]))
                    {
                        intRowIndex = intRow;
                    }
                }
            }

            this.pvtblnUserDataGridViewLoaded = true;

            if (this.dgvUserDataGridView.Rows.Count > 0)
            {
                this.btnUpdate.Enabled = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvUserDataGridView, intRowIndex);
            }
            else
            {
                this.btnUpdate.Enabled = false;
            }
        }

        private void Load_Users_Companys()
        {
            this.pvtblnCompanyDataGridViewLoaded = false;

            this.Clear_DataGridView(this.dgvCompanyDataGridView);

            for (int intRow = 0; intRow < this.pvtDataView.Count; intRow++)
            {
                if (pvtintUserNo == Convert.ToInt32(pvtDataView[intRow]["USER_NO"]))
                {
                    for (int intCompanyRow = 0; intCompanyRow < this.pvtDataSet.Tables["Company"].Rows.Count; intCompanyRow++)
                    {
                        if (Convert.ToDouble(pvtDataView[intRow]["COMPANY_NO"]) == Convert.ToDouble(this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_NO"]))
                        {
                            this.dgvCompanyDataGridView.Rows.Add(this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_DESC"].ToString(),
                                                                    intCompanyRow.ToString());
                            break;
                        }
                    }
                }
            }

            this.pvtblnCompanyDataGridViewLoaded = true;

            if (this.dgvCompanyDataGridView.Rows.Count > 0)
            {
                this.btnUpdate.Enabled = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvCompanyDataGridView,0);
            }
            else
            {
                this.btnUpdate.Enabled = false;

                this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
                this.Clear_DataGridView(this.dgvPayCategorySelectedDataGridView);

                this.Clear_DataGridView(this.dgvDepartmentDataGridView);
                this.Clear_DataGridView(this.dgvDepartmentSelectedDataGridView);

                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                this.Clear_DataGridView(this.dgvEmployeeSelectedDataGridView);

                this.Clear_DataGridView(this.dgvEmployeeNotLinkedDataGridView);
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            this.btnUpdate.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.btnPayCategoryAdd.Enabled = true;
            this.btnPayCategoryRemove.Enabled = true;

            this.btnDepartmentAdd.Enabled = true;
            this.btnDepartmentRemove.Enabled = true;

            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;

            this.picUserLock.Visible = true;
            this.picCompanyLock.Visible = true;

            this.dgvUserDataGridView.Enabled = false;
            this.dgvCompanyDataGridView.Enabled = false;
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf(" - Update"));

            this.pvtDataSet.RejectChanges();

            this.btnUpdate.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.picUserLock.Visible = false;
            this.picCompanyLock.Visible = false;

            this.btnPayCategoryAdd.Enabled = false;
            this.btnPayCategoryRemove.Enabled = false;

            this.btnDepartmentAdd.Enabled = false;
            this.btnDepartmentRemove.Enabled = false;

            this.btnAdd.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;

            this.dgvUserDataGridView.Enabled = true;
            this.dgvCompanyDataGridView.Enabled = true;

            this.Set_DataGridView_SelectedRowIndex(this.dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvCompanyDataGridView));
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();

                DataTable myDataTable = this.pvtDataSet.Tables["UserPayCategory"].Clone();
                pvtTempDataSet.Tables.Add(myDataTable);

                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["UserPayCategory"],
                    "COMPANY_NO = " + this.pvtint64CompanyNo + " AND USER_NO = " + this.pvtintUserNo,
                    "",
                    DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent | DataViewRowState.Added);

                for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
                {
                    pvtTempDataSet.Tables["UserPayCategory"].ImportRow(pvtTempDataView[intRow].Row);

                    if (pvtTempDataView[intRow].Row.RowState == DataRowState.Added)
                    {
                        pvtTempDataView[intRow].Delete();

                        intRow -= 1;
                    }
                }

                myDataTable = this.pvtDataSet.Tables["UserDepartment"].Clone();
                pvtTempDataSet.Tables.Add(myDataTable);

                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["UserDepartment"],
                    "COMPANY_NO = " + this.pvtint64CompanyNo + " AND USER_NO = " + this.pvtintUserNo,
                    "",
                    DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent | DataViewRowState.Added);

                for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
                {
                    pvtTempDataSet.Tables["UserDepartment"].ImportRow(pvtTempDataView[intRow].Row);

                    if (pvtTempDataView[intRow].Row.RowState == DataRowState.Added)
                    {
                        pvtTempDataView[intRow].Delete();

                        intRow -= 1;
                    }
                }

                myDataTable = this.pvtDataSet.Tables["UserEmployee"].Clone();
                pvtTempDataSet.Tables.Add(myDataTable);

                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["UserEmployee"],
                    "COMPANY_NO = " + this.pvtint64CompanyNo + " AND USER_NO = " + this.pvtintUserNo,
                    "",
                    DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent | DataViewRowState.Added);

                for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
                {
                    pvtTempDataSet.Tables["UserEmployee"].ImportRow(pvtTempDataView[intRow].Row);

                    if (pvtTempDataView[intRow].Row.RowState == DataRowState.Added)
                    {
                        pvtTempDataView[intRow].Delete();

                        intRow -= 1;
                    }
                }

                this.pvtDataSet.AcceptChanges();

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                object[] objParm = new object[2];
                objParm[0] = pvtintUserNo;
                objParm[1] = pvtbytCompress;

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_User_PayCategory_Employee", objParm,true);
                
                pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                pvtDataSet.Merge(pvtTempDataSet);

                this.pvtDataSet.AcceptChanges();

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnPayCategoryAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
                {
                    if (Convert.ToInt32(this.pvtPayCategoryDataView[Convert.ToInt32(this.dgvPayCategoryDataGridView[2,this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)].Value)]["PAY_CATEGORY_NO"]) == Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[3,intRow].Value)]["PAY_CATEGORY_NO"]))
                    {
                        this.dgvEmployeeDataGridView.Rows.RemoveAt(intRow);
                        intRow -= 1;
                    }
                }

                for (int intRow = 0; intRow < this.dgvEmployeeSelectedDataGridView.Rows.Count; intRow++)
                {
                    if (Convert.ToInt32(this.pvtPayCategoryDataView[Convert.ToInt32(this.dgvPayCategoryDataGridView[2, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)].Value)]["PAY_CATEGORY_NO"]) == Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeSelectedDataGridView[3, intRow].Value)]["PAY_CATEGORY_NO"]))
                    {
                        pvtTempDataView = null;
                        pvtTempDataView = new DataView(pvtDataSet.Tables["UserEmployee"],
                            "COMPANY_NO = " + this.pvtint64CompanyNo + " AND USER_NO = " + this.pvtintUserNo + " AND EMPLOYEE_NO = " + Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeSelectedDataGridView[3,this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeSelectedDataGridView)].Value)]["EMPLOYEE_NO"]),
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtTempDataView.Count == 1)
                        {
                            pvtTempDataView[0].Delete();
                        }

                        this.dgvEmployeeSelectedDataGridView.Rows.RemoveAt(intRow);
                        intRow -= 1;
                    }
                }

                for (int intRow = 0; intRow < this.dgvEmployeeNotLinkedDataGridView.Rows.Count; intRow++)
                {
                    if (Convert.ToInt32(this.pvtPayCategoryDataView[Convert.ToInt32(this.dgvPayCategoryDataGridView[2,this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)].Value)]["PAY_CATEGORY_NO"]) == Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeNotLinkedDataGridView[3, intRow].Value)]["PAY_CATEGORY_NO"]))
                    {
                        this.dgvEmployeeNotLinkedDataGridView.Rows.RemoveAt(intRow);
                        intRow -= 1;
                    }
                }

                pvtdrvDataRowView = pvtUserPayCategoryDataView.AddNew();
                pvtdrvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                pvtdrvDataRowView["USER_NO"] = pvtintUserNo;
                pvtdrvDataRowView["PAY_CATEGORY_NO"] = Convert.ToInt32(this.pvtPayCategoryDataView[Convert.ToInt32(this.dgvPayCategoryDataGridView[2, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)].Value)]["PAY_CATEGORY_NO"]);
                pvtdrvDataRowView["PAY_CATEGORY_TYPE"] = this.pvtPayCategoryDataView[Convert.ToInt32(this.dgvPayCategoryDataGridView[2, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)].Value)]["PAY_CATEGORY_TYPE"].ToString();
                pvtdrvDataRowView["TIE_BREAKER"] = 0;
                pvtdrvDataRowView.EndEdit();

                DataGridViewRow myDataGridViewRow = this.dgvPayCategoryDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)];

                this.dgvPayCategoryDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvPayCategorySelectedDataGridView.Rows.Add(myDataGridViewRow);
            }
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                for (int intRow = 0; intRow < this.dgvEmployeeNotLinkedDataGridView.Rows.Count; intRow++)
                {
                    if (Convert.ToInt32(this.dgvEmployeeDataGridView[3,this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)].Value) == Convert.ToInt32(this.dgvEmployeeNotLinkedDataGridView[3, intRow].Value))
                    {
                        this.dgvEmployeeNotLinkedDataGridView.Rows.RemoveAt(intRow);
                        break;
                    }
                }

                if (pvtUserEmployeeDataView == null)
                {
                    //Temp For Row Add
                    pvtUserEmployeeDataView = new DataView(pvtDataSet.Tables["UserEmployee"],
                                "COMPANY_NO = -1",
                                "",
                                DataViewRowState.CurrentRows);

                }

                pvtdrvDataRowView = pvtUserEmployeeDataView.AddNew();
                pvtdrvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                pvtdrvDataRowView["USER_NO"] = pvtintUserNo;
                pvtdrvDataRowView["EMPLOYEE_NO"] = Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)].Value)]["EMPLOYEE_NO"]);
                pvtdrvDataRowView["PAY_CATEGORY_TYPE"] = pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)].Value)]["PAY_CATEGORY_TYPE"].ToString();
                pvtdrvDataRowView["TIE_BREAKER"] = 0;
                pvtdrvDataRowView.EndEdit();

                DataGridViewRow myDataGridViewRow = this.dgvEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)];

                this.dgvEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeSelectedDataGridView.Rows.Add(myDataGridViewRow);
            }
        }

        private void btnPayCategoryRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvPayCategorySelectedDataGridView.Rows.Count > 0)
            {
                for (int intRow = 0; intRow < this.pvtEmployeeDataView.Count; intRow++)
                {
                    if (Convert.ToInt32(this.pvtEmployeeDataView[intRow]["PAY_CATEGORY_NO"]) == Convert.ToInt32(this.pvtPayCategoryDataView[Convert.ToInt32(this.dgvPayCategorySelectedDataGridView[2,this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategorySelectedDataGridView)].Value)]["PAY_CATEGORY_NO"]))
                    {
                        this.dgvEmployeeDataGridView.Rows.Add(this.pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                              this.pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                              this.pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                              intRow.ToString());

                        pvtTempDataView = null;
                        pvtTempDataView = new DataView(pvtDataSet.Tables["UserEmployee"],
                            "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + this.pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtTempDataView.Count == 0)
                        {
                            this.dgvEmployeeNotLinkedDataGridView.Rows.Add(this.pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                           this.pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                           this.pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                           intRow.ToString());
                        }
                    }
                }

                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["UserPayCategory"],
                    "COMPANY_NO = " + this.pvtint64CompanyNo + " AND USER_NO = " + this.pvtintUserNo + " AND PAY_CATEGORY_NO = " + Convert.ToInt32(this.pvtPayCategoryDataView[Convert.ToInt32(this.dgvPayCategorySelectedDataGridView[2,this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategorySelectedDataGridView)].Value)]["PAY_CATEGORY_NO"]),
                    "",
                    DataViewRowState.CurrentRows);

                if (pvtTempDataView.Count == 1)
                {
                    pvtTempDataView[0].Delete();
                }

                DataGridViewRow myDataGridViewRow = this.dgvPayCategorySelectedDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvPayCategorySelectedDataGridView)];

                this.dgvPayCategorySelectedDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvPayCategoryDataGridView.Rows.Add(myDataGridViewRow);
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEmployeeSelectedDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow;

                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["UserEmployee"],
                    "COMPANY_NO = " + this.pvtint64CompanyNo + " AND USER_NO = " + this.pvtintUserNo + " AND EMPLOYEE_NO = " + Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeSelectedDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeSelectedDataGridView)].Value)]["EMPLOYEE_NO"]),
                    "",
                    DataViewRowState.CurrentRows);

                if (pvtTempDataView.Count == 1)
                {
                    pvtTempDataView[0].Delete();
                }

                //NB Exclude Current User (Might be Linked)
                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["UserEmployee"],
                    "COMPANY_NO = " + this.pvtint64CompanyNo + " AND USER_NO <> " + this.pvtintUserNo + " AND EMPLOYEE_NO = " + Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeSelectedDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeSelectedDataGridView)].Value)]["EMPLOYEE_NO"]).ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                if (pvtTempDataView.Count == 0)
                {
                    int intRow = Convert.ToInt32(this.dgvEmployeeSelectedDataGridView[3,Get_DataGridView_SelectedRowIndex(this.dgvEmployeeSelectedDataGridView)].Value);
                   
                    this.dgvEmployeeNotLinkedDataGridView.Rows.Add(this.pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                   this.pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                   this.pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                   intRow.ToString());
                }

                myDataGridViewRow = this.dgvEmployeeSelectedDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeSelectedDataGridView)];

                this.dgvEmployeeSelectedDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);
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

        private void btnDepartmentAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvDepartmentDataGridView.Rows.Count > 0)
            {
                for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
                {
                    if (Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentDataGridView[3,this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)].Value)]["PAY_CATEGORY_NO"]) == Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[3,intRow].Value)]["PAY_CATEGORY_NO"])
                    && this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)].Value)]["PAY_CATEGORY_TYPE"].ToString() == this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[3, intRow].Value)]["PAY_CATEGORY_TYPE"].ToString()
                    && Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)].Value)]["DEPARTMENT_NO"]) == Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[3, intRow].Value)]["DEPARTMENT_NO"]))
                    {
                        this.dgvEmployeeDataGridView.Rows.RemoveAt(intRow);
                        intRow -= 1;
                    }
                }

                for (int intRow = 0; intRow < this.dgvEmployeeSelectedDataGridView.Rows.Count; intRow++)
                {
                    if (Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)].Value)]["PAY_CATEGORY_NO"]) == Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeSelectedDataGridView[3, intRow].Value)]["PAY_CATEGORY_NO"])
                    && this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)].Value)]["PAY_CATEGORY_TYPE"].ToString() == this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeSelectedDataGridView[3, intRow].Value)]["PAY_CATEGORY_TYPE"].ToString()
                    && Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)].Value)]["DEPARTMENT_NO"]) == Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeSelectedDataGridView[3, intRow].Value)]["DEPARTMENT_NO"]))
                    {
                        pvtTempDataView = null;
                        pvtTempDataView = new DataView(pvtDataSet.Tables["UserEmployee"],
                            "COMPANY_NO = " + this.pvtint64CompanyNo + " AND USER_NO = " + this.pvtintUserNo + " AND EMPLOYEE_NO = " + Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeSelectedDataGridView[3,this.Get_DataGridView_SelectedRowIndex(this.dgvEmployeeSelectedDataGridView)].Value)]["EMPLOYEE_NO"]),
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtTempDataView.Count == 1)
                        {
                            pvtTempDataView[0].Delete();
                        }

                        this.dgvEmployeeSelectedDataGridView.Rows.RemoveAt(intRow);
                        intRow -= 1;
                    }
                }

                for (int intRow = 0; intRow < this.dgvEmployeeNotLinkedDataGridView.Rows.Count; intRow++)
                {
                    if (Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)].Value)]["PAY_CATEGORY_NO"]) == Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeNotLinkedDataGridView[3, intRow].Value)]["PAY_CATEGORY_NO"])
                    && this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)].Value)]["PAY_CATEGORY_TYPE"].ToString() == this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeNotLinkedDataGridView[3, intRow].Value)]["PAY_CATEGORY_TYPE"].ToString()
                    && Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentDataGridView[3,this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)].Value)]["DEPARTMENT_NO"]) == Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeNotLinkedDataGridView[3, intRow].Value)]["DEPARTMENT_NO"]))
                    {
                        this.dgvEmployeeNotLinkedDataGridView.Rows.RemoveAt(intRow);
                        intRow -= 1;
                    }
                }

                pvtdrvDataRowView = pvtUserDepartmentDataView.AddNew();
                pvtdrvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                pvtdrvDataRowView["USER_NO"] = pvtintUserNo;
                pvtdrvDataRowView["PAY_CATEGORY_NO"] = Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)].Value)]["PAY_CATEGORY_NO"]);
                pvtdrvDataRowView["PAY_CATEGORY_TYPE"] = this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)].Value)]["PAY_CATEGORY_TYPE"].ToString();
                pvtdrvDataRowView["DEPARTMENT_NO"] = Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)].Value)]["DEPARTMENT_NO"]);
                pvtdrvDataRowView["TIE_BREAKER"] = 0;
                pvtdrvDataRowView.EndEdit();

                DataGridViewRow myDataGridViewRow = this.dgvDepartmentDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView)];

                this.dgvDepartmentDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvDepartmentSelectedDataGridView.Rows.Add(myDataGridViewRow);
            }
        }

        private void btnDepartmentRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvDepartmentSelectedDataGridView.Rows.Count > 0)
            {
                this.pvtblnEmployeeDataGridViewLoaded = false;

                for (int intRow = 0; intRow < this.pvtEmployeeDataView.Count; intRow++)
                {
                    if (Convert.ToInt32(this.pvtEmployeeDataView[intRow]["PAY_CATEGORY_NO"]) == Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentSelectedDataGridView[3,this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentSelectedDataGridView)].Value)]["PAY_CATEGORY_NO"])
                        && this.pvtEmployeeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() == this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentSelectedDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentSelectedDataGridView)].Value)]["PAY_CATEGORY_TYPE"].ToString()
                        && Convert.ToInt32(this.pvtEmployeeDataView[intRow]["DEPARTMENT_NO"]) == Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentSelectedDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentSelectedDataGridView)].Value)]["DEPARTMENT_NO"]))
                        {

                        this.dgvEmployeeDataGridView.Rows.Add(this.pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                              this.pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                              this.pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                              intRow.ToString());

                        pvtTempDataView = null;
                        pvtTempDataView = new DataView(pvtDataSet.Tables["UserEmployee"],
                            "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + this.pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtTempDataView.Count == 0)
                        {
                            this.dgvEmployeeNotLinkedDataGridView.Rows.Add(this.pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                           this.pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                           this.pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                           intRow.ToString());
                        }
                    }
                }

                this.pvtblnEmployeeDataGridViewLoaded = true;

                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["UserDepartment"],
                    "COMPANY_NO = " + this.pvtint64CompanyNo + " AND USER_NO = " + this.pvtintUserNo +
                    " AND PAY_CATEGORY_NO = " + Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentSelectedDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentSelectedDataGridView)].Value)]["PAY_CATEGORY_NO"]) + " AND PAY_CATEGORY_TYPE = '" + this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentSelectedDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentSelectedDataGridView)].Value)]["PAY_CATEGORY_TYPE"].ToString() + "' AND DEPARTMENT_NO = " + Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentSelectedDataGridView[3,this.Get_DataGridView_SelectedRowIndex(this.dgvDepartmentSelectedDataGridView)].Value)]["DEPARTMENT_NO"]),
                    "",
                    DataViewRowState.CurrentRows);

                if (pvtTempDataView.Count == 1)
                {
                    pvtTempDataView[0].Delete();
                }

                DataGridViewRow myDataGridViewRow = this.dgvDepartmentSelectedDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvDepartmentSelectedDataGridView)];

                this.dgvDepartmentSelectedDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvDepartmentDataGridView.Rows.Add(myDataGridViewRow);
            }
        }

        private void dgvUserDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvUserDataGridView.Rows.Count > 0
                 & pvtblnUserDataGridViewLoaded == true)
            {
                pvtintUserNo = Convert.ToInt32(this.pvtDataView[Convert.ToInt32(this.dgvUserDataGridView[3,e.RowIndex].Value)]["USER_NO"]);

                Load_Users_Companys();
            }
        }

        private void dgvCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvCompanyDataGridView.Rows.Count > 0
                 & pvtblnCompanyDataGridViewLoaded == true)
            {
                try
                {
                    pvtint64CompanyNo = Convert.ToInt32(this.pvtDataSet.Tables["Company"].Rows[Convert.ToInt32(this.dgvCompanyDataGridView[1,e.RowIndex].Value)]["COMPANY_NO"]);

                    this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
                    this.Clear_DataGridView(this.dgvPayCategorySelectedDataGridView);

                    this.Clear_DataGridView(this.dgvDepartmentDataGridView);
                    this.Clear_DataGridView(this.dgvDepartmentSelectedDataGridView);

                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                    this.Clear_DataGridView(this.dgvEmployeeSelectedDataGridView);

                    this.Clear_DataGridView(this.dgvEmployeeNotLinkedDataGridView);

                    pvtblnPayCategoryDataGridViewLoaded = false;
                    pvtblnPayCategorySelectedDataGridViewLoaded = false;

                    pvtblnDepartmentDataGridViewLoaded = false;
                    pvtblnDepartmentSelectedDataGridViewLoaded = false;

                    this.pvtblnEmployeeDataGridViewLoaded = false;
                    this.pvtblnEmployeeSelectedDataGridViewLoaded = false;

                    pvtblnEmployeeNotLinkedDataGridViewLoaded = false;
                  
                    DataView myCompanyLoadedDataView = new DataView(pvtDataSet.Tables["CompanyLoaded"],
                        "COMPANY_NO = " + this.pvtint64CompanyNo,
                        "",
                        DataViewRowState.CurrentRows);

                    if (myCompanyLoadedDataView.Count == 0)
                    {
                        //Stops Duplicate Firing
                        if (pvtint64CompanyNoSaved != pvtint64CompanyNo)
                        {
                            pvtint64CompanyNoSaved = pvtint64CompanyNo;

                            object[] objParm = new object[1];
                            objParm[0] = pvtint64CompanyNo;

                            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Company_CostCentres_Departments_Employees", objParm);

                            pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                            pvtDataSet.Merge(pvtTempDataSet);
                        }
                    }

                    pvtEmployeeDataView = null;
                    pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                        "COMPANY_NO = " + this.pvtint64CompanyNo,
                        "",
                        DataViewRowState.CurrentRows);

                    pvtPayCategoryDataView = null;
                    pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                        "COMPANY_NO = " + this.pvtint64CompanyNo,
                        "",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < this.pvtPayCategoryDataView.Count; intRow++)
                    {
                        pvtUserPayCategoryDataView = null;
                        pvtUserPayCategoryDataView = new DataView(pvtDataSet.Tables["UserPayCategory"],
                            "COMPANY_NO = " + this.pvtint64CompanyNo + " AND USER_NO = " + this.pvtintUserNo + " AND PAY_CATEGORY_NO = " + this.pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtUserPayCategoryDataView.Count == 0)
                        {
                            this.dgvPayCategoryDataGridView.Rows.Add(this.pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                     this.pvtPayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString(),
                                                                     intRow.ToString());
                        }
                        else
                        {
                            this.dgvPayCategorySelectedDataGridView.Rows.Add(this.pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                             this.pvtPayCategoryDataView[intRow]["PAY_CATEGORY_TYPE"].ToString(),
                                                                             intRow.ToString());
                        }
                    }

                    pvtDepartmentDataView = null;
                    pvtDepartmentDataView = new DataView(pvtDataSet.Tables["Department"],
                        "COMPANY_NO = " + this.pvtint64CompanyNo,
                        "",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < this.pvtDepartmentDataView.Count; intRow++)
                    {
                        pvtUserDepartmentDataView = null;
                        pvtUserDepartmentDataView = new DataView(pvtDataSet.Tables["UserDepartment"],
                            "COMPANY_NO = " + this.pvtint64CompanyNo + " AND USER_NO = " + this.pvtintUserNo + " AND PAY_CATEGORY_NO = " + this.pvtDepartmentDataView[intRow]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + this.pvtDepartmentDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() + "' AND DEPARTMENT_NO = " + this.pvtDepartmentDataView[intRow]["DEPARTMENT_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtUserDepartmentDataView.Count == 0)
                        {
                            this.dgvDepartmentDataGridView.Rows.Add(this.pvtDepartmentDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                    this.pvtDepartmentDataView[intRow]["DEPARTMENT_DESC"].ToString(),
                                                                    this.pvtDepartmentDataView[intRow]["PAY_CATEGORY_TYPE"].ToString(),
                                                                    intRow.ToString());
                        }
                        else
                        {
                            this.dgvDepartmentSelectedDataGridView.Rows.Add(this.pvtDepartmentDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                            this.pvtDepartmentDataView[intRow]["DEPARTMENT_DESC"].ToString(),
                                                                            this.pvtDepartmentDataView[intRow]["PAY_CATEGORY_TYPE"].ToString(),
                                                                            intRow.ToString());
                        }
                    }

                    bool blnFound = false;

                    for (int intRow = 0; intRow < this.pvtEmployeeDataView.Count; intRow++)
                    {
                        blnFound = false;

                        for (int intPayCategoryRow = 0; intPayCategoryRow < this.dgvPayCategorySelectedDataGridView.Rows.Count; intPayCategoryRow++)
                        {
                            if (Convert.ToInt32(this.pvtEmployeeDataView[intRow]["PAY_CATEGORY_NO"]) == Convert.ToInt32(this.pvtPayCategoryDataView[Convert.ToInt32(this.dgvPayCategorySelectedDataGridView[2,intPayCategoryRow].Value)]["PAY_CATEGORY_NO"]))
                            {
                                blnFound = true;
                                break;
                            }
                        }

                        for (int intDepartmentRow = 0; intDepartmentRow < this.dgvDepartmentSelectedDataGridView.Rows.Count; intDepartmentRow++)
                        {
                            if (Convert.ToInt32(this.pvtEmployeeDataView[intRow]["PAY_CATEGORY_NO"]) == Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentSelectedDataGridView[3, intDepartmentRow].Value)]["PAY_CATEGORY_NO"])
                                && this.pvtEmployeeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() == this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentSelectedDataGridView[3, intDepartmentRow].Value)]["PAY_CATEGORY_TYPE"].ToString()
                                && Convert.ToInt32(this.pvtEmployeeDataView[intRow]["DEPARTMENT_NO"]) == Convert.ToInt32(this.pvtDepartmentDataView[Convert.ToInt32(this.dgvDepartmentSelectedDataGridView[3,intDepartmentRow].Value)]["DEPARTMENT_NO"]))
                            {
                                blnFound = true;
                                break;
                            }
                        }

                        if (blnFound == true)
                        {
                            continue;
                        }

                        pvtUserEmployeeDataView = null;
                        pvtUserEmployeeDataView = new DataView(pvtDataSet.Tables["UserEmployee"],
                            "COMPANY_NO = " + this.pvtint64CompanyNo + " AND USER_NO = " + this.pvtintUserNo + " AND EMPLOYEE_NO = " + this.pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtUserEmployeeDataView.Count == 0)
                        {
                            this.dgvEmployeeDataGridView.Rows.Add(this.pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                  this.pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                  this.pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                  intRow.ToString());

                            pvtTempDataView = null;
                            pvtTempDataView = new DataView(pvtDataSet.Tables["UserEmployee"],
                                "COMPANY_NO = " + this.pvtint64CompanyNo + " AND EMPLOYEE_NO = " + this.pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                "",
                                DataViewRowState.CurrentRows);

                            if (pvtTempDataView.Count == 0)
                            {
                                this.dgvEmployeeNotLinkedDataGridView.Rows.Add(this.pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                               this.pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                               this.pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                                intRow.ToString());
                            }
                        }
                        else
                        {
                            this.dgvEmployeeSelectedDataGridView.Rows.Add(this.pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                          this.pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                          this.pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                          intRow.ToString());

                        }
                    }

                    pvtblnPayCategoryDataGridViewLoaded = true;
                    pvtblnPayCategorySelectedDataGridViewLoaded = true;

                    pvtblnDepartmentDataGridViewLoaded = true;
                    pvtblnDepartmentSelectedDataGridViewLoaded = true;
                
                    this.pvtblnEmployeeDataGridViewLoaded = true;
                    this.pvtblnEmployeeSelectedDataGridViewLoaded = true;

                    pvtblnEmployeeNotLinkedDataGridViewLoaded = true;

                    if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
                    }

                    if (this.dgvPayCategorySelectedDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategorySelectedDataGridView, 0);
                    }

                    if (this.dgvDepartmentDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvDepartmentDataGridView, 0);
                    }

                    if (this.dgvDepartmentSelectedDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvDepartmentSelectedDataGridView, 0);
                    }

                    if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, 0);
                    }

                    if (this.dgvEmployeeSelectedDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeSelectedDataGridView, 0);
                    }

                    if (this.dgvEmployeeNotLinkedDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvEmployeeNotLinkedDataGridView, 0);
                    }
                }
                catch (Exception eException)
                {
                    clsISUtilities.ErrorHandler(eException);
                }
            }
        }

        private void dgvEmployeeNotLinkedDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEmployeeNotLinkedDataGridView.Rows.Count > 0
            & this.pvtblnEmployeeNotLinkedDataGridViewLoaded == true)
            {

            }
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEmployeeDataGridView.Rows.Count > 0
            & pvtblnUserDataGridViewLoaded == true)
            {
                pvtintEmployeeNo = Convert.ToInt32(this.pvtEmployeeDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[3,e.RowIndex].Value)]["EMPLOYEE_NO"]);
            }
        }

        private void dgvEmployeeSelectedDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEmployeeSelectedDataGridView.Rows.Count > 0
            & this.pvtblnEmployeeSelectedDataGridViewLoaded == true)
            {

            }
        }

        private void dgvDepartmentDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDepartmentDataGridView.Rows.Count > 0
            & this.pvtblnDepartmentDataGridViewLoaded == true)
            {

            }
        }

        private void dgvDepartmentSelectedDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDepartmentSelectedDataGridView.Rows.Count > 0
            & this.pvtblnDepartmentSelectedDataGridViewLoaded == true)
            {

            }
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPayCategoryDataGridView.Rows.Count > 0
            & this.pvtblnPayCategoryDataGridViewLoaded == true)
            {

            }
        }

        private void dgvPayCategorySelectedDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPayCategorySelectedDataGridView.Rows.Count > 0
            & this.pvtblnPayCategorySelectedDataGridViewLoaded == true)
            {

            }
        }

        private void dgvPayCategoryDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnPayCategoryAdd_Click(sender, e);
            }
        }

        private void dgvPayCategorySelectedDataGridView_DoubleClick(object sender, EventArgs e)
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

        private void dgvDepartmentSelectedDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnDepartmentRemove_Click(sender, e);
            }
        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnAdd_Click(sender, e);
            }
        }

        private void dgvEmployeeSelectedDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnRemove_Click(sender, e);
            }
        }
    }
}
