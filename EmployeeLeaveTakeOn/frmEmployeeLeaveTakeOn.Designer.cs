namespace InteractPayroll
{
    partial class frmEmployeeLeaveTakeOn
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblSickLeave = new System.Windows.Forms.Label();
            this.lblNormalLeave = new System.Windows.Forms.Label();
            this.lblEmployee = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lblPayrollType = new System.Windows.Forms.Label();
            this.dgvPayrollTypeDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.picPayrollTypeLock = new System.Windows.Forms.PictureBox();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvEmployeesDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPayrollTypeLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSickLeave
            // 
            this.lblSickLeave.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblSickLeave.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSickLeave.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSickLeave.ForeColor = System.Drawing.Color.Black;
            this.lblSickLeave.Location = new System.Drawing.Point(748, 95);
            this.lblSickLeave.Name = "lblSickLeave";
            this.lblSickLeave.Size = new System.Drawing.Size(99, 20);
            this.lblSickLeave.TabIndex = 198;
            this.lblSickLeave.Text = "Sick Leave";
            this.lblSickLeave.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNormalLeave
            // 
            this.lblNormalLeave.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblNormalLeave.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNormalLeave.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNormalLeave.ForeColor = System.Drawing.Color.Black;
            this.lblNormalLeave.Location = new System.Drawing.Point(588, 95);
            this.lblNormalLeave.Name = "lblNormalLeave";
            this.lblNormalLeave.Size = new System.Drawing.Size(163, 20);
            this.lblNormalLeave.TabIndex = 197;
            this.lblNormalLeave.Text = "Normal Leave";
            this.lblNormalLeave.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEmployee
            // 
            this.lblEmployee.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblEmployee.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEmployee.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmployee.ForeColor = System.Drawing.Color.Black;
            this.lblEmployee.Location = new System.Drawing.Point(8, 95);
            this.lblEmployee.Name = "lblEmployee";
            this.lblEmployee.Size = new System.Drawing.Size(585, 20);
            this.lblEmployee.TabIndex = 195;
            this.lblEmployee.Text = "Employee";
            this.lblEmployee.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(853, 96);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 194;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(853, 66);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 24);
            this.btnCancel.TabIndex = 193;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(853, 36);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 24);
            this.btnSave.TabIndex = 192;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Location = new System.Drawing.Point(853, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(72, 24);
            this.btnUpdate.TabIndex = 191;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // lblPayrollType
            // 
            this.lblPayrollType.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblPayrollType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPayrollType.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPayrollType.ForeColor = System.Drawing.Color.Black;
            this.lblPayrollType.Location = new System.Drawing.Point(8, 8);
            this.lblPayrollType.Name = "lblPayrollType";
            this.lblPayrollType.Size = new System.Drawing.Size(189, 20);
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
            // picPayrollTypeLock
            // 
            this.picPayrollTypeLock.BackColor = System.Drawing.SystemColors.Control;
            this.picPayrollTypeLock.Image = global::EmployeeLeaveTakeOn.Properties.Resources.NewLock16;
            this.picPayrollTypeLock.Location = new System.Drawing.Point(11, 29);
            this.picPayrollTypeLock.Name = "picPayrollTypeLock";
            this.picPayrollTypeLock.Size = new System.Drawing.Size(16, 16);
            this.picPayrollTypeLock.TabIndex = 279;
            this.picPayrollTypeLock.TabStop = false;
            this.picPayrollTypeLock.Visible = false;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Visible = false;
            // 
            // dgvEmployeesDataGridView
            // 
            this.dgvEmployeesDataGridView.AllowUserToAddRows = false;
            this.dgvEmployeesDataGridView.AllowUserToDeleteRows = false;
            this.dgvEmployeesDataGridView.AllowUserToResizeColumns = false;
            this.dgvEmployeesDataGridView.AllowUserToResizeRows = false;
            this.dgvEmployeesDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvEmployeesDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployeesDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvEmployeesDataGridView.ColumnHeadersHeight = 20;
            this.dgvEmployeesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvEmployeesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn10,
            this.dataGridViewTextBoxColumn11,
            this.Column1,
            this.Column2,
            this.Column3});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.NullValue = null;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvEmployeesDataGridView.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvEmployeesDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dgvEmployeesDataGridView.EnableHeadersVisualStyles = false;
            this.dgvEmployeesDataGridView.Location = new System.Drawing.Point(8, 113);
            this.dgvEmployeesDataGridView.MultiSelect = false;
            this.dgvEmployeesDataGridView.Name = "dgvEmployeesDataGridView";
            this.dgvEmployeesDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEmployeesDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvEmployeesDataGridView.RowHeadersWidth = 20;
            this.dgvEmployeesDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvEmployeesDataGridView.RowTemplate.Height = 19;
            this.dgvEmployeesDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvEmployeesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployeesDataGridView.ShowCellToolTips = false;
            this.dgvEmployeesDataGridView.ShowRowErrors = false;
            this.dgvEmployeesDataGridView.Size = new System.Drawing.Size(839, 440);
            this.dgvEmployeesDataGridView.TabIndex = 344;
            this.dgvEmployeesDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEmployeesDataGridView_CellEndEdit);
            this.dgvEmployeesDataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvEmployeesDataGridView_EditingControlShowing);
            this.dgvEmployeesDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEmployeesDataGridView_RowEnter);
            this.dgvEmployeesDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Code";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 90;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "Surname";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.Width = 190;
            // 
            // dataGridViewTextBoxColumn8
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.NullValue = null;
            this.dataGridViewTextBoxColumn8.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn8.HeaderText = "Name";
            this.dataGridViewTextBoxColumn8.MaxInputLength = 10;
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            this.dataGridViewTextBoxColumn8.Width = 180;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.HeaderText = "Effective Date";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            this.dataGridViewTextBoxColumn10.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // dataGridViewTextBoxColumn11
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.dataGridViewTextBoxColumn11.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn11.HeaderText = "Previous Year";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.Width = 80;
            // 
            // Column1
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle6;
            this.Column1.HeaderText = "Current Year";
            this.Column1.Name = "Column1";
            this.Column1.Width = 80;
            // 
            // Column2
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle7;
            this.Column2.HeaderText = "Current Year";
            this.Column2.Name = "Column2";
            this.Column2.Width = 80;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Column3";
            this.Column3.Name = "Column3";
            this.Column3.Visible = false;
            // 
            // frmEmployeeLeaveTakeOn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(930, 562);
            this.Controls.Add(this.picPayrollTypeLock);
            this.Controls.Add(this.lblPayrollType);
            this.Controls.Add(this.lblSickLeave);
            this.Controls.Add(this.lblNormalLeave);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lblEmployee);
            this.Controls.Add(this.dgvPayrollTypeDataGridView);
            this.Controls.Add(this.dgvEmployeesDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmEmployeeLeaveTakeOn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmEmployeeLeaveTakeOn_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPayrollTypeLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployeesDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblSickLeave;
        private System.Windows.Forms.Label lblNormalLeave;
        private System.Windows.Forms.Label lblEmployee;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.PictureBox picPayrollTypeLock;
        private System.Windows.Forms.Label lblPayrollType;
        private System.Windows.Forms.DataGridView dgvPayrollTypeDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridView dgvEmployeesDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
    }
}

