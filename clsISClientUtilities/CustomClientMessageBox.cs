using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InteractPayrollClient
{
    public partial class CustomClientMessageBox : Form
    {
        private bool _dragging = false;
        private Point _start_point = new Point(0, 0);

        public static DialogResult Show(string message, string headerText, MessageBoxButtons messageBoxButtons, MessageBoxIcon messageBoxIcon)
        {
            using (CustomClientMessageBox form = new CustomClientMessageBox(message, headerText, messageBoxButtons, messageBoxIcon))
            {
                return form.ShowDialog();
            }
        }

        public CustomClientMessageBox(string message, string headerText, MessageBoxButtons messageBoxButtons, MessageBoxIcon messageBoxIcon)
        {
            InitializeComponent();

            this.lblHeader.Text = headerText;
            this.lblText.Text = message;

            //if (message.IndexOf("FingerPrintClockTimeAttendanceService") > -1
            //|| message.IndexOf("Local Database CONVERT FAILURE") > -1
            //|| message.IndexOf("Currently a Payroll/Time Attendance Run Date Open") > -1
            //|| message.IndexOf("Employee/s with 2 or More Records Exist") > -1
            //|| message.IndexOf("Binding Error") > -1)
            //{
            //    this.lblText.Height += 75;
            //    this.btnOk.Top += 75;
            //    this.Height += 75;

            //    this.Width += 75;
            //    this.lblText.Width += 75;
            //    this.lblHeader.Width += 75;

            //    this.btnOk.Left += 25;
            //}

            if (messageBoxButtons == MessageBoxButtons.YesNoCancel)
            {
                int intbtnCancelLeft = this.btnCancel.Left;
                int intbtnNoLeft = this.btnNo.Left;

                this.btnNo.Left = intbtnCancelLeft;
                this.btnCancel.Left = intbtnNoLeft;
                this.btnCancel.Top = this.btnNo.Top;

                this.btnYes.Left -= 34;
                this.btnCancel.Left += 34;

                this.btnYes.Visible = true;
                this.btnNo.Visible = true;
                this.btnCancel.Visible = true;
            }
            else
            {
                if (messageBoxButtons == MessageBoxButtons.OK)
                {
                    this.btnOk.Visible = true;
                }
                else
                {
                    if (messageBoxButtons == MessageBoxButtons.YesNo)
                    {
                        this.btnYes.Visible = true;
                        this.btnNo.Visible = true;
                    }
                    else
                    {
                        if (messageBoxButtons == MessageBoxButtons.OKCancel)
                        {
                            this.btnOk.Left = this.btnYes.Left;
                            this.btnCancel.Left = this.btnNo.Left;

                            this.btnCancel.Top = this.btnNo.Top;

                            this.btnOk.Visible = true;
                            this.btnCancel.Visible = true;
                        }
                        else
                        {
                            MessageBox.Show("MessageBoxButtons Not Catered For");
                        }
                    }
                }
            }

            //Reposition Buttons
            int intNewTop = this.lblText.Height + 60;

            if (this.btnOk.Top < intNewTop)
            {
                this.btnOk.Top = intNewTop;
                this.btnYes.Top = intNewTop;
                this.btnNo.Top = intNewTop;
                this.btnCancel.Top = intNewTop;

                this.Height = intNewTop + 40;
            }

            if (messageBoxIcon == MessageBoxIcon.Exclamation)
            {
                this.picPicture.Image = InteractPayrollClient.Properties.Resources.Exclamation96;
            }
            else
            {
                if (messageBoxIcon == MessageBoxIcon.Error)
                {
                    this.picPicture.Image = InteractPayrollClient.Properties.Resources.Error96;
                }
                else
                {
                    if (messageBoxIcon == MessageBoxIcon.Information)
                    {
                        this.picPicture.Image = InteractPayrollClient.Properties.Resources.Info96;
                    }
                    else
                    {
                        if (messageBoxIcon == MessageBoxIcon.Warning)
                        {
                            this.picPicture.Image = InteractPayrollClient.Properties.Resources.Exclamation96;
                        }
                        else
                        {
                            if (messageBoxIcon == MessageBoxIcon.Question)
                            {
                                this.picPicture.Image = InteractPayrollClient.Properties.Resources.Question96;
                            }
                            else
                            {
                                MessageBox.Show("messageBoxIcon Not Catered For");
                            }
                        }
                    }
                }
            }

            this.Paint += Form_Paint;
            this.lblHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(lblHeader_MouseDown);
            this.lblHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(lblHeader_MouseMove);
            this.lblHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(lblHeader_MouseUp);
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            Form frm = (Form)sender;
            Rectangle myRectangle = new Rectangle(frm.ClientRectangle.X - 2, frm.ClientRectangle.Y - 2, frm.ClientRectangle.Width - 2, frm.ClientRectangle.Height - 2);

            ControlPaint.DrawBorder(e.Graphics, frm.ClientRectangle,
            System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
            System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
            System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
            System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid);

            Pen blackPen = new Pen(Color.Black, 1);
            e.Graphics.DrawLine(blackPen, 0, 31, frm.Width, 31);
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
                Point p = myForm.PointToScreen(new Point(Cursor.Position.X - myForm.PointToScreen(lblHeader.Location).X, Cursor.Position.Y - myForm.PointToScreen(lblHeader.Location).Y));

                myForm.Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);
            }
        }
    }
}
