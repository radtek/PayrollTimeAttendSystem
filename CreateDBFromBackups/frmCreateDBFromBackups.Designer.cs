namespace CreateDBFromBackups
{
    partial class frmCreateDBFromBackups
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            this.picCompanyLock = new System.Windows.Forms.PictureBox();
            this.lblFilesHeader = new System.Windows.Forms.Label();
            this.dgvFilesDataGridView = new System.Windows.Forms.DataGridView();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbnRemote = new System.Windows.Forms.RadioButton();
            this.rbnLocal = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.picCompanyLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilesDataGridView)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // picCompanyLock
            // 
            this.picCompanyLock.BackColor = System.Drawing.SystemColors.Control;
            this.picCompanyLock.Location = new System.Drawing.Point(11, 29);
            this.picCompanyLock.Name = "picCompanyLock";
            this.picCompanyLock.Size = new System.Drawing.Size(16, 16);
            this.picCompanyLock.TabIndex = 295;
            this.picCompanyLock.TabStop = false;
            this.picCompanyLock.Visible = false;
            // 
            // lblFilesHeader
            // 
            this.lblFilesHeader.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblFilesHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFilesHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilesHeader.ForeColor = System.Drawing.Color.Black;
            this.lblFilesHeader.Location = new System.Drawing.Point(8, 8);
            this.lblFilesHeader.Name = "lblFilesHeader";
            this.lblFilesHeader.Size = new System.Drawing.Size(889, 20);
            this.lblFilesHeader.TabIndex = 294;
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
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFilesDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
            this.dgvFilesDataGridView.ColumnHeadersHeight = 20;
            this.dgvFilesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvFilesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column3,
            this.Column1,
            this.Column4,
            this.Column5,
            this.Column2,
            this.Column6});
            this.dgvFilesDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvFilesDataGridView.EnableHeadersVisualStyles = false;
            this.dgvFilesDataGridView.Location = new System.Drawing.Point(8, 26);
            this.dgvFilesDataGridView.MultiSelect = false;
            this.dgvFilesDataGridView.Name = "dgvFilesDataGridView";
            this.dgvFilesDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFilesDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.dgvFilesDataGridView.RowHeadersWidth = 20;
            this.dgvFilesDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvFilesDataGridView.RowTemplate.Height = 19;
            this.dgvFilesDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvFilesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFilesDataGridView.ShowCellToolTips = false;
            this.dgvFilesDataGridView.ShowEditingIcon = false;
            this.dgvFilesDataGridView.ShowRowErrors = false;
            this.dgvFilesDataGridView.Size = new System.Drawing.Size(889, 550);
            this.dgvFilesDataGridView.TabIndex = 296;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Backup Date";
            this.Column3.Name = "Column3";
            this.Column3.Width = 120;
            // 
            // Column1
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle14;
            this.Column1.HeaderText = "Company";
            this.Column1.Name = "Column1";
            this.Column1.Width = 60;
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
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(916, 37);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 298;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(916, 7);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 24);
            this.btnOK.TabIndex = 297;
            this.btnOK.Text = "Restore";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbnRemote);
            this.groupBox3.Controls.Add(this.rbnLocal);
            this.groupBox3.Location = new System.Drawing.Point(909, 78);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(79, 78);
            this.groupBox3.TabIndex = 370;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Option";
            // 
            // rbnRemote
            // 
            this.rbnRemote.Location = new System.Drawing.Point(8, 47);
            this.rbnRemote.Name = "rbnRemote";
            this.rbnRemote.Size = new System.Drawing.Size(63, 20);
            this.rbnRemote.TabIndex = 1;
            this.rbnRemote.Text = "Remote";
            this.rbnRemote.Click += new System.EventHandler(this.rbnButton_Click);
            // 
            // rbnLocal
            // 
            this.rbnLocal.Checked = true;
            this.rbnLocal.Location = new System.Drawing.Point(8, 21);
            this.rbnLocal.Name = "rbnLocal";
            this.rbnLocal.Size = new System.Drawing.Size(58, 20);
            this.rbnLocal.TabIndex = 0;
            this.rbnLocal.TabStop = true;
            this.rbnLocal.Text = "Local";
            this.rbnLocal.Click += new System.EventHandler(this.rbnButton_Click);
            // 
            // frmCreateDBFromBackups
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(996, 588);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.picCompanyLock);
            this.Controls.Add(this.lblFilesHeader);
            this.Controls.Add(this.dgvFilesDataGridView);
            this.Name = "frmCreateDBFromBackups";
            this.Text = "Create DB / Restore DB";
            this.Load += new System.EventHandler(this.frmCreateDBFromBackups_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picCompanyLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilesDataGridView)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picCompanyLock;
        private System.Windows.Forms.Label lblFilesHeader;
        private System.Windows.Forms.DataGridView dgvFilesDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbnRemote;
        private System.Windows.Forms.RadioButton rbnLocal;
    }
}

