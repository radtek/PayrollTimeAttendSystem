namespace InteractPayroll
{
    partial class frmDataUpload
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDataUpload));
            this.grbRunTimeAttendance = new System.Windows.Forms.GroupBox();
            this.lblResetTimeAttendanceMenu = new System.Windows.Forms.Label();
            this.btnRunTimeAttendance = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblNoPayrollRunDateOpen = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCostCentreSpreadsheetHeader = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pgbProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblPayrollTypeSpreadsheetHeader = new System.Windows.Forms.Label();
            this.grbNoDateOpen = new System.Windows.Forms.GroupBox();
            this.lblOpenPayrollMenu = new System.Windows.Forms.Label();
            this.btnOpenPayrollRun = new System.Windows.Forms.Button();
            this.lblNoOpenPayCategory = new System.Windows.Forms.Label();
            this.dgvPayrollTypeDataGridView = new System.Windows.Forms.DataGridView();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvPayCategoryDataGridView = new System.Windows.Forms.DataGridView();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grbUploadProgress = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grbRunTimeAttendance.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.grbNoDateOpen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayCategoryDataGridView)).BeginInit();
            this.grbUploadProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbRunTimeAttendance
            // 
            this.grbRunTimeAttendance.Controls.Add(this.lblResetTimeAttendanceMenu);
            this.grbRunTimeAttendance.Controls.Add(this.btnRunTimeAttendance);
            this.grbRunTimeAttendance.Controls.Add(this.lblMessage);
            this.grbRunTimeAttendance.Location = new System.Drawing.Point(726, 316);
            this.grbRunTimeAttendance.Name = "grbRunTimeAttendance";
            this.grbRunTimeAttendance.Size = new System.Drawing.Size(240, 106);
            this.grbRunTimeAttendance.TabIndex = 226;
            this.grbRunTimeAttendance.TabStop = false;
            this.grbRunTimeAttendance.Text = "Make Data Upload Option Available";
            this.grbRunTimeAttendance.Visible = false;
            // 
            // lblResetTimeAttendanceMenu
            // 
            this.lblResetTimeAttendanceMenu.ForeColor = System.Drawing.Color.Black;
            this.lblResetTimeAttendanceMenu.Location = new System.Drawing.Point(5, 78);
            this.lblResetTimeAttendanceMenu.Name = "lblResetTimeAttendanceMenu";
            this.lblResetTimeAttendanceMenu.Size = new System.Drawing.Size(227, 19);
            this.lblResetTimeAttendanceMenu.TabIndex = 161;
            this.lblResetTimeAttendanceMenu.Text = "Click Icon above to go to Open Payroll Run.";
            this.lblResetTimeAttendanceMenu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRunTimeAttendance
            // 
            this.btnRunTimeAttendance.Image = global::InteractPayroll.Properties.Resources.RunTimeAttendance48;
            this.btnRunTimeAttendance.Location = new System.Drawing.Point(8, 21);
            this.btnRunTimeAttendance.Name = "btnRunTimeAttendance";
            this.btnRunTimeAttendance.Size = new System.Drawing.Size(54, 54);
            this.btnRunTimeAttendance.TabIndex = 158;
            this.btnRunTimeAttendance.UseVisualStyleBackColor = true;
            this.btnRunTimeAttendance.Click += new System.EventHandler(this.btnRunTimeAttendance_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.ForeColor = System.Drawing.Color.Black;
            this.lblMessage.Location = new System.Drawing.Point(68, 25);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(170, 44);
            this.lblMessage.TabIndex = 157;
            this.lblMessage.Text = "Go to                                              Run Time and Attendance Menu  " +
    "Click Reset button";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Lime;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(9, 19);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(18, 18);
            this.panel2.TabIndex = 12;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.panel3);
            this.groupBox3.Controls.Add(this.lblNoPayrollRunDateOpen);
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.panel2);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(726, 428);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(240, 106);
            this.groupBox3.TabIndex = 227;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Row Legend";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Red;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Location = new System.Drawing.Point(9, 78);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(18, 18);
            this.panel3.TabIndex = 16;
            // 
            // lblNoPayrollRunDateOpen
            // 
            this.lblNoPayrollRunDateOpen.AutoSize = true;
            this.lblNoPayrollRunDateOpen.Location = new System.Drawing.Point(29, 81);
            this.lblNoPayrollRunDateOpen.Name = "lblNoPayrollRunDateOpen";
            this.lblNoPayrollRunDateOpen.Size = new System.Drawing.Size(110, 13);
            this.lblNoPayrollRunDateOpen.TabIndex = 15;
            this.lblNoPayrollRunDateOpen.Text = "No Payroll Date Open";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Orange;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(9, 48);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(18, 18);
            this.panel1.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Time Attendance Run Already Processed";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Time Sheet Upload Available";
            // 
            // lblCostCentreSpreadsheetHeader
            // 
            this.lblCostCentreSpreadsheetHeader.BackColor = System.Drawing.Color.DimGray;
            this.lblCostCentreSpreadsheetHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCostCentreSpreadsheetHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCostCentreSpreadsheetHeader.ForeColor = System.Drawing.Color.Black;
            this.lblCostCentreSpreadsheetHeader.Location = new System.Drawing.Point(8, 94);
            this.lblCostCentreSpreadsheetHeader.Name = "lblCostCentreSpreadsheetHeader";
            this.lblCostCentreSpreadsheetHeader.Size = new System.Drawing.Size(709, 20);
            this.lblCostCentreSpreadsheetHeader.TabIndex = 225;
            this.lblCostCentreSpreadsheetHeader.Text = "Cost Centre";
            this.lblCostCentreSpreadsheetHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(896, 38);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 222;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(896, 6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 24);
            this.btnOK.TabIndex = 221;
            this.btnOK.Text = "Upload";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pgbProgressBar
            // 
            this.pgbProgressBar.Location = new System.Drawing.Point(7, 20);
            this.pgbProgressBar.Name = "pgbProgressBar";
            this.pgbProgressBar.Size = new System.Drawing.Size(499, 20);
            this.pgbProgressBar.TabIndex = 223;
            // 
            // lblPayrollTypeSpreadsheetHeader
            // 
            this.lblPayrollTypeSpreadsheetHeader.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblPayrollTypeSpreadsheetHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPayrollTypeSpreadsheetHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPayrollTypeSpreadsheetHeader.ForeColor = System.Drawing.Color.Black;
            this.lblPayrollTypeSpreadsheetHeader.Location = new System.Drawing.Point(8, 8);
            this.lblPayrollTypeSpreadsheetHeader.Name = "lblPayrollTypeSpreadsheetHeader";
            this.lblPayrollTypeSpreadsheetHeader.Size = new System.Drawing.Size(189, 20);
            this.lblPayrollTypeSpreadsheetHeader.TabIndex = 277;
            this.lblPayrollTypeSpreadsheetHeader.Text = "Type";
            this.lblPayrollTypeSpreadsheetHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grbNoDateOpen
            // 
            this.grbNoDateOpen.Controls.Add(this.lblOpenPayrollMenu);
            this.grbNoDateOpen.Controls.Add(this.btnOpenPayrollRun);
            this.grbNoDateOpen.Controls.Add(this.lblNoOpenPayCategory);
            this.grbNoDateOpen.Location = new System.Drawing.Point(726, 329);
            this.grbNoDateOpen.Name = "grbNoDateOpen";
            this.grbNoDateOpen.Size = new System.Drawing.Size(240, 106);
            this.grbNoDateOpen.TabIndex = 278;
            this.grbNoDateOpen.TabStop = false;
            this.grbNoDateOpen.Text = "Data Upload Option Un-Available";
            this.grbNoDateOpen.Visible = false;
            // 
            // lblOpenPayrollMenu
            // 
            this.lblOpenPayrollMenu.ForeColor = System.Drawing.Color.Black;
            this.lblOpenPayrollMenu.Location = new System.Drawing.Point(5, 78);
            this.lblOpenPayrollMenu.Name = "lblOpenPayrollMenu";
            this.lblOpenPayrollMenu.Size = new System.Drawing.Size(227, 19);
            this.lblOpenPayrollMenu.TabIndex = 160;
            this.lblOpenPayrollMenu.Text = "Click Icon above to go to Open Payroll Run.";
            this.lblOpenPayrollMenu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOpenPayrollRun
            // 
            this.btnOpenPayrollRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenPayrollRun.Image = global::InteractPayroll.Properties.Resources.OpenPayroll48;
            this.btnOpenPayrollRun.Location = new System.Drawing.Point(8, 21);
            this.btnOpenPayrollRun.Name = "btnOpenPayrollRun";
            this.btnOpenPayrollRun.Size = new System.Drawing.Size(54, 54);
            this.btnOpenPayrollRun.TabIndex = 159;
            this.btnOpenPayrollRun.UseVisualStyleBackColor = true;
            this.btnOpenPayrollRun.Click += new System.EventHandler(this.btnOpenPayrollRun_Click);
            // 
            // lblNoOpenPayCategory
            // 
            this.lblNoOpenPayCategory.ForeColor = System.Drawing.Color.Black;
            this.lblNoOpenPayCategory.Location = new System.Drawing.Point(76, 27);
            this.lblNoOpenPayCategory.Name = "lblNoOpenPayCategory";
            this.lblNoOpenPayCategory.Size = new System.Drawing.Size(148, 45);
            this.lblNoOpenPayCategory.TabIndex = 157;
            this.lblNoOpenPayCategory.Text = "There is currently No Payroll Date Open for this Company.";
            this.lblNoOpenPayCategory.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.Column7,
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn3});
            this.dgvPayrollTypeDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvPayrollTypeDataGridView.EnableHeadersVisualStyles = false;
            this.dgvPayrollTypeDataGridView.Location = new System.Drawing.Point(8, 26);
            this.dgvPayrollTypeDataGridView.MultiSelect = false;
            this.dgvPayrollTypeDataGridView.Name = "dgvPayrollTypeDataGridView";
            this.dgvPayrollTypeDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPayrollTypeDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvPayrollTypeDataGridView.RowHeadersWidth = 20;
            this.dgvPayrollTypeDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvPayrollTypeDataGridView.RowTemplate.Height = 19;
            this.dgvPayrollTypeDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvPayrollTypeDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPayrollTypeDataGridView.ShowCellToolTips = false;
            this.dgvPayrollTypeDataGridView.ShowEditingIcon = false;
            this.dgvPayrollTypeDataGridView.ShowRowErrors = false;
            this.dgvPayrollTypeDataGridView.Size = new System.Drawing.Size(189, 60);
            this.dgvPayrollTypeDataGridView.TabIndex = 342;
            this.dgvPayrollTypeDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPayrollTypeDataGridView_RowEnter);
            // 
            // Column7
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            this.Column7.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column7.HeaderText = "";
            this.Column7.Name = "Column7";
            this.Column7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column7.Width = 20;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Description";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.dataGridViewTextBoxColumn1.Width = 149;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Visible = false;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Visible = false;
            // 
            // dgvPayCategoryDataGridView
            // 
            this.dgvPayCategoryDataGridView.AllowUserToAddRows = false;
            this.dgvPayCategoryDataGridView.AllowUserToDeleteRows = false;
            this.dgvPayCategoryDataGridView.AllowUserToResizeColumns = false;
            this.dgvPayCategoryDataGridView.AllowUserToResizeRows = false;
            this.dgvPayCategoryDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvPayCategoryDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPayCategoryDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvPayCategoryDataGridView.ColumnHeadersHeight = 20;
            this.dgvPayCategoryDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvPayCategoryDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column8,
            this.dataGridViewTextBoxColumn4,
            this.Column1,
            this.Column3,
            this.Column2,
            this.dataGridViewTextBoxColumn5,
            this.Column4,
            this.Column5,
            this.Column6});
            this.dgvPayCategoryDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvPayCategoryDataGridView.EnableHeadersVisualStyles = false;
            this.dgvPayCategoryDataGridView.Location = new System.Drawing.Point(8, 112);
            this.dgvPayCategoryDataGridView.MultiSelect = false;
            this.dgvPayCategoryDataGridView.Name = "dgvPayCategoryDataGridView";
            this.dgvPayCategoryDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPayCategoryDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvPayCategoryDataGridView.RowHeadersWidth = 20;
            this.dgvPayCategoryDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvPayCategoryDataGridView.RowTemplate.Height = 19;
            this.dgvPayCategoryDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvPayCategoryDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPayCategoryDataGridView.ShowCellToolTips = false;
            this.dgvPayCategoryDataGridView.ShowEditingIcon = false;
            this.dgvPayCategoryDataGridView.ShowRowErrors = false;
            this.dgvPayCategoryDataGridView.Size = new System.Drawing.Size(709, 421);
            this.dgvPayCategoryDataGridView.TabIndex = 343;
            this.dgvPayCategoryDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPayCategoryDataGridView_RowEnter);
            this.dgvPayCategoryDataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgvPayCategoryDataGridView_SortCompare);
            this.dgvPayCategoryDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            // 
            // Column8
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Control;
            this.Column8.DefaultCellStyle = dataGridViewCellStyle5;
            this.Column8.HeaderText = "";
            this.Column8.Name = "Column8";
            this.Column8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column8.Width = 20;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Description";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 205;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Wage Date";
            this.Column1.Name = "Column1";
            this.Column1.Width = 110;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "TimeSheet / Break End Date";
            this.Column3.Name = "Column3";
            this.Column3.Width = 175;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Last Upload Date / Time";
            this.Column2.Name = "Column2";
            this.Column2.Width = 160;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Visible = false;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Column4";
            this.Column4.Name = "Column4";
            this.Column4.Visible = false;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Column5";
            this.Column5.Name = "Column5";
            this.Column5.Visible = false;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Column6";
            this.Column6.Name = "Column6";
            this.Column6.Visible = false;
            // 
            // grbUploadProgress
            // 
            this.grbUploadProgress.Controls.Add(this.pgbProgressBar);
            this.grbUploadProgress.Location = new System.Drawing.Point(203, 34);
            this.grbUploadProgress.Name = "grbUploadProgress";
            this.grbUploadProgress.Size = new System.Drawing.Size(514, 53);
            this.grbUploadProgress.TabIndex = 344;
            this.grbUploadProgress.TabStop = false;
            this.grbUploadProgress.Text = "Data Upload Progress";
            this.grbUploadProgress.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(38, 110);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 345;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(18, 23);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox2.TabIndex = 346;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(38, 39);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(32, 32);
            this.pictureBox3.TabIndex = 347;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(38, 74);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(32, 32);
            this.pictureBox4.TabIndex = 348;
            this.pictureBox4.TabStop = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(73, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 30);
            this.label3.TabIndex = 349;
            this.label3.Text = "Internet Database";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(73, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 30);
            this.label5.TabIndex = 350;
            this.label5.Text = "Local Database";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(72, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 27);
            this.label6.TabIndex = 351;
            this.label6.Text = "Upload Time Sheets";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox3);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.pictureBox2);
            this.groupBox1.Controls.Add(this.pictureBox4);
            this.groupBox1.Location = new System.Drawing.Point(726, 88);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 158);
            this.groupBox1.TabIndex = 352;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Upload";
            // 
            // frmDataUpload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(974, 541);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbUploadProgress);
            this.Controls.Add(this.grbNoDateOpen);
            this.Controls.Add(this.lblPayrollTypeSpreadsheetHeader);
            this.Controls.Add(this.grbRunTimeAttendance);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.lblCostCentreSpreadsheetHeader);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dgvPayrollTypeDataGridView);
            this.Controls.Add(this.dgvPayCategoryDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmDataUpload";
            this.Text = "frmDataUpload";
            this.Load += new System.EventHandler(this.frmDataUpload_Load);
            this.grbRunTimeAttendance.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.grbNoDateOpen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayCategoryDataGridView)).EndInit();
            this.grbUploadProgress.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbRunTimeAttendance;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblCostCentreSpreadsheetHeader;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ProgressBar pgbProgressBar;
        private System.Windows.Forms.Label lblPayrollTypeSpreadsheetHeader;
        private System.Windows.Forms.GroupBox grbNoDateOpen;
        private System.Windows.Forms.Label lblNoOpenPayCategory;
        private System.Windows.Forms.DataGridView dgvPayrollTypeDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridView dgvPayCategoryDataGridView;
        private System.Windows.Forms.Button btnRunTimeAttendance;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblNoPayrollRunDateOpen;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOpenPayrollRun;
        private System.Windows.Forms.GroupBox grbUploadProgress;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblOpenPayrollMenu;
        private System.Windows.Forms.Label lblResetTimeAttendanceMenu;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
    }
}

