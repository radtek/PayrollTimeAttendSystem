using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busEasyFile
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busEasyFile()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo, string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" BANK_NO");
            strQry.AppendLine(",BANK_DESC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.BANK ");

            strQry.AppendLine(" WHERE BANK_NO <> 1");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" BANK_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Bank", -1);

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" C.COMPANY_DESC");

            strQry.AppendLine(",C.RES_UNIT_NUMBER");
            strQry.AppendLine(",C.RES_COMPLEX");
            strQry.AppendLine(",C.RES_STREET_NUMBER");
            strQry.AppendLine(",C.RES_STREET_NAME");

            strQry.AppendLine(",C.RES_SUBURB");
            strQry.AppendLine(",C.RES_CITY");
            strQry.AppendLine(",C.RES_ADDR_CODE");
            strQry.AppendLine(",C.RES_ADDR_PROVINCE_NO");

            strQry.AppendLine(",C.TEL_WORK");

            strQry.AppendLine(",C.EFILING_NAMES");
            strQry.AppendLine(",C.EFILING_CONTACT_NO");
            strQry.AppendLine(",C.EFILING_EMAIL");

            strQry.AppendLine(",C.TRADE_CLASSIFICATION_CODE");

            //ELR - 2014-08-27
            strQry.AppendLine(",C.SIC7_GROUP_CODE");

            strQry.AppendLine(",PAYE_NO = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT C.PAYE_REF_NO IS NULL ");
            strQry.AppendLine(" THEN C.PAYE_REF_NO ");

            strQry.AppendLine(" ELSE C.TAX_REF_NO ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",C.UIF_REF_NO");
            strQry.AppendLine(",C.SDL_REF_NO");
 
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");
           
            if (parstrCurrentUserAccess == "A")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                strQry.AppendLine(" ON UCA.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND C.COMPANY_NO = UCA.COMPANY_NO ");
                strQry.AppendLine(" AND UCA.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A' ");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
            }

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parInt64CompanyNo);

            //ELR - 2014-08-27
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" SIC7_GROUP_NO");
            strQry.AppendLine(",SIC7_GROUP_DESC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.SIC7_GROUP ");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" SIC7_GROUP_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Sic7CodeGroup", -1);

            //ELR - 2014-08-27
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" SIC7_GROUP_NO");
            strQry.AppendLine(",SIC7_GROUP_CODE");
            strQry.AppendLine(",SIC7_GROUP_CODE_DESC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.SIC7_GROUP_CODE_DESC ");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" SIC7_GROUP_NO");
            strQry.AppendLine(",SIC7_GROUP_CODE_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Sic7Code", -1);
           
            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" SDL_LEVY");

            strQry.AppendLine(" FROM InteractPayroll.dbo.SDL_LEVY");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "SDL", parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" BANK_ACCOUNT_RELATIONSHIP_TYPE_NO");
            strQry.AppendLine(",BANK_ACCOUNT_RELATIONSHIP_TYPE_DESC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.BANK_ACCOUNT_RELATIONSHIP_TYPE");

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" BANK_ACCOUNT_RELATIONSHIP_TYPE_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "BankRelationshipType", parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" BANK_ACCOUNT_TYPE_NO");
            strQry.AppendLine(",BANK_ACCOUNT_TYPE_DESC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.BANK_ACCOUNT_TYPE");

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" BANK_ACCOUNT_TYPE_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "BankAccountType", parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" NATURE_PERSON_NO");
            strQry.AppendLine(",NATURE_PERSON_ID");
            strQry.AppendLine(",NATURE_PERSON_DESC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.NATURE_PERSON");

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" NATURE_PERSON_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "NaturePerson", parInt64CompanyNo);

            DateTime dtPeriod;

            if (DateTime.Now.Month > 6)
            {
                dtPeriod = new DateTime(DateTime.Now.Year, 9, 1).AddDays(-1);
            }
            else
            {
                dtPeriod = new DateTime(DateTime.Now.Year, 3, 1).AddDays(-1);
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EFILING_NO ");
            strQry.AppendLine(",EFILING_PERIOD ");
            strQry.AppendLine(",EFILING_COMPANY_CHECK_USER_NO ");
            strQry.AppendLine(",EFILING_CLOSED_IND ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EFILING ");
            
            strQry.AppendLine(" WHERE EFILING_PERIOD = '" + dtPeriod.ToString("yyyy-MM-dd") + "'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Period", parInt64CompanyNo);

            DateTime dtBeginFinancialYear;
            DateTime dtEndFinancialYear;
            DateTime dtEasyFileEndYear;
            string strDateFilter = "";

            int intTaxYearEnd = - 1;

            if (DataSet.Tables["Period"].Rows.Count > 0)
            {
                if (Convert.ToDateTime(DataSet.Tables["Period"].Rows[0]["EFILING_PERIOD"]).Month > 2)
                {
                    dtBeginFinancialYear = new DateTime(Convert.ToDateTime(DataSet.Tables["Period"].Rows[0]["EFILING_PERIOD"]).Year, 3, 1);
                }
                else
                {
                    dtBeginFinancialYear = new DateTime(Convert.ToDateTime(DataSet.Tables["Period"].Rows[0]["EFILING_PERIOD"]).Year - 1, 3, 1);
                }

                //Last Day Of Fiscal Year
                dtEndFinancialYear = dtBeginFinancialYear.AddYears(1).AddDays(-1);
                intTaxYearEnd = dtEndFinancialYear.Year;

                strDateFilter = "";

                strDateFilter += " AND XXX.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "'";

                //20xx-08-30
                if (Convert.ToDateTime(DataSet.Tables["Period"].Rows[0]["EFILING_PERIOD"]).Month == 8)
                {
                    strDateFilter += " AND XXX.PAY_PERIOD_DATE <= '" + Convert.ToDateTime(DataSet.Tables["Period"].Rows[0]["EFILING_PERIOD"]).ToString("yyyy-MM-dd") + "'";
                    dtEasyFileEndYear = Convert.ToDateTime(DataSet.Tables["Period"].Rows[0]["EFILING_PERIOD"]); 
                }
                else
                {
                    strDateFilter += " AND XXX.PAY_PERIOD_DATE <= '" + dtEndFinancialYear.ToString("yyyy-MM-dd") + "'";
                    dtEasyFileEndYear = dtEndFinancialYear;
                }

                //2017-04-15 Fix IRP5 Code for Leave (Have No Entry field for IRP5 Code)
                strQry.Clear();

                strQry.AppendLine(" UPDATE EARN");

                strQry.AppendLine(" SET EARN.IRP5_CODE = 3601");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EARN");
                strQry.AppendLine(" ON C.COMPANY_NO = EARN.COMPANY_NO");
                //Leave
                strQry.AppendLine(" AND EARN.EARNING_NO >= 200");
                strQry.AppendLine(" AND EARN.IRP5_CODE IS NULL");

                strQry.AppendLine(" WHERE C.COMPANY_NO = " + parInt64CompanyNo);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                strQry.Clear();
                
                strQry.AppendLine(" SELECT DISTINCT ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",E.EMPLOYEE_INITIALS");
                strQry.AppendLine(",E.EMPLOYEE_ID_NO");
                strQry.AppendLine(",E.EMPLOYEE_PASSPORT_NO");
                strQry.AppendLine(",E.EMPLOYEE_PASSPORT_COUNTRY_CODE");

                strQry.AppendLine(",E.EMPLOYEE_RES_UNIT_NUMBER");
                strQry.AppendLine(",E.EMPLOYEE_RES_COMPLEX");
                strQry.AppendLine(",E.EMPLOYEE_RES_STREET_NUMBER");
                strQry.AppendLine(",E.EMPLOYEE_RES_STREET_NAME");
                strQry.AppendLine(",E.EMPLOYEE_RES_SUBURB");
                strQry.AppendLine(",E.EMPLOYEE_RES_CITY");
                strQry.AppendLine(",E.EMPLOYEE_RES_CODE");
                strQry.AppendLine(",E.EMPLOYEE_RES_COUNTRY_CODE2");

                //strQry.AppendLine(",E.EMPLOYEE_POST_ADDR_LINE1");
                //strQry.AppendLine(",E.EMPLOYEE_POST_ADDR_LINE2");
                //strQry.AppendLine(",E.EMPLOYEE_POST_ADDR_LINE3");

                //ELR - 20141014
                strQry.AppendLine(",E.EMPLOYEE_POST_UNIT_NUMBER");
                strQry.AppendLine(",E.EMPLOYEE_POST_COMPLEX");
                strQry.AppendLine(",E.EMPLOYEE_POST_STREET_NUMBER");
                strQry.AppendLine(",E.EMPLOYEE_POST_STREET_NAME");
                strQry.AppendLine(",E.EMPLOYEE_POST_SUBURB");
                strQry.AppendLine(",E.EMPLOYEE_POST_CITY");
                strQry.AppendLine(",E.EMPLOYEE_POST_COUNTRY_CODE2");
                strQry.AppendLine(",E.EMPLOYEE_POST_OPTION_IND");
                
                strQry.AppendLine(",E.EMPLOYEE_POST_CODE");
                strQry.AppendLine(",E.EMPLOYEE_TEL_HOME");
                strQry.AppendLine(",E.EMPLOYEE_TEL_WORK");
                strQry.AppendLine(",E.EMPLOYEE_TEL_CELL");
                strQry.AppendLine(",E.EMPLOYEE_BIRTHDATE");
                strQry.AppendLine(",E.EMPLOYEE_TAX_STARTDATE");
                strQry.AppendLine(",E.EMPLOYEE_ENDDATE");

                strQry.AppendLine(",E.TAX_DIRECTIVE_NO1");
                strQry.AppendLine(",E.TAX_DIRECTIVE_NO2");
                strQry.AppendLine(",E.TAX_DIRECTIVE_NO3");
                strQry.AppendLine(",E.TAX_DIRECTIVE_PERCENTAGE");
                strQry.AppendLine(",E.EMPLOYEE_EMAIL");
                strQry.AppendLine(",E.EMPLOYEE_TAX_NO");
                strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
                strQry.AppendLine(",E.NATURE_PERSON_NO");
                strQry.AppendLine(",E.BANK_ACCOUNT_TYPE_NO");
                strQry.AppendLine(",E.BANK_ACCOUNT_RELATIONSHIP_TYPE_NO");

                strQry.AppendLine(",E.BRANCH_CODE");

                //ELR - 20150228
                strQry.AppendLine(",E.BRANCH_DESC");
                strQry.AppendLine(",E.ACCOUNT_NO");
                strQry.AppendLine(",E.BANK_NO");

                strQry.AppendLine(",E.ACCOUNT_NAME");
                strQry.AppendLine(",E.TAX_TYPE_IND");

                strQry.AppendLine(",E.USE_RES_ADDR_COMPANY_IND");
                strQry.AppendLine(",E.USE_WORK_TEL_IND");
                strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

                //2014-08-27
                strQry.AppendLine(",E.SIC7_GROUP_CODE");

                strQry.AppendLine(",'' AS OK_IND");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                //Get PAY_CATEGORY_NO For Use of Cost Centre Address
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y' ");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH");
                strQry.AppendLine(" ON E.COMPANY_NO = EIH.COMPANY_NO ");

                strQry.Append(strDateFilter.Replace("XXX","EIH"));
                
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EIH.EMPLOYEE_NO ");
               
                if (parstrCurrentUserAccess == "A")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                    strQry.AppendLine(" ON UCA.USER_NO = " + parint64CurrentUserNo);
                    strQry.AppendLine(" AND E.COMPANY_NO = UCA.COMPANY_NO ");
                    strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A' ");
                    strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
                }

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);

                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);
                
                strQry.Clear();

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" EEH.EMPLOYEE_NO");
                strQry.AppendLine(",E.IRP5_CODE");
                //Drop Decimals
                strQry.AppendLine(",ROUND(SUM(EEH.TOTAL),0) AS SUMMED_TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING E ");
                strQry.AppendLine(" ON EEH.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EEH.EARNING_NO = E.EARNING_NO");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE EEH.COMPANY_NO = " + parInt64CompanyNo);

                strQry.Append(strDateFilter.Replace("XXX", "EEH"));

                strQry.AppendLine(" GROUP BY");
                strQry.AppendLine(" EEH.EMPLOYEE_NO");
                strQry.AppendLine(",E.IRP5_CODE");

                strQry.AppendLine(" HAVING ROUND(SUM(EEH.TOTAL),0) <> 0");

                strQry.AppendLine(" ORDER BY");
                strQry.AppendLine(" EEH.EMPLOYEE_NO");
                strQry.AppendLine(",E.IRP5_CODE");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeEarning", parInt64CompanyNo);
                
                //Earnings < 0
                strQry.Clear();

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" EEH.EMPLOYEE_NO");
                strQry.AppendLine(",EEH.PAY_CATEGORY_TYPE");

                //Drop Decimals
                strQry.AppendLine(",ROUND(SUM(EEH.TOTAL),0) AS SUMMED_TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING E ");
                strQry.AppendLine(" ON EEH.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EEH.EARNING_NO = E.EARNING_NO");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE EEH.COMPANY_NO = " + parInt64CompanyNo);

                strQry.Append(strDateFilter.Replace("XXX", "EEH"));

                strQry.AppendLine(" GROUP BY");
                strQry.AppendLine(" EEH.EMPLOYEE_NO");
                strQry.AppendLine(",EEH.PAY_CATEGORY_TYPE");
              
                strQry.AppendLine(" HAVING ROUND(SUM(EEH.TOTAL),0) < 0");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeEarningLessZero", parInt64CompanyNo);

                DataView dvEmployee = null;

                //Remove Employee From easyFile
                for (int intRow = 0; intRow < DataSet.Tables["EmployeeEarningLessZero"].Rows.Count; intRow++)
                {
                    dvEmployee = null;
                    dvEmployee = new DataView(DataSet.Tables["Employee"],
                                             "PAY_CATEGORY_TYPE = '" + DataSet.Tables["EmployeeEarningLessZero"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "' AND EMPLOYEE_NO = " + DataSet.Tables["EmployeeEarningLessZero"].Rows[intRow]["EMPLOYEE_NO"].ToString(),
                                              "",
                                              DataViewRowState.CurrentRows);

                    dvEmployee[0].Delete();
                }

                if (DataSet.Tables["EmployeeEarningLessZero"].Rows.Count > 0)
                {
                    DataSet.AcceptChanges();
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" EDH.EMPLOYEE_NO");
                strQry.AppendLine(",D.IRP5_CODE");
                //Drop Decimals

                strQry.AppendLine(",ROUND(SUM(EDH.TOTAL),0) AS SUMMED_TOTAL");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D ");
                strQry.AppendLine(" ON EDH.COMPANY_NO = D.COMPANY_NO ");
                strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EDH.DEDUCTION_NO = D.DEDUCTION_NO");
                strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");
                //Only eFiling Records
                //4001=Current Pension Fund Contributions
                //4002=Arrear Pension Fund Contributions
                //4003=Current and Arrear Provident Fund Contributions
                //4005=Medical Aid Contributions
                //4006=Current Retirement Annuity Contributions
                //4007=Arrear (Re-Instated) Retirement Annuity Contributions
                //NB - Extra Codes Not here will have to be looked at carefully
                strQry.AppendLine(" AND D.IRP5_CODE IN ('4001','4002','4003','4005','4006','4007')");

                strQry.AppendLine(" WHERE EDH.COMPANY_NO = " + parInt64CompanyNo);

                strQry.Append(strDateFilter.Replace("XXX", "EDH"));

                strQry.AppendLine(" GROUP BY");
                strQry.AppendLine(" EDH.EMPLOYEE_NO");
                strQry.AppendLine(",D.IRP5_CODE");

                strQry.AppendLine(" HAVING ROUND(SUM(EDH.TOTAL),0) <> 0");

                strQry.AppendLine(" ORDER BY");
                strQry.AppendLine(" EDH.EMPLOYEE_NO");
                strQry.AppendLine(",D.IRP5_CODE");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeDeductionContribution", parInt64CompanyNo);

                DataView myMedicalAidDataView = new DataView(DataSet.Tables["EmployeeDeductionContribution"], "IRP5_CODE = '4005'", "", DataViewRowState.CurrentRows);

                string strEmployeeNos = "";

                for (int intRow = 0; intRow < myMedicalAidDataView.Count; intRow++)
                {
                    if (intRow == 0)
                    {
                        strEmployeeNos = "(" + myMedicalAidDataView[intRow]["EMPLOYEE_NO"].ToString();
                    }
                    else
                    {
                        strEmployeeNos += "," + myMedicalAidDataView[intRow]["EMPLOYEE_NO"].ToString();
                    }
                }

                if (strEmployeeNos != "")
                {
                    strEmployeeNos += ")";

                    //Used To Test 4116 - Tax Credits (Code is Used Lower Down)

#if(DEBUG)

                    strQry.Clear();

                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" E.EMPLOYEE_NO");
                    strQry.AppendLine(",E.EMPLOYEE_TAX_STARTDATE");
                    strQry.AppendLine(",E.NUMBER_MEDICAL_AID_DEPENDENTS");
                    strQry.AppendLine(",TMACA.MAJOR_AMOUNT");
                    strQry.AppendLine(",TMACA.MINOR_AMOUNT");

                    strQry.AppendLine(",'4116' AS IRP5_CODE");

                    strQry.AppendLine(",NEWSUMMED_TOTAL = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN E.NUMBER_MEDICAL_AID_DEPENDENTS > 1 THEN ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN E.EMPLOYEE_TAX_STARTDATE <= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "' THEN ");

                    strQry.AppendLine(" DATEDIFF (month,'" + dtBeginFinancialYear + "','" + dtEasyFileEndYear.AddDays(1).ToString("yyyy-MM-dd") + "') * ((2 * TMACA.MAJOR_AMOUNT) + ((E.NUMBER_MEDICAL_AID_DEPENDENTS - 2) * TMACA.MINOR_AMOUNT))");

                    strQry.AppendLine(" ELSE ");

                    strQry.AppendLine(" DATEDIFF (month,E.EMPLOYEE_TAX_STARTDATE,'" + dtEasyFileEndYear.AddDays(1).ToString("yyyy-MM-dd") + "') * ((2 * TMACA.MAJOR_AMOUNT) + ((E.NUMBER_MEDICAL_AID_DEPENDENTS - 2) * TMACA.MINOR_AMOUNT))");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" ELSE ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN E.EMPLOYEE_TAX_STARTDATE <= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "' THEN ");

                    strQry.AppendLine(" DATEDIFF (month,'" + dtBeginFinancialYear + "','" + dtEasyFileEndYear.AddDays(1).ToString("yyyy-MM-dd") + "') * ((E.NUMBER_MEDICAL_AID_DEPENDENTS + 1) * TMACA.MAJOR_AMOUNT) ");

                    strQry.AppendLine(" ELSE ");

                    strQry.AppendLine(" DATEDIFF (month,E.EMPLOYEE_TAX_STARTDATE,'" + dtEasyFileEndYear.AddDays(1).ToString("yyyy-MM-dd") + "') * ((E.NUMBER_MEDICAL_AID_DEPENDENTS + 1) * TMACA.MAJOR_AMOUNT) ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(",SUMMED_TOTAL = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN E.NUMBER_MEDICAL_AID_DEPENDENTS > 1 THEN ");

                    strQry.AppendLine(" (2 * TMACA.MAJOR_AMOUNT) + ((E.NUMBER_MEDICAL_AID_DEPENDENTS - 2) * TMACA.MINOR_AMOUNT) ");

                    strQry.AppendLine(" ELSE ");

                    strQry.AppendLine(" (E.NUMBER_MEDICAL_AID_DEPENDENTS + 1) * TMACA.MAJOR_AMOUNT ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(",TIME_PERIOD = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN E.EMPLOYEE_TAX_STARTDATE <= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "' THEN ");

                    strQry.AppendLine(" DATEDIFF (month,'" + dtBeginFinancialYear + "','" + dtEasyFileEndYear.AddDays(1).ToString("yyyy-MM-dd") + "') ");

                    strQry.AppendLine(" ELSE ");

                    strQry.AppendLine(" DATEDIFF (month,E.EMPLOYEE_TAX_STARTDATE,'" + dtEasyFileEndYear.AddDays(1).ToString("yyyy-MM-dd") + "') ");

                    strQry.AppendLine(" END ");
                    
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.TAX_MEDICAL_AID_CAPPED_AMOUNT TMACA ");
                    strQry.AppendLine(" ON TMACA.TAX_YEAR_END = " + intTaxYearEnd);

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_NO IN " + strEmployeeNos);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Test", parInt64CompanyNo);
#endif
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" EDH.EMPLOYEE_NO");
                strQry.AppendLine(",D.IRP5_CODE");

                strQry.AppendLine(",SUMMED_TOTAL = ");

                strQry.AppendLine(" CASE ");

                //UIF (Employer + Employees Portion)
                strQry.AppendLine(" WHEN D.IRP5_CODE = '4141' THEN ");
                strQry.AppendLine(" ROUND(SUM(EDH.TOTAL) * 2,0) ");

                //2 Decimals
                strQry.AppendLine(" ELSE ");
                strQry.AppendLine(" ROUND(SUM(EDH.TOTAL),2) ");

                strQry.AppendLine(" END");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D ");
                strQry.AppendLine(" ON EDH.COMPANY_NO = D.COMPANY_NO ");
                strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EDH.DEDUCTION_NO = D.DEDUCTION_NO");
                strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");
                //Only eFiling Records
                //4101=Site
                //4102=PAYE
                //4105=Tax on Lump Sum Benefit
                //4141 = UIF (Employee and Employer Portion)
                strQry.AppendLine(" AND D.IRP5_CODE IN ('4101','4102','4115','4141')");

                strQry.AppendLine(" WHERE EDH.COMPANY_NO = " + parInt64CompanyNo);

                strQry.Append(strDateFilter.Replace("XXX", "EDH"));

                strQry.AppendLine(" GROUP BY");
                strQry.AppendLine(" EDH.EMPLOYEE_NO");
                strQry.AppendLine(",D.IRP5_CODE");

                strQry.AppendLine(" HAVING ROUND(SUM(EDH.TOTAL),0) <> 0");

                //2017-04-27 - Fix to Create UIF Record where None has been Paid but Tax has been Paid
                strQry.AppendLine(" UNION");

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" ED.EMPLOYEE_NO");
                strQry.AppendLine(",D.IRP5_CODE");

                strQry.AppendLine(",SUMMED_TOTAL = 0");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION D ");
                strQry.AppendLine(" ON ED.COMPANY_NO = D.COMPANY_NO ");
                strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND ED.DEDUCTION_NO = D.DEDUCTION_NO");
                strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");
                //UIF
                strQry.AppendLine(" AND D.IRP5_CODE = '4141'");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH ");
                strQry.AppendLine(" ON ED.COMPANY_NO = EDH.COMPANY_NO ");
                strQry.AppendLine(" AND EDH.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "' AND EDH.PAY_PERIOD_DATE <= '" + dtEasyFileEndYear.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = EDH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND ED.EMPLOYEE_NO = EDH.EMPLOYEE_NO");
                strQry.AppendLine(" AND EDH.RUN_TYPE = 'P'");
                //Has Tax Deduction
                strQry.AppendLine(" AND EDH.DEDUCTION_NO = 1");
                
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH1 ");
                strQry.AppendLine(" ON ED.COMPANY_NO = EDH1.COMPANY_NO ");
                strQry.AppendLine(" AND EDH1.PAY_PERIOD_DATE >= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "' AND EDH1.PAY_PERIOD_DATE <= '" + dtEasyFileEndYear.ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = EDH1.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND ED.EMPLOYEE_NO = EDH1.EMPLOYEE_NO");
                strQry.AppendLine(" AND EDH1.RUN_TYPE = 'P'");
                strQry.AppendLine(" AND ED.DEDUCTION_NO = EDH1.DEDUCTION_NO");
                strQry.AppendLine(" AND ED.DEDUCTION_SUB_ACCOUNT_NO = EDH1.DEDUCTION_SUB_ACCOUNT_NO");

                strQry.AppendLine(" WHERE ED.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND ED.EMPLOYEE_NO > 0");
                //No UIF Deduction
                strQry.AppendLine(" AND EDH1.COMPANY_NO IS NULL");

                strQry.AppendLine(" GROUP BY");
                strQry.AppendLine(" ED.EMPLOYEE_NO");
                strQry.AppendLine(",D.IRP5_CODE");
                
                strQry.AppendLine(" ORDER BY");
                strQry.AppendLine(" 1");
                strQry.AppendLine(",2");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeDeductionTaxUif", parInt64CompanyNo);
                
                if (strEmployeeNos != "")
                {
                    //4116=Medical Fees Tax Credit (Dynamically Worked Out)
                    strQry.Clear();

                    strQry.AppendLine(" SELECT");
                    strQry.AppendLine(" E.EMPLOYEE_NO");

                    strQry.AppendLine(",'4116' AS IRP5_CODE");

                    strQry.AppendLine(",SUMMED_TOTAL = ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN E.NUMBER_MEDICAL_AID_DEPENDENTS > 1 THEN ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN E.EMPLOYEE_TAX_STARTDATE <= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "' THEN ");

                    strQry.AppendLine(" ROUND(DATEDIFF (month,'" + dtBeginFinancialYear + "','" + dtEasyFileEndYear.AddDays(1).ToString("yyyy-MM-dd") + "') * ((2 * TMACA.MAJOR_AMOUNT) + ((E.NUMBER_MEDICAL_AID_DEPENDENTS - 2) * TMACA.MINOR_AMOUNT)),2) ");

                    strQry.AppendLine(" ELSE ");

                    strQry.AppendLine(" ROUND(DATEDIFF (month,E.EMPLOYEE_TAX_STARTDATE,'" + dtEasyFileEndYear.AddDays(1).ToString("yyyy-MM-dd") + "') * ((2 * TMACA.MAJOR_AMOUNT) + ((E.NUMBER_MEDICAL_AID_DEPENDENTS - 2) * TMACA.MINOR_AMOUNT)),2) ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" ELSE ");

                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN E.EMPLOYEE_TAX_STARTDATE <= '" + dtBeginFinancialYear.ToString("yyyy-MM-dd") + "' THEN ");

                    strQry.AppendLine(" ROUND(DATEDIFF (month,'" + dtBeginFinancialYear + "','" + dtEasyFileEndYear.AddDays(1).ToString("yyyy-MM-dd") + "') * ((E.NUMBER_MEDICAL_AID_DEPENDENTS + 1) * TMACA.MAJOR_AMOUNT),2) ");

                    strQry.AppendLine(" ELSE ");

                    strQry.AppendLine(" ROUND(DATEDIFF (month,E.EMPLOYEE_TAX_STARTDATE,'" + dtEasyFileEndYear.AddDays(1).ToString("yyyy-MM-dd") + "') * ((E.NUMBER_MEDICAL_AID_DEPENDENTS + 1) * TMACA.MAJOR_AMOUNT),2) ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" END ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.TAX_MEDICAL_AID_CAPPED_AMOUNT TMACA ");
                    strQry.AppendLine(" ON TMACA.TAX_YEAR_END = " + intTaxYearEnd);

                    strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND E.EMPLOYEE_NO IN " + strEmployeeNos);
                    strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

                    //2016-09-15 - Hard Coded to Fix Hollow Bar Willimina Nevillin Who is Over 65
                    if (parInt64CompanyNo == 13)
                    {
                        strQry.AppendLine(" UNION ");
                        strQry.AppendLine(" SELECT");
                        strQry.AppendLine(" E.EMPLOYEE_NO");

                        //Additional Medical Expenses Tax Credit if employee ≥65 
                        strQry.AppendLine(",'4120' AS IRP5_CODE");

                        strQry.AppendLine(",SUMMED_TOTAL = 0 ");

                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                        strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.TAX_MEDICAL_AID_CAPPED_AMOUNT TMACA ");
                        strQry.AppendLine(" ON TMACA.TAX_YEAR_END = " + intTaxYearEnd);

                        strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
                        strQry.AppendLine(" AND E.EMPLOYEE_NO IN " + strEmployeeNos);
                        //Willimina Nevillin
                        strQry.AppendLine(" AND E.EMPLOYEE_NO = 15 ");

                        strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
                    }

                    clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeDeductionTaxCredit", parInt64CompanyNo);
               }

                strQry.Clear();

                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" PC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

                strQry.AppendLine(",PC.RES_UNIT_NUMBER");
                strQry.AppendLine(",PC.RES_COMPLEX");
                strQry.AppendLine(",PC.RES_STREET_NUMBER");

                strQry.AppendLine(",PC.RES_STREET_NAME");
                strQry.AppendLine(",PC.RES_SUBURB");
                strQry.AppendLine(",PC.RES_CITY");
                strQry.AppendLine(",PC.RES_ADDR_CODE");

                //2016-09-14 - Quick Fix for EasyFile
                strQry.AppendLine(",'ZA' AS RES_COUNTRY_CODE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
                strQry.AppendLine(" ON PC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND E.USE_RES_ADDR_COMPANY_IND = 'Y' ");
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                //Get PAY_CATEGORY_NO For Use of Cost Centre Address
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y' ");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH");
                strQry.AppendLine(" ON E.COMPANY_NO = EIH.COMPANY_NO ");

                strQry.Append(strDateFilter.Replace("XXX", "EIH"));

                strQry.AppendLine(" AND E.EMPLOYEE_NO = EIH.EMPLOYEE_NO ");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");

                strQry.AppendLine(" AND (NOT PC.RES_UNIT_NUMBER IS NULL");
                strQry.AppendLine(" OR NOT PC.RES_COMPLEX IS NULL");
                strQry.AppendLine(" OR NOT PC.RES_STREET_NUMBER IS NULL");

                strQry.AppendLine(" OR NOT PC.RES_STREET_NAME IS NULL");
                strQry.AppendLine(" OR NOT PC.RES_SUBURB IS NULL");
                strQry.AppendLine(" OR NOT PC.RES_CITY IS NULL");
                strQry.AppendLine(" OR NOT PC.RES_ADDR_CODE IS NULL)");

                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategoryAddress", parInt64CompanyNo);
            }
            else
            {
                //Empty Dataset 
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.COMPANY_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" WHERE E.COMPANY_NO = -1");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Update_Closed_Ind(Int64 parInt64CompanyNo, int parIntEfiningNo)
        {
            StringBuilder strQry = new StringBuilder();
          
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EFILING");
            strQry.AppendLine(" SET EFILING_CLOSED_IND = 'Y'");
            strQry.AppendLine(" WHERE EFILING_NO = " + parIntEfiningNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return 0;
        }

        public void Update_Record(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
          
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" EMPLOYEE_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_CODE"].ToString()));
            strQry.AppendLine(",EMPLOYEE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString()));
            strQry.AppendLine(",EMPLOYEE_SURNAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString()));
            strQry.AppendLine(",EMPLOYEE_INITIALS = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_INITIALS"].ToString()));

            strQry.AppendLine(",EMPLOYEE_ID_NO = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_ID_NO"].ToString()));
            strQry.AppendLine(",EMPLOYEE_PASSPORT_NO = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_PASSPORT_NO"].ToString()));
            strQry.AppendLine(",EMPLOYEE_PASSPORT_COUNTRY_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_PASSPORT_COUNTRY_CODE"].ToString()));

            strQry.AppendLine(",EMPLOYEE_RES_UNIT_NUMBER = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_UNIT_NUMBER"].ToString()));
            strQry.AppendLine(",EMPLOYEE_RES_COMPLEX = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_COMPLEX"].ToString()));
            strQry.AppendLine(",EMPLOYEE_RES_STREET_NUMBER = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_STREET_NUMBER"].ToString()));
            strQry.AppendLine(",EMPLOYEE_RES_STREET_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_STREET_NAME"].ToString()));
            strQry.AppendLine(",EMPLOYEE_RES_SUBURB = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_SUBURB"].ToString()));
            strQry.AppendLine(",EMPLOYEE_RES_CITY = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_CITY"].ToString()));

            strQry.AppendLine(",EMPLOYEE_RES_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_CODE"].ToString()));
            strQry.AppendLine(",EMPLOYEE_RES_COUNTRY_CODE2 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_COUNTRY_CODE2"].ToString()));
    
            //ELR - 20141014
            strQry.AppendLine(",EMPLOYEE_POST_OPTION_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_OPTION_IND"].ToString()));

            if (parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_OPTION_IND"].ToString() == "R")
            {
                strQry.AppendLine(",EMPLOYEE_POST_UNIT_NUMBER = NULL ");
                strQry.AppendLine(",EMPLOYEE_POST_COMPLEX = NULL ");
                strQry.AppendLine(",EMPLOYEE_POST_STREET_NUMBER = NULL ");
                strQry.AppendLine(",EMPLOYEE_POST_STREET_NAME = NULL ");
                strQry.AppendLine(",EMPLOYEE_POST_SUBURB = NULL ");
                strQry.AppendLine(",EMPLOYEE_POST_CITY = NULL ");
                strQry.AppendLine(",EMPLOYEE_POST_CODE = NULL ");
                strQry.AppendLine(",EMPLOYEE_POST_COUNTRY_CODE2 = NULL ");
            }
            else
            {
                if (parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_OPTION_IND"].ToString() == "S")
                {
                    strQry.AppendLine(",EMPLOYEE_POST_UNIT_NUMBER = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_UNIT_NUMBER"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_POST_COMPLEX = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_COMPLEX"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_POST_STREET_NUMBER = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_STREET_NUMBER"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_POST_STREET_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_STREET_NAME"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_POST_SUBURB = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_SUBURB"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_POST_CITY = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_CITY"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_POST_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_CODE"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_POST_COUNTRY_CODE2  = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_COUNTRY_CODE2"].ToString()));
                }
                else
                {
                    //P=PO Box
                    //B=Private Bag 
                    strQry.AppendLine(",EMPLOYEE_POST_UNIT_NUMBER = NULL");
                    strQry.AppendLine(",EMPLOYEE_POST_COMPLEX = NULL ");
                    strQry.AppendLine(",EMPLOYEE_POST_STREET_NUMBER = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_STREET_NUMBER"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_POST_STREET_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_STREET_NAME"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_POST_SUBURB = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_SUBURB"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_POST_CITY = NULL ");
                    strQry.AppendLine(",EMPLOYEE_POST_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_CODE"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_POST_COUNTRY_CODE2  = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_COUNTRY_CODE2"].ToString()));
                }
            }

            //strQry.AppendLine(",EMPLOYEE_POST_ADDR_LINE1 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_ADDR_LINE1"].ToString());
            //strQry.AppendLine(",EMPLOYEE_POST_ADDR_LINE2 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_ADDR_LINE2"].ToString());
            //strQry.AppendLine(",EMPLOYEE_POST_ADDR_LINE3 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_ADDR_LINE3"].ToString());
            //strQry.AppendLine(",EMPLOYEE_POST_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_CODE"].ToString());

            strQry.AppendLine(",EMPLOYEE_TEL_HOME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_HOME"].ToString()));
            strQry.AppendLine(",EMPLOYEE_TEL_WORK = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_WORK"].ToString()));
            strQry.AppendLine(",EMPLOYEE_TEL_CELL = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_CELL"].ToString()));

            strQry.AppendLine(",EMPLOYEE_BIRTHDATE = '" + Convert.ToDateTime(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_BIRTHDATE"]).ToString("yyyy-MM-dd") + "'");

            strQry.AppendLine(",TAX_DIRECTIVE_NO1 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["TAX_DIRECTIVE_NO1"].ToString()));

            strQry.AppendLine(",TAX_DIRECTIVE_NO2 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["TAX_DIRECTIVE_NO2"].ToString()));
            strQry.AppendLine(",TAX_DIRECTIVE_NO3 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["TAX_DIRECTIVE_NO3"].ToString()));

            strQry.AppendLine(",TAX_DIRECTIVE_PERCENTAGE = " + parDataSet.Tables["Employee"].Rows[0]["TAX_DIRECTIVE_PERCENTAGE"].ToString());
            strQry.AppendLine(",EMPLOYEE_EMAIL = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_EMAIL"].ToString()));

            strQry.AppendLine(",EMPLOYEE_TAX_NO = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TAX_NO"].ToString()));
           
            strQry.AppendLine(",NATURE_PERSON_NO = " + parDataSet.Tables["Employee"].Rows[0]["NATURE_PERSON_NO"].ToString());
           
            if (parDataSet.Tables["Employee"].Rows[0]["BANK_ACCOUNT_TYPE_NO"] != System.DBNull.Value)
            {
                strQry.AppendLine(",BANK_ACCOUNT_TYPE_NO = " + parDataSet.Tables["Employee"].Rows[0]["BANK_ACCOUNT_TYPE_NO"].ToString());
            }
            else
            {
                strQry.AppendLine(",BANK_ACCOUNT_TYPE_NO = 0");
            }

            if (parDataSet.Tables["Employee"].Rows[0]["BANK_NO"] != System.DBNull.Value)
            {
                strQry.AppendLine(",BANK_NO = " + parDataSet.Tables["Employee"].Rows[0]["BANK_NO"].ToString());
            }
            else
            {
                strQry.AppendLine(",BANK_NO = 0");
            }

            if (parDataSet.Tables["Employee"].Rows[0]["BANK_ACCOUNT_RELATIONSHIP_TYPE_NO"] != System.DBNull.Value)
            {
                strQry.AppendLine(",BANK_ACCOUNT_RELATIONSHIP_TYPE_NO = " + parDataSet.Tables["Employee"].Rows[0]["BANK_ACCOUNT_RELATIONSHIP_TYPE_NO"].ToString());
            }
            else
            {
                strQry.AppendLine(",BANK_ACCOUNT_RELATIONSHIP_TYPE_NO = NULL");
            }

            strQry.AppendLine(",BRANCH_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["BRANCH_CODE"].ToString()));
            //ELR - 20150228
            strQry.AppendLine(",BRANCH_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["BRANCH_DESC"].ToString()));
            
            strQry.AppendLine(",ACCOUNT_NO = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["ACCOUNT_NO"].ToString()));
            
            strQry.AppendLine(",ACCOUNT_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["ACCOUNT_NAME"].ToString()));
            strQry.AppendLine(",TAX_TYPE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["TAX_TYPE_IND"].ToString()));

            strQry.AppendLine(",USE_RES_ADDR_COMPANY_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["USE_RES_ADDR_COMPANY_IND"].ToString()));
            strQry.AppendLine(",USE_WORK_TEL_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["USE_WORK_TEL_IND"].ToString()));

            //ELR - 2014-08-27
            strQry.AppendLine(",SIC7_GROUP_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["SIC7_GROUP_CODE"].ToString()));

            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Employee"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString());

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
