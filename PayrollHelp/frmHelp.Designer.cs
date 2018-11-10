namespace InteractPayroll
{
    partial class frmHelp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHelp));
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tvwTreeViewContent = new System.Windows.Forms.TreeView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.tvwSearchTreeView = new System.Windows.Forms.TreeView();
            this.prgProgressBar = new System.Windows.Forms.ProgressBar();
            this.grbEdit = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtDetail = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnDelImage = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnFile = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbnLinkNode = new System.Windows.Forms.RadioButton();
            this.rbnLevel1 = new System.Windows.Forms.RadioButton();
            this.txtNode = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.grbEdit.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // reportViewer
            // 
            this.reportViewer.IsDocumentMapWidthFixed = true;
            this.reportViewer.LocalReport.ReportEmbeddedResource = "InteractPayroll.Report.rdlc";
            this.reportViewer.Location = new System.Drawing.Point(420, -1);
            this.reportViewer.Name = "reportViewer";
            this.reportViewer.PageCountMode = Microsoft.Reporting.WinForms.PageCountMode.Actual;
            this.reportViewer.ShowBackButton = false;
            this.reportViewer.ShowContextMenu = false;
            this.reportViewer.ShowCredentialPrompts = false;
            this.reportViewer.ShowDocumentMapButton = false;
            this.reportViewer.ShowProgress = false;
            this.reportViewer.ShowPromptAreaButton = false;
            this.reportViewer.ShowStopButton = false;
            this.reportViewer.Size = new System.Drawing.Size(811, 612);
            this.reportViewer.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Location = new System.Drawing.Point(3, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(416, 609);
            this.tabControl.TabIndex = 1;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tvwTreeViewContent);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(408, 583);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Content";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tvwTreeViewContent
            // 
            this.tvwTreeViewContent.HideSelection = false;
            this.tvwTreeViewContent.Location = new System.Drawing.Point(0, 2);
            this.tvwTreeViewContent.Name = "tvwTreeViewContent";
            this.tvwTreeViewContent.Size = new System.Drawing.Size(406, 580);
            this.tvwTreeViewContent.TabIndex = 0;
            this.tvwTreeViewContent.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwTreeViewContent_AfterSelect);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtSearch);
            this.tabPage2.Controls.Add(this.btnSearch);
            this.tabPage2.Controls.Add(this.tvwSearchTreeView);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(408, 583);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Search";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(0, 4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(382, 20);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(385, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(22, 22);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // tvwSearchTreeView
            // 
            this.tvwSearchTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvwSearchTreeView.HideSelection = false;
            this.tvwSearchTreeView.Location = new System.Drawing.Point(0, 28);
            this.tvwSearchTreeView.Name = "tvwSearchTreeView";
            this.tvwSearchTreeView.Size = new System.Drawing.Size(406, 554);
            this.tvwSearchTreeView.TabIndex = 2;
            this.tvwSearchTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwSearchTreeView_AfterSelect);
            // 
            // prgProgressBar
            // 
            this.prgProgressBar.Location = new System.Drawing.Point(1129, 2);
            this.prgProgressBar.Name = "prgProgressBar";
            this.prgProgressBar.Size = new System.Drawing.Size(97, 20);
            this.prgProgressBar.TabIndex = 2;
            this.prgProgressBar.Visible = false;
            // 
            // grbEdit
            // 
            this.grbEdit.Controls.Add(this.groupBox3);
            this.grbEdit.Controls.Add(this.btnSave);
            this.grbEdit.Controls.Add(this.btnCancel);
            this.grbEdit.Controls.Add(this.btnDelete);
            this.grbEdit.Controls.Add(this.btnUpdate);
            this.grbEdit.Controls.Add(this.btnNew);
            this.grbEdit.Controls.Add(this.groupBox2);
            this.grbEdit.Controls.Add(this.groupBox1);
            this.grbEdit.Controls.Add(this.txtNode);
            this.grbEdit.Location = new System.Drawing.Point(3, 472);
            this.grbEdit.Name = "grbEdit";
            this.grbEdit.Size = new System.Drawing.Size(1226, 136);
            this.grbEdit.TabIndex = 4;
            this.grbEdit.TabStop = false;
            this.grbEdit.Text = "Edit";
            this.grbEdit.Visible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtDetail);
            this.groupBox3.Location = new System.Drawing.Point(574, 9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(644, 119);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Text";
            // 
            // txtDetail
            // 
            this.txtDetail.Enabled = false;
            this.txtDetail.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDetail.Location = new System.Drawing.Point(8, 16);
            this.txtDetail.Multiline = true;
            this.txtDetail.Name = "txtDetail";
            this.txtDetail.Size = new System.Drawing.Size(628, 96);
            this.txtDetail.TabIndex = 10;
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(166, 101);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(50, 25);
            this.btnSave.TabIndex = 9;
            this.btnSave.TabStop = false;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(219, 101);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(50, 25);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.TabStop = false;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(113, 101);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(50, 25);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.TabStop = false;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.Location = new System.Drawing.Point(60, 101);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(50, 25);
            this.btnUpdate.TabIndex = 6;
            this.btnUpdate.TabStop = false;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(7, 101);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(50, 25);
            this.btnNew.TabIndex = 5;
            this.btnNew.TabStop = false;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtFileName);
            this.groupBox2.Controls.Add(this.btnDelImage);
            this.groupBox2.Controls.Add(this.btnClear);
            this.groupBox2.Controls.Add(this.btnFile);
            this.groupBox2.Location = new System.Drawing.Point(274, 9);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(294, 119);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Image";
            // 
            // txtFileName
            // 
            this.txtFileName.Enabled = false;
            this.txtFileName.Location = new System.Drawing.Point(81, 21);
            this.txtFileName.Multiline = true;
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(199, 85);
            this.txtFileName.TabIndex = 3;
            // 
            // btnDelImage
            // 
            this.btnDelImage.Enabled = false;
            this.btnDelImage.Location = new System.Drawing.Point(8, 82);
            this.btnDelImage.Name = "btnDelImage";
            this.btnDelImage.Size = new System.Drawing.Size(67, 25);
            this.btnDelImage.TabIndex = 2;
            this.btnDelImage.TabStop = false;
            this.btnDelImage.Text = "Delete";
            this.btnDelImage.UseVisualStyleBackColor = true;
            this.btnDelImage.Click += new System.EventHandler(this.btnDelNodeFile_Click);
            // 
            // btnClear
            // 
            this.btnClear.Enabled = false;
            this.btnClear.Location = new System.Drawing.Point(8, 51);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(67, 25);
            this.btnClear.TabIndex = 1;
            this.btnClear.TabStop = false;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnFile
            // 
            this.btnFile.Enabled = false;
            this.btnFile.Location = new System.Drawing.Point(8, 20);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(67, 25);
            this.btnFile.TabIndex = 0;
            this.btnFile.TabStop = false;
            this.btnFile.Text = "File";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbnLinkNode);
            this.groupBox1.Controls.Add(this.rbnLevel1);
            this.groupBox1.Location = new System.Drawing.Point(7, 47);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(262, 42);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "New Treeview Node Level";
            // 
            // rbnLinkNode
            // 
            this.rbnLinkNode.AutoSize = true;
            this.rbnLinkNode.Enabled = false;
            this.rbnLinkNode.Location = new System.Drawing.Point(101, 16);
            this.rbnLinkNode.Name = "rbnLinkNode";
            this.rbnLinkNode.Size = new System.Drawing.Size(143, 17);
            this.rbnLinkNode.TabIndex = 1;
            this.rbnLinkNode.TabStop = true;
            this.rbnLinkNode.Text = "Linked to Selected Node";
            this.rbnLinkNode.UseVisualStyleBackColor = true;
            this.rbnLinkNode.Click += new System.EventHandler(this.radiobutton_Level_Click);
            // 
            // rbnLevel1
            // 
            this.rbnLevel1.AutoSize = true;
            this.rbnLevel1.Enabled = false;
            this.rbnLevel1.Location = new System.Drawing.Point(9, 16);
            this.rbnLevel1.Name = "rbnLevel1";
            this.rbnLevel1.Size = new System.Drawing.Size(63, 17);
            this.rbnLevel1.TabIndex = 0;
            this.rbnLevel1.TabStop = true;
            this.rbnLevel1.Text = "Level 1 ";
            this.rbnLevel1.UseVisualStyleBackColor = true;
            this.rbnLevel1.Click += new System.EventHandler(this.radiobutton_Level_Click);
            // 
            // txtNode
            // 
            this.txtNode.Enabled = false;
            this.txtNode.Location = new System.Drawing.Point(7, 21);
            this.txtNode.Name = "txtNode";
            this.txtNode.Size = new System.Drawing.Size(262, 20);
            this.txtNode.TabIndex = 2;
            // 
            // frmHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1229, 614);
            this.Controls.Add(this.grbEdit);
            this.Controls.Add(this.prgProgressBar);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.reportViewer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "frmHelp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Interact Payroll Help";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmHelp_FormClosing);
            this.Load += new System.EventHandler(this.frmHelp_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmHelp_KeyDown);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.grbEdit.ResumeLayout(false);
            this.grbEdit.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TreeView tvwTreeViewContent;
        private System.Windows.Forms.ProgressBar prgProgressBar;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TreeView tvwSearchTreeView;
        private System.Windows.Forms.GroupBox grbEdit;
        private System.Windows.Forms.TextBox txtDetail;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnDelImage;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbnLinkNode;
        private System.Windows.Forms.RadioButton rbnLevel1;
        private System.Windows.Forms.TextBox txtNode;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TabPage tabPage2;
    }
}

