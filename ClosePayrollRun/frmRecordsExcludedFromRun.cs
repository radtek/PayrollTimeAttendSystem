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
    public partial class frmRecordsExcludedFromRun : Form
    {
        public frmRecordsExcludedFromRun()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmRecordsExcludedFromRun_Paint(object sender, PaintEventArgs e)
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

        private void btnHeaderClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
