using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InteractPayroll
{
    public partial class frmRoster : Form
    {
        clsISUtilities clsISUtilities;
        frmRptRoster frmRptRoster;

        private DataSet pvtDataSet;
        private DataView pvtEmployeeDataView;
        private DataView pvtPayCategoryDataView;
        
        private DataView pvtShiftScheduleDataView;
        private DataView pvtEmployeeShiftScheduleDataView;
        private DataView pvtPublicHolidayDataView;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private string pvtstrPayrollType = "";
        
        int pvtintDelCol = 0;
        int pvtintDateCol = 1;
        int pvtintNamesCol = 2;
        int pvtintPositionCol = 3;
        int pvtintFromHHMMCol = 4;
        int pvtintToHHMMCol = 5;
        int pvtintTotalCol = 6;
        int pvtintDay0Col = 7;
        int pvtintDay24Col = 55;
        int pvtintDateYYYYMMDDCol = 55;
        int pvtintEmployeeNoCol = 56;
       
        private int pvtintPayCategoryNo = -1;
        private int pvtintPayCategoryShiftScheduleNo = -1;

        private int pvtintPayCategoryDataGridViewRowIndex = -1;
        private int pvtintPayCategoryShiftScheduleDataGridViewRowIndex = -1;
        private int pvtintShiftScheduleDataGridViewRowIndex = -1;

        private byte[] pvtbytCompress;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;

        private bool pvtblnPayCategoryShiftScheduleDataGridViewLoaded = false;

        DataGridViewCellStyle ErrorDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;
        DataGridViewCellStyle PublicHolidayDataGridViewCellStyle;
        DataGridViewCellStyle WeekEndDataGridViewCellStyle;
    
        public frmRoster()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 95;

                this.dgvShiftScheduleDataGridView.Height += 95;
            }
        }

        private void frmRoster_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busRoster");

                ErrorDataGridViewCellStyle = new DataGridViewCellStyle();
                ErrorDataGridViewCellStyle.BackColor = Color.Red;
                ErrorDataGridViewCellStyle.SelectionBackColor = Color.Red;

                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;

                PublicHolidayDataGridViewCellStyle = new DataGridViewCellStyle();
                PublicHolidayDataGridViewCellStyle.BackColor = Color.SlateBlue;
                PublicHolidayDataGridViewCellStyle.SelectionBackColor = Color.SlateBlue;

                WeekEndDataGridViewCellStyle = new DataGridViewCellStyle();
                WeekEndDataGridViewCellStyle.BackColor = SystemColors.Info;

                this.lblPayrollTypeSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblCostCentreSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblRosterSchedule.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeRosterSchedule.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add("Time Attendance");
                }
                else
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add("Wages");
                    this.dgvPayrollTypeDataGridView.Rows.Add("Salaries");
                }

                pvtblnPayrollTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);

                string strTimeAttendInd = "N";

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    strTimeAttendInd = "Y";
                }

                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = strTimeAttendInd;
                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[3] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                Load_CurrentForm_Records();
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
                    case "dgvPayCategoryShiftScheduleDataGridView":

                        pvtintPayCategoryShiftScheduleDataGridViewRowIndex = -1;
                        this.dgvPayCategoryShiftScheduleDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvShiftScheduleDataGridView":

                        pvtintShiftScheduleDataGridViewRowIndex = -1;
                        this.dgvShiftScheduleDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayrollTypeDataGridView":

                        pvtintPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        pvtintPayCategoryDataGridViewRowIndex = -1;
                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;
                    default:

                        MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                if (myDataGridView.Name == "dgvPayCategoryDataGridView")
                {
                    pvtintPayCategoryDataGridViewRowIndex = -1;
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

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    pvtstrPayrollType = dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                    if (pvtDataSet != null)
                    {
                        Load_CurrentForm_Records();
                    }
                }
            }
        }

        private void Load_CurrentForm_Records()
        {
            this.Clear_DataGridView(this.dgvPayCategoryShiftScheduleDataGridView);
            this.Clear_DataGridView(this.dgvShiftScheduleDataGridView);

            pvtblnPayCategoryShiftScheduleDataGridViewLoaded = false;

            pvtShiftScheduleDataView = null;
            pvtShiftScheduleDataView = new DataView(pvtDataSet.Tables["ShiftSchedule"],
                    "PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                    "",
                    DataViewRowState.CurrentRows);

            string strLocked = "";

            for (int intRow = 0; intRow < pvtShiftScheduleDataView.Count; intRow++)
            {
                if (Convert.ToBoolean(pvtShiftScheduleDataView[intRow]["LOCKED_IND"]) == true)
                {
                    strLocked = "Y";
                }
                else
                {
                    strLocked = "N";
                }

                this.dgvPayCategoryShiftScheduleDataGridView.Rows.Add(Convert.ToDateTime(pvtShiftScheduleDataView[intRow]["FROM_DATETIME"]).ToString("dd MMMM yyyy - ddd"),
                                                                      Convert.ToDateTime(pvtShiftScheduleDataView[intRow]["TO_DATETIME"]).ToString("dd MMMM yyyy - ddd"),
                                                                      strLocked,
                                                                      intRow.ToString());
            }

            pvtblnPayCategoryShiftScheduleDataGridViewLoaded = true;

            Load_PayCategory_SpreadSheet();
        }


        private void Load_PayCategory_SpreadSheet()
        {
            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);

            pvtPayCategoryDataView = null;

            string strFilter = "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_TYPE = '" + this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1) + "'";

            pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                strFilter,
                "PAY_CATEGORY_DESC",
                DataViewRowState.CurrentRows);

            if (pvtPayCategoryDataView.Count == 0)
            {
                Set_Form_For_Read();
            }
            else
            {
                int intSelectedRow = 0;

                Set_Form_For_Read();

                this.pvtblnPayCategoryDataGridViewLoaded = false;

                for (int intRowCount = 0; intRowCount < pvtPayCategoryDataView.Count; intRowCount++)
                {
                    this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString(),
                                                             intRowCount.ToString());


                    if (Convert.ToInt32(pvtPayCategoryDataView[intRowCount]["PAY_CATEGORY_NO"]) == pvtintPayCategoryNo)
                    {
                        intSelectedRow = intRowCount;
                    }
                }

                this.pvtblnPayCategoryDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, intSelectedRow);
            }
        }

        private void Set_Form_For_Read()
        {
            if (this.Text.IndexOf("- New") > -1)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf("- New") - 1);
            }
            else
            {
                if (this.Text.IndexOf("- Update") > -1)
                {
                    this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);
                }
            }
                        
            this.btnUpdate.Enabled = true;
            this.btnPrint.Enabled = true;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.dgvPayCategoryDataGridView.Enabled = true;
            this.dgvPayrollTypeDataGridView.Enabled = true;
            this.dgvPayCategoryShiftScheduleDataGridView.Enabled = true;

            this.dgvShiftScheduleDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

            this.picPayCategoryLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;
            this.picRosterScheduleLock.Visible = false;
            
            if (pvtPayCategoryDataView.Count == 0)
            {
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }
        }

        private void Set_Form_For_Edit()
        {
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;
            this.btnPrint.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
       
            this.dgvPayCategoryDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;
            this.dgvPayCategoryShiftScheduleDataGridView.Enabled = false;

            this.dgvShiftScheduleDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

            this.picPayCategoryLock.Visible = true;
            this.picPayrollTypeLock.Visible = true;
            this.picRosterScheduleLock.Visible = true;
        }

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    clsISUtilities.DataViewIndex = Convert.ToInt32(this.dgvPayCategoryDataGridView[1, e.RowIndex].Value);
                    pvtintPayCategoryNo = Convert.ToInt32(pvtPayCategoryDataView[clsISUtilities.DataViewIndex]["PAY_CATEGORY_NO"]);
                    
                    this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryShiftScheduleDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryShiftScheduleDataGridView));
                }
            }
        }

        private void dgvPayCategoryShiftScheduleDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnPayCategoryShiftScheduleDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryShiftScheduleDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryShiftScheduleDataGridViewRowIndex = Convert.ToInt32(this.dgvPayCategoryShiftScheduleDataGridView[3, e.RowIndex].Value);

                    pvtintPayCategoryShiftScheduleNo = Convert.ToInt32(pvtShiftScheduleDataView[pvtintPayCategoryShiftScheduleDataGridViewRowIndex]["PAY_CATEGORY_SHIFT_SCHEDULE_NO"]);

                    pvtEmployeeShiftScheduleDataView = null;
                    pvtEmployeeShiftScheduleDataView = new DataView(this.pvtDataSet.Tables["EmployeeShiftSchedule"],
                        "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_SHIFT_SCHEDULE_NO = " + pvtintPayCategoryShiftScheduleNo,
                        "SHIFT_SCHEDULE_DATETIME,EMPLOYEE_NO",
                        DataViewRowState.CurrentRows);

                    pvtEmployeeDataView = null;
                    pvtEmployeeDataView = new DataView(this.pvtDataSet.Tables["Employee"],
                        "COMPANY_NO = " + Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo")) + " AND PAY_CATEGORY_NO = " + this.pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "EMPLOYEE_NO",
                        DataViewRowState.CurrentRows);

                    DateTime dtFromDate = Convert.ToDateTime(this.pvtShiftScheduleDataView[pvtintPayCategoryShiftScheduleDataGridViewRowIndex]["FROM_DATETIME"]);
                    DateTime dtToDate = Convert.ToDateTime(this.pvtShiftScheduleDataView[pvtintPayCategoryShiftScheduleDataGridViewRowIndex]["TO_DATETIME"]);
                    string strDate = "";
                    string strFromHHMM = "";
                    string strToHHMM = "";
                    string strTotalHHMM = "";

                    if (Convert.ToBoolean(this.pvtShiftScheduleDataView[pvtintPayCategoryShiftScheduleDataGridViewRowIndex]["LOCKED_IND"]) == false)
                    {
                        this.btnUpdate.Enabled = true;
                    }
                    else
                    {
                        this.btnUpdate.Enabled = false;
                    }

                    int intFindRow = -1;
                    object[] searchKeys = new object[2];
                    bool blnPublicHoliday = false;
                    
                    this.Clear_DataGridView(dgvShiftScheduleDataGridView);

                    while (dtFromDate <= dtToDate)
                    {
                        blnPublicHoliday = false;

                        pvtPublicHolidayDataView = null;
                        pvtPublicHolidayDataView = new DataView(this.pvtDataSet.Tables["PublicHoliday"],
                            "PUBLIC_HOLIDAY_DATE = '" + dtFromDate.ToString("yyyy-MM-dd") + "'",
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtPublicHolidayDataView.Count > 0)
                        {
                            blnPublicHoliday = true;
                        }

                        strDate = dtFromDate.ToString("dd MMM - ddd");

                        for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                        {
                            searchKeys[0] = dtFromDate;
                            searchKeys[1] = pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString();

                            intFindRow = pvtEmployeeShiftScheduleDataView.Find(searchKeys);

                            if (intFindRow > -1)
                            {
                                strFromHHMM = Convert.ToInt32(Convert.ToInt32(pvtEmployeeShiftScheduleDataView[intFindRow]["FROM_HHMM_MINUTES"]) / 60).ToString("00") + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeShiftScheduleDataView[intFindRow]["FROM_HHMM_MINUTES"]) % 60).ToString("00");
                                strToHHMM = Convert.ToInt32(Convert.ToInt32(pvtEmployeeShiftScheduleDataView[intFindRow]["TO_HHMM_MINUTES"]) / 60).ToString("00") + ":" + Convert.ToInt32(Convert.ToInt32(pvtEmployeeShiftScheduleDataView[intFindRow]["TO_HHMM_MINUTES"]) % 60).ToString("00");

                                int intTotalMinutes = Convert.ToInt32(pvtEmployeeShiftScheduleDataView[intFindRow]["TO_HHMM_MINUTES"]) - Convert.ToInt32(pvtEmployeeShiftScheduleDataView[intFindRow]["FROM_HHMM_MINUTES"]);
                                strTotalHHMM = (intTotalMinutes / 60).ToString() + ":" + (intTotalMinutes % 60).ToString("00");
                            }
                            else
                            {
                                strFromHHMM = "";
                                strToHHMM = "";
                                strTotalHHMM = "";
                            }

                            this.dgvShiftScheduleDataGridView.Rows.Add("Del",
                                                                        strDate,
                                                                        pvtEmployeeDataView[intRow]["EMPLOYEE_NAMES"].ToString(),
                                                                        pvtEmployeeDataView[intRow]["OCCUPATION_DESC"].ToString(),
                                                                        strFromHHMM,
                                                                        strToHHMM,
                                                                        strTotalHHMM,
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        dtFromDate.ToString("yyyy-MM-dd"),
                                                                        pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString());

                            if (intFindRow > -1)
                            {
                                int intFromHH = Convert.ToInt32(pvtEmployeeShiftScheduleDataView[intFindRow]["FROM_HHMM_MINUTES"]) / 60;
                                int intToHH = Convert.ToInt32(pvtEmployeeShiftScheduleDataView[intFindRow]["TO_HHMM_MINUTES"]) / 60;

                                bool blnStarted = false;

                                for (int intCol = pvtintDay0Col; intCol <= pvtintDay24Col; intCol++)
                                {
                                    if (blnStarted == false)
                                    {
                                        if (this.dgvShiftScheduleDataGridView.Columns[intCol].HeaderText == strFromHHMM.ToString())
                                        {
                                            dgvShiftScheduleDataGridView[intCol, dgvShiftScheduleDataGridView.Rows.Count - 1].Style.BackColor = SystemColors.ControlDark;
                                            blnStarted = true;
                                        }
                                    }
                                    else
                                    {
                                        if (this.dgvShiftScheduleDataGridView.Columns[intCol].HeaderText == strToHHMM.ToString())
                                        {
                                            //dgvShiftScheduleDataGridView[intCol, dgvShiftScheduleDataGridView.Rows.Count - 1].Style.BackColor = SystemColors.ControlDark;
                                            break;
                                        }
                                        else
                                        {
                                            dgvShiftScheduleDataGridView[intCol, dgvShiftScheduleDataGridView.Rows.Count - 1].Style.BackColor = SystemColors.ControlDark;
                                        }
                                    }
                                }
                            }

                            if (blnPublicHoliday == true)
                            {
                                this.dgvShiftScheduleDataGridView.Rows[this.dgvShiftScheduleDataGridView.Rows.Count - 1].HeaderCell.Style = PublicHolidayDataGridViewCellStyle;
                            }


                            if (Convert.ToInt32(dtFromDate.DayOfWeek) == 6
                            || Convert.ToInt32(dtFromDate.DayOfWeek) == 0)
                            {
                                dgvShiftScheduleDataGridView[pvtintDelCol, dgvShiftScheduleDataGridView.Rows.Count - 1].Style = WeekEndDataGridViewCellStyle;
                                dgvShiftScheduleDataGridView[pvtintDateCol, dgvShiftScheduleDataGridView.Rows.Count - 1].Style = WeekEndDataGridViewCellStyle;
                                dgvShiftScheduleDataGridView[pvtintNamesCol, dgvShiftScheduleDataGridView.Rows.Count - 1].Style = WeekEndDataGridViewCellStyle;
                                dgvShiftScheduleDataGridView[pvtintPositionCol, dgvShiftScheduleDataGridView.Rows.Count - 1].Style = WeekEndDataGridViewCellStyle;
                                dgvShiftScheduleDataGridView[pvtintTotalCol, dgvShiftScheduleDataGridView.Rows.Count - 1].Style = WeekEndDataGridViewCellStyle;
                            }

                            strDate = "";
                        }

                        dtFromDate = dtFromDate.AddDays(1);
                    }

                    if (dgvShiftScheduleDataGridView.Rows.Count > 0)
                    {
                        //Set Current Cell off Page
                        //dgvShiftScheduleDataGridView.CurrentCell = this.dgvShiftScheduleDataGridView[pvtintColourCol, 0];
                        dgvShiftScheduleDataGridView.ClearSelection();
                    }
                }
            }
        }

        private void dgvShiftScheduleDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.pvtDataSet.RejectChanges();

            Set_Form_For_Read();
            
            this.Set_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView));
        }
      
        private void dgvShiftScheduleDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (e.RowIndex > -1
                && e.ColumnIndex == pvtintDelCol)
                {
                    dgvShiftScheduleDataGridView[pvtintFromHHMMCol, e.RowIndex].Value = "";
                    dgvShiftScheduleDataGridView[pvtintToHHMMCol, e.RowIndex].Value = "";
                    dgvShiftScheduleDataGridView[pvtintTotalCol, e.RowIndex].Value = "";
                    
                    //Clear
                    for (int intCol = pvtintDay0Col; intCol <= pvtintDay24Col; intCol++)
                    {
                        dgvShiftScheduleDataGridView[intCol, e.RowIndex].Style.BackColor = Color.White;
                    }

                    this.dgvShiftScheduleDataGridView.Rows[e.RowIndex].HeaderCell.Style = NormalDataGridViewCellStyle;
                }
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = pvtstrPayrollType;
                objParm[2] = pvtintPayCategoryNo;
                objParm[3] = pvtintPayCategoryShiftScheduleNo;

                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Print_Report", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                frmRptRoster = new frmRptRoster(pvtDataSet);

                frmRptRoster.ShowDialog();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Tables to Be Moved to Business Layer
                DataSet TempDataSet = new DataSet();

                DataTable myDataTable = this.pvtDataSet.Tables["EmployeeShiftSchedule"].Clone();

                //Check Roster
                for (int intRow = 0; intRow < dgvShiftScheduleDataGridView.Rows.Count; intRow++)
                {
                    if (this.dgvShiftScheduleDataGridView.Rows[intRow].HeaderCell.Style == ErrorDataGridViewCellStyle)
                    {
                        //Set Cell
                        if (dgvShiftScheduleDataGridView[pvtintNamesCol, intRow].Style.BackColor.ToArgb() == Color.White.ToArgb())
                        {
                            MessageBox.Show("Background Color is White.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                            return;
                        }

                        MessageBox.Show("Error Exists in Roster.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return;
                    }
                    else
                    {
                        if (dgvShiftScheduleDataGridView[this.pvtintFromHHMMCol, intRow].Value.ToString().Trim() != "")
                        {
                            DataRow myDataRow = myDataTable.NewRow();

                            myDataRow["COMPANY_NO"] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("CompanyNo"));
                            myDataRow["EMPLOYEE_NO"] = dgvShiftScheduleDataGridView[this.pvtintEmployeeNoCol, intRow].Value.ToString();
                            myDataRow["PAY_CATEGORY_NO"] = pvtintPayCategoryNo;
                            myDataRow["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                            myDataRow["PAY_CATEGORY_SHIFT_SCHEDULE_NO"] = pvtintPayCategoryShiftScheduleNo;
                            myDataRow["SHIFT_SCHEDULE_DATETIME"] = dgvShiftScheduleDataGridView[this.pvtintDateYYYYMMDDCol, intRow].Value.ToString();
                            myDataRow["FROM_HHMM"] = dgvShiftScheduleDataGridView[this.pvtintFromHHMMCol, intRow].Value.ToString();
                            myDataRow["TO_HHMM"] = dgvShiftScheduleDataGridView[this.pvtintToHHMMCol, intRow].Value.ToString();
                      
                            myDataTable.Rows.Add(myDataRow);
                        }
                    }
                }

                for (int intRow = 0; intRow < pvtEmployeeShiftScheduleDataView.Count; intRow++)
                {
                    pvtEmployeeShiftScheduleDataView[intRow].Delete();
                    intRow -= 1;
                }
             
                TempDataSet.Tables.Add(myDataTable);
                
                //Compress DataSet
                pvtbytCompress = clsISUtilities.Compress_DataSet(TempDataSet);
                
                object[] objParm = new object[4];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = pvtbytCompress;
                objParm[3] = this.dgvPayrollTypeDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView)].Value.ToString().Substring(0, 1);
                
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Record", objParm, true);

                TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                pvtDataSet.Merge(TempDataSet);
                
                this.pvtDataSet.AcceptChanges();

                Load_PayCategory_SpreadSheet();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
    }
}
