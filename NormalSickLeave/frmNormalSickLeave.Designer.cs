namespace InteractPayroll
{
    partial class frmNormalSickLeave
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txtLeave = new System.Windows.Forms.TextBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.label52 = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblPayrollType = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtSickLeaveAccum = new System.Windows.Forms.TextBox();
            this.lblSickDesc = new System.Windows.Forms.Label();
            this.cboSickPaid = new System.Windows.Forms.ComboBox();
            this.lblSickPaid = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtNormLeaveAccum = new System.Windows.Forms.TextBox();
            this.lblNormalDesc = new System.Windows.Forms.Label();
            this.cboNormalPaid = new System.Windows.Forms.ComboBox();
            this.lblNormalPaid = new System.Windows.Forms.Label();
            this.grbShiftDetails = new System.Windows.Forms.GroupBox();
            this.label44 = new System.Windows.Forms.Label();
            this.cboMaxShifts = new System.Windows.Forms.ComboBox();
            this.cboMinShiftMinutes = new System.Windows.Forms.ComboBox();
            this.cboMinShiftHours = new System.Windows.Forms.ComboBox();
            this.lblMaxShifts = new System.Windows.Forms.Label();
            this.lblMinShift = new System.Windows.Forms.Label();
            this.grbWageDayOption = new System.Windows.Forms.GroupBox();
            this.rbnWeekDaySaturday = new System.Windows.Forms.RadioButton();
            this.rbnAllDays = new System.Windows.Forms.RadioButton();
            this.rbnWeekDays = new System.Windows.Forms.RadioButton();
            this.dgvPayrollTypeDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvNormalSickLeaveCategoryDataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grbLeaveLock = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblLeaveTypeLockDesc = new System.Windows.Forms.Label();
            this.picLeaveLock = new System.Windows.Forms.PictureBox();
            this.picPayrollTypeLock = new System.Windows.Forms.PictureBox();
            this.groupBox11.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.grbShiftDetails.SuspendLayout();
            this.grbWageDayOption.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNormalSickLeaveCategoryDataGridView)).BeginInit();
            this.grbLeaveLock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLeaveLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPayrollTypeLock)).BeginInit();
            this.SuspendLayout();
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
            // txtLeave
            // 
            this.txtLeave.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLeave.Enabled = false;
            this.txtLeave.Location = new System.Drawing.Point(74, 250);
            this.txtLeave.MaxLength = 30;
            this.txtLeave.Name = "txtLeave";
            this.txtLeave.Size = new System.Drawing.Size(264, 20);
            this.txtLeave.TabIndex = 0;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.panel3);
            this.groupBox11.Controls.Add(this.label52);
            this.groupBox11.Location = new System.Drawing.Point(461, 402);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(155, 44);
            this.groupBox11.TabIndex = 245;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Row Legend";
            // 
            // label52
            // 
            this.label52.Location = new System.Drawing.Point(32, 20);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(112, 16);
            this.label52.TabIndex = 2;
            this.label52.Text = "Locked - Payroll Run";
            // 
            // lblDescription
            // 
            this.lblDescription.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDescription.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.ForeColor = System.Drawing.Color.Black;
            this.lblDescription.Location = new System.Drawing.Point(8, 8);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(329, 20);
            this.lblDescription.TabIndex = 243;
            this.lblDescription.Text = "Normal Leave / Sick Leave Category";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(5, 253);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(64, 20);
            this.label13.TabIndex = 125;
            this.label13.Text = "Description ";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Location = new System.Drawing.Point(544, 40);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(72, 24);
            this.btnUpdate.TabIndex = 238;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnNew
            // 
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Location = new System.Drawing.Point(544, 8);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(72, 24);
            this.btnNew.TabIndex = 237;
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(544, 136);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 24);
            this.btnCancel.TabIndex = 241;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(544, 104);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 24);
            this.btnSave.TabIndex = 240;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(544, 168);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 242;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(544, 72);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(72, 24);
            this.btnDelete.TabIndex = 239;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblPayrollType
            // 
            this.lblPayrollType.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblPayrollType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPayrollType.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPayrollType.ForeColor = System.Drawing.Color.Black;
            this.lblPayrollType.Location = new System.Drawing.Point(347, 8);
            this.lblPayrollType.Name = "lblPayrollType";
            this.lblPayrollType.Size = new System.Drawing.Size(189, 20);
            this.lblPayrollType.TabIndex = 278;
            this.lblPayrollType.Text = "Type";
            this.lblPayrollType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtSickLeaveAccum);
            this.groupBox3.Controls.Add(this.lblSickDesc);
            this.groupBox3.Controls.Add(this.cboSickPaid);
            this.groupBox3.Controls.Add(this.lblSickPaid);
            this.groupBox3.Location = new System.Drawing.Point(8, 366);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(258, 80);
            this.groupBox3.TabIndex = 282;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sick Leave";
            // 
            // txtSickLeaveAccum
            // 
            this.txtSickLeaveAccum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSickLeaveAccum.Enabled = false;
            this.txtSickLeaveAccum.Location = new System.Drawing.Point(200, 48);
            this.txtSickLeaveAccum.Name = "txtSickLeaveAccum";
            this.txtSickLeaveAccum.Size = new System.Drawing.Size(44, 20);
            this.txtSickLeaveAccum.TabIndex = 46;
            this.txtSickLeaveAccum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblSickDesc
            // 
            this.lblSickDesc.Location = new System.Drawing.Point(8, 52);
            this.lblSickDesc.Name = "lblSickDesc";
            this.lblSickDesc.Size = new System.Drawing.Size(184, 20);
            this.lblSickDesc.TabIndex = 45;
            this.lblSickDesc.Text = "Sick Leave Accumulated / Day";
            // 
            // cboSickPaid
            // 
            this.cboSickPaid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSickPaid.Enabled = false;
            this.cboSickPaid.Location = new System.Drawing.Point(200, 19);
            this.cboSickPaid.Name = "cboSickPaid";
            this.cboSickPaid.Size = new System.Drawing.Size(44, 21);
            this.cboSickPaid.TabIndex = 43;
            this.cboSickPaid.SelectedIndexChanged += new System.EventHandler(this.cboSickPaid_SelectedIndexChanged);
            // 
            // lblSickPaid
            // 
            this.lblSickPaid.Location = new System.Drawing.Point(8, 24);
            this.lblSickPaid.Name = "lblSickPaid";
            this.lblSickPaid.Size = new System.Drawing.Size(116, 20);
            this.lblSickPaid.TabIndex = 0;
            this.lblSickPaid.Text = "Number of days paid";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtNormLeaveAccum);
            this.groupBox2.Controls.Add(this.lblNormalDesc);
            this.groupBox2.Controls.Add(this.cboNormalPaid);
            this.groupBox2.Controls.Add(this.lblNormalPaid);
            this.groupBox2.Location = new System.Drawing.Point(8, 282);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(258, 80);
            this.groupBox2.TabIndex = 281;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Normal Leave";
            // 
            // txtNormLeaveAccum
            // 
            this.txtNormLeaveAccum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNormLeaveAccum.Enabled = false;
            this.txtNormLeaveAccum.Location = new System.Drawing.Point(200, 49);
            this.txtNormLeaveAccum.Name = "txtNormLeaveAccum";
            this.txtNormLeaveAccum.Size = new System.Drawing.Size(44, 20);
            this.txtNormLeaveAccum.TabIndex = 44;
            this.txtNormLeaveAccum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblNormalDesc
            // 
            this.lblNormalDesc.Location = new System.Drawing.Point(8, 52);
            this.lblNormalDesc.Name = "lblNormalDesc";
            this.lblNormalDesc.Size = new System.Drawing.Size(188, 16);
            this.lblNormalDesc.TabIndex = 43;
            this.lblNormalDesc.Text = "Normal Leave Accumulated / Day";
            // 
            // cboNormalPaid
            // 
            this.cboNormalPaid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNormalPaid.Enabled = false;
            this.cboNormalPaid.Location = new System.Drawing.Point(200, 20);
            this.cboNormalPaid.Name = "cboNormalPaid";
            this.cboNormalPaid.Size = new System.Drawing.Size(44, 21);
            this.cboNormalPaid.TabIndex = 42;
            this.cboNormalPaid.SelectedIndexChanged += new System.EventHandler(this.cboNormalPaid_SelectedIndexChanged);
            // 
            // lblNormalPaid
            // 
            this.lblNormalPaid.Location = new System.Drawing.Point(8, 24);
            this.lblNormalPaid.Name = "lblNormalPaid";
            this.lblNormalPaid.Size = new System.Drawing.Size(116, 18);
            this.lblNormalPaid.TabIndex = 0;
            this.lblNormalPaid.Text = "Number of days paid";
            // 
            // grbShiftDetails
            // 
            this.grbShiftDetails.Controls.Add(this.label44);
            this.grbShiftDetails.Controls.Add(this.cboMaxShifts);
            this.grbShiftDetails.Controls.Add(this.cboMinShiftMinutes);
            this.grbShiftDetails.Controls.Add(this.cboMinShiftHours);
            this.grbShiftDetails.Controls.Add(this.lblMaxShifts);
            this.grbShiftDetails.Controls.Add(this.lblMinShift);
            this.grbShiftDetails.Location = new System.Drawing.Point(274, 282);
            this.grbShiftDetails.Name = "grbShiftDetails";
            this.grbShiftDetails.Size = new System.Drawing.Size(342, 80);
            this.grbShiftDetails.TabIndex = 283;
            this.grbShiftDetails.TabStop = false;
            this.grbShiftDetails.Text = "Shift Details";
            // 
            // label44
            // 
            this.label44.Location = new System.Drawing.Point(271, 23);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(48, 16);
            this.label44.TabIndex = 78;
            this.label44.Text = "(hh:mm)";
            // 
            // cboMaxShifts
            // 
            this.cboMaxShifts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMaxShifts.Enabled = false;
            this.cboMaxShifts.Location = new System.Drawing.Point(177, 49);
            this.cboMaxShifts.Name = "cboMaxShifts";
            this.cboMaxShifts.Size = new System.Drawing.Size(44, 21);
            this.cboMaxShifts.TabIndex = 2;
            this.cboMaxShifts.SelectedIndexChanged += new System.EventHandler(this.cboMaxShifts_SelectedIndexChanged);
            // 
            // cboMinShiftMinutes
            // 
            this.cboMinShiftMinutes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMinShiftMinutes.Enabled = false;
            this.cboMinShiftMinutes.Location = new System.Drawing.Point(227, 20);
            this.cboMinShiftMinutes.Name = "cboMinShiftMinutes";
            this.cboMinShiftMinutes.Size = new System.Drawing.Size(36, 21);
            this.cboMinShiftMinutes.TabIndex = 1;
            this.cboMinShiftMinutes.SelectedIndexChanged += new System.EventHandler(this.MinimumShift_SelectedIndexChanged);
            // 
            // cboMinShiftHours
            // 
            this.cboMinShiftHours.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMinShiftHours.Enabled = false;
            this.cboMinShiftHours.Location = new System.Drawing.Point(177, 20);
            this.cboMinShiftHours.Name = "cboMinShiftHours";
            this.cboMinShiftHours.Size = new System.Drawing.Size(44, 21);
            this.cboMinShiftHours.TabIndex = 0;
            this.cboMinShiftHours.SelectedIndexChanged += new System.EventHandler(this.MinimumShift_SelectedIndexChanged);
            // 
            // lblMaxShifts
            // 
            this.lblMaxShifts.Location = new System.Drawing.Point(8, 52);
            this.lblMaxShifts.Name = "lblMaxShifts";
            this.lblMaxShifts.Size = new System.Drawing.Size(148, 16);
            this.lblMaxShifts.TabIndex = 1;
            this.lblMaxShifts.Text = "Number of shifts for the year";
            // 
            // lblMinShift
            // 
            this.lblMinShift.Location = new System.Drawing.Point(8, 24);
            this.lblMinShift.Name = "lblMinShift";
            this.lblMinShift.Size = new System.Drawing.Size(155, 18);
            this.lblMinShift.TabIndex = 0;
            this.lblMinShift.Text = "Minimum hours for a valid shift";
            // 
            // grbWageDayOption
            // 
            this.grbWageDayOption.Controls.Add(this.rbnWeekDaySaturday);
            this.grbWageDayOption.Controls.Add(this.rbnAllDays);
            this.grbWageDayOption.Controls.Add(this.rbnWeekDays);
            this.grbWageDayOption.Location = new System.Drawing.Point(274, 366);
            this.grbWageDayOption.Name = "grbWageDayOption";
            this.grbWageDayOption.Size = new System.Drawing.Size(179, 80);
            this.grbWageDayOption.TabIndex = 284;
            this.grbWageDayOption.TabStop = false;
            this.grbWageDayOption.Text = "Leave Pay Option";
            // 
            // rbnWeekDaySaturday
            // 
            this.rbnWeekDaySaturday.Enabled = false;
            this.rbnWeekDaySaturday.Location = new System.Drawing.Point(8, 35);
            this.rbnWeekDaySaturday.Name = "rbnWeekDaySaturday";
            this.rbnWeekDaySaturday.Size = new System.Drawing.Size(144, 21);
            this.rbnWeekDaySaturday.TabIndex = 1;
            this.rbnWeekDaySaturday.Text = "Week Days + Saturdays ";
            // 
            // rbnAllDays
            // 
            this.rbnAllDays.Enabled = false;
            this.rbnAllDays.Location = new System.Drawing.Point(8, 55);
            this.rbnAllDays.Name = "rbnAllDays";
            this.rbnAllDays.Size = new System.Drawing.Size(72, 21);
            this.rbnAllDays.TabIndex = 2;
            this.rbnAllDays.Text = "All Days";
            // 
            // rbnWeekDays
            // 
            this.rbnWeekDays.Enabled = false;
            this.rbnWeekDays.Location = new System.Drawing.Point(8, 16);
            this.rbnWeekDays.Name = "rbnWeekDays";
            this.rbnWeekDays.Size = new System.Drawing.Size(136, 20);
            this.rbnWeekDays.TabIndex = 0;
            this.rbnWeekDays.Text = "Week Days Only";
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
            this.dgvPayrollTypeDataGridView.Location = new System.Drawing.Point(347, 26);
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
            this.dgvPayrollTypeDataGridView.TabIndex = 344;
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
            // dgvNormalSickLeaveCategoryDataGridView
            // 
            this.dgvNormalSickLeaveCategoryDataGridView.AllowUserToAddRows = false;
            this.dgvNormalSickLeaveCategoryDataGridView.AllowUserToDeleteRows = false;
            this.dgvNormalSickLeaveCategoryDataGridView.AllowUserToResizeColumns = false;
            this.dgvNormalSickLeaveCategoryDataGridView.AllowUserToResizeRows = false;
            this.dgvNormalSickLeaveCategoryDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvNormalSickLeaveCategoryDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvNormalSickLeaveCategoryDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvNormalSickLeaveCategoryDataGridView.ColumnHeadersHeight = 20;
            this.dgvNormalSickLeaveCategoryDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvNormalSickLeaveCategoryDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn4});
            this.dgvNormalSickLeaveCategoryDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvNormalSickLeaveCategoryDataGridView.EnableHeadersVisualStyles = false;
            this.dgvNormalSickLeaveCategoryDataGridView.Location = new System.Drawing.Point(8, 26);
            this.dgvNormalSickLeaveCategoryDataGridView.MultiSelect = false;
            this.dgvNormalSickLeaveCategoryDataGridView.Name = "dgvNormalSickLeaveCategoryDataGridView";
            this.dgvNormalSickLeaveCategoryDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvNormalSickLeaveCategoryDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvNormalSickLeaveCategoryDataGridView.RowHeadersWidth = 20;
            this.dgvNormalSickLeaveCategoryDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvNormalSickLeaveCategoryDataGridView.RowTemplate.Height = 19;
            this.dgvNormalSickLeaveCategoryDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvNormalSickLeaveCategoryDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNormalSickLeaveCategoryDataGridView.ShowCellToolTips = false;
            this.dgvNormalSickLeaveCategoryDataGridView.ShowEditingIcon = false;
            this.dgvNormalSickLeaveCategoryDataGridView.ShowRowErrors = false;
            this.dgvNormalSickLeaveCategoryDataGridView.Size = new System.Drawing.Size(329, 212);
            this.dgvNormalSickLeaveCategoryDataGridView.TabIndex = 345;
            this.dgvNormalSickLeaveCategoryDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNormalSickLeaveCategoryDataGridView_RowEnter);
            this.dgvNormalSickLeaveCategoryDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
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
            this.dataGridViewTextBoxColumn2.Width = 270;
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
            this.grbLeaveLock.Location = new System.Drawing.Point(346, 90);
            this.grbLeaveLock.Name = "grbLeaveLock";
            this.grbLeaveLock.Size = new System.Drawing.Size(190, 77);
            this.grbLeaveLock.TabIndex = 346;
            this.grbLeaveLock.TabStop = false;
            this.grbLeaveLock.Text = "Record Lock";
            this.grbLeaveLock.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::InteractPayroll.Properties.Resources.NewLock48;
            this.pictureBox1.Location = new System.Drawing.Point(8, 19);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // lblLeaveTypeLockDesc
            // 
            this.lblLeaveTypeLockDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeaveTypeLockDesc.ForeColor = System.Drawing.Color.Black;
            this.lblLeaveTypeLockDesc.Location = new System.Drawing.Point(69, 11);
            this.lblLeaveTypeLockDesc.Name = "lblLeaveTypeLockDesc";
            this.lblLeaveTypeLockDesc.Size = new System.Drawing.Size(107, 60);
            this.lblLeaveTypeLockDesc.TabIndex = 0;
            this.lblLeaveTypeLockDesc.Text = "Leave Records are Locked Due to Current Wage Run.";
            this.lblLeaveTypeLockDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picLeaveLock
            // 
            this.picLeaveLock.BackColor = System.Drawing.SystemColors.Control;
            this.picLeaveLock.Image = global::InteractPayroll.Properties.Resources.NewLock16;
            this.picLeaveLock.Location = new System.Drawing.Point(11, 29);
            this.picLeaveLock.Name = "picLeaveLock";
            this.picLeaveLock.Size = new System.Drawing.Size(16, 16);
            this.picLeaveLock.TabIndex = 244;
            this.picLeaveLock.TabStop = false;
            this.picLeaveLock.Visible = false;
            // 
            // picPayrollTypeLock
            // 
            this.picPayrollTypeLock.BackColor = System.Drawing.SystemColors.Control;
            this.picPayrollTypeLock.Image = global::InteractPayroll.Properties.Resources.NewLock16;
            this.picPayrollTypeLock.Location = new System.Drawing.Point(350, 29);
            this.picPayrollTypeLock.Name = "picPayrollTypeLock";
            this.picPayrollTypeLock.Size = new System.Drawing.Size(16, 16);
            this.picPayrollTypeLock.TabIndex = 279;
            this.picPayrollTypeLock.TabStop = false;
            this.picPayrollTypeLock.Visible = false;
            // 
            // frmNormalSickLeave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(622, 453);
            this.Controls.Add(this.grbLeaveLock);
            this.Controls.Add(this.picLeaveLock);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox11);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grbShiftDetails);
            this.Controls.Add(this.grbWageDayOption);
            this.Controls.Add(this.picPayrollTypeLock);
            this.Controls.Add(this.lblPayrollType);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtLeave);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.dgvPayrollTypeDataGridView);
            this.Controls.Add(this.dgvNormalSickLeaveCategoryDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmNormalSickLeave";
            this.Text = "frmNormalSickLeave";
            this.Load += new System.EventHandler(this.frmNormalSickLeave_Load);
            this.groupBox11.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.grbShiftDetails.ResumeLayout(false);
            this.grbWageDayOption.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNormalSickLeaveCategoryDataGridView)).EndInit();
            this.grbLeaveLock.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLeaveLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPayrollTypeLock)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txtLeave;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.PictureBox picLeaveLock;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.PictureBox picPayrollTypeLock;
        private System.Windows.Forms.Label lblPayrollType;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtSickLeaveAccum;
        private System.Windows.Forms.Label lblSickDesc;
        private System.Windows.Forms.ComboBox cboSickPaid;
        private System.Windows.Forms.Label lblSickPaid;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtNormLeaveAccum;
        private System.Windows.Forms.Label lblNormalDesc;
        private System.Windows.Forms.ComboBox cboNormalPaid;
        private System.Windows.Forms.Label lblNormalPaid;
        private System.Windows.Forms.GroupBox grbShiftDetails;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.ComboBox cboMaxShifts;
        private System.Windows.Forms.ComboBox cboMinShiftMinutes;
        private System.Windows.Forms.ComboBox cboMinShiftHours;
        private System.Windows.Forms.Label lblMaxShifts;
        private System.Windows.Forms.Label lblMinShift;
        private System.Windows.Forms.GroupBox grbWageDayOption;
        private System.Windows.Forms.RadioButton rbnWeekDaySaturday;
        private System.Windows.Forms.RadioButton rbnAllDays;
        private System.Windows.Forms.RadioButton rbnWeekDays;
        private System.Windows.Forms.DataGridView dgvPayrollTypeDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridView dgvNormalSickLeaveCategoryDataGridView;
        private System.Windows.Forms.GroupBox grbLeaveLock;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblLeaveTypeLockDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    }
}

