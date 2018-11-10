using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace InteractPayroll
{
	public class clsFileDownLoad
	{
		frmReadWriteFile frmReadWriteFile;
        
		private DataTable pvtDataTable;
		private DataTable pvtVersionDataTable;
		private DataView pvtLocalFilesDataView;
		private DataTable pvtDownloadDataTable;

		private FileVersionInfo pvtFileVersionInfo;
		private FileInfo pvtfiFileInfo;

		private string pvtstrApplicationPath = "";
		
		public clsFileDownLoad()
		{
            pvtDataTable = new DataTable();

			pvtDataTable.Columns.Add("FILE_NAME", typeof(String));
			pvtDataTable.Columns.Add("FILE_LAST_UPDATED_DATE", typeof(DateTime));
			pvtDataTable.Columns.Add("FILE_SIZE", typeof(Double));
			pvtDataTable.Columns.Add("FILE_SIZE_COMPRESSED", typeof(Double));
			pvtDataTable.Columns.Add("FILE_VERSION_NO", typeof(String));
			pvtDataTable.Columns.Add("MAX_FILE_CHUNK_NO", typeof(Double));
            
			pvtVersionDataTable = new DataTable();

			pvtVersionDataTable.Columns.Add("PROJECT_VERSION", typeof(String));
			pvtVersionDataTable.Columns.Add("FOUND_IND", typeof(String));
		}

		public DataTable Get_Files_Directories()
		{
			pvtstrApplicationPath = AppDomain.CurrentDomain.BaseDirectory;

#if(DEBUG)
            pvtstrApplicationPath = AppDomain.CurrentDomain.BaseDirectory + "bin";
#endif
			//Get Directory Info
            DirectoryInfo di = new DirectoryInfo(pvtstrApplicationPath);
			
			//pvtTreeNode = the Final Node from Initial_Node_Link
			DirSearch(di);

			pvtLocalFilesDataView = new DataView(pvtDataTable,
				"",
				"FILE_NAME",
				DataViewRowState.CurrentRows);

			return pvtVersionDataTable;
		}

		private void DirSearch(DirectoryInfo sDir) 
		{
			DataRow drDataRow;
        
            FileInfo[] fsi = sDir.GetFiles();

            foreach (FileInfo info in fsi)
			{
				drDataRow = pvtDataTable.NewRow();

                drDataRow["FILE_NAME"] = info.Name.ToString();
                drDataRow["FILE_LAST_UPDATED_DATE"] = info.LastWriteTime;
                drDataRow["FILE_SIZE"] = info.Length.ToString();
				drDataRow["MAX_FILE_CHUNK_NO"] = 0;

				pvtDataTable.Rows.Add(drDataRow);
			}
		}

        public int DownLoad_Files_From_Database(DataTable pvtDownloadDataTable)
        {
            int intReturnCode = 1;

            frmReadWriteFile = new frmReadWriteFile();

            try
            {
                intReturnCode = frmReadWriteFile.DownLoad_Files_From_Database(pvtDownloadDataTable);
            }
            catch
            {
            }

            frmReadWriteFile.Dispose();
            frmReadWriteFile = null;

            return intReturnCode;
        }

        private DataSet DeCompress_Array_To_DataSet(byte[] parbytArray)
        {
            DataSet DataSet = new DataSet();
            DataSet.RemotingFormat = SerializationFormat.Binary;

            MemoryStream msMemoryStreamCompressed = new MemoryStream(parbytArray);
            System.IO.Compression.GZipStream GZipStreamCompressed = new GZipStream(msMemoryStreamCompressed, CompressionMode.Decompress, true);

            byte[] byteDecompressed = ReadFullStream(GZipStreamCompressed);
            GZipStreamCompressed.Flush();
            GZipStreamCompressed.Close();

            MemoryStream msMemoryStreamDecompressed = new MemoryStream(byteDecompressed);

            BinaryFormatter bf = new BinaryFormatter();
            DataSet = (DataSet)bf.Deserialize(msMemoryStreamDecompressed, null);

            return DataSet;
        }

        private byte[] ReadFullStream(Stream stream)
        {
            byte[] bytBuffer = new byte[32768];

            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int intCharCount = stream.Read(bytBuffer, 0, bytBuffer.Length);

                    if (intCharCount <= 0)
                    {
                        return ms.ToArray();
                    }
                    else
                    {
                        ms.Write(bytBuffer, 0, intCharCount);
                    }
                }
            }
        }

        public int DownLoad_Files(ref DataSet parDataSet,ref DataSet ClientDataSet, ref bool parblnLogoff, string strClientDBConnected) 
		{
          	int intFoundRow = -1;
			int intReturnCode = 0;
            object[] objFind = new object[2];

            string strKeepNewerDlls = "";

			DataRow drDataRow;
			
            DataView ClientDataView = null;

            DateTime dtDownloadMessage = DateTime.Now.AddSeconds(3);

            frmDownloadMessage frmDownloadMessage = new frmDownloadMessage();
           
            if (strClientDBConnected == "Y")
            {
                frmDownloadMessage.lblLocalFilesDesc.Visible = true;

                //Files Downloaded from Internet
                DataView FileDataView = new DataView(parDataSet.Tables["Files"], "PROJECT_VERSION IN ('_Client','_Beta','_Current') AND FILE_LAYER_IND = 'P'", "FILE_LAYER_IND,FILE_NAME", DataViewRowState.CurrentRows);

                ClientDataView = new DataView(ClientDataSet.Tables["ClientFile"], "FILE_LAYER_IND = 'P'", "FILE_LAYER_IND,FILE_NAME", DataViewRowState.CurrentRows);

                objFind[0] = "P";

                for (int intRow = 0; intRow < ClientDataView.Count; intRow++)
                {
                    objFind[1] = ClientDataView[intRow]["FILE_NAME"].ToString();

                    intFoundRow = FileDataView.Find(objFind);

                    if (intFoundRow == -1)
                    {
                        drDataRow = ClientDataSet.Tables["FileToDelete"].NewRow();

                        drDataRow["FILE_LAYER_IND"] = "P";
                        drDataRow["FILE_NAME"] = ClientDataView[intRow]["FILE_NAME"].ToString();

                        ClientDataSet.Tables["FileToDelete"].Rows.Add(drDataRow);
                    }
                }

                ClientDataView = null;
                ClientDataView = new DataView(ClientDataSet.Tables["ClientFile"], "", "FILE_LAYER_IND,FILE_NAME", DataViewRowState.CurrentRows);
            }

            frmDownloadMessage.Show();
            frmDownloadMessage.Refresh();
            
			parblnLogoff = false;

			pvtDownloadDataTable = new DataTable();

			pvtDownloadDataTable.Columns.Add("PROJECT_VERSION", typeof(String));
            pvtDownloadDataTable.Columns.Add("FILE_LAYER_IND", typeof(String));
            pvtDownloadDataTable.Columns.Add("FILE_NAME", typeof(String));
			pvtDownloadDataTable.Columns.Add("FILE_SIZE", typeof(Double));
			pvtDownloadDataTable.Columns.Add("FILE_SIZE_COMPRESSED", typeof(Double));
			pvtDownloadDataTable.Columns.Add("MAX_FILE_CHUNK_NO", typeof(Double));
            pvtDownloadDataTable.Columns.Add("FROM_IND", typeof(String));
            pvtDownloadDataTable.Columns.Add("COMPANY_NO", typeof(Int64));
			pvtDownloadDataTable.Columns.Add("FILE_LAST_UPDATED_DATE", typeof(DateTime));
            pvtDownloadDataTable.Columns.Add("FILE_VERSION_NO", typeof(String));
            pvtDownloadDataTable.Columns.Add("FILE_CRC_VALUE", typeof(String));

            for (int intRow = 0; intRow < parDataSet.Tables["Files"].Rows.Count; intRow++)
            {
                if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Client"
                || parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Beta"
                || parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Current")
                {
                    objFind[0] = parDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                    objFind[1] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();

                    intFoundRow = ClientDataView.Find(objFind);

                    //Version on S=Server with 
                    if (intFoundRow == -1
                    && objFind[0].ToString() == "S")
                    {
                        objFind[1] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "_";

                        intFoundRow = ClientDataView.Find(objFind);
                    }
                }
                else
                {
                    intFoundRow = pvtLocalFilesDataView.Find(parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString());
                }
#if(DEBUG)
                string strProjectVersion = parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();
                string strFileName = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();

                if (strFileName == "URLConfig.txt")
                {
                    string strOK = "";
                    string strLayerInd = parDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();

                    //ERROL ADDED TO TEST - MUST BE REMOVED
                    if (strLayerInd == "S")
                    {
                        intFoundRow = -1;
                    }
                }
#endif
                if (intFoundRow == -1)
                {
                    //Add for Download
                    drDataRow = pvtDownloadDataTable.NewRow();

                    drDataRow["PROJECT_VERSION"] = parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();

                    if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "Current"
                    && (parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLIS.EXE"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLL.EXE"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLLOGON.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNET.EXE"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETIS.EXE"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCELOGON.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETLOGON.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "DOWNLOADFILES.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PASSWORDCHANGE.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "URLCONFIG.TXT"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSDBCONNECTIONOBJECTS.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISCLIENTUTILITIES.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISUTILITIES.DLL"))
                    {
                        drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "_";
                        parblnLogoff = true;
                    }
                    else
                    {
                        drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                    }

                    drDataRow["FILE_LAYER_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                    drDataRow["FILE_SIZE"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE"]);
                    drDataRow["FILE_SIZE_COMPRESSED"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]);
                    drDataRow["MAX_FILE_CHUNK_NO"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"]);
                    drDataRow["FROM_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FROM_IND"].ToString();
                    drDataRow["COMPANY_NO"] = parDataSet.Tables["Files"].Rows[intRow]["COMPANY_NO"].ToString();
                    drDataRow["FILE_LAST_UPDATED_DATE"] = Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]);

                    drDataRow["FILE_CRC_VALUE"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString();

                    pvtDownloadDataTable.Rows.Add(drDataRow);
                }
                else
                {
#if(DEBUG)
                    strFileName = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                    string strFileOnServerDateTime = Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]).ToString("yyyy-MM-dd HH:mm:ss");
                    string strFileLocalDateTime = "";

                    if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Client"
                    || parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Beta"
                    || parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Current")
                    {
                        strFileLocalDateTime = Convert.ToDateTime(ClientDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        strFileLocalDateTime = Convert.ToDateTime(pvtLocalFilesDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).ToString("yyyy-MM-dd HH:mm:ss");
                    }
#endif
                    if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Client"
                    || parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Beta"
                    || parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Current")
                    {
                        if (Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) >= Convert.ToDateTime(ClientDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(-3)
                            & Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) <= Convert.ToDateTime(ClientDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(3))
                        {
                        }
                        else
                        {
                            //Add for DownLoad
                            drDataRow = pvtDownloadDataTable.NewRow();

                            drDataRow["PROJECT_VERSION"] = parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();
                            drDataRow["FILE_LAYER_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                            drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                            drDataRow["FILE_SIZE"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE"]);
                            drDataRow["FILE_SIZE_COMPRESSED"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]);
                            drDataRow["MAX_FILE_CHUNK_NO"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"]);
                            drDataRow["FROM_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FROM_IND"].ToString();
                            drDataRow["COMPANY_NO"] = parDataSet.Tables["Files"].Rows[intRow]["COMPANY_NO"].ToString();
                            drDataRow["FILE_LAST_UPDATED_DATE"] = Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]);
                            drDataRow["FILE_CRC_VALUE"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString();

                            pvtDownloadDataTable.Rows.Add(drDataRow);
                        }
                    }
                    else
                    {
                        if (AppDomain.CurrentDomain.GetData("UserNo").ToString() != "0")
                        {
                            if (Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) >= Convert.ToDateTime(pvtLocalFilesDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(-3)
                            && Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) <= Convert.ToDateTime(pvtLocalFilesDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(3))
                            {
                            }
                            else
                            {
                                //Add for DownLoad
                                drDataRow = pvtDownloadDataTable.NewRow();

                                drDataRow["PROJECT_VERSION"] = parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();

                                if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "Current"
                                && (parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLIS.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLL.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLLOGON.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNET.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETIS.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCELOGON.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETLOGON.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "DOWNLOADFILES.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PASSWORDCHANGE.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "URLCONFIG.TXT"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSDBCONNECTIONOBJECTS.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISCLIENTUTILITIES.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISUTILITIES.DLL"))
                                {
                                    parblnLogoff = true;
                                    drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "_";
                                }
                                else
                                {
                                    drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                                }

                                drDataRow["FILE_LAYER_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                                drDataRow["FILE_SIZE"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE"]);
                                drDataRow["FILE_SIZE_COMPRESSED"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]);
                                drDataRow["MAX_FILE_CHUNK_NO"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"]);
                                drDataRow["FROM_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FROM_IND"].ToString();
                                drDataRow["COMPANY_NO"] = parDataSet.Tables["Files"].Rows[intRow]["COMPANY_NO"].ToString();
                                drDataRow["FILE_LAST_UPDATED_DATE"] = Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]);
                                drDataRow["FILE_CRC_VALUE"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString();

                                pvtDownloadDataTable.Rows.Add(drDataRow);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) >= Convert.ToDateTime(pvtLocalFilesDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(3))
                            {
                                //Add for DownLoad
                                drDataRow = pvtDownloadDataTable.NewRow();

                                drDataRow["PROJECT_VERSION"] = parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();

                                if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "Current"
                                && (parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLIS.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLL.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLLOGON.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNET.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETIS.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCELOGON.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETLOGON.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "DOWNLOADFILES.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PASSWORDCHANGE.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "URLCONFIG.TXT"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSDBCONNECTIONOBJECTS.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISCLIENTUTILITIES.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISUTILITIES.DLL"))
                                {
                                    parblnLogoff = true;
                                    drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "_";
                                }
                                else
                                {
                                    drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                                }

                                drDataRow["FILE_LAYER_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                                drDataRow["FILE_SIZE"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE"]);
                                drDataRow["FILE_SIZE_COMPRESSED"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]);
                                drDataRow["MAX_FILE_CHUNK_NO"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"]);
                                drDataRow["FROM_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FROM_IND"].ToString();
                                drDataRow["COMPANY_NO"] = parDataSet.Tables["Files"].Rows[intRow]["COMPANY_NO"].ToString();
                                drDataRow["FILE_LAST_UPDATED_DATE"] = Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]);
                                drDataRow["FILE_CRC_VALUE"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString();

                                pvtDownloadDataTable.Rows.Add(drDataRow);
                            }
                            else
                            {
                                if (Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) <= Convert.ToDateTime(pvtLocalFilesDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(-3))
                                {
                                    if (strKeepNewerDlls == "")
                                    {
                                        DialogResult myDialogResult = MessageBox.Show("NEWER Dlls Have been Found than those Downloaded.\n\nWould you like to keep Newer Dlls?", "Newer DLLs",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                                        if (myDialogResult == DialogResult.No)
                                        {
                                            strKeepNewerDlls = "N";
                                        }
                                        else
                                        {
                                            strKeepNewerDlls = "Y";
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (strKeepNewerDlls == "Y")
                                        {
                                            continue;
                                        }
                                    }

                                    //Add for DownLoad
                                    drDataRow = pvtDownloadDataTable.NewRow();

                                    drDataRow["PROJECT_VERSION"] = parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();

                                    if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "Current"
                                    && (parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLIS.EXE"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLL.EXE"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLLOGON.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNET.EXE"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETIS.EXE"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCELOGON.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETLOGON.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "DOWNLOADFILES.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PASSWORDCHANGE.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "URLCONFIG.TXT"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSDBCONNECTIONOBJECTS.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISCLIENTUTILITIES.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISUTILITIES.DLL"))
                                    {
                                        if (parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLL.EXE")
                                        {
                                        }
                                        else
                                        {
                                            parblnLogoff = true;
                                        }

                                        drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "_";
                                    }
                                    else
                                    {
                                        drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                                    }

                                    drDataRow["FILE_LAYER_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                                    drDataRow["FILE_SIZE"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE"]);
                                    drDataRow["FILE_SIZE_COMPRESSED"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]);
                                    drDataRow["MAX_FILE_CHUNK_NO"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"]);
                                    drDataRow["FROM_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FROM_IND"].ToString();
                                    drDataRow["COMPANY_NO"] = parDataSet.Tables["Files"].Rows[intRow]["COMPANY_NO"].ToString();
                                    drDataRow["FILE_LAST_UPDATED_DATE"] = Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]);
                                    drDataRow["FILE_CRC_VALUE"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString();

                                    pvtDownloadDataTable.Rows.Add(drDataRow);
                                }
                            }
                        }
                    }
                }
            }

            while (dtDownloadMessage > DateTime.Now)
            {
                string strStop = "";
            }

            frmDownloadMessage.Close();
            frmDownloadMessage = null;

			if (pvtDownloadDataTable.Rows.Count > 0)
			{
                intReturnCode = DownLoad_Files_From_Database(pvtDownloadDataTable);
			}
	
			return intReturnCode;
		}

        public int DownLoad_Files_From_TimeAttendance(ref DataSet parDataSet, ref DataSet ClientDataSet, ref bool parblnLogoff, string strClientDBConnected)
        {
            int intFoundRow = -1;
            int intReturnCode = 0;
            object[] objFind = new object[2];

            string strKeepNewerDlls = "";

            DataRow drDataRow;

            DataView ClientDataView = null;

            DateTime dtDownloadMessage = DateTime.Now.AddSeconds(3);

            frmDownloadMessage frmDownloadMessage = new frmDownloadMessage();

            if (strClientDBConnected == "Y")
            {
                frmDownloadMessage.lblLocalFilesDesc.Visible = true;

                //Files Downloaded from Internet
                DataView FileDataView = new DataView(parDataSet.Tables["Files"], "PROJECT_VERSION IN ('_Client','_Beta','_Current') AND FILE_LAYER_IND = 'P'", "FILE_LAYER_IND,FILE_NAME", DataViewRowState.CurrentRows);

                ClientDataView = new DataView(ClientDataSet.Tables["ClientFile"], "FILE_LAYER_IND = 'P'", "FILE_LAYER_IND,FILE_NAME", DataViewRowState.CurrentRows);

                objFind[0] = "P";

                for (int intRow = 0; intRow < ClientDataView.Count; intRow++)
                {
                    objFind[1] = ClientDataView[intRow]["FILE_NAME"].ToString();

                    intFoundRow = FileDataView.Find(objFind);

                    if (intFoundRow == -1)
                    {
                        drDataRow = ClientDataSet.Tables["FileToDelete"].NewRow();

                        drDataRow["FILE_LAYER_IND"] = "P";
                        drDataRow["FILE_NAME"] = ClientDataView[intRow]["FILE_NAME"].ToString();

                        ClientDataSet.Tables["FileToDelete"].Rows.Add(drDataRow);
                    }
                }

                ClientDataView = null;
                ClientDataView = new DataView(ClientDataSet.Tables["ClientFile"], "", "FILE_LAYER_IND,FILE_NAME", DataViewRowState.CurrentRows);
            }

            frmDownloadMessage.Show();
            frmDownloadMessage.Refresh();

            parblnLogoff = false;

            pvtDownloadDataTable = new DataTable();

            pvtDownloadDataTable.Columns.Add("PROJECT_VERSION", typeof(String));
            pvtDownloadDataTable.Columns.Add("FILE_LAYER_IND", typeof(String));
            pvtDownloadDataTable.Columns.Add("FILE_NAME", typeof(String));
            pvtDownloadDataTable.Columns.Add("FILE_SIZE", typeof(Double));
            pvtDownloadDataTable.Columns.Add("FILE_SIZE_COMPRESSED", typeof(Double));
            pvtDownloadDataTable.Columns.Add("MAX_FILE_CHUNK_NO", typeof(Double));
            pvtDownloadDataTable.Columns.Add("FROM_IND", typeof(String));
            pvtDownloadDataTable.Columns.Add("COMPANY_NO", typeof(Int64));
            pvtDownloadDataTable.Columns.Add("FILE_LAST_UPDATED_DATE", typeof(DateTime));
            pvtDownloadDataTable.Columns.Add("FILE_VERSION_NO", typeof(String));
            pvtDownloadDataTable.Columns.Add("FILE_CRC_VALUE", typeof(String));

            for (int intRow = 0; intRow < parDataSet.Tables["Files"].Rows.Count; intRow++)
            {
                if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Client"
                || parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Beta"
                || parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Current")
                {
                    objFind[0] = parDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                    objFind[1] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();

                    intFoundRow = ClientDataView.Find(objFind);

                    //Version on S=Server with 
                    if (intFoundRow == -1
                    && objFind[0].ToString() == "S")
                    {
                        objFind[1] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "_";

                        intFoundRow = ClientDataView.Find(objFind);
                    }
                }
                else
                {
                    intFoundRow = pvtLocalFilesDataView.Find(parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString());
                }
#if(DEBUG)
                string strProjectVersion = parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();
                string strFileName = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();

                if (strFileName == "TimeAttendanceDataDownload.dll")
                {
                    string strOK = "";
                }
#endif
                if (intFoundRow == -1)
                {
                    //Add for Download
                    drDataRow = pvtDownloadDataTable.NewRow();

                    drDataRow["PROJECT_VERSION"] = parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();

                    if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "Current"
                    && (parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLIS.EXE"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLL.EXE"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLLOGON.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNET.EXE"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETIS.EXE"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCELOGON.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETLOGON.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "DOWNLOADFILES.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PASSWORDCHANGE.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "URLCONFIG.TXT"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSDBCONNECTIONOBJECTS.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISCLIENTUTILITIES.DLL"
                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISUTILITIES.DLL"))
                    {
                        drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "_";
                        parblnLogoff = true;
                    }
                    else
                    {
                        drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                    }

                    drDataRow["FILE_LAYER_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                    drDataRow["FILE_SIZE"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE"]);
                    drDataRow["FILE_SIZE_COMPRESSED"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]);
                    drDataRow["MAX_FILE_CHUNK_NO"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"]);
                    drDataRow["FROM_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FROM_IND"].ToString();
                    drDataRow["COMPANY_NO"] = parDataSet.Tables["Files"].Rows[intRow]["COMPANY_NO"].ToString();
                    drDataRow["FILE_LAST_UPDATED_DATE"] = Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]);

                    drDataRow["FILE_CRC_VALUE"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString();

                    pvtDownloadDataTable.Rows.Add(drDataRow);
                }
                else
                {
#if(DEBUG)
                    strFileName = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                    string strFileOnServerDateTime = Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]).ToString("yyyy-MM-dd HH:mm:ss");

                    if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Client"
                    || parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Beta"
                    || parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Current")
                    {
                        string strFileLocalDateTime = Convert.ToDateTime(ClientDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        string strFileLocalDateTime = Convert.ToDateTime(pvtLocalFilesDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).ToString("yyyy-MM-dd HH:mm:ss");
                    }
#endif
                    if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Client"
                    || parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Beta"
                    || parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "_Current")
                    {
                        if (Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) >= Convert.ToDateTime(ClientDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(-3)
                        & Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) <= Convert.ToDateTime(ClientDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(3))
                        {
                        }
                        else
                        {
                            //Add for DownLoad
                            drDataRow = pvtDownloadDataTable.NewRow();

                            drDataRow["PROJECT_VERSION"] = parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();
                            drDataRow["FILE_LAYER_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                            drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                            drDataRow["FILE_SIZE"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE"]);
                            drDataRow["FILE_SIZE_COMPRESSED"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]);
                            drDataRow["MAX_FILE_CHUNK_NO"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"]);
                            drDataRow["FROM_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FROM_IND"].ToString();
                            drDataRow["COMPANY_NO"] = parDataSet.Tables["Files"].Rows[intRow]["COMPANY_NO"].ToString();
                            drDataRow["FILE_LAST_UPDATED_DATE"] = Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]);
                            drDataRow["FILE_CRC_VALUE"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString();

                            pvtDownloadDataTable.Rows.Add(drDataRow);
                        }
                    }
                    else
                    {
                        if (AppDomain.CurrentDomain.GetData("UserNo").ToString() != "0")
                        {
                            if (Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) >= Convert.ToDateTime(pvtLocalFilesDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(-3)
                            && Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) <= Convert.ToDateTime(pvtLocalFilesDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(3))
                            {
                            }
                            else
                            {
                                //Add for DownLoad
                                drDataRow = pvtDownloadDataTable.NewRow();

                                drDataRow["PROJECT_VERSION"] = parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();

                                if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "Current"
                                && (parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLIS.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLL.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLLOGON.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNET.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETIS.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCELOGON.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETLOGON.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "DOWNLOADFILES.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PASSWORDCHANGE.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "URLCONFIG.TXT"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSDBCONNECTIONOBJECTS.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISCLIENTUTILITIES.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISUTILITIES.DLL"))
                                {
                                    parblnLogoff = true;
                                    drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "_";
                                }
                                else
                                {
                                    drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                                }

                                drDataRow["FILE_LAYER_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                                drDataRow["FILE_SIZE"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE"]);
                                drDataRow["FILE_SIZE_COMPRESSED"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]);
                                drDataRow["MAX_FILE_CHUNK_NO"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"]);
                                drDataRow["FROM_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FROM_IND"].ToString();
                                drDataRow["COMPANY_NO"] = parDataSet.Tables["Files"].Rows[intRow]["COMPANY_NO"].ToString();
                                drDataRow["FILE_LAST_UPDATED_DATE"] = Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]);
                                drDataRow["FILE_CRC_VALUE"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString();

                                pvtDownloadDataTable.Rows.Add(drDataRow);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) >= Convert.ToDateTime(pvtLocalFilesDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(3))
                            {
                                //Add for DownLoad
                                drDataRow = pvtDownloadDataTable.NewRow();

                                drDataRow["PROJECT_VERSION"] = parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();

                                if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "Current"
                                && (parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLIS.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLL.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLLOGON.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNET.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETIS.EXE"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCELOGON.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETLOGON.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "DOWNLOADFILES.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PASSWORDCHANGE.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "URLCONFIG.TXT"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSDBCONNECTIONOBJECTS.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISCLIENTUTILITIES.DLL"
                                || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISUTILITIES.DLL"))
                                {
                                    parblnLogoff = true;
                                    drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "_";
                                }
                                else
                                {
                                    drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                                }

                                drDataRow["FILE_LAYER_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                                drDataRow["FILE_SIZE"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE"]);
                                drDataRow["FILE_SIZE_COMPRESSED"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]);
                                drDataRow["MAX_FILE_CHUNK_NO"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"]);
                                drDataRow["FROM_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FROM_IND"].ToString();
                                drDataRow["COMPANY_NO"] = parDataSet.Tables["Files"].Rows[intRow]["COMPANY_NO"].ToString();
                                drDataRow["FILE_LAST_UPDATED_DATE"] = Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]);
                                drDataRow["FILE_CRC_VALUE"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString();

                                pvtDownloadDataTable.Rows.Add(drDataRow);
                            }
                            else
                            {
                                if (Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]) <= Convert.ToDateTime(pvtLocalFilesDataView[intFoundRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(-3))
                                {
                                    if (strKeepNewerDlls == "")
                                    {
                                        DialogResult myDialogResult = MessageBox.Show("NEWER Dlls Have been Found than those Downloaded.\n\nWould you like to keep Newer Dlls?", "Newer DLLs",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                                        if (myDialogResult == DialogResult.No)
                                        {
                                            strKeepNewerDlls = "N";
                                        }
                                        else
                                        {
                                            strKeepNewerDlls = "Y";
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (strKeepNewerDlls == "Y")
                                        {
                                            continue;
                                        }
                                    }

                                    //Add for DownLoad
                                    drDataRow = pvtDownloadDataTable.NewRow();

                                    drDataRow["PROJECT_VERSION"] = parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString();

                                    if (parDataSet.Tables["Files"].Rows[intRow]["PROJECT_VERSION"].ToString() == "Current"
                                    && (parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLIS.EXE"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLL.EXE"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLLLOGON.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNET.EXE"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETIS.EXE"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCELOGON.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "TIMEATTENDANCEINTERNETLOGON.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "DOWNLOADFILES.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PASSWORDCHANGE.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "URLCONFIG.TXT"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSDBCONNECTIONOBJECTS.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISCLIENTUTILITIES.DLL"
                                    || parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "CLSISUTILITIES.DLL"))
                                    {
                                        if (parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString().ToUpper() == "PAYROLL.EXE")
                                        {
                                        }
                                        else
                                        {
                                            parblnLogoff = true;
                                        }

                                        drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString() + "_";
                                    }
                                    else
                                    {
                                        drDataRow["FILE_NAME"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                                    }

                                    drDataRow["FILE_LAYER_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_LAYER_IND"].ToString();
                                    drDataRow["FILE_SIZE"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE"]);
                                    drDataRow["FILE_SIZE_COMPRESSED"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"]);
                                    drDataRow["MAX_FILE_CHUNK_NO"] = Convert.ToDouble(parDataSet.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"]);
                                    drDataRow["FROM_IND"] = parDataSet.Tables["Files"].Rows[intRow]["FROM_IND"].ToString();
                                    drDataRow["COMPANY_NO"] = parDataSet.Tables["Files"].Rows[intRow]["COMPANY_NO"].ToString();
                                    drDataRow["FILE_LAST_UPDATED_DATE"] = Convert.ToDateTime(parDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]);
                                    drDataRow["FILE_CRC_VALUE"] = parDataSet.Tables["Files"].Rows[intRow]["FILE_CRC_VALUE"].ToString();

                                    pvtDownloadDataTable.Rows.Add(drDataRow);
                                }
                            }
                        }
                    }
                }
            }

            while (dtDownloadMessage > DateTime.Now)
            {
            }

            frmDownloadMessage.Close();
            frmDownloadMessage = null;

            if (pvtDownloadDataTable.Rows.Count > 0)
            {
                if (pvtDownloadDataTable.Rows[0]["FILE_NAME"].ToString() == "FingerPrintClockServiceStartStop.dll"
                && pvtDownloadDataTable.Rows[0]["FILE_LAYER_IND"].ToString() == "P"
                && pvtDownloadDataTable.Rows.Count == 1)
                {
                    //Other Windows Service
                }
                else
                {
                    parblnLogoff = true;
                }

                intReturnCode = DownLoad_Files_From_Database(pvtDownloadDataTable);

                //Force Program To Restart which will do Restart of Service
                if (intReturnCode == 99)
                {
                    intReturnCode = 0;
                }

            }

            return intReturnCode;
        }
	}
}
