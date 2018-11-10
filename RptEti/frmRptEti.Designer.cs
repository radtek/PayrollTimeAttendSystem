namespace InteractPayroll
{
    partial class frmRptEti
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.grbReportPeriod = new System.Windows.Forms.GroupBox();
            this.rbnPrevYear = new System.Windows.Forms.RadioButton();
            this.rbnYTD = new System.Windows.Forms.RadioButton();
            this.rbnMonthly = new System.Windows.Forms.RadioButton();
            this.lblChosenEmployee = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.lblEmployee = new System.Windows.Forms.Label();
            this.dgvChosenEmployeeDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dgvEmployeeDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbnPrintSurname = new System.Windows.Forms.RadioButton();
            this.rbnPrintCode = new System.Windows.Forms.RadioButton();
            this.rbnPrintName = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.grbEmployee = new System.Windows.Forms.GroupBox();
            this.rbnAllEmployees = new System.Windows.Forms.RadioButton();
            this.rbnSelectedEmployees = new System.Windows.Forms.RadioButton();
            this.lblRunDate = new System.Windows.Forms.Label();
            this.dgvDateDataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.tabControlMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.grbReportPeriod.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChosenEmployeeDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeDataGridView)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.grbEmployee.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDateDataGridView)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPage1);
            this.tabControlMain.Controls.Add(this.tabPage2);
            this.tabControlMain.Location = new System.Drawing.Point(8, 8);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(1078, 565);
            this.tabControlMain.TabIndex = 354;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.grbReportPeriod);
            this.tabPage1.Controls.Add(this.lblChosenEmployee);
            this.tabPage1.Controls.Add(this.btnClose);
            this.tabPage1.Controls.Add(this.btnRemoveAll);
            this.tabPage1.Controls.Add(this.lblEmployee);
            this.tabPage1.Controls.Add(this.dgvChosenEmployeeDataGridView);
            this.tabPage1.Controls.Add(this.btnAdd);
            this.tabPage1.Controls.Add(this.dgvEmployeeDataGridView);
            this.tabPage1.Controls.Add(this.btnAddAll);
            this.tabPage1.Controls.Add(this.btnRemove);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.btnOK);
            this.tabPage1.Controls.Add(this.grbEmployee);
            this.tabPage1.Controls.Add(this.lblRunDate);
            this.tabPage1.Controls.Add(this.dgvDateDataGridView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1070, 539);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Selection";
            // 
            // grbReportPeriod
            // 
            this.grbReportPeriod.Controls.Add(this.rbnPrevYear);
            this.grbReportPeriod.Controls.Add(this.rbnYTD);
            this.grbReportPeriod.Controls.Add(this.rbnMonthly);
            this.grbReportPeriod.Location = new System.Drawing.Point(256, 81);
            this.grbReportPeriod.Name = "grbReportPeriod";
            this.grbReportPeriod.Size = new System.Drawing.Size(122, 84);
            this.grbReportPeriod.TabIndex = 366;
            this.grbReportPeriod.TabStop = false;
            this.grbReportPeriod.Text = "Report Period";
            this.grbReportPeriod.Visible = false;
            // 
            // rbnPrevYear
            // 
            this.rbnPrevYear.Location = new System.Drawing.Point(6, 60);
            this.rbnPrevYear.Name = "rbnPrevYear";
            this.rbnPrevYear.Size = new System.Drawing.Size(100, 16);
            this.rbnPrevYear.TabIndex = 350;
            this.rbnPrevYear.Text = "Previous Year";
            // 
            // rbnYTD
            // 
            this.rbnYTD.Location = new System.Drawing.Point(6, 40);
            this.rbnYTD.Name = "rbnYTD";
            this.rbnYTD.Size = new System.Drawing.Size(88, 16);
            this.rbnYTD.TabIndex = 28;
            this.rbnYTD.Text = "Year to Date";
            // 
            // rbnMonthly
            // 
            this.rbnMonthly.Checked = true;
            this.rbnMonthly.Location = new System.Drawing.Point(6, 20);
            this.rbnMonthly.Name = "rbnMonthly";
            this.rbnMonthly.Size = new System.Drawing.Size(64, 16);
            this.rbnMonthly.TabIndex = 25;
            this.rbnMonthly.TabStop = true;
            this.rbnMonthly.Text = "Monthly";
            // 
            // lblChosenEmployee
            // 
            this.lblChosenEmployee.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblChosenEmployee.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblChosenEmployee.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChosenEmployee.ForeColor = System.Drawing.Color.Black;
            this.lblChosenEmployee.Location = new System.Drawing.Point(576, 173);
            this.lblChosenEmployee.Name = "lblChosenEmployee";
            this.lblChosenEmployee.Size = new System.Drawing.Size(484, 20);
            this.lblChosenEmployee.TabIndex = 218;
            this.lblChosenEmployee.Text = "Selected Employees";
            this.lblChosenEmployee.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(988, 38);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 365;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Enabled = false;
            this.btnRemoveAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveAll.Location = new System.Drawing.Point(498, 347);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(72, 24);
            this.btnRemoveAll.TabIndex = 215;
            this.btnRemoveAll.Text = "Remove All";
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // lblEmployee
            // 
            this.lblEmployee.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblEmployee.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEmployee.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmployee.ForeColor = System.Drawing.Color.Black;
            this.lblEmployee.Location = new System.Drawing.Point(8, 173);
            this.lblEmployee.Name = "lblEmployee";
            this.lblEmployee.Size = new System.Drawing.Size(484, 20);
            this.lblEmployee.TabIndex = 219;
            this.lblEmployee.Text = "List of Employees";
            this.lblEmployee.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvChosenEmployeeDataGridView
            // 
            this.dgvChosenEmployeeDataGridView.AllowUserToAddRows = false;
            this.dgvChosenEmployeeDataGridView.AllowUserToDeleteRows = false;
            this.dgvChosenEmployeeDataGridView.AllowUserToResizeColumns = false;
            this.dgvChosenEmployeeDataGridView.AllowUserToResizeRows = false;
            this.dgvChosenEmployeeDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvChosenEmployeeDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvChosenEmployeeDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvChosenEmployeeDataGridView.ColumnHeadersHeight = 20;
            this.dgvChosenEmployeeDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvChosenEmployeeDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8});
            this.dgvChosenEmployeeDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvChosenEmployeeDataGridView.EnableHeadersVisualStyles = false;
            this.dgvChosenEmployeeDataGridView.Location = new System.Drawing.Point(576, 191);
            this.dgvChosenEmployeeDataGridView.MultiSelect = false;
            this.dgvChosenEmployeeDataGridView.Name = "dgvChosenEmployeeDataGridView";
            this.dgvChosenEmployeeDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvChosenEmployeeDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvChosenEmployeeDataGridView.RowHeadersWidth = 20;
            this.dgvChosenEmployeeDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvChosenEmployeeDataGridView.RowTemplate.Height = 19;
            this.dgvChosenEmployeeDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvChosenEmployeeDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChosenEmployeeDataGridView.ShowCellToolTips = false;
            this.dgvChosenEmployeeDataGridView.ShowEditingIcon = false;
            this.dgvChosenEmployeeDataGridView.ShowRowErrors = false;
            this.dgvChosenEmployeeDataGridView.Size = new System.Drawing.Size(484, 250);
            this.dgvChosenEmployeeDataGridView.TabIndex = 352;
            this.dgvChosenEmployeeDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvChosenEmployeeDataGridView_RowEnter);
            this.dgvChosenEmployeeDataGridView.DoubleClick += new System.EventHandler(this.dgvChosenEmployeeDataGridView_DoubleClick);
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Code";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 85;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Surname";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.Width = 180;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "Name";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.Width = 180;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.dataGridViewTextBoxColumn8.Visible = false;
            this.dataGridViewTextBoxColumn8.Width = 180;
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(498, 255);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(72, 24);
            this.btnAdd.TabIndex = 216;
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
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn4,
            this.Column2,
            this.dataGridViewTextBoxColumn2});
            this.dgvEmployeeDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvEmployeeDataGridView.EnableHeadersVisualStyles = false;
            this.dgvEmployeeDataGridView.Location = new System.Drawing.Point(8, 191);
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
            this.dgvEmployeeDataGridView.Size = new System.Drawing.Size(484, 250);
            this.dgvEmployeeDataGridView.TabIndex = 351;
            this.dgvEmployeeDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEmployeeDataGridView_RowEnter);
            this.dgvEmployeeDataGridView.DoubleClick += new System.EventHandler(this.dgvEmployeeDataGridView_DoubleClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Code";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 85;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Surname";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 180;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Name";
            this.Column2.Name = "Column2";
            this.Column2.Width = 180;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.dataGridViewTextBoxColumn2.Visible = false;
            this.dataGridViewTextBoxColumn2.Width = 180;
            // 
            // btnAddAll
            // 
            this.btnAddAll.Enabled = false;
            this.btnAddAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddAll.Location = new System.Drawing.Point(498, 287);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(72, 24);
            this.btnAddAll.TabIndex = 214;
            this.btnAddAll.Text = "Add All";
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(498, 317);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(72, 24);
            this.btnRemove.TabIndex = 217;
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbnPrintSurname);
            this.groupBox4.Controls.Add(this.rbnPrintCode);
            this.groupBox4.Controls.Add(this.rbnPrintName);
            this.groupBox4.Location = new System.Drawing.Point(938, 81);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(122, 84);
            this.groupBox4.TabIndex = 212;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Print Order";
            // 
            // rbnPrintSurname
            // 
            this.rbnPrintSurname.Location = new System.Drawing.Point(6, 60);
            this.rbnPrintSurname.Name = "rbnPrintSurname";
            this.rbnPrintSurname.Size = new System.Drawing.Size(104, 16);
            this.rbnPrintSurname.TabIndex = 2;
            this.rbnPrintSurname.Text = "Surname";
            // 
            // rbnPrintCode
            // 
            this.rbnPrintCode.Checked = true;
            this.rbnPrintCode.Location = new System.Drawing.Point(6, 20);
            this.rbnPrintCode.Name = "rbnPrintCode";
            this.rbnPrintCode.Size = new System.Drawing.Size(104, 16);
            this.rbnPrintCode.TabIndex = 1;
            this.rbnPrintCode.TabStop = true;
            this.rbnPrintCode.Text = "Employee Code";
            // 
            // rbnPrintName
            // 
            this.rbnPrintName.Location = new System.Drawing.Point(6, 40);
            this.rbnPrintName.Name = "rbnPrintName";
            this.rbnPrintName.Size = new System.Drawing.Size(104, 16);
            this.rbnPrintName.TabIndex = 0;
            this.rbnPrintName.Text = "Name";
            // 
            // btnOK
            // 
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(988, 8);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 24);
            this.btnOK.TabIndex = 223;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grbEmployee
            // 
            this.grbEmployee.Controls.Add(this.rbnAllEmployees);
            this.grbEmployee.Controls.Add(this.rbnSelectedEmployees);
            this.grbEmployee.Location = new System.Drawing.Point(8, 450);
            this.grbEmployee.Name = "grbEmployee";
            this.grbEmployee.Size = new System.Drawing.Size(238, 79);
            this.grbEmployee.TabIndex = 211;
            this.grbEmployee.TabStop = false;
            this.grbEmployee.Text = "Employee";
            // 
            // rbnAllEmployees
            // 
            this.rbnAllEmployees.Checked = true;
            this.rbnAllEmployees.Location = new System.Drawing.Point(8, 23);
            this.rbnAllEmployees.Name = "rbnAllEmployees";
            this.rbnAllEmployees.Size = new System.Drawing.Size(56, 20);
            this.rbnAllEmployees.TabIndex = 25;
            this.rbnAllEmployees.TabStop = true;
            this.rbnAllEmployees.Text = "All";
            this.rbnAllEmployees.Click += new System.EventHandler(this.rbnAllEmployees_Click);
            // 
            // rbnSelectedEmployees
            // 
            this.rbnSelectedEmployees.Location = new System.Drawing.Point(8, 52);
            this.rbnSelectedEmployees.Name = "rbnSelectedEmployees";
            this.rbnSelectedEmployees.Size = new System.Drawing.Size(65, 20);
            this.rbnSelectedEmployees.TabIndex = 26;
            this.rbnSelectedEmployees.Text = "Select";
            this.rbnSelectedEmployees.Click += new System.EventHandler(this.rbnSelectedEmployees_Click);
            // 
            // lblRunDate
            // 
            this.lblRunDate.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblRunDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRunDate.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRunDate.ForeColor = System.Drawing.Color.Black;
            this.lblRunDate.Location = new System.Drawing.Point(8, 8);
            this.lblRunDate.Name = "lblRunDate";
            this.lblRunDate.Size = new System.Drawing.Size(239, 20);
            this.lblRunDate.TabIndex = 213;
            this.lblRunDate.Text = "ETI Run Date";
            this.lblRunDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvDateDataGridView
            // 
            this.dgvDateDataGridView.AllowUserToAddRows = false;
            this.dgvDateDataGridView.AllowUserToDeleteRows = false;
            this.dgvDateDataGridView.AllowUserToResizeColumns = false;
            this.dgvDateDataGridView.AllowUserToResizeRows = false;
            this.dgvDateDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvDateDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDateDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvDateDataGridView.ColumnHeadersHeight = 20;
            this.dgvDateDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvDateDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.dataGridViewTextBoxColumn5});
            this.dgvDateDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvDateDataGridView.EnableHeadersVisualStyles = false;
            this.dgvDateDataGridView.Location = new System.Drawing.Point(8, 26);
            this.dgvDateDataGridView.MultiSelect = false;
            this.dgvDateDataGridView.Name = "dgvDateDataGridView";
            this.dgvDateDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDateDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvDateDataGridView.RowHeadersWidth = 20;
            this.dgvDateDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvDateDataGridView.RowTemplate.Height = 19;
            this.dgvDateDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvDateDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDateDataGridView.ShowCellToolTips = false;
            this.dgvDateDataGridView.ShowEditingIcon = false;
            this.dgvDateDataGridView.ShowRowErrors = false;
            this.dgvDateDataGridView.Size = new System.Drawing.Size(239, 136);
            this.dgvDateDataGridView.TabIndex = 350;
            this.dgvDateDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDateDataGridView_RowEnter);
            this.dgvDateDataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgvDateDataGridView_SortCompare);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Description";
            this.Column1.Name = "Column1";
            this.Column1.Width = 200;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.dataGridViewTextBoxColumn5.Visible = false;
            this.dataGridViewTextBoxColumn5.Width = 180;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.reportViewer);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1070, 539);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Report";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // reportViewer
            // 
            this.reportViewer.LocalReport.ReportEmbeddedResource = "InteractPayroll.Report.rdlc";
            this.reportViewer.Location = new System.Drawing.Point(6, 8);
            this.reportViewer.Name = "reportViewer";
            this.reportViewer.Size = new System.Drawing.Size(1058, 525);
            this.reportViewer.TabIndex = 0;
            // 
            // frmRptEti
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1093, 582);
            this.Controls.Add(this.tabControlMain);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmRptEti";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmRptEti_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.grbReportPeriod.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChosenEmployeeDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeeDataGridView)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.grbEmployee.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDateDataGridView)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblChosenEmployee;
        private System.Windows.Forms.Label lblEmployee;
        private System.Windows.Forms.DataGridView dgvEmployeeDataGridView;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.DataGridView dgvChosenEmployeeDataGridView;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbnPrintSurname;
        private System.Windows.Forms.RadioButton rbnPrintCode;
        private System.Windows.Forms.RadioButton rbnPrintName;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox grbEmployee;
        private System.Windows.Forms.RadioButton rbnAllEmployees;
        private System.Windows.Forms.RadioButton rbnSelectedEmployees;
        private System.Windows.Forms.Label lblRunDate;
        private System.Windows.Forms.DataGridView dgvDateDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer;
        private System.Windows.Forms.GroupBox grbReportPeriod;
        private System.Windows.Forms.RadioButton rbnPrevYear;
        private System.Windows.Forms.RadioButton rbnYTD;
        private System.Windows.Forms.RadioButton rbnMonthly;
    }
}

