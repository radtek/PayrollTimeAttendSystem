namespace InteractPayrollClient
{
    partial class frmFingerPrintReader
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.cboFingerPrintReader = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtRemoteBackupSiteName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(324, 38);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(76, 24);
            this.btnSave.TabIndex = 229;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(324, 68);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 24);
            this.btnCancel.TabIndex = 228;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnUpdate.Location = new System.Drawing.Point(324, 8);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(76, 24);
            this.btnUpdate.TabIndex = 227;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(324, 98);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(76, 24);
            this.btnClose.TabIndex = 226;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // cboFingerPrintReader
            // 
            this.cboFingerPrintReader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFingerPrintReader.Enabled = false;
            this.cboFingerPrintReader.FormattingEnabled = true;
            this.cboFingerPrintReader.Items.AddRange(new object[] {
            "URU4500 (Digital Persona)",
            "Curve (Integrated Biometrics)"});
            this.cboFingerPrintReader.Location = new System.Drawing.Point(8, 28);
            this.cboFingerPrintReader.Name = "cboFingerPrintReader";
            this.cboFingerPrintReader.Size = new System.Drawing.Size(294, 21);
            this.cboFingerPrintReader.TabIndex = 230;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboFingerPrintReader);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(311, 65);
            this.groupBox1.TabIndex = 231;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Choose Fingerprint Reader to be used in this program";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtRemoteBackupSiteName);
            this.groupBox2.Location = new System.Drawing.Point(8, 82);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(311, 65);
            this.groupBox2.TabIndex = 232;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Remote Backup Site Name  (No Spaces)";
            // 
            // txtRemoteBackupSiteName
            // 
            this.txtRemoteBackupSiteName.Enabled = false;
            this.txtRemoteBackupSiteName.Location = new System.Drawing.Point(8, 28);
            this.txtRemoteBackupSiteName.MaxLength = 30;
            this.txtRemoteBackupSiteName.Name = "txtRemoteBackupSiteName";
            this.txtRemoteBackupSiteName.Size = new System.Drawing.Size(294, 20);
            this.txtRemoteBackupSiteName.TabIndex = 0;
            this.txtRemoteBackupSiteName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRemoteBackupSiteName_KeyPress);
            // 
            // frmFingerPrintReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(406, 156);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmFingerPrintReader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmFingerPrintReader_FormClosing);
            this.Load += new System.EventHandler(this.frmFingerPrintReader_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ComboBox cboFingerPrintReader;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtRemoteBackupSiteName;
    }
}

