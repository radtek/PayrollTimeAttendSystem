namespace InteractPayroll
{
    partial class frmOccupationDepartmentLink
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblEmployeeNotLinked = new System.Windows.Forms.Label();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lblEmployeeLinked = new System.Windows.Forms.Label();
            this.lblListEmployees = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.picOccupationDepartmentLock = new System.Windows.Forms.PictureBox();
            this.picPayrollTypeLock = new System.Windows.Forms.PictureBox();
            this.lblPayrollType = new System.Windows.Forms.Label();
            this.dgvPayrollTypeDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvOccupationDepartmentDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvEmployeeDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvEmployeeSelectedDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvEmployeeNotLinkedDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grbLegend = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblLegend = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picOccupationDepartmentLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPayrollTypeLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOccupationDepartmentDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeSelectedDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeNotLinkedDataGridView)).BeginInit();
            this.grbLegend.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblEmployeeNotLinked
            // 
            this.lblEmployeeNotLinked.BackColor = System.Drawing.Color.Yellow;
            this.lblEmployeeNotLinked.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEmployeeNotLinked.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmployeeNotLinked.ForeColor = System.Drawing.Color.Black;
            this.lblEmployeeNotLinked.Location = new System.Drawing.Point(8, 409);
            this.lblEmployeeNotLinked.Name = "lblEmployeeNotLinked";
            this.lblEmployeeNotLinked.Size = new System.Drawing.Size(454, 20);
            this.lblEmployeeNotLinked.TabIndex = 265;
            this.lblEmployeeNotLinked.Text = "Employees NOT Linked to Any User (Excludes Administrators)";
            this.lblEmployeeNotLinked.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAddAll
            // 
            this.btnAddAll.Enabled = false;
            this.btnAddAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddAll.Location = new System.Drawing.Point(465, 237);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(72, 24);
            this.btnAddAll.TabIndex = 260;
            this.btnAddAll.Text = "Add All";
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Enabled = false;
            this.btnRemoveAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveAll.Location = new System.Drawing.Point(465, 301);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(72, 24);
            this.btnRemoveAll.TabIndex = 262;
            this.btnRemoveAll.Text = "Remove All";
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(465, 205);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(72, 24);
            this.btnAdd.TabIndex = 259;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(465, 269);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(72, 24);
            this.btnRemove.TabIndex = 261;
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lblEmployeeLinked
            // 
            this.lblEmployeeLinked.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblEmployeeLinked.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEmployeeLinked.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmployeeLinked.ForeColor = System.Drawing.Color.Black;
            this.lblEmployeeLinked.Location = new System.Drawing.Point(540, 132);
            this.lblEmployeeLinked.Name = "lblEmployeeLinked";
            this.lblEmployeeLinked.Size = new System.Drawing.Size(454, 20);
            this.lblEmployeeLinked.TabIndex = 264;
            this.lblEmployeeLinked.Text = "Employees Linked";
            this.lblEmployeeLinked.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblListEmployees
            // 
            this.lblListEmployees.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblListEmployees.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblListEmployees.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblListEmployees.ForeColor = System.Drawing.Color.Black;
            this.lblListEmployees.Location = new System.Drawing.Point(8, 132);
            this.lblListEmployees.Name = "lblListEmployees";
            this.lblListEmployees.Size = new System.Drawing.Size(454, 20);
            this.lblListEmployees.TabIndex = 263;
            this.lblListEmployees.Text = "List of Employees";
            this.lblListEmployees.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDescription
            // 
            this.lblDescription.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDescription.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.ForeColor = System.Drawing.Color.Black;
            this.lblDescription.Location = new System.Drawing.Point(8, 8);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(327, 20);
            this.lblDescription.TabIndex = 257;
            this.lblDescription.Text = "Department";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(922, 97);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 255;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(922, 67);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 24);
            this.btnCancel.TabIndex = 254;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(922, 37);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 24);
            this.btnSave.TabIndex = 253;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Location = new System.Drawing.Point(922, 7);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(72, 24);
            this.btnUpdate.TabIndex = 251;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // picOccupationDepartmentLock
            // 
            this.picOccupationDepartmentLock.BackColor = System.Drawing.SystemColors.Control;
            this.picOccupationDepartmentLock.Image = global::OccupationDepartmentLink.Properties.Resources.NewLock16;
            this.picOccupationDepartmentLock.Location = new System.Drawing.Point(11, 29);
            this.picOccupationDepartmentLock.Name = "picOccupationDepartmentLock";
            this.picOccupationDepartmentLock.Size = new System.Drawing.Size(16, 16);
            this.picOccupationDepartmentLock.TabIndex = 269;
            this.picOccupationDepartmentLock.TabStop = false;
            this.picOccupationDepartmentLock.Visible = false;
            // 
            // picPayrollTypeLock
            // 
            this.picPayrollTypeLock.BackColor = System.Drawing.SystemColors.Control;
            this.picPayrollTypeLock.Image = global::OccupationDepartmentLink.Properties.Resources.NewLock16;
            this.picPayrollTypeLock.Location = new System.Drawing.Point(346, 29);
            this.picPayrollTypeLock.Name = "picPayrollTypeLock";
            this.picPayrollTypeLock.Size = new System.Drawing.Size(16, 16);
            this.picPayrollTypeLock.TabIndex = 279;
            this.picPayrollTypeLock.TabStop = false;
            this.picPayrollTypeLock.Visible = false;
            // 
            // lblPayrollType
            // 
            this.lblPayrollType.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblPayrollType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPayrollType.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPayrollType.ForeColor = System.Drawing.Color.Black;
            this.lblPayrollType.Location = new System.Drawing.Point(343, 8);
            this.lblPayrollType.Name = "lblPayrollType";
            this.lblPayrollType.Size = new System.Drawing.Size(119, 20);
            this.lblPayrollType.TabIndex = 278;
            this.lblPayrollType.Text = "Type";
            this.lblPayrollType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.dgvPayrollTypeDataGridView.Location = new System.Drawing.Point(343, 26);
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
            this.dgvPayrollTypeDataGridView.Size = new System.Drawing.Size(119, 60);
            this.dgvPayrollTypeDataGridView.TabIndex = 343;
            this.dgvPayrollTypeDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPayrollTypeDataGridView_RowEnter);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Description";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.dataGridViewTextBoxColumn1.Width = 99;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Visible = false;
            // 
            // dgvOccupationDepartmentDataGridView
            // 
            this.dgvOccupationDepartmentDataGridView.AllowUserToAddRows = false;
            this.dgvOccupationDepartmentDataGridView.AllowUserToDeleteRows = false;
            this.dgvOccupationDepartmentDataGridView.AllowUserToResizeColumns = false;
            this.dgvOccupationDepartmentDataGridView.AllowUserToResizeRows = false;
            this.dgvOccupationDepartmentDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvOccupationDepartmentDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvOccupationDepartmentDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvOccupationDepartmentDataGridView.ColumnHeadersHeight = 20;
            this.dgvOccupationDepartmentDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvOccupationDepartmentDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn4});
            this.dgvOccupationDepartmentDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvOccupationDepartmentDataGridView.EnableHeadersVisualStyles = false;
            this.dgvOccupationDepartmentDataGridView.Location = new System.Drawing.Point(8, 26);
            this.dgvOccupationDepartmentDataGridView.MultiSelect = false;
            this.dgvOccupationDepartmentDataGridView.Name = "dgvOccupationDepartmentDataGridView";
            this.dgvOccupationDepartmentDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvOccupationDepartmentDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvOccupationDepartmentDataGridView.RowHeadersWidth = 20;
            this.dgvOccupationDepartmentDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvOccupationDepartmentDataGridView.RowTemplate.Height = 19;
            this.dgvOccupationDepartmentDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvOccupationDepartmentDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOccupationDepartmentDataGridView.ShowCellToolTips = false;
            this.dgvOccupationDepartmentDataGridView.ShowEditingIcon = false;
            this.dgvOccupationDepartmentDataGridView.ShowRowErrors = false;
            this.dgvOccupationDepartmentDataGridView.Size = new System.Drawing.Size(327, 98);
            this.dgvOccupationDepartmentDataGridView.TabIndex = 344;
            this.dgvOccupationDepartmentDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOccupationDepartmentDataGridView_RowEnter);
            this.dgvOccupationDepartmentDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Description";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 288;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Visible = false;
            // 
            // dgvEmployeeDataGridView
            // 
            this.dgvEmployeeDataGridView.AllowUserToAddRows = false;
            this.dgvEmployeeDataGridView.AllowUserToDeleteRows = false;
            this.dgvEmployeeDataGridView.AllowUserToResizeColumns = false;
            this.dgvEmployeeDataGridView.AllowUserToResizeRows = false;
            this.dgvEmployeeDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvEmployeeDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployeeDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvEmployeeDataGridView.ColumnHeadersHeight = 20;
            this.dgvEmployeeDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvEmployeeDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.Column1,
            this.Column2,
            this.dataGridViewTextBoxColumn6});
            this.dgvEmployeeDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvEmployeeDataGridView.EnableHeadersVisualStyles = false;
            this.dgvEmployeeDataGridView.Location = new System.Drawing.Point(8, 150);
            this.dgvEmployeeDataGridView.MultiSelect = false;
            this.dgvEmployeeDataGridView.Name = "dgvEmployeeDataGridView";
            this.dgvEmployeeDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployeeDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvEmployeeDataGridView.RowHeadersWidth = 20;
            this.dgvEmployeeDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvEmployeeDataGridView.RowTemplate.Height = 19;
            this.dgvEmployeeDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvEmployeeDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployeeDataGridView.ShowCellToolTips = false;
            this.dgvEmployeeDataGridView.ShowEditingIcon = false;
            this.dgvEmployeeDataGridView.ShowRowErrors = false;
            this.dgvEmployeeDataGridView.Size = new System.Drawing.Size(454, 250);
            this.dgvEmployeeDataGridView.TabIndex = 345;
            this.dgvEmployeeDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEmployeeDataGridView_RowEnter);
            this.dgvEmployeeDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            this.dgvEmployeeDataGridView.DoubleClick += new System.EventHandler(this.dgvEmployeeDataGridView_DoubleClick);
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Code";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 70;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Surname";
            this.Column1.Name = "Column1";
            this.Column1.Width = 175;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Name";
            this.Column2.Name = "Column2";
            this.Column2.Width = 170;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.Visible = false;
            // 
            // dgvEmployeeSelectedDataGridView
            // 
            this.dgvEmployeeSelectedDataGridView.AllowUserToAddRows = false;
            this.dgvEmployeeSelectedDataGridView.AllowUserToDeleteRows = false;
            this.dgvEmployeeSelectedDataGridView.AllowUserToResizeColumns = false;
            this.dgvEmployeeSelectedDataGridView.AllowUserToResizeRows = false;
            this.dgvEmployeeSelectedDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvEmployeeSelectedDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployeeSelectedDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvEmployeeSelectedDataGridView.ColumnHeadersHeight = 20;
            this.dgvEmployeeSelectedDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvEmployeeSelectedDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn9,
            this.dataGridViewTextBoxColumn10});
            this.dgvEmployeeSelectedDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvEmployeeSelectedDataGridView.EnableHeadersVisualStyles = false;
            this.dgvEmployeeSelectedDataGridView.Location = new System.Drawing.Point(540, 150);
            this.dgvEmployeeSelectedDataGridView.MultiSelect = false;
            this.dgvEmployeeSelectedDataGridView.Name = "dgvEmployeeSelectedDataGridView";
            this.dgvEmployeeSelectedDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployeeSelectedDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvEmployeeSelectedDataGridView.RowHeadersWidth = 20;
            this.dgvEmployeeSelectedDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvEmployeeSelectedDataGridView.RowTemplate.Height = 19;
            this.dgvEmployeeSelectedDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvEmployeeSelectedDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployeeSelectedDataGridView.ShowCellToolTips = false;
            this.dgvEmployeeSelectedDataGridView.ShowEditingIcon = false;
            this.dgvEmployeeSelectedDataGridView.ShowRowErrors = false;
            this.dgvEmployeeSelectedDataGridView.Size = new System.Drawing.Size(454, 250);
            this.dgvEmployeeSelectedDataGridView.TabIndex = 346;
            this.dgvEmployeeSelectedDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            this.dgvEmployeeSelectedDataGridView.DoubleClick += new System.EventHandler(this.dgvEmployeeSelectedDataGridView_DoubleClick);
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "Code";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.Width = 70;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "Surname";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.Width = 175;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.HeaderText = "Name";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.Width = 170;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.Visible = false;
            // 
            // dgvEmployeeNotLinkedDataGridView
            // 
            this.dgvEmployeeNotLinkedDataGridView.AllowUserToAddRows = false;
            this.dgvEmployeeNotLinkedDataGridView.AllowUserToDeleteRows = false;
            this.dgvEmployeeNotLinkedDataGridView.AllowUserToResizeColumns = false;
            this.dgvEmployeeNotLinkedDataGridView.AllowUserToResizeRows = false;
            this.dgvEmployeeNotLinkedDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvEmployeeNotLinkedDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployeeNotLinkedDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvEmployeeNotLinkedDataGridView.ColumnHeadersHeight = 20;
            this.dgvEmployeeNotLinkedDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvEmployeeNotLinkedDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn11,
            this.dataGridViewTextBoxColumn12,
            this.dataGridViewTextBoxColumn13,
            this.dataGridViewTextBoxColumn14});
            this.dgvEmployeeNotLinkedDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvEmployeeNotLinkedDataGridView.EnableHeadersVisualStyles = false;
            this.dgvEmployeeNotLinkedDataGridView.Location = new System.Drawing.Point(8, 427);
            this.dgvEmployeeNotLinkedDataGridView.MultiSelect = false;
            this.dgvEmployeeNotLinkedDataGridView.Name = "dgvEmployeeNotLinkedDataGridView";
            this.dgvEmployeeNotLinkedDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployeeNotLinkedDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvEmployeeNotLinkedDataGridView.RowHeadersWidth = 20;
            this.dgvEmployeeNotLinkedDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvEmployeeNotLinkedDataGridView.RowTemplate.Height = 19;
            this.dgvEmployeeNotLinkedDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvEmployeeNotLinkedDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployeeNotLinkedDataGridView.ShowCellToolTips = false;
            this.dgvEmployeeNotLinkedDataGridView.ShowEditingIcon = false;
            this.dgvEmployeeNotLinkedDataGridView.ShowRowErrors = false;
            this.dgvEmployeeNotLinkedDataGridView.Size = new System.Drawing.Size(454, 117);
            this.dgvEmployeeNotLinkedDataGridView.TabIndex = 347;
            this.dgvEmployeeNotLinkedDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.HeaderText = "Code";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.Width = 70;
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.HeaderText = "Surname";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            this.dataGridViewTextBoxColumn12.Width = 175;
            // 
            // dataGridViewTextBoxColumn13
            // 
            this.dataGridViewTextBoxColumn13.HeaderText = "Name";
            this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
            this.dataGridViewTextBoxColumn13.Width = 170;
            // 
            // dataGridViewTextBoxColumn14
            // 
            this.dataGridViewTextBoxColumn14.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn14.Name = "dataGridViewTextBoxColumn14";
            this.dataGridViewTextBoxColumn14.Visible = false;
            // 
            // grbLegend
            // 
            this.grbLegend.Controls.Add(this.panel1);
            this.grbLegend.Controls.Add(this.lblLegend);
            this.grbLegend.Location = new System.Drawing.Point(540, 498);
            this.grbLegend.Name = "grbLegend";
            this.grbLegend.Size = new System.Drawing.Size(454, 48);
            this.grbLegend.TabIndex = 348;
            this.grbLegend.TabStop = false;
            this.grbLegend.Text = "Row Legend";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Yellow;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(9, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(18, 18);
            this.panel1.TabIndex = 15;
            // 
            // lblLegend
            // 
            this.lblLegend.Location = new System.Drawing.Point(29, 22);
            this.lblLegend.Name = "lblLegend";
            this.lblLegend.Size = new System.Drawing.Size(404, 16);
            this.lblLegend.TabIndex = 14;
            this.lblLegend.Text = "Days Excluded";
            // 
            // frmOccupationDepartmentLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1000, 552);
            this.Controls.Add(this.grbLegend);
            this.Controls.Add(this.picPayrollTypeLock);
            this.Controls.Add(this.picOccupationDepartmentLock);
            this.Controls.Add(this.lblPayrollType);
            this.Controls.Add(this.lblEmployeeNotLinked);
            this.Controls.Add(this.btnAddAll);
            this.Controls.Add(this.btnRemoveAll);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.lblEmployeeLinked);
            this.Controls.Add(this.lblListEmployees);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.dgvPayrollTypeDataGridView);
            this.Controls.Add(this.dgvOccupationDepartmentDataGridView);
            this.Controls.Add(this.dgvEmployeeDataGridView);
            this.Controls.Add(this.dgvEmployeeSelectedDataGridView);
            this.Controls.Add(this.dgvEmployeeNotLinkedDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmOccupationDepartmentLink";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmOccupationDepartment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picOccupationDepartmentLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPayrollTypeLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOccupationDepartmentDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeSelectedDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeNotLinkedDataGridView)).EndInit();
            this.grbLegend.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblEmployeeNotLinked;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label lblEmployeeLinked;
        private System.Windows.Forms.Label lblListEmployees;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.PictureBox picOccupationDepartmentLock;
        private System.Windows.Forms.PictureBox picPayrollTypeLock;
        private System.Windows.Forms.Label lblPayrollType;
        private System.Windows.Forms.DataGridView dgvPayrollTypeDataGridView;
        private System.Windows.Forms.DataGridView dgvOccupationDepartmentDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridView dgvEmployeeDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridView dgvEmployeeSelectedDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridView dgvEmployeeNotLinkedDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn14;
        private System.Windows.Forms.GroupBox grbLegend;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblLegend;
    }
}

