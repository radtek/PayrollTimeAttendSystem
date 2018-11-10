using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.dgvPayrollTypeDataGridView.Rows.Add("",
                                                     "");
            this.dgvPayrollTypeDataGridView.Rows.Add("",
                                                     "");
            this.dgvPayrollTypeDataGridView.Rows.Add("",
                                                     "");
        }

        private void dgvPayrollTypeDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                this.comboBox1.Text = dgvPayrollTypeDataGridView[dgvPayrollTypeDataGridView.CurrentCell.ColumnIndex, dgvPayrollTypeDataGridView.CurrentCell.RowIndex].Value.ToString();

                var cellRectangle = dgvPayrollTypeDataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                this.comboBox1.Left = dgvPayrollTypeDataGridView.Left + cellRectangle.Left - 1;
                this.comboBox1.Top = dgvPayrollTypeDataGridView.Top + cellRectangle.Top - 1;

                this.comboBox1.Visible = true;
            }
            else
            {
                this.comboBox1.Visible = false;

            }


        }


        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            dgvPayrollTypeDataGridView[dgvPayrollTypeDataGridView.CurrentCell.ColumnIndex, dgvPayrollTypeDataGridView.CurrentCell.RowIndex].Value = this.comboBox1.Text;

        }
    }
}
