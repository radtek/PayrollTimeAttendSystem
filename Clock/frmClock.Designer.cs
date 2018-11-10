namespace InteractPayrollClient
{
    partial class frmClock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmClock));
            this.cboCompany = new System.Windows.Forms.ComboBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.rbnWAN = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cboFAR = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbnLAN = new System.Windows.Forms.RadioButton();
            this.txtDeviceNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.grbSmartKey = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.cboLockOut = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.rbnClockFirstLast = new System.Windows.Forms.RadioButton();
            this.rbnClockNormal = new System.Windows.Forms.RadioButton();
            this.rbnBoth = new System.Windows.Forms.RadioButton();
            this.rbnTimeAttend = new System.Windows.Forms.RadioButton();
            this.rbnAccess = new System.Windows.Forms.RadioButton();
            this.txtReader = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grbClockOptions = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cboToHour = new System.Windows.Forms.ComboBox();
            this.cboFromHour = new System.Windows.Forms.ComboBox();
            this.cboToMinute = new System.Windows.Forms.ComboBox();
            this.cboFromMinute = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rbnClockRange = new System.Windows.Forms.RadioButton();
            this.rbnDynamic = new System.Windows.Forms.RadioButton();
            this.rbnOutOnly = new System.Windows.Forms.RadioButton();
            this.rbnInOnly = new System.Windows.Forms.RadioButton();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.lblClock = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.picReaderLock = new System.Windows.Forms.PictureBox();
            this.dgvClockDataGridView = new System.Windows.Forms.DataGridView();
            this.ClockDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClockNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RecCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grbSmartKey.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.grbClockOptions.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picReaderLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClockDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // cboCompany
            // 
            this.cboCompany.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCompany.Enabled = false;
            this.cboCompany.Location = new System.Drawing.Point(8, 18);
            this.cboCompany.Name = "cboCompany";
            this.cboCompany.Size = new System.Drawing.Size(401, 21);
            this.cboCompany.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(814, 159);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(76, 25);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // rbnWAN
            // 
            this.rbnWAN.Enabled = false;
            this.rbnWAN.Location = new System.Drawing.Point(135, 22);
            this.rbnWAN.Name = "rbnWAN";
            this.rbnWAN.Size = new System.Drawing.Size(59, 16);
            this.rbnWAN.TabIndex = 2;
            this.rbnWAN.Text = "WAN";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(63, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 16);
            this.label9.TabIndex = 2;
            this.label9.Text = "Minutes";
            // 
            // label7
            // 
            this.label7.ForeColor = System.Drawing.Color.Blue;
            this.label7.Location = new System.Drawing.Point(14, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 17);
            this.label7.TabIndex = 1;
            this.label7.Text = "Not Active";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Controls.Add(this.txtDeviceNo);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.grbSmartKey);
            this.groupBox3.Controls.Add(this.groupBox7);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.txtReader);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.grbClockOptions);
            this.groupBox3.Location = new System.Drawing.Point(7, 206);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(883, 286);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Clock Reader";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cboFAR);
            this.groupBox5.Location = new System.Drawing.Point(10, 222);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(212, 50);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Fingerprint False Acceptance Read";
            // 
            // cboFAR
            // 
            this.cboFAR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFAR.Enabled = false;
            this.cboFAR.Location = new System.Drawing.Point(10, 20);
            this.cboFAR.Name = "cboFAR";
            this.cboFAR.Size = new System.Drawing.Size(113, 21);
            this.cboFAR.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbnWAN);
            this.groupBox1.Controls.Add(this.rbnLAN);
            this.groupBox1.Location = new System.Drawing.Point(231, 166);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(212, 50);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Access to Fingerprint Clock Server via";
            // 
            // rbnLAN
            // 
            this.rbnLAN.Enabled = false;
            this.rbnLAN.Location = new System.Drawing.Point(8, 21);
            this.rbnLAN.Name = "rbnLAN";
            this.rbnLAN.Size = new System.Drawing.Size(48, 16);
            this.rbnLAN.TabIndex = 1;
            this.rbnLAN.Text = "LAN";
            // 
            // txtDeviceNo
            // 
            this.txtDeviceNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDeviceNo.Enabled = false;
            this.txtDeviceNo.Location = new System.Drawing.Point(580, 24);
            this.txtDeviceNo.MaxLength = 30;
            this.txtDeviceNo.Name = "txtDeviceNo";
            this.txtDeviceNo.Size = new System.Drawing.Size(33, 20);
            this.txtDeviceNo.TabIndex = 220;
            this.txtDeviceNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(449, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 17);
            this.label6.TabIndex = 195;
            this.label6.Text = "Device Number (No.)";
            // 
            // grbSmartKey
            // 
            this.grbSmartKey.Controls.Add(this.cboCompany);
            this.grbSmartKey.Controls.Add(this.label7);
            this.grbSmartKey.Location = new System.Drawing.Point(452, 167);
            this.grbSmartKey.Name = "grbSmartKey";
            this.grbSmartKey.Size = new System.Drawing.Size(421, 50);
            this.grbSmartKey.TabIndex = 6;
            this.grbSmartKey.TabStop = false;
            this.grbSmartKey.Text = "Company that owns Clock Reader";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label9);
            this.groupBox7.Controls.Add(this.cboLockOut);
            this.groupBox7.Location = new System.Drawing.Point(10, 166);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(212, 50);
            this.groupBox7.TabIndex = 2;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Access Control - Employee Lock Out";
            // 
            // cboLockOut
            // 
            this.cboLockOut.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLockOut.Enabled = false;
            this.cboLockOut.Items.AddRange(new object[] {
            "0",
            "15",
            "30",
            "45",
            "60",
            "75",
            "90",
            "105",
            "120"});
            this.cboLockOut.Location = new System.Drawing.Point(10, 20);
            this.cboLockOut.Name = "cboLockOut";
            this.cboLockOut.Size = new System.Drawing.Size(48, 21);
            this.cboLockOut.TabIndex = 1;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.groupBox6);
            this.groupBox4.Controls.Add(this.rbnBoth);
            this.groupBox4.Controls.Add(this.rbnTimeAttend);
            this.groupBox4.Controls.Add(this.rbnAccess);
            this.groupBox4.Location = new System.Drawing.Point(10, 55);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(433, 107);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Clock Reader Type";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.rbnClockFirstLast);
            this.groupBox6.Controls.Add(this.rbnClockNormal);
            this.groupBox6.Location = new System.Drawing.Point(221, 13);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(201, 81);
            this.groupBox6.TabIndex = 21;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Access / Time && Attendance Option";
            // 
            // rbnClockFirstLast
            // 
            this.rbnClockFirstLast.Enabled = false;
            this.rbnClockFirstLast.Location = new System.Drawing.Point(9, 46);
            this.rbnClockFirstLast.Name = "rbnClockFirstLast";
            this.rbnClockFirstLast.Size = new System.Drawing.Size(164, 24);
            this.rbnClockFirstLast.TabIndex = 4;
            this.rbnClockFirstLast.Text = "Use First IN and Last OUT";
            // 
            // rbnClockNormal
            // 
            this.rbnClockNormal.Enabled = false;
            this.rbnClockNormal.Location = new System.Drawing.Point(9, 20);
            this.rbnClockNormal.Name = "rbnClockNormal";
            this.rbnClockNormal.Size = new System.Drawing.Size(108, 24);
            this.rbnClockNormal.TabIndex = 3;
            this.rbnClockNormal.Text = "Normal";
            // 
            // rbnBoth
            // 
            this.rbnBoth.Enabled = false;
            this.rbnBoth.Location = new System.Drawing.Point(8, 70);
            this.rbnBoth.Name = "rbnBoth";
            this.rbnBoth.Size = new System.Drawing.Size(172, 24);
            this.rbnBoth.TabIndex = 4;
            this.rbnBoth.Text = "Access / Time && Attendance";
            this.rbnBoth.Click += new System.EventHandler(this.rbnBoth_Click);
            // 
            // rbnTimeAttend
            // 
            this.rbnTimeAttend.Enabled = false;
            this.rbnTimeAttend.Location = new System.Drawing.Point(8, 19);
            this.rbnTimeAttend.Name = "rbnTimeAttend";
            this.rbnTimeAttend.Size = new System.Drawing.Size(128, 24);
            this.rbnTimeAttend.TabIndex = 3;
            this.rbnTimeAttend.Text = "Time && Attendance";
            this.rbnTimeAttend.Click += new System.EventHandler(this.rbnTimeAttend_Click);
            // 
            // rbnAccess
            // 
            this.rbnAccess.Enabled = false;
            this.rbnAccess.Location = new System.Drawing.Point(8, 45);
            this.rbnAccess.Name = "rbnAccess";
            this.rbnAccess.Size = new System.Drawing.Size(72, 24);
            this.rbnAccess.TabIndex = 2;
            this.rbnAccess.Text = "Access";
            this.rbnAccess.Click += new System.EventHandler(this.rbnAccess_Click);
            // 
            // txtReader
            // 
            this.txtReader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtReader.Enabled = false;
            this.txtReader.Location = new System.Drawing.Point(76, 23);
            this.txtReader.MaxLength = 30;
            this.txtReader.Name = "txtReader";
            this.txtReader.Size = new System.Drawing.Size(216, 20);
            this.txtReader.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 12);
            this.label5.TabIndex = 21;
            this.label5.Text = "Description";
            // 
            // grbClockOptions
            // 
            this.grbClockOptions.Controls.Add(this.groupBox2);
            this.grbClockOptions.Controls.Add(this.rbnClockRange);
            this.grbClockOptions.Controls.Add(this.rbnDynamic);
            this.grbClockOptions.Controls.Add(this.rbnOutOnly);
            this.grbClockOptions.Controls.Add(this.rbnInOnly);
            this.grbClockOptions.Location = new System.Drawing.Point(452, 55);
            this.grbClockOptions.Name = "grbClockOptions";
            this.grbClockOptions.Size = new System.Drawing.Size(421, 107);
            this.grbClockOptions.TabIndex = 5;
            this.grbClockOptions.TabStop = false;
            this.grbClockOptions.Text = "Clock Option";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cboToHour);
            this.groupBox2.Controls.Add(this.cboFromHour);
            this.groupBox2.Controls.Add(this.cboToMinute);
            this.groupBox2.Controls.Add(this.cboFromMinute);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(162, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(247, 81);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Clock Range - IN Range";
            // 
            // cboToHour
            // 
            this.cboToHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboToHour.Enabled = false;
            this.cboToHour.Items.AddRange(new object[] {
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
            this.cboToHour.Location = new System.Drawing.Point(130, 38);
            this.cboToHour.Name = "cboToHour";
            this.cboToHour.Size = new System.Drawing.Size(48, 21);
            this.cboToHour.TabIndex = 4;
            // 
            // cboFromHour
            // 
            this.cboFromHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFromHour.Enabled = false;
            this.cboFromHour.Items.AddRange(new object[] {
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
            this.cboFromHour.Location = new System.Drawing.Point(9, 38);
            this.cboFromHour.Name = "cboFromHour";
            this.cboFromHour.Size = new System.Drawing.Size(48, 21);
            this.cboFromHour.TabIndex = 0;
            // 
            // cboToMinute
            // 
            this.cboToMinute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboToMinute.Enabled = false;
            this.cboToMinute.Items.AddRange(new object[] {
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59"});
            this.cboToMinute.Location = new System.Drawing.Point(189, 38);
            this.cboToMinute.Name = "cboToMinute";
            this.cboToMinute.Size = new System.Drawing.Size(48, 21);
            this.cboToMinute.TabIndex = 6;
            // 
            // cboFromMinute
            // 
            this.cboFromMinute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFromMinute.Enabled = false;
            this.cboFromMinute.Items.AddRange(new object[] {
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59"});
            this.cboFromMinute.Location = new System.Drawing.Point(68, 38);
            this.cboFromMinute.Name = "cboFromMinute";
            this.cboFromMinute.Size = new System.Drawing.Size(48, 21);
            this.cboFromMinute.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(177, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 23);
            this.label3.TabIndex = 7;
            this.label3.Text = ":";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(127, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "To";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(56, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = ":";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "From";
            // 
            // rbnClockRange
            // 
            this.rbnClockRange.Checked = true;
            this.rbnClockRange.Enabled = false;
            this.rbnClockRange.Location = new System.Drawing.Point(8, 79);
            this.rbnClockRange.Name = "rbnClockRange";
            this.rbnClockRange.Size = new System.Drawing.Size(144, 20);
            this.rbnClockRange.TabIndex = 3;
            this.rbnClockRange.TabStop = true;
            this.rbnClockRange.Text = "Clock Range";
            this.rbnClockRange.CheckedChanged += new System.EventHandler(this.rbnClockRange_CheckedChanged);
            // 
            // rbnDynamic
            // 
            this.rbnDynamic.Enabled = false;
            this.rbnDynamic.Location = new System.Drawing.Point(8, 59);
            this.rbnDynamic.Name = "rbnDynamic";
            this.rbnDynamic.Size = new System.Drawing.Size(156, 16);
            this.rbnDynamic.TabIndex = 2;
            this.rbnDynamic.Text = "In / Out (select on Clock)";
            // 
            // rbnOutOnly
            // 
            this.rbnOutOnly.Enabled = false;
            this.rbnOutOnly.Location = new System.Drawing.Point(8, 39);
            this.rbnOutOnly.Name = "rbnOutOnly";
            this.rbnOutOnly.Size = new System.Drawing.Size(48, 16);
            this.rbnOutOnly.TabIndex = 1;
            this.rbnOutOnly.Text = "Out";
            // 
            // rbnInOnly
            // 
            this.rbnInOnly.Enabled = false;
            this.rbnInOnly.Location = new System.Drawing.Point(8, 19);
            this.rbnInOnly.Name = "rbnInOnly";
            this.rbnInOnly.Size = new System.Drawing.Size(48, 16);
            this.rbnInOnly.TabIndex = 0;
            this.rbnInOnly.Text = "In";
            // 
            // btnDelete
            // 
            this.btnDelete.AutoSize = true;
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(814, 66);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(76, 25);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnNew
            // 
            this.btnNew.AutoSize = true;
            this.btnNew.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Location = new System.Drawing.Point(814, 5);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(76, 25);
            this.btnNew.TabIndex = 3;
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // lblClock
            // 
            this.lblClock.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblClock.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblClock.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClock.ForeColor = System.Drawing.Color.Black;
            this.lblClock.Location = new System.Drawing.Point(7, 7);
            this.lblClock.Name = "lblClock";
            this.lblClock.Size = new System.Drawing.Size(364, 20);
            this.lblClock.TabIndex = 0;
            this.lblClock.Text = "Clock Reader";
            this.lblClock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnUpdate
            // 
            this.btnUpdate.AutoSize = true;
            this.btnUpdate.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Location = new System.Drawing.Point(814, 35);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(76, 25);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(814, 97);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(76, 25);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(814, 128);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 25);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // picReaderLock
            // 
            this.picReaderLock.BackColor = System.Drawing.SystemColors.Control;
            this.picReaderLock.Image = global::InteractPayrollClient.Properties.Resources.NewLock16;
            this.picReaderLock.Location = new System.Drawing.Point(10, 28);
            this.picReaderLock.Name = "picReaderLock";
            this.picReaderLock.Size = new System.Drawing.Size(16, 16);
            this.picReaderLock.TabIndex = 203;
            this.picReaderLock.TabStop = false;
            this.picReaderLock.Visible = false;
            // 
            // dgvClockDataGridView
            // 
            this.dgvClockDataGridView.AllowUserToAddRows = false;
            this.dgvClockDataGridView.AllowUserToDeleteRows = false;
            this.dgvClockDataGridView.AllowUserToOrderColumns = true;
            this.dgvClockDataGridView.AllowUserToResizeColumns = false;
            this.dgvClockDataGridView.AllowUserToResizeRows = false;
            this.dgvClockDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvClockDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvClockDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvClockDataGridView.ColumnHeadersHeight = 20;
            this.dgvClockDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvClockDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ClockDesc,
            this.ClockNo,
            this.RecCount});
            this.dgvClockDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvClockDataGridView.EnableHeadersVisualStyles = false;
            this.dgvClockDataGridView.Location = new System.Drawing.Point(7, 25);
            this.dgvClockDataGridView.MultiSelect = false;
            this.dgvClockDataGridView.Name = "dgvClockDataGridView";
            this.dgvClockDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvClockDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvClockDataGridView.RowHeadersWidth = 20;
            this.dgvClockDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvClockDataGridView.RowTemplate.Height = 19;
            this.dgvClockDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvClockDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvClockDataGridView.ShowCellToolTips = false;
            this.dgvClockDataGridView.ShowEditingIcon = false;
            this.dgvClockDataGridView.ShowRowErrors = false;
            this.dgvClockDataGridView.Size = new System.Drawing.Size(364, 174);
            this.dgvClockDataGridView.TabIndex = 1;
            this.dgvClockDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvClockDataGridView_RowEnter);
            this.dgvClockDataGridView.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgvClockDataGridView_SortCompare);
            this.dgvClockDataGridView.Sorted += new System.EventHandler(this.dgvClockDataGridView_Sorted);
            // 
            // ClockDesc
            // 
            this.ClockDesc.HeaderText = "Description";
            this.ClockDesc.Name = "ClockDesc";
            this.ClockDesc.Width = 275;
            // 
            // ClockNo
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.ClockNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.ClockNo.HeaderText = "No.";
            this.ClockNo.Name = "ClockNo";
            this.ClockNo.Width = 50;
            // 
            // RecCount
            // 
            this.RecCount.HeaderText = "RecCount";
            this.RecCount.Name = "RecCount";
            this.RecCount.Visible = false;
            // 
            // frmClock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(896, 500);
            this.Controls.Add(this.picReaderLock);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.lblClock);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.dgvClockDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmClock";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmClock";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmClock_FormClosing);
            this.Load += new System.EventHandler(this.frmClock_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.grbSmartKey.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.grbClockOptions.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picReaderLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClockDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboCompany;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.RadioButton rbnWAN;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbnLAN;
        private System.Windows.Forms.TextBox txtDeviceNo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox grbSmartKey;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.ComboBox cboLockOut;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton rbnClockFirstLast;
        private System.Windows.Forms.RadioButton rbnClockNormal;
        private System.Windows.Forms.RadioButton rbnBoth;
        private System.Windows.Forms.RadioButton rbnTimeAttend;
        private System.Windows.Forms.RadioButton rbnAccess;
        private System.Windows.Forms.TextBox txtReader;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox grbClockOptions;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboToHour;
        private System.Windows.Forms.ComboBox cboFromHour;
        private System.Windows.Forms.ComboBox cboToMinute;
        private System.Windows.Forms.ComboBox cboFromMinute;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbnClockRange;
        private System.Windows.Forms.RadioButton rbnDynamic;
        private System.Windows.Forms.RadioButton rbnOutOnly;
        private System.Windows.Forms.RadioButton rbnInOnly;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Label lblClock;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox picReaderLock;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox cboFAR;
        private System.Windows.Forms.DataGridView dgvClockDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClockDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClockNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn RecCount;
    }
}