using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Configuration;


namespace InteractPayroll
{
    public partial class frmProductUpload : Form
    {
        clsISUtilities clsISUtilities;
        clsCrc32 clsCrc32;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private DataView pvtUserDataView;
        private DataTable pvtDataTable;
        private DataRow pvtDataRow;

        private byte[] pvtbytCompress;
       
        public string pubstrConfigSettings = "";
       
        private double pvtdblUserNo = -1;
        private double pvtdblSelectedUserNo = -1;

        private string pvtstrLayerID = "";

        private string pvtstrPayrollPath = "";
        private string pvtstrTimeAttendPath = "";
        private string pvtstrTimeAttendInternetPath = "";
        
        private int pvtintProductRowIndex = -1;
        private int pvtintVersionRowIndex = -1;
        
        private object[] pvtobjRowArray = new object[1];

        private object[] pvtobjCostCentreRowArray = new object[2];

        private bool pvtblnProductDataGridViewLoaded = false;
        private bool pvtblnFileDataGridViewLoaded = false;
        private bool pvtblnVersionDataGridViewLoaded = false;
        private bool pvtblnFileToUploadDataGridViewLoaded = false;
        private bool pvtblnCompanyDataGridViewLoaded = false;

        private bool pvtblnUpdateGoogleDrive = false;

        DataGridViewCellStyle WarningDataGridViewCellStyle;

        public frmProductUpload()
        {
            InitializeComponent();
        }

        private void frmProductUpload_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsCrc32 = new clsCrc32();

                WarningDataGridViewCellStyle = new DataGridViewCellStyle();
                WarningDataGridViewCellStyle.BackColor = Color.Orange;
                WarningDataGridViewCellStyle.SelectionBackColor = Color.Orange;

                string strURLPath = ConfigurationSettings.AppSettings["WebServerIP"];
                
                this.Text = "Product Upload - " + strURLPath;

                AppDomain.CurrentDomain.SetData("URLPath", strURLPath);

#if (DEBUG)

                ////Added Temp
                //AppDomain.CurrentDomain.SetData("URLPath", "");
                //strURLPath = "";


                if (strURLPath != "")
                {
                    DialogResult myDialogResult = MessageBox.Show("You are going to Use the Web Service Layer (Live System).\n\nWould you like to Continue.",
                    "Error",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error);

                    if (myDialogResult == System.Windows.Forms.DialogResult.No)
                    {
                        this.Close();
                        return;
                    }
                }
#endif
                string[] strDriveLetter = AppDomain.CurrentDomain.BaseDirectory.ToString().Split(':');

                pvtstrPayrollPath = strDriveLetter[0] + ":\\" + ConfigurationSettings.AppSettings["PayrollLiveFiles"];
                pvtstrTimeAttendPath = strDriveLetter[0] + ":\\" + ConfigurationSettings.AppSettings["TimeAttendanceLiveFiles"];
                pvtstrTimeAttendInternetPath = strDriveLetter[0] + ":\\" + ConfigurationSettings.AppSettings["TimeAttendanceInternetLiveFiles"];

                //ERROL CHANGED TO FIX PROGRAM
                //pvtstrPayrollPath = AppDomain.CurrentDomain.BaseDirectory + "\\_PayrollLive Files";
                //pvtstrTimeAttendPath = AppDomain.CurrentDomain.BaseDirectory + "\\_TimeAttendanceClientLive Files";
                //pvtstrTimeAttendInternetPath = AppDomain.CurrentDomain.BaseDirectory + "\\_TimeAttendanceInternetLive Files";

                AppDomain.CurrentDomain.SetData("KillApp", "N");

                clsISUtilities = new InteractPayroll.clsISUtilities(this, "busProductUpload");

                this.lblVersions.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblProduct.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblListUsers.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenUsers.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblFilesToUpload.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblFilesOnServer.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblListCostCentres.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblChosenCostCentres.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                Cursor = Cursors.WaitCursor;
                
                this.dgvVersionDataGridView.Rows.Add("Beta");
                this.dgvVersionDataGridView.Rows.Add("Current");

                this.pvtblnVersionDataGridViewLoaded = true;

                this.dgvProductDataGridView.Rows.Add("Payroll Internet - Client");
                this.dgvProductDataGridView.Rows.Add("Payroll Internet - Server");
                
                this.dgvProductDataGridView.Rows.Add("Time Attendance - Client");
                this.dgvProductDataGridView.Rows.Add("Time Attendance - Server");

                this.dgvProductDataGridView.Rows.Add("Time Attendance Internet - Client");
                
                pvtblnProductDataGridViewLoaded = true;
                
                object[] objParm = null;
                pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);
                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                this.Set_DataGridView_SelectedRowIndex(dgvVersionDataGridView, 0);

                Cursor = Cursors.Default;
            }
            catch (Exception eException)
            {
                string strExceptionError;
                string strAreaFrom = eException.StackTrace;

                DateTime dtDateTime = DateTime.Now;

                String strDateTime = dtDateTime.ToString("yyyy/MM/dd HH:mm:ss");

                strExceptionError = "Date/Time  : " + strDateTime
                    + "\r\n" + "Where      : " + strAreaFrom
                    + "\r\n" + "Error Desc : " + eException.Message;

                MessageBox.Show(strExceptionError,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                FileInfo fiErrorFile = new FileInfo("ProductUploadError.txt");

                StreamWriter swErrorStreamWriter = fiErrorFile.AppendText();

                swErrorStreamWriter.WriteLine("");
                swErrorStreamWriter.WriteLine(strExceptionError);

                swErrorStreamWriter.Close();
            }
        }

        public int Get_DataGridView_SelectedRowIndex(DataGridView myDataGridView)
        {
            int intReturnIndex = -1;

            if (myDataGridView.SelectedRows.Count > 0)
            {
                if (myDataGridView.SelectedRows[0].Selected == true)
                {
                    intReturnIndex = myDataGridView.SelectedRows[0].Index;
                }
            }

            return intReturnIndex;
        }

        public void Set_DataGridView_SelectedRowIndex(DataGridView myDataGridView, int intRow)
        {
            //Fires DataGridView RowEnter Function
            if (myDataGridView.CurrentCell.RowIndex == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvProductDataGridView":

                        this.dgvProductDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvVersionDataGridView":

                        this.dgvVersionDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvUserDataGridView":

                        this.dgvUserDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvUserChosenDataGridView":

                        this.dgvUserChosenDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvCostCentreDataGridView":

                        this.dgvCostCentreDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvCostCentreChosenDataGridView":

                        this.dgvCostCentreChosenDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;
                        
                    default:

                        MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                myDataGridView.CurrentCell = myDataGridView[0, intRow];
            }
        }

        private void Clear_DataGridView(DataGridView myDataGridView)
        {
            myDataGridView.Rows.Clear();

            if (myDataGridView.SortedColumn != null)
            {
                myDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
        }
        
        private void Set_Save_User_Values()
        {
            if (this.dgvUserDataGridView.Rows.Count > 0)
            {
                pvtdblUserNo = Convert.ToDouble(this.dgvUserDataGridView[3, this.Get_DataGridView_SelectedRowIndex(dgvUserDataGridView)].Value.ToString());
            }
            else
            {
                pvtdblUserNo = -1;
            }

            if (this.dgvUserChosenDataGridView.Rows.Count > 0)
            {
                pvtdblSelectedUserNo = Convert.ToDouble(this.dgvUserChosenDataGridView[3, this.Get_DataGridView_SelectedRowIndex(this.dgvUserChosenDataGridView)].Value.ToString());
            }
            else
            {
                pvtdblSelectedUserNo = -1;
            }
        }

        private void btnUserAdd_Click(object sender, System.EventArgs e)
        {
            if (this.dgvUserDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvUserDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvUserDataGridView)];

                this.dgvUserDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvUserChosenDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvUserDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvUserDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvUserChosenDataGridView, this.dgvUserChosenDataGridView.Rows.Count - 1);
            }
        }

        private void btnUserDelete_Click(object sender, System.EventArgs e)
        {
            if (this.dgvUserChosenDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvUserChosenDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvUserChosenDataGridView)];

                this.dgvUserChosenDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvUserDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvUserChosenDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvUserChosenDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvUserDataGridView, this.dgvUserDataGridView.Rows.Count - 1);
            }
        }

        private void btnUserDeleteAll_Click(object sender, System.EventArgs e)
        {
        btnUserDeleteAll_Click_Continue:

            for (int intRowCount = 0; intRowCount < this.dgvUserChosenDataGridView.Rows.Count; intRowCount++)
            {
                this.btnUserDelete_Click(sender, e);

                goto btnUserDeleteAll_Click_Continue;
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        
        private void btnUpdate_Click(object sender, System.EventArgs e)
        {
            this.Text += " - Update";

            Set_Form_For_Edit();
        }

        private void Set_Form_For_Edit()
        {
            if (this.Text.IndexOf(" - New") > -1)
            {
                this.Clear_DataGridView(this.dgvFileDataGridView);
                this.txtVersion.Text = "";
                this.txtVersion.Enabled = true;
            }

            this.dgvVersionDataGridView.Enabled = false;
            this.dgvProductDataGridView.Enabled = false;
            
            this.btnUpdate.Enabled = false;
           
            this.btnSave.Enabled = true;
            this.btnRefresh.Enabled = true;
           
            this.btnUserAdd.Enabled = true;
            this.btnUserDelete.Enabled = true;
            this.btnUserDeleteAll.Enabled = true;

            this.btnCostCentreAdd.Enabled = true;
            this.btnCostCentreDelete.Enabled = true;
            this.btnCostCentreDeleteAll.Enabled = true;

            this.btnDeleteFiles.Enabled = true;

            if (pvtDataSet.Tables["TempFiles"] != null)
            {
                pvtDataSet.Tables["TempFiles"].Clear();
            }

            if (pvtblnUpdateGoogleDrive == true)
            {
                MessageBox.Show("Remember to Update Google Drive with File Changes.",
                  this.Text,
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Information);
            }

            if (this.txtVersion.Text == "Beta")
            {
                if (dgvFileDataGridView.Rows.Count > 0)
                {
                    this.btnMoveToCurrent.Enabled = true;
                }
            }
        }

        private void Set_Form_For_Read()
        {
            if (this.Text.IndexOf(" - Update") > -1)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf(" - Update"));
            }

            if (this.Text.IndexOf(" - New") > -1)
            {
                this.Text = this.Text.Substring(0, this.Text.IndexOf(" - New"));
            }

            this.dgvVersionDataGridView.Enabled = true;
            this.dgvProductDataGridView.Enabled = true;
            this.txtVersion.Enabled = false;
            
            this.btnUpdate.Enabled = true;
          
            this.btnSave.Enabled = false;
            this.btnRefresh.Enabled = false;

            this.btnUserAdd.Enabled = false;
            this.btnUserDelete.Enabled = false;
            this.btnUserDeleteAll.Enabled = false;

            this.btnCostCentreAdd.Enabled = false;
            this.btnCostCentreDelete.Enabled = false;
            this.btnCostCentreDeleteAll.Enabled = false;
            
            this.btnDeleteFiles.Enabled = false;

            this.btnMoveToCurrent.Enabled = false;
        }

        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            Set_Form_For_Read();

            pvtintProductRowIndex = -1;
            this.Set_DataGridView_SelectedRowIndex(this.dgvProductDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvProductDataGridView));
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            string strCRC32Value = "";
          
            if (this.Get_DataGridView_SelectedRowIndex(this.dgvProductDataGridView) == 0)
            {
                if (this.txtVersion.Text != "Current")
                {
                    if (this.dgvUserChosenDataGridView.Rows.Count == 0
                    && this.dgvCostCentreChosenDataGridView.Rows.Count == 0)
                    {
                         MessageBox.Show("Capture Cost Centre/s And/Or User/s.",
                                this.Text,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                            return;
                    }
                }
            }

            Cursor = Cursors.WaitCursor;

            int intNumberOfBytesToRead = 10000;

            byte[] pvtbytes = new byte[intNumberOfBytesToRead];

            DateTime dtFileLastUpdated;
            int intFileSize;
            int intCompressedSize;
            string strVersionNumber;
            bool blnComplete = false;
            string strProgramID = "";
          
            for (int intRow = 0; intRow < this.dgvFileToUploadDataGridView.Rows.Count; intRow++)
            {
                grbFileUpload.Visible = true;

                this.lblFileName.Text = this.dgvFileToUploadDataGridView[0, intRow].Value.ToString();
                this.prgProgressBar.Value = 0;

                grbFileUpload.Refresh();

                //ReInitialised 
                intNumberOfBytesToRead = 10000;

                strProgramID = this.dgvFileToUploadDataGridView[1, intRow].Value.ToString();
                dtFileLastUpdated = Convert.ToDateTime(this.dgvFileToUploadDataGridView[2, intRow].Value.ToString().Replace("  ", " "));
                intFileSize = Convert.ToInt32(this.dgvFileToUploadDataGridView[3, intRow].Value);
                strVersionNumber = this.dgvFileToUploadDataGridView[4, intRow].Value.ToString();
                blnComplete = false;

                pvtbytes = null;
                pvtbytes = new byte[intNumberOfBytesToRead];

                FileStream pvtfsFileStream = new FileStream(this.txtFilePath.Text + "//" + this.dgvFileToUploadDataGridView[0, intRow].Value.ToString(), FileMode.Open, FileAccess.Read);

                //Read FileStream To Bytes Array
                byte[] ByteArray = new byte[pvtfsFileStream.Length];
                pvtfsFileStream.Read(ByteArray, 0, Convert.ToInt32(pvtfsFileStream.Length));

                //New CRC32 Value
                strCRC32Value = "";

                foreach (byte b in clsCrc32.ComputeHash(ByteArray))
                {
                    strCRC32Value += b.ToString("x2").ToLower();
                }

                //Open memory stream (Compressed)
                MemoryStream msMemoryStream = new MemoryStream();

                System.IO.Compression.GZipStream GZipStreamCompressed = new GZipStream(msMemoryStream, CompressionMode.Compress, true);
                GZipStreamCompressed.Write(ByteArray, 0, (int)ByteArray.Length);
                GZipStreamCompressed.Flush();
                GZipStreamCompressed.Close();

                //This is The File Length
                int intNumberBlocks = Convert.ToInt32(msMemoryStream.Length / intNumberOfBytesToRead);
                int intNumberBytesAlreadyRead = 0;
                int intNumberBytesRead = 0;

                if (intNumberBlocks * intNumberOfBytesToRead != msMemoryStream.Length)
                {
                    intNumberBlocks += 1;
                }

                this.prgProgressBar.Maximum = intNumberBlocks;
                this.prgProgressBar.Minimum = 0;

                BinaryReader pvtbrBinaryReader = new BinaryReader(msMemoryStream);

                intCompressedSize = Convert.ToInt32(msMemoryStream.Length);

                for (int intBlockNumber = 1; intBlockNumber <= intNumberBlocks; intBlockNumber++)
                {
                    if (intBlockNumber == intNumberBlocks)
                    {
                        intNumberOfBytesToRead = Convert.ToInt32(msMemoryStream.Length - intNumberBytesAlreadyRead);

                        pvtbytes = null;
                        pvtbytes = new byte[intNumberOfBytesToRead];

                        blnComplete = true;
                    }

                    pvtbrBinaryReader.BaseStream.Position = intNumberBytesAlreadyRead;

                    intNumberBytesRead = pvtbrBinaryReader.Read(pvtbytes, 0, intNumberOfBytesToRead);

                    object[] objParm = new object[13];
                    objParm[0] = pvtstrLayerID;
                    objParm[1] = strProgramID;

                    objParm[2] = this.txtVersion.Text;
                    objParm[3] = intBlockNumber;

                    objParm[4] = dgvFileToUploadDataGridView[0, intRow].Value.ToString();
                    objParm[5] = pvtbytes;
                    objParm[6] = dtFileLastUpdated;

                    objParm[7] = intFileSize;
                    objParm[8] = intCompressedSize;
                    objParm[9] = strVersionNumber;

                    objParm[10] = blnComplete;
                    objParm[11] = this.dgvProductDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvProductDataGridView)].Value.ToString();
                    objParm[12] = strCRC32Value;

                    clsISUtilities.DynamicFunction("Insert_File_Chunk", objParm);

                    intNumberBytesAlreadyRead += intNumberBytesRead;

                    this.prgProgressBar.Value += 1;

                    this.grbFileUpload.Refresh();
                }

                string strFromFile = pvtfsFileStream.Name;
                string strToFile = pvtfsFileStream.Name.Replace("GoingLive",this.txtVersion.Text + "Live");
                
                pvtfsFileStream.Close();
                msMemoryStream.Close();
                pvtbrBinaryReader.Close();

                pvtfsFileStream = null;
                msMemoryStream = null;
                pvtbrBinaryReader = null;

                File.Copy(strFromFile, strToFile,true);
                File.Delete(strFromFile);

                this.dgvFileToUploadDataGridView.Rows.RemoveAt(intRow);

                //Reset - Row Removed
                intRow -= 1;
            }

            grbFileUpload.Visible = false;

            pvtDataSet.Tables["TempUsers"].Rows.Clear();
            pvtDataSet.Tables["TempPayCategories"].Rows.Clear();

            for (int intRow = 0; intRow < this.dgvUserChosenDataGridView.Rows.Count; intRow++)
            {
                //NODE_PATH
                pvtobjRowArray[0] = this.dgvUserChosenDataGridView[3, intRow].Value.ToString();

                pvtDataRow = pvtDataSet.Tables["TempUsers"].NewRow();
                pvtDataRow.ItemArray = pvtobjRowArray;

                pvtDataSet.Tables["TempUsers"].Rows.Add(pvtDataRow);
            }

            for (int intRow = 0; intRow < this.dgvCostCentreChosenDataGridView.Rows.Count; intRow++)
            {
                //NODE_PATH
                pvtobjCostCentreRowArray[0] = this.dgvCostCentreChosenDataGridView[2, intRow].Value.ToString();
                pvtobjCostCentreRowArray[1] = this.dgvCostCentreChosenDataGridView[3, intRow].Value.ToString();

                pvtDataRow = pvtDataSet.Tables["TempPayCategories"].NewRow();
                pvtDataRow.ItemArray = pvtobjCostCentreRowArray;

                pvtDataSet.Tables["TempPayCategories"].Rows.Add(pvtDataRow);
            }

            pvtbytCompress = clsISUtilities.Compress_DataSet(pvtDataSet);

            //Not 'Payroll Internet - Server'
            if (this.Get_DataGridView_SelectedRowIndex(dgvProductDataGridView) != 1)
            {
                object[] objParm = new object[2];
                objParm[0] = pvtbytCompress;
                objParm[1] = this.txtVersion.Text;

                clsISUtilities.DynamicFunction("Insert_Version_Records", objParm);
            }

            Set_Form_For_Read();
            
            pvtintProductRowIndex = -1;
            this.Set_DataGridView_SelectedRowIndex(dgvProductDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvProductDataGridView));
            
            Cursor = Cursors.Default;
        }

       
        
        private void btnDeleteFiles_Click(object sender, EventArgs e)
        {
            object[] objParm = new object[4];

            objParm[0] = this.pvtstrLayerID;
            objParm[1] = this.dgvVersionDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvVersionDataGridView)].Value.ToString();
            objParm[2] = this.dgvFileDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvFileDataGridView)].Value.ToString();
            objParm[3] = dgvProductDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvProductDataGridView)].Value.ToString();

            clsISUtilities.DynamicFunction("Delete_File", objParm);

            btnRefresh_Click(sender, e);
        }

        private void dgvProductDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvProductDataGridView.Rows.Count > 0
            && this.pvtblnProductDataGridViewLoaded == true)
            {
                if (pvtintProductRowIndex != e.RowIndex)
                {
                    pvtintProductRowIndex = e.RowIndex;

                    if (pvtintProductRowIndex == 0
                        || pvtintProductRowIndex == 2
                        || pvtintProductRowIndex == 4)
                    {
                        pvtstrLayerID = "P";
                    }
                    else
                    {
                        pvtstrLayerID = "S";
                    }

                    this.Clear_DataGridView(this.dgvFileDataGridView);
                    this.Clear_DataGridView(this.dgvFileToUploadDataGridView);

                    this.Clear_DataGridView(this.dgvUserDataGridView);
                    this.Clear_DataGridView(this.dgvUserChosenDataGridView);

                    this.Clear_DataGridView(this.dgvCostCentreDataGridView);
                    this.Clear_DataGridView(this.dgvCostCentreChosenDataGridView);

                    if (this.pvtDataSet.Tables["SelectedUsers"] != null)
                    {
                        this.pvtDataSet.Tables["SelectedUsers"].Clear();
                    }

                    if (this.pvtDataSet.Tables["SelectedPayCategories"] != null)
                    {
                        this.pvtDataSet.Tables["SelectedPayCategories"].Clear();
                    }

                    pvtblnUpdateGoogleDrive = false;

                    if (e.RowIndex == 0
                        || e.RowIndex == 1)
                    {
                        //Payroll - Client
                        if (e.RowIndex == 0)
                        {
                            this.txtFilePath.Text = pvtstrPayrollPath + "\\Client\\GoingLive";
                        }
                        else
                        {
                            this.txtFilePath.Text = pvtstrPayrollPath + "\\Server\\GoingLive";

                            pvtblnUpdateGoogleDrive = true;
                        }
                    }
                    else
                    {
                        if (e.RowIndex == 2
                        || e.RowIndex == 3)
                        {
                            if (e.RowIndex == 2)
                            {
                                //Time Attendance
                                this.txtFilePath.Text = pvtstrTimeAttendPath + "\\Client\\GoingLive"; ;
                            }
                            else
                            {
                                //Time Attendance
                                this.txtFilePath.Text = pvtstrTimeAttendPath + "\\Server\\GoingLive";
                            }
                        }
                        else
                        {
                            //Time Attendance
                            this.txtFilePath.Text = pvtstrTimeAttendInternetPath + "\\Client\\GoingLive";
                        }
                    }

                    bool blnWindowsServiceToBeStopped = false;

                    if (pvtDataSet.Tables["SelectedUsers"] != null)
                    {
                        pvtDataSet.Tables.Remove(pvtDataSet.Tables["SelectedUsers"]);
                    }

                    if (pvtDataSet.Tables["VersionFiles"] != null)
                    {
                        pvtDataSet.Tables.Remove(pvtDataSet.Tables["VersionFiles"]);
                    }

                    if (this.txtVersion.Text == "Beta"
                        && e.RowIndex == 1)
                    {
                        this.btnUpdate.Enabled = false;
                        goto dgvProductDataGridView_RowEnter_Continue;
                    }
                    else
                    {
                        this.btnUpdate.Enabled = true;
                    }

                    object[] objParm = new object[2];
                    objParm[0] = this.txtVersion.Text;
                    objParm[1] = dgvProductDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvProductDataGridView)].Value.ToString();

                    pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Version_Records", objParm);

                    pvtTempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);
                    pvtDataSet.Merge(pvtTempDataSet);

                    this.pvtblnFileDataGridViewLoaded = false;

                    this.Clear_DataGridView(this.dgvFileDataGridView);

                    for (int intRow = 0; intRow < this.pvtDataSet.Tables["VersionFiles"].Rows.Count; intRow++)
                    {
                        if (this.pvtDataSet.Tables["VersionFiles"].Rows[intRow]["FILE_NAME"].ToString() == "clsTax.dll"
                        && pvtintProductRowIndex == 0)
                        {
                            this.pvtDataSet.Tables["VersionFiles"].Rows[intRow]["PROGRAM_ID"] = "B";
                        }

                        this.dgvFileDataGridView.Rows.Add(this.pvtDataSet.Tables["VersionFiles"].Rows[intRow]["FILE_NAME"].ToString(),
                                                          this.pvtDataSet.Tables["VersionFiles"].Rows[intRow]["PROGRAM_ID"].ToString(),
                                                          Convert.ToDateTime(this.pvtDataSet.Tables["VersionFiles"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]).ToString("yyyy-MM-dd  HH:mm:ss"),
                                                          Convert.ToDouble(this.pvtDataSet.Tables["VersionFiles"].Rows[intRow]["FILE_SIZE"]).ToString(""),
                                                          this.pvtDataSet.Tables["VersionFiles"].Rows[intRow]["FILE_VERSION_NO"].ToString());
                    }

                    this.pvtblnFileDataGridViewLoaded = true;

                    DataView DataView;

                    //Load Files To Be Uploaded
                    DirectoryInfo di = new DirectoryInfo(this.txtFilePath.Text);
                    FileInfo[] fiFiles = di.GetFiles("*.*");
                    FileVersionInfo fivFileVersionInfo;

                    this.pvtblnFileToUploadDataGridViewLoaded = false;

                    this.Clear_DataGridView(this.dgvFileToUploadDataGridView);

                    foreach (FileInfo fi in fiFiles)
                    {
                        //These Files are NOT For Download
                        if (fi.Name != "msvcr100.dll"
                        & fi.Name != "DBConfig.txt"
                        & fi.Name != "DBClientConfig.txt"
                        & fi.Name != "Payroll.exe"
                        & fi.Name != "TimeAttendanceClient.exe"
                        & fi.Name != "TimeSheetDynamicUpload.exe"
                        & fi.Name != "TimeAttendanceInternet.exe")
                        //& fi.Name != "FingerPrintClockTimeAttendanceService.exe"
                        //& fi.Name != "FingerPrintClockTimeAttendanceServiceStartStop.exe")
                        {
                            if (fi.Name == "URLConfig.txt"
                            || fi.Name == "clsISUtilities.dll"
                            || fi.Name == "Calender.dll"
                            || fi.Name == "PublicHoliday.dll"
                            || fi.Name == "DownloadFiles.dll"
                            || fi.Name == "PasswordChange.dll"
                            || fi.Name == "BackupRestoreDatabase.dll"
                            || fi.Name == "ClosePayrollRun.dll"
                            || fi.Name == "CostCentre.dll"
                            || fi.Name == "CompanyPaymentOptions.dll"
                            || fi.Name == "DataDownload.dll"
                            || fi.Name == "DataUpload.dll"
                            || fi.Name == "DateLoad.dll"
                            || fi.Name == "EmployeeActivateSelection.dll"
                            || fi.Name == "FileUpload.dll"
                            || fi.Name == "FileDownload.dll"
                            || fi.Name == "Microsoft.ReportViewer.Common.dll"
                            || fi.Name == "Microsoft.ReportViewer.ProcessingObjectModel.DLL"
                            || fi.Name == "Microsoft.ReportViewer.WinForms.DLL"
                            || fi.Name == "Microsoft.SqlServer.Types.dll"
                            || fi.Name == "OccupationDepartment.dll"
                            || fi.Name == "OccupationDepartmentLink.dll"
                            || fi.Name == "OpenPayrollRun.dll"
                            || fi.Name == "PasswordReset.dll"
                            || fi.Name == "RptTimeSheet.dll"
                            || fi.Name == "RptTimeSheetTotals.dll"
                            || fi.Name == "SetupLocalWebServer.dll"
                            || fi.Name == "TimeAttendanceAnalysis.dll"
                            || fi.Name == "TimeAttendanceRun.dll"
                            || fi.Name == "TimeSheet.dll"
                            || fi.Name == "TimeSheetAuthorise.dll"
                            || fi.Name == "TimeSheetBatch.dll"
                            || fi.Name == "User.dll"
                            || fi.Name == "IBScanUltimate.dll"
                            || fi.Name == "DPCtlUruNet.dll"
                            || fi.Name == "DPUruNet.dll"
                            || fi.Name == "clsReadersToDp.dll"
                            || fi.Name == "ChooseFingerprintReader.dll"
                            || fi.Name == "UserMenuAccess.dll"
                            || fi.Name == "UserEmployeeLink.dll")
                            {
                                if (pvtintProductRowIndex != 0)
                                {
                                    MessageBox.Show("File '" + this.txtFilePath.Text + "\\" + fi.Name + "' NOT Allowed\n\nFile Will be Deleted",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);

                                    File.Delete(this.txtFilePath.Text + "\\" + fi.Name);

                                    continue;
                                }
                            }
                            else
                            {
                                if (fi.Name == "clsISClientUtilities.dll")
                                {
                                    if (pvtintProductRowIndex != 2)
                                    {
                                        MessageBox.Show("File '" + this.txtFilePath.Text + "\\" + fi.Name + "' NOT Allowed\n\nFile Will be Deleted",
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);

                                        File.Delete(this.txtFilePath.Text + "\\" + fi.Name);

                                        continue;
                                    }
                                }
                                else
                                {
                                    if (fi.Name == "clsDBConnectionObjects.dll"
                                    || fi.Name == "busFileDownload.dll"
                                    || fi.Name == "busFileUpload.dll")
                                    {
                                        //Payroll Server - clsDBConnectionObjects.dll It Will also be Moved To FILE_CLIENT_DOWNLOAD_DETAILS (Server Option)
                                        if (pvtintProductRowIndex != 1)
                                        {
                                            MessageBox.Show("File '" + this.txtFilePath.Text + "\\" + fi.Name + "' NOT Allowed\n\nFile Will be Deleted",
                                            "Error",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);

                                            File.Delete(this.txtFilePath.Text + "\\" + fi.Name);

                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (fi.Name == "busClientPayrollLogon.dll")
                                        {
                                            //Client Server - It Will also be Moved To FILE_CLIENT_DOWNLOAD_DETAILS (Server Option)
                                            if (pvtintProductRowIndex != 3)
                                            {
                                                MessageBox.Show("File '" + this.txtFilePath.Text + "\\" + fi.Name + "' NOT Allowed\n\nFile Will be Deleted",
                                                "Error",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);

                                                File.Delete(this.txtFilePath.Text + "\\" + fi.Name);

                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            if (fi.Name == "FingerPrintClockServiceStartStop.dll"
                                            || fi.Name == "FingerPrintClockService.dll")
                                            {
                                                //temp remove
                                                //Client Server - It Will also be Moved To FILE_CLIENT_DOWNLOAD_DETAILS (Server Option)
                                                if (pvtintProductRowIndex != 3)
                                                {
                                                    MessageBox.Show("File '" + this.txtFilePath.Text + "\\" + fi.Name + "' NOT Allowed\n\nFile Will be Deleted",
                                                    "Error",
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Warning);

                                                    File.Delete(this.txtFilePath.Text + "\\" + fi.Name);

                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                if (fi.Name == "clsTaxTableRead.dll"
                                                    | fi.Name == "clsTax.dll")
                                                {
                                                    //Client Server - It Will also be Moved To FILE_CLIENT_DOWNLOAD_DETAILS (Server Option)
                                                    if (pvtintProductRowIndex != 1)
                                                    {
                                                        MessageBox.Show("File '" + this.txtFilePath.Text + "\\" + fi.Name + "' NOT Allowed\n\nFile Will be Deleted",
                                                        "Error",
                                                        MessageBoxButtons.OK,
                                                        MessageBoxIcon.Warning);

                                                        File.Delete(this.txtFilePath.Text + "\\" + fi.Name);

                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    if (fi.Name.Substring(0, 3) == "bus")
                                                    {
                                                        if (pvtintProductRowIndex != 1
                                                            & pvtintProductRowIndex != 3)
                                                        {
                                                            MessageBox.Show("File '" + this.txtFilePath.Text + "\\" + fi.Name + "' NOT Allowed\n\nFile Will be Deleted",
                                                            "Error",
                                                            MessageBoxButtons.OK,
                                                            MessageBoxIcon.Warning);

                                                            File.Delete(this.txtFilePath.Text + "\\" + fi.Name);

                                                            continue;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            string strProgramID = "";

                            if (pvtintProductRowIndex == 0)
                            {
                                //Payroll
                                strProgramID = "P";

                                if (fi.Name == "URLConfig.txt"
                                || fi.Name == "clsISUtilities.dll"
                                || fi.Name == "Calender.dll"
                                || fi.Name == "PublicHoliday.dll"
                                || fi.Name == "DownloadFiles.dll"
                                || fi.Name == "PasswordChange.dll"
                                || fi.Name == "BackupRestoreDatabase.dll"
                                || fi.Name == "ClosePayrollRun.dll"
                                || fi.Name == "CostCentre.dll"
                                || fi.Name == "CompanyPaymentOptions.dll"
                                || fi.Name == "DataDownload.dll"
                                || fi.Name == "DataUpload.dll"
                                || fi.Name == "DateLoad.dll"
                                || fi.Name == "EmployeeActivateSelection.dll"
                                || fi.Name == "Microsoft.ReportViewer.Common.dll"
                                || fi.Name == "Microsoft.ReportViewer.ProcessingObjectModel.DLL"
                                || fi.Name == "Microsoft.ReportViewer.WinForms.DLL"
                                || fi.Name == "Microsoft.SqlServer.Types.dll"
                                || fi.Name == "OccupationDepartment.dll"
                                || fi.Name == "OccupationDepartmentLink.dll"
                                || fi.Name == "OpenPayrollRun.dll"
                                || fi.Name == "PasswordReset.dll"
                                || fi.Name == "RptTimeSheet.dll"
                                || fi.Name == "RptTimeSheetTotals.dll"
                                || fi.Name == "SetupLocalWebServer.dll"
                                || fi.Name == "TimeAttendanceAnalysis.dll"
                                || fi.Name == "TimeAttendanceRun.dll"
                                || fi.Name == "TimeSheet.dll"
                                || fi.Name == "TimeSheetAuthorise.dll"
                                || fi.Name == "TimeSheetBatch.dll"
                                || fi.Name == "User.dll"
                                || fi.Name == "UserMenuAccess.dll"
                                || fi.Name == "UserEmployeeLink.dll")
                                {
                                    //B=Both (Payroll and Time Attendance Internet)
                                    strProgramID = "B";
                                }
                                else
                                {
                                    if (fi.Name == "FileUpload.dll"
                                    || fi.Name == "FileDownload.dll"
                                    || fi.Name == "clsReadersToDp.dll"
                                    || fi.Name == "IBScanUltimate.dll"
                                    || fi.Name == "DPCtlUruNet.dll"
                                    || fi.Name == "DPUruNet.dll"
                                    || fi.Name == "ChooseFingerprintReader.dll")
                                    {
                                        //C=Complete Set (Payroll / Time Attendance Internet / Time Attendance Client)
                                        strProgramID = "C";

                                    }
                                }
                            }
                            else
                            {
                                if (pvtintProductRowIndex == 1)
                                {
                                    //Server
                                    strProgramID = "S";
                                }
                                else
                                {
                                    if (pvtintProductRowIndex == 3)
                                    {

                                    }
                                    else
                                    {
                                        if (pvtintProductRowIndex == 4)
                                        {
                                            //Time Attendance Internet
                                            strProgramID = "T";
                                        }
                                    }
                                }
                            }

                            DataView = null;
                            DataView = new DataView(pvtDataSet.Tables["VersionFiles"], "FILE_NAME = '" + fi.Name + "'", "", DataViewRowState.CurrentRows);

                            if (DataView.Count == 0)
                            {
                                if (fi.Name == "busTimeAttendanceRun.dll"
                                || fi.Name == "busClosePayrollRun.dll"
                                || fi.Name == "busOpenPayrollRun.dll"
                                || fi.Name == "busRptPaySlip.dll"
                                || fi.Name == "clsDBConnectionObjects.dll"
                                || fi.Name == "clsProcessPayrollRunFromQueue.dll"
                                || fi.Name == "clsProcessPayrollRunFromQueueDataContract.dll"
                                || fi.Name == "clsTax.dll"
                                || fi.Name == "clsISUtilities.dll"
                                || fi.Name == "clsTaxTableRead.dll")
                                {
                                    //2 Places where this must be Changed
                                    blnWindowsServiceToBeStopped = true;
                                }

                                fivFileVersionInfo = FileVersionInfo.GetVersionInfo(this.txtFilePath.Text + "//" + fi.Name);

                                this.dgvFileToUploadDataGridView.Rows.Add(fi.Name,
                                                                          strProgramID,
                                                                          fi.LastWriteTime.ToString("yyyy-MM-dd  HH:mm:ss"),
                                                                          fi.Length.ToString(),
                                                                          fivFileVersionInfo.FileMajorPart.ToString() + "." + fivFileVersionInfo.FileMinorPart.ToString("00"));
                            }
                            else
                            {
                                if (fi.LastWriteTime < Convert.ToDateTime(DataView[0]["FILE_LAST_UPDATED_DATE"]).AddSeconds(-2)
                                    || fi.LastWriteTime > Convert.ToDateTime(DataView[0]["FILE_LAST_UPDATED_DATE"]).AddSeconds(2))
                                {
                                    if (fi.Name == "busTimeAttendanceRun.dll"
                                    || fi.Name == "busClosePayrollRun.dll"
                                    || fi.Name == "busOpenPayrollRun.dll"
                                    || fi.Name == "busRptPaySlip.dll"
                                    || fi.Name == "clsDBConnectionObjects.dll"
                                    || fi.Name == "clsProcessPayrollRunFromQueue.dll"
                                    || fi.Name == "clsProcessPayrollRunFromQueueDataContract.dll"
                                    || fi.Name == "clsTax.dll"
                                    || fi.Name == "clsISUtilities.dll"
                                    || fi.Name == "clsTaxTableRead.dll")
                                    {
                                        //2 Places where this must be Changed
                                        blnWindowsServiceToBeStopped = true;
                                    }

                                    fivFileVersionInfo = FileVersionInfo.GetVersionInfo(this.txtFilePath.Text + "//" + fi.Name);

                                    this.dgvFileToUploadDataGridView.Rows.Add(fi.Name,
                                                                              strProgramID,
                                                                              fi.LastWriteTime.ToString("yyyy-MM-dd  HH:mm:ss"),
                                                                              fi.Length.ToString(),
                                                                              fivFileVersionInfo.FileMajorPart.ToString() + "." + fivFileVersionInfo.FileMinorPart.ToString("00"));

                                    if (fi.LastWriteTime < Convert.ToDateTime(DataView[0]["FILE_LAST_UPDATED_DATE"]))
                                    {
                                        this.dgvFileToUploadDataGridView.Rows[this.dgvFileToUploadDataGridView.Rows.Count - 1].HeaderCell.Style = WarningDataGridViewCellStyle;
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("File '" + this.txtFilePath.Text + "\\" + fi.Name + "'" + "NOT Allowed\n\nFile Will be Deleted",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                            File.Delete(this.txtFilePath.Text + "\\" + fi.Name);
                        }
                    }

                    if (this.txtVersion.Text == "Beta"
                    &&  e.RowIndex != 1)
                    {
                        for (int intRow = 0; intRow < this.pvtDataSet.Tables["Users"].Rows.Count; intRow++)
                        {
                            DataView SelectedUsersDataView = new DataView(this.pvtDataSet.Tables["SelectedUsers"], "USER_NO = " + this.pvtDataSet.Tables["Users"].Rows[intRow]["USER_NO"].ToString(), "", DataViewRowState.CurrentRows);

                            if (SelectedUsersDataView.Count > 0)
                            {
                                this.dgvUserChosenDataGridView.Rows.Add(this.pvtDataSet.Tables["Users"].Rows[intRow]["USER_ID"].ToString(),
                                                                        this.pvtDataSet.Tables["Users"].Rows[intRow]["SURNAME"].ToString(),
                                                                        this.pvtDataSet.Tables["Users"].Rows[intRow]["FIRSTNAME"].ToString(),
                                                                        this.pvtDataSet.Tables["Users"].Rows[intRow]["USER_NO"].ToString());
                            }
                            else
                            {
                                this.dgvUserDataGridView.Rows.Add(this.pvtDataSet.Tables["Users"].Rows[intRow]["USER_ID"].ToString(),
                                                                  this.pvtDataSet.Tables["Users"].Rows[intRow]["SURNAME"].ToString(),
                                                                  this.pvtDataSet.Tables["Users"].Rows[intRow]["FIRSTNAME"].ToString(),
                                                                  this.pvtDataSet.Tables["Users"].Rows[intRow]["USER_NO"].ToString());
                            }
                        }

                        for (int intCompanyRow = 0; intCompanyRow < pvtDataSet.Tables["Company"].Rows.Count; intCompanyRow++)
                        {
                            DataView SelectedPayCategoriesDataView = new DataView(this.pvtDataSet.Tables["SelectedPayCategories"], "COMPANY_NO = " + this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_NO"].ToString() + " AND PAY_CATEGORY_NO = " + this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["PAY_CATEGORY_NO"].ToString(), "", DataViewRowState.CurrentRows);

                            if (SelectedPayCategoriesDataView.Count > 0)
                            {
                                this.dgvCostCentreChosenDataGridView.Rows.Add(this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_DESC"].ToString(),
                                                                              this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                              this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_NO"].ToString(),
                                                                              this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["PAY_CATEGORY_NO"].ToString());
                            }
                            else
                            {
                                this.dgvCostCentreDataGridView.Rows.Add(this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_DESC"].ToString(),
                                                                        this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                        this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["COMPANY_NO"].ToString(),
                                                                        this.pvtDataSet.Tables["Company"].Rows[intCompanyRow]["PAY_CATEGORY_NO"].ToString());
                            }
                        }
                    }
                    
                    dgvProductDataGridView_RowEnter_Continue:


                    this.pvtblnFileToUploadDataGridViewLoaded = true;
                    
                    Cursor = Cursors.Default;

                    if (blnWindowsServiceToBeStopped == true)
                    {
                        MessageBox.Show("RunTimeAttenanceWinService.exe Needs To be Stopped");
                    }
                }
            }
        }

        private void dgvFilesDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1
                & this.pvtblnFileDataGridViewLoaded == true)
            {

            }
        }

        private void dgvVersionDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvVersionDataGridView.Rows.Count > 0
                & this.pvtblnVersionDataGridViewLoaded == true)
            {
                if (pvtintVersionRowIndex != e.RowIndex)
                {
                    pvtintVersionRowIndex = e.RowIndex;

                    Cursor = Cursors.WaitCursor;

                    this.txtVersion.Text = this.dgvVersionDataGridView[0, e.RowIndex].Value.ToString();

                    this.dgvUserDataGridView.Rows.Clear();
                    this.dgvUserChosenDataGridView.Rows.Clear();
                    this.dgvCostCentreDataGridView.Rows.Clear();
                    this.dgvCostCentreChosenDataGridView.Rows.Clear();
                    
                    pvtintProductRowIndex = -1;
                    this.Set_DataGridView_SelectedRowIndex(dgvProductDataGridView, 0);
                }
            }
        }

        private void dgvFileToUploadDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvFileToUploadDataGridView.Rows.Count > 0
               & this.pvtblnFileToUploadDataGridViewLoaded == true)
            {

            }
        }

        private void dgvFileToUploadDataGridView_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.dgvFileToUploadDataGridView.Rows.Count > 0)
            {
                if (e.KeyValue == 46)
                {
                    this.dgvFileToUploadDataGridView.Rows.RemoveAt(this.Get_DataGridView_SelectedRowIndex(this.dgvFileToUploadDataGridView));
                }
            }
        }

        private void frmProductUpload_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (clsISUtilities != null)
            {
                clsISUtilities = null;
            }

            GC.Collect();
        }

        private void dgvUserDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvUserChosenDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvCostCentreDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvCostCentreChosenDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnCostCentreAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvCostCentreDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvCostCentreDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView)];

                this.dgvCostCentreDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvCostCentreChosenDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvCostCentreDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreChosenDataGridView, this.dgvCostCentreChosenDataGridView.Rows.Count - 1);
            }
        }

        private void btnCostCentreDelete_Click(object sender, EventArgs e)
        {
            if (this.dgvCostCentreChosenDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvCostCentreChosenDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvCostCentreChosenDataGridView)];

                this.dgvCostCentreChosenDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvCostCentreDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvCostCentreChosenDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreChosenDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvCostCentreDataGridView, this.dgvCostCentreDataGridView.Rows.Count - 1);
            }
        }

        private void btnCostCentreDeleteAll_Click(object sender, EventArgs e)
        {
        btnCostCentreDeleteAll_Click_Continue:

            for (int intRowCount = 0; intRowCount < this.dgvCostCentreChosenDataGridView.Rows.Count; intRowCount++)
            {
                this.btnCostCentreDelete_Click(sender, e);

                goto btnCostCentreDeleteAll_Click_Continue;
            }
        }

        private void dgvUserDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnUserAdd_Click(sender, e);
            }
        }

        private void dgvUserChosenDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnUserDelete_Click(sender, e);
            }
        }

        private void dgvCostCentreDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnCostCentreAdd_Click(sender, e);
            }
        }

        private void dgvCostCentreChosenDataGridView_DoubleClick(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                btnCostCentreDelete_Click(sender, e);
            }
        }

        private void btnMoveToCurrent_Click(object sender, EventArgs e)
        {
            DialogResult myDialogResult = MessageBox.Show("Are you sure you want to move ALL Files from Beta to Current?",
                   "Move Files",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Question);

            if (myDialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                string strFromDirectory = pvtstrPayrollPath + "\\Client\\BetaLive";
                string strToDirectory = pvtstrPayrollPath + "\\Client\\CurrentLive";

                foreach (string newPath in Directory.GetFiles(strFromDirectory, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(strFromDirectory, strToDirectory), true);
                }

                strFromDirectory = pvtstrTimeAttendPath + "\\Client\\BetaLive";
                strToDirectory = pvtstrTimeAttendPath + "\\Client\\CurrentLive";

                foreach (string newPath in Directory.GetFiles(strFromDirectory, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(strFromDirectory, strToDirectory), true);
                }

                strFromDirectory = pvtstrTimeAttendPath + "\\Server\\BetaLive";
                strToDirectory = pvtstrTimeAttendPath + "\\Server\\CurrentLive";

                foreach (string newPath in Directory.GetFiles(strFromDirectory, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(strFromDirectory, strToDirectory), true);
                }

                strFromDirectory = pvtstrTimeAttendInternetPath + "\\Client\\BetaLive";
                strToDirectory = pvtstrTimeAttendInternetPath + "\\Client\\CurrentLive";

                foreach (string newPath in Directory.GetFiles(strFromDirectory, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(strFromDirectory, strToDirectory), true);
                }
                
                clsISUtilities.DynamicFunction("Move_Files_From_Beta_To_Current", null);

                btnRefresh_Click(sender, e);
            }
        }
    }
}
