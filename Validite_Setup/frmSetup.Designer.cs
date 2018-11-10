namespace Setup
{
    partial class frmSetup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetup));
            this.chkTimeAttendanceService = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkClock = new System.Windows.Forms.CheckBox();
            this.chkDatabase = new System.Windows.Forms.CheckBox();
            this.chkTimeAttendance = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkPayroll = new System.Windows.Forms.CheckBox();
            this.chkInternetTimeAttendance = new System.Windows.Forms.CheckBox();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.btnInstall = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkTimeAttendanceService
            // 
            this.chkTimeAttendanceService.AutoSize = true;
            this.chkTimeAttendanceService.Location = new System.Drawing.Point(15, 32);
            this.chkTimeAttendanceService.Name = "chkTimeAttendanceService";
            this.chkTimeAttendanceService.Size = new System.Drawing.Size(137, 17);
            this.chkTimeAttendanceService.TabIndex = 0;
            this.chkTimeAttendanceService.Text = "Install Validite Services ";
            this.chkTimeAttendanceService.UseVisualStyleBackColor = true;
            this.chkTimeAttendanceService.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            this.chkTimeAttendanceService.Click += new System.EventHandler(this.chkTimeAttendanceService_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkClock);
            this.groupBox1.Controls.Add(this.chkDatabase);
            this.groupBox1.Controls.Add(this.chkTimeAttendance);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.txtInfo);
            this.groupBox1.Controls.Add(this.btnInstall);
            this.groupBox1.Controls.Add(this.chkTimeAttendanceService);
            this.groupBox1.Location = new System.Drawing.Point(11, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(378, 552);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Programs Required ";
            // 
            // chkClock
            // 
            this.chkClock.AutoSize = true;
            this.chkClock.Location = new System.Drawing.Point(15, 97);
            this.chkClock.Name = "chkClock";
            this.chkClock.Size = new System.Drawing.Size(241, 17);
            this.chkClock.TabIndex = 265;
            this.chkClock.Text = "Install Validite Clock (Needs Validite Services)";
            this.chkClock.UseVisualStyleBackColor = true;
            this.chkClock.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkDatabase
            // 
            this.chkDatabase.AutoSize = true;
            this.chkDatabase.Location = new System.Drawing.Point(233, 32);
            this.chkDatabase.Name = "chkDatabase";
            this.chkDatabase.Size = new System.Drawing.Size(102, 17);
            this.chkDatabase.TabIndex = 264;
            this.chkDatabase.Text = "Install Database";
            this.chkDatabase.UseVisualStyleBackColor = true;
            this.chkDatabase.Visible = false;
            // 
            // chkTimeAttendance
            // 
            this.chkTimeAttendance.AutoSize = true;
            this.chkTimeAttendance.Location = new System.Drawing.Point(15, 64);
            this.chkTimeAttendance.Name = "chkTimeAttendance";
            this.chkTimeAttendance.Size = new System.Drawing.Size(240, 17);
            this.chkTimeAttendance.TabIndex = 262;
            this.chkTimeAttendance.Text = "Install Validite Client (Needs Validite Services)";
            this.chkTimeAttendance.UseVisualStyleBackColor = true;
            this.chkTimeAttendance.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkPayroll);
            this.groupBox2.Controls.Add(this.chkInternetTimeAttendance);
            this.groupBox2.Location = new System.Drawing.Point(15, 134);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(270, 92);
            this.groupBox2.TabIndex = 261;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Internet Applications";
            // 
            // chkPayroll
            // 
            this.chkPayroll.AutoSize = true;
            this.chkPayroll.Location = new System.Drawing.Point(14, 27);
            this.chkPayroll.Name = "chkPayroll";
            this.chkPayroll.Size = new System.Drawing.Size(124, 17);
            this.chkPayroll.TabIndex = 1;
            this.chkPayroll.Text = "Install Validite Payroll";
            this.chkPayroll.UseVisualStyleBackColor = true;
            this.chkPayroll.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkInternetTimeAttendance
            // 
            this.chkInternetTimeAttendance.AutoSize = true;
            this.chkInternetTimeAttendance.Location = new System.Drawing.Point(14, 60);
            this.chkInternetTimeAttendance.Name = "chkInternetTimeAttendance";
            this.chkInternetTimeAttendance.Size = new System.Drawing.Size(116, 17);
            this.chkInternetTimeAttendance.TabIndex = 2;
            this.chkInternetTimeAttendance.Text = "Install Validite Time";
            this.chkInternetTimeAttendance.UseVisualStyleBackColor = true;
            this.chkInternetTimeAttendance.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // txtInfo
            // 
            this.txtInfo.Enabled = false;
            this.txtInfo.Location = new System.Drawing.Point(15, 250);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInfo.Size = new System.Drawing.Size(348, 281);
            this.txtInfo.TabIndex = 260;
            // 
            // btnInstall
            // 
            this.btnInstall.Enabled = false;
            this.btnInstall.Location = new System.Drawing.Point(298, 139);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(65, 24);
            this.btnInstall.TabIndex = 259;
            this.btnInstall.Text = "Install";
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(396, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 24);
            this.btnCancel.TabIndex = 258;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.ForeColor = System.Drawing.Color.Red;
            this.lblInfo.Location = new System.Drawing.Point(12, 577);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(453, 57);
            this.lblInfo.TabIndex = 259;
            this.lblInfo.Text = resources.GetString("lblInfo.Text");
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 641);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmSetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Setup of Programs";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkTimeAttendanceService;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.CheckBox chkInternetTimeAttendance;
        private System.Windows.Forms.CheckBox chkPayroll;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkTimeAttendance;
        private System.Windows.Forms.CheckBox chkDatabase;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox chkClock;
    }
}

