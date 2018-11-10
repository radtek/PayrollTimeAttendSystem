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
    public partial class frmCompanyPaymentOptions : Form
    {
        clsISUtilities clsISUtilities;

        private Control myControl;

        string pvtstrPrintedDocumentType = "";

        private DataView pvtCompanyInvoiceHistoryDataView;
        private DataView pvtCompanyInvoiceItemHistoryDataView;
        private DataView pvtCompanyInvoiceItemCurrentDataView;

        private DataView pvtInvoiceEmailDataView;
        private DataView pvtStatementEmailDataView;
        
        private DataView pvtCompanyStatementHistoryDataView;
        private DataView pvtCompanyStatementItemHistoryDataView;
        private DataView pvtCompanyStatementItemCurrentDataView;

        private DataView pvtCompanyStatementInvoiceChoiceHistoryDataView;
        
        private DataView pvtCompanyInvoiceYYYYMMDDDataView;
        private DataView pvtCompanyStatementYYYYMMDataView;

        private DataSet pvtDataSet;
        private byte[] pvtbytCompress;
            
        private Int64 pvtint64CompanyNo = -1;
        private string pvtstrDocumentType = "";

        private int pvtintCompanyInvoiceDataViewRowIndex = -1;
        private int pvtintCompanyInvoiceItemDataViewRowIndex = -1;
        
        private int pvtintCompanyDataGridViewRowIndex = -1;
        private int pvtintDocumentTypeDataGridViewRowIndex = -1;
        
        private int pvtintInvoiceDataGridViewRowIndex = -1;
        private int pvtintInvoiceItemDataGridViewRowIndex = -1;

        private bool pvtblnCompanyDataGridViewLoaded = false;
        private bool pvtblnInvoiceDataGridViewLoaded = false;
        private bool pvtblnInvoiceItemDataGridViewLoaded = false;
        private bool pvtblnDocumentTypeDataGridViewLoaded = false;

        //dgvInvoiceItemDataGridView Columns
        int pvtintInvDate = 0;
        int pvtintInvOption = 1;
        int pvtintInvChoice = 2;
        int pvtintInvQuantity = 3;
        int pvtintInvDesc = 4;
        int pvtintInvUnitPrice = 5;
        int pvtintInvTotal = 6;
        int pvtintInvRow = 7;

        int pvtintInvOptionWidth = 0;
        int pvtintInvQuantityWidth = 0;

        int pvtintInvHeight = 0;
        int pvtintInvItemHeight = 0;
        int pvtintInvItemDescWidth = 0;

        double pvtdblLastStatementBalance = 0;

        DataGridViewCellStyle ProFormaInvoiceDataGridViewCellStyle;
        DataGridViewCellStyle MonthlyGeneratedInvoiceDataGridViewCellStyle;

        private bool pvtblnLoadedExportButton = false;

        private string pvtstrDateFormat;

        public frmCompanyPaymentOptions()
        {
            InitializeComponent();

            pvtintInvOptionWidth = dgvInvoiceItemDataGridView.Columns[pvtintInvOption].Width;
            pvtintInvQuantityWidth = dgvInvoiceItemDataGridView.Columns[pvtintInvQuantity].Width;
            pvtintInvHeight = dgvInvoiceDataGridView.Height;
            
            pvtintInvItemDescWidth = dgvInvoiceItemDataGridView.Columns[pvtintInvDesc].Width;

            pvtintInvItemHeight = dgvInvoiceItemDataGridView.Height;
        }

        private void frmCompanyPaymentOptions_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busCompanyPaymentOptions");

                ProFormaInvoiceDataGridViewCellStyle = new DataGridViewCellStyle();
                ProFormaInvoiceDataGridViewCellStyle.BackColor = Color.Yellow;
                ProFormaInvoiceDataGridViewCellStyle.SelectionBackColor = Color.Yellow;

                MonthlyGeneratedInvoiceDataGridViewCellStyle = new DataGridViewCellStyle();
                MonthlyGeneratedInvoiceDataGridViewCellStyle.BackColor = Color.Aqua;
                MonthlyGeneratedInvoiceDataGridViewCellStyle.SelectionBackColor = Color.Aqua;

                pvtstrDateFormat = AppDomain.CurrentDomain.GetData("DateFormat").ToString();
                pvtint64CompanyNo = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                this.txtWageAmount.KeyPress += clsISUtilities.Numeric_2_Decimal_KeyPress;
                this.txtSalaryAmount.KeyPress += clsISUtilities.Numeric_2_Decimal_KeyPress;
                this.txtTimeAttendAmount.KeyPress += clsISUtilities.Numeric_2_Decimal_KeyPress;
                this.txtHourlySupportAmount.KeyPress += clsISUtilities.Numeric_2_Decimal_KeyPress;

                this.lblInvoiceDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblInvoiceItemDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblDocumentType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblListInvoices.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedInvoices.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblListStatements.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedStatements.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblInvoiceMaintain.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                
                pvtDataSet = new DataSet();

                object[] objParm = new object[1];
                objParm[0] = pvtint64CompanyNo;

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                //Load Invoice Dates
                DateTime myDateTime = DateTime.Now.AddMonths(-1);

                while (true)
                { 
                    this.cboInvoiceDate.Items.Add(myDateTime.ToString("dd MMMM yyyy") + "                    " + myDateTime.ToString("yyyyMMdd"));

                    if (myDateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                    {
                        this.cboInvoiceDate.SelectedIndex = this.cboInvoiceDate.Items.Count - 1;
                    }

                    myDateTime = myDateTime.AddDays(1);

                    if (this.cboInvoiceDate.Items.Count > 62)
                    {
                        break;
                    }
                }
               
                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
            
            this.reportViewer.RefreshReport();
        }

        void Export_Click(object sender, EventArgs e)
        {
            if (this.reportViewer.LocalReport.DataSources.Count > 0)
            {
                try
                {
                    Microsoft.Reporting.WinForms.Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string filenameExtension;

                    byte[] byteArrayFile = this.reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

                    object[] objParm = new object[4];
                    objParm[0] = Convert.ToInt64(this.pvtDataSet.Tables["Company"].Rows[0]["COMPANY_NO"]);

                    if (pvtstrPrintedDocumentType == "S")
                    {
                        objParm[1] = Convert.ToInt32(pvtCompanyStatementHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["STATEMENT_NUMBER"]);
                    }
                    else
                    {
                        objParm[1] = Convert.ToInt32(pvtCompanyInvoiceHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["INVOICE_NUMBER"]);
                    }

                    objParm[2] = byteArrayFile;
                    objParm[3] = pvtstrPrintedDocumentType;

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("ExportFile", objParm);

                    DataSet TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                    //Remove Existing Email Records
                    if (pvtstrPrintedDocumentType == "I"
                    ||  pvtstrPrintedDocumentType == "P")
                    {
                        for (int intRow = 0; intRow < pvtDataSet.Tables["InvoiceEmail"].Rows.Count; intRow++)
                        {
                            pvtDataSet.Tables["InvoiceEmail"].Rows[intRow].Delete();
                        }
                    }
                    else
                    {
                        for (int intRow = 0; intRow < pvtDataSet.Tables["StatementEmail"].Rows.Count; intRow++)
                        {
                            pvtDataSet.Tables["StatementEmail"].Rows[intRow].Delete();
                        }
                    }

                    pvtDataSet.Merge(TempDataSet);

                    this.pvtDataSet.AcceptChanges();

                    if (Convert.ToInt32(pvtDataSet.Tables["Return"].Rows[0]["RETURN_CODE"]) == 0)
                    {
                        //Reload Emails
                        if (pvtstrPrintedDocumentType == "I"
                        ||  pvtstrPrintedDocumentType == "P")
                        {
                            this.Clear_DataGridView(dgvListInvoicesDataGridView);
                            this.Clear_DataGridView(dgvSelectedInvoicesDataGridView);
                            
                            pvtInvoiceEmailDataView = null;
                            pvtInvoiceEmailDataView = new DataView(pvtDataSet.Tables["InvoiceEmail"],
                                "",
                                "INVOICE_NUMBER DESC",
                                DataViewRowState.CurrentRows);

                            for (int intRow = 0; intRow < pvtInvoiceEmailDataView.Count; intRow++)
                            {
                                this.dgvListInvoicesDataGridView.Rows.Add(pvtInvoiceEmailDataView[intRow]["INVOICE_NUMBER"].ToString(),
                                                                          pvtInvoiceEmailDataView[intRow]["INVOICE_TYPE"].ToString(),
                                                                          Convert.ToDateTime(pvtInvoiceEmailDataView[intRow]["INVOICE_DATE"]).ToString(pvtstrDateFormat),
                                                                          Convert.ToDouble(pvtInvoiceEmailDataView[intRow]["INVOICE_TOTAL"]).ToString("#########0.00"),
                                                                          Convert.ToDouble(pvtInvoiceEmailDataView[intRow]["INVOICE_VAT_TOTAL"]).ToString("#########0.00"),
                                                                          Convert.ToDouble(pvtInvoiceEmailDataView[intRow]["INVOICE_FINAL_TOTAL"]).ToString("#########0.00"),
                                                                          pvtInvoiceEmailDataView[intRow]["CONTACT_PERSON"].ToString(),
                                                                          intRow.ToString());

                                if (pvtInvoiceEmailDataView[intRow]["INVOICE_TYPE"].ToString() == "P")
                                {
                                    this.dgvListInvoicesDataGridView.Rows[this.dgvListInvoicesDataGridView.Rows.Count - 1].HeaderCell.Style = ProFormaInvoiceDataGridViewCellStyle;
                                }
                            }
                        }
                        else
                        {
                            this.Clear_DataGridView(dgvListStatementsDataGridView);
                            this.Clear_DataGridView(dgvSelectedStatementsDataGridView);

                            pvtStatementEmailDataView = null;
                            pvtStatementEmailDataView = new DataView(pvtDataSet.Tables["StatementEmail"],
                                "",
                                "STATEMENT_NUMBER DESC",
                                DataViewRowState.CurrentRows);

                            for (int intRow = 0; intRow < pvtStatementEmailDataView.Count; intRow++)
                            {
                                DateTime dtDateTime = new DateTime(Convert.ToInt32(pvtStatementEmailDataView[intRow]["STATEMENT_NUMBER"].ToString().Substring(0, 4)), Convert.ToInt32(pvtStatementEmailDataView[intRow]["STATEMENT_NUMBER"].ToString().Substring(4)), 1);

                                string strStatementNumber = dtDateTime.ToString("yyyyMMM");

                                this.dgvListStatementsDataGridView.Rows.Add(strStatementNumber,
                                                                          Convert.ToDateTime(pvtStatementEmailDataView[intRow]["STATEMENT_DATE"]).ToString(pvtstrDateFormat),
                                                                          Convert.ToDouble(pvtStatementEmailDataView[intRow]["STATEMENT_OPEN_BALANCE"]).ToString("#########0.00"),
                                                                          Convert.ToDouble(pvtStatementEmailDataView[intRow]["STATEMENT_CLOSE_BALANCE"]).ToString("#########0.00"),
                                                                          pvtStatementEmailDataView[intRow]["CONTACT_PERSON"].ToString(),
                                                                          intRow.ToString());
                            }
                        }

                        if (pvtstrPrintedDocumentType == "S")
                        {
                            CustomMessageBox.Show("Statement Exported Successfully", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            CustomMessageBox.Show("Invoice Exported Successfully", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(pvtDataSet.Tables["Return"].Rows[0]["RETURN_CODE"]) == 1)
                        {
                            if (pvtstrPrintedDocumentType == "S")
                            {
                                CustomMessageBox.Show("Statement Export Failed. See Server Logs for Details", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                CustomMessageBox.Show("Invoice Export Failed. See Server Logs for Details", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            return;
                        }
                        else
                        {
                            if (Convert.ToInt32(pvtDataSet.Tables["Return"].Rows[0]["RETURN_CODE"]) == 9)
                            {
                                CustomMessageBox.Show("Company Folder Name has NOT been Setup for Invoices / Statements", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                }
                catch (Exception eException)
                {
                    clsISUtilities.ErrorHandler(eException);
                }
            }
        }

        private void Load_Invoice_Maintenance()
        {
            dgvInvoiceMainteneDataGridView.Rows.Clear();

            string strStatementDate = "";

            for (int intRow = 0; intRow < this.pvtDataSet.Tables["CompanyInvoiceMaintain"].Rows.Count; intRow++)
            {
                if (this.pvtDataSet.Tables["CompanyInvoiceMaintain"].Rows[intRow]["STATEMENT_DATE"] == System.DBNull.Value)
                {
                    strStatementDate = "";
                }
                else
                {
                    strStatementDate = Convert.ToDateTime(this.pvtDataSet.Tables["CompanyInvoiceMaintain"].Rows[intRow]["STATEMENT_DATE"]).ToString("yyyy MMMM");
                }

                dgvInvoiceMainteneDataGridView.Rows.Add(this.pvtDataSet.Tables["CompanyInvoiceMaintain"].Rows[intRow]["INVOICE_NUMBER"].ToString(),
                                                        Convert.ToDateTime(this.pvtDataSet.Tables["CompanyInvoiceMaintain"].Rows[intRow]["INVOICE_DATE"]).ToString("yyyy-MM-dd"),
                                                        this.pvtDataSet.Tables["CompanyInvoiceMaintain"].Rows[intRow]["STATEMENT_NUMBER"].ToString(),
                                                        strStatementDate,
                                                        Convert.ToDouble(this.pvtDataSet.Tables["CompanyInvoiceMaintain"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"]).ToString("########0.00"),
                                                        this.pvtDataSet.Tables["CompanyInvoiceMaintain"].Rows[intRow]["INVOICE_PAID_IND"].ToString(),
                                                        this.pvtDataSet.Tables["CompanyInvoiceMaintain"].Rows[intRow]["INVOICE_PAID_IND"].ToString());
            }
        }

        private void Load_CurrentForm_Records()
        {
            Load_Invoice_Maintenance();

            this.dgvDocumentTypeDataGridView.Rows.Clear();
            this.dgvDocumentTypeDataGridView.Rows.Add("Invoice");
            this.dgvDocumentTypeDataGridView.Rows.Add("Statement");
            this.dgvDocumentTypeDataGridView.Rows.Add("Audit Report");

            pvtblnDocumentTypeDataGridViewLoaded = true;

            this.Set_DataGridView_SelectedRowIndex(dgvDocumentTypeDataGridView, 0);

            pvtblnInvoiceDataGridViewLoaded = false;

            this.txtWageAmount.Text = Convert.ToDouble(this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_AMOUNT"]).ToString("#####0.00");
            this.txtSalaryAmount.Text = Convert.ToDouble(this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_AMOUNT"]).ToString("#####0.00");
            this.txtTimeAttendAmount.Text = Convert.ToDouble(this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTEND_AMOUNT"]).ToString("#####0.00");
            this.txtHourlySupportAmount.Text = Convert.ToDouble(this.pvtDataSet.Tables["Company"].Rows[0]["HOURLY_SUPPORT_AMOUNT"]).ToString("#####0.00");

            this.txtContactPerson1.Text = this.pvtDataSet.Tables["Company"].Rows[0]["PERSON_NAMES1"].ToString();
            this.txtPhone1.Text = this.pvtDataSet.Tables["Company"].Rows[0]["PHONE1"].ToString();
            this.txtEmail1.Text = this.pvtDataSet.Tables["Company"].Rows[0]["EMAIL1"].ToString();

            this.txtContactPerson2.Text = this.pvtDataSet.Tables["Company"].Rows[0]["PERSON_NAMES2"].ToString();
            this.txtPhone2.Text = this.pvtDataSet.Tables["Company"].Rows[0]["PHONE2"].ToString();
            this.txtEmail2.Text = this.pvtDataSet.Tables["Company"].Rows[0]["EMAIL2"].ToString();

            pvtInvoiceEmailDataView = null;
            pvtInvoiceEmailDataView = new DataView(pvtDataSet.Tables["InvoiceEmail"],
                "",
                "INVOICE_NUMBER DESC",
                DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtInvoiceEmailDataView.Count; intRow++)
            {
                this.dgvListInvoicesDataGridView.Rows.Add(pvtInvoiceEmailDataView[intRow]["INVOICE_NUMBER"].ToString(),
                                                          pvtInvoiceEmailDataView[intRow]["INVOICE_TYPE"].ToString(),
                                                          Convert.ToDateTime(pvtInvoiceEmailDataView[intRow]["INVOICE_DATE"]).ToString(pvtstrDateFormat),
                                                          Convert.ToDouble(pvtInvoiceEmailDataView[intRow]["INVOICE_TOTAL"]).ToString("#########0.00"),
                                                          Convert.ToDouble(pvtInvoiceEmailDataView[intRow]["INVOICE_VAT_TOTAL"]).ToString("#########0.00"),
                                                          Convert.ToDouble(pvtInvoiceEmailDataView[intRow]["INVOICE_FINAL_TOTAL"]).ToString("#########0.00"),
                                                          pvtInvoiceEmailDataView[intRow]["CONTACT_PERSON"].ToString(),
                                                          intRow.ToString());

                if (pvtInvoiceEmailDataView[intRow]["INVOICE_TYPE"].ToString() == "P")
                {
                    this.dgvListInvoicesDataGridView.Rows[this.dgvListInvoicesDataGridView.Rows.Count - 1].HeaderCell.Style = ProFormaInvoiceDataGridViewCellStyle;
                }
            }

            pvtStatementEmailDataView = null;
            pvtStatementEmailDataView = new DataView(pvtDataSet.Tables["StatementEmail"],
                "",
                "STATEMENT_NUMBER DESC",
                DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtStatementEmailDataView.Count; intRow++)
            {
                DateTime dtDateTime = new DateTime(Convert.ToInt32(pvtStatementEmailDataView[intRow]["STATEMENT_NUMBER"].ToString().Substring(0, 4)), Convert.ToInt32(pvtStatementEmailDataView[intRow]["STATEMENT_NUMBER"].ToString().Substring(4)), 1);

                string strStatementNumber = dtDateTime.ToString("yyyyMMM");

                this.dgvListStatementsDataGridView.Rows.Add(strStatementNumber,
                                                          Convert.ToDateTime(pvtStatementEmailDataView[intRow]["STATEMENT_DATE"]).ToString(pvtstrDateFormat),
                                                          Convert.ToDouble(pvtStatementEmailDataView[intRow]["STATEMENT_OPEN_BALANCE"]).ToString("#########0.00"),
                                                          Convert.ToDouble(pvtStatementEmailDataView[intRow]["STATEMENT_CLOSE_BALANCE"]).ToString("#########0.00"),
                                                          pvtStatementEmailDataView[intRow]["CONTACT_PERSON"].ToString(),
                                                          intRow.ToString());
            }
            
            pvtCompanyInvoiceHistoryDataView = null;
            pvtCompanyInvoiceHistoryDataView = new DataView(pvtDataSet.Tables["CompanyInvoiceHistory"],
                "COMPANY_NO = " + pvtint64CompanyNo,
                "INVOICE_NUMBER DESC",
                DataViewRowState.CurrentRows);

            pvtCompanyInvoiceItemCurrentDataView = null;
            pvtCompanyInvoiceItemCurrentDataView = new DataView(pvtDataSet.Tables["CompanyInvoiceItemCurrent"],
                "COMPANY_NO = " + pvtint64CompanyNo,
                "INVOICE_LINE_NO",
                DataViewRowState.CurrentRows);

            pvtCompanyStatementHistoryDataView = null;
            pvtCompanyStatementHistoryDataView = new DataView(pvtDataSet.Tables["CompanyStatementHistory"],
                "COMPANY_NO = " + pvtint64CompanyNo,
                "STATEMENT_NUMBER DESC",
                DataViewRowState.CurrentRows);

            if (pvtCompanyStatementHistoryDataView.Count > 0)
            {
                pvtdblLastStatementBalance = Convert.ToDouble(pvtCompanyStatementHistoryDataView[0]["STATEMENT_CLOSE_BALANCE"]);
            }
            
            pvtCompanyStatementItemCurrentDataView = null;
            pvtCompanyStatementItemCurrentDataView = new DataView(pvtDataSet.Tables["CompanyStatementItemCurrent"],
                "COMPANY_NO = " + pvtint64CompanyNo,
                "STATEMENT_LINE_NO",
                DataViewRowState.CurrentRows);

            pvtCompanyInvoiceYYYYMMDDDataView = null;
            pvtCompanyInvoiceYYYYMMDDDataView = new DataView(pvtDataSet.Tables["CompanyInvoiceYYYYMMDD"],
                "COMPANY_NO = " + pvtint64CompanyNo,
                "INVOICE_DATE",
                DataViewRowState.CurrentRows);

            pvtCompanyStatementYYYYMMDataView = null;
            pvtCompanyStatementYYYYMMDataView = new DataView(pvtDataSet.Tables["CompanyStatementYYYYMM"],
                "COMPANY_NO = " + pvtint64CompanyNo,
                "",
                DataViewRowState.CurrentRows);
          
            this.Clear_DataGridView(this.dgvInvoiceDataGridView);
            this.Clear_DataGridView(this.dgvInvoiceItemDataGridView);

            this.btnUpdate.Enabled = true;

            Load_Company_Invoices();
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
                    case "dgvDocumentTypeDataGridView":

                        pvtintDocumentTypeDataGridViewRowIndex = -1;
                        this.dgvDocumentTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvInvoiceItemDataGridView":

                        pvtintInvoiceItemDataGridViewRowIndex = -1;
                        this.dgvInvoiceItemDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvInvoiceDataGridView":

                        pvtintInvoiceDataGridViewRowIndex = -1;
                        this.dgvInvoiceDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvListInvoicesDataGridView":

                        this.dgvListInvoicesDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvSelectedInvoicesDataGridView":

                        this.dgvSelectedInvoicesDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;
                        
                    case "dgvListStatementsDataGridView":

                        this.dgvListStatementsDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;
   
                    case "dgvSelectedStatementsDataGridView":

                        this.dgvSelectedStatementsDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                if (myDataGridView.Name == "dgvInvoiceDataGridView")
                {
                    pvtintInvoiceDataGridViewRowIndex = -1;
                }

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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            this.dgvInvoiceDataGridView.Enabled = false;

            this.dgvDocumentTypeDataGridView.Enabled = false;

            this.txtWageAmount.Enabled = true;
            this.txtSalaryAmount.Enabled = true;
            this.txtTimeAttendAmount.Enabled = true;
            this.txtHourlySupportAmount.Enabled = true;

            this.txtContactPerson1.Enabled = true;
            this.txtPhone1.Enabled = true;
            this.txtEmail1.Enabled = true;
            this.txtContactPerson2.Enabled = true;
            this.txtPhone2.Enabled = true;
            this.txtEmail2.Enabled = true;

            this.btnInvoiceItemDeleteRec.Enabled = true;

            this.btnUpdate.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.cboInvoiceGen.Enabled = false;
            this.btnGenerate.Enabled = false;
            this.btnStandaloneGenerate.Enabled = false;
            this.btnInvoicePrint.Enabled = false;

            this.rbnInvoice.Enabled = false;
            this.rbnProformaInvoice.Enabled = false;

            if (pvtstrDocumentType == "I")
            {
                CheckToAddNewInvoiceItemRow(this.dgvInvoiceItemDataGridView.RowCount - 1);

                this.dgvInvoiceItemDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
                this.dgvInvoiceItemDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;
            }
            else
            {
                if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                {
                    //Current Only
                    CheckToAddNewStatementItemRow(this.dgvInvoiceItemDataGridView.RowCount - 1);

                    this.dgvInvoiceItemDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
                    this.dgvInvoiceItemDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;
                }
                else
                {
                    picInvoiceItemsLock.Visible = true;
                }
            }

            this.picInvoiceLock.Visible = true;
        }

        private void Amount_TextChanged(object sender, EventArgs e)
        {
            TextBox myTextBox = (TextBox)sender;

            if (this.btnSave.Enabled == true)
            {
                if (this.txtWageAmount.Text.Trim() == "")
                {
                    switch (myTextBox.Name)
                    {
                        case "txtWageAmount":

                            this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_AMOUNT"] = 0;
                            break;

                        case "txtSalaryAmount":

                            this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_AMOUNT"] = 0;
                            break;

                        case "txtTimeAttendAmount":

                            this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTEND_AMOUNT"] = 0;
                            break;

                        case "txtHourlySupportAmount":

                            this.pvtDataSet.Tables["Company"].Rows[0]["HOURLY_SUPPORT_AMOUNT"] = 0;
                            break;
                    }
                }
                else
                {
                    switch (myTextBox.Name)
                    {
                        case "txtWageAmount":

                            this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_AMOUNT"] = Convert.ToDouble(this.txtWageAmount.Text);
                            break;

                        case "txtSalaryAmount":

                            this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_AMOUNT"] = Convert.ToDouble(this.txtSalaryAmount.Text);
                            break;

                        case "txtTimeAttendAmount":

                            this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTEND_AMOUNT"] = Convert.ToDouble(this.txtTimeAttendAmount.Text);
                            break;

                        case "txtHourlySupportAmount":

                            this.pvtDataSet.Tables["Company"].Rows[0]["HOURLY_SUPPORT_AMOUNT"] = Convert.ToDouble(this.txtHourlySupportAmount.Text);
                            break;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.txtWageAmount.Text.Trim() == "")
                {
                    CustomMessageBox.Show("Capture Wage Amount", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    this.txtWageAmount.Focus();

                    return;
                }

                if (this.txtSalaryAmount.Text.Trim() == "")
                {
                    CustomMessageBox.Show("Capture Salary Amount", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    this.txtSalaryAmount.Focus();

                    return;
                }

                if (this.txtTimeAttendAmount.Text.Trim() == "")
                {
                    CustomMessageBox.Show("Capture Time Attendance Amount", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    this.txtTimeAttendAmount.Focus();

                    return;
                }

                if (this.txtHourlySupportAmount.Text.Trim() == "")
                {
                    CustomMessageBox.Show("Capture Hourly Support Amount", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    this.txtHourlySupportAmount.Focus();

                    return;
                }

                if (this.txtContactPerson1.Text.Trim() == ""
                    | this.txtPhone1.Text.Trim() == ""
                    | this.txtEmail1.Text.Trim() == "")
                {
                    if (this.txtContactPerson1.Text.Trim() == "")
                    {
                        CustomMessageBox.Show("Capture Contact Person", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        this.txtContactPerson1.Focus();

                        return;
                    }
                    else
                    {
                        if (this.txtPhone1.Text.Trim() == "")
                        {
                            CustomMessageBox.Show("Capture Phone", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                            this.txtPhone1.Focus();

                            return;
                        }
                        else
                        {
                            CustomMessageBox.Show("Capture Email", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                            this.txtEmail1.Focus();

                            return;
                        }
                    }
                }

                DataSet TempDataSet = new DataSet();

                //Check Spreadsheet
                for (int intCheckRow = 0; intCheckRow < this.dgvInvoiceItemDataGridView.Rows.Count; intCheckRow++)
                {
                    if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                    {
                        if (this.pvtstrDocumentType == "I")
                        {
                            if (Convert.ToDouble(this.dgvInvoiceItemDataGridView[pvtintInvQuantity, intCheckRow].Value) == 0
                            && this.dgvInvoiceItemDataGridView[pvtintInvDesc, intCheckRow].Value.ToString() == ""
                            && Convert.ToDouble(this.dgvInvoiceItemDataGridView[pvtintInvUnitPrice, intCheckRow].Value) == 0)
                            {
                                if (intCheckRow == this.dgvInvoiceItemDataGridView.Rows.Count - 1)
                                {
                                }
                                else
                                {
                                    //Error
                                    CustomMessageBox.Show("Error in Invoice Items.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }
                            }
                            else
                            {
                                if (Convert.ToDouble(this.dgvInvoiceItemDataGridView[pvtintInvQuantity, intCheckRow].Value) == 0
                                || this.dgvInvoiceItemDataGridView[pvtintInvDesc, intCheckRow].Value.ToString() == ""
                                || Convert.ToDouble(this.dgvInvoiceItemDataGridView[pvtintInvUnitPrice, intCheckRow].Value) == 0)
                                {
                                    //Error
                                    CustomMessageBox.Show("Error in Invoice Items.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (this.dgvInvoiceItemDataGridView[pvtintInvDate, intCheckRow].Value.ToString() == ""
                            && this.dgvInvoiceItemDataGridView[pvtintInvChoice, intCheckRow].Value.ToString() == ""
                            && this.dgvInvoiceItemDataGridView[pvtintInvDesc, intCheckRow].Value.ToString() == ""
                            && Convert.ToDouble(this.dgvInvoiceItemDataGridView[pvtintInvUnitPrice, intCheckRow].Value) == 0
                            && Convert.ToDouble(this.dgvInvoiceItemDataGridView[pvtintInvTotal, intCheckRow].Value) == 0)
                            {
                                if (intCheckRow == this.dgvInvoiceItemDataGridView.Rows.Count - 1)
                                {
                                }
                                else
                                {
                                    //Error
                                    CustomMessageBox.Show("Error in Statement Items.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }
                            }
                            else
                            {
                                if (this.dgvInvoiceItemDataGridView[pvtintInvDate, intCheckRow].Value.ToString() == ""
                                || this.dgvInvoiceItemDataGridView[pvtintInvChoice, intCheckRow].Value.ToString() == ""
                                || this.dgvInvoiceItemDataGridView[pvtintInvDesc, intCheckRow].Value.ToString() == "")
                                {
                                }
                                else
                                {
                                    if (Convert.ToDouble(this.dgvInvoiceItemDataGridView[pvtintInvUnitPrice, intCheckRow].Value) == 0
                                    && Convert.ToDouble(this.dgvInvoiceItemDataGridView[pvtintInvTotal, intCheckRow].Value) == 0)
                                    {
                                        //Error
                                        CustomMessageBox.Show("Error in Statement Items.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //History
                    }
                }

                int intNumber = 0;

                if (this.pvtstrDocumentType == "I")
                {
                    //Check Invoice Items
                    if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                    {
                        TempDataSet.Tables.Add(pvtDataSet.Tables["CompanyInvoiceItemCurrent"].Clone());

                        DataView CompanyInvoiceItemCurrentDataView = new DataView(pvtDataSet.Tables["CompanyInvoiceItemCurrent"],
                                                                            "COMPANY_NO = " + this.pvtint64CompanyNo,
                                                                            "",
                                                                             DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

                        for (int intRow = 0; intRow < CompanyInvoiceItemCurrentDataView.Count; intRow++)
                        {
                            if (Convert.ToDouble(CompanyInvoiceItemCurrentDataView[intRow]["INVOICE_LINE_QTY"]) == 0
                                && CompanyInvoiceItemCurrentDataView[intRow]["INVOICE_LINE_DESC"].ToString() == ""
                                && Convert.ToDouble(CompanyInvoiceItemCurrentDataView[intRow]["INVOICE_LINE_UNIT_PRICE"]) == 0)
                            {
                                continue;
                            }
                            else
                            {
                                TempDataSet.Tables["CompanyInvoiceItemCurrent"].ImportRow(CompanyInvoiceItemCurrentDataView[intRow].Row);
                            }
                        }
                    }
                    else
                    {
                        TempDataSet.Tables.Add(pvtDataSet.Tables["CompanyInvoiceItemHistory"].Clone());

                        intNumber = Convert.ToInt32(this.pvtCompanyInvoiceHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["INVOICE_NUMBER"]);

                        DataView CompanyInvoiceItemHistoryDataView = new DataView(pvtDataSet.Tables["CompanyInvoiceItemHistory"],
                                                                            "COMPANY_NO = " + this.pvtint64CompanyNo + " AND INVOICE_NUMBER = " + intNumber,
                                                                            "",
                                                                            DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

                        for (int intRow = 0; intRow < CompanyInvoiceItemHistoryDataView.Count; intRow++)
                        {
                            if (Convert.ToDouble(CompanyInvoiceItemHistoryDataView[intRow]["INVOICE_LINE_QTY"]) == 0
                                && CompanyInvoiceItemHistoryDataView[intRow]["INVOICE_LINE_DESC"].ToString() == ""
                                && Convert.ToDouble(CompanyInvoiceItemHistoryDataView[intRow]["INVOICE_LINE_UNIT_PRICE"]) == 0)
                            {
                                continue;
                            }
                            else
                            {
                                TempDataSet.Tables["CompanyInvoiceItemHistory"].ImportRow(CompanyInvoiceItemHistoryDataView[intRow].Row);
                            }
                        }
                    }
                }
                else
                {
                    //Check Statement Items
                    if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                    {
                        TempDataSet.Tables.Add(pvtDataSet.Tables["CompanyStatementItemCurrent"].Clone());

                        DataView CompanyStatementItemCurrentDataView = new DataView(pvtDataSet.Tables["CompanyStatementItemCurrent"],
                                                                            "COMPANY_NO = " + this.pvtint64CompanyNo,
                                                                            "",
                                                                             DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

                        for (int intRow = 0; intRow < CompanyStatementItemCurrentDataView.Count; intRow++)
                        {
                            if (CompanyStatementItemCurrentDataView[intRow]["STATEMENT_LINE_DESC"].ToString() == ""
                            && Convert.ToDouble(CompanyStatementItemCurrentDataView[intRow]["STATEMENT_LINE_DR_TOTAL"]) == 0
                            && Convert.ToDouble(CompanyStatementItemCurrentDataView[intRow]["STATEMENT_LINE_CR_TOTAL"]) == 0)
                            {
                                continue;
                            }
                            else
                            {
                                TempDataSet.Tables["CompanyStatementItemCurrent"].ImportRow(CompanyStatementItemCurrentDataView[intRow].Row);
                            }
                        }
                    }
                    else
                    {
                        TempDataSet.Tables.Add(pvtDataSet.Tables["CompanyStatementItemHistory"].Clone());

                        intNumber = Convert.ToInt32(this.pvtCompanyStatementHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["STATEMENT_NUMBER"]);

                        DataView CompanyStatementItemHistoryDataView = new DataView(pvtDataSet.Tables["CompanyStatementItemHistory"],
                                                                            "COMPANY_NO = " + this.pvtint64CompanyNo + " AND STATEMENT_NUMBER = " + intNumber,
                                                                            "",
                                                                            DataViewRowState.Added | DataViewRowState.ModifiedCurrent | DataViewRowState.Deleted);

                        for (int intRow = 0; intRow < CompanyStatementItemHistoryDataView.Count; intRow++)
                        {
                            if (CompanyStatementItemHistoryDataView[intRow]["STATEMENT_LINE_DESC"].ToString() == ""
                            && Convert.ToDouble(CompanyStatementItemHistoryDataView[intRow]["STATEMENT_LINE_DR_TOTAL"]) == 0
                            && Convert.ToDouble(CompanyStatementItemHistoryDataView[intRow]["STATEMENT_LINE_CR_TOTAL"]) == 0)
                            {
                                continue;
                            }
                            else
                            {
                                TempDataSet.Tables["CompanyStatementItemHistory"].ImportRow(CompanyStatementItemHistoryDataView[intRow].Row);
                            }
                        }
                    }
                }

                TempDataSet.Tables.Add(this.pvtDataSet.Tables["Company"].Clone());

                TempDataSet.Tables["Company"].ImportRow(this.pvtDataSet.Tables["Company"].Rows[0]);

                byte[] byteDataSetCompressed = clsISUtilities.Compress_DataSet(TempDataSet);

                object[] objParm = new object[5];
                objParm[0] = Convert.ToInt64(this.pvtDataSet.Tables["Company"].Rows[0]["COMPANY_NO"]);
                objParm[1] = byteDataSetCompressed;
                objParm[2] = pvtintCompanyInvoiceDataViewRowIndex;
                objParm[3] = intNumber;
                objParm[4] = pvtstrDocumentType;
                
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Record", objParm, true);

                pvtDataSet.RejectChanges();

                Remove_Company_Invoices_And_Statements();
                
                TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                pvtDataSet.Merge(TempDataSet);

                this.pvtDataSet.AcceptChanges();

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Remove_Company_Invoices_And_Statements()
        {
            pvtDataSet.Tables["CompanyInvoiceHistory"].Rows.Clear();
            pvtDataSet.Tables["CompanyInvoiceItemHistory"].Rows.Clear();
            pvtDataSet.Tables["CompanyInvoiceItemCurrent"].Rows.Clear();
            pvtDataSet.Tables["CompanyStatementHistory"].Rows.Clear();
            pvtDataSet.Tables["CompanyStatementItemHistory"].Rows.Clear();
            pvtDataSet.Tables["CompanyStatementItemCurrent"].Rows.Clear();
            pvtDataSet.Tables["CompanyStatementInvoiceChoiceCurrent"].Rows.Clear();
            pvtDataSet.Tables["CompanyStatementInvoiceChoiceHistory"].Rows.Clear();
        }

        private void ContactPerson_TextChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                TextBox myTextBox = (TextBox)sender;

                if (myTextBox.Name == "txtContactPerson1")
                {
                    this.pvtDataSet.Tables["Company"].Rows[0]["PERSON_NAMES1"] = myTextBox.Text.Trim();
                }
                else
                {
                    if (myTextBox.Name == "txtContactPerson2")
                    {
                        this.pvtDataSet.Tables["Company"].Rows[0]["PERSON_NAMES2"] = myTextBox.Text.Trim();
                    }
                }
            }
        }

        private void Email_TextChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                TextBox myTextBox = (TextBox)sender;

                if (myTextBox.Name == "txtEmail1")
                {
                    this.pvtDataSet.Tables["Company"].Rows[0]["EMAIL1"] = myTextBox.Text.Trim();
                }
                else
                {
                    if (myTextBox.Name == "txtEmail2")
                    {
                        this.pvtDataSet.Tables["Company"].Rows[0]["EMAIL2"] = myTextBox.Text.Trim();
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);

            this.pvtDataSet.RejectChanges();

            this.dgvInvoiceDataGridView.Enabled = true;
            this.dgvDocumentTypeDataGridView.Enabled = true;

            this.txtWageAmount.Enabled = false;
            this.txtSalaryAmount.Enabled = false;
            this.txtTimeAttendAmount.Enabled = false;
            this.txtHourlySupportAmount.Enabled = false;

            this.txtContactPerson1.Enabled = false;
            this.txtPhone1.Enabled = false;
            this.txtEmail1.Enabled = false;
            this.txtContactPerson2.Enabled = false;
            this.txtPhone2.Enabled = false;
            this.txtEmail2.Enabled = false;

            this.btnInvoiceItemDeleteRec.Enabled = false;

            this.rbnInvoice.Enabled = true;
            this.rbnProformaInvoice.Enabled = true;

            this.btnUpdate.Enabled = true;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.cboInvoiceGen.Enabled = true;
            this.btnGenerate.Enabled = true;
            this.btnStandaloneGenerate.Enabled = true;
            this.btnInvoicePrint.Enabled = true;

            this.dgvInvoiceItemDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvInvoiceItemDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            if (pvtstrDocumentType == "I")
            {
                Load_Company_Invoices();
            }
            else
            {
                if (pvtstrDocumentType == "S")
                {
                    Load_Company_Statements(true);
                }
                else
                {
                    Load_Company_Statements(false);
                }
            }
            
            this.picInvoiceLock.Visible = false;
            this.picInvoiceItemsLock.Visible = false;
        }

        private void Load_Company_Invoices()
        {
            DateTime myDateTime = DateTime.Now;

            this.cboInvoiceGen.Items.Clear();

            for (int intRow = 0; intRow < this.pvtCompanyInvoiceYYYYMMDDDataView.Count; intRow++)
            {
                this.cboInvoiceGen.Items.Add(Convert.ToDateTime(pvtCompanyInvoiceYYYYMMDDDataView[intRow]["INVOICE_DATE"]).ToString("dd MMMM yyyy"));
            }

            if (this.pvtCompanyInvoiceYYYYMMDDDataView.Count > 0)
            {
                this.cboInvoiceGen.SelectedIndex = 0;
            }

            this.Clear_DataGridView(this.dgvInvoiceDataGridView);
            this.Clear_DataGridView(this.dgvInvoiceItemDataGridView);

            this.dgvInvoiceDataGridView.Rows.Add("Current",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  "",
                                                  -1);

            for (int intRow = 0; intRow < this.pvtCompanyInvoiceHistoryDataView.Count; intRow++)
            {
                this.dgvInvoiceDataGridView.Rows.Add(Convert.ToDateTime(pvtCompanyInvoiceHistoryDataView[intRow]["INVOICE_DATE"]).ToString(pvtstrDateFormat),
                                                     this.pvtCompanyInvoiceHistoryDataView[intRow]["INVOICE_NUMBER"].ToString(),
                                                     pvtCompanyInvoiceHistoryDataView[intRow]["INVOICE_TYPE"].ToString(),
                                                     this.pvtCompanyInvoiceHistoryDataView[intRow]["CONTACT_PERSON"].ToString(),
                                                     this.pvtCompanyInvoiceHistoryDataView[intRow]["CONTACT_PHONE"].ToString(),
                                                     this.pvtCompanyInvoiceHistoryDataView[intRow]["CONTACT_EMAIL"].ToString(),
                                                     Convert.ToDouble(this.pvtCompanyInvoiceHistoryDataView[intRow]["INVOICE_VAT_TOTAL"]).ToString("#########0.00"),
                                                     Convert.ToDouble(this.pvtCompanyInvoiceHistoryDataView[intRow]["INVOICE_FINAL_TOTAL"]).ToString("#########0.00"),
                                                     intRow);

                if (pvtCompanyInvoiceHistoryDataView[intRow]["INVOICE_TYPE_IND"].ToString() == "P")
                {
                    this.dgvInvoiceDataGridView.Rows[this.dgvInvoiceDataGridView.Rows.Count - 1].HeaderCell.Style = ProFormaInvoiceDataGridViewCellStyle;
                }

                if (pvtCompanyInvoiceHistoryDataView[intRow]["INVOICE_MONTHLY_INVOICE_IND"].ToString() == "Y")
                {
                    this.dgvInvoiceDataGridView.Rows[this.dgvInvoiceDataGridView.Rows.Count - 1].HeaderCell.Style = MonthlyGeneratedInvoiceDataGridViewCellStyle;
                }
            }

            pvtblnInvoiceDataGridViewLoaded = true;

            if (this.dgvInvoiceDataGridView.RowCount > 0)
            {
                this.btnInvoicePrint.Enabled = true;

                if (pvtintInvoiceDataGridViewRowIndex != -1
                    && pvtintInvoiceDataGridViewRowIndex <= this.dgvInvoiceDataGridView.Rows.Count - 1)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvInvoiceDataGridView, pvtintInvoiceDataGridViewRowIndex);
                }
                else
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvInvoiceDataGridView, 0);
                }
            }
            else
            {
                this.btnInvoicePrint.Enabled = false;
            }
        }

        private void Load_Company_Statements(bool showCurrent)
        {
            DateTime myDateTime = DateTime.Now;

            this.cboInvoiceGen.Items.Clear();

            for (int intRow = 0; intRow < this.pvtCompanyStatementYYYYMMDataView.Count; intRow++)
            {
                myDateTime = new DateTime(Convert.ToInt32(pvtCompanyStatementYYYYMMDataView[intRow]["STATEMENT_YYYYMM"].ToString().Substring(0, 4)), Convert.ToInt32(pvtCompanyStatementYYYYMMDataView[intRow]["STATEMENT_YYYYMM"].ToString().Substring(4)), 1).AddMonths(1).AddDays(-1);

                this.cboInvoiceGen.Items.Add(myDateTime.ToString("dd MMMM yyyy"));
            }

            if (this.pvtCompanyStatementYYYYMMDataView.Count > 0)
            {
                this.cboInvoiceGen.SelectedIndex = 0;
            }

            this.Clear_DataGridView(this.dgvInvoiceDataGridView);
            this.Clear_DataGridView(this.dgvInvoiceItemDataGridView);

            if (showCurrent == true)
            {
                this.dgvInvoiceDataGridView.Rows.Add("Current",
                                                      "",
                                                      "",
                                                      "",
                                                      "",
                                                      "",
                                                      "",
                                                       "",
                                                      -1);
            }

            for (int intRow = 0; intRow < this.pvtCompanyStatementHistoryDataView.Count; intRow++)
            {
                myDateTime = new DateTime(Convert.ToInt32(this.pvtCompanyStatementHistoryDataView[intRow]["STATEMENT_NUMBER"].ToString().Substring(0, 4)), Convert.ToInt32(this.pvtCompanyStatementHistoryDataView[intRow]["STATEMENT_NUMBER"].ToString().Substring(4)), 1);

                this.dgvInvoiceDataGridView.Rows.Add(myDateTime.ToString("yyyy MMMM"),
                                                     this.pvtCompanyStatementHistoryDataView[intRow]["STATEMENT_NUMBER"].ToString(),
                                                     Convert.ToDateTime(pvtCompanyStatementHistoryDataView[intRow]["STATEMENT_DATE"]).ToString(pvtstrDateFormat),
                                                     this.pvtCompanyStatementHistoryDataView[intRow]["CONTACT_PERSON"].ToString(),
                                                     this.pvtCompanyStatementHistoryDataView[intRow]["CONTACT_PHONE"].ToString(),
                                                     this.pvtCompanyStatementHistoryDataView[intRow]["CONTACT_EMAIL"].ToString(),
                                                     Convert.ToDouble(this.pvtCompanyStatementHistoryDataView[intRow]["STATEMENT_OPEN_BALANCE"]).ToString("#########0.00"),
                                                     Convert.ToDouble(this.pvtCompanyStatementHistoryDataView[intRow]["STATEMENT_CLOSE_BALANCE"]).ToString("#########0.00"),
                                                     intRow);
            }

            pvtblnInvoiceDataGridViewLoaded = true;

            if (this.dgvInvoiceDataGridView.RowCount > 0)
            {
                this.btnInvoicePrint.Enabled = true;

                if (pvtintInvoiceDataGridViewRowIndex != -1
                    && pvtintInvoiceDataGridViewRowIndex <= this.dgvInvoiceDataGridView.Rows.Count - 1)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvInvoiceDataGridView, pvtintInvoiceDataGridViewRowIndex);
                }
                else
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvInvoiceDataGridView, 0);
                }
            }
            else
            {
                this.btnInvoicePrint.Enabled = false;
            }
        }

        private void Load_Company_Invoice_Items()
        {
            string strOption = "";
            string strChoice = "";
            double dblTotal = 0;
            double dblFinalTotal = 0;

            pvtblnInvoiceItemDataGridViewLoaded = false;

            this.Clear_DataGridView(this.dgvInvoiceItemDataGridView);

            if (pvtintCompanyInvoiceDataViewRowIndex == -1)
            {
                for (int intRow = 0; intRow < this.pvtCompanyInvoiceItemCurrentDataView.Count; intRow++)
                {
                    if (Convert.ToInt32(this.pvtCompanyInvoiceItemCurrentDataView[intRow]["INVOICE_LINE_OPTION_NO"]) == 0)
                    {
                        strOption = "Next";
                    }
                    else
                    {
                        strOption = "Pending";
                    }

                    strChoice = this.pvtDataSet.Tables["ItemChoice"].Rows[Convert.ToInt32(this.pvtCompanyInvoiceItemCurrentDataView[intRow]["INVOICE_LINE_CHOICE_NO"])]["INVOICE_LINE_CHOICE_DESC"].ToString();

                    dblTotal = Convert.ToDouble(this.pvtCompanyInvoiceItemCurrentDataView[intRow]["INVOICE_LINE_QTY"]) * Convert.ToDouble(this.pvtCompanyInvoiceItemCurrentDataView[intRow]["INVOICE_LINE_UNIT_PRICE"]);
                    dblFinalTotal += dblTotal;

                    this.dgvInvoiceItemDataGridView.Rows.Add(Convert.ToDateTime(this.pvtCompanyInvoiceItemCurrentDataView[intRow]["INVOICE_LINE_DATE"]).ToString(pvtstrDateFormat),
                                                             strOption,
                                                             strChoice,
                                                             Convert.ToDouble(this.pvtCompanyInvoiceItemCurrentDataView[intRow]["INVOICE_LINE_QTY"]).ToString("####0.00"),
                                                             this.pvtCompanyInvoiceItemCurrentDataView[intRow]["INVOICE_LINE_DESC"].ToString(),
                                                             Convert.ToDouble(this.pvtCompanyInvoiceItemCurrentDataView[intRow]["INVOICE_LINE_UNIT_PRICE"]).ToString("#########0.00"),
                                                             dblTotal.ToString("#########0.00"),
                                                             intRow);
                }
            }
            else
            {
                pvtCompanyInvoiceItemHistoryDataView = null;
                pvtCompanyInvoiceItemHistoryDataView = new DataView(pvtDataSet.Tables["CompanyInvoiceItemHistory"],
                    "COMPANY_NO = " + pvtint64CompanyNo + " AND INVOICE_NUMBER = " + pvtCompanyInvoiceHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["INVOICE_NUMBER"].ToString(),
                    "INVOICE_LINE_NO",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < this.pvtCompanyInvoiceItemHistoryDataView.Count; intRow++)
                {
                    strOption = "";

                    strChoice = this.pvtDataSet.Tables["ItemChoice"].Rows[Convert.ToInt32(this.pvtCompanyInvoiceItemHistoryDataView[intRow]["INVOICE_LINE_CHOICE_NO"])]["INVOICE_LINE_CHOICE_DESC"].ToString();

                    dblTotal = Convert.ToDouble(this.pvtCompanyInvoiceItemHistoryDataView[intRow]["INVOICE_LINE_TOTAL"]);
                    dblFinalTotal += dblTotal;

                    this.dgvInvoiceItemDataGridView.Rows.Add(Convert.ToDateTime(this.pvtCompanyInvoiceItemHistoryDataView[intRow]["INVOICE_LINE_DATE"]).ToString(pvtstrDateFormat),
                                                             strOption,
                                                             strChoice,
                                                             Convert.ToDouble(this.pvtCompanyInvoiceItemHistoryDataView[intRow]["INVOICE_LINE_QTY"]).ToString("####0.00"),
                                                             this.pvtCompanyInvoiceItemHistoryDataView[intRow]["INVOICE_LINE_DESC"].ToString(),
                                                             Convert.ToDouble(this.pvtCompanyInvoiceItemHistoryDataView[intRow]["INVOICE_LINE_UNIT_PRICE"]).ToString("#########0.00"),
                                                             dblTotal.ToString("#########0.00"),
                                                             intRow);
                }
            }

            pvtblnInvoiceItemDataGridViewLoaded = true;

            this.txtInvoicesTotal.Text = dblFinalTotal.ToString("########0.00");
            this.txtStatementBalance.Text = pvtdblLastStatementBalance.ToString("########0.00");
            
            if (this.dgvInvoiceItemDataGridView.RowCount > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvInvoiceItemDataGridView, 0);
            }
        }

        private void Load_Company_Statement_Items()
        {
            pvtblnInvoiceItemDataGridViewLoaded = false;

            this.Clear_DataGridView(this.dgvInvoiceItemDataGridView);

            if (pvtintCompanyInvoiceDataViewRowIndex == -1)
            {
                for (int intRow = 0; intRow < this.pvtCompanyStatementItemCurrentDataView.Count; intRow++)
                {
                    this.dgvInvoiceItemDataGridView.Rows.Add(Convert.ToDateTime(this.pvtCompanyStatementItemCurrentDataView[intRow]["STATEMENT_LINE_DATE"]).ToString(pvtstrDateFormat),
                                                             "",
                                                             this.pvtCompanyStatementItemCurrentDataView[intRow]["INVOICE_NUMBER"].ToString(),
                                                             "",
                                                             this.pvtCompanyStatementItemCurrentDataView[intRow]["STATEMENT_LINE_DESC"].ToString(),
                                                             Convert.ToDouble(this.pvtCompanyStatementItemCurrentDataView[intRow]["STATEMENT_LINE_DR_TOTAL"]).ToString("#########0.00"),
                                                             Convert.ToDouble(this.pvtCompanyStatementItemCurrentDataView[intRow]["STATEMENT_LINE_CR_TOTAL"]).ToString("#########0.00"),
                                                             intRow);

                    if (this.pvtCompanyStatementItemCurrentDataView[intRow]["LOCK_IND"].ToString() == "Y")
                    {
                        this.dgvInvoiceItemDataGridView.Rows[this.dgvInvoiceItemDataGridView.Rows.Count - 1].ReadOnly = true;
                    }
                }
            }
            else
            {
                pvtCompanyStatementItemHistoryDataView = null;
                pvtCompanyStatementItemHistoryDataView = new DataView(pvtDataSet.Tables["CompanyStatementItemHistory"],
                    "COMPANY_NO = " + pvtint64CompanyNo + " AND STATEMENT_NUMBER = " + pvtCompanyStatementHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["STATEMENT_NUMBER"].ToString(),
                    "STATEMENT_LINE_NO",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < this.pvtCompanyStatementItemHistoryDataView.Count; intRow++)
                {
                    this.dgvInvoiceItemDataGridView.Rows.Add(Convert.ToDateTime(this.pvtCompanyStatementItemHistoryDataView[intRow]["STATEMENT_LINE_DATE"]).ToString(pvtstrDateFormat),
                                                             "",
                                                              this.pvtCompanyStatementItemHistoryDataView[intRow]["INVOICE_NUMBER"].ToString(),
                                                             "",
                                                             this.pvtCompanyStatementItemHistoryDataView[intRow]["STATEMENT_LINE_DESC"].ToString(),
                                                             Convert.ToDouble(this.pvtCompanyStatementItemHistoryDataView[intRow]["STATEMENT_LINE_DR_TOTAL"]).ToString("#########0.00"),
                                                             Convert.ToDouble(this.pvtCompanyStatementItemHistoryDataView[intRow]["STATEMENT_LINE_CR_TOTAL"]).ToString("#########0.00"),
                                                             intRow);
                }
            }

            pvtblnInvoiceItemDataGridViewLoaded = true;

            if (this.dgvInvoiceItemDataGridView.RowCount > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(this.dgvInvoiceItemDataGridView, 0);
            }
        }
        
        private void dgvInvoiceItemDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.Control is TextBox)
                {
                    int intColIndex = this.dgvInvoiceItemDataGridView.CurrentCell.ColumnIndex;

                    if (intColIndex == pvtintInvQuantity)
                    {
                        myControl = e.Control;

                        e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                        e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                    }
                    else
                    {
                        if (intColIndex == pvtintInvUnitPrice)
                        {
                            myControl = e.Control;

                            e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                            e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                        }
                        else
                        {
                            if (intColIndex == pvtintInvTotal)
                            {
                                myControl = e.Control;

                                e.Control.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                                e.Control.KeyPress += new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);
                            }
                        }
                    }
                }
                else
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
                }
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                ComboBox myComboBox = (ComboBox)sender;

                if (this.pvtstrDocumentType == "I")
                {
                    DataView ItemChoiceDataView = new DataView(pvtDataSet.Tables["ItemChoice"],
                            "INVOICE_LINE_CHOICE_DESC = '" + myComboBox.SelectedItem.ToString() + "'",
                            "",
                            DataViewRowState.CurrentRows);

                    if (ItemChoiceDataView.Count > 0)
                    {
                        int intChoiceNo = myComboBox.SelectedIndex;

                        if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                        {
                            pvtCompanyInvoiceItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_CHOICE_NO"] = myComboBox.SelectedIndex;
                        }
                        else
                        {
                            pvtCompanyInvoiceItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_CHOICE_NO"] = myComboBox.SelectedIndex;
                        }

                        string strLine = ItemChoiceDataView[0]["INVOICE_LINE_CHOICE_DETAIL"].ToString();
                        double dblUnitPrice = 0;

                        switch (myComboBox.SelectedItem.ToString())
                        {
                            case "TimeAttendance":

                                dblUnitPrice = Convert.ToDouble(this.pvtDataSet.Tables["Company"].Rows[0]["TIME_ATTEND_AMOUNT"]);
                                break;

                            case "Salary":

                                dblUnitPrice = Convert.ToDouble(this.pvtDataSet.Tables["Company"].Rows[0]["SALARY_AMOUNT"]);
                                break;

                            case "Wage":

                                dblUnitPrice = Convert.ToDouble(this.pvtDataSet.Tables["Company"].Rows[0]["WAGE_AMOUNT"]);
                                break;

                            case "Support":

                                dblUnitPrice = Convert.ToDouble(this.pvtDataSet.Tables["Company"].Rows[0]["HOURLY_SUPPORT_AMOUNT"]);
                                break;

                            case "ShoutItNow":

                                dblUnitPrice = Convert.ToDouble(ItemChoiceDataView[0]["INVOICE_LINE_VALUE"]);
                                break;
                        }

                        strLine = strLine.Replace("#Amount#", dblUnitPrice.ToString("########0.00"));

                        if (DateTime.Now.Day > 15)
                        {
                            strLine = strLine.Replace("#DateMonthPrev#", DateTime.Now.ToString("MMMM yyyy"));
                        }
                        else
                        {
                            strLine = strLine.Replace("#DateMonthPrev#", DateTime.Now.AddMonths(-1).ToString("MMMM yyyy"));
                        }

                        this.dgvInvoiceItemDataGridView[pvtintInvDesc, this.dgvInvoiceItemDataGridView.CurrentCell.RowIndex].Value = strLine;

                        if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                        {
                            pvtCompanyInvoiceItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_DESC"] = strLine;
                        }
                        else
                        {
                            pvtCompanyInvoiceItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_DESC"] = strLine;
                        }

                        this.dgvInvoiceItemDataGridView[pvtintInvUnitPrice, this.dgvInvoiceItemDataGridView.CurrentCell.RowIndex].Value = dblUnitPrice.ToString("########0.00");

                        if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                        {
                            pvtCompanyInvoiceItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_UNIT_PRICE"] = dblUnitPrice;
                        }
                        else
                        {
                            pvtCompanyInvoiceItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_UNIT_PRICE"] = dblUnitPrice;
                        }

                        if (myComboBox.SelectedItem.ToString() == "ShoutItNow")
                        {
                            this.dgvInvoiceItemDataGridView[pvtintInvQuantity, this.dgvInvoiceItemDataGridView.CurrentCell.RowIndex].Value = "1.00";

                            pvtCompanyInvoiceItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_QTY"] = 1;

                            this.dgvInvoiceItemDataGridView[pvtintInvTotal, this.dgvInvoiceItemDataGridView.CurrentCell.RowIndex].Value = dblUnitPrice.ToString("#########0.00");
                        }
                        else
                        {
                            this.dgvInvoiceItemDataGridView[pvtintInvQuantity, this.dgvInvoiceItemDataGridView.CurrentCell.RowIndex].Value = "0.00";
                            this.dgvInvoiceItemDataGridView[pvtintInvTotal, this.dgvInvoiceItemDataGridView.CurrentCell.RowIndex].Value = "0.00";
                        }

                        CalculateInvoiceItemRowTotal(this.dgvInvoiceItemDataGridView.CurrentCell.RowIndex);

                        CheckToAddNewInvoiceItemRow(this.dgvInvoiceItemDataGridView.CurrentCell.RowIndex);
                    }
                    else
                    {
                        if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                        {
                            if (myComboBox.SelectedItem.ToString() == "Next")
                            {
                                pvtCompanyInvoiceItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_OPTION_NO"] = 0;
                            }
                            else
                            {
                                pvtCompanyInvoiceItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_OPTION_NO"] = 1;
                            }
                        }
                    }
                }
                else
                {
                    DataView InvoiceNumberChoiceDataView = new DataView(pvtDataSet.Tables["CompanyStatementInvoiceChoiceCurrent"],
                           "INVOICE_NUMBER = '" + myComboBox.SelectedItem.ToString() + "'",
                           "",
                           DataViewRowState.CurrentRows);

                    if (myComboBox.SelectedItem.ToString() != "-1")
                    {
                        this.dgvInvoiceItemDataGridView[pvtintInvDesc, this.dgvInvoiceItemDataGridView.CurrentCell.RowIndex].Value = "Payment of Invoice " + myComboBox.SelectedItem.ToString();
                    }
                    else
                    {
                        this.dgvInvoiceItemDataGridView[pvtintInvDesc, this.dgvInvoiceItemDataGridView.CurrentCell.RowIndex].Value = "";
                    }

                    this.dgvInvoiceItemDataGridView[pvtintInvTotal, this.dgvInvoiceItemDataGridView.CurrentCell.RowIndex].Value = Convert.ToDouble(InvoiceNumberChoiceDataView[0]["INVOICE_FINAL_TOTAL"]).ToString("#########0.00");

                    if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                    {
                        if (myComboBox.SelectedItem.ToString() == "-1")
                        {
                            pvtCompanyStatementItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["STATEMENT_LINE_DESC"] = "";
                        }
                        else
                        {
                            pvtCompanyStatementItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["STATEMENT_LINE_DESC"] = "Payment of Invoice " + myComboBox.SelectedItem.ToString();
                           
                        }

                        pvtCompanyStatementItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_NUMBER"] = myComboBox.SelectedItem.ToString();

                        pvtCompanyStatementItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["STATEMENT_LINE_CR_TOTAL"] = Convert.ToDouble(InvoiceNumberChoiceDataView[0]["INVOICE_FINAL_TOTAL"]);
                    }
                    else
                    {
                        if (myComboBox.SelectedItem.ToString() == "")
                        {
                            pvtCompanyStatementItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_NUMBER"] = -1;
                        }
                        else
                        {
                            pvtCompanyStatementItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["STATEMENT_LINE_DESC"] = "Payment of Invoice " + myComboBox.SelectedItem.ToString();
                            pvtCompanyStatementItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_NUMBER"] = myComboBox.SelectedItem.ToString();
                        }

                        pvtCompanyStatementItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["STATEMENT_LINE_CR_TOTAL"] = Convert.ToDouble(InvoiceNumberChoiceDataView[0]["INVOICE_FINAL_TOTAL"]);
                    }
                }
            }
        }

        private void dgvInvoiceItemDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == pvtintInvQuantity)
            {
                //Only Invoices
                myControl.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);

                if (dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString() == "")
                {
                    dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value = "0.00";
                }

                if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                {
                    pvtCompanyInvoiceItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_QTY"] = dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value;
                }
                else
                {
                    //History
                    pvtCompanyInvoiceItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_QTY"] = dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value;
                }

                this.dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value = Convert.ToDouble(this.dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value).ToString("####0.00");

                CalculateInvoiceItemRowTotal(e.RowIndex);
            }
            else
            {
                if (e.ColumnIndex == pvtintInvDesc)
                {
                    if (pvtstrDocumentType == "I")
                    {
                        if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                        {
                            pvtCompanyInvoiceItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_DESC"] = dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value;
                        }
                        else
                        {
                            pvtCompanyInvoiceItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_DESC"] = dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value;
                        }
                    }
                    else
                    {
                        //Statements
                        if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                        {
                            pvtCompanyStatementItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["STATEMENT_LINE_DESC"] = dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value;
                        }
                        else
                        {
                            pvtCompanyStatementItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["STATEMENT_LINE_DESC"] = dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value;
                        }
                    }
                }
                else
                {
                    if (e.ColumnIndex == pvtintInvUnitPrice)
                    {
                        //Only Invoice (DR Column Locked for Statements)
                        myControl.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);

                        if (this.dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value == null)
                        {
                            dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value = "0.00";
                        }
                           
                        if (dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString() == "")
                        {
                            dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value = "0.00";
                        }

                        if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                        {
                            pvtCompanyInvoiceItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_UNIT_PRICE"] = dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value;
                        }
                        else
                        {
                            pvtCompanyInvoiceItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_UNIT_PRICE"] = dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value;
                        }

                        this.dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value = Convert.ToDouble(this.dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value).ToString("#########0.00");

                        CalculateInvoiceItemRowTotal(e.RowIndex);
                    }
                    else
                    {
                        if (e.ColumnIndex == pvtintInvTotal)
                        {
                            //Only STtatement (Invoice Total Column Locked for Invoices)
                            myControl.KeyPress -= new KeyPressEventHandler(clsISUtilities.Numeric_2_Decimal_KeyPress);

                            if (this.dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value == null)
                            {
                                dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value = "0.00";
                            }

                            if (dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString() == "")
                            {
                                dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value = "0.00";
                            }

                            if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                            {
                                pvtCompanyStatementItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["STATEMENT_LINE_CR_TOTAL"] = Convert.ToDouble(dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value);
                            }
                            else
                            {
                                pvtCompanyStatementItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["STATEMENT_LINE_CR_TOTAL"] = Convert.ToDouble(dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value);
                            }

                            this.dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value = Convert.ToDouble(this.dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value).ToString("#########0.00");
                        }
                        else
                        {

                        }
                    }
                }
            }

            if (pvtstrDocumentType == "I")
            {
                CheckToAddNewInvoiceItemRow(e.RowIndex);
            }
            else
            {
                CheckToAddNewStatementItemRow(e.RowIndex);
            }
        }

        private void CalculateInvoiceItemRowTotal(int RowIndex)
        {
            if (this.dgvInvoiceItemDataGridView[pvtintInvQuantity, RowIndex].Value != null
             && this.dgvInvoiceItemDataGridView[pvtintInvUnitPrice, RowIndex].Value != null)
            {
                double dblTotal = Convert.ToDouble(this.dgvInvoiceItemDataGridView[pvtintInvQuantity, RowIndex].Value) * Convert.ToDouble(this.dgvInvoiceItemDataGridView[pvtintInvUnitPrice, RowIndex].Value);

                this.dgvInvoiceItemDataGridView[pvtintInvTotal, RowIndex].Value = dblTotal.ToString("#########0.00");

                if (pvtintCompanyInvoiceDataViewRowIndex != -1)
                {
                    //History
                    pvtCompanyInvoiceItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_TOTAL"] = dgvInvoiceItemDataGridView[pvtintInvTotal, RowIndex].Value;
                }
            }
        }

        private void CheckToAddNewInvoiceItemRow(int RowIndex)
        {
            if (RowIndex == this.dgvInvoiceItemDataGridView.RowCount - 1)
            {
                if (RowIndex == -1)
                {
                    DataRowView myDataRowView = null;

                    if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                    {
                        myDataRowView = pvtCompanyInvoiceItemCurrentDataView.AddNew();
                    }
                    else
                    {
                        myDataRowView = pvtCompanyInvoiceItemHistoryDataView.AddNew();
                    }

                    myDataRowView["COMPANY_NO"] = pvtint64CompanyNo;

                    if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                    {
                        if (pvtCompanyInvoiceItemCurrentDataView.Count == 1)
                        {
                            myDataRowView["INVOICE_LINE_NO"] = 1;
                        }
                        else
                        {
                            myDataRowView["INVOICE_LINE_NO"] = Convert.ToInt64(pvtCompanyInvoiceItemCurrentDataView[pvtCompanyInvoiceItemCurrentDataView.Count - 1]["INVOICE_LINE_NO"]) + 1;
                        }
                    }
                    else
                    {
                        //History
                        if (pvtCompanyInvoiceItemHistoryDataView.Count == 1)
                        {
                            myDataRowView["INVOICE_LINE_NO"] = 1;
                        }
                        else
                        {
                            myDataRowView["INVOICE_LINE_NO"] = Convert.ToInt64(pvtCompanyInvoiceItemCurrentDataView[pvtCompanyInvoiceItemHistoryDataView.Count - 1]["INVOICE_LINE_NO"]) + 1;
                        }

                    }

                    myDataRowView["INVOICE_LINE_DATE"] = DateTime.Now;

                    if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                    {
                        myDataRowView["INVOICE_LINE_OPTION_NO"] = 0;
                    }

                    myDataRowView["INVOICE_LINE_CHOICE_NO"] = 0;
                    myDataRowView["INVOICE_LINE_QTY"] = 0;
                    myDataRowView["INVOICE_LINE_DESC"] = "";
                    myDataRowView["INVOICE_LINE_UNIT_PRICE"] = 0;

                    myDataRowView.EndEdit();

                    if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                    {
                        this.dgvInvoiceItemDataGridView.Rows.Add(DateTime.Now.ToString(pvtstrDateFormat),
                                                                    "Next",
                                                                    "Default",
                                                                    Convert.ToDouble(this.pvtCompanyInvoiceItemCurrentDataView[0]["INVOICE_LINE_QTY"]).ToString("####0.00"),
                                                                    this.pvtCompanyInvoiceItemCurrentDataView[0]["INVOICE_LINE_DESC"].ToString(),
                                                                    Convert.ToDouble(this.pvtCompanyInvoiceItemCurrentDataView[0]["INVOICE_LINE_UNIT_PRICE"]).ToString("#########0.00"),
                                                                    0.ToString("#########0.00"),
                                                                    0);
                    }
                    else
                    {
                        //History
                        this.dgvInvoiceItemDataGridView.Rows.Add(DateTime.Now.ToString(pvtstrDateFormat),
                                                                "Next",
                                                                "Default",
                                                                Convert.ToDouble(this.pvtCompanyInvoiceItemHistoryDataView[0]["INVOICE_LINE_QTY"]).ToString("####0.00"),
                                                                this.pvtCompanyInvoiceItemHistoryDataView[0]["INVOICE_LINE_DESC"].ToString(),
                                                                Convert.ToDouble(this.pvtCompanyInvoiceItemHistoryDataView[0]["INVOICE_LINE_UNIT_PRICE"]).ToString("#########0.00"),
                                                                0.ToString("#########0.00"),
                                                                0);

                    }
                }
                else
                {
                    if (this.dgvInvoiceItemDataGridView[pvtintInvDate, RowIndex].Value != null
                    && this.dgvInvoiceItemDataGridView[pvtintInvOption, RowIndex].Value != null
                    && this.dgvInvoiceItemDataGridView[pvtintInvChoice, RowIndex].Value != null
                    && this.dgvInvoiceItemDataGridView[pvtintInvQuantity, RowIndex].Value != null
                    && this.dgvInvoiceItemDataGridView[pvtintInvDesc, RowIndex].Value != null
                    && this.dgvInvoiceItemDataGridView[pvtintInvUnitPrice, RowIndex].Value != null)
                    {
                        if (Convert.ToDouble(this.dgvInvoiceItemDataGridView[pvtintInvQuantity, RowIndex].Value) != 0
                            && this.dgvInvoiceItemDataGridView[pvtintInvDesc, RowIndex].Value.ToString().Trim() != ""
                            && Convert.ToDouble(this.dgvInvoiceItemDataGridView[pvtintInvUnitPrice, RowIndex].Value) != 0)
                        {
                            if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                            {
                                DataRowView myDataRowView = pvtCompanyInvoiceItemCurrentDataView.AddNew();

                                myDataRowView["COMPANY_NO"] = pvtint64CompanyNo;

                                DataView CompanyInvoiceItemCurrentDataView = new DataView(pvtDataSet.Tables["CompanyInvoiceItemCurrent"],
                                    "COMPANY_NO = " + pvtint64CompanyNo,
                                    "INVOICE_LINE_NO DESC",
                                    DataViewRowState.CurrentRows);

                                if (CompanyInvoiceItemCurrentDataView.Count == 0)
                                {
                                    myDataRowView["INVOICE_LINE_NO"] = 1;
                                }
                                else
                                {
                                    myDataRowView["INVOICE_LINE_NO"] = Convert.ToInt64(CompanyInvoiceItemCurrentDataView[0]["INVOICE_LINE_NO"]) + 1;
                                }

                                myDataRowView["INVOICE_LINE_DATE"] = DateTime.Now;
                                myDataRowView["INVOICE_LINE_OPTION_NO"] = 0;
                                myDataRowView["INVOICE_LINE_CHOICE_NO"] = 0;
                                myDataRowView["INVOICE_LINE_QTY"] = 0;
                                myDataRowView["INVOICE_LINE_DESC"] = "";
                                myDataRowView["INVOICE_LINE_UNIT_PRICE"] = 0;

                                myDataRowView.EndEdit();

                                int intRow = pvtCompanyInvoiceItemCurrentDataView.Count - 1;

                                this.dgvInvoiceItemDataGridView.Rows.Add(DateTime.Now.ToString(pvtstrDateFormat),
                                                                            "Next",
                                                                            "Default",
                                                                            Convert.ToDouble(this.pvtCompanyInvoiceItemCurrentDataView[pvtCompanyInvoiceItemCurrentDataView.Count - 1]["INVOICE_LINE_QTY"]).ToString("####0.00"),
                                                                            this.pvtCompanyInvoiceItemCurrentDataView[pvtCompanyInvoiceItemCurrentDataView.Count - 1]["INVOICE_LINE_DESC"].ToString(),
                                                                            Convert.ToDouble(this.pvtCompanyInvoiceItemCurrentDataView[pvtCompanyInvoiceItemCurrentDataView.Count - 1]["INVOICE_LINE_UNIT_PRICE"]).ToString("#########0.00"),
                                                                            0.ToString("#########0.00"),
                                                                            intRow);

                            }
                            else
                            {
                                DataRowView myDataRowView = pvtCompanyInvoiceItemHistoryDataView.AddNew();

                                myDataRowView["COMPANY_NO"] = pvtint64CompanyNo;

                                int intInvoiceNumber = Convert.ToInt32(this.pvtCompanyInvoiceHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["INVOICE_NUMBER"]);

                                myDataRowView["INVOICE_NUMBER"] = intInvoiceNumber;

                                DataView CompanyInvoiceItemHistoryDataView = new DataView(pvtDataSet.Tables["CompanyInvoiceItemHistory"],
                                    "COMPANY_NO = " + pvtint64CompanyNo + " AND INVOICE_NUMBER = " + intInvoiceNumber,
                                    "INVOICE_LINE_NO DESC",
                                    DataViewRowState.CurrentRows);

                                if (CompanyInvoiceItemHistoryDataView.Count == 0)
                                {
                                    myDataRowView["INVOICE_LINE_NO"] = 1;
                                }
                                else
                                {
                                    myDataRowView["INVOICE_LINE_NO"] = Convert.ToInt64(CompanyInvoiceItemHistoryDataView[0]["INVOICE_LINE_NO"]) + 1;
                                }

                                myDataRowView["INVOICE_LINE_DATE"] = DateTime.Now;
                                myDataRowView["INVOICE_LINE_CHOICE_NO"] = 0;
                                myDataRowView["INVOICE_LINE_QTY"] = 0;
                                myDataRowView["INVOICE_LINE_DESC"] = "";
                                myDataRowView["INVOICE_LINE_UNIT_PRICE"] = 0;

                                myDataRowView.EndEdit();

                                int intRow = pvtCompanyInvoiceItemHistoryDataView.Count - 1;

                                this.dgvInvoiceItemDataGridView.Rows.Add(DateTime.Now.ToString(pvtstrDateFormat),
                                                                            "",
                                                                            "Default",
                                                                            Convert.ToDouble(this.pvtCompanyInvoiceItemHistoryDataView[pvtCompanyInvoiceItemHistoryDataView.Count - 1]["INVOICE_LINE_QTY"]).ToString("####0.00"),
                                                                            this.pvtCompanyInvoiceItemHistoryDataView[pvtCompanyInvoiceItemHistoryDataView.Count - 1]["INVOICE_LINE_DESC"].ToString(),
                                                                            Convert.ToDouble(this.pvtCompanyInvoiceItemHistoryDataView[pvtCompanyInvoiceItemHistoryDataView.Count - 1]["INVOICE_LINE_UNIT_PRICE"]).ToString("#########0.00"),
                                                                            0.ToString("#########0.00"),
                                                                            intRow);
                            }
                        }
                    }
                }
            }
        }
        
        private void CheckToAddNewStatementItemRow(int RowIndex)
        {
            if (RowIndex == this.dgvInvoiceItemDataGridView.RowCount - 1)
            {
                if (RowIndex == -1)
                {
                    DataRowView myDataRowView = null;

                    if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                    {
                        myDataRowView = pvtCompanyStatementItemCurrentDataView.AddNew();
                    }
                    else
                    {
                        myDataRowView = pvtCompanyStatementItemHistoryDataView.AddNew();
                    }

                    myDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                                       
                    if (pvtCompanyStatementItemCurrentDataView.Count == 1)
                    {
                        myDataRowView["STATEMENT_LINE_NO"] = 1;
                    }
                    else
                    {
                        myDataRowView["STATEMENT_LINE_NO"] = Convert.ToInt64(pvtCompanyStatementItemCurrentDataView[pvtCompanyStatementItemCurrentDataView.Count - 1]["STATEMENT_LINE_NO"]) + 1;
                    }
                 
                    myDataRowView["STATEMENT_LINE_DATE"] = DateTime.Now;
                    myDataRowView["INVOICE_NUMBER"] = 0;

                    myDataRowView["STATEMENT_LINE_DESC"] = "";
                    myDataRowView["STATEMENT_LINE_DR_TOTAL"] = 0;
                    myDataRowView["STATEMENT_LINE_CR_TOTAL"] = 0;

                    myDataRowView["LOCK_IND"] = "N";

                    myDataRowView.EndEdit();
                    
                    this.dgvInvoiceItemDataGridView.Rows.Add("",
                                                             "",
                                                             "",
                                                             "",
                                                             "",
                                                             0.ToString("#########0.00"),
                                                             0.ToString("#########0.00"),
                                                             0);
                }
                else
                {
                    if (this.dgvInvoiceItemDataGridView[pvtintInvDate, RowIndex].Value != null
                    && this.dgvInvoiceItemDataGridView[pvtintInvChoice, RowIndex].Value != null
                    && this.dgvInvoiceItemDataGridView[pvtintInvDesc, RowIndex].Value != null
                    && this.dgvInvoiceItemDataGridView[pvtintInvUnitPrice, RowIndex].Value != null
                    && this.dgvInvoiceItemDataGridView[pvtintInvTotal, RowIndex].Value != null)
                    {
                        if (this.dgvInvoiceItemDataGridView[pvtintInvDate, RowIndex].Value.ToString().Trim() != ""
                        &&  this.dgvInvoiceItemDataGridView[pvtintInvDesc, RowIndex].Value.ToString().Trim() != "")
                        {
                            DataRowView myDataRowView = pvtCompanyStatementItemCurrentDataView.AddNew();

                            myDataRowView["COMPANY_NO"] = pvtint64CompanyNo;

                            DataView CompanyStatementItemCurrentDataView = new DataView(pvtDataSet.Tables["CompanyStatementItemCurrent"],
                                "COMPANY_NO = " + pvtint64CompanyNo,
                                "STATEMENT_LINE_NO DESC",
                                DataViewRowState.CurrentRows);

                            if (CompanyStatementItemCurrentDataView.Count == 0)
                            {
                                myDataRowView["STATEMENT_LINE_NO"] = 1;
                            }
                            else
                            {
                                myDataRowView["STATEMENT_LINE_NO"] = Convert.ToInt64(CompanyStatementItemCurrentDataView[0]["STATEMENT_LINE_NO"]) + 1;
                            }

                            //myDataRowView["STATEMENT_LINE_DATE"] = DateTime.Now;
                            myDataRowView["INVOICE_NUMBER"] = 0;
                            myDataRowView["STATEMENT_LINE_DESC"] = "";
                            myDataRowView["STATEMENT_LINE_DR_TOTAL"] = 0;
                            myDataRowView["STATEMENT_LINE_CR_TOTAL"] = 0;
                            myDataRowView["LOCK_IND"] = "N";

                            myDataRowView.EndEdit();

                            int intRow = pvtCompanyStatementItemCurrentDataView.Count - 1;

                            this.dgvInvoiceItemDataGridView.Rows.Add("",
                                                                     "",
                                                                     "",
                                                                     "",
                                                                     "",
                                                                     0.ToString("#########0.00"),
                                                                     0.ToString("#########0.00"),
                                                                     intRow);
                        }
                    }
                }
            }
        }

        private void dgvInvoiceItemDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvInvoiceItemDataGridView.Rows.Count > 0
            && pvtblnInvoiceItemDataGridViewLoaded == true
            && pvtintInvoiceItemDataGridViewRowIndex != e.RowIndex)
            {
                pvtintInvoiceItemDataGridViewRowIndex = e.RowIndex;

                pvtintCompanyInvoiceItemDataViewRowIndex = Convert.ToInt32(this.dgvInvoiceItemDataGridView[pvtintInvRow, e.RowIndex].Value);
            }
        }

        private void btnInvoicePrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (pvtblnLoadedExportButton == false)
                {
                    ToolStrip toolStrip = (ToolStrip)reportViewer.Controls.Find("toolStrip1", true)[0];

                    for (int intCount = 0;intCount < toolStrip.Items.Count;intCount++)
                    {
                        ToolStripItem tsiToolStripItem = toolStrip.Items[intCount];

                        if (tsiToolStripItem.Name == "export")
                        {
                            toolStrip.Items[intCount].Visible = false;
                        }
                    }

                    ToolStripMenuItem exportToolStripMenuItem = new ToolStripMenuItem("Export");

                    exportToolStripMenuItem.ToolTipText = "Export document to Server (pdf Format)";

                    exportToolStripMenuItem.Click += Export_Click;
                    toolStrip.Items.Add(exportToolStripMenuItem);
                                       
                    pvtblnLoadedExportButton = true;
                }

                if (pvtstrDocumentType == "I")
                {
                    if (pvtCompanyInvoiceHistoryDataView.Count > 0)
                    {
                        pvtstrPrintedDocumentType = pvtCompanyInvoiceHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["INVOICE_TYPE_IND"].ToString();

                        this.tabControlMain.SelectedIndex = 1;

                        object[] objParm = new object[3];
                        objParm[0] = Convert.ToInt64(this.pvtDataSet.Tables["Company"].Rows[0]["COMPANY_NO"]);
                        objParm[1] = Convert.ToInt32(pvtCompanyInvoiceHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["INVOICE_NUMBER"]);
                        objParm[2] = pvtCompanyInvoiceHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["INVOICE_TYPE_IND"].ToString();
                        
                        pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Invoice", objParm);

                        DataSet TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                        Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", TempDataSet.Tables["PrintInvoice"]);

                        this.reportViewer.LocalReport.ReportEmbeddedResource = "CompanyPaymentOptions.ReportInvoice.rdlc";

                        this.reportViewer.LocalReport.DataSources.Clear();
                        this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                        //Calculates Number of Pages in Report and Display in Viewer
                        this.reportViewer.PageCountMode = Microsoft.Reporting.WinForms.PageCountMode.Actual;

                        this.reportViewer.RefreshReport();
                        this.reportViewer.Focus();
                    }
                }
                else
                {
                    if (pvtstrDocumentType == "S")
                    {
                        if (pvtCompanyStatementHistoryDataView.Count > 0)
                        {
                            pvtstrPrintedDocumentType = "S";

                            this.tabControlMain.SelectedIndex = 1;

                            object[] objParm = new object[2];
                            objParm[0] = Convert.ToInt64(this.pvtDataSet.Tables["Company"].Rows[0]["COMPANY_NO"]);
                            objParm[1] = Convert.ToInt32(pvtCompanyStatementHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["STATEMENT_NUMBER"]);

                            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Statement", objParm);

                            DataSet TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                            Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", TempDataSet.Tables["PrintStatement"]);

                            this.reportViewer.LocalReport.ReportEmbeddedResource = "CompanyPaymentOptions.ReportStatement.rdlc";

                            this.reportViewer.LocalReport.DataSources.Clear();
                            this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                            //Calculates Number of Pages in Report and Display in Viewer
                            this.reportViewer.PageCountMode = Microsoft.Reporting.WinForms.PageCountMode.Actual;

                            this.reportViewer.RefreshReport();
                            this.reportViewer.Focus();
                        }
                    }
                    else
                    {
                        if (pvtCompanyStatementHistoryDataView.Count > 0)
                        {
                            pvtstrPrintedDocumentType = "A";

                            this.tabControlMain.SelectedIndex = 1;
                            
                            object[] objParm = new object[2];
                            objParm[0] = Convert.ToInt64(this.pvtDataSet.Tables["Company"].Rows[0]["COMPANY_NO"]);
                            objParm[1] = Convert.ToInt32(pvtCompanyStatementHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["STATEMENT_NUMBER"]);

                            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_AuditTrail", objParm);

                            DataSet TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                            Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("Report", TempDataSet.Tables["PrintAuditReport"]);

                            this.reportViewer.LocalReport.ReportEmbeddedResource = "CompanyPaymentOptions.ReportAuditReport.rdlc";

                            this.reportViewer.LocalReport.DataSources.Clear();
                            this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);

                            //Calculates Number of Pages in Report and Display in Viewer
                            this.reportViewer.PageCountMode = Microsoft.Reporting.WinForms.PageCountMode.Actual;

                            this.reportViewer.RefreshReport();
                            this.reportViewer.Focus();
                        }
                    }
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                string strInvoiceMonthly = "Y";
                string strInvoiceType = "I";

                Button myButton = (Button)sender;

                if (myButton.Name == "btnStandaloneGenerate")
                {
                    strInvoiceMonthly = "N";

                    if (rbnProformaInvoice.Checked == true)
                    {
                        //ProForma
                        strInvoiceType = "P";
                    }
                }

                if (this.txtWageAmount.Text == "")
                {
                    CustomMessageBox.Show("No Wages Amount Specified,", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (this.txtSalaryAmount.Text == "")
                {
                    CustomMessageBox.Show("No Salaries Amount Specified,", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (this.txtTimeAttendAmount.Text == "")
                {
                    CustomMessageBox.Show("No Time Attendance Amount Specified,", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (this.txtHourlySupportAmount.Text == "")
                {
                    CustomMessageBox.Show("No Hourly Support Amount Specified,", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (this.txtContactPerson1.Text == "")
                {
                    CustomMessageBox.Show("No Contact Person(1)", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (this.txtPhone1.Text == "")
                {
                    CustomMessageBox.Show("No Phone (1)", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (this.txtEmail1.Text == "")
                {
                    CustomMessageBox.Show("No Contact Email(1)", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (pvtstrDocumentType == "I")
                {
                    //Check Spreadsheet
                    for (int intCheckRow = 0; intCheckRow < this.dgvInvoiceItemDataGridView.Rows.Count; intCheckRow++)
                    {
                        if (dgvInvoiceItemDataGridView[pvtintInvOption, intCheckRow].Value.ToString() == "Pending")
                        {
                            DialogResult dlgDialogResult = CustomMessageBox.Show("There are Invoice Items that won't be Linked (Pending)?\nWould you like to Continue?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                            if (dlgDialogResult == DialogResult.No)
                            {
                                return;
                            }
                        }
                    }
                }

                if (pvtstrDocumentType == "I")
                {
                    if (strInvoiceMonthly == "N")
                    {
                        //INVOICE_LINE_OPTION_NO = 0 = Next (Not Pending)
                        DataView myCompanyInvoiceItemDataView = new DataView(pvtDataSet.Tables["CompanyInvoiceItemCurrent"],
                          "COMPANY_NO = " + pvtint64CompanyNo + " AND INVOICE_LINE_OPTION_NO = 0",
                          "INVOICE_LINE_NO",
                          DataViewRowState.CurrentRows);

                        if (myCompanyInvoiceItemDataView.Count == 0)
                        {
                            CustomMessageBox.Show("No Invoice Items to Generate an Invoice", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                object[] objParm = new object[4];

                if (pvtstrDocumentType == "S")
                {
                    objParm = new object[3];
                }

                objParm[0] = Convert.ToInt64(this.pvtDataSet.Tables["Company"].Rows[0]["COMPANY_NO"]);

                if (pvtstrDocumentType == "I")
                {
                    if (strInvoiceMonthly == "Y")
                    {
                        objParm[1] = Convert.ToDateTime(pvtCompanyInvoiceYYYYMMDDDataView[this.cboInvoiceGen.SelectedIndex]["INVOICE_DATE"]).ToString("yyyyMMdd");
                    }
                    else
                    {
                        string[] strDateSplit = this.cboInvoiceDate.SelectedItem.ToString().Split(' ');

                        objParm[1] = strDateSplit[strDateSplit.Count() - 1];
                    }

                    //Check Line Item Date Not Graeter Than Invoice Date
                    for (int intCheckRow = 0; intCheckRow < pvtCompanyInvoiceItemCurrentDataView.Count; intCheckRow++)
                    {
                        //Next
                        if (Convert.ToInt32(pvtCompanyInvoiceItemCurrentDataView[intCheckRow]["INVOICE_LINE_OPTION_NO"]) == 0)
                        {
                            if (Convert.ToInt32(Convert.ToDateTime(pvtCompanyInvoiceItemCurrentDataView[intCheckRow]["INVOICE_LINE_DATE"]).ToString("yyyyMMdd")) > Convert.ToInt32(objParm[1]))
                            {
                                CustomMessageBox.Show("Invoice Date Less Than Line Item Date\nAction Cancelled.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                                return;
                            }
                        }
                    }
                }
                else
                {
                    objParm[1] = pvtCompanyStatementYYYYMMDataView[this.cboInvoiceGen.SelectedIndex]["STATEMENT_YYYYMM"].ToString();

                    //Check Line Item Date Not Graeter Than Invoice Date
                    for (int intCheckRow = 0; intCheckRow < pvtCompanyStatementItemCurrentDataView.Count; intCheckRow++)
                    {
                        if (Convert.ToInt32(Convert.ToDateTime(pvtCompanyStatementItemCurrentDataView[intCheckRow]["STATEMENT_LINE_DATE"]).ToString("yyyyMM")) > Convert.ToInt32(objParm[1]))
                        {
                            CustomMessageBox.Show("Statement Date Less Than Line Item Date\nAction Cancelled.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            return;
                        }
                    }
                }

                objParm[2] = strInvoiceMonthly;

                if (pvtstrDocumentType == "I")
                {
                    objParm[3] = strInvoiceType;
                }

                if (pvtstrDocumentType == "I")
                {
                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Generate_Invoice", objParm);
                }
                else
                {
                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Generate_Statement", objParm);
                }

                Remove_Company_Invoices_And_Statements();

                DataSet TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                pvtDataSet.Merge(TempDataSet);

                this.pvtDataSet.AcceptChanges();

                if (pvtstrDocumentType == "I")
                {
                    Load_Company_Invoices();
                }
                else
                {
                    if (pvtstrDocumentType == "S")
                    {
                        Load_Company_Statements(true);
                    }
                    else
                    {
                        Load_Company_Statements(false);
                    }
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void dgvInvoiceDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvInvoiceDataGridView.Rows.Count > 0
            && pvtblnInvoiceDataGridViewLoaded == true
            && pvtintInvoiceDataGridViewRowIndex != e.RowIndex)
            {
                this.txtInvoicesTotal.Text = "0.00";
                pvtintInvoiceDataGridViewRowIndex = e.RowIndex;

                pvtintCompanyInvoiceDataViewRowIndex = Convert.ToInt32(this.dgvInvoiceDataGridView[8, e.RowIndex].Value);

                btnConvert.Visible = false;

                if (pvtstrDocumentType == "I")
                {
                    DataGridViewComboBoxColumn myDataGridViewComboBoxColumn = (DataGridViewComboBoxColumn)dgvInvoiceItemDataGridView.Columns[pvtintInvChoice];

                    myDataGridViewComboBoxColumn.Items.Clear();

                    //Remove All Invoice Items - New Batch from Server
                    for (int intRow = 0; intRow < this.pvtDataSet.Tables["ItemChoice"].Rows.Count; intRow++)
                    {
                        myDataGridViewComboBoxColumn.Items.Add(this.pvtDataSet.Tables["ItemChoice"].Rows[intRow]["INVOICE_LINE_CHOICE_DESC"].ToString());
                    }
                    
                    myDataGridViewComboBoxColumn = (DataGridViewComboBoxColumn)dgvInvoiceItemDataGridView.Columns[1];
                 
                    if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                    {
                        this.grbMonthlyGenerate.Visible = true;
                        this.grbStandaloneGenerate.Visible = true;
                        this.btnInvoicePrint.Enabled = false;
                       
                        if (myDataGridViewComboBoxColumn.Items.Count != 2)
                        {
                            myDataGridViewComboBoxColumn.Items.Clear();

                            myDataGridViewComboBoxColumn.Items.Add("Next");
                            myDataGridViewComboBoxColumn.Items.Add("Pending");
                        }

                        this.btnUpdate.Enabled = true;
                    }
                    else
                    {
                        if (pvtCompanyInvoiceHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["INVOICE_TYPE_IND"].ToString() == "P")
                        {
                            btnConvert.Visible = true;
                        }

                        this.grbMonthlyGenerate.Visible = false;
                        this.grbStandaloneGenerate.Visible = false;
                        this.btnInvoicePrint.Enabled = true;
                        
                        if (myDataGridViewComboBoxColumn.Items.Count != 1)
                        {
                            myDataGridViewComboBoxColumn.Items.Clear();

                            myDataGridViewComboBoxColumn.Items.Add("");
                        }
                    }

                    Load_Company_Invoice_Items();
                }
                else
                {
                    //Statements
                    DataGridViewComboBoxColumn myDataGridViewComboBoxColumn = (DataGridViewComboBoxColumn)dgvInvoiceItemDataGridView.Columns[pvtintInvChoice];
                    myDataGridViewComboBoxColumn.Items.Clear();
                
                    if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                    {
                        this.btnInvoicePrint.Enabled = false;
                        this.grbMonthlyGenerate.Visible = true;

                        //Current
                        for (int intRow = 0; intRow < this.pvtDataSet.Tables["CompanyStatementInvoiceChoiceCurrent"].Rows.Count; intRow++)
                        {
                            myDataGridViewComboBoxColumn.Items.Add(this.pvtDataSet.Tables["CompanyStatementInvoiceChoiceCurrent"].Rows[intRow]["INVOICE_NUMBER"].ToString());
                        }
                    }
                    else
                    {
                        this.btnInvoicePrint.Enabled = true;
                        this.grbMonthlyGenerate.Visible = false;

                        pvtCompanyStatementInvoiceChoiceHistoryDataView = null;
                        pvtCompanyStatementInvoiceChoiceHistoryDataView = new DataView(pvtDataSet.Tables["CompanyStatementInvoiceChoiceHistory"],
                        "STATEMENT_NUMBER = " + pvtCompanyStatementHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["STATEMENT_NUMBER"].ToString(),
                        "INVOICE_NUMBER",
                        DataViewRowState.CurrentRows);

                        //History
                        for (int intRow = 0; intRow < this.pvtCompanyStatementInvoiceChoiceHistoryDataView.Count; intRow++)
                        {
                            myDataGridViewComboBoxColumn.Items.Add(pvtCompanyStatementInvoiceChoiceHistoryDataView[intRow]["INVOICE_NUMBER"].ToString());
                        }
                    }

                    Load_Company_Statement_Items();
                }
            }
        }

        private void Phone_TextChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                TextBox myTextBox = (TextBox)sender;

                if (myTextBox.Name == "txtPhone1")
                {
                    this.pvtDataSet.Tables["Company"].Rows[0]["PHONE1"] = myTextBox.Text.Trim();
                }
                else
                {
                    if (myTextBox.Name == "txtPhone2")
                    {
                        this.pvtDataSet.Tables["Company"].Rows[0]["PHONE2"] = myTextBox.Text.Trim();
                    }
                }
            }
        }

        private void btnInvoiceItemDeleteRec_Click(object sender, EventArgs e)
        {
            //Delete Record
            if (pvtstrDocumentType == "I")
            {
                if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                {
                    this.pvtCompanyInvoiceItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex].Delete();
                }
                else
                {
                    this.pvtCompanyInvoiceItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex].Delete();
                }
            }
            else
            {
                if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                {
                    this.pvtCompanyStatementItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex].Delete();
                }
                else
                {
                    this.pvtCompanyStatementItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex].Delete();
                }
            }

            DataGridViewRow myDataGridViewRow = this.dgvInvoiceItemDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvInvoiceItemDataGridView)];

            this.dgvInvoiceItemDataGridView.Rows.Remove(myDataGridViewRow);

            if (pvtstrDocumentType == "I")
            {
                CheckToAddNewInvoiceItemRow(this.dgvInvoiceItemDataGridView.RowCount - 1);
            }
            else
            {
                CheckToAddNewStatementItemRow(this.dgvInvoiceItemDataGridView.RowCount - 1);
            }
        }

        private void dgvInvoiceItemDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.ColumnIndex == pvtintInvDate)
                {
                    DateTime myDateTime;

                    try
                    {
                        myDateTime = DateTime.ParseExact(dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                        if (pvtstrDocumentType == "I")
                        {
                            if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                            {
                                pvtCompanyInvoiceItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_DATE"] = myDateTime;
                            }
                            else
                            {
                                pvtCompanyInvoiceItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["INVOICE_LINE_DATE"] = myDateTime;
                            }
                        }
                        else
                        {
                            if (pvtintCompanyInvoiceDataViewRowIndex == -1)
                            {
                                pvtCompanyStatementItemCurrentDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["STATEMENT_LINE_DATE"] = myDateTime;
                            }
                            else
                            {
                                pvtCompanyStatementItemHistoryDataView[pvtintCompanyInvoiceItemDataViewRowIndex]["STATEMENT_LINE_DATE"] = myDateTime;
                            }
                        }
                    }
                    catch
                    {
                        dgvInvoiceItemDataGridView[e.ColumnIndex, e.RowIndex].Value = null;
                    }
                }
            }
        }

        private void dgvDocumentTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnDocumentTypeDataGridViewLoaded == true
            && pvtintDocumentTypeDataGridViewRowIndex != e.RowIndex)
            {
                pvtintDocumentTypeDataGridViewRowIndex = e.RowIndex;

                pvtstrDocumentType = dgvDocumentTypeDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, 1);

                btnConvert.Visible = false;
                this.btnUpdate.Enabled = true;

                if (pvtstrDocumentType == "I")
                {
                    Set_Bottom_Page_Controls_Visibilty(true);

                    this.lblInvoiceTotals.Visible = true;
                    this.txtInvoicesTotal.Visible = true;

                    this.lblsLastStatementBalance.Visible = true;
                    this.txtStatementBalance.Visible = true;

                    dgvInvoiceDataGridView.Height = pvtintInvHeight;
                    dgvInvoiceItemDataGridView.Height = pvtintInvItemHeight;
                    
                    this.btnInvoiceItemDeleteRec.Left = this.dgvInvoiceDataGridView.Left;

                    grbInvoiceLegend.Visible = true;

                    this.grbInvoiceType.Visible = true;
                    this.rbnInvoice.Checked = true;

                    if (dgvInvoiceItemDataGridView.Columns[pvtintInvOption].Visible == false)
                    {
                        dgvInvoiceItemDataGridView.Columns[pvtintInvOption].Visible = true;
                        dgvInvoiceItemDataGridView.Columns[pvtintInvQuantity].Visible = true;

                        dgvInvoiceItemDataGridView.Columns[pvtintInvDesc].Width = pvtintInvItemDescWidth;
                    }

                    this.grbStandaloneGenerate.Visible = true;

                    this.lblInvoiceDesc.Text = "Invoices";
                    this.lblInvoiceItemDesc.Text = "Invoice Items";

                    grbMonthlyGenerate.Text = "Monthly Invoice";

                    dgvInvoiceDataGridView.Columns[pvtintInvDate].HeaderText = "Invoice Period";
                    dgvInvoiceDataGridView.Columns[pvtintInvOption].HeaderText = "Invoice No.";
                    dgvInvoiceDataGridView.Columns[pvtintInvChoice].HeaderText = "Type";
                    
                    dgvInvoiceDataGridView.Columns[6].HeaderText = "VAT";
                    dgvInvoiceDataGridView.Columns[7].HeaderText = "Final Total";

                    dgvInvoiceItemDataGridView.Columns[pvtintInvChoice].HeaderText = "Choice";

                    dgvInvoiceItemDataGridView.Columns[pvtintInvUnitPrice].HeaderText = "Price";
                    dgvInvoiceItemDataGridView.Columns[pvtintInvTotal].HeaderText = "Total";

                    dgvInvoiceItemDataGridView.Columns[pvtintInvUnitPrice].ReadOnly = false;
                    dgvInvoiceItemDataGridView.Columns[pvtintInvTotal].ReadOnly = true;

                    if (pvtCompanyInvoiceHistoryDataView != null)
                    {
                        Load_Company_Invoices();
                    }
                }
                else
                {
                    dgvInvoiceDataGridView.Columns[pvtintInvDate].HeaderText = "Statement Period";
                    dgvInvoiceDataGridView.Columns[pvtintInvOption].HeaderText = "Statement No.";
                    dgvInvoiceDataGridView.Columns[pvtintInvChoice].HeaderText = "Date";

                    dgvInvoiceDataGridView.Columns[6].HeaderText = "Open Balance";
                    dgvInvoiceDataGridView.Columns[7].HeaderText = "Close Balance";

                    if (pvtstrDocumentType == "S")
                    {
                        Set_Bottom_Page_Controls_Visibilty(true);

                        this.lblInvoiceTotals.Visible = false;
                        this.txtInvoicesTotal.Visible = false;

                        this.lblsLastStatementBalance.Visible = false;
                        this.txtStatementBalance.Visible = false;

                        dgvInvoiceDataGridView.Height = pvtintInvHeight;
                        dgvInvoiceItemDataGridView.Height = pvtintInvItemHeight + 38;

                        grbInvoiceLegend.Visible = false;
                        this.grbInvoiceType.Visible = false;

                        this.btnInvoiceItemDeleteRec.Left = this.grbMonthlyGenerate.Left;

                        dgvInvoiceItemDataGridView.Columns[pvtintInvOption].Visible = false;
                        dgvInvoiceItemDataGridView.Columns[pvtintInvQuantity].Visible = false;

                        dgvInvoiceItemDataGridView.Columns[pvtintInvDesc].Width = pvtintInvItemDescWidth + (pvtintInvOptionWidth + pvtintInvQuantityWidth);

                        this.grbStandaloneGenerate.Visible = false;

                        this.lblInvoiceDesc.Text = "Statements";
                        this.lblInvoiceItemDesc.Text = "Statement Items";

                        grbMonthlyGenerate.Text = "Monthly Statement";
                        
                        dgvInvoiceItemDataGridView.Columns[pvtintInvChoice].HeaderText = "Invoice Number";

                        dgvInvoiceItemDataGridView.Columns[pvtintInvUnitPrice].HeaderText = "Debit";
                        dgvInvoiceItemDataGridView.Columns[pvtintInvTotal].HeaderText = "Credit";

                        dgvInvoiceItemDataGridView.Columns[pvtintInvUnitPrice].ReadOnly = true;
                        dgvInvoiceItemDataGridView.Columns[pvtintInvTotal].ReadOnly = false;

                        Load_Company_Statements(true);
                    }
                    else
                    {
                        //Audit Trail
                        this.btnUpdate.Enabled = false;
                        this.lblInvoiceDesc.Text = "Audit Report - Select From Statement Date";

                        Set_Bottom_Page_Controls_Visibilty(false);

                        Load_Company_Statements(false);

                        dgvInvoiceDataGridView.Height = pvtintInvHeight + pvtintInvItemHeight + 53;
                        this.grbStandaloneGenerate.Visible = false;
                    }
                }
            }
        }

        private void Set_Bottom_Page_Controls_Visibilty(bool blnVisible)
        {
            this.lblInvoiceItemDesc.Visible = blnVisible;
            this.dgvInvoiceItemDataGridView.Visible = blnVisible;
            this.lblInvoiceTotals.Visible = blnVisible;
            this.lblsLastStatementBalance.Visible = blnVisible;
            this.txtInvoicesTotal.Visible = blnVisible;
            this.txtStatementBalance.Visible = blnVisible;
            this.btnInvoiceItemDeleteRec.Visible = blnVisible;
        }

        private void CreateEmailMessage()
        {
            if (this.dgvSelectedInvoicesDataGridView.Rows.Count > 0
            || this.dgvSelectedStatementsDataGridView.Rows.Count > 0)
            {
                this.btnSendEmail.Enabled = true;

                this.txtEmailMessage.Text = "Dear " + this.pvtDataSet.Tables["Company"].Rows[0]["PERSON_NAMES1"].ToString();
                this.txtEmailMessage.Text += "\r\n\r\nSee attached ";

                if (this.dgvSelectedInvoicesDataGridView.Rows.Count > 0)
                {
                    this.txtEmailMessage.Text += "Invoice";
                    
                    if (this.dgvSelectedInvoicesDataGridView.Rows.Count > 1)
                    {
                        this.txtEmailMessage.Text += "s";
                    }

                    if (this.dgvSelectedStatementsDataGridView.Rows.Count > 0)
                    {
                        this.txtEmailMessage.Text += " and Statement";
                        if (this.dgvSelectedStatementsDataGridView.Rows.Count > 1)
                        {
                            this.txtEmailMessage.Text += "s";
                        }
                    }
                }
                else
                {
                    this.txtEmailMessage.Text += "Statement";
                   
                    if (this.dgvSelectedInvoicesDataGridView.Rows.Count > 1)
                    {
                        this.txtEmailMessage.Text += "s";
                    }
                }

                this.txtEmailMessage.Text += ".";

                this.txtEmailMessage.Text += "\r\n\r\nPlease use Invoice Number as Reference Number for any Banking Transaction.";

                this.txtEmailMessage.Text += "\r\n\r\nThanks";

                this.txtEmailMessage.Text += "\r\n\r\nRegards";
                this.txtEmailMessage.Text += "\r\nErrol";
            }
            else
            {
                this.btnSendEmail.Enabled = false;
                this.txtEmailMessage.Text = "";
            }
        }
        
        private void btnAddInvoice_Click(object sender, EventArgs e)
        {
            if (this.dgvListInvoicesDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvListInvoicesDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvListInvoicesDataGridView)];

                this.dgvListInvoicesDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvSelectedInvoicesDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvListInvoicesDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvListInvoicesDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvSelectedInvoicesDataGridView, this.dgvSelectedInvoicesDataGridView.Rows.Count - 1);

                CreateEmailMessage();
            }
        }

        private void btnRemoveInvoice_Click(object sender, EventArgs e)
        {
            if (this.dgvSelectedInvoicesDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvSelectedInvoicesDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvSelectedInvoicesDataGridView)];

                this.dgvSelectedInvoicesDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvListInvoicesDataGridView.Rows.Add(myDataGridViewRow);
                
                if (this.dgvSelectedInvoicesDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvSelectedInvoicesDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvListInvoicesDataGridView, this.dgvListInvoicesDataGridView.Rows.Count - 1);

                CreateEmailMessage();
            }
        }

        private void dgvListInvoicesDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvSelectedInvoicesDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvListStatementsDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvSelectedStatementsDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnRemoveAllInvoice_Click(object sender, EventArgs e)
        {
        btnRemoveAllInvoice_Click_Continue:

            if (this.dgvSelectedInvoicesDataGridView.Rows.Count > 0)
            {
                btnRemoveInvoice_Click(null, null);

                goto btnRemoveAllInvoice_Click_Continue;
            }
        }

        private void dgvListInvoicesDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnAddInvoice_Click(sender, e);
        }

        private void dgvSelectedInvoicesDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnRemoveInvoice_Click(sender, e);
        }

        private void btnAddStatement_Click(object sender, EventArgs e)
        {
            if (this.dgvListStatementsDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvListStatementsDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvListStatementsDataGridView)];

                this.dgvListStatementsDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvSelectedStatementsDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvListStatementsDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvListStatementsDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvSelectedStatementsDataGridView, this.dgvSelectedStatementsDataGridView.Rows.Count - 1);

                CreateEmailMessage();
            }
        }

        private void btnRemoveStatement_Click(object sender, EventArgs e)
        {
            if (this.dgvSelectedStatementsDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvSelectedStatementsDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvSelectedStatementsDataGridView)];

                this.dgvSelectedStatementsDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvListStatementsDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvSelectedStatementsDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvSelectedStatementsDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvListStatementsDataGridView, this.dgvListStatementsDataGridView.Rows.Count - 1);

                CreateEmailMessage();
            }
        }

        private void btnRemoveAllStatement_Click(object sender, EventArgs e)
        {
        btnRemoveAllStatement_Click_Continue:

            if (this.dgvSelectedStatementsDataGridView.Rows.Count > 0)
            {
                btnRemoveStatement_Click(null, null);

                goto btnRemoveAllStatement_Click_Continue;
            }
        }

        private void dgvListStatementsDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnAddStatement_Click(sender, e);
        }

        private void dgvSelectedStatementsDataGridView_DoubleClick(object sender, EventArgs e)
        {
            btnRemoveStatement_Click(sender, e);
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            try
            {
                string strInvoices = "";
                string strStatements = "";

                //Invoices
                for (int intCheckRow = 0; intCheckRow < this.dgvSelectedInvoicesDataGridView.Rows.Count; intCheckRow++)
                {
                    string strInvoiceNumber = "";

                    if (pvtInvoiceEmailDataView[Convert.ToInt32(this.dgvSelectedInvoicesDataGridView[7, intCheckRow].Value)]["INVOICE_TYPE"].ToString() == "I")
                    {
                        strInvoiceNumber = "Inv" + pvtInvoiceEmailDataView[Convert.ToInt32(this.dgvSelectedInvoicesDataGridView[7, intCheckRow].Value)]["INVOICE_NUMBER"].ToString();
                    }
                    else
                    {
                        strInvoiceNumber = "ProInv" + pvtInvoiceEmailDataView[Convert.ToInt32(this.dgvSelectedInvoicesDataGridView[7, intCheckRow].Value)]["INVOICE_NUMBER"].ToString();
                    }

                    if (intCheckRow == 0)
                    {
                        strInvoices = strInvoiceNumber;
                    }
                    else
                    {
                        strInvoices += "," + strInvoiceNumber;
                    }
                }

                //Statements
                for (int intCheckRow = 0; intCheckRow < this.dgvSelectedStatementsDataGridView.Rows.Count; intCheckRow++)
                {
                    string strStatementNumber = "Stmnt" + pvtStatementEmailDataView[Convert.ToInt32(this.dgvSelectedStatementsDataGridView[5, intCheckRow].Value)]["STATEMENT_NUMBER"].ToString();

                    if (intCheckRow == 0)
                    {
                        strStatements = strStatementNumber;
                    }
                    else
                    {
                        strStatements += "," + strStatementNumber;
                    }
                }

                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt64(this.pvtDataSet.Tables["Company"].Rows[0]["COMPANY_NO"]);
                objParm[1] = strInvoices;
                objParm[2] = strStatements;
                objParm[3] = this.txtEmailMessage.Text;

                int intReturnCode = (int)clsISUtilities.DynamicFunction("Send_Email", objParm);
                
                if (intReturnCode == 0)
                {
                    CustomMessageBox.Show("Email Sent Successfully", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    CustomMessageBox.Show("Email Send Failed. See Server Logs for Details", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            frmConvertInvoice frmConvertInvoice = new frmConvertInvoice();
            frmConvertInvoice.dtDateTimePicker.Text = this.dgvInvoiceDataGridView[0, pvtintInvoiceDataGridViewRowIndex].Value.ToString();
            frmConvertInvoice.txtInvoiceAmount.Text = this.dgvInvoiceDataGridView[7, pvtintInvoiceDataGridViewRowIndex].Value.ToString();

            DialogResult dlgDialogResult = frmConvertInvoice.ShowDialog();


            //DialogResult dlgDialogResult = CustomMessageBox.Show("Are you sure you want to convert this Proforma Invoice to an Invoice?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dlgDialogResult == DialogResult.Cancel)
            {
                return;
            }

            try
            {
                object[] objParm = new object[3];
                objParm[0] = pvtint64CompanyNo;
                objParm[1] = Convert.ToInt32(pvtCompanyInvoiceHistoryDataView[pvtintCompanyInvoiceDataViewRowIndex]["INVOICE_NUMBER"]);
                objParm[2] = AppDomain.CurrentDomain.GetData("NewInvoiceDate").ToString();
                
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Convert_Invoice", objParm);

                //Remove CompanyInvoiceHistory Records
                for (int intRow = 0; intRow < pvtDataSet.Tables["CompanyInvoiceHistory"].Rows.Count; intRow++)
                {
                    pvtDataSet.Tables["CompanyInvoiceHistory"].Rows[intRow].Delete();
                }

                //Remove CompanyInvoiceItemHistory Records
                for (int intRow = 0; intRow < pvtDataSet.Tables["CompanyInvoiceItemHistory"].Rows.Count; intRow++)
                {
                    pvtDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow].Delete();
                }
                
                DataSet DataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                pvtDataSet.Merge(DataSet);

                pvtDataSet.AcceptChanges();
                
                this.Set_DataGridView_SelectedRowIndex(dgvDocumentTypeDataGridView, 0);

            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
        
        private void btnInvoiceSave_Click(object sender, EventArgs e)
        {
            string strInvoiceAction = "";
            string strInvoiceType = "";

            for (int intRow = 0; intRow < this.dgvInvoiceMainteneDataGridView.Rows.Count; intRow++)
            {
                if (this.dgvInvoiceMainteneDataGridView[5, intRow].Value.ToString() != this.dgvInvoiceMainteneDataGridView[6, intRow].Value.ToString())
                {
                    if (strInvoiceAction != "")
                    {
                        strInvoiceAction = strInvoiceAction + ",";
                    }

                    switch (this.dgvInvoiceMainteneDataGridView[5, intRow].Value.ToString())
                    {
                        case "Unpaid":

                            strInvoiceType = "N";
                            break;

                        case "Partially Paid":

                            strInvoiceType = "P";
                            break;

                        case "Paid":

                            strInvoiceType = "Y";
                            break;
                    }

                    strInvoiceAction = strInvoiceAction + this.dgvInvoiceMainteneDataGridView[0, intRow].Value.ToString() + "#" + strInvoiceType;
                }
            }

            if (strInvoiceAction != "")
            {
                DialogResult dlgDialogResult = CustomMessageBox.Show("Are you sure you want to Save the Invoice Changes?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dlgDialogResult == DialogResult.No)
                {
                    return;
                }

                try
                {
                    object[] objParm = new object[2];
                    objParm[0] = pvtint64CompanyNo;
                    objParm[1] = strInvoiceAction;

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Change_InvoiceType", objParm);

                    pvtDataSet.Tables["CompanyStatementInvoiceChoiceCurrent"].Rows.Clear();
                    pvtDataSet.Tables["CompanyInvoiceMaintain"].Rows.Clear();

                    DataSet DataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                    pvtDataSet.Merge(DataSet);

                    pvtDataSet.AcceptChanges();

                    Load_Invoice_Maintenance();
                    
                    this.Set_DataGridView_SelectedRowIndex(dgvInvoiceDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvInvoiceDataGridView));
                }
                catch (Exception eException)
                {
                    clsISUtilities.ErrorHandler(eException);
                }
            }
        }
    }
}
