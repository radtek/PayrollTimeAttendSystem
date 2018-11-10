using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using InteractPayroll;

namespace InteractPayrollClient
{                                                                    
    public partial class frmTimeAttendanceMain : Form
    {
        clsISClientUtilities clsISClientUtilities;

        clsISUtilities clsISUtilities;

        frmMessage frmMessage;

        private string pvtstrWebServerBeenUsed = "W";

        clsFileDownLoad clsFileDownLoad;
             
        Form CurrentActiveForm;
        Assembly myAssembly;
        ToolStripMenuItem myMenuItem;
        ToolStripButton myToolStripButton;
        
        string pvtstrControlText;

        private bool _dragging = false;
        private Point _offset;
        private Point _start_point = new Point(0, 0);

        int intHeight = 32;

        private DateTime pvtDateTimeLast = DateTime.Now;

        public frmTimeAttendanceMain()
        {
            InitializeComponent();
            
            System.Drawing.Rectangle rect = Screen.GetWorkingArea(this);
            this.MaximizedBounds = Screen.GetWorkingArea(this);
            this.WindowState = FormWindowState.Maximized;

            //Reposition Image And Menus
            int intControlsWidth = (this.toolMainStrip.Location.X + this.toolMainStrip.Width) - this.picValiditeImage.Location.X + 56;
            int intExtraMove = (this.MaximizedBounds.Width - intControlsWidth) / 2;

            this.picValiditeImage.Left = this.picValiditeImage.Left + intExtraMove;
            this.menuMainStrip.Left = this.menuMainStrip.Left + intExtraMove;
            this.toolMainStrip.Left = this.toolMainStrip.Left + intExtraMove;
            
            this.menuMainStrip.Renderer = new CustomMenuStriplRenderer();
            this.toolMainStrip.Renderer = new CustomMenuStriplRenderer();

            Disable_MDIForm_Edit_Items();

            AppDomain.CurrentDomain.SetData("TimerCloseForm",this.tmrCloseForm);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void MdiChild_ButtonClose_Click(object sender, EventArgs e)
        {
            Button currentButton = (Button)sender;

            Form currentForm = (Form)currentButton.Parent;

            currentForm.Close();
        }

        private void MdiChild_ButtonMinimize_Click(object sender, EventArgs e)
        {
            Button currentButton = (Button)sender;

            Form currentForm = (Form)currentButton.Parent;

            currentForm.WindowState = FormWindowState.Minimized;
        }

        public void MdiChild_Paint(object sender, PaintEventArgs e)
        {
            Form frm = (Form)sender;
            Rectangle myRectangle = new Rectangle(frm.ClientRectangle.X - 2, frm.ClientRectangle.Y - 2, frm.ClientRectangle.Width - 2, frm.ClientRectangle.Height - 2);

            if (this.ActiveMdiChild == frm
            || frm.Name == "frmCloseOrSave")
            {
                ControlPaint.DrawBorder(e.Graphics, frm.ClientRectangle,
                System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
                System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
                System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
                System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid);

                Pen blackPen = new Pen(Color.Black, 1);
                e.Graphics.DrawLine(blackPen, 0, 31, frm.Width, 31);
            }
            else
            {
                Color myColor = frm.BackColor;

                ControlPaint.DrawBorder(e.Graphics, frm.ClientRectangle,
                myColor, 1, ButtonBorderStyle.Solid,
                myColor, 1, ButtonBorderStyle.Solid,
                myColor, 1, ButtonBorderStyle.Solid,
                myColor, 1, ButtonBorderStyle.Solid);

                Pen myPen = new Pen(myColor, 1);
                e.Graphics.DrawLine(myPen, 0, 31, frm.Width, 31);
            }
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strPath = "";
            object lateBoundObj = null;
            Form myForm;

            if (pvtDateTimeLast > DateTime.Now.AddSeconds(-1))
            {
                return;
            }
            else
            {
                pvtDateTimeLast = DateTime.Now;
            }

            ToolStripMenuItem tsMenuItem = (ToolStripMenuItem) sender;

            AppDomain.CurrentDomain.SetData("LinkedMenuItem", tsMenuItem);

            try
            {
                switch (tsMenuItem.Name)
                {
                    case "convertCostCenterToolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "ClientConvertCostCentre.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmClientConvertCostCentre");

                        break;

                    case "fileDownloadToolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "FileDownload.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayroll.frmFileDownload");

                        break;

                    case "fileUploadToolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "FileUpload.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayroll.frmFileUpload");

                        break;

                    case "databaseBackupToolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "BackupRestoreClientDatabase.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmBackupDatabase");

                        break;

                    case "databaseRestoreToolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "BackupRestoreClientDatabase.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmRestoreDatabase");

                        break;

                    case "fingerPrintReadertoolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "ChooseFingerprintReader.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayroll.frmChooseFingerprintReader");

                        break;

                    case "clockReaderEmployeesLinkToolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "ClockToEmployeeLink.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmClockToEmployeeLink");

                        break;

                    case "dataDownloadToolStripMenuItem":
                    case "dataUploadTimeSheetsToolStripMenuItem":
                        
                        DateTime dtDownloadMessage = DateTime.Now.AddSeconds(3);

                        frmMessage = new InteractPayrollClient.frmMessage();
                        frmMessage.Show();

                        tsMenuItem.Enabled = false;

                        object[] objParm = null;
                         
                        DataSet DataSet = new DataSet();

                        pvtstrWebServerBeenUsed = "L";
                        clsISClientUtilities.Set_New_BusinessObjectName("busClientPayrollLogon");

                        //Check Client Databse ia Available / Check Tables are Correct
                        byte[] pvtbytCompress = (byte[])clsISClientUtilities.DynamicFunction("Logon_Client_DataBase", objParm, false);

                        if (pvtbytCompress == null)
                        {
                            frmMessage.Close();
                            frmMessage = null;

                            return;
                        }

                        DataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                        object[] obj = new object[1];
                        obj[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                        pvtstrWebServerBeenUsed = "W";
                        byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_New_Client_Download_Files", obj, false);

                        if (pvtbytCompress == null)
                        {
                            frmMessage.Close();
                            frmMessage = null;

                            return;
                        }
                        else
                        {
                            //Wait 3 Seconds
                            while (dtDownloadMessage > DateTime.Now)
                            {
                            }

                            frmMessage.Close();
                            frmMessage = null;
                        }

                        DataSet pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                        if (pvtDataSet.Tables["Files"].Rows.Count > 0)
                        {
                            bool blnLogoff = false;

                            int intReturnCode = clsFileDownLoad.DownLoad_Files_From_TimeAttendance(ref pvtDataSet, ref DataSet, ref blnLogoff, "Y");
                            
                            if (intReturnCode != 0)
                            {
                                if (AppDomain.CurrentDomain.GetData("KillApp").ToString() == "Y")
                                {
                                }
                                else
                                {
                                    System.Windows.Forms.MessageBox.Show("Error In Download of File.\nProgram Closing.", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                                //Force Program To Close
                                AppDomain.CurrentDomain.SetData("UserNo", -1);

                                this.Close();
                                return;
                            }

                            if (DataSet.Tables["FileToDelete"].Rows.Count > 0)
                            {
                                DataSet myDataSet = new System.Data.DataSet();

                                DataTable myDataTable = DataSet.Tables["FileToDelete"].Copy();
                                myDataSet.Tables.Add(myDataTable);

                                byte[] mybytCompress = clsISClientUtilities.Compress_DataSet(DataSet);

                                obj = new object[1];
                                obj[0] = mybytCompress;

                                clsISClientUtilities.DynamicFunction("Cleanup_Client_DataBase_Files", obj, false);
                            }

                            if (blnLogoff == true)
                            {
                                System.Windows.Forms.MessageBox.Show("Changes Have been Made to the Main Program.\nYou need to Restart the Program.",
                                "Program Changes",
                                MessageBoxButtons.OK,
                                 MessageBoxIcon.Exclamation);

                                AppDomain.CurrentDomain.SetData("Logoff", true);

                                this.Close();
                                return;
                            }
                        }

                        //Reset Back To busTimeAttendanceMain - What It was Initially
                        clsISClientUtilities.Set_New_BusinessObjectName("busTimeAttendanceMain");

                        if (tsMenuItem.Name == "dataDownloadToolStripMenuItem")
                        {
                            strPath = AppDomain.CurrentDomain.BaseDirectory + "TimeAttendanceDataDownload.dll";
                            myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                            lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmTimeAttendanceDataDownload");
                        }
                        else
                        {
                            strPath = AppDomain.CurrentDomain.BaseDirectory + "TimeAttendanceDataUpload.dll";
                            myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                            lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmTimeAttendanceDataUpload");
                        }
                       
                        break;
                     
                    case "employeeOverrideMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "EmployeeOverride.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmEmployeeOverride");

                        break;

                    case "usertoolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "UserClient.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmUserClient");

                        break;

                    case "fingerprintWebServerIPandPort":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "SetupInternetWebServerIP.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmSetupInternetWebServerIP");

                        break;

                    case "clockToolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "Clock.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmClock");

                        break;

                    case "clockInBoundarytoolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "ClockInBoundary.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmClockInBoundary");

                        break;

                    case "employeeToolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "EmployeeClient.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmEmployeeClient");
                                      
                        break;

                    case "employeeGroupToolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "EmployeeGroup.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmEmployeeGroup");

                        break;

                    case "employeeLinkToClockReaderToolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "EmployeeLinkDevice.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmEmployeeLinkDevice");

                        break;

                    case "timeSheetsToolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "TimeSheetClient.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmTimeSheetClient");

                        break;

                    case "timesheetBatchToolStripMenuItem":

                        strPath = AppDomain.CurrentDomain.BaseDirectory + "TimeSheetBatchClient.dll";
                        myAssembly = System.Reflection.Assembly.LoadFile(strPath);
                        lateBoundObj = myAssembly.CreateInstance("InteractPayrollClient.frmTimeSheetBatchClient");

                        break;

                    default:
                        
                        //Window - Dynamic Add/Delete of Forms
                        foreach (Form form in this.MdiChildren)
                        {
                            if (tsMenuItem.Tag != null)
                            {
                                if (tsMenuItem.Tag == form.Tag)
                                {
                                    if (this.ActiveMdiChild == form)
                                    {
                                    }
                                    else
                                    {
                                        form.Activate();
                                    }

                                    break;
                                }
                            }
                            else
                            {
                                if (tsMenuItem.Text.IndexOf(form.Text) > -1)
                                {
                                    if (this.ActiveMdiChild == form)
                                    {
                                    }
                                    else
                                    {
                                        form.Activate();
                                    }

                                    break;
                                }
                            }
                        }
                        
                        return;
                }

                myForm = (Form)lateBoundObj;
                myForm.MdiParent = this;
                myForm.Tag = tsMenuItem.Tag;

                if (myForm.FormBorderStyle == FormBorderStyle.None)
                {
                    if (myForm.Name != "frmPasswordChange")
                    {
                        myForm.Height += intHeight;
                    }
                }
                else
                {

                    myForm.Icon = this.Icon;
                    myForm.MaximizeBox = false;
                    myForm.MinimizeBox = false;
                }

                foreach (Control myControl in myForm.Controls)
                {
                    //ELR-2018-08-18
                    if (myForm.FormBorderStyle == FormBorderStyle.None
                    && myForm.Name != "frmPasswordChange")
                    {
                        myControl.Top += intHeight;
                    }

                    if (myControl is Button)
                    {
                        Button button = (Button)myControl;

#if (DEBUG)
                        if (button.FlatStyle != FlatStyle.Flat)
                        {
                            System.Windows.Forms.MessageBox.Show(button.Name + " Not FlatStyle");
                        }
#endif
                        button.FlatStyle = FlatStyle.Flat;

                        if (button.Text.ToUpper().IndexOf("CLOSE") > -1)
                        {
                            button.FlatAppearance.MouseOverBackColor = Color.Red;
                        }

                        //switch (myControl.Text.Replace("&", ""))
                        //{
                        //    case "New":
                        //    case "Update":
                        //    case "Delete":
                        //    case "Save":
                        //    case "Cancel":

                        //        myControl.EnabledChanged += new System.EventHandler(this.MdiChild_Edit_Button_EnabledChanged);

                        //        break;
                        //}
                    }
                }

                //ELR-2018-08-18
                if (myForm.FormBorderStyle == FormBorderStyle.None
                && myForm.Name != "frmPasswordChange")
                {
                    Label lblHeader = new Label();
                    lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    lblHeader.BackColor = System.Drawing.Color.DimGray;
                    lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    lblHeader.ForeColor = System.Drawing.Color.Black;
                    lblHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                    lblHeader.Location = new System.Drawing.Point(1, 1);
                    lblHeader.Name = "lblHeader";
                    lblHeader.Size = new System.Drawing.Size(myForm.Width - (2 * intHeight), intHeight - 2);
                    lblHeader.Text = tsMenuItem.Text;
                    lblHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseDown);
                    lblHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseMove);
                    lblHeader.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblHeader_MouseUp);

                    myForm.Text = lblHeader.Text;

                    myForm.Controls.Add(lblHeader);

                    Button btnHeaderClose = new Button();
                    btnHeaderClose.BackColor = System.Drawing.Color.DimGray;
                    btnHeaderClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
                    btnHeaderClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    btnHeaderClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    btnHeaderClose.ForeColor = System.Drawing.Color.Black;
                    btnHeaderClose.Location = new System.Drawing.Point(myForm.Width - intHeight, 0);
                    btnHeaderClose.Name = "btnHeaderClose";
                    btnHeaderClose.Size = new System.Drawing.Size(intHeight, intHeight);
                    btnHeaderClose.TabStop = false;
                    btnHeaderClose.Text = "X";
                    btnHeaderClose.Click += new System.EventHandler(MdiChild_ButtonClose_Click);

                    myForm.Controls.Add(btnHeaderClose);

                    Button btnHeaderMinimize = new Button();
                    btnHeaderMinimize.BackColor = System.Drawing.Color.DimGray;
                    btnHeaderMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    btnHeaderMinimize.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    btnHeaderMinimize.ForeColor = System.Drawing.Color.Black;
                    btnHeaderMinimize.Location = new System.Drawing.Point(myForm.Width - (2 * intHeight) + 1, 0);
                    btnHeaderMinimize.Name = "btnHeaderMinimize";
                    btnHeaderMinimize.Size = new System.Drawing.Size(intHeight, intHeight);
                    btnHeaderMinimize.TabStop = false;
                    btnHeaderMinimize.Text = "_";
                    btnHeaderMinimize.Click += new System.EventHandler(MdiChild_ButtonMinimize_Click);

                    myForm.Controls.Add(btnHeaderMinimize);
                }

                if (myForm.FormBorderStyle == FormBorderStyle.None)
                {
                    myForm.Paint += new System.Windows.Forms.PaintEventHandler(MdiChild_Paint);
                }

                System.Drawing.Rectangle rect = Screen.GetWorkingArea(this);

                int intX = (rect.Width - myForm.Width) / 2;
                int intY = (rect.Height - myForm.Height + this.pnlHeader.Height) / 2;

                if (AppDomain.CurrentDomain.GetData("FormSmallest") != null)
                {
                    //Errol Use For Screenshots Program
                    intY = this.pnlHeader.Height + 10;
                }

                myForm.StartPosition = FormStartPosition.Manual;
                myForm.Location = new Point(intX, intY);
                
                //Hook Up so That Menu Item Can br Enabled On Close
                myForm.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MdiChild_FormClosing);
                myForm.TextChanged += new System.EventHandler(MdiChild_TextChanged);
                myForm.Leave += new System.EventHandler(MdiChild_Leave);
                
                ToolStripMenuItem newToolStripMenuItem = new ToolStripMenuItem(myForm.Text);

                if (tsMenuItem.Tag != null)
                {
                    myForm.Tag = tsMenuItem.Tag;
                    newToolStripMenuItem.Tag = tsMenuItem.Tag;
                }
                
                windowToolStripMenuItem.DropDownItems.Add(newToolStripMenuItem);
                newToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
                
                myForm.Show();

                tsMenuItem.Enabled = false;
            }
            catch(Exception ex)
            {
                tsMenuItem.Enabled = true;

                if (pvtstrWebServerBeenUsed == "L")
                {
                    clsISClientUtilities.ErrorHandler(ex);
                }
                else
                {
                    clsISUtilities.ErrorHandler(ex);
                }

                if (frmMessage != null)
                {
                    frmMessage.Close();
                    frmMessage = null;
                }
            }
        }

        private void MdiChild_TextChanged(object sender, EventArgs e)
        {
            Form myForm = (Form)sender;

            foreach (Control myContol in myForm.Controls)
            {
                if (myContol.Name == "lblHeader")
                {
                    myContol.Text = myForm.Text;
                    break;
                }
            }
        }

        private void MdiChild_Leave(object sender, EventArgs e)
        {
            Form myForm = (Form)sender;
            Color myColor = myForm.BackColor;

            Control mylblHeaderControl = myForm.Controls["lblHeader"];

            if (mylblHeaderControl != null)
            {
                mylblHeaderControl.ForeColor = myColor;
            }

            Control mybtnHeaderCloseControl = myForm.Controls["btnHeaderClose"];

            if (mybtnHeaderCloseControl != null)
            {
                mybtnHeaderCloseControl.ForeColor = myColor;
            }

            Control mybtnHeaderMinimizeControl = myForm.Controls["btnHeaderMinimize"];

            if (mybtnHeaderMinimizeControl != null)
            {
                mybtnHeaderMinimizeControl.ForeColor = myColor;
            }
        }
        private void MdiChild_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form myForm = (Form)sender;

            Control mybtnUpdateSave = null;
            mybtnUpdateSave = myForm.Controls["btnSave"];
           
            if (mybtnUpdateSave != null)
            {
                if (mybtnUpdateSave.Enabled == true)
                {
                    frmCloseOrSave frmCloseOrSave = new frmCloseOrSave();
                    frmCloseOrSave.Paint += new System.Windows.Forms.PaintEventHandler(MdiChild_Paint);

                    DialogResult myDialogResult = frmCloseOrSave.ShowDialog();

                    if (myDialogResult == DialogResult.Yes)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
            
            //Remove From Window Drop Down
            for (int intCount = 0; intCount < windowToolStripMenuItem.DropDownItems.Count; intCount++)
            {
                if (myForm.Tag == windowToolStripMenuItem.DropDownItems[intCount].Tag)
                {
                    windowToolStripMenuItem.DropDownItems.RemoveAt(intCount);
                    break;
                }
            }
            
            //Set Form's MenuItem / ToolBarItem to Enabled
            //Errol Still To Fix
            //Enable_Disable_MenuItem_ToolBarItem_From_Forms_Tag(myForm.Tag.ToString(), true);
        }

        private void frmTimeAttendanceMain_MdiChildActivate(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
            {
                Disable_MDIForm_Edit_Items();
            }
            else
            {
                Activate_MenuItems(this.ActiveMdiChild);

                //Force Form to Redraw
                this.ActiveMdiChild.Invalidate();
            }
		}

        private void Disable_MDIForm_Edit_Items()
        {
            foreach (ToolStripItem tsItem in this.toolMainStrip.Items)
            {
                if (tsItem.Text == "New"
                | tsItem.Text == "Update"
                | tsItem.Text == "Delete"
                | tsItem.Text == "Save"
                | tsItem.Text == "Cancel")
                {
                    tsItem.Enabled = false;
                }
            }

            for (int intCount = 0; intCount < editToolStripMenuItem.DropDownItems.Count; intCount++)
            {
                if (editToolStripMenuItem.DropDownItems[intCount].Text == "New"
                | editToolStripMenuItem.DropDownItems[intCount].Text == "Update"
                | editToolStripMenuItem.DropDownItems[intCount].Text == "Delete"
                | editToolStripMenuItem.DropDownItems[intCount].Text == "Save"
                | editToolStripMenuItem.DropDownItems[intCount].Text == "Cancel")
                {
                    editToolStripMenuItem.DropDownItems[intCount].Enabled = false;
                }
            }
        }

        private void Activate_MenuItems(Form ActiveForm)
        {
            CurrentActiveForm = ActiveForm;
            bool blnButtonFound = false;
            
            Control mylblHeaderControl = CurrentActiveForm.Controls["lblHeader"];

            if (mylblHeaderControl != null)
            {
                mylblHeaderControl.ForeColor = Color.Black;
            }

            Control mybtnHeaderCloseControl = CurrentActiveForm.Controls["btnHeaderClose"];

            if (mybtnHeaderCloseControl != null)
            {
                mybtnHeaderCloseControl.ForeColor = Color.Black;
            }

            Control mybtnHeaderMinimizeControl = CurrentActiveForm.Controls["btnHeaderMinimize"];

            if (mybtnHeaderMinimizeControl != null)
            {
                mybtnHeaderMinimizeControl.ForeColor = Color.Black;
            }

            foreach (ToolStripItem tsItem in this.toolMainStrip.Items)
            {
                if (tsItem.Text == "New"
                | tsItem.Text == "Update"
                | tsItem.Text == "Delete"
                | tsItem.Text == "Save"
                | tsItem.Text == "Cancel")
                {
                    blnButtonFound = false;

                    //Find If Button Exist is Form
                    foreach (Control ActiveFormControl in ActiveForm.Controls)
                    {
                        if (ActiveFormControl.GetType().ToString() == "System.Windows.Forms.Button")
                        {
                            if (ActiveFormControl.Text.Replace("&","") == tsItem.Text)
                            {
                                blnButtonFound = true;
                                tsItem.Enabled = ActiveFormControl.Enabled;

                                //Hook Up Enable / Disable Event
                                ActiveFormControl.EnabledChanged += new System.EventHandler(Button_EnabledChanged);
                                
                                for(int intCount = 0;intCount < editToolStripMenuItem.DropDownItems.Count;intCount++)
                                {
                                    if (editToolStripMenuItem.DropDownItems[intCount].Text == ActiveFormControl.Text.Replace("&", ""))
                                    {
                                        editToolStripMenuItem.DropDownItems[intCount].Enabled = ActiveFormControl.Enabled;
                                        break;
                                    }
                                }

                                break;
                            }
                        }
                    }

                    if (blnButtonFound == false)
                    {
                        tsItem.Enabled = false;
                    }
                }
            }
        }

        private void Button_EnabledChanged(object sender, EventArgs e)
        {
            Button myButton = (Button)sender;

            foreach (ToolStripItem tsItem in this.toolMainStrip.Items)
            {
                if (tsItem.Text == myButton.Text.Replace("&", ""))
                {
                    tsItem.Enabled = myButton.Enabled;

                    break;
                }
            }

            for (int intCount = 0; intCount < editToolStripMenuItem.DropDownItems.Count; intCount++)
            {
                if (editToolStripMenuItem.DropDownItems[intCount].Text == myButton.Text.Replace("&", ""))
                {
                    editToolStripMenuItem.DropDownItems[intCount].Enabled = myButton.Enabled;
                    break;
                }
            }
        }

        private void EditControl_MainForm_Click(object sender, EventArgs e)
        {
            myAssembly = Assembly.GetAssembly(CurrentActiveForm.GetType());

            if (sender.GetType().ToString() == "System.Windows.Forms.ToolStripMenuItem")
            {
                myMenuItem = (ToolStripMenuItem)sender;
                pvtstrControlText = myMenuItem.Text;
            }
            else
            {
                myToolStripButton = (ToolStripButton)sender;
                pvtstrControlText = myToolStripButton.Text;
            }
         
            try
            {
                ActiveMdiChild.GetType().InvokeMember("btn" + pvtstrControlText + "_Click", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, this.ActiveMdiChild, new object[] { null, e });
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("'" + ActiveMdiChild.GetType().ToString() + ".btn" + pvtstrControlText + "_Click' Needs to be Set to Public.\n\nSpeak to System Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button_MainForm_Click(object sender, EventArgs e)
        {
            ToolStripButton myToolStripButton = (ToolStripButton)sender;
            ToolStripMenuItem myToolStripMenuItem = null;
 
            switch (myToolStripButton.Name)
            {
                case "DataDownloadtoolStripButton":

                    myToolStripMenuItem = dataDownloadToolStripMenuItem;
                    break;

                case "DataUploadtoolStripButton":

                    myToolStripMenuItem = dataUploadTimeSheetsToolStripMenuItem;
                    break;

                case "employeeToolStripButton":

                    myToolStripMenuItem = employeeToolStripMenuItem;
                    break;

                case "employeeGroupToolStripButton":

                    myToolStripMenuItem = employeeGroupToolStripMenuItem;
                    break;

                case "timesheetToolStripButton":

                    myToolStripMenuItem = this.timeSheetsToolStripMenuItem;
                    break;

                case "timesheetBatchToolStripButton":

                    myToolStripMenuItem = this.timesheetBatchToolStripMenuItem;
                    break;

                case "UserToolStripButton":

                    myToolStripMenuItem = this.usertoolStripMenuItem;
                    break;

                case "DeviceToolStripButton":

                    myToolStripMenuItem = this.clockToolStripMenuItem;
                    break;

                case "deviceEmployeeToolStripButton":

                    myToolStripMenuItem = this.employeeLinkToClockReaderToolStripMenuItem;
                    break;
            }

            ToolStripMenuItem_Click(myToolStripMenuItem, e);
        }

        private void tmrCloseForm_Tick(object sender, EventArgs e)
        {
            tmrCloseForm.Enabled = false;
            this.pnlGlobe.Visible = false;
            this.Refresh();

            try
            {
                //Enable Edit Menus
                foreach (Control myControl in this.ActiveMdiChild.Controls)
                {
                    if (myControl.GetType().ToString() == "System.Windows.Forms.Button")
                    {
                        string strControlText = myControl.Text.Replace("&", "");

                        if (strControlText == "Save"
                            | strControlText == "Cancel")
                        {
                            myControl.Enabled = false;
                        }
                    }
                }

                try
                {
                    this.ActiveMdiChild.Close();
                }
                catch
                {
                }
            }
            catch
            {
            }
        }

        private void frmTimeAttendanceMain_Load(object sender, EventArgs e)
        {
            try
            {
                AppDomain.CurrentDomain.SetData("Globe", pnlGlobe);

                clsISClientUtilities = new clsISClientUtilities(this, "busTimeAttendanceMain");

                clsISUtilities = new clsISUtilities(this,"busPayrollLogon");

                clsISUtilities.Set_WebService_Timeout_Value(50000);

                clsFileDownLoad = new clsFileDownLoad();

                if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() == "U")
                {
                    toolStripSeparator1.Visible = false;

                    this.setupToolStripMenuItem.Visible = false;
                    this.viewToolStripMenuItem.Visible = false;

                    this.UserToolStripButton.Visible = false;
                    this.DeviceToolStripButton.Visible = false;
                    this.employeeToolStripButton.Visible = false;

                    this.employeeGroupToolStripButton.Visible = false;
                    this.deviceEmployeeToolStripButton.Visible = false;
                }

                pvtstrWebServerBeenUsed = "L";
                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Form_Records", null, false);
                DataSet DataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "URLClientConfig.txt");

                if (fiFileInfo.Exists == true)
                {
                    StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "URLClientConfig.txt");

                    string strURLClientConfig = srStreamReader.ReadLine();

                    string[] strParameters = strURLClientConfig.Split(':');

                    if (strParameters.Length != 2)
                    {
                        System.Windows.Forms.MessageBox.Show("Error in 'URLClientConfig.txt'. Contents = " + strURLClientConfig + "\n\nSpeak to System Administrator.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (strParameters[0] == "0.0.0.0")
                    {
                        this.Text += " [Local]";
                    }
                    else
                    {
                        this.Text += " [" + strURLClientConfig + "]";
                    }
                }
                else
                {
                    this.Text += " [Local]";
                }

                string strSoftwareToUse = "";

                if (DataSet.Tables["SoftwareToUse"].Rows.Count == 0)
                {
                    strSoftwareToUse = "D";
                }
                else
                {
                    strSoftwareToUse = DataSet.Tables["SoftwareToUse"].Rows[0]["FINGERPRINT_SOFTWARE_IND"].ToString();
                }

                AppDomain.CurrentDomain.SetData("BiometricSoftware", strSoftwareToUse);
                AppDomain.CurrentDomain.SetData("MdiForm", this);

                if (strSoftwareToUse == "D")
                {
                    this.Text += " [Digital Persona]";
                }
                else
                {
                    this.Text += " [Griaule]";
                }

                AppDomain.CurrentDomain.SetData("InternetGlobe", this.pnlGlobe);
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void ToolStripMenuItem_EnabledChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem myToolStripMenuItem = (ToolStripMenuItem)sender;
          
            foreach (ToolStripItem tsItem in this.toolMainStrip.Items)
            {
                if (myToolStripMenuItem.Tag != null)
                {
                    if (tsItem.Tag == myToolStripMenuItem.Tag)
                    {
                        tsItem.Enabled = myToolStripMenuItem.Enabled;

                        break;
                    }
                }
                else
                {
                    if (tsItem.Text == myToolStripMenuItem.Text.Replace("&", ""))
                    {
                        tsItem.Enabled = myToolStripMenuItem.Enabled;

                        break;
                    }
                }
            }
        }

        private void btnHeaderClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimise_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void toolMainStrip_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void toolMainStrip_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void menuMainStrip_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void menuMainStrip_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
    }

    class CustomMenuStriplRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected)
            {
                base.OnRenderButtonBackground(e);
            }
            else
            {
                Rectangle rectangle = new Rectangle(0, 0, e.Item.Size.Width, e.Item.Size.Height);
                e.Graphics.FillRectangle(Brushes.Silver, rectangle);
                //e.Graphics.DrawRectangle(Pens.Black, rectangle);
            }
        }
        
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected == true)
            {
                if (e.Item.Name == "fileToolStripMenuItem"
                || e.Item.Name == "editToolStripMenuItem"
                || e.Item.Name == "setupToolStripMenuItem"
                || e.Item.Name == "viewToolStripMenuItem"
                || e.Item.Name == "processToolStripMenuItem"
                || e.Item.Name == "windowToolStripMenuItem")
                {
                    e.Item.ForeColor = Color.Silver;
                    ToolStripDropDownItem menuItem = (ToolStripDropDownItem)e.Item;
                    menuItem.ShowDropDown();
                }
                else
                {
                    Rectangle rc = new Rectangle(1, 0, e.Item.Size.Width, e.Item.Size.Height);

                    if (e.Item.Enabled == true)
                    {
                        e.Graphics.FillRectangle(Brushes.DimGray, rc);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(Brushes.Silver, rc);
                    }
                }
            }
            else
            {
                if (e.Item.Enabled == true)
                {
                    //Disabled
                    e.Item.ForeColor = Color.Black;
                }
                else
                {
                    e.Item.ForeColor = Color.Silver;
                }
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //Removes White Line Under ToolStrip
            //base.OnRenderToolStripBorder(e);
        }
    }








}
