namespace InteractPayroll
{
    partial class frmRestoreDatabase
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRestoreDatabase));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblFilesHeader = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.grbActivationProcess = new System.Windows.Forms.GroupBox();
            this.pnlRestoreDatabase = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.picRestoreDatabase = new System.Windows.Forms.PictureBox();
            this.pnlDatabaseBackupBefore = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.picBackupBefore = new System.Windows.Forms.PictureBox();
            this.tmrTimer = new System.Windows.Forms.Timer(this.components);
            this.dgvFilesDataGridView = new System.Windows.Forms.DataGridView();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grbDynamicTimeSheet = new System.Windows.Forms.GroupBox();
            this.chkCopy = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.picCompanyLock = new System.Windows.Forms.PictureBox();
            this.grbActivationProcess.SuspendLayout();
            this.pnlRestoreDatabase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRestoreDatabase)).BeginInit();
            this.pnlDatabaseBackupBefore.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackupBefore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilesDataGridView)).BeginInit();
            this.grbDynamicTimeSheet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCompanyLock)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFilesHeader
            // 
            this.lblFilesHeader.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblFilesHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFilesHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilesHeader.ForeColor = System.Drawing.Color.Black;
            this.lblFilesHeader.Location = new System.Drawing.Point(8, 67);
            this.lblFilesHeader.Name = "lblFilesHeader";
            this.lblFilesHeader.Size = new System.Drawing.Size(889, 20);
            this.lblFilesHeader.TabIndex = 245;
            this.lblFilesHeader.Text = "Files Available to Restore";
            this.lblFilesHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(825, 36);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 248;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(825, 6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 24);
            this.btnOK.TabIndex = 247;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grbActivationProcess
            // 
            this.grbActivationProcess.Controls.Add(this.pnlRestoreDatabase);
            this.grbActivationProcess.Controls.Add(this.pnlDatabaseBackupBefore);
            this.grbActivationProcess.Location = new System.Drawing.Point(8, 5);
            this.grbActivationProcess.Name = "grbActivationProcess";
            this.grbActivationProcess.Size = new System.Drawing.Size(438, 55);
            this.grbActivationProcess.TabIndex = 292;
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
            this.pnlDatabaseBackupBefore.Controls.Add(this.label4);
            this.pnlDatabaseBackupBefore.Controls.Add(this.pictureBox3);
            this.pnlDatabaseBackupBefore.Controls.Add(this.picBackupBefore);
            this.pnlDatabaseBackupBefore.Location = new System.Drawing.Point(4, 14);
            this.pnlDatabaseBackupBefore.Name = "pnlDatabaseBackupBefore";
            this.pnlDatabaseBackupBefore.Size = new System.Drawing.Size(210, 36);
            this.pnlDatabaseBackupBefore.TabIndex = 290;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(63, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 27);
            this.label4.TabIndex = 284;
            this.label4.Text = "Database Backup (Before Restore)";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::InteractPayroll.Properties.Resources.DatabaseBackup;
            this.pictureBox3.Location = new System.Drawing.Point(25, 2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(32, 32);
            this.pictureBox3.TabIndex = 283;
            this.pictureBox3.TabStop = false;
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
            // tmrTimer
            // 
            this.tmrTimer.Interval = 1000;
            this.tmrTimer.Tick += new System.EventHandler(this.tmrTimer_Tick);
            // 
            // dgvFilesDataGridView
            // 
            this.dgvFilesDataGridView.AllowUserToAddRows = false;
            this.dgvFilesDataGridView.AllowUserToDeleteRows = false;
            this.dgvFilesDataGridView.AllowUserToResizeColumns = false;
            this.dgvFilesDataGridView.AllowUserToResizeRows = false;
            this.dgvFilesDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvFilesDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFilesDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFilesDataGridView.ColumnHeadersHeight = 20;
            this.dgvFilesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvFilesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column3,
            this.Column1,
            this.dataGridViewTextBoxColumn4,
            this.Column4,
            this.Column5,
            this.Column2,
            this.Column6});
            this.dgvFilesDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvFilesDataGridView.EnableHeadersVisualStyles = false;
            this.dgvFilesDataGridView.Location = new System.Drawing.Point(8, 85);
            this.dgvFilesDataGridView.MultiSelect = false;
            this.dgvFilesDataGridView.Name = "dgvFilesDataGridView";
            this.dgvFilesDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFilesDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFilesDataGridView.RowHeadersWidth = 20;
            this.dgvFilesDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvFilesDataGridView.RowTemplate.Height = 19;
            this.dgvFilesDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvFilesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFilesDataGridView.ShowCellToolTips = false;
            this.dgvFilesDataGridView.ShowEditingIcon = false;
            this.dgvFilesDataGridView.ShowRowErrors = false;
            this.dgvFilesDataGridView.Size = new System.Drawing.Size(889, 402);
            this.dgvFilesDataGridView.TabIndex = 293;
            this.dgvFilesDataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgvFilesDataGridView_SortCompare);
            this.dgvFilesDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Backup Date";
            this.Column3.Name = "Column3";
            this.Column3.Width = 120;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "#";
            this.Column1.Name = "Column1";
            this.Column1.Width = 20;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Payroll Date";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // Column4
            // 
            this.Column4.HeaderText = "File";
            this.Column4.Name = "Column4";
            this.Column4.Width = 610;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "tYPE1";
            this.Column5.Name = "Column5";
            this.Column5.Visible = false;
            this.Column5.Width = 20;
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
            // grbDynamicTimeSheet
            // 
            this.grbDynamicTimeSheet.Controls.Add(this.chkCopy);
            this.grbDynamicTimeSheet.Controls.Add(this.label2);
            this.grbDynamicTimeSheet.Controls.Add(this.pictureBox4);
            this.grbDynamicTimeSheet.Controls.Add(this.pictureBox1);
            this.grbDynamicTimeSheet.Location = new System.Drawing.Point(452, 5);
            this.grbDynamicTimeSheet.Name = "grbDynamicTimeSheet";
            this.grbDynamicTimeSheet.Size = new System.Drawing.Size(289, 55);
            this.grbDynamicTimeSheet.TabIndex = 294;
            this.grbDynamicTimeSheet.TabStop = false;
            this.grbDynamicTimeSheet.Text = "Dynamic Upload of Timesheets";
            this.grbDynamicTimeSheet.Visible = false;
            // 
            // chkCopy
            // 
            this.chkCopy.Location = new System.Drawing.Point(196, 9);
            this.chkCopy.Name = "chkCopy";
            this.chkCopy.Size = new System.Drawing.Size(83, 43);
            this.chkCopy.TabIndex = 287;
            this.chkCopy.Text = "Copy Timesheets Over";
            this.chkCopy.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(46, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 30);
            this.label2.TabIndex = 286;
            this.label2.Text = "Company has Dynamic Upload";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::InteractPayroll.Properties.Resources.warning;
            this.pictureBox4.Location = new System.Drawing.Point(151, 16);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(32, 32);
            this.pictureBox4.TabIndex = 285;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::InteractPayroll.Properties.Resources.timesheet32;
            this.pictureBox1.Location = new System.Drawing.Point(8, 16);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 284;
            this.pictureBox1.TabStop = false;
            // 
            // btnDelete
            // 
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(747, 6);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(72, 24);
            this.btnDelete.TabIndex = 295;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // picCompanyLock
            // 
            this.picCompanyLock.BackColor = System.Drawing.SystemColors.Control;
            this.picCompanyLock.Image = global::InteractPayroll.Properties.Resources.Lock16;
            this.picCompanyLock.Location = new System.Drawing.Point(11, 88);
            this.picCompanyLock.Name = "picCompanyLock";
            this.picCompanyLock.Size = new System.Drawing.Size(16, 16);
            this.picCompanyLock.TabIndex = 246;
            this.picCompanyLock.TabStop = false;
            this.picCompanyLock.Visible = false;
            // 
            // frmRestoreDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(906, 496);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.grbDynamicTimeSheet);
            this.Controls.Add(this.grbActivationProcess);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.picCompanyLock);
            this.Controls.Add(this.lblFilesHeader);
            this.Controls.Add(this.dgvFilesDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmRestoreDatabase";
            this.Text = "frmRestoreDatabase";
            this.Load += new System.EventHandler(this.frmRestoreDatabase_Load);
            this.grbActivationProcess.ResumeLayout(false);
            this.pnlRestoreDatabase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRestoreDatabase)).EndInit();
            this.pnlDatabaseBackupBefore.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackupBefore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilesDataGridView)).EndInit();
            this.grbDynamicTimeSheet.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCompanyLock)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picCompanyLock;
        private System.Windows.Forms.Label lblFilesHeader;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox grbActivationProcess;
        private System.Windows.Forms.Panel pnlRestoreDatabase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox picRestoreDatabase;
        private System.Windows.Forms.Panel pnlDatabaseBackupBefore;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox picBackupBefore;
        private System.Windows.Forms.Timer tmrTimer;
        private System.Windows.Forms.DataGridView dgvFilesDataGridView;
        private System.Windows.Forms.GroupBox grbDynamicTimeSheet;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.CheckBox chkCopy;
    }
}