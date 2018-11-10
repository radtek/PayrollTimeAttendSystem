namespace InteractPayroll
{
    partial class frmRecordsExcludedFromRun
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
            this.picWarningPicture = new System.Windows.Forms.PictureBox();
            this.lblNoOpenPayCategory = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnHeaderClose = new System.Windows.Forms.Button();
            this.lblHeader = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picWarningPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // picWarningPicture
            // 
            this.picWarningPicture.Image = global::ClosePayrollRun.Properties.Resources.account_book_error;
            this.picWarningPicture.Location = new System.Drawing.Point(12, 41);
            this.picWarningPicture.Name = "picWarningPicture";
            this.picWarningPicture.Size = new System.Drawing.Size(51, 50);
            this.picWarningPicture.TabIndex = 283;
            this.picWarningPicture.TabStop = false;
            // 
            // lblNoOpenPayCategory
            // 
            this.lblNoOpenPayCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoOpenPayCategory.ForeColor = System.Drawing.Color.Black;
            this.lblNoOpenPayCategory.Location = new System.Drawing.Point(69, 44);
            this.lblNoOpenPayCategory.Name = "lblNoOpenPayCategory";
            this.lblNoOpenPayCategory.Size = new System.Drawing.Size(344, 43);
            this.lblNoOpenPayCategory.TabIndex = 284;
            this.lblNoOpenPayCategory.Text = "There are Time-Sheet records that have been Uploaded after you have Run your Time" +
    " Attendance Run.";
            this.lblNoOpenPayCategory.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(204, 124);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 285;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(79, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(323, 26);
            this.label1.TabIndex = 286;
            this.label1.Text = "Speak to your System Administrator for Advice.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnHeaderClose
            // 
            this.btnHeaderClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHeaderClose.BackColor = System.Drawing.Color.Silver;
            this.btnHeaderClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnHeaderClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeaderClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHeaderClose.Location = new System.Drawing.Point(448, 0);
            this.btnHeaderClose.Name = "btnHeaderClose";
            this.btnHeaderClose.Size = new System.Drawing.Size(32, 32);
            this.btnHeaderClose.TabIndex = 362;
            this.btnHeaderClose.TabStop = false;
            this.btnHeaderClose.Text = "X";
            this.btnHeaderClose.UseVisualStyleBackColor = false;
            this.btnHeaderClose.Click += new System.EventHandler(this.btnHeaderClose_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.BackColor = System.Drawing.Color.Yellow;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.Black;
            this.lblHeader.Location = new System.Drawing.Point(1, 1);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(451, 30);
            this.lblHeader.TabIndex = 363;
            this.lblHeader.Text = "Time-Sheets Excluded from Run Warning";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmRecordsExcludedFromRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 173);
            this.Controls.Add(this.btnHeaderClose);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblNoOpenPayCategory);
            this.Controls.Add(this.picWarningPicture);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRecordsExcludedFromRun";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Records Excluded From Run";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmRecordsExcludedFromRun_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.picWarningPicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picWarningPicture;
        private System.Windows.Forms.Label lblNoOpenPayCategory;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnHeaderClose;
        public System.Windows.Forms.Label lblHeader;
    }
}