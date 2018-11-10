using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busClientConvertCostCentre
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busClientConvertCostCentre()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records()
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT");
           
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",COMPANY_DESC");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Convert_Client_Records(Int64 parint64CompanyNo, string parstrToPayCategoryType, byte[] parbytCompressDataSet)
        {
            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbytCompressDataSet);
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" A.TABLE_NAME");
            
            strQry.AppendLine(" FROM InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS A ");
            
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.INFORMATION_SCHEMA.COLUMNS B");
            strQry.AppendLine(" ON B.TABLE_NAME = A.TABLE_NAME ");
            strQry.AppendLine(" AND B.COLUMN_NAME = 'PAY_CATEGORY_NO' ");

            strQry.AppendLine(" WHERE A.COLUMN_NAME = 'PAY_CATEGORY_TYPE'");
            
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "TablesToFix");
 
            for (int intRow = 0; intRow < DataSet.Tables["PayCategory"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" UPDATE UE ");

                strQry.AppendLine(" SET UE.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrToPayCategoryType));

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.USER_EMPLOYEE UE ");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
                
                strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                
                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(") AS JOIN_TABLE");
                
                strQry.AppendLine(" ON UE.COMPANY_NO = JOIN_TABLE.COMPANY_NO ");
                strQry.AppendLine(" AND UE.EMPLOYEE_NO = JOIN_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND UE.PAY_CATEGORY_TYPE = JOIN_TABLE.PAY_CATEGORY_TYPE");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();

                strQry.AppendLine(" UPDATE E ");

                strQry.AppendLine(" SET E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrToPayCategoryType));

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                
                strQry.AppendLine(") AS JOIN_TABLE");
                
                strQry.AppendLine(" ON E.COMPANY_NO = JOIN_TABLE.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = JOIN_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = JOIN_TABLE.PAY_CATEGORY_TYPE");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                //Break
                strQry.Clear();

                if (parstrToPayCategoryType == "W")
                {
                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT ");
                }
                else
                {
                    if (parstrToPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ");
                    }
                    else
                    {
                        //Time Attend
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                    }
                }

                strQry.AppendLine("(COMPANY_NO ");
                strQry.AppendLine(",EMPLOYEE_NO ");
                strQry.AppendLine(",PAY_CATEGORY_NO ");
                strQry.AppendLine(",BREAK_DATE ");
                strQry.AppendLine(",BREAK_SEQ ");
                strQry.AppendLine(",BREAK_TIME_IN_MINUTES ");
                strQry.AppendLine(",BREAK_TIME_OUT_MINUTES ");
                strQry.AppendLine(",CLOCKED_TIME_IN_MINUTES ");
                strQry.AppendLine(",CLOCKED_TIME_OUT_MINUTES) ");
                
                strQry.AppendLine(" SELECT ");
                
                strQry.AppendLine(" A.COMPANY_NO ");
                strQry.AppendLine(",A.EMPLOYEE_NO ");
                strQry.AppendLine(",A.PAY_CATEGORY_NO ");
                strQry.AppendLine(",A.BREAK_DATE ");
                strQry.AppendLine(",A.BREAK_SEQ ");
                strQry.AppendLine(",A.BREAK_TIME_IN_MINUTES ");
                strQry.AppendLine(",A.BREAK_TIME_OUT_MINUTES ");
                strQry.AppendLine(",A.CLOCKED_TIME_IN_MINUTES ");
                strQry.AppendLine(",A.CLOCKED_TIME_OUT_MINUTES ");

                if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT A");
                }
                else
                {
                    if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                    {
                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT A");
                    }
                    else
                    {
                        //Time Attend
                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT A");
                    }
                }

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(") AS JOIN_TABLE");

                strQry.AppendLine(" ON A.COMPANY_NO = JOIN_TABLE.COMPANY_NO ");
                strQry.AppendLine(" AND A.EMPLOYEE_NO = JOIN_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND A.PAY_CATEGORY_NO = JOIN_TABLE.PAY_CATEGORY_NO");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();

                strQry.AppendLine(" DELETE A ");

                if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_BREAK_CURRENT A ");
                }
                else
                {
                    if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                    {
                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_BREAK_CURRENT A ");
                    }
                    else
                    {
                        //Time Attend
                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT A ");
                    }
                }

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(") AS JOIN_TABLE");

                strQry.AppendLine(" ON A.COMPANY_NO = JOIN_TABLE.COMPANY_NO ");
                strQry.AppendLine(" AND A.EMPLOYEE_NO = JOIN_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND A.PAY_CATEGORY_NO = JOIN_TABLE.PAY_CATEGORY_NO");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                
                //Timesheets
                strQry.Clear();

                if (parstrToPayCategoryType == "W")
                {
                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
                }
                else
                {
                    if (parstrToPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ");
                    }
                    else
                    {
                        //Time Attend
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                    }
                }

                strQry.AppendLine("(COMPANY_NO ");
                strQry.AppendLine(",EMPLOYEE_NO ");
                strQry.AppendLine(",PAY_CATEGORY_NO ");
                strQry.AppendLine(",TIMESHEET_DATE ");
                strQry.AppendLine(",TIMESHEET_SEQ ");
                strQry.AppendLine(",TIMESHEET_TIME_IN_MINUTES ");
                strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES ");
                strQry.AppendLine(",CLOCKED_TIME_IN_MINUTES ");
                strQry.AppendLine(",CLOCKED_TIME_OUT_MINUTES) ");
                
                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(" A.COMPANY_NO ");
                strQry.AppendLine(",A.EMPLOYEE_NO ");
                strQry.AppendLine(",A.PAY_CATEGORY_NO ");
                strQry.AppendLine(",A.TIMESHEET_DATE ");
                strQry.AppendLine(",A.TIMESHEET_SEQ ");
                strQry.AppendLine(",A.TIMESHEET_TIME_IN_MINUTES ");
                strQry.AppendLine(",A.TIMESHEET_TIME_OUT_MINUTES ");
                strQry.AppendLine(",A.CLOCKED_TIME_IN_MINUTES ");
                strQry.AppendLine(",A.CLOCKED_TIME_OUT_MINUTES ");

                if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT A ");
                }
                else
                {
                    if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                    {
                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT A ");
                    }
                    else
                    {
                        //Time Attend
                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT A ");
                    }
                }

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(") AS JOIN_TABLE");

                strQry.AppendLine(" ON A.COMPANY_NO = JOIN_TABLE.COMPANY_NO ");
                strQry.AppendLine(" AND A.EMPLOYEE_NO = JOIN_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND A.PAY_CATEGORY_NO = JOIN_TABLE.PAY_CATEGORY_NO");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                strQry.Clear();

                strQry.AppendLine(" DELETE A ");

                if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                {
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIMESHEET_CURRENT A ");
                }
                else
                {
                    if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                    {
                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT A ");
                    }
                    else
                    {
                        //Time Attend
                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT A ");
                    }
                }

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");

                strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(") AS JOIN_TABLE");

                strQry.AppendLine(" ON A.COMPANY_NO = JOIN_TABLE.COMPANY_NO ");
                strQry.AppendLine(" AND A.EMPLOYEE_NO = JOIN_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND A.PAY_CATEGORY_NO = JOIN_TABLE.PAY_CATEGORY_NO");

                clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

                for (int intTableRow = 0; intTableRow < DataSet.Tables["TablesToFix"].Rows.Count; intTableRow++)
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE A ");

                    strQry.AppendLine(" SET A.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrToPayCategoryType));

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo." + DataSet.Tables["TablesToFix"].Rows[intTableRow]["TABLE_NAME"].ToString() + " A ");

                    strQry.AppendLine(" WHERE A.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND A.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND A.PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
                }
            }
        }
        
        public byte[] Get_Company_CostCentres(Int64 parint64CompanyNo, string parstrToPayCategoryType)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY PC ");

            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");

            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE <> " + clsDBConnectionObjects.Text2DynamicSQL(parstrToPayCategoryType));

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
