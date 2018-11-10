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
    public partial class frmLoadNewDemoRun : Form
    {
        clsISUtilities clsISUtilities;

        private Boolean pvtblnDateDataGridViewLoaded = false;

        private int pvtintDateDataGridViewRowIndex = -1;

        private string pvtstrDate = "";

        private DataSet pvtDataSet;
        public frmLoadNewDemoRun()
        {
            InitializeComponent();
        }

        private void frmLoadNewDemoRun_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busLoadNewDemoRun");

                this.lblDate.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                object[] objParm = new object[1];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);
                
                this.lblDesc.Text = "Choose Date to Load Company " + pvtDataSet.Tables["CompanyName"].Rows[0]["COMPANY_DESC"].ToString();
                
                for (int intRow = 0; intRow < pvtDataSet.Tables["PayDates"].Rows.Count; intRow++)
                {
                    this.dgvDateDataGridView.Rows.Add(Convert.ToDateTime(pvtDataSet.Tables["PayDates"].Rows[intRow]["PAY_PERIOD_DATE"]).ToString("dd MMM yyyy"),
                                                      Convert.ToDateTime(pvtDataSet.Tables["PayDates"].Rows[intRow]["PAY_PERIOD_DATE"]).ToString("yyyy-MM-dd"));
                }

                pvtblnDateDataGridViewLoaded = true;

                if (pvtDataSet.Tables["PayDates"].Rows.Count > 0)
                {
                    Set_DataGridView_SelectedRowIndex(dgvDateDataGridView, 0);
                    this.btnLoad.Enabled = true;
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult myDialogResult = CustomMessageBox.Show("Are you sure you want to Load?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (myDialogResult == DialogResult.Yes)
                {
                    object[] objParm = new object[2];
                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                    objParm[1] = pvtstrDate;

                    clsISUtilities.DynamicFunction("Load_New_Run", objParm, false);

                    CustomMessageBox.Show("Load Successful", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void dgvDateDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnDateDataGridViewLoaded == true)
            {
                if (pvtintDateDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintDateDataGridViewRowIndex = e.RowIndex;

                    pvtstrDate = this.dgvDateDataGridView[1, e.RowIndex].Value.ToString();
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
                    case "dgvDateDataGridView":

                        pvtintDateDataGridViewRowIndex = -1;
                        this.dgvDateDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

    }
}
