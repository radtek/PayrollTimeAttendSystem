using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using InteractPayroll;

namespace InteractPayroll
{
    public class busPayrollHelp
    {
       clsDBConnectionObjects clsDBConnectionObjects;

        public busPayrollHelp()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records()
        {
            string strQry = "";
            DataSet DataSet = new DataSet();

            //Get All Level Descriptions
            strQry = "";
            strQry += " SELECT ";
            strQry += " HF.HELP_LEVEL1_NO";
            strQry += ",HF.HELP_LEVEL2_NO";
            strQry += ",HF.HELP_LEVEL3_NO";
            strQry += ",HF.HELP_LEVEL4_NO";
            strQry += ",HL1.HELP_LEVEL1_DESC";
            strQry += ",HL2.HELP_LEVEL2_DESC";
            strQry += ",HL3.HELP_LEVEL3_DESC";
            strQry += ",HL4.HELP_LEVEL4_DESC";
                        
            strQry += " FROM InteractPayroll.dbo.HELP_FILE HF";

            strQry += " INNER JOIN InteractPayroll.dbo.HELP_LEVEL1 HL1";
            strQry += " ON HF.HELP_LEVEL1_NO = HL1.HELP_LEVEL1_NO";

            strQry += " LEFT JOIN InteractPayroll.dbo.HELP_LEVEL2 HL2";
            strQry += " ON HF.HELP_LEVEL2_NO = HL2.HELP_LEVEL2_NO";

            strQry += " LEFT JOIN InteractPayroll.dbo.HELP_LEVEL3 HL3";
            strQry += " ON HF.HELP_LEVEL3_NO = HL3.HELP_LEVEL3_NO";

            strQry += " LEFT JOIN InteractPayroll.dbo.HELP_LEVEL4 HL4";
            strQry += " ON HF.HELP_LEVEL4_NO = HL4.HELP_LEVEL4_NO";
                                
            strQry += " ORDER BY ";
            strQry += " HL1.HELP_LEVEL1_DESC";
            strQry += ",HL2.HELP_LEVEL2_DESC";
            strQry += ",HL3.HELP_LEVEL3_DESC";
            strQry += ",HL4.HELP_LEVEL4_DESC";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "HelpLevelDesc",-1);
         
            strQry = "";
            strQry += " SELECT ";
            strQry += " HELP_LEVEL1_NO";
            strQry += ",HELP_LEVEL2_NO";
            strQry += ",HELP_LEVEL3_NO";
            strQry += ",HELP_LEVEL4_NO";
            strQry += ",HELP_TEXT";
            
            strQry += " FROM InteractPayroll.dbo.HELP_FILE_TEXT ";
            
            strQry += " WHERE HELP_LEVEL1_NO = -1";
            
            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "HelpLevelText", -1);

            //Empty Table - Used for DataView
            strQry = "";
            strQry += " SELECT ";
            strQry += " HELP_LEVEL1_NO";
            strQry += ",HELP_LEVEL2_NO";
            strQry += ",HELP_LEVEL3_NO";
            strQry += ",HELP_LEVEL4_NO";
            strQry += ",COUNT(*) AS HELP_GRAPHIC_NUMBER_OF_CHUNKS";

            strQry += " FROM InteractPayroll.dbo.HELP_FILE_GRAPHIC_CHUNK ";

            strQry += " WHERE HELP_LEVEL1_NO = -1";

            strQry += " GROUP BY ";
            strQry += " HELP_LEVEL1_NO";
            strQry += ",HELP_LEVEL2_NO";
            strQry += ",HELP_LEVEL3_NO";
            strQry += ",HELP_LEVEL4_NO";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "HelpLevelNumberChunks", -1);

            //Empty Table - Used for DataView
            strQry = "";
            strQry += " SELECT ";
            strQry += " HFGC.HELP_LEVEL1_NO";
            strQry += ",HFGC.HELP_LEVEL2_NO";
            strQry += ",HFGC.HELP_LEVEL3_NO";
            strQry += ",HFGC.HELP_LEVEL4_NO";
            strQry += ",HFGD.HELP_FILE_TYPE";
            strQry += ",HFGD.HELP_IMAGE_SIZE";
            strQry += ",HFGC.HELP_CHUNK_NO";
            strQry += ",HFGC.HELP_CHUNK_IMAGE";

            strQry += " FROM InteractPayroll.dbo.HELP_FILE_GRAPHIC_CHUNK HFGC ";

            strQry += " INNER JOIN InteractPayroll.dbo.HELP_FILE_GRAPHIC_DETAIL HFGD ";
            strQry += " ON HFGC.HELP_LEVEL1_NO = HFGD.HELP_LEVEL1_NO";
            strQry += " AND HFGC.HELP_LEVEL2_NO = HFGD.HELP_LEVEL2_NO";
            strQry += " AND HFGC.HELP_LEVEL3_NO = HFGD.HELP_LEVEL3_NO";
            strQry += " AND HFGC.HELP_LEVEL4_NO = HFGD.HELP_LEVEL4_NO";

            strQry += " WHERE HFGC.HELP_LEVEL1_NO = -1";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "HelpLevelGraphicChunk", -1);

            DataTable DataTable = new DataTable("Report");

            DataTable.Columns.Add("LEVEL1_DESC", typeof(String));
            DataTable.Columns.Add("LEVEL1_TEXT", typeof(String));
            DataTable.Columns.Add("LEVEL1_IMAGE", typeof(byte[]));
                     
            DataSet.Tables.Add(DataTable);

            DataRow drDataRow = DataSet.Tables["Report"].NewRow();

            drDataRow["LEVEL1_DESC"] = "Getting Started";
            drDataRow["LEVEL1_TEXT"] = "Choose either Content or Search Tab.";

            DataSet.Tables["Report"].Rows.Add(drDataRow);

            drDataRow = DataSet.Tables["Report"].NewRow();

            drDataRow["LEVEL1_DESC"] = "Content";
            drDataRow["LEVEL1_TEXT"] = "Click on a Node for desired Information.";

            DataSet.Tables["Report"].Rows.Add(drDataRow);

            drDataRow = DataSet.Tables["Report"].NewRow();

            drDataRow["LEVEL1_DESC"] = "Search";
            drDataRow["LEVEL1_TEXT"] = "Enter search criteria into Search TextBox and Click Search.";

            DataSet.Tables["Report"].Rows.Add(drDataRow);


            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet = null;
            return bytCompress;
        }

        public byte[] Get_Main_Node_Text_Records(int parintLevelNumber)
        {
            string strQry = "";
            DataSet DataSet = new DataSet();

            //Get All Text linked to Main Node
            strQry = "";
            strQry += " SELECT ";
            strQry += " HELP_LEVEL1_NO";
            strQry += ",HELP_LEVEL2_NO";
            strQry += ",HELP_LEVEL3_NO";
            strQry += ",HELP_LEVEL4_NO";
            strQry += ",HELP_TEXT";
            strQry += " FROM InteractPayroll.dbo.HELP_FILE_TEXT ";
            
            strQry += " WHERE HELP_LEVEL1_NO = " + parintLevelNumber;

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "HelpLevelText", -1);

            //Get All Level Descriptions
            strQry = "";
            strQry += " SELECT ";
            strQry += " HELP_LEVEL1_NO";
            strQry += ",HELP_LEVEL2_NO";
            strQry += ",HELP_LEVEL3_NO";
            strQry += ",HELP_LEVEL4_NO";
            strQry += ",COUNT(*) AS HELP_GRAPHIC_NUMBER_OF_CHUNKS";

            strQry += " FROM InteractPayroll.dbo.HELP_FILE_GRAPHIC_CHUNK ";

            strQry += " WHERE HELP_LEVEL1_NO = " + parintLevelNumber;

            strQry += " GROUP BY ";
            strQry += " HELP_LEVEL1_NO";
            strQry += ",HELP_LEVEL2_NO";
            strQry += ",HELP_LEVEL3_NO";
            strQry += ",HELP_LEVEL4_NO";

            strQry += " ORDER BY ";
            strQry += " HELP_LEVEL1_NO";
            strQry += ",HELP_LEVEL2_NO";
            strQry += ",HELP_LEVEL3_NO";
            strQry += ",HELP_LEVEL4_NO";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "HelpLevelNumberChunks", -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet = null;
            return bytCompress;
        }

        public int Update_Structure(string parstrUpdateType, int parintLevelNo, int parintLevel1No, int parintLevel2No, int parintLevel3No, int parintLevel4No, string parstrNodeDesc, string parstrNodeDetailDesc)
        {
            DataSet DataSet = new DataSet();
            string strQry = "";
            int intKey = -1;

            if (parstrUpdateType == "A")
            {
                if (parintLevelNo > 1)
                {
                    //Delete Link Where Node Has No Chilren
                    strQry = "";
                    strQry += " DELETE FROM InteractPayroll.dbo.HELP_FILE";
                    strQry += " WHERE HELP_LEVEL1_NO = " + parintLevel1No;

                    if (parintLevelNo == 2)
                    {
                        strQry += " AND HELP_LEVEL2_NO = 0 ";
                    }
                    else
                    {
                        if (parintLevelNo == 3)
                        {
                            strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No;
                            strQry += " AND HELP_LEVEL3_NO = 0 ";
                        }
                        else
                        {
                            strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No;
                            strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No;
                            strQry += " AND HELP_LEVEL4_NO = 0 ";
                        }
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
                }

                strQry = "";
                strQry += " SELECT ";
                strQry += " MAX(HELP_LEVEL" + parintLevelNo.ToString() + "_NO) AS MAX_NO";
                strQry += " FROM InteractPayroll.dbo.HELP_LEVEL" + parintLevelNo.ToString();

                clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "Temp", -1);

                if (DataSet.Tables["Temp"].Rows[0]["MAX_NO"] == System.DBNull.Value)
                {
                    intKey = 1;
                }
                else
                {
                    intKey = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
                }

                strQry = "";
                strQry += " INSERT INTO InteractPayroll.dbo.HELP_FILE";
                strQry += "(HELP_LEVEL1_NO";
                strQry += ",HELP_LEVEL2_NO";
                strQry += ",HELP_LEVEL3_NO";
                strQry += ",HELP_LEVEL4_NO)";
                strQry += " VALUES ";

                if (parintLevelNo == 1)
                {
                    strQry += "(" + intKey.ToString();
                }
                else
                {
                    strQry += "(" + parintLevel1No.ToString();
                }

                if (parintLevelNo == 2)
                {
                    strQry += "," + intKey.ToString();
                }
                else
                {
                    strQry += "," + parintLevel2No.ToString();
                }

                if (parintLevelNo == 3)
                {
                    strQry += "," + intKey.ToString();
                }
                else
                {
                    strQry += "," + parintLevel3No.ToString();
                }

                if (parintLevelNo == 4)
                {
                    strQry += "," + intKey.ToString() + ")";
                }
                else
                {
                    strQry += "," + parintLevel4No.ToString() + ")";
                }

                clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);

                strQry = "";
                strQry += " INSERT INTO InteractPayroll.dbo.HELP_LEVEL" + parintLevelNo.ToString();
                strQry += "(HELP_LEVEL" + parintLevelNo.ToString() + "_NO";
                strQry += ",HELP_LEVEL" + parintLevelNo.ToString() + "_DESC)";
                strQry += " VALUES ";
                strQry += "(" + intKey.ToString();
                strQry += "," + clsDBConnectionObjects.Text2DynamicSQL(parstrNodeDesc) + ")";

                clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);

                if (parstrNodeDetailDesc != "")
                {
                    strQry = "";
                    strQry += " INSERT INTO InteractPayroll.dbo.HELP_FILE_TEXT";
                    strQry += "(HELP_LEVEL1_NO";
                    strQry += ",HELP_LEVEL2_NO";
                    strQry += ",HELP_LEVEL3_NO";
                    strQry += ",HELP_LEVEL4_NO";
                    strQry += ",HELP_TEXT)";

                    strQry += " VALUES ";

                    if (parintLevelNo == 1)
                    {
                        strQry += "(" + intKey.ToString();
                    }
                    else
                    {
                        strQry += "(" + parintLevel1No.ToString();
                    }

                    if (parintLevelNo == 2)
                    {
                        strQry += "," + intKey.ToString();
                    }
                    else
                    {
                        strQry += "," + parintLevel2No.ToString();
                    }

                    if (parintLevelNo == 3)
                    {
                        strQry += "," + intKey.ToString();
                    }
                    else
                    {
                        strQry += "," + parintLevel3No.ToString();
                    }

                    if (parintLevelNo == 4)
                    {
                        strQry += "," + intKey.ToString();
                    }
                    else
                    {
                        strQry += "," + parintLevel4No.ToString();
                    }

                    strQry += "," + clsDBConnectionObjects.Text2DynamicSQL(parstrNodeDetailDesc) + ")";

                    clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
                }
            }
            else
            {
                if (parstrUpdateType == "U")
                {
                    strQry = "";
                    strQry += " UPDATE InteractPayroll.dbo.HELP_LEVEL" + parintLevelNo.ToString();
                    strQry += " SET HELP_LEVEL" + parintLevelNo.ToString() + "_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parstrNodeDesc);

                    strQry += " WHERE HELP_LEVEL" + parintLevelNo.ToString() + "_NO = ";

                    if (parintLevelNo == 1)
                    {
                        strQry += parintLevel1No;
                    }
                    else
                    {
                        if (parintLevelNo == 2)
                        {
                            strQry += parintLevel2No;
                        }
                        else
                        {
                            if (parintLevelNo == 3)
                            {
                                strQry += parintLevel3No;
                            }
                            else
                            {
                                strQry += parintLevel3No;
                            }

                        }
                    }
                    
                    clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);

                    strQry = "";
                    strQry += " DELETE FROM InteractPayroll.dbo.HELP_FILE_TEXT";
                    
                    strQry += " WHERE HELP_LEVEL1_NO = " + parintLevel1No;
                    strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No;
                    strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No;
                    strQry += " AND HELP_LEVEL4_NO = " + parintLevel4No;

                    clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);

                    if (parstrNodeDetailDesc != "")
                    {
                        strQry = "";
                        strQry += " INSERT INTO InteractPayroll.dbo.HELP_FILE_TEXT";
                        strQry += "(HELP_LEVEL1_NO";
                        strQry += ",HELP_LEVEL2_NO";
                        strQry += ",HELP_LEVEL3_NO";
                        strQry += ",HELP_LEVEL4_NO";
                        strQry += ",HELP_TEXT)";

                        strQry += " VALUES ";

                        strQry += "(" + parintLevel1No.ToString();
                        strQry += "," + parintLevel2No.ToString();
                        strQry += "," + parintLevel3No.ToString();
                        strQry += "," + parintLevel4No.ToString();
                        
                        strQry += "," + clsDBConnectionObjects.Text2DynamicSQL(parstrNodeDetailDesc) + ")";

                        clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
                    }
                }
                else
                {
                    strQry = "";
                    strQry += " SELECT ";
                    strQry += " HELP_LEVEL1_NO";
                    strQry += ",HELP_LEVEL2_NO";
                    strQry += ",HELP_LEVEL3_NO";
                    strQry += ",HELP_LEVEL4_NO";
                    strQry += " FROM InteractPayroll.dbo.HELP_FILE";

                    strQry += " WHERE HELP_LEVEL1_NO = " + parintLevel1No;
                   
                    if (parintLevelNo >= 2)
                    {
                        strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                    }

                    if (parintLevelNo >= 3)
                    {
                        strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No.ToString();
                    }

                    if (parintLevelNo >= 4)
                    {
                        strQry += " AND HELP_LEVEL4_NO = " + parintLevel4No.ToString();
                    }

                    clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "Temp", -1);

                    for (int intRow = 0; intRow < DataSet.Tables["Temp"].Rows.Count; intRow++)
                    {
                        if (intRow == 0
                            & parintLevelNo > 1)
                        {
                            //Go Up A level To see if There Is Only 1 Record - Then Update
                            strQry = "";
                            strQry += " SELECT ";
                            strQry += " HELP_LEVEL1_NO";
                            strQry += " FROM InteractPayroll.dbo.HELP_FILE";

                            strQry += " WHERE HELP_LEVEL1_NO = " + parintLevel1No;

                            if (parintLevelNo == 3)
                            {
                                strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                            }
                            else
                            {

                                if (parintLevelNo == 4)
                                {
                                    strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                                    strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No.ToString();
                                }
                            }

                            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "Test", -1);

                            if (DataSet.Tables["Test"].Rows.Count == 1)
                            {
                                strQry = " UPDATE InteractPayroll.dbo.HELP_FILE";
                                strQry += " SET ";

                                if (parintLevelNo == 4)
                                {
                                    strQry += " HELP_LEVEL4_NO = 0 ";
                                }
                                else
                                {
                                    if (parintLevelNo == 3)
                                    {
                                        strQry += " HELP_LEVEL3_NO = 0 ";
                                        strQry += ",HELP_LEVEL4_NO = 0 ";
                                    }
                                    else
                                    {
                                        if (parintLevelNo == 2)
                                        {
                                            strQry += " HELP_LEVEL2_NO = 0 ";
                                            strQry += ",HELP_LEVEL3_NO = 0 ";
                                            strQry += ",HELP_LEVEL4_NO = 0 ";
                                        }
                                    }
                                }

                                strQry += " WHERE HELP_LEVEL1_NO = " + DataSet.Tables["Temp"].Rows[intRow]["HELP_LEVEL1_NO"].ToString();
                                strQry += " AND HELP_LEVEL2_NO = " + DataSet.Tables["Temp"].Rows[intRow]["HELP_LEVEL2_NO"].ToString();
                                strQry += " AND HELP_LEVEL3_NO = " + DataSet.Tables["Temp"].Rows[intRow]["HELP_LEVEL3_NO"].ToString();
                                strQry += " AND HELP_LEVEL3_NO = " + DataSet.Tables["Temp"].Rows[intRow]["HELP_LEVEL4_NO"].ToString();

                                clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
                            }
                        }

                        if (parintLevelNo == 1)
                        {
                            strQry = "";
                            strQry += " DELETE FROM InteractPayroll.dbo.HELP_LEVEL1";
                            strQry += " WHERE HELP_LEVEL1_NO = " + DataSet.Tables["Temp"].Rows[intRow]["HELP_LEVEL1_NO"].ToString();

                            clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
                        }

                        if (parintLevelNo <= 2)
                        {
                            strQry = "";
                            strQry += " DELETE FROM InteractPayroll.dbo.HELP_LEVEL2";
                            strQry += " WHERE HELP_LEVEL2_NO = " + DataSet.Tables["Temp"].Rows[intRow]["HELP_LEVEL2_NO"].ToString();

                            clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
                        }

                        if (parintLevelNo <= 3)
                        {
                            strQry = "";
                            strQry += " DELETE FROM InteractPayroll.dbo.HELP_LEVEL3";
                            strQry += " WHERE HELP_LEVEL3_NO = " + DataSet.Tables["Temp"].Rows[intRow]["HELP_LEVEL3_NO"].ToString();

                            clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
                        }

                        if (parintLevelNo <= 4)
                        {
                            strQry = "";
                            strQry += " DELETE FROM InteractPayroll.dbo.HELP_LEVEL4";
                            strQry += " WHERE HELP_LEVEL4_NO = " + DataSet.Tables["Temp"].Rows[intRow]["HELP_LEVEL4_NO"].ToString();

                            clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
                        }
                    }

                    strQry = "";
                    strQry += " DELETE FROM InteractPayroll.dbo.HELP_FILE";
                    strQry += " WHERE HELP_LEVEL1_NO = " + parintLevel1No.ToString();

                    if (parintLevelNo == 2)
                    {
                        strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                    }
                    else
                    {
                        if (parintLevelNo == 3)
                        {
                            strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                            strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No.ToString();
                        }
                        else
                        {
                            if (parintLevelNo == 4)
                            {
                                strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                                strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No.ToString();
                                strQry += " AND HELP_LEVEL4_NO = " + parintLevel4No.ToString();
                            }
                        }
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
                 
                    strQry = "";
                    strQry += " DELETE FROM InteractPayroll.dbo.HELP_FILE_TEXT";
                    strQry += " WHERE HELP_LEVEL1_NO = " + parintLevel1No.ToString();

                    if (parintLevelNo == 2)
                    {
                        strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                    }
                    else
                    {
                        if (parintLevelNo == 3)
                        {
                            strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                            strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No.ToString();
                        }
                        else
                        {
                            if (parintLevelNo == 4)
                            {
                                strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                                strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No.ToString();
                                strQry += " AND HELP_LEVEL4_NO = " + parintLevel4No.ToString();
                            }
                        }
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);

                    strQry = "";
                    strQry += " DELETE FROM InteractPayroll.dbo.HELP_FILE_GRAPHIC_CHUNK";
                    strQry += " WHERE HELP_LEVEL1_NO = " + parintLevel1No.ToString();

                    if (parintLevelNo == 2)
                    {
                        strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                    }
                    else
                    {
                        if (parintLevelNo == 3)
                        {
                            strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                            strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No.ToString();
                        }
                        else
                        {
                            if (parintLevelNo == 4)
                            {
                                strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                                strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No.ToString();
                                strQry += " AND HELP_LEVEL4_NO = " + parintLevel4No.ToString();
                            }
                        }
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);

                    strQry = "";
                    strQry += " DELETE FROM InteractPayroll.dbo.HELP_FILE_GRAPHIC_DETAIL";
                    strQry += " WHERE HELP_LEVEL1_NO = " + parintLevel1No.ToString();

                    if (parintLevelNo == 2)
                    {
                        strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                    }
                    else
                    {
                        if (parintLevelNo == 3)
                        {
                            strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                            strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No.ToString();
                        }
                        else
                        {
                            if (parintLevelNo == 4)
                            {
                                strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No.ToString();
                                strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No.ToString();
                                strQry += " AND HELP_LEVEL4_NO = " + parintLevel4No.ToString();
                            }
                        }
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
                }
            }

            return intKey;
        }

        public void Delete_Node_Image(int parintLevel1No, int parintLevel2No, int parintLevel3No, int parintLevel4No)
        {
            string strQry = "";

            strQry = "";
            strQry += " DELETE FROM InteractPayroll.dbo.HELP_FILE_GRAPHIC_DETAIL";
            strQry += " WHERE HELP_LEVEL1_NO = " + parintLevel1No;
            strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No;
            strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No;
            strQry += " AND HELP_LEVEL3_NO = " + parintLevel4No;

            clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);

            strQry = "";
            strQry += " DELETE FROM InteractPayroll.dbo.HELP_FILE_GRAPHIC_CHUNK";
            strQry += " WHERE HELP_LEVEL1_NO = " + parintLevel1No;
            strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No;
            strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No;
            strQry += " AND HELP_LEVEL3_NO = " + parintLevel4No;

            clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
        }

        public byte[] Search_For_String(string parstrSearch)
        {
            DataSet DataSet = new System.Data.DataSet();
            string strQry = "";

            strQry += " SELECT ";

            strQry += " HL1.HELP_LEVEL1_DESC";
            strQry += ",HL2.HELP_LEVEL2_DESC";
            strQry += ",HL3.HELP_LEVEL3_DESC";
            strQry += ",HL4.HELP_LEVEL4_DESC";
            strQry += ",HFT.HELP_LEVEL1_NO";
            strQry += ",HFT.HELP_LEVEL2_NO";
            strQry += ",HFT.HELP_LEVEL3_NO";
            strQry += ",HFT.HELP_LEVEL4_NO";
           
            strQry += " FROM InteractPayroll.dbo.HELP_FILE_TEXT HFT";
            
            strQry += " INNER JOIN InteractPayroll.dbo.HELP_LEVEL1 HL1";
            strQry += " ON HFT.HELP_LEVEL1_NO = HL1.HELP_LEVEL1_NO";

            strQry += " LEFT JOIN InteractPayroll.dbo.HELP_LEVEL2 HL2";
            strQry += " ON HFT.HELP_LEVEL2_NO = HL2.HELP_LEVEL2_NO";

            strQry += " LEFT JOIN InteractPayroll.dbo.HELP_LEVEL3 HL3";
            strQry += " ON HFT.HELP_LEVEL3_NO = HL3.HELP_LEVEL3_NO";

            strQry += " LEFT JOIN InteractPayroll.dbo.HELP_LEVEL4 HL4";
            strQry += " ON HFT.HELP_LEVEL4_NO = HL4.HELP_LEVEL4_NO";

            strQry += " WHERE HFT.HELP_TEXT LIKE '%" + parstrSearch.ToLower() + "%'";

            strQry += " GROUP BY ";
            strQry += " HL1.HELP_LEVEL1_DESC";
            strQry += ",HL2.HELP_LEVEL2_DESC";
            strQry += ",HL3.HELP_LEVEL3_DESC";
            strQry += ",HL4.HELP_LEVEL4_DESC";
            strQry += ",HFT.HELP_LEVEL1_NO";
            strQry += ",HFT.HELP_LEVEL2_NO";
            strQry += ",HFT.HELP_LEVEL3_NO";
            strQry += ",HFT.HELP_LEVEL4_NO";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "HelpSearch", -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet = null;
            return bytCompress;
        }

        public void Insert_File_Graphic_Chunk(string parFileName, string parFileType, int parintLevel1No, int parintLevel2No, int parintLevel3No, int parintLevel4No, int parintChunkNo, byte[] parFileBytes, int parintFileSize)
        {
            string strQry = "";

            if (parintChunkNo == 1)
            {
                strQry = "";
                strQry += " DELETE FROM InteractPayroll.dbo.HELP_FILE_GRAPHIC_DETAIL";
                strQry += " WHERE HELP_LEVEL1_NO = " + parintLevel1No;
                strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No;
                strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No;
                strQry += " AND HELP_LEVEL3_NO = " + parintLevel4No;

                clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);

                strQry = "";
                strQry += " DELETE FROM InteractPayroll.dbo.HELP_FILE_GRAPHIC_CHUNK";
                strQry += " WHERE HELP_LEVEL1_NO = " + parintLevel1No;
                strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No;
                strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No;
                strQry += " AND HELP_LEVEL3_NO = " + parintLevel4No;

                clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
         
                strQry = "";
                strQry += " INSERT INTO InteractPayroll.dbo.HELP_FILE_GRAPHIC_DETAIL";
                strQry += " (HELP_LEVEL1_NO";
                strQry += " ,HELP_LEVEL2_NO";
                strQry += " ,HELP_LEVEL3_NO";
                strQry += " ,HELP_LEVEL4_NO";
                strQry += " ,HELP_FILE_NAME";
                strQry += " ,HELP_FILE_TYPE";
                strQry += " ,HELP_IMAGE_SIZE)";
                strQry += "  VALUES";
                strQry += " (" + parintLevel1No;
                strQry += " ," + parintLevel2No;
                strQry += " ," + parintLevel3No;
                strQry += " ," + parintLevel4No;

                strQry += " ," + clsDBConnectionObjects.Text2DynamicSQL(parFileName);
                strQry += " ," + clsDBConnectionObjects.Text2DynamicSQL(parFileType);
                strQry += " ," + parintFileSize + ")";

                clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);
            }

            strQry = "";
            strQry += " INSERT INTO InteractPayroll.dbo.HELP_FILE_GRAPHIC_CHUNK";
            strQry += " (HELP_LEVEL1_NO";
            strQry += " ,HELP_LEVEL2_NO";
            strQry += " ,HELP_LEVEL3_NO";
            strQry += " ,HELP_LEVEL4_NO";
            strQry += " ,HELP_CHUNK_NO)";
            strQry += "  VALUES";
            strQry += " (" + parintLevel1No;
            strQry += " ," + parintLevel2No;
            strQry += " ," + parintLevel3No;
            strQry += " ," + parintLevel4No;
            strQry += " ," + parintChunkNo + ")";

            clsDBConnectionObjects.Execute_SQLCommand(strQry, -1);

            strQry = "";
            strQry += " UPDATE InteractPayroll.dbo.HELP_FILE_GRAPHIC_CHUNK";
            strQry += " SET HELP_CHUNK_IMAGE = @HELP_CHUNK_IMAGE";
            strQry += " WHERE HELP_LEVEL1_NO = " + parintLevel1No;
            strQry += " AND HELP_LEVEL2_NO = " + parintLevel2No;
            strQry += " AND HELP_LEVEL3_NO = " + parintLevel3No;
            strQry += " AND HELP_LEVEL4_NO = " + parintLevel4No;
            strQry += " AND HELP_CHUNK_NO = " + parintChunkNo;

            clsDBConnectionObjects.Execute_SQLCommand(strQry, parFileBytes, "@HELP_CHUNK_IMAGE");
        }

        public byte[] Get_File_Chunk(int parintLevel1No, int parintLevel2No, int parintLevel3No, int parintLevel4No, int parintChunkNo)
        {
            DataSet DataSet = new DataSet();

            string strQry = "";

            //Get All Images Linked to Main Node
            strQry = "";
            strQry += " SELECT ";
            strQry += " HFGC.HELP_LEVEL1_NO";
            strQry += ",HFGC.HELP_LEVEL2_NO";
            strQry += ",HFGC.HELP_LEVEL3_NO";
            strQry += ",HFGC.HELP_LEVEL4_NO";
            strQry += ",HFGD.HELP_FILE_TYPE";
            strQry += ",HFGD.HELP_IMAGE_SIZE";
            strQry += ",HFGC.HELP_CHUNK_NO";
            strQry += ",HFGC.HELP_CHUNK_IMAGE";

            strQry += " FROM InteractPayroll.dbo.HELP_FILE_GRAPHIC_CHUNK HFGC ";

            strQry += " INNER JOIN InteractPayroll.dbo.HELP_FILE_GRAPHIC_DETAIL HFGD ";
            strQry += " ON HFGC.HELP_LEVEL1_NO = HFGD.HELP_LEVEL1_NO";
            strQry += " AND HFGC.HELP_LEVEL2_NO = HFGD.HELP_LEVEL2_NO";
            strQry += " AND HFGC.HELP_LEVEL3_NO = HFGD.HELP_LEVEL3_NO";
            strQry += " AND HFGC.HELP_LEVEL4_NO = HFGD.HELP_LEVEL4_NO";

            strQry += " WHERE HFGC.HELP_LEVEL1_NO = " + parintLevel1No;
            strQry += " AND HFGC.HELP_LEVEL2_NO = " + parintLevel2No;
            strQry += " AND HFGC.HELP_LEVEL3_NO = " + parintLevel3No;
            strQry += " AND HFGC.HELP_LEVEL4_NO = " + parintLevel4No;
            strQry += " AND HFGC.HELP_CHUNK_NO = " + parintChunkNo;

            strQry += " ORDER BY ";
            strQry += " HFGC.HELP_LEVEL1_NO";
            strQry += ",HFGC.HELP_LEVEL2_NO";
            strQry += ",HFGC.HELP_LEVEL3_NO";
            strQry += ",HFGC.HELP_LEVEL4_NO";
            strQry += ",HFGC.HELP_CHUNK_NO";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "HelpLevelGraphicChunk", -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet = null;
            return bytCompress;
        }
    }
}
