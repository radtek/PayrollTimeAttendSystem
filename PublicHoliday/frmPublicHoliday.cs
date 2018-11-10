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
    public partial class frmPublicHoliday : Form
    {
        clsISUtilities clsISUtilities;

        private DateTime pvtDateTime = DateTime.Now;

        private DataSet pvtDataSet;

        DataRowView pvtDataRowView;
        private DataView pvtDataView;

        private int pvtintPublicHolidayNo = 0;

        private bool pvtblnPublicHolidayDataGridViewLoaded = false;
        
        public frmPublicHoliday()
        {
            InitializeComponent();
        }

        private void frmPublicHoliday_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busPublicHoliday");

                if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() == "S")
                {
                    this.btnAll.Visible = true;
                }
                
                this.lblDescription.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

                byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                this.dgvPublicHolidayDataGridView.Columns[0].HeaderText = "Date (" + AppDomain.CurrentDomain.GetData("DateFormat").ToString() + ")";

                Load_CurrentForm_Records();

                Set_Form_For_Read();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private int Save_Check()
        {
            //Loop through all SpreadSheets
            for (int intRow = 0; intRow < this.dgvPublicHolidayDataGridView.Rows.Count; intRow++)
            {
               if (this.dgvPublicHolidayDataGridView[0, intRow].Value.ToString() == ""
               & this.dgvPublicHolidayDataGridView[1, intRow].Value == null
               & intRow == this.dgvPublicHolidayDataGridView.Rows.Count - 1)
                {
                }
                else
                {
                    if (this.dgvPublicHolidayDataGridView[0, intRow].Value.ToString() == ""
                    & this.dgvPublicHolidayDataGridView[1, intRow].Value.ToString() == ""
                    & intRow == this.dgvPublicHolidayDataGridView.Rows.Count - 1)
                    {
                    }
                    else
                    {
                        //Test Date
                        if (this.dgvPublicHolidayDataGridView[0, intRow].Value.ToString() == "")
                        {
                            CustomMessageBox.Show("Capture Date",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                            dgvPublicHolidayDataGridView.CurrentCell = dgvPublicHolidayDataGridView[0, intRow];

                            return 1;
                        }

                        //Test Description
                        if (this.dgvPublicHolidayDataGridView[1, intRow].Value.ToString() == "")
                        {
                            CustomMessageBox.Show("Enter Description",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                            dgvPublicHolidayDataGridView.CurrentCell = dgvPublicHolidayDataGridView[1, intRow];

                            return 1;
                        }
                    }
                }
            }

            return 0;
        }

        private void Set_Form_For_Read()
        {
            if (pvtDataSet.Tables["CurrentRunInd"] != null)
            {
                if (pvtDataSet.Tables["CurrentRunInd"].Rows.Count > 0)
                {
                    if (pvtDataSet.Tables["CurrentRunInd"].Rows[0]["RUN_IND"].ToString() == "W")
                    {
                        this.lblPublicHolidayLock.Text = "Records are Locked Due to Current Wage Run.";
                    }
                    else
                    {
                        if (pvtDataSet.Tables["CurrentRunInd"].Rows[0]["RUN_IND"].ToString() == "S")
                        {
                            this.lblPublicHolidayLock.Text = "Records are Locked Due to Current Salary Run.";
                        }
                        else
                        {
                            if (pvtDataSet.Tables["CurrentRunInd"].Rows[0]["RUN_IND"].ToString() == "T")
                            {
                                this.lblPublicHolidayLock.Text = "Records are Locked Due to Current Time Attendance Run.";
                            }
                        }
                    }

                    this.grbDeductionLock.Visible = true;

                    this.btnUpdate.Enabled = false;
                    this.btnAll.Enabled = false;
                }
                else
                {
                    this.grbDeductionLock.Visible = false;

                    this.btnUpdate.Enabled = true;
                    this.btnAll.Enabled = true;
                }
            }
            else
            {
                this.grbDeductionLock.Visible = false;

                this.btnUpdate.Enabled = true;
                this.btnAll.Enabled = true;
            }

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.btnDeleteRow.Enabled = false;

            this.dgvPublicHolidayDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvPublicHolidayDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private void Set_Form_For_Edit()
        {
            this.btnUpdate.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.btnDeleteRow.Enabled = true;

            this.btnAll.Enabled = false;

            this.dgvPublicHolidayDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dgvPublicHolidayDataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;
            
            pvtintPublicHolidayNo = 9999;

            Check_To_Add_New_Public_Holiday_Row();
        }

        private void Load_CurrentForm_Records()
        {
            this.Clear_DataGridView(this.dgvPublicHolidayDataGridView);

            pvtblnPublicHolidayDataGridViewLoaded = false;

            pvtDataView = null;
            pvtDataView = new DataView(this.pvtDataSet.Tables["PaidHoliday"],
                "",
                "",
                DataViewRowState.CurrentRows);

            for (int intRowCount = 0; intRowCount < pvtDataView.Count; intRowCount++)
            {
                this.dgvPublicHolidayDataGridView.Rows.Add(Convert.ToDateTime(pvtDataView[intRowCount]["PUBLIC_HOLIDAY_DATE"]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString()),
                                                           pvtDataView[intRowCount]["PUBLIC_HOLIDAY_DESC"].ToString(),
                                                            Convert.ToDateTime(pvtDataView[intRowCount]["PUBLIC_HOLIDAY_DATE"]).ToString("dd MMMM yyyy - ddd"),
                                                           Convert.ToDateTime(pvtDataView[intRowCount]["PUBLIC_HOLIDAY_DATE"]).ToString("yyyyMMdd"),
                                                           pvtDataView[intRowCount]["PUBLIC_HOLIDAY_NO"].ToString());
            }

            pvtblnPublicHolidayDataGridViewLoaded = true;

            if (pvtDataView.Count > 0)
            {
                this.dgvPublicHolidayDataGridView.Refresh();
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Text = this.Text.Substring(0, this.Text.IndexOf("-") - 1);

            this.pvtDataSet.RejectChanges();

            Set_Form_For_Read();

            Load_CurrentForm_Records();
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
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
                    case "dgvPublicHolidayDataGridView":

                        this.dgvPublicHolidayDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                int intReturnCode = Save_Check();

                if (intReturnCode != 0)
                {
                    return;
                }

                DataSet TempDataSet = new DataSet();

                DataTable myDataTable = this.pvtDataSet.Tables["PaidHoliday"].Clone();
                TempDataSet.Tables.Add(myDataTable);

                pvtDataView = null;
                pvtDataView = new DataView(this.pvtDataSet.Tables["PaidHoliday"],
                    "",
                    "",
                    DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent);

                for (int intRow = 0; intRow < pvtDataView.Count; intRow++)
                {
                    if (pvtDataView[intRow]["PUBLIC_HOLIDAY_DESC"].ToString().Trim() == ""
                    & pvtDataView[intRow]["PUBLIC_HOLIDAY_DATE"] == System.DBNull.Value)
                    {
                        continue;
                    }

                    TempDataSet.Tables[0].ImportRow(pvtDataView[intRow].Row);
                }

                if (TempDataSet.Tables[0].Rows.Count > 0)
                {
                    Int64 Int64CompanyNo = 999999;

                    if (this.btnAll.Text == "All")
                    {
                        Int64CompanyNo = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    }

                    byte[] bytCompress = clsISUtilities.Compress_DataSet(TempDataSet);

                    object[] objParm = new object[3];
                    objParm[0] = Int64CompanyNo;
                    objParm[1] = bytCompress;
                    objParm[2] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();
                    
                    bytCompress = (byte[])clsISUtilities.DynamicFunction("Update_Records", objParm,true);

                    pvtDataSet = null;
                    pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);
                }

                btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();
        }

        private void dgvPublicHolidayDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPublicHolidayDataGridView.Rows.Count > 0
                & pvtblnPublicHolidayDataGridViewLoaded == true)
            {
            }
        }

        private void dgvPublicHolidayDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 0)
            {
                if (dgvPublicHolidayDataGridView[e.Column.Index + 3, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvPublicHolidayDataGridView[e.Column.Index + 3, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvPublicHolidayDataGridView[e.Column.Index + 3, e.RowIndex1].Value.ToString()) > double.Parse(dgvPublicHolidayDataGridView[e.Column.Index + 3, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvPublicHolidayDataGridView[e.Column.Index + 3, e.RowIndex1].Value.ToString()) < double.Parse(dgvPublicHolidayDataGridView[e.Column.Index + 3, e.RowIndex2].Value.ToString()))
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

        private void Check_To_Add_New_Public_Holiday_Row()
        {
            if (this.dgvPublicHolidayDataGridView.Rows.Count > 0)
            {
                if (this.dgvPublicHolidayDataGridView[0, this.dgvPublicHolidayDataGridView.Rows.Count - 1].Value.ToString() != "")
                {
                    if (this.dgvPublicHolidayDataGridView[1, this.dgvPublicHolidayDataGridView.Rows.Count - 1].Value.ToString() != "")
                    {
                        pvtintPublicHolidayNo += 1;

                        this.dgvPublicHolidayDataGridView.Rows.Add("",
                                                                  "",
                                                                  "",
                                                                  "",
                                                                  pvtintPublicHolidayNo);

                        pvtDataRowView = pvtDataView.AddNew();
                        pvtDataRowView["PUBLIC_HOLIDAY_DESC"] = "";
                        pvtDataRowView["PUBLIC_HOLIDAY_NO"] = pvtintPublicHolidayNo;
                        pvtDataRowView.EndEdit();

                        int intFirstDisplayedScrollingRowIndex = dgvPublicHolidayDataGridView.Rows.Count - 21;

                        if (intFirstDisplayedScrollingRowIndex < 0)
                        {
                            intFirstDisplayedScrollingRowIndex = 0;
                        }

                        dgvPublicHolidayDataGridView.FirstDisplayedScrollingRowIndex = intFirstDisplayedScrollingRowIndex;
                    }
                }
            }
            else
            {
                this.dgvPublicHolidayDataGridView.Rows.Add("",
                                                           "",
                                                           "",
                                                           "",
                                                           pvtintPublicHolidayNo);

                pvtDataRowView = pvtDataView.AddNew();
                pvtDataRowView["PUBLIC_HOLIDAY_DESC"] = "";
                pvtDataRowView["PUBLIC_HOLIDAY_NO"] = pvtintPublicHolidayNo;
                pvtDataRowView.EndEdit();

                int intFirstDisplayedScrollingRowIndex = dgvPublicHolidayDataGridView.Rows.Count - 21;

                if (intFirstDisplayedScrollingRowIndex < 0)
                {
                    intFirstDisplayedScrollingRowIndex = 0;
                }

                dgvPublicHolidayDataGridView.FirstDisplayedScrollingRowIndex = intFirstDisplayedScrollingRowIndex;
            }
        }

        private void dgvPublicHolidayDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.btnSave.Enabled == true
            && this.dgvPublicHolidayDataGridView.Rows.Count > 0)
            {
                if (e.ColumnIndex == 0)
                {
                    if (this.dgvPublicHolidayDataGridView[0, this.dgvPublicHolidayDataGridView.Rows.Count - 1].Value.ToString() != "")
                    {
                        DateTime myDateTime;

                        try
                        {
                            myDateTime = DateTime.ParseExact(this.dgvPublicHolidayDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                        }
                        catch
                        {
                            dgvPublicHolidayDataGridView[e.ColumnIndex, e.RowIndex].Value = "";
                            goto dgvPublicHolidayDataGridView_CellValueChanged_Error;
                        }


                        int intPublicHolidayNo = Convert.ToInt32(this.dgvPublicHolidayDataGridView[4, e.RowIndex].Value);

                        DataView myDataView = new DataView(this.pvtDataSet.Tables["PaidHoliday"],
                            "PUBLIC_HOLIDAY_NO = " + intPublicHolidayNo,
                            "",
                            DataViewRowState.CurrentRows);

                        myDataView[0]["PUBLIC_HOLIDAY_DATE"] = myDateTime;

                        Check_To_Add_New_Public_Holiday_Row();
                    }

                dgvPublicHolidayDataGridView_CellValueChanged_Error:

                    int intError = 0;
                }
                else
                {
                    if (e.ColumnIndex == 1)
                    {
                        int intPublicHolidayNo = Convert.ToInt32(this.dgvPublicHolidayDataGridView[4, e.RowIndex].Value);

                        DataView myDataView = new DataView(this.pvtDataSet.Tables["PaidHoliday"],
                            "PUBLIC_HOLIDAY_NO = " + intPublicHolidayNo,
                            "",
                            DataViewRowState.CurrentRows);

                        myDataView[0]["PUBLIC_HOLIDAY_DESC"] = this.dgvPublicHolidayDataGridView[1, e.RowIndex].Value.ToString();

                        Check_To_Add_New_Public_Holiday_Row();
                    }
                }
            }
        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            if (this.dgvPublicHolidayDataGridView.Rows.Count > 0)
            {
                if (this.dgvPublicHolidayDataGridView[0,this.Get_DataGridView_SelectedRowIndex(dgvPublicHolidayDataGridView)].Value.ToString() == ""
                & this.dgvPublicHolidayDataGridView[1,this.Get_DataGridView_SelectedRowIndex(dgvPublicHolidayDataGridView)].Value.ToString() == ""
                & this.Get_DataGridView_SelectedRowIndex(dgvPublicHolidayDataGridView) == this.dgvPublicHolidayDataGridView.Rows.Count - 1)
                {
                }
                else
                {
                    int intPublicHolidayNo = Convert.ToInt32(this.dgvPublicHolidayDataGridView[4, this.Get_DataGridView_SelectedRowIndex(dgvPublicHolidayDataGridView)].Value);

                    DataView myDataView = new DataView(this.pvtDataSet.Tables["PaidHoliday"],
                        "PUBLIC_HOLIDAY_NO = " + intPublicHolidayNo,
                        "",
                        DataViewRowState.CurrentRows);

                    myDataView[0].Delete();

                    this.dgvPublicHolidayDataGridView.Rows.RemoveAt(this.Get_DataGridView_SelectedRowIndex(this.dgvPublicHolidayDataGridView));
                }
            }
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            Button myButton = (Button)sender;

            Int64 Int64CompanyNo = 999999;

            if (myButton.Text == "All")
            {
                this.BackColor = System.Drawing.SystemColors.ControlDark;

                myButton.Text = "Current";
            }
            else
            {
                this.BackColor = System.Drawing.SystemColors.Control;

                Int64CompanyNo = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                myButton.Text = "All";
            }

            object[] objParm = new object[2];
            objParm[0] = Int64CompanyNo;
            objParm[1] = AppDomain.CurrentDomain.GetData("FromProgramInd").ToString();

            byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

            pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

            Load_CurrentForm_Records();

            Set_Form_For_Read();
        }
    }
}
