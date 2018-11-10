using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace InteractPayrollClient
{
	public class frmClock : System.Windows.Forms.Form
	{
        clsTAUtilities clsTAUtilities;

        ToolStripMenuItem miLinkedMenuItem;
			
		private DataSet pvtDataSet;
		private DataView pvtDataView;
		private string pvtstrDeviceDesc = "";
		private string pvtstrDeviceUsage = "";
		private string pvtstrTimeAttendClockFirstLastInd = "";
		private string pvtstrClockInOutParm = "";
        private string pvtstrLanWanInd = "";
        private int pvtintCompanyNo = -1;
		private int pvtintClockInRangeFrom = -1;
		private int pvtintClockInRangeTo = -1;
		private int pvtintLockOutMinutes = -1;

        private int pvtintDataViewReaderIndex = -1;
      
        private System.Windows.Forms.RadioButton rbnInOnly;
		private System.Windows.Forms.RadioButton rbnOutOnly;
		private System.Windows.Forms.RadioButton rbnDynamic;
		private System.Windows.Forms.RadioButton rbnClockRange;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox cboFromHour;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cboFromMinute;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cboToMinute;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cboToHour;
		private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox grbClockOptions;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Label lblClock;
		private C1.Win.C1FlexGrid.C1FlexGrid flxgReader;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton rbnTimeAttend;
		private System.Windows.Forms.RadioButton rbnAccess;
		private System.Windows.Forms.Button btnNew;
		private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox txtReader;
		private System.Windows.Forms.RadioButton rbnBoth;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.RadioButton rbnClockNormal;
		private System.Windows.Forms.RadioButton rbnClockFirstLast;
		private System.Windows.Forms.PictureBox picReaderLock;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.ComboBox cboLockOut;
        private System.Windows.Forms.Label label9;
		private System.Windows.Forms.GroupBox grbSmartKey;
		private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cboCompany;
        private TextBox txtDeviceNo;
        private Label label6;
        private GroupBox groupBox1;
        private RadioButton rbnWAN;
        private RadioButton rbnLAN;
		private System.ComponentModel.IContainer components;

		public frmClock()
		{
			InitializeComponent();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmClock));
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
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lblClock = new System.Windows.Forms.Label();
            this.flxgReader = new C1.Win.C1FlexGrid.C1FlexGrid();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtDeviceNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.grbSmartKey = new System.Windows.Forms.GroupBox();
            this.cboCompany = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
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
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.picReaderLock = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbnLAN = new System.Windows.Forms.RadioButton();
            this.rbnWAN = new System.Windows.Forms.RadioButton();
            this.grbClockOptions.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.flxgReader)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.grbSmartKey.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picReaderLock)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbClockOptions
            // 
            this.grbClockOptions.Controls.Add(this.groupBox2);
            this.grbClockOptions.Controls.Add(this.rbnClockRange);
            this.grbClockOptions.Controls.Add(this.rbnDynamic);
            this.grbClockOptions.Controls.Add(this.rbnOutOnly);
            this.grbClockOptions.Controls.Add(this.rbnInOnly);
            this.grbClockOptions.Location = new System.Drawing.Point(408, 52);
            this.grbClockOptions.Name = "grbClockOptions";
            this.grbClockOptions.Size = new System.Drawing.Size(420, 100);
            this.grbClockOptions.TabIndex = 0;
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
            this.groupBox2.Location = new System.Drawing.Point(158, 16);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(246, 73);
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
            this.cboToHour.Location = new System.Drawing.Point(131, 35);
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
            this.cboFromHour.Location = new System.Drawing.Point(10, 35);
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
            this.cboToMinute.Location = new System.Drawing.Point(190, 35);
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
            this.cboFromMinute.Location = new System.Drawing.Point(69, 35);
            this.cboFromMinute.Name = "cboFromMinute";
            this.cboFromMinute.Size = new System.Drawing.Size(48, 21);
            this.cboFromMinute.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(178, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 23);
            this.label3.TabIndex = 7;
            this.label3.Text = ":";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(128, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "To";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(57, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = ":";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "From";
            // 
            // rbnClockRange
            // 
            this.rbnClockRange.Checked = true;
            this.rbnClockRange.Enabled = false;
            this.rbnClockRange.Location = new System.Drawing.Point(8, 76);
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
            this.rbnDynamic.Location = new System.Drawing.Point(8, 56);
            this.rbnDynamic.Name = "rbnDynamic";
            this.rbnDynamic.Size = new System.Drawing.Size(156, 16);
            this.rbnDynamic.TabIndex = 2;
            this.rbnDynamic.Text = "In / Out (select on Clock)";
            // 
            // rbnOutOnly
            // 
            this.rbnOutOnly.Enabled = false;
            this.rbnOutOnly.Location = new System.Drawing.Point(8, 36);
            this.rbnOutOnly.Name = "rbnOutOnly";
            this.rbnOutOnly.Size = new System.Drawing.Size(48, 16);
            this.rbnOutOnly.TabIndex = 1;
            this.rbnOutOnly.Text = "Out";
            // 
            // rbnInOnly
            // 
            this.rbnInOnly.Enabled = false;
            this.rbnInOnly.Location = new System.Drawing.Point(8, 16);
            this.rbnInOnly.Name = "rbnInOnly";
            this.rbnInOnly.Size = new System.Drawing.Size(48, 16);
            this.rbnInOnly.TabIndex = 0;
            this.rbnInOnly.Text = "In";
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(776, 168);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(76, 24);
            this.btnClose.TabIndex = 16;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(776, 104);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(76, 24);
            this.btnSave.TabIndex = 24;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(776, 136);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 24);
            this.btnCancel.TabIndex = 23;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.AutoSize = true;
            this.btnUpdate.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnUpdate.Location = new System.Drawing.Point(776, 40);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(76, 24);
            this.btnUpdate.TabIndex = 22;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // lblClock
            // 
            this.lblClock.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblClock.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblClock.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClock.ForeColor = System.Drawing.Color.Black;
            this.lblClock.Location = new System.Drawing.Point(8, 9);
            this.lblClock.Name = "lblClock";
            this.lblClock.Size = new System.Drawing.Size(301, 20);
            this.lblClock.TabIndex = 189;
            this.lblClock.Text = "Clock Reader";
            this.lblClock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flxgReader
            // 
            this.flxgReader.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.flxgReader.AllowEditing = false;
            this.flxgReader.AllowResizing = C1.Win.C1FlexGrid.AllowResizingEnum.None;
            this.flxgReader.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flxgReader.BorderStyle = C1.Win.C1FlexGrid.Util.BaseControls.BorderStyleEnum.FixedSingle;
            this.flxgReader.ColumnInfo = resources.GetString("flxgReader.ColumnInfo");
            this.flxgReader.FocusRect = C1.Win.C1FlexGrid.FocusRectEnum.None;
            this.flxgReader.Location = new System.Drawing.Point(8, 28);
            this.flxgReader.Name = "flxgReader";
            this.flxgReader.Rows.Count = 1;
            this.flxgReader.Rows.DefaultSize = 17;
            this.flxgReader.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.flxgReader.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;
            this.flxgReader.ShowCursor = true;
            this.flxgReader.ShowErrors = true;
            this.flxgReader.Size = new System.Drawing.Size(301, 172);
            this.flxgReader.StyleInfo = resources.GetString("flxgReader.StyleInfo");
            this.flxgReader.TabIndex = 188;
            this.flxgReader.AfterRowColChange += new C1.Win.C1FlexGrid.RangeEventHandler(this.flxgReader_AfterRowColChange);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Controls.Add(this.txtDeviceNo);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.grbSmartKey);
            this.groupBox3.Controls.Add(this.groupBox7);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.txtReader);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.grbClockOptions);
            this.groupBox3.Location = new System.Drawing.Point(8, 207);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(844, 223);
            this.groupBox3.TabIndex = 190;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Clock Reader";
            // 
            // txtDeviceNo
            // 
            this.txtDeviceNo.Enabled = false;
            this.txtDeviceNo.Location = new System.Drawing.Point(493, 21);
            this.txtDeviceNo.MaxLength = 30;
            this.txtDeviceNo.Name = "txtDeviceNo";
            this.txtDeviceNo.Size = new System.Drawing.Size(33, 20);
            this.txtDeviceNo.TabIndex = 220;
            this.txtDeviceNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(405, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 17);
            this.label6.TabIndex = 195;
            this.label6.Text = "Device Number";
            // 
            // grbSmartKey
            // 
            this.grbSmartKey.Controls.Add(this.cboCompany);
            this.grbSmartKey.Controls.Add(this.label7);
            this.grbSmartKey.Location = new System.Drawing.Point(236, 158);
            this.grbSmartKey.Name = "grbSmartKey";
            this.grbSmartKey.Size = new System.Drawing.Size(320, 52);
            this.grbSmartKey.TabIndex = 219;
            this.grbSmartKey.TabStop = false;
            this.grbSmartKey.Text = "Company that owns Clock Reader";
            // 
            // cboCompany
            // 
            this.cboCompany.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCompany.Enabled = false;
            this.cboCompany.Location = new System.Drawing.Point(8, 20);
            this.cboCompany.Name = "cboCompany";
            this.cboCompany.Size = new System.Drawing.Size(306, 21);
            this.cboCompany.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.ForeColor = System.Drawing.Color.Blue;
            this.label7.Location = new System.Drawing.Point(10, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 17);
            this.label7.TabIndex = 1;
            this.label7.Text = "Not Active";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label9);
            this.groupBox7.Controls.Add(this.cboLockOut);
            this.groupBox7.Location = new System.Drawing.Point(12, 158);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(212, 50);
            this.groupBox7.TabIndex = 194;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Access Control - Employee Lock Out";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(59, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 16);
            this.label9.TabIndex = 2;
            this.label9.Text = "Minutes";
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
            this.cboLockOut.Location = new System.Drawing.Point(6, 19);
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
            this.groupBox4.Location = new System.Drawing.Point(12, 52);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(384, 100);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Clock Reader Type";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.rbnClockFirstLast);
            this.groupBox6.Controls.Add(this.rbnClockNormal);
            this.groupBox6.Location = new System.Drawing.Point(172, 13);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(198, 76);
            this.groupBox6.TabIndex = 21;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Access / Time && Attendance Option";
            // 
            // rbnClockFirstLast
            // 
            this.rbnClockFirstLast.Enabled = false;
            this.rbnClockFirstLast.Location = new System.Drawing.Point(8, 44);
            this.rbnClockFirstLast.Name = "rbnClockFirstLast";
            this.rbnClockFirstLast.Size = new System.Drawing.Size(164, 24);
            this.rbnClockFirstLast.TabIndex = 4;
            this.rbnClockFirstLast.Text = "Use First IN and Last OUT";
            // 
            // rbnClockNormal
            // 
            this.rbnClockNormal.Enabled = false;
            this.rbnClockNormal.Location = new System.Drawing.Point(8, 20);
            this.rbnClockNormal.Name = "rbnClockNormal";
            this.rbnClockNormal.Size = new System.Drawing.Size(108, 24);
            this.rbnClockNormal.TabIndex = 3;
            this.rbnClockNormal.Text = "Normal";
            // 
            // rbnBoth
            // 
            this.rbnBoth.Enabled = false;
            this.rbnBoth.Location = new System.Drawing.Point(6, 68);
            this.rbnBoth.Name = "rbnBoth";
            this.rbnBoth.Size = new System.Drawing.Size(172, 24);
            this.rbnBoth.TabIndex = 4;
            this.rbnBoth.Text = "Access / Time && Attendance";
            this.rbnBoth.CheckedChanged += new System.EventHandler(this.rbnBoth_CheckedChanged);
            // 
            // rbnTimeAttend
            // 
            this.rbnTimeAttend.Enabled = false;
            this.rbnTimeAttend.Location = new System.Drawing.Point(6, 19);
            this.rbnTimeAttend.Name = "rbnTimeAttend";
            this.rbnTimeAttend.Size = new System.Drawing.Size(128, 24);
            this.rbnTimeAttend.TabIndex = 3;
            this.rbnTimeAttend.Text = "Time && Attendance";
            this.rbnTimeAttend.CheckedChanged += new System.EventHandler(this.rbnTimeAttend_CheckedChanged);
            // 
            // rbnAccess
            // 
            this.rbnAccess.Enabled = false;
            this.rbnAccess.Location = new System.Drawing.Point(6, 44);
            this.rbnAccess.Name = "rbnAccess";
            this.rbnAccess.Size = new System.Drawing.Size(72, 24);
            this.rbnAccess.TabIndex = 2;
            this.rbnAccess.Text = "Access";
            this.rbnAccess.CheckedChanged += new System.EventHandler(this.rbnAccess_CheckedChanged);
            // 
            // txtReader
            // 
            this.txtReader.Enabled = false;
            this.txtReader.Location = new System.Drawing.Point(72, 20);
            this.txtReader.MaxLength = 30;
            this.txtReader.Name = "txtReader";
            this.txtReader.Size = new System.Drawing.Size(216, 20);
            this.txtReader.TabIndex = 22;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 12);
            this.label5.TabIndex = 21;
            this.label5.Text = "Description";
            // 
            // btnNew
            // 
            this.btnNew.AutoSize = true;
            this.btnNew.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNew.Location = new System.Drawing.Point(776, 8);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(76, 24);
            this.btnNew.TabIndex = 191;
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.AutoSize = true;
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDelete.Location = new System.Drawing.Point(776, 72);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(76, 24);
            this.btnDelete.TabIndex = 192;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // picReaderLock
            // 
            this.picReaderLock.BackColor = System.Drawing.SystemColors.Control;
            this.picReaderLock.Image = ((System.Drawing.Image)(resources.GetObject("picReaderLock.Image")));
            this.picReaderLock.Location = new System.Drawing.Point(11, 31);
            this.picReaderLock.Name = "picReaderLock";
            this.picReaderLock.Size = new System.Drawing.Size(12, 12);
            this.picReaderLock.TabIndex = 193;
            this.picReaderLock.TabStop = false;
            this.picReaderLock.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbnWAN);
            this.groupBox1.Controls.Add(this.rbnLAN);
            this.groupBox1.Location = new System.Drawing.Point(566, 160);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(212, 50);
            this.groupBox1.TabIndex = 195;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Access to Fingerprint Clock Server via";
            // 
            // rbnLAN
            // 
            this.rbnLAN.Enabled = false;
            this.rbnLAN.Location = new System.Drawing.Point(10, 22);
            this.rbnLAN.Name = "rbnLAN";
            this.rbnLAN.Size = new System.Drawing.Size(48, 16);
            this.rbnLAN.TabIndex = 1;
            this.rbnLAN.Text = "LAN";
            // 
            // rbnWAN
            // 
            this.rbnWAN.Enabled = false;
            this.rbnWAN.Location = new System.Drawing.Point(131, 23);
            this.rbnWAN.Name = "rbnWAN";
            this.rbnWAN.Size = new System.Drawing.Size(59, 16);
            this.rbnWAN.TabIndex = 2;
            this.rbnWAN.Text = "WAN";
            // 
            // frmClock
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(860, 441);
            this.Controls.Add(this.picReaderLock);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.lblClock);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.flxgReader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmClock";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Clock";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.frmClock_Closing);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmClock_FormClosing);
            this.Load += new System.EventHandler(this.frmClock_Load);
            this.grbClockOptions.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.flxgReader)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.grbSmartKey.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picReaderLock)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void frmClock_Load(object sender, System.EventArgs e)
		{
            try
            {
                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

			    string strTag = "Start";

			    C1.Win.C1FlexGrid.CellStyle flxgReaderNoEmployeeStyle = this.flxgReader.Styles.Add("NoEmployees");
			    flxgReaderNoEmployeeStyle.BackColor = Color.Orange;

				clsTAUtilities = new clsTAUtilities("busClock");

                this.lblClock.Paint += new System.Windows.Forms.PaintEventHandler(clsTAUtilities.Label_Paint);

                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                byte[] bytCompress = (byte[])clsTAUtilities.DynamicFunction("Get_Form_Records", objParm);
                pvtDataSet = clsTAUtilities.DeCompress_Array_To_DataSet(bytCompress);
                
				for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["Company"].Rows.Count;intRowCount++)
				{
					this.cboCompany.Items.Add(pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_DESC"].ToString());
				}

				strTag = "After DataSet";

				Load_CurrentForm_Records();
			}
			catch (Exception eException)
			{
				clsTAUtilities.ErrorHandler(eException);
			}
		}
		
		private void Load_CurrentForm_Records()
		{
			pvtDataView = null;
			pvtDataView = new DataView(pvtDataSet.Tables["Clock"],
				"",
				"DEVICE_DESC",
				DataViewRowState.CurrentRows);
				
			this.flxgReader.Rows.Count = 1;

			this.flxgReader.BeginInit();

			for (int intRow = 0;intRow < pvtDataView.Count;intRow++)
			{
				this.flxgReader.Rows.Count += 1;

                this.flxgReader[this.flxgReader.Rows.Count - 1, 1] = pvtDataView[intRow]["DEVICE_NO"].ToString();
				this.flxgReader[this.flxgReader.Rows.Count - 1,2] = pvtDataView[intRow]["DEVICE_DESC"].ToString();
                this.flxgReader[this.flxgReader.Rows.Count - 1, 3] = intRow;
			}

			this.flxgReader.EndInit();

			if (pvtDataView.Count > 0)
			{
				this.flxgReader.Row = 0;
				this.flxgReader.Row = 1;
			}
			else
			{
				this.btnDelete.Enabled = false;
				this.btnUpdate.Enabled = false;
			}
		}

		private void rbnClockRange_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.btnSave.Enabled == true)
			{
				if (rbnClockRange.Checked == true)
				{
					this.cboFromHour.Enabled = true;
					this.cboFromMinute.Enabled = true;
					this.cboToHour.Enabled = true;
					this.cboToMinute.Enabled = true;
				}
				else
				{
					this.cboFromHour.Enabled = false;
					this.cboFromMinute.Enabled = false;
					this.cboToHour.Enabled = false;
					this.cboToMinute.Enabled = false;

					this.cboFromHour.SelectedIndex = -1;
					this.cboFromMinute.SelectedIndex = -1;
					this.cboToHour.SelectedIndex = -1;
					this.cboToMinute.SelectedIndex = -1;
				}
			}
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

        public void btnSave_Click(object sender, System.EventArgs e)
		{
            try
            {
                string strTimeFrom = "";
                string strTimeTo = "";

                if (this.txtReader.Text.Trim() == "")
                {
                    MessageBox.Show("Enter Description.");
                    this.txtReader.Focus();
                    return;
                }
                else
                {
                    pvtstrDeviceDesc = this.txtReader.Text.Trim();
                }

                pvtstrTimeAttendClockFirstLastInd = "";

                if (this.rbnTimeAttend.Checked == true)
                {
                    pvtstrDeviceUsage = "T";
                }
                else
                {
                    if (this.rbnAccess.Checked == true)
                    {
                        pvtstrDeviceUsage = "A";
                    }
                    else
                    {
                        if (this.rbnBoth.Checked == true)
                        {
                            pvtstrDeviceUsage = "B";

                            if (this.rbnClockNormal.Checked == true)
                            {
                                pvtstrTimeAttendClockFirstLastInd = "N";
                            }
                            else
                            {
                                if (this.rbnClockFirstLast.Checked == true)
                                {
                                    pvtstrTimeAttendClockFirstLastInd = "Y";
                                }
                                else
                                {
                                    MessageBox.Show("Choose Access / Time & Attendance Option");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Choose Device Usage");
                            return;
                        }
                    }
                }

                if (this.rbnInOnly.Checked == true)
                {
                    pvtstrClockInOutParm = "I";
                }
                else
                {
                    if (this.rbnOutOnly.Checked == true)
                    {
                        pvtstrClockInOutParm = "O";
                    }
                    else
                    {
                        if (this.rbnDynamic.Checked == true)
                        {
                            pvtstrClockInOutParm = "D";
                        }
                        else
                        {
                            if (this.rbnClockRange.Checked == true)
                            {
                                if (this.cboFromHour.SelectedIndex == -1)
                                {
                                    this.cboFromHour.Focus();
                                    MessageBox.Show("Select From Hour");
                                    return;
                                }

                                if (this.cboFromMinute.SelectedIndex == -1)
                                {
                                    this.cboFromMinute.Focus();
                                    MessageBox.Show("Select From Minute");
                                    return;
                                }

                                if (this.cboToHour.SelectedIndex == -1)
                                {
                                    this.cboToHour.Focus();
                                    MessageBox.Show("Select To Hour");
                                    return;
                                }

                                if (this.cboToMinute.SelectedIndex == -1)
                                {
                                    this.cboToMinute.Focus();
                                    MessageBox.Show("Select To Minute");
                                    return;
                                }

                                pvtstrClockInOutParm = "R";

                                strTimeFrom = this.cboFromHour.SelectedItem.ToString() + this.cboFromMinute.SelectedItem.ToString();
                                strTimeTo = this.cboToHour.SelectedItem.ToString() + this.cboToMinute.SelectedItem.ToString();

                                pvtintClockInRangeFrom = Convert.ToInt32(strTimeFrom);
                                pvtintClockInRangeTo = Convert.ToInt32(strTimeTo);
                            }
                            else
                            {
                                MessageBox.Show("Select a " + this.grbClockOptions.Text);
                                return;
                            }
                        }
                    }
                }

                if (this.cboLockOut.SelectedIndex == -1)
                {
                    MessageBox.Show("Select Lock Out Time.");
                    this.cboLockOut.Focus();
                    return;
                }
                else
                {
                    pvtintLockOutMinutes = Convert.ToInt32(this.cboLockOut.SelectedItem.ToString());
                }
              
                if (this.cboCompany.SelectedIndex == -1)
                {
                    MessageBox.Show("Select Company that owns Clock Reader.");
                    this.cboCompany.Focus();
                    return;
                }
                else
                {
                    pvtintCompanyNo = Convert.ToInt32(pvtDataSet.Tables["Company"].Rows[this.cboCompany.SelectedIndex]["COMPANY_NO"]);
                }

                if (this.rbnWAN.Checked == true)
                {
                    pvtstrLanWanInd = "W";
                }
                else
                {
                    pvtstrLanWanInd = "L";
                }

                if (this.Text.IndexOf(" - New") > 0)
                {
                    DataRowView drvDataRowView = this.pvtDataView.AddNew();
                    //Set Key for Find
                    drvDataRowView["DEVICE_NO"] = 0;
                    drvDataRowView["DEVICE_DESC"] = pvtstrDeviceDesc;
                    drvDataRowView["DEVICE_USAGE"] = pvtstrDeviceUsage;
                    drvDataRowView["TIME_ATTEND_CLOCK_FIRST_LAST_IND"] = pvtstrTimeAttendClockFirstLastInd;
                    drvDataRowView["CLOCK_IN_OUT_PARM"] = pvtstrClockInOutParm;
                    drvDataRowView["CLOCK_IN_RANGE_FROM"] = pvtintClockInRangeFrom;
                    drvDataRowView["CLOCK_IN_RANGE_TO"] = pvtintClockInRangeTo;
                    drvDataRowView["LOCK_OUT_MINUTES"] = pvtintLockOutMinutes;
                    drvDataRowView["COMPANY_NO"] = pvtintCompanyNo;
                    drvDataRowView["LAN_WAN_IND"] = pvtstrLanWanInd;

                    object[] objParm = new object[1];
                    objParm[0] = drvDataRowView;

                    drvDataRowView["DEVICE_NO"] = clsTAUtilities.DynamicFunction("Insert_Record", objParm);

                    drvDataRowView.EndEdit();
                }
                else
                {
                    pvtDataView[pvtintDataViewReaderIndex]["DEVICE_DESC"] = pvtstrDeviceDesc;
                    pvtDataView[pvtintDataViewReaderIndex]["DEVICE_USAGE"] = pvtstrDeviceUsage;
                    pvtDataView[pvtintDataViewReaderIndex]["TIME_ATTEND_CLOCK_FIRST_LAST_IND"] = pvtstrTimeAttendClockFirstLastInd;
                    pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_OUT_PARM"] = pvtstrClockInOutParm;
                    pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_RANGE_FROM"] = pvtintClockInRangeFrom;
                    pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_RANGE_TO"] = pvtintClockInRangeTo;
                    pvtDataView[pvtintDataViewReaderIndex]["LOCK_OUT_MINUTES"] = pvtintLockOutMinutes;
                    pvtDataView[pvtintDataViewReaderIndex]["COMPANY_NO"] = pvtintCompanyNo;
                    pvtDataView[pvtintDataViewReaderIndex]["LAN_WAN_IND"] = pvtstrLanWanInd;

                    object[] objParm = new object[1];
                    objParm[0] = pvtDataView[pvtintDataViewReaderIndex].Row;

                    clsTAUtilities.DynamicFunction("Update_Record", objParm);

                    this.flxgReader[this.flxgReader.Row, 2] = pvtstrDeviceDesc;
                }

                pvtDataSet.AcceptChanges();

                if (this.Text.IndexOf(" - New") > 0)
                {
                    Load_CurrentForm_Records();
                }

                btnCancel_Click(sender, e);
            }
            catch (Exception ex)
            {
                clsTAUtilities.ErrorHandler(ex);
            }
		}

		private void Set_Form_For_Edit()
		{
			this.flxgReader.Enabled = false;

			this.txtReader.Enabled = true;

			this.picReaderLock.Visible = true;

			if (this.Text.IndexOf(" - New") > 0)
			{
				this.cboLockOut.SelectedIndex = -1;
                this.cboCompany.SelectedIndex = -1;
				this.txtReader.Text = "";
                this.txtDeviceNo.Text = "";
                this.rbnLAN.Checked = true;
			}

            this.cboCompany.Enabled = true;
			this.rbnTimeAttend.Checked = true;
		
			this.rbnAccess.Enabled = true;
			this.rbnTimeAttend.Enabled = true;
			this.rbnBoth.Enabled = true;
			this.cboLockOut.Enabled = true;

			if (this.rbnBoth.Checked == true)
			{
				this.rbnClockNormal.Enabled = true;
				this.rbnClockFirstLast.Enabled = true;
			}

			this.rbnInOnly.Enabled = true;
			this.rbnOutOnly.Enabled = true;

			if (this.rbnTimeAttend.Checked == true)
			{
				this.rbnDynamic.Enabled = true;
				this.rbnClockRange.Enabled = true;
			}

			if (this.rbnClockRange.Checked == true)
			{
				this.cboFromHour.Enabled = true;
				this.cboFromMinute.Enabled = true;
				this.cboToHour.Enabled = true;
				this.cboToMinute.Enabled = true;
			}

            this.rbnWAN.Enabled = true;
            this.rbnLAN.Enabled = true;

			this.btnNew.Enabled = false;
			this.btnUpdate.Enabled = false;
			this.btnDelete.Enabled = false;
			this.btnSave.Enabled = true;
			this.btnCancel.Enabled = true;
		}

        public void btnUpdate_Click(object sender, System.EventArgs e)
		{
			this.Text += " - Update";

			Set_Form_For_Edit();

			this.txtReader.Focus();
		}

        public void btnCancel_Click(object sender, System.EventArgs e)
		{
            if (this.Text.LastIndexOf(" - ") != -1)
            {
                this.Text = this.Text.Substring(0, this.Text.LastIndexOf(" - "));
            }

			this.picReaderLock.Visible = false;

			this.txtReader.Enabled = false;

			this.rbnAccess.Enabled = false;
			this.rbnTimeAttend.Enabled = false;
			this.rbnBoth.Enabled = false;

			this.rbnClockNormal.Enabled = false;
			this.rbnClockFirstLast.Enabled = false;
		
			this.cboLockOut.Enabled = false;
            this.cboCompany.Enabled = false;

			this.rbnInOnly.Enabled = false;
			this.rbnOutOnly.Enabled = false;
			this.rbnDynamic.Enabled = false;
			this.rbnClockRange.Enabled = false;

			this.cboFromHour.Enabled = false;
			this.cboFromMinute.Enabled = false;
			this.cboToHour.Enabled = false;
			this.cboToMinute.Enabled = false;

            this.rbnWAN.Enabled = false;
            this.rbnLAN.Enabled = false;

			this.btnNew.Enabled = true;
			this.btnUpdate.Enabled = true;
			this.btnDelete.Enabled = true;
			this.btnSave.Enabled = false;
			this.btnCancel.Enabled = false;

			this.flxgReader.Enabled = true;

			int intReaderRow = this.flxgReader.Row;

			this.flxgReader.Row = 0;
			this.flxgReader.Row = intReaderRow;
		}

		private void frmClock_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.btnSave.Enabled == true)
			{
				DialogResult dlgResult = MessageBox.Show("Are you sure you want to Close this Form without Saving?",
					this.Text, 
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question);

				if (dlgResult == DialogResult.No)
				{
					e.Cancel = true;
				}
			}
		}

		private void flxgReader_AfterRowColChange(object sender, C1.Win.C1FlexGrid.RangeEventArgs e)
		{
			try
			{
				if (this.flxgReader.Row > 0)
				{
                    pvtintDataViewReaderIndex = Convert.ToInt32(flxgReader[flxgReader.Row, 3]);

					this.txtReader.Text = flxgReader[flxgReader.Row,2].ToString();
                    this.txtDeviceNo.Text = pvtDataView[pvtintDataViewReaderIndex]["DEVICE_NO"].ToString();

					if (pvtDataView[pvtintDataViewReaderIndex]["DEVICE_USAGE"].ToString() == "A")
					{
						this.rbnAccess.Checked = true;
					}
					else
					{
						if (pvtDataView[pvtintDataViewReaderIndex]["DEVICE_USAGE"].ToString() == "T")
						{
							this.rbnTimeAttend.Checked = true;
						}
						else
						{
							if (pvtDataView[pvtintDataViewReaderIndex]["DEVICE_USAGE"].ToString() == "B")
							{
								this.rbnBoth.Checked = true;
							}
							else
							{
								this.rbnTimeAttend.Checked = false;
								this.rbnAccess.Checked = false;
								this.rbnBoth.Checked = false;
							}
						}
					}

					if (pvtDataView[pvtintDataViewReaderIndex]["TIME_ATTEND_CLOCK_FIRST_LAST_IND"].ToString() == "Y")
					{
						this.rbnClockFirstLast.Checked = true;
					}
					else
					{
						if (pvtDataView[pvtintDataViewReaderIndex]["TIME_ATTEND_CLOCK_FIRST_LAST_IND"].ToString() == "N")
						{
							this.rbnClockNormal.Checked = true;
						}
						else
						{
							this.rbnClockNormal.Checked = false;
							this.rbnClockFirstLast.Checked = false;
						}
					}

					this.cboFromHour.SelectedIndex = -1;
					this.cboFromMinute.SelectedIndex = -1;
					this.cboToHour.SelectedIndex = -1;
					this.cboToMinute.SelectedIndex = -1;

					if (pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_OUT_PARM"].ToString() == "")
					{
						this.rbnClockRange.Checked = false;
					}
					else
					{
						if (pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_OUT_PARM"].ToString() == "I")
						{
							this.rbnInOnly.Checked = true;
						}
						else
						{
							if (pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_OUT_PARM"].ToString() == "O")
							{
								this.rbnOutOnly.Checked = true;
							}
							else
							{
								if (pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_OUT_PARM"].ToString() == "D")
								{
									this.rbnDynamic.Checked = true;
								}
								else
								{
									//Range
									this.rbnClockRange.Checked = true;

									string strTimeFrom = pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_RANGE_FROM"].ToString().PadLeft(4,'0');
									string strTimeTo = pvtDataView[pvtintDataViewReaderIndex]["CLOCK_IN_RANGE_TO"].ToString().PadLeft(4,'0');

									for (int intRow = 0;intRow < this.cboFromHour.Items.Count;intRow++)
									{
										if (this.cboFromHour.Items[intRow].ToString() == strTimeFrom.Substring(0,2))
										{
											this.cboFromHour.SelectedIndex = intRow;
											break;
										}
									}

									for (int intRow = 0;intRow < this.cboFromMinute.Items.Count;intRow++)
									{
										if (this.cboFromMinute.Items[intRow].ToString() == strTimeFrom.Substring(2))
										{
											this.cboFromMinute.SelectedIndex = intRow;
											break;
										}
									}

									for (int intRow = 0;intRow < this.cboToHour.Items.Count;intRow++)
									{
										if (this.cboToHour.Items[intRow].ToString() == strTimeTo.Substring(0,2))
										{
											this.cboToHour.SelectedIndex = intRow;
											break;
										}
									}

									for (int intRow = 0;intRow < this.cboToMinute.Items.Count;intRow++)
									{
										if (this.cboToMinute.Items[intRow].ToString() == strTimeTo.Substring(2))
										{
											this.cboToMinute.SelectedIndex = intRow;
											break;
										}
									}
								}
							}
						}
					}
					
					if (pvtDataView[pvtintDataViewReaderIndex]["LOCK_OUT_MINUTES"] == System.DBNull.Value)
					{
						this.cboLockOut.SelectedIndex = -1;
					}
					else
					{
						this.cboLockOut.Text = pvtDataView[pvtintDataViewReaderIndex]["LOCK_OUT_MINUTES"].ToString();
					}

					this.cboCompany.SelectedIndex = -1;

					if (pvtDataView[pvtintDataViewReaderIndex]["COMPANY_NO"] != System.DBNull.Value)
					{
						for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["Company"].Rows.Count;intRowCount++)
						{
							if (Convert.ToInt32(pvtDataView[pvtintDataViewReaderIndex]["COMPANY_NO"]) == Convert.ToInt32(pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_NO"]))
							{
								this.cboCompany.SelectedIndex = intRowCount;
								break;
							}
						}
					}

                    //WAN - Clock Pings Server every Minute
                    if (pvtDataView[pvtintDataViewReaderIndex]["LAN_WAN_IND"].ToString() == "W")
                    {
                        this.rbnWAN.Checked = true;
                    }
                    else
                    {
                        //LAN - Clock Pings Server every 15 Seconds
                        this.rbnLAN.Checked = true;
                    }
				}
			}
			catch (Exception eException)
			{
				clsTAUtilities.ErrorHandler(eException);
			}
		}
		
		public void btnNew_Click(object sender, System.EventArgs e)
		{
			this.Text += " - New";

			Set_Form_For_Edit();

			this.txtReader.Focus();
		}

		private void rbnAccess_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.btnSave.Enabled == true)
			{
				if (rbnAccess.Checked == true)
				{
					this.rbnDynamic.Enabled = false;
					this.rbnClockRange.Enabled = false;

					this.rbnDynamic.Checked = false;
					this.rbnClockRange.Checked = false;
					
					this.rbnClockNormal.Checked = false;
					this.rbnClockFirstLast.Checked = false;

					this.rbnClockNormal.Enabled = false;
					this.rbnClockFirstLast.Enabled = false;
				}
			}
		}

        public void btnDelete_Click(object sender, System.EventArgs e)
		{
			try
			{
				DialogResult dlgResult = MessageBox.Show("Delete Device '" + pvtDataView[pvtintDataViewReaderIndex]["DEVICE_DESC"].ToString() + "'",
					this.Text, 
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning);

				if (dlgResult == DialogResult.Yes)
				{
                    object[] objParm = new object[1];
                    objParm[0] = Convert.ToInt32(pvtDataView[pvtintDataViewReaderIndex]["DEVICE_NO"]);
                   
                    clsTAUtilities.DynamicFunction("Delete_Record", objParm);
					                    					
					pvtDataView[this.flxgReader.Row - 1].Delete();

					this.pvtDataSet.AcceptChanges();

					this.Load_CurrentForm_Records();
				}
			}
			catch (Exception eException)
			{
				clsTAUtilities.ErrorHandler(eException);
			}
		}
		
		private void rbnBoth_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.btnSave.Enabled == true)
			{
				if (this.rbnBoth.Checked == true)
				{
					this.rbnClockNormal.Enabled = true;
					this.rbnClockFirstLast.Enabled = true;

					this.rbnDynamic.Enabled = false;
					this.rbnClockRange.Enabled = false;

					this.rbnDynamic.Checked = false;
					this.rbnClockRange.Checked = false;
				}
			}
		}

		private void rbnTimeAttend_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.btnSave.Enabled == true)
			{
				if (rbnTimeAttend.Checked == true)
				{
					this.rbnDynamic.Enabled = true;
					this.rbnClockRange.Enabled = true;

					this.rbnClockNormal.Checked = false;
					this.rbnClockFirstLast.Checked = false;

					this.rbnClockNormal.Enabled = false;
					this.rbnClockFirstLast.Enabled = false;
				}
			}
		}

        private void frmClock_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }
	}
}
