using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace InteractPayroll
{
    public class busCompanyPaymentOptions
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busCompanyPaymentOptions()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" INVOICE_LINE_CHOICE_NO");
            strQry.AppendLine(",INVOICE_LINE_CHOICE_DESC");
            strQry.AppendLine(",INVOICE_LINE_CHOICE_DETAIL");
            strQry.AppendLine(",INVOICE_LINE_VALUE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_LINE_CHOICE_DESC ");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" INVOICE_LINE_CHOICE_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ItemChoice", -1);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_DESC");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",ISNULL(WAGE_AMOUNT,0) AS WAGE_AMOUNT ");
            strQry.AppendLine(",ISNULL(SALARY_AMOUNT,0) AS SALARY_AMOUNT ");
            strQry.AppendLine(",ISNULL(TIME_ATTEND_AMOUNT,0) AS TIME_ATTEND_AMOUNT ");
            strQry.AppendLine(",ISNULL(HOURLY_SUPPORT_AMOUNT,0) AS HOURLY_SUPPORT_AMOUNT ");
            strQry.AppendLine(",PERSON_NAMES1");
            strQry.AppendLine(",PHONE1");
            strQry.AppendLine(",EMAIL1");
            strQry.AppendLine(",PERSON_NAMES2");
            strQry.AppendLine(",PHONE2");
            strQry.AppendLine(",EMAIL2");
            strQry.AppendLine(",GENERATE_INVOICE_IND");
            strQry.AppendLine(",COMPANY_FOLDER_NAME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", -1);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" BACKUP_DATABASE_PATH");

            strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Info", -1);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" IHH.INVOICE_NUMBER");
            strQry.AppendLine(",IHH.INVOICE_DATE");
            strQry.AppendLine(",IHH.CONTACT_PERSON");
            strQry.AppendLine(",IHH.INVOICE_TOTAL");
            strQry.AppendLine(",IHH.INVOICE_VAT_TOTAL");
            strQry.AppendLine(",IHH.INVOICE_FINAL_TOTAL");
            strQry.AppendLine(",'I' AS INVOICE_TYPE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY IHH ");

            strQry.AppendLine(" WHERE IHH.COMPANY_NO = " + parint64CompanyNo);

            DirectoryInfo diDirectoryPath = Directory.GetParent(DataSet.Tables["Info"].Rows[0]["BACKUP_DATABASE_PATH"].ToString());

            string strFilePath = diDirectoryPath.FullName;
            string strCompanyPath = parint64CompanyNo.ToString("00000") + "_" + DataSet.Tables["Company"].Rows[0]["COMPANY_FOLDER_NAME"].ToString();
            string strCompanyInvPrefix = strCompanyPath + "_Inv";
            
            strFilePath += "\\InvoicesStatements\\" + strCompanyPath;

            if (Directory.Exists(strFilePath) == true)
            {
                string[] fileCompanyInvoiceFile = Directory.GetFiles(strFilePath, strCompanyPath + "_Inv*.pdf");
                
                if (fileCompanyInvoiceFile.Length > 0)
                {
                    string strInvoiceNumbersIN = "";

                    for (int intFile = 0; intFile < fileCompanyInvoiceFile.Length; intFile++)
                    {
                        string[] fileParts = fileCompanyInvoiceFile[intFile].Split('.');

                        if (fileParts.Length == 2)
                        {
                            int intOffSet = fileParts[0].IndexOf(strCompanyInvPrefix);

                            if (intOffSet > -1)
                            {
                                try
                                {
                                    int intInvNumberOffset = intOffSet + strCompanyInvPrefix.Length;
                                    string strInvoiceNumber = fileParts[0].Substring(intInvNumberOffset);

                                    if (strInvoiceNumbersIN == "")
                                    {
                                        strInvoiceNumbersIN = " AND IHH.INVOICE_NUMBER IN (" + strInvoiceNumber;
                                    }
                                    else
                                    {
                                        strInvoiceNumbersIN += "," + strInvoiceNumber;
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }

                    if (strInvoiceNumbersIN != "")
                    {
                        strQry.AppendLine(strInvoiceNumbersIN + ")");
                    }
                    else
                    {
                        strQry.AppendLine(" AND IHH.INVOICE_NUMBER = 0 ");
                    }
                }
                else
                {
                    strQry.AppendLine(" AND IHH.INVOICE_NUMBER = 0 ");
                }
            }
            else
            {
                strQry.AppendLine(" AND IHH.INVOICE_NUMBER = 0 ");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" IHH.INVOICE_NUMBER DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "InvoiceEmail", -1);
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" IHH.INVOICE_NUMBER");
            strQry.AppendLine(",IHH.INVOICE_DATE");
            strQry.AppendLine(",IHH.CONTACT_PERSON");
            strQry.AppendLine(",IHH.INVOICE_TOTAL");
            strQry.AppendLine(",IHH.INVOICE_VAT_TOTAL");
            strQry.AppendLine(",IHH.INVOICE_FINAL_TOTAL");
            strQry.AppendLine(",'P' AS INVOICE_TYPE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY IHH ");

            strQry.AppendLine(" WHERE IHH.COMPANY_NO = " + parint64CompanyNo);
            
            strCompanyInvPrefix = strCompanyPath + "_ProInv";

            if (Directory.Exists(strFilePath) == true)
            {
                string[] fileCompanyInvoiceFile = Directory.GetFiles(strFilePath, strCompanyPath + "_ProInv*.pdf");

                if (fileCompanyInvoiceFile.Length > 0)
                {
                    string strInvoiceNumbersIN = "";

                    for (int intFile = 0; intFile < fileCompanyInvoiceFile.Length; intFile++)
                    {
                        string[] fileParts = fileCompanyInvoiceFile[intFile].Split('.');

                        if (fileParts.Length == 2)
                        {
                            int intOffSet = fileParts[0].IndexOf(strCompanyInvPrefix);

                            if (intOffSet > -1)
                            {
                                try
                                {
                                    int intInvNumberOffset = intOffSet + strCompanyInvPrefix.Length;
                                    string strInvoiceNumber = fileParts[0].Substring(intInvNumberOffset);

                                    if (strInvoiceNumbersIN == "")
                                    {
                                        strInvoiceNumbersIN = " AND IHH.INVOICE_NUMBER IN (" + strInvoiceNumber;
                                    }
                                    else
                                    {
                                        strInvoiceNumbersIN += "," + strInvoiceNumber;
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }

                    if (strInvoiceNumbersIN != "")
                    {
                        strQry.AppendLine(strInvoiceNumbersIN + ")");
                    }
                    else
                    {
                        strQry.AppendLine(" AND IHH.INVOICE_NUMBER = 0 ");
                    }
                }
                else
                {
                    strQry.AppendLine(" AND IHH.INVOICE_NUMBER = 0 ");
                }
            }
            else
            {
                strQry.AppendLine(" AND IHH.INVOICE_NUMBER = 0 ");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" IHH.INVOICE_NUMBER DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "InvoiceEmail", -1);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" STATEMENT_NUMBER");
            strQry.AppendLine(",STATEMENT_DATE");
            strQry.AppendLine(",CONTACT_PERSON");
            strQry.AppendLine(",STATEMENT_OPEN_BALANCE");
            strQry.AppendLine(",STATEMENT_CLOSE_BALANCE");
       
            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_HEADER_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
           
            string strCompanyStatementPrefix = strCompanyPath + "_Stmnt";
         
            if (Directory.Exists(strFilePath) == true)
            {
                string[] fileCompanyStatementFile = Directory.GetFiles(strFilePath, strCompanyPath + "_Stmnt*.pdf");
                
                if (fileCompanyStatementFile.Length > 0)
                {
                    string strStatementNumbersIN = "";

                    for (int intFile = 0; intFile < fileCompanyStatementFile.Length; intFile++)
                    {
                        string[] fileParts = fileCompanyStatementFile[intFile].Split('.');

                        if (fileParts.Length == 2)
                        {
                            int intOffSet = fileParts[0].IndexOf(strCompanyStatementPrefix);

                            if (intOffSet > -1)
                            {
                                try
                                {
                                    int intStatementNumberOffset = intOffSet + strCompanyStatementPrefix.Length;
                                    string strStatementNumber = fileParts[0].Substring(intStatementNumberOffset);

                                    if (strStatementNumbersIN == "")
                                    {
                                        strStatementNumbersIN = " AND STATEMENT_NUMBER IN (" + strStatementNumber;
                                    }
                                    else
                                    {
                                        strStatementNumbersIN += "," + strStatementNumber;
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }

                    if (strStatementNumbersIN != "")
                    {
                        strQry.AppendLine(strStatementNumbersIN + ")");
                    }
                    else
                    {
                        strQry.AppendLine(" AND STATEMENT_NUMBER = 0 ");
                    }
                }
                else
                {
                    strQry.AppendLine(" AND STATEMENT_NUMBER = 0 ");
                }
            }
            else
            {
                strQry.AppendLine(" AND STATEMENT_NUMBER = 0 ");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" STATEMENT_NUMBER DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "StatementEmail", -1);
            
            byte[] bytTempCompress = Get_CompanyInvoiceStatementItem_Records(parint64CompanyNo);

            DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);

            DataSet.Merge(TempDataSet);
            
            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Change_InvoiceType(Int64 parint64CompanyNo, string strInvoiceAction)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            string[] strInvoiceActionRecords = strInvoiceAction.Split(',');

            for (int intRow = 0; intRow < strInvoiceActionRecords.Length; intRow++)
            {
                string[] strInvoiceActionParts = strInvoiceActionRecords[intRow].Split('#');

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.INVOICE_HEADER_HISTORY ");
                strQry.AppendLine(" SET INVOICE_PAID_IND = " + clsDBConnectionObjects.Text2DynamicSQL(strInvoiceActionParts[1]));

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND INVOICE_NUMBER = " + strInvoiceActionParts[0]);

                clsDBConnectionObjects.Execute_SQLCommand(strQry .ToString(),-1);
            }

            Get_Invoice_Items(parint64CompanyNo, DataSet);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Update_Record(Int64 parint64CompanyNo, byte[] parbyteDataSetCompressed, int parintCompanyInvoiceRowIndex, int parintInvoiceNumber, string pvtstrDocumentType)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSetCompressed);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK ");
            strQry.AppendLine(" SET ");

            strQry.AppendLine(" WAGE_AMOUNT = " + parDataSet.Tables["Company"].Rows[0]["WAGE_AMOUNT"].ToString());
            strQry.AppendLine(",SALARY_AMOUNT = " + parDataSet.Tables["Company"].Rows[0]["SALARY_AMOUNT"].ToString());
            strQry.AppendLine(",TIME_ATTEND_AMOUNT = " + parDataSet.Tables["Company"].Rows[0]["TIME_ATTEND_AMOUNT"].ToString());
            strQry.AppendLine(",HOURLY_SUPPORT_AMOUNT = " + parDataSet.Tables["Company"].Rows[0]["HOURLY_SUPPORT_AMOUNT"].ToString());

            strQry.AppendLine(",PERSON_NAMES1 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["PERSON_NAMES1"].ToString()));
            strQry.AppendLine(",PHONE1 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["PHONE1"].ToString()));
            strQry.AppendLine(",EMAIL1 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["EMAIL1"].ToString()));
            strQry.AppendLine(",PERSON_NAMES2 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["PERSON_NAMES2"].ToString()));
            strQry.AppendLine(",PHONE2 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["PHONE2"].ToString()));
            strQry.AppendLine(",EMAIL2 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["EMAIL2"].ToString()));
            //strQry.AppendLine(",GENERATE_INVOICE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Company"].Rows[0]["GENERATE_INVOICE_IND"].ToString()));

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            if (parintCompanyInvoiceRowIndex == -1)
            {
                if (pvtstrDocumentType == "I")
                {
                    for (int intRow = 0; intRow < parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows.Count; intRow++)
                    {
                        if (parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow].RowState == DataRowState.Deleted)
                        {
                            strQry.Clear();

                            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.INVOICE_ITEM_CURRENT ");
                            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND INVOICE_LINE_NO = " + parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_NO", DataRowVersion.Original].ToString());

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                        }
                        else
                        {
                            if (parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow].RowState == DataRowState.Added)
                            {
                                strQry.Clear();

                                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.INVOICE_ITEM_CURRENT ");
                                strQry.AppendLine("(COMPANY_NO");

                                strQry.AppendLine(",INVOICE_LINE_NO");

                                strQry.AppendLine(",INVOICE_LINE_DATE");
                                strQry.AppendLine(",INVOICE_LINE_OPTION_NO");
                                strQry.AppendLine(",INVOICE_LINE_CHOICE_NO");
                                strQry.AppendLine(",INVOICE_LINE_QTY");
                                strQry.AppendLine(",INVOICE_LINE_DESC");
                                strQry.AppendLine(",INVOICE_LINE_UNIT_PRICE)");

                                strQry.AppendLine(" SELECT ");

                                strQry.AppendLine(parint64CompanyNo.ToString());
                                strQry.AppendLine(",ISNULL(MAX(INVOICE_LINE_NO),0) + 1");
                                strQry.AppendLine(",'" + Convert.ToDateTime(parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_DATE"]).ToString("yyyy-MM-dd") + "'");
                                strQry.AppendLine("," + parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_OPTION_NO"].ToString());
                                strQry.AppendLine("," + parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_CHOICE_NO"].ToString());
                                strQry.AppendLine("," + parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_QTY"].ToString());
                                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_DESC"].ToString()));
                                strQry.AppendLine("," + parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_UNIT_PRICE"].ToString());

                                strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_ITEM_CURRENT ");

                                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                            }
                            else
                            {
                                strQry.Clear();

                                strQry.AppendLine(" UPDATE InteractPayroll.dbo.INVOICE_ITEM_CURRENT ");
                                strQry.AppendLine(" SET ");
                                strQry.AppendLine(" INVOICE_LINE_DATE = '" + Convert.ToDateTime(parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_DATE"]).ToString("yyyy-MM-dd") + "'");
                                strQry.AppendLine(",INVOICE_LINE_OPTION_NO = " + parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_OPTION_NO"].ToString());
                                strQry.AppendLine(",INVOICE_LINE_CHOICE_NO = " + parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_CHOICE_NO"].ToString());
                                strQry.AppendLine(",INVOICE_LINE_QTY = " + parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_QTY"].ToString());
                                strQry.AppendLine(",INVOICE_LINE_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_DESC"].ToString()));
                                strQry.AppendLine(",INVOICE_LINE_UNIT_PRICE = " + parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_UNIT_PRICE"].ToString());

                                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                                strQry.AppendLine(" AND INVOICE_LINE_NO = " + parDataSet.Tables["CompanyInvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_NO", DataRowVersion.Original].ToString());

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                            }
                        }
                    }
                }
                else
                {
                    //Statements
                    for (int intRow = 0; intRow < parDataSet.Tables["CompanyStatementItemCurrent"].Rows.Count; intRow++)
                    {
                        if (parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow].RowState == DataRowState.Deleted)
                        {
                            strQry.Clear();

                            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.STATEMENT_ITEM_CURRENT ");
                            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND STATEMENT_LINE_NO = " + parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_NO", DataRowVersion.Original].ToString());

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                        }
                        else
                        {
                            if (parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow].RowState == DataRowState.Added)
                            {
                                strQry.Clear();

                                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.STATEMENT_ITEM_CURRENT ");
                                strQry.AppendLine("(COMPANY_NO");
                                strQry.AppendLine(",STATEMENT_LINE_NO");
                                strQry.AppendLine(",INVOICE_NUMBER");
                                strQry.AppendLine(",STATEMENT_LINE_DATE");
                                strQry.AppendLine(",STATEMENT_LINE_DESC");
                                strQry.AppendLine(",STATEMENT_LINE_DR_TOTAL");
                                strQry.AppendLine(",STATEMENT_LINE_CR_TOTAL)");
                                
                                strQry.AppendLine(" SELECT ");
                                strQry.AppendLine(parint64CompanyNo.ToString());
                                strQry.AppendLine(",ISNULL(MAX(STATEMENT_LINE_NO),0) + 1");
                                strQry.AppendLine("," + parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow]["INVOICE_NUMBER"].ToString());
                                strQry.AppendLine(",'" + Convert.ToDateTime(parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_DATE"]).ToString("yyyy-MM-dd") + "'");
                                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_DESC"].ToString()));
                                strQry.AppendLine("," + parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"].ToString());
                                strQry.AppendLine("," + parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"].ToString());

                                strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_ITEM_CURRENT ");

                                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                            }
                            else
                            {
                                strQry.Clear();

                                strQry.AppendLine(" UPDATE InteractPayroll.dbo.STATEMENT_ITEM_CURRENT ");
                                strQry.AppendLine(" SET ");

                                strQry.AppendLine(" INVOICE_NUMBER = " + parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow]["INVOICE_NUMBER"].ToString());

                                strQry.AppendLine(",STATEMENT_LINE_DATE = '" + Convert.ToDateTime(parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_DATE"]).ToString("yyyy-MM-dd") + "'");
                                
                                strQry.AppendLine(",STATEMENT_LINE_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_DESC"].ToString()));
                                strQry.AppendLine(",STATEMENT_LINE_DR_TOTAL = " + parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"].ToString());
                                strQry.AppendLine(",STATEMENT_LINE_CR_TOTAL = " + parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"].ToString());

                                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                                strQry.AppendLine(" AND STATEMENT_LINE_NO = " + parDataSet.Tables["CompanyStatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_NO", DataRowVersion.Original].ToString());

                                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                            }
                        }
                    }
                }
            }
            else
            {
                //Fix History
                for (int intRow = 0; intRow < parDataSet.Tables["CompanyInvoiceItemHistory"].Rows.Count; intRow++)
                {
                    if (parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow].RowState == DataRowState.Deleted)
                    {
                        strQry.Clear();

                        strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.INVOICE_ITEM_HISTORY ");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND INVOICE_NUMBER = " + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_NUMBER"].ToString());
                        strQry.AppendLine(" AND INVOICE_LINE_NO = " + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_NO"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                    }
                    else
                    {
                        if (parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow].RowState == DataRowState.Added)
                        {
                            strQry.Clear();

                            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.INVOICE_ITEM_HISTORY ");
                            strQry.AppendLine("(COMPANY_NO");
                            strQry.AppendLine(",INVOICE_NUMBER");
                            strQry.AppendLine(",INVOICE_LINE_NO");
                            strQry.AppendLine(",INVOICE_LINE_DATE");
                            strQry.AppendLine(",INVOICE_LINE_CHOICE_NO");
                            strQry.AppendLine(",INVOICE_LINE_QTY");
                            strQry.AppendLine(",INVOICE_LINE_DESC");
                            strQry.AppendLine(",INVOICE_LINE_UNIT_PRICE");
                            strQry.AppendLine(",INVOICE_LINE_TOTAL)");

                            strQry.AppendLine(" SELECT ");

                            strQry.AppendLine(parint64CompanyNo.ToString());
                            strQry.AppendLine("," + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_NUMBER"].ToString());
                            strQry.AppendLine(",ISNULL(MAX(INVOICE_LINE_NO),0) + 1");
                            strQry.AppendLine(",'" + Convert.ToDateTime(parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_DATE"]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine("," + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_CHOICE_NO"].ToString());
                            strQry.AppendLine("," + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_QTY"].ToString());
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_DESC"].ToString()));
                            strQry.AppendLine("," + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_UNIT_PRICE"].ToString());
                            strQry.AppendLine("," + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_TOTAL"].ToString());

                            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_ITEM_HISTORY ");

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND INVOICE_NUMBER = " + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_NUMBER"].ToString());

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                        }
                        else
                        {
                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll.dbo.INVOICE_ITEM_HISTORY ");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" INVOICE_LINE_DATE = '" + Convert.ToDateTime(parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_DATE"]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(",INVOICE_LINE_CHOICE_NO = " + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_CHOICE_NO"].ToString());
                            strQry.AppendLine(",INVOICE_LINE_QTY = " + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_QTY"].ToString());
                            strQry.AppendLine(",INVOICE_LINE_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_DESC"].ToString()));
                            strQry.AppendLine(",INVOICE_LINE_UNIT_PRICE = " + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_UNIT_PRICE"].ToString());
                            strQry.AppendLine(",INVOICE_LINE_TOTAL = " + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_TOTAL"].ToString());

                            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND INVOICE_NUMBER = " + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_NUMBER"].ToString());
                            strQry.AppendLine(" AND INVOICE_LINE_NO = " + parDataSet.Tables["CompanyInvoiceItemHistory"].Rows[intRow]["INVOICE_LINE_NO"].ToString());

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                        }
                    }
                }

                strQry.Clear();

                DataSet myDataSet = new System.Data.DataSet();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" IHH.VAT_PERCENTAGE ");
                strQry.AppendLine(",SUM(IIH.INVOICE_LINE_TOTAL) AS INVOICE_TOTAL ");

                strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY IHH ");

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.INVOICE_ITEM_HISTORY IIH ");

                strQry.AppendLine(" ON IHH.COMPANY_NO = IIH.COMPANY_NO ");
                strQry.AppendLine(" AND IHH.INVOICE_NUMBER = IIH.INVOICE_NUMBER ");

                strQry.AppendLine(" WHERE IHH.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND IHH.INVOICE_NUMBER = " + parintInvoiceNumber);

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" IHH.VAT_PERCENTAGE ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), myDataSet, "InvoiceTotal", -1);

                double dblInvoiceTotal = Convert.ToInt32(myDataSet.Tables["InvoiceTotal"].Rows[0]["INVOICE_TOTAL"]);
                double dblInvoiceVatTotal = Math.Round(dblInvoiceTotal * (Convert.ToDouble(myDataSet.Tables["InvoiceTotal"].Rows[0]["VAT_PERCENTAGE"]) / 100), 2);
                double dblInvoiceFinalTotal = dblInvoiceTotal + dblInvoiceVatTotal;

                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.INVOICE_HEADER_HISTORY ");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" INVOICE_TOTAL = " + dblInvoiceTotal.ToString("##########.00"));
                strQry.AppendLine(",INVOICE_VAT_TOTAL = " + dblInvoiceVatTotal.ToString("##########.00"));
                strQry.AppendLine(",INVOICE_FINAL_TOTAL = " + dblInvoiceFinalTotal.ToString("##########.00"));
                
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND INVOICE_NUMBER = " + parintInvoiceNumber);
             
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                myDataSet.Dispose();
                myDataSet = null;
            }

            DataSet DataSet = GetCompanyInvoiceAndItems(parint64CompanyNo);
            DataSet MyDataset = GetCompanyStatementAndItems(parint64CompanyNo);

            DataSet.Merge(MyDataset);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] ExportFile(Int64 parint64CompanyNo, int intDocumentNumber, byte[] parbyteArrayFile, string parstrPrintedDocumentType)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            try
            {
                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" 1 AS RETURN_CODE ");

                strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Return", -1);
                
                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" COMPANY_FOLDER_NAME");

                strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", -1);

                if (DataSet.Tables["Company"].Rows[0]["COMPANY_FOLDER_NAME"].ToString().Trim() == "")
                {
                    DataSet.Tables["Return"].Rows[0]["RETURN_CODE"] = 9;

                    goto ExportFile_Continue;
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" CONTACT_PERSON");
                strQry.AppendLine(",CONTACT_EMAIL");

                if (parstrPrintedDocumentType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_HEADER_HISTORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND STATEMENT_NUMBER = " + intDocumentNumber);
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND INVOICE_NUMBER = " + intDocumentNumber);
                }

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ContactPerson", -1);

                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" BACKUP_DATABASE_PATH");
                strQry.AppendLine(",BACKUP_GMAIL_ACCOUNT");
                strQry.AppendLine(",BACKUP_GMAIL_PASSWORD");
                strQry.AppendLine(",SMTP_PORT");
                strQry.AppendLine(",SMTP_HOST_NAME");

                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Info", -1);

                DirectoryInfo diDirectoryPath = Directory.GetParent(DataSet.Tables["Info"].Rows[0]["BACKUP_DATABASE_PATH"].ToString());

                string strFilePath = diDirectoryPath.FullName;

                strFilePath += "\\InvoicesStatements";

                if (Directory.Exists(strFilePath) == false)
                {
                    Directory.CreateDirectory(strFilePath);
                }

                strFilePath += "\\" + parint64CompanyNo.ToString("00000") + "_" + DataSet.Tables["Company"].Rows[0]["COMPANY_FOLDER_NAME"].ToString();

                if (Directory.Exists(strFilePath) == false)
                {
                    Directory.CreateDirectory(strFilePath);
                }

                string strDocumentFileName = "";
                              
                if (parstrPrintedDocumentType == "I")
                {
                    strDocumentFileName = strFilePath + "\\" + parint64CompanyNo.ToString("00000") + "_" + DataSet.Tables["Company"].Rows[0]["COMPANY_FOLDER_NAME"].ToString() + "_Inv" + intDocumentNumber.ToString() + ".pdf";
                }
                else
                {
                    if (parstrPrintedDocumentType == "P")
                    {
                        strDocumentFileName = strFilePath + "\\" + parint64CompanyNo.ToString("00000") + "_" + DataSet.Tables["Company"].Rows[0]["COMPANY_FOLDER_NAME"].ToString() + "_ProInv" + intDocumentNumber.ToString() + ".pdf";
                    }
                    else
                    {
                        strDocumentFileName = strFilePath + "\\" + parint64CompanyNo.ToString("00000") + "_" + DataSet.Tables["Company"].Rows[0]["COMPANY_FOLDER_NAME"].ToString() + "_Stmnt" + intDocumentNumber.ToString() + ".pdf";
                    }
                }
                using (FileStream fs = new FileStream(strDocumentFileName, FileMode.Create))
                {
                    fs.Write(parbyteArrayFile, 0, parbyteArrayFile.Length);
                }

                string strCompanyPath = parint64CompanyNo.ToString("00000") + "_" + DataSet.Tables["Company"].Rows[0]["COMPANY_FOLDER_NAME"].ToString();
                string strCompanyInvPrefix = "";
                
                if (parstrPrintedDocumentType == "I"
                ||  parstrPrintedDocumentType == "P")
                {
                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" IHH.INVOICE_NUMBER");
                    strQry.AppendLine(",IHH.INVOICE_DATE");
                    strQry.AppendLine(",IHH.CONTACT_PERSON");
                    strQry.AppendLine(",IHH.INVOICE_TOTAL");
                    strQry.AppendLine(",IHH.INVOICE_VAT_TOTAL");
                    strQry.AppendLine(",IHH.INVOICE_FINAL_TOTAL");
                    strQry.AppendLine(",'I' AS INVOICE_TYPE");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY IHH ");

                    strQry.AppendLine(" WHERE IHH.COMPANY_NO = " + parint64CompanyNo);
                    
                    strCompanyInvPrefix = strCompanyPath + "_Inv";

                    if (Directory.Exists(strFilePath) == true)
                    {
                        string[] fileCompanyInvoiceFile = Directory.GetFiles(strFilePath, strCompanyPath + "_Inv*.pdf");

                        if (fileCompanyInvoiceFile.Length > 0)
                        {
                            string strInvoiceNumbersIN = "";

                            for (int intFile = 0; intFile < fileCompanyInvoiceFile.Length; intFile++)
                            {
                                string[] fileParts = fileCompanyInvoiceFile[intFile].Split('.');

                                if (fileParts.Length == 2)
                                {
                                    int intOffSet = fileParts[0].IndexOf(strCompanyInvPrefix);

                                    if (intOffSet > -1)
                                    {
                                        try
                                        {
                                            int intInvNumberOffset = intOffSet + strCompanyInvPrefix.Length;
                                            string strInvoiceNumber = fileParts[0].Substring(intInvNumberOffset);

                                            if (strInvoiceNumbersIN == "")
                                            {
                                                strInvoiceNumbersIN = " AND IHH.INVOICE_NUMBER IN (" + strInvoiceNumber;
                                            }
                                            else
                                            {
                                                strInvoiceNumbersIN += "," + strInvoiceNumber;
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }

                            if (strInvoiceNumbersIN != "")
                            {
                                strQry.AppendLine(strInvoiceNumbersIN + ")");
                            }
                            else
                            {
                                strQry.AppendLine(" AND IHH.INVOICE_NUMBER = 0 ");
                            }
                        }
                        else
                        {
                            strQry.AppendLine(" AND IHH.INVOICE_NUMBER = 0 ");
                        }
                    }
                    else
                    {
                        strQry.AppendLine(" AND IHH.INVOICE_NUMBER = 0 ");
                    }

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" IHH.INVOICE_NUMBER DESC");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "InvoiceEmail", -1);

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" IHH.INVOICE_NUMBER");
                    strQry.AppendLine(",IHH.INVOICE_DATE");
                    strQry.AppendLine(",IHH.CONTACT_PERSON");
                    strQry.AppendLine(",IHH.INVOICE_TOTAL");
                    strQry.AppendLine(",IHH.INVOICE_VAT_TOTAL");
                    strQry.AppendLine(",IHH.INVOICE_FINAL_TOTAL");
                    strQry.AppendLine(",'P' AS INVOICE_TYPE");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY IHH ");

                    strQry.AppendLine(" WHERE IHH.COMPANY_NO = " + parint64CompanyNo);

                    strCompanyInvPrefix = strCompanyPath + "_ProInv";

                    if (Directory.Exists(strFilePath) == true)
                    {
                        string[] fileCompanyInvoiceFile = Directory.GetFiles(strFilePath, strCompanyPath + "_ProInv*.pdf");

                        if (fileCompanyInvoiceFile.Length > 0)
                        {
                            string strInvoiceNumbersIN = "";

                            for (int intFile = 0; intFile < fileCompanyInvoiceFile.Length; intFile++)
                            {
                                string[] fileParts = fileCompanyInvoiceFile[intFile].Split('.');

                                if (fileParts.Length == 2)
                                {
                                    int intOffSet = fileParts[0].IndexOf(strCompanyInvPrefix);

                                    if (intOffSet > -1)
                                    {
                                        try
                                        {
                                            int intInvNumberOffset = intOffSet + strCompanyInvPrefix.Length;
                                            string strInvoiceNumber = fileParts[0].Substring(intInvNumberOffset);

                                            if (strInvoiceNumbersIN == "")
                                            {
                                                strInvoiceNumbersIN = " AND IHH.INVOICE_NUMBER IN (" + strInvoiceNumber;
                                            }
                                            else
                                            {
                                                strInvoiceNumbersIN += "," + strInvoiceNumber;
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }

                            if (strInvoiceNumbersIN != "")
                            {
                                strQry.AppendLine(strInvoiceNumbersIN + ")");
                            }
                            else
                            {
                                strQry.AppendLine(" AND IHH.INVOICE_NUMBER = 0 ");
                            }
                        }
                        else
                        {
                            strQry.AppendLine(" AND IHH.INVOICE_NUMBER = 0 ");
                        }
                    }
                    else
                    {
                        strQry.AppendLine(" AND IHH.INVOICE_NUMBER = 0 ");
                    }

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" IHH.INVOICE_NUMBER DESC");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "InvoiceEmail", -1);
                }
                else
                {
                    //Statements
                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" STATEMENT_NUMBER");
                    strQry.AppendLine(",STATEMENT_DATE");
                    strQry.AppendLine(",CONTACT_PERSON");
                    strQry.AppendLine(",STATEMENT_OPEN_BALANCE");
                    strQry.AppendLine(",STATEMENT_CLOSE_BALANCE");

                    strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_HEADER_HISTORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                    string strCompanyStatementPrefix = strCompanyPath + "_Stmnt";

                    if (Directory.Exists(strFilePath) == true)
                    {
                        string[] fileCompanyStatementFile = Directory.GetFiles(strFilePath, strCompanyPath + "_Stmnt*.pdf");

                        if (fileCompanyStatementFile.Length > 0)
                        {
                            string strStatementNumbersIN = "";

                            for (int intFile = 0; intFile < fileCompanyStatementFile.Length; intFile++)
                            {
                                string[] fileParts = fileCompanyStatementFile[intFile].Split('.');

                                if (fileParts.Length == 2)
                                {
                                    int intOffSet = fileParts[0].IndexOf(strCompanyStatementPrefix);

                                    if (intOffSet > -1)
                                    {
                                        try
                                        {
                                            int intStatementNumberOffset = intOffSet + strCompanyStatementPrefix.Length;
                                            string strStatementNumber = fileParts[0].Substring(intStatementNumberOffset);

                                            if (strStatementNumbersIN == "")
                                            {
                                                strStatementNumbersIN = " AND STATEMENT_NUMBER IN (" + strStatementNumber;
                                            }
                                            else
                                            {
                                                strStatementNumbersIN += "," + strStatementNumber;
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }

                            if (strStatementNumbersIN != "")
                            {
                                strQry.AppendLine(strStatementNumbersIN + ")");
                            }
                            else
                            {
                                strQry.AppendLine(" AND STATEMENT_NUMBER = 0 ");
                            }
                        }
                        else
                        {
                            strQry.AppendLine(" AND STATEMENT_NUMBER = 0 ");
                        }
                    }
                    else
                    {
                        strQry.AppendLine(" AND STATEMENT_NUMBER = 0 ");
                    }

                    strQry.AppendLine(" ORDER BY ");
                    strQry.AppendLine(" STATEMENT_NUMBER DESC");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "StatementEmail", -1);
                }

                DataSet.Tables["Return"].Rows[0]["RETURN_CODE"] = 0;
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "ExportFile_Error.txt", true))
                {
                    file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ExportFile Error = " + ex.Message);
                }
            }
                        
            ExportFile_Continue:

            if (DataSet.Tables["Company"] != null)
            {
                DataSet.Tables.Remove("Company");
            }

            if (DataSet.Tables["ContactPerson"] != null)
            {
                DataSet.Tables.Remove("ContactPerson");
            }

            if (DataSet.Tables["Info"] != null)
            {
                DataSet.Tables.Remove("Info");
            }
            
            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Send_Email(Int64 parint64CompanyNo, string parstrInvoices, string parstrStatements,string parstrEmailMessage)
        {
            int intReturnCode = 1;

            SmtpClient smtp = null;

            try
            {
                StringBuilder strQry = new StringBuilder();

                DataSet DataSet = new DataSet();

                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" COMPANY_FOLDER_NAME");

                strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", -1);

                if (DataSet.Tables["Company"].Rows[0]["COMPANY_FOLDER_NAME"].ToString().Trim() == "")
                {
                    intReturnCode = 9;
                    goto ExportFile_Continue;
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" PERSON_NAMES1 AS CONTACT_PERSON");
                strQry.AppendLine(",EMAIL1 AS CONTACT_EMAIL");
                
                strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ContactPerson", -1);


                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" EMAIL_ADDRESS");
                strQry.AppendLine(",EMAIL_PASSWORD");
                strQry.AppendLine(",NAMES");

                strQry.AppendLine(" FROM InteractPayroll.dbo.EMAIL_ACCOUNTS_AND_INFO ");
                
                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmailFrom", -1);
                
                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" BACKUP_DATABASE_PATH");
                strQry.AppendLine(",SMTP_PORT");
                strQry.AppendLine(",SMTP_HOST_NAME");

                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Info", -1);

                DirectoryInfo diDirectoryPath = Directory.GetParent(DataSet.Tables["Info"].Rows[0]["BACKUP_DATABASE_PATH"].ToString());

                string strFilePath = diDirectoryPath.FullName + "\\InvoicesStatements\\";
                string strCompanyFilePart = parint64CompanyNo.ToString("00000") + "_" + DataSet.Tables["Company"].Rows[0]["COMPANY_FOLDER_NAME"].ToString();

                smtp = new SmtpClient();

                //Email
                string strEmail = DataSet.Tables["EmailFrom"].Rows[0]["EMAIL_ADDRESS"].ToString();
                string strEmailPassword = DataSet.Tables["EmailFrom"].Rows[0]["EMAIL_PASSWORD"].ToString();

                var fromAddress = new MailAddress(strEmail, "Validite (Pty) Ltd");

                var toAddress = new MailAddress(DataSet.Tables["ContactPerson"].Rows[0]["CONTACT_EMAIL"].ToString(), DataSet.Tables["ContactPerson"].Rows[0]["CONTACT_PERSON"].ToString());
#if(DEBUG)
                toAddress = new MailAddress(strEmail, DataSet.Tables["EmailFrom"].Rows[0]["NAMES"].ToString());
#endif
                var ccAddress = new MailAddress(strEmail, DataSet.Tables["EmailFrom"].Rows[0]["NAMES"].ToString());

                string subject = "Validite (Pty) Ltd - Invoice";

                if (parstrInvoices != "")
                {
                    if (parstrStatements != "")
                    {
                        subject = "Validite (Pty) Ltd - Invoice / Statement";
                    }
                }
                else
                {
                    subject = "Validite (Pty) Ltd - Statement";
                }

                string body = parstrEmailMessage;

                smtp.Host = DataSet.Tables["Info"].Rows[0]["SMTP_HOST_NAME"].ToString();
                smtp.Port = Convert.ToInt32(DataSet.Tables["Info"].Rows[0]["SMTP_PORT"]);
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(fromAddress.Address, strEmailPassword);

                var message = new MailMessage(fromAddress, toAddress);

                //CC Myself
                message.CC.Add(ccAddress);

                message.Subject = subject;
                message.Body = body;

                if (parstrInvoices != "")
                {
                    string[] fileInvoiceParts = parstrInvoices.Split(',');

                    for (int intFileCount = 0; intFileCount < fileInvoiceParts.Length; intFileCount++)
                    {
                        string strFileAttachment = strFilePath + strCompanyFilePart + "\\" + strCompanyFilePart + "_" + fileInvoiceParts[intFileCount] + ".pdf";

                        if (File.Exists(strFileAttachment) == true)
                        {
                            //Attach Invoice
                            Attachment fileAttachment = new Attachment(strFileAttachment, System.Net.Mime.MediaTypeNames.Application.Octet);

                            message.Attachments.Add(fileAttachment);
                        }
                    }
                }

                if (parstrStatements != "")
                {
                    string[] fileStatementParts = parstrStatements.Split(',');

                    for (int intFileCount = 0; intFileCount < fileStatementParts.Length; intFileCount++)
                    {
                        string strFileAttachment = strFilePath + strCompanyFilePart + "\\" + strCompanyFilePart + "_" + fileStatementParts[intFileCount] + ".pdf";

                        if (File.Exists(strFileAttachment) == true)
                        {
                            //Attach Statement
                            Attachment fileAttachment = new Attachment(strFileAttachment, System.Net.Mime.MediaTypeNames.Application.Octet);

                            message.Attachments.Add(fileAttachment);
                        }
                    }
                }

                smtp.Send(message);

                intReturnCode = 0;
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Send_Email_Error.txt", true))
                {
                    file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Send Email Exception = " + ex.Message);
                }
            }

        ExportFile_Continue:

            return intReturnCode;
        }

        public byte[] Convert_Invoice(Int64 parint64CompanyNo,int parintInvoiceNumber,string strInvoiceDate)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" MAX(INVOICE_NUMBER) + 1 AS INVOICE_NUMBER");

            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY ");

            strQry.AppendLine(" WHERE INVOICE_TYPE_IND = 'I' ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "InvoiceNumber", -1);

            Int64 int64InvoiceNumber = Convert.ToInt64(DataSet.Tables["InvoiceNumber"].Rows[0]["INVOICE_NUMBER"]);
            
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.INVOICE_HEADER_HISTORY ");

            strQry.AppendLine(" SET ");

            strQry.AppendLine(" INVOICE_NUMBER = " + int64InvoiceNumber);
            strQry.AppendLine(",INVOICE_DATE = '" + strInvoiceDate + "'");
            strQry.AppendLine(",INVOICE_TYPE_IND = 'I'");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND INVOICE_NUMBER = " + parintInvoiceNumber);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.INVOICE_ITEM_HISTORY ");

            strQry.AppendLine(" SET ");

            strQry.AppendLine(" INVOICE_NUMBER = " + int64InvoiceNumber);
            strQry.AppendLine(",INVOICE_LINE_DATE = '" + strInvoiceDate + "'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND INVOICE_NUMBER = " + parintInvoiceNumber);
            
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            strQry.Clear();

            strQry.Append(GetCompanyInvoiceHeaderHistorySQL(parint64CompanyNo));

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyInvoiceHistory", parint64CompanyNo);

            strQry.Clear();

            strQry.Append(GetCompanyInvoiceItemHistorySQL(parint64CompanyNo));

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyInvoiceItemHistory", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
        
        public byte[] Get_CompanyInvoiceStatementItem_Records(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT TOP 1 ");

            strQry.AppendLine(" COMPANY_NO");

            //2017-09-04 Change Monthly Invoice date to End of Month
            strQry.AppendLine(",INVOICE_DATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN DATEPART(D, INVOICE_DATE) = 1 THEN  DATEADD(D, -1, DATEADD(M, 1, INVOICE_DATE)) ");

            strQry.AppendLine(" ELSE DATEADD(D, -1, DATEADD(M, 1, DATEADD(D, 1, INVOICE_DATE))) ");

            strQry.AppendLine(" END ");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND INVOICE_MONTHLY_INVOICE_IND = 'Y'");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" INVOICE_DATE DESC ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyInvoiceYYYYMMDD", parint64CompanyNo);

            if (DataSet.Tables["CompanyInvoiceYYYYMMDD"].Rows.Count == 0)
            {
                DataRow myDataRow = DataSet.Tables["CompanyInvoiceYYYYMMDD"].NewRow();

                myDataRow["COMPANY_NO"] = parint64CompanyNo;
                myDataRow["INVOICE_DATE"] = DateTime.Now.AddMonths(-4).ToString("yyyy-MM" + "-01");

                DataSet.Tables["CompanyInvoiceYYYYMMDD"].Rows.Add(myDataRow);

                myDataRow = DataSet.Tables["CompanyInvoiceYYYYMMDD"].NewRow();

                myDataRow["COMPANY_NO"] = parint64CompanyNo;
                myDataRow["INVOICE_DATE"] = DateTime.Now.AddMonths(-3).ToString("yyyy-MM" + "-01");

                DataSet.Tables["CompanyInvoiceYYYYMMDD"].Rows.Add(myDataRow);

                myDataRow = DataSet.Tables["CompanyInvoiceYYYYMMDD"].NewRow();

                myDataRow["COMPANY_NO"] = parint64CompanyNo;
                myDataRow["INVOICE_DATE"] = DateTime.Now.AddMonths(-2).ToString("yyyy-MM" + "-01");

                DataSet.Tables["CompanyInvoiceYYYYMMDD"].Rows.Add(myDataRow);

                myDataRow = DataSet.Tables["CompanyInvoiceYYYYMMDD"].NewRow();

                myDataRow["COMPANY_NO"] = parint64CompanyNo;
                myDataRow["INVOICE_DATE"] = DateTime.Now.AddMonths(-1).ToString("yyyy-MM" + "-01");

                DataSet.Tables["CompanyInvoiceYYYYMMDD"].Rows.Add(myDataRow);

                myDataRow = DataSet.Tables["CompanyInvoiceYYYYMMDD"].NewRow();

                myDataRow["COMPANY_NO"] = parint64CompanyNo;
                myDataRow["INVOICE_DATE"] = DateTime.Now.ToString("yyyy-MM" + "-01");

                DataSet.Tables["CompanyInvoiceYYYYMMDD"].Rows.Add(myDataRow);

                myDataRow = DataSet.Tables["CompanyInvoiceYYYYMMDD"].NewRow();

                myDataRow["COMPANY_NO"] = parint64CompanyNo;
                myDataRow["INVOICE_DATE"] = DateTime.Now.AddMonths(1).ToString("yyyy-MM" + "-01");

                DataSet.Tables["CompanyInvoiceYYYYMMDD"].Rows.Add(myDataRow);
            }
            else
            {
                //Add Another Month
                DataRow myDataRow = DataSet.Tables["CompanyInvoiceYYYYMMDD"].NewRow();

                myDataRow["COMPANY_NO"] = parint64CompanyNo;
                myDataRow["INVOICE_DATE"] = Convert.ToDateTime(DataSet.Tables["CompanyInvoiceYYYYMMDD"].Rows[0]["INVOICE_DATE"]).AddDays(1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

                DataSet.Tables["CompanyInvoiceYYYYMMDD"].Rows.Add(myDataRow);

                myDataRow = DataSet.Tables["CompanyInvoiceYYYYMMDD"].NewRow();
            }
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" COMPANY_NO  ");
            strQry.AppendLine(",STATEMENT_NUMBER AS STATEMENT_YYYYMM  ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_HEADER_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" STATEMENT_NUMBER DESC");
       
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyStatementYYYYMM", parint64CompanyNo);

            if (DataSet.Tables["CompanyStatementYYYYMM"].Rows.Count == 0)
            {
                DataRow myDataRow = DataSet.Tables["CompanyStatementYYYYMM"].NewRow();

                myDataRow["COMPANY_NO"] = parint64CompanyNo;
                myDataRow["STATEMENT_YYYYMM"] = DateTime.Now.AddMonths(-4).ToString("yyyyMM");

                DataSet.Tables["CompanyStatementYYYYMM"].Rows.Add(myDataRow);
                
                myDataRow = DataSet.Tables["CompanyStatementYYYYMM"].NewRow();

                myDataRow["COMPANY_NO"] = parint64CompanyNo;
                myDataRow["STATEMENT_YYYYMM"] = DateTime.Now.AddMonths(-3).ToString("yyyyMM");

                DataSet.Tables["CompanyStatementYYYYMM"].Rows.Add(myDataRow);

                myDataRow = DataSet.Tables["CompanyStatementYYYYMM"].NewRow();

                myDataRow["COMPANY_NO"] = parint64CompanyNo;
                myDataRow["STATEMENT_YYYYMM"] = DateTime.Now.AddMonths(-2).ToString("yyyyMM");

                DataSet.Tables["CompanyStatementYYYYMM"].Rows.Add(myDataRow);

                myDataRow = DataSet.Tables["CompanyStatementYYYYMM"].NewRow();

                myDataRow["COMPANY_NO"] = parint64CompanyNo;
                myDataRow["STATEMENT_YYYYMM"] = DateTime.Now.AddMonths(-1).ToString("yyyyMM");

                DataSet.Tables["CompanyStatementYYYYMM"].Rows.Add(myDataRow);

                myDataRow = DataSet.Tables["CompanyStatementYYYYMM"].NewRow();

                myDataRow["COMPANY_NO"] = parint64CompanyNo;
                myDataRow["STATEMENT_YYYYMM"] = DateTime.Now.ToString("yyyyMM");

                DataSet.Tables["CompanyStatementYYYYMM"].Rows.Add(myDataRow);

                myDataRow = DataSet.Tables["CompanyStatementYYYYMM"].NewRow();

                myDataRow["COMPANY_NO"] = parint64CompanyNo;
                myDataRow["STATEMENT_YYYYMM"] = DateTime.Now.AddMonths(1).ToString("yyyyMM");

                DataSet.Tables["CompanyStatementYYYYMM"].Rows.Add(myDataRow);
            }
            else
            {
                int intYear = Convert.ToInt32(DataSet.Tables["CompanyStatementYYYYMM"].Rows[0]["STATEMENT_YYYYMM"].ToString().Substring(0, 4));
                int intMonth = Convert.ToInt32(DataSet.Tables["CompanyStatementYYYYMM"].Rows[0]["STATEMENT_YYYYMM"].ToString().Substring(4));

                if (intMonth == 12)
                {
                    intYear += 1;
                    intMonth = 1;
                }
                else
                {
                    intMonth += 1;
                }

                //New Invoice
                DataSet.Tables["CompanyStatementYYYYMM"].Rows[0]["STATEMENT_YYYYMM"] = intYear.ToString("0000") + intMonth.ToString("00");
            }

            DataSet myDataSet = GetCompanyInvoiceAndItems(parint64CompanyNo);
            DataSet.Merge(myDataSet);
            myDataSet = GetCompanyStatementAndItems(parint64CompanyNo);
            DataSet.Merge(myDataSet);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        private DataSet GetCompanyInvoiceAndItems(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new System.Data.DataSet();

            strQry.Clear();

            strQry.Append(GetCompanyInvoiceHeaderHistorySQL(parint64CompanyNo));

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyInvoiceHistory", parint64CompanyNo);

            strQry.Clear();

            strQry.Append(GetCompanyInvoiceItemHistorySQL(parint64CompanyNo));
                       
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyInvoiceItemHistory", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",INVOICE_LINE_NO");
            strQry.AppendLine(",INVOICE_LINE_DATE");

            strQry.AppendLine(",INVOICE_LINE_OPTION_NO");
            strQry.AppendLine(",INVOICE_LINE_CHOICE_NO");
            strQry.AppendLine(",INVOICE_LINE_QTY");
            strQry.AppendLine(",INVOICE_LINE_DESC");
            strQry.AppendLine(",INVOICE_LINE_UNIT_PRICE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_ITEM_CURRENT ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyInvoiceItemCurrent", parint64CompanyNo);

            return DataSet;
        }

        private string GetCompanyInvoiceItemHistorySQL(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();


            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",INVOICE_NUMBER");
            strQry.AppendLine(",INVOICE_LINE_NO");
            strQry.AppendLine(",INVOICE_LINE_DATE");
            strQry.AppendLine(",INVOICE_LINE_CHOICE_NO");
            strQry.AppendLine(",INVOICE_LINE_QTY");
            strQry.AppendLine(",INVOICE_LINE_DESC");
            strQry.AppendLine(",INVOICE_LINE_UNIT_PRICE");
            strQry.AppendLine(",INVOICE_LINE_TOTAL");

            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_ITEM_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            return strQry.ToString();
        }
        
        private string GetCompanyInvoiceHeaderHistorySQL(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" IH.COMPANY_NO");
            strQry.AppendLine(",IH.INVOICE_DATE");
            strQry.AppendLine(",IH.INVOICE_NUMBER");

            strQry.AppendLine(",INVOICE_TYPE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN ISNULL(IH.INVOICE_TYPE_IND,'I') = 'P'  ");
            strQry.AppendLine(" THEN 'Proforma'");

            strQry.AppendLine(" ELSE 'Invoice'");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",ISNULL(IH.INVOICE_TYPE_IND,'I') AS INVOICE_TYPE_IND ");

            strQry.AppendLine(",IH.INVOICE_MONTHLY_INVOICE_IND ");

            strQry.AppendLine(",IH.CONTACT_PERSON");
            strQry.AppendLine(",IH.CONTACT_PHONE");
            strQry.AppendLine(",IH.CONTACT_EMAIL");
            strQry.AppendLine(",ISNULL(IH.INVOICE_PAID_IND,'N') AS INVOICE_PAID_IND");
            strQry.AppendLine(",INVOICE_TOTAL");
            strQry.AppendLine(",INVOICE_VAT_TOTAL");
            strQry.AppendLine(",INVOICE_FINAL_TOTAL");

            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY IH ");

            strQry.AppendLine(" WHERE IH.COMPANY_NO = " + parint64CompanyNo);
            
            strQry.AppendLine(" ORDER BY ");

            strQry.AppendLine(" INVOICE_DATE DESC");

            return strQry.ToString();
        }

        private DataSet GetCompanyStatementAndItems(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new System.Data.DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",STATEMENT_NUMBER");
            strQry.AppendLine(",STATEMENT_DATE");
            strQry.AppendLine(",CONTACT_PERSON");
            strQry.AppendLine(",CONTACT_PHONE");
            strQry.AppendLine(",CONTACT_EMAIL");
            strQry.AppendLine(",STATEMENT_OPEN_BALANCE");
            strQry.AppendLine(",STATEMENT_CLOSE_BALANCE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_HEADER_HISTORY SH ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" ORDER BY ");

            strQry.AppendLine(" STATEMENT_NUMBER DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyStatementHistory", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",STATEMENT_NUMBER");
            strQry.AppendLine(",STATEMENT_LINE_NO");
            strQry.AppendLine(",STATEMENT_LINE_DATE");
            strQry.AppendLine(",INVOICE_NUMBER");
            strQry.AppendLine(",STATEMENT_LINE_DESC");
            strQry.AppendLine(",STATEMENT_LINE_DR_TOTAL");
            strQry.AppendLine(",STATEMENT_LINE_CR_TOTAL");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_ITEM_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyStatementItemHistory", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" IHH.COMPANY_NO");
            strQry.AppendLine(",0 AS STATEMENT_LINE_NO");
            strQry.AppendLine(",IHH.INVOICE_DATE AS STATEMENT_LINE_DATE");
            strQry.AppendLine(",IHH.INVOICE_NUMBER");
            strQry.AppendLine(",'Invoice Number ' + CONVERT(VARCHAR,IHH.INVOICE_NUMBER) AS STATEMENT_LINE_DESC");
            strQry.AppendLine(",IHH.INVOICE_FINAL_TOTAL AS STATEMENT_LINE_DR_TOTAL");
            strQry.AppendLine(",0 AS STATEMENT_LINE_CR_TOTAL");
            strQry.AppendLine(",'Y' AS LOCK_IND ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY IHH ");

            strQry.AppendLine(" INNER JOIN ");

            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" COMPANY_NO ");
            strQry.AppendLine(",MAX(STATEMENT_DATE) AS MAX_STATEMENT_DATE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_HEADER_HISTORY ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" COMPANY_NO) AS MAX_DATE_TABLE ");
            strQry.AppendLine(" ON IHH.COMPANY_NO = MAX_DATE_TABLE.COMPANY_NO ");
            //2017-05-29
            strQry.AppendLine(" AND IHH.INVOICE_TYPE_IND = 'I' ");
            
            //End Of Month
            strQry.AppendLine(" AND IHH.INVOICE_DATE > MAX_DATE_TABLE.MAX_STATEMENT_DATE ");
            //1st Of Month (Add 1 Day and Add 1 Month)
            strQry.AppendLine(" AND IHH.INVOICE_DATE < DATEADD(M,1,DATEADD(D,1,MAX_DATE_TABLE.MAX_STATEMENT_DATE))");
        
            strQry.AppendLine(" WHERE IHH.COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",STATEMENT_LINE_NO");
            strQry.AppendLine(",STATEMENT_LINE_DATE");
            strQry.AppendLine(",INVOICE_NUMBER");
            strQry.AppendLine(",STATEMENT_LINE_DESC");
            strQry.AppendLine(",STATEMENT_LINE_DR_TOTAL");
            strQry.AppendLine(",STATEMENT_LINE_CR_TOTAL");
            strQry.AppendLine(",'N' AS LOCK_IND ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_ITEM_CURRENT ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyStatementItemCurrent", parint64CompanyNo);
            
            Get_Invoice_Items(parint64CompanyNo, DataSet);
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" STATEMENT_NUMBER");
            strQry.AppendLine(",INVOICE_NUMBER");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_ITEM_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" GROUP BY ");

            strQry.AppendLine(" STATEMENT_NUMBER");
            strQry.AppendLine(",INVOICE_NUMBER");
          
            strQry.AppendLine(" ORDER BY ");

            strQry.AppendLine(" STATEMENT_NUMBER");
            strQry.AppendLine(",INVOICE_NUMBER");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyStatementInvoiceChoiceHistory", parint64CompanyNo);

            return DataSet;
        }

        private void Get_Invoice_Items(Int64 parint64CompanyNo,DataSet dsDataSet)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            //2017-11-11
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" -1 AS INVOICE_NUMBER");
            strQry.AppendLine(",0 AS INVOICE_FINAL_TOTAL");

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" INVOICE_NUMBER");
            strQry.AppendLine(",STATEMENT_LINE_CR_TOTAL AS INVOICE_FINAL_TOTAL");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_ITEM_CURRENT ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" INVOICE_NUMBER");
            strQry.AppendLine(",INVOICE_FINAL_TOTAL");

            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND INVOICE_TYPE_IND = 'I'");
            strQry.AppendLine(" AND ISNULL(INVOICE_PAID_IND,'N') <> 'Y' ");

            strQry.AppendLine(" ORDER BY ");

            strQry.AppendLine(" 1");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), dsDataSet, "CompanyStatementInvoiceChoiceCurrent", parint64CompanyNo);

            strQry.Clear();

            //2018-06-02
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" IHH.INVOICE_NUMBER");
            strQry.AppendLine(",IHH.INVOICE_DATE ");
            strQry.AppendLine(",SIH.STATEMENT_NUMBER ");
            strQry.AppendLine(",ISNULL(SIH.STATEMENT_LINE_CR_TOTAL, 0) AS STATEMENT_LINE_CR_TOTAL ");
            strQry.AppendLine(",INVOICE_PAID_IND = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN IHH.INVOICE_PAID_IND = 'N' THEN 'Unpaid' ");
            strQry.AppendLine(" WHEN IHH.INVOICE_PAID_IND = 'P' THEN 'Partially Paid' ");
            strQry.AppendLine(" ELSE 'Paid' ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",SHH.STATEMENT_DATE ");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY IHH");

            strQry.AppendLine(" LEFT JOIN  InteractPayroll.dbo.STATEMENT_ITEM_HISTORY SIH");
            strQry.AppendLine(" ON IHH.COMPANY_NO = SIH.COMPANY_NO");
            strQry.AppendLine(" AND IHH.INVOICE_NUMBER = SIH.INVOICE_NUMBER");
            strQry.AppendLine(" AND SIH.STATEMENT_LINE_CR_TOTAL > 0");
            
            strQry.AppendLine(" LEFT JOIN  InteractPayroll.dbo.STATEMENT_HEADER_HISTORY SHH");
            strQry.AppendLine(" ON SIH.COMPANY_NO = SHH.COMPANY_NO");
            strQry.AppendLine(" AND SIH.STATEMENT_NUMBER = SHH.STATEMENT_NUMBER");
            
            strQry.AppendLine(" WHERE IHH.COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" AND ISNULL(IHH.INVOICE_PAID_IND, 'N') <> 'Y'");
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" IHH.INVOICE_NUMBER DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), dsDataSet, "CompanyInvoiceMaintain", parint64CompanyNo);
        }

        public byte[] Generate_Invoice(Int64 parint64CompanyNo, string parstrYYYYMMDD, string parstrMonthlyInvoice, string parstrInvoiceType)
        {
            StringBuilder strQry = new StringBuilder();
            double dblQty = 0;
            double dblUnitPrice = 0;
            double dblTotal = 0;
            double dblInvoiceTotal = 0;
            Int64 int64InvoiceNumber = 0;
            int intLineNo = 1;
            int intPostAddressLine = 0;
            string[] strPostAddr = new string[5];

            DataSet DataSet = new DataSet();

            DateTime myDateTime = DateTime.ParseExact(parstrYYYYMMDD,"yyyyMMdd",null);
            bool blnInvoiceNoUsed = false;
            string strDesc = "";

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" INVOICE_LINE_CHOICE_NO");
            strQry.AppendLine(",INVOICE_LINE_CHOICE_DESC");
            strQry.AppendLine(",INVOICE_LINE_CHOICE_DETAIL");

            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_LINE_CHOICE_DESC ");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" INVOICE_LINE_CHOICE_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ItemChoice", -1);

            strQry.Clear();

            if (parstrInvoiceType == "I")
            {
                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" MAX(INVOICE_NUMBER) + 1 AS INVOICE_NUMBER");

                strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY ");

                strQry.AppendLine(" WHERE INVOICE_TYPE_IND = 'I' ");
            }
            else
            {
                //ProForma
                strQry.AppendLine(" SELECT  TOP 1");

                strQry.AppendLine(" INVOICE_NUMBER = ");

                strQry.AppendLine(" CASE ");
                
                strQry.AppendLine(" WHEN INVOICE_DATE = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND INVOICE_TYPE_IND = 'P'");

                strQry.AppendLine(" THEN INVOICE_NUMBER + 1 ");

                strQry.AppendLine(" ELSE " + DateTime.Now.ToString("yyMMdd") + "01");
                
                strQry.AppendLine(" END ");

                strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY ");

                strQry.AppendLine(" ORDER BY 1 DESC ");
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "InvoiceNumber", -1);

            int64InvoiceNumber = Convert.ToInt64(DataSet.Tables["InvoiceNumber"].Rows[0]["INVOICE_NUMBER"]);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" VAT_PERCENT");

            strQry.AppendLine(" FROM InteractPayroll.dbo.VAT_PERCENT ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Vat", -1);

            //NB. NB. NB
            //I Set Vat to Zero (Don't have VAT NUMBER)
            DataSet.Tables["Vat"].Rows[0]["VAT_PERCENT"] = 0;
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_DESC");
            strQry.AppendLine(",ISNULL(WAGE_AMOUNT,0) AS WAGE_AMOUNT ");
            strQry.AppendLine(",ISNULL(SALARY_AMOUNT,0) AS SALARY_AMOUNT ");
            strQry.AppendLine(",ISNULL(TIME_ATTEND_AMOUNT,0) AS TIME_ATTEND_AMOUNT ");

            strQry.AppendLine(",PERSON_NAMES1");
            strQry.AppendLine(",PHONE1");
            strQry.AppendLine(",EMAIL1");

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", -1);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" POST_ADDR_LINE1");
            strQry.AppendLine(",POST_ADDR_LINE2 ");
            strQry.AppendLine(",POST_ADDR_LINE3");
            strQry.AppendLine(",POST_ADDR_LINE4 ");
            strQry.AppendLine(",POST_ADDR_CODE");

            strQry.AppendLine(",VAT_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "OwnCompany", parint64CompanyNo);

            //Data Is Passed to Reports
            if (DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE1"].ToString() != "")
            {
                strPostAddr[intPostAddressLine] = DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE1"].ToString();

                intPostAddressLine += 1;
            }

            if (DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE2"].ToString() != "")
            {
                strPostAddr[intPostAddressLine] = DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE2"].ToString();

                intPostAddressLine += 1;
            }

            if (DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE3"].ToString() != "")
            {
                strPostAddr[intPostAddressLine] = DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE3"].ToString();

                intPostAddressLine += 1;
            }

            if (DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE4"].ToString() != "")
            {
                strPostAddr[intPostAddressLine] = DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE4"].ToString();

                intPostAddressLine += 1;
            }

            if (DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_CODE"].ToString() != "")
            {
                strPostAddr[intPostAddressLine] = DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_CODE"].ToString();

                intPostAddressLine += 1;
            }

            //Initialise Fields (Remove null)
            for (int intRow = intPostAddressLine; intRow < 5; intRow++)
            {
                strPostAddr[intRow] = "";
            }

            if (parstrMonthlyInvoice == "Y")
            {
                DataView myTempDataView;

                strQry.Clear();

                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" PAY_CATEGORY_TYPE ");
                strQry.AppendLine(",COUNT(EMPLOYEE_NO) AS NUMBER_COUNT");

                strQry.AppendLine(" FROM ");

                strQry.AppendLine("(SELECT ");

                strQry.AppendLine(" PAY_CATEGORY_TYPE ");
                strQry.AppendLine(",EMPLOYEE_NO ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE ");

                //Not Closed
                strQry.AppendLine(" WHERE EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" GROUP BY ");

                strQry.AppendLine(" PAY_CATEGORY_TYPE ");
                strQry.AppendLine(",EMPLOYEE_NO) AS TEMP_TABLE ");

                strQry.AppendLine(" GROUP BY");

                strQry.AppendLine(" PAY_CATEGORY_TYPE ");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parint64CompanyNo);

                myTempDataView = null;
                myTempDataView = new DataView(DataSet.Tables["Temp"],
                "PAY_CATEGORY_TYPE = 'S'",
                "",
                DataViewRowState.CurrentRows);

                if (myTempDataView.Count > 0)
                {
                    dblQty = Convert.ToDouble(myTempDataView[0]["NUMBER_COUNT"]);

                    dblUnitPrice = Convert.ToDouble(DataSet.Tables["Company"].Rows[0]["SALARY_AMOUNT"]);

                    dblTotal = dblQty * dblUnitPrice;
                    dblInvoiceTotal += dblTotal; 

                    DataView ItemChoiceDataView = new DataView(DataSet.Tables["ItemChoice"],
                    "INVOICE_LINE_CHOICE_NO = 3",
                    "",
                    DataViewRowState.CurrentRows);

                    strDesc = ItemChoiceDataView[0]["INVOICE_LINE_CHOICE_DETAIL"].ToString().Replace("#Amount#", dblUnitPrice.ToString("########0.00"));

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.INVOICE_ITEM_HISTORY");

                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",INVOICE_NUMBER");
                    strQry.AppendLine(",INVOICE_LINE_NO");
                    strQry.AppendLine(",INVOICE_LINE_DATE");
                    strQry.AppendLine(",INVOICE_LINE_CHOICE_NO");
                    strQry.AppendLine(",INVOICE_LINE_QTY");
                    strQry.AppendLine(",INVOICE_LINE_DESC");
                    strQry.AppendLine(",INVOICE_LINE_UNIT_PRICE");
                    strQry.AppendLine(",INVOICE_LINE_TOTAL)");

                    strQry.AppendLine(" VALUES ");

                    strQry.AppendLine("(" + parint64CompanyNo);
                    strQry.AppendLine("," + DataSet.Tables["InvoiceNumber"].Rows[0]["INVOICE_NUMBER"].ToString());
                    strQry.AppendLine("," + intLineNo);
                    strQry.AppendLine(",'" + myDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + ItemChoiceDataView[0]["INVOICE_LINE_CHOICE_NO"].ToString());
                    strQry.AppendLine("," + dblQty.ToString());
                    strQry.AppendLine(",'" + strDesc + " - " + myDateTime.ToString("MMMM yyyy") + "'");
                    strQry.AppendLine("," + dblUnitPrice.ToString());
                    strQry.AppendLine("," + dblTotal.ToString() + ")");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    intLineNo += 1;
                    blnInvoiceNoUsed = true;
                }

                myTempDataView = null;
                myTempDataView = new DataView(DataSet.Tables["Temp"],
                "PAY_CATEGORY_TYPE = 'W'",
                "",
                DataViewRowState.CurrentRows);

                if (myTempDataView.Count > 0)
                {
                    dblQty = Convert.ToDouble(myTempDataView[0]["NUMBER_COUNT"]);

                    dblUnitPrice = Convert.ToDouble(DataSet.Tables["Company"].Rows[0]["WAGE_AMOUNT"]);

                    dblTotal = dblQty * dblUnitPrice;
                    dblInvoiceTotal += dblTotal; 

                    DataView ItemChoiceDataView = new DataView(DataSet.Tables["ItemChoice"],
                    "INVOICE_LINE_CHOICE_NO = 2",
                    "",
                    DataViewRowState.CurrentRows);

                    strDesc = ItemChoiceDataView[0]["INVOICE_LINE_CHOICE_DETAIL"].ToString().Replace("#Amount#", dblUnitPrice.ToString("########0.00"));

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.INVOICE_ITEM_HISTORY");

                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",INVOICE_NUMBER");
                    strQry.AppendLine(",INVOICE_LINE_NO");
                    strQry.AppendLine(",INVOICE_LINE_DATE");
                    strQry.AppendLine(",INVOICE_LINE_CHOICE_NO");
                    strQry.AppendLine(",INVOICE_LINE_QTY");
                    strQry.AppendLine(",INVOICE_LINE_DESC");
                    strQry.AppendLine(",INVOICE_LINE_UNIT_PRICE");
                    strQry.AppendLine(",INVOICE_LINE_TOTAL)");

                    strQry.AppendLine(" VALUES ");

                    strQry.AppendLine("(" + parint64CompanyNo);
                    strQry.AppendLine("," + DataSet.Tables["InvoiceNumber"].Rows[0]["INVOICE_NUMBER"].ToString());
                    strQry.AppendLine("," + intLineNo);
                    strQry.AppendLine(",'" + myDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + ItemChoiceDataView[0]["INVOICE_LINE_CHOICE_NO"].ToString());
                    strQry.AppendLine("," + dblQty.ToString());
                    strQry.AppendLine(",'" + strDesc + " - " + myDateTime.ToString("MMMM yyyy") + "'");
                    strQry.AppendLine("," + dblUnitPrice.ToString());
                    strQry.AppendLine("," + dblTotal.ToString() + ")");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    intLineNo += 1;
                    blnInvoiceNoUsed = true;
                }

                myTempDataView = null;
                myTempDataView = new DataView(DataSet.Tables["Temp"],
                "PAY_CATEGORY_TYPE = 'T'",
                "",
                DataViewRowState.CurrentRows);

                if (myTempDataView.Count > 0)
                {
                    dblQty = Convert.ToDouble(myTempDataView[0]["NUMBER_COUNT"]);
                    dblUnitPrice = Convert.ToDouble(DataSet.Tables["Company"].Rows[0]["TIME_ATTEND_AMOUNT"]);

                    dblTotal = dblQty * dblUnitPrice;
                    dblInvoiceTotal += dblTotal;

                    DataView ItemChoiceDataView = new DataView(DataSet.Tables["ItemChoice"],
                    "INVOICE_LINE_CHOICE_NO = 4",
                    "",
                    DataViewRowState.CurrentRows);

                    strDesc = ItemChoiceDataView[0]["INVOICE_LINE_CHOICE_DETAIL"].ToString().Replace("#Amount#", dblUnitPrice.ToString("########0.00"));

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.INVOICE_ITEM_HISTORY");

                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",INVOICE_NUMBER");
                    strQry.AppendLine(",INVOICE_LINE_NO");
                    strQry.AppendLine(",INVOICE_LINE_DATE");

                    strQry.AppendLine(",INVOICE_LINE_CHOICE_NO");

                    strQry.AppendLine(",INVOICE_LINE_QTY");
                    strQry.AppendLine(",INVOICE_LINE_DESC");
                    strQry.AppendLine(",INVOICE_LINE_UNIT_PRICE");
                    strQry.AppendLine(",INVOICE_LINE_TOTAL)");

                    strQry.AppendLine(" VALUES ");

                    strQry.AppendLine("(" + parint64CompanyNo);
                    strQry.AppendLine("," + DataSet.Tables["InvoiceNumber"].Rows[0]["INVOICE_NUMBER"].ToString());
                    strQry.AppendLine("," + intLineNo);
                    strQry.AppendLine(",'" + myDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + ItemChoiceDataView[0]["INVOICE_LINE_CHOICE_NO"].ToString());
                    strQry.AppendLine("," + dblQty.ToString());
                    strQry.AppendLine(",'" + strDesc + " - " + myDateTime.ToString("MMMM yyyy") + "'");
                    strQry.AppendLine("," + dblUnitPrice.ToString());
                    strQry.AppendLine("," + dblTotal.ToString() + ")");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    intLineNo += 1;
                    blnInvoiceNoUsed = true;
                }
                else
                {
                    //NB To Be Remove - This is Temporarily
                    if (parint64CompanyNo == 11)
                    {
                        strQry.Clear();

                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" DISTINCT(EMPLOYEE_NO)");
                                           
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND TIMESHEET_DATE >= '" + myDateTime.AddMonths(-1).ToString("yyyy-MM-dd") + "'");
                        strQry.AppendLine(" AND TIMESHEET_DATE < '" + myDateTime.ToString("yyyy-MM-dd") + "'");

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeCount", parint64CompanyNo);

                        if (DataSet.Tables["EmployeeCount"].Rows.Count > 0)
                        {
                            dblQty = DataSet.Tables["EmployeeCount"].Rows.Count;
                            dblUnitPrice = Convert.ToDouble(DataSet.Tables["Company"].Rows[0]["TIME_ATTEND_AMOUNT"]);

                            dblTotal = dblQty * dblUnitPrice;
                            dblInvoiceTotal += dblTotal;

                            DataView ItemChoiceDataView = new DataView(DataSet.Tables["ItemChoice"],
                            "INVOICE_LINE_CHOICE_NO = 4",
                            "",
                            DataViewRowState.CurrentRows);

                            strDesc = ItemChoiceDataView[0]["INVOICE_LINE_CHOICE_DETAIL"].ToString().Replace("#Amount#", dblUnitPrice.ToString("########0.00"));

                            strQry.Clear();

                            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.INVOICE_ITEM_HISTORY");

                            strQry.AppendLine("(COMPANY_NO");
                            strQry.AppendLine(",INVOICE_NUMBER");
                            strQry.AppendLine(",INVOICE_LINE_NO");
                            strQry.AppendLine(",INVOICE_LINE_DATE");

                            strQry.AppendLine(",INVOICE_LINE_CHOICE_NO");

                            strQry.AppendLine(",INVOICE_LINE_QTY");
                            strQry.AppendLine(",INVOICE_LINE_DESC");
                            strQry.AppendLine(",INVOICE_LINE_UNIT_PRICE");
                            strQry.AppendLine(",INVOICE_LINE_TOTAL)");

                            strQry.AppendLine(" VALUES ");

                            strQry.AppendLine("(" + parint64CompanyNo);
                            strQry.AppendLine("," + DataSet.Tables["InvoiceNumber"].Rows[0]["INVOICE_NUMBER"].ToString());
                            strQry.AppendLine("," + intLineNo);
                            strQry.AppendLine(",'" + myDateTime.ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine("," + ItemChoiceDataView[0]["INVOICE_LINE_CHOICE_NO"].ToString());
                            strQry.AppendLine("," + dblQty.ToString());
                            strQry.AppendLine(",'" + strDesc + " - " + myDateTime.AddMonths(-1).ToString("MMMM yyyy") + "'");
                            strQry.AppendLine("," + dblUnitPrice.ToString());
                            strQry.AppendLine("," + dblTotal.ToString() + ")");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                            intLineNo += 1;
                            blnInvoiceNoUsed = true;
                        }
                    }
                }
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" INVOICE_LINE_NO ");
            strQry.AppendLine(",INVOICE_LINE_DATE");
            strQry.AppendLine(",INVOICE_LINE_CHOICE_NO");
            strQry.AppendLine(",INVOICE_LINE_QTY");
            strQry.AppendLine(",INVOICE_LINE_DESC");
            strQry.AppendLine(",INVOICE_LINE_UNIT_PRICE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_ITEM_CURRENT ");

            //Next Invoice (Ignore Pending Items)
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND INVOICE_LINE_OPTION_NO = 0 ");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" INVOICE_LINE_DATE");
            strQry.AppendLine(",INVOICE_LINE_NO ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "InvoiceItemCurrent", parint64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["InvoiceItemCurrent"].Rows.Count; intRow++)
            {
                dblQty = Convert.ToDouble(DataSet.Tables["InvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_QTY"]);
                dblUnitPrice = Convert.ToDouble(DataSet.Tables["InvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_UNIT_PRICE"]);

                dblTotal = dblQty * dblUnitPrice;
                dblInvoiceTotal += dblTotal; 

                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.INVOICE_ITEM_HISTORY");

                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",INVOICE_NUMBER");
                strQry.AppendLine(",INVOICE_LINE_NO");
                strQry.AppendLine(",INVOICE_LINE_DATE");
                strQry.AppendLine(",INVOICE_LINE_CHOICE_NO");
                strQry.AppendLine(",INVOICE_LINE_QTY");
                strQry.AppendLine(",INVOICE_LINE_DESC");
                strQry.AppendLine(",INVOICE_LINE_UNIT_PRICE");
                strQry.AppendLine(",INVOICE_LINE_TOTAL)");

                strQry.AppendLine(" VALUES ");

                strQry.AppendLine("(" + parint64CompanyNo);
                strQry.AppendLine("," + DataSet.Tables["InvoiceNumber"].Rows[0]["INVOICE_NUMBER"].ToString());
                strQry.AppendLine("," + intLineNo);
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDateTime(DataSet.Tables["InvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_DATE"]).ToString("yyyy-MM-dd")));
                strQry.AppendLine("," + DataSet.Tables["InvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_CHOICE_NO"].ToString());
                strQry.AppendLine("," + dblQty.ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["InvoiceItemCurrent"].Rows[intRow]["INVOICE_LINE_DESC"].ToString()));
                strQry.AppendLine("," + dblUnitPrice.ToString());
                strQry.AppendLine("," + dblTotal.ToString() + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                intLineNo += 1;
                blnInvoiceNoUsed = true;
            }

            if (blnInvoiceNoUsed == true)
            {
                double dblInvoiceVatTotal =  Math.Round(dblInvoiceTotal * (Convert.ToDouble(DataSet.Tables["Vat"].Rows[0]["VAT_PERCENT"]) / 100),2);
                double dblInvoiceFinalTotal = dblInvoiceTotal + dblInvoiceVatTotal;
                
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.INVOICE_HEADER_HISTORY");

                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",INVOICE_NUMBER");
                strQry.AppendLine(",CUSTOMER_DESC");
                //strQry.AppendLine(",INVOICE_YYYYMM");
                strQry.AppendLine(",INVOICE_DATE");
                strQry.AppendLine(",VAT_NUMBER");
                strQry.AppendLine(",CONTACT_PERSON");
                strQry.AppendLine(",CONTACT_PHONE");
                strQry.AppendLine(",CONTACT_EMAIL");
                strQry.AppendLine(",ADDRESS_LINE1");
                strQry.AppendLine(",ADDRESS_LINE2");
                strQry.AppendLine(",ADDRESS_LINE3");
                strQry.AppendLine(",ADDRESS_LINE4");
                strQry.AppendLine(",ADDRESS_LINE5");
              
                strQry.AppendLine(",VAT_PERCENTAGE");
                strQry.AppendLine(",INVOICE_TOTAL");
                strQry.AppendLine(",INVOICE_VAT_TOTAL");
                strQry.AppendLine(",INVOICE_FINAL_TOTAL");
                strQry.AppendLine(",INVOICE_PAID_IND");

                strQry.AppendLine(",INVOICE_TYPE_IND");
                strQry.AppendLine(",INVOICE_MONTHLY_INVOICE_IND)");
                
                strQry.AppendLine(" VALUES ");

                strQry.AppendLine("(" + parint64CompanyNo);
                strQry.AppendLine("," + DataSet.Tables["InvoiceNumber"].Rows[0]["INVOICE_NUMBER"].ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Company"].Rows[0]["COMPANY_DESC"].ToString()));
                strQry.AppendLine(",'" + myDateTime.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["OwnCompany"].Rows[0]["VAT_NO"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Company"].Rows[0]["PERSON_NAMES1"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Company"].Rows[0]["PHONE1"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["Company"].Rows[0]["EMAIL1"].ToString()));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strPostAddr[0]));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strPostAddr[1]));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strPostAddr[2]));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strPostAddr[3]));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strPostAddr[4]));
                strQry.AppendLine("," + DataSet.Tables["Vat"].Rows[0]["VAT_PERCENT"].ToString());
                strQry.AppendLine("," + dblInvoiceTotal.ToString("#########0.00"));
                strQry.AppendLine("," + dblInvoiceVatTotal.ToString("#########0.00"));
                strQry.AppendLine("," + dblInvoiceFinalTotal.ToString("#########0.00"));
                strQry.AppendLine(",'N'");

                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrInvoiceType));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrMonthlyInvoice) + ")");
              
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.INVOICE_ITEM_CURRENT ");
                //Next Invoice
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND INVOICE_LINE_OPTION_NO = 0 ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            DataSet = null;
            DataSet = GetCompanyInvoiceAndItems(parint64CompanyNo);
            DataSet myDataSet = GetCompanyStatementAndItems(parint64CompanyNo);
            DataSet.Merge(myDataSet);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
        
        public byte[] Generate_Statement(Int64 parint64CompanyNo, string parstrYYYYMMDD, string parstrMonthlyInvoice)
        {
            StringBuilder strQry = new StringBuilder();
       
            int intLineNo = 1000;
            int intPostAddressLine = 0;
            string[] strPostAddr = new string[5];
            double dblStatementOpeningBalance = 0;
            double dblStatementClosingBalance = 0;
            double dblStatementTotal = 0;

            DataSet DataSet = new DataSet();

            DateTime myDateTime = new DateTime(Convert.ToInt32(parstrYYYYMMDD.Substring(0, 4)), Convert.ToInt32(parstrYYYYMMDD.Substring(4)), 1).AddMonths(1).AddDays(-1);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" STATEMENT_CLOSE_BALANCE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_HEADER_HISTORY ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND STATEMENT_NUMBER = " + myDateTime.AddMonths(-1).ToString("yyyyMM"));

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "OpeningBalance", parint64CompanyNo);

            if (DataSet.Tables["OpeningBalance"].Rows.Count > 0)
            {
                dblStatementOpeningBalance = Convert.ToDouble(DataSet.Tables["OpeningBalance"].Rows[0]["STATEMENT_CLOSE_BALANCE"]);
            }
       
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" POST_ADDR_LINE1");
            strQry.AppendLine(",POST_ADDR_LINE2 ");
            strQry.AppendLine(",POST_ADDR_LINE3");
            strQry.AppendLine(",POST_ADDR_LINE4 ");
            strQry.AppendLine(",POST_ADDR_CODE");

            strQry.AppendLine(",VAT_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "OwnCompany", parint64CompanyNo);

            //Data Is Passed to Reports
            if (DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE1"].ToString() != "")
            {
                strPostAddr[intPostAddressLine] = DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE1"].ToString();

                intPostAddressLine += 1;
            }

            if (DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE2"].ToString() != "")
            {
                strPostAddr[intPostAddressLine] = DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE2"].ToString();

                intPostAddressLine += 1;
            }

            if (DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE3"].ToString() != "")
            {
                strPostAddr[intPostAddressLine] = DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE3"].ToString();

                intPostAddressLine += 1;
            }

            if (DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE4"].ToString() != "")
            {
                strPostAddr[intPostAddressLine] = DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_LINE4"].ToString();

                intPostAddressLine += 1;
            }

            if (DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_CODE"].ToString() != "")
            {
                strPostAddr[intPostAddressLine] = DataSet.Tables["OwnCompany"].Rows[0]["POST_ADDR_CODE"].ToString();

                intPostAddressLine += 1;
            }

            //Initialise Fields (Remove null)
            for (int intRow = intPostAddressLine; intRow < 5; intRow++)
            {
                strPostAddr[intRow] = "";
            }
            
            strQry.Clear();

            //Get Invoices that Fall in Statement Month
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" IHH.INVOICE_DATE");
            strQry.AppendLine(",IHH.INVOICE_NUMBER");
            strQry.AppendLine(",'Invoice Number ' + CONVERT(VARCHAR,IHH.INVOICE_NUMBER) AS STATEMENT_LINE_DESC");
            strQry.AppendLine(",IHH.INVOICE_FINAL_TOTAL");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY IHH");

            strQry.AppendLine(" LEFT JOIN InteractPayroll.dbo.STATEMENT_ITEM_HISTORY SIH");
            strQry.AppendLine(" ON IHH.COMPANY_NO = SIH.COMPANY_NO ");
            strQry.AppendLine(" AND IHH.INVOICE_NUMBER = SIH.INVOICE_NUMBER ");
            strQry.AppendLine(" AND SIH.STATEMENT_LINE_DR_TOTAL > 0 ");
            
            strQry.AppendLine(" WHERE IHH.COMPANY_NO = " + parint64CompanyNo);
            //Type = Invoice (Not Proforma) 
            strQry.AppendLine(" AND IHH.INVOICE_TYPE_IND = 'I'");
            //1st Of Current Month (Inclusive)
            strQry.AppendLine(" AND IHH.INVOICE_DATE <= '" + myDateTime.ToString("yyyy-MM-dd") + "'");

            //Record does Not Exist
            strQry.AppendLine(" AND SIH.COMPANY_NO IS NULL ");

            strQry.AppendLine(" ORDER BY ");

            strQry.AppendLine(" IHH.INVOICE_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "StatementInvoicesCurrent", parint64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["StatementInvoicesCurrent"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.STATEMENT_ITEM_HISTORY");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",STATEMENT_NUMBER");
                strQry.AppendLine(",STATEMENT_LINE_NO");
                strQry.AppendLine(",INVOICE_NUMBER");
                strQry.AppendLine(",STATEMENT_LINE_DATE");
                strQry.AppendLine(",STATEMENT_LINE_DESC");
                strQry.AppendLine(",STATEMENT_LINE_DR_TOTAL");
                strQry.AppendLine(",STATEMENT_LINE_CR_TOTAL)");

                strQry.AppendLine(" VALUES ");

                strQry.AppendLine("(" + parint64CompanyNo);
                strQry.AppendLine("," + myDateTime.ToString("yyyyMM"));
                strQry.AppendLine("," + intLineNo);
                strQry.AppendLine("," + DataSet.Tables["StatementInvoicesCurrent"].Rows[intRow]["INVOICE_NUMBER"].ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDateTime(DataSet.Tables["StatementInvoicesCurrent"].Rows[intRow]["INVOICE_DATE"]).ToString("yyyy-MM-dd")));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["StatementInvoicesCurrent"].Rows[intRow]["STATEMENT_LINE_DESC"].ToString()));
                strQry.AppendLine("," + Convert.ToDouble(DataSet.Tables["StatementInvoicesCurrent"].Rows[intRow]["INVOICE_FINAL_TOTAL"]).ToString("#########0.00"));
                strQry.AppendLine(",0)");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                dblStatementTotal += Convert.ToDouble(DataSet.Tables["StatementInvoicesCurrent"].Rows[intRow]["INVOICE_FINAL_TOTAL"]);
               
                intLineNo += 1;
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" STATEMENT_LINE_NO ");
            strQry.AppendLine(",STATEMENT_LINE_DATE");
            strQry.AppendLine(",INVOICE_NUMBER");
            strQry.AppendLine(",STATEMENT_LINE_DATE");
            strQry.AppendLine(",STATEMENT_LINE_DESC");
            strQry.AppendLine(",STATEMENT_LINE_DR_TOTAL");
            strQry.AppendLine(",STATEMENT_LINE_CR_TOTAL");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_ITEM_CURRENT ");

            //Next Statement
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            //1st Of Current Month (Inclusive)
            strQry.AppendLine(" AND STATEMENT_LINE_DATE <= '" + myDateTime.ToString("yyyy-MM-dd") + "'");
        
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" STATEMENT_LINE_NO ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "StatementItemCurrent", parint64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["StatementItemCurrent"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.STATEMENT_ITEM_HISTORY");
                strQry.AppendLine("(COMPANY_NO");
                strQry.AppendLine(",STATEMENT_NUMBER");
                strQry.AppendLine(",STATEMENT_LINE_NO");
                strQry.AppendLine(",INVOICE_NUMBER");
                strQry.AppendLine(",STATEMENT_LINE_DATE");
                strQry.AppendLine(",STATEMENT_LINE_DESC");
                strQry.AppendLine(",STATEMENT_LINE_DR_TOTAL");
                strQry.AppendLine(",STATEMENT_LINE_CR_TOTAL)");

                strQry.AppendLine(" VALUES ");

                strQry.AppendLine("(" + parint64CompanyNo);
                strQry.AppendLine("," + myDateTime.ToString("yyyyMM"));
                strQry.AppendLine("," + intLineNo);
                strQry.AppendLine("," + DataSet.Tables["StatementItemCurrent"].Rows[intRow]["INVOICE_NUMBER"].ToString());
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(Convert.ToDateTime(DataSet.Tables["StatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_DATE"]).ToString("yyyy-MM-dd")));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["StatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_DESC"].ToString()));
                strQry.AppendLine("," + Convert.ToDouble(DataSet.Tables["StatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"]).ToString("#########0.00"));
                strQry.AppendLine("," + Convert.ToDouble(DataSet.Tables["StatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"]).ToString("#########0.00") + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                dblStatementTotal += Convert.ToDouble(DataSet.Tables["StatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"]);
                dblStatementTotal -= Convert.ToDouble(DataSet.Tables["StatementItemCurrent"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"]);

                intLineNo += 1;
            }

            //Reorder STATEMENT_ITEM_HISTORY
            intLineNo = 1;

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" STATEMENT_LINE_NO ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_ITEM_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND STATEMENT_NUMBER = " + myDateTime.ToString("yyyyMM"));
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" STATEMENT_LINE_DATE ");
            strQry.AppendLine(",INVOICE_NUMBER ");
            strQry.AppendLine(",STATEMENT_LINE_NO ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "StatementLineReorder", parint64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["StatementLineReorder"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" UPDATE InteractPayroll.dbo.STATEMENT_ITEM_HISTORY");

                strQry.AppendLine(" SET STATEMENT_LINE_NO = " + intLineNo.ToString());

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND STATEMENT_NUMBER = " + myDateTime.ToString("yyyyMM"));
                strQry.AppendLine(" AND STATEMENT_LINE_NO = " + DataSet.Tables["StatementLineReorder"].Rows[intRow]["STATEMENT_LINE_NO"].ToString());

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                intLineNo += 1;
            }

            dblStatementClosingBalance = dblStatementOpeningBalance + dblStatementTotal;

            strQry.Clear();

            strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.STATEMENT_HEADER_HISTORY");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",STATEMENT_NUMBER");
            strQry.AppendLine(",STATEMENT_DATE");
          
            strQry.AppendLine(",CUSTOMER_DESC");
            strQry.AppendLine(",VAT_NUMBER");
            strQry.AppendLine(",CONTACT_PERSON");
            strQry.AppendLine(",CONTACT_PHONE");
            strQry.AppendLine(",CONTACT_EMAIL");
            strQry.AppendLine(",ADDRESS_LINE1");
            strQry.AppendLine(",ADDRESS_LINE2");
            strQry.AppendLine(",ADDRESS_LINE3");
            strQry.AppendLine(",ADDRESS_LINE4");
            strQry.AppendLine(",ADDRESS_LINE5");
            strQry.AppendLine(",STATEMENT_OPEN_BALANCE");
            strQry.AppendLine(",STATEMENT_CLOSE_BALANCE)");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(parint64CompanyNo.ToString());
            strQry.AppendLine("," + myDateTime.ToString("yyyyMM"));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(myDateTime.ToString("yyyy-MM-dd")));

            strQry.AppendLine(",CL.COMPANY_DESC");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["OwnCompany"].Rows[0]["VAT_NO"].ToString()));
            strQry.AppendLine(",CL.PERSON_NAMES1");
            strQry.AppendLine(",CL.PHONE1");
            strQry.AppendLine(",CL.EMAIL1");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strPostAddr[0]));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strPostAddr[1]));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strPostAddr[2]));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strPostAddr[3]));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strPostAddr[4]));

            strQry.AppendLine("," + dblStatementOpeningBalance.ToString("#########0.00"));
            strQry.AppendLine("," + dblStatementClosingBalance.ToString("#########0.00"));

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK CL ");

            strQry.AppendLine(" LEFT JOIN InteractPayroll.dbo.STATEMENT_HEADER_HISTORY SHH");
            strQry.AppendLine(" ON CL.COMPANY_NO = SHH.COMPANY_NO ");
            strQry.AppendLine(" AND SHH.STATEMENT_NUMBER = " + myDateTime.AddMonths(-1).ToString("yyyyMM"));

            strQry.AppendLine(" WHERE CL.COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE IHH ");

            strQry.AppendLine(" SET IHH.INVOICE_PAID_IND = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN INVOICE_TOTAL_TABLE.INVOICE_TOTAL >= IHH.INVOICE_FINAL_TOTAL ");
            //Fully Paid
            strQry.AppendLine(" THEN 'Y'");
            //Partly Paid
            strQry.AppendLine(" ELSE 'P'");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY IHH");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.STATEMENT_ITEM_HISTORY SIH");
            strQry.AppendLine(" ON IHH.COMPANY_NO = SIH.COMPANY_NO");
            strQry.AppendLine(" AND IHH.INVOICE_NUMBER = SIH.INVOICE_NUMBER");

            strQry.AppendLine(" INNER JOIN ");

            strQry.AppendLine("(SELECT ");

            strQry.AppendLine(" INVOICE_NUMBER");
            strQry.AppendLine(",SUM(STATEMENT_LINE_CR_TOTAL) AS INVOICE_TOTAL");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_ITEM_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" GROUP BY ");

            strQry.AppendLine(" INVOICE_NUMBER) AS INVOICE_TOTAL_TABLE");

            strQry.AppendLine(" ON SIH.INVOICE_NUMBER = INVOICE_TOTAL_TABLE.INVOICE_NUMBER");

            strQry.AppendLine(" WHERE IHH.COMPANY_NO = " + parint64CompanyNo);
            //Type = Invoice (Not Proforma) 
            strQry.AppendLine(" AND IHH.INVOICE_TYPE_IND = 'I'");
            strQry.AppendLine(" AND SIH.STATEMENT_NUMBER = " + myDateTime.ToString("yyyyMM"));

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.STATEMENT_ITEM_CURRENT ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            //1st Of Current Month (Inclusive)
            strQry.AppendLine(" AND STATEMENT_LINE_DATE <= '" + myDateTime.ToString("yyyy-MM-dd") + "'");
            
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
          
            DataSet = null;
            DataSet = GetCompanyInvoiceAndItems(parint64CompanyNo);
            DataSet myDataSet = GetCompanyStatementAndItems(parint64CompanyNo);
            DataSet.Merge(myDataSet);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Print_Invoice(Int64 parint64CompanyNo, int parintInvoiceNumber,string parstrInvoiceType)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new System.Data.DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" BANK_DETAIL_LINE1");
            strQry.AppendLine(",BANK_DETAIL_LINE2");
            strQry.AppendLine(",BANK_DETAIL_LINE3");
            strQry.AppendLine(",BANK_DETAIL_LINE4");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.MY_BANK_DETAILS ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "MyBankDetails", -1);

            strQry.Clear();
            
            strQry.AppendLine(" SELECT ");

            if (parstrInvoiceType == "I")
            {
                strQry.AppendLine(" 'Tax Invoice' AS DOCUMENT_DESC");
            }
            else
            {
                strQry.AppendLine(" 'Proforma Invoice' AS DOCUMENT_DESC");
            }

            strQry.AppendLine(",'Validite (Pty) Ltd' AS OUR_COMPANY_DESC");
            strQry.AppendLine(",'272 Oak Avenue' AS OUR_COMPANY_ADDRESS_LINE1");
            strQry.AppendLine(",'Ferndale, Randburg' AS OUR_COMPANY_ADDRESS_LINE2");
            strQry.AppendLine(",'2194' AS OUR_COMPANY_ADDRESS_LINE3");
            strQry.AppendLine(",'' AS OUR_COMPANY_ADDRESS_LINE4");
            //strQry.AppendLine(",'VAT Number 1234567890' AS OUR_COMPANY_ADDRESS_LINE4");
            strQry.AppendLine(",IH.CUSTOMER_DESC");
            strQry.AppendLine(",IH.INVOICE_NUMBER");
            strQry.AppendLine(",'Invoice No.' AS INVOICE_NUMBER_DESC");
            strQry.AppendLine(",IH.INVOICE_DATE");
            strQry.AppendLine(",'Invoice Date' AS INVOICE_DATE_DESC");
            strQry.AppendLine(",IH.VAT_NUMBER");
            strQry.AppendLine(",IH.CONTACT_PERSON");
            strQry.AppendLine(",IH.CONTACT_PHONE");
            strQry.AppendLine(",IH.CONTACT_EMAIL");
            strQry.AppendLine(",IH.ADDRESS_LINE1");
            strQry.AppendLine(",IH.ADDRESS_LINE2");
            strQry.AppendLine(",IH.ADDRESS_LINE3");
            strQry.AppendLine(",IH.ADDRESS_LINE4");
            strQry.AppendLine(",IH.ADDRESS_LINE5");
            strQry.AppendLine(",IH.INVOICE_TOTAL");
            strQry.AppendLine(",IH.INVOICE_VAT_TOTAL");
            strQry.AppendLine(",IH.INVOICE_FINAL_TOTAL");

            strQry.AppendLine(",'VAT (' + CONVERT(VARCHAR,CAST(IH.VAT_PERCENTAGE AS DECIMAL(18,2))) + '%)' AS VAT_DESC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_HEADER_HISTORY IH ");

            strQry.AppendLine(" WHERE IH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND IH.INVOICE_NUMBER = " + parintInvoiceNumber);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PrintInvoice", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" INVOICE_NUMBER");
            strQry.AppendLine(",INVOICE_LINE_NO");
            strQry.AppendLine(",INVOICE_LINE_QTY");
            strQry.AppendLine(",INVOICE_LINE_DESC");
            strQry.AppendLine(",INVOICE_LINE_DATE");
            strQry.AppendLine(",INVOICE_LINE_UNIT_PRICE");
            strQry.AppendLine(",INVOICE_LINE_TOTAL");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.INVOICE_ITEM_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND INVOICE_NUMBER = " + parintInvoiceNumber);

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" INVOICE_LINE_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "InvoiceTemp", parint64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["InvoiceTemp"].Rows.Count; intRow++)
            {
                if (intRow == 0)
                {
                    DataSet.Tables["PrintInvoice"].Columns.Add("INVOICE_LINE_NO", DataSet.Tables["InvoiceTemp"].Columns["INVOICE_LINE_NO"].DataType);
                    DataSet.Tables["PrintInvoice"].Columns.Add("INVOICE_LINE_QTY", DataSet.Tables["InvoiceTemp"].Columns["INVOICE_LINE_QTY"].DataType);
                    DataSet.Tables["PrintInvoice"].Columns.Add("INVOICE_LINE_DESC", DataSet.Tables["InvoiceTemp"].Columns["INVOICE_LINE_DESC"].DataType);
                    //Make it String
                    DataSet.Tables["PrintInvoice"].Columns.Add("INVOICE_LINE_DATE", DataSet.Tables["InvoiceTemp"].Columns["INVOICE_LINE_DESC"].DataType);

                    DataSet.Tables["PrintInvoice"].Columns.Add("INVOICE_LINE_UNIT_PRICE", DataSet.Tables["InvoiceTemp"].Columns["INVOICE_LINE_UNIT_PRICE"].DataType);
                    DataSet.Tables["PrintInvoice"].Columns.Add("INVOICE_LINE_TOTAL", DataSet.Tables["InvoiceTemp"].Columns["INVOICE_LINE_TOTAL"].DataType);

                    DataSet.Tables["PrintInvoice"].Columns.Add("BANK_DETAIL_LINE1", DataSet.Tables["InvoiceTemp"].Columns["INVOICE_LINE_DESC"].DataType);
                    DataSet.Tables["PrintInvoice"].Columns.Add("BANK_DETAIL_LINE2", DataSet.Tables["InvoiceTemp"].Columns["INVOICE_LINE_DESC"].DataType);
                    DataSet.Tables["PrintInvoice"].Columns.Add("BANK_DETAIL_LINE3", DataSet.Tables["InvoiceTemp"].Columns["INVOICE_LINE_DESC"].DataType);
                    DataSet.Tables["PrintInvoice"].Columns.Add("BANK_DETAIL_LINE4", DataSet.Tables["InvoiceTemp"].Columns["INVOICE_LINE_DESC"].DataType);

                    DataSet.Tables["PrintInvoice"].Rows[intRow]["BANK_DETAIL_LINE1"] = DataSet.Tables["MyBankDetails"].Rows[intRow]["BANK_DETAIL_LINE1"].ToString();
                    DataSet.Tables["PrintInvoice"].Rows[intRow]["BANK_DETAIL_LINE2"] = DataSet.Tables["MyBankDetails"].Rows[intRow]["BANK_DETAIL_LINE2"].ToString();
                    DataSet.Tables["PrintInvoice"].Rows[intRow]["BANK_DETAIL_LINE3"] = DataSet.Tables["MyBankDetails"].Rows[intRow]["BANK_DETAIL_LINE3"].ToString();
                    DataSet.Tables["PrintInvoice"].Rows[intRow]["BANK_DETAIL_LINE4"] = DataSet.Tables["MyBankDetails"].Rows[intRow]["BANK_DETAIL_LINE4"].ToString();
                }
                else
                {
                    DataRow myDataRow = DataSet.Tables["PrintInvoice"].NewRow();
                    DataSet.Tables["PrintInvoice"].Rows.Add(myDataRow);

                    DataSet.Tables["PrintInvoice"].Rows[intRow]["INVOICE_NUMBER"] = DataSet.Tables["PrintInvoice"].Rows[0]["INVOICE_NUMBER"].ToString();
                }

                DataSet.Tables["PrintInvoice"].Rows[intRow]["INVOICE_LINE_NO"] = Convert.ToByte(DataSet.Tables["InvoiceTemp"].Rows[intRow]["INVOICE_LINE_NO"]);
                DataSet.Tables["PrintInvoice"].Rows[intRow]["INVOICE_LINE_QTY"] = Convert.ToDouble(DataSet.Tables["InvoiceTemp"].Rows[intRow]["INVOICE_LINE_QTY"]);
                DataSet.Tables["PrintInvoice"].Rows[intRow]["INVOICE_LINE_DESC"] = DataSet.Tables["InvoiceTemp"].Rows[intRow]["INVOICE_LINE_DESC"].ToString();

                DataSet.Tables["PrintInvoice"].Rows[intRow]["INVOICE_LINE_DATE"] = Convert.ToDateTime(DataSet.Tables["InvoiceTemp"].Rows[intRow]["INVOICE_LINE_DATE"]).ToString("dd-MM-yyyy");

                DataSet.Tables["PrintInvoice"].Rows[intRow]["INVOICE_LINE_UNIT_PRICE"] = Convert.ToDouble(DataSet.Tables["InvoiceTemp"].Rows[intRow]["INVOICE_LINE_UNIT_PRICE"]);
                DataSet.Tables["PrintInvoice"].Rows[intRow]["INVOICE_LINE_TOTAL"] = Convert.ToDouble(DataSet.Tables["InvoiceTemp"].Rows[intRow]["INVOICE_LINE_TOTAL"]);

                //For Testing
                //DataSet.Tables["PrintInvoice"].Rows[intRow]["INVOICE_LINE_QTY"] = 99999999.99;
                //DataSet.Tables["PrintInvoice"].Rows[intRow]["INVOICE_LINE_UNIT_PRICE"] = 99999999.99;
                //DataSet.Tables["PrintInvoice"].Rows[intRow]["INVOICE_LINE_TOTAL"] = 99999999.99;
            }

            for (int intRow = DataSet.Tables["InvoiceTemp"].Rows.Count; intRow < 22; intRow++)
            {
                DataRow myDataRow = DataSet.Tables["PrintInvoice"].NewRow();
                DataSet.Tables["PrintInvoice"].Rows.Add(myDataRow);

                DataSet.Tables["PrintInvoice"].Rows[intRow]["INVOICE_NUMBER"] = DataSet.Tables["PrintInvoice"].Rows[0]["INVOICE_NUMBER"].ToString();
                DataSet.Tables["PrintInvoice"].Rows[intRow]["INVOICE_LINE_NO"] = intRow + 1;
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Print_Statement(Int64 parint64CompanyNo, int parintStatementNumber)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new System.Data.DataSet();
            double dblBalance = 0;

            DateTime myDateTime = new DateTime(Convert.ToInt32(parintStatementNumber.ToString().Substring(0, 4)), Convert.ToInt32(parintStatementNumber.ToString().Substring(4)), 1).AddMonths(1).AddDays(-1);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" BANK_DETAIL_LINE1");
            strQry.AppendLine(",BANK_DETAIL_LINE2");
            strQry.AppendLine(",BANK_DETAIL_LINE3");
            strQry.AppendLine(",BANK_DETAIL_LINE4");

            strQry.AppendLine(" FROM InteractPayroll.dbo.MY_BANK_DETAILS ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "MyBankDetails", -1);


            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine("'Validite (Pty) Ltd' AS OUR_COMPANY_DESC");
            strQry.AppendLine(",'272 Oak Avenue' AS OUR_COMPANY_ADDRESS_LINE1");
            strQry.AppendLine(",'Ferndale, Randburg' AS OUR_COMPANY_ADDRESS_LINE2");
            strQry.AppendLine(",'2194' AS OUR_COMPANY_ADDRESS_LINE3");
            //strQry.AppendLine(",'VAT Number 1234567890' AS OUR_COMPANY_ADDRESS_LINE4");
            strQry.AppendLine(",'' AS OUR_COMPANY_ADDRESS_LINE4");
            strQry.AppendLine(",CUSTOMER_DESC");
            strQry.AppendLine(",STATEMENT_DATE");
            //Usually Blank - Use If has Value
            strQry.AppendLine(",STATEMENT_FROM_DATE");
            strQry.AppendLine(",VAT_NUMBER");
            strQry.AppendLine(",CONTACT_PERSON");
            strQry.AppendLine(",CONTACT_PHONE");
            strQry.AppendLine(",CONTACT_EMAIL");
            strQry.AppendLine(",ADDRESS_LINE1");
            strQry.AppendLine(",ADDRESS_LINE2");
            strQry.AppendLine(",ADDRESS_LINE3");
            strQry.AppendLine(",ADDRESS_LINE4");
            strQry.AppendLine(",ADDRESS_LINE5");
            strQry.AppendLine(",STATEMENT_OPEN_BALANCE");
            strQry.AppendLine(",STATEMENT_CLOSE_BALANCE");
                            
            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_HEADER_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND STATEMENT_NUMBER = " + parintStatementNumber);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PrintStatement", parint64CompanyNo);

            strQry.Clear();
            
            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(parintStatementNumber + " AS STATEMENT_NUMBER ");
            strQry.AppendLine(",0 AS STATEMENT_LINE_NO");

            if (DataSet.Tables["PrintStatement"].Rows[0]["STATEMENT_FROM_DATE"] == System.DBNull.Value)
            {
                strQry.AppendLine(",'" + myDateTime.AddDays(1).AddMonths(-1).ToString("yyyy-MM-dd") + "' AS STATEMENT_LINE_DATE");
            }
            else
            {
                strQry.AppendLine(",'" + Convert.ToDateTime(DataSet.Tables["PrintStatement"].Rows[0]["STATEMENT_FROM_DATE"]).ToString("yyyy-MM-dd") + "' AS STATEMENT_LINE_DATE");
            }

            strQry.AppendLine(",'Balance b/f' AS STATEMENT_LINE_DESC");
            strQry.AppendLine(",'' AS INVOICE_NUMBER");

            if (Convert.ToDouble(DataSet.Tables["PrintStatement"].Rows[0]["STATEMENT_OPEN_BALANCE"]) < 0)
            {
                double dblStatementOpenBalance = Convert.ToDouble(DataSet.Tables["PrintStatement"].Rows[0]["STATEMENT_OPEN_BALANCE"]) * -1;

                strQry.AppendLine(",0.00 AS STATEMENT_LINE_DR_TOTAL");
                strQry.AppendLine("," + dblStatementOpenBalance.ToString("#########0.00") + " AS STATEMENT_LINE_CR_TOTAL");
            }
            else
            {
                strQry.AppendLine("," + Convert.ToDouble(DataSet.Tables["PrintStatement"].Rows[0]["STATEMENT_OPEN_BALANCE"]).ToString("#########0.00") + " AS STATEMENT_LINE_DR_TOTAL");
                strQry.AppendLine(",0.00 AS STATEMENT_LINE_CR_TOTAL");
            }

            strQry.AppendLine("," + Convert.ToDouble(DataSet.Tables["PrintStatement"].Rows[0]["STATEMENT_OPEN_BALANCE"]).ToString("#########0.00") + " AS STATEMENT_BALANCE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" STATEMENT_NUMBER ");
            strQry.AppendLine(",STATEMENT_LINE_NO");
            strQry.AppendLine(",CONVERT(VARCHAR,STATEMENT_LINE_DATE) AS STATEMENT_LINE_DATE ");
            strQry.AppendLine(",STATEMENT_LINE_DESC");
            strQry.AppendLine(",CONVERT(VARCHAR,INVOICE_NUMBER) AS INVOICE_NUMBER");
          
            strQry.AppendLine(",STATEMENT_LINE_DR_TOTAL");
            strQry.AppendLine(",STATEMENT_LINE_CR_TOTAL");
            strQry.AppendLine(",STATEMENT_BALANCE = ");
            strQry.AppendLine(" CASE ");
            strQry.AppendLine(" WHEN STATEMENT_LINE_DR_TOTAL <> 0 ");

            strQry.AppendLine(" THEN STATEMENT_LINE_DR_TOTAL ");

            strQry.AppendLine(" ELSE STATEMENT_LINE_CR_TOTAL * -1 ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_ITEM_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND STATEMENT_NUMBER = " + parintStatementNumber);

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "StatementTemp", parint64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["StatementTemp"].Rows.Count; intRow++)
            {
                if (intRow == 0)
                {
                    //Make it String
                    DataSet.Tables["PrintStatement"].Columns.Add("STATEMENT_NUMBER", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_DESC"].DataType);

                    DataSet.Tables["PrintStatement"].Columns.Add("STATEMENT_LINE_NO", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_NO"].DataType);
                    DataSet.Tables["PrintStatement"].Columns.Add("STATEMENT_LINE_DESC", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_DESC"].DataType);
                    DataSet.Tables["PrintStatement"].Columns.Add("INVOICE_NUMBER", DataSet.Tables["StatementTemp"].Columns["INVOICE_NUMBER"].DataType);
                    //Make it String
                    DataSet.Tables["PrintStatement"].Columns.Add("STATEMENT_LINE_DATE", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_DESC"].DataType);

                    DataSet.Tables["PrintStatement"].Columns.Add("STATEMENT_LINE_DR_TOTAL", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_DR_TOTAL"].DataType);
                    DataSet.Tables["PrintStatement"].Columns.Add("STATEMENT_LINE_CR_TOTAL", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_CR_TOTAL"].DataType);
                    DataSet.Tables["PrintStatement"].Columns.Add("STATEMENT_BALANCE", DataSet.Tables["StatementTemp"].Columns["STATEMENT_BALANCE"].DataType);

                    DataSet.Tables["PrintStatement"].Columns.Add("BANK_DETAIL_LINE1", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_DESC"].DataType);
                    DataSet.Tables["PrintStatement"].Columns.Add("BANK_DETAIL_LINE2", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_DESC"].DataType);
                    DataSet.Tables["PrintStatement"].Columns.Add("BANK_DETAIL_LINE3", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_DESC"].DataType);
                    DataSet.Tables["PrintStatement"].Columns.Add("BANK_DETAIL_LINE4", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_DESC"].DataType);

                    DataSet.Tables["PrintStatement"].Rows[intRow]["BANK_DETAIL_LINE1"] = DataSet.Tables["MyBankDetails"].Rows[intRow]["BANK_DETAIL_LINE1"].ToString();
                    DataSet.Tables["PrintStatement"].Rows[intRow]["BANK_DETAIL_LINE2"] = DataSet.Tables["MyBankDetails"].Rows[intRow]["BANK_DETAIL_LINE2"].ToString();
                    DataSet.Tables["PrintStatement"].Rows[intRow]["BANK_DETAIL_LINE3"] = DataSet.Tables["MyBankDetails"].Rows[intRow]["BANK_DETAIL_LINE3"].ToString();
                    DataSet.Tables["PrintStatement"].Rows[intRow]["BANK_DETAIL_LINE4"] = DataSet.Tables["MyBankDetails"].Rows[intRow]["BANK_DETAIL_LINE4"].ToString();

                    if (Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"]) > 0)
                    {
                        dblBalance = Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"]);
                    }
                    else
                    {
                        dblBalance = Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"]) * -1;
                    }
                }
                else
                {
                    DataRow myDataRow = DataSet.Tables["PrintStatement"].NewRow();
                    DataSet.Tables["PrintStatement"].Rows.Add(myDataRow);

                    if (Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"]) > 0)
                    {
                        dblBalance += Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"]);
                    }
                    else
                    {
                        dblBalance += Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"]) * -1;
                    }
                }

                DataSet.Tables["PrintStatement"].Rows[intRow]["STATEMENT_NUMBER"] = myDateTime.ToString("d MMMM yyyy");
                DataSet.Tables["PrintStatement"].Rows[intRow]["STATEMENT_LINE_NO"] = Convert.ToInt32(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_NO"]);
                DataSet.Tables["PrintStatement"].Rows[intRow]["STATEMENT_LINE_DESC"] = DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DESC"].ToString();

                if (DataSet.Tables["StatementTemp"].Rows[intRow]["INVOICE_NUMBER"].ToString() == "-1")
                {
                    DataSet.Tables["PrintStatement"].Rows[intRow]["INVOICE_NUMBER"] = "";
                }
                else
                {
                    DataSet.Tables["PrintStatement"].Rows[intRow]["INVOICE_NUMBER"] = DataSet.Tables["StatementTemp"].Rows[intRow]["INVOICE_NUMBER"].ToString();
                }

                DataSet.Tables["PrintStatement"].Rows[intRow]["STATEMENT_LINE_DATE"] = Convert.ToDateTime(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DATE"]).ToString("dd-MM-yyyy");

                DataSet.Tables["PrintStatement"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"] = Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"]);
                DataSet.Tables["PrintStatement"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"] = Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"]);
                DataSet.Tables["PrintStatement"].Rows[intRow]["STATEMENT_BALANCE"] = dblBalance;
                //For Testing
             
                //DataSet.Tables["PrintStatement"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"] = 99999999.99;
                //DataSet.Tables["PrintStatement"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"] = 99999999.99;
                //DataSet.Tables["PrintStatement"].Rows[intRow]["STATEMENT_BALANCE"] = 99999999.99;
                //DataSet.Tables["PrintStatement"].Rows[intRow]["STATEMENT_CLOSE_BALANCE"] = 99999999.99;
            }

            for (int intRow = DataSet.Tables["StatementTemp"].Rows.Count; intRow < 22; intRow++)
            {
                DataRow myDataRow = DataSet.Tables["PrintStatement"].NewRow();
                DataSet.Tables["PrintStatement"].Rows.Add(myDataRow);

                DataSet.Tables["PrintStatement"].Rows[intRow]["STATEMENT_NUMBER"] = myDateTime.ToString("d MMMM yyyy");
                DataSet.Tables["PrintStatement"].Rows[intRow]["STATEMENT_LINE_NO"] = intRow + 1;
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Print_AuditTrail(Int64 parint64CompanyNo, int parintStatementNumber)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new System.Data.DataSet();

            double dblBalance = 0;

            DateTime myDateTime = new DateTime(Convert.ToInt32(parintStatementNumber.ToString().Substring(0, 4)), Convert.ToInt32(parintStatementNumber.ToString().Substring(4)), 1).AddMonths(1).AddDays(-1);
            DateTime myDateFrom = new DateTime(Convert.ToInt32(parintStatementNumber.ToString().Substring(0, 4)), Convert.ToInt32(parintStatementNumber.ToString().Substring(4)), 1);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine("'Validite (Pty) Ltd' AS OUR_COMPANY_DESC");
            strQry.AppendLine(",'272 Oak Avenue' AS OUR_COMPANY_ADDRESS_LINE1");
            strQry.AppendLine(",'Ferndale, Randburg' AS OUR_COMPANY_ADDRESS_LINE2");
            strQry.AppendLine(",'2194' AS OUR_COMPANY_ADDRESS_LINE3");
            //strQry.AppendLine(",'VAT Number 1234567890' AS OUR_COMPANY_ADDRESS_LINE4");
            strQry.AppendLine(",'' AS OUR_COMPANY_ADDRESS_LINE4");
            strQry.AppendLine(",SHH.CUSTOMER_DESC");
            strQry.AppendLine(",SHH.STATEMENT_DATE");
            strQry.AppendLine(",SHH.CUSTOMER_DESC AS AUDIT_FROM_DATE");
            strQry.AppendLine(",SHH.CUSTOMER_DESC AS AUDIT_TO_DATE");
            strQry.AppendLine(",LAST_STATEMENT.MAX_STATEMENT_DATE");
            strQry.AppendLine(",SHH.VAT_NUMBER");
            strQry.AppendLine(",SHH.CONTACT_PERSON");
            strQry.AppendLine(",SHH.CONTACT_PHONE");
            strQry.AppendLine(",SHH.CONTACT_EMAIL");
            strQry.AppendLine(",SHH.ADDRESS_LINE1");
            strQry.AppendLine(",SHH.ADDRESS_LINE2");
            strQry.AppendLine(",SHH.ADDRESS_LINE3");
            strQry.AppendLine(",SHH.ADDRESS_LINE4");
            strQry.AppendLine(",SHH.ADDRESS_LINE5");
            strQry.AppendLine(",SHH.STATEMENT_OPEN_BALANCE");
            strQry.AppendLine(",SHH.STATEMENT_CLOSE_BALANCE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_HEADER_HISTORY SHH ");

            strQry.AppendLine(" LEFT JOIN");
            strQry.AppendLine("(SELECT ");
            strQry.AppendLine(" COMPANY_NO ");
            strQry.AppendLine(",MAX(STATEMENT_DATE)AS MAX_STATEMENT_DATE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_HEADER_HISTORY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(") AS LAST_STATEMENT ");
            strQry.AppendLine(" ON SHH.COMPANY_NO = LAST_STATEMENT.COMPANY_NO");
         
            strQry.AppendLine(" WHERE SHH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND SHH.STATEMENT_NUMBER = " + parintStatementNumber);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PrintAuditReport", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(parintStatementNumber + " AS STATEMENT_NUMBER ");
            strQry.AppendLine(",0 AS STATEMENT_LINE_NO");
            strQry.AppendLine(",'" + myDateTime.AddDays(1).AddMonths(-1).ToString("yyyy-MM-dd") + "' AS STATEMENT_LINE_DATE");
            strQry.AppendLine(",'Balance b/f' AS STATEMENT_LINE_DESC");
            strQry.AppendLine(",'' AS INVOICE_NUMBER");

            if (Convert.ToDouble(DataSet.Tables["PrintAuditReport"].Rows[0]["STATEMENT_OPEN_BALANCE"]) < 0)
            {
                double dblStatementOpenBalance = Convert.ToDouble(DataSet.Tables["PrintAuditReport"].Rows[0]["STATEMENT_OPEN_BALANCE"]) * -1;

                strQry.AppendLine(",0.00 AS STATEMENT_LINE_DR_TOTAL");
                strQry.AppendLine("," + dblStatementOpenBalance.ToString("#########0.00") + " AS STATEMENT_LINE_CR_TOTAL");
            }
            else
            {
                strQry.AppendLine("," + Convert.ToDouble(DataSet.Tables["PrintAuditReport"].Rows[0]["STATEMENT_OPEN_BALANCE"]).ToString("#########0.00") + " AS STATEMENT_LINE_DR_TOTAL");
                strQry.AppendLine(",0.00 AS STATEMENT_LINE_CR_TOTAL");
            }

            strQry.AppendLine("," + Convert.ToDouble(DataSet.Tables["PrintAuditReport"].Rows[0]["STATEMENT_OPEN_BALANCE"]).ToString("#########0.00") + " AS STATEMENT_BALANCE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" STATEMENT_NUMBER ");
            strQry.AppendLine(",STATEMENT_LINE_NO");
            strQry.AppendLine(",CONVERT(VARCHAR(10),STATEMENT_LINE_DATE,120) AS STATEMENT_LINE_DATE ");
            strQry.AppendLine(",STATEMENT_LINE_DESC");
            strQry.AppendLine(",CONVERT(VARCHAR,INVOICE_NUMBER) AS INVOICE_NUMBER");

            strQry.AppendLine(",STATEMENT_LINE_DR_TOTAL");
            strQry.AppendLine(",STATEMENT_LINE_CR_TOTAL");
            strQry.AppendLine(",STATEMENT_BALANCE = ");
            strQry.AppendLine(" CASE ");
            strQry.AppendLine(" WHEN STATEMENT_LINE_DR_TOTAL <> 0 ");

            strQry.AppendLine(" THEN STATEMENT_LINE_DR_TOTAL ");

            strQry.AppendLine(" ELSE STATEMENT_LINE_CR_TOTAL * -1 ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll.dbo.STATEMENT_ITEM_HISTORY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND STATEMENT_NUMBER >= " + parintStatementNumber);

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "StatementTemp", parint64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["StatementTemp"].Rows.Count; intRow++)
            {
                if (intRow == 0)
                {
                    //Make it String
                    DataSet.Tables["PrintAuditReport"].Columns.Add("STATEMENT_NUMBER", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_DESC"].DataType);

                    DataSet.Tables["PrintAuditReport"].Columns.Add("STATEMENT_LINE_NO", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_NO"].DataType);
                    DataSet.Tables["PrintAuditReport"].Columns.Add("STATEMENT_LINE_DESC", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_DESC"].DataType);
                    DataSet.Tables["PrintAuditReport"].Columns.Add("INVOICE_NUMBER", DataSet.Tables["StatementTemp"].Columns["INVOICE_NUMBER"].DataType);
                    //Make it String
                    DataSet.Tables["PrintAuditReport"].Columns.Add("STATEMENT_LINE_DATE", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_DESC"].DataType);

                    DataSet.Tables["PrintAuditReport"].Columns.Add("STATEMENT_LINE_DR_TOTAL", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_DR_TOTAL"].DataType);
                    DataSet.Tables["PrintAuditReport"].Columns.Add("STATEMENT_LINE_CR_TOTAL", DataSet.Tables["StatementTemp"].Columns["STATEMENT_LINE_CR_TOTAL"].DataType);
                    DataSet.Tables["PrintAuditReport"].Columns.Add("STATEMENT_BALANCE", DataSet.Tables["StatementTemp"].Columns["STATEMENT_BALANCE"].DataType);
                                              
                    DataSet.Tables["PrintAuditReport"].Rows[intRow]["AUDIT_FROM_DATE"] = myDateFrom.ToString("d MMMM yyyy");
                    DataSet.Tables["PrintAuditReport"].Rows[intRow]["AUDIT_TO_DATE"] = Convert.ToDateTime(DataSet.Tables["PrintAuditReport"].Rows[intRow]["MAX_STATEMENT_DATE"]).ToString("d MMMM yyyy");

                    if (Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"]) > 0)
                    {
                        dblBalance = Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"]);
                    }
                    else
                    {
                        dblBalance = Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"]) * -1;
                    }
                }
                else
                {
                    DataRow myDataRow = DataSet.Tables["PrintAuditReport"].NewRow();
                    DataSet.Tables["PrintAuditReport"].Rows.Add(myDataRow);

                    if (Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"]) > 0)
                    {
                        dblBalance += Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"]);
                    }
                    else
                    {
                        dblBalance += Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"]) * -1;
                    }
                }

                DataSet.Tables["PrintAuditReport"].Rows[intRow]["STATEMENT_NUMBER"] = myDateTime.ToString("d MMMM yyyy");
                DataSet.Tables["PrintAuditReport"].Rows[intRow]["STATEMENT_LINE_NO"] = intRow.ToString();
                DataSet.Tables["PrintAuditReport"].Rows[intRow]["STATEMENT_LINE_DESC"] = DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DESC"].ToString();

                if (DataSet.Tables["StatementTemp"].Rows[intRow]["INVOICE_NUMBER"].ToString() == "-1")
                {
                    DataSet.Tables["PrintAuditReport"].Rows[intRow]["INVOICE_NUMBER"] = "";
                }
                else
                {
                    DataSet.Tables["PrintAuditReport"].Rows[intRow]["INVOICE_NUMBER"] = DataSet.Tables["StatementTemp"].Rows[intRow]["INVOICE_NUMBER"].ToString();
                }

                DataSet.Tables["PrintAuditReport"].Rows[intRow]["STATEMENT_LINE_DATE"] = Convert.ToDateTime(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DATE"]).ToString("yyyy-MM-dd");

                DataSet.Tables["PrintAuditReport"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"] = Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"]);
                DataSet.Tables["PrintAuditReport"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"] = Convert.ToDouble(DataSet.Tables["StatementTemp"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"]);
                DataSet.Tables["PrintAuditReport"].Rows[intRow]["STATEMENT_BALANCE"] = dblBalance;
                //For Testing

                //DataSet.Tables["PrintAuditReport"].Rows[intRow]["STATEMENT_LINE_DR_TOTAL"] = 99999999.99;
                //DataSet.Tables["PrintAuditReport"].Rows[intRow]["STATEMENT_LINE_CR_TOTAL"] = 99999999.99;
                //DataSet.Tables["PrintAuditReport"].Rows[intRow]["STATEMENT_BALANCE"] = 99999999.99;
                //DataSet.Tables["PrintAuditReport"].Rows[intRow]["STATEMENT_CLOSE_BALANCE"] = 99999999.99;
            }

            //ELR - 2018-06-08
            if (DataSet.Tables["StatementTemp"].Rows.Count > 0)
            {
                DataSet.Tables["PrintAuditReport"].Rows[0]["STATEMENT_CLOSE_BALANCE"] = dblBalance;
            }
          
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
