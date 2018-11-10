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
    public partial class frmPasswordChange : Form
    {
        clsISUtilities clsISUtilities;
        
        public frmPasswordChange()
        {
            AppDomain.CurrentDomain.SetData("PasswordChanged", "N");

            InitializeComponent();
        }


        public void Form_Paint(object sender, PaintEventArgs e)
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

        private void frmPasswordChange_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this,"busPasswordChange");

                if (this.Tag == null)
                {
                    this.Paint += new System.Windows.Forms.PaintEventHandler(Form_Paint);

                    this.btnHeaderMinimize.Visible = false;
                }
          
                this.lblHeader.MouseDown += clsISUtilities.lblHeader_MouseDown;
                this.lblHeader.MouseMove += clsISUtilities.lblHeader_MouseMove;
                this.lblHeader.MouseUp += clsISUtilities.lblHeader_MouseUp;
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("PasswordChanged", "N");
            this.Close();
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (this.txtPassword1.Text.Trim().ToUpper() == this.txtPassword2.Text.Trim().ToUpper())
                {
                    if (this.txtPassword1.Text.Trim() == "")
                    {
                        CustomMessageBox.Show("Enter Passwords", this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        this.txtPassword1.Focus();
                        return;
                    }
                }
                else
                {
                    CustomMessageBox.Show("Passwords are NOT the same", this.Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    this.txtPassword1.Focus();
                    return;
                }

                DialogResult dlgResult = CustomMessageBox.Show("Are you sure you want to Change your Password?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dlgResult == DialogResult.Yes)
                {
                    string strPassword = this.txtPassword1.Text.Trim().ToUpper();
                    Int64 int64CurrentUserNo = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                    object[] objParm = new object[2];
                    objParm[0] = int64CurrentUserNo;
                    objParm[1] = strPassword;

                    clsISUtilities.DynamicFunction("Update_Password", objParm,true);

                    AppDomain.CurrentDomain.SetData("PasswordChanged", "Y");
                    AppDomain.CurrentDomain.SetData("NewPassword", strPassword);

                    this.Close();
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnHeaderClose_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("PasswordChanged", "N");
            this.Close();
        }

        private void btnHeaderMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
