using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using InteractPayrollClient;

namespace InteractPayroll
{
    public partial class frmReadWriteFile : Form
    {
        clsISUtilities clsISUtilities;

        clsISClientUtilities clsISClientUtilities;

        clsCrc32 clsCrc32;
        
        private byte[] pvtbytBytes;
        private byte[] pvtbytDecompressedBytes;
        private byte[] pvtbytTempBytes;

        private long pvtlngDestinationFileStartIndex = 0;
        
        private FileStream pvtfsFileStream;
        private BinaryReader pvtbrBinaryReader;
        private BinaryWriter pvtbwBinaryWriter;

        public frmReadWriteFile()
        {
            InitializeComponent();

            clsISUtilities = new clsISUtilities(this, "busPayrollLogon");
            this.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Form_Paint);

            clsISClientUtilities = new InteractPayrollClient.clsISClientUtilities(this, "busClientPayrollLogon");

            clsCrc32 = new clsCrc32();
        }

        public int DownLoad_Files_From_Database(DataTable parDataTable)
        {
            try
            {
                this.Show();

                this.Refresh();

                int intProgressBarMaxValue = 0;
                int intProgressBarCombinedMaxValue = 0;
                string strCRC32Value = "";

                string strFileLayerInd = "";
                string strFileName = "";
                string strFileDownloadName = "";
                string strFilePathName = "";
                string strProjectVersion = "";
                object[] objParm = null;
                bool blnRestartService = false;

                bool blnComplete = false;

                pvtlngDestinationFileStartIndex = 0;

                for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
                {
                    //strFileName = parDataTable.Rows[intRow]["FILE_NAME"].ToString();
                    intProgressBarCombinedMaxValue += Convert.ToInt32(parDataTable.Rows[intRow]["MAX_FILE_CHUNK_NO"]);
                }

                this.prbAllFileProgress.Maximum = intProgressBarCombinedMaxValue;
                this.prbAllFileProgress.Value = 0;

                //Download Files
                for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
                {
                    strProjectVersion = parDataTable.Rows[intRow]["PROJECT_VERSION"].ToString();
                    strFileLayerInd = parDataTable.Rows[intRow]["FILE_LAYER_IND"].ToString();
                    strFileName = parDataTable.Rows[intRow]["FILE_NAME"].ToString();
                    
                    strFilePathName = AppDomain.CurrentDomain.BaseDirectory + parDataTable.Rows[intRow]["FILE_NAME"].ToString();
#if(DEBUG)
                    strFilePathName = AppDomain.CurrentDomain.BaseDirectory + "bin\\" + parDataTable.Rows[intRow]["FILE_NAME"].ToString();

                    if (strFileName == "URLConfig.txt")
                    {
                        string strStop = "";
                    }
#endif
                    pvtlngDestinationFileStartIndex = 0;

                    intProgressBarMaxValue = Convert.ToInt32(parDataTable.Rows[intRow]["MAX_FILE_CHUNK_NO"]);

                    this.prbFileProgress.Maximum = intProgressBarMaxValue;
                    this.prbFileProgress.Value = 0;

                    pvtbytBytes = null;
                    pvtbytBytes = new byte[Convert.ToInt32(parDataTable.Rows[intRow]["FILE_SIZE_COMPRESSED"])];

                    this.lblFileName.Text = parDataTable.Rows[intRow]["FILE_NAME"].ToString();

                    if (strFileName.Substring(strFileName.Length - 1) == "_")
                    {
                        strFileDownloadName = strFileName.Substring(0, strFileName.Length - 1);
                    }
                    else
                    {
                        strFileDownloadName = strFileName;
                    }
                   
                    for (int intRow1 = 1; intRow1 <= Convert.ToInt32(parDataTable.Rows[intRow]["MAX_FILE_CHUNK_NO"]); intRow1++)
                    {
                        objParm = new object[6];
                        objParm[0] = Convert.ToInt64(parDataTable.Rows[intRow]["COMPANY_NO"]);
                        objParm[1] = strProjectVersion;
                        objParm[2] = parDataTable.Rows[intRow]["FROM_IND"].ToString();
                        objParm[3] = parDataTable.Rows[intRow]["FILE_LAYER_IND"].ToString();
                        objParm[4] = strFileDownloadName;
                        objParm[5] = intRow1;

                        pvtbytTempBytes = (byte[])clsISUtilities.DynamicFunction("Get_New_File_Chunk", objParm);

                        if (AppDomain.CurrentDomain.GetData("KillApp").ToString() == "Y")
                        {
                            break;
                        }

                        //0=Client Database 1=Presentation Layer
                        if (parDataTable.Rows[intRow]["FROM_IND"].ToString() == "0")
                        {
                            if (intRow1 == Convert.ToInt32(parDataTable.Rows[intRow]["MAX_FILE_CHUNK_NO"]))
                            {
                                blnComplete = true;
                            }
                            else
                            {
                                blnComplete = false;
                            }

                            objParm = new object[10];
                            objParm[0] = strFileDownloadName;
                            objParm[1] = parDataTable.Rows[intRow]["FILE_LAYER_IND"].ToString();
                            objParm[2] = intRow1;
                            objParm[3] = pvtbytTempBytes;
                            objParm[4] = blnComplete;
                            objParm[5] = parDataTable.Rows[intRow]["FILE_CRC_VALUE"].ToString();
                            objParm[6] = Convert.ToInt32(parDataTable.Rows[intRow]["FILE_SIZE_COMPRESSED"]);
                            objParm[7] = Convert.ToInt32(parDataTable.Rows[intRow]["FILE_SIZE"]);
                            objParm[8] = Convert.ToDateTime(parDataTable.Rows[intRow]["FILE_LAST_UPDATED_DATE"]);
                            objParm[9] = parDataTable.Rows[intRow]["FILE_VERSION_NO"].ToString();

                            //Client Database
                            int intReturnCode = (int)clsISClientUtilities.DynamicFunction("Insert_New_File_Chunk", objParm, false);

                            if (intReturnCode == 1)
                            {
                                System.Windows.Forms.MessageBox.Show("File CRC Error",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                                return 1;
                            }
                            else
                            {
                                if (intReturnCode == 9)
                                {
                                    blnRestartService = true;
                                }
                            }

                        }
                        else
                        {
                            Array.Copy(pvtbytTempBytes, 0, pvtbytBytes, pvtlngDestinationFileStartIndex, pvtbytTempBytes.Length);
                            pvtlngDestinationFileStartIndex += pvtbytTempBytes.Length;
                        }

                        this.prbAllFileProgress.Value += 1;
                        this.prbFileProgress.Value += 1;
                        this.Refresh();
                        Application.DoEvents();
                    }
                   
                    if (AppDomain.CurrentDomain.GetData("KillApp").ToString() == "Y")
                    {
                        this.Hide();

                        this.Refresh();

                        return 1;
                    }

                    if (parDataTable.Rows[intRow]["FROM_IND"].ToString() != "0")
                    {
                        pvtbytDecompressedBytes = null;
                        pvtbytDecompressedBytes = new byte[Convert.ToInt32(parDataTable.Rows[intRow]["FILE_SIZE"])];

                        //Open Memory Stream with Compressed Data
                        MemoryStream msMemoryStream = new MemoryStream(pvtbytBytes);

                        System.IO.Compression.GZipStream GZipStreamDecompress = new GZipStream(msMemoryStream, CompressionMode.Decompress);

                        //Decompress Bytes
                        pvtbrBinaryReader = new BinaryReader(GZipStreamDecompress);
                        pvtbytDecompressedBytes = pvtbrBinaryReader.ReadBytes(Convert.ToInt32(parDataTable.Rows[intRow]["FILE_SIZE"]));

                        if (parDataTable.Rows[intRow]["FILE_CRC_VALUE"].ToString() != "")
                        {
                            //CRC32 Value
                            strCRC32Value = "";

                            foreach (byte b in clsCrc32.ComputeHash(pvtbytDecompressedBytes))
                            {
                                strCRC32Value += b.ToString("x2").ToLower();
                            }

                            if (strCRC32Value != parDataTable.Rows[intRow]["FILE_CRC_VALUE"].ToString())
                            {
                                //Error
                                return 1;
                            }
                        }

                        pvtfsFileStream = null;
                        pvtbwBinaryWriter = null;

                        pvtfsFileStream = new FileStream(strFilePathName, FileMode.Create);
                        pvtbwBinaryWriter = new BinaryWriter(pvtfsFileStream);

                        pvtbwBinaryWriter.Write(pvtbytDecompressedBytes);

                        //Write Memory Portion To Disk
                        pvtbwBinaryWriter.Close();

                        File.SetLastWriteTime(strFilePathName, Convert.ToDateTime(parDataTable.Rows[intRow]["FILE_LAST_UPDATED_DATE"]));
                    }
                }

                this.Hide();

                this.Refresh();

                if (blnRestartService == true)
                {
                    return 99;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception eException)
            {
                ErrorHandler(eException);
                return -1;
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("KillApp", "Y");
        }

        public void ErrorHandler(Exception parException)
        {
            string strExceptionError;
            string strAreaFrom = parException.StackTrace;

            DateTime dtDateTime = DateTime.Now;

            String strDateTime = dtDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            strExceptionError = "Date/Time  : " + strDateTime
                + "\r\n" + "Where      : " + strAreaFrom
                + "\r\n" + "Error Desc : " + parException.Message;

            System.Windows.Forms.MessageBox.Show(strExceptionError,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            FileInfo fiErrorFile = new FileInfo("PayrollError.txt");

            StreamWriter swErrorStreamWriter = fiErrorFile.AppendText();

            swErrorStreamWriter.WriteLine("");
            swErrorStreamWriter.WriteLine(strExceptionError);

            swErrorStreamWriter.Close();

            System.Windows.Forms.MessageBox.Show(parException.InnerException.Message);
        }
    }
}
