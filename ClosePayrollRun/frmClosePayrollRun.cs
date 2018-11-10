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
    public partial class frmClosePayrollRun : Form
    {
        clsISUtilities clsISUtilities;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtPayCategoryDataView;
        private DataTable pvtDataTable;
        private DataRow pvtDataRow;
      
        private int pvtintProcess = 0;
        private string pvtstrRunType = "";

        int pvtintTimerCount = 0;
        int pvtintNextQueueCheck = 0;

        private byte[] pvtbytCompress;

        private bool pvtblnRecordInQueue = false;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnChosenPayCategoryDataGridViewLoaded = false;
        
        public frmClosePayrollRun()
        {
            InitializeComponent();

            grbStatus.Top = this.grbCloseReminder.Top;
            grbStatus.Left = this.grbCloseReminder.Left;
        }

        private void frmClosePayrollRun_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    btnDateClose.Text = "Close Date";

                    grbCloseReminder.Text = grbCloseReminder.Text.Replace("Payroll","Time Attendance");
                    this.lblCloseInfo.Text = this.lblCloseInfo.Text.Replace("Payroll","Time Sheet");

                    this.picWarningPicture.Image = global::ClosePayrollRun.Properties.Resources.TimeSheetTotals48;
                }

                clsISUtilities = new clsISUtilities(this,"busClosePayrollRun");

                int intTimeout = Convert.ToInt32(AppDomain.CurrentDomain.GetData("TimeSheetReadTimeoutSeconds")) * 1000;

                clsISUtilities.Set_WebService_Timeout_Value(intTimeout);

                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                pvtDataSet = new DataSet();

                pvtTempDataSet = new DataSet();
             
                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
               
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                               
                if (this.pvtDataSet.Tables["CloseQueue"].Rows.Count > 0)
                {
                    pvtblnRecordInQueue = true;

                    if (this.pvtDataSet.Tables["CloseQueue"].Rows[0]["CLOSE_RUN_QUEUE_IND"].ToString() == "S")
                    {
                        pvtstrRunType = this.pvtDataSet.Tables["CloseQueue"].Rows[0]["RUN_TYPE"].ToString();
                        Busy_With_Run();
                    }
                    else
                    {
                        Set_Run_Failed();
                    }
                }

                //ELR - 2016-07-29
                DataView PayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"],
                "PAY_CATEGORY_TYPE = 'S' AND SALARY_TIMESHEET_UPLOAD_REQUIRED_IND = 'Y'",
                "",
                DataViewRowState.CurrentRows);

                if (PayCategoryDataView.Count > 0)
                {
                    CustomMessageBox.Show("'Salaries Timesheets have not yet been UPLOADED.\n\nNB.You may continue without uploading but these Timesheets will then be Lost.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                Load_CurrentForm_Records();
                    
                if (pvtblnRecordInQueue == false)
                {
                    this.tmrTimer.Enabled = true;
                }
              
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Busy_With_Run()
        {
            this.grbCloseReminder.Visible = false;
            this.grbStatus.Visible = true;
            this.timerRun.Enabled = true;

            lblRunStatus.Text = "Busy with Close ...";

            this.picStatus.Image = (Image)global::ClosePayrollRun.Properties.Resources.Question48;
            this.pnlImage.Visible = true;
            pvtintNextQueueCheck = 3;
            this.btnDateClose.Enabled = false;
        }
            

        private void Set_Run_Failed()
        {
            this.grbCloseReminder.Visible = false;
            this.grbStatus.Visible = true;
            this.timerRun.Enabled = false;

            this.lblRunStatus.Text = "Close Run FAILED\n\nSpeak to Administrator.";

            this.picStatus.Image = (Image)global::ClosePayrollRun.Properties.Resources.Cross48;
            pnlImage.Visible = true;
        }
        
        private void Load_CurrentForm_Records()
        {
            string strFilter = "COMPANY_NO = " + AppDomain.CurrentDomain.GetData("CompanyNo").ToString();

            pvtPayCategoryDataView = null;
            pvtPayCategoryDataView = new DataView(this.pvtDataSet.Tables["PayCategory"],
                strFilter,
                "",
                DataViewRowState.CurrentRows);

            this.Clear_DataGridView(this.dgvPayCategoryDataGridView);
            this.Clear_DataGridView(this.dgvChosenPayCategoryDataGridView);

            pvtblnPayCategoryDataGridViewLoaded = false;
            pvtblnChosenPayCategoryDataGridViewLoaded = false;
                
            this.btnDateClose.Enabled = false;

            bool blnRecordsExcludeFromRun = false;
            
            for (int intIndex = 0; intIndex < pvtPayCategoryDataView.Count; intIndex++)
            {
                if (pvtPayCategoryDataView[intIndex]["PREV_PAY_PERIOD_DATE"] == System.DBNull.Value)
                {
                    if (pvtblnRecordInQueue == false)
                    {
                        this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                                 pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_TYPE"].ToString(),
                                                                 Convert.ToDateTime(pvtPayCategoryDataView[intIndex]["PAY_PERIOD_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                                 Convert.ToDateTime(pvtPayCategoryDataView[intIndex]["PREV_EMPLOYEE_LAST_RUNDATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                                 pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());
                    }
                    else
                    {
                        this.dgvChosenPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                                       pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_TYPE"].ToString(),
                                                                       Convert.ToDateTime(pvtPayCategoryDataView[intIndex]["PAY_PERIOD_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                                       Convert.ToDateTime(pvtPayCategoryDataView[intIndex]["PREV_EMPLOYEE_LAST_RUNDATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                                       pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());

                    }
                }
                else
                {
                    if (pvtblnRecordInQueue == false)
                    {
                        this.dgvPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                             pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_TYPE"].ToString(),
                                                             Convert.ToDateTime(pvtPayCategoryDataView[intIndex]["PAY_PERIOD_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                             Convert.ToDateTime(pvtPayCategoryDataView[intIndex]["PREV_PAY_PERIOD_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                             pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());
                    }
                    else
                    {
                        this.dgvChosenPayCategoryDataGridView.Rows.Add(pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_DESC"].ToString(),
                                                                       pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_TYPE"].ToString(),
                                                                       Convert.ToDateTime(pvtPayCategoryDataView[intIndex]["PAY_PERIOD_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                                       Convert.ToDateTime(pvtPayCategoryDataView[intIndex]["PREV_PAY_PERIOD_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                                       pvtPayCategoryDataView[intIndex]["PAY_CATEGORY_NO"].ToString());

                    }
                }

                if (pvtPayCategoryDataView[intIndex]["RECORDS_EXCLUDED_FROM_RUN"].ToString() == "Y")
                {
                    blnRecordsExcludeFromRun = true;
                }
            }

            pvtblnPayCategoryDataGridViewLoaded = true;
            pvtblnChosenPayCategoryDataGridViewLoaded = true;

            if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
            {
                this.btnUpdate.Enabled = true;
                this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView,0);
            }
            else
            {
                this.btnUpdate.Enabled = false;
                this.btnAddAll.Enabled = false;
                this.btnRemoveAll.Enabled = false;
            }
            
            if (blnRecordsExcludeFromRun == true)
            {
                frmRecordsExcludedFromRun frmRecordsExcludedFromRun = new frmRecordsExcludedFromRun();
                frmRecordsExcludedFromRun.ShowDialog();
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnDateClose_Click(object sender, System.EventArgs e)
        {
            try
            {
                DateTime dtWageRunDateTime = DateTime.Now;
                DateTime dtSalaryRunDateTime = DateTime.Now;

                string strRunDesc = "Payroll";

                if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                {
                    strRunDesc = "Time Attendance";
                }

                DialogResult dlgResult = CustomMessageBox.Show("Are you sure you want to Close the " + strRunDesc + " Run?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dlgResult == DialogResult.Yes)
                {
                    int intReturnCode = 0;
                    object[] objParm = null;
                    string strPayrollType = "";
                    string strWagesPayCategoryNumber = "";
                    string strSalariesPayCategoryNumber = "";

                    this.btnCancel.Enabled = false;
                    this.btnDateClose.Enabled = false;
                    this.btnAddAll.Enabled = false;
                    this.btnRemoveAll.Enabled = false;

                    this.grbActivationProcess.Visible = true;
                    this.Refresh();

                    for (int intChosenRow = 0; intChosenRow < this.dgvChosenPayCategoryDataGridView.Rows.Count; intChosenRow++)
                    {
                        if (this.dgvChosenPayCategoryDataGridView[1, intChosenRow].Value.ToString() == "S")
                        {
                            dtSalaryRunDateTime = DateTime.ParseExact(this.dgvChosenPayCategoryDataGridView[2, intChosenRow].Value.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                            if (strSalariesPayCategoryNumber == "")
                            {
                                strSalariesPayCategoryNumber = this.dgvChosenPayCategoryDataGridView[4, intChosenRow].Value.ToString();
                            }
                            else
                            {
                                strSalariesPayCategoryNumber += "," + this.dgvChosenPayCategoryDataGridView[4, intChosenRow].Value.ToString();
                            }
                        }
                        else
                        {
                            dtWageRunDateTime = DateTime.ParseExact(this.dgvChosenPayCategoryDataGridView[2, intChosenRow].Value.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);

                            if (strWagesPayCategoryNumber == "")
                            {
                                strWagesPayCategoryNumber = this.dgvChosenPayCategoryDataGridView[4, intChosenRow].Value.ToString();
                            }
                            else
                            {
                                strWagesPayCategoryNumber += "," + this.dgvChosenPayCategoryDataGridView[4, intChosenRow].Value.ToString();
                            }
                        }
                    }

                    if (strWagesPayCategoryNumber != ""
                    && strSalariesPayCategoryNumber != "")
                    {
                        strPayrollType = "B";
                    }
                    else
                    {
                        if (strWagesPayCategoryNumber != "")
                        {
                            if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                            {
                                strPayrollType = "T";
                            }
                            else
                            {
                                strPayrollType = "W";
                            }
                        }
                        else
                        {
                            strPayrollType = "S";
                        }
                    }

                    pvtintProcess = 1;
                    this.tmrTimer.Enabled = true;

#if (DEBUG)
                    DateTime waitDateTime = DateTime.Now.AddSeconds(1);
#else
                    DateTime waitDateTime = DateTime.Now.AddSeconds(5);
#endif

                    objParm = null;
                    objParm = new object[2];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = strPayrollType;

                    intReturnCode = (int)clsISUtilities.DynamicFunction("Backup_DataBase", objParm);

                    while (waitDateTime > DateTime.Now)
                    {
                        Application.DoEvents();
                    }

                    this.tmrTimer.Enabled = false;
                    this.picBackupBefore.Visible = true;

                    if (intReturnCode == 0)
                    {
                        this.picBackupBefore.Image = (Image)global::ClosePayrollRun.Properties.Resources.tick48;
                        
                        lblDatabaseBackupBeforeClose.Text = "Backup of Database Successful";

                        this.Refresh();

                        //Compress DataSet
                        pvtbytCompress = clsISUtilities.Compress_DataSet(pvtTempDataSet);

                        objParm = null;
#if (DEBUG)
                        if (strPayrollType == "B")
                        {
                            objParm = new object[6];
                        }
                        else
                        {
                            objParm = new object[4];
                        }

                        objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                        if (strPayrollType == "B")
                        {
                            objParm[1] = dtWageRunDateTime;
                            objParm[2] = dtSalaryRunDateTime;
                            objParm[3] = strWagesPayCategoryNumber;
                            objParm[4] = strSalariesPayCategoryNumber;
                            objParm[5] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                        }
                        else
                        {
                            if (strPayrollType == "S")
                            {
                                objParm[1] = dtSalaryRunDateTime;
                                objParm[2] = strSalariesPayCategoryNumber;
                                objParm[3] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                            }
                            else
                            {
                                objParm[1] = dtWageRunDateTime;
                                objParm[2] = strWagesPayCategoryNumber;
                                objParm[3] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                            }
                        }

                        if (strPayrollType == "B")
                        {
                            intReturnCode = (int)clsISUtilities.DynamicFunction("Close_Both_Run", objParm);
                        }
                        else
                        {
                            if (strPayrollType == "W")
                            {
                                intReturnCode = (int)clsISUtilities.DynamicFunction("Close_Wage_Run", objParm);
                            }
                            else
                            {
                                if (strPayrollType == "S")
                                {
                                    intReturnCode = (int)clsISUtilities.DynamicFunction("Close_Salary_Run", objParm);
                                }
                                else
                                {
                                    intReturnCode = (int)clsISUtilities.DynamicFunction("Close_TimeAttendance_Run", objParm);
                                }
                            }
                        }

                        if (intReturnCode == 0)
                        {
                            CustomMessageBox.Show("Close " + strRunDesc + " Run Successful.",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            CustomMessageBox.Show("CLOSE " + strRunDesc + " Run UNSUCCESSFUL.\n\nSpeak to your System Administrator.",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
#else
                        objParm = new object[7];

                        objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                        objParm[1] = dtWageRunDateTime;
                        objParm[2] = dtSalaryRunDateTime;
                        objParm[3] = strPayrollType;
                        objParm[4] = strWagesPayCategoryNumber;
                        objParm[5] = strSalariesPayCategoryNumber;
                        objParm[6] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                        intReturnCode = (int)clsISUtilities.DynamicFunction("Insert_Close_Into_Queue", objParm);

                        if (intReturnCode == 0)
                        {
                            pvtstrRunType = strPayrollType;
                            Busy_With_Run();
                        }
                        else
                        {
                            CustomMessageBox.Show("Insert Close Job into Queue FAILED.\n\nSpeak To your System Administrator.",
                                this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        }
#endif
                    }
                    else
                    {
                        this.picBackupBefore.Image = (Image)global::ClosePayrollRun.Properties.Resources.Cross48;
                        this.Refresh();

                        this.lblDatabaseBackupBeforeClose.Text = "Backup of Database UNSUCCESSFUL";

                        CustomMessageBox.Show("Backup of Database Failed.\n\nSpeak To your System Administrator.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnRemoveAll_Click(object sender, System.EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.btnCancel.Enabled == true)
            {
                if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvChosenPayCategoryDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenPayCategoryDataGridView)];

                    this.dgvChosenPayCategoryDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvPayCategoryDataGridView.Rows.Add(myDataGridViewRow);

                    this.dgvPayCategoryDataGridView.CurrentCell = this.dgvPayCategoryDataGridView[0, this.dgvPayCategoryDataGridView.Rows.Count - 1];

                    if (this.dgvChosenPayCategoryDataGridView.Rows.Count > 0)
                    {
                        goto btnRemoveAll_Click_Continue;
                    }

                    if (this.dgvChosenPayCategoryDataGridView.Rows.Count == 0)
                    {
                        this.btnDateClose.Enabled = false;
                    }
                }
            }
        }
        
        private void btnAddAll_Click(object sender, System.EventArgs e)
        {
        btnAddAll_Click_Continue:

            if (this.btnCancel.Enabled == true)
            {
                if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                {
                    DataGridViewRow myDataGridViewRow = this.dgvPayCategoryDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvPayCategoryDataGridView)];

                    this.dgvPayCategoryDataGridView.Rows.Remove(myDataGridViewRow);

                    this.dgvChosenPayCategoryDataGridView.Rows.Add(myDataGridViewRow);

                    this.dgvChosenPayCategoryDataGridView.CurrentCell = this.dgvChosenPayCategoryDataGridView[0, this.dgvChosenPayCategoryDataGridView.Rows.Count - 1];

                    if (this.dgvPayCategoryDataGridView.Rows.Count > 0)
                    {
                        goto btnAddAll_Click_Continue;
                    }

                    this.btnDateClose.Enabled = true;
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

        public void Set_DataGridView_SelectedRowIndex(DataGridView myDataGridView, int intRow)
        {
            //Fires DataGridView RowEnter Function
            if (this.Get_DataGridView_SelectedRowIndex(myDataGridView) == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvPayCategoryDataGridView":

                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            this.btnUpdate.Enabled = false;

            this.btnCancel.Enabled = true;

            this.btnAddAll.Enabled = true;
            this.btnRemoveAll.Enabled = true;
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf("-") - 1);

            this.btnUpdate.Enabled = true;

            this.btnCancel.Enabled = false;
            
            this.btnAddAll.Enabled = false;
            this.btnRemoveAll.Enabled = false;

            Load_CurrentForm_Records();
        }
               

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void timerRun_Tick(object sender, EventArgs e)
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
                objParm[1] = pvtstrRunType;

                //Reset to Check in 5 Seconds
                pvtintNextQueueCheck = 0;
                string strReturn = (string)clsISUtilities.DynamicFunction("Check_Queue", objParm);

                //S=Started,E=Timesheets Error,F=Job Failed
                if (strReturn == "")
                {
                    //Successful Run
                    this.timerRun.Enabled = false;
                    //Set_Form_For_Read();
               
                    lblRunStatus.Text = "Close Completed Successfully";
                    this.picStatus.Image = global::ClosePayrollRun.Properties.Resources.tick48;

                    //this.grbStatus.Visible = true;
                    this.pnlImage.Visible = true;
                }
                else
                {
                    if (strReturn == "F")
                    {
                        this.timerRun.Enabled = false;
                        //Set_Form_For_Read();
                        Set_Run_Failed();
                    }
                }
            }
            else
            {
                pvtintNextQueueCheck += 1;
            }
        }

        private void dgvChosenPayCategoryDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnUpdate.Enabled == false)
            {
                btnRemoveAll_Click(sender, e);
            }
        }

        private void dgvPayCategoryDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnUpdate.Enabled == false)
            {
                btnAddAll_Click(sender, e);
            }
        }
        
        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            if (pvtintTimerCount == 6
            && pvtintProcess == 0)
            {
                this.tmrTimer.Enabled = false;
            }
            else
            {
                pvtintTimerCount += 1;

                switch (pvtintProcess)
                {
                    case 0:

                        if (this.picWarningPicture.Visible == true)
                        {
                            this.picWarningPicture.Visible = false;
                            this.lblCloseInfo.Visible = false;
                        }
                        else
                        {
                            this.picWarningPicture.Visible = true;
                            this.lblCloseInfo.Visible = true;
                        }

                        break;

                    case 1:

                        if (this.picBackupBefore.Visible == true)
                        {
                            this.picBackupBefore.Visible = false;
                        }
                        else
                        {
                            this.picBackupBefore.Visible = true;
                        }

                        break;
                }
            }
        }
    }
}
