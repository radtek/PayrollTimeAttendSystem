using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Diagnostics;
using InteractPayrollClient;

namespace InteractPayroll
{
    public partial class frmFileUpload : Form
    {
        clsISUtilities clsISUtilities;
        clsISClientUtilities clsISClientUtilities;
        clsCrc32 clsCrc32;
        private DataSet pvtDataSet;
        DataView pvtDataView;

        ToolStripMenuItem miLinkedMenuItem;

        private int pvtintUserDataGridViewRowIndex = -1;
        private bool pvtblnUserDataGridViewLoaded = false;
        private string pvtstrFileName = "";

        private int pvtintFilesDataGridViewRowIndex = -1;
        private bool pvtblnFilesDataGridViewLoaded = false;

        int intUserExtraHeight = 228;
        int intSelectedUserExtraHeight = 190;

        public frmFileUpload()
        {
            InitializeComponent();

            this.lblFilesHeader.Visible = false;
            dgvFilesDataGridView.Visible = false;

            dgvUserDataGridView.Height += intUserExtraHeight;
            dgvSelectedUserDataGridView.Height += intSelectedUserExtraHeight;
        }

        private void frmFileUpload_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busFileUpload");
                clsISClientUtilities = new clsISClientUtilities(this, "busBackupRestoreClientDatabase");
                
                clsCrc32 = new clsCrc32();

                this.lblUserSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblSelectedUserSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblFilesHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                try
                {
                    //User For Time Attendance Client (Not Payroll / Time Attendance Internet)
                    miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");
                }
                catch
                {
                }

                pvtDataSet = new DataSet();

                object[] objParm = new object[2];
                objParm[0] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                for (int intRow = 0; intRow < pvtDataSet.Tables["User"].Rows.Count; intRow++)
                {
                    this.dgvUserDataGridView.Rows.Add(pvtDataSet.Tables["User"].Rows[intRow]["USER_ID"].ToString(),
                                                      pvtDataSet.Tables["User"].Rows[intRow]["SURNAME"].ToString(),
                                                      pvtDataSet.Tables["User"].Rows[intRow]["FIRSTNAME"].ToString(),
                                                      pvtDataSet.Tables["User"].Rows[intRow]["USER_NO"].ToString());

                    if (pvtDataSet.Tables["User"].Rows.Count == 1)
                    {
                       
                    }
                }

                pvtblnUserDataGridViewLoaded = true;

                if (this.dgvUserDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvUserDataGridView, 0);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "All files (*.*)|*.*";
                dialog.Title = "Select a File to Upload.";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.txtFilePath.Text = dialog.FileName;

                    pvtstrFileName = dialog.SafeFileName;

                    this.btnUpload.Enabled = true;
                }
                else
                {
                    this.txtFilePath.Text = "";
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
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
            else
            {
                if (myDataGridView.SelectionMode == DataGridViewSelectionMode.CellSelect)
                {
                    intReturnIndex = myDataGridView.CurrentCell.RowIndex;
                }
            }

            return intReturnIndex;
        }

        public void Set_DataGridView_SelectedRowIndex(DataGridView myDataGridView, int intRow)
        {
            switch (myDataGridView.Name)
            {
                case "dgvUserDataGridView":

                    pvtintUserDataGridViewRowIndex = -1;
                    break;

                case "dgvSelectedUserDataGridView":

                    pvtintUserDataGridViewRowIndex = -1;
                    break;

                case "dgvFilesDataGridView":

                    pvtintFilesDataGridViewRowIndex = -1;
                    break;

                default:

                    break;
            }

            //Fires DataGridView RowEnter Function
            if (this.Get_DataGridView_SelectedRowIndex(myDataGridView) == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvUserDataGridView":

                        dgvUserDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvSelectedUserDataGridView":

                        dgvUserDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvFilesDataGridView":

                        dgvFilesDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        System.Windows.Forms.MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                string strUsers = "";

                if (this.dgvSelectedUserDataGridView.Rows.Count > 0)
                {
                    for (int intRow = 0; intRow < this.dgvSelectedUserDataGridView.Rows.Count; intRow++)
                    {
                        if (strUsers == "")
                        {
                            strUsers = this.dgvSelectedUserDataGridView[3, intRow].Value.ToString();
                        }
                        else
                        {
                            strUsers += "," + this.dgvSelectedUserDataGridView[3, intRow].Value.ToString();
                        }
                    }
                }
                else
                {
                    CustomMessageBox.Show("You need to Select User/s to Upload File to.", "File Upload", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                if (this.txtFilePath.Text != "")
                {
                    DialogResult dlgResult = CustomMessageBox.Show("Are you sure you want to Upload this File?",
                        this.Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (dlgResult == DialogResult.Yes)
                    {
                        this.dgvUserDataGridView.Enabled = false;
                        this.dgvSelectedUserDataGridView.Enabled = false;
                        this.dgvFilesDataGridView.Enabled = false;

                        this.btnAdd.Enabled = false;
                        this.btnAddAll.Enabled = false;
                        this.btnRemove.Enabled = false;
                        this.btnRemoveAll.Enabled = false;

                        this.btnUpload.Enabled = false;

                        DateTime dtUploadDateTime = DateTime.Now;

                        if (this.rbnFile.Checked == true)
                        {
                            FileInfo fi = new FileInfo(this.txtFilePath.Text);
                            FileVersionInfo fivFileVersionInfo = FileVersionInfo.GetVersionInfo(this.txtFilePath.Text);

                            string strCRC32Value = "";
                            int intNumberOfBytesToRead = 50000;
                         
                            byte[] pvtbytes = new byte[intNumberOfBytesToRead];

                            DateTime dtFileLastUpdated = fi.LastWriteTime;
                            int intFileSize = Convert.ToInt32(fi.Length);
                            int intCompressedSize;
                            string strVersionNumber = fivFileVersionInfo.FileMajorPart.ToString() + "." + fivFileVersionInfo.FileMinorPart.ToString("00");
                            bool blnComplete = false;

                            FileStream pvtfsFileStream = new FileStream(this.txtFilePath.Text, FileMode.Open, FileAccess.Read);

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
                            this.prgProgressBar.Value = 0;
                            this.prgProgressBar.Visible = true;

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

                                object[] objParm = new object[12];
                                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                                objParm[1] = dtUploadDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                                objParm[2] = intBlockNumber;
                                objParm[3] = pvtstrFileName;
                                objParm[4] = pvtbytes;
                                objParm[5] = dtFileLastUpdated.ToString("yyyy-MM-dd HH:mm:ss");
                                objParm[6] = intFileSize;
                                objParm[7] = intCompressedSize;
                                objParm[8] = strVersionNumber;
                                objParm[9] = blnComplete;
                                objParm[10] = strCRC32Value;
                                objParm[11] = strUsers;

                                bool blnSuccessful = (bool)clsISUtilities.DynamicFunction("Upload_File", objParm);

                                if (blnSuccessful == true)
                                {
                                    intNumberBytesAlreadyRead += intNumberBytesRead;

                                    this.prgProgressBar.Value += 1;
                                    this.prgProgressBar.Refresh();
                                }
                                else
                                {
                                    CustomMessageBox.Show("Upload Error", "File Upload", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                    this.prgProgressBar.Visible = false;

                                    this.dgvUserDataGridView.Enabled = true;
                                    this.dgvSelectedUserDataGridView.Enabled = true;
                                    this.dgvFilesDataGridView.Enabled = true;

                                    this.btnAdd.Enabled = true;
                                    this.btnAddAll.Enabled = true;
                                    this.btnRemove.Enabled = true;
                                    this.btnRemoveAll.Enabled = true;

                                    this.btnUpload.Enabled = true;

                                    return;
                                }
                            }

                            pvtfsFileStream.Close();
                            msMemoryStream.Close();
                            pvtbrBinaryReader.Close();

                            pvtfsFileStream = null;
                            msMemoryStream = null;
                            pvtbrBinaryReader = null;
                        }
                        else
                        {
                            bool blnComplete = false;

                            object[] objParm = new object[1];
                            objParm[0] = this.txtFilePath.Text;

                            byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Load_File_Into_Database", objParm, false);
                            DataSet myDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                            int intNumberBlocks = Convert.ToInt32(myDataSet.Tables["FileUploaded"].Rows[0]["FILE_NUMBER_BLOCKS"]);
                         
                            this.prgProgressBar.Maximum = intNumberBlocks;
                            this.prgProgressBar.Minimum = 0;
                            this.prgProgressBar.Value = 0;
                            this.prgProgressBar.Visible = true;
                            
                            if (intNumberBlocks != -1)
                            {
                                for (int intBlockNumber = 1; intBlockNumber <= intNumberBlocks; intBlockNumber++)
                                {
                                    objParm = new object[2];
                                    objParm[0] = myDataSet.Tables["FileUploaded"].Rows[0]["FILE_NAME"].ToString();
                                    objParm[1] = intBlockNumber;

                                    byte[] bytArray = (byte[])clsISClientUtilities.DynamicFunction("Get_File_Chunk", objParm, false);

                                    if (intBlockNumber == intNumberBlocks)
                                    {
                                        blnComplete = true;
                                    }

                                    objParm = new object[12];
                                    objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                                    objParm[1] = dtUploadDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                                    objParm[2] = intBlockNumber;
                                    objParm[3] = myDataSet.Tables["FileUploaded"].Rows[0]["FILE_NAME"].ToString(); ;
                                    objParm[4] = bytArray;
                                    objParm[5] = Convert.ToDateTime(myDataSet.Tables["FileUploaded"].Rows[0]["FILE_LAST_UPDATED_DATE"]).ToString("yyyy-MM-dd HH:mm:ss");
                                    objParm[6] = Convert.ToInt32(myDataSet.Tables["FileUploaded"].Rows[0]["FILE_SIZE"]);
                                    objParm[7] = Convert.ToInt32(myDataSet.Tables["FileUploaded"].Rows[0]["FILE_SIZE_COMPRESSED"]);
                                    objParm[8] = myDataSet.Tables["FileUploaded"].Rows[0]["FILE_VERSION_NO"].ToString(); ;
                                    objParm[9] = blnComplete;
                                    objParm[10] = myDataSet.Tables["FileUploaded"].Rows[0]["FILE_CRC_VALUE"].ToString();
                                    objParm[11] = strUsers;

                                    bool blnSuccessful = (bool)clsISUtilities.DynamicFunction("Upload_File", objParm);

                                    if (blnSuccessful == true)
                                    {
                                        this.prgProgressBar.Value += 1;
                                        this.prgProgressBar.Refresh();
                                    }
                                    else
                                    {
                                        CustomMessageBox.Show("Upload Error", "File Upload", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                        this.prgProgressBar.Visible = false;

                                        this.dgvUserDataGridView.Enabled = true;
                                        this.dgvSelectedUserDataGridView.Enabled = true;
                                        this.dgvFilesDataGridView.Enabled = true;

                                        this.btnAdd.Enabled = true;
                                        this.btnAddAll.Enabled = true;
                                        this.btnRemove.Enabled = true;
                                        this.btnRemoveAll.Enabled = true;

                                        this.btnUpload.Enabled = true;

                                        return;
                                    }
                                }
                            }
                            else
                            {
                                //Error
                            }
                        }

                        CustomMessageBox.Show("File Upload Successful.", "File Upload", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.prgProgressBar.Visible = false;

                        this.dgvUserDataGridView.Enabled = true;
                        this.dgvSelectedUserDataGridView.Enabled = true;
                        this.dgvFilesDataGridView.Enabled = true;

                        this.btnAdd.Enabled = true;
                        this.btnAddAll.Enabled = true;
                        this.btnRemove.Enabled = true;
                        this.btnRemoveAll.Enabled = true;

                        this.btnUpload.Enabled = true;
                    }
                }
                else
                {
                    CustomMessageBox.Show("Choose a File to Upload.", "File Upload", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);

                this.dgvUserDataGridView.Enabled = true;
                this.dgvSelectedUserDataGridView.Enabled = true;

                this.btnAdd.Enabled = true;
                this.btnAddAll.Enabled = true;
                this.btnRemove.Enabled = true;
                this.btnRemoveAll.Enabled = true;

                this.btnUpload.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvUserDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnUserDataGridViewLoaded == true)
            {
                if (pvtintUserDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintUserDataGridViewRowIndex = e.RowIndex;
                }
            }
        }

        private void frmFileUpload_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                miLinkedMenuItem.Enabled = true;
            }
            catch
            {
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvUserDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvUserDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvUserDataGridView)];

                this.dgvUserDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvSelectedUserDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvUserDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvUserDataGridView, 0);
                }

                this.Set_DataGridView_SelectedRowIndex(this.dgvSelectedUserDataGridView, this.dgvSelectedUserDataGridView.Rows.Count - 1);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvSelectedUserDataGridView.Rows.Count > 0)
            {
                DataGridViewRow myDataGridViewRow = this.dgvSelectedUserDataGridView.Rows[Get_DataGridView_SelectedRowIndex(this.dgvSelectedUserDataGridView)];

                this.dgvSelectedUserDataGridView.Rows.Remove(myDataGridViewRow);

                this.dgvUserDataGridView.Rows.Add(myDataGridViewRow);

                if (this.dgvSelectedUserDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvSelectedUserDataGridView, 0);
                }
              
                this.Set_DataGridView_SelectedRowIndex(this.dgvUserDataGridView, this.dgvUserDataGridView.Rows.Count - 1);
            }
        }

        private void dgvUserDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnAdd_Click(sender, e);
        }

        private void dgvSelectedUserDataGridView_DoubleClick(object sender, EventArgs e)
        {
            this.btnRemove_Click(sender, e);
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
        btnAddAll_Click_Continue:

            if (this.dgvUserDataGridView.Rows.Count > 0)
            {
                btnAdd_Click(null, null);

                goto btnAddAll_Click_Continue;
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
        btnRemoveAll_Click_Continue:

            if (this.dgvSelectedUserDataGridView.Rows.Count > 0)
            {
                btnRemove_Click(null, null);

                goto btnRemoveAll_Click_Continue;
            }
        }

        private void rbnFileOption_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            this.txtFilePath.Text = "";

            if (myRadioButton.Name == "rbnBackupFile")
            {
                dgvUserDataGridView.Height -= intUserExtraHeight;
                dgvSelectedUserDataGridView.Height -= intSelectedUserExtraHeight;

                this.btnFile.Visible = false;
                this.lblFilesHeader.Visible = true;
                this.dgvFilesDataGridView.Visible = true;

                string strPayrollRunDate = "";

                try
                {
                    byte[] byteCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Restore_Files", null, false);

                    pvtDataSet = null;
                    pvtDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(byteCompress);

                    this.dgvFilesDataGridView.Rows.Clear();

                    pvtblnFilesDataGridViewLoaded = false;

                    pvtDataView = null;
                    pvtDataView = new DataView(pvtDataSet.Tables["RestoreFiles"], "", "RESTORE_DATETIME DESC", DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtDataView.Count; intRow++)
                    {
                        if (pvtDataView[intRow]["RESTORE_DATETIME"] != System.DBNull.Value)
                        {
                            strPayrollRunDate = Convert.ToDateTime(pvtDataView[intRow]["RESTORE_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            strPayrollRunDate = "";
                        }

                        this.dgvFilesDataGridView.Rows.Add(strPayrollRunDate,
                                                           pvtDataView[intRow]["RESTORE_FILE"].ToString());
                    }

                    pvtblnFilesDataGridViewLoaded = true;

                    if (this.dgvFilesDataGridView.Rows.Count > 0)
                    {
                        Set_DataGridView_SelectedRowIndex(dgvFilesDataGridView, 0);
                    }
                }
                catch(Exception ex)
                {

                }
            }
            else
            {
                //File 
                this.btnFile.Visible = true;
                this.lblFilesHeader.Visible = false;
                this.dgvFilesDataGridView.Visible = false;

                dgvUserDataGridView.Height += intUserExtraHeight;
                dgvSelectedUserDataGridView.Height += intSelectedUserExtraHeight;
            }
        }

        private void dgvFilesDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnFilesDataGridViewLoaded == true)
            {
                if (pvtintFilesDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintFilesDataGridViewRowIndex = e.RowIndex;

                    this.txtFilePath.Text = dgvFilesDataGridView[1, pvtintFilesDataGridViewRowIndex].Value.ToString();
                }
            }
        }
    }
}
