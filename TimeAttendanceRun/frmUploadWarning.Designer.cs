namespace InteractPayroll
{
    partial class frmUploadWarning
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUploadWarning));
            this.lblInfo1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblCostCentre = new System.Windows.Forms.Label();
            this.dgvPayCategoryDataGridView = new System.Windows.Forms.DataGridView();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtPayrollRunDate = new System.Windows.Forms.TextBox();
            this.lblDateDescription = new System.Windows.Forms.Label();
            this.lblInfo2 = new System.Windows.Forms.Label();
            this.btnHeaderClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayCategoryDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // lblInfo1
            // 
            this.lblInfo1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo1.Location = new System.Drawing.Point(9, 204);
            this.lblInfo1.Name = "lblInfo1";
            this.lblInfo1.Size = new System.Drawing.Size(545, 30);
            this.lblInfo1.TabIndex = 293;
            this.lblInfo1.Text = "These Cost Centres Last Upload Date/Time of Timesheets does not Exceed the Payrol" +
    "l Run Date.";
            this.lblInfo1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.SystemColors.Control;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.Color.Black;
            this.btnOK.Location = new System.Drawing.Point(485, 287);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(69, 24);
            this.btnOK.TabIndex = 356;
            this.btnOK.TabStop = false;
            this.btnOK.Text = "Close";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblCostCentre
            // 
            this.lblCostCentre.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblCostCentre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCostCentre.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCostCentre.ForeColor = System.Drawing.Color.Black;
            this.lblCostCentre.Location = new System.Drawing.Point(13, 44);
            this.lblCostCentre.Name = "lblCostCentre";
            this.lblCostCentre.Size = new System.Drawing.Size(541, 20);
            this.lblCostCentre.TabIndex = 357;
            this.lblCostCentre.Text = "Cost Centre/s ";
            this.lblCostCentre.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.Column8,
            this.dataGridViewTextBoxColumn2,
            this.Column5,
            this.dataGridViewTextBoxColumn4,
            this.Column2,
            this.Column7});
            this.dgvPayCategoryDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvPayCategoryDataGridView.EnableHeadersVisualStyles = false;
            this.dgvPayCategoryDataGridView.Location = new System.Drawing.Point(13, 62);
            this.dgvPayCategoryDataGridView.MultiSelect = false;
            this.dgvPayCategoryDataGridView.Name = "dgvPayCategoryDataGridView";
            this.dgvPayCategoryDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPayCategoryDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvPayCategoryDataGridView.RowHeadersWidth = 20;
            this.dgvPayCategoryDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvPayCategoryDataGridView.RowTemplate.Height = 19;
            this.dgvPayCategoryDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvPayCategoryDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPayCategoryDataGridView.ShowCellToolTips = false;
            this.dgvPayCategoryDataGridView.ShowEditingIcon = false;
            this.dgvPayCategoryDataGridView.ShowRowErrors = false;
            this.dgvPayCategoryDataGridView.Size = new System.Drawing.Size(541, 136);
            this.dgvPayCategoryDataGridView.TabIndex = 359;
            this.dgvPayCategoryDataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgvPayCategoryDataGridView_SortCompare);
            this.dgvPayCategoryDataGridView.Sorted += new System.EventHandler(this.DataGridView_Sorted);
            // 
            // Column8
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            this.Column8.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column8.HeaderText = "";
            this.Column8.Name = "Column8";
            this.Column8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column8.Width = 20;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Description";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 332;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Last Upload Date / Time";
            this.Column5.Name = "Column5";
            this.Column5.Width = 150;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Visible = false;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.Visible = false;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "Column7";
            this.Column7.Name = "Column7";
            this.Column7.Visible = false;
            // 
            // txtPayrollRunDate
            // 
            this.txtPayrollRunDate.Enabled = false;
            this.txtPayrollRunDate.Location = new System.Drawing.Point(155, 249);
            this.txtPayrollRunDate.MaxLength = 6;
            this.txtPayrollRunDate.Name = "txtPayrollRunDate";
            this.txtPayrollRunDate.Size = new System.Drawing.Size(131, 20);
            this.txtPayrollRunDate.TabIndex = 360;
            // 
            // lblDateDescription
            // 
            this.lblDateDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateDescription.Location = new System.Drawing.Point(12, 252);
            this.lblDateDescription.Name = "lblDateDescription";
            this.lblDateDescription.Size = new System.Drawing.Size(137, 17);
            this.lblDateDescription.TabIndex = 361;
            this.lblDateDescription.Text = "Payroll Run Date - Wages";
            // 
            // lblInfo2
            // 
            this.lblInfo2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo2.Location = new System.Drawing.Point(13, 289);
            this.lblInfo2.Name = "lblInfo2";
            this.lblInfo2.Size = new System.Drawing.Size(439, 22);
            this.lblInfo2.TabIndex = 362;
            this.lblInfo2.Text = "This Run could possibly exclude Time-Sheets from these Cost Centres.";
            this.lblInfo2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnHeaderClose
            // 
            this.btnHeaderClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHeaderClose.BackColor = System.Drawing.Color.Silver;
            this.btnHeaderClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnHeaderClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeaderClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHeaderClose.Location = new System.Drawing.Point(535, 0);
            this.btnHeaderClose.Name = "btnHeaderClose";
            this.btnHeaderClose.Size = new System.Drawing.Size(32, 32);
            this.btnHeaderClose.TabIndex = 363;
            this.btnHeaderClose.TabStop = false;
            this.btnHeaderClose.Text = "X";
            this.btnHeaderClose.UseVisualStyleBackColor = false;
            this.btnHeaderClose.Click += new System.EventHandler(this.btnHeaderClose_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Yellow;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(1, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(538, 30);
            this.label1.TabIndex = 364;
            this.label1.Text = "Time-Sheet Warning";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmUploadWarning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 324);
            this.Controls.Add(this.btnHeaderClose);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblInfo2);
            this.Controls.Add(this.lblDateDescription);
            this.Controls.Add(this.txtPayrollRunDate);
            this.Controls.Add(this.lblCostCentre);
            this.Controls.Add(this.dgvPayCategoryDataGridView);
            this.Controls.Add(this.lblInfo1);
            this.Controls.Add(this.btnOK);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUploadWarning";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Upload Timesheets Warning";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayCategoryDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        public System.Windows.Forms.DataGridView dgvPayCategoryDataGridView;
        public System.Windows.Forms.Label lblCostCentre;
        public System.Windows.Forms.TextBox txtPayrollRunDate;
        public System.Windows.Forms.Label lblDateDescription;
        public System.Windows.Forms.Label lblInfo1;
        public System.Windows.Forms.Label lblInfo2;
        private System.Windows.Forms.Button btnHeaderClose;
        private System.Windows.Forms.Label label1;
    }
}