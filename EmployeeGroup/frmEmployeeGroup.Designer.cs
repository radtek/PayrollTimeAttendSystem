namespace InteractPayrollClient
{
    partial class frmEmployeeGroup
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEmployeeGroup));
            this.lblEmployee = new System.Windows.Forms.Label();
            this.lblSelectedEmployee = new System.Windows.Forms.Label();
            this.txtGroup = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.grbGroup = new System.Windows.Forms.GroupBox();
            this.dgvEmployeeLinkedDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvEmployeeDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblCompany = new System.Windows.Forms.Label();
            this.tmrTimer = new System.Windows.Forms.Timer(this.components);
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.lblGroup = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.dgvCompanyDataGridView = new System.Windows.Forms.DataGridView();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RecordIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvGroupDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbnNameSurname = new System.Windows.Forms.RadioButton();
            this.rbnSurnameName = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.picCompanyLock = new System.Windows.Forms.PictureBox();
            this.picGroupLock = new System.Windows.Forms.PictureBox();
            this.grbGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeLinkedDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCompanyDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroupDataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCompanyLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGroupLock)).BeginInit();
            this.SuspendLayout();
            // 
            // lblEmployee
            // 
            this.lblEmployee.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblEmployee.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEmployee.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmployee.ForeColor = System.Drawing.Color.Black;
            this.lblEmployee.Location = new System.Drawing.Point(10, 53);
            this.lblEmployee.Name = "lblEmployee";
            this.lblEmployee.Size = new System.Drawing.Size(529, 20);
            this.lblEmployee.TabIndex = 190;
            this.lblEmployee.Text = "List of Employees";
            this.lblEmployee.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSelectedEmployee
            // 
            this.lblSelectedEmployee.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblSelectedEmployee.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSelectedEmployee.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedEmployee.ForeColor = System.Drawing.Color.Black;
            this.lblSelectedEmployee.Location = new System.Drawing.Point(625, 53);
            this.lblSelectedEmployee.Name = "lblSelectedEmployee";
            this.lblSelectedEmployee.Size = new System.Drawing.Size(529, 20);
            this.lblSelectedEmployee.TabIndex = 193;
            this.lblSelectedEmployee.Text = "Linked Employees";
            this.lblSelectedEmployee.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtGroup
            // 
            this.txtGroup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGroup.Enabled = false;
            this.txtGroup.Location = new System.Drawing.Point(116, 21);
            this.txtGroup.MaxLength = 30;
            this.txtGroup.Name = "txtGroup";
            this.txtGroup.Size = new System.Drawing.Size(261, 20);
            this.txtGroup.TabIndex = 192;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(7, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 18);
            this.label5.TabIndex = 191;
            this.label5.Text = "Group Description";
            // 
            // btnAddAll
            // 
            this.btnAddAll.Enabled = false;
            this.btnAddAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddAll.Location = new System.Drawing.Point(545, 169);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(75, 24);
            this.btnAddAll.TabIndex = 94;
            this.btnAddAll.Text = "Add All";
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Enabled = false;
            this.btnRemoveAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveAll.Location = new System.Drawing.Point(545, 229);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(75, 24);
            this.btnRemoveAll.TabIndex = 93;
            this.btnRemoveAll.Text = "Remove All";
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(545, 199);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 24);
            this.btnRemove.TabIndex = 89;
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(545, 139);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 24);
            this.btnAdd.TabIndex = 88;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // grbGroup
            // 
            this.grbGroup.Controls.Add(this.lblSelectedEmployee);
            this.grbGroup.Controls.Add(this.dgvEmployeeLinkedDataGridView);
            this.grbGroup.Controls.Add(this.lblEmployee);
            this.grbGroup.Controls.Add(this.dgvEmployeeDataGridView);
            this.grbGroup.Controls.Add(this.txtGroup);
            this.grbGroup.Controls.Add(this.label5);
            this.grbGroup.Controls.Add(this.btnAddAll);
            this.grbGroup.Controls.Add(this.btnRemoveAll);
            this.grbGroup.Controls.Add(this.btnRemove);
            this.grbGroup.Controls.Add(this.btnAdd);
            this.grbGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbGroup.Location = new System.Drawing.Point(8, 256);
            this.grbGroup.Name = "grbGroup";
            this.grbGroup.Size = new System.Drawing.Size(1163, 312);
            this.grbGroup.TabIndex = 229;
            this.grbGroup.TabStop = false;
            this.grbGroup.Text = "Group";
            // 
            // dgvEmployeeLinkedDataGridView
            // 
            this.dgvEmployeeLinkedDataGridView.AllowUserToAddRows = false;
            this.dgvEmployeeLinkedDataGridView.AllowUserToDeleteRows = false;
            this.dgvEmployeeLinkedDataGridView.AllowUserToResizeColumns = false;
            this.dgvEmployeeLinkedDataGridView.AllowUserToResizeRows = false;
            this.dgvEmployeeLinkedDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvEmployeeLinkedDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployeeLinkedDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvEmployeeLinkedDataGridView.ColumnHeadersHeight = 20;
            this.dgvEmployeeLinkedDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvEmployeeLinkedDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn9,
            this.dataGridViewTextBoxColumn10});
            this.dgvEmployeeLinkedDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvEmployeeLinkedDataGridView.EnableHeadersVisualStyles = false;
            this.dgvEmployeeLinkedDataGridView.Location = new System.Drawing.Point(625, 71);
            this.dgvEmployeeLinkedDataGridView.MultiSelect = false;
            this.dgvEmployeeLinkedDataGridView.Name = "dgvEmployeeLinkedDataGridView";
            this.dgvEmployeeLinkedDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployeeLinkedDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvEmployeeLinkedDataGridView.RowHeadersWidth = 20;
            this.dgvEmployeeLinkedDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvEmployeeLinkedDataGridView.RowTemplate.Height = 19;
            this.dgvEmployeeLinkedDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvEmployeeLinkedDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployeeLinkedDataGridView.ShowCellToolTips = false;
            this.dgvEmployeeLinkedDataGridView.ShowEditingIcon = false;
            this.dgvEmployeeLinkedDataGridView.ShowRowErrors = false;
            this.dgvEmployeeLinkedDataGridView.Size = new System.Drawing.Size(529, 231);
            this.dgvEmployeeLinkedDataGridView.TabIndex = 289;
            this.dgvEmployeeLinkedDataGridView.Sorted += new System.EventHandler(this.DataGrid_Sorted);
            this.dgvEmployeeLinkedDataGridView.DoubleClick += new System.EventHandler(this.dgvEmployeeLinkedDataGridView_DoubleClick);
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Code";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 70;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Surname / Name";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.Width = 195;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "#";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.Width = 20;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "Cost Centre";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.Width = 205;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.HeaderText = "Column3";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.Visible = false;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.HeaderText = "Column4";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.Visible = false;
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
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.dgvEmployeeDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvEmployeeDataGridView.EnableHeadersVisualStyles = false;
            this.dgvEmployeeDataGridView.Location = new System.Drawing.Point(10, 71);
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
            this.dgvEmployeeDataGridView.Size = new System.Drawing.Size(529, 231);
            this.dgvEmployeeDataGridView.TabIndex = 288;
            this.dgvEmployeeDataGridView.Sorted += new System.EventHandler(this.DataGrid_Sorted);
            this.dgvEmployeeDataGridView.DoubleClick += new System.EventHandler(this.dgvEmployeeDataGridView_DoubleClick);
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Code";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 70;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Surname / Name";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 195;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "#";
            this.Column1.Name = "Column1";
            this.Column1.Width = 20;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Cost Centre";
            this.Column2.Name = "Column2";
            this.Column2.Width = 205;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Column3";
            this.Column3.Name = "Column3";
            this.Column3.Visible = false;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Column4";
            this.Column4.Name = "Column4";
            this.Column4.Visible = false;
            // 
            // lblCompany
            // 
            this.lblCompany.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblCompany.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCompany.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCompany.ForeColor = System.Drawing.Color.Black;
            this.lblCompany.Location = new System.Drawing.Point(8, 9);
            this.lblCompany.Name = "lblCompany";
            this.lblCompany.Size = new System.Drawing.Size(329, 20);
            this.lblCompany.TabIndex = 228;
            this.lblCompany.Text = "Company";
            this.lblCompany.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnDelete
            // 
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(1097, 72);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(76, 24);
            this.btnDelete.TabIndex = 225;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnNew
            // 
            this.btnNew.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNew.Enabled = false;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Location = new System.Drawing.Point(1097, 8);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(76, 24);
            this.btnNew.TabIndex = 224;
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // lblGroup
            // 
            this.lblGroup.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblGroup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGroup.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGroup.ForeColor = System.Drawing.Color.Black;
            this.lblGroup.Location = new System.Drawing.Point(8, 115);
            this.lblGroup.Name = "lblGroup";
            this.lblGroup.Size = new System.Drawing.Size(329, 20);
            this.lblGroup.TabIndex = 223;
            this.lblGroup.Text = "Group";
            this.lblGroup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(1097, 104);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(76, 24);
            this.btnSave.TabIndex = 221;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(1097, 136);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 24);
            this.btnCancel.TabIndex = 220;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Location = new System.Drawing.Point(1097, 40);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(76, 24);
            this.btnUpdate.TabIndex = 219;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(1097, 168);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(76, 24);
            this.btnClose.TabIndex = 218;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dgvCompanyDataGridView
            // 
            this.dgvCompanyDataGridView.AllowUserToAddRows = false;
            this.dgvCompanyDataGridView.AllowUserToDeleteRows = false;
            this.dgvCompanyDataGridView.AllowUserToResizeColumns = false;
            this.dgvCompanyDataGridView.AllowUserToResizeRows = false;
            this.dgvCompanyDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvCompanyDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCompanyDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvCompanyDataGridView.ColumnHeadersHeight = 20;
            this.dgvCompanyDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvCompanyDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Description,
            this.RecordIndex});
            this.dgvCompanyDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvCompanyDataGridView.EnableHeadersVisualStyles = false;
            this.dgvCompanyDataGridView.Location = new System.Drawing.Point(8, 27);
            this.dgvCompanyDataGridView.MultiSelect = false;
            this.dgvCompanyDataGridView.Name = "dgvCompanyDataGridView";
            this.dgvCompanyDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCompanyDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvCompanyDataGridView.RowHeadersWidth = 20;
            this.dgvCompanyDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvCompanyDataGridView.RowTemplate.Height = 19;
            this.dgvCompanyDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvCompanyDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCompanyDataGridView.ShowCellToolTips = false;
            this.dgvCompanyDataGridView.ShowEditingIcon = false;
            this.dgvCompanyDataGridView.ShowRowErrors = false;
            this.dgvCompanyDataGridView.Size = new System.Drawing.Size(329, 79);
            this.dgvCompanyDataGridView.TabIndex = 286;
            this.dgvCompanyDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCompanyDataGridView_RowEnter);
            this.dgvCompanyDataGridView.Sorted += new System.EventHandler(this.DataGrid_Sorted);
            // 
            // Description
            // 
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.Width = 290;
            // 
            // RecordIndex
            // 
            this.RecordIndex.HeaderText = "Column1";
            this.RecordIndex.Name = "RecordIndex";
            this.RecordIndex.Visible = false;
            // 
            // dgvGroupDataGridView
            // 
            this.dgvGroupDataGridView.AllowUserToAddRows = false;
            this.dgvGroupDataGridView.AllowUserToDeleteRows = false;
            this.dgvGroupDataGridView.AllowUserToResizeColumns = false;
            this.dgvGroupDataGridView.AllowUserToResizeRows = false;
            this.dgvGroupDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvGroupDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGroupDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvGroupDataGridView.ColumnHeadersHeight = 20;
            this.dgvGroupDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvGroupDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dgvGroupDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvGroupDataGridView.EnableHeadersVisualStyles = false;
            this.dgvGroupDataGridView.Location = new System.Drawing.Point(8, 133);
            this.dgvGroupDataGridView.MultiSelect = false;
            this.dgvGroupDataGridView.Name = "dgvGroupDataGridView";
            this.dgvGroupDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGroupDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvGroupDataGridView.RowHeadersWidth = 20;
            this.dgvGroupDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvGroupDataGridView.RowTemplate.Height = 19;
            this.dgvGroupDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvGroupDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGroupDataGridView.ShowCellToolTips = false;
            this.dgvGroupDataGridView.ShowEditingIcon = false;
            this.dgvGroupDataGridView.ShowRowErrors = false;
            this.dgvGroupDataGridView.Size = new System.Drawing.Size(329, 117);
            this.dgvGroupDataGridView.TabIndex = 287;
            this.dgvGroupDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGroupDataGridView_RowEnter);
            this.dgvGroupDataGridView.Sorted += new System.EventHandler(this.DataGrid_Sorted);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Description";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 290;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbnNameSurname);
            this.groupBox1.Controls.Add(this.rbnSurnameName);
            this.groupBox1.Location = new System.Drawing.Point(943, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(146, 104);
            this.groupBox1.TabIndex = 194;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Employee Names Order";
            // 
            // rbnNameSurname
            // 
            this.rbnNameSurname.AutoSize = true;
            this.rbnNameSurname.Location = new System.Drawing.Point(9, 64);
            this.rbnNameSurname.Name = "rbnNameSurname";
            this.rbnNameSurname.Size = new System.Drawing.Size(106, 17);
            this.rbnNameSurname.TabIndex = 1;
            this.rbnNameSurname.Text = "Name / Surname";
            this.rbnNameSurname.UseVisualStyleBackColor = true;
            this.rbnNameSurname.Click += new System.EventHandler(this.rbnSurnameOrNameOrder_Click);
            // 
            // rbnSurnameName
            // 
            this.rbnSurnameName.AutoSize = true;
            this.rbnSurnameName.Checked = true;
            this.rbnSurnameName.Location = new System.Drawing.Point(9, 30);
            this.rbnSurnameName.Name = "rbnSurnameName";
            this.rbnSurnameName.Size = new System.Drawing.Size(106, 17);
            this.rbnSurnameName.TabIndex = 0;
            this.rbnSurnameName.TabStop = true;
            this.rbnSurnameName.Text = "Surname / Name";
            this.rbnSurnameName.UseVisualStyleBackColor = true;
            this.rbnSurnameName.Click += new System.EventHandler(this.rbnSurnameOrNameOrder_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.pictureBox1);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(345, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(536, 104);
            this.groupBox2.TabIndex = 288;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Group Setup";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(76, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(453, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Make sure that Employee/s are not Linked to 2 different Cost Centres.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(76, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(453, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "This could result in undesired results.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::InteractPayrollClient.Properties.Resources.employees;
            this.pictureBox1.Location = new System.Drawing.Point(22, 35);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(76, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(400, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Setting up a Group must be approached with caution.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(345, 199);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(253, 52);
            this.groupBox3.TabIndex = 289;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "# Column - Possible Values";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(214, 13);
            this.label4.TabIndex = 192;
            this.label4.Text = "W=Wages, S=Salaries, T=Time Attendance";
            // 
            // picCompanyLock
            // 
            this.picCompanyLock.BackColor = System.Drawing.SystemColors.Control;
            this.picCompanyLock.Image = global::InteractPayrollClient.Properties.Resources.NewLock16;
            this.picCompanyLock.Location = new System.Drawing.Point(11, 30);
            this.picCompanyLock.Name = "picCompanyLock";
            this.picCompanyLock.Size = new System.Drawing.Size(16, 16);
            this.picCompanyLock.TabIndex = 230;
            this.picCompanyLock.TabStop = false;
            this.picCompanyLock.Visible = false;
            // 
            // picGroupLock
            // 
            this.picGroupLock.BackColor = System.Drawing.SystemColors.Control;
            this.picGroupLock.Image = global::InteractPayrollClient.Properties.Resources.NewLock16;
            this.picGroupLock.Location = new System.Drawing.Point(11, 136);
            this.picGroupLock.Name = "picGroupLock";
            this.picGroupLock.Size = new System.Drawing.Size(16, 16);
            this.picGroupLock.TabIndex = 226;
            this.picGroupLock.TabStop = false;
            this.picGroupLock.Visible = false;
            // 
            // frmEmployeeGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1179, 573);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.picCompanyLock);
            this.Controls.Add(this.grbGroup);
            this.Controls.Add(this.lblCompany);
            this.Controls.Add(this.picGroupLock);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.lblGroup);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.dgvCompanyDataGridView);
            this.Controls.Add(this.dgvGroupDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmEmployeeGroup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmEmployeeGroup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmEmployeeGroup_FormClosing);
            this.Load += new System.EventHandler(this.frmEmployeeGroup_Load);
            this.grbGroup.ResumeLayout(false);
            this.grbGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeLinkedDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCompanyDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroupDataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCompanyLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGroupLock)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblEmployee;
        private System.Windows.Forms.Label lblSelectedEmployee;
        private System.Windows.Forms.TextBox txtGroup;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.PictureBox picCompanyLock;
        private System.Windows.Forms.GroupBox grbGroup;
        private System.Windows.Forms.Label lblCompany;
        private System.Windows.Forms.Timer tmrTimer;
        private System.Windows.Forms.PictureBox picGroupLock;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Label lblGroup;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvCompanyDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn RecordIndex;
        private System.Windows.Forms.DataGridView dgvGroupDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbnNameSurname;
        private System.Windows.Forms.RadioButton rbnSurnameName;
        private System.Windows.Forms.DataGridView dgvEmployeeDataGridView;
        private System.Windows.Forms.DataGridView dgvEmployeeLinkedDataGridView;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
    }
}