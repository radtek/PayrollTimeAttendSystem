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
using System.Reflection;

namespace InteractPayroll
{
    public partial class frmHelp : Form
    {
        clsISUtilities clsISUtilities;

        DataSet pvtDataSet;
        DataView pvtDataViewHelpLevelText;
        DataView pvtDataViewHelpLevelGraphicChunk;

        private int pvtCurrentLevelNo = 0;
        private int pvtintLevel1No = 0;
        private int pvtintLevel2No = 0;
        private int pvtintLevel3No = 0;
        private int pvtintLevel4No = 0;

        string pvtstrNodeKey = "";

        private byte[] pvtbytes;
        private byte[] pvtbytFileBytes;
       
        private FileStream pvtfsFileStream = null;
        
        private int pvtintNumberOfBytesToRead = 50000;
        private long pvtlngDestinationFileStartIndex = 0;

        private bool pvtblnNew = false;

        public frmHelp()
        {
            InitializeComponent();
        }

        private void frmHelp_Load(object sender, EventArgs e)
        {
            clsISUtilities = new clsISUtilities(this, "busPayrollHelp");

            byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", null);

            pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

            //Get Empty 
            pvtDataViewHelpLevelGraphicChunk = null;
            pvtDataViewHelpLevelGraphicChunk = new DataView(pvtDataSet.Tables["HelpLevelGraphicChunk"],
            "HELP_LEVEL1_NO = -1  ",
            "",
            DataViewRowState.CurrentRows);

            Build_Profile();
   
            Microsoft.Reporting.WinForms.ReportDataSource myReportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource("pvtDataSet",pvtDataSet.Tables["Report"]);

            this.reportViewer.LocalReport.DataSources.Clear();
            this.reportViewer.LocalReport.DataSources.Add(myReportDataSource);
        }

        private void Build_Profile()
        {
            TreeNode ndeLevel1;
            TreeNode ndeLevel2;
            TreeNode ndeLevel3;
            TreeNode ndeLevel4;

            this.tvwTreeViewContent.Nodes.Clear();

            int intRow = 0;

            if (this.pvtDataSet.Tables["HelpLevelDesc"].Rows.Count > 0)
            {
                while (true)
                {
                    ndeLevel1 = null;
                    ndeLevel1 = new TreeNode();

                    pvtintLevel1No = Convert.ToInt32(pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL1_NO"]);

                    pvtstrNodeKey = pvtintLevel1No.ToString().PadLeft(5, '0');

                    ndeLevel1.Text = pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL1_DESC"].ToString();
                    ndeLevel1.Tag = pvtstrNodeKey;

                    //Add Parent Node
                    this.tvwTreeViewContent.Nodes.Add(ndeLevel1);

                    while (Convert.ToInt32(pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL1_NO"]) == pvtintLevel1No)
                    {
                        if (Convert.ToInt32(pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL2_NO"]) == 0)
                        {
                            intRow += 1;

                            if (intRow == this.pvtDataSet.Tables["HelpLevelDesc"].Rows.Count)
                            {
                                goto Build_Profile_Continue;
                            }

                            break;
                        }
                        else
                        {
                            ndeLevel2 = null;
                            ndeLevel2 = new TreeNode();

                            pvtintLevel2No = Convert.ToInt32(pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL2_NO"]);

                            pvtstrNodeKey = pvtintLevel1No.ToString().PadLeft(5, '0')
                                + pvtintLevel2No.ToString().PadLeft(5, '0');

                            ndeLevel2.Text = pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL2_DESC"].ToString();
                            ndeLevel2.Tag = pvtstrNodeKey;

                            ndeLevel1.Nodes.Add(ndeLevel2);

                            while (Convert.ToInt32(pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL1_NO"]) == pvtintLevel1No
                                & Convert.ToInt32(pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL2_NO"]) == pvtintLevel2No)
                            {
                                if (Convert.ToInt32(pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL3_NO"]) == 0)
                                {
                                    intRow += 1;

                                    if (intRow == this.pvtDataSet.Tables["HelpLevelDesc"].Rows.Count)
                                    {
                                        goto Build_Profile_Continue;
                                    }

                                    break;
                                }
                                else
                                {
                                    ndeLevel3 = null;
                                    ndeLevel3 = new TreeNode();

                                    pvtintLevel3No = Convert.ToInt32(pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL3_NO"]);

                                    pvtstrNodeKey = pvtintLevel1No.ToString().PadLeft(5, '0')
                                        + pvtintLevel2No.ToString().PadLeft(5, '0')
                                        + pvtintLevel3No.ToString().PadLeft(5, '0');

                                    ndeLevel3.Text = pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL3_DESC"].ToString();
                                    ndeLevel3.Tag = pvtstrNodeKey;

                                    ndeLevel2.Nodes.Add(ndeLevel3);

                                    while (Convert.ToInt32(pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL1_NO"]) == pvtintLevel1No
                                        & Convert.ToInt32(pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL2_NO"]) == pvtintLevel2No
                                        & Convert.ToInt32(pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL3_NO"]) == pvtintLevel3No)
                                    {
                                        if (Convert.ToInt32(pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL4_NO"]) == 0)
                                        {
                                            intRow += 1;

                                            if (intRow == this.pvtDataSet.Tables["HelpLevelDesc"].Rows.Count)
                                            {
                                                goto Build_Profile_Continue;
                                            }

                                            break;
                                        }
                                        else
                                        {
                                            ndeLevel4 = null;
                                            ndeLevel4 = new TreeNode();

                                            pvtintLevel4No = Convert.ToInt32(pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL4_NO"]);

                                            pvtstrNodeKey = pvtintLevel1No.ToString().PadLeft(5, '0')
                                                + pvtintLevel2No.ToString().PadLeft(5, '0')
                                                + pvtintLevel3No.ToString().PadLeft(5, '0')
                                                + pvtintLevel4No.ToString().PadLeft(5, '0');

                                            ndeLevel4.Text = pvtDataSet.Tables["HelpLevelDesc"].Rows[intRow]["HELP_LEVEL4_DESC"].ToString();

                                            ndeLevel4.Tag = pvtstrNodeKey;

                                            ndeLevel3.Nodes.Add(ndeLevel4);

                                            //Add Row
                                            intRow += 1;

                                            if (intRow == this.pvtDataSet.Tables["HelpLevelDesc"].Rows.Count)
                                            {
                                                goto Build_Profile_Continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        Build_Profile_Continue:

            this.pvtDataSet.AcceptChanges();
        }

        private void tvwTreeViewContent_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.tvwTreeViewContent.SelectedNode != null)
            {
                //Used To Highlight Selected Node when Fired from Edit Button
                this.tvwTreeViewContent.Focus();

                this.txtDetail.Text = "";

                this.btnNew.Enabled = true;
                this.btnUpdate.Enabled = true;
                this.btnDelete.Enabled = true;
                this.btnDelImage.Enabled = false;

                pvtDataSet.Tables["Report"].Rows.Clear();

                string strLevel1Desc = "";
                string strLevel2Desc = "";
                string strLevel3Desc = "";
                string strLevel4Desc = "";

                TreeNode tv = (TreeNode)this.tvwTreeViewContent.SelectedNode;

                string strMainKey = tv.Tag.ToString().Substring(0, 5);

                DataView myDataView = new DataView(pvtDataSet.Tables["HelpLevelText"],
                    "HELP_LEVEL1_NO = " + strMainKey,
                    "",
                    DataViewRowState.CurrentRows);

                if (myDataView.Count == 0)
                {
                    //During Editing their might be No Text but Images
                    this.Delete_Text_Image_For_Main_Node(Convert.ToInt32(strMainKey));

                    object[] objParm = new object[1];

                    objParm[0] = Convert.ToInt32(strMainKey);

                    byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Main_Node_Text_Records", objParm);

                    DataSet TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);
                    pvtDataSet.Merge(TempDataSet);

                    int intProgressBarValue = 1;

                    this.prgProgressBar.Value = 0;
                    this.prgProgressBar.Minimum = 0;
                   
                    DataView myDataViewHelpLevelNumberChunks = new DataView(pvtDataSet.Tables["HelpLevelNumberChunks"],
                    "HELP_LEVEL1_NO = " + strMainKey,
                    "",
                    DataViewRowState.CurrentRows);

                    for (int intChunkRow = 0; intChunkRow < myDataViewHelpLevelNumberChunks.Count; intChunkRow++)
                    {
                        intProgressBarValue += Convert.ToInt32(myDataViewHelpLevelNumberChunks[intChunkRow]["HELP_GRAPHIC_NUMBER_OF_CHUNKS"]);
                    }

                    this.prgProgressBar.Maximum = intProgressBarValue;

                    this.prgProgressBar.Visible = true;
                    this.prgProgressBar.Refresh();

                    this.prgProgressBar.Value = 1;

                    for (int intChunkRow = 0; intChunkRow < myDataViewHelpLevelNumberChunks.Count; intChunkRow++)
                    {
                        objParm = new object[5];

                        objParm[0] = Convert.ToInt32(myDataViewHelpLevelNumberChunks[intChunkRow]["HELP_LEVEL1_NO"]);
                        objParm[1] = Convert.ToInt32(myDataViewHelpLevelNumberChunks[intChunkRow]["HELP_LEVEL2_NO"]);
                        objParm[2] = Convert.ToInt32(myDataViewHelpLevelNumberChunks[intChunkRow]["HELP_LEVEL3_NO"]);
                        objParm[3] = Convert.ToInt32(myDataViewHelpLevelNumberChunks[intChunkRow]["HELP_LEVEL4_NO"]);

                        for (int intChunkNo = 1; intChunkNo <= Convert.ToInt32(myDataViewHelpLevelNumberChunks[intChunkRow]["HELP_GRAPHIC_NUMBER_OF_CHUNKS"]); intChunkNo++)
                        {
                            objParm[4] = intChunkNo;

                            bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_File_Chunk", objParm);

                            TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);
                            pvtDataSet.Merge(TempDataSet);

                            this.prgProgressBar.Value += 1;
                            this.prgProgressBar.Refresh();
#if(DEBUG)
                            DateTime dtWait = DateTime.Now.AddMilliseconds(200);

                            while (dtWait > DateTime.Now)
                            {
                                Application.DoEvents();
                            }
#endif
                        }
                    }

                    this.prgProgressBar.Visible = false;
                    this.prgProgressBar.Refresh();
                }

                //Level1
                string strBuildKey = "HELP_LEVEL1_NO = " + Convert.ToInt32(strMainKey);

                pvtintLevel1No = Convert.ToInt32(strMainKey);
                pvtCurrentLevelNo = 1;

                if (tv.Tag.ToString().Length >= 10)
                {
                    strBuildKey += " AND HELP_LEVEL2_NO = " + Convert.ToInt32(tv.Tag.ToString().Substring(5, 5));
                    pvtintLevel2No = Convert.ToInt32(tv.Tag.ToString().Substring(5, 5));
                    pvtCurrentLevelNo = 2;
                }
                else
                {
                    pvtintLevel2No = 0;
                }

                if (tv.Tag.ToString().Length >= 15)
                {
                    strBuildKey += " AND HELP_LEVEL3_NO = " + Convert.ToInt32(tv.Tag.ToString().Substring(10, 5));
                    pvtintLevel3No = Convert.ToInt32(tv.Tag.ToString().Substring(10, 5));
                    pvtCurrentLevelNo = 3;
                }
                else
                {
                    pvtintLevel3No = 0;
                }

                if (tv.Tag.ToString().Length >= 20)
                {
                    strBuildKey += " AND HELP_LEVEL4_NO = " + Convert.ToInt32(tv.Tag.ToString().Substring(15, 5));
                    pvtintLevel4No = Convert.ToInt32(tv.Tag.ToString().Substring(15, 5));
                    pvtCurrentLevelNo = 4;
                }
                else
                {
                    pvtintLevel4No = 0;
                }

                myDataView = null;
                myDataView = new DataView(pvtDataSet.Tables["HelpLevelDesc"],
                    strBuildKey,
                    "HELP_LEVEL1_DESC,HELP_LEVEL2_DESC,HELP_LEVEL3_DESC,HELP_LEVEL4_DESC",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < myDataView.Count; intRow++)
                {
                    if (myDataView[intRow]["HELP_LEVEL1_DESC"].ToString() != ""
                        & strLevel1Desc != myDataView[intRow]["HELP_LEVEL1_DESC"].ToString())
                    {
                        DataRow drDataRow = pvtDataSet.Tables["Report"].NewRow();
                        pvtDataSet.Tables["Report"].Rows.Add(drDataRow);
                        pvtDataSet.Tables["Report"].Rows[intRow]["LEVEL1_DESC"] = myDataView[intRow]["HELP_LEVEL1_DESC"].ToString();

                        strLevel1Desc = myDataView[intRow]["HELP_LEVEL1_DESC"].ToString();

                        if (tv.Text.ToString() == strLevel1Desc)
                        {
                            this.txtNode.Text = strLevel1Desc;
                        }

                        pvtDataViewHelpLevelText = null;
                        pvtDataViewHelpLevelText = new DataView(pvtDataSet.Tables["HelpLevelText"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = 0",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelText.Count > 0)
                        {
                            pvtDataSet.Tables["Report"].Rows[intRow]["LEVEL1_TEXT"] = pvtDataViewHelpLevelText[0]["HELP_TEXT"].ToString();

                            if (tv.Text.ToString() == strLevel1Desc)
                            {
                                this.txtDetail.Text = pvtDataViewHelpLevelText[0]["HELP_TEXT"].ToString();
                            }
                        }
                        
                        pvtDataViewHelpLevelGraphicChunk = null;
                        pvtDataViewHelpLevelGraphicChunk = new DataView(pvtDataSet.Tables["HelpLevelGraphicChunk"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = 0",
                        "HELP_CHUNK_NO",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelGraphicChunk.Count > 0)
                        {
                            pvtbytFileBytes = null;
                            pvtbytFileBytes = new byte[Convert.ToInt32(pvtDataViewHelpLevelGraphicChunk[0]["HELP_IMAGE_SIZE"])];

                            pvtlngDestinationFileStartIndex = 0;

                            for (int intRowNo = 0; intRowNo < pvtDataViewHelpLevelGraphicChunk.Count; intRowNo++)
                            {
                                pvtbytes = (byte[])pvtDataViewHelpLevelGraphicChunk[intRowNo]["HELP_CHUNK_IMAGE"];

                                Array.Copy(pvtbytes, 0, pvtbytFileBytes, pvtlngDestinationFileStartIndex, pvtbytes.Length);
                                pvtlngDestinationFileStartIndex += pvtbytes.Length;
                            }

                            pvtDataSet.Tables["Report"].Rows[intRow]["LEVEL1_IMAGE"] = pvtbytFileBytes;

                            if (tv.Text.ToString() == strLevel1Desc)
                            {
                                btnDelImage.Enabled = true;
                            }
                        }
                    }

                    if (myDataView[intRow]["HELP_LEVEL2_DESC"].ToString() != ""
                        & strLevel2Desc != myDataView[intRow]["HELP_LEVEL2_DESC"].ToString())
                    {
                        DataRow drDataRow = pvtDataSet.Tables["Report"].NewRow();
                        pvtDataSet.Tables["Report"].Rows.Add(drDataRow);
                        pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_DESC"] = myDataView[intRow]["HELP_LEVEL2_DESC"].ToString();

                        strLevel2Desc = myDataView[intRow]["HELP_LEVEL2_DESC"].ToString();

                        if (tv.Text.ToString() == strLevel2Desc)
                        {
                            this.txtNode.Text = strLevel2Desc;
                        }

                        pvtDataViewHelpLevelText = null;
                        pvtDataViewHelpLevelText = new DataView(pvtDataSet.Tables["HelpLevelText"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL2_NO"].ToString()) + " AND HELP_LEVEL3_NO = 0",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelText.Count > 0)
                        {
                            pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_TEXT"] = pvtDataViewHelpLevelText[0]["HELP_TEXT"].ToString();

                            if (tv.Text.ToString() == strLevel2Desc)
                            {
                                this.txtDetail.Text = pvtDataViewHelpLevelText[0]["HELP_TEXT"].ToString();
                            }
                        }
                        
                        pvtDataViewHelpLevelGraphicChunk = null;
                        pvtDataViewHelpLevelGraphicChunk = new DataView(pvtDataSet.Tables["HelpLevelGraphicChunk"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL2_NO"].ToString()) + " AND HELP_LEVEL3_NO = 0",
                        "HELP_CHUNK_NO",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelGraphicChunk.Count > 0)
                        {
                            pvtbytFileBytes = null;
                            pvtbytFileBytes = new byte[Convert.ToInt32(pvtDataViewHelpLevelGraphicChunk[0]["HELP_IMAGE_SIZE"])];

                            pvtlngDestinationFileStartIndex = 0;

                            for (int intRowNo = 0; intRowNo < pvtDataViewHelpLevelGraphicChunk.Count; intRowNo++)
                            {
                                pvtbytes = (byte[])pvtDataViewHelpLevelGraphicChunk[intRowNo]["HELP_CHUNK_IMAGE"];

                                Array.Copy(pvtbytes, 0, pvtbytFileBytes, pvtlngDestinationFileStartIndex, pvtbytes.Length);
                                pvtlngDestinationFileStartIndex += pvtbytes.Length;
                            }

                            pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_IMAGE"] = pvtbytFileBytes;

                            if (tv.Text.ToString() == strLevel2Desc)
                            {
                                btnDelImage.Enabled = true;
                            }
                        }
                    }

                    if (myDataView[intRow]["HELP_LEVEL3_DESC"].ToString() != ""
                        & strLevel3Desc != myDataView[intRow]["HELP_LEVEL3_DESC"].ToString())
                    {
                        DataRow drDataRow = pvtDataSet.Tables["Report"].NewRow();
                        pvtDataSet.Tables["Report"].Rows.Add(drDataRow);
                        pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_DESC"] = myDataView[intRow]["HELP_LEVEL3_DESC"].ToString();

                        strLevel3Desc = myDataView[intRow]["HELP_LEVEL3_DESC"].ToString();

                        if (tv.Text.ToString() == strLevel3Desc)
                        {
                            this.txtNode.Text = strLevel3Desc;
                        }

                        pvtDataViewHelpLevelText = null;
                        pvtDataViewHelpLevelText = new DataView(pvtDataSet.Tables["HelpLevelText"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL2_NO"].ToString()) + " AND HELP_LEVEL3_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL3_NO"].ToString()) + " AND HELP_LEVEL4_NO = 0",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelText.Count > 0)
                        {
                            pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_TEXT"] = pvtDataViewHelpLevelText[0]["HELP_TEXT"].ToString();

                            if (tv.Text.ToString() == strLevel3Desc)
                            {
                                this.txtDetail.Text = pvtDataViewHelpLevelText[0]["HELP_TEXT"].ToString();
                            }
                        }
                       
                        pvtDataViewHelpLevelGraphicChunk = null;
                        pvtDataViewHelpLevelGraphicChunk = new DataView(pvtDataSet.Tables["HelpLevelGraphicChunk"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL2_NO"].ToString()) + " AND HELP_LEVEL3_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL3_NO"].ToString()) + " AND HELP_LEVEL4_NO = 0",
                        "HELP_CHUNK_NO",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelGraphicChunk.Count > 0)
                        {
                            pvtbytFileBytes = null;
                            pvtbytFileBytes = new byte[Convert.ToInt32(pvtDataViewHelpLevelGraphicChunk[0]["HELP_IMAGE_SIZE"])];

                            pvtlngDestinationFileStartIndex = 0;

                            for (int intRowNo = 0; intRowNo < pvtDataViewHelpLevelGraphicChunk.Count; intRowNo++)
                            {
                                pvtbytes = (byte[])pvtDataViewHelpLevelGraphicChunk[intRowNo]["HELP_CHUNK_IMAGE"];

                                Array.Copy(pvtbytes, 0, pvtbytFileBytes, pvtlngDestinationFileStartIndex, pvtbytes.Length);
                                pvtlngDestinationFileStartIndex += pvtbytes.Length;
                            }

                            pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_IMAGE"] = pvtbytFileBytes;

                            if (tv.Text.ToString() == strLevel3Desc)
                            {
                                btnDelImage.Enabled = true;
                            }
                        }
                    }

                    if (myDataView[intRow]["HELP_LEVEL4_DESC"].ToString() != "")
                    {
                        DataRow drDataRow = pvtDataSet.Tables["Report"].NewRow();
                        pvtDataSet.Tables["Report"].Rows.Add(drDataRow);
                        pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_DESC"] = myDataView[intRow]["HELP_LEVEL4_DESC"].ToString();

                        strLevel4Desc = myDataView[intRow]["HELP_LEVEL4_DESC"].ToString();

                        if (tv.Text.ToString() == myDataView[intRow]["HELP_LEVEL4_DESC"].ToString())
                        {
                            this.txtNode.Text = myDataView[intRow]["HELP_LEVEL4_DESC"].ToString();
                            this.btnNew.Enabled = false;
                        }

                        pvtDataViewHelpLevelText = null;
                        pvtDataViewHelpLevelText = new DataView(pvtDataSet.Tables["HelpLevelText"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL2_NO"].ToString()) + " AND HELP_LEVEL3_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL3_NO"].ToString()) + " AND HELP_LEVEL4_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL4_NO"].ToString()),
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelText.Count > 0)
                        {
                            pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_TEXT"] = pvtDataViewHelpLevelText[0]["HELP_TEXT"].ToString();
                       
                            if (tv.Text.ToString() == strLevel4Desc)
                            {
                                this.txtDetail.Text = pvtDataViewHelpLevelText[0]["HELP_TEXT"].ToString();
                            }
                        }

                        pvtDataViewHelpLevelGraphicChunk = null;
                        pvtDataViewHelpLevelGraphicChunk = new DataView(pvtDataSet.Tables["HelpLevelGraphicChunk"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL2_NO"].ToString()) + " AND HELP_LEVEL3_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL3_NO"].ToString()) + " AND HELP_LEVEL4_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL4_NO"].ToString()),
                        "HELP_CHUNK_NO",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelGraphicChunk.Count > 0)
                        {
                            pvtbytFileBytes = null;
                            pvtbytFileBytes = new byte[Convert.ToInt32(pvtDataViewHelpLevelGraphicChunk[0]["HELP_IMAGE_SIZE"])];

                            pvtlngDestinationFileStartIndex = 0;

                            for (int intRowNo = 0; intRowNo < pvtDataViewHelpLevelGraphicChunk.Count; intRowNo++)
                            {
                                pvtbytes = (byte[])pvtDataViewHelpLevelGraphicChunk[intRowNo]["HELP_CHUNK_IMAGE"];

                                Array.Copy(pvtbytes, 0, pvtbytFileBytes, pvtlngDestinationFileStartIndex, pvtbytes.Length);
                                pvtlngDestinationFileStartIndex += pvtbytes.Length;
                            }

                            pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_IMAGE"] = pvtbytFileBytes;

                            if (tv.Text.ToString() == strLevel4Desc)
                            {
                                btnDelImage.Enabled = true;
                            }
                        }
                    }
                }

                this.reportViewer.RefreshReport();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (this.tvwSearchTreeView.Nodes.Count > 0)
            {
                this.tvwSearchTreeView.Nodes.Clear();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            pvtDataSet.Tables["Report"].Rows.Clear();
            this.reportViewer.RefreshReport();

            if (this.txtSearch.Text.Trim() != "")
            {
                if (pvtDataSet.Tables["HelpSearch"] != null)
                {
                    pvtDataSet.Tables.Remove("HelpSearch");
                }

                object[] objParm = new object[1];

                objParm[0] = this.txtSearch.Text.Trim();

                byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Search_For_String", objParm);

                DataSet DataSetTemp = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);

                pvtDataSet.Merge(DataSetTemp);

                string strNodeKey = "";
                string strNodeDesc = "";
                TreeNode ndeLevel;

                if (pvtDataSet.Tables["HelpSearch"].Rows.Count == 0)
                {
                    MessageBox.Show("No Data Found for Search Criteria", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information); 
                }
                else
                {
                    for (int intRow = 0; intRow < pvtDataSet.Tables["HelpSearch"].Rows.Count; intRow++)
                    {
                        ndeLevel = null;
                        ndeLevel = new TreeNode();

                        if (Convert.ToInt32(pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL4_NO"]) > 0)
                        {
                            strNodeKey = Convert.ToInt32(pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL1_NO"]).ToString().PadLeft(5, '0')
                                                   + Convert.ToInt32(pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL2_NO"]).ToString().PadLeft(5, '0')
                                                   + Convert.ToInt32(pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL3_NO"]).ToString().PadLeft(5, '0')
                                                   + Convert.ToInt32(pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL4_NO"]).ToString().PadLeft(5, '0');

                            strNodeDesc = pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL1_DESC"].ToString();
                            strNodeDesc += "->" + pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL2_DESC"].ToString();
                            strNodeDesc += "->" + pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL3_DESC"].ToString();
                            strNodeDesc += "->" + pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL4_DESC"].ToString();
                        }
                        else
                        {
                            if (Convert.ToInt32(pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL3_NO"]) > 0)
                            {
                                strNodeKey = Convert.ToInt32(pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL1_NO"]).ToString().PadLeft(5, '0')
                                                       + Convert.ToInt32(pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL2_NO"]).ToString().PadLeft(5, '0')
                                                       + Convert.ToInt32(pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL3_NO"]).ToString().PadLeft(5, '0');

                                strNodeDesc = pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL1_DESC"].ToString();
                                strNodeDesc += "->" + pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL2_DESC"].ToString();
                                strNodeDesc += "->" + pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL3_DESC"].ToString();
                            }
                            else
                            {
                                if (Convert.ToInt32(pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL2_NO"]) > 0)
                                {
                                    strNodeKey = Convert.ToInt32(pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL1_NO"]).ToString().PadLeft(5, '0')
                                                           + Convert.ToInt32(pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL2_NO"]).ToString().PadLeft(5, '0');

                                    strNodeDesc = pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL1_DESC"].ToString();
                                    strNodeDesc += "->" + pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL2_DESC"].ToString();
                                }
                                else
                                {
                                    strNodeKey = Convert.ToInt32(pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL1_NO"]).ToString().PadLeft(5, '0');
                                    strNodeDesc = pvtDataSet.Tables["HelpSearch"].Rows[intRow]["HELP_LEVEL1_DESC"].ToString();
                                }
                            }
                        }

                        ndeLevel.Text = strNodeDesc;
                        ndeLevel.Tag = strNodeKey;

                        //Add Parent Node
                        this.tvwSearchTreeView.Nodes.Add(ndeLevel);
                    }
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            pvtblnNew = true;

            this.grbEdit.Text = "Edit - New";

            this.btnDelImage.Enabled = false;

            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.txtDetail.Enabled = true;

            this.btnFile.Enabled = true;

            this.rbnLevel1.Enabled = true;
            this.rbnLinkNode.Enabled = true;

            this.rbnLevel1.Checked = false;
            this.rbnLinkNode.Checked = false;

            this.tvwTreeViewContent.Enabled = false;

            this.txtDetail.Text = "";

            this.txtNode.Text = "";
            this.txtNode.Enabled = true;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            pvtblnNew = false;

            this.grbEdit.Text = "Edit - Update";

            this.btnDelImage.Enabled = false;

            this.btnNew.Enabled = false;
            this.btnUpdate.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.btnFile.Enabled = true;

            this.txtDetail.Enabled = true;

            this.tvwTreeViewContent.Enabled = false;

            this.txtNode.Enabled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.txtNode.Text.Trim() == "")
            {
                MessageBox.Show("Enter Node Text");

                this.txtNode.Focus();

                return;
            }

            if (this.pvtblnNew == true)
            {
                if (this.rbnLevel1.Checked == false
                    & this.rbnLinkNode.Checked == false)
                {
                    MessageBox.Show("Choose TreeView Level");

                    return;
                }
            }

            this.grbEdit.Text = "Edit";

            object[] objParm = new object[8];

            if (this.pvtblnNew == true)
            {
                objParm[0] = "A";
            }
            else
            {
                objParm[0] = "U";
            }

            objParm[1] = this.pvtCurrentLevelNo;

            objParm[2] = this.pvtintLevel1No;
            objParm[3] = this.pvtintLevel2No;
            objParm[4] = this.pvtintLevel3No;
            objParm[5] = this.pvtintLevel4No;
            objParm[6] = this.txtNode.Text;
            objParm[7] = this.txtDetail.Text;

            int intReturnLevelNo = (int)clsISUtilities.DynamicFunction("Update_Structure", objParm);

            if (this.pvtblnNew == true)
            {
                TreeNode ndeNode = new TreeNode();

                ndeNode.Text = txtNode.Text.Trim();

                DataRow myNewRow;
                
                if (pvtCurrentLevelNo == 1)
                {
                    ndeNode.Tag = intReturnLevelNo.ToString("00000");

                    this.tvwTreeViewContent.Nodes.Add(ndeNode);

                    myNewRow = this.pvtDataSet.Tables["HelpLevelDesc"].NewRow();

                    myNewRow["HELP_LEVEL1_NO"] = intReturnLevelNo;
                    myNewRow["HELP_LEVEL2_NO"] = 0;
                    myNewRow["HELP_LEVEL3_NO"] = 0;
                    myNewRow["HELP_LEVEL4_NO"] = 0;
                    myNewRow["HELP_LEVEL1_DESC"] = this.txtNode.Text.Trim();

                    this.pvtDataSet.Tables["HelpLevelDesc"].Rows.Add(myNewRow);
                 }
                else
                {
                    string strBuildKey = "HELP_LEVEL1_NO = " + pvtintLevel1No;

                    if (pvtCurrentLevelNo == 2)
                    {
                        strBuildKey += " AND HELP_LEVEL2_NO = 0";
                    }
                    else
                    {
                        strBuildKey += " AND HELP_LEVEL2_NO = " + pvtintLevel2No;

                        if (pvtCurrentLevelNo == 3)
                        {
                            strBuildKey += " AND HELP_LEVEL3_NO = 0";
                        }
                        else
                        {
                            strBuildKey += " AND HELP_LEVEL3_NO = " + pvtintLevel3No;
                            strBuildKey += " AND HELP_LEVEL4_NO = 0";
                        }
                    }

                    DataView myDataView = new DataView(pvtDataSet.Tables["HelpLevelDesc"],
                        strBuildKey,
                        "",
                        DataViewRowState.CurrentRows);

                    if (myDataView.Count == 1)
                    {
                        //Update
                        if (pvtCurrentLevelNo == 2)
                        {
                            myDataView[0]["HELP_LEVEL2_DESC"] = this.txtNode.Text.Trim();
                            myDataView[0]["HELP_LEVEL2_NO"] = intReturnLevelNo;
                            
                        }
                        else
                        {
                            if (pvtCurrentLevelNo == 3)
                            {
                                myDataView[0]["HELP_LEVEL3_DESC"] = this.txtNode.Text.Trim();
                                myDataView[0]["HELP_LEVEL3_NO"] = intReturnLevelNo;
                                
                            }
                            else
                            {
                                myDataView[0]["HELP_LEVEL4_DESC"] = this.txtNode.Text.Trim();
                                myDataView[0]["HELP_LEVEL4_NO"] = intReturnLevelNo;
                            }
                        }
                    }
                    else
                    {
                        //Insert
                        strBuildKey = "HELP_LEVEL1_NO = " + pvtintLevel1No;
                       
                        if (pvtCurrentLevelNo == 2)
                        {
                            strBuildKey += " AND HELP_LEVEL2_NO > 0";
                        }
                        else
                        {
                            strBuildKey += " AND HELP_LEVEL2_NO = " + pvtintLevel2No;
    
                            if (pvtCurrentLevelNo == 3)
                            {
                                strBuildKey += " AND HELP_LEVEL3_NO > 0";
                            }
                            else
                            {
                                strBuildKey += " AND HELP_LEVEL3_NO = " + pvtintLevel3No;
                                strBuildKey += " AND HELP_LEVEL4_NO > 0";
                            }
                        }

                        myDataView = null;
                        myDataView = new DataView(pvtDataSet.Tables["HelpLevelDesc"],
                        strBuildKey,
                        "",
                        DataViewRowState.CurrentRows);

                        myNewRow = this.pvtDataSet.Tables["HelpLevelDesc"].NewRow();

                        this.pvtDataSet.Tables["HelpLevelDesc"].BeginInit();

                        myNewRow["HELP_LEVEL1_NO"] = Convert.ToInt32(myDataView[0]["HELP_LEVEL1_NO"]);
                        myNewRow["HELP_LEVEL1_DESC"] = myDataView[0]["HELP_LEVEL1_DESC"].ToString();

                        if (pvtCurrentLevelNo == 2)
                        {
                            myNewRow["HELP_LEVEL2_NO"] = intReturnLevelNo;
                            myNewRow["HELP_LEVEL2_DESC"] = this.txtNode.Text.Trim();

                            myNewRow["HELP_LEVEL3_NO"] = 0;
                            myNewRow["HELP_LEVEL4_NO"] = 0;
                        }
                        else
                        {
                            myNewRow["HELP_LEVEL2_NO"] = Convert.ToInt32(myDataView[0]["HELP_LEVEL2_NO"]);
                            myNewRow["HELP_LEVEL2_DESC"] = myDataView[0]["HELP_LEVEL2_DESC"].ToString();

                            if (pvtCurrentLevelNo == 3)
                            {
                                myNewRow["HELP_LEVEL3_NO"] = intReturnLevelNo;
                                myNewRow["HELP_LEVEL3_DESC"] = this.txtNode.Text.Trim();

                                myNewRow["HELP_LEVEL4_NO"] = 0;
                            }
                            else
                            {
                                myNewRow["HELP_LEVEL3_NO"] = Convert.ToInt32(myDataView[0]["HELP_LEVEL3_NO"]);
                                myNewRow["HELP_LEVEL3_DESC"] = myDataView[0]["HELP_LEVEL3_DESC"].ToString();

                                myNewRow["HELP_LEVEL4_NO"] = intReturnLevelNo;
                                myNewRow["HELP_LEVEL4_DESC"] = this.txtNode.Text.Trim();

                            }
                        }

                        this.pvtDataSet.Tables["HelpLevelDesc"].EndInit();

                        this.pvtDataSet.Tables["HelpLevelDesc"].Rows.Add(myNewRow);
                    }

                    TreeNode ndeParent = (TreeNode)this.tvwTreeViewContent.SelectedNode;

                    ndeNode.Tag = ndeParent.Tag + intReturnLevelNo.ToString("00000");

                    ndeParent.Nodes.Add(ndeNode);
                }
            }
            else
            {
                //string strBuildKey = "HELP_LEVEL1_NO = " + pvtintLevel1No;

                //if (pvtCurrentLevelNo == 1)
                //{
                //    strBuildKey += " AND HELP_LEVEL2_NO = 0";
                //}
                //else
                //{
                //    strBuildKey += " AND HELP_LEVEL2_NO = " + pvtintLevel2No;

                //    if (pvtCurrentLevelNo == 2)
                //    {
                //        strBuildKey += " AND HELP_LEVEL3_NO = 0";
                //    }
                //    else
                //    {
                //        strBuildKey += " AND HELP_LEVEL3_NO = " + pvtintLevel3No;

                //        if (pvtCurrentLevelNo == 3)
                //        {
                //            strBuildKey += " AND HELP_LEVEL4_NO = 0";

                //        }
                //        else
                //        {
                //            strBuildKey += " AND HELP_LEVEL4_NO = " + pvtintLevel4No;
                //        }
                //    }
                //}

                //pvtDataViewHelpLevelText = null;
                //pvtDataViewHelpLevelText = new DataView(pvtDataSet.Tables["HelpLevelText"],
                //strBuildKey,
                //        "",
                //        DataViewRowState.CurrentRows);

                //if (pvtDataViewHelpLevelText.Count == 0)
                //{
                //    DataRow myNewRow = this.pvtDataSet.Tables["HelpLevelText"].NewRow();

                //    myNewRow["HELP_LEVEL1_NO"] = pvtintLevel1No;
                //    myNewRow["HELP_LEVEL2_NO"] = pvtintLevel2No;
                //    myNewRow["HELP_LEVEL3_NO"] = pvtintLevel3No;
                //    myNewRow["HELP_LEVEL4_NO"] = pvtintLevel4No;
                //    myNewRow["HELP_TEXT"] = this.txtDetail.Text.Trim();

                //    this.pvtDataSet.Tables["HelpLevelText"].Rows.Add(myNewRow);
                //}
                //else
                //{
                //    pvtDataViewHelpLevelText[0]["HELP_TEXT"] = this.txtDetail.Text.Trim();
                //}
            }

            if (this.txtFileName.Text != "")
            {
                if (this.pvtblnNew == true)
                {
                    switch (pvtCurrentLevelNo)
                    {
                        case 1:

                            pvtintLevel1No = intReturnLevelNo;

                            break;

                        case 2:

                            pvtintLevel2No = intReturnLevelNo;

                            break;

                        case 3:

                            pvtintLevel3No = intReturnLevelNo;

                            break;

                        case 4:

                            pvtintLevel4No = intReturnLevelNo;

                            break;
                    }
                }

                if (this.pvtblnNew == false)
                {
                    for (int intRow = 0; intRow < pvtDataViewHelpLevelGraphicChunk.Count; intRow++)
                    {
                        pvtDataViewHelpLevelGraphicChunk[intRow].Delete();

                        intRow -= 1;
                    }
                }

                //Png
                string strFileType = "P";
                FileInfo fiFileInfo = new FileInfo(this.txtFileName.Text);

                string strFileName = this.txtFileName.Text.Substring(this.txtFileName.Text.LastIndexOf("\\") + 1);

                int intFileSize = Convert.ToInt32(fiFileInfo.Length);

                pvtbytes = null;
                pvtbytes = new byte[pvtintNumberOfBytesToRead];

                pvtfsFileStream = new FileStream(this.txtFileName.Text, FileMode.Open, FileAccess.ReadWrite);

                //This is The File Length
                int intNumberBlocks = Convert.ToInt32(pvtfsFileStream.Length / pvtintNumberOfBytesToRead);
                int intNumberBytesAlreadyRead = 0;
                int intNumberBytesRead = 0;

                if (intNumberBlocks * pvtintNumberOfBytesToRead != pvtfsFileStream.Length)
                {
                    intNumberBlocks += 1;
                }

                this.prgProgressBar.Maximum = intNumberBlocks;
                this.prgProgressBar.Minimum = 0;
                this.prgProgressBar.Value = 0;

                //DataRowView DataRowView;
                //DataRow myNewRow;

                for (int intBlockNumber = 1; intBlockNumber <= intNumberBlocks; intBlockNumber++)
                {
                    if (intBlockNumber == intNumberBlocks)
                    {
                        pvtintNumberOfBytesToRead = Convert.ToInt32(pvtfsFileStream.Length - intNumberBytesAlreadyRead);

                        pvtbytes = null;
                        pvtbytes = new byte[pvtintNumberOfBytesToRead];
                    }

                    try
                    {
                        pvtfsFileStream.Position = intNumberBytesAlreadyRead;
                        intNumberBytesRead = pvtfsFileStream.Read(pvtbytes, 0, pvtintNumberOfBytesToRead);
                        pvtfsFileStream.Flush();
                    }
                    catch (Exception EX)
                    {
                        string A = "";
                    }

                    objParm = null;
                    objParm = new object[9];
                    objParm[0] = strFileName;
                    objParm[1] = strFileType;
                    objParm[2] = this.pvtintLevel1No;
                    objParm[3] = this.pvtintLevel2No;
                    objParm[4] = this.pvtintLevel3No;
                    objParm[5] = this.pvtintLevel4No;
                    objParm[6] = intBlockNumber;
                    objParm[7] = pvtbytes;
                    objParm[8] = intFileSize;
                  
                    clsISUtilities.DynamicFunction("Insert_File_Graphic_Chunk", objParm);

                    intNumberBytesAlreadyRead += intNumberBytesRead;

                    this.prgProgressBar.Value += 1;
                }

                pvtfsFileStream.Close();
            }

            Delete_Text_Image_For_Main_Node(pvtintLevel1No);

            this.pvtDataSet.AcceptChanges();

            this.btnNew.Enabled = true;
            this.btnUpdate.Enabled = true;

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.txtDetail.Enabled = false;

            this.rbnLevel1.Enabled = false;
            this.rbnLinkNode.Enabled = false;

            this.tvwTreeViewContent.Enabled = true;

            this.txtNode.Enabled = false;

            this.btnFile.Enabled = false;
            this.btnClear.Enabled = false;
            this.btnDelImage.Enabled = false;

            this.txtNode.Text = "";
            this.txtDetail.Text = "";
            this.txtFileName.Text = "";
        }

        private void Delete_Text_Image_For_Main_Node(int intLevel1No)
        {
            DataView myDataView = new DataView(pvtDataSet.Tables["HelpLevelText"],
            "HELP_LEVEL1_NO = " + intLevel1No ,
                    "",
                    DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < myDataView.Count; intRow++)
            {
                myDataView[intRow].Delete();

                intRow -= 1;
            }

            myDataView = null;
            myDataView = new DataView(pvtDataSet.Tables["HelpLevelNumberChunks"],
            "HELP_LEVEL1_NO = " + intLevel1No,
                    "",
                    DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < myDataView.Count; intRow++)
            {
                myDataView[intRow].Delete();

                intRow -= 1;
            }

            myDataView = null;
            myDataView = new DataView(pvtDataSet.Tables["HelpLevelGraphicChunk"],
            "HELP_LEVEL1_NO = " + intLevel1No,
                    "",
                    DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < myDataView.Count; intRow++)
            {
                myDataView[intRow].Delete();

                intRow -= 1;
            }
            
            pvtDataSet.AcceptChanges();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.btnNew.Enabled = true;

            if (this.tvwTreeViewContent.SelectedNode != null)
            {
                this.btnUpdate.Enabled = true;
            }

            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.txtDetail.Enabled = false;

            this.rbnLevel1.Enabled = false;
            this.rbnLinkNode.Enabled = false;

            this.btnFile.Enabled = false;
            this.btnClear.Enabled = false;
            this.btnDelImage.Enabled = false;

            this.txtNode.Text = "";
            this.txtDetail.Text = "";
            this.txtFileName.Text = "";

            this.rbnLevel1.Checked = false;
            this.rbnLinkNode.Checked = false;

            this.tvwTreeViewContent.Enabled = true;

            this.txtNode.Enabled = false;

            this.grbEdit.Text = "Edit";

            TreeViewEventArgs tvwArgs = new TreeViewEventArgs(null);
            tvwTreeViewContent_AfterSelect(sender, tvwArgs);
        }

        private void radiobutton_Level_Click(object sender, EventArgs e)
        {
            RadioButton myRadioButton = (RadioButton)sender;

            if (myRadioButton.Name == "rbnLevel1")
            {
                pvtCurrentLevelNo = 1;

                this.pvtintLevel1No = 0;
                this.pvtintLevel2No = 0;
                this.pvtintLevel3No = 0;
                this.pvtintLevel4No = 0;
            }
            else
            {
                if (this.tvwTreeViewContent.SelectedNode != null)
                {
                    //Set Levels According To TreeItem
                    this.tvwTreeViewContent_AfterSelect(null, null);

                    this.txtNode.Text = "";
                    this.txtDetail.Text = "";

                    //Adds To Current Level
                    pvtCurrentLevelNo += 1;
                }
                else
                {
                    MessageBox.Show("You have to Select a Node First.", "Node", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.rbnLinkNode.Checked = false;
                }
            }
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "png Files|*.png";

            ofd.ShowDialog();

            this.txtFileName.Text = ofd.FileName;

            if (this.txtFileName.Text != "")
            {
                this.btnClear.Enabled = true;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult myResult = MessageBox.Show("Are you Sure you want to delete this Node and ALL LINKED NODES?", "Delete Node", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (myResult == DialogResult.Yes)
            {
                object[] objParm = new object[8];
                objParm[0] = "D";

                objParm[1] = this.pvtCurrentLevelNo;

                objParm[2] = this.pvtintLevel1No;
                objParm[3] = this.pvtintLevel2No;
                objParm[4] = this.pvtintLevel3No;
                objParm[5] = this.pvtintLevel4No;
                objParm[6] = "";
                objParm[7] = "";

                clsISUtilities.DynamicFunction("Update_Structure", objParm);

                string strBuildKey = "HELP_LEVEL1_NO = " + pvtintLevel1No;

                if (pvtCurrentLevelNo > 2)
                {
                    strBuildKey += " AND HELP_LEVEL2_NO = " + pvtintLevel2No;

                    if (pvtCurrentLevelNo > 3)
                    {
                        strBuildKey += " AND HELP_LEVEL3_NO = " + pvtintLevel3No;
                    }
                }

                DataView myDataView = new DataView(pvtDataSet.Tables["HelpLevelDesc"],
                    strBuildKey,
                    "",
                    DataViewRowState.CurrentRows);

                if (myDataView.Count == 1)
                {
                    //Update
                    if (pvtCurrentLevelNo == 2)
                    {
                        myDataView[0]["HELP_LEVEL2_DESC"] = System.DBNull.Value;
                        myDataView[0]["HELP_LEVEL2_NO"] = 0;

                    }
                    else
                    {
                        if (pvtCurrentLevelNo == 3)
                        {
                            myDataView[0]["HELP_LEVEL3_DESC"] = System.DBNull.Value;
                            myDataView[0]["HELP_LEVEL3_NO"] = 0;
                        }
                        else
                        {
                            myDataView[0]["HELP_LEVEL4_DESC"] = System.DBNull.Value;
                            myDataView[0]["HELP_LEVEL4_NO"] = 0;
                        }
                    }
                }
                else
                {
                    if (pvtCurrentLevelNo == 2)
                    {
                        strBuildKey += " AND HELP_LEVEL2_NO = " + pvtintLevel2No;
                    }
                    else
                    {

                        if (pvtCurrentLevelNo == 3)
                        {
                            strBuildKey += " AND HELP_LEVEL3_NO = " + pvtintLevel3No;
                        }
                        else
                        {
                            if (pvtCurrentLevelNo == 4)
                            {
                                strBuildKey += " AND HELP_LEVEL4_NO = " + pvtintLevel4No;
                            }
                        }
                    }

                    myDataView = new DataView(pvtDataSet.Tables["HelpLevelDesc"],
                    strBuildKey,
                    "",
                    DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < myDataView.Count; intRow++)
                    {
                        myDataView[intRow].Delete();

                        intRow -= 1;
                    }
                }
           
                this.Delete_Text_Image_For_Main_Node(pvtintLevel1No);

                //NB This must be Here otherwise pvtintLevel1No changes due to New Selected Node
                //NB This must be Here otherwise pvtintLevel1No changes due to New Selected Node
                TreeNode item = (TreeNode)this.tvwTreeViewContent.SelectedNode;

                if (item.Tag.ToString().Length == 5)
                {
                    tvwTreeViewContent.Nodes.Remove(item);
                }
                else
                {
                    TreeNode itemParent = (TreeNode)item.Parent;
                    itemParent.Nodes.Remove(item);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtFileName.Text = "";
            this.btnClear.Enabled = false;
        }

        private void btnDelNodeFile_Click(object sender, EventArgs e)
        {
            if (this.tvwTreeViewContent.SelectedNode != null)
            {
                DialogResult myResult = MessageBox.Show("Are you Sure you want to delete Image LINKED to this Node?", "Delete Image", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (myResult == DialogResult.Yes)
                {
                    object[] objParm = new object[4];

                    objParm[0] = this.pvtintLevel1No;
                    objParm[1] = this.pvtintLevel2No;
                    objParm[2] = this.pvtintLevel3No;
                    objParm[3] = this.pvtintLevel4No;

                    clsISUtilities.DynamicFunction("Delete_Node_Image", objParm);

                    pvtDataViewHelpLevelGraphicChunk = null;
                    pvtDataViewHelpLevelGraphicChunk = new DataView(pvtDataSet.Tables["HelpLevelGraphicChunk"],
                    "HELP_LEVEL1_NO = " + pvtintLevel1No + " AND HELP_LEVEL2_NO = " + pvtintLevel2No + " AND HELP_LEVEL3_NO = " + pvtintLevel3No + " AND HELP_LEVEL4_NO = " + pvtintLevel4No,
                    "",
                    DataViewRowState.CurrentRows);

                    for (int intRow = 0; intRow < pvtDataViewHelpLevelGraphicChunk.Count; intRow++)
                    {
                        pvtDataViewHelpLevelGraphicChunk[intRow].Delete();

                        intRow -= 1;
                    }

                    pvtDataViewHelpLevelGraphicChunk = null;
                    pvtDataViewHelpLevelGraphicChunk = new DataView(pvtDataSet.Tables["HelpLevelNumberChunks"],
                    "HELP_LEVEL1_NO = " + pvtintLevel1No + " AND HELP_LEVEL2_NO = " + pvtintLevel2No + " AND HELP_LEVEL3_NO = " + pvtintLevel3No + " AND HELP_LEVEL4_NO = " + pvtintLevel4No,
                    "",
                    DataViewRowState.CurrentRows);

                    pvtDataViewHelpLevelGraphicChunk[0].Delete();

                    pvtDataSet.AcceptChanges();

                    this.tvwTreeViewContent_AfterSelect(null, null);
                }
            }
        }

        private void frmHelp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 112)
            {
                int intHeightDifference = this.grbEdit.Height + 4;
#if(DEBUG)
                if (this.grbEdit.Visible == true)
                {
                    this.grbEdit.Visible = false;

                    this.tabControl.Height += intHeightDifference;
                    this.tvwTreeViewContent.Height += intHeightDifference;
                    this.tvwSearchTreeView.Height += intHeightDifference;
                    this.reportViewer.Height += intHeightDifference;
                }
                else
                {
                    this.grbEdit.Visible = true;

                    this.tabControl.Height -= intHeightDifference;
                    this.tvwTreeViewContent.Height -= intHeightDifference;
                    this.tvwSearchTreeView.Height -= intHeightDifference;
                    this.reportViewer.Height -= intHeightDifference;

                }
#else
                try
                {
                    if (AppDomain.CurrentDomain.GetData("UserNo").ToString() == "0")
                    {
                        if (this.grbEdit.Visible == true)
                        {
                            this.grbEdit.Visible = false;

                            this.tabControl.Height += intHeightDifference;
                            this.tvwTreeViewContent.Height += intHeightDifference;
                            this.tvwSearchTreeView.Height += intHeightDifference;
                            this.reportViewer.Height += intHeightDifference;
                        }
                        else
                        {
                            this.grbEdit.Visible = true;

                            this.tabControl.Height -= intHeightDifference;
                            this.tvwTreeViewContent.Height -= intHeightDifference;
                            this.tvwSearchTreeView.Height -= intHeightDifference;
                            this.reportViewer.Height -= intHeightDifference;
                        }
                    }
                }
                catch
                {
                }

#endif
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl.SelectedIndex == 0)
            {
                if (this.tvwTreeViewContent.SelectedNode != null)
                {
                    tvwTreeViewContent_AfterSelect(null, null);
                }
               
                this.tvwTreeViewContent.Focus();
            }
            else
            {
                if (this.tvwSearchTreeView.SelectedNode != null)
                {
                    tvwSearchTreeView_AfterSelect(null, null);
                }

                this.tvwSearchTreeView.Focus();
            }
        }

        private void tvwSearchTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.tvwSearchTreeView.SelectedNode != null)
            {
                pvtDataSet.Tables["Report"].Rows.Clear();

                string strLevel1Desc = "";
                string strLevel2Desc = "";
                string strLevel3Desc = "";
                string strLevel4Desc = "";

                TreeNode tv = (TreeNode)this.tvwSearchTreeView.SelectedNode;

                string strMainKey = tv.Tag.ToString().Substring(0, 5);

                DataView myDataView = new DataView(pvtDataSet.Tables["HelpLevelText"],
                    "HELP_LEVEL1_NO = " + strMainKey,
                    "",
                    DataViewRowState.CurrentRows);

                if (myDataView.Count == 0)
                {
                    //During Editing their might be No Text but Images
                    this.Delete_Text_Image_For_Main_Node(Convert.ToInt32(strMainKey));

                    object[] objParm = new object[1];

                    objParm[0] = Convert.ToInt32(strMainKey);

                    byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Main_Node_Text_Records", objParm);

                    DataSet TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);
                    pvtDataSet.Merge(TempDataSet);

                    this.prgProgressBar.Value = 1;
                    int intProgressBarValue = 1;

                    DataView myDataViewHelpLevelNumberChunks = new DataView(pvtDataSet.Tables["HelpLevelNumberChunks"],
                    "HELP_LEVEL1_NO = " + strMainKey,
                    "",
                    DataViewRowState.CurrentRows);

                    for (int intChunkRow = 0; intChunkRow < myDataViewHelpLevelNumberChunks.Count; intChunkRow++)
                    {
                        intProgressBarValue += Convert.ToInt32(myDataViewHelpLevelNumberChunks[intChunkRow]["HELP_GRAPHIC_NUMBER_OF_CHUNKS"]);
                    }

                    this.prgProgressBar.Value = 0;
                    this.prgProgressBar.Minimum = 0;
                    this.prgProgressBar.Maximum = intProgressBarValue;

                    this.prgProgressBar.Visible = true;

                    for (int intChunkRow = 0; intChunkRow < myDataViewHelpLevelNumberChunks.Count; intChunkRow++)
                    {
                        objParm = new object[5];

                        objParm[0] = Convert.ToInt32(myDataViewHelpLevelNumberChunks[intChunkRow]["HELP_LEVEL1_NO"]);
                        objParm[1] = Convert.ToInt32(myDataViewHelpLevelNumberChunks[intChunkRow]["HELP_LEVEL2_NO"]);
                        objParm[2] = Convert.ToInt32(myDataViewHelpLevelNumberChunks[intChunkRow]["HELP_LEVEL3_NO"]);
                        objParm[3] = Convert.ToInt32(myDataViewHelpLevelNumberChunks[intChunkRow]["HELP_LEVEL4_NO"]);

                        for (int intChunkNo = 1; intChunkNo <= Convert.ToInt32(myDataViewHelpLevelNumberChunks[intChunkRow]["HELP_GRAPHIC_NUMBER_OF_CHUNKS"]); intChunkNo++)
                        {
                            objParm[4] = intChunkNo;

                            bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_File_Chunk", objParm);

                            TempDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);
                            pvtDataSet.Merge(TempDataSet);

                            this.prgProgressBar.Value += 1;
#if(DEBUG)
                            DateTime dtWait = DateTime.Now.AddMilliseconds(200);

                            while (dtWait > DateTime.Now)
                            {
                                Application.DoEvents();
                            }
#endif
                        }
                    }

                    this.prgProgressBar.Visible = false;
                }

                //Level1
                string strBuildKey = "HELP_LEVEL1_NO = " + Convert.ToInt32(strMainKey);

                pvtintLevel1No = Convert.ToInt32(strMainKey);
                pvtCurrentLevelNo = 1;

                if (tv.Tag.ToString().Length >= 10)
                {
                    strBuildKey += " AND HELP_LEVEL2_NO = " + Convert.ToInt32(tv.Tag.ToString().Substring(5, 5));
                    pvtintLevel2No = Convert.ToInt32(tv.Tag.ToString().Substring(5, 5));
                    pvtCurrentLevelNo = 2;
                }
                else
                {
                    pvtintLevel2No = 0;
                }

                if (tv.Tag.ToString().Length >= 15)
                {
                    strBuildKey += " AND HELP_LEVEL3_NO = " + Convert.ToInt32(tv.Tag.ToString().Substring(10, 5));
                    pvtintLevel3No = Convert.ToInt32(tv.Tag.ToString().Substring(10, 5));
                    pvtCurrentLevelNo = 3;
                }
                else
                {
                    pvtintLevel3No = 0;
                }

                if (tv.Tag.ToString().Length >= 20)
                {
                    strBuildKey += " AND HELP_LEVEL4_NO = " + Convert.ToInt32(tv.Tag.ToString().Substring(15, 5));
                    pvtintLevel4No = Convert.ToInt32(tv.Tag.ToString().Substring(15, 5));
                    pvtCurrentLevelNo = 4;
                }
                else
                {
                    pvtintLevel4No = 0;
                }

                myDataView = null;
                myDataView = new DataView(pvtDataSet.Tables["HelpLevelDesc"],
                    strBuildKey,
                    "HELP_LEVEL1_DESC,HELP_LEVEL2_DESC,HELP_LEVEL3_DESC,HELP_LEVEL4_DESC",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < myDataView.Count; intRow++)
                {
                    if (myDataView[intRow]["HELP_LEVEL1_DESC"].ToString() != ""
                        & strLevel1Desc != myDataView[intRow]["HELP_LEVEL1_DESC"].ToString())
                    {
                        DataRow drDataRow = pvtDataSet.Tables["Report"].NewRow();
                        pvtDataSet.Tables["Report"].Rows.Add(drDataRow);
                        pvtDataSet.Tables["Report"].Rows[intRow]["LEVEL1_DESC"] = myDataView[intRow]["HELP_LEVEL1_DESC"].ToString();

                        strLevel1Desc = myDataView[intRow]["HELP_LEVEL1_DESC"].ToString();

                        pvtDataViewHelpLevelText = null;
                        pvtDataViewHelpLevelText = new DataView(pvtDataSet.Tables["HelpLevelText"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = 0",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelText.Count > 0)
                        {
                            pvtDataSet.Tables["Report"].Rows[intRow]["LEVEL1_TEXT"] = pvtDataViewHelpLevelText[0]["HELP_TEXT"].ToString();
                        }

                        pvtDataViewHelpLevelGraphicChunk = null;
                        pvtDataViewHelpLevelGraphicChunk = new DataView(pvtDataSet.Tables["HelpLevelGraphicChunk"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = 0",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelGraphicChunk.Count > 0)
                        {
                            pvtbytFileBytes = null;
                            pvtbytFileBytes = new byte[Convert.ToInt32(pvtDataViewHelpLevelGraphicChunk[0]["HELP_IMAGE_SIZE"])];

                            pvtlngDestinationFileStartIndex = 0;

                            for (int intRowNo = 0; intRowNo < pvtDataViewHelpLevelGraphicChunk.Count; intRowNo++)
                            {
                                pvtbytes = (byte[])pvtDataViewHelpLevelGraphicChunk[intRowNo]["HELP_CHUNK_IMAGE"];

                                Array.Copy(pvtbytes, 0, pvtbytFileBytes, pvtlngDestinationFileStartIndex, pvtbytes.Length);
                                pvtlngDestinationFileStartIndex += pvtbytes.Length;
                            }

                            pvtDataSet.Tables["Report"].Rows[intRow]["LEVEL1_IMAGE"] = pvtbytFileBytes;
                        }
                    }

                    if (myDataView[intRow]["HELP_LEVEL2_DESC"].ToString() != ""
                        & strLevel2Desc != myDataView[intRow]["HELP_LEVEL2_DESC"].ToString())
                    {
                        DataRow drDataRow = pvtDataSet.Tables["Report"].NewRow();
                        pvtDataSet.Tables["Report"].Rows.Add(drDataRow);
                        pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_DESC"] = myDataView[intRow]["HELP_LEVEL2_DESC"].ToString();

                        strLevel2Desc = myDataView[intRow]["HELP_LEVEL2_DESC"].ToString();

                        pvtDataViewHelpLevelText = null;
                        pvtDataViewHelpLevelText = new DataView(pvtDataSet.Tables["HelpLevelText"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL2_NO"].ToString()) + " AND HELP_LEVEL3_NO = 0",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelText.Count > 0)
                        {
                            pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_TEXT"] = pvtDataViewHelpLevelText[0]["HELP_TEXT"].ToString();
                        }

                        pvtDataViewHelpLevelGraphicChunk = null;
                        pvtDataViewHelpLevelGraphicChunk = new DataView(pvtDataSet.Tables["HelpLevelGraphicChunk"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL2_NO"].ToString()) + " AND HELP_LEVEL3_NO = 0",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelGraphicChunk.Count > 0)
                        {
                            pvtbytFileBytes = null;
                            pvtbytFileBytes = new byte[Convert.ToInt32(pvtDataViewHelpLevelGraphicChunk[0]["HELP_IMAGE_SIZE"])];

                            pvtlngDestinationFileStartIndex = 0;

                            for (int intRowNo = 0; intRowNo < pvtDataViewHelpLevelGraphicChunk.Count; intRowNo++)
                            {
                                pvtbytes = (byte[])pvtDataViewHelpLevelGraphicChunk[intRowNo]["HELP_CHUNK_IMAGE"];

                                Array.Copy(pvtbytes, 0, pvtbytFileBytes, pvtlngDestinationFileStartIndex, pvtbytes.Length);
                                pvtlngDestinationFileStartIndex += pvtbytes.Length;
                            }

                            pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_IMAGE"] = pvtbytFileBytes;
                        }
                    }

                    if (myDataView[intRow]["HELP_LEVEL3_DESC"].ToString() != ""
                        & strLevel3Desc != myDataView[intRow]["HELP_LEVEL3_DESC"].ToString())
                    {
                        DataRow drDataRow = pvtDataSet.Tables["Report"].NewRow();
                        pvtDataSet.Tables["Report"].Rows.Add(drDataRow);
                        pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_DESC"] = myDataView[intRow]["HELP_LEVEL3_DESC"].ToString();

                        strLevel3Desc = myDataView[intRow]["HELP_LEVEL3_DESC"].ToString();

                        pvtDataViewHelpLevelText = null;
                        pvtDataViewHelpLevelText = new DataView(pvtDataSet.Tables["HelpLevelText"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL2_NO"].ToString()) + " AND HELP_LEVEL3_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL3_NO"].ToString()) + " AND HELP_LEVEL4_NO = 0",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelText.Count > 0)
                        {
                            pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_TEXT"] = pvtDataViewHelpLevelText[0]["HELP_TEXT"].ToString();
                        }

                        pvtDataViewHelpLevelGraphicChunk = null;
                        pvtDataViewHelpLevelGraphicChunk = new DataView(pvtDataSet.Tables["HelpLevelGraphicChunk"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL2_NO"].ToString()) + " AND HELP_LEVEL3_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL3_NO"].ToString()) + " AND HELP_LEVEL4_NO = 0",
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelGraphicChunk.Count > 0)
                        {
                            pvtbytFileBytes = null;
                            pvtbytFileBytes = new byte[Convert.ToInt32(pvtDataViewHelpLevelGraphicChunk[0]["HELP_IMAGE_SIZE"])];

                            pvtlngDestinationFileStartIndex = 0;

                            for (int intRowNo = 0; intRowNo < pvtDataViewHelpLevelGraphicChunk.Count; intRowNo++)
                            {
                                pvtbytes = (byte[])pvtDataViewHelpLevelGraphicChunk[intRowNo]["HELP_CHUNK_IMAGE"];

                                Array.Copy(pvtbytes, 0, pvtbytFileBytes, pvtlngDestinationFileStartIndex, pvtbytes.Length);
                                pvtlngDestinationFileStartIndex += pvtbytes.Length;
                            }

                            pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_IMAGE"] = pvtbytFileBytes;
                        }
                    }

                    if (myDataView[intRow]["HELP_LEVEL4_DESC"].ToString() != "")
                    {
                        DataRow drDataRow = pvtDataSet.Tables["Report"].NewRow();
                        pvtDataSet.Tables["Report"].Rows.Add(drDataRow);
                        pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_DESC"] = myDataView[intRow]["HELP_LEVEL4_DESC"].ToString();

                        strLevel4Desc = myDataView[intRow]["HELP_LEVEL4_DESC"].ToString();

                        pvtDataViewHelpLevelText = null;
                        pvtDataViewHelpLevelText = new DataView(pvtDataSet.Tables["HelpLevelText"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL2_NO"].ToString()) + " AND HELP_LEVEL3_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL3_NO"].ToString()) + " AND HELP_LEVEL4_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL4_NO"].ToString()),
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelText.Count > 0)
                        {
                            pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_TEXT"] = pvtDataViewHelpLevelText[0]["HELP_TEXT"].ToString();
                        }

                        pvtDataViewHelpLevelGraphicChunk = null;
                        pvtDataViewHelpLevelGraphicChunk = new DataView(pvtDataSet.Tables["HelpLevelGraphicChunk"],
                        "HELP_LEVEL1_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL1_NO"].ToString()) + " AND HELP_LEVEL2_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL2_NO"].ToString()) + " AND HELP_LEVEL3_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL3_NO"].ToString()) + " AND HELP_LEVEL4_NO = " + Convert.ToInt32(myDataView[intRow]["HELP_LEVEL4_NO"].ToString()),
                        "",
                        DataViewRowState.CurrentRows);

                        if (pvtDataViewHelpLevelGraphicChunk.Count > 0)
                        {
                            pvtbytFileBytes = null;
                            pvtbytFileBytes = new byte[Convert.ToInt32(pvtDataViewHelpLevelGraphicChunk[0]["HELP_IMAGE_SIZE"])];

                            pvtlngDestinationFileStartIndex = 0;

                            for (int intRowNo = 0; intRowNo < pvtDataViewHelpLevelGraphicChunk.Count; intRowNo++)
                            {
                                pvtbytes = (byte[])pvtDataViewHelpLevelGraphicChunk[intRowNo]["HELP_CHUNK_IMAGE"];

                                Array.Copy(pvtbytes, 0, pvtbytFileBytes, pvtlngDestinationFileStartIndex, pvtbytes.Length);
                                pvtlngDestinationFileStartIndex += pvtbytes.Length;
                            }

                            pvtDataSet.Tables["Report"].Rows[pvtDataSet.Tables["Report"].Rows.Count - 1]["LEVEL1_IMAGE"] = pvtbytFileBytes;
                        }
                    }
                }

                this.reportViewer.RefreshReport();
            }
        }

        private void frmHelp_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            this.Hide();
        }
    }
}
