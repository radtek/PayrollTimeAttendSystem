using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busConvertCostCentre
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busConvertCostCentre()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public int Check_Run(Int64 parint64CompanyNo, string parstrToPayCategoryType, byte[] parbytCompressDataSet)
        {
            int intReturnCode = 0;

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbytCompressDataSet);
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND ISNULL(ALLOW_COST_CENTRE_CONVERT_IND,0) = 1");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "AllowConvertCheck", parint64CompanyNo);

            if (DataSet.Tables["AllowConvertCheck"].Rows.Count == 0)
            {
                intReturnCode = 9;
            }
            else
            {
                //Check That there is No Current Payroll Run on the Go
                for (int intRow = 0; intRow < DataSet.Tables["PayCategory"].Rows.Count; intRow++)
                {
                    if (DataSet.Tables["RunCheck"] != null)
                    {
                        DataSet.Tables.Remove("RunCheck");
                    }

                    strQry.Clear();

                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND RUN_TYPE = 'P' ");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "RunCheck", parint64CompanyNo);

                    if (DataSet.Tables["RunCheck"].Rows.Count > 0)
                    {
                        intReturnCode = 1;

                        break;
                    }
                }
            }

            DataSet.Dispose();
            DataSet = null;

            return intReturnCode;
        }

        public int Convert_Records(Int64 parint64CompanyNo, string parstrToPayCategoryType, byte[] parbytCompressDataSet)
        {
            int intReturnCode = 0;

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbytCompressDataSet);
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" A.TABLE_NAME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS A ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS B");
            strQry.AppendLine(" ON B.TABLE_NAME = A.TABLE_NAME ");
            strQry.AppendLine(" AND B.COLUMN_NAME = 'PAY_CATEGORY_NO' ");

            strQry.AppendLine(" WHERE A.COLUMN_NAME = 'PAY_CATEGORY_TYPE'");
            strQry.AppendLine(" AND NOT A.TABLE_NAME LIKE '%_TEMP%'");
            strQry.AppendLine(" AND NOT A.TABLE_NAME LIKE '%PRINT%'");
            strQry.AppendLine(" AND NOT A.TABLE_NAME LIKE '%CLOCK_TIME%'");
            strQry.AppendLine(" AND NOT A.TABLE_NAME = 'SDL_REPORT'");
            strQry.AppendLine(" AND NOT A.TABLE_NAME LIKE 'EMPLOYEE_EARNING_%'");
            strQry.AppendLine(" AND NOT A.TABLE_NAME LIKE '%_HISTORY'");
            strQry.AppendLine(" AND NOT A.TABLE_NAME LIKE '%_CURRENT'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TablesToFix", parint64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["PayCategory"].Rows.Count; intRow++)
            {
                //Loans
                strQry.Clear();

                strQry.AppendLine(" UPDATE LN ");

                strQry.AppendLine(" SET LN.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrToPayCategoryType));

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LOANS LN ");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" L.COMPANY_NO ");
                strQry.AppendLine(",E.EMPLOYEE_NO ");
                strQry.AppendLine(",L.DEDUCTION_NO ");
                strQry.AppendLine(",L.DEDUCTION_SUB_ACCOUNT_NO ");
                strQry.AppendLine(",L.LOAN_REC_NO");
                strQry.AppendLine(",L.PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LOANS L ");
                strQry.AppendLine(" ON E.COMPANY_NO = L.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = L.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(") AS JOIN_TABLE");

                strQry.AppendLine(" ON E.COMPANY_NO = JOIN_TABLE.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = JOIN_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = JOIN_TABLE.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(") AS NEW_TABLE");

                strQry.AppendLine(" ON NEW_TABLE.COMPANY_NO = LN.COMPANY_NO ");
                strQry.AppendLine(" AND NEW_TABLE.EMPLOYEE_NO = LN.EMPLOYEE_NO ");
                strQry.AppendLine(" AND NEW_TABLE.DEDUCTION_NO = LN.DEDUCTION_NO ");
                strQry.AppendLine(" AND NEW_TABLE.DEDUCTION_SUB_ACCOUNT_NO = LN.DEDUCTION_SUB_ACCOUNT_NO ");
                strQry.AppendLine(" AND NEW_TABLE.LOAN_REC_NO = LN.LOAN_REC_NO ");
                strQry.AppendLine(" AND NEW_TABLE.PAY_CATEGORY_TYPE = LN.PAY_CATEGORY_TYPE ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                //Leave Current
                strQry.Clear();

                strQry.AppendLine(" UPDATE LCN ");

                strQry.AppendLine(" SET LCN.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrToPayCategoryType));

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LCN ");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" LC.COMPANY_NO ");
                strQry.AppendLine(",E.EMPLOYEE_NO ");
                strQry.AppendLine(",LC.EARNING_NO ");
                strQry.AppendLine(",LC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",LC.LEAVE_REC_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC ");
                strQry.AppendLine(" ON E.COMPANY_NO = LC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = LC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(") AS JOIN_TABLE");

                strQry.AppendLine(" ON E.COMPANY_NO = JOIN_TABLE.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = JOIN_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = JOIN_TABLE.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(") AS NEW_TABLE");

                strQry.AppendLine(" ON NEW_TABLE.COMPANY_NO = LCN.COMPANY_NO ");
                strQry.AppendLine(" AND NEW_TABLE.EMPLOYEE_NO = LCN.EMPLOYEE_NO ");
                strQry.AppendLine(" AND NEW_TABLE.EARNING_NO = LCN.EARNING_NO ");
                strQry.AppendLine(" AND NEW_TABLE.PAY_CATEGORY_TYPE = LCN.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND NEW_TABLE.LEAVE_REC_NO = LCN.LEAVE_REC_NO ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                //Employee Earnings
                strQry.Clear();

                strQry.AppendLine(" UPDATE EEN ");

                strQry.AppendLine(" SET EEN.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrToPayCategoryType));

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EEN ");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EE.COMPANY_NO ");
                strQry.AppendLine(",E.EMPLOYEE_NO ");
                strQry.AppendLine(",EE.EARNING_NO ");
                strQry.AppendLine(",EE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EE.TIE_BREAKER");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE ");
                strQry.AppendLine(" ON E.COMPANY_NO = EE.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EE.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(") AS JOIN_TABLE");

                strQry.AppendLine(" ON E.COMPANY_NO = JOIN_TABLE.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = JOIN_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = JOIN_TABLE.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(") AS NEW_TABLE");

                strQry.AppendLine(" ON NEW_TABLE.COMPANY_NO = EEN.COMPANY_NO ");
                strQry.AppendLine(" AND NEW_TABLE.EMPLOYEE_NO = EEN.EMPLOYEE_NO ");
                strQry.AppendLine(" AND NEW_TABLE.EARNING_NO = EEN.EARNING_NO ");
                strQry.AppendLine(" AND NEW_TABLE.PAY_CATEGORY_TYPE = EEN.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND NEW_TABLE.TIE_BREAKER = EEN.TIE_BREAKER ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                
                //Employee Deductions
                strQry.Clear();

                strQry.AppendLine(" UPDATE EDN ");

                strQry.AppendLine(" SET EDN.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrToPayCategoryType));

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION EDN ");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ED.COMPANY_NO ");
                strQry.AppendLine(",E.EMPLOYEE_NO ");
                strQry.AppendLine(",ED.DEDUCTION_NO ");
                strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO ");
                strQry.AppendLine(",ED.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",ED.TIE_BREAKER");
                   
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED ");
                strQry.AppendLine(" ON E.COMPANY_NO = ED.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = ED.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(") AS JOIN_TABLE");

                strQry.AppendLine(" ON E.COMPANY_NO = JOIN_TABLE.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = JOIN_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = JOIN_TABLE.PAY_CATEGORY_TYPE ");
                    
                strQry.AppendLine(") AS NEW_TABLE");

                strQry.AppendLine(" ON NEW_TABLE.COMPANY_NO = EDN.COMPANY_NO ");
                strQry.AppendLine(" AND NEW_TABLE.EMPLOYEE_NO = EDN.EMPLOYEE_NO ");
                strQry.AppendLine(" AND NEW_TABLE.DEDUCTION_NO = EDN.DEDUCTION_NO ");
                strQry.AppendLine(" AND NEW_TABLE.DEDUCTION_SUB_ACCOUNT_NO = EDN.DEDUCTION_SUB_ACCOUNT_NO ");
                strQry.AppendLine(" AND NEW_TABLE.PAY_CATEGORY_TYPE = EDN.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND NEW_TABLE.TIE_BREAKER = EDN.TIE_BREAKER ");
                 
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" UPDATE EDEPN ");

                strQry.AppendLine(" SET EDEPN.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrToPayCategoryType));

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE EDEPN ");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EDEP.COMPANY_NO ");
                strQry.AppendLine(",E.EMPLOYEE_NO ");
                strQry.AppendLine(",EDEP.DEDUCTION_NO ");
                strQry.AppendLine(",EDEP.DEDUCTION_SUB_ACCOUNT_NO ");
                strQry.AppendLine(",EDEP.EARNING_NO ");
                strQry.AppendLine(",EDEP.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EDEP.TIE_BREAKER");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE EDEP ");
                strQry.AppendLine(" ON E.COMPANY_NO = EDEP.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EDEP.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EDEP.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
                strQry.AppendLine(" ON PC.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                strQry.AppendLine(") AS JOIN_TABLE");

                strQry.AppendLine(" ON E.COMPANY_NO = JOIN_TABLE.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = JOIN_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = JOIN_TABLE.PAY_CATEGORY_TYPE ");

                strQry.AppendLine(") AS NEW_TABLE");

                strQry.AppendLine(" ON NEW_TABLE.COMPANY_NO = EDEPN.COMPANY_NO ");
                strQry.AppendLine(" AND NEW_TABLE.EMPLOYEE_NO = EDEPN.EMPLOYEE_NO ");
                strQry.AppendLine(" AND NEW_TABLE.DEDUCTION_NO = EDEPN.DEDUCTION_NO ");
                strQry.AppendLine(" AND NEW_TABLE.DEDUCTION_SUB_ACCOUNT_NO = EDEPN.DEDUCTION_SUB_ACCOUNT_NO ");
                strQry.AppendLine(" AND NEW_TABLE.EARNING_NO = EDEPN.EARNING_NO ");
                strQry.AppendLine(" AND NEW_TABLE.PAY_CATEGORY_TYPE = EDEPN.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND NEW_TABLE.TIE_BREAKER = EDEPN.TIE_BREAKER ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
               
                strQry.Clear();

                strQry.AppendLine(" UPDATE UE ");

                strQry.AppendLine(" SET UE.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrToPayCategoryType));

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE UE ");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
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

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" UPDATE E ");

                strQry.AppendLine(" SET E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrToPayCategoryType));

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
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

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                //Break
                strQry.Clear();

                if (parstrToPayCategoryType == "W")
                {
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");
                }
                else
                {
                    if (parstrToPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ");
                    }
                    else
                    {
                        //Time Attend
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
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
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT A");
                }
                else
                {
                    if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT A");
                    }
                    else
                    {
                        //Time Attend
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT A");
                    }
                }

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
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

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" DELETE A ");

                if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT A ");
                }
                else
                {
                    if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT A ");
                    }
                    else
                    {
                        //Time Attend
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT A ");
                    }
                }

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
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

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                //Timesheets
                strQry.Clear();

                if (parstrToPayCategoryType == "W")
                {
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
                }
                else
                {
                    if (parstrToPayCategoryType == "S")
                    {
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ");
                    }
                    else
                    {
                        //Time Attend
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
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
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT A ");
                }
                else
                {
                    if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT A ");
                    }
                    else
                    {
                        //Time Attend
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT A ");
                    }
                }

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
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

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" DELETE A ");

                if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "W")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT A ");
                }
                else
                {
                    if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT A ");
                    }
                    else
                    {
                        //Time Attend
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT A ");
                    }
                }

                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
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

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                for (int intTableRow = 0; intTableRow < DataSet.Tables["TablesToFix"].Rows.Count; intTableRow++)
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE A ");

                    strQry.AppendLine(" SET A.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrToPayCategoryType));

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo." + DataSet.Tables["TablesToFix"].Rows[intTableRow]["TABLE_NAME"].ToString() + " A ");

                    strQry.AppendLine(" WHERE A.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND A.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND A.PAY_CATEGORY_NO = " + DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }
            }

            Convert_Records_Continue:

            DataSet.Dispose();
            DataSet = null;

            return intReturnCode;
        }
    }
}
