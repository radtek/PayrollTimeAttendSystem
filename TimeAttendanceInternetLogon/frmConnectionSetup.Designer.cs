namespace InteractPayroll
{
    partial class frmConnectionSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConnectionSetup));
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtIP1 = new System.Windows.Forms.TextBox();
            this.txtIP2 = new System.Windows.Forms.TextBox();
            this.txtIP3 = new System.Windows.Forms.TextBox();
            this.txtIP4 = new System.Windows.Forms.TextBox();
            this.btnHeaderClose = new System.Windows.Forms.Button();
            this.lblHeader = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(246, 41);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 24);
            this.btnClose.TabIndex = 194;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnClose.MouseEnter += new System.EventHandler(this.btnClose_MouseEnter);
            this.btnClose.MouseLeave += new System.EventHandler(this.btnClose_MouseLeave);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.btnTest);
            this.groupBox3.Controls.Add(this.txtIP1);
            this.groupBox3.Controls.Add(this.txtIP2);
            this.groupBox3.Controls.Add(this.txtIP3);
            this.groupBox3.Controls.Add(this.txtIP4);
            this.groupBox3.Location = new System.Drawing.Point(9, 36);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(231, 142);
            this.groupBox3.TabIndex = 193;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Validite Web Server IP Address";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 19);
            this.label1.TabIndex = 184;
            this.label1.Text = "Change Web Server IP and Press Continue.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnTest
            // 
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Location = new System.Drawing.Point(154, 98);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(64, 24);
            this.btnTest.TabIndex = 183;
            this.btnTest.Text = "Continue";
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtIP1
            // 
            this.txtIP1.Location = new System.Drawing.Point(12, 32);
            this.txtIP1.MaxLength = 3;
            this.txtIP1.Name = "txtIP1";
            this.txtIP1.Size = new System.Drawing.Size(29, 20);
            this.txtIP1.TabIndex = 21;
            this.txtIP1.Text = " ";
            this.txtIP1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // txtIP2
            // 
            this.txtIP2.Location = new System.Drawing.Point(48, 32);
            this.txtIP2.MaxLength = 3;
            this.txtIP2.Name = "txtIP2";
            this.txtIP2.Size = new System.Drawing.Size(29, 20);
            this.txtIP2.TabIndex = 22;
            this.txtIP2.Text = " ";
            this.txtIP2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // txtIP3
            // 
            this.txtIP3.Location = new System.Drawing.Point(83, 32);
            this.txtIP3.MaxLength = 3;
            this.txtIP3.Name = "txtIP3";
            this.txtIP3.Size = new System.Drawing.Size(29, 20);
            this.txtIP3.TabIndex = 23;
            this.txtIP3.Text = " ";
            this.txtIP3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // txtIP4
            // 
            this.txtIP4.Location = new System.Drawing.Point(119, 32);
            this.txtIP4.MaxLength = 3;
            this.txtIP4.Name = "txtIP4";
            this.txtIP4.Size = new System.Drawing.Size(29, 20);
            this.txtIP4.TabIndex = 24;
            this.txtIP4.Text = " ";
            this.txtIP4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Numeric_KeyPress);
            // 
            // btnHeaderClose
            // 
            this.btnHeaderClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHeaderClose.BackColor = System.Drawing.Color.Silver;
            this.btnHeaderClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnHeaderClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeaderClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHeaderClose.Location = new System.Drawing.Point(285, 0);
            this.btnHeaderClose.Name = "btnHeaderClose";
            this.btnHeaderClose.Size = new System.Drawing.Size(32, 32);
            this.btnHeaderClose.TabIndex = 367;
            this.btnHeaderClose.TabStop = false;
            this.btnHeaderClose.Text = "X";
            this.btnHeaderClose.UseVisualStyleBackColor = false;
            this.btnHeaderClose.Click += new System.EventHandler(this.btnHeaderClose_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.BackColor = System.Drawing.Color.DimGray;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.Black;
            this.lblHeader.Location = new System.Drawing.Point(1, 1);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(287, 30);
            this.lblHeader.TabIndex = 366;
            this.lblHeader.Text = "Connection Setup";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmConnectionSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 186);
            this.Controls.Add(this.btnHeaderClose);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox3);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConnectionSetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connection Setup";
            this.Load += new System.EventHandler(this.frmConnectionSetup_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.TextBox txtIP1;
        private System.Windows.Forms.TextBox txtIP2;
        private System.Windows.Forms.TextBox txtIP3;
        private System.Windows.Forms.TextBox txtIP4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnHeaderClose;
        public System.Windows.Forms.Label lblHeader;
    }
}