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

namespace InteractPayroll
{
    public partial class frmFileDownload : Form
    {
        clsISUtilities clsISUtilities;
        clsCrc32 clsCrc32;

        DataSet pvtDataSet;

        ToolStripMenuItem miLinkedMenuItem;

        DataView pvtUserFileDataview;

        private int pvtintUserDataGridViewRowIndex = -1;
        private int pvtintUserFileDataGridViewRowIndex = -1;

        private bool pvtblnUserDataGridViewLoaded = false;
        private bool pvtblnUserFileDataGridViewLoaded = false;

        private int pvtintUserNo = -1;
        private string pvtstrFileUploadDatetime = "";
        private string pvtstrFileLastUpdatedDatetime = "";
        private string pvtstrFileName = "";
        private int pvtintChunkNo = -1;
        private int pvtintFileSize = -1;
        private int pvtintFileCompressedSize = -1;
        private string pvtstrFileCRC32Value = "";
        
        public frmFileDownload()
        {
            InitializeComponent();
        }

        private void frmFileDownload_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busFileDownload");
                clsCrc32 = new clsCrc32();

                this.lblUserSpreadsheetHeader.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblFileDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                try
                {
                    //User For Time Attendance Client (Not Payroll / Time Attendance Internet)
                    miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");
                }
                catch
                {
                }

                pvtDataSet = new DataSet();

                if (AppDomain.CurrentDomain.GetData("AccessInd").ToString() != "S")
                {
                    this.btnDelete.Visible = false;
                    this.btnClose.Top = this.btnDelete.Top;
                    this.grbToUser.Visible = false;
                }

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

                }

                pvtblnUserDataGridViewLoaded = true;

                if (this.dgvUserDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(this.dgvUserDataGridView, 0);
                }
                else
                {
                    CustomMessageBox.Show("There are No Files to Download.", "File Download", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                case "dgvFileDataGridView":

                    pvtintUserFileDataGridViewRowIndex = -1;
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

                    case "dgvFileDataGridView":

                        this.dgvFileDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;
                    

                    default:

                        MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
        }

        private void dgvUserDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnUserDataGridViewLoaded == true)
            {
                if (pvtintUserDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintUserDataGridViewRowIndex = e.RowIndex;

                    pvtintUserNo = Convert.ToInt32(this.dgvUserDataGridView[3, e.RowIndex].Value);

                    this.txtUser.Text = "";
                    this.txtName.Text = "";
                    this.txtSurname.Text = "";

                    this.Clear_DataGridView(this.dgvFileDataGridView);

                    pvtblnUserFileDataGridViewLoaded = false;

                    pvtUserFileDataview = null;
                    pvtUserFileDataview = new DataView(this.pvtDataSet.Tables["UserFile"],
                                                       "USER_NO = " + pvtintUserNo,
                                                       "",
                                                       DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtUserFileDataview.Count; intRow++)
                    {
                        this.dgvFileDataGridView.Rows.Add(Convert.ToDateTime(pvtUserFileDataview[intRow]["UPLOAD_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss"),
                                                          pvtUserFileDataview[intRow]["FILE_NAME"].ToString(),
                                                          Convert.ToDateTime(pvtUserFileDataview[intRow]["FILE_LAST_UPDATED_DATE"]).ToString("yyyy-MM-dd HH:mm:ss"),
                                                          pvtUserFileDataview[intRow]["FILE_SIZE"].ToString(),
                                                          pvtUserFileDataview[intRow]["FILE_SIZE_COMPRESSED"].ToString(),
                                                          pvtUserFileDataview[intRow]["FILE_VERSION_NO"].ToString(),
                                                          pvtUserFileDataview[intRow]["FILE_CRC_VALUE"].ToString(),
                                                          pvtUserFileDataview[intRow]["MAX_FILE_CHUNK_NO"].ToString(),
                                                          pvtUserFileDataview[intRow]["USER_ID"].ToString(),
                                                          pvtUserFileDataview[intRow]["FIRSTNAME"].ToString(),
                                                          pvtUserFileDataview[intRow]["SURNAME"].ToString(),
                                                          intRow.ToString());
                    }

                    pvtblnUserFileDataGridViewLoaded = true;

                    if (this.dgvFileDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(this.dgvFileDataGridView, 0);

                        this.btnDownload.Enabled = true;
                        this.btnDelete.Enabled = true;
                    }
                    else
                    {
                        this.btnDownload.Enabled = false;
                        this.btnDelete.Enabled = false;
                    }
                }
            }
        }

        private void dgvFileDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnUserFileDataGridViewLoaded == true)
            {
                if (pvtintUserFileDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintUserFileDataGridViewRowIndex = e.RowIndex;

                    pvtstrFileUploadDatetime = this.dgvFileDataGridView[0,e.RowIndex].Value.ToString();
                    pvtstrFileName = this.dgvFileDataGridView[1, e.RowIndex].Value.ToString();
                    pvtstrFileLastUpdatedDatetime = this.dgvFileDataGridView[2, e.RowIndex].Value.ToString();
                    pvtintFileSize = Convert.ToInt32(this.dgvFileDataGridView[3, e.RowIndex].Value);
                    pvtintFileCompressedSize = Convert.ToInt32(this.dgvFileDataGridView[4, e.RowIndex].Value);
                    pvtstrFileCRC32Value = this.dgvFileDataGridView[6, e.RowIndex].Value.ToString();
                    pvtintChunkNo = Convert.ToInt32(this.dgvFileDataGridView[7, e.RowIndex].Value);

                    this.txtUser.Text = this.dgvFileDataGridView[8, e.RowIndex].Value.ToString();
                    this.txtName.Text = this.dgvFileDataGridView[9, e.RowIndex].Value.ToString();
                    this.txtSurname.Text = this.dgvFileDataGridView[10, e.RowIndex].Value.ToString();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomMessageBox.Show("Are you sure you want to Delete this File?",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    object[] objParm = new object[3];
                    objParm[0] = pvtintUserNo;
                    objParm[1] = pvtstrFileUploadDatetime;
                    objParm[2] = pvtstrFileName;

                    clsISUtilities.DynamicFunction("Delete_Record", objParm);

                    this.pvtUserFileDataview[pvtintUserFileDataGridViewRowIndex].Delete();

                    this.pvtDataSet.AcceptChanges();

                    this.Set_DataGridView_SelectedRowIndex(this.dgvUserDataGridView, this.Get_DataGridView_SelectedRowIndex(this.dgvUserDataGridView));
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                this.prgProgressBar.Maximum = pvtintChunkNo;
                this.prgProgressBar.Minimum = 0;
                this.prgProgressBar.Value = 0;
                this.prgProgressBar.Visible = true;

                byte[] bytBytes = new byte[pvtintFileCompressedSize];
                long pvtlngDestinationFileStartIndex = 0;

                for (int intChunkNo = 1; intChunkNo <= pvtintChunkNo; intChunkNo++)
                {
                    object[] objParm = new object[4];
                    objParm[0] = pvtintUserNo;
                    objParm[1] = pvtstrFileUploadDatetime;
                    objParm[2] = pvtstrFileName;
                    objParm[3] = intChunkNo;

                    byte[] bytTempBytes = (byte[])clsISUtilities.DynamicFunction("Get_File_Chunk", objParm);

                    Array.Copy(bytTempBytes, 0, bytBytes, pvtlngDestinationFileStartIndex, bytTempBytes.Length);
                    pvtlngDestinationFileStartIndex += bytTempBytes.Length;

                    this.prgProgressBar.Value += 1;
                }

                byte[] pvtbytDecompressedBytes = new byte[pvtintFileSize];

                //Open Memory Stream with Compressed Data
                MemoryStream msMemoryStream = new MemoryStream(bytBytes);

                System.IO.Compression.GZipStream GZipStreamDecompress = new GZipStream(msMemoryStream, CompressionMode.Decompress);

                //Decompress Bytes
                BinaryReader pvtbrBinaryReader = new BinaryReader(GZipStreamDecompress);
                pvtbytDecompressedBytes = pvtbrBinaryReader.ReadBytes(pvtintFileSize);

                //CRC32 Value
                string strCRC32Value = "";

                foreach (byte b in clsCrc32.ComputeHash(pvtbytDecompressedBytes))
                {
                    strCRC32Value += b.ToString("x2").ToLower();
                }

                this.prgProgressBar.Visible = false;

                if (strCRC32Value == pvtstrFileCRC32Value)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "All files (*.*)|*.*";
                    saveFileDialog.FileName = pvtstrFileName;
                    saveFileDialog.Title = "Save an Image File";
                 
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        FileStream pvtfsFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create);
                        BinaryWriter pvtbwBinaryWriter = new BinaryWriter(pvtfsFileStream);

                        pvtbwBinaryWriter.Write(pvtbytDecompressedBytes);

                        //Write Memory Portion To Disk
                        pvtbwBinaryWriter.Close();

                        File.SetLastWriteTime(saveFileDialog.FileName, Convert.ToDateTime(pvtstrFileLastUpdatedDatetime));
                    }
                }
                else
                {
                    CustomMessageBox.Show("File Download UNSUCCESSFUL.",
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmFileDownload_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                miLinkedMenuItem.Enabled = true;
            }
            catch
            {
            }
        }
    }
}
