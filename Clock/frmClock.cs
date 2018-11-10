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
    public partial class frmClock : Form
    {
        clsISClientUtilities clsISClientUtilities;

        ToolStripMenuItem miLinkedMenuItem;

        private DataSet pvtDataSet;
        private DataView pvtDataView;
        private string pvtstrDeviceDesc = "";
        private string pvtstrDeviceUsage = "";
        private string pvtstrTimeAttendClockFirstLastInd = "";
        private string pvtstrClockInOutParm = "";
        private string pvtstrLanWanInd = "";
        private int pvtintFARRequested;
        private int pvtintCompanyNo = -1;
        private int pvtintClockInRangeFrom = -1;
        private int pvtintClockInRangeTo = -1;
        private int pvtintLockOutMinutes = -1;

        private int pvtintDeviceNo = -1;

        private int pvtintClockDataGridViewRowIndex = -1;

        private int[] intPossibleFARValuesArray;

        private bool pvtblnClockDataGridViewLoaded = false;
        private int pvtintClockDescCol = 0;
        private int pvtintClockRecCountCol = 2;

        //PROBABILITY_ONE = 2147483647;

        //  1/1000   = 2147483
        //  1/2500   = 858993
        //  1/5000   = 429496
        //  1/7500   = 286331
        //  1/10000  = 214748
        //  1/25000  = 85899
        //  1/50000  = 42949
        //  1/75000  = 28633
        //  1/100000 = 21475

        private int pvtintDataViewReaderIndex = -1;

        public frmClock()
        {
            InitializeComponent();
        }

        private void frmClock_Load(object sender, EventArgs e)
        {
            try
            {
                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

                cboFAR.Items.Add("1/10000");
                cboFAR.Items.Add("1/25000");
                cboFAR.Items.Add("1/50000");
                cboFAR.Items.Add("1/75000");
                cboFAR.Items.Add("1/100000");

                intPossibleFARValuesArray = new int[5];
    
                intPossibleFARValuesArray[0] = 214748;
                intPossibleFARValuesArray[1] = 85899;
                intPossibleFARValuesArray[2] = 42949;
                intPossibleFARValuesArray[3] = 28633;
                intPossibleFARValuesArray[4] = 21475;
      
                string strTag = "Start";

                clsISClientUtilities = new clsISClientUtilities(this,"busClock");

                this.lblClock.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Form_Records", objParm,false);
                pvtDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["Company"].Rows.Count; intRowCount++)
                {
                    this.cboCompany.Items.Add(pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_DESC"].ToString());
                }

                strTag = "After DataSet";

#if(DEBUG)
                int intWidth = this.dgvClockDataGridView.RowHeadersWidth;

                if (this.dgvClockDataGridView.ScrollBars == ScrollBars.Vertical
                    | this.dgvClockDataGridView.ScrollBars == ScrollBars.Both)
                {
                    intWidth += 19;
                }

                for (int intCol = 0; intCol < this.dgvClockDataGridView.ColumnCount; intCol++)
                {
                    if (this.dgvClockDataGridView.Columns[intCol].Visible == true)
                    {
                        intWidth += this.dgvClockDataGridView.Columns[intCol].Width;
                    }
                }

                if (intWidth != this.dgvClockDataGridView.Width)
                {
                    System.Windows.Forms.MessageBox.Show("Width should be " + intWidth.ToString());
                }

                int intHeight = this.dgvClockDataGridView.ColumnHeadersHeight + 2;
                int intNewHeight = this.dgvClockDataGridView.RowTemplate.Height / 2;

                for (int intRow = 0; intRow < 200; intRow++)
                {
                    intHeight += this.dgvClockDataGridView.RowTemplate.Height;

                    if (intHeight == this.dgvClockDataGridView.Height)
                    {
                        break;
                    }
                    else
                    {
                        if (intHeight > this.dgvClockDataGridView.Height)
                        {
                            System.Windows.Forms.MessageBox.Show("Height should be " + intHeight.ToString());
                            break;
                        }
                        else
                        {

                            if (intHeight + intNewHeight > this.dgvClockDataGridView.Height)
                            {
                                System.Windows.Forms.MessageBox.Show("Height should be " + intHeight.ToString());
                                break;
                            }
                        }
                    }
                }
#endif      
                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void Load_CurrentForm_Records()
        {
            int intRowIndex = 0;

            pvtDataView = null;
            pvtDataView = new DataView(pvtDataSet.Tables["Clock"],
                "",
                "DEVICE_DESC",
                DataViewRowState.CurrentRows);

            pvtblnClockDataGridViewLoaded = false;

            this.Clear_DataGridView(dgvClockDataGridView);
    
            for (int intRow = 0; intRow < pvtDataView.Count; intRow++)
            {
                this.dgvClockDataGridView.Rows.Add(pvtDataView[intRow]["DEVICE_DESC"].ToString(), 
                                                   pvtDataView[intRow]["DEVICE_NO"].ToString(), intRow.ToString());

                if (pvtintDeviceNo == Convert.ToInt32(pvtDataView[intRow]["DEVICE_NO"]))
                {
                    intRowIndex = intRow;
                }
            }

            pvtblnClockDataGridViewLoaded = true;

            if (dgvClockDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvClockDataGridView, intRowIndex);
            }
            else
            {
                this.btnDelete.Enabled = false;
                this.btnUpdate.Enabled = false;
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

            return intReturnIndex;
        }

        public void Set_DataGridView_SelectedRowIndex(DataGridView myDataGridView, int intRow)
        {
            //Fires DataGridView RowEnter Function
            if (myDataGridView.CurrentCell.RowIndex == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvClockDataGridView":

                        pvtintClockDataGridViewRowIndex = -1;
                        this.dgvClockDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        System.Windows.Forms.MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.Text += " - New";

            Set_Form_For_Edit();

            this.txtReader.Focus();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();

            this.txtReader.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomClientMessageBox.Show("Delete Device '" + pvtDataView[pvtintDataViewReaderIndex]["DEVICE_DESC"].ToString() + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[1];
                    objParm[0] = Convert.ToInt32(pvtDataView[pvtintDataViewReaderIndex]["DEVICE_NO"]);

                    clsISClientUtilities.DynamicFunction("Delete_Record", objParm,true);

                    pvtDataView[pvtintDataViewReaderIndex].Delete();

                    this.pvtDataSet.AcceptChanges();

                    this.Load_CurrentForm_Records();
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string strTimeFrom = "";
                string strTimeTo = "";

                if (this.cboCompany.SelectedIndex == -1)
                {
                    CustomClientMessageBox.Show("Select Company that owns Clock Reader.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.cboCompany.Focus();
                    return;
                }

                if (this.txtReader.Text.Trim() == "")
                {
                    CustomClientMessageBox.Show("Enter Clock Reader Description.",this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.txtReader.Focus();
                    return;
                }
                else
                {
                    pvtstrDeviceDesc = this.txtReader.Text.Trim();
                }

                pvtstrTimeAttendClockFirstLastInd = "";

                if (this.rbnTimeAttend.Checked == true)
                {
                    pvtstrDeviceUsage = "T";
                }
                else
                {
                    if (this.rbnAccess.Checked == true)
                    {
                        pvtstrDeviceUsage = "A";
                    }
                    else
                    {
                        if (this.rbnBoth.Checked == true)
                        {
                            pvtstrDeviceUsage = "B";

                            if (this.rbnClockNormal.Checked == true)
                            {
                                pvtstrTimeAttendClockFirstLastInd = "N";
                            }
                            else
                            {
                                if (this.rbnClockFirstLast.Checked == true)
                                {
                                    pvtstrTimeAttendClockFirstLastInd = "Y";
                                }
                                else
                                {
                                    CustomClientMessageBox.Show("Choose Access / Time & Attendance Option.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            CustomClientMessageBox.Show("Choose Device Usage.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                if (this.rbnInOnly.Checked == true)
                {
                    pvtstrClockInOutParm = "I";
                }
                else
                {
                    if (this.rbnOutOnly.Checked == true)
                    {
                        pvtstrClockInOutParm = "O";
                    }
                    else
                    {
                        if (this.rbnDynamic.Checked == true)
                        {
                            pvtstrClockInOutParm = "D";
                        }
                        else
                        {
                            if (this.rbnClockRange.Checked == true)
                            {
                                if (this.cboFromHour.SelectedIndex == -1)
                                {
                                    this.cboFromHour.Focus();
                                    CustomClientMessageBox.Show("Select From Hour.\n\n(Clock Range - In Range)", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                if (this.cboFromMinute.SelectedIndex == -1)
                                {
                                    this.cboFromMinute.Focus();
                                    CustomClientMessageBox.Show("Select From Minute.\n\n(Clock Range - In Range)", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                if (this.cboToHour.SelectedIndex == -1)
                                {
                                    this.cboToHour.Focus();
                                    CustomClientMessageBox.Show("Select To Hour.\n\n(Clock Range - In Range)", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                if (this.cboToMinute.SelectedIndex == -1)
                                {
                                    this.cboToMinute.Focus();
                                    CustomClientMessageBox.Show("Select To Minute.\n\n(Clock Range - In Range)", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                pvtstrClockInOutParm = "R";

                                strTimeFrom = this.cboFromHour.SelectedItem.ToString() + this.cboFromMinute.SelectedItem.ToString();
                                strTimeTo = this.cboToHour.SelectedItem.ToString() + this.cboToMinute.SelectedItem.ToString();

                                pvtintClockInRangeFrom = Convert.ToInt32(strTimeFrom);
                                pvtintClockInRangeTo = Convert.ToInt32(strTimeTo);
                            }
                            else
                            {
                                CustomClientMessageBox.Show("Select a " + this.grbClockOptions.Text + ".", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                }

                if (this.cboLockOut.SelectedIndex == -1)
                {
                    CustomClientMessageBox.Show("Select Lock Out Time.\n\n(Access Control - Employee Lock Out)", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.cboLockOut.Focus();
                    return;
                }
                else
                {
                    pvtintLockOutMinutes = Convert.ToInt32(this.cboLockOut.SelectedItem.ToString());
                }

                if (this.cboCompany.Items.Count > 0)
                {
                    if (this.cboCompany.SelectedIndex == -1)
                    {
                        CustomClientMessageBox.Show("Select Company that owns Clock Reader.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.cboCompany.Focus();
                        return;
                    }
                    else
                    {
                        pvtintCompanyNo = Convert.ToInt32(pvtDataSet.Tables["Company"].Rows[this.cboCompany.SelectedIndex]["COMPANY_NO"]);
                    }
                }
                else
                {
                    pvtintCompanyNo = -1;
                }

                if (this.rbnWAN.Checked == true)
                {
                    pvtstrLanWanInd = "W";
                }
                else
                {
                    pvtstrLanWanInd = "L";
                }

                if (this.cboFAR.SelectedIndex == -1)
                {
                    CustomClientMessageBox.Show("Select False Acceptance Read Value.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.cboFAR.Focus();
                    return;
                }
                else
                {
                    pvtintFARRequested = intPossibleFARValuesArray[this.cboFAR.SelectedIndex];
                }


                DataSet TempDataSet = new DataSet();
                TempDataSet.Tables.Add(pvtDataSet.Tables["Clock"].Clone());

                if (this.Text.IndexOf(" - New") > 0)
                {
                    DataRowView drvDataRowView = this.pvtDataView.AddNew();
                    //Set Key for Find
                    drvDataRowView["DEVICE_NO"] = 0;
                    drvDataRowView["DEVICE_DESC"] = pvtstrDeviceDesc;
                    drvDataRowView["DEVICE_USAGE"] = pvtstrDeviceUsage;
                    drvDataRowView["TIME_ATTEND_CLOCK_FIRST_LAST_IND"] = pvtstrTimeAttendClockFirstLastInd;
                    drvDataRowView["CLOCK_IN_OUT_PARM"] = pvtstrClockInOutParm;
                    drvDataRowView["CLOCK_IN_RANGE_FROM"] = pvtintClockInRangeFrom;
                    drvDataRowView["CLOCK_IN_RANGE_TO"] = pvtintClockInRangeTo;
                    drvDataRowView["LOCK_OUT_MINUTES"] = pvtintLockOutMinutes;
                    drvDataRowView["COMPANY_NO"] = pvtintCompanyNo;
                    drvDataRowView["LAN_WAN_IND"] = pvtstrLanWanInd;
                    drvDataRowView["FAR_REQUESTED"] = pvtintFARRequested;

                    drvDataRowView.EndEdit();

                    TempDataSet.Tables["Clock"].ImportRow(drvDataRowView.Row);

                    //Compress DataSet
                    byte[] pvtbytCompress = clsISClientUtilities.Compress_DataSet(TempDataSet);

                    object[] objParm = new object[1];
                    objParm[0] = pvtbytCompress;

                    drvDataRowView["DEVICE_NO"] = clsISClientUtilities.DynamicFunction("Insert_Record", objParm,true);

                    this.pvtintDeviceNo = Convert.ToInt32(drvDataRowView["DEVICE_NO"]);
                }
                else
                {
                    pvtDataView[pvtintDataViewReaderIndex]["DEVICE_DESC"] = pvtstrDeviceDesc;
                    pvtDataView[pvtintDataViewReaderIndex]["DEVICE_USAGE"] = pvtstrDeviceUsage;
                    pvtDataView[pvtintDataViewReaderIndex]["TIME_ATTEND_CLOCK_FIRST_LAST_IND"] = pvtstrTimeAttendClockFirstLastInd;
                    pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_OUT_PARM"] = pvtstrClockInOutParm;
                    pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_RANGE_FROM"] = pvtintClockInRangeFrom;
                    pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_RANGE_TO"] = pvtintClockInRangeTo;
                    pvtDataView[pvtintDataViewReaderIndex]["LOCK_OUT_MINUTES"] = pvtintLockOutMinutes;
                    pvtDataView[pvtintDataViewReaderIndex]["COMPANY_NO"] = pvtintCompanyNo;
                    pvtDataView[pvtintDataViewReaderIndex]["LAN_WAN_IND"] = pvtstrLanWanInd;
                    pvtDataView[pvtintDataViewReaderIndex]["FAR_REQUESTED"] = pvtintFARRequested;

                    TempDataSet.Tables["Clock"].ImportRow(pvtDataView[pvtintDataViewReaderIndex].Row);

                    //Compress DataSet
                    byte[] pvtbytCompress = clsISClientUtilities.Compress_DataSet(TempDataSet);

                    object[] objParm = new object[1];
                    objParm[0] = pvtbytCompress;

                    clsISClientUtilities.DynamicFunction("Update_Record", objParm,true);

                    this.dgvClockDataGridView[pvtintClockDescCol, this.dgvClockDataGridView.CurrentRow.Index].Value = pvtstrDeviceDesc;
                }

                pvtDataSet.AcceptChanges();

                if (this.Text.IndexOf(" - New") > 0)
                {
                    Load_CurrentForm_Records();
                }

                btnCancel_Click(sender, e);
            }
            catch (Exception ex)
            {
                clsISClientUtilities.ErrorHandler(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.Text.LastIndexOf(" - ") != -1)
            {
                this.Text = this.Text.Substring(0, this.Text.LastIndexOf(" - "));
            }

            this.picReaderLock.Visible = false;

            this.txtReader.Enabled = false;

            this.rbnAccess.Enabled = false;
            this.rbnTimeAttend.Enabled = false;
            this.rbnBoth.Enabled = false;

            this.rbnClockNormal.Enabled = false;
            this.rbnClockFirstLast.Enabled = false;

            this.cboLockOut.Enabled = false;
            this.cboCompany.Enabled = false;

            this.rbnInOnly.Enabled = false;
            this.rbnOutOnly.Enabled = false;
            this.rbnDynamic.Enabled = false;
            this.rbnClockRange.Enabled = false;

            this.cboFromHour.Enabled = false;
            this.cboFromMinute.Enabled = false;
            this.cboToHour.Enabled = false;
            this.cboToMinute.Enabled = false;

            this.rbnWAN.Enabled = false;
            this.rbnLAN.Enabled = false;

            this.cboFAR.Enabled = false;

            this.btnNew.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.dgvClockDataGridView.Enabled = true;

            if (dgvClockDataGridView.Rows.Count > 0)
            {
                this.btnUpdate.Enabled = true;
                this.btnDelete.Enabled = true;

                this.Set_DataGridView_SelectedRowIndex(dgvClockDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvClockDataGridView));
            }
            else
            {
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }
        }

        private void Set_Form_For_Edit()
        {
            this.dgvClockDataGridView.Enabled = false;

            this.txtReader.Enabled = true;

            this.picReaderLock.Visible = true;

            if (this.Text.IndexOf(" - New") > 0)
            {
                this.cboLockOut.SelectedIndex = 0;

                if (this.cboCompany.Items.Count == 1)
                {
                    this.cboCompany.SelectedIndex = 0;
                }
                else
                {
                    this.cboCompany.SelectedIndex = -1;
                }
                this.cboFAR.SelectedIndex = 4;
                this.txtReader.Text = "";
                this.txtDeviceNo.Text = "";
                this.rbnLAN.Checked = true;

                this.rbnTimeAttend.Checked = true;
            }

            this.cboCompany.Enabled = true;

            this.rbnAccess.Enabled = true;
            this.rbnTimeAttend.Enabled = true;
            this.rbnBoth.Enabled = true;
            this.cboLockOut.Enabled = true;

            if (this.rbnBoth.Checked == true)
            {
                this.rbnClockNormal.Enabled = true;
                this.rbnClockFirstLast.Enabled = true;
            }

            this.rbnInOnly.Enabled = true;
            this.rbnOutOnly.Enabled = true;

            if (this.rbnTimeAttend.Checked == true)
            {
                this.rbnDynamic.Enabled = true;
                this.rbnClockRange.Enabled = true;
            }

            if (this.rbnClockRange.Checked == true)
            {
                this.cboFromHour.Enabled = true;
                this.cboFromMinute.Enabled = true;
                this.cboToHour.Enabled = true;
                this.cboToMinute.Enabled = true;

                if (this.cboFromHour.SelectedIndex == -1)
                {
                    this.cboFromHour.SelectedIndex = 0;
                    this.cboFromMinute.SelectedIndex = 0;

                    this.cboToHour.SelectedIndex = 12;
                    this.cboToMinute.SelectedIndex = 0;
                }
            }

            this.rbnWAN.Enabled = true;
            this.rbnLAN.Enabled = true;

            this.cboFAR.Enabled = true;

            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;
            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;
        }

        private void frmClock_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }

        private void rbnTimeAttend_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
               
                this.rbnDynamic.Enabled = true;
                this.rbnClockRange.Enabled = true;

                this.rbnClockNormal.Checked = false;
                this.rbnClockFirstLast.Checked = false;

                this.rbnClockNormal.Enabled = false;
                this.rbnClockFirstLast.Enabled = false;
            }
        }

        private void rbnAccess_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.rbnDynamic.Enabled = false;
                this.rbnClockRange.Enabled = false;

                this.rbnDynamic.Checked = false;
                this.rbnClockRange.Checked = false;

                this.rbnClockNormal.Checked = false;
                this.rbnClockFirstLast.Checked = false;

                this.rbnClockNormal.Enabled = false;
                this.rbnClockFirstLast.Enabled = false;
            }
        }

        private void rbnBoth_Click(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                this.rbnClockNormal.Enabled = true;
                this.rbnClockFirstLast.Enabled = true;

                this.rbnDynamic.Enabled = false;
                this.rbnClockRange.Enabled = false;

                this.rbnDynamic.Checked = false;
                this.rbnClockRange.Checked = false;
            }
        }

        private void rbnClockRange_CheckedChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (rbnClockRange.Checked == true)
                {
                    this.cboFromHour.Enabled = true;
                    this.cboFromMinute.Enabled = true;
                    this.cboToHour.Enabled = true;
                    this.cboToMinute.Enabled = true;

                    this.cboFromHour.SelectedIndex = 0;
                    this.cboFromMinute.SelectedIndex = 0;
                    this.cboToHour.SelectedIndex = 12;
                    this.cboToMinute.SelectedIndex = 0;
                }
                else
                {
                    this.cboFromHour.Enabled = false;
                    this.cboFromMinute.Enabled = false;
                    this.cboToHour.Enabled = false;
                    this.cboToMinute.Enabled = false;

                    this.cboFromHour.SelectedIndex = -1;
                    this.cboFromMinute.SelectedIndex = -1;
                    this.cboToHour.SelectedIndex = -1;
                    this.cboToMinute.SelectedIndex = -1;
                }
            }
        }

        private void dgvClockDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnClockDataGridViewLoaded == true)
            {
                if (pvtintClockDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintClockDataGridViewRowIndex = e.RowIndex;

                    pvtintDataViewReaderIndex = Convert.ToInt32(dgvClockDataGridView[this.pvtintClockRecCountCol, e.RowIndex].Value);

                    this.txtReader.Text = pvtDataView[pvtintDataViewReaderIndex]["DEVICE_DESC"].ToString();

                    this.pvtintDeviceNo = Convert.ToInt32(pvtDataView[pvtintDataViewReaderIndex]["DEVICE_NO"]);
                    this.txtDeviceNo.Text = pvtDataView[pvtintDataViewReaderIndex]["DEVICE_NO"].ToString();

                    if (pvtDataView[pvtintDataViewReaderIndex]["DEVICE_USAGE"].ToString() == "A")
                    {
                        this.rbnAccess.Checked = true;
                    }
                    else
                    {
                        if (pvtDataView[pvtintDataViewReaderIndex]["DEVICE_USAGE"].ToString() == "T")
                        {
                            this.rbnTimeAttend.Checked = true;
                        }
                        else
                        {
                            if (pvtDataView[pvtintDataViewReaderIndex]["DEVICE_USAGE"].ToString() == "B")
                            {
                                this.rbnBoth.Checked = true;
                            }
                            else
                            {
                                this.rbnTimeAttend.Checked = false;
                                this.rbnAccess.Checked = false;
                                this.rbnBoth.Checked = false;
                            }
                        }
                    }

                    if (pvtDataView[pvtintDataViewReaderIndex]["TIME_ATTEND_CLOCK_FIRST_LAST_IND"].ToString() == "Y")
                    {
                        this.rbnClockFirstLast.Checked = true;
                    }
                    else
                    {
                        if (pvtDataView[pvtintDataViewReaderIndex]["TIME_ATTEND_CLOCK_FIRST_LAST_IND"].ToString() == "N")
                        {
                            this.rbnClockNormal.Checked = true;
                        }
                        else
                        {
                            this.rbnClockNormal.Checked = false;
                            this.rbnClockFirstLast.Checked = false;
                        }
                    }

                    this.cboFromHour.SelectedIndex = -1;
                    this.cboFromMinute.SelectedIndex = -1;
                    this.cboToHour.SelectedIndex = -1;
                    this.cboToMinute.SelectedIndex = -1;

                    if (pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_OUT_PARM"].ToString() == "")
                    {
                        this.rbnClockRange.Checked = false;
                    }
                    else
                    {
                        if (pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_OUT_PARM"].ToString() == "I")
                        {
                            this.rbnInOnly.Checked = true;
                        }
                        else
                        {
                            if (pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_OUT_PARM"].ToString() == "O")
                            {
                                this.rbnOutOnly.Checked = true;
                            }
                            else
                            {
                                if (pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_OUT_PARM"].ToString() == "D")
                                {
                                    this.rbnDynamic.Checked = true;
                                }
                                else
                                {
                                    //Range
                                    this.rbnClockRange.Checked = true;

                                    string strTimeFrom = pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_RANGE_FROM"].ToString().PadLeft(4, '0');
                                    string strTimeTo = pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_RANGE_TO"].ToString().PadLeft(4, '0');

                                    for (int intRow = 0; intRow < this.cboFromHour.Items.Count; intRow++)
                                    {
                                        if (this.cboFromHour.Items[intRow].ToString() == strTimeFrom.Substring(0, 2))
                                        {
                                            this.cboFromHour.SelectedIndex = intRow;
                                            break;
                                        }
                                    }

                                    for (int intRow = 0; intRow < this.cboFromMinute.Items.Count; intRow++)
                                    {
                                        if (this.cboFromMinute.Items[intRow].ToString() == strTimeFrom.Substring(2))
                                        {
                                            this.cboFromMinute.SelectedIndex = intRow;
                                            break;
                                        }
                                    }

                                    for (int intRow = 0; intRow < this.cboToHour.Items.Count; intRow++)
                                    {
                                        if (this.cboToHour.Items[intRow].ToString() == strTimeTo.Substring(0, 2))
                                        {
                                            this.cboToHour.SelectedIndex = intRow;
                                            break;
                                        }
                                    }

                                    for (int intRow = 0; intRow < this.cboToMinute.Items.Count; intRow++)
                                    {
                                        if (this.cboToMinute.Items[intRow].ToString() == strTimeTo.Substring(2))
                                        {
                                            this.cboToMinute.SelectedIndex = intRow;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (pvtDataView[pvtintDataViewReaderIndex]["LOCK_OUT_MINUTES"] == System.DBNull.Value)
                    {
                        this.cboLockOut.SelectedIndex = -1;
                    }
                    else
                    {
                        this.cboLockOut.Text = pvtDataView[pvtintDataViewReaderIndex]["LOCK_OUT_MINUTES"].ToString();
                    }

                    this.cboCompany.SelectedIndex = -1;

                    if (pvtDataView[pvtintDataViewReaderIndex]["COMPANY_NO"] != System.DBNull.Value)
                    {
                        for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["Company"].Rows.Count; intRowCount++)
                        {
                            if (Convert.ToInt32(pvtDataView[pvtintDataViewReaderIndex]["COMPANY_NO"]) == Convert.ToInt32(pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_NO"]))
                            {
                                this.cboCompany.SelectedIndex = intRowCount;
                                break;
                            }
                        }
                    }

                    //WAN - Clock Pings Server every Minute
                    if (pvtDataView[pvtintDataViewReaderIndex]["LAN_WAN_IND"].ToString() == "W")
                    {
                        this.rbnWAN.Checked = true;
                    }
                    else
                    {
                        //LAN - Clock Pings Server every 15 Seconds
                        this.rbnLAN.Checked = true;
                    }

                    for (int intCount = 0; intCount < this.cboFAR.Items.Count; intCount++)
                    {
                        if (Convert.ToInt32(pvtDataView[pvtintDataViewReaderIndex]["FAR_REQUESTED"]) == intPossibleFARValuesArray[intCount])
                        {
                            this.cboFAR.SelectedIndex = intCount;

                            break;
                        }
                    }
                }
            }
        }

        private void dgvClockDataGridView_Sorted(object sender, EventArgs e)
        {
            if (dgvClockDataGridView.Rows.Count > 0)
            {
                if (dgvClockDataGridView.SelectedRows.Count > 0)
                {
                    if (dgvClockDataGridView.SelectedRows[0].Selected == true)
                    {
                        dgvClockDataGridView.FirstDisplayedScrollingRowIndex = dgvClockDataGridView.SelectedRows[0].Index; ;
                    }
                }
            }
        }

        private void dgvClockDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 1) 
            { 
                if (double.Parse(e.CellValue1.ToString()) > double.Parse(e.CellValue2.ToString())) 
                { 
                    e.SortResult = 1; 
                } 
                else if (double.Parse(e.CellValue1.ToString()) < double.Parse(e.CellValue2.ToString())) 
                { 
                    e.SortResult = -1; 
                }              
                else 
                { 
                    e.SortResult = 0; 
                } 
                
                e.Handled = true; 
            } 
        }
    }
}
