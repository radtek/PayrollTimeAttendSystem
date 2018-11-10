namespace InteractPayroll
{
    partial class frmPublicHoliday
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPublicHoliday));
            this.lblDescription = new System.Windows.Forms.Label();
            this.dgvPublicHolidayDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn4 = new DataGridViewCalendarColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnDeleteRow = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAll = new System.Windows.Forms.Button();
            this.grbDeductionLock = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblPublicHolidayLock = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPublicHolidayDataGridView)).BeginInit();
            this.grbDeductionLock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            this.lblDescription.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDescription.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.ForeColor = System.Drawing.Color.Black;
            this.lblDescription.Location = new System.Drawing.Point(8, 8);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(489, 20);
            this.lblDescription.TabIndex = 345;
            this.lblDescription.Text = "Public Holiday";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvPublicHolidayDataGridView
            // 
            this.dgvPublicHolidayDataGridView.AllowUserToAddRows = false;
            this.dgvPublicHolidayDataGridView.AllowUserToDeleteRows = false;
            this.dgvPublicHolidayDataGridView.AllowUserToResizeColumns = false;
            this.dgvPublicHolidayDataGridView.AllowUserToResizeRows = false;
            this.dgvPublicHolidayDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvPublicHolidayDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPublicHolidayDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPublicHolidayDataGridView.ColumnHeadersHeight = 20;
            this.dgvPublicHolidayDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvPublicHolidayDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn4,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column1});
            this.dgvPublicHolidayDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvPublicHolidayDataGridView.EnableHeadersVisualStyles = false;
            this.dgvPublicHolidayDataGridView.Location = new System.Drawing.Point(8, 26);
            this.dgvPublicHolidayDataGridView.MultiSelect = false;
            this.dgvPublicHolidayDataGridView.Name = "dgvPublicHolidayDataGridView";
            this.dgvPublicHolidayDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPublicHolidayDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPublicHolidayDataGridView.RowHeadersWidth = 20;
            this.dgvPublicHolidayDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvPublicHolidayDataGridView.RowTemplate.Height = 19;
            this.dgvPublicHolidayDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvPublicHolidayDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPublicHolidayDataGridView.ShowCellToolTips = false;
            this.dgvPublicHolidayDataGridView.ShowEditingIcon = false;
            this.dgvPublicHolidayDataGridView.ShowRowErrors = false;
            this.dgvPublicHolidayDataGridView.Size = new System.Drawing.Size(489, 421);
            this.dgvPublicHolidayDataGridView.TabIndex = 346;
            this.dgvPublicHolidayDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPublicHolidayDataGridView_CellValueChanged);
            this.dgvPublicHolidayDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPublicHolidayDataGridView_RowEnter);
            this.dgvPublicHolidayDataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgvPublicHolidayDataGridView_SortCompare);
            this.dgvPublicHolidayDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Date";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dataGridViewTextBoxColumn4.Width = 130;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Description";
            this.Column2.MaxInputLength = 20;
            this.Column2.Name = "Column2";
            this.Column2.Width = 170;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Date Description";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column3.Width = 150;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Column4";
            this.Column4.Name = "Column4";
            this.Column4.Visible = false;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.Visible = false;
            // 
            // btnDeleteRow
            // 
            this.btnDeleteRow.Enabled = false;
            this.btnDeleteRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteRow.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteRow.Image")));
            this.btnDeleteRow.Location = new System.Drawing.Point(518, 419);
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new System.Drawing.Size(28, 28);
            this.btnDeleteRow.TabIndex = 351;
            this.btnDeleteRow.TabStop = false;
            this.toolTip1.SetToolTip(this.btnDeleteRow, "Delete current selected Public Holiday spreadsheet row");
            this.btnDeleteRow.Click += new System.EventHandler(this.btnDeleteRow_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(518, 103);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 350;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(518, 71);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 24);
            this.btnCancel.TabIndex = 349;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(518, 39);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 24);
            this.btnSave.TabIndex = 348;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Location = new System.Drawing.Point(518, 7);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(72, 24);
            this.btnUpdate.TabIndex = 347;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Description";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 285;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Column2";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Visible = false;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Column2";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // btnAll
            // 
            this.btnAll.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAll.Location = new System.Drawing.Point(518, 389);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(72, 24);
            this.btnAll.TabIndex = 352;
            this.btnAll.Text = "All";
            this.btnAll.Visible = false;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // grbDeductionLock
            // 
            this.grbDeductionLock.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.grbDeductionLock.Controls.Add(this.pictureBox1);
            this.grbDeductionLock.Controls.Add(this.lblPublicHolidayLock);
            this.grbDeductionLock.Location = new System.Drawing.Point(506, 133);
            this.grbDeductionLock.Name = "grbDeductionLock";
            this.grbDeductionLock.Size = new System.Drawing.Size(84, 163);
            this.grbDeductionLock.TabIndex = 353;
            this.grbDeductionLock.TabStop = false;
            this.grbDeductionLock.Text = "Record Lock";
            this.grbDeductionLock.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::PublicHoliday.Properties.Resources.NewLock48;
            this.pictureBox1.Location = new System.Drawing.Point(18, 23);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // lblPublicHolidayLock
            // 
            this.lblPublicHolidayLock.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPublicHolidayLock.ForeColor = System.Drawing.Color.Black;
            this.lblPublicHolidayLock.Location = new System.Drawing.Point(8, 74);
            this.lblPublicHolidayLock.Name = "lblPublicHolidayLock";
            this.lblPublicHolidayLock.Size = new System.Drawing.Size(68, 86);
            this.lblPublicHolidayLock.TabIndex = 0;
            this.lblPublicHolidayLock.Text = "Records are Locked Due to Current Wage Run.";
            this.lblPublicHolidayLock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmPublicHoliday
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(597, 454);
            this.Controls.Add(this.grbDeductionLock);
            this.Controls.Add(this.btnAll);
            this.Controls.Add(this.btnDeleteRow);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.dgvPublicHolidayDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "frmPublicHoliday";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmPublicHoliday_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPublicHolidayDataGridView)).EndInit();
            this.grbDeductionLock.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.DataGridView dgvPublicHolidayDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.Button btnDeleteRow;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.GroupBox grbDeductionLock;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblPublicHolidayLock;
        private DataGridViewCalendarColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

