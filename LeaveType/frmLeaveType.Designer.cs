namespace InteractPayroll
{
    partial class frmLeaveType
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label52 = new System.Windows.Forms.Label();
            this.txtHeader2 = new System.Windows.Forms.TextBox();
            this.txtHeader1 = new System.Windows.Forms.TextBox();
            this.cboPercentage = new System.Windows.Forms.ComboBox();
            this.lblPercentage = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbnUnPaid = new System.Windows.Forms.RadioButton();
            this.rbnPaid = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblHeader2 = new System.Windows.Forms.Label();
            this.lblHeader1 = new System.Windows.Forms.Label();
            this.txtLeaveType = new System.Windows.Forms.TextBox();
            this.lblLeaveType = new System.Windows.Forms.Label();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblPayrollType = new System.Windows.Forms.Label();
            this.dgvPayrollTypeDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvLeaveTypeDataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grbLeaveLock = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblLeaveTypeLockDesc = new System.Windows.Forms.Label();
            this.picPayrollTypeLock = new System.Windows.Forms.PictureBox();
            this.picLeaveTypeLock = new System.Windows.Forms.PictureBox();
            this.groupBox11.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeaveTypeDataGridView)).BeginInit();
            this.grbLeaveLock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPayrollTypeLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLeaveTypeLock)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.panel3);
            this.groupBox11.Controls.Add(this.label52);
            this.groupBox11.Location = new System.Drawing.Point(415, 317);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(191, 44);
            this.groupBox11.TabIndex = 245;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Row Legend";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Magenta;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Location = new System.Drawing.Point(10, 18);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(18, 18);
            this.panel3.TabIndex = 16;
            // 
            // label52
            // 
            this.label52.Location = new System.Drawing.Point(32, 20);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(112, 16);
            this.label52.TabIndex = 2;
            this.label52.Text = "Locked - Payroll Run";
            // 
            // txtHeader2
            // 
            this.txtHeader2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHeader2.Enabled = false;
            this.txtHeader2.Location = new System.Drawing.Point(48, 42);
            this.txtHeader2.MaxLength = 10;
            this.txtHeader2.Name = "txtHeader2";
            this.txtHeader2.Size = new System.Drawing.Size(88, 20);
            this.txtHeader2.TabIndex = 3;
            // 
            // txtHeader1
            // 
            this.txtHeader1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHeader1.Enabled = false;
            this.txtHeader1.Location = new System.Drawing.Point(48, 18);
            this.txtHeader1.MaxLength = 10;
            this.txtHeader1.Name = "txtHeader1";
            this.txtHeader1.Size = new System.Drawing.Size(88, 20);
            this.txtHeader1.TabIndex = 2;
            // 
            // cboPercentage
            // 
            this.cboPercentage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPercentage.Enabled = false;
            this.cboPercentage.Location = new System.Drawing.Point(158, 23);
            this.cboPercentage.Name = "cboPercentage";
            this.cboPercentage.Size = new System.Drawing.Size(52, 21);
            this.cboPercentage.TabIndex = 8;
            // 
            // lblPercentage
            // 
            this.lblPercentage.Location = new System.Drawing.Point(53, 23);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(99, 17);
            this.lblPercentage.TabIndex = 7;
            this.lblPercentage.Text = "Percentage Paid";
            this.lblPercentage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(5, 258);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 20);
            this.label1.TabIndex = 18;
            this.label1.Text = "Description ";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cboPercentage);
            this.groupBox4.Controls.Add(this.lblPercentage);
            this.groupBox4.Controls.Add(this.rbnUnPaid);
            this.groupBox4.Controls.Add(this.rbnPaid);
            this.groupBox4.Location = new System.Drawing.Point(8, 289);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(224, 72);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Option";
            // 
            // rbnUnPaid
            // 
            this.rbnUnPaid.Enabled = false;
            this.rbnUnPaid.Location = new System.Drawing.Point(8, 43);
            this.rbnUnPaid.Name = "rbnUnPaid";
            this.rbnUnPaid.Size = new System.Drawing.Size(60, 24);
            this.rbnUnPaid.TabIndex = 4;
            this.rbnUnPaid.Text = "Unpaid";
            this.rbnUnPaid.Click += new System.EventHandler(this.rbnUnPaid_Click);
            // 
            // rbnPaid
            // 
            this.rbnPaid.Checked = true;
            this.rbnPaid.Enabled = false;
            this.rbnPaid.Location = new System.Drawing.Point(8, 24);
            this.rbnPaid.Name = "rbnPaid";
            this.rbnPaid.Size = new System.Drawing.Size(56, 16);
            this.rbnPaid.TabIndex = 3;
            this.rbnPaid.TabStop = true;
            this.rbnPaid.Text = "&Paid";
            this.rbnPaid.Click += new System.EventHandler(this.rbnPaid_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtHeader2);
            this.groupBox1.Controls.Add(this.txtHeader1);
            this.groupBox1.Controls.Add(this.lblHeader2);
            this.groupBox1.Controls.Add(this.lblHeader1);
            this.groupBox1.Location = new System.Drawing.Point(239, 289);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(167, 72);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Spreadsheet Report Header/s";
            // 
            // lblHeader2
            // 
            this.lblHeader2.Location = new System.Drawing.Point(8, 46);
            this.lblHeader2.Name = "lblHeader2";
            this.lblHeader2.Size = new System.Drawing.Size(36, 16);
            this.lblHeader2.TabIndex = 1;
            this.lblHeader2.Text = "Line 2";
            // 
            // lblHeader1
            // 
            this.lblHeader1.Location = new System.Drawing.Point(8, 22);
            this.lblHeader1.Name = "lblHeader1";
            this.lblHeader1.Size = new System.Drawing.Size(36, 16);
            this.lblHeader1.TabIndex = 0;
            this.lblHeader1.Text = "Line 1";
            // 
            // txtLeaveType
            // 
            this.txtLeaveType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLeaveType.Enabled = false;
            this.txtLeaveType.Location = new System.Drawing.Point(73, 257);
            this.txtLeaveType.MaxLength = 20;
            this.txtLeaveType.Name = "txtLeaveType";
            this.txtLeaveType.Size = new System.Drawing.Size(260, 20);
            this.txtLeaveType.TabIndex = 13;
            this.txtLeaveType.Leave += new System.EventHandler(this.txtLeaveType_Leave);
            // 
            // lblLeaveType
            // 
            this.lblLeaveType.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblLeaveType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLeaveType.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeaveType.ForeColor = System.Drawing.Color.Black;
            this.lblLeaveType.Location = new System.Drawing.Point(8, 8);
            this.lblLeaveType.Name = "lblLeaveType";
            this.lblLeaveType.Size = new System.Drawing.Size(324, 20);
            this.lblLeaveType.TabIndex = 242;
            this.lblLeaveType.Text = "Leave Type";
            this.lblLeaveType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNew
            // 
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Location = new System.Drawing.Point(536, 8);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(72, 24);
            this.btnNew.TabIndex = 235;
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(536, 132);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 24);
            this.btnCancel.TabIndex = 239;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(536, 101);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 24);
            this.btnSave.TabIndex = 238;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(536, 163);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 240;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Location = new System.Drawing.Point(536, 39);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(72, 24);
            this.btnUpdate.TabIndex = 236;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(536, 70);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(72, 24);
            this.btnDelete.TabIndex = 237;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblPayrollType
            // 
            this.lblPayrollType.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblPayrollType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPayrollType.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPayrollType.ForeColor = System.Drawing.Color.Black;
            this.lblPayrollType.Location = new System.Drawing.Point(341, 8);
            this.lblPayrollType.Name = "lblPayrollType";
            this.lblPayrollType.Size = new System.Drawing.Size(189, 20);
            this.lblPayrollType.TabIndex = 281;
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
            this.dgvPayrollTypeDataGridView.Location = new System.Drawing.Point(341, 26);
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
            this.dgvPayrollTypeDataGridView.Size = new System.Drawing.Size(189, 60);
            this.dgvPayrollTypeDataGridView.TabIndex = 343;
            this.dgvPayrollTypeDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPayrollTypeDataGridView_RowEnter);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Description";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.dataGridViewTextBoxColumn1.Width = 169;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Visible = false;
            // 
            // dgvLeaveTypeDataGridView
            // 
            this.dgvLeaveTypeDataGridView.AllowUserToAddRows = false;
            this.dgvLeaveTypeDataGridView.AllowUserToDeleteRows = false;
            this.dgvLeaveTypeDataGridView.AllowUserToResizeColumns = false;
            this.dgvLeaveTypeDataGridView.AllowUserToResizeRows = false;
            this.dgvLeaveTypeDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvLeaveTypeDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLeaveTypeDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvLeaveTypeDataGridView.ColumnHeadersHeight = 20;
            this.dgvLeaveTypeDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvLeaveTypeDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn4});
            this.dgvLeaveTypeDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvLeaveTypeDataGridView.EnableHeadersVisualStyles = false;
            this.dgvLeaveTypeDataGridView.Location = new System.Drawing.Point(8, 26);
            this.dgvLeaveTypeDataGridView.MultiSelect = false;
            this.dgvLeaveTypeDataGridView.Name = "dgvLeaveTypeDataGridView";
            this.dgvLeaveTypeDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLeaveTypeDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvLeaveTypeDataGridView.RowHeadersWidth = 20;
            this.dgvLeaveTypeDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvLeaveTypeDataGridView.RowTemplate.Height = 19;
            this.dgvLeaveTypeDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvLeaveTypeDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLeaveTypeDataGridView.ShowCellToolTips = false;
            this.dgvLeaveTypeDataGridView.ShowEditingIcon = false;
            this.dgvLeaveTypeDataGridView.ShowRowErrors = false;
            this.dgvLeaveTypeDataGridView.Size = new System.Drawing.Size(324, 212);
            this.dgvLeaveTypeDataGridView.TabIndex = 344;
            this.dgvLeaveTypeDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLeaveTypeDataGridView_RowEnter);
            this.dgvLeaveTypeDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            // 
            // Column1
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Control;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column1.HeaderText = "";
            this.Column1.Name = "Column1";
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.Width = 20;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Description";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 265;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Visible = false;
            // 
            // grbLeaveLock
            // 
            this.grbLeaveLock.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.grbLeaveLock.Controls.Add(this.pictureBox1);
            this.grbLeaveLock.Controls.Add(this.lblLeaveTypeLockDesc);
            this.grbLeaveLock.Location = new System.Drawing.Point(340, 90);
            this.grbLeaveLock.Name = "grbLeaveLock";
            this.grbLeaveLock.Size = new System.Drawing.Size(190, 75);
            this.grbLeaveLock.TabIndex = 345;
            this.grbLeaveLock.TabStop = false;
            this.grbLeaveLock.Text = "Leave Type Record Lock";
            this.grbLeaveLock.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::InteractPayroll.Properties.Resources.NewLock48;
            this.pictureBox1.Location = new System.Drawing.Point(7, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // lblLeaveTypeLockDesc
            // 
            this.lblLeaveTypeLockDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeaveTypeLockDesc.ForeColor = System.Drawing.Color.Black;
            this.lblLeaveTypeLockDesc.Location = new System.Drawing.Point(61, 16);
            this.lblLeaveTypeLockDesc.Name = "lblLeaveTypeLockDesc";
            this.lblLeaveTypeLockDesc.Size = new System.Drawing.Size(117, 56);
            this.lblLeaveTypeLockDesc.TabIndex = 0;
            this.lblLeaveTypeLockDesc.Text = "Leave Records are Locked Due to Current Wage Run.";
            this.lblLeaveTypeLockDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picPayrollTypeLock
            // 
            this.picPayrollTypeLock.BackColor = System.Drawing.SystemColors.Control;
            this.picPayrollTypeLock.Image = global::InteractPayroll.Properties.Resources.NewLock16;
            this.picPayrollTypeLock.Location = new System.Drawing.Point(344, 29);
            this.picPayrollTypeLock.Name = "picPayrollTypeLock";
            this.picPayrollTypeLock.Size = new System.Drawing.Size(16, 16);
            this.picPayrollTypeLock.TabIndex = 282;
            this.picPayrollTypeLock.TabStop = false;
            this.picPayrollTypeLock.Visible = false;
            // 
            // picLeaveTypeLock
            // 
            this.picLeaveTypeLock.BackColor = System.Drawing.SystemColors.Control;
            this.picLeaveTypeLock.Image = global::InteractPayroll.Properties.Resources.NewLock16;
            this.picLeaveTypeLock.Location = new System.Drawing.Point(11, 29);
            this.picLeaveTypeLock.Name = "picLeaveTypeLock";
            this.picLeaveTypeLock.Size = new System.Drawing.Size(16, 16);
            this.picLeaveTypeLock.TabIndex = 244;
            this.picLeaveTypeLock.TabStop = false;
            this.picLeaveTypeLock.Visible = false;
            // 
            // frmLeaveType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(614, 369);
            this.Controls.Add(this.grbLeaveLock);
            this.Controls.Add(this.picPayrollTypeLock);
            this.Controls.Add(this.lblPayrollType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox11);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.picLeaveTypeLock);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtLeaveType);
            this.Controls.Add(this.lblLeaveType);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.dgvPayrollTypeDataGridView);
            this.Controls.Add(this.dgvLeaveTypeDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmLeaveType";
            this.Text = "frmLeaveType";
            this.Load += new System.EventHandler(this.frmLeaveType_Load);
            this.groupBox11.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeaveTypeDataGridView)).EndInit();
            this.grbLeaveLock.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPayrollTypeLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLeaveTypeLock)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.TextBox txtHeader2;
        private System.Windows.Forms.TextBox txtHeader1;
        private System.Windows.Forms.ComboBox cboPercentage;
        private System.Windows.Forms.Label lblPercentage;
        private System.Windows.Forms.PictureBox picLeaveTypeLock;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbnUnPaid;
        private System.Windows.Forms.RadioButton rbnPaid;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblHeader2;
        private System.Windows.Forms.Label lblHeader1;
        public System.Windows.Forms.TextBox txtLeaveType;
        private System.Windows.Forms.Label lblLeaveType;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox picPayrollTypeLock;
        private System.Windows.Forms.Label lblPayrollType;
        private System.Windows.Forms.DataGridView dgvPayrollTypeDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridView dgvLeaveTypeDataGridView;
        private System.Windows.Forms.GroupBox grbLeaveLock;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblLeaveTypeLockDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    }
}

