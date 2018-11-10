namespace InteractPayroll
{
    partial class frmCalender
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolTip1 = new System.Windows.Forms.ToolTip();
            this.btnMonthUp = new System.Windows.Forms.Button();
            this.btnMonthDown = new System.Windows.Forms.Button();
            this.btnJun = new System.Windows.Forms.Button();
            this.lblDate = new System.Windows.Forms.Label();
            this.btnMay = new System.Windows.Forms.Button();
            this.btnJul = new System.Windows.Forms.Button();
            this.btnApr = new System.Windows.Forms.Button();
            this.lblPaidDate = new System.Windows.Forms.Label();
            this.cboYear = new System.Windows.Forms.ComboBox();
            this.btnAug = new System.Windows.Forms.Button();
            this.btnJan = new System.Windows.Forms.Button();
            this.btnMar = new System.Windows.Forms.Button();
            this.btnSep = new System.Windows.Forms.Button();
            this.btnOct = new System.Windows.Forms.Button();
            this.lblMonth = new System.Windows.Forms.Label();
            this.btnDec = new System.Windows.Forms.Button();
            this.btnFeb = new System.Windows.Forms.Button();
            this.btnNov = new System.Windows.Forms.Button();
            this.dgvCalenderDataGridView = new System.Windows.Forms.DataGridView();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column23 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tmrTimer = new System.Windows.Forms.Timer();
            this.label1 = new System.Windows.Forms.Label();
            this.btnToday = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalenderDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // btnMonthUp
            // 
            this.btnMonthUp.BackColor = System.Drawing.Color.White;
            this.btnMonthUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMonthUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMonthUp.Location = new System.Drawing.Point(280, 81);
            this.btnMonthUp.Name = "btnMonthUp";
            this.btnMonthUp.Size = new System.Drawing.Size(48, 28);
            this.btnMonthUp.TabIndex = 88;
            this.btnMonthUp.TabStop = false;
            this.btnMonthUp.Text = ">";
            this.toolTip1.SetToolTip(this.btnMonthUp, "Next Month");
            this.btnMonthUp.UseVisualStyleBackColor = false;
            this.btnMonthUp.Click += new System.EventHandler(this.btnMonthUp_Click);
            // 
            // btnMonthDown
            // 
            this.btnMonthDown.BackColor = System.Drawing.Color.White;
            this.btnMonthDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMonthDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMonthDown.Location = new System.Drawing.Point(10, 80);
            this.btnMonthDown.Name = "btnMonthDown";
            this.btnMonthDown.Size = new System.Drawing.Size(48, 28);
            this.btnMonthDown.TabIndex = 89;
            this.btnMonthDown.TabStop = false;
            this.btnMonthDown.Text = "<";
            this.toolTip1.SetToolTip(this.btnMonthDown, "Previous Month");
            this.btnMonthDown.UseVisualStyleBackColor = false;
            this.btnMonthDown.Click += new System.EventHandler(this.btnMonthDown_Click);
            // 
            // btnJun
            // 
            this.btnJun.BackColor = System.Drawing.Color.White;
            this.btnJun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJun.Location = new System.Drawing.Point(280, 13);
            this.btnJun.Name = "btnJun";
            this.btnJun.Size = new System.Drawing.Size(48, 28);
            this.btnJun.TabIndex = 85;
            this.btnJun.TabStop = false;
            this.btnJun.Text = "Jun";
            this.btnJun.UseVisualStyleBackColor = false;
            this.btnJun.Click += new System.EventHandler(this.btnJun_Click);
            // 
            // lblDate
            // 
            this.lblDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblDate.Location = new System.Drawing.Point(6, 292);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(140, 20);
            this.lblDate.TabIndex = 99;
            this.lblDate.Text = "2008/01/01";
            this.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnMay
            // 
            this.btnMay.BackColor = System.Drawing.Color.White;
            this.btnMay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMay.Location = new System.Drawing.Point(226, 13);
            this.btnMay.Name = "btnMay";
            this.btnMay.Size = new System.Drawing.Size(48, 28);
            this.btnMay.TabIndex = 84;
            this.btnMay.TabStop = false;
            this.btnMay.Text = "May";
            this.btnMay.UseVisualStyleBackColor = false;
            this.btnMay.Click += new System.EventHandler(this.btnMay_Click);
            // 
            // btnJul
            // 
            this.btnJul.BackColor = System.Drawing.Color.White;
            this.btnJul.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJul.Location = new System.Drawing.Point(11, 47);
            this.btnJul.Name = "btnJul";
            this.btnJul.Size = new System.Drawing.Size(48, 28);
            this.btnJul.TabIndex = 90;
            this.btnJul.TabStop = false;
            this.btnJul.Text = "Jul";
            this.btnJul.UseVisualStyleBackColor = false;
            this.btnJul.Click += new System.EventHandler(this.btnJul_Click);
            // 
            // btnApr
            // 
            this.btnApr.BackColor = System.Drawing.Color.White;
            this.btnApr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApr.Location = new System.Drawing.Point(172, 13);
            this.btnApr.Name = "btnApr";
            this.btnApr.Size = new System.Drawing.Size(48, 28);
            this.btnApr.TabIndex = 83;
            this.btnApr.TabStop = false;
            this.btnApr.Text = "Apr";
            this.btnApr.UseVisualStyleBackColor = false;
            this.btnApr.Click += new System.EventHandler(this.btnApr_Click);
            // 
            // lblPaidDate
            // 
            this.lblPaidDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPaidDate.ForeColor = System.Drawing.Color.Red;
            this.lblPaidDate.Location = new System.Drawing.Point(7, 259);
            this.lblPaidDate.Name = "lblPaidDate";
            this.lblPaidDate.Size = new System.Drawing.Size(160, 24);
            this.lblPaidDate.TabIndex = 97;
            this.lblPaidDate.Text = "Christmas Day";
            this.lblPaidDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboYear
            // 
            this.cboYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboYear.FormattingEnabled = true;
            this.cboYear.Items.AddRange(new object[] {
            "1900",
            "1901",
            "1902",
            "1903",
            "1904",
            "1905",
            "1906",
            "1907",
            "1908",
            "1909",
            "1910",
            "1911",
            "1912",
            "1913",
            "1914",
            "1915",
            "1916",
            "1917",
            "1918",
            "1919",
            "1920",
            "1921",
            "1922",
            "1923",
            "1924",
            "1925",
            "1926",
            "1927",
            "1928",
            "1929",
            "1930",
            "1931",
            "1932",
            "1933",
            "1934",
            "1935",
            "1936",
            "1937",
            "1938",
            "1939",
            "1940",
            "1941",
            "1942",
            "1943",
            "1944",
            "1945",
            "1946",
            "1947",
            "1948",
            "1949",
            "1950",
            "1951",
            "1952",
            "1953",
            "1954",
            "1955",
            "1956",
            "1957",
            "1958",
            "1959",
            "1960",
            "1961",
            "1962",
            "1963",
            "1964",
            "1965",
            "1966",
            "1967",
            "1968",
            "1969",
            "1970",
            "1971",
            "1972",
            "1973",
            "1974",
            "1975",
            "1976",
            "1977",
            "1978",
            "1979",
            "1980",
            "1981",
            "1982",
            "1983",
            "1984",
            "1985",
            "1986",
            "1987",
            "1988",
            "1989",
            "1990",
            "1991",
            "1992",
            "1993",
            "1994",
            "1995",
            "1996",
            "1997",
            "1998",
            "1999",
            "2000",
            "2001",
            "2002",
            "2003",
            "2004",
            "2005",
            "2006"});
            this.cboYear.Location = new System.Drawing.Point(191, 84);
            this.cboYear.MaxDropDownItems = 13;
            this.cboYear.Name = "cboYear";
            this.cboYear.Size = new System.Drawing.Size(48, 21);
            this.cboYear.TabIndex = 98;
            this.cboYear.SelectedIndexChanged += new System.EventHandler(this.cboYear_SelectedIndexChanged);
            // 
            // btnAug
            // 
            this.btnAug.BackColor = System.Drawing.Color.White;
            this.btnAug.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAug.Location = new System.Drawing.Point(65, 47);
            this.btnAug.Name = "btnAug";
            this.btnAug.Size = new System.Drawing.Size(48, 28);
            this.btnAug.TabIndex = 91;
            this.btnAug.TabStop = false;
            this.btnAug.Text = "Aug";
            this.btnAug.UseVisualStyleBackColor = false;
            this.btnAug.Click += new System.EventHandler(this.btnAug_Click);
            // 
            // btnJan
            // 
            this.btnJan.BackColor = System.Drawing.Color.White;
            this.btnJan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJan.Location = new System.Drawing.Point(11, 13);
            this.btnJan.Name = "btnJan";
            this.btnJan.Size = new System.Drawing.Size(48, 28);
            this.btnJan.TabIndex = 80;
            this.btnJan.TabStop = false;
            this.btnJan.Text = "Jan";
            this.btnJan.UseVisualStyleBackColor = false;
            this.btnJan.Click += new System.EventHandler(this.btnJan_Click);
            // 
            // btnMar
            // 
            this.btnMar.BackColor = System.Drawing.Color.White;
            this.btnMar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMar.Location = new System.Drawing.Point(119, 13);
            this.btnMar.Name = "btnMar";
            this.btnMar.Size = new System.Drawing.Size(48, 28);
            this.btnMar.TabIndex = 82;
            this.btnMar.TabStop = false;
            this.btnMar.Text = "Mar";
            this.btnMar.UseVisualStyleBackColor = false;
            this.btnMar.Click += new System.EventHandler(this.btnMar_Click);
            // 
            // btnSep
            // 
            this.btnSep.BackColor = System.Drawing.Color.White;
            this.btnSep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSep.Location = new System.Drawing.Point(119, 47);
            this.btnSep.Name = "btnSep";
            this.btnSep.Size = new System.Drawing.Size(48, 28);
            this.btnSep.TabIndex = 92;
            this.btnSep.TabStop = false;
            this.btnSep.Text = "Sep";
            this.btnSep.UseVisualStyleBackColor = false;
            this.btnSep.Click += new System.EventHandler(this.btnSep_Click);
            // 
            // btnOct
            // 
            this.btnOct.BackColor = System.Drawing.Color.White;
            this.btnOct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOct.Location = new System.Drawing.Point(172, 47);
            this.btnOct.Name = "btnOct";
            this.btnOct.Size = new System.Drawing.Size(48, 28);
            this.btnOct.TabIndex = 93;
            this.btnOct.TabStop = false;
            this.btnOct.Text = "Oct";
            this.btnOct.UseVisualStyleBackColor = false;
            this.btnOct.Click += new System.EventHandler(this.btnOct_Click);
            // 
            // lblMonth
            // 
            this.lblMonth.BackColor = System.Drawing.SystemColors.Control;
            this.lblMonth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMonth.ForeColor = System.Drawing.Color.Black;
            this.lblMonth.Location = new System.Drawing.Point(102, 84);
            this.lblMonth.Name = "lblMonth";
            this.lblMonth.Size = new System.Drawing.Size(87, 20);
            this.lblMonth.TabIndex = 87;
            this.lblMonth.Text = "December";
            this.lblMonth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnDec
            // 
            this.btnDec.BackColor = System.Drawing.Color.White;
            this.btnDec.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDec.Location = new System.Drawing.Point(280, 47);
            this.btnDec.Name = "btnDec";
            this.btnDec.Size = new System.Drawing.Size(48, 28);
            this.btnDec.TabIndex = 95;
            this.btnDec.TabStop = false;
            this.btnDec.Text = "Dec";
            this.btnDec.UseVisualStyleBackColor = false;
            this.btnDec.Click += new System.EventHandler(this.btnDec_Click);
            // 
            // btnFeb
            // 
            this.btnFeb.BackColor = System.Drawing.Color.White;
            this.btnFeb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFeb.Location = new System.Drawing.Point(65, 13);
            this.btnFeb.Name = "btnFeb";
            this.btnFeb.Size = new System.Drawing.Size(48, 28);
            this.btnFeb.TabIndex = 81;
            this.btnFeb.TabStop = false;
            this.btnFeb.Text = "Feb";
            this.btnFeb.UseVisualStyleBackColor = false;
            this.btnFeb.Click += new System.EventHandler(this.btnFeb_Click);
            // 
            // btnNov
            // 
            this.btnNov.BackColor = System.Drawing.Color.White;
            this.btnNov.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNov.Location = new System.Drawing.Point(226, 47);
            this.btnNov.Name = "btnNov";
            this.btnNov.Size = new System.Drawing.Size(48, 28);
            this.btnNov.TabIndex = 94;
            this.btnNov.TabStop = false;
            this.btnNov.Text = "Nov";
            this.btnNov.UseVisualStyleBackColor = false;
            this.btnNov.Click += new System.EventHandler(this.btnNov_Click);
            // 
            // dgvCalenderDataGridView
            // 
            this.dgvCalenderDataGridView.AllowUserToAddRows = false;
            this.dgvCalenderDataGridView.AllowUserToDeleteRows = false;
            this.dgvCalenderDataGridView.AllowUserToResizeColumns = false;
            this.dgvCalenderDataGridView.AllowUserToResizeRows = false;
            this.dgvCalenderDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvCalenderDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCalenderDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCalenderDataGridView.ColumnHeadersHeight = 20;
            this.dgvCalenderDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvCalenderDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column8,
            this.Column23,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column7});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCalenderDataGridView.DefaultCellStyle = dataGridViewCellStyle9;
            this.dgvCalenderDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvCalenderDataGridView.EnableHeadersVisualStyles = false;
            this.dgvCalenderDataGridView.GridColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgvCalenderDataGridView.Location = new System.Drawing.Point(24, 115);
            this.dgvCalenderDataGridView.MultiSelect = false;
            this.dgvCalenderDataGridView.Name = "dgvCalenderDataGridView";
            this.dgvCalenderDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCalenderDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvCalenderDataGridView.RowHeadersVisible = false;
            this.dgvCalenderDataGridView.RowHeadersWidth = 20;
            this.dgvCalenderDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvCalenderDataGridView.RowTemplate.Height = 19;
            this.dgvCalenderDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvCalenderDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvCalenderDataGridView.ShowCellToolTips = false;
            this.dgvCalenderDataGridView.ShowEditingIcon = false;
            this.dgvCalenderDataGridView.ShowRowErrors = false;
            this.dgvCalenderDataGridView.Size = new System.Drawing.Size(290, 136);
            this.dgvCalenderDataGridView.TabIndex = 346;
            this.dgvCalenderDataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCalenderDataGridView_CellEnter);
            // 
            // Column8
            // 
            this.Column8.HeaderText = "Column8";
            this.Column8.Name = "Column8";
            this.Column8.Width = 5;
            // 
            // Column23
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column23.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column23.HeaderText = "  Sun";
            this.Column23.Name = "Column23";
            this.Column23.Width = 40;
            // 
            // Column1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column1.HeaderText = " Mon";
            this.Column1.Name = "Column1";
            this.Column1.Width = 40;
            // 
            // Column2
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column2.HeaderText = " Tue";
            this.Column2.Name = "Column2";
            this.Column2.Width = 40;
            // 
            // Column3
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column3.DefaultCellStyle = dataGridViewCellStyle5;
            this.Column3.HeaderText = " Wed";
            this.Column3.Name = "Column3";
            this.Column3.Width = 40;
            // 
            // Column4
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column4.DefaultCellStyle = dataGridViewCellStyle6;
            this.Column4.HeaderText = " Thu";
            this.Column4.Name = "Column4";
            this.Column4.Width = 40;
            // 
            // Column5
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column5.DefaultCellStyle = dataGridViewCellStyle7;
            this.Column5.HeaderText = " Fri";
            this.Column5.Name = "Column5";
            this.Column5.Width = 40;
            // 
            // Column6
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column6.DefaultCellStyle = dataGridViewCellStyle8;
            this.Column6.HeaderText = " Sat";
            this.Column6.Name = "Column6";
            this.Column6.Width = 40;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "Column7";
            this.Column7.Name = "Column7";
            this.Column7.Width = 5;
            // 
            // tmrTimer
            // 
            this.tmrTimer.Interval = 10;
            this.tmrTimer.Tick += new System.EventHandler(this.tmrTimer_Tick);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(172, 259);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 24);
            this.label1.TabIndex = 348;
            this.label1.Text = "Today :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnToday
            // 
            this.btnToday.BackColor = System.Drawing.Color.White;
            this.btnToday.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToday.Location = new System.Drawing.Point(226, 257);
            this.btnToday.Name = "btnToday";
            this.btnToday.Size = new System.Drawing.Size(102, 28);
            this.btnToday.TabIndex = 349;
            this.btnToday.TabStop = false;
            this.btnToday.Text = "2012/12/01";
            this.btnToday.UseVisualStyleBackColor = false;
            this.btnToday.Click += new System.EventHandler(this.btnToday_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(256, 291);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 352;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmCalender
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(339, 322);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnToday);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvCalenderDataGridView);
            this.Controls.Add(this.btnJun);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.btnMay);
            this.Controls.Add(this.btnJul);
            this.Controls.Add(this.btnApr);
            this.Controls.Add(this.lblPaidDate);
            this.Controls.Add(this.cboYear);
            this.Controls.Add(this.btnAug);
            this.Controls.Add(this.btnJan);
            this.Controls.Add(this.btnMar);
            this.Controls.Add(this.btnMonthUp);
            this.Controls.Add(this.btnSep);
            this.Controls.Add(this.btnOct);
            this.Controls.Add(this.lblMonth);
            this.Controls.Add(this.btnDec);
            this.Controls.Add(this.btnFeb);
            this.Controls.Add(this.btnMonthDown);
            this.Controls.Add(this.btnNov);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "frmCalender";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmCalender_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalenderDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnMonthUp;
        private System.Windows.Forms.Button btnMonthDown;
        private System.Windows.Forms.Button btnJun;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Button btnMay;
        private System.Windows.Forms.Button btnJul;
        private System.Windows.Forms.Button btnApr;
        private System.Windows.Forms.Label lblPaidDate;
        private System.Windows.Forms.ComboBox cboYear;
        private System.Windows.Forms.Button btnAug;
        private System.Windows.Forms.Button btnJan;
        private System.Windows.Forms.Button btnMar;
        private System.Windows.Forms.Button btnSep;
        private System.Windows.Forms.Button btnOct;
        private System.Windows.Forms.Label lblMonth;
        private System.Windows.Forms.Button btnDec;
        private System.Windows.Forms.Button btnFeb;
        private System.Windows.Forms.Button btnNov;
        private System.Windows.Forms.DataGridView dgvCalenderDataGridView;
        private System.Windows.Forms.Timer tmrTimer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnToday;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column23;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.Button btnClose;
    }
}

