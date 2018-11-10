namespace Test
{
    partial class Form1
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvPayrollTypeDataGridView = new System.Windows.Forms.DataGridView();
            this.Column48 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column42 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ListBox = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPayrollTypeDataGridView
            // 
            this.dgvPayrollTypeDataGridView.AllowUserToAddRows = false;
            this.dgvPayrollTypeDataGridView.AllowUserToDeleteRows = false;
            this.dgvPayrollTypeDataGridView.AllowUserToResizeColumns = false;
            this.dgvPayrollTypeDataGridView.AllowUserToResizeRows = false;
            this.dgvPayrollTypeDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvPayrollTypeDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPayrollTypeDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvPayrollTypeDataGridView.ColumnHeadersHeight = 20;
            this.dgvPayrollTypeDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvPayrollTypeDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column48,
            this.Column42,
            this.ListBox});
            this.dgvPayrollTypeDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvPayrollTypeDataGridView.EnableHeadersVisualStyles = false;
            this.dgvPayrollTypeDataGridView.Location = new System.Drawing.Point(24, 25);
            this.dgvPayrollTypeDataGridView.MultiSelect = false;
            this.dgvPayrollTypeDataGridView.Name = "dgvPayrollTypeDataGridView";
            this.dgvPayrollTypeDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPayrollTypeDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvPayrollTypeDataGridView.RowHeadersWidth = 20;
            this.dgvPayrollTypeDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvPayrollTypeDataGridView.RowTemplate.Height = 19;
            this.dgvPayrollTypeDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvPayrollTypeDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPayrollTypeDataGridView.ShowCellToolTips = false;
            this.dgvPayrollTypeDataGridView.ShowEditingIcon = false;
            this.dgvPayrollTypeDataGridView.ShowRowErrors = false;
            this.dgvPayrollTypeDataGridView.Size = new System.Drawing.Size(412, 140);
            this.dgvPayrollTypeDataGridView.TabIndex = 343;
            this.dgvPayrollTypeDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvPayrollTypeDataGridView_CellBeginEdit);
            
            // 
            // Column48
            // 
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Control;
            this.Column48.DefaultCellStyle = dataGridViewCellStyle8;
            this.Column48.HeaderText = "";
            this.Column48.Name = "Column48";
            this.Column48.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column48.Width = 20;
            // 
            // Column42
            // 
            this.Column42.HeaderText = "Description";
            this.Column42.Name = "Column42";
            this.Column42.Width = 149;
            // 
            // ListBox
            // 
            this.ListBox.HeaderText = "ListBox";
            this.ListBox.Name = "ListBox";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "a",
            "b",
            "c"});
            this.comboBox1.Location = new System.Drawing.Point(324, 238);
            this.comboBox1.MaxDropDownItems = 1;
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 345;
            this.comboBox1.Visible = false;
            
            this.comboBox1.TextChanged += new System.EventHandler(this.comboBox1_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 432);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.dgvPayrollTypeDataGridView);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayrollTypeDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPayrollTypeDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column48;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column42;
        private System.Windows.Forms.DataGridViewTextBoxColumn ListBox;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}

