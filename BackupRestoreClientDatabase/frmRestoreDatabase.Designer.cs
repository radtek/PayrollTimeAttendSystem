namespace InteractPayrollClient
{
    partial class frmRestoreDatabase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRestoreDatabase));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnDelete = new System.Windows.Forms.Button();
            this.grbActivationProcess = new System.Windows.Forms.GroupBox();
            this.pnlRestoreDatabase = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.picRestoreDatabase = new System.Windows.Forms.PictureBox();
            this.pnlDatabaseBackupBefore = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.picBackupBefore = new System.Windows.Forms.PictureBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.picCompanyLock = new System.Windows.Forms.PictureBox();
            this.lblFilesHeader = new System.Windows.Forms.Label();
            this.dgvFilesDataGridView = new System.Windows.Forms.DataGridView();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tmrTimer = new System.Windows.Forms.Timer(this.components);
            this.grbActivationProcess.SuspendLayout();
            this.pnlRestoreDatabase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRestoreDatabase)).BeginInit();
            this.pnlDatabaseBackupBefore.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackupBefore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCompanyLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(1042, 8);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(72, 24);
            this.btnDelete.TabIndex = 302;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // grbActivationProcess
            // 
            this.grbActivationProcess.Controls.Add(this.pnlRestoreDatabase);
            this.grbActivationProcess.Controls.Add(this.pnlDatabaseBackupBefore);
            this.grbActivationProcess.Location = new System.Drawing.Point(12, 7);
            this.grbActivationProcess.Name = "grbActivationProcess";
            this.grbActivationProcess.Size = new System.Drawing.Size(438, 55);
            this.grbActivationProcess.TabIndex = 300;
            this.grbActivationProcess.TabStop = false;
            this.grbActivationProcess.Text = "Restore Database Process";
            this.grbActivationProcess.Visible = false;
            // 
            // pnlRestoreDatabase
            // 
            this.pnlRestoreDatabase.Controls.Add(this.label1);
            this.pnlRestoreDatabase.Controls.Add(this.pictureBox2);
            this.pnlRestoreDatabase.Controls.Add(this.picRestoreDatabase);
            this.pnlRestoreDatabase.Location = new System.Drawing.Point(220, 14);
            this.pnlRestoreDatabase.Name = "pnlRestoreDatabase";
            this.pnlRestoreDatabase.Size = new System.Drawing.Size(211, 36);
            this.pnlRestoreDatabase.TabIndex = 291;
            this.pnlRestoreDatabase.Visible = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(64, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 18);
            this.label1.TabIndex = 284;
            this.label1.Text = "Restore Database";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(25, 2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox2.TabIndex = 283;
            this.pictureBox2.TabStop = false;
            // 
            // picRestoreDatabase
            // 
            this.picRestoreDatabase.Location = new System.Drawing.Point(4, 10);
            this.picRestoreDatabase.Name = "picRestoreDatabase";
            this.picRestoreDatabase.Size = new System.Drawing.Size(16, 16);
            this.picRestoreDatabase.TabIndex = 290;
            this.picRestoreDatabase.TabStop = false;
            // 
            // pnlDatabaseBackupBefore
            // 
            this.pnlDatabaseBackupBefore.Controls.Add(this.pictureBox1);
            this.pnlDatabaseBackupBefore.Controls.Add(this.label4);
            this.pnlDatabaseBackupBefore.Controls.Add(this.picBackupBefore);
            this.pnlDatabaseBackupBefore.Location = new System.Drawing.Point(4, 14);
            this.pnlDatabaseBackupBefore.Name = "pnlDatabaseBackupBefore";
            this.pnlDatabaseBackupBefore.Size = new System.Drawing.Size(210, 36);
            this.pnlDatabaseBackupBefore.TabIndex = 290;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(27, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 291;
            this.pictureBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(63, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 27);
            this.label4.TabIndex = 284;
            this.label4.Text = "Database Backup (Before Restore)";
            // 
            // picBackupBefore
            // 
            this.picBackupBefore.Location = new System.Drawing.Point(4, 10);
            this.picBackupBefore.Name = "picBackupBefore";
            this.picBackupBefore.Size = new System.Drawing.Size(16, 16);
            this.picBackupBefore.TabIndex = 290;
            this.picBackupBefore.TabStop = false;
            this.picBackupBefore.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(1120, 38);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 299;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(1120, 8);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 24);
            this.btnOK.TabIndex = 298;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // picCompanyLock
            // 
            this.picCompanyLock.BackColor = System.Drawing.SystemColors.Control;
            this.picCompanyLock.Image = global::InteractPayrollClient.Properties.Resources.NewLock16;
            this.picCompanyLock.Location = new System.Drawing.Point(15, 90);
            this.picCompanyLock.Name = "picCompanyLock";
            this.picCompanyLock.Size = new System.Drawing.Size(16, 16);
            this.picCompanyLock.TabIndex = 297;
            this.picCompanyLock.TabStop = false;
            this.picCompanyLock.Visible = false;
            // 
            // lblFilesHeader
            // 
            this.lblFilesHeader.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblFilesHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFilesHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilesHeader.ForeColor = System.Drawing.Color.Black;
            this.lblFilesHeader.Location = new System.Drawing.Point(12, 69);
            this.lblFilesHeader.Name = "lblFilesHeader";
            this.lblFilesHeader.Size = new System.Drawing.Size(1179, 20);
            this.lblFilesHeader.TabIndex = 296;
            this.lblFilesHeader.Text = "Files Available to Restore";
            this.lblFilesHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvFilesDataGridView
            // 
            this.dgvFilesDataGridView.AllowUserToAddRows = false;
            this.dgvFilesDataGridView.AllowUserToDeleteRows = false;
            this.dgvFilesDataGridView.AllowUserToResizeColumns = false;
            this.dgvFilesDataGridView.AllowUserToResizeRows = false;
            this.dgvFilesDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvFilesDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFilesDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvFilesDataGridView.ColumnHeadersHeight = 20;
            this.dgvFilesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvFilesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column3,
            this.Column4,
            this.Column2,
            this.Column6});
            this.dgvFilesDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvFilesDataGridView.EnableHeadersVisualStyles = false;
            this.dgvFilesDataGridView.Location = new System.Drawing.Point(12, 87);
            this.dgvFilesDataGridView.MultiSelect = false;
            this.dgvFilesDataGridView.Name = "dgvFilesDataGridView";
            this.dgvFilesDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFilesDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvFilesDataGridView.RowHeadersWidth = 20;
            this.dgvFilesDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvFilesDataGridView.RowTemplate.Height = 19;
            this.dgvFilesDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvFilesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFilesDataGridView.ShowCellToolTips = false;
            this.dgvFilesDataGridView.ShowEditingIcon = false;
            this.dgvFilesDataGridView.ShowRowErrors = false;
            this.dgvFilesDataGridView.Size = new System.Drawing.Size(1179, 402);
            this.dgvFilesDataGridView.TabIndex = 301;
            this.dgvFilesDataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgvFilesDataGridView_SortCompare);
            this.dgvFilesDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Backup Date";
            this.Column3.Name = "Column3";
            this.Column3.Width = 120;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "File";
            this.Column4.Name = "Column4";
            this.Column4.Width = 1020;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.Visible = false;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Column6";
            this.Column6.Name = "Column6";
            this.Column6.Visible = false;
            // 
            // tmrTimer
            // 
            this.tmrTimer.Tick += new System.EventHandler(this.tmrTimer_Tick);
            // 
            // frmRestoreDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1196, 500);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.grbActivationProcess);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.picCompanyLock);
            this.Controls.Add(this.lblFilesHeader);
            this.Controls.Add(this.dgvFilesDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmRestoreDatabase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmRestoreDatabase";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmRestoreDatabase_FormClosing);
            this.Load += new System.EventHandler(this.frmRestoreDatabase_Load);
            this.grbActivationProcess.ResumeLayout(false);
            this.pnlRestoreDatabase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRestoreDatabase)).EndInit();
            this.pnlDatabaseBackupBefore.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackupBefore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCompanyLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilesDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.GroupBox grbActivationProcess;
        private System.Windows.Forms.Panel pnlRestoreDatabase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox picRestoreDatabase;
        private System.Windows.Forms.Panel pnlDatabaseBackupBefore;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox picBackupBefore;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.PictureBox picCompanyLock;
        private System.Windows.Forms.Label lblFilesHeader;
        private System.Windows.Forms.DataGridView dgvFilesDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.Timer tmrTimer;
    }
}