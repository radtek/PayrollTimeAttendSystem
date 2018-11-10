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
    public partial class frmEmployeeActivateSelection : Form
    {
        clsISUtilities clsISUtilities;

        private byte[] pvtbytCompress;
        private int pvtintRunNo = 1;

        private int pvtintProcess = 0;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataSet pvtUploadDataSet;
        private DataView pvtSalaryDateDataView;
        private DataTable pvtDataTable;
      
        private DataView pvtEmployeeDataView;

        private bool pvtblnPayrollTypeDataGridViewLoaded = false;
        private bool pvtblnEmployeeDataGridViewLoaded = false;
        private bool pvtblnChosenEmployeeDataGridViewLoaded = false;

        private int pvtintPayrollTypeDataGridViewRowIndex = -1;

        Pen Pen3Pixels;
        Pen Pen1Pixel;

        private string pvtstrPayrollType = "";

        public frmEmployeeActivateSelection()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void frmEmployeeActivateSelection_Load(object sender, System.EventArgs e)
        {
            try
            {
                Pen3Pixels = new Pen(Color.Black, 3);
                Pen1Pixel = new Pen(Color.Black, 1);

                clsISUtilities = new clsISUtilities(this,"busEmployeeActivateSelection");

                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                clsISUtilities.Create_Calender_Control_From_TextBox(this.txtDate);

                clsISUtilities.NotDataBound_Date_TextBox(txtDate, "Capture Tax Effective Date.");
                clsISUtilities.NotDataBound_ComboBox(this.cboRunDate, "Select Tax Effective Date.");

                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
  
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                pvtSalaryDateDataView = null;
                pvtSalaryDateDataView = new DataView(pvtDataSet.Tables["SalaryPrevDate"],
                    "COMPANY_NO = " + Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo")),
                    "",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString());
                }

                pvtblnPayrollTypeDataGridViewLoaded = true;

                if (dgvPayrollTypeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);
                    dgvPayrollTypeDataGridView.Refresh();
                }
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
                    case "dgvPayrollTypeDataGridView":

                        pvtintPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;


                    case "dgvChosenEmployeeDataGridView":

                        this.dgvChosenEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvEmployeeDataGridView":

                        this.dgvEmployeeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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
       
        private void Load_CurrentForm_Records()
        {
            Set_Form_For_Read();

            if (pvtstrPayrollType == "S")
            {
                this.cboRunDate.Items.Clear();
                this.cboRunDate.BringToFront();

                //this.btnDate.Visible = false;
                this.txtDate.Visible = false;
                this.cboRunDate.Visible = true;
 
                DateTime myPrevDateTime;

                if (pvtSalaryDateDataView.Count > 0)
                {
                    if (pvtSalaryDateDataView[0]["PREV_PAY_PERIOD_DATE"] != System.DBNull.Value)
                    {
                        //Payroll Run will always fall ito This Logic
                        myPrevDateTime = Convert.ToDateTime(pvtSalaryDateDataView[0]["PREV_PAY_PERIOD_DATE"]);
                    }
                    else
                    {
                        //Set Year so that all Dates will be accepted - Lower down in Logic
                        myPrevDateTime = DateTime.Now.AddYears(-2);
                    }
                }
                else
                {
                    //Set Year so that all Dates will be accepted - Lower down in Logic
                    myPrevDateTime = DateTime.Now.AddYears(-2);
                }

                //Load Date ComboBox
                DateTime myDateTime;

                if (DateTime.Now.Month == 3)
                {
                    myDateTime = new DateTime(DateTime.Now.Year, 1, 1);
                }
                else
                {
                    if (DateTime.Now.Month > 2)
                    {
                        myDateTime = new DateTime(DateTime.Now.Year, 3, 1);

                    }
                    else
                    {
                        //Take-On
                        myDateTime = new DateTime(DateTime.Now.Year - 1, 3, 1);
                    }
                }
                                
                while (true)
                {
                    if (myDateTime > DateTime.Now.AddMonths(1))
                    {
                        break;
                    }

                    if (myPrevDateTime < myDateTime)
                    {
                        this.cboRunDate.Items.Add(myDateTime.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()));
                    }

                    myDateTime = myDateTime.AddMonths(1);
                }
            }
            else
            {
                this.cboRunDate.Visible = false;
                
                this.txtDate.Visible = true;
            }

            Load_Employees();
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvEmployeeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvEmployeeDataGridView)];

                this.dgvEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvChosenEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvChosenEmployeeDataGridView.CurrentCell = this.dgvChosenEmployeeDataGridView[0, this.dgvChosenEmployeeDataGridView.Rows.Count - 1];
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvChosenEmployeeDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvChosenEmployeeDataGridView)];

                this.dgvChosenEmployeeDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvEmployeeDataGridView.Rows.Add(myDataGridViewRow);

                this.dgvEmployeeDataGridView.CurrentCell = this.dgvEmployeeDataGridView[0, this.dgvEmployeeDataGridView.Rows.Count - 1];
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

            if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        public void btnNew_Click(object sender, System.EventArgs e)
        {
            this.Text += " - New";

            Set_Form_For_Edit();

            Clear_Form_Fields();
        }

        private void Set_Form_For_Edit()
        {
            this.dgvPayrollTypeDataGridView.Enabled = false;
            this.picPayrollTypeLock.Visible = true;

            this.btnNew.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
            
            clsISUtilities.Calender_Control_From_TextBox_Enable(this.txtDate);

            clsISUtilities.Set_Form_For_Edit(false);

            this.cboRunDate.Enabled = true;

            this.btnAdd.Enabled = true;
            this.btnAddAll.Enabled = true;
            this.btnRemove.Enabled = true;
            this.btnRemoveAll.Enabled = true;

            this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);
            
        }

        private void Set_Form_For_Read()
        {
            this.grbActivationProcess.Visible = false;
            this.tmrTimer.Enabled = false;

            this.dgvPayrollTypeDataGridView.Enabled = true;
            this.picPayrollTypeLock.Visible = false;

            this.btnNew.Enabled = true;
            
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.btnAdd.Enabled = false;
            this.btnAddAll.Enabled = false;
            this.btnRemove.Enabled = false;
            this.btnRemoveAll.Enabled = false;

            clsISUtilities.Calender_Control_From_TextBox_Disable(this.txtDate);
            this.cboRunDate.Enabled = false;

            clsISUtilities.Set_Form_For_Read();

            Clear_Form_Fields();
        }

        private void Clear_Form_Fields()
        {
            this.txtDate.Text = "";
            this.cboRunDate.SelectedIndex = -1;
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (this.Text.IndexOf(" - New", 0) > 0)
            {
                this.Text = this.Text.Substring(0, this.Text.LastIndexOf("-") - 1);
            }

            this.Refresh();

            Set_Form_For_Read();

            Load_Employees();
        }

        private int Save_Check()
        {
            int intReturnCode = 0;
           
            if (pvtstrPayrollType != "S")
            {
                if (this.txtDate.Text.Trim() == "")
                {
                    CustomMessageBox.Show("Enter " + this.lblDate.Text,
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    this.txtDate.Focus();
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

            if (this.dgvChosenEmployeeDataGridView.Rows.Count == 0)
            {
                CustomMessageBox.Show("Choose from '" + this.lblEmployee.Text + "'",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return 1;
            }

            return intReturnCode;
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strArrayEmployeeNumbers = "";
              
                DateTime dCurrentPayCategoryDateTime;

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

                //Added To End Of SQL Statement
                if (this.dgvChosenEmployeeDataGridView.Rows.Count > 0)
                {
                    for (int intIndex = 0; intIndex < this.dgvChosenEmployeeDataGridView.Rows.Count; intIndex++)
                    {
                        if (intIndex == 0)
                        {
                            strArrayEmployeeNumbers = this.dgvChosenEmployeeDataGridView[3,intIndex].Value.ToString();
                        }
                        else
                        {
                            strArrayEmployeeNumbers += "," + this.dgvChosenEmployeeDataGridView[3, intIndex].Value.ToString();
                        }
                    }
                }
                
                this.picBackupBefore.Image = EmployeeActivateSelection.Properties.Resources.Question;
                this.picEmployeeActivation.Image = null;
                this.picBackupAfter.Image = null;

                this.pnlEmployeeActivation.Visible = false;
                this.pnlDatabaseBackupAfter.Visible = false;
                
                this.grbActivationProcess.Visible = true;

                pvtintProcess = 0;

                this.tmrTimer.Enabled = true;

                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = pvtstrPayrollType;
                objParm[2] = "B";

#if (DEBUG)
#else
                intReturnCode = (int)clsISUtilities.DynamicFunction("Backup_DataBase", objParm);
#endif

                if (intReturnCode == 0)
                {
                    this.picBackupBefore.Image = (Image)global::EmployeeActivateSelection.Properties.Resources.Ok;
                    this.picEmployeeActivation.Image = EmployeeActivateSelection.Properties.Resources.Question;
                    this.pnlEmployeeActivation.Visible = true;

                    pvtintProcess += 1;
                    this.picBackupBefore.Visible = true;

                    //Insert Pay Category Record
                    objParm = new object[6];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtstrPayrollType;
                    objParm[2] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                    objParm[3] = dCurrentPayCategoryDateTime;
                    objParm[4] = strArrayEmployeeNumbers;
                    objParm[5] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Insert_TakeOn_Records", objParm);

                    pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                
                    this.picEmployeeActivation.Image = (Image)global::EmployeeActivateSelection.Properties.Resources.Ok;
                    this.picBackupAfter.Image = EmployeeActivateSelection.Properties.Resources.Question;
                    this.pnlDatabaseBackupAfter.Visible = true;

                    pvtintProcess += 1;
                    this.picEmployeeActivation.Visible = true;

                    objParm = new object[3];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtstrPayrollType;
                    objParm[2] = "A";
#if (DEBUG)
#else

                    intReturnCode = (int)clsISUtilities.DynamicFunction("Backup_DataBase", objParm);
#endif
                    this.tmrTimer.Enabled = false;

                    if (intReturnCode == 0)
                    {
                        this.picBackupAfter.Image = (Image)global::EmployeeActivateSelection.Properties.Resources.Ok;
                    }
                    else
                    {
                        this.picBackupAfter.Image = (Image)global::EmployeeActivateSelection.Properties.Resources.Error;
                    }

                    this.picBackupAfter.Visible = true;

                    CustomMessageBox.Show("Employee Take-On Successful.",
                            this.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);


                    btnCancel_Click(sender, e);
                }
                else
                {
                    this.tmrTimer.Enabled = false;
                    this.picBackupBefore.Image = (Image)global::EmployeeActivateSelection.Properties.Resources.Error;
                    this.picBackupBefore.Visible = true;

                    CustomMessageBox.Show("Backup of Database Failed.Speak To System Administrator.",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }


                Set_Form_For_Read();

                this.cboRunDate.Items.Clear();
                this.Clear_DataGridView(this.dgvPayrollTypeDataGridView);
                this.Clear_DataGridView(this.dgvEmployeeDataGridView);
                this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

                pvtblnPayrollTypeDataGridViewLoaded = false;

                for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString());
                }

                pvtblnPayrollTypeDataGridViewLoaded = true;

                if (dgvPayrollTypeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvPayrollTypeDataGridView, 0);
                    dgvPayrollTypeDataGridView.Refresh();
                }
                else
                {
                    this.btnNew.Enabled = false;
                }
            }
            catch (Exception eException)
            {
                this.tmrTimer.Enabled = false;
                this.picEmployeeActivation.Image = EmployeeActivateSelection.Properties.Resources.Error;
                this.picEmployeeActivation.Visible = true;

                CustomMessageBox.Show("Unsuccessful",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_Employees()
        {
            this.Clear_DataGridView(this.dgvEmployeeDataGridView);
            this.Clear_DataGridView(this.dgvChosenEmployeeDataGridView);

            if (this.pvtDataSet.Tables["Employee"].Rows.Count == 0)
            {
                this.btnNew.Enabled = false;
            }
            else
            {
                this.btnNew.Enabled = true;
            
                pvtEmployeeDataView = null;
                pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                    "PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "'",
                    "EMPLOYEE_CODE",
                    DataViewRowState.CurrentRows);

                pvtblnEmployeeDataGridViewLoaded = false;

                for (int intIndex = 0; intIndex < pvtEmployeeDataView.Count; intIndex++)
                {
                    this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intIndex]["EMPLOYEE_CODE"].ToString(),
                                                          pvtEmployeeDataView[intIndex]["EMPLOYEE_SURNAME"].ToString(),
                                                          pvtEmployeeDataView[intIndex]["EMPLOYEE_NAME"].ToString(),
                                                          pvtEmployeeDataView[intIndex]["EMPLOYEE_NO"].ToString());
                }

                pvtblnEmployeeDataGridViewLoaded = true;

                if (this.dgvEmployeeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvEmployeeDataGridView, 0);
                }
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

                    Load_CurrentForm_Records();
                }
            }
        }

        private void dgvEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvEmployeeDataGridView.Rows.Count > 0
            & pvtblnEmployeeDataGridViewLoaded == true)
            {

            }
        }

        private void dgvChosenEmployeeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvChosenEmployeeDataGridView.Rows.Count > 0
            & pvtblnChosenEmployeeDataGridViewLoaded == true)
            {

            }
        }

        private void dgvEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnAdd_Click(sender, e);
            }
        }

        private void dgvChosenEmployeeDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.btnRemove_Click(sender, e);
            }
        }

        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            switch (pvtintProcess)
            {
                case 0:

                    if (this.picBackupBefore.Visible == true)
                    {
                        this.picBackupBefore.Visible = false;
                    }
                    else
                    {
                        this.picBackupBefore.Visible = true;
                    }

                    break;

                case 1:

                    if (this.picEmployeeActivation.Visible == true)
                    {
                        this.picEmployeeActivation.Visible = false;
                    }
                    else
                    {
                        this.picEmployeeActivation.Visible = true;
                    }

                    break;

                case 2:

                    if (this.picBackupAfter.Visible == true)
                    {
                        this.picBackupAfter.Visible = false;
                    }
                    else
                    {
                        this.picBackupAfter.Visible = true;
                    }

                    break;
            }
        }

        private void grbSchema_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pen3Pixels, 49, 60, 301, 60);

            e.Graphics.DrawLine(Pen1Pixel, 305, 59, 305, 61);
            e.Graphics.DrawLine(Pen1Pixel, 304, 58, 304, 62);
            e.Graphics.DrawLine(Pen1Pixel, 303, 57, 303, 63);
            e.Graphics.DrawLine(Pen1Pixel, 302, 56, 302, 64);
            e.Graphics.DrawLine(Pen1Pixel, 301, 55, 301, 65);



            //First Vertical Line
            e.Graphics.DrawLine(Pen1Pixel, 49, 30, 51, 30);
            e.Graphics.DrawLine(Pen1Pixel, 48, 31, 52, 31);
            e.Graphics.DrawLine(Pen1Pixel, 47, 32, 53, 32);
            e.Graphics.DrawLine(Pen1Pixel, 46, 33, 54, 33);
            e.Graphics.DrawLine(Pen1Pixel, 45, 34, 55, 34);
           
            e.Graphics.DrawLine(Pen3Pixels, 50, 34, 50, 62);

            //Second Vertical Line
            e.Graphics.DrawLine(Pen1Pixel, 259, 30, 261, 30);
            e.Graphics.DrawLine(Pen1Pixel, 258, 31, 262, 31);
            e.Graphics.DrawLine(Pen1Pixel, 257, 32, 263, 32);
            e.Graphics.DrawLine(Pen1Pixel, 256, 33, 264, 33);
            e.Graphics.DrawLine(Pen1Pixel, 255, 34, 265, 34);
          
            e.Graphics.DrawLine(Pen3Pixels, 260, 34, 260, 62);
        }
    }
}
