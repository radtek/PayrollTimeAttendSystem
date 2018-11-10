using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InteractPayroll;

namespace InteractPayrollClient
{
    public partial class frmClientConvertCostCentre : Form
    {
        clsISClientUtilities clsISClientUtilities;

        clsISUtilities clsISUtilities;

        ToolStripMenuItem miLinkedMenuItem;

        private DataSet pvtDataSet;
        private byte[] pvtbytCompress;

        private int pvtintCompanyDataGridViewRowIndex = -1;
        private bool pvtblnCompanyDataGridViewLoaded = false;

        private int pvtintConvertToPayrollTypeDataGridViewRowIndex = -1;
        private bool pvtblnConvertToPayrollTypeDataGridViewLoaded = false;

        private int pvtintPayCategoryDataGridViewRowIndex = -1;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;

        private int pvtintChosenPayCategoryDataGridViewRowIndex = -1;
        private bool pvtblnChosenPayCategoryDataGridViewLoaded = false;

        private Int64 pvtInt64CompanyNo = 0;
        private string pvtstrToPayCategoryType = "";

        private int pvtintProcessNo = 0;
        private bool pvtblnShowImage = false;

        private int pvtintTimeDelaySeconds = 7;

        public frmClientConvertCostCentre()
        {
            InitializeComponent();
        }

        private void frmClientConvertCostCentre_Load(object sender, EventArgs e)
        {
            try
            {
                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

                clsISClientUtilities = new clsISClientUtilities(this, "busClientConvertCostCentre");

                this.lblCompanySpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblPayrollTypeHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblPayCategoryDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblPayCategorySelectDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                this.dgvConvertToPayrollTypeDataGridView.Rows.Add("Wages");
                this.dgvConvertToPayrollTypeDataGridView.Rows.Add("TimeSheets");
                this.dgvConvertToPayrollTypeDataGridView.Rows.Add("Salaries");

                pvtblnConvertToPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvConvertToPayrollTypeDataGridView, 0);

                try
                {
                    clsISUtilities = new InteractPayroll.clsISUtilities(this, "busConvertCostCentre");
                }
                catch (Exception ex)
                {
                    CustomClientMessageBox.Show("Connection to Internet Database Could Not be Established", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                object[] objParm = new object[0];

                pvtbytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Form_Records", objParm,false);

                pvtDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                for (int intRow = 0; intRow < this.pvtDataSet.Tables["Company"].Rows.Count; intRow++)
                {
                    this.dgvCompanyDataGridView.Rows.Add(this.pvtDataSet.Tables["Company"].Rows[intRow]["COMPANY_DESC"].ToString(),
                                                         this.pvtDataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                }

                pvtblnCompanyDataGridViewLoaded = true;

                if (this.dgvCompanyDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvCompanyDataGridView, 0);
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
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
                    case "dgvCompanyDataGridView":

                        pvtintCompanyDataGridViewRowIndex = -1;
                        this.dgvCompanyDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvConvertToPayrollTypeDataGridView":

                        pvtintConvertToPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvConvertToPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        pvtintPayCategoryDataGridViewRowIndex = -1;
                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvChosenPayCategoryDataGridView":

                        pvtintChosenPayCategoryDataGridViewRowIndex = -1;
                        this.dgvChosenPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void DataGridView_Sorted(object sender, EventArgs e)
        {

        }

        private void dgvCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnCompanyDataGridViewLoaded == true)
            {
                if (pvtintCompanyDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintCompanyDataGridViewRowIndex = e.RowIndex;

                    pvtInt64CompanyNo = Convert.ToInt64(dgvCompanyDataGridView[1,e.RowIndex].Value);

                    Load_CostCentres();
                }
            }
        }

        private void dgvConvertToPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnConvertToPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintConvertToPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintConvertToPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    pvtstrToPayCategoryType = dgvConvertToPayrollTypeDataGridView[e.ColumnIndex,e.RowIndex].Value.ToString().Substring(0,1); 

                    if (pvtDataSet != null)
                    {
                        Load_CostCentres();
                    }
                }
            }
        }

        private void Load_CostCentres()
        {
            try
            {
                if (pvtDataSet.Tables["PayCategory"] != null)
                {
                    pvtDataSet.Tables.Remove("PayCategory");
                }

                object[] objParm = new object[2];
                objParm[0] = pvtInt64CompanyNo;
                objParm[1] = pvtstrToPayCategoryType;

                pvtbytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Company_CostCentres", objParm,false);

                DataSet DataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                pvtDataSet.Merge(DataSet);

                this.Clear_DataGridView(dgvPayCategoryDataGridView);
                this.Clear_DataGridView(this.dgvChosenPayCategoryDataGridView);

                pvtblnPayCategoryDataGridViewLoaded = false;
                pvtblnChosenPayCategoryDataGridViewLoaded = false;

                for (int intRow = 0; intRow < this.pvtDataSet.Tables["PayCategory"].Rows.Count; intRow++)
                {
                    this.dgvPayCategoryDataGridView.Rows.Add(this.pvtDataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                             this.pvtDataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString(),
                                                             this.pvtDataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                }

                pvtblnPayCategoryDataGridViewLoaded = true;

                if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, 0);
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvChosenPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf(" - Update"));

            this.btnUpdate.Enabled = true;
        
            this.dgvCompanyDataGridView.Enabled = true;
            this.picCompanyLock.Visible = false;
            this.dgvConvertToPayrollTypeDataGridView.Enabled = true;
            this.picConvertPayrollTypeLock.Visible = false;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.btnPayCategoryAdd.Enabled = false;
            this.btnPayCategoryAddAll.Enabled = false;
            this.btnPayCategoryRemove.Enabled = false;
            this.btnPayCategoryRemoveAll.Enabled = false;

            this.tmrProcess.Enabled = false;
            this.grbConversionProcess.Visible = false;
                    
            this.Set_DataGridView_SelectedRowIndex(this.dgvConvertToPayrollTypeDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvConvertToPayrollTypeDataGridView));
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text = this.Text + " - Update";

            this.btnUpdate.Enabled = false;
            
            this.dgvCompanyDataGridView.Enabled = false;
            this.picCompanyLock.Visible = true;
            this.dgvConvertToPayrollTypeDataGridView.Enabled = false;
            this.picConvertPayrollTypeLock.Visible = true;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.btnPayCategoryAdd.Enabled = true;
            this.btnPayCategoryAddAll.Enabled = true;
            this.btnPayCategoryRemove.Enabled = true;
            this.btnPayCategoryRemoveAll.Enabled = true;
        }

        private void btnPayCategoryAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvPayCategoryDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)];

                this.dgvPayCategoryDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvChosenPayCategoryDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvChosenPayCategoryDataGridView.CurrentCell = this.dgvChosenPayCategoryDataGridView[0, this.dgvChosenPayCategoryDataGridView.Rows.Count - 1];
            }
        }

        private void btnPayCategoryRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvChosenPayCategoryDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)];

                this.dgvChosenPayCategoryDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvPayCategoryDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvPayCategoryDataGridView.CurrentCell = this.dgvPayCategoryDataGridView[0, this.dgvPayCategoryDataGridView.Rows.Count - 1];
            }
        }

        private void btnPayCategoryAddAll_Click(object sender, EventArgs e)
        {
            btnPayCategoryAddAll_Click_Continue:

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                btnPayCategoryAdd_Click(sender, e);

                goto btnPayCategoryAddAll_Click_Continue;
            }
        }

        private void btnPayCategoryRemoveAll_Click(object sender, EventArgs e)
        {
            btnPayCategoryRemoveAll_Click_Continue:

            if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
            {
                btnPayCategoryRemove_Click(sender, e);

                goto btnPayCategoryRemoveAll_Click_Continue;
            }
        }

        private void dgvPayCategoryDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnPayCategoryAdd_Click(sender, e);
            }
        }

        private void dgvChosenPayCategoryDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnPayCategoryRemove_Click(sender, e);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool blnServerDatabaseConverted = false;

            try
            {
                if (this.dgvChosenPayCategoryDataGridView.Rows.Count == 0)
                {
                    CustomClientMessageBox.Show("There are NO Selected Cost Centres.\nFix to Continue.",
                               this.Text,
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
                    return;
                }

                DialogResult dlgResult = CustomClientMessageBox.Show("Are you absolutely sure you want to Convert the Selected Cost Centre/s to '" + dgvConvertToPayrollTypeDataGridView[0, pvtintConvertToPayrollTypeDataGridViewRowIndex].Value.ToString() + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    DataSet DataSet = new DataSet();

                    DataTable myDataTable = this.pvtDataSet.Tables["PayCategory"].Clone();

                    for (int intRow = 0; intRow < this.dgvChosenPayCategoryDataGridView.Rows.Count; intRow++)
                    {
                        DataRow drDataRow = myDataTable.NewRow();

                        drDataRow["COMPANY_NO"] = pvtInt64CompanyNo;
                        drDataRow["PAY_CATEGORY_NO"] = Convert.ToInt32(this.dgvChosenPayCategoryDataGridView[2, intRow].Value);
                        drDataRow["PAY_CATEGORY_TYPE"] = this.dgvChosenPayCategoryDataGridView[1, intRow].Value.ToString();

                        myDataTable.Rows.Add(drDataRow);
                    }

                    DataSet.Tables.Add(myDataTable);

                    //Compress DataSet
                    pvtbytCompress = clsISClientUtilities.Compress_DataSet(DataSet);

                    object[]  objParm = new object[3];
                    objParm[0] = pvtInt64CompanyNo;
                    objParm[1] = pvtstrToPayCategoryType;
                    objParm[2] = pvtbytCompress;
                    
                    //Backup Internet Database
                    int intReturnCode = (int)clsISUtilities.DynamicFunction("Check_Run", objParm, false);

                    if (intReturnCode == 9)
                    {
                        CustomClientMessageBox.Show("You do NOT have permission to perform this operation.\nSpeak to System Administrator.",
                                        this.Text,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);

                        goto btnSave_Click_Continue;
                    }
                    else
                    {
                        if (intReturnCode == 1)
                        {
                            this.tmrProcess.Enabled = false;
                            this.picDatabaseConvertInternet.Image = global::ConvertCostCentre.Properties.Resources.Error;

                            CustomClientMessageBox.Show("There is Currently a Payroll/Time Attendance Run Date Open.\nFix to Continue.",
                                            this.Text,
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);

                            goto btnSave_Click_Continue;
                        }
                    }
                    
                    pvtintProcessNo = 0;
                    pvtblnShowImage = false;

                    this.picDatabaseBackupInternet.Image = null;
                    this.picDatabaseBackupLocal.Image = null;
                    this.picDatabaseConvertInternet.Image = null;
                    this.picDatabaseConvertLocal.Image = null;

                    this.grbConversionProcess.Visible = true;

                    this.Refresh();

                    this.tmrProcess.Enabled = true;
                    DateTime dtDateTimeNext = DateTime.Now.AddSeconds(pvtintTimeDelaySeconds);
                    
                    clsISUtilities.Set_New_BusinessObjectName("busBackupRestoreDatabase");

                    objParm = new object[2];
                    objParm[0] = pvtInt64CompanyNo;
                    objParm[1] = "_Backup_Before_CostCentre_Conversion";
                    
                    //Backup Internet Database
                    intReturnCode = (int)clsISUtilities.DynamicFunction("Backup_DataBase_New", objParm, false);
                    
                    if (intReturnCode != 0)
                    {
                        this.tmrProcess.Enabled = false;
                        this.picDatabaseBackupInternet.Image = global::ConvertCostCentre.Properties.Resources.Error;

                        CustomClientMessageBox.Show("Error Backing up Internet Database.\nFix to Continue.",
                                      this.Text,
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);

                        goto btnSave_Click_Continue;
                    }

                    while (DateTime.Now < dtDateTimeNext)
                    {
                        Application.DoEvents();
                    }

                    this.picDatabaseBackupInternet.Image = global::ConvertCostCentre.Properties.Resources.Ok;
                    this.pvtintProcessNo += 1;
                    dtDateTimeNext = DateTime.Now.AddSeconds(pvtintTimeDelaySeconds);
                    
                    //Reset Class
                    clsISUtilities.Set_New_BusinessObjectName("busConvertCostCentre");

                    this.pnlDatabaseLocalBackup.Visible = true;
                    this.Refresh();

                    clsISClientUtilities.Set_New_BusinessObjectName("busBackupRestoreClientDatabase");

                    objParm = new object[1];
                    objParm[0] = "_Backup_Before_CostCentre_Conversion";
                    
                    //Backup Client Database
                    intReturnCode = (int)clsISClientUtilities.DynamicFunction("Backup_DataBase_New", objParm, false);

                    if (intReturnCode != 0)
                    {
                        this.tmrProcess.Enabled = false;
                        this.picDatabaseBackupLocal.Image = global::ConvertCostCentre.Properties.Resources.Error;

                        CustomClientMessageBox.Show("Error Backing up Client Database.\nFix to Continue.",
                                      this.Text,
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);

                        goto btnSave_Click_Continue; 
                    }

                    while (DateTime.Now < dtDateTimeNext)
                    {
                        Application.DoEvents();
                    }

                    this.picDatabaseBackupLocal.Image = global::ConvertCostCentre.Properties.Resources.Ok;
                    this.pvtintProcessNo += 1;
                    dtDateTimeNext = DateTime.Now.AddSeconds(pvtintTimeDelaySeconds);

                    //Reset Class
                    clsISClientUtilities.Set_New_BusinessObjectName("busClientConvertCostCentre");

                    objParm = new object[3];
                    objParm[0] = pvtInt64CompanyNo;
                    objParm[1] = pvtstrToPayCategoryType;
                    objParm[2] = pvtbytCompress;

                    //Convert Server
                    intReturnCode = (int)clsISUtilities.DynamicFunction("Convert_Records", objParm, false);

                    if (intReturnCode == 0)
                    {
                        blnServerDatabaseConverted = true;

                        while (DateTime.Now < dtDateTimeNext)
                        {
                            Application.DoEvents();
                        }

                        this.picDatabaseConvertInternet.Image = global::ConvertCostCentre.Properties.Resources.Ok;
                        this.pvtintProcessNo += 1;
                        dtDateTimeNext = DateTime.Now.AddSeconds(pvtintTimeDelaySeconds);

                        //Convert Client Databases
                        clsISClientUtilities.DynamicFunction("Convert_Client_Records", objParm, true);

                        while (DateTime.Now < dtDateTimeNext)
                        {
                            Application.DoEvents();
                        }

                        this.tmrProcess.Enabled = false;
                        this.picDatabaseConvertLocal.Image = global::ConvertCostCentre.Properties.Resources.Ok;

                        CustomClientMessageBox.Show("Conversion Successful.",
                                     this.Text,
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    }

                btnSave_Click_Continue:
                 
                    btnCancel_Click(sender, e);
                 }
            }
            catch (Exception eException)
            {
                this.tmrProcess.Enabled = false;

                if (pvtintProcessNo == 0)
                {
                    this.picDatabaseBackupInternet.Image = global::ConvertCostCentre.Properties.Resources.Error;
                }
                else
                {
                    if (pvtintProcessNo == 1)
                    {
                        this.picDatabaseBackupLocal.Image = global::ConvertCostCentre.Properties.Resources.Error;
                    }
                    else
                    {
                        if (pvtintProcessNo == 2)
                        {
                            this.picDatabaseConvertInternet.Image = global::ConvertCostCentre.Properties.Resources.Error;
                        }
                        else
                        {
                            if (pvtintProcessNo == 3)
                            {
                                this.picDatabaseConvertLocal.Image = global::ConvertCostCentre.Properties.Resources.Error;
                            }
                        }
                    }
                }

                if (blnServerDatabaseConverted == true)
                {
                    CustomClientMessageBox.Show("Server Database Converted. / Local Database CONVERT FAILURE.\nURGENT - Speak to System Administrator.",
                                  this.Text,
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }

                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void tmrProcess_Tick(object sender, EventArgs e)
        {
            if (pvtintProcessNo == 0)
            {
                if (pvtblnShowImage == true)
                {
                    pvtblnShowImage = false;
                    this.picDatabaseBackupInternet.Image = null;
                }
                else
                {
                    pvtblnShowImage = true;
                    this.picDatabaseBackupInternet.Image = global::ConvertCostCentre.Properties.Resources.Question;
                }
            }
            else
            {
                if (pvtintProcessNo == 1)
                {
                    if (pvtblnShowImage == true)
                    {
                        pvtblnShowImage = false;
                        this.picDatabaseBackupLocal.Image = null;
                    }
                    else
                    {
                        pvtblnShowImage = true;
                        this.picDatabaseBackupLocal.Image = global::ConvertCostCentre.Properties.Resources.Question;
                    }
                }
                else
                {
                    if (pvtintProcessNo == 2)
                    {
                        if (pvtblnShowImage == true)
                        {
                            pvtblnShowImage = false;
                            this.picDatabaseConvertInternet.Image = null;
                        }
                        else
                        {
                            pvtblnShowImage = true;
                            this.picDatabaseConvertInternet.Image = global::ConvertCostCentre.Properties.Resources.Question;
                        }
                    }
                    else
                    {
                        if (pvtintProcessNo == 3)
                        {
                            if (pvtblnShowImage == true)
                            {
                                pvtblnShowImage = false;
                                this.picDatabaseConvertLocal.Image = null;
                            }
                            else
                            {
                                pvtblnShowImage = true;
                                this.picDatabaseConvertLocal.Image = global::ConvertCostCentre.Properties.Resources.Question;
                            }
                        }
                    }
                }
            }
        }

        private void frmClientConvertCostCentre_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }
    }
}
