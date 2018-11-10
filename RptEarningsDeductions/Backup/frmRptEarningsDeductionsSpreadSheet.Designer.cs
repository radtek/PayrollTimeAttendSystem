namespace InteractPayroll
{
    partial class frmRptEarningsDeductionsSpreadSheet
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
            this.SuspendLayout();
            // 
            // printDocument
            // 
            this.printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument_PrintPage);
            // 
            // frmRptEarningsDeductionsSpreadSheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(990, 607);
            this.Name = "frmRptEarningsDeductionsSpreadSheet";
            this.Text = "frmRptEarningsDeductionsSpreadSheet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmRptEarningsDeductionsSpreadSheet_FormClosing);
            this.Load += new System.EventHandler(this.frmRptEarningsDeductionsSpreadSheet_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}