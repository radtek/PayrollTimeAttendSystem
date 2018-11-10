namespace InteractPayroll
{
    partial class frmEmployeeRecover
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
            this.btnRunFix = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.txtIP1 = new System.Windows.Forms.TextBox();
            this.txtIP2 = new System.Windows.Forms.TextBox();
            this.txtIP3 = new System.Windows.Forms.TextBox();
            this.txtIP4 = new System.Windows.Forms.TextBox();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRunFix
            // 
            this.btnRunFix.Enabled = false;
            this.btnRunFix.Location = new System.Drawing.Point(227, 17);
            this.btnRunFix.Name = "btnRunFix";
            this.btnRunFix.Size = new System.Drawing.Size(60, 24);
            this.btnRunFix.TabIndex = 197;
            this.btnRunFix.Text = "Run Fix";
            this.btnRunFix.Click += new System.EventHandler(this.btnRunFix_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.btnTestConnection);
            this.groupBox3.Controls.Add(this.txtIP1);
            this.groupBox3.Controls.Add(this.txtIP2);
            this.groupBox3.Controls.Add(this.txtIP3);
            this.groupBox3.Controls.Add(this.txtIP4);
            this.groupBox3.Location = new System.Drawing.Point(10, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(207, 144);
            this.groupBox3.TabIndex = 196;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Web Server IP Address";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(5, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(197, 41);
            this.label1.TabIndex = 184;
            this.label1.Text = "Change Web Server IP and press Test. (Save available if Successful)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Payroll Web Server IP";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(100, 106);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(101, 24);
            this.btnTestConnection.TabIndex = 183;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // txtIP1
            // 
            this.txtIP1.Enabled = false;
            this.txtIP1.Location = new System.Drawing.Point(12, 39);
            this.txtIP1.MaxLength = 3;
            this.txtIP1.Name = "txtIP1";
            this.txtIP1.Size = new System.Drawing.Size(29, 20);
            this.txtIP1.TabIndex = 21;
            this.txtIP1.Text = "41";
            // 
            // txtIP2
            // 
            this.txtIP2.Enabled = false;
            this.txtIP2.Location = new System.Drawing.Point(48, 39);
            this.txtIP2.MaxLength = 3;
            this.txtIP2.Name = "txtIP2";
            this.txtIP2.Size = new System.Drawing.Size(29, 20);
            this.txtIP2.TabIndex = 22;
            this.txtIP2.Text = "86";
            // 
            // txtIP3
            // 
            this.txtIP3.Enabled = false;
            this.txtIP3.Location = new System.Drawing.Point(83, 39);
            this.txtIP3.MaxLength = 3;
            this.txtIP3.Name = "txtIP3";
            this.txtIP3.Size = new System.Drawing.Size(29, 20);
            this.txtIP3.TabIndex = 23;
            this.txtIP3.Text = "99";
            // 
            // txtIP4
            // 
            this.txtIP4.Enabled = false;
            this.txtIP4.Location = new System.Drawing.Point(119, 39);
            this.txtIP4.MaxLength = 3;
            this.txtIP4.Name = "txtIP4";
            this.txtIP4.Size = new System.Drawing.Size(29, 20);
            this.txtIP4.TabIndex = 24;
            this.txtIP4.Text = "45";
            // 
            // frmEmployeeRecover
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 167);
            this.Controls.Add(this.btnRunFix);
            this.Controls.Add(this.groupBox3);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmEmployeeRecover";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmEmployeeRecover_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRunFix;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.TextBox txtIP1;
        private System.Windows.Forms.TextBox txtIP2;
        private System.Windows.Forms.TextBox txtIP3;
        private System.Windows.Forms.TextBox txtIP4;
    }
}

