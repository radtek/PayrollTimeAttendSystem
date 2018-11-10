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
    public partial class frmTimeSheetClient : Form
    {
        clsISClientUtilities clsISClientUtilities;

        ToolStripMenuItem miLinkedMenuItem;

        private int pvtintEmployeeCodeWidth;
        private int pvtintEmployeeSurnameWidth;
        private int pvtintEmployeeNameWidth;
        private int pvtintDayExceptionWidth;
        private int pvtintDayBreakHoursWidth;
        private int pvtintDayPaidHoursWidth;

        private int pvtintEmployeeNoCol = 5;
        private int pvtintPayCategoryNo = -1;
        private int pvtintPayCategoryTableRowNo;
        private int pvtintDateColNo = 8;

        private int pvtintTimeInCol = 2;
        private int pvtintTimeOutCol = 3;
        private int pvtintSpreadSheetAccumTotalCol = 5;
        private int pvtintSpreadSheetSeqNoCol = 6;

        private int pvtintCompanyDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintEmployeeDataGridViewRowIndex = -1;
        private int pvtintDayDataGridViewRowIndex = -1;
        
        //dgvTimeSheetDataGridView and dgvBreakDataGridView Cols
        private int pvtintIndicatorColTimeSheetOrBreakDataGridView = 0;
        private int pvtintClockInColTimeSheetOrBreakDataGridView = 1;
        private int pvtintClockInSetColTimeSheetOrBreakDataGridView = 2;
        private int pvtintClockOutSetColTimeSheetOrBreakDataGridView = 3;
        private int pvtintClockOutColTimeSheetOrBreakDataGridView = 4;
        private int pvtintTotalColTimeSheetOrBreakDataGridView = 5;
        private int pvtintSeqNoColTimeSheetOrBreakDataGridView = 6;
        
        private byte[] pvtbytCompress;
        private string pvtstrPayCategoryFilter = "";
        private string pvtstrCategoryType = "";
        private string pvtstrDataAndTypeFilter = "";

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;

        private DataView pvtTempDataView;
        private DataView pvtPayCategoryDataView;

        private DataView pvtEmployeeDataView;
        private DataView pvtEmployeeOrDateDataView;
        private DataView pvtTimeSheetDataView;
        private DataView pvtBreakDataView;
        private DataView pvtBreakRangeDataView;
        private DataView pvtDayTotalDataView;

        private int pvtintTotalTimeSheetMinutes = 0;
        private int pvtintTotalBreakMinutes = 0;

        private bool pvtblnTimeSheetInError = false;
        private bool pvtblnBreakInError = false;

        private Int64 pvtint64CompanyNo = -1;
        private int pvtintEmployeeNo = -1;
        private string pvtstrPayrollType = "";
        private DateTime pvtDateTime;
        private DateTime pvtdtFilterEndDate;
        private DateTime pvtdtPayCategoryWageEndDate;

        DataGridViewCellStyle ErrorDataGridViewCellStyle;
        DataGridViewCellStyle ExceptionDataGridViewCellStyle;
        DataGridViewCellStyle NoRecordDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;
        DataGridViewCellStyle BreakExceptionDataGridViewCellStyle;
        DataGridViewCellStyle WeekEndDataGridViewCellStyle;
        DataGridViewCellStyle LunchTotalDataGridViewCellStyle;
        DataGridViewCellStyle TotalDataGridViewCellStyle;
        
        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnCompanyDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;
        private bool pvtblnDayDataGridViewLoaded = false;
        private bool pvtblnBreakRangeDataGridViewLoaded = false;
        private bool pvtblnTimeSheetDataGridViewLoaded = false;
        private bool pvtblnBreakDataGridViewLoaded = false;

        private bool pvtblnAllowToEnter = true;

        public frmTimeSheetClient()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 98;

                this.dgvTimeSheetDataGridView.Height += 95;

                this.lblBreakRange.Top += 95;
                this.dgvBreakRangeDataGridView.Top += 95;
                this.dgvBreakExceptionDataGridView.Top += 95;
                this.dgvTimeSheetTotalsDataGridView.Top += 95;
                this.lblDeleteTimeSheetRowDesc.Top += 95;
                this.btnDeleteTimeSheetRow.Top += 95;

                this.dgvBreakDataGridView.Height += 95;
                this.dgvBreakTotalsDataGridView.Top += 95;

                this.grbBreakError.Top += 95;
                this.lblDeleteBreakRowDesc.Top += 95;
                this.btnDeleteBreakRow.Top += 95;
                this.btnRefresh.Top += 95;

                this.dgvPayCategoryDataGridView.Height += 38;

                int intEmployeeTop = 38;

                this.lblEmployee.Top += intEmployeeTop;
                this.picEmployeeLock.Top += intEmployeeTop;
                this.dgvEmployeeDataGridView.Top += intEmployeeTop;
                this.dgvEmployeeDataGridView.Height += 57;

                this.lblDate.Top += 95;
                this.dgvDayDataGridView.Top += 95;
            }

            //2014-11-29 (Remove Delete of all TimeSheets for Cost Centre) 
            if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() != "S")
            {
                this.btnDelete.Visible = false;

                this.btnClose.Top = this.btnCancel.Top;
                this.btnCancel.Top = this.btnSave.Top;
                this.btnSave.Top = this.btnDelete.Top;
            }
        }

        private void frmTimeSheetClient_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISClientUtilities = new clsISClientUtilities(this, "busTimeSheetClient");

                this.lblCompany.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblDate.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblDayDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                this.lblTimesheets.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblBreaks.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                this.lblBreakBlank.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblBreakStart.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblBreakStop.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblBreakAccum.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                //this.lblBreakRange.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                this.lblTimeSheetBlank.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblTimeSheetStart.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblTimeSheetStop.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblTimeSheetAccum.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

                pvtdtFilterEndDate = DateTime.Now;

                this.dgvBreakExceptionDataGridView.Rows.Add("");

                this.dgvTimeSheetTotalsDataGridView.Rows.Add("Total Work Hours");
                this.dgvTimeSheetTotalsDataGridView.Rows.Add("Break After 0:00");
                this.dgvTimeSheetTotalsDataGridView.Rows.Add("Total Paid Hours");
                this.dgvTimeSheetTotalsDataGridView.Rows.Add("");

                this.dgvBreakTotalsDataGridView.Rows.Add("",
                                                         "Total Break Hours",
                                                         "0:00");

                this.dgvPayCategoryDataGridView.Columns[1].HeaderText = "B";
                this.dgvEmployeeDataGridView.Columns[1].HeaderText = "B";
                this.dgvDayDataGridView.Columns[1].HeaderText = "B";

                pvtintEmployeeCodeWidth = this.dgvDayDataGridView.Columns[2].Width;
                pvtintEmployeeSurnameWidth = this.dgvDayDataGridView.Columns[3].Width;
                pvtintEmployeeNameWidth = this.dgvDayDataGridView.Columns[4].Width;
                pvtintDayExceptionWidth = this.dgvDayDataGridView.Columns[5].Width;
                pvtintDayPaidHoursWidth = this.dgvDayDataGridView.Columns[6].Width;
                pvtintDayBreakHoursWidth = this.dgvDayDataGridView.Columns[7].Width;

                ErrorDataGridViewCellStyle = new DataGridViewCellStyle();
                ErrorDataGridViewCellStyle.BackColor = Color.Red;
                ErrorDataGridViewCellStyle.SelectionBackColor = Color.Red;

                ExceptionDataGridViewCellStyle = new DataGridViewCellStyle();
                ExceptionDataGridViewCellStyle.BackColor = Color.Lime;
                ExceptionDataGridViewCellStyle.SelectionBackColor = Color.Lime;

                NoRecordDataGridViewCellStyle = new DataGridViewCellStyle();
                NoRecordDataGridViewCellStyle.BackColor = Color.Yellow;
                NoRecordDataGridViewCellStyle.SelectionBackColor = Color.Yellow;

                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;

                BreakExceptionDataGridViewCellStyle = new DataGridViewCellStyle();
                BreakExceptionDataGridViewCellStyle.BackColor = Color.Teal;
                BreakExceptionDataGridViewCellStyle.SelectionBackColor = Color.Teal;

                WeekEndDataGridViewCellStyle = new DataGridViewCellStyle();
                WeekEndDataGridViewCellStyle.BackColor = SystemColors.Info;

                LunchTotalDataGridViewCellStyle = new DataGridViewCellStyle();
                LunchTotalDataGridViewCellStyle.BackColor = Color.Silver;

                TotalDataGridViewCellStyle = new DataGridViewCellStyle();
                TotalDataGridViewCellStyle.BackColor = SystemColors.ControlDarkDark;
                TotalDataGridViewCellStyle.SelectionBackColor = SystemColors.ControlDarkDark;

                this.dgvBreakTotalsDataGridView[2, 0].Style = this.TotalDataGridViewCellStyle;
                this.dgvTimeSheetTotalsDataGridView[1, 3].Style = this.TotalDataGridViewCellStyle;

                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Form_Records", objParm,false);
                pvtDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                if (pvtDataSet.Tables["Company"].Rows.Count > 0)
                {
                    for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                    {
                        this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString());
                    }

                    pvtblnPayrollTypeDataGridViewLoaded = true;

                    if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
                    {
                        Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);
                    }

                    int intComboSelectIndex = -1;

                    DataTable myDataTable = new DataTable("DateSelection");
                    myDataTable.Columns.Add("ACTUAL_DATE", typeof(String));

                    DateTime myStartDateTime = DateTime.Now.AddDays(-15);

                    while (myStartDateTime <= DateTime.Now.AddDays(15))
                    {
                        DataRow MyDataRow = myDataTable.NewRow();

                        MyDataRow["ACTUAL_DATE"] = myStartDateTime.ToString("yyyy-MM-dd");

                        myDataTable.Rows.Add(MyDataRow);

                        this.cboDateFilter.Items.Add(myStartDateTime.ToString("dd MMMM yyyy - dddd"));

                        if (myStartDateTime.ToString("yyyyMMdd") == pvtdtFilterEndDate.ToString("yyyyMMdd"))
                        {
                            intComboSelectIndex = this.cboDateFilter.Items.Count - 1;
                        }

                        myStartDateTime = myStartDateTime.AddDays(1);
                    }

                    pvtDataSet.Tables.Add(myDataTable);

                    pvtDataSet.AcceptChanges();

                    this.cboDateFilter.SelectedIndex = intComboSelectIndex;

                    for (int intRow = 0; intRow < pvtDataSet.Tables["Company"].Rows.Count; intRow++)
                    {
                        this.dgvCompanyDataGridView.Rows.Add(pvtDataSet.Tables["Company"].Rows[intRow]["COMPANY_DESC"].ToString(),
                                                            intRow.ToString());
                    }

                    pvtblnCompanyDataGridViewLoaded = true;

                    if (dgvCompanyDataGridView.Rows.Count > 0)
                    {
                        Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, 0);
                    }
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
            switch (myDataGridView.Name)
            {
                case "dgvPayrollTypeDataGridView":

                    pvtintPayrollTypeDataGridViewRowIndex = -1;
                    break;

                case "dgvCompanyDataGridView":

                    pvtintCompanyDataGridViewRowIndex = -1;
                    break;

                case "dgvPayCategoryDataGridView":

                    pvtintPayCategoryDataGridViewRowIndex = -1;
                    break;

                case "dgvDayDataGridView":

                    pvtintDayDataGridViewRowIndex = -1;
                    break;

                case "dgvEmployeeDataGridView":

                    pvtintEmployeeDataGridViewRowIndex = -1;
                    break;

                
                default:

                    break;
            }

            //Fires DataGridView RowEnter Function
            if (this.Get_DataGridView_SelectedRowIndex(myDataGridView) == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvPayrollTypeDataGridView":

                        dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvCompanyDataGridView":

                        dgvCompanyDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        dgvPayCategoryDataGridView_RowEnter(myDataGridView, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDayDataGridView":

                        dgvDayDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeDataGridView":

                        dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvBreakRangeDataGridView":

                        dgvBreakRangeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvTimeSheetDataGridView":

                        dgvTimeSheetDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvBreakDataGridView":

                        dgvBreakDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        System.Windows.Forms.MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                if (myDataGridView.Name == "dgvTimeSheetDataGridView"
                    | myDataGridView.Name == "dgvBreakDataGridView"
                    | myDataGridView.Name == "dgvEmployeeDataGridView"
                    | myDataGridView.Name == "dgvDayDataGridView")
                {
                    myDataGridView.CurrentCell = myDataGridView[1, intRow];

                    DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                    switch (myDataGridView.Name)
                    {
                        case "dgvEmployeeDataGridView":

                            dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                            break;

                        case "dgvDayDataGridView":

                            dgvDayDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                            break;
                    }
                }
                else
                {
                    myDataGridView.CurrentCell = myDataGridView[0, intRow];
                }
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

        private void Load_PayCategory_Records()
        {
            if (this.dgvCompanyDataGridView.Rows.Count > 0
                & pvtblnCompanyDataGridViewLoaded == true)
            {
                this.Cursor = Cursors.WaitCursor;

                this.lblDayDesc.Text = "";

                //Clear Totals
                this.dgvTimeSheetTotalsDataGridView[1, 0].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 2].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 3].Value = "0.00";

                this.dgvTimeSheetTotalsDataGridView[0,1].Value = "Break After 0:00";

                int intPayCategoryRow = 0;

                Clear_DataGridView(dgvPayCategoryDataGridView);
                Clear_DataGridView(dgvEmployeeDataGridView);
                Clear_DataGridView(dgvDayDataGridView);
                Clear_DataGridView(dgvTimeSheetDataGridView);
                Clear_DataGridView(dgvBreakRangeDataGridView);
                Clear_DataGridView(dgvBreakDataGridView);

                dgvBreakTotalsDataGridView[0, 0].Style = LunchTotalDataGridViewCellStyle;

                this.dgvBreakTotalsDataGridView[0, 0].Value = "";
                this.dgvBreakTotalsDataGridView[2,0].Value = "0:00";

                this.dgvBreakExceptionDataGridView[0, 0].Value = "";

                this.grbBreakError.Visible = false;

                this.btnDelete.Enabled = false;
                this.btnUpdate.Enabled = false;
                this.btnRefresh.Enabled = false;

                pvtstrDataAndTypeFilter = " AND DAY_DATE <= '" + pvtdtFilterEndDate.ToString("yyyy-MM-dd") + "'";

                if (this.chkRemoveSat.Checked == true
                | this.chkRemoveSun.Checked == true)
                {
                    pvtstrDataAndTypeFilter += " AND NOT DAY_NO IN (";

                    if (this.chkRemoveSat.Checked == true)
                    {
                        pvtstrDataAndTypeFilter += "6";
                    }

                    if (this.chkRemoveSun.Checked == true)
                    {
                        if (this.chkRemoveSat.Checked == true)
                        {
                            pvtstrDataAndTypeFilter += ",0";

                        }
                        else
                        {
                            pvtstrDataAndTypeFilter += "0";
                        }
                    }

                    pvtstrDataAndTypeFilter += ")";
                }

                if (this.rbnErrors.Checked == true)
                {
                    pvtstrCategoryType = " AND INDICATOR = 'X'";
                }
                else
                {
                    if (this.rbnException.Checked == true)
                    {
                        pvtstrCategoryType = " AND INDICATOR = 'E'";
                    }
                    else
                    {
                        if (this.rbnNormal.Checked == true)
                        {
                            pvtstrCategoryType = " AND INDICATOR = ''";
                        }
                        else
                        {
                            if (this.rbnBreakException.Checked == true)
                            {
                                pvtstrCategoryType = " AND BREAK_INDICATOR = 'Y'";
                            }
                            else
                            {
                                pvtstrCategoryType = "";
                            }
                        }
                    }
                }

                string strWhere = "COMPANY_NO = " + pvtint64CompanyNo;

                if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
                {
                    strWhere += " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'";
                }
                else
                {
                    strWhere += " AND PAY_CATEGORY_TYPE = ''";
                }

                pvtPayCategoryDataView = null;
                pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                    strWhere,
                    "PAY_CATEGORY_DESC",
                    DataViewRowState.CurrentRows);

                string strDate = "";
                string strDateCompare = "";

                this.pvtblnPayCategoryDataGridViewLoaded = false;

                this.dgvPayCategoryDataGridView.Rows.Clear();
                
                for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
                {
                    strDate = "";
                    strDateCompare = "";

                    DataView DataView = new System.Data.DataView(pvtDataSet.Tables["DayTotal"],
                                       "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"] + pvtstrDataAndTypeFilter + pvtstrCategoryType,
                                        "INDICATOR DESC",
                                        DataViewRowState.CurrentRows);

                    //NB Blank Cannot be Checked - it Wont Exist
                    if ((this.rbnErrors.Checked == true
                    | this.rbnException.Checked == true
                    | this.rbnNormal.Checked == true
                    | this.rbnBreakException.Checked == true)
                    & DataView.Count == 0)
                    {
                        continue;
                    }
                   
                    if (pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"] != System.DBNull.Value)
                    {
                        strDate = Convert.ToDateTime(pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("dd MMM yyyy   -   HH:mm");

                        strDateCompare = Convert.ToDateTime(pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("yyyyMMddHHmm");
                    }

                    this.dgvPayCategoryDataGridView.Rows.Add("",
                                                             "", 
                                                             pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                             strDate,
                                                             intRow.ToString()
                                                             ,strDateCompare);

                    if (this.rbnBlank.Checked == true)
                    {
                        this.dgvPayCategoryDataGridView[0,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = NoRecordDataGridViewCellStyle;
                    }
                    else
                    {
                        if (DataView.Count > 0)
                        {
                            if (DataView[0]["INDICATOR"].ToString() == "X")
                            {
                                this.dgvPayCategoryDataGridView[0, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;
                            }
                            else
                            {
                                if (DataView[0]["INDICATOR"].ToString() == "E")
                                {
                                    this.dgvPayCategoryDataGridView[0, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = ExceptionDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (DataView[0]["INDICATOR"].ToString() == "B")
                                    {
                                        this.dgvPayCategoryDataGridView[0, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = NoRecordDataGridViewCellStyle;
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.dgvPayCategoryDataGridView[0, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = NoRecordDataGridViewCellStyle;
                        }
                    }

                    DataView.Sort = "BREAK_INDICATOR DESC";

                    if (this.rbnBlank.Checked == false)
                    {
                        if (DataView.Count > 0)
                        {
                            if (DataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                            {
                                dgvPayCategoryDataGridView[1, this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = BreakExceptionDataGridViewCellStyle;
                            }
                        }
                    }

                    if (Convert.ToInt32(pvtPayCategoryDataView[intRow]["PAY_CATEGORY_NO"]) == this.pvtintPayCategoryNo)
                    {
                        intPayCategoryRow = this.dgvPayCategoryDataGridView.Rows.Count - 1;
                    }
                }

                this.pvtblnPayCategoryDataGridViewLoaded = true;
                
                if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, intPayCategoryRow);
                }

                this.Cursor = Cursors.Default;
            }
        }

        private void Load_Employee_SpreadSheet()
        {
            try
            {
                this.pvtblnEmployeeDataGridViewLoaded = false;
             
                Clear_DataGridView(dgvEmployeeDataGridView);
                Clear_DataGridView(dgvDayDataGridView);
                Clear_DataGridView(dgvTimeSheetDataGridView);
                Clear_DataGridView(dgvBreakDataGridView);

                //Set To Choose First Row In Spreadsheet
                int intEmployeeNoRow = 0;

                this.lblDayDesc.Text = "";

                if (this.rbnEmployeeDate.Checked == true)
                {
                    pvtEmployeeOrDateDataView = null;
                    pvtEmployeeOrDateDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                        pvtstrPayCategoryFilter + " AND EMPLOYEE_LAST_RUNDATE < '" + pvtdtFilterEndDate.ToString("yyyy-MM-dd") + "'",
                        "",
                        DataViewRowState.CurrentRows);
                }
                else
                {
                    pvtEmployeeOrDateDataView = null;
                    pvtEmployeeOrDateDataView = new DataView(this.pvtDataSet.Tables["Dates"],
                        pvtstrPayCategoryFilter + pvtstrDataAndTypeFilter,
                        "DAY_DATE DESC",
                        DataViewRowState.CurrentRows);
                }

                //Set Error
                for (int intRow = 0; intRow < pvtEmployeeOrDateDataView.Count; intRow++)
                {
                    pvtTempDataView = null;

                    if (rbnEmployeeDate.Checked == true)
                    {
                        pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                            pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_NO"].ToString() + " " + pvtstrDataAndTypeFilter + pvtstrCategoryType,
                            "INDICATOR DESC",
                            DataViewRowState.CurrentRows);
                    }
                    else
                    {
                        pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                            pvtstrPayCategoryFilter + " AND DAY_DATE = '" + Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]).ToString("yyyy-MM-dd") + "' " + pvtstrDataAndTypeFilter + pvtstrCategoryType,
                            "INDICATOR DESC",
                               DataViewRowState.CurrentRows);
                    }

                    if (((this.rbnErrors.Checked == true
                    | this.rbnException.Checked == true
                    | this.rbnNormal.Checked == true
                    | this.rbnBreakException.Checked == true
                    | this.chkRemoveBlanks.Checked == true)
                    & pvtTempDataView.Count == 0))
                    {
                        continue;
                    }
                    //else
                    //{
                    //    if (this.rbnBlank.Checked == true
                    //         & pvtTempDataView.Count > 0)
                    //    {
                    //        continue;
                    //    }
                    //}

                    if (rbnEmployeeDate.Checked == true)
                    {
                        this.dgvEmployeeDataGridView.Rows.Add("",
                                                              "",
                                                              pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                              pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                              pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                              intRow.ToString());

                        if (pvtintEmployeeNo == Convert.ToInt32(pvtEmployeeOrDateDataView[intRow]["EMPLOYEE_NO"]))
                        {
                            intEmployeeNoRow = dgvEmployeeDataGridView.Rows.Count - 1;
                        }
                    }
                    else
                    {
                         this.dgvEmployeeDataGridView.Rows.Add("",
                                                               "",
                                                               Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]).ToString("dd MMMM yyyy - dddd"),
                                                               Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]),
                                                               "",
                                                               intRow.ToString());

                        if (Convert.ToInt32(Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]).DayOfWeek) == 6
                            | Convert.ToInt32(Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]).DayOfWeek) == 0)
                        {
                            this.dgvEmployeeDataGridView.Rows[dgvEmployeeDataGridView.Rows.Count - 1].DefaultCellStyle = WeekEndDataGridViewCellStyle;

                            this.dgvEmployeeDataGridView[1, dgvEmployeeDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                        }

                         if (pvtDateTime == Convert.ToDateTime(pvtEmployeeOrDateDataView[intRow]["DAY_DATE"]))
                        {
                            intEmployeeNoRow = dgvEmployeeDataGridView.Rows.Count - 1;
                        }
                    }

                    if (this.rbnBlank.Checked == true
                        | pvtTempDataView.Count == 0)
                    {
                        this.dgvEmployeeDataGridView[0,dgvEmployeeDataGridView.Rows.Count - 1].Style = NoRecordDataGridViewCellStyle;
                    }
                    else
                    {
                        if (pvtTempDataView[0]["INDICATOR"].ToString() == "X")
                        {
                            this.dgvEmployeeDataGridView[0, dgvEmployeeDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;
                        }
                        else
                        {
                            if (pvtTempDataView[0]["INDICATOR"].ToString() == "E")
                            {
                                this.dgvEmployeeDataGridView[0, dgvEmployeeDataGridView.Rows.Count - 1].Style = ExceptionDataGridViewCellStyle;
                            }
                            else
                            {
                                if (pvtTempDataView[0]["INDICATOR"].ToString() == "B")
                                {
                                    this.dgvEmployeeDataGridView[0, dgvEmployeeDataGridView.Rows.Count - 1].Style = NoRecordDataGridViewCellStyle;
                                }
                            }
                        }

                        pvtTempDataView.Sort = "BREAK_INDICATOR DESC";
                    }

                    if (this.rbnBlank.Checked == false
                    & pvtTempDataView.Count > 0)
                    {
                        if (pvtTempDataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                        {
                            dgvEmployeeDataGridView[1, this.dgvEmployeeDataGridView.Rows.Count - 1].Style = BreakExceptionDataGridViewCellStyle;
                        }
                    }
                }

                this.pvtblnEmployeeDataGridViewLoaded = true;

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["NO_EDIT_IND"].ToString() == "Y")
                    {
#if (DEBUG)
                        this.btnUpdate.Enabled = true;
                        this.btnDelete.Enabled = true;
#else
                        this.btnUpdate.Enabled = false;
                        this.btnDelete.Enabled = false;
#endif
                        this.grbEmployeeLock.Visible = true;
                    }
                    else
                    {
                        this.btnUpdate.Enabled = true;
                        this.btnDelete.Enabled = true;
                    }

                    this.btnRefresh.Enabled = true;

                    this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, intEmployeeNoRow);
                }
                else
                {
                    //Clear Totals
                    this.dgvTimeSheetTotalsDataGridView[1, 0].Value = "0.00";
                    this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "0.00";
                    this.dgvTimeSheetTotalsDataGridView[1, 2].Value = "0.00";
                    this.dgvTimeSheetTotalsDataGridView[1, 3].Value = "0.00";

                    this.btnDelete.Enabled = false;
                    this.btnUpdate.Enabled = false;
                    this.btnRefresh.Enabled = false;
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

        private void Load_Day_SpreadSheet()
        {
            pvtblnDayDataGridViewLoaded = false;
           
            Clear_DataGridView(dgvDayDataGridView);
            Clear_DataGridView(dgvTimeSheetDataGridView);
            Clear_DataGridView(dgvBreakDataGridView);

            //Set To Choose First Row In Spreadsheet
            int intSelectedDayRow = 0;
            int intBelowHH = 0;
            int intBelowMM = 0;
            int intAboveHH = 0;
            int intAboveMM = 0;

            string strException = "";
            string strDayPaidMinutes = "";
            string strBreakPaidMinutes = "";

            //Load Employees
            int intEmployeeNo = -1;

            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
            {
                if (this.rbnEmployeeDate.Checked == true)
                {
                    pvtTempDataView = null;
                    pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                            pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DAY_DATE = '" + Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("yyyy-MM-dd") + "'" + pvtstrDataAndTypeFilter + pvtstrCategoryType,
                            "INDICATOR DESC",
                            DataViewRowState.CurrentRows);
                }
                else
                {
                    intEmployeeNo = Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"]);

                    pvtTempDataView = null;
                    pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                            pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + intEmployeeNo + " AND DAY_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'" + pvtstrDataAndTypeFilter + pvtstrCategoryType,
                            "INDICATOR DESC",
                            DataViewRowState.CurrentRows);
                }

                if (((this.rbnErrors.Checked == true
                | this.rbnException.Checked == true
                | this.rbnNormal.Checked == true
                | this.rbnBreakException.Checked == true
                | this.chkRemoveBlanks.Checked == true)
                & pvtTempDataView.Count == 0)
                | (this.rbnBlank.Checked == true
                & pvtTempDataView.Count > 0))
                {
                    continue;
                }

                if (this.rbnEmployeeDate.Checked == true)
                {
                    if (Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) == 0)
                    {
                        strException = ">= 0";
                    }
                    else
                    {
                        intBelowHH = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) / 60;
                        intBelowMM = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) % 60; ;
                        intAboveHH = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_ABOVE_MINUTES"]) / 60;
                        intAboveMM = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("ddd").ToUpper() + "_ABOVE_MINUTES"]) % 60; ;

                        strException = intBelowHH.ToString() + ":" + intBelowMM.ToString("00") + " - " + intAboveHH.ToString() + ":" + intAboveMM.ToString("00");
                    }

                    if (this.rbnBlank.Checked == true
                    | pvtTempDataView.Count == 0)
                    {
                        strDayPaidMinutes = "0.00";
                        strBreakPaidMinutes = "0.00";
                    }
                    else
                    {
                        strDayPaidMinutes = Convert.ToInt32(Convert.ToInt32(pvtTempDataView[0]["DAY_PAID_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtTempDataView[0]["DAY_PAID_MINUTES"]) % 60).ToString("00");
                        strBreakPaidMinutes = Convert.ToInt32(Convert.ToInt32(pvtTempDataView[0]["BREAK_ACCUM_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtTempDataView[0]["BREAK_ACCUM_MINUTES"]) % 60).ToString("00");

                    }

                    this.dgvDayDataGridView.Rows.Add("",
                                                     "",
                                                     Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("dd MMMM yyyy - dddd"),
                                                     strException,
                                                     strDayPaidMinutes,
                                                     strBreakPaidMinutes,
                                                     "",
                                                     "",
                                                     Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]).ToString("yyyy-MM-dd"));

                    //Weekend
                    if (Convert.ToInt32(pvtEmployeeDataView[intRow]["DAY_NO"]) == 6
                        | Convert.ToInt32(pvtEmployeeDataView[intRow]["DAY_NO"]) == 0)
                    {
                        this.dgvDayDataGridView.Rows[dgvDayDataGridView.Rows.Count - 1].DefaultCellStyle = WeekEndDataGridViewCellStyle;

                        //this.dgvDayDataGridView[1,dgvDayDataGridView.Rows.Count - 1].Style = NormalDataGridViewCellStyle;
                    }

                    if (Convert.ToDateTime(pvtEmployeeDataView[intRow]["DAY_DATE"]) == pvtDateTime)
                    {
                        intSelectedDayRow = dgvDayDataGridView.Rows.Count - 1;
                    }
                }
                else
                {
                    if (Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) == 0)
                    {
                        strException = ">= 0";
                    }
                    else
                    {
                        intBelowHH = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) / 60;
                        intBelowMM = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) % 60; ;
                        intAboveHH = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_ABOVE_MINUTES"]) / 60;
                        intAboveMM = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_ABOVE_MINUTES"]) % 60; ;

                        strException = intBelowHH.ToString() + ":" + intBelowMM.ToString("00") + " - " + intAboveHH.ToString() + ":" + intAboveMM.ToString("00");
                    }

                    if (this.rbnBlank.Checked == true
                    | pvtTempDataView.Count == 0)
                    {
                        strDayPaidMinutes = "0.00";
                        strBreakPaidMinutes = "0.00";
                    }
                    else
                    {
                        strDayPaidMinutes = Convert.ToInt32(Convert.ToInt32(pvtTempDataView[0]["DAY_PAID_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtTempDataView[0]["DAY_PAID_MINUTES"]) % 60).ToString("00");
                        strBreakPaidMinutes = Convert.ToInt32(Convert.ToInt32(pvtTempDataView[0]["BREAK_ACCUM_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtTempDataView[0]["BREAK_ACCUM_MINUTES"]) % 60).ToString("00");
                    }

                    this.dgvDayDataGridView.Rows.Add("",
                                                     "",
                                                     pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                     pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                     pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                     strException,
                                                     strDayPaidMinutes,
                                                     strBreakPaidMinutes,
                                                     intRow.ToString());

                    if (intEmployeeNo == pvtintEmployeeNo)
                    {
                        intSelectedDayRow = this.dgvDayDataGridView.Rows.Count - 1;
                    }
                }

                if (this.rbnBlank.Checked == true
                    | pvtTempDataView.Count == 0)
                {
                    this.dgvDayDataGridView[0,dgvDayDataGridView.Rows.Count - 1].Style = NoRecordDataGridViewCellStyle;
                }
                else
                {
                
                    if (pvtTempDataView[0]["INDICATOR"].ToString() == "X")
                    {
                        this.dgvDayDataGridView[0, dgvDayDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;
                    }
                    else
                    {
                        if (pvtTempDataView[0]["INDICATOR"].ToString() == "E")
                        {
                            this.dgvDayDataGridView[0, dgvDayDataGridView.Rows.Count - 1].Style = ExceptionDataGridViewCellStyle;
                        }
                    }

                    pvtTempDataView.Sort = "BREAK_INDICATOR DESC";
                }

                if (this.rbnBlank.Checked == false
                & pvtTempDataView.Count > 0)
                {
                    if (pvtTempDataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                    {
                        this.dgvDayDataGridView[1, dgvDayDataGridView.Rows.Count - 1].Style = BreakExceptionDataGridViewCellStyle;
                    }
                }
            }

            pvtblnDayDataGridViewLoaded = true;

            if (dgvDayDataGridView.Rows.Count > 0)
            {
                //2013-03-07 btnUpdate.Enabled is Set at Cost Centre Level based on Whether there is a Dynamic Upload
                this.Set_DataGridView_SelectedRowIndex(dgvDayDataGridView, intSelectedDayRow);
            }
            else
            {
                //Clear Totals
                this.dgvTimeSheetTotalsDataGridView[1, 0].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 2].Value = "0.00";
                this.dgvTimeSheetTotalsDataGridView[1, 3].Value = "0.00";

                this.lblDayDesc.Text = "";

                this.btnUpdate.Enabled = false;
                this.btnRefresh.Enabled = false;
            }
        }

        private void Check_To_Add_New_TimeSheet_Row()
        {
            if (this.dgvTimeSheetDataGridView.Rows.Count > 0)
            {
                if (this.dgvTimeSheetDataGridView[pvtintTimeInCol, this.dgvTimeSheetDataGridView.Rows.Count - 1].Value.ToString() == ""
                    & this.dgvTimeSheetDataGridView[pvtintTimeOutCol, this.dgvTimeSheetDataGridView.Rows.Count - 1].Value.ToString() == "")
                {
                    dgvTimeSheetDataGridView.CurrentCell = dgvTimeSheetDataGridView[2, this.dgvTimeSheetDataGridView.Rows.Count - 1];

                    return;
                }
            }

            DataView TempDataView = new DataView(this.pvtDataSet.Tables["TimeSheet"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND TIMESHEET_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "' AND INDICATOR = 'X'",
                "TIMESHEET_SEQ DESC",
                DataViewRowState.CurrentRows);

            if (TempDataView.Count == 0)
            {
                DataRowView drvDataRowView = this.pvtTimeSheetDataView.AddNew();

                drvDataRowView.BeginEdit();

                drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                //NB This Will Be Changed When A Time-In/Time-Out Is Added
                drvDataRowView["PAY_CATEGORY_NO"] = pvtintPayCategoryNo;
                drvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                drvDataRowView["EMPLOYEE_NO"] = pvtintEmployeeNo;
                drvDataRowView["TIMESHEET_DATE"] = pvtDateTime;
                drvDataRowView["TIMESHEET_TIME_IN_MINUTES"] = System.DBNull.Value;
                drvDataRowView["TIMESHEET_TIME_OUT_MINUTES"] = System.DBNull.Value;
                drvDataRowView["TIMESHEET_ACCUM_MINUTES"] = 0;

                //Remove Filter
                TempDataView.RowFilter = TempDataView.RowFilter.Replace(" AND INDICATOR = 'X'", "");
                TempDataView.RowStateFilter = TempDataView.RowStateFilter | DataViewRowState.Deleted;

                if (TempDataView.Count == 0)
                {
                    drvDataRowView["TIMESHEET_SEQ"] = 1;
                }
                else
                {
                    drvDataRowView["TIMESHEET_SEQ"] = Convert.ToInt32(TempDataView[0]["TIMESHEET_SEQ"]) + 1;
                }

                TempDataView = null;

                this.pvtblnTimeSheetDataGridViewLoaded = false;

                this.dgvTimeSheetDataGridView.Rows.Add("",
                                                       "",
                                                       "",
                                                       "",
                                                       "",
                                                       "0:00",
                                                       drvDataRowView["TIMESHEET_SEQ"].ToString());

                drvDataRowView.EndEdit();

                this.pvtblnTimeSheetDataGridViewLoaded = true;

                dgvTimeSheetDataGridView.CurrentCell = dgvTimeSheetDataGridView[2,this.dgvTimeSheetDataGridView.Rows.Count - 1];

                SendKeys.Send("{Left}");
            }
            else
            {
                dgvTimeSheetDataGridView.CurrentCell = dgvTimeSheetDataGridView[2, this.dgvTimeSheetDataGridView.Rows.Count - 1];
            }
        }

        private void Check_To_Add_New_Break_Row()
        {
            if (this.dgvBreakDataGridView.Rows.Count > 0)
            {
                if (this.dgvBreakDataGridView[pvtintTimeInCol, this.dgvBreakDataGridView.Rows.Count - 1].Value.ToString() == ""
                    & this.dgvBreakDataGridView[pvtintTimeOutCol, this.dgvBreakDataGridView.Rows.Count - 1].Value.ToString() == "")
                {
                    dgvBreakDataGridView.CurrentCell = dgvBreakDataGridView[2, this.dgvBreakDataGridView.Rows.Count - 1];

                    return;
                }
            }

            DataView myDataView = new DataView(this.pvtDataSet.Tables["Break"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND BREAK_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "' AND INDICATOR = 'X'",
                "BREAK_SEQ DESC",
                DataViewRowState.CurrentRows);

            if (myDataView.Count == 0)
            {
                DataRowView drvDataRowView = myDataView.AddNew();

                drvDataRowView.BeginEdit();

                drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
                //NB This Will Be Changed When A Time-In/Time-Out Is Added
                drvDataRowView["PAY_CATEGORY_NO"] = pvtintPayCategoryNo;
                drvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                drvDataRowView["EMPLOYEE_NO"] = pvtintEmployeeNo;
                drvDataRowView["BREAK_DATE"] = pvtDateTime;
                drvDataRowView["BREAK_TIME_IN_MINUTES"] = System.DBNull.Value;
                drvDataRowView["BREAK_TIME_OUT_MINUTES"] = System.DBNull.Value;
                drvDataRowView["BREAK_ACCUM_MINUTES"] = 0;

                int intBreakSeq = 1;

                //Remove Filter
                myDataView.RowFilter = myDataView.RowFilter.Replace(" AND INDICATOR = 'X'", "");
                myDataView.RowStateFilter = myDataView.RowStateFilter | DataViewRowState.Deleted;

                if (myDataView.Count == 1)
                {
                    drvDataRowView["BREAK_SEQ"] = intBreakSeq;
                }
                else
                {
                    intBreakSeq = Convert.ToInt32(myDataView[0]["BREAK_SEQ"]) + 1;
                    drvDataRowView["BREAK_SEQ"] = intBreakSeq;
                }

                this.pvtblnBreakDataGridViewLoaded = false;

                this.dgvBreakDataGridView.Rows.Add("",
                                                   "",
                                                   "",
                                                   "",
                                                   "",
                                                   "0:00",
                                                   drvDataRowView["BREAK_SEQ"].ToString());

                drvDataRowView.EndEdit();

                this.pvtblnBreakDataGridViewLoaded = true;
                dgvBreakDataGridView.CurrentCell = dgvBreakDataGridView[2, this.dgvBreakDataGridView.Rows.Count - 1];

                SendKeys.Send("{Left}");
            }
            else
            {
                dgvBreakDataGridView.CurrentCell = dgvBreakDataGridView[2, this.dgvBreakDataGridView.Rows.Count - 1];
            }
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            this.dgvTimeSheetDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvTimeSheetDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;
            this.dgvBreakDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvBreakDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

            this.btnDeleteTimeSheetRow.Enabled = true;
            this.btnDeleteBreakRow.Enabled = true;

            this.dgvCompanyDataGridView.Enabled = false;
            this.dgvPayCategoryDataGridView.Enabled = false;
            this.dgvEmployeeDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;

            Show_Update_Lock_Images();

            this.chkRemoveBlanks.Enabled = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Enabled = false;

            this.btnRefresh.Enabled = false;

            this.cboDateFilter.Enabled = false;

            this.btnToday.Enabled = false;
            this.btnDatePrevious.Enabled = false;
            this.btnDateNext.Enabled = false;

            this.rbnErrors.Enabled = false;
            this.rbnException.Enabled = false;
            this.rbnNone.Enabled = false;
            this.rbnBlank.Enabled = false;
            this.rbnNormal.Enabled = false;

            this.rbnBreakException.Enabled = false;

            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.rbnDateEmployee.Enabled = false;
            this.rbnEmployeeDate.Enabled = false;

            if (this.dgvDayDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView));
            }
        }

        private void Show_Update_Lock_Images()
        {
            this.picCompanyLock.Visible = true;
            this.picPayrollLock.Visible = true;
            this.picPayCategoryLock.Visible = true;
            this.picEmployeeLock.Visible = true;
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (this.Text.LastIndexOf("- Update") != -1)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);
            }

            this.dgvTimeSheetDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvTimeSheetDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.dgvBreakDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvBreakDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.rbnErrors.Enabled = true;
            this.rbnException.Enabled = true;
            this.rbnNone.Enabled = true;
            this.rbnBlank.Enabled = true;
            this.rbnNormal.Enabled = true;
            this.rbnBreakException.Enabled = true;

            this.picCompanyLock.Visible = false;
            this.picPayrollLock.Visible = false;
            this.picPayCategoryLock.Visible = false;
            this.picEmployeeLock.Visible = false;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.dgvCompanyDataGridView.Enabled = true;
            this.dgvPayCategoryDataGridView.Enabled = true;
            this.dgvEmployeeDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;
           
            this.btnRefresh.Enabled = true;

            this.cboDateFilter.Enabled = true;

            this.btnToday.Enabled = true;
            this.btnDatePrevious.Enabled = true;
            this.btnDateNext.Enabled = true;

            if (this.rbnNone.Checked == true)
            {
                this.chkRemoveBlanks.Enabled = true;
            }
            else
            {
                if (this.rbnBlank.Checked == true)
                {
                    this.chkRemoveSat.Enabled = true;
                    this.chkRemoveSun.Enabled = true;
                }
            }
            
            this.rbnDateEmployee.Enabled = true;
            this.rbnEmployeeDate.Enabled = true;

            this.dgvTimeSheetDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.dgvBreakDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.btnDeleteTimeSheetRow.Enabled = false;
            this.btnDeleteBreakRow.Enabled = false;

            pvtDataSet.RejectChanges();

            Load_PayCategory_Records();
        }

        private void Load_TimeSheets()
        {
            pvtintTotalTimeSheetMinutes = 0;
            pvtintTotalBreakMinutes = 0;
            string strClockTimeIn = "";
            string strActualTimeIn = "";
            string strActualTimeOut = "";
            string strClockTimeOut = "";

            this.pvtblnBreakDataGridViewLoaded = false;
            this.pvtblnTimeSheetDataGridViewLoaded = false;
            
            pvtblnTimeSheetInError = false;
            pvtblnBreakInError = false;

            pvtBreakDataView = null;
            pvtBreakDataView = new DataView(pvtDataSet.Tables["Break"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND BREAK_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                "BREAK_TIME_IN_MINUTES,BREAK_TIME_OUT_MINUTES",
                DataViewRowState.CurrentRows);

            Clear_DataGridView(dgvBreakDataGridView);

            //2017-06-23
            int intBreakBlankSeqNo = -1;

            //Set Indicator
            for (int intRow = 0; intRow < this.pvtBreakDataView.Count; intRow++)
            {
                if (pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] == System.DBNull.Value)
                {
                    strClockTimeIn = "";
                }
                else
                {
                    if (Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) > 59)
                    {
                        strClockTimeIn = Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) / 60).ToString()
                                                               + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockTimeIn = Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]).ToString();
                    }
                }

                if (pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"]) > 59)
                    {
                        strActualTimeIn = Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"]) / 60).ToString()
                                                               + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualTimeIn = Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_IN_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualTimeIn = "";
                }

                if (pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"]) > 59)
                    {
                        strActualTimeOut = Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"]) / 60).ToString()
                                                               + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualTimeOut = Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualTimeOut = "";
                }

                if (pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] == System.DBNull.Value)
                {
                    strClockTimeOut = "";
                }
                else
                {
                    if (Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) > 59)
                    {
                        strClockTimeOut = Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) / 60).ToString()
                                                               + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockTimeOut = Convert.ToInt32(pvtBreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]).ToString();
                    }
                }

                pvtintTotalBreakMinutes += Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_ACCUM_MINUTES"]);

                if (strActualTimeIn == ""
                && strActualTimeOut == "")
                {
                    intBreakBlankSeqNo = Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_SEQ"]);
                    continue;
                }

                this.dgvBreakDataGridView.Rows.Add("",
                                                   strClockTimeIn,
                                                   strActualTimeIn,
                                                   strActualTimeOut,
                                                   strClockTimeOut,
                                                   Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_ACCUM_MINUTES"]) / 60).ToString() + ":"
                                                   + Convert.ToInt32(Convert.ToInt32(pvtBreakDataView[intRow]["BREAK_ACCUM_MINUTES"]) % 60).ToString("00"),
                                                   pvtBreakDataView[intRow]["BREAK_SEQ"].ToString());

                if (pvtBreakDataView[intRow]["INDICATOR"].ToString() == "X")
                {
                    dgvBreakDataGridView[0,dgvBreakDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;

                    pvtblnBreakInError = true;
                }
            }

            //2017-06-23
            if (intBreakBlankSeqNo != -1)
            {
                //Move Blank Record to Bottom
                this.dgvBreakDataGridView.Rows.Add("",
                                                   "",
                                                   "",
                                                   "",
                                                   "",
                                                   "0:00",
                                                   intBreakBlankSeqNo.ToString());

            }

            if (pvtintTotalBreakMinutes == 0)
            {
                this.dgvBreakTotalsDataGridView[2,0].Value = "0:00";
            }
            else
            {
                this.dgvBreakTotalsDataGridView[2,0].Value = Convert.ToInt32(pvtintTotalBreakMinutes / 60).ToString() + ":" + Convert.ToInt32(pvtintTotalBreakMinutes % 60).ToString("00");
            }

            pvtTimeSheetDataView = null;
            pvtTimeSheetDataView = new DataView(pvtDataSet.Tables["TimeSheet"],
                "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND TIMESHEET_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                "TIMESHEET_TIME_IN_MINUTES,TIMESHEET_TIME_OUT_MINUTES",
                DataViewRowState.CurrentRows);

            Clear_DataGridView(dgvTimeSheetDataGridView);

            //2017-06-23
            int intTimesheetBlankSeqNo = -1;
             
            //Set Indicator
            for (int intRow = 0; intRow < this.pvtTimeSheetDataView.Count; intRow++)
            {
                if (pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] == System.DBNull.Value)
                {
                    strClockTimeIn = "";
                }
                else
                {
                    if (Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) > 59)
                    {
                        strClockTimeIn = Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) / 60).ToString()
                                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockTimeIn = Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]).ToString();
                    }
                }

                if (pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]) > 59)
                    {
                        strActualTimeIn = Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]) / 60).ToString()
                                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualTimeIn = Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualTimeIn = "";
                }

                if (pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]) > 59)
                    {
                        strActualTimeOut = Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]) / 60).ToString()
                                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualTimeOut = Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualTimeOut = "";
                }

                if (pvtTimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] == System.DBNull.Value)
                {
                    strClockTimeOut = "";
                }
                else
                {
                    if (Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) > 59)
                    {
                        strClockTimeOut = Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) / 60).ToString()
                                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockTimeOut = Convert.ToInt32(pvtTimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]).ToString();
                    }
                }

                pvtintTotalTimeSheetMinutes += Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_ACCUM_MINUTES"]);

                if (strActualTimeIn == ""
                && strActualTimeOut == "")
                {
                    intTimesheetBlankSeqNo = Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_SEQ"]);
                    continue;
                }

                this.dgvTimeSheetDataGridView.Rows.Add("",
                                                       strClockTimeIn,
                                                       strActualTimeIn,
                                                       strActualTimeOut,
                                                       strClockTimeOut,
                                                       Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_ACCUM_MINUTES"]) / 60).ToString() + ":"
                                                       + Convert.ToInt32(Convert.ToInt32(pvtTimeSheetDataView[intRow]["TIMESHEET_ACCUM_MINUTES"]) % 60).ToString("00"),
                                                       pvtTimeSheetDataView[intRow]["TIMESHEET_SEQ"].ToString());

                if (pvtTimeSheetDataView[intRow]["INDICATOR"].ToString() == "X")
                {
                    dgvTimeSheetDataGridView[0,dgvTimeSheetDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;

                    pvtblnTimeSheetInError = true;
                }
            }

            //2017-06-23
            if (intTimesheetBlankSeqNo != -1)
            {
                //Move Blank Record to Bottom
                this.dgvTimeSheetDataGridView.Rows.Add("",
                                                       "",
                                                       "",
                                                       "",
                                                       "",
                                                       "0:00",
                                                       intTimesheetBlankSeqNo.ToString());

            }

            this.pvtblnBreakDataGridViewLoaded = true;
            this.pvtblnTimeSheetDataGridViewLoaded = true;

            if (this.dgvBreakDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvBreakDataGridView, 0);
            }
          
            //Set Break (NB intBreakMinutesDefault NOT Used in this Function)
            int intBreakMinutesDefault = 0;
            Set_Break_Value(ref intBreakMinutesDefault);

            if (this.dgvTimeSheetDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvTimeSheetDataGridView, 0);
            }

            if (this.btnSave.Enabled == true)
            {
                Check_To_Add_New_TimeSheet_Row();
                Check_To_Add_New_Break_Row();
            }
        }

        private int Set_Break_Value(ref int intBreakTimeMinutesDefault)
        {
            //Set Break Value
            int intDayPaidHours = 0;
            int intBreakTimeMinutes = 0;
            intBreakTimeMinutesDefault = 0;

            this.grbBreakError.Visible = false;

            if (pvtBreakRangeDataView.Count > 0)
            {
                this.dgvBreakExceptionDataGridView[0, 0].Value = "=>";

                if (pvtintTotalTimeSheetMinutes == 0
                    & pvtintTotalBreakMinutes == 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvBreakRangeDataGridView, 0);

                    this.dgvTimeSheetTotalsDataGridView[0,1].Value = "Break After 0:00";

                    dgvBreakTotalsDataGridView[0, 0].Style = LunchTotalDataGridViewCellStyle;

                    this.dgvBreakTotalsDataGridView[0, 0].Value = "";
                }
                else
                {
                    for (int intBreakRow = 0; intBreakRow < this.pvtBreakRangeDataView.Count; intBreakRow++)
                    {
                        if (Convert.ToInt32(pvtBreakRangeDataView[intBreakRow]["WORKED_TIME_MINUTES"]) <= pvtintTotalTimeSheetMinutes)
                        {
                            intBreakTimeMinutesDefault = Convert.ToInt32(pvtBreakRangeDataView[intBreakRow]["BREAK_MINUTES"]);
                            intBreakTimeMinutes = Convert.ToInt32(pvtBreakRangeDataView[intBreakRow]["BREAK_MINUTES"]);

                            this.dgvTimeSheetTotalsDataGridView[0, 1].Value = "Break After " + Convert.ToInt32(Convert.ToInt32(pvtBreakRangeDataView[intBreakRow]["WORKED_TIME_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtBreakRangeDataView[intBreakRow]["WORKED_TIME_MINUTES"]) % 60).ToString("00");

                            if (pvtintTotalTimeSheetMinutes < pvtintTotalBreakMinutes)
                            {
                                dgvBreakTotalsDataGridView[0, 0].Style = BreakExceptionDataGridViewCellStyle;
                               
                                this.dgvBreakTotalsDataGridView[0, 0].Value = "<=";

                                this.lblBreakHours.Text = "Total Break Hours " + Convert.ToInt32(pvtintTotalBreakMinutes / 60).ToString() + ":" + Convert.ToInt32(pvtintTotalBreakMinutes % 60).ToString("00");
                                this.lblTimesheetHours.Text = "Total Worked Hours " + Convert.ToInt32(pvtintTotalTimeSheetMinutes / 60).ToString() + ":" + Convert.ToInt32(pvtintTotalTimeSheetMinutes % 60).ToString("00");

                                this.dgvBreakExceptionDataGridView[0, 0].Value = "";

                                this.grbBreakError.Visible = true;
                            }
                            else
                            {
                                if (intBreakTimeMinutes < pvtintTotalBreakMinutes)
                                {
                                    intBreakTimeMinutes = pvtintTotalBreakMinutes;
                                                                  
                                    dgvBreakTotalsDataGridView[0, 0].Style = BreakExceptionDataGridViewCellStyle;

                                    this.dgvBreakExceptionDataGridView[0, 0].Value = "";

                                    this.dgvBreakTotalsDataGridView[0, 0].Value = "<=";
                                }
                                else
                                {
                                    dgvBreakTotalsDataGridView[0, 0].Style = LunchTotalDataGridViewCellStyle;
                                    
                                    this.dgvBreakTotalsDataGridView[0, 0].Value = "";

                                    this.dgvBreakExceptionDataGridView[0, 0].Value = "=>";
                                }
                            }

                            this.Set_DataGridView_SelectedRowIndex(dgvBreakRangeDataGridView, intBreakRow);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                //Set All Break Values to Zero
                dgvBreakTotalsDataGridView[0, 0].Style = LunchTotalDataGridViewCellStyle;

                this.dgvBreakTotalsDataGridView[0, 0].Value = "";
                this.dgvTimeSheetTotalsDataGridView[0, 1].Value = "Break After 0:00";

                this.dgvBreakExceptionDataGridView[0, 0].Value = "";
            }

            this.dgvTimeSheetTotalsDataGridView[1,0].Value = Convert.ToInt32(pvtintTotalTimeSheetMinutes / 60).ToString() + ":" + Convert.ToInt32(pvtintTotalTimeSheetMinutes % 60).ToString("00");

            if (pvtintTotalTimeSheetMinutes >= intBreakTimeMinutes)
            {
                intDayPaidHours = pvtintTotalTimeSheetMinutes - intBreakTimeMinutes;

                this.dgvTimeSheetTotalsDataGridView[1, 1].Value = Convert.ToInt32(intBreakTimeMinutes / 60).ToString() + ":" + Convert.ToInt32(intBreakTimeMinutes % 60).ToString("00");
                this.dgvTimeSheetTotalsDataGridView[1,2].Value = Convert.ToInt32(intDayPaidHours / 60).ToString() + ":" + Convert.ToInt32(intDayPaidHours % 60).ToString("00");
            }
            else
            {
                this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "0:00";
                this.dgvTimeSheetTotalsDataGridView[1, 2].Value = this.dgvTimeSheetTotalsDataGridView[1,0].Value;

                intDayPaidHours = pvtintTotalTimeSheetMinutes;
            }

            //Round Day - Return 'intDayPaidHours'
            clsISClientUtilities.Round_For_Period(Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_IND"]), Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_MINUTES"]), ref intDayPaidHours);

            this.dgvTimeSheetTotalsDataGridView[1,3].Value = Convert.ToInt32(intDayPaidHours / 60).ToString() + ":" + Convert.ToInt32(intDayPaidHours % 60).ToString("00");

            return intDayPaidHours;
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                DataSet TempDataSet = new DataSet();
                TempDataSet.Tables.Add(this.pvtDataSet.Tables["TimeSheet"].Clone());

                //Remove Any New Records with Time In & Time Out = Null
                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["TimeSheet"],
                    "",
                    "",
                    DataViewRowState.ModifiedCurrent | DataViewRowState.Added | DataViewRowState.Deleted);

                for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
                {
                    if (pvtTempDataView[intRow].Row.RowState == DataRowState.Deleted)
                    {
                        TempDataSet.Tables["TimeSheet"].ImportRow(pvtTempDataView[intRow].Row);
                    }
                    else
                    {
                        if (pvtTempDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"] == System.DBNull.Value
                        & pvtTempDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"] == System.DBNull.Value)
                        {
                            if (pvtTempDataView[intRow].Row.RowState == DataRowState.Added)
                            {
                                //DataView Count Decreases When Row was Added
                                pvtTempDataView[intRow].Delete();
                                intRow -= 1;
                                continue;
                            }
                        }

                        TempDataSet.Tables["TimeSheet"].ImportRow(pvtTempDataView[intRow].Row);

                        if (pvtTempDataView[intRow].Row.RowState == DataRowState.Added)
                        {
                            //DataView Count Decreases When Row was Added
                            pvtTempDataView[intRow].Delete();
                            intRow -= 1;
                        }
                        else
                        {
                            pvtTempDataView[intRow].Delete();
                        }
                    }
                }

                TempDataSet.Tables.Add(this.pvtDataSet.Tables["Break"].Clone());

                //Remove Any New Recods with Time In & Time Out = Null
                pvtTempDataView = null;
                pvtTempDataView = new DataView(pvtDataSet.Tables["Break"],
                    "",
                    "",
                    DataViewRowState.ModifiedCurrent | DataViewRowState.Added | DataViewRowState.Deleted);

                for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
                {
                    if (pvtTempDataView[intRow].Row.RowState == DataRowState.Deleted)
                    {
                        TempDataSet.Tables["Break"].ImportRow(pvtTempDataView[intRow].Row);
                    }
                    else
                    {
                        if (pvtTempDataView[intRow]["BREAK_TIME_IN_MINUTES"] == System.DBNull.Value
                        & pvtTempDataView[intRow]["BREAK_TIME_OUT_MINUTES"] == System.DBNull.Value)
                        {
                            if (pvtTempDataView[intRow].Row.RowState == DataRowState.Added)
                            {
                                //DataView Count Decreases When Row was Added
                                pvtTempDataView[intRow].Delete();
                                intRow -= 1;
                                continue;
                            }
                        }

                        TempDataSet.Tables["Break"].ImportRow(pvtTempDataView[intRow].Row);

                        if (pvtTempDataView[intRow].Row.RowState == DataRowState.Added)
                        {
                            //DataView Count Decreases When Row was Added
                            pvtTempDataView[intRow].Delete();
                            intRow -= 1;
                        }
                        else
                        {
                            pvtTempDataView[intRow].Delete();
                        }
                    }
                }

                //Compress DataSet
                pvtbytCompress = clsISClientUtilities.Compress_DataSet(TempDataSet);

                object[] objParm = new object[2];
                objParm[0] = pvtbytCompress;
                objParm[1] = pvtstrPayrollType;

                pvtbytCompress = null;
                pvtbytCompress = (byte[])clsISClientUtilities.DynamicFunction("Update_TimeSheet_Records", objParm,true);

                pvtTempDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                pvtDataSet.Merge(pvtTempDataSet);

                this.pvtDataSet.AcceptChanges();

                btnCancel_Click(sender, e);
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
            }
        }

        private void btnDeleteTimeSheetRow_Click(object sender, System.EventArgs e)
        {
            string strRowValue = " AND TIMESHEET_SEQ = " + this.dgvTimeSheetDataGridView[pvtintSpreadSheetSeqNoCol, this.Get_DataGridView_SelectedRowIndex(this.dgvTimeSheetDataGridView)].Value.ToString();

            DataView myDataView = new DataView(this.pvtDataSet.Tables["TimeSheet"],
            "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND TIMESHEET_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'" + strRowValue,
            "TIMESHEET_SEQ",
            DataViewRowState.CurrentRows);

            myDataView.Delete(0);

            //Get All Rows for Current Employee / Date
            myDataView.RowFilter = myDataView.RowFilter.Replace(strRowValue, "");

            RePaint_SpreadSheet_Indicators(this.dgvTimeSheetDataGridView, myDataView);
        }

        private void rbnNone_Click(object sender, System.EventArgs e)
        {
            this.chkRemoveBlanks.Enabled = true;

            this.chkRemoveSat.Checked = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveSun.Enabled = false;

            Load_PayCategory_Records();
        }

        private void rbnEmployeeDate_Click(object sender, System.EventArgs e)
        {
            //Set so that First Rows will be Selected
            pvtintEmployeeNo = -1;
            pvtDateTime = DateTime.Now.AddYears(100);

            this.lblEmployee.Text = "Employee";
            this.lblDate.Text = "Date";

            this.dgvEmployeeDataGridView.Columns[2].HeaderText = "Code";
            this.dgvEmployeeDataGridView.Columns[2].Width = pvtintEmployeeCodeWidth;

            this.dgvEmployeeDataGridView.Columns[3].HeaderText = "Surname";
            this.dgvEmployeeDataGridView.Columns[3].Width = pvtintEmployeeSurnameWidth + pvtintDayPaidHoursWidth;

            this.dgvEmployeeDataGridView.Columns[4].HeaderText = "Name";
            this.dgvEmployeeDataGridView.Columns[4].Width = pvtintEmployeeNameWidth + pvtintDayExceptionWidth + pvtintDayBreakHoursWidth;

            this.dgvEmployeeDataGridView.Columns[3].Visible = true;
            this.dgvEmployeeDataGridView.Columns[4].Visible = true;

            this.dgvEmployeeDataGridView.Columns[2].SortMode = DataGridViewColumnSortMode.Automatic;
            this.dgvEmployeeDataGridView.Columns[3].SortMode = DataGridViewColumnSortMode.Automatic;
            this.dgvEmployeeDataGridView.Columns[4].SortMode = DataGridViewColumnSortMode.Automatic;

            this.dgvDayDataGridView.Columns[2].HeaderText = "Description";
            this.dgvDayDataGridView.Columns[2].Width = pvtintEmployeeCodeWidth + pvtintEmployeeSurnameWidth + pvtintEmployeeNameWidth;

            this.dgvDayDataGridView.Columns[3].HeaderText = "Exception";
            this.dgvDayDataGridView.Columns[3].Width = pvtintDayExceptionWidth;

            this.dgvDayDataGridView.Columns[4].HeaderText = "Total Hrs";
            this.dgvDayDataGridView.Columns[4].Width = pvtintDayPaidHoursWidth;
            
            this.dgvDayDataGridView.Columns[5].HeaderText = "Break Hrs";
            this.dgvDayDataGridView.Columns[5].Width = pvtintDayBreakHoursWidth;

            this.dgvDayDataGridView.Columns[6].Visible = false;
            this.dgvDayDataGridView.Columns[7].Visible = false;

            this.dgvDayDataGridView.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvDayDataGridView.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgvDayDataGridView.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvDayDataGridView.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvDayDataGridView.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;

            Load_PayCategory_Records();
        }

        private void rbnDateEmployee_Click(object sender, System.EventArgs e)
        {
            Clear_DataGridView(dgvEmployeeDataGridView);
            Clear_DataGridView(dgvDayDataGridView);
            Clear_DataGridView(dgvTimeSheetDataGridView);
            Clear_DataGridView(dgvBreakDataGridView);

            //Set so that First Rows will be Selected
            pvtintEmployeeNo = -1;
            pvtDateTime = DateTime.Now.AddYears(100);

            this.lblEmployee.Text = "Date";
            this.lblDate.Text = "Employee";

            this.dgvEmployeeDataGridView.Columns[2].HeaderText = "Description";
            this.dgvEmployeeDataGridView.Columns[2].Width = pvtintEmployeeCodeWidth + pvtintEmployeeSurnameWidth + pvtintEmployeeNameWidth + pvtintDayExceptionWidth + pvtintDayBreakHoursWidth + pvtintDayPaidHoursWidth;

            this.dgvEmployeeDataGridView.Columns[3].Visible = false;
            this.dgvEmployeeDataGridView.Columns[4].Visible = false;
            this.dgvEmployeeDataGridView.Columns[5].Visible = false;

            this.dgvDayDataGridView.Columns[2].HeaderText = "Code";
            this.dgvDayDataGridView.Columns[2].Width = pvtintEmployeeCodeWidth;

            this.dgvDayDataGridView.Columns[3].HeaderText = "Surname";
            this.dgvDayDataGridView.Columns[3].Width = pvtintEmployeeSurnameWidth;

            this.dgvDayDataGridView.Columns[4].HeaderText = "Name";
            this.dgvDayDataGridView.Columns[4].Width = pvtintEmployeeNameWidth;

            this.dgvDayDataGridView.Columns[3].SortMode = DataGridViewColumnSortMode.Automatic;
            this.dgvDayDataGridView.Columns[4].SortMode = DataGridViewColumnSortMode.Automatic;

            this.dgvDayDataGridView.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dgvDayDataGridView.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            this.dgvDayDataGridView.Columns[5].HeaderText = "Exception";
            this.dgvDayDataGridView.Columns[5].Width = pvtintDayExceptionWidth;

            this.dgvDayDataGridView.Columns[6].HeaderText = "Total Hrs";
            this.dgvDayDataGridView.Columns[6].Width = pvtintDayPaidHoursWidth;

            this.dgvDayDataGridView.Columns[7].HeaderText = "Break Hrs";
            this.dgvDayDataGridView.Columns[7].Width = pvtintDayBreakHoursWidth;

            this.dgvDayDataGridView.Columns[6].Visible = true;
            this.dgvDayDataGridView.Columns[7].Visible = true;

            Load_PayCategory_Records();
        }

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dlgResult;
                DateTime dtDateTime = DateTime.Now;

                dlgResult = CustomClientMessageBox.Show("Delete ALL TimeSheet / Break Records for Cost Centre '" + this.dgvPayCategoryDataGridView[2, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Value.ToString() + "'.",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[5];
                    objParm[0] = pvtint64CompanyNo;
                    objParm[1] = pvtintPayCategoryNo;
                    objParm[2] = pvtstrPayrollType;
                    objParm[3] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[4] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                    clsISClientUtilities.DynamicFunction("Delete_PayCategory_TimeSheet_Records", objParm, true);

                    //Remove All TimeSheets For Cost Centre
                    pvtTempDataView = null;
                    pvtTempDataView = new DataView(pvtDataSet.Tables["TimeSheet"],
                            "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo,
                        "",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
                    {
                        pvtTempDataView[intRow].Delete();

                        intRow -= 1;
                    }

                    //Remove All Breaks For Cost Centre
                    pvtTempDataView = null;
                    pvtTempDataView = new DataView(pvtDataSet.Tables["Break"],
                            "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo,
                        "",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
                    {
                        pvtTempDataView[intRow].Delete();

                        intRow -= 1;
                    }

                    //Remove All TimeSheets For Cost Centre
                    pvtTempDataView = null;
                    pvtTempDataView = new DataView(pvtDataSet.Tables["DayTotal"],
                            "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo,
                        "",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtTempDataView.Count; intRow++)
                    {
                        pvtTempDataView[intRow].Delete();

                        intRow -= 1;
                    }

                    this.pvtDataSet.AcceptChanges();

                    Load_PayCategory_Records();
                }
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
            }
        }

        private void GeneralFilter_Click(object sender, EventArgs e)
        {
            this.chkRemoveBlanks.Checked = false;
            this.chkRemoveBlanks.Enabled = false;

            this.chkRemoveSat.Checked = false;
            this.chkRemoveSat.Enabled = false;
            this.chkRemoveSun.Checked = false;
            this.chkRemoveSun.Enabled = false;

            Load_PayCategory_Records();
        }

        private void rbnBlank_Click(object sender, EventArgs e)
        {
            this.chkRemoveBlanks.Checked = false;
            this.chkRemoveBlanks.Enabled = false;

            this.chkRemoveSat.Enabled = true;
            this.chkRemoveSun.Enabled = true;

            Load_PayCategory_Records();
        }

        private void chkRemoveBlanks_Click(object sender, EventArgs e)
        {
            if (this.rbnNone.Checked == true)
            {
                Load_PayCategory_Records();
            }
        }

        private void frmTimeSheetClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }

        private void cboDateFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDateFilter.SelectedIndex > -1)
            {
                DateTime dtToday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                pvtdtFilterEndDate = Convert.ToDateTime(pvtDataSet.Tables["DateSelection"].Rows[cboDateFilter.SelectedIndex]["ACTUAL_DATE"]);

                int intTodayOffset = pvtdtFilterEndDate.Subtract(dtToday).Days;

                this.lblTodayOffset.Text = intTodayOffset.ToString();

                if (this.dgvCompanyDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
                }
            }
        }

        private void chkRemove_Saturday_Sunday_Click(object sender, EventArgs e)
        {
            Load_PayCategory_Records();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (pvtint64CompanyNo != -1)
                {
                    DataView myDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                    "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND DAY_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                    "",
                    DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < myDataView.Count; intRow++)
                    {
                        myDataView[intRow].Delete();
                        
                        intRow -= 1;
                    }

                    myDataView = new DataView(this.pvtDataSet.Tables["TimeSheet"],
                    "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND TIMESHEET_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                        "TIMESHEET_SEQ DESC",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < myDataView.Count; intRow++)
                    {
                        myDataView[intRow].Delete();

                        intRow -= 1;
                    }

                    myDataView = new DataView(this.pvtDataSet.Tables["Break"],
                    "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND BREAK_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                        "",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < myDataView.Count; intRow++)
                    {
                        myDataView[intRow].Delete();

                        intRow -= 1;
                    }

                    myDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                    "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo,
                        "",
                        DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < myDataView.Count; intRow++)
                    {
                        myDataView[intRow].Delete();

                        intRow -= 1;
                    }

                    //Delete Rows
                    pvtDataSet.AcceptChanges();

                    object[] objParm = new object[6];
                    objParm[0] = pvtint64CompanyNo;
                    objParm[1] = pvtintPayCategoryNo;
                    objParm[2] = pvtstrPayrollType;
                    objParm[3] = pvtDateTime.ToString("yyyy-MM-dd");
                    objParm[4] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[5] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                    byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Day_Timesheets_Records", objParm,false);
                    DataSet DataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                    pvtDataSet.Merge(DataSet);

                    //Delete Rows
                    pvtDataSet.AcceptChanges();
                    
                    //Set Colour For PayCategory Spreadsheet
                    pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                    pvtstrPayCategoryFilter + pvtstrDataAndTypeFilter + pvtstrCategoryType,
                            "INDICATOR DESC",
                            DataViewRowState.CurrentRows);

                    if (pvtTempDataView.Count > 0)
                    {
                        if (pvtTempDataView[0]["INDICATOR"].ToString() == "X")
                        {
                            this.dgvPayCategoryDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = ErrorDataGridViewCellStyle;
                        }
                        else
                        {
                            if (pvtTempDataView[0]["INDICATOR"].ToString() == "E")
                            {
                                this.dgvPayCategoryDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = ExceptionDataGridViewCellStyle;
                            }
                            else
                            {
                                if (pvtTempDataView[0]["INDICATOR"].ToString() == "B")
                                {
                                    this.dgvPayCategoryDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NoRecordDataGridViewCellStyle;
                                }
                                else
                                {
                                    if (pvtTempDataView[0]["INDICATOR"].ToString() == "")
                                    {
                                        this.dgvPayCategoryDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NormalDataGridViewCellStyle;
                                    }
                                }
                            }
                        }

                        pvtTempDataView.Sort = "BREAK_INDICATOR DESC";

                        if (pvtTempDataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                        {
                            this.dgvPayCategoryDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = BreakExceptionDataGridViewCellStyle;
                        }
                        else
                        {
                            this.dgvPayCategoryDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NormalDataGridViewCellStyle;
                        }
                    }
                    else
                    {
                        this.dgvPayCategoryDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NoRecordDataGridViewCellStyle;

                        this.dgvPayCategoryDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NormalDataGridViewCellStyle;
                    }

                    //Update Current Pay Category with Upload Date/Time
                    if (myDataView[0]["LAST_UPLOAD_DATETIME"] != System.DBNull.Value)
                    {
                        this.dgvPayCategoryDataGridView[2, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Value = Convert.ToDateTime(myDataView[0]["LAST_UPLOAD_DATETIME"]).ToString("dd MMMM yyyy   -   HH:mm");
                    }

                    if (dgvPayCategoryDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView));
                    }
                }
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
            }
        }

        private void Add_DayTotal_Record()
        {
            DataRowView drvDataRowView = pvtDayTotalDataView.AddNew();

            drvDataRowView.BeginEdit();

            drvDataRowView["COMPANY_NO"] = pvtint64CompanyNo;
            drvDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
            drvDataRowView["PAY_CATEGORY_NO"] = pvtintPayCategoryNo;
            drvDataRowView["EMPLOYEE_NO"] = pvtintEmployeeNo;
            drvDataRowView["DAY_DATE"] = pvtDateTime;

            drvDataRowView["DAY_NO"] = pvtDateTime.DayOfWeek;
            drvDataRowView["DAY_PAID_MINUTES"] = 0;
            drvDataRowView["INDICATOR"] = "";

            //Ended Here so that TIMESHEET_SEQ can be Added to TimeSheet Row
            drvDataRowView.EndEdit();
        }

        private void btnDeleteBreakRow_Click(object sender, EventArgs e)
        {
            string strRowValue = " AND BREAK_SEQ = " + this.dgvBreakDataGridView[pvtintSpreadSheetSeqNoCol, this.Get_DataGridView_SelectedRowIndex(dgvBreakDataGridView)].Value.ToString();

            DataView myDataView = new DataView(this.pvtDataSet.Tables["Break"],
            "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND BREAK_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'" + strRowValue,
            "",
            DataViewRowState.CurrentRows);

            myDataView.Delete(0);

            //Get All Rows for Current Employee / Date
            myDataView.RowFilter = myDataView.RowFilter.Replace(strRowValue, "");

            RePaint_SpreadSheet_Indicators(this.dgvBreakDataGridView, myDataView);
        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            if (this.cboDateFilter.Items.Count > 0)
            {
                for (int intRow = 0; intRow < pvtDataSet.Tables["DateSelection"].Rows.Count; intRow++)
                {
                    if (pvtDataSet.Tables["DateSelection"].Rows[intRow]["ACTUAL_DATE"].ToString() == DateTime.Now.ToString("yyyy-MM-dd"))
                    {
                        this.cboDateFilter.SelectedIndex = intRow;

                        break;
                    }
                }
            }
        }

        private void btnDatePrevious_Click(object sender, EventArgs e)
        {
            if (this.cboDateFilter.SelectedIndex > 0)
            {
                this.cboDateFilter.SelectedIndex -= 1;
            }
        }

        private void btnDateNext_Click(object sender, EventArgs e)
        {
            if (this.cboDateFilter.SelectedIndex < this.cboDateFilter.Items.Count - 1)
            {
                this.cboDateFilter.SelectedIndex += 1;
            }
        }

        private void dgvCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (pvtblnCompanyDataGridViewLoaded == true                    )
                {
                    if (pvtintCompanyDataGridViewRowIndex != e.RowIndex)
                    {
                        pvtintCompanyDataGridViewRowIndex = e.RowIndex;

                        this.Cursor = Cursors.AppStarting;

                        this.grbEmployeeLock.Visible = false;

                        Clear_DataGridView(dgvPayCategoryDataGridView);
                        Clear_DataGridView(dgvEmployeeDataGridView);
                        Clear_DataGridView(dgvDayDataGridView);
                        Clear_DataGridView(dgvTimeSheetDataGridView);
                        Clear_DataGridView(dgvBreakDataGridView);
                        Clear_DataGridView(dgvBreakRangeDataGridView);

                        dgvBreakTotalsDataGridView[0, 0].Style = LunchTotalDataGridViewCellStyle;
                        this.dgvBreakTotalsDataGridView[0, 0].Value = "";
                        this.dgvBreakTotalsDataGridView[2, 0].Value = "0:00";

                        this.dgvBreakExceptionDataGridView[0, 0].Value = "";

                        this.grbBreakError.Visible = false;

                        pvtint64CompanyNo = Convert.ToInt32(pvtDataSet.Tables["Company"].Rows[Convert.ToInt32(this.dgvCompanyDataGridView[1, e.RowIndex].Value)]["COMPANY_NO"]);

                        DataView DataView = new DataView(pvtDataSet.Tables["PayCategory"],
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

                        //Errol 2018-10-06
                        if (pvtDataSet.Tables["Dates"] != null)
                        {
                            DataView = null;
                            DataView = new DataView(pvtDataSet.Tables["Dates"],
                             "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "'",
                             "",
                             DataViewRowState.CurrentRows);

                            if (DataView.Count == 0)
                            {
                                object[] objParm = new object[4];
                                objParm[0] = pvtint64CompanyNo;
                                objParm[1] = pvtstrPayrollType;
                                objParm[2] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                                objParm[3] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_PayCategory_Records", objParm, false);
                                DataSet TempDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                                if (TempDataSet.Tables["Dates"].Rows.Count > 0)
                                {
                                    pvtDataSet.Merge(TempDataSet);
                                }
                            }
                        }

                        //Sets Correct Column Headers when Fired this Way
                        if (rbnDateEmployee.Checked == true)
                        {
                            rbnDateEmployee_Click(sender, e);
                        }
                        else
                        {
                            this.rbnEmployeeDate_Click(sender, e);
                        }

                        this.Cursor = Cursors.Default;
                    }
                }
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
            }
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (pvtblnPayrollTypeDataGridViewLoaded == true)
                {
                    if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                    {
                        this.rbnNone.Checked = true;
                        this.chkRemoveBlanks.Checked = false;
                        this.chkRemoveSat.Checked = false;
                        this.chkRemoveSun.Checked = false;

                        this.chkRemoveBlanks.Enabled = true;

                        this.chkRemoveSat.Enabled = false;
                        this.chkRemoveSun.Enabled = false;

                        pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                        this.Cursor = Cursors.AppStarting;

                        pvtstrPayrollType = this.dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                        if (dgvCompanyDataGridView.Rows.Count > 0)
                        {
                            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Dates"], "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'", "", DataViewRowState.CurrentRows);

                            if (DataView.Count == 0)
                            {
                                object[] objParm = new object[4];
                                objParm[0] = pvtint64CompanyNo;
                                objParm[1] = pvtstrPayrollType;
                                objParm[2] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                                objParm[3] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_PayCategory_Records", objParm, false);
                                DataSet TempDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                                if (TempDataSet.Tables["Dates"].Rows.Count > 0)
                                {
                                    pvtDataSet.Merge(TempDataSet);
                                }
                            }

                            this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
                        }

                        this.Cursor = Cursors.Default;
                    }
                }
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
            }
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    this.Cursor = Cursors.AppStarting;

                    Clear_DataGridView(dgvEmployeeDataGridView);
                    Clear_DataGridView(dgvDayDataGridView);
                    Clear_DataGridView(dgvTimeSheetDataGridView);
                    Clear_DataGridView(dgvBreakRangeDataGridView);
                    Clear_DataGridView(dgvBreakDataGridView);

                    dgvBreakTotalsDataGridView[0, 0].Style = LunchTotalDataGridViewCellStyle;
                    this.dgvBreakTotalsDataGridView[0, 0].Value = "";
                    this.dgvTimeSheetTotalsDataGridView[0, 1].Value = "Break After 0:00";
                    this.dgvTimeSheetTotalsDataGridView[1, 1].Value = "0:00";

                    this.dgvBreakExceptionDataGridView[0, 0].Value = "";

                    this.grbBreakError.Visible = false;

                    pvtintPayCategoryTableRowNo = Convert.ToInt32(this.dgvPayCategoryDataGridView[4, e.RowIndex].Value);

                    pvtintPayCategoryNo = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_CATEGORY_NO"]);

                    pvtstrPayCategoryFilter = "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo;

                    pvtBreakRangeDataView = null;
                    pvtBreakRangeDataView = new DataView(this.pvtDataSet.Tables["BreakRange"],
                    pvtstrPayCategoryFilter,
                    "WORKED_TIME_MINUTES,BREAK_MINUTES",
                    DataViewRowState.CurrentRows);

                    pvtblnBreakRangeDataGridViewLoaded = false;

                    for (int intRow = 0; intRow < pvtBreakRangeDataView.Count; intRow++)
                    {
                        this.dgvBreakRangeDataGridView.Rows.Add(Convert.ToInt32(Convert.ToInt32(pvtBreakRangeDataView[intRow]["WORKED_TIME_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtBreakRangeDataView[intRow]["WORKED_TIME_MINUTES"]) % 60).ToString("00"),
                                                                Convert.ToInt32(Convert.ToInt32(pvtBreakRangeDataView[intRow]["BREAK_MINUTES"]) / 60).ToString() + ":" + Convert.ToInt32(Convert.ToInt32(pvtBreakRangeDataView[intRow]["BREAK_MINUTES"]) % 60).ToString("00"));
                    }

                    pvtblnBreakRangeDataGridViewLoaded = true;

                    if (dgvBreakRangeDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvBreakRangeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvBreakRangeDataGridView));
                    }

                    string strDirection = "";

                    if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_IND"].ToString() == "0")
                    {
                        this.dgvTimeSheetTotalsDataGridView[0, 3].Value = "No Rounding";
                    }
                    else
                    {
                        if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_IND"].ToString() == "1")
                        {
                            strDirection = "Rnd Up";
                        }
                        else
                        {
                            if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_IND"].ToString() == "2")
                            {
                                strDirection = "Rnd Down";
                            }
                            else
                            {
                                if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_IND"].ToString() == "3")
                                {
                                    strDirection = "Rnd Closest";
                                }
                            }
                        }

                        this.dgvTimeSheetTotalsDataGridView[0, 3].Value = strDirection + " " + Convert.ToDouble(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["DAILY_ROUNDING_MINUTES"]).ToString("#0") + " Min";
                    }

                    Load_Employee_SpreadSheet();

                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnEmployeeDataGridViewLoaded == true)
            {
                if (pvtintEmployeeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintEmployeeDataGridViewRowIndex = e.RowIndex;

                    this.Cursor = Cursors.AppStarting;

                    if (rbnEmployeeDate.Checked == true)
                    {
                        //Get Employee Number
                        pvtintEmployeeNo = Convert.ToInt32(pvtEmployeeOrDateDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[this.pvtintEmployeeNoCol, e.RowIndex].Value)]["EMPLOYEE_NO"]);

                        pvtEmployeeDataView = null;
                        pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Dates"],
                            pvtstrPayCategoryFilter + " AND DAY_DATE > '" + Convert.ToDateTime(pvtEmployeeOrDateDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[this.pvtintEmployeeNoCol, e.RowIndex].Value)]["EMPLOYEE_LAST_RUNDATE"]).ToString("yyyy-MM-dd") + "'" + pvtstrDataAndTypeFilter,
                            "DAY_DATE DESC",
                            DataViewRowState.CurrentRows);
                    }
                    else
                    {
                        //Get Current Date
                        this.pvtDateTime = Convert.ToDateTime(pvtEmployeeOrDateDataView[Convert.ToInt32(this.dgvEmployeeDataGridView[this.pvtintEmployeeNoCol, e.RowIndex].Value)]["DAY_DATE"]);

                        this.toolTip.SetToolTip(btnRefresh, "Refresh " + pvtDateTime.ToString("dd MMMM yyyy") + " (Selected Date Row)");

                        pvtEmployeeDataView = null;
                        pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                            pvtstrPayCategoryFilter + " AND EMPLOYEE_LAST_RUNDATE < '" + pvtDateTime.ToString("yyyy-MM-dd") + "'",
                            "EMPLOYEE_CODE",
                            DataViewRowState.CurrentRows);
                    }

                    Load_Day_SpreadSheet();

                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void dgvDayDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnDayDataGridViewLoaded == true)
            {
                if (pvtintDayDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintDayDataGridViewRowIndex = e.RowIndex;

                    this.grbBreakError.Visible = false;

                    if (this.rbnEmployeeDate.Checked == true)
                    {
                        //Get Date
                        pvtDateTime = Convert.ToDateTime(this.dgvDayDataGridView[this.pvtintDateColNo, e.RowIndex].Value);

                        this.toolTip.SetToolTip(btnRefresh, "Refresh " + pvtDateTime.ToString("dd MMMM yyyy") + " (Selected Date Row)");

                        pvtDayTotalDataView = null;
                        pvtDayTotalDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                            "COMPANY_NO = " + this.pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DAY_DATE = '" + this.pvtDateTime.ToString("yyyy-MM-dd") + "'",
                            "DAY_DATE",
                            DataViewRowState.CurrentRows);

                        this.lblDayDesc.Text = this.dgvEmployeeDataGridView[4, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Value.ToString() + " " + this.dgvEmployeeDataGridView[3, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Value.ToString() + "    " + this.dgvDayDataGridView[2, e.RowIndex].Value.ToString();
                    }
                    else
                    {
                        pvtintEmployeeNo = Convert.ToInt32(pvtEmployeeDataView[Convert.ToInt32(this.dgvDayDataGridView[pvtintDateColNo, e.RowIndex].Value)]["EMPLOYEE_NO"]);

                        pvtDayTotalDataView = null;
                        pvtDayTotalDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                            "COMPANY_NO = " + this.pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND DAY_DATE = '" + this.pvtDateTime.ToString("yyyy-MM-dd") + "'",
                            "DAY_DATE",
                            DataViewRowState.CurrentRows);

                        this.lblDayDesc.Text = this.dgvDayDataGridView[4, e.RowIndex].Value.ToString() + " " + this.dgvDayDataGridView[3, e.RowIndex].Value.ToString() + "    " + this.dgvEmployeeDataGridView[2, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Value.ToString();
                    }

                    Load_TimeSheets();
                }
            }
        }

        private void dgvBreakRangeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvBreakRangeDataGridView.Rows.Count > 0
                & pvtblnBreakRangeDataGridViewLoaded == true)
            {
                //dgvBreakRangeDataGridView.CurrentCell = dgvBreakRangeDataGridView[0,e.RowIndex];
            }
        }

        private void dgvTimeSheetDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvTimeSheetDataGridView.Rows.Count > 0
                & pvtblnTimeSheetDataGridViewLoaded == true)
            {
            }
        }
        
        private void Cell_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox myTextBox = (TextBox)sender;

            //NB When an Invalid Time Passes this Test eg 70 then it will be caught in The CellChanged Event
            //NB When an Invalid Time Passes this Test eg 70 then it will be caught in The CellChanged Event

            if (e.KeyChar == (char)8
                | (e.KeyChar > (char)47
                & e.KeyChar < (char)58))
            {
                if (e.KeyChar != (char)8
                    & myTextBox.Text.Length == 4)
                {
                    if (myTextBox.SelectionLength == 0)
                    {
                        System.Console.Beep();
                        e.Handled = true;
                    }
                }
                else
                {
                    //Build TextBox Field
                    if (myTextBox.Text != "")
                    {
                        string strNewTextField = "";
                        int intTimeMM = 0;
                        int intTimeHH = 0;

                        if (myTextBox.SelectionLength == myTextBox.Text.Length)
                        {
                            //Replace All Characters with 1 New Numeric Character 
                        }
                        else
                        {
                            if (myTextBox.SelectionLength > 0)
                            {
                                //Replace 
                                strNewTextField = myTextBox.Text.Remove(myTextBox.SelectionStart, myTextBox.SelectionLength);

                                if (myTextBox.SelectionStart == 0)
                                {
                                    strNewTextField = e.KeyChar + strNewTextField;
                                }
                                else
                                {

                                    if (myTextBox.SelectionStart == strNewTextField.Length)
                                    {
                                        //Add to End
                                        strNewTextField = strNewTextField + e.KeyChar;
                                    }
                                    else
                                    {
                                        //Middle
                                        strNewTextField = strNewTextField.Substring(0, myTextBox.SelectionStart) + e.KeyChar + strNewTextField.Substring(myTextBox.SelectionStart);
                                    }
                                }
                            }
                            else
                            {
                                if (myTextBox.Text.Length == myTextBox.SelectionStart)
                                {
                                    if (e.KeyChar == 8)
                                    {
                                        strNewTextField = myTextBox.Text.Substring(0, myTextBox.Text.Length - 1);
                                    }
                                    else
                                    {
                                        strNewTextField = myTextBox.Text + e.KeyChar;
                                    }
                                }
                                else
                                {
                                    if (e.KeyChar == 8)
                                    {
                                        strNewTextField = myTextBox.Text.Substring(0, myTextBox.SelectionStart - 1) + myTextBox.Text.Substring(myTextBox.SelectionStart);
                                    }
                                    else
                                    {
                                        strNewTextField = myTextBox.Text.Substring(0, myTextBox.SelectionStart) + e.KeyChar + myTextBox.Text.Substring(myTextBox.SelectionStart);
                                    }
                                }
                            }
                        }

                        if (strNewTextField != "")
                        {
                            //Check that Time is in Correct Format
                            if (strNewTextField.Length == 2
                                & strNewTextField.Substring(0, 1) == "0")
                            {
                                System.Console.Beep();
                                e.Handled = true;
                            }
                            else
                            {

                                if (strNewTextField.Length >= 3)
                                {
                                    intTimeMM = Convert.ToInt32(strNewTextField.Substring(strNewTextField.Length - 2, 2));
                                }
                                else
                                {
                                    intTimeMM = Convert.ToInt32(strNewTextField);
                                }

                                if (intTimeMM > 59)
                                {
                                    if (strNewTextField.Length > 3
                                        & myTextBox.SelectionLength == 0)
                                    {
                                        System.Console.Beep();
                                        e.Handled = true;
                                    }
                                    else
                                    {
                                        if (strNewTextField.Length == 3)
                                        {
                                            if (Convert.ToInt32(strNewTextField.Substring(2, 1)) > 5)
                                            {
                                                System.Console.Beep();
                                                e.Handled = true;
                                            }
                                            else
                                            {
                                                if (Convert.ToInt32(strNewTextField) > 259)
                                                {
                                                    System.Console.Beep();
                                                    e.Handled = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToInt32(strNewTextField.Substring(1, 1)) > 5)
                                            {
                                                System.Console.Beep();
                                                e.Handled = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (strNewTextField.Length >= 3)
                                    {
                                        intTimeHH = Convert.ToInt32(strNewTextField.Substring(0, strNewTextField.Length - 2));

                                        if (intTimeHH > 24)
                                        {
                                            System.Console.Beep();
                                            e.Handled = true;
                                        }
                                        else
                                        {
                                            if (Convert.ToInt32(strNewTextField) > 2400)
                                            {
                                                System.Console.Beep();
                                                e.Handled = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                }
                else
                {
                    System.Console.Beep();
                    e.Handled = true;
                }
            }
        }

        private void TimeSheet_Or_Break_DataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Cell_KeyPress);
            e.Control.KeyPress += new KeyPressEventHandler(Cell_KeyPress);
        }

        private void TimeSheet_Or_Break_DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (this.btnSave.Enabled == true
            && pvtblnAllowToEnter == true)
            {
                DataGridView myDataGridView = (DataGridView)sender;

                //Stop ReEntrant Call
                if (myDataGridView[e.ColumnIndex, e.RowIndex].Selected == false)
                {
                    return;
                }
                
                if (e.ColumnIndex == pvtintClockInSetColTimeSheetOrBreakDataGridView
                || e.ColumnIndex == pvtintClockOutSetColTimeSheetOrBreakDataGridView)
                {
                    if (myDataGridView[e.ColumnIndex, e.RowIndex].Value == null)
                    {
                        //Causes ReEntry to This Function
                        pvtblnAllowToEnter = false;
                        myDataGridView[e.ColumnIndex, e.RowIndex].Value = "";
                        pvtblnAllowToEnter = true;
                    }
                    
                    if (myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString() == ""
                    || myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString() == "2400")
                    {
                    }
                    else
                    {
                        if (myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Length > 2)
                        {
                            if (Convert.ToInt32(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Length - 2)) > 23
                                | Convert.ToInt32(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Length - 2, 2)) > 59)
                            {
                                //Causes ReEntry to This Function
                                pvtblnAllowToEnter = false;
                                myDataGridView[e.ColumnIndex, e.RowIndex].Value = "";
                                pvtblnAllowToEnter = true;
                                System.Console.Beep();
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(myDataGridView[e.ColumnIndex, e.RowIndex].Value) > 59)
                            {
                                //Causes ReEntry to This Function
                                pvtblnAllowToEnter = false;
                                myDataGridView[e.ColumnIndex, e.RowIndex].Value = "";
                                pvtblnAllowToEnter = true;
                                System.Console.Beep();
                            }
                        }
                    }

                    int intTimeMinutes = 0;
                    string strFieldPrefix = "";
                    string strInOutDef = "";
                    string strRowValue = "";

                    DataView myDataView = null;

                    if (myDataGridView.Name == "dgvBreakDataGridView")
                    {
                        strRowValue = " AND BREAK_SEQ = " + myDataGridView[pvtintSpreadSheetSeqNoCol, e.RowIndex].Value.ToString();

                        myDataView = new DataView(this.pvtDataSet.Tables["Break"],
                        "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND BREAK_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'" + strRowValue,
                        "BREAK_TIME_IN_MINUTES,BREAK_TIME_OUT_MINUTES",
                        DataViewRowState.CurrentRows);

                        strFieldPrefix = "BREAK";
                    }
                    else
                    {
                        strRowValue = " AND TIMESHEET_SEQ = " + myDataGridView[pvtintSpreadSheetSeqNoCol, e.RowIndex].Value.ToString();

                        myDataView = new DataView(this.pvtDataSet.Tables["TimeSheet"],
                        "COMPANY_NO = " + pvtint64CompanyNo + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_NO = " + pvtintEmployeeNo + " AND TIMESHEET_DATE = '" + pvtDateTime.ToString("yyyy-MM-dd") + "'" + strRowValue,
                         "TIMESHEET_TIME_IN_MINUTES,TIMESHEET_TIME_OUT_MINUTES",
                        DataViewRowState.CurrentRows);

                        strFieldPrefix = "TIMESHEET";
                    }

                    if (e.ColumnIndex == pvtintClockInSetColTimeSheetOrBreakDataGridView)
                    {
                        strInOutDef = "IN";
                    }
                    else
                    {
                        strInOutDef = "OUT";
                    }

                    if (myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString() == "")
                    {
                        myDataView[0][strFieldPrefix + "_TIME_" + strInOutDef + "_MINUTES"] = System.DBNull.Value;
                        
                        int intCheckCol = pvtintClockOutSetColTimeSheetOrBreakDataGridView;

                        if (e.ColumnIndex == pvtintClockOutSetColTimeSheetOrBreakDataGridView)
                        {
                            intCheckCol = pvtintClockInSetColTimeSheetOrBreakDataGridView;
                        }
                        
                        //On Delete of Value it Can be null
                        if (myDataGridView[intCheckCol, e.RowIndex].Value == null)
                        {
                            myDataView[0]["INDICATOR"] = "";
                        }
                        else
                        {
                            if (myDataGridView[intCheckCol, e.RowIndex].Value.ToString() == "")
                            {
                                myDataView[0]["INDICATOR"] = "";
                            }
                            else
                            {
                                myDataView[0]["INDICATOR"] = "X";
                            }
                        }

                        myDataView[0][strFieldPrefix + "_ACCUM_MINUTES"] = 0;
                    }
                    else
                    {
                        if (myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Length >= 3)
                        {
                            intTimeMinutes = (Convert.ToInt32(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Length - 2)) * 60)
                                                + Convert.ToInt32(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString().Length - 2, 2));
                        }
                        else
                        {
                            intTimeMinutes = Convert.ToInt32(myDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString());
                        }

                        myDataView[0][strFieldPrefix + "_TIME_" + strInOutDef + "_MINUTES"] = intTimeMinutes;
                    }

                    if (myDataView[0][strFieldPrefix + "_TIME_IN_MINUTES"] == System.DBNull.Value
                    || myDataView[0][strFieldPrefix + "_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                        if (myDataView[0][strFieldPrefix + "_TIME_IN_MINUTES"] == System.DBNull.Value
                        && myDataView[0][strFieldPrefix + "_TIME_OUT_MINUTES"] == System.DBNull.Value)
                        {
                        }
                        else
                        {
                            myDataView[0]["INDICATOR"] = "X";
                            myDataView[0][strFieldPrefix + "_ACCUM_MINUTES"] = 0;
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(myDataView[0][strFieldPrefix + "_TIME_OUT_MINUTES"]) > Convert.ToInt32(myDataView[0][strFieldPrefix + "_TIME_IN_MINUTES"]))
                        {
                            //Correct
                            myDataView[0]["INDICATOR"] = "";
                            myDataView[0][strFieldPrefix + "_ACCUM_MINUTES"] = Convert.ToInt32(myDataView[0][strFieldPrefix + "_TIME_OUT_MINUTES"]) - Convert.ToInt32(myDataView[0][strFieldPrefix + "_TIME_IN_MINUTES"]);
                        }
                        else
                        {
                            myDataView[0]["INDICATOR"] = "X";
                            myDataView[0][strFieldPrefix + "_ACCUM_MINUTES"] = 0;
                        }
                    }

                    //Set SpreadSheet Row
                    for (int intSpreadsheetRow = 0; intSpreadsheetRow < myDataGridView.Rows.Count; intSpreadsheetRow++)
                    {
                        if (myDataGridView[pvtintSpreadSheetSeqNoCol, intSpreadsheetRow].Value.ToString() == myDataView[0][strFieldPrefix + "_SEQ"].ToString())
                        {
                            if (myDataView[0]["INDICATOR"].ToString() == "X")
                            {
                                myDataGridView[0,intSpreadsheetRow].Style = ErrorDataGridViewCellStyle;
                            }
                            else
                            {
                                myDataGridView[0,intSpreadsheetRow].Style = NormalDataGridViewCellStyle;
                            }

                            myDataGridView[pvtintSpreadSheetAccumTotalCol, intSpreadsheetRow].Value = Convert.ToInt32(Convert.ToInt32(myDataView[0][strFieldPrefix + "_ACCUM_MINUTES"]) / 60).ToString() + ":"
                                + Convert.ToInt32(Convert.ToInt32(myDataView[0][strFieldPrefix + "_ACCUM_MINUTES"]) % 60).ToString("00");

                            break;
                        }
                    }

                    //Get All Rows for Current Employee / Date
                    myDataView.RowFilter = myDataView.RowFilter.Replace(strRowValue, "");

                    RePaint_SpreadSheet_Indicators(myDataGridView, myDataView);
                }
            }
        }


        private void TimeSheet_Or_Break_DataGridView_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {



            if (this.btnSave.Enabled == true)
            {
                if (e.RowIndex > -1
                & e.ColumnIndex > -1)
                {
                    if (e.ColumnIndex == pvtintClockInSetColTimeSheetOrBreakDataGridView
                        | e.ColumnIndex == pvtintClockOutSetColTimeSheetOrBreakDataGridView)
                    {
                        this.Cursor = System.Windows.Forms.Cursors.IBeam;
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
        }

        private void TimeSheet_Or_Break_DataGridView_MouseLeave(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.Cursor = Cursors.Default;
            }
        }
        
        private void dgvBreakDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvBreakDataGridView.Rows.Count > 0
             & this.pvtblnBreakDataGridViewLoaded == true)
            {
            }
        }

        private void dgvEmployeeDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (rbnDateEmployee.Checked == true)
            {
                if (e.Column.Index == 2)
                {
                    if (double.Parse(Convert.ToDateTime(dgvEmployeeDataGridView[3, e.RowIndex1].Value).ToString("yyyyMMdd")) > double.Parse(Convert.ToDateTime(dgvEmployeeDataGridView[3, e.RowIndex2].Value).ToString("yyyyMMdd")))
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(Convert.ToDateTime(dgvEmployeeDataGridView[3, e.RowIndex1].Value).ToString("yyyyMMdd")) < double.Parse(Convert.ToDateTime(dgvEmployeeDataGridView[3, e.RowIndex2].Value).ToString("yyyyMMdd")))
                        {
                            e.SortResult = -1;
                        }
                        else
                        {
                            e.SortResult = 0;
                        }
                    }

                    e.Handled = true;
                }
            }
        }

        private void dgvDayDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (rbnEmployeeDate.Checked == true)
            {
                if (e.Column.Index == 2)
                {
                    if (double.Parse(dgvDayDataGridView[8, e.RowIndex1].Value.ToString().Replace("-", "")) > double.Parse(dgvDayDataGridView[8, e.RowIndex2].Value.ToString().Replace("-", "")))
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvDayDataGridView[8, e.RowIndex1].Value.ToString().Replace("-", "")) < double.Parse(dgvDayDataGridView[8, e.RowIndex2].Value.ToString().Replace("-", "")))
                        {
                            e.SortResult = -1;
                        }
                        else
                        {
                            e.SortResult = 0;
                        }
                    }

                    e.Handled = true;
                }
            }
        }

        private void RePaint_SpreadSheet_Indicators(DataGridView myDataGridView, DataView myDataView)
        {
            int intTotalTimeMinutes = 0;
            bool blnError = false;
            string strFieldPrefix = "";

            if (myDataGridView.Name == "dgvBreakDataGridView")
            {
                this.dgvBreakDataGridView.Rows.Clear();
                strFieldPrefix = "BREAK";
                pvtblnBreakInError = false;
            }
            else
            {
                this.dgvTimeSheetDataGridView.Rows.Clear();
                strFieldPrefix = "TIMESHEET";
                pvtblnTimeSheetInError = false;
            }

            bool blnRowExistsWithInAndOutValues = false;

            string strClockTimeIn = "";
            string strActualTimeIn = "";
            string strActualTimeOut = "";
            string strClockTimeOut = "";

            //Check For OverLap Errors
            for (int intRow = 0; intRow < myDataView.Count; intRow++)
            {
                if (myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"] == System.DBNull.Value
                 && myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"] == System.DBNull.Value)
                {
                    myDataView[intRow].Delete();

                    intRow -= 1;

                    continue;
                }

                blnError = false;
                intTotalTimeMinutes += Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_ACCUM_MINUTES"]);

                if (intRow > 0)
                {
                    //Check OVerlap
                    if (myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"] == System.DBNull.Value
                        | myDataView[intRow - 1][strFieldPrefix + "_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                    }
                    else
                    {
                        if (Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"]) < Convert.ToInt32(myDataView[intRow - 1][strFieldPrefix + "_TIME_OUT_MINUTES"]))
                        {
                            //Current Row
                            blnError = true;
                        }
                    }
                }

                if (myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"] == System.DBNull.Value
                    | myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"] == System.DBNull.Value)
                {
                    if (myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"] == System.DBNull.Value
                    & myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                    }
                    else
                    {
                        //Error
                        blnError = true;
                    }
                }
                else
                {
                    blnRowExistsWithInAndOutValues = true;

                    if (Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"]) >= Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"]))
                    {
                        blnError = true;
                    }
                }

                if (myDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] == System.DBNull.Value)
                {
                    strClockTimeIn = "";
                }
                else
                {
                    if (Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) > 59)
                    {
                        strClockTimeIn = Convert.ToInt32(Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) / 60).ToString()
                                                                + Convert.ToInt32(Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockTimeIn = Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_IN_MINUTES"]).ToString();
                    }
                }

                if (myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"]) > 59)
                    {
                        strActualTimeIn = Convert.ToInt32(Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"]) / 60).ToString()
                                                                + Convert.ToInt32(Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualTimeIn = Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_IN_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualTimeIn = "";
                }

                if (myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"] != System.DBNull.Value)
                {
                    if (Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"]) > 59)
                    {
                        strActualTimeOut = Convert.ToInt32(Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"]) / 60).ToString()
                                                                + Convert.ToInt32(Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strActualTimeOut = Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_TIME_OUT_MINUTES"]).ToString();
                    }
                }
                else
                {
                    strActualTimeOut = "";
                }

                if (myDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] == System.DBNull.Value)
                {
                    strClockTimeOut = "";
                }
                else
                {
                    if (Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) > 59)
                    {
                        strClockTimeOut = Convert.ToInt32(Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) / 60).ToString()
                                                                + Convert.ToInt32(Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]) % 60).ToString("00");
                    }
                    else
                    {
                        strClockTimeOut = Convert.ToInt32(myDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"]).ToString();
                    }
                }

                myDataGridView.Rows.Add("",
                                        strClockTimeIn,
                                        strActualTimeIn,
                                        strActualTimeOut,
                                        strClockTimeOut,
                                        Convert.ToInt32(Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_ACCUM_MINUTES"]) / 60).ToString() + ":"
                                        + Convert.ToInt32(Convert.ToInt32(myDataView[intRow][strFieldPrefix + "_ACCUM_MINUTES"]) % 60).ToString("00"),
                                        myDataView[intRow][strFieldPrefix + "_SEQ"].ToString());

                if (blnError == true)
                {
                    myDataGridView[0,intRow].Style = ErrorDataGridViewCellStyle;

                    myDataView[intRow]["INDICATOR"] = "X";

                    if (myDataGridView.Name == "dgvBreakDataGridView")
                    {
                        this.pvtblnBreakInError = true;
                    }
                    else
                    {
                        this.pvtblnTimeSheetInError = true;
                    }
                }
                else
                {
                    myDataGridView[0,intRow].Style = NormalDataGridViewCellStyle;

                    myDataView[intRow]["INDICATOR"] = "";
                }
            }

            if (myDataGridView.Name == "dgvBreakDataGridView")
            {
                pvtintTotalBreakMinutes = intTotalTimeMinutes;
                this.dgvBreakTotalsDataGridView[2, 0].Value = Convert.ToInt32(intTotalTimeMinutes / 60).ToString() + ":" + Convert.ToInt32(intTotalTimeMinutes % 60).ToString("00");
            }
            else
            {
                pvtintTotalTimeSheetMinutes = intTotalTimeMinutes;
            }

            int intBreakMinutesDefault = 0;

            int intDayPaidMinutes = Set_Break_Value(ref intBreakMinutesDefault);
            string strIndicator = "";

            if (pvtDayTotalDataView.Count == 0)
            {
                //No Record because all Related Tables were Empty
                Add_DayTotal_Record();
            }
            else
            {
                if (pvtDayTotalDataView.Count > 1)
                {
                    CustomClientMessageBox.Show("Stop There is an Error.\nSpeak to Administrator.",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }

            pvtDayTotalDataView[0]["DAY_PAID_MINUTES"] = intDayPaidMinutes;
            pvtDayTotalDataView[0]["BREAK_ACCUM_MINUTES"] = pvtintTotalBreakMinutes;

            if (this.rbnEmployeeDate.Checked == true)
            {
                int pvtintNameColDayDataGridView = 4;
                int pvtintExceptionColDayDataGridView = 5;

                this.dgvDayDataGridView[pvtintNameColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value = Convert.ToInt32(intDayPaidMinutes / 60).ToString() + ":" + Convert.ToInt32(intDayPaidMinutes % 60).ToString("00");
                this.dgvDayDataGridView[pvtintExceptionColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value = Convert.ToInt32(pvtintTotalBreakMinutes / 60).ToString() + ":" + Convert.ToInt32(pvtintTotalBreakMinutes % 60).ToString("00");
            }
            else
            {
                int pvtintTotalHoursColDayDataGridView = 6;
                int pvtintBreakHoursColDayDataGridView = 7;
                
                this.dgvDayDataGridView[pvtintTotalHoursColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value = Convert.ToInt32(intDayPaidMinutes / 60).ToString() + ":" + Convert.ToInt32(intDayPaidMinutes % 60).ToString("00");
                this.dgvDayDataGridView[pvtintBreakHoursColDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value = Convert.ToInt32(pvtintTotalBreakMinutes / 60).ToString() + ":" + Convert.ToInt32(pvtintTotalBreakMinutes % 60).ToString("00");
            }

            if ((this.pvtblnBreakInError == true
            | this.pvtblnTimeSheetInError == true)
            | pvtintTotalTimeSheetMinutes < pvtintTotalBreakMinutes)
            {
                strIndicator = "X";
            }
            else
            {
                if (intDayPaidMinutes == 0
                & pvtintTotalTimeSheetMinutes == 0
                & pvtintTotalBreakMinutes == 0)
                {
                    if (blnRowExistsWithInAndOutValues == true)
                    {
                        //Row Where IN and Out have same Value (therefore Accum = 0)
                        strIndicator = "E";
                    }
                    else
                    {
                        //Remove DayTotal Record So That we can Get Blank again
                        pvtDayTotalDataView[0].Delete();

                        //this.dgvDayDataGridView.Rows[this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].HeaderCell.Style = NoRecordDataGridViewCellStyle;
                        this.dgvDayDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = NoRecordDataGridViewCellStyle;

                        this.dgvDayDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = NormalDataGridViewCellStyle;

                        goto RePaint_SpreadSheet_Indicators_Continue;
                    }
                }
                else
                {
                    if (Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_BELOW_MINUTES"]) == 0)
                    {
                        strIndicator = "E";
                    }
                    else
                    {
                        if (intDayPaidMinutes < Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_BELOW_MINUTES"])
                            | intDayPaidMinutes > Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["EXCEPTION_" + pvtDateTime.ToString("ddd").ToUpper() + "_ABOVE_MINUTES"]))
                        {
                            if (intDayPaidMinutes != 0
                                | (pvtintTotalTimeSheetMinutes > 0
                                & intDayPaidMinutes == 0))
                            {
                                strIndicator = "E";
                            }
                        }
                    }
                }
            }

            //Set Colour For Day Spreadsheet
            pvtDayTotalDataView[0]["INDICATOR"] = strIndicator;

            if (strIndicator == "X")
            {
                this.dgvDayDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = ErrorDataGridViewCellStyle;
            }
            else
            {
                if (strIndicator == "E")
                {
                    this.dgvDayDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = ExceptionDataGridViewCellStyle;
                }
                else
                {
                    if (strIndicator == "B")
                    {
                        this.dgvDayDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = NoRecordDataGridViewCellStyle;
                    }
                    else
                    {
                        if (strIndicator == "")
                        {
                            this.dgvDayDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = NormalDataGridViewCellStyle;
                        }
                    }
                }
            }

            if (pvtintTotalBreakMinutes > intBreakMinutesDefault)
            {
                pvtDayTotalDataView[0]["BREAK_INDICATOR"] = "Y";

                this.dgvDayDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = BreakExceptionDataGridViewCellStyle;
            }
            else
            {
                pvtDayTotalDataView[0]["BREAK_INDICATOR"] = "";

                this.dgvDayDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Style = NormalDataGridViewCellStyle;
            }

        RePaint_SpreadSheet_Indicators_Continue:

            //Set Colour For Employee Spreadsheet
            if (rbnEmployeeDate.Checked == true)
            {
                pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                    pvtstrPayCategoryFilter + " AND EMPLOYEE_NO = " + this.pvtintEmployeeNo.ToString() + " " + pvtstrDataAndTypeFilter + pvtstrCategoryType,
                    "INDICATOR DESC",
                    DataViewRowState.CurrentRows);
            }
            else
            {
                pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
                    pvtstrPayCategoryFilter + " AND DAY_DATE = '" + this.pvtDateTime.ToString("yyyy-MM-dd") + "' " + pvtstrDataAndTypeFilter + pvtstrCategoryType,
                    "INDICATOR DESC",
                        DataViewRowState.CurrentRows);
            }

            if (pvtTempDataView.Count > 0)
            {
                if (pvtTempDataView[0]["INDICATOR"].ToString() == "X")
                {
                    this.dgvEmployeeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = ErrorDataGridViewCellStyle;
                }
                else
                {
                    if (pvtTempDataView[0]["INDICATOR"].ToString() == "E")
                    {
                        this.dgvEmployeeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = ExceptionDataGridViewCellStyle;
                    }
                    else
                    {
                        if (pvtTempDataView[0]["INDICATOR"].ToString() == "B")
                        {
                            this.dgvEmployeeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = NoRecordDataGridViewCellStyle;
                        }
                        else
                        {
                            if (pvtTempDataView[0]["INDICATOR"].ToString() == "")
                            {
                                this.dgvEmployeeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = NormalDataGridViewCellStyle;
                            }
                        }
                    }
                }

                pvtTempDataView.Sort = "BREAK_INDICATOR DESC";

                if (pvtTempDataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                {
                    dgvEmployeeDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = BreakExceptionDataGridViewCellStyle;
                }
                else
                {
                    dgvEmployeeDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = NormalDataGridViewCellStyle;
                }
            }
            else
            {
                this.dgvEmployeeDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = NoRecordDataGridViewCellStyle;

                dgvEmployeeDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView)].Style = NormalDataGridViewCellStyle;
            }

            //Set Colour For PayCategory Spreadsheet
            pvtTempDataView = new DataView(this.pvtDataSet.Tables["DayTotal"],
            pvtstrPayCategoryFilter + pvtstrDataAndTypeFilter + pvtstrCategoryType,
                    "INDICATOR DESC",
                    DataViewRowState.CurrentRows);

            if (pvtTempDataView.Count > 0)
            {
                if (pvtTempDataView[0]["INDICATOR"].ToString() == "X")
                {
                    this.dgvPayCategoryDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = ErrorDataGridViewCellStyle;
                }
                else
                {
                    if (pvtTempDataView[0]["INDICATOR"].ToString() == "E")
                    {
                        this.dgvPayCategoryDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = ExceptionDataGridViewCellStyle;
                    }
                    else
                    {
                        if (pvtTempDataView[0]["INDICATOR"].ToString() == "B")
                        {
                            this.dgvPayCategoryDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NoRecordDataGridViewCellStyle;
                        }
                        else
                        {
                            if (pvtTempDataView[0]["INDICATOR"].ToString() == "")
                            {
                                this.dgvPayCategoryDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NormalDataGridViewCellStyle;
                            }
                        }
                    }
                }

                pvtTempDataView.Sort = "BREAK_INDICATOR DESC";

                if (pvtTempDataView[0]["BREAK_INDICATOR"].ToString() == "Y")
                {
                    this.dgvPayCategoryDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = BreakExceptionDataGridViewCellStyle;
                }
                else
                {
                    this.dgvPayCategoryDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NormalDataGridViewCellStyle;
                }
            }
            else
            {
                this.dgvPayCategoryDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NoRecordDataGridViewCellStyle;

                this.dgvPayCategoryDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView)].Style = NormalDataGridViewCellStyle;
            }

            if (myDataGridView.Name == "dgvBreakDataGridView")
            {
                this.Check_To_Add_New_Break_Row();
            }
            else
            {
                this.Check_To_Add_New_TimeSheet_Row();
            }
        }

        private void dgvPayCategoryDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 3)
            {
                if (dgvPayCategoryDataGridView[5, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvPayCategoryDataGridView[5, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvPayCategoryDataGridView[5, e.RowIndex1].Value.ToString()) > double.Parse(dgvPayCategoryDataGridView[5, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvPayCategoryDataGridView[5, e.RowIndex1].Value.ToString()) < double.Parse(dgvPayCategoryDataGridView[5, e.RowIndex2].Value.ToString()))
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

        private void TimeSheet_Or_Break_DataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView myDataGridView = (DataGridView)sender;

            if (myDataGridView.Name == "dgvBreakDataGridView")
            {
                if (this.pvtblnBreakDataGridViewLoaded == false)
                {
                    return;
                }
            }
            else
            {
                if (this.pvtblnTimeSheetDataGridViewLoaded == false)
                {
                    return;
                }
            }
            
            if (e.ColumnIndex < pvtintClockInSetColTimeSheetOrBreakDataGridView)
            {
                SendKeys.Send("{Right}");
            }
            else
            {
                if (e.ColumnIndex > pvtintClockOutSetColTimeSheetOrBreakDataGridView)
                {
                    SendKeys.Send("{Left}");
                }
            }
        }
    }
}
