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
    public partial class frmUploadWarning : Form
    {
        public frmUploadWarning()
        {
            InitializeComponent();

            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmUploadWarning_Paint);
        }
        
        private void dgvPayCategoryDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 2)
            {
                if (dgvPayCategoryDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString() == "")
                {
                    e.SortResult = -1;
                }
                else
                {
                    if (dgvPayCategoryDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString() == "")
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString()) > double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 1, e.RowIndex1].Value.ToString()) < double.Parse(dgvPayCategoryDataGridView[e.Column.Index + 1, e.RowIndex2].Value.ToString()))
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

        private void frmUploadWarning_Paint(object sender, PaintEventArgs e)
        {
            Rectangle myRectangle = new Rectangle(this.ClientRectangle.X - 2, this.ClientRectangle.Y - 2, this.ClientRectangle.Width - 2, this.ClientRectangle.Height - 2);

            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
            System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
            System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
            System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
            System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid);

            Pen blackPen = new Pen(Color.Black, 1);
            e.Graphics.DrawLine(blackPen, 0, 31, this.Width, 31);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnHeaderClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
