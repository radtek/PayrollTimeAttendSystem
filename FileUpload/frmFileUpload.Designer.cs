namespace InteractPayroll
{
    partial class frmFileUpload
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnFile = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.prgProgressBar = new System.Windows.Forms.ProgressBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip();
            this.lblUserSpreadsheetHeader = new System.Windows.Forms.Label();
            this.dgvUserDataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClose = new System.Windows.Forms.Button();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblSelectedUserSpreadsheetHeader = new System.Windows.Forms.Label();
            this.dgvSelectedUserDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.grbOption = new System.Windows.Forms.GroupBox();
            this.rbnBackupFile = new System.Windows.Forms.RadioButton();
            this.rbnFile = new System.Windows.Forms.RadioButton();
            this.lblFilesHeader = new System.Windows.Forms.Label();
            this.dgvFilesDataGridView = new System.Windows.Forms.DataGridView();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSelectedUserDataGridView)).BeginInit();
            this.grbOption.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // btnUpload
            // 
            this.btnUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpload.Location = new System.Drawing.Point(1137, 506);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(72, 24);
            this.btnUpload.TabIndex = 4;
            this.btnUpload.Text = "Upload";
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnFile
            // 
            this.btnFile.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFile.Location = new System.Drawing.Point(1093, 538);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(33, 24);
            this.btnFile.TabIndex = 287;
            this.btnFile.Text = "...";
            this.toolTip1.SetToolTip(this.btnFile, "Select File to Upload");
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFilePath.Location = new System.Drawing.Point(53, 540);
            this.txtFilePath.MaxLength = 8;
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(1035, 20);
            this.txtFilePath.TabIndex = 286;
            this.txtFilePath.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 543);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 285;
            this.label1.Text = "File Path";
            // 
            // prgProgressBar
            // 
            this.prgProgressBar.Location = new System.Drawing.Point(53, 507);
            this.prgProgressBar.Name = "prgProgressBar";
            this.prgProgressBar.Size = new System.Drawing.Size(1035, 23);
            this.prgProgressBar.TabIndex = 288;
            this.prgProgressBar.Visible = false;
            // 
            // lblUserSpreadsheetHeader
            // 
            this.lblUserSpreadsheetHeader.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblUserSpreadsheetHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblUserSpreadsheetHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserSpreadsheetHeader.ForeColor = System.Drawing.Color.Black;
            this.lblUserSpreadsheetHeader.Location = new System.Drawing.Point(8, 8);
            this.lblUserSpreadsheetHeader.Name = "lblUserSpreadsheetHeader";
            this.lblUserSpreadsheetHeader.Size = new System.Drawing.Size(559, 20);
            this.lblUserSpreadsheetHeader.TabIndex = 355;
            this.lblUserSpreadsheetHeader.Text = "List of Users";
            this.lblUserSpreadsheetHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvUserDataGridView
            // 
            this.dgvUserDataGridView.AllowUserToAddRows = false;
            this.dgvUserDataGridView.AllowUserToDeleteRows = false;
            this.dgvUserDataGridView.AllowUserToResizeColumns = false;
            this.dgvUserDataGridView.AllowUserToResizeRows = false;
            this.dgvUserDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvUserDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvUserDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvUserDataGridView.ColumnHeadersHeight = 20;
            this.dgvUserDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvUserDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.dataGridViewTextBoxColumn4,
            this.Column2,
            this.dataGridViewTextBoxColumn5});
            this.dgvUserDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvUserDataGridView.EnableHeadersVisualStyles = false;
            this.dgvUserDataGridView.Location = new System.Drawing.Point(8, 26);
            this.dgvUserDataGridView.MultiSelect = false;
            this.dgvUserDataGridView.Name = "dgvUserDataGridView";
            this.dgvUserDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvUserDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvUserDataGridView.RowHeadersWidth = 20;
            this.dgvUserDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvUserDataGridView.RowTemplate.Height = 19;
            this.dgvUserDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvUserDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUserDataGridView.ShowCellToolTips = false;
            this.dgvUserDataGridView.ShowEditingIcon = false;
            this.dgvUserDataGridView.ShowRowErrors = false;
            this.dgvUserDataGridView.Size = new System.Drawing.Size(559, 250);
            this.dgvUserDataGridView.TabIndex = 356;
            this.dgvUserDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUserDataGridView_RowEnter);
            this.dgvUserDataGridView.DoubleClick += new System.EventHandler(this.dgvUserDataGridView_DoubleClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "User Id.";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 120;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Surname";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 200;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Name";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 200;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Visible = false;
            this.dataGridViewTextBoxColumn5.Width = 180;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(1137, 538);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 388;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "User Id.";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 120;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Surname";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 200;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Name";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 200;
            // 
            // lblSelectedUserSpreadsheetHeader
            // 
            this.lblSelectedUserSpreadsheetHeader.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblSelectedUserSpreadsheetHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSelectedUserSpreadsheetHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedUserSpreadsheetHeader.ForeColor = System.Drawing.Color.Black;
            this.lblSelectedUserSpreadsheetHeader.Location = new System.Drawing.Point(651, 8);
            this.lblSelectedUserSpreadsheetHeader.Name = "lblSelectedUserSpreadsheetHeader";
            this.lblSelectedUserSpreadsheetHeader.Size = new System.Drawing.Size(559, 20);
            this.lblSelectedUserSpreadsheetHeader.TabIndex = 393;
            this.lblSelectedUserSpreadsheetHeader.Text = "Upload File to Users";
            this.lblSelectedUserSpreadsheetHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvSelectedUserDataGridView
            // 
            this.dgvSelectedUserDataGridView.AllowUserToAddRows = false;
            this.dgvSelectedUserDataGridView.AllowUserToDeleteRows = false;
            this.dgvSelectedUserDataGridView.AllowUserToResizeColumns = false;
            this.dgvSelectedUserDataGridView.AllowUserToResizeRows = false;
            this.dgvSelectedUserDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvSelectedUserDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSelectedUserDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSelectedUserDataGridView.ColumnHeadersHeight = 20;
            this.dgvSelectedUserDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvSelectedUserDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn9});
            this.dgvSelectedUserDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvSelectedUserDataGridView.EnableHeadersVisualStyles = false;
            this.dgvSelectedUserDataGridView.Location = new System.Drawing.Point(651, 26);
            this.dgvSelectedUserDataGridView.MultiSelect = false;
            this.dgvSelectedUserDataGridView.Name = "dgvSelectedUserDataGridView";
            this.dgvSelectedUserDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSelectedUserDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSelectedUserDataGridView.RowHeadersWidth = 20;
            this.dgvSelectedUserDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvSelectedUserDataGridView.RowTemplate.Height = 19;
            this.dgvSelectedUserDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvSelectedUserDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSelectedUserDataGridView.ShowCellToolTips = false;
            this.dgvSelectedUserDataGridView.ShowEditingIcon = false;
            this.dgvSelectedUserDataGridView.ShowRowErrors = false;
            this.dgvSelectedUserDataGridView.Size = new System.Drawing.Size(559, 212);
            this.dgvSelectedUserDataGridView.TabIndex = 394;
            this.dgvSelectedUserDataGridView.DoubleClick += new System.EventHandler(this.dgvSelectedUserDataGridView_DoubleClick);
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "User Id.";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 120;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "Surname";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.Width = 200;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "Name";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            this.dataGridViewTextBoxColumn8.Width = 200;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.Visible = false;
            this.dataGridViewTextBoxColumn9.Width = 180;
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveAll.Location = new System.Drawing.Point(573, 174);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(72, 24);
            this.btnRemoveAll.TabIndex = 397;
            this.btnRemoveAll.Text = "Remove All";
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(573, 81);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(72, 24);
            this.btnAdd.TabIndex = 398;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddAll.Location = new System.Drawing.Point(573, 112);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(72, 24);
            this.btnAddAll.TabIndex = 396;
            this.btnAddAll.Text = "Add All";
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(573, 143);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(72, 24);
            this.btnRemove.TabIndex = 399;
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // grbOption
            // 
            this.grbOption.Controls.Add(this.rbnBackupFile);
            this.grbOption.Controls.Add(this.rbnFile);
            this.grbOption.Location = new System.Drawing.Point(1067, 433);
            this.grbOption.Name = "grbOption";
            this.grbOption.Size = new System.Drawing.Size(142, 66);
            this.grbOption.TabIndex = 400;
            this.grbOption.TabStop = false;
            this.grbOption.Text = "Option";
            // 
            // rbnBackupFile
            // 
            this.rbnBackupFile.AutoSize = true;
            this.rbnBackupFile.Location = new System.Drawing.Point(6, 40);
            this.rbnBackupFile.Name = "rbnBackupFile";
            this.rbnBackupFile.Size = new System.Drawing.Size(130, 17);
            this.rbnBackupFile.TabIndex = 3;
            this.rbnBackupFile.Text = "Database Backup File";
            this.rbnBackupFile.UseVisualStyleBackColor = true;
            this.rbnBackupFile.Click += new System.EventHandler(this.rbnFileOption_Click);
            // 
            // rbnFile
            // 
            this.rbnFile.AutoSize = true;
            this.rbnFile.Checked = true;
            this.rbnFile.Location = new System.Drawing.Point(6, 19);
            this.rbnFile.Name = "rbnFile";
            this.rbnFile.Size = new System.Drawing.Size(41, 17);
            this.rbnFile.TabIndex = 2;
            this.rbnFile.TabStop = true;
            this.rbnFile.Text = "File";
            this.rbnFile.UseVisualStyleBackColor = true;
            this.rbnFile.Click += new System.EventHandler(this.rbnFileOption_Click);
            // 
            // lblFilesHeader
            // 
            this.lblFilesHeader.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblFilesHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFilesHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilesHeader.ForeColor = System.Drawing.Color.Black;
            this.lblFilesHeader.Location = new System.Drawing.Point(8, 287);
            this.lblFilesHeader.Name = "lblFilesHeader";
            this.lblFilesHeader.Size = new System.Drawing.Size(1051, 20);
            this.lblFilesHeader.TabIndex = 401;
            this.lblFilesHeader.Text = "Database Backup Files Available to Upload";
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
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFilesDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvFilesDataGridView.ColumnHeadersHeight = 20;
            this.dgvFilesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvFilesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column3,
            this.Column4,
            this.dataGridViewTextBoxColumn10,
            this.Column6});
            this.dgvFilesDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvFilesDataGridView.EnableHeadersVisualStyles = false;
            this.dgvFilesDataGridView.Location = new System.Drawing.Point(8, 305);
            this.dgvFilesDataGridView.MultiSelect = false;
            this.dgvFilesDataGridView.Name = "dgvFilesDataGridView";
            this.dgvFilesDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFilesDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvFilesDataGridView.RowHeadersWidth = 20;
            this.dgvFilesDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvFilesDataGridView.RowTemplate.Height = 19;
            this.dgvFilesDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvFilesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFilesDataGridView.ShowCellToolTips = false;
            this.dgvFilesDataGridView.ShowEditingIcon = false;
            this.dgvFilesDataGridView.ShowRowErrors = false;
            this.dgvFilesDataGridView.Size = new System.Drawing.Size(1051, 193);
            this.dgvFilesDataGridView.TabIndex = 402;
            this.dgvFilesDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFilesDataGridView_RowEnter);
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
            this.Column4.Width = 892;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.HeaderText = "Column2";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.Visible = false;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Column6";
            this.Column6.Name = "Column6";
            this.Column6.Visible = false;
            // 
            // frmFileUpload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1218, 571);
            this.Controls.Add(this.lblFilesHeader);
            this.Controls.Add(this.dgvFilesDataGridView);
            this.Controls.Add(this.grbOption);
            this.Controls.Add(this.btnRemoveAll);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnAddAll);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.lblSelectedUserSpreadsheetHeader);
            this.Controls.Add(this.dgvSelectedUserDataGridView);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblUserSpreadsheetHeader);
            this.Controls.Add(this.dgvUserDataGridView);
            this.Controls.Add(this.prgProgressBar);
            this.Controls.Add(this.btnFile);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnUpload);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmFileUpload";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmFileUpload_FormClosing);
            this.Load += new System.EventHandler(this.frmFileUpload_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSelectedUserDataGridView)).EndInit();
            this.grbOption.ResumeLayout(false);
            this.grbOption.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilesDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar prgProgressBar;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblUserSpreadsheetHeader;
        private System.Windows.Forms.DataGridView dgvUserDataGridView;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.Label lblSelectedUserSpreadsheetHeader;
        private System.Windows.Forms.DataGridView dgvSelectedUserDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.GroupBox grbOption;
        private System.Windows.Forms.RadioButton rbnBackupFile;
        private System.Windows.Forms.RadioButton rbnFile;
        private System.Windows.Forms.Label lblFilesHeader;
        private System.Windows.Forms.DataGridView dgvFilesDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
    }
}

