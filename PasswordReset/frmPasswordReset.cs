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
    public partial class frmPasswordReset : Form
    {
        clsISUtilities clsISUtilities;

        private int pvtintUserDataGridViewRowIndex = -1;

        DataSet pvtDataSet;

        private string pvtstrUserId = "";
        private string pvtstrPassword = "";

        private bool pvtblnUserDataGridViewLoaded = false;
        
        public frmPasswordReset()
        {
            InitializeComponent();
        }

        private void frmPassword_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busPasswordReset");

                this.lblUser.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void Load_CurrentForm_Records()
        {
            try
            {
                object[] objParm = new object[2];
                objParm[0] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                this.Clear_DataGridView(this.dgvUserDataGridView);

                pvtblnUserDataGridViewLoaded = false;

                for (int intRowCount = 0; intRowCount < this.pvtDataSet.Tables["User"].Rows.Count; intRowCount++)
                {
                    this.dgvUserDataGridView.Rows.Add(this.pvtDataSet.Tables["User"].Rows[intRowCount]["USER_ID"].ToString(),
                                                      this.pvtDataSet.Tables["User"].Rows[intRowCount]["SURNAME"].ToString(),
                                                      this.pvtDataSet.Tables["User"].Rows[intRowCount]["FIRSTNAME"].ToString(),
                                                      this.pvtDataSet.Tables["User"].Rows[intRowCount]["USER_NO"].ToString());
                }

                pvtblnUserDataGridViewLoaded = true;

                if (this.dgvUserDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvUserDataGridView, 0);
                }
                else
                {
                    this.btnOK.Enabled = false;
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
                    case "dgvUserDataGridView":

                        pvtintUserDataGridViewRowIndex = -1;
                        this.dgvUserDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomMessageBox.Show("Are you sure you want to Reset the Password for\n\nUser Id = " + this.dgvUserDataGridView[0, this.Get_DataGridView_SelectedRowIndex(this.dgvUserDataGridView)].Value.ToString() + "\nSurname = " + this.dgvUserDataGridView[1, this.Get_DataGridView_SelectedRowIndex(this.dgvUserDataGridView)].Value.ToString() + "\nName = " + this.dgvUserDataGridView[2, this.Get_DataGridView_SelectedRowIndex(this.dgvUserDataGridView)].Value.ToString(),
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dlgResult == DialogResult.Yes)
                {
                    Int64 int64UserNo = Convert.ToInt64(this.dgvUserDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvUserDataGridView)].Value);

                    object[] objParm = new object[2];
                    objParm[0] = int64UserNo;
                    objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                    pvtstrPassword = (string)clsISUtilities.DynamicFunction("Update_Password", objParm,true);

                    CustomMessageBox.Show("User Id = " + pvtstrUserId + "\n\nNew Password = " + pvtstrPassword, this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void dgvUserDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnUserDataGridViewLoaded == true)
            {
                if (pvtintUserDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintUserDataGridViewRowIndex = e.RowIndex;

                    pvtstrUserId = this.dgvUserDataGridView[0, e.RowIndex].Value.ToString();

                    this.txtUser.Text = this.dgvUserDataGridView[0, e.RowIndex].Value.ToString();
                    this.txtSurname.Text = this.dgvUserDataGridView[1, e.RowIndex].Value.ToString();
                    this.txtName.Text = this.dgvUserDataGridView[2, e.RowIndex].Value.ToString();
                }
            }
        }
    }
}
