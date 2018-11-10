namespace InteractPayroll
{
    partial class frmOccupationDepartment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOccupationDepartment));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtOccupationDepartment = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.picOccupationDepartmentLock = new System.Windows.Forms.PictureBox();
            this.dgvOccupationDepartmentDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.picOccupationDepartmentLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOccupationDepartmentDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // txtOccupationDepartment
            // 
            this.txtOccupationDepartment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOccupationDepartment.Enabled = false;
            this.txtOccupationDepartment.Location = new System.Drawing.Point(78, 326);
            this.txtOccupationDepartment.MaxLength = 30;
            this.txtOccupationDepartment.Name = "txtOccupationDepartment";
            this.txtOccupationDepartment.Size = new System.Drawing.Size(254, 20);
            this.txtOccupationDepartment.TabIndex = 140;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 328);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 20);
            this.label2.TabIndex = 141;
            this.label2.Text = "Description ";
            // 
            // lblDescription
            // 
            this.lblDescription.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDescription.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.ForeColor = System.Drawing.Color.Black;
            this.lblDescription.Location = new System.Drawing.Point(8, 8);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(324, 20);
            this.lblDescription.TabIndex = 150;
            this.lblDescription.Text = "Department";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNew
            // 
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Location = new System.Drawing.Point(340, 8);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(72, 24);
            this.btnNew.TabIndex = 143;
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(340, 168);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 148;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(340, 136);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 24);
            this.btnCancel.TabIndex = 147;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(340, 104);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 24);
            this.btnSave.TabIndex = 146;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Location = new System.Drawing.Point(340, 40);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(72, 24);
            this.btnUpdate.TabIndex = 144;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(340, 72);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(72, 24);
            this.btnDelete.TabIndex = 145;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // picOccupationDepartmentLock
            // 
            this.picOccupationDepartmentLock.BackColor = System.Drawing.SystemColors.Control;
            this.picOccupationDepartmentLock.Image = ((System.Drawing.Image)(resources.GetObject("picOccupationDepartmentLock.Image")));
            this.picOccupationDepartmentLock.Location = new System.Drawing.Point(11, 29);
            this.picOccupationDepartmentLock.Name = "picOccupationDepartmentLock";
            this.picOccupationDepartmentLock.Size = new System.Drawing.Size(16, 16);
            this.picOccupationDepartmentLock.TabIndex = 244;
            this.picOccupationDepartmentLock.TabStop = false;
            this.picOccupationDepartmentLock.Visible = false;
            // 
            // dgvOccupationDepartmentDataGridView
            // 
            this.dgvOccupationDepartmentDataGridView.AllowUserToAddRows = false;
            this.dgvOccupationDepartmentDataGridView.AllowUserToDeleteRows = false;
            this.dgvOccupationDepartmentDataGridView.AllowUserToResizeColumns = false;
            this.dgvOccupationDepartmentDataGridView.AllowUserToResizeRows = false;
            this.dgvOccupationDepartmentDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvOccupationDepartmentDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvOccupationDepartmentDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvOccupationDepartmentDataGridView.ColumnHeadersHeight = 20;
            this.dgvOccupationDepartmentDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvOccupationDepartmentDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn3});
            this.dgvOccupationDepartmentDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvOccupationDepartmentDataGridView.EnableHeadersVisualStyles = false;
            this.dgvOccupationDepartmentDataGridView.Location = new System.Drawing.Point(8, 26);
            this.dgvOccupationDepartmentDataGridView.MultiSelect = false;
            this.dgvOccupationDepartmentDataGridView.Name = "dgvOccupationDepartmentDataGridView";
            this.dgvOccupationDepartmentDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvOccupationDepartmentDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvOccupationDepartmentDataGridView.RowHeadersWidth = 20;
            this.dgvOccupationDepartmentDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvOccupationDepartmentDataGridView.RowTemplate.Height = 19;
            this.dgvOccupationDepartmentDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvOccupationDepartmentDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOccupationDepartmentDataGridView.ShowCellToolTips = false;
            this.dgvOccupationDepartmentDataGridView.ShowEditingIcon = false;
            this.dgvOccupationDepartmentDataGridView.ShowRowErrors = false;
            this.dgvOccupationDepartmentDataGridView.Size = new System.Drawing.Size(324, 288);
            this.dgvOccupationDepartmentDataGridView.TabIndex = 339;
            this.dgvOccupationDepartmentDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOccupationDepartmentDataGridView_RowEnter);
            this.dgvOccupationDepartmentDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Description";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 285;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Visible = false;
            // 
            // frmOccupationDepartment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(419, 358);
            this.Controls.Add(this.picOccupationDepartmentLock);
            this.Controls.Add(this.txtOccupationDepartment);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.dgvOccupationDepartmentDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmOccupationDepartment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmOccupationDepartment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picOccupationDepartmentLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOccupationDepartmentDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtOccupationDepartment;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.PictureBox picOccupationDepartmentLock;
        private System.Windows.Forms.DataGridView dgvOccupationDepartmentDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    }
}

