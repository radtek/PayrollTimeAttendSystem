namespace InteractPayrollClient
{
    partial class frmConnectionSetup
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbnLocalHost = new System.Windows.Forms.RadioButton();
            this.rbnServer = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPortNumber = new System.Windows.Forms.TextBox();
            this.txtIP1 = new System.Windows.Forms.TextBox();
            this.txtIP2 = new System.Windows.Forms.TextBox();
            this.txtIP3 = new System.Windows.Forms.TextBox();
            this.txtIP4 = new System.Windows.Forms.TextBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnHeaderClose = new System.Windows.Forms.Button();
            this.lblHeader = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbnLocalHost);
            this.groupBox3.Controls.Add(this.rbnServer);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.btnTest);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtPortNumber);
            this.groupBox3.Controls.Add(this.txtIP1);
            this.groupBox3.Controls.Add(this.txtIP2);
            this.groupBox3.Controls.Add(this.txtIP3);
            this.groupBox3.Controls.Add(this.txtIP4);
            this.groupBox3.Location = new System.Drawing.Point(8, 40);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(316, 125);
            this.groupBox3.TabIndex = 188;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Local Time Attendance / Clock Web Server Settings";
            // 
            // rbnLocalHost
            // 
            this.rbnLocalHost.AutoSize = true;
            this.rbnLocalHost.Enabled = false;
            this.rbnLocalHost.Location = new System.Drawing.Point(175, 34);
            this.rbnLocalHost.Name = "rbnLocalHost";
            this.rbnLocalHost.Size = new System.Drawing.Size(108, 17);
            this.rbnLocalHost.TabIndex = 2;
            this.rbnLocalHost.TabStop = true;
            this.rbnLocalHost.Text = "Local (LocalHost)";
            this.rbnLocalHost.UseVisualStyleBackColor = true;
            this.rbnLocalHost.Click += new System.EventHandler(this.rbnLocalHost_Click);
            // 
            // rbnServer
            // 
            this.rbnServer.AutoSize = true;
            this.rbnServer.Enabled = false;
            this.rbnServer.Location = new System.Drawing.Point(175, 59);
            this.rbnServer.Name = "rbnServer";
            this.rbnServer.Size = new System.Drawing.Size(82, 17);
            this.rbnServer.TabIndex = 1;
            this.rbnServer.TabStop = true;
            this.rbnServer.Text = "Web Server";
            this.rbnServer.UseVisualStyleBackColor = true;
            this.rbnServer.Click += new System.EventHandler(this.rbnServer_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Web Server IP";
            // 
            // btnTest
            // 
            this.btnTest.Enabled = false;
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Location = new System.Drawing.Point(240, 88);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(64, 24);
            this.btnTest.TabIndex = 183;
            this.btnTest.Text = "Test";
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Port Number";
            // 
            // txtPortNumber
            // 
            this.txtPortNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPortNumber.Enabled = false;
            this.txtPortNumber.Location = new System.Drawing.Point(12, 91);
            this.txtPortNumber.MaxLength = 4;
            this.txtPortNumber.Name = "txtPortNumber";
            this.txtPortNumber.Size = new System.Drawing.Size(62, 20);
            this.txtPortNumber.TabIndex = 20;
            this.txtPortNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // txtIP1
            // 
            this.txtIP1.Enabled = false;
            this.txtIP1.Location = new System.Drawing.Point(12, 37);
            this.txtIP1.MaxLength = 3;
            this.txtIP1.Name = "txtIP1";
            this.txtIP1.Size = new System.Drawing.Size(29, 20);
            this.txtIP1.TabIndex = 21;
            this.txtIP1.Text = " ";
            this.txtIP1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // txtIP2
            // 
            this.txtIP2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIP2.Enabled = false;
            this.txtIP2.Location = new System.Drawing.Point(48, 37);
            this.txtIP2.MaxLength = 3;
            this.txtIP2.Name = "txtIP2";
            this.txtIP2.Size = new System.Drawing.Size(29, 20);
            this.txtIP2.TabIndex = 22;
            this.txtIP2.Text = " ";
            this.txtIP2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // txtIP3
            // 
            this.txtIP3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIP3.Enabled = false;
            this.txtIP3.Location = new System.Drawing.Point(83, 37);
            this.txtIP3.MaxLength = 3;
            this.txtIP3.Name = "txtIP3";
            this.txtIP3.Size = new System.Drawing.Size(29, 20);
            this.txtIP3.TabIndex = 23;
            this.txtIP3.Text = " ";
            this.txtIP3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // txtIP4
            // 
            this.txtIP4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIP4.Enabled = false;
            this.txtIP4.Location = new System.Drawing.Point(119, 37);
            this.txtIP4.MaxLength = 3;
            this.txtIP4.Name = "txtIP4";
            this.txtIP4.Size = new System.Drawing.Size(29, 20);
            this.txtIP4.TabIndex = 24;
            this.txtIP4.Text = " ";
            this.txtIP4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // btnUpdate
            // 
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Location = new System.Drawing.Point(331, 45);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(64, 24);
            this.btnUpdate.TabIndex = 186;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(331, 77);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(64, 24);
            this.btnSave.TabIndex = 185;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(331, 141);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 24);
            this.btnClose.TabIndex = 189;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(331, 109);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(64, 24);
            this.btnCancel.TabIndex = 190;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnHeaderClose
            // 
            this.btnHeaderClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHeaderClose.BackColor = System.Drawing.Color.Silver;
            this.btnHeaderClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnHeaderClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeaderClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHeaderClose.Location = new System.Drawing.Point(368, 0);
            this.btnHeaderClose.Name = "btnHeaderClose";
            this.btnHeaderClose.Size = new System.Drawing.Size(32, 32);
            this.btnHeaderClose.TabIndex = 367;
            this.btnHeaderClose.TabStop = false;
            this.btnHeaderClose.Text = "X";
            this.btnHeaderClose.UseVisualStyleBackColor = false;
            this.btnHeaderClose.Click += new System.EventHandler(this.btnHeaderClose_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.BackColor = System.Drawing.Color.DimGray;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.Black;
            this.lblHeader.Location = new System.Drawing.Point(1, 1);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(370, 30);
            this.lblHeader.TabIndex = 366;
            this.lblHeader.Text = "Connection Setup";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmConnectionSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 178);
            this.Controls.Add(this.btnHeaderClose);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox3);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConnectionSetup";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client Connection Setup";
            this.Load += new System.EventHandler(this.frmConnectionSetup_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbnLocalHost;
        private System.Windows.Forms.RadioButton rbnServer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPortNumber;
        private System.Windows.Forms.TextBox txtIP1;
        private System.Windows.Forms.TextBox txtIP2;
        private System.Windows.Forms.TextBox txtIP3;
        private System.Windows.Forms.TextBox txtIP4;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnHeaderClose;
        public System.Windows.Forms.Label lblHeader;
    }
}