namespace InteractPayrollClient
{
    partial class frmSetupInternetWebServerIP
    {
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetupInternetWebServerIP));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtIP1 = new System.Windows.Forms.TextBox();
            this.txtIP2 = new System.Windows.Forms.TextBox();
            this.txtIP3 = new System.Windows.Forms.TextBox();
            this.txtIP4 = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pictureBox2);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.btnTest);
            this.groupBox3.Controls.Add(this.txtIP1);
            this.groupBox3.Controls.Add(this.txtIP2);
            this.groupBox3.Controls.Add(this.txtIP3);
            this.groupBox3.Controls.Add(this.txtIP4);
            this.groupBox3.Location = new System.Drawing.Point(8, 8);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(315, 125);
            this.groupBox3.TabIndex = 198;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Internet Web Server IP Address";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(11, 20);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(48, 48);
            this.pictureBox2.TabIndex = 201;
            this.pictureBox2.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Web Server IP";
            // 
            // btnTest
            // 
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Location = new System.Drawing.Point(166, 88);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(64, 24);
            this.btnTest.TabIndex = 183;
            this.btnTest.Text = "Test";
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtIP1
            // 
            this.txtIP1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIP1.Enabled = false;
            this.txtIP1.Location = new System.Drawing.Point(12, 91);
            this.txtIP1.MaxLength = 3;
            this.txtIP1.Name = "txtIP1";
            this.txtIP1.Size = new System.Drawing.Size(29, 20);
            this.txtIP1.TabIndex = 21;
            this.txtIP1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // txtIP2
            // 
            this.txtIP2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIP2.Enabled = false;
            this.txtIP2.Location = new System.Drawing.Point(48, 91);
            this.txtIP2.MaxLength = 3;
            this.txtIP2.Name = "txtIP2";
            this.txtIP2.Size = new System.Drawing.Size(29, 20);
            this.txtIP2.TabIndex = 22;
            this.txtIP2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // txtIP3
            // 
            this.txtIP3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIP3.Enabled = false;
            this.txtIP3.Location = new System.Drawing.Point(83, 91);
            this.txtIP3.MaxLength = 3;
            this.txtIP3.Name = "txtIP3";
            this.txtIP3.Size = new System.Drawing.Size(29, 20);
            this.txtIP3.TabIndex = 23;
            this.txtIP3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // txtIP4
            // 
            this.txtIP4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIP4.Enabled = false;
            this.txtIP4.Location = new System.Drawing.Point(119, 91);
            this.txtIP4.MaxLength = 3;
            this.txtIP4.Name = "txtIP4";
            this.txtIP4.Size = new System.Drawing.Size(29, 20);
            this.txtIP4.TabIndex = 24;
            this.txtIP4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(329, 45);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(64, 24);
            this.btnSave.TabIndex = 196;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(329, 109);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 24);
            this.btnClose.TabIndex = 199;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnClose.MouseEnter += new System.EventHandler(this.btnClose_MouseEnter);
            this.btnClose.MouseLeave += new System.EventHandler(this.btnClose_MouseLeave);
            // 
            // btnUpdate
            // 
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Location = new System.Drawing.Point(329, 13);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(64, 24);
            this.btnUpdate.TabIndex = 197;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(329, 77);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(64, 24);
            this.btnCancel.TabIndex = 200;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmSetupInternetWebServerIP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(403, 143);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnCancel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "frmSetupInternetWebServerIP";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSetupInternetWebServerIP_FormClosing);
            this.Load += new System.EventHandler(this.frmSetupInternetWebServerIP_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.TextBox txtIP1;
        private System.Windows.Forms.TextBox txtIP2;
        private System.Windows.Forms.TextBox txtIP3;
        private System.Windows.Forms.TextBox txtIP4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}

