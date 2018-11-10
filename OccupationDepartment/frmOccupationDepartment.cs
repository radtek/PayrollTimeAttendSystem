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
    public partial class frmOccupationDepartment : Form
    {
        clsISUtilities clsISUtilities;

        public DataSet pvtDataSet;
        public DataView pvtOccupationDepartmentDataView;
        public DataView pvtEmployeeDataView;
        public DataSet pvtTempDataSet;
        private DataRowView pvtDataRowView;

        private int pvtintOccupationDepartmentNo = -1;
       
        private int pvtintOccupationDepartmentRowIndex;

        private string pvtstrMenuId = "";
        private string pvtstrTableName = "";

        private byte[] pvtbytCompress;

        private bool pvtblnOccupationDepartmentDataGridViewLoaded = false;

        public frmOccupationDepartment()
        {
            InitializeComponent();
        }

        private void frmOccupationDepartment_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busOccupationDepartment");

                this.lblDescription.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                pvtstrMenuId = AppDomain.CurrentDomain.GetData("MenuId").ToString();

                if (pvtstrMenuId == "3A")
                {
                    this.Name = "frmOccupation";
                    this.lblDescription.Text = "Occupation";
                    pvtstrTableName = "OCCUPATION";

                    clsISUtilities.NotDataBound_TextBox(this.txtOccupationDepartment, "Enter Occupation.");
                }
                else
                {
                    pvtstrTableName = "DEPARTMENT";

                    clsISUtilities.NotDataBound_TextBox(this.txtOccupationDepartment, "Enter Department.");
                }

                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = pvtstrMenuId;
              
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public void Load_CurrentForm_Records()
        {
            int intOccupationDepartmentRow = 0;
            this.btnNew.Enabled = true;

            this.Clear_DataGridView(this.dgvOccupationDepartmentDataGridView);

            pvtOccupationDepartmentDataView = new DataView(pvtDataSet.Tables["OccupationDepartment"],
                "",
                pvtstrTableName + "_DESC",
                DataViewRowState.CurrentRows);

            this.pvtblnOccupationDepartmentDataGridViewLoaded = false;

            for (int intRow = 0; intRow < pvtOccupationDepartmentDataView.Count; intRow++)
            {
                this.dgvOccupationDepartmentDataGridView.Rows.Add(pvtOccupationDepartmentDataView[intRow][pvtstrTableName + "_DESC"].ToString(),
                                                                  intRow.ToString());

                if (Convert.ToInt32(pvtOccupationDepartmentDataView[intRow][pvtstrTableName + "_NO"]) == pvtintOccupationDepartmentNo)
                {
                    intOccupationDepartmentRow = intRow;
                }
            }

            this.pvtblnOccupationDepartmentDataGridViewLoaded = true;

            if (this.dgvOccupationDepartmentDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvOccupationDepartmentDataGridView, intOccupationDepartmentRow);
            
                this.btnUpdate.Enabled = true;
                this.btnDelete.Enabled = true;
            }
            else
            {
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
                    case "dgvOccupationDepartmentDataGridView":

                        pvtintOccupationDepartmentRowIndex = -1;
                        this.dgvOccupationDepartmentDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

                string strDesc = "";
                string[] strDescPiesces = this.txtOccupationDepartment.Text.Trim().Split(' ');

                for (int intCount = 0; intCount < strDescPiesces.Length; intCount++)
                {
                    strDescPiesces[intCount] = strDescPiesces[intCount].Substring(0, 1).ToUpper() + strDescPiesces[intCount].Substring(1).ToLower();

                    if (intCount == 0)
                    {
                        strDesc = strDescPiesces[intCount];
                    }
                    else
                    {
                        strDesc += " " + strDescPiesces[intCount];
                    }
                }

                if (this.Text.EndsWith(" - New") == true)
                {
                    object[] objParm = new object[4];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[2] = pvtstrMenuId;
                    objParm[3] = strDesc;

                    pvtintOccupationDepartmentNo = (int)clsISUtilities.DynamicFunction("Insert_New_Record", objParm,true);
                    
                    pvtDataRowView = pvtOccupationDepartmentDataView.AddNew();

                    pvtDataRowView.BeginEdit();

                    pvtDataRowView[pvtstrTableName + "_NO"] = pvtintOccupationDepartmentNo;
                    pvtDataRowView[pvtstrTableName + "_DESC"] = strDesc;

                    pvtDataRowView.EndEdit();

                    Load_CurrentForm_Records();
                }
                else
                {
                    object[] objParm = new object[5];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtintOccupationDepartmentNo;
                    objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[3] = pvtstrMenuId;
                    objParm[4] = strDesc;
                   
                    clsISUtilities.DynamicFunction("Update_Record", objParm,true);

                    pvtOccupationDepartmentDataView[pvtintOccupationDepartmentRowIndex][pvtstrTableName + "_DESC"] = strDesc;
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
                this.txtOccupationDepartment.Text = "";
            }

            this.dgvOccupationDepartmentDataGridView.Enabled = false;
            picOccupationDepartmentLock.Visible = true;

            clsISUtilities.Set_Form_For_Edit(blnNew);
            
            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.txtOccupationDepartment.Enabled = true;

            this.txtOccupationDepartment.Focus();
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

            this.dgvOccupationDepartmentDataGridView.Enabled = true;
            picOccupationDepartmentLock.Visible = false;

            clsISUtilities.Set_Form_For_Read();

            this.btnNew.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.txtOccupationDepartment.Enabled = false;

            if (this.pvtOccupationDepartmentDataView.Count > 0)
            {
                this.btnUpdate.Enabled = true;
                this.btnDelete.Enabled = true;
            }
            else
            {
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;

                this.txtOccupationDepartment.Text = "";
            }
   
            Load_CurrentForm_Records();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomMessageBox.Show("Delete " + this.Text + " '" + this.pvtOccupationDepartmentDataView[pvtintOccupationDepartmentRowIndex][this.pvtstrTableName + "_DESC"].ToString() + "'",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[4];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                    objParm[1] = pvtintOccupationDepartmentNo;
                    objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[3] = pvtstrMenuId;

                    clsISUtilities.DynamicFunction("Delete_Record", objParm,true);
                    
                    this.pvtintOccupationDepartmentNo = -1;

                    pvtOccupationDepartmentDataView[pvtintOccupationDepartmentRowIndex].Delete();

                    this.pvtDataSet.AcceptChanges();

                    Load_CurrentForm_Records();

                    if (pvtOccupationDepartmentDataView.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvOccupationDepartmentDataGridView, 0);
                    }
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void dgvOccupationDepartmentDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnOccupationDepartmentDataGridViewLoaded == true)
            {
                if (pvtintOccupationDepartmentRowIndex != e.RowIndex)
                {
                    pvtintOccupationDepartmentRowIndex = Convert.ToInt32(this.dgvOccupationDepartmentDataGridView[1, e.RowIndex].Value);

                    pvtintOccupationDepartmentNo = Convert.ToInt32(pvtOccupationDepartmentDataView[pvtintOccupationDepartmentRowIndex][pvtstrTableName + "_NO"]);
                    this.txtOccupationDepartment.Text = pvtOccupationDepartmentDataView[pvtintOccupationDepartmentRowIndex][pvtstrTableName + "_DESC"].ToString();
                }
            }
        }
    }
}
