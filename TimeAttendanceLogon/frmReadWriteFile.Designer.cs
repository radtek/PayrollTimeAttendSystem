namespace InteractPayrollClient
{
    partial class frmReadWriteFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReadWriteFile));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.prbAllFileProgress = new System.Windows.Forms.ProgressBar();
            this.lblDescription = new System.Windows.Forms.Label();
            this.prbFileProgress = new System.Windows.Forms.ProgressBar();
            this.lblFileName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblHeader = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(13, 53);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(264, 36);
            this.pictureBox1.TabIndex = 22;
            this.pictureBox1.TabStop = false;
            // 
            // prbAllFileProgress
            // 
            this.prbAllFileProgress.Location = new System.Drawing.Point(10, 132);
            this.prbAllFileProgress.Name = "prbAllFileProgress";
            this.prbAllFileProgress.Size = new System.Drawing.Size(436, 20);
            this.prbAllFileProgress.TabIndex = 20;
            // 
            // lblDescription
            // 
            this.lblDescription.ForeColor = System.Drawing.Color.Black;
            this.lblDescription.Location = new System.Drawing.Point(10, 40);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(228, 20);
            this.lblDescription.TabIndex = 18;
            // 
            // prbFileProgress
            // 
            this.prbFileProgress.Location = new System.Drawing.Point(10, 180);
            this.prbFileProgress.Name = "prbFileProgress";
            this.prbFileProgress.Size = new System.Drawing.Size(436, 20);
            this.prbFileProgress.TabIndex = 16;
            // 
            // lblFileName
            // 
            this.lblFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileName.Location = new System.Drawing.Point(14, 160);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(428, 20);
            this.lblFileName.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(428, 20);
            this.label1.TabIndex = 21;
            this.label1.Text = "All Files";
            // 
            // lblHeader
            // 
            this.lblHeader.BackColor = System.Drawing.Color.DimGray;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.Black;
            this.lblHeader.Location = new System.Drawing.Point(1, 1);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(454, 30);
            this.lblHeader.TabIndex = 366;
            this.lblHeader.Text = "Downloading Files From Local Site";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmReadWriteFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 220);
            this.ControlBox = false;
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.prbAllFileProgress);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.prbFileProgress);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmReadWriteFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Downloading Files From Local Database";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ProgressBar prbAllFileProgress;
        public System.Windows.Forms.ProgressBar prbFileProgress;
        public System.Windows.Forms.Label lblFileName;
        public System.Windows.Forms.Label lblHeader;
    }
}