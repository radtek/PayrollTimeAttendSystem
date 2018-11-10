namespace InteractPayroll
{
    partial class frmEmployeeActivate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEmployeeActivate));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtDate = new System.Windows.Forms.TextBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblPayrollType = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboRunDate = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grbActivationProcess = new System.Windows.Forms.GroupBox();
            this.pnlDatabaseBackupAfter = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.picBackupAfter = new System.Windows.Forms.PictureBox();
            this.pnlDatabaseBackupBefore = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.picBackupBefore = new System.Windows.Forms.PictureBox();
            this.pnlEmployeeActivation = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.picEmployeeActivation = new System.Windows.Forms.PictureBox();
            this.tmrTimer = new System.Windows.Forms.Timer();
            this.dgvPayrollTypeDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblChosenEmployee = new System.Windows.Forms.Label();
            this.lblEmployee = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dgvEmployeeDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvChosenEmployeeDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.picPayrollTypeLock = new System.Windows.Forms.PictureBox();
            this.grbSchema = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.grbActivationProcess.SuspendLayout();
            this.pnlDatabaseBackupAfter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackupAfter)).BeginInit();
            this.pnlDatabaseBackupBefore.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackupBefore)).BeginInit();
            this.pnlEmployeeActivation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEmployeeActivation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChosenEmployeeDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPayrollTypeLock)).BeginInit();
            this.grbSchema.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(984, 99);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 92;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtDate
            // 
            this.txtDate.Enabled = false;
            this.txtDate.Location = new System.Drawing.Point(109, 16);
            this.txtDate.MaxLength = 10;
            this.txtDate.Name = "txtDate";
            this.txtDate.Size = new System.Drawing.Size(64, 20);
            this.txtDate.TabIndex = 83;
            this.txtDate.TextChanged += new System.EventHandler(this.txtDate_TextChanged);
            // 
            // lblDate
            // 
            this.lblDate.Location = new System.Drawing.Point(6, 19);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(97, 18);
            this.lblDate.TabIndex = 82;
            this.lblDate.Text = "Tax Effective Date";
            // 
            // lblPayrollType
            // 
            this.lblPayrollType.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblPayrollType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPayrollType.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPayrollType.ForeColor = System.Drawing.Color.Black;
            this.lblPayrollType.Location = new System.Drawing.Point(8, 8);
            this.lblPayrollType.Name = "lblPayrollType";
            this.lblPayrollType.Size = new System.Drawing.Size(160, 20);
            this.lblPayrollType.TabIndex = 280;
            this.lblPayrollType.Text = "Type";
            this.lblPayrollType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboRunDate);
            this.groupBox1.Controls.Add(this.lblDate);
            this.groupBox1.Controls.Add(this.txtDate);
            this.groupBox1.Location = new System.Drawing.Point(572, 76);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(235, 48);
            this.groupBox1.TabIndex = 282;
            this.groupBox1.TabStop = false;
            // 
            // cboRunDate
            // 
            this.cboRunDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRunDate.Enabled = false;
            this.cboRunDate.Location = new System.Drawing.Point(109, 16);
            this.cboRunDate.Name = "cboRunDate";
            this.cboRunDate.Size = new System.Drawing.Size(110, 21);
            this.cboRunDate.TabIndex = 84;
            this.cboRunDate.Visible = false;
            this.cboRunDate.SelectedIndexChanged += new System.EventHandler(this.cboRunDate_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(63, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 30);
            this.label1.TabIndex = 284;
            this.label1.Text = "Database Backup (Before Activation)";
            // 
            // grbActivationProcess
            // 
            this.grbActivationProcess.Controls.Add(this.pnlDatabaseBackupAfter);
            this.grbActivationProcess.Controls.Add(this.pnlDatabaseBackupBefore);
            this.grbActivationProcess.Controls.Add(this.pnlEmployeeActivation);
            this.grbActivationProcess.Location = new System.Drawing.Point(299, 2);
            this.grbActivationProcess.Name = "grbActivationProcess";
            this.grbActivationProcess.Size = new System.Drawing.Size(183, 126);
            this.grbActivationProcess.TabIndex = 289;
            this.grbActivationProcess.TabStop = false;
            this.grbActivationProcess.Text = "Activation Process";
            this.grbActivationProcess.Visible = false;
            // 
            // pnlDatabaseBackupAfter
            // 
            this.pnlDatabaseBackupAfter.Controls.Add(this.label3);
            this.pnlDatabaseBackupAfter.Controls.Add(this.pictureBox4);
            this.pnlDatabaseBackupAfter.Controls.Add(this.picBackupAfter);
            this.pnlDatabaseBackupAfter.Location = new System.Drawing.Point(6, 88);
            this.pnlDatabaseBackupAfter.Name = "pnlDatabaseBackupAfter";
            this.pnlDatabaseBackupAfter.Size = new System.Drawing.Size(163, 36);
            this.pnlDatabaseBackupAfter.TabIndex = 292;
            this.pnlDatabaseBackupAfter.Visible = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(63, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 30);
            this.label3.TabIndex = 284;
            this.label3.Text = "Database Backup (After Activation)";
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(25, 2);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(32, 32);
            this.pictureBox4.TabIndex = 283;
            this.pictureBox4.TabStop = false;
            // 
            // picBackupAfter
            // 
            this.picBackupAfter.Image = ((System.Drawing.Image)(resources.GetObject("picBackupAfter.Image")));
            this.picBackupAfter.Location = new System.Drawing.Point(4, 10);
            this.picBackupAfter.Name = "picBackupAfter";
            this.picBackupAfter.Size = new System.Drawing.Size(16, 16);
            this.picBackupAfter.TabIndex = 290;
            this.picBackupAfter.TabStop = false;
            // 
            // pnlDatabaseBackupBefore
            // 
            this.pnlDatabaseBackupBefore.Controls.Add(this.label1);
            this.pnlDatabaseBackupBefore.Controls.Add(this.pictureBox1);
            this.pnlDatabaseBackupBefore.Controls.Add(this.picBackupBefore);
            this.pnlDatabaseBackupBefore.Location = new System.Drawing.Point(6, 14);
            this.pnlDatabaseBackupBefore.Name = "pnlDatabaseBackupBefore";
            this.pnlDatabaseBackupBefore.Size = new System.Drawing.Size(161, 36);
            this.pnlDatabaseBackupBefore.TabIndex = 290;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(25, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 283;
            this.pictureBox1.TabStop = false;
            // 
            // picBackupBefore
            // 
            this.picBackupBefore.Image = global::EmployeeActivate.Properties.Resources.Question;
            this.picBackupBefore.Location = new System.Drawing.Point(4, 10);
            this.picBackupBefore.Name = "picBackupBefore";
            this.picBackupBefore.Size = new System.Drawing.Size(16, 16);
            this.picBackupBefore.TabIndex = 290;
            this.picBackupBefore.TabStop = false;
            // 
            // pnlEmployeeActivation
            // 
            this.pnlEmployeeActivation.Controls.Add(this.label2);
            this.pnlEmployeeActivation.Controls.Add(this.pictureBox2);
            this.pnlEmployeeActivation.Controls.Add(this.picEmployeeActivation);
            this.pnlEmployeeActivation.Location = new System.Drawing.Point(6, 51);
            this.pnlEmployeeActivation.Name = "pnlEmployeeActivation";
            this.pnlEmployeeActivation.Size = new System.Drawing.Size(168, 36);
            this.pnlEmployeeActivation.TabIndex = 291;
            this.pnlEmployeeActivation.Visible = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(63, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 18);
            this.label2.TabIndex = 284;
            this.label2.Text = "Employee Activation";
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
            // picEmployeeActivation
            // 
            this.picEmployeeActivation.Image = global::EmployeeActivate.Properties.Resources.Question;
            this.picEmployeeActivation.Location = new System.Drawing.Point(4, 10);
            this.picEmployeeActivation.Name = "picEmployeeActivation";
            this.picEmployeeActivation.Size = new System.Drawing.Size(16, 16);
            this.picEmployeeActivation.TabIndex = 290;
            this.picEmployeeActivation.TabStop = false;
            // 
            // tmrTimer
            // 
            this.tmrTimer.Interval = 1000;
            this.tmrTimer.Tick += new System.EventHandler(this.tmrTimer_Tick);
            // 
            // dgvPayrollTypeDataGridView
            // 
            this.dgvPayrollTypeDataGridView.AllowUserToAddRows = false;
            this.dgvPayrollTypeDataGridView.AllowUserToDeleteRows = false;
            this.dgvPayrollTypeDataGridView.AllowUserToResizeColumns = false;
            this.dgvPayrollTypeDataGridView.AllowUserToResizeRows = false;
            this.dgvPayrollTypeDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvPayrollTypeDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPayrollTypeDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPayrollTypeDataGridView.ColumnHeadersHeight = 20;
            this.dgvPayrollTypeDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvPayrollTypeDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn3});
            this.dgvPayrollTypeDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvPayrollTypeDataGridView.EnableHeadersVisualStyles = false;
            this.dgvPayrollTypeDataGridView.Location = new System.Drawing.Point(8, 26);
            this.dgvPayrollTypeDataGridView.MultiSelect = false;
            this.dgvPayrollTypeDataGridView.Name = "dgvPayrollTypeDataGridView";
            this.dgvPayrollTypeDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPayrollTypeDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPayrollTypeDataGridView.RowHeadersWidth = 20;
            this.dgvPayrollTypeDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvPayrollTypeDataGridView.RowTemplate.Height = 19;
            this.dgvPayrollTypeDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvPayrollTypeDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPayrollTypeDataGridView.ShowCellToolTips = false;
            this.dgvPayrollTypeDataGridView.ShowEditingIcon = false;
            this.dgvPayrollTypeDataGridView.ShowRowErrors = false;
            this.dgvPayrollTypeDataGridView.Size = new System.Drawing.Size(160, 60);
            this.dgvPayrollTypeDataGridView.TabIndex = 342;
            this.dgvPayrollTypeDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPayrollTypeDataGridView_RowEnter);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Description";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.dataGridViewTextBoxColumn1.Width = 140;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Visible = false;
            // 
            // lblChosenEmployee
            // 
            this.lblChosenEmployee.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblChosenEmployee.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblChosenEmployee.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChosenEmployee.ForeColor = System.Drawing.Color.Black;
            this.lblChosenEmployee.Location = new System.Drawing.Point(572, 131);
            this.lblChosenEmployee.Name = "lblChosenEmployee";
            this.lblChosenEmployee.Size = new System.Drawing.Size(484, 20);
            this.lblChosenEmployee.TabIndex = 344;
            this.lblChosenEmployee.Text = "Selected Employees";
            this.lblChosenEmployee.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEmployee
            // 
            this.lblEmployee.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblEmployee.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEmployee.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmployee.ForeColor = System.Drawing.Color.Black;
            this.lblEmployee.Location = new System.Drawing.Point(8, 131);
            this.lblEmployee.Name = "lblEmployee";
            this.lblEmployee.Size = new System.Drawing.Size(484, 20);
            this.lblEmployee.TabIndex = 343;
            this.lblEmployee.Text = "List of Employees";
            this.lblEmployee.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(496, 304);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(72, 24);
            this.btnRemove.TabIndex = 348;
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Enabled = false;
            this.btnAddAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddAll.Location = new System.Drawing.Point(496, 272);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(72, 24);
            this.btnAddAll.TabIndex = 345;
            this.btnAddAll.Text = "Add All";
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Enabled = false;
            this.btnRemoveAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveAll.Location = new System.Drawing.Point(496, 336);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(72, 24);
            this.btnRemoveAll.TabIndex = 346;
            this.btnRemoveAll.Text = "Remove All";
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(496, 240);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(72, 24);
            this.btnAdd.TabIndex = 347;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // dgvEmployeeDataGridView
            // 
            this.dgvEmployeeDataGridView.AllowUserToAddRows = false;
            this.dgvEmployeeDataGridView.AllowUserToDeleteRows = false;
            this.dgvEmployeeDataGridView.AllowUserToResizeColumns = false;
            this.dgvEmployeeDataGridView.AllowUserToResizeRows = false;
            this.dgvEmployeeDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvEmployeeDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployeeDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvEmployeeDataGridView.ColumnHeadersHeight = 20;
            this.dgvEmployeeDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvEmployeeDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column7,
            this.Column4});
            this.dgvEmployeeDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvEmployeeDataGridView.EnableHeadersVisualStyles = false;
            this.dgvEmployeeDataGridView.Location = new System.Drawing.Point(8, 149);
            this.dgvEmployeeDataGridView.MultiSelect = false;
            this.dgvEmployeeDataGridView.Name = "dgvEmployeeDataGridView";
            this.dgvEmployeeDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployeeDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvEmployeeDataGridView.RowHeadersWidth = 20;
            this.dgvEmployeeDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvEmployeeDataGridView.RowTemplate.Height = 19;
            this.dgvEmployeeDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvEmployeeDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployeeDataGridView.ShowCellToolTips = false;
            this.dgvEmployeeDataGridView.ShowEditingIcon = false;
            this.dgvEmployeeDataGridView.ShowRowErrors = false;
            this.dgvEmployeeDataGridView.Size = new System.Drawing.Size(484, 326);
            this.dgvEmployeeDataGridView.TabIndex = 349;
            this.dgvEmployeeDataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgvEmployeeDataGridView_SortCompare);
            this.dgvEmployeeDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            this.dgvEmployeeDataGridView.DoubleClick += new System.EventHandler(this.dgvEmployeeDataGridView_DoubleClick);
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Code";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 65;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Surname";
            this.Column1.Name = "Column1";
            this.Column1.Width = 155;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Name";
            this.Column2.Name = "Column2";
            this.Column2.Width = 140;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Take-On Date";
            this.Column3.Name = "Column3";
            this.Column3.Width = 85;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "Column7";
            this.Column7.Name = "Column7";
            this.Column7.Visible = false;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Column4";
            this.Column4.Name = "Column4";
            this.Column4.Visible = false;
            // 
            // dgvChosenEmployeeDataGridView
            // 
            this.dgvChosenEmployeeDataGridView.AllowUserToAddRows = false;
            this.dgvChosenEmployeeDataGridView.AllowUserToDeleteRows = false;
            this.dgvChosenEmployeeDataGridView.AllowUserToResizeColumns = false;
            this.dgvChosenEmployeeDataGridView.AllowUserToResizeRows = false;
            this.dgvChosenEmployeeDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvChosenEmployeeDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvChosenEmployeeDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvChosenEmployeeDataGridView.ColumnHeadersHeight = 20;
            this.dgvChosenEmployeeDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvChosenEmployeeDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.Column6,
            this.Column5,
            this.Column8});
            this.dgvChosenEmployeeDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvChosenEmployeeDataGridView.EnableHeadersVisualStyles = false;
            this.dgvChosenEmployeeDataGridView.Location = new System.Drawing.Point(572, 149);
            this.dgvChosenEmployeeDataGridView.MultiSelect = false;
            this.dgvChosenEmployeeDataGridView.Name = "dgvChosenEmployeeDataGridView";
            this.dgvChosenEmployeeDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvChosenEmployeeDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvChosenEmployeeDataGridView.RowHeadersWidth = 20;
            this.dgvChosenEmployeeDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvChosenEmployeeDataGridView.RowTemplate.Height = 19;
            this.dgvChosenEmployeeDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvChosenEmployeeDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChosenEmployeeDataGridView.ShowCellToolTips = false;
            this.dgvChosenEmployeeDataGridView.ShowEditingIcon = false;
            this.dgvChosenEmployeeDataGridView.ShowRowErrors = false;
            this.dgvChosenEmployeeDataGridView.Size = new System.Drawing.Size(484, 326);
            this.dgvChosenEmployeeDataGridView.TabIndex = 350;
            this.dgvChosenEmployeeDataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgvChosenEmployeeDataGridView_SortCompare);
            this.dgvChosenEmployeeDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            this.dgvChosenEmployeeDataGridView.DoubleClick += new System.EventHandler(this.dgvChosenEmployeeDataGridView_DoubleClick);
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewCellStyle6.Format = "######0.00";
            dataGridViewCellStyle6.NullValue = null;
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewTextBoxColumn5.HeaderText = "Code";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 65;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Surname";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 155;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "Name";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.Width = 140;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Take-On Date";
            this.Column6.Name = "Column6";
            this.Column6.Width = 85;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Column5";
            this.Column5.Name = "Column5";
            this.Column5.Visible = false;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "Column8";
            this.Column8.Name = "Column8";
            this.Column8.Visible = false;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Location = new System.Drawing.Point(984, 9);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(72, 24);
            this.btnUpdate.TabIndex = 354;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(984, 69);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 24);
            this.btnCancel.TabIndex = 352;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(984, 39);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 24);
            this.btnSave.TabIndex = 351;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // picPayrollTypeLock
            // 
            this.picPayrollTypeLock.BackColor = System.Drawing.SystemColors.Control;
            this.picPayrollTypeLock.Image = global::EmployeeActivate.Properties.Resources.NewLock16;
            this.picPayrollTypeLock.Location = new System.Drawing.Point(11, 29);
            this.picPayrollTypeLock.Name = "picPayrollTypeLock";
            this.picPayrollTypeLock.Size = new System.Drawing.Size(16, 16);
            this.picPayrollTypeLock.TabIndex = 355;
            this.picPayrollTypeLock.TabStop = false;
            this.picPayrollTypeLock.Visible = false;
            // 
            // grbSchema
            // 
            this.grbSchema.Controls.Add(this.label5);
            this.grbSchema.Controls.Add(this.label4);
            this.grbSchema.Controls.Add(this.label6);
            this.grbSchema.Location = new System.Drawing.Point(572, 3);
            this.grbSchema.Name = "grbSchema";
            this.grbSchema.Size = new System.Drawing.Size(321, 73);
            this.grbSchema.TabIndex = 356;
            this.grbSchema.TabStop = false;
            this.grbSchema.Paint += new System.Windows.Forms.PaintEventHandler(this.grbSchema_Paint);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(203, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 18);
            this.label5.TabIndex = 79;
            this.label5.Text = "First Payroll Run";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Navy;
            this.label4.Location = new System.Drawing.Point(6, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 20);
            this.label4.TabIndex = 78;
            this.label4.Text = "Tax Effective Date";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(80, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(154, 19);
            this.label6.TabIndex = 80;
            this.label6.Text = "Leave /  Earnings / Deductions";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmEmployeeActivate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1063, 484);
            this.Controls.Add(this.grbSchema);
            this.Controls.Add(this.picPayrollTypeLock);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblChosenEmployee);
            this.Controls.Add(this.lblEmployee);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAddAll);
            this.Controls.Add(this.btnRemoveAll);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dgvEmployeeDataGridView);
            this.Controls.Add(this.dgvChosenEmployeeDataGridView);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblPayrollType);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.grbActivationProcess);
            this.Controls.Add(this.dgvPayrollTypeDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmEmployeeActivate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmEmployeeActivate_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grbActivationProcess.ResumeLayout(false);
            this.pnlDatabaseBackupAfter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackupAfter)).EndInit();
            this.pnlDatabaseBackupBefore.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackupBefore)).EndInit();
            this.pnlEmployeeActivation.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEmployeeActivation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChosenEmployeeDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPayrollTypeLock)).EndInit();
            this.grbSchema.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtDate;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblPayrollType;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grbActivationProcess;
        private System.Windows.Forms.Timer tmrTimer;
        private System.Windows.Forms.PictureBox picBackupBefore;
        private System.Windows.Forms.Panel pnlDatabaseBackupBefore;
        private System.Windows.Forms.Panel pnlEmployeeActivation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox picEmployeeActivation;
        private System.Windows.Forms.Panel pnlDatabaseBackupAfter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox picBackupAfter;
        private System.Windows.Forms.DataGridView dgvPayrollTypeDataGridView;
        private System.Windows.Forms.Label lblChosenEmployee;
        private System.Windows.Forms.Label lblEmployee;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridView dgvEmployeeDataGridView;
        private System.Windows.Forms.DataGridView dgvChosenEmployeeDataGridView;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cboRunDate;
        private System.Windows.Forms.PictureBox picPayrollTypeLock;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.GroupBox grbSchema;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
    }
}

