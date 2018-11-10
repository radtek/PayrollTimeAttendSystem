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
    public partial class frmTimeSheetBatchClient : Form
    {
        clsISClientUtilities clsISClientUtilities;

        ToolStripMenuItem miLinkedMenuItem;

        private Int64 pvtintCompanyNo = -1;
        private int pvtintPayCategoryNo;
        private int pvtintPayCategoryTableRowNo;

        private string pvtstrTableDef = "TIMESHEET";
        private string pvtstrTableName = "Timesheet";

        private DateTime pvtdtDayDateTime;

        private DataSet pvtDataSet;

        private DataView pvtPayCategoryDataView;
        private DataView pvtEmployeeDataView;
        private DataView pvtTempDataView;
        private DataView pvtEmployeeRejectedDataView;
        private DataView pvtTimesheetOrBreakDataView;

        private int pvtintTimerCount = 0;

        private bool pvtblnAddAll = false;

        private DateTime pvtdtDateTimeTo;

        private bool pvtblnCompanyDataGridViewLoaded = false;
        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnDayDataGridViewLoaded = false;
        private bool pvtblnDayChosenDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;
        private bool pvtblnEmployeeChosenDataGridViewLoaded = true;

        private int pvtintCompanyDataGridViewRowIndex = -1;
        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;
        private int pvtintDayDataGridViewRowIndex = -1;
        private int pvtintEmployeeDataGridViewRowIndex = -1;
        private int pvtintEmployeeChosenDataGridViewRowIndex = -1;
        private int pvtintDayChosenDataGridViewRowIndex = -1;

        private string pvtstrPayrollType = "";

        DataGridViewCellStyle RejectedDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;

        public frmTimeSheetBatchClient()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 95;

                this.dgvPayCategoryDataGridView.Height += 57;

                this.lblEmployee.Top += 55;
                this.dgvEmployeeDataGridView.Top += 55;
                this.dgvEmployeeDataGridView.Height += 38;

                this.lblEmployeeRejectedName.Top += 95;
                this.dgvEmployeeRejectedDataGridView.Top += 95;

                this.lblSelectedEmployee.Top = this.lblEmployee.Top;
                this.dgvEmployeeChosenDataGridView.Top = this.dgvEmployeeDataGridView.Top;
                this.dgvEmployeeChosenDataGridView.Height = this.dgvEmployeeDataGridView.Height;

                this.lblEmployeeChosenRejectedName.Top += 95;
                this.dgvEmployeeChosenRejectedDataGridView.Top += 95;

                this.btnAdd.Top += 55;
                this.btnAddAll.Top += 55;
                this.btnRemove.Top += 55;
                this.btnRemoveAll.Top += 55;

                this.grbOption.Top += 19;
                this.grbRowLegend.Top += 19;
                this.grbSelection.Top += 19;
                this.grbDateException.Top += 19;

                this.dgvDayDataGridView.Height += 19;
                this.dgvDayChosenDataGridView.Height += 19;
            }
        }

        private void frmTimeSheetBatchClient_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISClientUtilities = new clsISClientUtilities(this, "busTimeSheetBatchClient");

                RejectedDataGridViewCellStyle = new DataGridViewCellStyle();
                RejectedDataGridViewCellStyle.BackColor = Color.Yellow;
                RejectedDataGridViewCellStyle.SelectionBackColor = Color.Yellow;

                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;

                this.lblCompany.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblSelectedEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblDate.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblChosenDates.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                
                this.lblEmployeeRejectedName.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblEmployeeChosenRejectedName.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

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

                    //Select First Row
                    if (dgvPayrollTypeDataGridView.Rows.Count > 0)
                    {
                        Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);
                    }

                    this.pvtdtDayDateTime = DateTime.Now;

                    Load_CurrentForm_Records();
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void Load_CurrentForm_Records()
        {
            int intCompanyRow = 0;

            Clear_DataGridView(this.dgvCompanyDataGridView);
            Clear_DataGridView(this.dgvPayCategoryDataGridView);
            Clear_DataGridView(this.dgvDayDataGridView);

            this.btnDelete.Enabled = false;
            this.btnUpdate.Enabled = false;

            if (this.dgvCompanyDataGridView.Rows.Count > 0)
            {
                intCompanyRow = this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView);
            }

            this.pvtblnCompanyDataGridViewLoaded = false;
           
            for (int intRow = 0; intRow < pvtDataSet.Tables["Company"].Rows.Count; intRow++)
            {
                dgvCompanyDataGridView.Rows.Add(pvtDataSet.Tables["Company"].Rows[intRow]["COMPANY_DESC"].ToString(),
                                                intRow.ToString());
            }

            this.pvtblnCompanyDataGridViewLoaded = true;

            if (this.dgvCompanyDataGridView.Rows.Count > 0)
            {
                //Select First Row
                this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, intCompanyRow);
            }
            else
            {
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (this.Text.IndexOf("- Update") > -1)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf("- Update") - 1);
            }

            this.rbnTimesheet.Enabled = true;
            this.rbnBreak.Enabled = true;

            Remove_Selected_Dates();

            grbOption.Text = "Option";

            this.chkUpdate.Checked = false;
            this.chkUpdate.Enabled = false;

            this.tmrOptionTimer.Enabled = false;
            this.lblChooseOption.Visible = false;

            Clear_DataGridView(this.dgvEmployeeDataGridView);
            Clear_DataGridView(this.dgvEmployeeChosenDataGridView);
            Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);

            this.lblEmployeeRejectedName.Text = "";
            this.lblEmployeeChosenRejectedName.Text = "";

            this.dgvPayrollTypeDataGridView.Enabled = true;
            this.dgvCompanyDataGridView.Enabled = true;
            this.dgvPayCategoryDataGridView.Enabled = true;

            this.picCompanyLock.Visible = false;
            this.picPayrollLock.Visible = false;
            this.picPayCategoryLock.Visible = false;

            this.btnDelete.Enabled = true;
            this.btnUpdate.Enabled = true;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.rbnIn.Enabled = false;
            this.rbnOut.Enabled = false;
            this.rbnBoth.Enabled = false;
            this.rbnDeleteRow.Enabled = false;

            this.rbnIn.Checked = false;
            this.rbnOut.Checked = false;
            this.rbnBoth.Checked = false;
            this.rbnDeleteRow.Checked = false;

            this.btnAdd.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;

            this.txtIn.Text = "";
            this.txtOut.Text = "";

            this.txtIn.Enabled = false;
            this.txtOut.Enabled = false;

            pvtDataSet.RejectChanges();

            //if (this.dgvDayDataGridView.Rows.Count > 0)
            //{
            //    Set_DataGridView_SelectedRowIndex(dgvDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView));
            //}

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView));
            }
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                int intReturnCode = 0;
                int intTimeInMinutes = 0;
                int intTimeOutMinutes = 0;

                if (this.rbnIn.Checked == false
                    & this.rbnOut.Checked == false
                    & this.rbnBoth.Checked == false
                    & this.rbnDeleteRow.Checked == false)
                {
                    CustomClientMessageBox.Show("Select an Option.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (this.txtIn.Enabled == true)
                {
                    intReturnCode = Check_Time(this.txtIn.Text);

                    if (intReturnCode != 0)
                    {
                        CustomClientMessageBox.Show("Invalid Start Time.\n\nTime Format = 'hhmm'\nwhere 'hh' = hours (Max 24 hours)\n'mm' = minutes (Max 59 minutes)", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        this.txtIn.Focus();

                        return;
                    }
                    else
                    {
                        intTimeInMinutes = Return_Time_In_Minutes(this.txtIn.Text);
                    }
                }

                if (this.txtOut.Enabled == true)
                {
                    intReturnCode = Check_Time(this.txtOut.Text);

                    if (intReturnCode != 0)
                    {
                        if (intReturnCode != 0)
                        {
                            CustomClientMessageBox.Show("Invalid End Time.\n\nTime Format = 'hhmm'\nwhere 'hh' = hours (Max 24 hours)\n'mm' = minutes (Max 59 minutes)", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        this.txtOut.Focus();

                        return;
                    }
                    else
                    {
                        intTimeOutMinutes = Return_Time_In_Minutes(this.txtOut.Text);
                    }
                }

                if (this.txtIn.Enabled == true
                    & this.txtOut.Enabled == true)
                {
                    if (Convert.ToInt32(this.txtIn.Text) >= Convert.ToInt32(this.txtOut.Text))
                    {
                        CustomClientMessageBox.Show("End Time must be Greater than Start Time.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (this.rbnBoth.Checked == true
                    & this.chkUpdate.Checked == false
                    & this.dgvDayChosenDataGridView.Rows.Count == 0)
                {
                    CustomClientMessageBox.Show("No Dates Selected.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    if (this.dgvDayDataGridView.Rows.Count == 0)
                    {
                        CustomClientMessageBox.Show("There are No Dates for Selection.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (this.dgvEmployeeChosenDataGridView.Rows.Count == 0)
                {
                    if (this.rbnTimesheet.Checked == true)
                    {
                        CustomClientMessageBox.Show("No Employee Timesheets have been Selected.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        CustomClientMessageBox.Show("No Employee Breaks have been Selected.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                string strDates = "";

                if (this.rbnBoth.Checked == true
                && this.chkUpdate.Checked == false)
                {
                    for (int intRow = 0; intRow < this.dgvDayChosenDataGridView.Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            strDates = this.dgvDayChosenDataGridView[1, intRow].Value.ToString().Substring(0, 4) + "-" + this.dgvDayChosenDataGridView[1, intRow].Value.ToString().Substring(4, 2) + "-" + this.dgvDayChosenDataGridView[1, intRow].Value.ToString().Substring(6, 2);
                        }
                        else
                        {
                            strDates += "|" + this.dgvDayChosenDataGridView[1, intRow].Value.ToString().Substring(0, 4) + "-" + this.dgvDayChosenDataGridView[1, intRow].Value.ToString().Substring(4, 2) + "-" + this.dgvDayChosenDataGridView[1, intRow].Value.ToString().Substring(6, 2);
                        }
                    }
                }
                else
                {
                    int intCurrentRow = Get_DataGridView_SelectedRowIndex(dgvDayDataGridView);

                    strDates = this.dgvDayDataGridView[1, intCurrentRow].Value.ToString().Substring(0, 4) + "-" + this.dgvDayDataGridView[1, intCurrentRow].Value.ToString().Substring(4, 2) + "-" + this.dgvDayDataGridView[1, intCurrentRow].Value.ToString().Substring(6, 2);
                }

                string strOption = "I";

                if (this.rbnOut.Checked == true)
                {
                    strOption = "O";
                }
                else
                {
                    if (this.rbnBoth.Checked == true)
                    {
                        strOption = "B";
                    }
                    else
                    {
                        if (this.rbnDeleteRow.Checked == true)
                        {
                            strOption = "D";
                        }
                    }
                }

                string strEmployeeNos = "";
                string strEmployeeRecNos = "";

                for (int intRow = 0; intRow < this.dgvEmployeeChosenDataGridView.Rows.Count; intRow++)
                {
                    if (intRow == 0)
                    {
                        strEmployeeNos = this.dgvEmployeeChosenDataGridView[6, intRow].Value.ToString();
                        strEmployeeRecNos = this.dgvEmployeeChosenDataGridView[7, intRow].Value.ToString();
                    }
                    else
                    {
                        strEmployeeNos += "|" + this.dgvEmployeeChosenDataGridView[6, intRow].Value.ToString();
                        strEmployeeRecNos += "|" + this.dgvEmployeeChosenDataGridView[7, intRow].Value.ToString();
                    }
                }

                string strUpdateInd = "N";

                if (this.chkUpdate.Checked == true)
                {
                    strUpdateInd = "Y";
                }

                string strRecordType = "T";

                if (this.rbnBreak.Checked == true)
                {
                    strRecordType = "B";
                }

                //Remove TimeSheets For Company
                DataView myTimesheetDataView = new DataView(pvtDataSet.Tables["TimeSheet"],
                   "COMPANY_NO = " + pvtintCompanyNo,
                   "",
                   DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < myTimesheetDataView.Count; intRow++)
                {
                    myTimesheetDataView[intRow].Delete();

                    intRow -= 1;
                }

                //Remove Breaks For Company
                myTimesheetDataView = new DataView(pvtDataSet.Tables["Break"],
                   "COMPANY_NO = " + pvtintCompanyNo,
                   "",
                   DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < myTimesheetDataView.Count; intRow++)
                {
                    myTimesheetDataView[intRow].Delete();

                    intRow -= 1;
                }

                pvtDataSet.AcceptChanges();

                object[] objParm = new object[13];
                objParm[0] = pvtintCompanyNo;
                objParm[1] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[3] = pvtintPayCategoryNo;
                objParm[4] = pvtstrPayrollType;
                objParm[5] = intTimeInMinutes;
                objParm[6] = intTimeOutMinutes;
                objParm[7] = strDates;
                objParm[8] = strOption;
                objParm[9] = strEmployeeNos;
                objParm[10] = strEmployeeRecNos;
                objParm[11] = strUpdateInd;
                objParm[12] = strRecordType;

                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Update_New_Records", objParm, true);

                //Re-Add TimeSheets and Breaks For Company
                DataSet DataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);
                pvtDataSet.Merge(DataSet);
                pvtDataSet.AcceptChanges();

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strDates = "";
                string strMessage = "Delete ALL Timesheet Records for Cost Centre\n\n" + this.dgvPayCategoryDataGridView[1, pvtintPayCategoryDataGridViewRowIndex].Value.ToString() + "\n\nfor Date\n\n" + this.dgvDayDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value.ToString();

                if (this.rbnBreak.Checked == true)
                {
                    strMessage = strMessage.Replace("Timesheet", "Break");
                }

                DialogResult dlgResult = CustomClientMessageBox.Show(strMessage,
                       this.Text,
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question);

                if (dlgResult == DialogResult.Yes)
                {
                    //Remove TimeSheets For Company
                    DataView myTimesheetDataView = new DataView(pvtDataSet.Tables["TimeSheet"],
                       "COMPANY_NO = " + pvtintCompanyNo,
                       "",
                       DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < myTimesheetDataView.Count; intRow++)
                    {
                        myTimesheetDataView[intRow].Delete();

                        intRow -= 1;
                    }

                    //Remove Breaks For Company
                    myTimesheetDataView = new DataView(pvtDataSet.Tables["Break"],
                       "COMPANY_NO = " + pvtintCompanyNo,
                       "",
                       DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < myTimesheetDataView.Count; intRow++)
                    {
                        myTimesheetDataView[intRow].Delete();

                        intRow -= 1;
                    }

                    pvtDataSet.AcceptChanges();

                    strDates = this.dgvDayDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value.ToString().Substring(0, 4) + "-" + this.dgvDayDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value.ToString().Substring(4, 2) + "-" + this.dgvDayDataGridView[1, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value.ToString().Substring(6, 2);
                    
                    string strRecordType = "T";

                    if (this.rbnBreak.Checked == true)
                    {
                        strRecordType = "B";
                    }

                    object[] objParm = new object[5];
                    objParm[0] = pvtintCompanyNo;
                    objParm[1] = pvtintPayCategoryNo;
                    objParm[2] = pvtstrPayrollType;
                    objParm[3] = strDates;
                    objParm[4] = strRecordType;

                    clsISClientUtilities.DynamicFunction("Delete_Records", objParm,true);

                    CustomClientMessageBox.Show("Records Deleted Successfully.",
                       this.Text, 
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Information);

                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private int Check_Time(string strTime)
        {
            int intReturnCode = 0;

            int intHH = 0;
            int intMM = 0;

            if (strTime.Length > 2)
            {
                intHH = Convert.ToInt32(strTime.Substring(0, strTime.Length - 2));
                intMM = Convert.ToInt32(strTime.Substring(strTime.Length - 2, 2));
            }
            else
            {
                if (strTime.Length == 0)
                {
                    intReturnCode = 1;
                }
                else
                {
                    intMM = Convert.ToInt32(strTime);
                }
            }

            if (intHH >= 24)
            {
                if (intHH == 24)
                {
                    if (intMM != 0)
                    {
                        intReturnCode = 1;
                    }
                }
                else
                {
                    intReturnCode = 1;
                }
            }

            if (intMM > 59)
            {
                intReturnCode = 1;
            }

            return intReturnCode;
        }

        private int Return_Time_In_Minutes(string strTime)
        {
            int intTimeInMinutes = 0;
            int intHH = 0;
            int intMM = 0;

            if (strTime.Length > 2)
            {
                intHH = Convert.ToInt32(strTime.Substring(0, strTime.Length - 2));
                intMM = Convert.ToInt32(strTime.Substring(strTime.Length - 2, 2));
            }
            else
            {
                intMM = Convert.ToInt32(strTime);
            }

            intTimeInMinutes = (intHH * 60) + intMM;

            return intTimeInMinutes;
        }

        private void rbnIn_Click(object sender, EventArgs e)
        {
            this.tmrOptionTimer.Enabled = false;
            this.lblChooseOption.Visible = false;

            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;
            
            Remove_Selected_Dates();

            this.chkUpdate.Checked = false;
            this.chkUpdate.Enabled = true;

            this.txtIn.Enabled = true;
            this.txtOut.Enabled = false;
            this.txtOut.Text = "";

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, pvtintPayCategoryDataGridViewRowIndex);
            }
        }

        private void Remove_Selected_Dates()
        {
            this.btnDateAdd.Visible = false;
            this.btnDateRemove.Visible = false;

            this.lblChosenDates.Visible = false;
            this.dgvDayChosenDataGridView.Visible = false;

            this.grbDateException.Visible = false;

            this.rbnListDates.Checked = true;

            this.lblDate.Text = "Selected Date";
        }

        private void Load_All_Employees()
        {
            Clear_DataGridView(this.dgvEmployeeDataGridView);
            Clear_DataGridView(this.dgvEmployeeChosenDataGridView);
            Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
            Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);

            pvtblnEmployeeDataGridViewLoaded = false;

            pvtdtDayDateTime = new DateTime(Convert.ToInt32(this.dgvDayDataGridView[1, dgvDayDataGridView.SelectedRows[0].Index].Value.ToString().Substring(0, 4)), Convert.ToInt32(this.dgvDayDataGridView[1, dgvDayDataGridView.SelectedRows[0].Index].Value.ToString().Substring(4, 2)), Convert.ToInt32(this.dgvDayDataGridView[1, dgvDayDataGridView.SelectedRows[0].Index].Value.ToString().Substring(6, 2)));

            pvtEmployeeDataView = null;
            pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                "COMPANY_NO = " + this.pvtintCompanyNo + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_LAST_RUNDATE <= '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'",
                "",
                DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
            {
                this.dgvEmployeeDataGridView.Rows.Add("",
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                      "",
                                                      "",
                                                      pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                                      "-1");
            }

            pvtblnEmployeeDataGridViewLoaded = true;
        }

        private void Clear_DataGridView(DataGridView myDataGridView)
        {
            myDataGridView.Rows.Clear();

            if (myDataGridView.SortedColumn != null)
            {
                myDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
        }

        private void rbnOut_Click(object sender, EventArgs e)
        {
            this.tmrOptionTimer.Enabled = false;
            this.lblChooseOption.Visible = false;

            Remove_Selected_Dates();

            this.chkUpdate.Checked = false;
            this.chkUpdate.Enabled = true;

            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;
            
            this.txtOut.Enabled = true;
            this.txtIn.Enabled = false;
            this.txtIn.Text = "";

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, pvtintPayCategoryDataGridViewRowIndex);
            }
        }

        private void rbnBoth_Click(object sender, EventArgs e)
        {
            this.tmrOptionTimer.Enabled = false;
            this.lblChooseOption.Visible = false;

            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;
            
            this.chkUpdate.Checked = false;
            this.chkUpdate.Enabled = true;

            this.txtIn.Enabled = true;
            this.txtOut.Enabled = true;

            Add_Selected_Dates();

            if (this.dgvDayDataGridView.Rows.Count > 0)
            {
                this.Load_All_Employees();

                this.Set_DataGridView_SelectedRowIndex(dgvDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView));

                this.dgvDayDataGridView.Focus();
            }
        }

        private void Add_Selected_Dates()
        {
            this.btnDateAdd.Visible = true;
            this.btnDateRemove.Visible = true;

            this.lblChosenDates.Visible = true;

            this.Clear_DataGridView(dgvDayChosenDataGridView);

            this.dgvDayChosenDataGridView.Visible = true;

            this.grbDateException.Visible = true;

            this.lblDate.Text = "List of Dates";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                bool blnAddEmployee = true;

                if (this.rbnDeleteRow.Checked == false)
                {
                    for (int intRow = 0; intRow < this.dgvEmployeeChosenDataGridView.Rows.Count; intRow++)
                    {
                        if (this.dgvEmployeeDataGridView[6, Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)].Value.ToString() == this.dgvEmployeeChosenDataGridView[6, intRow].Value.ToString())
                        {
                            if (pvtblnAddAll == false)
                            {
                                CustomClientMessageBox.Show("A record for this Employee has already been selected", this.Text,MessageBoxButtons.OK,MessageBoxIcon.Information);
                            }
               
                            blnAddEmployee = false;

                            break;
                        }
                    }
                }

                if (blnAddEmployee == true)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)];

                    this.dgvEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvEmployeeChosenDataGridView.Rows.Add(myDataGridViewRow);

                    this.dgvEmployeeChosenDataGridView.CurrentCell = this.dgvEmployeeChosenDataGridView[0, this.dgvEmployeeChosenDataGridView.Rows.Count - 1];

                    if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                    {
                        pvtintEmployeeDataGridViewRowIndex = -1;

                        this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, 0);
                    }
                    else
                    {
                        Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
                        this.lblEmployeeRejectedName.Text = "";
                    }
                }
                else
                {
                    Set_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView));
                }
            }
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            pvtblnAddAll = true;
            bool blnDuplicates = false;
            bool blnExists = false;
            int intMainEmployeeNo = -1;

            for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
            {
                blnExists = false;
                intMainEmployeeNo = Convert.ToInt32(this.dgvEmployeeDataGridView[6, intRow].Value);

                if (this.rbnDeleteRow.Checked == false)
                {
                    for (int intCompareRow = 0; intCompareRow < this.dgvEmployeeDataGridView.Rows.Count; intCompareRow++)
                    {
                        if (intCompareRow == intRow)
                        {
                            continue;
                        }

                        if (intMainEmployeeNo == Convert.ToInt32(this.dgvEmployeeDataGridView[6, intCompareRow].Value))
                        {
                            blnExists = true;
                            break;
                        }
                    }
                }

                if (blnExists == true)
                {
                    blnDuplicates = true;
                    continue;
                }

                dgvEmployeeDataGridView.CurrentCell = dgvEmployeeDataGridView[0, intRow];
                btnAdd_Click(null, null);
                intRow -= 1;
            }

            if (blnDuplicates == true)
            {
                CustomClientMessageBox.Show("Employee/s with 2 or More Records Exist.\nThese Records have been ignored.\n\nYou may manually select 1 of these Records.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            pvtblnAddAll = false;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeeChosenDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeChosenDataGridView)];

                this.dgvEmployeeChosenDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvEmployeeDataGridView.CurrentCell = this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1];
               
                if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvEmployeeChosenDataGridView, 0);
                }
                else
                {
                    Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);
                    this.lblEmployeeChosenRejectedName.Text = "";
                }
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            this.rbnTimesheet.Enabled = false;
            this.rbnBreak.Enabled = false;

            this.lblChooseOption.Visible = true;
            this.tmrOptionTimer.Enabled = true;

            this.dgvCompanyDataGridView.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;
            this.dgvPayCategoryDataGridView.Enabled = false;

            this.picCompanyLock.Visible = true;
            this.picPayrollLock.Visible = true;
            this.picPayCategoryLock.Visible = true;

            this.btnSave.Enabled = true;

            this.btnDelete.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnCancel.Enabled = true;

            this.rbnIn.Enabled = true;
            this.rbnOut.Enabled = true;
            this.rbnBoth.Enabled = true;
            this.rbnDeleteRow.Enabled = true;
        }

        private void Text_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox myTextBox = (TextBox)sender;

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
                                    if (strNewTextField.Length > 3)
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

        public int Get_DataGridView_SelectedRowIndex(DataGridView myDataGridView)
        {
            int intReturnIndex = -1;

            if (myDataGridView.SelectedRows.Count > 0)
            {
                if (myDataGridView.SelectedRows[0].Selected == true)
                {
                    intReturnIndex = myDataGridView.SelectedRows[0].Index;
                }
                else
                {
                    string strStop = "";
                }
            }
            else
            {
                string strStop = "";

            }
            
            return intReturnIndex;
        }

        public void Set_DataGridView_SelectedRowIndex(DataGridView myDataGridView, int intRow)
        {

#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("Set_DataGridView_SelectedRowIndex + myDataGridView.Name = " + myDataGridView.Name);
#endif
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

                case "dgvEmployeeChosenDataGridView":

                    pvtintEmployeeChosenDataGridViewRowIndex = -1;
                    break;

                case "dgvDayChosenDataGridView":

                    pvtintDayChosenDataGridViewRowIndex = -1;
                    break;

                default:

                    break;
            }

            //Fires DataGridView RowEnter Function
            if (myDataGridView.CurrentCell.RowIndex == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvPayrollTypeDataGridView":

                        dgvPayrollTypeDataGridView_RowEnter(myDataGridView, myDataGridViewCellEventArgs);
                        break;

                    case "dgvCompanyDataGridView":

                        dgvCompanyDataGridView_RowEnter(myDataGridView, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        dgvPayCategoryDataGridView_RowEnter(myDataGridView, myDataGridViewCellEventArgs);
                        break;
                    
                    case "dgvDayDataGridView":

                        dgvDayDataGridView_RowEnter(myDataGridView, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeDataGridView":

                        dgvEmployeeDataGridView_RowEnter(myDataGridView, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeChosenDataGridView":

                        dgvEmployeeChosenDataGridView_RowEnter(myDataGridView, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDayChosenDataGridView":

                        dgvDayChosenDataGridView_RowEnter(myDataGridView, myDataGridViewCellEventArgs);
                        break;

                    default:

                        System.Windows.Forms.MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex",this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                if (myDataGridView.Rows.Count > 0)
                {
                    myDataGridView.CurrentCell = myDataGridView[0, intRow];
                }
            }
        }
       
        private void tmrOptionTimer_Tick(object sender, EventArgs e)
        {
            if (this.lblChooseOption.Visible == true)
            {
                this.lblChooseOption.Visible = false;
            }
            else
            {
                this.lblChooseOption.Visible = true;
            }
        }

        private void rbnDeleteRow_Click(object sender, EventArgs e)
        {
            this.tmrOptionTimer.Enabled = false;
            this.lblChooseOption.Visible = false;

            this.chkUpdate.Checked = false;
            this.chkUpdate.Enabled = false;

            this.Remove_Selected_Dates();

            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;
            
            this.txtIn.Text = "";
            this.txtOut.Text = "";

            this.txtIn.Enabled = false;
            this.txtOut.Enabled = false;

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, pvtintPayCategoryDataGridViewRowIndex);
            }
        }

        private void frmTimeSheetBatchClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }

        private void chkUpdate_Click(object sender, EventArgs e)
        {
            if (this.rbnIn.Checked == true
                | this.rbnOut.Checked == true
                | this.rbnBoth.Checked == true)
            {
                if (this.chkUpdate.Checked == true)
                {
                    Remove_Selected_Dates();
                }
                else
                {
                    if (this.rbnBoth.Checked == true)
                    {
                        rbnBoth_Click(sender, e);
                        return;
                    }
                }

                if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, pvtintPayCategoryDataGridViewRowIndex);
                }
            }
        }

        private void RecordType_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            if (myRadioButton.Name == "rbnTimesheet")
            {
                pvtstrTableDef = "TIMESHEET";
                pvtstrTableName = "Timesheet";
                this.pnlBreak.Visible = false;

                this.lblEmployee.Text = this.lblEmployee.Text.Replace("Break", "Timesheet");
                this.lblSelectedEmployee.Text = this.lblSelectedEmployee.Text.Replace("Break", "Timesheet");

                this.tmrBreakTimer.Enabled = false;
                this.pnlBreak.Visible = false;
            }
            else
            {
                pvtstrTableDef = "BREAK";
                pvtstrTableName = "Break";
                this.pnlBreak.Visible = true;

                this.lblEmployee.Text = this.lblEmployee.Text.Replace("Timesheet", "Break");
                this.lblSelectedEmployee.Text = this.lblSelectedEmployee.Text.Replace("Timesheet", "Break");

                pvtintTimerCount = 0;
                this.tmrBreakTimer.Enabled = true;
            }
        }

        private void TimeField_Leave(object sender, EventArgs e)
        {
            TextBox myTextBox = (TextBox)sender;

            if (myTextBox.Text == ""
                |  myTextBox.Text == "2400")
            {
            }
            else
            {
                if (myTextBox.Text.Length > 2)
                {
                    if (Convert.ToInt32(myTextBox.Text.Substring(0, myTextBox.Text.Length - 2)) > 23
                        | Convert.ToInt32(myTextBox.Text.Substring(myTextBox.Text.Length - 2, 2)) > 59)
                    {
                        myTextBox.Text = "";
                        System.Console.Beep();
                    }
                }
                else
                {
                    if (Convert.ToInt32(myTextBox.Text) > 59)
                    {
                        myTextBox.Text = "";
                        System.Console.Beep();
                    }
                }
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

                        this.grbEmployeeLock.Visible = false;

                        pvtintCompanyNo = Convert.ToInt32(pvtDataSet.Tables["Company"].Rows[Convert.ToInt32(this.dgvCompanyDataGridView[1, e.RowIndex].Value)]["COMPANY_NO"]);

                        DataView myDataView = new DataView(pvtDataSet.Tables["PayCategory"], "COMPANY_NO = " + pvtintCompanyNo, "", DataViewRowState.CurrentRows);

                        if (myDataView.Count == 0)
                        {
                            object[] objParm = new object[4];
                            objParm[0] = pvtintCompanyNo;
                            objParm[1] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                            objParm[2] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                            objParm[3] = "";

                            byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_New_Company_Records", objParm, false);
                            DataSet TempDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                            pvtDataSet.Merge(TempDataSet);
                        }

                        pvtPayCategoryDataView = null;
                        pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"], "COMPANY_NO = " + pvtintCompanyNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'", "", DataViewRowState.CurrentRows);

                        Clear_DataGridView(this.dgvPayCategoryDataGridView);
                        Clear_DataGridView(this.dgvEmployeeDataGridView);
                        Clear_DataGridView(this.dgvEmployeeChosenDataGridView);
                        Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
                        Clear_DataGridView(this.dgvDayDataGridView);

                        this.lblEmployeeRejectedName.Text = "";
                        this.lblEmployeeChosenRejectedName.Text = "";

                        pvtdtDateTimeTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(15);

                        this.btnDelete.Enabled = false;
                        this.btnUpdate.Enabled = false;

                        pvtblnPayCategoryDataGridViewLoaded = false;
                        string strDateTime = "";
                        string strDateTimeSort = "";

                        for (int intRow = 0; intRow < pvtPayCategoryDataView.Count; intRow++)
                        {
                            if (pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"] != System.DBNull.Value)
                            {
                                strDateTime = Convert.ToDateTime(pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("dd MMMM yyyy   -   HH:mm");
                                strDateTimeSort = Convert.ToDateTime(pvtPayCategoryDataView[intRow]["LAST_UPLOAD_DATETIME"]).ToString("yyyyMMddHHmm");
                            }
                            else
                            {
                                strDateTime = "";
                                strDateTimeSort = "";
                            }

                            this.dgvPayCategoryDataGridView.Rows.Add("",
                                                                     pvtPayCategoryDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                     strDateTime,
                                                                     intRow.ToString(),
                                                                     strDateTimeSort);
                        }

                        pvtblnPayCategoryDataGridViewLoaded = true;

                        if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                        {
                            this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, 0);
                        }
                    }
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtintPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    this.pvtstrPayrollType = dgvPayrollTypeDataGridView[0, e.RowIndex].Value.ToString().Substring(0, 1);

                    if (dgvCompanyDataGridView.Rows.Count > 0)
                    {
                        //Select First Row
                        this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
                    }
                }
            }
        }

        private void dgvDayDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (pvtblnDayDataGridViewLoaded == true)
                {
                    if (pvtintDayDataGridViewRowIndex != e.RowIndex)
                    {
                        pvtintDayDataGridViewRowIndex = e.RowIndex;

                        int intFindRow = -1;
                        bool blnPayCategoryDataGridViewNormal = true;
                        string strFilter = "";
                        string strStart = "";
                        string strEnd = "";

                        int intHH = 0;
                        int intMM = 0;

                        pvtdtDayDateTime = DateTime.ParseExact(this.dgvDayDataGridView[1, e.RowIndex].Value.ToString(), "yyyyMMdd", null);

                        Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
                        Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);

                        this.lblEmployeeRejectedName.Text = "";
                        this.lblEmployeeChosenRejectedName.Text = "";

                        if (this.rbnIn.Checked == true
                            | this.rbnOut.Checked == true
                            | this.rbnDeleteRow.Checked == true
                            | this.chkUpdate.Checked == true)
                        {
                            Clear_DataGridView(this.dgvEmployeeDataGridView);
                            Clear_DataGridView(this.dgvEmployeeChosenDataGridView);

                            strFilter = "";

                            if (this.rbnOut.Checked == true
                                & this.chkUpdate.Checked == false)
                            {
                                strFilter = " AND " + pvtstrTableDef + "_TIME_OUT_MINUTES IS NULL";
                            }
                            
                            pvtEmployeeDataView = null;
                            pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                                "COMPANY_NO = " + pvtintCompanyNo + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND EMPLOYEE_LAST_RUNDATE < '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'",
                                "",
                                DataViewRowState.CurrentRows);

                            pvtblnEmployeeDataGridViewLoaded = false;

                            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
                            {
                                strStart = "";
                                strEnd = "";

                                pvtTimesheetOrBreakDataView = null;
                                pvtTimesheetOrBreakDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                                   "COMPANY_NO = " + pvtintCompanyNo + " AND EMPLOYEE_NO = " + pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString() + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND " + pvtstrTableDef + "_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'" + strFilter,
                                   "EMPLOYEE_NO",
                                   DataViewRowState.CurrentRows);

                                if (pvtTimesheetOrBreakDataView.Count > 0)
                                {
                                    for (int intRow1 = 0; intRow1 < pvtTimesheetOrBreakDataView.Count; intRow1++)
                                    {
                                        strStart = "";
                                        strEnd = "";

                                        if (this.rbnOut.Checked == true
                                             & this.chkUpdate.Checked == false)
                                        {
                                            intHH = Convert.ToInt32(pvtTimesheetOrBreakDataView[intRow1][pvtstrTableDef + "_TIME_IN_MINUTES"]) / 60;
                                            intMM = Convert.ToInt32(pvtTimesheetOrBreakDataView[intRow1][pvtstrTableDef + "_TIME_IN_MINUTES"]) % 60;

                                            strStart = intHH.ToString() + intMM.ToString("00");
                                        }
                                        else
                                        {
                                            if (this.rbnDeleteRow.Checked == true
                                                | this.chkUpdate.Checked == true)
                                            {
                                                if (pvtTimesheetOrBreakDataView[intRow1][pvtstrTableDef + "_TIME_IN_MINUTES"] != System.DBNull.Value)
                                                {
                                                    intHH = Convert.ToInt32(pvtTimesheetOrBreakDataView[intRow1][pvtstrTableDef + "_TIME_IN_MINUTES"]) / 60;
                                                    intMM = Convert.ToInt32(pvtTimesheetOrBreakDataView[intRow1][pvtstrTableDef + "_TIME_IN_MINUTES"]) % 60;

                                                    strStart = intHH.ToString() + intMM.ToString("00");
                                                }

                                                if (pvtTimesheetOrBreakDataView[intRow1][pvtstrTableDef + "_TIME_OUT_MINUTES"] != System.DBNull.Value)
                                                {
                                                    intHH = Convert.ToInt32(pvtTimesheetOrBreakDataView[intRow1][pvtstrTableDef + "_TIME_OUT_MINUTES"]) / 60;
                                                    intMM = Convert.ToInt32(pvtTimesheetOrBreakDataView[intRow1][pvtstrTableDef + "_TIME_OUT_MINUTES"]) % 60;

                                                    strEnd = intHH.ToString() + intMM.ToString("00");
                                                }
                                            }
                                        }


                                        this.dgvEmployeeDataGridView.Rows.Add("",
                                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                              strStart,
                                                                              strEnd,
                                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                                                              pvtTimesheetOrBreakDataView[intRow1][pvtstrTableDef + "_SEQ"].ToString());
                                    }
                                }
                                else
                                {
                                    if (this.rbnOut.Checked == true
                                        | this.rbnDeleteRow.Checked == true
                                        | this.chkUpdate.Checked == true)
                                    {
                                    }
                                    else
                                    {
                                        this.dgvEmployeeDataGridView.Rows.Add("", 
                                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                              strStart,
                                                                              strEnd,
                                                                              pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                                                              "-1");
                                    }
                                }
                            }

                            pvtblnEmployeeDataGridViewLoaded = true;
                        }

                        strFilter = "";

                        if (this.rbnDeleteRow.Checked == true
                               | this.chkUpdate.Checked == true)
                        {
                            strFilter = " AND EMPLOYEE_NO = -1";
                        }
                        else
                        {
                            if (this.rbnOut.Checked == true)
                            {
                                strFilter = " AND NOT " + pvtstrTableDef + "_TIME_OUT_MINUTES IS NULL";
                            }
                        }

                        pvtEmployeeRejectedDataView = null;
                        pvtEmployeeRejectedDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                                "COMPANY_NO = " + pvtintCompanyNo + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND " + pvtstrTableDef + "_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'" + strFilter,
                                "EMPLOYEE_NO",
                                DataViewRowState.CurrentRows);

                        pvtblnEmployeeDataGridViewLoaded = false;

                        for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
                        {
                            int intEmployeeNo = Convert.ToInt32(dgvEmployeeDataGridView[6, intRow].Value);

                            intFindRow = pvtEmployeeRejectedDataView.Find(intEmployeeNo);

                            if (intFindRow == -1)
                            {
                                this.dgvEmployeeDataGridView[0,intRow].Style = NormalDataGridViewCellStyle;
                            }
                            else
                            {
                                blnPayCategoryDataGridViewNormal = false;
                                this.dgvEmployeeDataGridView[0,intRow].Style = RejectedDataGridViewCellStyle;
                            }
                        }

                        pvtblnEmployeeChosenDataGridViewLoaded = false;

                        for (int intRow = 0; intRow < this.dgvEmployeeChosenDataGridView.Rows.Count; intRow++)
                        {
                            int intEmployeeNo = Convert.ToInt32(dgvEmployeeChosenDataGridView[6, intRow].Value);

                            intFindRow = pvtEmployeeRejectedDataView.Find(intEmployeeNo);

                            if (intFindRow == -1)
                            {
                                this.dgvEmployeeChosenDataGridView[0,intRow].Style = NormalDataGridViewCellStyle;
                            }
                            else
                            {
                                blnPayCategoryDataGridViewNormal = false;
                                this.dgvEmployeeChosenDataGridView[0,intRow].Style = RejectedDataGridViewCellStyle;
                            }
                        }

                        if (blnPayCategoryDataGridViewNormal == false)
                        {
                            this.dgvPayCategoryDataGridView[0,pvtintPayCategoryDataGridViewRowIndex].Style = RejectedDataGridViewCellStyle;
                        }
                        else
                        {
                            this.dgvPayCategoryDataGridView[0,pvtintPayCategoryDataGridViewRowIndex].Style = NormalDataGridViewCellStyle;
                        }

                        pvtblnEmployeeDataGridViewLoaded = true;

                        if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                        {
                            this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, 0);
                        }

                        pvtblnEmployeeChosenDataGridViewLoaded = true;

                        if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0)
                        {
                            this.Set_DataGridView_SelectedRowIndex(dgvEmployeeChosenDataGridView, 0);
                        }
                    }
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
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

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    //Not Part of "In and Out" Selection
                    if (grbDateException.Visible == false)
                    {
                        Clear_DataGridView(this.dgvEmployeeDataGridView);
                        Clear_DataGridView(this.dgvEmployeeChosenDataGridView);
                    }

                    Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
                    Clear_DataGridView(this.dgvDayDataGridView);

                    this.lblEmployeeRejectedName.Text = "";
                    this.lblEmployeeChosenRejectedName.Text = "";

                    pvtintPayCategoryTableRowNo = Convert.ToInt32(this.dgvPayCategoryDataGridView[3, e.RowIndex].Value);
                    pvtintPayCategoryNo = Convert.ToInt32(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["PAY_CATEGORY_NO"]);

                    //Not Already in Edit Mode
                    if (this.btnCancel.Enabled == false)
                    {
                        if (pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["NO_EDIT_IND"].ToString() == "Y")
                        {
                            this.btnUpdate.Enabled = false;
                            this.btnDelete.Enabled = false;

                            this.grbEmployeeLock.Visible = true;
                        }
                        else
                        {
                            this.btnUpdate.Enabled = true;
                            this.btnDelete.Enabled = true;
                        }
                    }

                    DateTime dtDateTimeFrom = Convert.ToDateTime(pvtPayCategoryDataView[pvtintPayCategoryTableRowNo]["FROM_DATE"]);

                    DateTime dtDateTime = pvtdtDateTimeTo;

                    int intSelectedRow = 0;

                    this.pvtblnDayDataGridViewLoaded = false;

                    while (dtDateTimeFrom <= dtDateTime)
                    {
                        this.dgvDayDataGridView.Rows.Add(dtDateTime.ToString("dd MMM yyyy - ddd"),
                                                         dtDateTime.ToString("yyyyMMdd"));

                        if (dtDateTime.ToString("yyyyMMdd") == this.pvtdtDayDateTime.ToString("yyyyMMdd"))
                        {
                            intSelectedRow = this.dgvDayDataGridView.Rows.Count - 1;
                        }

                        dtDateTime = dtDateTime.AddDays(-1);
                    }

                    this.pvtblnDayDataGridViewLoaded = true;

                    if (this.dgvDayDataGridView.Rows.Count > 0)
                    {
                        Set_DataGridView_SelectedRowIndex(dgvDayDataGridView, intSelectedRow);
                    }
                }
            }
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnEmployeeDataGridViewLoaded == true)
            {
                if (pvtintEmployeeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintEmployeeDataGridViewRowIndex = e.RowIndex;

                    this.Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);

                    if (this.rbnDeleteRow.Checked == true
                    | this.chkUpdate.Checked == true)
                    {
                        this.lblEmployeeRejectedName.Text = "";
                    }
                    else
                    {
                        int intEmployeeNo = Convert.ToInt32(dgvEmployeeDataGridView[6, e.RowIndex].Value);

                        pvtEmployeeDataView = null;
                        pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                        "COMPANY_NO = " + this.pvtintCompanyNo + " AND EMPLOYEE_NO = " + intEmployeeNo.ToString() + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "",
                        DataViewRowState.CurrentRows);

                        string strFilter = "";

                        if (this.rbnOut.Checked == true)
                        {
                            strFilter = " AND NOT " + pvtstrTableDef + "_TIME_OUT_MINUTES IS NULL";
                        }

                        pvtEmployeeRejectedDataView = null;
                        pvtEmployeeRejectedDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                            "COMPANY_NO = " + this.pvtintCompanyNo + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + intEmployeeNo.ToString() + " AND " + pvtstrTableDef + "_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'" + strFilter,
                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtEmployeeRejectedDataView.Count > 0)
                        {
                            if (pvtEmployeeDataView.Count > 0)
                            {
                                int intHH = 0;
                                int intMM = 0;
                                string strIn = "";
                                string strOut = "";

                                for (int intRow = 0; intRow < pvtEmployeeRejectedDataView.Count; intRow++)
                                {
                                    if (pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_IN_MINUTES"] != System.DBNull.Value)
                                    {
                                        intHH = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_IN_MINUTES"]) / 60;
                                        intMM = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_IN_MINUTES"]) % 60;

                                        strIn = intHH.ToString() + intMM.ToString("00");
                                    }
                                    else
                                    {
                                        strIn = "";
                                    }

                                    if (pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_OUT_MINUTES"] != System.DBNull.Value)
                                    {
                                        intHH = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_OUT_MINUTES"]) / 60;
                                        intMM = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_OUT_MINUTES"]) % 60;

                                        strOut = intHH.ToString() + intMM.ToString("00");
                                    }
                                    else
                                    {
                                        strOut = "";
                                    }

                                    this.dgvEmployeeRejectedDataGridView.Rows.Add(pvtEmployeeDataView[0]["EMPLOYEE_CODE"].ToString(),
                                                                                  pvtEmployeeDataView[0]["EMPLOYEE_SURNAME"].ToString(),
                                                                                  pvtEmployeeDataView[0]["EMPLOYEE_NAME"].ToString(),
                                                                                  strIn,
                                                                                  strOut);
                                }

                                if (this.dgvEmployeeRejectedDataGridView.Rows.Count > 0)
                                {
                                    if (this.rbnSelectedDates.Checked == true)
                                    {
                                        this.lblEmployeeRejectedName.Text = pvtEmployeeDataView[0]["EMPLOYEE_NAME"].ToString() + " " + pvtEmployeeDataView[0]["EMPLOYEE_SURNAME"].ToString() + " - " + dgvDayChosenDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvDayChosenDataGridView)].Value.ToString();

                                    }
                                    else
                                    {
                                        this.lblEmployeeRejectedName.Text = pvtEmployeeDataView[0]["EMPLOYEE_NAME"].ToString() + " " + pvtEmployeeDataView[0]["EMPLOYEE_SURNAME"].ToString() + " - " + dgvDayDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value.ToString();
                                    }
                                }
                                else
                                {
                                    this.lblEmployeeRejectedName.Text = "";
                                }
                            }
                            else
                            {
                                this.lblEmployeeRejectedName.Text = "";
                            }
                        }
                        else
                        {
                            this.lblEmployeeRejectedName.Text = "";
                        }
                    }
                }
            }
        }

        private void btnDateRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvDayChosenDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvDayChosenDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvDayChosenDataGridView)];

                pvtblnEmployeeDataGridViewLoaded = false;

                this.dgvDayChosenDataGridView.Rows.Remove(myDataGridViewRow);

                pvtblnEmployeeDataGridViewLoaded = true;

                this.dgvDayDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvDayDataGridView.CurrentCell = this.dgvDayDataGridView[0, this.dgvDayDataGridView.Rows.Count - 1];
            }
        }

        private void btnDateAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvDayDataGridView.Rows.Count > 0)
            {
                //Stops DataGridView from Falling Over
                pvtblnDayDataGridViewLoaded = false;
                pvtblnDayChosenDataGridViewLoaded = false;

                if (this.dgvDayChosenDataGridView.Rows.Count == 0)
                {
                    this.rbnSelectedDates.Checked = true;
                }
              
                DataGridViewRow myDataGridViewRow = this.dgvDayDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvDayDataGridView)];

                this.dgvDayDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvDayChosenDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvDayChosenDataGridView.CurrentCell = this.dgvDayChosenDataGridView[0, this.dgvDayChosenDataGridView.Rows.Count - 1];

                pvtblnDayDataGridViewLoaded = true;
                pvtblnDayChosenDataGridViewLoaded = true;

                if (this.rbnSelectedDates.Checked == true)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvDayChosenDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayChosenDataGridView));
                }
                else
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvDayDataGridView,this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView));
                }
            }
        }

        private void dgvEmployeeChosenDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnEmployeeChosenDataGridViewLoaded == true)
            {
                if (pvtintEmployeeChosenDataGridViewRowIndex != e.RowIndex)
                {
#if(DEBUG)
                    System.Diagnostics.Debug.WriteLine("dgvEmployeeChosenDataGridView_RowEnter");
#endif
                    pvtintEmployeeChosenDataGridViewRowIndex = e.RowIndex;

                    this.Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);

                    if (this.rbnDeleteRow.Checked == true
                    | this.chkUpdate.Checked == true)
                    {
                        this.lblEmployeeChosenRejectedName.Text = "";
                    }
                    else
                    {
                        int intEmployeeNo = Convert.ToInt32(dgvEmployeeChosenDataGridView[6, e.RowIndex].Value);

                        pvtEmployeeDataView = null;
                        pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                        "COMPANY_NO = " + this.pvtintCompanyNo + " AND EMPLOYEE_NO = " + intEmployeeNo.ToString() + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'",
                        "",
                        DataViewRowState.CurrentRows);

                        string strFilter = "";

                        if (this.rbnOut.Checked == true)
                        {
                            strFilter = " AND NOT " + pvtstrTableDef + "_TIME_OUT_MINUTES IS NULL";
                        }

                        pvtEmployeeRejectedDataView = null;
                        pvtEmployeeRejectedDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                            "COMPANY_NO = " + this.pvtintCompanyNo + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND EMPLOYEE_NO = " + intEmployeeNo.ToString() + " AND " + pvtstrTableDef + "_DATE = '" + pvtdtDayDateTime.ToString("yyyy-MM-dd") + "'" + strFilter,

                            "",
                            DataViewRowState.CurrentRows);

                        if (pvtEmployeeRejectedDataView.Count > 0)
                        {
                            if (pvtEmployeeDataView.Count > 0)
                            {
                                int intHH = 0;
                                int intMM = 0;
                                string strIn = "";
                                string strOut = "";

                                for (int intRow = 0; intRow < pvtEmployeeRejectedDataView.Count; intRow++)
                                {
                                    if (pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_IN_MINUTES"] != System.DBNull.Value)
                                    {
                                        intHH = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_IN_MINUTES"]) / 60;
                                        intMM = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_IN_MINUTES"]) % 60;

                                        strIn = intHH.ToString() + intMM.ToString("00");
                                    }
                                    else
                                    {
                                        strIn = "";
                                    }

                                    if (pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_OUT_MINUTES"] != System.DBNull.Value)
                                    {
                                        intHH = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_OUT_MINUTES"]) / 60;
                                        intMM = Convert.ToInt32(pvtEmployeeRejectedDataView[intRow][pvtstrTableDef + "_TIME_OUT_MINUTES"]) % 60;

                                        strOut = intHH.ToString() + intMM.ToString("00");
                                    }
                                    else
                                    {
                                        strOut = "";
                                    }

                                    this.dgvEmployeeChosenRejectedDataGridView.Rows.Add(pvtEmployeeDataView[0]["EMPLOYEE_CODE"].ToString(),
                                                                                  pvtEmployeeDataView[0]["EMPLOYEE_SURNAME"].ToString(),
                                                                                  pvtEmployeeDataView[0]["EMPLOYEE_NAME"].ToString(),
                                                                                  strIn,
                                                                                  strOut);
                                }

                                if (this.dgvEmployeeChosenRejectedDataGridView.Rows.Count > 0)
                                {
                                    if (this.rbnSelectedDates.Checked == true)
                                    {
                                        this.lblEmployeeChosenRejectedName.Text = pvtEmployeeDataView[0]["EMPLOYEE_NAME"].ToString() + " " + pvtEmployeeDataView[0]["EMPLOYEE_SURNAME"].ToString() + " - " + dgvDayChosenDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvDayChosenDataGridView)].Value.ToString();
                                    }
                                    else
                                    {
                                        this.lblEmployeeChosenRejectedName.Text = pvtEmployeeDataView[0]["EMPLOYEE_NAME"].ToString() + " " + pvtEmployeeDataView[0]["EMPLOYEE_SURNAME"].ToString() + " - " + dgvDayDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView)].Value.ToString();
                                    }
                                }
                                else
                                {
                                    this.lblEmployeeChosenRejectedName.Text = "";
                                }
                            }
                            else
                            {
                                this.lblEmployeeChosenRejectedName.Text = "";
                            }
                        }
                        else
                        {
                            this.lblEmployeeChosenRejectedName.Text = "";
                        }
                    }
                }
            }
        }

        private void ExceptionDate_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            this.dgvPayCategoryDataGridView[0,pvtintPayCategoryDataGridViewRowIndex].Style = NormalDataGridViewCellStyle;

            this.lblEmployeeRejectedName.Text = "";
            this.lblEmployeeChosenRejectedName.Text = "";

            this.Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
            this.Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);

            //Clear EmployeeChosen Spreadsheet To Normal
            pvtblnEmployeeChosenDataGridViewLoaded = false;

            for (int intRow = 0; intRow < this.dgvEmployeeChosenDataGridView.Rows.Count; intRow++)
            {
                this.dgvEmployeeChosenDataGridView[0,intRow].Style = NormalDataGridViewCellStyle;
            }

            pvtblnEmployeeChosenDataGridViewLoaded = true;

            if (myRadioButton.Name == "rbnListDates")
            {
                if (this.dgvDayDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvDayDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayDataGridView));
                }
                else
                {
                    pvtblnEmployeeDataGridViewLoaded = false;

                    for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
                    {
                        this.dgvEmployeeDataGridView[0,intRow].Style = NormalDataGridViewCellStyle;
                    }

                    pvtblnEmployeeDataGridViewLoaded = true;
                }
            }
            else
            {
                if (this.dgvDayChosenDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvDayChosenDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDayChosenDataGridView));
                }
                else
                {
                    pvtblnEmployeeDataGridViewLoaded = false;

                    for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
                    {
                        this.dgvEmployeeDataGridView[0,intRow].Style = NormalDataGridViewCellStyle;
                    }

                    pvtblnEmployeeDataGridViewLoaded = true;

                }

            }
        }

        private void dgvDayChosenDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (pvtblnDayChosenDataGridViewLoaded == true)
                {
                    if (pvtintDayChosenDataGridViewRowIndex != e.RowIndex)
                    {
                        pvtintDayChosenDataGridViewRowIndex = e.RowIndex;
#if(DEBUG)
                        System.Diagnostics.Debug.WriteLine("dgvDayChosenDataGridView_RowEnter");
#endif
                        if (this.rbnSelectedDates.Checked == true)
                        {
                            this.dgvPayCategoryDataGridView[0,pvtintPayCategoryDataGridViewRowIndex].Style = NormalDataGridViewCellStyle;

                            pvtdtDayDateTime = new DateTime(Convert.ToInt32(this.dgvDayChosenDataGridView[1, e.RowIndex].Value.ToString().Substring(0, 4)), Convert.ToInt32(this.dgvDayChosenDataGridView[1, e.RowIndex].Value.ToString().Substring(4, 2)), Convert.ToInt32(this.dgvDayChosenDataGridView[1, e.RowIndex].Value.ToString().Substring(6, 2)));

                            this.lblEmployeeRejectedName.Text = "";
                            this.lblEmployeeChosenRejectedName.Text = "";

                            this.Clear_DataGridView(this.dgvEmployeeRejectedDataGridView);
                            this.Clear_DataGridView(this.dgvEmployeeChosenRejectedDataGridView);

                            int intEmployeeNo = -1;

                            for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
                            {
                                intEmployeeNo = Convert.ToInt32(dgvEmployeeDataGridView[6, intRow].Value);

                                pvtEmployeeRejectedDataView = null;
                                pvtEmployeeRejectedDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                                    "COMPANY_NO = " + this.pvtintCompanyNo + " AND EMPLOYEE_NO = " + intEmployeeNo.ToString(),
                                    "",
                                    DataViewRowState.CurrentRows);

                                if (pvtEmployeeRejectedDataView.Count > 0)
                                {
                                    this.dgvEmployeeDataGridView[0,intRow].Style = RejectedDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeDataGridView[0,intRow].Style = NormalDataGridViewCellStyle;
                                }
                            }

                            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                            {
                                this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView));
                            }

                            for (int intRow = 0; intRow < this.dgvEmployeeChosenDataGridView.Rows.Count; intRow++)
                            {
                                intEmployeeNo = Convert.ToInt32(dgvEmployeeChosenDataGridView[6, intRow].Value);

                                pvtEmployeeRejectedDataView = null;
                                pvtEmployeeRejectedDataView = new DataView(pvtDataSet.Tables[pvtstrTableName],
                                    "COMPANY_NO = " + this.pvtintCompanyNo + " AND EMPLOYEE_NO = " + intEmployeeNo.ToString(),
                                    "",
                                    DataViewRowState.CurrentRows);

                                if (pvtEmployeeRejectedDataView.Count > 0)
                                {
                                    this.dgvEmployeeChosenDataGridView[0,intRow].Style = RejectedDataGridViewCellStyle;
                                }
                                else
                                {
                                    this.dgvEmployeeChosenDataGridView[0,intRow].Style = NormalDataGridViewCellStyle;
                                }
                            }

                            if (this.dgvEmployeeChosenDataGridView.Rows.Count > 0)
                            {
                                this.Set_DataGridView_SelectedRowIndex(dgvEmployeeChosenDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvEmployeeChosenDataGridView));
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

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnCancel.Enabled == true)
            {
                this.btnAdd_Click(sender, e);
            }
        }

        private void dgvEmployeeChosenDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnCancel.Enabled == true)
            {
                this.btnRemove_Click(sender, e);
            }
        }

        private void Date_DataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            DataGridView myDataGridView = (DataGridView)sender;

            if (double.Parse(myDataGridView[1, e.RowIndex1].Value.ToString()) > double.Parse(myDataGridView[1, e.RowIndex2].Value.ToString()))
            {
                e.SortResult = 1;
            }
            else
            {
                if (double.Parse(myDataGridView[1, e.RowIndex1].Value.ToString()) < double.Parse(myDataGridView[1, e.RowIndex2].Value.ToString()))
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

        private void dgvPayCategoryDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 2)
            {
                if (dgvPayCategoryDataGridView[4, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvPayCategoryDataGridView[4, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvPayCategoryDataGridView[4, e.RowIndex1].Value.ToString()) > double.Parse(dgvPayCategoryDataGridView[4, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvPayCategoryDataGridView[4, e.RowIndex1].Value.ToString()) < double.Parse(dgvPayCategoryDataGridView[4, e.RowIndex2].Value.ToString()))
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

        private void dgvDayDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnDateAdd.Visible == true)
            {
                Application.DoEvents();

                btnDateAdd_Click(sender, e);
            }
        }

        private void dgvDayChosenDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnDateRemove.Visible == true)
            {
                btnDateRemove_Click(sender, e);
            }
        }

        private void tmrBreakTimer_Tick(object sender, EventArgs e)
        {
            if (pvtintTimerCount == 3)
            {
                this.tmrBreakTimer.Enabled = false;
            }
            else
            {
                if (this.pnlBreak.Visible == true)
                {
                    this.pnlBreak.Visible = false;
                }
                else
                {
                    pvtintTimerCount += 1;
                    this.pnlBreak.Visible = true;
                }
            }
        }
    }
}
