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
    public partial class frmShiftSchedule : Form
    {
        clsISUtilities clsISUtilities;

        public DataSet pvtDataSet;
        public DataView pvtShiftScheduleDataView;
        public DataView pvtEmployeeDataView;
        public DataSet pvtTempDataSet;
        private DataRowView pvtDataRowView;

        private int pvtintPayCategoryShiftScheduleNo = -1;

        private int pvtintShiftScheduleDataVieweRowNo = -1;

        private int pvtintShiftScheduleDataGridViewRowIndex;
        private int pvtintPayrollTypeDataGridViewRowIndex;

        private byte[] pvtbytCompress;

        private bool pvtblnShiftScheduleDataGridViewLoaded = false;
        private bool pvtblnPayrollTypeDataGridViewLoaded = false;

        private string pvtstrPayrollType = "";
        
        public frmShiftSchedule()
        {
            InitializeComponent();
        }

        private void frmShiftSchedule_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busShiftSchedule");

                this.lblDescription.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblPayrollType.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                
                clsISUtilities.Create_Calender_Control_From_TextBox(this.txtFromDate);
                clsISUtilities.Create_Calender_Control_From_TextBox(this.txtToDate);
                
                clsISUtilities.NotDataBound_Date_TextBox(this.txtFromDate, "Enter From Date");
                clsISUtilities.NotDataBound_Date_TextBox(this.txtToDate, "Enter To Date");
                                
                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                pvtblnPayrollTypeDataGridViewLoaded = false;

                for (int intRow = 0; intRow < pvtDataSet.Tables["PayrollType"].Rows.Count; intRow++)
                {
                    this.dgvPayrollTypeDataGridView.Rows.Add(pvtDataSet.Tables["PayrollType"].Rows[intRow]["PAYROLL_TYPE_DESC"].ToString());
                }

                pvtblnPayrollTypeDataGridViewLoaded = true;

                if (this.dgvPayrollTypeDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvPayrollTypeDataGridView, 0);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public void Load_CurrentForm_Records()
        {
            int intShiftScheduleRow = 0;
            this.btnNew.Enabled = true;

            this.Clear_DataGridView(this.dgvSchiftScheduleDataGridView);

            pvtShiftScheduleDataView = new DataView(pvtDataSet.Tables["ShiftSchedule"],
                "PAY_CATEGORY_TYPE = '" + this.pvtstrPayrollType + "'",
                "",
                DataViewRowState.CurrentRows);

            this.pvtblnShiftScheduleDataGridViewLoaded = false;

            for (int intRow = 0; intRow < pvtShiftScheduleDataView.Count; intRow++)
            {
                this.dgvSchiftScheduleDataGridView.Rows.Add(Convert.ToDateTime(pvtShiftScheduleDataView[intRow]["FROM_DATETIME"]).ToString("dd MMMM yyyy - dddd"),
                                                            Convert.ToDateTime(pvtShiftScheduleDataView[intRow]["TO_DATETIME"]).ToString("dd MMMM yyyy - dddd"),
                                                            Convert.ToDateTime(pvtShiftScheduleDataView[intRow]["FROM_DATETIME"]).ToString("yyyyMMdd"),
                                                            Convert.ToDateTime(pvtShiftScheduleDataView[intRow]["TO_DATETIME"]).ToString("yyyyMMdd"),
                                                            intRow.ToString());

                if (Convert.ToInt32(pvtShiftScheduleDataView[intRow]["PAY_CATEGORY_SHIFT_SCHEDULE_NO"]) == pvtintPayCategoryShiftScheduleNo)
                {
                    intShiftScheduleRow = intRow;
                }
            }

            this.pvtblnShiftScheduleDataGridViewLoaded = true;

            if (this.dgvSchiftScheduleDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvSchiftScheduleDataGridView, intShiftScheduleRow);

                this.btnUpdate.Enabled = true;
                this.btnDelete.Enabled = true;
            }
            else
            {
                this.txtFromDate.Text = "";
                this.txtToDate.Text = "";

                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
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
                    case "dgvSchiftScheduleDataGridView":

                        pvtintShiftScheduleDataGridViewRowIndex = -1;
                        this.dgvShiftScheduleDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayrollTypeDataGridView":

                        pvtintPayrollTypeDataGridViewRowIndex = -1;
                        this.dgvPayrollTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        public void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void btnNew_Click(object sender, System.EventArgs e)
        {
            this.Text += " - New";

            Set_Form_For_Edit();
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.pvtDataSet.RejectChanges();

            Set_Form_For_Read();
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                int intReturnCode = clsISUtilities.DataBind_Save_Check();

                if (intReturnCode != 0)
                {
                    return;
                }

                if (DateTime.ParseExact(this.txtFromDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null) > DateTime.ParseExact(this.txtToDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null))
                {
                    MessageBox.Show("From Date must be Less than or Equal to To Date.",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                    return;
                }
                            
                string strFromDate = DateTime.ParseExact(this.txtFromDate.Text,AppDomain.CurrentDomain.GetData("DateFormat").ToString(),null).ToString("yyyy-MM-dd");
                string strToDate = DateTime.ParseExact(this.txtToDate.Text, AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null).ToString("yyyy-MM-dd");
                bool blnLocked = false;

                if (this.rbnLocked.Checked == true)
                {
                    blnLocked = true;
                }

                if (this.Text.EndsWith(" - New") == true)
                {
                    object[] objParm = new object[5];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtstrPayrollType;
                    objParm[2] = strFromDate;
                    objParm[3] = strToDate;
                    objParm[4] = blnLocked;

                    pvtintPayCategoryShiftScheduleNo = (int)clsISUtilities.DynamicFunction("Insert_New_Record", objParm,true);
                    
                    pvtDataRowView = pvtShiftScheduleDataView.AddNew();

                    pvtDataRowView.BeginEdit();

                    pvtDataRowView["COMPANY_NO"] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    pvtDataRowView["PAY_CATEGORY_TYPE"] = pvtstrPayrollType;
                    pvtDataRowView["PAY_CATEGORY_SHIFT_SCHEDULE_NO"] = pvtintPayCategoryShiftScheduleNo;
                    pvtDataRowView["FROM_DATETIME"] = DateTime.ParseExact(strFromDate, "yyyy-MM-dd", null);
                    pvtDataRowView["TO_DATETIME"] = DateTime.ParseExact(strToDate, "yyyy-MM-dd", null);
                    pvtDataRowView["LOCKED_IND"] = blnLocked;
                    
                    pvtDataRowView.EndEdit();

                    Load_CurrentForm_Records();
                }
                else
                {
                    object[] objParm = new object[6];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtstrPayrollType;
                    objParm[2] = pvtintPayCategoryShiftScheduleNo;
                    objParm[3] = strFromDate;
                    objParm[4] = strToDate;
                    objParm[5] = blnLocked;
                   
                    clsISUtilities.DynamicFunction("Update_Record", objParm,true);

                    pvtShiftScheduleDataView[pvtintShiftScheduleDataVieweRowNo]["FROM_DATETIME"] = DateTime.ParseExact(strFromDate, "yyyy-MM-dd", null);
                    pvtShiftScheduleDataView[pvtintShiftScheduleDataVieweRowNo]["TO_DATETIME"] = DateTime.ParseExact(strToDate, "yyyy-MM-dd", null);

                    if (blnLocked == true)
                    {
                        pvtShiftScheduleDataView[pvtintShiftScheduleDataVieweRowNo]["LOCKED_IND"] = true;

                    }
                    else
                    {
                        pvtShiftScheduleDataView[pvtintShiftScheduleDataVieweRowNo]["LOCKED_IND"] = false;
                    }
                }

                this.pvtDataSet.AcceptChanges();

                Set_Form_For_Read();

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Set_Form_For_Edit()
        {
            bool blnNew = false;

            if (this.Text.EndsWith(" - New") == true)
            {
                blnNew = true;
                this.txtFromDate.Text = "";
                this.txtToDate.Text = "";

                this.rbnUnlocked.Checked = true;
            }

            this.dgvPayrollTypeDataGridView.Enabled = false;
            this.dgvSchiftScheduleDataGridView.Enabled = false;
            picShiftScheduleLock.Visible = true;
            this.picPayrollTypeLock.Visible = true;

            clsISUtilities.Set_Form_For_Edit(blnNew);
            
            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.txtFromDate.Enabled = true;
            this.txtToDate.Enabled = true;

            this.rbnLocked.Enabled = true;
            this.rbnUnlocked.Enabled = true;

            this.txtFromDate.Focus();
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

            this.dgvPayrollTypeDataGridView.Enabled = true;
            this.dgvSchiftScheduleDataGridView.Enabled = true;
            picShiftScheduleLock.Visible = false;
            this.picPayrollTypeLock.Visible = false;

            clsISUtilities.Set_Form_For_Read();

            this.btnNew.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.txtFromDate.Enabled = false;
            this.txtToDate.Enabled = false;

            this.rbnLocked.Enabled = false;
            this.rbnUnlocked.Enabled = false;

            if (this.pvtShiftScheduleDataView.Count > 0)
            {
                this.btnUpdate.Enabled = true;
                this.btnDelete.Enabled = true;
            }
            else
            {
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;

                this.txtFromDate.Text = "";
                this.txtToDate.Text = "";
            }
   
            Load_CurrentForm_Records();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlgResult = MessageBox.Show("Delete " + this.Text + " '" + Convert.ToDateTime(this.pvtShiftScheduleDataView[pvtintShiftScheduleDataVieweRowNo]["FROM_DATETIME"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()) + " to " + Convert.ToDateTime(this.pvtShiftScheduleDataView[pvtintShiftScheduleDataVieweRowNo]["TO_DATETIME"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()) + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[3];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtstrPayrollType;
                    objParm[2] = pvtintPayCategoryShiftScheduleNo;

                    clsISUtilities.DynamicFunction("Delete_Record", objParm, true);

                    this.pvtintPayCategoryShiftScheduleNo = -1;

                    pvtShiftScheduleDataView[pvtintShiftScheduleDataVieweRowNo].Delete();

                    this.pvtDataSet.AcceptChanges();

                    Load_CurrentForm_Records();

                    if (pvtShiftScheduleDataView.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvSchiftScheduleDataGridView, 0);
                    }
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void dgvShiftScheduleDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnShiftScheduleDataGridViewLoaded == true)
            {
                if (pvtintShiftScheduleDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintShiftScheduleDataGridViewRowIndex = e.RowIndex;

                    pvtintShiftScheduleDataVieweRowNo = Convert.ToInt32(this.dgvSchiftScheduleDataGridView[4, e.RowIndex].Value);

                    pvtintPayCategoryShiftScheduleNo = Convert.ToInt32(pvtShiftScheduleDataView[pvtintShiftScheduleDataVieweRowNo]["PAY_CATEGORY_SHIFT_SCHEDULE_NO"]);

                    this.txtFromDate.Text = Convert.ToDateTime(pvtShiftScheduleDataView[pvtintShiftScheduleDataVieweRowNo]["FROM_DATETIME"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());
                    this.txtToDate.Text = Convert.ToDateTime(pvtShiftScheduleDataView[pvtintShiftScheduleDataVieweRowNo]["TO_DATETIME"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());

                    if (Convert.ToBoolean(pvtShiftScheduleDataView[pvtintShiftScheduleDataVieweRowNo]["LOCKED_IND"]) == true)
                    {
                        this.rbnLocked.Checked = true;
                    }
                    else
                    {
                        this.rbnUnlocked.Checked = true;
                    }
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

                    if (pvtDataSet != null)
                    {
                        Load_CurrentForm_Records();
                    }
                }
            }
        }

        private void dgvSchiftScheduleDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 0
            || e.Column.Index == 1)
            {
                if (dgvSchiftScheduleDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvSchiftScheduleDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvSchiftScheduleDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString()) > double.Parse(dgvSchiftScheduleDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvSchiftScheduleDataGridView[e.Column.Index + 2, e.RowIndex1].Value.ToString()) < double.Parse(dgvSchiftScheduleDataGridView[e.Column.Index + 2, e.RowIndex2].Value.ToString()))
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
