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
    public partial class frmOpenPayrollRun : Form
    {
        clsISUtilities clsISUtilities;

        private byte[] pvtbytCompress;
        private int pvtintReturnCode = -1;
       
        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtEmployeeNotInRunDataView;
        private DataTable pvtDataTable;
        private DataRow pvtDataViewRow;
        private DataView pvtNotActiveDataView;
        private DataView pvtPayCategoryDataView;
        private DataView pvtPayCategoryChosenDataView;
        
        private int pvtintProcess = 0;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;
        private int pvtintChosenPayCategoryDataGridViewRowIndex = -1;

        int pvtintTimerCount = 0;
        int pvtintNextQueueCheck = 0;

        private string pvtstrPayrollType = "";
  
        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnChosenPayCategoryDataGridViewLoaded = false;

        DataGridViewCellStyle EmployeeNotActiveDataGridViewCellStyle;
        DataGridViewCellStyle PayrollTypeDateOpenDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;
        DataGridViewCellStyle ErrorDataGridViewCellStyle;
        
        private int pvtPayrollTypeDataGridViewRowIndex = -1;

        public frmOpenPayrollRun()
        {
            InitializeComponent();
            
            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.grbList.Height += 114;
                this.dgvPayCategoryDataGridView.Height += 114;

                this.lblEmployeeNotSelected.Top += 114;
                this.dgvEmployeeNotSelectedDataGridView.Top += 114;
                this.grbLegend.Top += 114;

                this.grbChosenList.Height += 114;
                this.dgvChosenPayCategoryDataGridView.Height += 114;

                this.lblEmployee.Top += 114;
                this.dgvEmployeeDataGridView.Top += 114;
            }

            this.grbStatus.Top = this.grbLeaveReminder.Top;
            this.grbStatus.Left = this.grbLeaveReminder.Left;
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void frmOpenPayrollRun_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busOpenPayrollRun");

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    lblDate.Text = "Time Attendance Run Date";
                    lblPrevDate.Text = "Previous Time Attendance Run Date";
                    lblPayrollDateOpen.Text = lblPayrollDateOpen.Text.Replace("Payroll Type", "Time Attendance"); 

                    grbLeaveReminder.Visible = false;
                }

                EmployeeNotActiveDataGridViewCellStyle = new DataGridViewCellStyle();
                EmployeeNotActiveDataGridViewCellStyle.BackColor = Color.Orange;
                EmployeeNotActiveDataGridViewCellStyle.SelectionBackColor = Color.Orange;

                PayrollTypeDateOpenDataGridViewCellStyle = new DataGridViewCellStyle();
                PayrollTypeDateOpenDataGridViewCellStyle.BackColor = Color.GreenYellow;
                PayrollTypeDateOpenDataGridViewCellStyle.SelectionBackColor = Color.GreenYellow;

                ErrorDataGridViewCellStyle = new DataGridViewCellStyle();
                ErrorDataGridViewCellStyle.BackColor = Color.Red;
                ErrorDataGridViewCellStyle.SelectionBackColor = Color.Red;
                
                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;

                clsISUtilities.Create_Calender_Control_From_TextBox(this.txtDate);

                clsISUtilities.NotDataBound_Date_TextBox(txtDate, "Capture Tax Effective Date.");
                clsISUtilities.NotDataBound_ComboBox(this.cboRunDate, "Select Tax Effective Date.");

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeNotSelected.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayCategory.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenPayCategory.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
   
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                int intSetRow = 0;
                
                for (int intRow = 0; intRow < this.pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add("",
                                                             "",
                                                             this.pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE"].ToString(),
                                                             this.pvtDataSet.Tables["PayrollType"].Rows[intRow]["OPEN_RUN_QUEUE_IND"].ToString());

                    if (this.pvtDataSet.Tables["PayrollType"].Rows[intRow]["OPEN_RUN_QUEUE_IND"].ToString() != "")
                    {
                        intSetRow = intRow;

                        if (this.pvtDataSet.Tables["PayrollType"].Rows[intRow]["OPEN_RUN_QUEUE_IND"].ToString() == "F")
                        {
                            this.dgvPayrollTypeDataGridView[1, this.dgvPayrollTypeDataGridView.Rows.Count - 1].Style = ErrorDataGridViewCellStyle;
                        }
                    }
                    
                    if (this.pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAY_PERIOD_DATE"] != System.DBNull.Value)
                    {
                        this.dgvPayrollTypeDataGridView[0,this.dgvPayrollTypeDataGridView.Rows.Count - 1].Style = PayrollTypeDateOpenDataGridViewCellStyle;
                    }
                }

                pvtblnPayrollTypeDataGridViewLoaded = true;
                
                if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, intSetRow);
                }
                
                if (pvtDataSet.Tables["OpenRunQueue"] != null)
                {
                    if (pvtDataSet.Tables["OpenRunQueue"].Rows.Count > 0)
                    {
                        if (pvtDataSet.Tables["OpenRunQueue"].Rows[0]["OPEN_RUN_QUEUE_IND"].ToString() == "S")
                        {
                            Busy_With_Run();
                        }
                        else
                        {
                            if (pvtDataSet.Tables["OpenRunQueue"].Rows[0]["OPEN_RUN_QUEUE_IND"].ToString() == "F")
                            {
                                Set_Run_Failed();
                            }
                        }
                    }
                    else
                    {
                        this.grbStatus.Visible = false;
                    }
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_CurrentForm_Records()
        {
            Set_Form_For_Read();

            Employee_WageRun();
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Clear_DataGridView(this.dgvEmployeeNotSelectedDataGridView);

                DataGridViewRow myDataGridViewRow = this.dgvPayCategoryDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)];

                this.dgvPayCategoryDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvChosenPayCategoryDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvChosenPayCategoryDataGridView.CurrentCell = this.dgvChosenPayCategoryDataGridView[0, this.dgvChosenPayCategoryDataGridView.Rows.Count - 1];

                if (this.dgvPayCategoryDataGridView.Rows.Count == 0)
                {
                    this.lblEmployeeNotSelected.Text = "";
                }
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvChosenPayCategoryDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)];

                this.dgvChosenPayCategoryDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvPayCategoryDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvPayCategoryDataGridView.CurrentCell = this.dgvPayCategoryDataGridView[0, this.dgvPayCategoryDataGridView.Rows.Count - 1];

                if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvChosenPayCategoryDataGridView, 0);
                }
                else
                {
                    Clear_DataGridView(this.dgvEmployeeDataGridView);
                    this.lblEmployee.Text = "";
                }
            }
        }

        private void Employee_WageRun()
        {
            try
            {
                pvtPayCategoryDataView = null;
                pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"], "PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'", "", DataViewRowState.CurrentRows);

                pvtPayCategoryChosenDataView = null;
                pvtPayCategoryChosenDataView = new DataView(pvtDataSet.Tables["PayCategoryChosen"], "PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "'", "", DataViewRowState.CurrentRows);
               
                if (pvtPayCategoryDataView.Count == 0
                    & pvtPayCategoryChosenDataView.Count == 0)
                {
                    object[] objParm = new object[2];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtstrPayrollType;
                    
                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Employee_WageRun", objParm);

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                    pvtDataSet.Merge(pvtTempDataSet);
                }

                if (pvtstrPayrollType == "S")
                {
                    int intComboBoxSelectedIndex = -1;
                    this.cboRunDate.Items.Clear();
                    this.cboRunDate.Visible = true;
                    this.cboRunDate.BringToFront();

                    clsISUtilities.Calender_Control_From_TextBox_SetInvisible(this.txtDate);

                    DateTime myPrevDateTime;
                    DateTime myCurrentRunDateTime = DateTime.Now.AddYears(-5);

                    if (this.pvtDataSet.Tables["PayrollType"].Rows[pvtPayrollTypeDataGridViewRowIndex]["PAY_PERIOD_DATE"] != System.DBNull.Value)
                    {
                        //Payroll Run will always fall ito This Logic
                        myCurrentRunDateTime = Convert.ToDateTime(this.pvtDataSet.Tables["PayrollType"].Rows[pvtPayrollTypeDataGridViewRowIndex]["PAY_PERIOD_DATE"]);
                    }

                    if (this.pvtDataSet.Tables["PayrollType"].Rows[pvtPayrollTypeDataGridViewRowIndex]["PREV_PAY_PERIOD_DATE"] != System.DBNull.Value)
                    {
                        //Payroll Run will always fall ito This Logic
                        myPrevDateTime = Convert.ToDateTime(this.pvtDataSet.Tables["PayrollType"].Rows[pvtPayrollTypeDataGridViewRowIndex]["PREV_PAY_PERIOD_DATE"]);
                    }
                    else
                    {
                        //Set Year so that all Dates will be accepted - Lower down in Logic
                        myPrevDateTime = new DateTime(DateTime.Now.AddMonths(-4).Year, DateTime.Now.AddMonths(-4).Month, 1).AddDays(-1);
                    }

                    //2012-10-29 Fix (Should Never Fire in Normal Circumstance)
                    if (myPrevDateTime == myCurrentRunDateTime)
                    {
                        myPrevDateTime = myCurrentRunDateTime.AddDays(1).AddMonths(-1).AddDays(-1);
                    }

                    //Load Date ComboBox
                    DateTime myDateTime;

                    if (DateTime.Now.Month > 2)
                    {
                        myDateTime = new DateTime(DateTime.Now.Year, 3, 31);

                        //ELR - 2015-03-26
                        if (myPrevDateTime < myDateTime)
                        {
                            myDateTime = myPrevDateTime.AddDays(1).AddMonths(1).AddDays(-1);
                        }
                    }
                    else
                    {
                        myDateTime = new DateTime(DateTime.Now.Year - 1, 3, 31);
                    }

                    while (true)
                    {
                        if (myPrevDateTime != null)
                        {
                            if (myDateTime > myPrevDateTime.AddMonths(12))
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (myDateTime > DateTime.Now.AddMonths(12))
                            {
                                break;
                            }
                        }

                        if (myPrevDateTime < myDateTime)
                        {
                            this.cboRunDate.Items.Add(myDateTime.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()));

                            if (myDateTime == myCurrentRunDateTime)
                            {
                                intComboBoxSelectedIndex = this.cboRunDate.Items.Count - 1;
                            }

                        }

                        //Last Day of Month
                        myDateTime = myDateTime.AddDays(1).AddMonths(1).AddDays(-1);
                    }

                    if (this.cboRunDate.Items.Count == 0)
                    {
                        this.cboRunDate.Items.Add(myDateTime.AddMonths(-1).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()));
                        this.cboRunDate.Items.Add(myDateTime.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()));

                        for (int intRow = 0;intRow < this.cboRunDate.Items.Count;intRow++)
                        {
                            if (myCurrentRunDateTime.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()) == this.cboRunDate.Items[intRow].ToString())
                            {
                                this.cboRunDate.SelectedIndex = intRow;
                                break;
                            }
                        }
                    }
                    else
                    {
                        this.cboRunDate.SelectedIndex = intComboBoxSelectedIndex;
                    }
                }
                else
                {
                    clsISUtilities.Calender_Control_From_TextBox_SetVisible(this.txtDate);
                    this.cboRunDate.Visible = false;

                    if (this.pvtDataSet.Tables["PayrollType"].Rows[pvtPayrollTypeDataGridViewRowIndex]["PAY_PERIOD_DATE"] != System.DBNull.Value)
                    {
                        this.txtDate.Text = Convert.ToDateTime(this.pvtDataSet.Tables["PayrollType"].Rows[pvtPayrollTypeDataGridViewRowIndex]["PAY_PERIOD_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());
                    }
                    else
                    {
                        this.txtDate.Text = "";
                    }
                }

                if (this.pvtDataSet.Tables["PayrollType"].Rows[pvtPayrollTypeDataGridViewRowIndex]["PREV_PAY_PERIOD_DATE"] != System.DBNull.Value)
                {
                    this.txtPreviousRunDate.Text = Convert.ToDateTime(this.pvtDataSet.Tables["PayrollType"].Rows[pvtPayrollTypeDataGridViewRowIndex]["PREV_PAY_PERIOD_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());
                }
                else
                {
                    this.txtPreviousRunDate.Text = "";
                }

                Load_Employees();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnAddAll_Click(object sender, System.EventArgs e)
        {
            this.pvtblnChosenPayCategoryDataGridViewLoaded = false;

        btnAddAll_Click_Continue:

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                btnAdd_Click(null, null);

                goto btnAddAll_Click_Continue;
            }

            this.pvtblnChosenPayCategoryDataGridViewLoaded = true;

            if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvChosenPayCategoryDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvChosenPayCategoryDataGridView));
            }
        }

        private void btnRemoveAll_Click(object sender, System.EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        public void btnNew_Click(object sender, System.EventArgs e)
        {
            this.Text += " - New";

            Set_Form_For_Edit();
        }

        private void Set_Form_For_Edit()
        {
            this.btnNew.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.tmrTimer.Enabled = false;
            this.grbLeaveReminder.Visible = false;
            
            this.dgvPayrollTypeDataGridView.Enabled = false;
            this.picPayrollTypeLock.Visible = true;
                    
            clsISUtilities.Calender_Control_From_TextBox_Enable(this.txtDate);

            this.cboRunDate.Enabled = true;

            clsISUtilities.Set_Form_For_Edit(false);

            this.btnAdd.Enabled = true;
            this.btnRemove.Enabled = true;
                
            this.btnAddAll.Enabled = true;
                
            this.btnRemoveAll.Enabled = true;

            this.Clear_DataGridView(this.dgvChosenPayCategoryDataGridView);
        }

        private void Set_Form_For_Read()
        {
            this.btnNew.Enabled = true;
            
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.btnAdd.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;

            this.txtDate.Text = "";

            clsISUtilities.Calender_Control_From_TextBox_Disable(this.txtDate);

            this.cboRunDate.Enabled = false;

            clsISUtilities.Set_Form_For_Read();

            this.dgvPayrollTypeDataGridView.Enabled = true;
            this.picPayrollTypeLock.Visible = false;
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (this.Text.IndexOf(" - New", 0) > 0
                | this.Text.IndexOf(" - Update", 0) > 0)
            {
                this.Text = this.Text.Substring(0, this.Text.LastIndexOf("-") - 1);
            }

            this.Refresh();

            Set_Form_For_Read();

            Employee_WageRun();
        }

        private int Save_Check()
        {
            if (pvtstrPayrollType != "S")
            {
                if (this.txtDate.Text.Trim() == "")
                {
                    CustomMessageBox.Show("Enter " + this.lblDate.Text,
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    return 1;
                }
            }
            else
            {
                if (this.cboRunDate.SelectedIndex < 0)
                {
                    CustomMessageBox.Show("Select " + this.lblDate.Text,
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    this.cboRunDate.Focus();
                    return 1;
                }
            }

            if (this.dgvChosenPayCategoryDataGridView.Rows.Count == 0)
            {
                CustomMessageBox.Show("Choose from '" + this.lblPayCategory.Text + "'",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return 1;
            }

            if (pvtstrPayrollType != "S")
            {
                DateTime dtNewDateTime = DateTime.ParseExact(this.txtDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                if (this.txtPreviousRunDate.Text != "")
                {
                    DateTime dtPrevDateTime = DateTime.ParseExact(this.txtPreviousRunDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                    if (dtPrevDateTime > dtNewDateTime)
                    {
                        CustomMessageBox.Show("Run Date '" + this.txtDate.Text + "'\n\nmust be Greater than Or Equal To\n\nPrevious Run Date '" + this.txtPreviousRunDate.Text + "'",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return 1;
                    }
                }
            }

            return 0;
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                DateTime dCurrentPayCategoryDateTime;

                string strPayCategoryNumbers = "";
                string strPayCategoryNumbersNotUsed = "";
                
                int intReturnCode = Save_Check();

                if (intReturnCode != 0)
                {
                    return;
                }

                if (pvtstrPayrollType != "S")
                {
                    dCurrentPayCategoryDateTime = DateTime.ParseExact(this.txtDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                }
                else
                {
                    dCurrentPayCategoryDateTime = DateTime.ParseExact(this.cboRunDate.SelectedItem.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                }
              
                //Insert Records For Each Pay category
                for (int intIndex = 0; intIndex < this.dgvChosenPayCategoryDataGridView.Rows.Count; intIndex++)
                {
                    if (strPayCategoryNumbers == "")
                    {
                        strPayCategoryNumbers = this.dgvChosenPayCategoryDataGridView[6, intIndex].Value.ToString();
                    }
                    else
                    {
                        strPayCategoryNumbers += "," + this.dgvChosenPayCategoryDataGridView[6, intIndex].Value.ToString();
                    }
                }
                
                for (int intIndex = 0; intIndex < this.dgvPayCategoryDataGridView.Rows.Count; intIndex++)
                {
                    if (strPayCategoryNumbersNotUsed == "")
                    {
                        strPayCategoryNumbersNotUsed = this.dgvPayCategoryDataGridView[6, intIndex].Value.ToString();
                    }
                    else
                    {
                        strPayCategoryNumbersNotUsed += "," + this.dgvPayCategoryDataGridView[6, intIndex].Value.ToString();
                    }
                }

                this.grbActivationProcess.Visible = true;
                this.picBackupBefore.Image = (Image)global::OpenPayrollRun.Properties.Resources.Question48;
                this.lblBackupText.Text = "Busy with Database Backup ...";
                this.picBackupBefore.Refresh();
                this.btnSave.Enabled = false;
                this.btnCancel.Enabled = false;
#if(DEBUG)
                DateTime waitDateTime = DateTime.Now.AddSeconds(1);
#else
                DateTime waitDateTime = DateTime.Now.AddSeconds(5);
#endif
                pvtintProcess = 1;
                this.tmrTimer.Enabled = true;

                this.txtDate.Enabled = false;
                this.cboRunDate.Enabled = false;

                object[] objParm = null;
                intReturnCode = 0;
#if (DEBUG)
#else
                objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = pvtstrPayrollType;
                
                intReturnCode = (int)clsISUtilities.DynamicFunction("Backup_DataBase", objParm);
#endif
                while (waitDateTime > DateTime.Now)
                {
                    Application.DoEvents();
                }
                
                this.tmrTimer.Enabled = false;
                this.picBackupBefore.Visible = true;
                this.pnlDatabaseBackupBefore.Visible = true;

                if (intReturnCode == 0)
                {
                    this.picBackupBefore.Image = (Image)global::OpenPayrollRun.Properties.Resources.tick48;
                    this.lblBackupText.Text = "Backup of Database Successful";

                    pvtintProcess += 1;
                    this.tmrTimer.Enabled = true;

                    this.pvtDataSet.Tables["PayrollType"].Rows[pvtPayrollTypeDataGridViewRowIndex]["PAY_PERIOD_DATE"] = dCurrentPayCategoryDateTime;
#if (DEBUG)
                    objParm = new object[3];
#else
                    objParm = new object[6];
#endif
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = dCurrentPayCategoryDateTime;
                    objParm[2] = strPayCategoryNumbers;
#if (DEBUG)
                    if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                    {
                        pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Insert_TimeAttendance_Run_Records", objParm);
                    }
                    else
                    {
                        if (pvtstrPayrollType == "W")
                        {
                            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Insert_Wage_Run_Records", objParm);
                        }
                        else
                        {
                            pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Insert_Salary_Run_Records", objParm);
                        }
                    }
#else
                    objParm[3] = strPayCategoryNumbersNotUsed;
                    objParm[4] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                    if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                    {
                         objParm[5] = "T";
                    }
                    else
                    {
                        if (pvtstrPayrollType == "W")
                        {
                            objParm[5] = "W";
                        }
                        else
                        {
                            objParm[5] = "S";
                        }
                    }

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Insert_Open_Into_Queue", objParm);
#endif
                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                    if (pvtTempDataSet.Tables["Upload"] != null
                    || pvtTempDataSet.Tables["LeaveAuthorisationError"] != null)
                    {
                        grbActivationProcess.Visible = false;
                        this.tmrTimer.Enabled = true;

                        if (pvtTempDataSet.Tables["LeaveAuthorisationError"] != null)
                        {
                            this.pvtDataSet.Tables["PayrollType"].Rows[pvtPayrollTypeDataGridViewRowIndex]["PAY_PERIOD_DATE"] = System.DBNull.Value;

                            CustomMessageBox.Show("There are Employees with Leave that has Not been Authorised.\n\nRun Cancelled.",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                            goto btnSave_Click_Continue;
                        }
                        else
                        {
                            for (int intIndex = 0; intIndex < dgvChosenPayCategoryDataGridView.Rows.Count; intIndex++)
                            {
                                DataView PublicHolidayErrorDataView = new DataView(pvtTempDataSet.Tables["Upload"],
                                        "PAY_CATEGORY_NO = " + this.dgvChosenPayCategoryDataGridView[6, intIndex].Value.ToString(),
                                        "",
                                        DataViewRowState.CurrentRows);

                                if (PublicHolidayErrorDataView[0]["PUBLIC_HOLIDAYS_ERROR"].ToString() == "Y")
                                {
                                    this.dgvChosenPayCategoryDataGridView[0, intIndex].Style = this.ErrorDataGridViewCellStyle;
                                }
                            }

                            CustomMessageBox.Show("There are more than 5 Public Holidays in Run.\n\nThese Cost Centres are Highlighted in Red (Error).\n\nSpeak to System Administrator.\n\nRun Cancelled.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                            this.btnSave.Enabled = true;
                            this.btnCancel.Enabled = true;

                            return;
                        }
                    }
                    else
                    {
                        //Remove - Will be Replaced
                        this.pvtDataSet.Tables.Remove("EmployeeNotInRun");
                        this.pvtDataSet.Tables.Remove("PayCategory");
                        this.pvtDataSet.Tables.Remove("PayCategoryChosen");

                        pvtDataSet.Merge(pvtTempDataSet);

                        pvtDataSet.AcceptChanges();
                    }

                    this.tmrTimer.Enabled = false;

                    this.Refresh();
#if (DEBUG)
                    this.dgvPayrollTypeDataGridView[0, pvtPayrollTypeDataGridViewRowIndex].Style = this.PayrollTypeDateOpenDataGridViewCellStyle;

                    if (pvtTempDataSet.Tables["PublicHolidayTest"] == null)
                    {
                        if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                        {
                            CustomMessageBox.Show("Open Time Attendance Run Successful.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        }
                        else
                        {
                            if (pvtstrPayrollType == "W")
                            {
                                CustomMessageBox.Show("Open Wage Run Successful.",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            }
                            else
                            {
                                CustomMessageBox.Show("Open Salary Run Successful.",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            }
                        }
                    }
#else
                    Busy_With_Run();
                    return;
#endif
                }
                else
                {
                    this.picBackupBefore.Image = (Image)global::OpenPayrollRun.Properties.Resources.Cross48;

                    this.lblBackupText.Text = "Backup of Database UNSUCCESSFUL";

                    CustomMessageBox.Show("Backup of Database Failed.\n\nSpeak To your System Administrator.",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }

                grbActivationProcess.Visible = false;
                
                btnSave_Click_Continue:
                
                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    CustomMessageBox.Show("Open Time Attendance Run UNSUCCESSFUL.\n\nSpeak To your System Administrator.",
                       this.Text,
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                }
                else
                {
                    CustomMessageBox.Show("Open Payroll Run UNSUCCESSFUL.\n\nSpeak To your System Administrator.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Busy_With_Run()
        {
            this.grbLeaveReminder.Visible = false;
            this.tmrTimer.Enabled = false;
            this.grbStatus.Visible = true;
            this.tmrTimerRun.Enabled = true;
            
            lblRunStatus.Text = "Busy with Open ...";

            this.picStatus.Image = (Image)global::OpenPayrollRun.Properties.Resources.Question48;
            this.pnlImage.Visible = true;
            pvtintNextQueueCheck = 3;
            
            this.btnDelete.Enabled = false;
            this.btnNew.Enabled = false;
            this.dgvPayrollTypeDataGridView.Enabled = false;
        }


        private void Set_Run_Failed()
        {
            this.grbLeaveReminder.Visible = false;
            this.tmrTimer.Enabled = false;
            this.grbStatus.Visible = true;
            this.tmrTimerRun.Enabled = false;

            this.lblRunStatus.Text = "Open Run FAILED\n\nSpeak to Administrator.";

            this.dgvPayrollTypeDataGridView[0, pvtPayrollTypeDataGridViewRowIndex].Style = PayrollTypeDateOpenDataGridViewCellStyle;
            this.dgvPayrollTypeDataGridView[1, pvtPayrollTypeDataGridViewRowIndex].Style = this.ErrorDataGridViewCellStyle;

            this.dgvPayrollTypeDataGridView[3, pvtPayrollTypeDataGridViewRowIndex].Value = "F";

            this.picStatus.Image = (Image)global::OpenPayrollRun.Properties.Resources.Cross48;
            pnlImage.Visible = true;
            this.btnDelete.Enabled = true;
            this.btnNew.Enabled = false;
        }

        private void Load_Employees()
        {
            bool blnEmployeesActive = true;
            string strLastUploadDate = "";
            string strLastRunDate = "";
            string strLastRunDateYYYYMMDD = "";
            string strLastUploadDateYYYYMMDDHHMM = "";

            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvChosenPayCategoryDataGridView);

            this.pvtblnPayCategoryDataGridViewLoaded = false;

            if (this.pvtPayCategoryDataView.Count == 0)
            {
                this.btnNew.Enabled = false;
                this.btnDelete.Enabled = false;

                this.lblEmployeeNotSelected.Text = "";
            }
            else
            {
                this.btnNew.Enabled = true;
                
                //Pay category Desc
                for (int intIndex = 0; intIndex < pvtPayCategoryDataView.Count; intIndex++)
                {
                    if (pvtPayCategoryDataView[intIndex]["LAST_UPLOAD_DATETIME"] != System.DBNull.Value)
                    {
                        strLastUploadDate = Convert.ToDateTime(pvtPayCategoryDataView[intIndex]["LAST_UPLOAD_DATETIME"]).ToString("dd MMM yyyy - HH:mm");
                        strLastUploadDateYYYYMMDDHHMM = Convert.ToDateTime(pvtPayCategoryDataView[intIndex]["LAST_UPLOAD_DATETIME"]).ToString("yyyyMMddHHmm");
                    }
                    else
                    {
                        strLastUploadDate = "";
                        strLastUploadDateYYYYMMDDHHMM = "";
                    }

                    if (pvtPayCategoryDataView[intIndex]["MAX_PAY_PERIOD_DATE"] != System.DBNull.Value)
                    {
                        strLastRunDate = Convert.ToDateTime(pvtPayCategoryDataView[intIndex]["MAX_PAY_PERIOD_DATE"]).ToString("dd MMM yyyy");
                        strLastRunDateYYYYMMDD = Convert.ToDateTime(pvtPayCategoryDataView[intIndex]["MAX_PAY_PERIOD_DATE"]).ToString("yyyyMMdd");
                    }
                    else
                    {
                        strLastRunDate = "";
                        strLastRunDateYYYYMMDD = "";
                    }

                    this.dgvPayCategoryDataGridView.Rows.Add("",
                                                             pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                             strLastUploadDate,
                                                             strLastRunDate,
                                                             strLastUploadDateYYYYMMDDHHMM,
                                                             strLastRunDateYYYYMMDD,
                                                             pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());

                    pvtNotActiveDataView = null;
                    pvtNotActiveDataView = new DataView(pvtDataSet.Tables["EmployeeNotInRun"],
                        "PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString(),
                        "",
                        DataViewRowState.CurrentRows);

                    if (pvtNotActiveDataView.Count > 0)
                    {
                        this.dgvPayCategoryDataGridView[0,this.dgvPayCategoryDataGridView.Rows.Count - 1].Style = EmployeeNotActiveDataGridViewCellStyle;
                        blnEmployeesActive = false;
                    }
                }
            }

            this.pvtblnPayCategoryDataGridViewLoaded = true;

            if (dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, 0);
            }

            this.Clear_DataGridView(this.dgvEmployeeDataGridView);

            this.pvtblnChosenPayCategoryDataGridViewLoaded = false;
            
            if (pvtPayCategoryChosenDataView.Count == 0)
            {
                this.btnDelete.Enabled = false;

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    grbLeaveReminder.Visible = false;
                }
                else
                {
                    this.tmrTimer.Enabled = true;
                }

                this.lblEmployee.Text = "";
            }
            else
            {
                this.btnNew.Enabled = false;
                this.btnDelete.Enabled = true;
               
                for (int intIndex = 0; intIndex < pvtPayCategoryChosenDataView.Count; intIndex++)
                {
                    if (pvtPayCategoryChosenDataView[intIndex]["LAST_UPLOAD_DATETIME"] != System.DBNull.Value)
                    {
                        strLastUploadDate = Convert.ToDateTime(pvtPayCategoryChosenDataView[intIndex]["LAST_UPLOAD_DATETIME"]).ToString("dd MMM yyyy - HH:mm");
                        strLastUploadDateYYYYMMDDHHMM = Convert.ToDateTime(pvtPayCategoryChosenDataView[intIndex]["LAST_UPLOAD_DATETIME"]).ToString("yyyyMMddHHmm");
                    }
                    else
                    {
                        strLastUploadDate = "";
                        strLastUploadDateYYYYMMDDHHMM = "";
                    }

                    if (pvtPayCategoryChosenDataView[intIndex]["MAX_PAY_PERIOD_DATE"] != System.DBNull.Value)
                    {
                        strLastRunDate = Convert.ToDateTime(pvtPayCategoryChosenDataView[intIndex]["MAX_PAY_PERIOD_DATE"]).ToString("dd MMM yyyy");
                        strLastRunDateYYYYMMDD = Convert.ToDateTime(pvtPayCategoryChosenDataView[intIndex]["MAX_PAY_PERIOD_DATE"]).ToString("yyyyMMdd");

                    }
                    else
                    {
                        strLastRunDate = "";
                        strLastRunDateYYYYMMDD = "";
                    }

                    this.dgvChosenPayCategoryDataGridView.Rows.Add("",
                                                                   pvtPayCategoryChosenDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                                   strLastUploadDate,
                                                                   strLastRunDate,
                                                                   strLastUploadDateYYYYMMDDHHMM,
                                                                   strLastRunDateYYYYMMDD,
                                                                   pvtPayCategoryChosenDataView[intIndex]["PAY_CATEGORY_NO"].ToString());
                 
                    pvtNotActiveDataView = null;
                    pvtNotActiveDataView = new DataView(pvtDataSet.Tables["EmployeeNotInRun"],
                        "PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + pvtPayCategoryChosenDataView[intIndex]["PAY_CATEGORY_NO"].ToString(),
                        "",
                        DataViewRowState.CurrentRows);

                    if (pvtNotActiveDataView.Count > 0)
                    {
                        this.dgvChosenPayCategoryDataGridView[0,this.dgvChosenPayCategoryDataGridView.Rows.Count - 1].Style = EmployeeNotActiveDataGridViewCellStyle;
                        blnEmployeesActive = false;
                    }
                }
            }

            if (blnEmployeesActive == true)
            {
                this.lblEmployee.Visible = false;
                this.dgvEmployeeDataGridView.Visible = false;

                this.lblEmployeeNotSelected.Visible = false;
                this.dgvEmployeeNotSelectedDataGridView.Visible = false;

                this.grbLegend.Visible = false;

                if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
                && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
                {
                    this.dgvPayCategoryDataGridView.Height = 421 + 114;
                    this.dgvChosenPayCategoryDataGridView.Height = 364 + 114;
                }
                else
                {
                    this.dgvPayCategoryDataGridView.Height = 421;
                    this.dgvChosenPayCategoryDataGridView.Height = 364;
                }
            }
            else
            {
                this.lblEmployee.Visible = true;
                this.dgvEmployeeDataGridView.Visible = true;

                this.lblEmployeeNotSelected.Visible = true;
                this.dgvEmployeeNotSelectedDataGridView.Visible = true;

                this.grbLegend.Visible = true;

                if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
                && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
                {
                    this.dgvPayCategoryDataGridView.Height = 193 + 114;
                    this.dgvChosenPayCategoryDataGridView.Height = 174 + 114;
                }
                else
                {
                    this.dgvPayCategoryDataGridView.Height = 193;
                    this.dgvChosenPayCategoryDataGridView.Height = 174;
                }
            }

            this.Refresh();

            this.pvtblnChosenPayCategoryDataGridViewLoaded = true;

            if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvChosenPayCategoryDataGridView, 0);
            }
        }

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                DateTime dCurrentPayCategoryDateTime;
                                
                DialogResult dlgResult = CustomMessageBox.Show("Are you sure you want to Delete All Selected Cost Centre/s?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation);

                if (dlgResult == DialogResult.Yes)
                {
                    this.Refresh();

                    if (pvtstrPayrollType != "S")
                    {
                       dCurrentPayCategoryDateTime = DateTime.ParseExact(this.txtDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                    }
                    else
                    {
                        //dCurrentPayCategoryDateTime = DateTime.ParseExact("2015-02-28", "yyyy-MM-dd",null);
                        dCurrentPayCategoryDateTime = DateTime.ParseExact(this.cboRunDate.SelectedItem.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                    }

                    //Delete Pay Categories
                    string strPayCategoryNos = "";

                    for (int intRow = 0; intRow < this.dgvChosenPayCategoryDataGridView.Rows.Count; intRow++)
                    {
                        if (intRow == 0)
                        {
                            strPayCategoryNos = this.dgvChosenPayCategoryDataGridView[6,intRow].Value.ToString();
                        }
                        else
                        {
                            strPayCategoryNos += "|" + this.dgvChosenPayCategoryDataGridView[6,intRow].Value.ToString();
                        }
                    }
                    
                    object[] objParm = new object[5];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[2] = strPayCategoryNos;
                    objParm[3] = pvtstrPayrollType;
                    objParm[4] = dCurrentPayCategoryDateTime;
                   
                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Delete_Records", objParm);

                    //Remove - Will be Replaced
                    this.pvtDataSet.Tables.Remove("EmployeeNotInRun");
                    this.pvtDataSet.Tables.Remove("PayCategory");
                    this.pvtDataSet.Tables.Remove("PayCategoryChosen");

                    this.pvtDataSet.Tables["PayrollType"].Rows[pvtPayrollTypeDataGridViewRowIndex]["PAY_PERIOD_DATE"] = System.DBNull.Value;

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                    pvtDataSet.Merge(pvtTempDataSet);

                    pvtDataSet.AcceptChanges();

                    this.grbActivationProcess.Visible = false;
                    this.txtDate.Text = "";
                    this.cboRunDate.SelectedIndex = -1;

                    this.dgvPayrollTypeDataGridView[0,pvtPayrollTypeDataGridViewRowIndex].Style = this.NormalDataGridViewCellStyle;
                    this.dgvPayrollTypeDataGridView[1, pvtPayrollTypeDataGridViewRowIndex].Style = NormalDataGridViewCellStyle;
                    //Set Status to Normal
                    this.dgvPayrollTypeDataGridView[3, pvtPayrollTypeDataGridViewRowIndex].Value = "";
                    this.grbStatus.Visible = false;
                    
                    if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                    {
                        CustomMessageBox.Show("Delete of Time Attendance Run Records Successful.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    }
                    else
                    {
                        if (pvtstrPayrollType == "W")
                        {
                            CustomMessageBox.Show("Delete of Wage Run Records Successful.",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            CustomMessageBox.Show("Delete of Salary Run Records Successful.",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                        }
                    }

                    //Refresh ListBoxes
                    btnCancel_Click(sender, e);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            if (pvtintTimerCount == 4
                & pvtintProcess == 0)
            {
                this.tmrTimer.Enabled = false;
            }
            else
            {
                pvtintTimerCount += 1;

                switch (pvtintProcess)
                {
                    case 0:

                        if (this.grbLeaveReminder.Visible == true)
                        {
                            this.grbLeaveReminder.Visible = false;
                        }
                        else
                        {
                            this.grbLeaveReminder.Visible = true;
                        }

                        break;

                    case 1:

                        if (this.pnlDatabaseBackupBefore.Visible == true)
                        {
                            this.pnlDatabaseBackupBefore.Visible = false;
                        }
                        else
                        {
                            this.pnlDatabaseBackupBefore.Visible = true;
                        }

                        break;

                }
            }
        }

        private void dgvPayrollTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayrollTypeDataGridViewLoaded == true)
            {
                if (pvtPayrollTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtPayrollTypeDataGridViewRowIndex = e.RowIndex;

                    pvtstrPayrollType = this.dgvPayrollTypeDataGridView[2, e.RowIndex].Value.ToString().Substring(0, 1);

                    if (pvtstrPayrollType == "W")
                    {
                        this.grbActivationProcess.Text = "Backup of Database Before Open Wage Run";
                    }
                    else
                    {
                        if (pvtstrPayrollType == "S")
                        {
                            this.grbActivationProcess.Text = "Backup of Database Before Open Salary Run";
                        }
                        else
                        {
                            this.grbActivationProcess.Text = "Backup of Database Before Open Time Attendance Run";
                        }
                    }

                    this.grbActivationProcess.Visible = false;

                    if (this.dgvPayrollTypeDataGridView[3, e.RowIndex].Value.ToString() == "")
                    {
                        this.grbStatus.Visible = false;
                    }
                   
                    this.txtPreviousRunDate.Text = "";

                    Load_CurrentForm_Records();

                    if (this.dgvPayrollTypeDataGridView[3, e.RowIndex].Value.ToString() != "")
                    {
                        if (this.dgvPayrollTypeDataGridView[3, e.RowIndex].Value.ToString() == "F")
                        {
                            this.lblRunStatus.Text = "Open Run FAILED\n\nSpeak to Administrator.";
                            this.picStatus.Image = (Image)global::OpenPayrollRun.Properties.Resources.Cross48;

                            this.btnNew.Enabled = false;
                            this.btnDelete.Enabled = true;
                        }

                        pnlImage.Visible = true;
                        this.grbStatus.Visible = true;
                    }
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

        public void Set_DataGridView_SelectedRowIndex(DataGridView myDataGridView, int intRow)
        {
            //Fires DataGridView RowEnter Function
            if (this.Get_DataGridView_SelectedRowIndex(myDataGridView) == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvPayrollTypeDataGridView":

                        pvtintPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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
      
        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    this.lblEmployeeNotSelected.Text = "Employee NOT Activated for Cost Centre - " + this.dgvPayCategoryDataGridView[1, e.RowIndex].Value.ToString();

                    this.Clear_DataGridView(this.dgvEmployeeNotSelectedDataGridView);

                    //Find Employees for Pay Category NOT Taken On
                    pvtEmployeeNotInRunDataView = null;
                    pvtEmployeeNotInRunDataView = new DataView(pvtDataSet.Tables["EmployeeNotInRun"],
                            "PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + this.dgvPayCategoryDataGridView[6, e.RowIndex].Value.ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                    if (pvtEmployeeNotInRunDataView.Count != 0)
                    {
                        for (int intRow = 0; intRow < this.pvtEmployeeNotInRunDataView.Count; intRow++)
                        {
                            this.dgvEmployeeNotSelectedDataGridView.Rows.Add("",
                                                                             pvtEmployeeNotInRunDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                             pvtEmployeeNotInRunDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                             pvtEmployeeNotInRunDataView[intRow]["EMPLOYEE_NAME"].ToString());

                            this.dgvEmployeeNotSelectedDataGridView[0,this.dgvEmployeeNotSelectedDataGridView.Rows.Count - 1].Style = EmployeeNotActiveDataGridViewCellStyle;
                        }
                    }
                }
            }
        }

        private void dgvChosenPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnChosenPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintChosenPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintChosenPayCategoryDataGridViewRowIndex = e.RowIndex;

                    this.lblEmployee.Text = "Employee NOT Activated for Cost Centre - " + this.dgvChosenPayCategoryDataGridView[1, e.RowIndex].Value.ToString();

                    this.Clear_DataGridView(this.dgvEmployeeDataGridView);

                    //Find Employees for Pay Category NOT Taken On
                    pvtEmployeeNotInRunDataView = null;
                    pvtEmployeeNotInRunDataView = new DataView(pvtDataSet.Tables["EmployeeNotInRun"],
                            "PAY_CATEGORY_TYPE = '" + pvtstrPayrollType + "' AND PAY_CATEGORY_NO = " + this.dgvChosenPayCategoryDataGridView[6, e.RowIndex].Value.ToString(),
                            "",
                            DataViewRowState.CurrentRows);

                    if (pvtEmployeeNotInRunDataView.Count != 0)
                    {
                        for (int intRow = 0; intRow < this.pvtEmployeeNotInRunDataView.Count; intRow++)
                        {
                            this.dgvEmployeeDataGridView.Rows.Add("",
                                                                  pvtEmployeeNotInRunDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                  pvtEmployeeNotInRunDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                  pvtEmployeeNotInRunDataView[intRow]["EMPLOYEE_NAME"].ToString());

                            this.dgvEmployeeDataGridView[0,this.dgvEmployeeDataGridView.Rows.Count - 1].Style = EmployeeNotActiveDataGridViewCellStyle;
                        }
                    }
                }
            }
        }

        private void dgvPayCategoryDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 2
            ||  e.Column.Index == 3)
                {
                if (dgvPayCategoryDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvPayCategoryDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString()) > double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString()) < double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString()))
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

        private void dgvChosenPayCategoryDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 2
           || e.Column.Index == 3)
            {
                if (dgvChosenPayCategoryDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvChosenPayCategoryDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvChosenPayCategoryDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString()) > double.Parse(dgvChosenPayCategoryDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvChosenPayCategoryDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString()) < double.Parse(dgvChosenPayCategoryDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString()))
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

        private void dgvPayCategoryDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnAdd_Click(sender, e);
            }
        }

        private void dgvChosenPayCategoryDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnRemove_Click(sender, e);
            }
        }

        private void tmrTimerRun_Tick(object sender, EventArgs e)
        {
            if (pnlImage.Visible == true)
            {
                pnlImage.Visible = false;
            }
            else
            {
                pnlImage.Visible = true;
            }

            if (pvtintNextQueueCheck == 5)
            {
                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    objParm[1] = "T";
                }
                else
                {
                    objParm[1] = pvtstrPayrollType;
                }

                //Reset to Check in 5 Seconds
                pvtintNextQueueCheck = 0;
                byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Check_Queue", objParm);
                
                DataSet TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                //S=Started,E=Timesheets Error,F=Job Failed
                if (TempDataSet.Tables["Reply"].Rows[0]["CHECK_QUEUE_IND"].ToString() == "")
                {
                    //Successful Run
                    this.tmrTimerRun.Enabled = false;
                    Set_Form_For_Read();
                    this.btnDelete.Enabled = true;
                    lblRunStatus.Text = "Run Completed Successfully";
                    this.picStatus.Image = global::OpenPayrollRun.Properties.Resources.tick48;

                    this.dgvPayrollTypeDataGridView[0, pvtPayrollTypeDataGridViewRowIndex].Style = PayrollTypeDateOpenDataGridViewCellStyle;
                    //Reset Status
                    this.dgvPayrollTypeDataGridView[3, pvtPayrollTypeDataGridViewRowIndex].Value = "";
                    
                    this.grbStatus.Visible = true;
                    this.pnlImage.Visible = true;

                    //2017-09-05
                    TempDataSet.Tables.Remove("Reply");

                    this.pvtDataSet.Tables.Remove("EmployeeNotInRun");
                    this.pvtDataSet.Tables.Remove("PayCategory");
                    this.pvtDataSet.Tables.Remove("PayCategoryChosen");

                    pvtDataSet.Merge(TempDataSet);

                    pvtDataSet.AcceptChanges();

                    btnCancel_Click(sender, e);
                }
                else
                {
                    if (TempDataSet.Tables["Reply"].Rows[0]["CHECK_QUEUE_IND"].ToString() == "F"
                    || TempDataSet.Tables["Reply"].Rows[0]["CHECK_QUEUE_IND"].ToString() == "E")
                    {
                        this.tmrTimerRun.Enabled = false;
                        Set_Form_For_Read();
                        Set_Run_Failed();

                        //2017-09-05
                        TempDataSet.Tables.Remove("Reply");

                        this.pvtDataSet.Tables.Remove("EmployeeNotInRun");
                        this.pvtDataSet.Tables.Remove("PayCategory");
                        this.pvtDataSet.Tables.Remove("PayCategoryChosen");

                        pvtDataSet.Merge(TempDataSet);

                        pvtDataSet.AcceptChanges();

                        this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView));
                    }
                }
            }
            else
            {
                pvtintNextQueueCheck += 1;
            }
        }
    }
}
