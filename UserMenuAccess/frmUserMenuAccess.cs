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
    public partial class frmUserMenuAccess : Form
    {
        clsISUtilities clsISUtilities;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtUserDataView;
        private DataView pvtUserCompanyDataView;
        private DataView pvtUserMenuDataView;
        private DataRowView pvtDataRowView;

        private Int64 pvtint64UserNo = -1;
        private Int64 pvtint64CompanyNo = -1;
        private int pvtintUserDataViewIndex;
        private int pvtintFindRow = -1;
        private byte[] pvtbytCompress;

        private bool pvtblnUserDataGridViewLoaded = false;
        private bool pvtblnCompanyDataGridViewLoaded = false;
        private bool pvtblnMenuItemDataGridViewLoaded = false;

        public frmUserMenuAccess()
        {
            InitializeComponent();
        }

        private void frmUserMenuAccess_Load(object sender, System.EventArgs e)
        {
            try
            {
                //This Must Replace TreeView First Row (Happens When GUI is Changed)
                //AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj0yLjAuMC4w

                clsISUtilities = new clsISUtilities(this,"busUserMenuAccess");

                this.lblUserSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCompanySpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblMenuStructureSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblMenuAccessSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                Build_Menu_Structure();

                pvtDataSet = new DataSet();

                object[] objParm = new object[3];
                objParm[0] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Users", objParm);
                
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                Load_Users();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_Users()
        {
            int intRowIndex = 0;

            pvtUserDataView = null;
            pvtUserDataView = new DataView(pvtDataSet.Tables["User"]
                , ""
                , "SURNAME"
                , DataViewRowState.CurrentRows);

            this.Clear_DataGridView(this.dgvUserDataGridView);

            this.pvtblnUserDataGridViewLoaded = false;

            for (int intRow = 0; intRow < this.pvtUserDataView.Count; intRow++)
            {
                this.dgvUserDataGridView.Rows.Add(pvtUserDataView[intRow]["USER_ID"].ToString(),
                                                  pvtUserDataView[intRow]["SURNAME"].ToString(),
                                                  pvtUserDataView[intRow]["FIRSTNAME"].ToString(),
                                                  intRow.ToString());

                if (pvtint64UserNo == Convert.ToInt64(pvtUserDataView[intRow]["USER_NO"]))
                {
                    intRowIndex = intRow;
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
            pvtUserCompanyDataView = null;
            pvtUserCompanyDataView = new DataView(pvtDataSet.Tables["UserCompany"],
                "USER_NO = " + pvtint64UserNo,
                "",
                DataViewRowState.CurrentRows);

            this.Clear_DataGridView(this.dgvCompanyDataGridView);

            this.pvtblnCompanyDataGridViewLoaded = false;

            for (int intRow = 0; intRow < this.pvtUserCompanyDataView.Count; intRow++)
            {
                for (int intCompanyRow = 0; intCompanyRow < this.pvtDataSet.Tables["Company"].Rows.Count; intCompanyRow++)
                {
                    if (Convert.ToInt32(pvtUserCompanyDataView[intRow]["COMPANY_NO"]) == Convert.ToInt32(this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_NO"]))
                    {
                        this.dgvCompanyDataGridView.Rows.Add(this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_DESC"].ToString(),
                                                             intCompanyRow.ToString());
                        break;
                    }
                }
            }

            this.pvtblnCompanyDataGridViewLoaded = true;

            if (this.dgvCompanyDataGridView.Rows.Count > 0)
            {
                this.btnUpdate.Enabled = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvCompanyDataGridView, 0);
            }
            else
            {
                this.btnUpdate.Enabled = false;

                this.Clear_DataGridView(this.dgvMenuItemDataGridView);
                Clear_Nodes();
            }
        }

        private void Build_Menu_Structure()
        {
            TreeNode ndeLevel1;
            TreeNode ndeLevel2;
            TreeNode ndeLevel3;

            MenuStrip mainMenuStrip = (MenuStrip)AppDomain.CurrentDomain.GetData("MainMenuStrip");

            foreach (ToolStripMenuItem tsMenuItem in mainMenuStrip.Items)
            {
                if (tsMenuItem.GetType().Name == "ToolStripMenuItem")
                {
                    if (tsMenuItem.Text != "Edit"
                        && tsMenuItem.Text != "Tool"
                        && tsMenuItem.Text != "Password"
                        && tsMenuItem.Text != "Window"
                        && tsMenuItem.Text != "Activate"
                        && tsMenuItem.Text != "Payroll Run"
                        && tsMenuItem.Text != "Time Attendance Run"
                        && tsMenuItem.Text != "Help")
                    {
                        if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                        {
                            if (tsMenuItem.Text == "Setup")
                            {
                                continue;
                            }
                        }

                        ndeLevel1 = null;
                        ndeLevel1 = new TreeNode();
                        ndeLevel1.Text = tsMenuItem.Text;
                        //Doesn't Exist Therefore Blank
                        ndeLevel1.ImageIndex = 2;
                        ndeLevel1.SelectedImageIndex = 2;

                        this.tvwCompany.Nodes.Add(ndeLevel1);

                        for (int intItem1Count = 0; intItem1Count < tsMenuItem.DropDownItems.Count; intItem1Count++)
                        {
                            if (tsMenuItem.DropDownItems[intItem1Count].GetType().Name == "ToolStripMenuItem")
                            {
                                ToolStripMenuItem tsToolStripMenuItem1 = (ToolStripMenuItem)tsMenuItem.DropDownItems[intItem1Count];

                                ndeLevel2 = null;
                                ndeLevel2 = new TreeNode();

                                if (tsToolStripMenuItem1.Tag != null)
                                {
                                    if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "P")
                                    {
                                        if (tsToolStripMenuItem1.Name == "localWebServerToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "dataUploadTimesheetsToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "dataDownloadToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "salaryWageIncreaseToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "timesheetToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "timesheetBatchToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "leaveToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "timesheetAuthorisationToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "leaveAuthorisationToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "batchEarningToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "batchDeductionToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "rptPayslipToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "rptEarningDeductionToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "rptTimesheetToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "rptTimeSheetTotalToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "rptLeaveToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "rptleaveCalculatedTimeSheetToolStripMenuItem")
                                        {
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //Time Attendance Internet Program
                                        if (tsToolStripMenuItem1.Name == "localWebServerToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "dataUploadTimesheetsToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "dataDownloadToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "timesheetToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "timesheetBatchToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "timesheetAuthorisationToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "rptTimesheetToolStripMenuItem"
                                        || tsToolStripMenuItem1.Name == "rptTimesheetTotalToolStripMenuItem")
                                        {
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                ndeLevel2.Tag = tsToolStripMenuItem1.Tag.ToString();

                                ndeLevel2.Text = tsToolStripMenuItem1.Text;

                                if (tsToolStripMenuItem1.DropDownItems.Count > 0)
                                {
                                    ndeLevel2.SelectedImageIndex = 2;
                                }
                                else
                                {
                                    ndeLevel2.ImageIndex = 0;
                                    ndeLevel2.SelectedImageIndex = 0;
                                }

                                ndeLevel1.Nodes.Add(ndeLevel2);

                                for (int intItem2Count = 0; intItem2Count < tsToolStripMenuItem1.DropDownItems.Count; intItem2Count++)
                                {
                                    if (tsToolStripMenuItem1.DropDownItems[intItem2Count].GetType().Name == "ToolStripMenuItem")
                                    {
                                        ToolStripMenuItem tsToolStripMenuItem2 = (ToolStripMenuItem)tsToolStripMenuItem1.DropDownItems[intItem2Count];

                                        if (tsToolStripMenuItem2.Tag == null)
                                        {
                                            continue;
                                        }

                                        ndeLevel3 = null;
                                        ndeLevel3 = new TreeNode();

                                        ndeLevel3.Tag = tsToolStripMenuItem2.Tag.ToString();
                                        ndeLevel3.Text = tsToolStripMenuItem2.Text;

                                        if (tsToolStripMenuItem2.DropDownItems.Count > 0)
                                        {
                                            ndeLevel3.SelectedImageIndex = 2;
                                        }
                                        else
                                        {
                                            ndeLevel3.ImageIndex = 0;
                                            ndeLevel3.SelectedImageIndex = 0;
                                        }

                                        ndeLevel2.Nodes.Add(ndeLevel3);
                                    }
                                }
                            }
                        }
                    }
                }
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

                    case "dgvMenuItemDataGridView":

                        this.dgvMenuItemDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void tvwCompany_DoubleClick(object sender, System.EventArgs e)
        {
            if (this.tvwCompany.SelectedNode != null)
            {
                if (this.btnSave.Enabled == true
                    & this.tvwCompany.SelectedNode.GetNodeCount(true) == 0)
                {
                    if (this.tvwCompany.SelectedNode.ImageIndex == 1)
                    {
                        this.tvwCompany.SelectedNode.ImageIndex = 0;
                        this.tvwCompany.SelectedNode.SelectedImageIndex = 0;
                    }
                    else
                    {
                        this.tvwCompany.SelectedNode.ImageIndex = 1;
                        this.tvwCompany.SelectedNode.SelectedImageIndex = 1;
                    }
                }
            }
        }

        private void Set_MenuItem_Access()
        {
            this.Clear_DataGridView(this.dgvMenuItemDataGridView);

            this.pvtblnMenuItemDataGridViewLoaded = false;

            pvtUserMenuDataView = null;
            pvtUserMenuDataView = new DataView(pvtDataSet.Tables["UserMenu"],
                "USER_NO = " + pvtint64UserNo + " AND COMPANY_NO = " + pvtint64CompanyNo,
                "MENU_ITEM_ID",
                DataViewRowState.CurrentRows | DataViewRowState.Deleted);

            foreach (TreeNode ndeLevel1 in this.tvwCompany.Nodes)
            {
                if (ndeLevel1.GetNodeCount(true) > 0)
                {
                    foreach (TreeNode ndeLevel2 in ndeLevel1.Nodes)
                    {
                        if (ndeLevel2.GetNodeCount(true) == 0)
                        {
                            pvtintFindRow = this.pvtUserMenuDataView.Find(ndeLevel2.Tag.ToString());

                            if (pvtintFindRow == -1)
                            {
                                //No Access
                                ndeLevel2.ImageIndex = 0;
                                ndeLevel2.SelectedImageIndex = 0;
                            }
                            else
                            {
                                //Found
                                this.dgvMenuItemDataGridView.Rows.Add(ndeLevel2.Text,
                                                                      ndeLevel2.Tag.ToString());

                                ndeLevel2.ImageIndex = 1;
                                ndeLevel2.SelectedImageIndex = 1;
                            }
                        }
                        else
                        {
                            foreach (TreeNode ndeLevel3 in ndeLevel2.Nodes)
                            {
                                if (ndeLevel3.GetNodeCount(true) == 0)
                                {
                                    pvtintFindRow = this.pvtUserMenuDataView.Find(ndeLevel3.Tag.ToString());

                                    if (pvtintFindRow == -1)
                                    {
                                        ndeLevel3.ImageIndex = 0;
                                        ndeLevel3.SelectedImageIndex = 0;
                                    }
                                    else
                                    {
                                        //Found
                                        this.dgvMenuItemDataGridView.Rows.Add(ndeLevel3.Text,
                                                                              ndeLevel3.Tag.ToString());

                                       ndeLevel3.ImageIndex = 1;
                                       ndeLevel3.SelectedImageIndex = 1;
                                    }
                                }
                                else
                                {
                                    foreach (TreeNode ndeLevel4 in ndeLevel3.Nodes)
                                    {
                                        if (ndeLevel4.GetNodeCount(true) == 0)
                                        {
                                            pvtintFindRow = pvtUserMenuDataView.Find(ndeLevel4.Tag.ToString());

                                            if (pvtintFindRow == -1)
                                            {
                                                ndeLevel4.ImageIndex = 0;
                                                ndeLevel4.SelectedImageIndex = 0;
                                            }
                                            else
                                            {
                                                //Found
                                                this.dgvMenuItemDataGridView.Rows.Add(ndeLevel4.Text,
                                                                                      ndeLevel4.Tag.ToString());

                                                ndeLevel4.ImageIndex = 1;
                                                ndeLevel4.SelectedImageIndex = 1;
                                            }
                                        }
                                        else
                                        {
                                            CustomMessageBox.Show("Error in Set_MenuItems_Access",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            this.pvtblnMenuItemDataGridViewLoaded = true;

            if (this.dgvMenuItemDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvMenuItemDataGridView, 0);
            }
        }

        private void Clear_Nodes()
        {
            foreach (TreeNode ndeLevel1 in this.tvwCompany.Nodes)
            {
                if (ndeLevel1.GetNodeCount(true) > 0)
                {
                    foreach (TreeNode ndeLevel2 in ndeLevel1.Nodes)
                    {
                        if (ndeLevel2.GetNodeCount(true) == 0)
                        {
                            //No Access
                            ndeLevel2.ImageIndex = 0;
                            ndeLevel2.SelectedImageIndex = 0;
                        }
                        else
                        {
                            foreach (TreeNode ndeLevel3 in ndeLevel2.Nodes)
                            {
                                if (ndeLevel3.GetNodeCount(true) == 0)
                                {
                                    ndeLevel3.ImageIndex = 0;
                                    ndeLevel3.SelectedImageIndex = 0;
                                }
                                else
                                {
                                    foreach (TreeNode ndeLevel4 in ndeLevel3.Nodes)
                                    {
                                        if (ndeLevel4.GetNodeCount(true) == 0)
                                        {
                                            ndeLevel4.ImageIndex = 0;
                                            ndeLevel4.SelectedImageIndex = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            this.grbInfo.Visible = true;

            this.btnUpdate.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.picUserLock.Visible = true;
            this.picCompanyLock.Visible = true;
            this.picMenuLock.Visible = true;

            this.dgvUserDataGridView.Enabled = false;
            this.dgvCompanyDataGridView.Enabled = false;

            this.Clear_DataGridView(this.dgvMenuItemDataGridView);

            tvwCompany.Focus();
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf(" - Update"));

            this.grbInfo.Visible = false;

            this.picUserLock.Visible = false;
            this.picCompanyLock.Visible = false;
            this.picMenuLock.Visible = false;

            this.btnUpdate.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.dgvUserDataGridView.Enabled = true;
            this.dgvCompanyDataGridView.Enabled = true;

            Set_MenuItem_Access();
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                foreach (TreeNode ndeLevel1 in this.tvwCompany.Nodes)
                {
                    if (ndeLevel1.GetNodeCount(true) > 0)
                    {
                        foreach (TreeNode ndeLevel2 in ndeLevel1.Nodes)
                        {
                            if (ndeLevel2.GetNodeCount(true) == 0)
                            {
                                pvtintFindRow = pvtUserMenuDataView.Find(ndeLevel2.Tag.ToString());

                                if (ndeLevel2.ImageIndex == 1)
                                {
                                    if (pvtintFindRow == -1)
                                    {
                                        pvtDataRowView = pvtUserMenuDataView.AddNew();

                                        pvtDataRowView.BeginEdit();

                                        pvtDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                                        pvtDataRowView["USER_NO"] = pvtint64UserNo;
                                        pvtDataRowView["MENU_ITEM_ID"] = ndeLevel2.Tag.ToString();

                                        pvtDataRowView["ACCESS_IND"] = "A";
                                        
                                        pvtDataRowView.EndEdit();
                                    }
                                }
                                else
                                {
                                    if (pvtintFindRow > -1)
                                    {
                                        pvtUserMenuDataView[pvtintFindRow].Delete();
                                    }
                                }
                            }
                            else
                            {
                                foreach (TreeNode ndeLevel3 in ndeLevel2.Nodes)
                                {
                                    if (ndeLevel3.GetNodeCount(true) == 0)
                                    {
                                        pvtintFindRow = pvtUserMenuDataView.Find(ndeLevel3.Tag.ToString());

                                        if (ndeLevel3.ImageIndex == 1)
                                        {
                                            if (pvtintFindRow == -1)
                                            {
                                                pvtDataRowView = pvtUserMenuDataView.AddNew();

                                                pvtDataRowView.BeginEdit();

                                                pvtDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                                                pvtDataRowView["USER_NO"] = pvtint64UserNo;
                                                pvtDataRowView["MENU_ITEM_ID"] = ndeLevel3.Tag.ToString();

                                                pvtDataRowView["ACCESS_IND"] = "A";
                                                
                                                pvtDataRowView.EndEdit();
                                            }
                                        }
                                        else
                                        {
                                            if (pvtintFindRow > -1)
                                            {
                                                pvtUserMenuDataView[pvtintFindRow].Delete();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (TreeNode ndeLevel4 in ndeLevel3.Nodes)
                                        {
                                            if (ndeLevel4.GetNodeCount(true) == 0)
                                            {
                                                if (ndeLevel4.ImageIndex == 1)
                                                {
                                                    pvtintFindRow = pvtUserMenuDataView.Find(ndeLevel4.Tag.ToString());

                                                    if (pvtintFindRow == -1)
                                                    {
                                                        pvtDataRowView = pvtUserMenuDataView.AddNew();

                                                        pvtDataRowView.BeginEdit();

                                                        pvtDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                                                        pvtDataRowView["USER_NO"] = pvtint64UserNo;
                                                        pvtDataRowView["MENU_ITEM_ID"] = ndeLevel4.Tag.ToString();

                                                        pvtDataRowView["ACCESS_IND"] = "A";
                                                        
                                                        pvtDataRowView.EndEdit();
                                                    }
                                                }
                                                else
                                                {
                                                    if (pvtintFindRow > -1)
                                                    {
                                                        pvtUserMenuDataView[pvtintFindRow].Delete();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                pvtTempDataSet = null;
                pvtTempDataSet = new DataSet();

                DataTable myDataTable = this.pvtDataSet.Tables["UserMenu"].Clone();
                pvtTempDataSet.Tables.Add(myDataTable);

                for (int intRow = 0; intRow < this.pvtUserMenuDataView.Count; intRow++)
                {
                    if (this.pvtUserMenuDataView[intRow].Row.RowState == DataRowState.Added
                        | this.pvtUserMenuDataView[intRow].Row.RowState == DataRowState.Deleted
                        | this.pvtUserMenuDataView[intRow].Row.RowState == DataRowState.Modified)
                    {
                        pvtTempDataSet.Tables["UserMenu"].ImportRow(this.pvtUserMenuDataView[intRow].Row);
                    }
                }

                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                if (pvtTempDataSet.Tables["UserMenu"].Rows.Count > 0)
                {
                    object[] objParm = new object[3];

                    objParm[0] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = pvtbytCompress;

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_User_Menus", objParm,true);
                   
                    pvtUserMenuDataView = null;
                    pvtUserMenuDataView = new DataView(pvtDataSet.Tables["UserMenu"],
                        "USER_NO = " + pvtint64UserNo,
                        "MENU_ITEM_ID",
                        DataViewRowState.CurrentRows | DataViewRowState.Deleted);

                    for (int intRow = 0; intRow < this.pvtUserMenuDataView.Count; intRow++)
                    {
                        this.pvtUserMenuDataView[intRow].Delete();
                    }

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                    pvtDataSet.Merge(pvtTempDataSet);

                    pvtDataSet.AcceptChanges();
                }

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void tvwCompany_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            if (this.btnSave.Enabled == false
                & this.tvwCompany.SelectedNode.GetNodeCount(true) == 0)
            {
                if (this.tvwCompany.SelectedNode.ImageIndex == 2)
                {
                    //ERROL To Look At
                    //this.flxgMenuItem.Row = -1;
                }
                else
                {
                    for (int intRow = 0; intRow < this.dgvMenuItemDataGridView.Rows.Count; intRow++)
                    {
                        if (tvwCompany.SelectedNode.Tag.ToString() == this.dgvMenuItemDataGridView[1,intRow].Value.ToString())
                        {
                            this.Set_DataGridView_SelectedRowIndex(this.dgvMenuItemDataGridView, intRow);

                            break;
                        }
                    }
                }
            }
        }

        private void dgvUserDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvUserDataGridView.Rows.Count > 0
                & pvtblnUserDataGridViewLoaded == true)
            {
                try
                {
                    pvtintUserDataViewIndex = Convert.ToInt32(this.dgvUserDataGridView[3,e.RowIndex].Value);

                    pvtint64UserNo = Convert.ToInt64(this.pvtUserDataView[pvtintUserDataViewIndex]["USER_NO"]);

                    DataView myUserLoadedDataView = new DataView(pvtDataSet.Tables["UserLoaded"],
                        "USER_NO = " + pvtint64UserNo,
                        "",
                        DataViewRowState.CurrentRows);

                    if (myUserLoadedDataView.Count == 0)
                    {
                        object[] objParm = new object[2];
                        objParm[0] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                        objParm[1] = pvtint64UserNo;

                        pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_User_Menus", objParm);

                        pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                        pvtDataSet.Merge(pvtTempDataSet);
                    }

                    pvtUserMenuDataView = null;
                    pvtUserMenuDataView = new DataView(pvtDataSet.Tables["UserMenu"],
                        "USER_NO = " + pvtint64UserNo,
                        "MENU_ITEM_ID",
                        DataViewRowState.CurrentRows);

                    Load_Users_Companys();
                }
                catch (Exception eException)
                {
                    clsISUtilities.ErrorHandler(eException);
                }
            }
        }

        private void dgvCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCompanyDataGridView.Rows.Count > 0
                & pvtblnCompanyDataGridViewLoaded == true
                & this.dgvUserDataGridView.Rows.Count > 0)
            {
                pvtint64CompanyNo = Convert.ToInt64(this.pvtDataSet.Tables["Company"].Rows[Convert.ToInt32(this.dgvCompanyDataGridView[1,e.RowIndex].Value)]["COMPANY_NO"]);

                Set_MenuItem_Access();
            }
        }

        private void dgvMenuItemDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvMenuItemDataGridView.Rows.Count > 0
               & pvtblnMenuItemDataGridViewLoaded == true)
            {
                foreach (TreeNode ndeLevel1 in this.tvwCompany.Nodes)
                {
                    if (ndeLevel1.GetNodeCount(true) > 0)
                    {
                        foreach (TreeNode ndeLevel2 in ndeLevel1.Nodes)
                        {
                            if (ndeLevel2.GetNodeCount(true) == 0)
                            {
                                if (ndeLevel2.ImageIndex == 1)
                                {
                                    if (ndeLevel2.Tag.ToString() == this.dgvMenuItemDataGridView[1,e.RowIndex].Value.ToString())
                                    {
                                        this.tvwCompany.SelectedNode = ndeLevel2;
                                        this.tvwCompany.Focus();

                                        return;
                                    }
                                }
                            }
                            else
                            {
                                foreach (TreeNode ndeLevel3 in ndeLevel2.Nodes)
                                {
                                    if (ndeLevel3.GetNodeCount(true) == 0)
                                    {
                                        if (ndeLevel3.ImageIndex == 1)
                                        {
                                            if (ndeLevel3.Tag.ToString() == this.dgvMenuItemDataGridView[1, e.RowIndex].Value.ToString())
                                            {
                                                this.tvwCompany.SelectedNode = ndeLevel3;
                                                this.tvwCompany.Focus();

                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (TreeNode ndeLevel4 in ndeLevel3.Nodes)
                                        {
                                            if (ndeLevel4.GetNodeCount(true) == 0)
                                            {
                                                if (ndeLevel4.ImageIndex == 1)
                                                {
                                                    if (ndeLevel4.Tag.ToString() == this.dgvMenuItemDataGridView[1, e.RowIndex].Value.ToString())
                                                    {
                                                        this.tvwCompany.SelectedNode = ndeLevel4;
                                                        this.tvwCompany.Focus();

                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
