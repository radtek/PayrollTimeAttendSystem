namespace InteractPayroll
{
    partial class frmClosePayrollRun
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmClosePayrollRun));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lblSelectedCostCentre = new System.Windows.Forms.Label();
            this.lblCostCentre = new System.Windows.Forms.Label();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDateClose = new System.Windows.Forms.Button();
            this.grbActivationProcess = new System.Windows.Forms.GroupBox();
            this.pnlDatabaseBackupBefore = new System.Windows.Forms.Panel();
            this.lblDatabaseBackupBeforeClose = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.picBackupBefore = new System.Windows.Forms.PictureBox();
            this.grbCloseReminder = new System.Windows.Forms.GroupBox();
            this.picWarningPicture = new System.Windows.Forms.PictureBox();
            this.lblCloseInfo = new System.Windows.Forms.Label();
            this.tmrTimer = new System.Windows.Forms.Timer(this.components);
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvPayCategoryDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvChosenPayCategoryDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grbStatus = new System.Windows.Forms.GroupBox();
            this.pnlImage = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.picClosePayrollRun = new System.Windows.Forms.PictureBox();
            this.picStatus = new System.Windows.Forms.PictureBox();
            this.lblRunStatus = new System.Windows.Forms.Label();
            this.timerRun = new System.Windows.Forms.Timer(this.components);
            this.grbActivationProcess.SuspendLayout();
            this.pnlDatabaseBackupBefore.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackupBefore)).BeginInit();
            this.grbCloseReminder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWarningPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayCategoryDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChosenPayCategoryDataGridView)).BeginInit();
            this.grbStatus.SuspendLayout();
            this.pnlImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picClosePayrollRun)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(876, 83);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 24);
            this.btnCancel.TabIndex = 223;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Location = new System.Drawing.Point(876, 53);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(72, 24);
            this.btnUpdate.TabIndex = 222;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // lblSelectedCostCentre
            // 
            this.lblSelectedCostCentre.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblSelectedCostCentre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSelectedCostCentre.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedCostCentre.ForeColor = System.Drawing.Color.Black;
            this.lblSelectedCostCentre.Location = new System.Drawing.Point(516, 144);
            this.lblSelectedCostCentre.Name = "lblSelectedCostCentre";
            this.lblSelectedCostCentre.Size = new System.Drawing.Size(432, 20);
            this.lblSelectedCostCentre.TabIndex = 221;
            this.lblSelectedCostCentre.Text = "Selected Cost Centres";
            this.lblSelectedCostCentre.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCostCentre
            // 
            this.lblCostCentre.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblCostCentre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCostCentre.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCostCentre.ForeColor = System.Drawing.Color.Black;
            this.lblCostCentre.Location = new System.Drawing.Point(8, 144);
            this.lblCostCentre.Name = "lblCostCentre";
            this.lblCostCentre.Size = new System.Drawing.Size(432, 20);
            this.lblCostCentre.TabIndex = 219;
            this.lblCostCentre.Text = "List of Cost Centres";
            this.lblCostCentre.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAddAll
            // 
            this.btnAddAll.Enabled = false;
            this.btnAddAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddAll.Location = new System.Drawing.Point(442, 282);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(72, 24);
            this.btnAddAll.TabIndex = 216;
            this.btnAddAll.Text = "Add All";
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Enabled = false;
            this.btnRemoveAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveAll.Location = new System.Drawing.Point(442, 312);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(72, 24);
            this.btnRemoveAll.TabIndex = 217;
            this.btnRemoveAll.Text = "Remove All";
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(876, 113);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 214;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDateClose
            // 
            this.btnDateClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDateClose.Enabled = false;
            this.btnDateClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDateClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDateClose.ForeColor = System.Drawing.Color.Black;
            this.btnDateClose.Location = new System.Drawing.Point(876, 8);
            this.btnDateClose.Name = "btnDateClose";
            this.btnDateClose.Size = new System.Drawing.Size(72, 39);
            this.btnDateClose.TabIndex = 215;
            this.btnDateClose.Text = "Close Payroll Date";
            this.btnDateClose.Click += new System.EventHandler(this.btnDateClose_Click);
            // 
            // grbActivationProcess
            // 
            this.grbActivationProcess.Controls.Add(this.pnlDatabaseBackupBefore);
            this.grbActivationProcess.Location = new System.Drawing.Point(516, 3);
            this.grbActivationProcess.Name = "grbActivationProcess";
            this.grbActivationProcess.Size = new System.Drawing.Size(354, 133);
            this.grbActivationProcess.TabIndex = 290;
            this.grbActivationProcess.TabStop = false;
            this.grbActivationProcess.Text = "Close Run Database Backup Process";
            this.grbActivationProcess.Visible = false;
            // 
            // pnlDatabaseBackupBefore
            // 
            this.pnlDatabaseBackupBefore.Controls.Add(this.lblDatabaseBackupBeforeClose);
            this.pnlDatabaseBackupBefore.Controls.Add(this.pictureBox1);
            this.pnlDatabaseBackupBefore.Controls.Add(this.picBackupBefore);
            this.pnlDatabaseBackupBefore.Location = new System.Drawing.Point(6, 15);
            this.pnlDatabaseBackupBefore.Name = "pnlDatabaseBackupBefore";
            this.pnlDatabaseBackupBefore.Size = new System.Drawing.Size(342, 110);
            this.pnlDatabaseBackupBefore.TabIndex = 290;
            // 
            // lblDatabaseBackupBeforeClose
            // 
            this.lblDatabaseBackupBeforeClose.ForeColor = System.Drawing.Color.Red;
            this.lblDatabaseBackupBeforeClose.Location = new System.Drawing.Point(1, 56);
            this.lblDatabaseBackupBeforeClose.Name = "lblDatabaseBackupBeforeClose";
            this.lblDatabaseBackupBeforeClose.Size = new System.Drawing.Size(339, 51);
            this.lblDatabaseBackupBeforeClose.TabIndex = 284;
            this.lblDatabaseBackupBeforeClose.Text = "Busy with Database Backup ...";
            this.lblDatabaseBackupBeforeClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ClosePayrollRun.Properties.Resources.database_export;
            this.pictureBox1.Location = new System.Drawing.Point(119, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 283;
            this.pictureBox1.TabStop = false;
            // 
            // picBackupBefore
            // 
            this.picBackupBefore.Image = global::ClosePayrollRun.Properties.Resources.Question48;
            this.picBackupBefore.Location = new System.Drawing.Point(171, 6);
            this.picBackupBefore.Name = "picBackupBefore";
            this.picBackupBefore.Size = new System.Drawing.Size(48, 48);
            this.picBackupBefore.TabIndex = 290;
            this.picBackupBefore.TabStop = false;
            // 
            // grbCloseReminder
            // 
            this.grbCloseReminder.Controls.Add(this.picWarningPicture);
            this.grbCloseReminder.Controls.Add(this.lblCloseInfo);
            this.grbCloseReminder.Location = new System.Drawing.Point(8, 3);
            this.grbCloseReminder.Name = "grbCloseReminder";
            this.grbCloseReminder.Size = new System.Drawing.Size(432, 133);
            this.grbCloseReminder.TabIndex = 291;
            this.grbCloseReminder.TabStop = false;
            this.grbCloseReminder.Text = "Close Payroll Run Warning";
            // 
            // picWarningPicture
            // 
            this.picWarningPicture.Image = global::ClosePayrollRun.Properties.Resources.PayrollTotals48;
            this.picWarningPicture.Location = new System.Drawing.Point(12, 47);
            this.picWarningPicture.Name = "picWarningPicture";
            this.picWarningPicture.Size = new System.Drawing.Size(51, 50);
            this.picWarningPicture.TabIndex = 282;
            this.picWarningPicture.TabStop = false;
            // 
            // lblCloseInfo
            // 
            this.lblCloseInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCloseInfo.ForeColor = System.Drawing.Color.Black;
            this.lblCloseInfo.Location = new System.Drawing.Point(69, 39);
            this.lblCloseInfo.Name = "lblCloseInfo";
            this.lblCloseInfo.Size = new System.Drawing.Size(306, 65);
            this.lblCloseInfo.TabIndex = 0;
            this.lblCloseInfo.Text = "Make sure that you have Checked all Employee Payroll Totals before you Proceed.";
            this.lblCloseInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmrTimer
            // 
            this.tmrTimer.Interval = 1000;
            this.tmrTimer.Tick += new System.EventHandler(this.tmrTimer_Tick);
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPayCategoryDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPayCategoryDataGridView.ColumnHeadersHeight = 20;
            this.dgvPayCategoryDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvPayCategoryDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn4,
            this.Column3,
            this.Column1,
            this.Column2,
            this.dataGridViewTextBoxColumn5});
            this.dgvPayCategoryDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvPayCategoryDataGridView.EnableHeadersVisualStyles = false;
            this.dgvPayCategoryDataGridView.Location = new System.Drawing.Point(8, 162);
            this.dgvPayCategoryDataGridView.MultiSelect = false;
            this.dgvPayCategoryDataGridView.Name = "dgvPayCategoryDataGridView";
            this.dgvPayCategoryDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPayCategoryDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPayCategoryDataGridView.RowHeadersWidth = 20;
            this.dgvPayCategoryDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvPayCategoryDataGridView.RowTemplate.Height = 19;
            this.dgvPayCategoryDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvPayCategoryDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPayCategoryDataGridView.ShowCellToolTips = false;
            this.dgvPayCategoryDataGridView.ShowEditingIcon = false;
            this.dgvPayCategoryDataGridView.ShowRowErrors = false;
            this.dgvPayCategoryDataGridView.Size = new System.Drawing.Size(432, 326);
            this.dgvPayCategoryDataGridView.TabIndex = 344;
            this.dgvPayCategoryDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPayCategoryDataGridView_RowEnter);
            this.dgvPayCategoryDataGridView.DoubleClick += new System.EventHandler(this.dgvPayCategoryDataGridView_DoubleClick);
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Description";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn4.Width = 210;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "#";
            this.Column3.Name = "Column3";
            this.Column3.Width = 20;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Run Date";
            this.Column1.Name = "Column1";
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.Width = 75;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Prev. Run Date";
            this.Column2.Name = "Column2";
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column2.Width = 88;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Visible = false;
            // 
            // dgvChosenPayCategoryDataGridView
            // 
            this.dgvChosenPayCategoryDataGridView.AllowUserToAddRows = false;
            this.dgvChosenPayCategoryDataGridView.AllowUserToDeleteRows = false;
            this.dgvChosenPayCategoryDataGridView.AllowUserToResizeColumns = false;
            this.dgvChosenPayCategoryDataGridView.AllowUserToResizeRows = false;
            this.dgvChosenPayCategoryDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvChosenPayCategoryDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvChosenPayCategoryDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvChosenPayCategoryDataGridView.ColumnHeadersHeight = 20;
            this.dgvChosenPayCategoryDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvChosenPayCategoryDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn6,
            this.Column4,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn9});
            this.dgvChosenPayCategoryDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvChosenPayCategoryDataGridView.EnableHeadersVisualStyles = false;
            this.dgvChosenPayCategoryDataGridView.Location = new System.Drawing.Point(516, 162);
            this.dgvChosenPayCategoryDataGridView.MultiSelect = false;
            this.dgvChosenPayCategoryDataGridView.Name = "dgvChosenPayCategoryDataGridView";
            this.dgvChosenPayCategoryDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvChosenPayCategoryDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvChosenPayCategoryDataGridView.RowHeadersWidth = 20;
            this.dgvChosenPayCategoryDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvChosenPayCategoryDataGridView.RowTemplate.Height = 19;
            this.dgvChosenPayCategoryDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvChosenPayCategoryDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChosenPayCategoryDataGridView.ShowCellToolTips = false;
            this.dgvChosenPayCategoryDataGridView.ShowEditingIcon = false;
            this.dgvChosenPayCategoryDataGridView.ShowRowErrors = false;
            this.dgvChosenPayCategoryDataGridView.Size = new System.Drawing.Size(432, 326);
            this.dgvChosenPayCategoryDataGridView.TabIndex = 345;
            this.dgvChosenPayCategoryDataGridView.DoubleClick += new System.EventHandler(this.dgvChosenPayCategoryDataGridView_DoubleClick);
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Description";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn6.Width = 210;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "#";
            this.Column4.Name = "Column4";
            this.Column4.Width = 20;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "Run Date";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn7.Width = 75;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "Prev. Run Date";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn8.Width = 88;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.Visible = false;
            // 
            // grbStatus
            // 
            this.grbStatus.Controls.Add(this.pnlImage);
            this.grbStatus.Location = new System.Drawing.Point(446, 342);
            this.grbStatus.Name = "grbStatus";
            this.grbStatus.Size = new System.Drawing.Size(432, 133);
            this.grbStatus.TabIndex = 346;
            this.grbStatus.TabStop = false;
            this.grbStatus.Text = "Status";
            this.grbStatus.Visible = false;
            // 
            // pnlImage
            // 
            this.pnlImage.Controls.Add(this.label2);
            this.pnlImage.Controls.Add(this.picClosePayrollRun);
            this.pnlImage.Controls.Add(this.picStatus);
            this.pnlImage.Controls.Add(this.lblRunStatus);
            this.pnlImage.Location = new System.Drawing.Point(6, 15);
            this.pnlImage.Name = "pnlImage";
            this.pnlImage.Size = new System.Drawing.Size(420, 110);
            this.pnlImage.TabIndex = 353;
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(27, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 48);
            this.label2.TabIndex = 30;
            this.label2.Text = "This is moved at Run Time Don\'t Worry";
            this.label2.Visible = false;
            // 
            // picClosePayrollRun
            // 
            this.picClosePayrollRun.Image = global::ClosePayrollRun.Properties.Resources.ClosePayroll48;
            this.picClosePayrollRun.Location = new System.Drawing.Point(165, 6);
            this.picClosePayrollRun.Name = "picClosePayrollRun";
            this.picClosePayrollRun.Size = new System.Drawing.Size(48, 48);
            this.picClosePayrollRun.TabIndex = 0;
            this.picClosePayrollRun.TabStop = false;
            // 
            // picStatus
            // 
            this.picStatus.Image = global::ClosePayrollRun.Properties.Resources.Cross48;
            this.picStatus.Location = new System.Drawing.Point(217, 6);
            this.picStatus.Name = "picStatus";
            this.picStatus.Size = new System.Drawing.Size(48, 48);
            this.picStatus.TabIndex = 2;
            this.picStatus.TabStop = false;
            // 
            // lblRunStatus
            // 
            this.lblRunStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRunStatus.ForeColor = System.Drawing.Color.Red;
            this.lblRunStatus.Location = new System.Drawing.Point(1, 56);
            this.lblRunStatus.Name = "lblRunStatus";
            this.lblRunStatus.Size = new System.Drawing.Size(418, 51);
            this.lblRunStatus.TabIndex = 1;
            this.lblRunStatus.Text = "Pending";
            this.lblRunStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerRun
            // 
            this.timerRun.Interval = 1000;
            this.timerRun.Tick += new System.EventHandler(this.timerRun_Tick);
            // 
            // frmClosePayrollRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(955, 495);
            this.Controls.Add(this.grbActivationProcess);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.grbStatus);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lblSelectedCostCentre);
            this.Controls.Add(this.lblCostCentre);
            this.Controls.Add(this.btnAddAll);
            this.Controls.Add(this.btnRemoveAll);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDateClose);
            this.Controls.Add(this.dgvPayCategoryDataGridView);
            this.Controls.Add(this.dgvChosenPayCategoryDataGridView);
            this.Controls.Add(this.grbCloseReminder);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmClosePayrollRun";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmClosePayrollRun_Load);
            this.grbActivationProcess.ResumeLayout(false);
            this.pnlDatabaseBackupBefore.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackupBefore)).EndInit();
            this.grbCloseReminder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picWarningPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayCategoryDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChosenPayCategoryDataGridView)).EndInit();
            this.grbStatus.ResumeLayout(false);
            this.pnlImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picClosePayrollRun)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label lblSelectedCostCentre;
        private System.Windows.Forms.Label lblCostCentre;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDateClose;
        private System.Windows.Forms.GroupBox grbActivationProcess;
        private System.Windows.Forms.Panel pnlDatabaseBackupBefore;
        private System.Windows.Forms.Label lblDatabaseBackupBeforeClose;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox picBackupBefore;
        private System.Windows.Forms.GroupBox grbCloseReminder;
        private System.Windows.Forms.PictureBox picWarningPicture;
        private System.Windows.Forms.Label lblCloseInfo;
        private System.Windows.Forms.Timer tmrTimer;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridView dgvPayCategoryDataGridView;
        private System.Windows.Forms.DataGridView dgvChosenPayCategoryDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.GroupBox grbStatus;
        private System.Windows.Forms.Panel pnlImage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox picClosePayrollRun;
        private System.Windows.Forms.PictureBox picStatus;
        private System.Windows.Forms.Label lblRunStatus;
        private System.Windows.Forms.Timer timerRun;
    }
}

