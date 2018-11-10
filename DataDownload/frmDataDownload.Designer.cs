﻿namespace InteractPayroll
{
    partial class frmDataDownload
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDataDownload));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblSelectedCostCentreSpreadsheetHeader = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblCostCentreSpreadsheetHeader = new System.Windows.Forms.Label();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dgvCostCentreDataGridView = new System.Windows.Forms.DataGridView();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RecordIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCostCentreChosenDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.btnAddDeleted = new System.Windows.Forms.Button();
            this.lblCostCentreDelete = new System.Windows.Forms.Label();
            this.dgvCostCentreDeletedDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCostCentreDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCostCentreChosenDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCostCentreDeletedDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(1097, 36);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(81, 24);
            this.btnClose.TabIndex = 213;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblSelectedCostCentreSpreadsheetHeader
            // 
            this.lblSelectedCostCentreSpreadsheetHeader.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblSelectedCostCentreSpreadsheetHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSelectedCostCentreSpreadsheetHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedCostCentreSpreadsheetHeader.ForeColor = System.Drawing.Color.Black;
            this.lblSelectedCostCentreSpreadsheetHeader.Location = new System.Drawing.Point(430, 8);
            this.lblSelectedCostCentreSpreadsheetHeader.Name = "lblSelectedCostCentreSpreadsheetHeader";
            this.lblSelectedCostCentreSpreadsheetHeader.Size = new System.Drawing.Size(509, 20);
            this.lblSelectedCostCentreSpreadsheetHeader.TabIndex = 218;
            this.lblSelectedCostCentreSpreadsheetHeader.Text = "Selected Cost Centres to Synchronize";
            this.lblSelectedCostCentreSpreadsheetHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(1097, 6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(81, 24);
            this.btnOK.TabIndex = 212;
            this.btnOK.Text = "Synchronize";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblCostCentreSpreadsheetHeader
            // 
            this.lblCostCentreSpreadsheetHeader.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblCostCentreSpreadsheetHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCostCentreSpreadsheetHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCostCentreSpreadsheetHeader.ForeColor = System.Drawing.Color.Black;
            this.lblCostCentreSpreadsheetHeader.Location = new System.Drawing.Point(8, 8);
            this.lblCostCentreSpreadsheetHeader.Name = "lblCostCentreSpreadsheetHeader";
            this.lblCostCentreSpreadsheetHeader.Size = new System.Drawing.Size(334, 20);
            this.lblCostCentreSpreadsheetHeader.TabIndex = 217;
            this.lblCostCentreSpreadsheetHeader.Text = "List of Cost Centres to Synchronize";
            this.lblCostCentreSpreadsheetHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveAll.Location = new System.Drawing.Point(348, 260);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(76, 24);
            this.btnRemoveAll.TabIndex = 216;
            this.btnRemoveAll.Text = "Remove All";
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(348, 230);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(76, 24);
            this.btnRemove.TabIndex = 215;
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(348, 200);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(76, 24);
            this.btnAdd.TabIndex = 214;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // dgvCostCentreDataGridView
            // 
            this.dgvCostCentreDataGridView.AllowUserToAddRows = false;
            this.dgvCostCentreDataGridView.AllowUserToDeleteRows = false;
            this.dgvCostCentreDataGridView.AllowUserToResizeColumns = false;
            this.dgvCostCentreDataGridView.AllowUserToResizeRows = false;
            this.dgvCostCentreDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvCostCentreDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCostCentreDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCostCentreDataGridView.ColumnHeadersHeight = 20;
            this.dgvCostCentreDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvCostCentreDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Description,
            this.Column2,
            this.Column1,
            this.RecordIndex,
            this.Column3});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCostCentreDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCostCentreDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvCostCentreDataGridView.EnableHeadersVisualStyles = false;
            this.dgvCostCentreDataGridView.Location = new System.Drawing.Point(8, 26);
            this.dgvCostCentreDataGridView.MultiSelect = false;
            this.dgvCostCentreDataGridView.Name = "dgvCostCentreDataGridView";
            this.dgvCostCentreDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCostCentreDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCostCentreDataGridView.RowHeadersWidth = 20;
            this.dgvCostCentreDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvCostCentreDataGridView.RowTemplate.Height = 19;
            this.dgvCostCentreDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvCostCentreDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCostCentreDataGridView.ShowCellToolTips = false;
            this.dgvCostCentreDataGridView.ShowEditingIcon = false;
            this.dgvCostCentreDataGridView.ShowRowErrors = false;
            this.dgvCostCentreDataGridView.Size = new System.Drawing.Size(334, 364);
            this.dgvCostCentreDataGridView.TabIndex = 337;
            this.dgvCostCentreDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCostCentreDataGridView_RowEnter);
            this.dgvCostCentreDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            this.dgvCostCentreDataGridView.DoubleClick += new System.EventHandler(this.dgvCostCentreDataGridView_DoubleClick);
            // 
            // Description
            // 
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.Width = 195;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.Visible = false;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Type";
            this.Column1.Name = "Column1";
            // 
            // RecordIndex
            // 
            this.RecordIndex.HeaderText = "Column1";
            this.RecordIndex.Name = "RecordIndex";
            this.RecordIndex.Visible = false;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Column3";
            this.Column3.Name = "Column3";
            this.Column3.Visible = false;
            // 
            // dgvCostCentreChosenDataGridView
            // 
            this.dgvCostCentreChosenDataGridView.AllowUserToAddRows = false;
            this.dgvCostCentreChosenDataGridView.AllowUserToDeleteRows = false;
            this.dgvCostCentreChosenDataGridView.AllowUserToResizeColumns = false;
            this.dgvCostCentreChosenDataGridView.AllowUserToResizeRows = false;
            this.dgvCostCentreChosenDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvCostCentreChosenDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCostCentreChosenDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvCostCentreChosenDataGridView.ColumnHeadersHeight = 20;
            this.dgvCostCentreChosenDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvCostCentreChosenDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.Column4,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.Column5});
            this.dgvCostCentreChosenDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvCostCentreChosenDataGridView.EnableHeadersVisualStyles = false;
            this.dgvCostCentreChosenDataGridView.Location = new System.Drawing.Point(430, 26);
            this.dgvCostCentreChosenDataGridView.MultiSelect = false;
            this.dgvCostCentreChosenDataGridView.Name = "dgvCostCentreChosenDataGridView";
            this.dgvCostCentreChosenDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCostCentreChosenDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvCostCentreChosenDataGridView.RowHeadersWidth = 20;
            this.dgvCostCentreChosenDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvCostCentreChosenDataGridView.RowTemplate.Height = 19;
            this.dgvCostCentreChosenDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvCostCentreChosenDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCostCentreChosenDataGridView.ShowCellToolTips = false;
            this.dgvCostCentreChosenDataGridView.ShowEditingIcon = false;
            this.dgvCostCentreChosenDataGridView.ShowRowErrors = false;
            this.dgvCostCentreChosenDataGridView.Size = new System.Drawing.Size(509, 516);
            this.dgvCostCentreChosenDataGridView.TabIndex = 338;
            this.dgvCostCentreChosenDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCostCentreChosenDataGridView_RowEnter);
            this.dgvCostCentreChosenDataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgvCostCentreChosenDataGridView_SortCompare);
            this.dgvCostCentreChosenDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            this.dgvCostCentreChosenDataGridView.DoubleClick += new System.EventHandler(this.dgvCostCentreChosenDataGridView_DoubleClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Description";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 195;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Last Synchronize Date / Time";
            this.Column4.Name = "Column4";
            this.Column4.Width = 175;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Type";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Visible = false;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Column5";
            this.Column5.Name = "Column5";
            this.Column5.Visible = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(29, 84);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(32, 48);
            this.pictureBox4.TabIndex = 354;
            this.pictureBox4.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.pictureBox5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.pictureBox4);
            this.groupBox1.Controls.Add(this.pictureBox6);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.pictureBox7);
            this.groupBox1.Location = new System.Drawing.Point(947, 354);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 184);
            this.groupBox1.TabIndex = 357;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Synchronize Internet and Local Databases";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(146, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 366;
            this.label5.Text = "Fingerprints";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(146, 123);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 13);
            this.label8.TabIndex = 365;
            this.label8.Text = "Departments";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(146, 108);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 364;
            this.label9.Text = "Cost Centres";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(146, 93);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 363;
            this.label7.Text = "Companies";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(146, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 362;
            this.label4.Text = "Employees";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(146, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 361;
            this.label3.Text = "Users";
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
            this.pictureBox5.Location = new System.Drawing.Point(29, 45);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(32, 32);
            this.pictureBox5.TabIndex = 347;
            this.pictureBox5.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(64, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 30);
            this.label1.TabIndex = 349;
            this.label1.Text = "Internet Database";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(63, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 27);
            this.label6.TabIndex = 351;
            this.label6.Text = "Synchronize Data";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox6.Image")));
            this.pictureBox6.Location = new System.Drawing.Point(29, 141);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(32, 32);
            this.pictureBox6.TabIndex = 345;
            this.pictureBox6.TabStop = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(64, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 30);
            this.label2.TabIndex = 350;
            this.label2.Text = "Local Database";
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox7.Image")));
            this.pictureBox7.Location = new System.Drawing.Point(9, 29);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(32, 32);
            this.pictureBox7.TabIndex = 346;
            this.pictureBox7.TabStop = false;
            // 
            // btnAddDeleted
            // 
            this.btnAddDeleted.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddDeleted.Location = new System.Drawing.Point(348, 468);
            this.btnAddDeleted.Name = "btnAddDeleted";
            this.btnAddDeleted.Size = new System.Drawing.Size(76, 24);
            this.btnAddDeleted.TabIndex = 375;
            this.btnAddDeleted.Text = "Add";
            this.btnAddDeleted.Click += new System.EventHandler(this.btnAddDeleted_Click);
            // 
            // lblCostCentreDelete
            // 
            this.lblCostCentreDelete.BackColor = System.Drawing.Color.Orange;
            this.lblCostCentreDelete.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCostCentreDelete.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCostCentreDelete.ForeColor = System.Drawing.Color.Black;
            this.lblCostCentreDelete.Location = new System.Drawing.Point(8, 401);
            this.lblCostCentreDelete.Name = "lblCostCentreDelete";
            this.lblCostCentreDelete.Size = new System.Drawing.Size(334, 20);
            this.lblCostCentreDelete.TabIndex = 373;
            this.lblCostCentreDelete.Text = "List of Cost Centres to be Deleted (Local Machine)";
            this.lblCostCentreDelete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvCostCentreDeletedDataGridView
            // 
            this.dgvCostCentreDeletedDataGridView.AllowUserToAddRows = false;
            this.dgvCostCentreDeletedDataGridView.AllowUserToDeleteRows = false;
            this.dgvCostCentreDeletedDataGridView.AllowUserToResizeColumns = false;
            this.dgvCostCentreDeletedDataGridView.AllowUserToResizeRows = false;
            this.dgvCostCentreDeletedDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvCostCentreDeletedDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCostCentreDeletedDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvCostCentreDeletedDataGridView.ColumnHeadersHeight = 20;
            this.dgvCostCentreDeletedDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvCostCentreDeletedDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn9,
            this.Column6});
            this.dgvCostCentreDeletedDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvCostCentreDeletedDataGridView.EnableHeadersVisualStyles = false;
            this.dgvCostCentreDeletedDataGridView.Location = new System.Drawing.Point(8, 419);
            this.dgvCostCentreDeletedDataGridView.MultiSelect = false;
            this.dgvCostCentreDeletedDataGridView.Name = "dgvCostCentreDeletedDataGridView";
            this.dgvCostCentreDeletedDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCostCentreDeletedDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvCostCentreDeletedDataGridView.RowHeadersWidth = 20;
            this.dgvCostCentreDeletedDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvCostCentreDeletedDataGridView.RowTemplate.Height = 19;
            this.dgvCostCentreDeletedDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvCostCentreDeletedDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCostCentreDeletedDataGridView.ShowCellToolTips = false;
            this.dgvCostCentreDeletedDataGridView.ShowEditingIcon = false;
            this.dgvCostCentreDeletedDataGridView.ShowRowErrors = false;
            this.dgvCostCentreDeletedDataGridView.Size = new System.Drawing.Size(334, 117);
            this.dgvCostCentreDeletedDataGridView.StandardTab = true;
            this.dgvCostCentreDeletedDataGridView.TabIndex = 374;
            this.dgvCostCentreDeletedDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Description";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.Width = 195;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "Column3";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.Visible = false;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "Type";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.Visible = false;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Column6";
            this.Column6.Name = "Column6";
            this.Column6.Visible = false;
            // 
            // frmDataDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1185, 550);
            this.Controls.Add(this.btnAddDeleted);
            this.Controls.Add(this.lblCostCentreDelete);
            this.Controls.Add(this.dgvCostCentreDeletedDataGridView);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblSelectedCostCentreSpreadsheetHeader);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblCostCentreSpreadsheetHeader);
            this.Controls.Add(this.btnRemoveAll);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dgvCostCentreDataGridView);
            this.Controls.Add(this.dgvCostCentreChosenDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmDataDownload";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmDataDownload";
            this.Load += new System.EventHandler(this.frmDataDownload_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCostCentreDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCostCentreChosenDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCostCentreDeletedDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblSelectedCostCentreSpreadsheetHeader;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblCostCentreSpreadsheetHeader;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridView dgvCostCentreDataGridView;
        private System.Windows.Forms.DataGridView dgvCostCentreChosenDataGridView;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnAddDeleted;
        private System.Windows.Forms.Label lblCostCentreDelete;
        private System.Windows.Forms.DataGridView dgvCostCentreDeletedDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn RecordIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
    }
}
