using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InteractPayrollClient;

namespace InteractPayroll
{
    public partial class frmDataDownload : Form
    {
        clsISUtilities clsISUtilities;
        //Local
        clsISClientUtilities clsISClientUtilities;

        bool blnclsISClientUtilities = false;
 
        DataView pvtPayCategoryDataView;
     
        DataSet pvtDataSet;
        DataSet pvtDataSetClient;
        DataSet pvtTempDataSet;

        byte[] pvtbytCompress;

        private bool pvtblnCostCentreDataGridViewLoaded = false;
     
        public frmDataDownload()
        {
            InitializeComponent();
        }

        private void frmDataDownload_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new InteractPayroll.clsISUtilities(this, "busDataDownload");
                //Local
                clsISClientUtilities = new clsISClientUtilities(this, "busClientDataDownload");

                this.lblCostCentreSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedCostCentreSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentreDelete.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                
                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records_New", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_CurrentForm_Records()
        {
            try
            {
                this.btnOK.Enabled = false;

                this.Clear_DataGridView(this.dgvCostCentreDataGridView);
                this.Clear_DataGridView(this.dgvCostCentreChosenDataGridView);
                this.Clear_DataGridView(this.dgvCostCentreDeletedDataGridView);
                
                this.dgvCostCentreDataGridView.Height = 516;

                this.lblCostCentreDelete.Visible = false;
                this.dgvCostCentreDeletedDataGridView.Visible = false;
                this.btnAddDeleted.Visible = false;
              
                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                pvtbytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_PayCategory_Records_New", objParm, false);

                pvtDataSetClient = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                if (pvtDataSetClient == null)
                {
                    CustomMessageBox.Show("Connection to Local Database Could not be Established.",
                                     this.Text,
                                     MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);

                    this.Close();
                }

                string strPayCategoryTypeDesc = "";
                string strLastDownloadDateTime = "";
                string strLastDownloadDateTimeYYMMDDHHMMSS = "";

                pvtPayCategoryDataView = new DataView(this.pvtDataSet.Tables["PayCategory"]
                    , ""
                    , "PAY_CATEGORY_DESC,PAY_CATEGORY_TYPE DESC"
                    , DataViewRowState.CurrentRows);

                for (int intIndex = 0; intIndex < pvtPayCategoryDataView.Count; intIndex++)
                {
                    if (pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_TYPE"].ToString() == "W")
                    {
                        strPayCategoryTypeDesc = "Wages";
                    }
                    else
                    {
                        if (pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_TYPE"].ToString() == "S")
                        {
                            strPayCategoryTypeDesc = "Salaries";
                        }
                        else
                        {
                            strPayCategoryTypeDesc = "Time Attendance";
                        }
                    }

                    DataView PayCategoryClientDataView = new DataView(pvtDataSetClient.Tables["PayCategoryClient"]
                    , "PAY_CATEGORY_NO = " + pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_TYPE"].ToString() + "'"
                    , ""
                    , DataViewRowState.CurrentRows);

                    if (PayCategoryClientDataView.Count == 0)
                    {
                        this.dgvCostCentreDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                                "",
                                                                strPayCategoryTypeDesc,
                                                                pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString(),
                                                                "");
                    }
                    else
                    {
                        this.btnOK.Enabled = true;

                        if (PayCategoryClientDataView[0]["LAST_DOWNLOAD_DATETIME"] != System.DBNull.Value)
                        {
                            strLastDownloadDateTime = Convert.ToDateTime(PayCategoryClientDataView[0]["LAST_DOWNLOAD_DATETIME"]).ToString("dd MMM yyyy - HH:mm:ss");
                            strLastDownloadDateTimeYYMMDDHHMMSS = Convert.ToDateTime(PayCategoryClientDataView[0]["LAST_DOWNLOAD_DATETIME"]).ToString("yyyyMMddHHmmss");

                        }
                        else
                        {
                            strLastDownloadDateTime = "";
                            strLastDownloadDateTimeYYMMDDHHMMSS = "";
                        }

                        this.dgvCostCentreChosenDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                                      strLastDownloadDateTime,
                                                                      strPayCategoryTypeDesc,
                                                                      pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString(),
                                                                      strLastDownloadDateTimeYYMMDDHHMMSS);
                    }
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvCostCentreDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvCostCentreDataGridView.Rows[this.dgvCostCentreDataGridView.CurrentRow.Index];

                this.dgvCostCentreDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvCostCentreChosenDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvCostCentreChosenDataGridView.CurrentCell = this.dgvCostCentreChosenDataGridView[0, this.dgvCostCentreChosenDataGridView.Rows.Count - 1];

                this.btnOK.Enabled = true;
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvCostCentreChosenDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvCostCentreChosenDataGridView.Rows[this.dgvCostCentreChosenDataGridView.CurrentRow.Index];

                string strPayCategoryNo = myDataGridViewRow.Cells[3].Value.ToString();
                string strPayCategoryType = myDataGridViewRow.Cells[2].Value.ToString().Substring(0, 1);

                this.dgvCostCentreChosenDataGridView.Rows.Remove(myDataGridViewRow);

                DataView PayCategoryClientDataView = new DataView(pvtDataSetClient.Tables["PayCategoryClient"]
                , "PAY_CATEGORY_NO = " + strPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + strPayCategoryType + "'"
                , ""
                , DataViewRowState.CurrentRows);

                if (PayCategoryClientDataView.Count == 0)
                {
                    this.dgvCostCentreDataGridView.Rows.Add(myDataGridViewRow);
                    this.dgvCostCentreDataGridView.CurrentCell = this.dgvCostCentreDataGridView[0, this.dgvCostCentreDataGridView.Rows.Count - 1];
                }
                else
                {
                    if (this.dgvCostCentreDataGridView.Height == 516)
                    {
                        this.dgvCostCentreDataGridView.Height = 364;

                        this.lblCostCentreDelete.Visible = true;
                        this.dgvCostCentreDeletedDataGridView.Visible = true;
                        this.btnAddDeleted.Visible = true;
                    }

                    this.dgvCostCentreDeletedDataGridView.Rows.Add(myDataGridViewRow);
                    this.dgvCostCentreDeletedDataGridView.CurrentCell = this.dgvCostCentreDeletedDataGridView[0, this.dgvCostCentreDeletedDataGridView.Rows.Count - 1];
                }
            }
        }

        private void btnRemoveAll_Click(object sender, System.EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.dgvCostCentreChosenDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strPayCategoryFilterWages = " AND PAY_CATEGORY_NO IN (";
                string strPayCategoryFilterSalaries = " AND PAY_CATEGORY_NO IN (";
                string strPayCategoryFilterTimeAttendance = " AND PAY_CATEGORY_NO IN (";

                string strCostCentresWages = "";
                string strCostCentresSalaries = "";
                string strCostCentresTimeAttendance = "";
                object[] objFind = new object[3];

                object[] objParm;
                
                if (this.dgvCostCentreChosenDataGridView.Rows.Count == 0)
                {
                    DialogResult myDialogResult = CustomMessageBox.Show("Would you like to DELETE this Company From the Local Database?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                    if (myDialogResult == DialogResult.Yes)
                    {
                        objParm = new object[2];
                        objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        objParm[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                        blnclsISClientUtilities = true;
                        clsISClientUtilities.DynamicFunction("Delete_All_PayCategory_Records_New", objParm,false);

                        CustomMessageBox.Show("Delete from Local Database Successful.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                        Load_CurrentForm_Records();
                    }

                    return;
                }
                else
                {
                    for (int intRow = 0; intRow < this.dgvCostCentreChosenDataGridView.Rows.Count; intRow++)
                    {
                        if (dgvCostCentreChosenDataGridView[2, intRow].Value.ToString().Substring(0, 1) == "W")
                        {
                            if (strCostCentresWages == "")
                            {
                                strCostCentresWages = this.dgvCostCentreChosenDataGridView[3, intRow].Value.ToString();
                                strPayCategoryFilterWages += this.dgvCostCentreChosenDataGridView[3, intRow].Value.ToString();
                            }
                            else
                            {
                                strCostCentresWages += "|" + this.dgvCostCentreChosenDataGridView[3, intRow].Value.ToString();
                                strPayCategoryFilterWages += "," + this.dgvCostCentreChosenDataGridView[3, intRow].Value.ToString();
                            }
                        }
                        else
                        {
                            if (dgvCostCentreChosenDataGridView[2, intRow].Value.ToString().Substring(0, 1) == "S")
                            {
                                if (strCostCentresSalaries == "")
                                {
                                    strCostCentresSalaries = this.dgvCostCentreChosenDataGridView[3, intRow].Value.ToString();
                                    strPayCategoryFilterSalaries += this.dgvCostCentreChosenDataGridView[3, intRow].Value.ToString();
                                }
                                else
                                {
                                    strCostCentresSalaries += "|" + this.dgvCostCentreChosenDataGridView[3, intRow].Value.ToString();
                                    strPayCategoryFilterSalaries += "," + this.dgvCostCentreChosenDataGridView[3, intRow].Value.ToString();
                                }
                            }
                            else
                            {
                                if (strCostCentresTimeAttendance == "")
                                {
                                    strCostCentresTimeAttendance = this.dgvCostCentreChosenDataGridView[3, intRow].Value.ToString();
                                    strPayCategoryFilterTimeAttendance += this.dgvCostCentreChosenDataGridView[3, intRow].Value.ToString();
                                }
                                else
                                {
                                    strCostCentresTimeAttendance += "|" + this.dgvCostCentreChosenDataGridView[3, intRow].Value.ToString();
                                    strPayCategoryFilterTimeAttendance += "," + this.dgvCostCentreChosenDataGridView[3, intRow].Value.ToString();
                                }
                            }
                        }
                    }
                }

                if (strPayCategoryFilterWages != " AND PAY_CATEGORY_NO IN (")
                {
                    strPayCategoryFilterWages += ")";
                }
                else
                {
                    strPayCategoryFilterWages += "-1)";
                }

                if (strPayCategoryFilterSalaries != " AND PAY_CATEGORY_NO IN (")
                {
                    strPayCategoryFilterSalaries += ")";
                }
                else
                {
                    strPayCategoryFilterSalaries += "-1)";
                }

                if (strPayCategoryFilterTimeAttendance != " AND PAY_CATEGORY_NO IN (")
                {
                    strPayCategoryFilterTimeAttendance += ")";
                }
                else
                {
                    strPayCategoryFilterTimeAttendance += "-1)";
                }

                objParm = new object[4];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = strPayCategoryFilterWages;
                objParm[2] = strPayCategoryFilterSalaries;
                objParm[3] = strPayCategoryFilterTimeAttendance;

                blnclsISClientUtilities = true;
                byte[] bytClientDataSetCompress = (byte[])clsISClientUtilities.DynamicFunction("Check_PayCategory_Records_New", objParm, false);

                DataSet DataSetClient = clsISUtilities.DeCompress_Array_To_DataSet(bytClientDataSetCompress);

                if (DataSetClient.Tables["Temp"].Rows.Count > 0)
                {
                    string strPayCategoryDesc = "Cost Centre\n\n";
                    string strPayCategoryNoWages = "(";
                    string strPayCategoryNoSalaries = "(";
                    string strPayCategoryNoTimeAttendance = "(";

                    for (int intRow = 0; intRow < DataSetClient.Tables["Temp"].Rows.Count; intRow++)
                    {
                        if (DataSetClient.Tables["Temp"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                        {
                            strPayCategoryDesc += DataSetClient.Tables["Temp"].Rows[intRow]["PAY_CATEGORY_DESC"].ToString() + " (Wages)\n";

                            if (strPayCategoryNoWages == "(")
                            {
                                strPayCategoryNoWages += DataSetClient.Tables["Temp"].Rows[intRow]["PAY_CATEGORY_NO"].ToString();
                            }
                            else
                            {
                                strPayCategoryNoWages += "," + DataSetClient.Tables["Temp"].Rows[intRow]["PAY_CATEGORY_NO"].ToString();
                            }
                        }
                        else
                        {
                            if (DataSetClient.Tables["Temp"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                            {
                                strPayCategoryDesc += DataSetClient.Tables["Temp"].Rows[intRow]["PAY_CATEGORY_DESC"].ToString() + " (Salaries)\n";

                                if (strPayCategoryNoSalaries == "(")
                                {
                                    strPayCategoryNoSalaries += DataSetClient.Tables["Temp"].Rows[intRow]["PAY_CATEGORY_NO"].ToString();
                                }
                                else
                                {
                                    strPayCategoryNoSalaries += "," + DataSetClient.Tables["Temp"].Rows[intRow]["PAY_CATEGORY_NO"].ToString();
                                }
                            }
                            else
                            {
                                strPayCategoryDesc += DataSetClient.Tables["Temp"].Rows[intRow]["PAY_CATEGORY_DESC"].ToString() + " (Time Attendance)\n";

                                if (strPayCategoryNoTimeAttendance == "(")
                                {
                                    strPayCategoryNoTimeAttendance += DataSetClient.Tables["Temp"].Rows[intRow]["PAY_CATEGORY_NO"].ToString();
                                }
                                else
                                {
                                    strPayCategoryNoTimeAttendance += "," + DataSetClient.Tables["Temp"].Rows[intRow]["PAY_CATEGORY_NO"].ToString();
                                }

                            }
                        }
                    }

                    if (strPayCategoryNoWages != "(")
                    {
                        strPayCategoryNoWages += ")";
                    }
                    else
                    {
                        strPayCategoryNoWages += "-1)";
                    }

                    if (strPayCategoryNoSalaries != "(")
                    {
                        strPayCategoryNoSalaries += ")";
                    }
                    else
                    {
                        strPayCategoryNoSalaries += "-1)";
                    }

                    if (strPayCategoryNoTimeAttendance != "(")
                    {
                        strPayCategoryNoTimeAttendance += ")";
                    }
                    else
                    {
                        strPayCategoryNoTimeAttendance += "-1)";
                    }

                    strPayCategoryDesc += "\nExist on Local Database.\n\nClick Yes to Delete this Cost Centre and also Synchronize Selected Cost Centre.\n\nClick No to Synchronize Selected Cost Centre Only.";

                    if (DataSetClient.Tables["Temp"].Rows.Count > 1)
                    {
                        strPayCategoryDesc = strPayCategoryDesc.Replace("Centre", "Centres");
                        strPayCategoryDesc = strPayCategoryDesc.Replace("this", "these");
                    }
                    else
                    {
                        strPayCategoryDesc = strPayCategoryDesc.Replace("Exist", "Exists");
                    }

                    DialogResult myDialogResult = CustomMessageBox.Show(strPayCategoryDesc,
                    this.Text,
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                    if (myDialogResult == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        if (myDialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            objParm = new object[5];
                            objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                            objParm[1] = strPayCategoryNoWages;
                            objParm[2] = strPayCategoryNoSalaries;
                            objParm[3] = strPayCategoryNoTimeAttendance;
                            objParm[4] = bytClientDataSetCompress;

                            blnclsISClientUtilities = true;
                            this.clsISClientUtilities.DynamicFunction("Delete_Certain_PayCategory_Records_New", objParm, false);
                        }
                    }
                }
                else
                {
                    DialogResult myDialogResult = CustomMessageBox.Show("Would you like to Synchronize Selected Cost Centre/s?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                    if (myDialogResult == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                }

                //get Records from Internet Database
                objParm = new object[5];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = strCostCentresWages;
                objParm[2] = strCostCentresSalaries;
                objParm[3] = strCostCentresTimeAttendance;
                //2017-09-28 (Upload Employee Parameters)
                objParm[4] = bytClientDataSetCompress;

                blnclsISClientUtilities = false;
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Download_Records_Fix_Parameters", objParm);
                 
                pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                pvtDataSet.Merge(pvtTempDataSet);

                pvtbytCompress = clsISUtilities.Compress_DataSet(pvtDataSet);

                //Insert Records Into Client Database
                objParm = new object[8];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = strPayCategoryFilterWages;
                objParm[2] = strPayCategoryFilterSalaries;
                objParm[3] = strPayCategoryFilterTimeAttendance;
                objParm[4] = strCostCentresWages;
                objParm[5] = strCostCentresSalaries;
                objParm[6] = strCostCentresTimeAttendance;
                
                objParm[7] = pvtbytCompress;

                blnclsISClientUtilities = true;
                pvtbytCompress = (byte[])this.clsISClientUtilities.DynamicFunction("Download_Records_New", objParm,false);

                pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                if (pvtTempDataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows.Count > 0
                || pvtTempDataSet.Tables["EmployeeFingerPrintTemplateDelete"].Rows.Count > 0
                || pvtTempDataSet.Tables["UserFingerPrintTemplateUpload"].Rows.Count > 0
                || pvtTempDataSet.Tables["UserFingerPrintTemplateDelete"].Rows.Count > 0
                || pvtTempDataSet.Tables["PayCategoryInteractInd"].Rows.Count > 0)
                {
                    objParm = new object[2];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtbytCompress;
                    
                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Maintain_Templates", objParm);

                    objParm = new object[2];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")); 
                    objParm[1] = pvtbytCompress;

                    //Update CREATION_DATETIME which comes from Server
                    clsISClientUtilities.DynamicFunction("Maintain_Templates", objParm, false);
                }

                CustomMessageBox.Show("Synchronization of Databases Successful.",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                if (blnclsISClientUtilities == true)
                {
                    clsISClientUtilities.ErrorHandler(eException);
                }
                else
                {
                    clsISUtilities.ErrorHandler(eException);
                }
            }
        }

        private void dgvCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCostCentreDataGridView.Rows.Count > 0
                & pvtblnCostCentreDataGridViewLoaded == true)
            {

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
                    case "dgvCostCentreDataGridView":

                        dgvCostCentreDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvCostCentreChosenDataGridView":

                        this.dgvCostCentreChosenDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void dgvCostCentreChosenDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvCostCentreDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnAdd_Click(sender, e);
        }

        private void dgvCostCentreChosenDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnRemove_Click(sender, e);
        }

        private void btnAddDeleted_Click(object sender, EventArgs e)
        {
            if (this.dgvCostCentreDeletedDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvCostCentreDeletedDataGridView.Rows[this.dgvCostCentreDeletedDataGridView.CurrentRow.Index];

                this.dgvCostCentreDeletedDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvCostCentreChosenDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvCostCentreChosenDataGridView.CurrentCell = this.dgvCostCentreChosenDataGridView[0, this.dgvCostCentreChosenDataGridView.Rows.Count - 1];

                this.btnOK.Enabled = true;
            }
        }

        private void dgvCostCentreChosenDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 1)
            {
                if (dgvCostCentreChosenDataGridView[e.Column.Index + 3, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvCostCentreChosenDataGridView[e.Column.Index + 3, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvCostCentreChosenDataGridView[e.Column.Index + 3, e.RowIndex1].Value.ToString()) > double.Parse(dgvCostCentreChosenDataGridView[e.Column.Index + 3, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvCostCentreChosenDataGridView[e.Column.Index + 3, e.RowIndex1].Value.ToString()) < double.Parse(dgvCostCentreChosenDataGridView[e.Column.Index + 3, e.RowIndex2].Value.ToString()))
                            {
                                e.SortResult = -1;
                            }
                            else
                            {
                                e.SortResult = 0;
                            }
                        }
                    }
                }

                e.Handled = true;
            }
        }
    }
}
