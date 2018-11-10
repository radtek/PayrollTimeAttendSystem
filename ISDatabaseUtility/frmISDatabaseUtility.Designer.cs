namespace CreateDatabase
{
    partial class frmISDatabaseUtility
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmISDatabaseUtility));
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.txtWorkDirectory = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDeleteDownloadFiles = new System.Windows.Forms.Button();
            this.cboServer = new System.Windows.Forms.ComboBox();
            this.lblDb = new System.Windows.Forms.Label();
            this.txtDatabaseExists = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSystemUser = new System.Windows.Forms.TextBox();
            this.txtDatabaseFileName = new System.Windows.Forms.TextBox();
            this.btnDrop = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInteractUser = new System.Windows.Forms.TextBox();
            this.btnSqlUserAction = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.cboAuthentication = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtWindowsLogin = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnWinUserAction = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server :";
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.SystemColors.Control;
            this.btnOK.Enabled = false;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.Color.Black;
            this.btnOK.Location = new System.Drawing.Point(709, 357);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(69, 24);
            this.btnOK.TabIndex = 3;
            this.btnOK.TabStop = false;
            this.btnOK.Text = "Create DB";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(296, 166);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(69, 24);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.TabStop = false;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.Control;
            this.btnDelete.Enabled = false;
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.Color.Black;
            this.btnDelete.Location = new System.Drawing.Point(212, 361);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(208, 24);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.TabStop = false;
            this.btnDelete.Text = "Clear Database (Factory Settings)";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // txtWorkDirectory
            // 
            this.txtWorkDirectory.Location = new System.Drawing.Point(94, 404);
            this.txtWorkDirectory.Name = "txtWorkDirectory";
            this.txtWorkDirectory.Size = new System.Drawing.Size(306, 20);
            this.txtWorkDirectory.TabIndex = 6;
            this.txtWorkDirectory.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 407);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Work Directory";
            // 
            // btnDeleteDownloadFiles
            // 
            this.btnDeleteDownloadFiles.BackColor = System.Drawing.SystemColors.Control;
            this.btnDeleteDownloadFiles.Enabled = false;
            this.btnDeleteDownloadFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteDownloadFiles.ForeColor = System.Drawing.Color.Black;
            this.btnDeleteDownloadFiles.Location = new System.Drawing.Point(15, 361);
            this.btnDeleteDownloadFiles.Name = "btnDeleteDownloadFiles";
            this.btnDeleteDownloadFiles.Size = new System.Drawing.Size(153, 24);
            this.btnDeleteDownloadFiles.TabIndex = 8;
            this.btnDeleteDownloadFiles.TabStop = false;
            this.btnDeleteDownloadFiles.Text = "Clear Downloaded Dll Files";
            this.btnDeleteDownloadFiles.UseVisualStyleBackColor = false;
            this.btnDeleteDownloadFiles.Click += new System.EventHandler(this.btnDeleteDownloadFiles_Click);
            // 
            // cboServer
            // 
            this.cboServer.FormattingEnabled = true;
            this.cboServer.Items.AddRange(new object[] {
            "(localdb)\\MSSQLLocalDB",
            ".\\SqlExpress"});
            this.cboServer.Location = new System.Drawing.Point(113, 21);
            this.cboServer.Name = "cboServer";
            this.cboServer.Size = new System.Drawing.Size(307, 21);
            this.cboServer.TabIndex = 10;
            // 
            // lblDb
            // 
            this.lblDb.AutoSize = true;
            this.lblDb.Location = new System.Drawing.Point(12, 274);
            this.lblDb.Name = "lblDb";
            this.lblDb.Size = new System.Drawing.Size(136, 13);
            this.lblDb.TabIndex = 11;
            this.lblDb.Text = "InteractPayrollClient Exists?";
            // 
            // txtDatabaseExists
            // 
            this.txtDatabaseExists.Enabled = false;
            this.txtDatabaseExists.Location = new System.Drawing.Point(197, 271);
            this.txtDatabaseExists.Name = "txtDatabaseExists";
            this.txtDatabaseExists.Size = new System.Drawing.Size(47, 20);
            this.txtDatabaseExists.TabIndex = 12;
            this.txtDatabaseExists.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(442, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(153, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Windows Authentication User :";
            // 
            // txtSystemUser
            // 
            this.txtSystemUser.Enabled = false;
            this.txtSystemUser.Location = new System.Drawing.Point(608, 21);
            this.txtSystemUser.Name = "txtSystemUser";
            this.txtSystemUser.ReadOnly = true;
            this.txtSystemUser.Size = new System.Drawing.Size(182, 20);
            this.txtSystemUser.TabIndex = 14;
            this.txtSystemUser.TabStop = false;
            // 
            // txtDatabaseFileName
            // 
            this.txtDatabaseFileName.Location = new System.Drawing.Point(15, 308);
            this.txtDatabaseFileName.Name = "txtDatabaseFileName";
            this.txtDatabaseFileName.Size = new System.Drawing.Size(688, 20);
            this.txtDatabaseFileName.TabIndex = 15;
            this.txtDatabaseFileName.TabStop = false;
            // 
            // btnDrop
            // 
            this.btnDrop.BackColor = System.Drawing.SystemColors.Control;
            this.btnDrop.Enabled = false;
            this.btnDrop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDrop.ForeColor = System.Drawing.Color.Black;
            this.btnDrop.Location = new System.Drawing.Point(709, 304);
            this.btnDrop.Name = "btnDrop";
            this.btnDrop.Size = new System.Drawing.Size(69, 24);
            this.btnDrop.TabIndex = 16;
            this.btnDrop.TabStop = false;
            this.btnDrop.Text = "Drop DB";
            this.btnDrop.UseVisualStyleBackColor = false;
            this.btnDrop.Click += new System.EventHandler(this.btnDrop_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(442, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Sql Authentication User :";
            // 
            // txtInteractUser
            // 
            this.txtInteractUser.BackColor = System.Drawing.SystemColors.Control;
            this.txtInteractUser.Enabled = false;
            this.txtInteractUser.Location = new System.Drawing.Point(608, 58);
            this.txtInteractUser.Name = "txtInteractUser";
            this.txtInteractUser.Size = new System.Drawing.Size(182, 20);
            this.txtInteractUser.TabIndex = 18;
            this.txtInteractUser.TabStop = false;
            // 
            // btnSqlUserAction
            // 
            this.btnSqlUserAction.BackColor = System.Drawing.SystemColors.Control;
            this.btnSqlUserAction.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSqlUserAction.Enabled = false;
            this.btnSqlUserAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSqlUserAction.ForeColor = System.Drawing.Color.Black;
            this.btnSqlUserAction.Location = new System.Drawing.Point(808, 55);
            this.btnSqlUserAction.Name = "btnSqlUserAction";
            this.btnSqlUserAction.Size = new System.Drawing.Size(69, 24);
            this.btnSqlUserAction.TabIndex = 19;
            this.btnSqlUserAction.TabStop = false;
            this.btnSqlUserAction.Text = "Create";
            this.btnSqlUserAction.UseVisualStyleBackColor = false;
            this.btnSqlUserAction.Click += new System.EventHandler(this.btnAction_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Password :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "User Name :";
            // 
            // txtPassword
            // 
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(113, 126);
            this.txtPassword.MaxLength = 30;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(307, 20);
            this.txtPassword.TabIndex = 23;
            // 
            // txtUserName
            // 
            this.txtUserName.Enabled = false;
            this.txtUserName.Location = new System.Drawing.Point(113, 91);
            this.txtUserName.MaxLength = 100;
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(307, 20);
            this.txtUserName.TabIndex = 22;
            // 
            // cboAuthentication
            // 
            this.cboAuthentication.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAuthentication.FormattingEnabled = true;
            this.cboAuthentication.Items.AddRange(new object[] {
            "Windows Authentication",
            "Sql Server Authentication"});
            this.cboAuthentication.Location = new System.Drawing.Point(113, 55);
            this.cboAuthentication.Name = "cboAuthentication";
            this.cboAuthentication.Size = new System.Drawing.Size(307, 21);
            this.cboAuthentication.TabIndex = 21;
            this.cboAuthentication.SelectedIndexChanged += new System.EventHandler(this.cboAuthentication_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Authentication :";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(169, 167);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 26;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtWindowsLogin
            // 
            this.txtWindowsLogin.Enabled = false;
            this.txtWindowsLogin.Location = new System.Drawing.Point(608, 170);
            this.txtWindowsLogin.Name = "txtWindowsLogin";
            this.txtWindowsLogin.ReadOnly = true;
            this.txtWindowsLogin.Size = new System.Drawing.Size(182, 20);
            this.txtWindowsLogin.TabIndex = 28;
            this.txtWindowsLogin.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(442, 173);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(111, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Windows Login User :";
            // 
            // btnWinUserAction
            // 
            this.btnWinUserAction.BackColor = System.Drawing.SystemColors.Control;
            this.btnWinUserAction.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnWinUserAction.Enabled = false;
            this.btnWinUserAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWinUserAction.ForeColor = System.Drawing.Color.Black;
            this.btnWinUserAction.Location = new System.Drawing.Point(808, 17);
            this.btnWinUserAction.Name = "btnWinUserAction";
            this.btnWinUserAction.Size = new System.Drawing.Size(69, 24);
            this.btnWinUserAction.TabIndex = 29;
            this.btnWinUserAction.TabStop = false;
            this.btnWinUserAction.Text = "Create";
            this.btnWinUserAction.UseVisualStyleBackColor = false;
            this.btnWinUserAction.Click += new System.EventHandler(this.btnWinUserAction_Click);
            // 
            // frmISDatabaseUtility
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(956, 438);
            this.Controls.Add(this.btnWinUserAction);
            this.Controls.Add(this.txtWindowsLogin);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.cboAuthentication);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnSqlUserAction);
            this.Controls.Add(this.txtInteractUser);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnDrop);
            this.Controls.Add(this.txtDatabaseFileName);
            this.Controls.Add(this.txtSystemUser);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtDatabaseExists);
            this.Controls.Add(this.lblDb);
            this.Controls.Add(this.cboServer);
            this.Controls.Add(this.btnDeleteDownloadFiles);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtWorkDirectory);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmISDatabaseUtility";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Local Time Attendance Database";
            this.Load += new System.EventHandler(this.frmISDatabaseUtility_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox txtWorkDirectory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDeleteDownloadFiles;
        private System.Windows.Forms.ComboBox cboServer;
        private System.Windows.Forms.Label lblDb;
        private System.Windows.Forms.TextBox txtDatabaseExists;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSystemUser;
        private System.Windows.Forms.TextBox txtDatabaseFileName;
        private System.Windows.Forms.Button btnDrop;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInteractUser;
        private System.Windows.Forms.Button btnSqlUserAction;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.ComboBox cboAuthentication;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtWindowsLogin;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnWinUserAction;
    }
}

