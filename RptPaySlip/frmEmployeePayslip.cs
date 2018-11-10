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
    public partial class frmEmployeePayslip : Form
    {
        private bool _dragging = false;
        private Point _offset;
        private Point _start_point = new Point(0, 0);

        public frmEmployeePayslip()
        {
            InitializeComponent();
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

        private void frmEmployeePayslip_Paint(object sender, PaintEventArgs e)
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

        private void lblHeader_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _start_point = new Point(e.X, e.Y);
        }

        private void lblHeader_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void lblHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Form myForm = (Form)((Label)sender).Parent;
                Label lblHeader = (Label)sender;

                //Cursor Position relative to lblHeader (On Screen)
                Point p = PointToScreen(new Point(Cursor.Position.X - PointToScreen(lblHeader.Location).X, Cursor.Position.Y - PointToScreen(lblHeader.Location).Y));

                myForm.Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("EmailPayslips", "Y");

            this.Close();
        }

        private void btnHeaderClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
