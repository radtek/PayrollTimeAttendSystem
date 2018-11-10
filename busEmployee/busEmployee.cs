using System;
using System.Text;
using System.Data;
using DPUruNet;
using System.Runtime.Serialization;

namespace InteractPayroll
{
    public class busEmployee
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busEmployee()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records_TimeAttend(Int64 parInt64CompanyNo, string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",COMPANY_DESC");
            strQry.AppendLine(",GENERATE_EMPLOYEE_NUMBER_IND");
            strQry.AppendLine(",TEL_WORK");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" OCCUPATION_NO");
            strQry.AppendLine(",OCCUPATION_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.OCCUPATION");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" OCCUPATION_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Occupation", parInt64CompanyNo);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" DEPARTMENT_NO");
            strQry.AppendLine(",DEPARTMENT_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEPARTMENT");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" DEPARTMENT_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Department", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_DESC");
            strQry.AppendLine(",ISNULL(CLOSED_IND,'N') AS CLOSED_IND");
           
            strQry.AppendLine(" FROM  InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T' ");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND PAY_CATEGORY_NO > 0");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.COMPANY_NO");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.TIE_BREAKER");

            //Passed So That We Can Use 1 Function form Save
            strQry.AppendLine(",EPC.HOURLY_RATE");
            strQry.AppendLine(",EPC.DEFAULT_IND");
            strQry.AppendLine(",EPC.LEAVE_DAY_RATE_DECIMAL");
           
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'T' ");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.OCCUPATION_NO");
            strQry.AppendLine(",E.DEPARTMENT_NO");
            strQry.AppendLine(",E.EMPLOYEE_TEL_HOME");
            strQry.AppendLine(",E.EMPLOYEE_TEL_WORK");
            strQry.AppendLine(",E.EMPLOYEE_TEL_CELL");
            strQry.AppendLine(",E.USE_WORK_TEL_IND");
            strQry.AppendLine(",E.EMPLOYEE_TAX_STARTDATE");
            strQry.AppendLine(",E.EMPLOYEE_ENDDATE");
            strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(",E.EMPLOYEE_TAKEON_IND");
            strQry.AppendLine(",E.EMPLOYEE_3RD_PARTY_CODE");

            //2017-09-29
            strQry.AppendLine(",E.USE_EMPLOYEE_NO_IND");
            strQry.AppendLine(",E.EMPLOYEE_PIN");
            strQry.AppendLine(",E.EMPLOYEE_RFID_CARD_NO");

            strQry.AppendLine(",E.ETI_START_DATE");

            //ELR - 2015-02-17
            strQry.AppendLine(",'' AS CLOSE_IND");
            
            strQry.AppendLine(",MAX(PCPC.COMPANY_NO) AS PAYROLL_LINK");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = 'T' ");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL ");

            if (parstrCurrentUserAccess == "A")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                strQry.AppendLine(" ON UCA.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND E.COMPANY_NO = UCA.COMPANY_NO ");
                strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A' ");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
            }

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
            strQry.AppendLine(" ON E.COMPANY_NO = PCPC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" AND (PCPC.RUN_TYPE = 'P'");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y')");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.OCCUPATION_NO");
            strQry.AppendLine(",E.DEPARTMENT_NO");
            strQry.AppendLine(",E.EMPLOYEE_TEL_HOME");
            strQry.AppendLine(",E.EMPLOYEE_TEL_WORK");
            strQry.AppendLine(",E.EMPLOYEE_TEL_CELL");
            strQry.AppendLine(",E.USE_WORK_TEL_IND");
            strQry.AppendLine(",E.EMPLOYEE_TAX_STARTDATE");
            strQry.AppendLine(",E.EMPLOYEE_ENDDATE");
            strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(",E.EMPLOYEE_TAKEON_IND");
            strQry.AppendLine(",E.EMPLOYEE_3RD_PARTY_CODE");
            //2017-09-29
            strQry.AppendLine(",E.USE_EMPLOYEE_NO_IND");
            strQry.AppendLine(",E.EMPLOYEE_PIN");
            strQry.AppendLine(",E.EMPLOYEE_RFID_CARD_NO");
            strQry.AppendLine(",E.ETI_START_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EFT.COMPANY_NO");
            strQry.AppendLine(",EFT.EMPLOYEE_NO");
            strQry.AppendLine(",EFT.FINGER_NO");
                       
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFT");

            strQry.AppendLine(" WHERE EFT.COMPANY_NO = " + parInt64CompanyNo.ToString());
          
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EFT.EMPLOYEE_NO");
            strQry.AppendLine(",EFT.FINGER_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeFingerTemplate", parInt64CompanyNo);

            DataSet.Tables["EmployeeFingerTemplate"].Columns.Add("FINGER_TEMPLATE", typeof(System.Byte[]));
   
            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo, string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

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
            strQry.AppendLine(" TAX_OFFICE_NO");
            strQry.AppendLine(",TAX_OFFICE_DESC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.TAX_OFFICE");

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" TAX_OFFICE_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TaxOffice", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" DEPARTMENT_NO");
            strQry.AppendLine(",DEPARTMENT_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEPARTMENT");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");
            
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" DEPARTMENT_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Department", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" OCCUPATION_NO");
            strQry.AppendLine(",OCCUPATION_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.OCCUPATION");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");
            
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" OCCUPATION_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Occupation", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" GENDER_IND");
            strQry.AppendLine(",GENDER_DESC");

            strQry.AppendLine(" FROM InteractPayroll.dbo.GENDER");

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" GENDER_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Gender", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" NATURE_PERSON_NO");
            strQry.AppendLine(",NATURE_PERSON_DESC");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.NATURE_PERSON");
            
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" NATURE_PERSON_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "NaturePerson", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" MARITAL_STATUS_NO");
            strQry.AppendLine(",MARITAL_STATUS_DESC");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.MARITAL_STATUS");
            
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" MARITAL_STATUS_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "MaritalStatus", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" RACE_NO");
            strQry.AppendLine(",RACE_DESC");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.RACE");
            
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" RACE_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Race", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PROCESS_NO");
            strQry.AppendLine(",PROCESS_DESC");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.PROCESS ");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PROCESS_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Process", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" C.OVERTIME1_RATE");
            strQry.AppendLine(",C.OVERTIME2_RATE");
            strQry.AppendLine(",C.OVERTIME3_RATE");
            strQry.AppendLine(",C.GENERATE_EMPLOYEE_NUMBER_IND");
            strQry.AppendLine(",C.SALARY_DOUBLE_CHEQUE_BIRTHDAY_IND");
            strQry.AppendLine(",CL.DATE_FORMAT");
            strQry.AppendLine(",'A' AS ACCESS_IND");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
            strQry.AppendLine(" ON C.COMPANY_NO = CL.COMPANY_NO ");

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

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",DAY_NO");
            strQry.AppendLine(",TIME_DECIMAL");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" PAY_CATEGORY_NO");
            strQry.AppendLine(",DAY_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryTimeDecimal", parInt64CompanyNo);

            DateTime dtDateNowAYearAgo = DateTime.Now.AddYears(-1);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PUBLIC_HOLIDAY_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY ");

            strQry.AppendLine(" WHERE  PUBLIC_HOLIDAY_DATE > '" + dtDateNowAYearAgo.ToString("yyyy-MM-dd") + "'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHoliday", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EARNING_NO");
            strQry.AppendLine(",E.EARNING_DESC");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING E");
            
            strQry.AppendLine(" WHERE E.EARNING_NO >= 200");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
            
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" E.EARNING_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveType", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" D.COMPANY_NO");
            strQry.AppendLine(",D.DEDUCTION_NO");
            strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",D.DEDUCTION_DESC");
            strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_COUNT");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
            
            strQry.AppendLine(" WHERE D.DEDUCTION_LOAN_TYPE_IND = 'Y'");
            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" D.DEDUCTION_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LoanType", parInt64CompanyNo);

            DateTime dtEndTaxYear;
            DateTime dtStartTaxYear;

            if (DateTime.Now.Month > 2)
            {
                dtEndTaxYear = new DateTime(DateTime.Now.Year + 1, 3, 1).AddDays(-1);
            }
            else
            {
                dtEndTaxYear = new DateTime(DateTime.Now.Year, 3, 1).AddDays(-1);
            }

            dtStartTaxYear = new DateTime(dtEndTaxYear.Year - 1, 3, 1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
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
            //ELR - 20150214
            strQry.AppendLine(",E.EMPLOYEE_RES_COUNTRY_CODE2");

            //ELR - 20150214
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
            strQry.AppendLine(",ISNULL(E.ANNUAL_SALARY,0) AS ANNUAL_SALARY ");

            strQry.AppendLine(",E.OCCUPATION_NO");
            strQry.AppendLine(",E.DEPARTMENT_NO");

            strQry.AppendLine(",E.TAX_DIRECTIVE_NO1");
            strQry.AppendLine(",E.TAX_DIRECTIVE_NO2");
            strQry.AppendLine(",E.TAX_DIRECTIVE_NO3");
            strQry.AppendLine(",E.TAX_DIRECTIVE_PERCENTAGE");
            strQry.AppendLine(",E.EMPLOYEE_EMAIL");
            strQry.AppendLine(",E.EMPLOYEE_TAX_NO");
            strQry.AppendLine(",E.EMPLOYEE_TAX_OFFICE_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.LEAVE_SHIFT_NO");
            strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(",E.EMPLOYEE_NUMBER_CHEQUES");
            strQry.AppendLine(",E.NATURE_PERSON_NO");
            strQry.AppendLine(",E.RACE_NO");
            strQry.AppendLine(",E.MARITAL_STATUS_NO");
            strQry.AppendLine(",E.NUMBER_MEDICAL_AID_DEPENDENTS");
            //ELR 2014-05-01
            strQry.AppendLine(",E.MEDICAL_AID_DISABILITY_IND");
            //2017-09-23
            strQry.AppendLine(",E.EMAIL_VIA_PAYSLIP_IND");

            strQry.AppendLine(",E.GENDER_IND");

            strQry.AppendLine(",E.BANK_ACCOUNT_TYPE_NO");
            strQry.AppendLine(",E.BANK_ACCOUNT_RELATIONSHIP_TYPE_NO");

            strQry.AppendLine(",E.BRANCH_CODE");
            strQry.AppendLine(",E.ACCOUNT_NO");

            //Errol - 2015-02-18
            strQry.AppendLine(",E.BRANCH_DESC");

            strQry.AppendLine(",E.ACCOUNT_NAME");
            strQry.AppendLine(",E.TAX_TYPE_IND");

            strQry.AppendLine(",E.OCCUPATION_LEVEL_NO");
            strQry.AppendLine(",E.OCCUPATION_CATEGORY_NO");
            strQry.AppendLine(",E.OCCUPATION_FUNCTION_NO");
            strQry.AppendLine(",E.DISABLED_IND");

            strQry.AppendLine(",E.USE_RES_ADDR_COMPANY_IND");
            strQry.AppendLine(",E.USE_WORK_TEL_IND");

            strQry.AppendLine(",E.NATURE_OF_DISABILITY");
            strQry.AppendLine(",E.SKILLS_EQUITY_PROVINCE_NO");

            strQry.AppendLine(",E.EMPLOYEE_TAKEON_IND");
            strQry.AppendLine(",E.ETI_START_DATE");

            strQry.AppendLine(",E.EMPLOYEE_3RD_PARTY_CODE");

            //2017-09-29
            strQry.AppendLine(",E.USE_EMPLOYEE_NO_IND");
            strQry.AppendLine(",E.EMPLOYEE_PIN");
            strQry.AppendLine(",E.EMPLOYEE_RFID_CARD_NO");
            
            ////Leave PAY_CATEGORY_NO
            strQry.AppendLine(",CHECK_TABLE.PAY_CATEGORY_NO AS LEAVE_PAY_CATEGORY_NO");
            strQry.AppendLine(",CHECK_TABLE.LEAVE_PAID_ACCUMULATOR_IND");
            strQry.AppendLine(",CHECK_TABLE.LEAVE_CONFIG_ERROR_IND");

            strQry.AppendLine(",PCPC.COMPANY_NO AS PAYROLL_LINK");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN ");

            strQry.AppendLine("(SELECT ");

            strQry.AppendLine(" E.EMPLOYEE_NO");
            //Leave PAY_CATEGORY_NO
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",LS.LEAVE_PAID_ACCUMULATOR_IND ");

            strQry.AppendLine(",LEAVE_CONFIG_ERROR_IND = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.PAY_CATEGORY_TYPE = 'S'");

            strQry.AppendLine(" THEN 'N'");

            strQry.AppendLine(" WHEN LS.LEAVE_PAID_ACCUMULATOR_IND = 1 AND COUNT(PCTD.DAY_NO) = 5 ");

            strQry.AppendLine(" THEN 'N'");

            strQry.AppendLine(" WHEN LS.LEAVE_PAID_ACCUMULATOR_IND = 2 AND COUNT(PCTD.DAY_NO) = 6 ");

            strQry.AppendLine(" THEN 'N'");

            strQry.AppendLine(" WHEN LS.LEAVE_PAID_ACCUMULATOR_IND = 3 AND COUNT(PCTD.DAY_NO) = 7 ");

            strQry.AppendLine(" THEN 'N'");

            strQry.AppendLine(" ELSE 'Y'");

            strQry.AppendLine(" END ");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            //Leave Related
            strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y' ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");
  
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL PCTD");
            strQry.AppendLine(" ON E.COMPANY_NO = PCTD.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PCTD.PAY_CATEGORY_NO ");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT LS");
            strQry.AppendLine(" ON E.COMPANY_NO = LS.COMPANY_NO");
            strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = LS.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND (((LS.LEAVE_PAID_ACCUMULATOR_IND = 1");
            strQry.AppendLine(" AND PCTD.DAY_NO IN (1,2,3,4,5))");
            //Saturday Included
            strQry.AppendLine(" OR (LS.LEAVE_PAID_ACCUMULATOR_IND = 2");
            strQry.AppendLine(" AND PCTD.DAY_NO IN (1,2,3,4,5,6)))");
            //Sunday Included
            strQry.AppendLine(" OR (LS.LEAVE_PAID_ACCUMULATOR_IND = 3");
            strQry.AppendLine(" AND PCTD.DAY_NO IN (0,1,2,3,4,5,6)))");
           
            strQry.AppendLine(" AND LS.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" E.EMPLOYEE_NO");
            //Leave PAY_CATEGORY_NO
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",LS.LEAVE_PAID_ACCUMULATOR_IND ");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE) AS CHECK_TABLE");

            strQry.AppendLine(" ON E.EMPLOYEE_NO = CHECK_TABLE.EMPLOYEE_NO");
          
            if (parstrCurrentUserAccess == "A")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                strQry.AppendLine(" ON UCA.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND E.COMPANY_NO = UCA.COMPANY_NO ");
                strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A' ");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");
            }
            
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
            strQry.AppendLine(" ON E.COMPANY_NO = PCPC.COMPANY_NO");
            strQry.AppendLine(" AND CHECK_TABLE.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" AND ((PCPC.RUN_TYPE = 'T'");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND <> 'Y')");
            strQry.AppendLine(" OR (PCPC.RUN_TYPE = 'P'");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'))");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);
            
            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EFT.COMPANY_NO");
            strQry.AppendLine(",EFT.EMPLOYEE_NO");
            strQry.AppendLine(",EFT.FINGER_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFT");

            strQry.AppendLine(" WHERE EFT.COMPANY_NO = " + parInt64CompanyNo.ToString());

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EFT.EMPLOYEE_NO");
            strQry.AppendLine(",EFT.FINGER_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeFingerTemplate", parInt64CompanyNo);

            DataSet.Tables["EmployeeFingerTemplate"].Columns.Add("FINGER_TEMPLATE", typeof(System.Byte[]));
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.COMPANY_NO");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.TIE_BREAKER");
            strQry.AppendLine(",EPC.DATETIME_NEW_RECORD");
            strQry.AppendLine(",EPC.HOURLY_RATE");
            strQry.AppendLine(",LEAVE_DAY_RATE_DECIMAL = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN EPC.PAY_CATEGORY_TYPE = 'W'");
            
            strQry.AppendLine(" THEN EPC.LEAVE_DAY_RATE_DECIMAL");

            strQry.AppendLine(" ELSE ROUND(PC.SALARY_MINUTES_PAID_PER_DAY / 60,2)");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",EPC.DEFAULT_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
         
            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EARNING_NO");
            strQry.AppendLine(",E.EARNING_DESC");
            strQry.AppendLine(",E.IRP5_CODE");
            strQry.AppendLine(",'S' AS EARNING_TYPE_IND");
            strQry.AppendLine(",'E' AS EARNING_PERIOD_IND");

            strQry.AppendLine(",0 AS EARNING_DAY_VALUE");

            strQry.AppendLine(",CONVERT(DECIMAL,0) AS AMOUNT");
           
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING E");

            strQry.AppendLine(" INNER JOIN ");
            strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.COMPANY C ");
            strQry.AppendLine(" ON E.COMPANY_NO = C.COMPANY_NO ");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" AND ((E.EARNING_NO = 3");
            strQry.AppendLine(" AND C.OVERTIME1_RATE <> 0)");

            strQry.AppendLine(" OR (E.EARNING_NO = 4");
            strQry.AppendLine(" AND C.OVERTIME2_RATE <> 0)");

            strQry.AppendLine(" OR (E.EARNING_NO = 5");
            strQry.AppendLine(" AND C.OVERTIME3_RATE <> 0)");
            
            strQry.AppendLine(" OR (E.EARNING_NO IN (9) AND E.PAY_CATEGORY_TYPE = 'W')");
            
            //ELR 2018/11/10 - Any Paid Leave
            strQry.AppendLine(" OR (E.EARNING_NO >= 200  AND E.LEAVE_PERCENTAGE > 0 AND E.PAY_CATEGORY_TYPE = 'W')");
            
            //System Defined
            strQry.AppendLine(" OR (E.EARNING_NO < 11");
            strQry.AppendLine(" AND E.EARNING_NO NOT IN (3,4,5,9)))");

            strQry.AppendLine(" UNION ");
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EARNING_NO");
            strQry.AppendLine(",E.EARNING_DESC");
            strQry.AppendLine(",E.IRP5_CODE");
            strQry.AppendLine(",EE.EARNING_TYPE_IND");
            strQry.AppendLine(",EE.EARNING_PERIOD_IND");
            strQry.AppendLine(",EE.EARNING_DAY_VALUE");
            strQry.AppendLine(",EE.AMOUNT");
           
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE ");
            strQry.AppendLine(" ON E.COMPANY_NO = EE.COMPANY_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND E.EARNING_NO = EE.EARNING_NO");
            strQry.AppendLine(" AND EE.EMPLOYEE_NO = 0 ");
            strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            //User Defined
            strQry.AppendLine(" AND (E.EARNING_NO > 10");
            strQry.AppendLine(" AND E.EARNING_NO < 200)");

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" E.EARNING_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Earning", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO <> 0 ");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EARNING_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DeductionEarningPercentage", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO = 0 ");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EARNING_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DeductionEarningPercentageDefault", parInt64CompanyNo);

            DataSet TempDataSet;

            if (DataSet.Tables["Employee"].Rows.Count > 0)
            {
                TempDataSet = Get_Employee_EarningDeductionLeaveLoans_DataSet(parInt64CompanyNo, Convert.ToInt32(DataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"]), DataSet.Tables["Employee"].Rows[0]["PAY_CATEGORY_TYPE"].ToString());
            }
            else
            {
                TempDataSet = Get_Employee_EarningDeductionLeaveLoans_DataSet(parInt64CompanyNo, -1, "W");
            }

            DataSet.Merge(TempDataSet);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_DESC");
            strQry.AppendLine(",MON_TIME_MINUTES");
            strQry.AppendLine(",TUE_TIME_MINUTES");
            strQry.AppendLine(",WED_TIME_MINUTES");
            strQry.AppendLine(",THU_TIME_MINUTES");
            strQry.AppendLine(",FRI_TIME_MINUTES");
            strQry.AppendLine(",SAT_TIME_MINUTES");
            strQry.AppendLine(",SUN_TIME_MINUTES");
            strQry.AppendLine(",ISNULL(CLOSED_IND,'N') AS CLOSED_IND");
            
            strQry.AppendLine(" FROM  InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND PAY_CATEGORY_NO > 0");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LEAVE_SHIFT_NO");
            strQry.AppendLine(",LEAVE_SHIFT_DESC");
            strQry.AppendLine(",LEAVE_PAID_ACCUMULATOR_IND");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LEAVE_SHIFT_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveLink", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" D.COMPANY_NO");
            strQry.AppendLine(",D.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",D.DEDUCTION_NO");
            strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",ED.TIE_BREAKER");
            strQry.AppendLine(",D.DEDUCTION_DESC");
            strQry.AppendLine(",ED.DEDUCTION_TYPE_IND");
            strQry.AppendLine(",ED.DEDUCTION_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",ED.DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",D.IRP5_CODE");
            strQry.AppendLine(",D.DEDUCTION_LOAN_TYPE_IND");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED");
            strQry.AppendLine(" ON D.COMPANY_NO = ED.COMPANY_NO ");
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND D.DEDUCTION_NO = ED.DEDUCTION_NO ");
            strQry.AppendLine(" AND D.DEDUCTION_SUB_ACCOUNT_NO = ED.DEDUCTION_SUB_ACCOUNT_NO ");
            strQry.AppendLine(" AND ED.EMPLOYEE_NO = 0");
            strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE D.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" D.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",D.DEDUCTION_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Deduction", parInt64CompanyNo);

            //strQry.Clear();
            //strQry.AppendLine(" SELECT");
            //strQry.AppendLine(" IRP5_CODE");
            //strQry.AppendLine(" FROM ");
            //strQry.AppendLine(" InteractPayroll.dbo.EARNING_TWELVE_PERIODS");

            //clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TwelvePeriods", parInt64CompanyNo);

            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Employee_EarningDeductionLeaveLoans(Int64 parInt64CompanyNo, int parintEmployeeNo, string parstrPayCategoryType)
        {
            DataSet DataSet = Get_Employee_EarningDeductionLeaveLoans_DataSet(parInt64CompanyNo, parintEmployeeNo, parstrPayCategoryType);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        private DataSet Get_Deduction(Int64 parInt64CompanyNo, int parintEmployeeNo, string parstrPayCategoryType)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ED.COMPANY_NO");
            strQry.AppendLine(",ED.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ED.EMPLOYEE_NO");
            strQry.AppendLine(",ED.DEDUCTION_NO");
            strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",ED.TIE_BREAKER");
            strQry.AppendLine(",ED.DEDUCTION_TYPE_IND");
            strQry.AppendLine(",ED.DEDUCTION_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",ED.DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",D.DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",SUM(L.LOAN_AMOUNT_PAID) - SUM(L.LOAN_AMOUNT_RECEIVED) AS LOAN_OUTSTANDING");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEDUCTION D ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED ");
            strQry.AppendLine(" ON D.COMPANY_NO = ED.COMPANY_NO");
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND D.DEDUCTION_NO = ED.DEDUCTION_NO");
            strQry.AppendLine(" AND D.DEDUCTION_SUB_ACCOUNT_NO = ED.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(" AND ED.EMPLOYEE_NO = " + parintEmployeeNo);
            
            //2014-03-15 Let Medical Aid through
            strQry.AppendLine(" AND D.DEDUCTION_NO NOT IN (2)");

            //2=UIF,3=Pension Fund,4=Retirement Annuity,5=Medical Aid
            //strQry.AppendLine(" AND D.DEDUCTION_NO NOT IN (2,3,4,5)");

            strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.LOANS L ");
            strQry.AppendLine(" ON ED.COMPANY_NO = L.COMPANY_NO");
            strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = L.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND ED.EMPLOYEE_NO = L.EMPLOYEE_NO");
            strQry.AppendLine(" AND ED.DEDUCTION_NO = L.DEDUCTION_NO");
            strQry.AppendLine(" AND ED.DEDUCTION_SUB_ACCOUNT_NO = L.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(" AND L.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE D.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" ED.COMPANY_NO");
            strQry.AppendLine(",ED.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ED.EMPLOYEE_NO");
            strQry.AppendLine(",ED.DEDUCTION_NO");
            strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",ED.TIE_BREAKER");
            strQry.AppendLine(",ED.DEDUCTION_TYPE_IND");
            strQry.AppendLine(",ED.DEDUCTION_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",ED.DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",D.DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_COUNT");

            //UIF - Get Percentage from UIF Table
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ED.COMPANY_NO");
            strQry.AppendLine(",ED.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ED.EMPLOYEE_NO");
            strQry.AppendLine(",ED.DEDUCTION_NO");
            strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",ED.TIE_BREAKER");
            strQry.AppendLine(",ED.DEDUCTION_TYPE_IND");
            strQry.AppendLine(",UT.UIF_PERCENTAGE AS DEDUCTION_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",ED.DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",D.DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",0 AS LOAN_OUTSTANDING");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEDUCTION D ");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED ");
            strQry.AppendLine(" ON D.COMPANY_NO = ED.COMPANY_NO");
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND D.DEDUCTION_NO = ED.DEDUCTION_NO");
            strQry.AppendLine(" AND D.DEDUCTION_SUB_ACCOUNT_NO = ED.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.UIF_THRESHOLD UT ");
            strQry.AppendLine(" ON UT.UIF_PERCENTAGE = UT.UIF_PERCENTAGE");
         
            strQry.AppendLine(" WHERE D.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ED.EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND D.DEDUCTION_NO = 2");
         
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" ED.COMPANY_NO");
            strQry.AppendLine(",ED.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ED.EMPLOYEE_NO");
            strQry.AppendLine(",ED.DEDUCTION_NO");
            strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",ED.TIE_BREAKER");
            strQry.AppendLine(",ED.DEDUCTION_TYPE_IND");
            strQry.AppendLine(",UT.UIF_PERCENTAGE");
            strQry.AppendLine(",ED.DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",ED.DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",D.DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 2");
            strQry.AppendLine(",4");
            strQry.AppendLine(",5");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeDeduction", parInt64CompanyNo);

            //strQry.Clear();
            //strQry.AppendLine(" SELECT ");
            //strQry.AppendLine(" COMPANY_NO");
            //strQry.AppendLine(",PAY_CATEGORY_TYPE");
            //strQry.AppendLine(",EMPLOYEE_NO");
            //strQry.AppendLine(",DEDUCTION_NO");
            //strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            //strQry.AppendLine(",EARNING_NO");
            //strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE ");
            
            //strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            //strQry.AppendLine(" AND EMPLOYEE_NO  = " + parintEmployeeNo);
            //strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType);
            //strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");
            
            //strQry.AppendLine(" ORDER BY ");
            //strQry.AppendLine(" PAY_CATEGORY_TYPE");
            //strQry.AppendLine(",EMPLOYEE_NO");
            //strQry.AppendLine(",DEDUCTION_NO");
            //strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            //strQry.AppendLine(",EARNING_NO");

            //clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DeductionEarningPercentage", parInt64CompanyNo);

            return DataSet;
        }

        private DataSet Get_Earning(Int64 parInt64CompanyNo, int parintEmployeeNo, string parstrPayCategoryType)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EN.COMPANY_NO");
            strQry.AppendLine(",EN.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",EN.TIE_BREAKER");
            strQry.AppendLine(",EN.IRP5_CODE");
            strQry.AppendLine(",EN.EARNING_NO");
            strQry.AppendLine(",'E' AS EARNING_PERIOD_IND");
            strQry.AppendLine(",'S' AS EARNING_TYPE_IND");
            strQry.AppendLine(",0 AS EARNING_DAY_VALUE");
            strQry.AppendLine(",CONVERT(DECIMAL,0) AS AMOUNT");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
            strQry.AppendLine(" ON EN.COMPANY_NO = C.COMPANY_NO ");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EN.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" LEFT JOIN ");

            strQry.AppendLine("(SELECT DISTINCT ");
            strQry.AppendLine(" EPC.EMPLOYEE_NO ");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",PC.PAY_PUBLIC_HOLIDAY_IND ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE ");

            //Pay Out Public Holiday's
            strQry.AppendLine(" AND PC.PAY_PUBLIC_HOLIDAY_IND = 'Y' ");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EPC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL) AS PUBLIC_HOLIDAY_TABLE");

            strQry.AppendLine(" ON E.EMPLOYEE_NO = PUBLIC_HOLIDAY_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PUBLIC_HOLIDAY_TABLE.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" AND (EN.EARNING_NO IN (1,2,7)");
            
            //Public Holiday - Company Paid 
            strQry.AppendLine(" OR (EN.EARNING_NO in (8,9)");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND PUBLIC_HOLIDAY_TABLE.PAY_PUBLIC_HOLIDAY_IND = 'Y')");

            //Overtime 
            strQry.AppendLine(" OR (EN.EARNING_NO = 3");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND C.OVERTIME1_RATE <> 0)");

            strQry.AppendLine(" OR (EN.EARNING_NO = 4");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND C.OVERTIME2_RATE <> 0)");

            strQry.AppendLine(" OR (EN.EARNING_NO = 5");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" AND C.OVERTIME3_RATE <> 0)");

            strQry.AppendLine(" OR (EN.EARNING_NO = 3");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND C.SALARY_OVERTIME1_RATE <> 0)");

            strQry.AppendLine(" OR (EN.EARNING_NO = 4");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND C.SALARY_OVERTIME2_RATE <> 0)");

            strQry.AppendLine(" OR (EN.EARNING_NO = 5");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" AND C.SALARY_OVERTIME3_RATE <> 0))");

            strQry.AppendLine(" UNION ");
            
            //ELR - 2018-11-10 
            //Leave (Only for Wages) 
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EN.COMPANY_NO");
            strQry.AppendLine(",EN.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",EN.TIE_BREAKER");
            strQry.AppendLine(",EN.IRP5_CODE");
            strQry.AppendLine(",EN.EARNING_NO");
            strQry.AppendLine(",'E' AS EARNING_PERIOD_IND");
            strQry.AppendLine(",'S' AS EARNING_TYPE_IND");
            strQry.AppendLine(",0 AS EARNING_DAY_VALUE");
            strQry.AppendLine(",CONVERT(DECIMAL,0) AS AMOUNT");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C");
            strQry.AppendLine(" ON EN.COMPANY_NO = C.COMPANY_NO ");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EN.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EN.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

            //Leave (Only for Wages)
            strQry.AppendLine(" AND EN.EARNING_NO >= 200");
            strQry.AppendLine(" AND EN.LEAVE_PERCENTAGE > 0");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = 'W'");
            
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EE.COMPANY_NO");
            strQry.AppendLine(",EE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EE.EMPLOYEE_NO");
            strQry.AppendLine(",EE.TIE_BREAKER");
            strQry.AppendLine(",EN.IRP5_CODE");
            strQry.AppendLine(",EN.EARNING_NO");
            strQry.AppendLine(",EE.EARNING_PERIOD_IND");
            strQry.AppendLine(",EE.EARNING_TYPE_IND");
            strQry.AppendLine(",EE.EARNING_DAY_VALUE");

            strQry.AppendLine(",EE.AMOUNT");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING EN ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE ");
            strQry.AppendLine(" ON EN.COMPANY_NO = EE.COMPANY_NO ");
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EN.EARNING_NO = EE.EARNING_NO");
            strQry.AppendLine(" AND EE.EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE EN.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EN.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayCategoryType));
            strQry.AppendLine(" AND EN.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EN.EARNING_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeEarning", parInt64CompanyNo);

            return DataSet;
        }

        public DataSet Get_Employee_EarningDeductionLeaveLoans_DataSet(Int64 parInt64CompanyNo, int parintEmployeeNo, string parstrPayCategoryType)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            DataSet TempDataSet = new DataSet();
            DateTime dtEndTaxYear;
            DateTime dtStartTaxYear;
            DateTime dtStartLeaveTaxYear;

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ISNULL(MAX(PAY_PERIOD_DATE),GETDATE()) AS MAX_PAY_PERIOD_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), TempDataSet, "MaxDate", parInt64CompanyNo);

            //Position Within Current Financial Year
            if (Convert.ToDateTime(TempDataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Month > 2)
            {
                dtEndTaxYear = new DateTime(Convert.ToDateTime(TempDataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year + 1, 3, 1).AddDays(-1);
            }
            else
            {
                dtEndTaxYear = new DateTime(Convert.ToDateTime(TempDataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year, 3, 1).AddDays(-1);
            }

            dtStartTaxYear = new DateTime(dtEndTaxYear.Year - 1, 3, 1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" LEAVE_BEGIN_MONTH");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY E");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), TempDataSet, "LeaveStartDate", parInt64CompanyNo);

            if (TempDataSet.Tables["LeaveStartDate"].Rows.Count > 0)
            {
                //Position Within Current Financial Year
                if (Convert.ToDateTime(TempDataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Month >= Convert.ToInt32(TempDataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]))
                {
                    dtStartLeaveTaxYear = new DateTime(Convert.ToDateTime(TempDataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year, Convert.ToInt32(TempDataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
                }
                else
                {
                    dtStartLeaveTaxYear = new DateTime(Convert.ToDateTime(TempDataSet.Tables["MaxDate"].Rows[0]["MAX_PAY_PERIOD_DATE"]).Year - 1, Convert.ToInt32(TempDataSet.Tables["LeaveStartDate"].Rows[0]["LEAVE_BEGIN_MONTH"]), 1);
                }
            }
            else
            {
                dtStartLeaveTaxYear = dtStartTaxYear;
            }
            
            TempDataSet = Get_Deduction(parInt64CompanyNo, parintEmployeeNo, parstrPayCategoryType);

            DataSet.Merge(TempDataSet);

            TempDataSet = Get_Earning(parInt64CompanyNo, parintEmployeeNo, parstrPayCategoryType);

            DataSet.Merge(TempDataSet);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 1 AS SORT_ORDER");
            strQry.AppendLine(",LH.COMPANY_NO");
            strQry.AppendLine(",LH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LH.EMPLOYEE_NO");
            strQry.AppendLine(",LH.EARNING_NO");
            strQry.AppendLine(",'" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'AS LEAVE_PROCESSED_DATE");
            strQry.AppendLine(",0 AS LEAVE_REC_NO");
            strQry.AppendLine(",'D' AS LEAVE_OPTION");
            strQry.AppendLine(",0 AS LEAVE_DAYS_DECIMAL");
            strQry.AppendLine(",0 AS LEAVE_HOURS_DECIMAL");
            strQry.AppendLine(",'Accumulated Days Balance c/f' AS LEAVE_DESC");

            strQry.AppendLine(",CONVERT(SMALLINT,99) AS PROCESS_NO");
            strQry.AppendLine(",0 AS DATE_DIFF_NO_DAYS");

            strQry.AppendLine(",MIN(LH.LEAVE_FROM_DATE) AS LEAVE_FROM_DATE");
            strQry.AppendLine(",MAX(LH.LEAVE_TO_DATE) AS LEAVE_TO_DATE ");
            strQry.AppendLine(",SUM(ROUND(LH.LEAVE_ACCUM_DAYS,2)) AS LEAVE_ACCUM_DAYS ");
            strQry.AppendLine(",SUM(ROUND(LH.LEAVE_PAID_DAYS,2)) AS LEAVE_PAID_DAYS ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON LH.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND LH.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            //2017-02-16 - Removed for when Employee Changes Type
            //strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND LH.EMPLOYEE_NO = " + parintEmployeeNo);
            
            //2017-01-10 - Covers YearEnd Boundary
            strQry.AppendLine(" AND ((LH.PAY_PERIOD_DATE < '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "')");

            //Extra Leave Carried over YearEnd Boundary
            strQry.AppendLine(" OR (LH.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(" AND LH.PAY_PERIOD_DATE <= '" + dtStartLeaveTaxYear.AddMonths(1).ToString("yyyy-MM-dd") + "'");
            //Leave ToDate is End of Year
            strQry.AppendLine(" AND LH.LEAVE_TO_DATE = '" + dtStartLeaveTaxYear.AddDays(-1).ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(" AND LH.PROCESS_NO = 98))");
            
            //Normal Leave
            strQry.AppendLine(" AND LH.EARNING_NO = 200");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" LH.COMPANY_NO");
            strQry.AppendLine(",LH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LH.EMPLOYEE_NO");
            strQry.AppendLine(",LH.EARNING_NO");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 2 AS SORT_ORDER");
            strQry.AppendLine(",LH.COMPANY_NO");
            strQry.AppendLine(",LH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LH.EMPLOYEE_NO");
            strQry.AppendLine(",LH.EARNING_NO");
            strQry.AppendLine(",MAX(LH.LEAVE_TO_DATE) AS LEAVE_PROCESSED_DATE");
            strQry.AppendLine(",0 AS LEAVE_REC_NO");
            strQry.AppendLine(",'D' AS LEAVE_OPTION");
            strQry.AppendLine(",SUM(LH.LEAVE_DAYS_DECIMAL) AS LEAVE_DAYS_DECIMAL");
            strQry.AppendLine(",0 AS LEAVE_HOURS_DECIMAL");
            strQry.AppendLine(",'Current Year Accumulated Days Balance' AS LEAVE_DESC");

            strQry.AppendLine(",CONVERT(SMALLINT,99) AS PROCESS_NO");
            strQry.AppendLine(",0 AS DATE_DIFF_NO_DAYS");

            //2017-01-10
            //strQry.AppendLine(",MIN(LH.LEAVE_FROM_DATE) AS LEAVE_FROM_DATE");
            strQry.AppendLine(",'" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'AS LEAVE_FROM_DATE");

            strQry.AppendLine(",MAX(LH.LEAVE_TO_DATE) AS LEAVE_TO_DATE ");
            strQry.AppendLine(",SUM(ROUND(LH.LEAVE_ACCUM_DAYS,2)) AS LEAVE_ACCUM_DAYS ");
            strQry.AppendLine(",SUM(ROUND(LH.LEAVE_PAID_DAYS,2)) AS LEAVE_PAID_DAYS ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON LH.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND LH.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            //2017-02-16 - Removed for when Employee Changes Type
            //strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND LH.EMPLOYEE_NO = " + parintEmployeeNo);

            //2017-01-10
            strQry.AppendLine(" AND (LH.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");
            //Covers YearEnd Boundary 
            //Extra Leave Carried over YearEnd Boundary
            strQry.AppendLine(" AND NOT (LH.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(" AND LH.PAY_PERIOD_DATE <= '" + dtStartLeaveTaxYear.AddMonths(1).ToString("yyyy-MM-dd") + "'");
            //Leave ToDate is End of Year
            strQry.AppendLine(" AND LH.LEAVE_TO_DATE = '" + dtStartLeaveTaxYear.AddDays(-1).ToString("yyyy-MM-dd") + "'))");
            
            //Not Accumulated Leave for Pay Period 
            strQry.AppendLine(" AND LH.PROCESS_NO = 98");
            //Normal Leave / Sick Leave
            strQry.AppendLine(" AND LH.EARNING_NO IN (200,201)");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" LH.COMPANY_NO");
            strQry.AppendLine(",LH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LH.EMPLOYEE_NO");
            strQry.AppendLine(",LH.EARNING_NO");
          
            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 2 AS SORT_ORDER");
            strQry.AppendLine(",LH.COMPANY_NO");
            strQry.AppendLine(",LH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LH.EMPLOYEE_NO");
            strQry.AppendLine(",LH.EARNING_NO");
            strQry.AppendLine(",LH.PAY_PERIOD_DATE AS LEAVE_PROCESSED_DATE");
            strQry.AppendLine(",0 AS LEAVE_REC_NO");
            strQry.AppendLine(",LH.LEAVE_OPTION");
            strQry.AppendLine(",LH.LEAVE_DAYS_DECIMAL");
            strQry.AppendLine(",LH.LEAVE_HOURS_DECIMAL");
            strQry.AppendLine(",LH.LEAVE_DESC");

            strQry.AppendLine(",LH.PROCESS_NO");
            strQry.AppendLine(",0 AS DATE_DIFF_NO_DAYS");

            strQry.AppendLine(",LH.LEAVE_FROM_DATE");
            strQry.AppendLine(",LH.LEAVE_TO_DATE");
            strQry.AppendLine(",ROUND(LH.LEAVE_ACCUM_DAYS,2) AS LEAVE_ACCUM_DAYS ");
            strQry.AppendLine(",ROUND(LH.LEAVE_PAID_DAYS,2) AS LEAVE_PAID_DAYS ");
      
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_HISTORY LH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON LH.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND LH.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            //2017-02-16 - Removed for when Employee Changes Type
            //strQry.AppendLine(" AND LH.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
            
            strQry.AppendLine(" WHERE LH.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND LH.EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND LH.PAY_PERIOD_DATE >= '" + dtStartLeaveTaxYear.ToString("yyyy-MM-dd") + "'");

            //Not Accumulated Leave for Pay Period 
            strQry.AppendLine(" AND LH.PROCESS_NO <> 98");

            strQry.AppendLine(" UNION ");

            //Records Not Yet Processed
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 3 AS SORT_ORDER");
            strQry.AppendLine(",LC.COMPANY_NO");
            strQry.AppendLine(",LC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LC.EMPLOYEE_NO");
            strQry.AppendLine(",LC.EARNING_NO");
            strQry.AppendLine(",NULL AS LEAVE_PROCESSED_DATE");
            strQry.AppendLine(",LC.LEAVE_REC_NO");
            strQry.AppendLine(",LC.LEAVE_OPTION");
            strQry.AppendLine(",LC.LEAVE_DAYS_DECIMAL");
            strQry.AppendLine(",LC.LEAVE_HOURS_DECIMAL");
            strQry.AppendLine(",LC.LEAVE_DESC");

            strQry.AppendLine(",LC.PROCESS_NO");
            strQry.AppendLine(",DATEDIFF(d,LC.LEAVE_FROM_DATE,LC.LEAVE_TO_DATE ) + 1 AS DATE_DIFF_NO_DAYS");
                      
            strQry.AppendLine(",LC.LEAVE_FROM_DATE");
            strQry.AppendLine(",LC.LEAVE_TO_DATE");
            strQry.AppendLine(",0 AS LEAVE_ACCUM_DAYS");
            strQry.AppendLine(",0 AS LEAVE_PAID_DAYS");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT LC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON LC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND LC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND LC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE LC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND LC.EMPLOYEE_NO = " + parintEmployeeNo);
                
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",LEAVE_FROM_DATE");
            strQry.AppendLine(",LEAVE_TO_DATE");
        
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeLeave", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",LOAN_PROCESSED_DATE");
            strQry.AppendLine(",LOAN_REC_NO");
            strQry.AppendLine(",LOAN_DESC");
            strQry.AppendLine(",LOAN_AMOUNT_PAID");
            strQry.AppendLine(",LOAN_AMOUNT_RECEIVED");
            strQry.AppendLine(",PROCESS_NO");
            strQry.AppendLine(",SORT_ORDER = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT LOAN_PROCESSED_DATE IS NULL");
            strQry.AppendLine(" THEN 0 ");

            strQry.AppendLine(" ELSE 1 ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LOANS");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",LOAN_PROCESSED_DATE");
            strQry.AppendLine(",LOAN_REC_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeLoan", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",'" + dtStartTaxYear.ToString("yyyy-MM-dd") + "' AS LOAN_PROCESSED_DATE");
            strQry.AppendLine(",'Balance b/f' AS LOAN_DESC");
            strQry.AppendLine(",99 AS PROCESS_NO");
            strQry.AppendLine(",SUM(LOAN_AMOUNT_PAID) AS LOAN_AMOUNT_PAID");
            strQry.AppendLine(",SUM(LOAN_AMOUNT_RECEIVED) AS LOAN_AMOUNT_RECEIVED");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LOANS");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND LOAN_PROCESSED_DATE < '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "'");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
           
            //NB PROCESS_NO IS Excluded from GROUP BY Beacause it would Cause More Than 1 Record for SUM
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
           
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeLoanCF", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" '" + dtStartTaxYear.ToString("yyyy-MM-dd") + "' AS FISCAL_START_DATE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "BeginYear", parInt64CompanyNo);

            return DataSet;
        }

        public int Insert_New_Record(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, byte[] parbyteDataSet, string parstrPayrollType)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
            string strEmployeeCode = "";
            StringBuilder strFieldNamesInitialised = new StringBuilder();
            int intEmployeeNo = -1;

            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT MAX(EMPLOYEE_NO) AS MAX_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo.ToString());

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
            {
                intEmployeeNo = 1;
            }
            else
            {
                intEmployeeNo = Convert.ToInt32(DataSet.Tables[0].Rows[0]["MAX_NO"]) + 1;
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT GENERATE_EMPLOYEE_NUMBER_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo.ToString());

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp1", parInt64CompanyNo);

            if (DataSet.Tables["Temp1"].Rows[0]["GENERATE_EMPLOYEE_NUMBER_IND"].ToString() == "N")
            {
                strEmployeeCode = intEmployeeNo.ToString("00000");
            }

            DataSet.Dispose();
            DataSet = null;

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE ");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",EMPLOYEE_CODE");
            strQry.AppendLine(",EMPLOYEE_NAME");
            strQry.AppendLine(",EMPLOYEE_SURNAME");
            strQry.AppendLine(",EMPLOYEE_INITIALS");

            strQry.AppendLine(",EMPLOYEE_3RD_PARTY_CODE");
            
            strQry.AppendLine(",EMPLOYEE_ID_NO");

            strQry.AppendLine(",EMPLOYEE_RES_UNIT_NUMBER ");
            strQry.AppendLine(",EMPLOYEE_RES_COMPLEX ");
            strQry.AppendLine(",EMPLOYEE_RES_STREET_NUMBER ");
            strQry.AppendLine(",EMPLOYEE_RES_STREET_NAME ");
            strQry.AppendLine(",EMPLOYEE_RES_SUBURB ");
            strQry.AppendLine(",EMPLOYEE_RES_CITY ");
            strQry.AppendLine(",EMPLOYEE_RES_CODE");

            //strQry.AppendLine(",EMPLOYEE_POST_ADDR_LINE1");
            //strQry.AppendLine(",EMPLOYEE_POST_ADDR_LINE2");
            //strQry.AppendLine(",EMPLOYEE_POST_ADDR_LINE3");

            //ELR - 20150214
            strQry.AppendLine(",EMPLOYEE_POST_OPTION_IND");
            strQry.AppendLine(",EMPLOYEE_POST_UNIT_NUMBER");
            strQry.AppendLine(",EMPLOYEE_POST_COMPLEX");
            strQry.AppendLine(",EMPLOYEE_POST_STREET_NUMBER");
            strQry.AppendLine(",EMPLOYEE_POST_STREET_NAME");
            strQry.AppendLine(",EMPLOYEE_POST_SUBURB");
            strQry.AppendLine(",EMPLOYEE_POST_CITY");
            strQry.AppendLine(",EMPLOYEE_POST_CODE");
            strQry.AppendLine(",EMPLOYEE_POST_COUNTRY_CODE2");
            
            strQry.AppendLine(",EMPLOYEE_TEL_HOME");
            strQry.AppendLine(",EMPLOYEE_TEL_WORK");
            strQry.AppendLine(",EMPLOYEE_TEL_CELL");
            strQry.AppendLine(",EMPLOYEE_BIRTHDATE");

            if (parstrPayrollType == "S")
            {
                strQry.AppendLine(",ANNUAL_SALARY");
            }

            strQry.AppendLine(",TAX_DIRECTIVE_NO1");
            strQry.AppendLine(",TAX_DIRECTIVE_NO2");
            strQry.AppendLine(",TAX_DIRECTIVE_NO3");
            strQry.AppendLine(",TAX_DIRECTIVE_PERCENTAGE");
            strQry.AppendLine(",EMPLOYEE_EMAIL");

            //strQry.AppendLine(",EMPLOYEE_ACCUM_TAX_PAYOUT_DATE");
            //strQry.AppendLine(",ACCUM_TAX_PAYOUT_AMOUNT");
            strQry.AppendLine(",EMPLOYEE_TAX_NO");
            strQry.AppendLine(",EMPLOYEE_TAX_OFFICE_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LEAVE_SHIFT_NO");
            strQry.AppendLine(",EMPLOYEE_NUMBER_CHEQUES");
            strQry.AppendLine(",NATURE_PERSON_NO");

            strQry.AppendLine(",DEPARTMENT_NO");
            strQry.AppendLine(",OCCUPATION_NO");

            strQry.AppendLine(",RACE_NO");
            strQry.AppendLine(",MARITAL_STATUS_NO");
            strQry.AppendLine(",NUMBER_MEDICAL_AID_DEPENDENTS");

            //ELR 2014-05-01
            strQry.AppendLine(",MEDICAL_AID_DISABILITY_IND");
            //ELR 2017-09-23
            strQry.AppendLine(",EMAIL_VIA_PAYSLIP_IND");
            //2017-09-29
            strQry.AppendLine(",USE_EMPLOYEE_NO_IND");
            strQry.AppendLine(",EMPLOYEE_PIN");
            strQry.AppendLine(",EMPLOYEE_RFID_CARD_NO");
            
            strQry.AppendLine(",GENDER_IND");

            strQry.AppendLine(",BANK_ACCOUNT_TYPE_NO");
            strQry.AppendLine(",BANK_ACCOUNT_RELATIONSHIP_TYPE_NO");

            strQry.AppendLine(",BRANCH_CODE");
            strQry.AppendLine(",BRANCH_DESC");
            strQry.AppendLine(",ACCOUNT_NO");

            strQry.AppendLine(",ACCOUNT_NAME");
            strQry.AppendLine(",TAX_TYPE_IND");

            strQry.AppendLine(",OCCUPATION_LEVEL_NO");
            strQry.AppendLine(",OCCUPATION_CATEGORY_NO");
            strQry.AppendLine(",OCCUPATION_FUNCTION_NO");
            strQry.AppendLine(",DISABLED_IND");

            strQry.AppendLine(",E.USE_RES_ADDR_COMPANY_IND");
            strQry.AppendLine(",E.USE_WORK_TEL_IND");

            strQry.AppendLine(",NATURE_OF_DISABILITY");
            strQry.AppendLine(",SKILLS_EQUITY_PROVINCE_NO");

            strQry.AppendLine(",EMPLOYEE_PASSPORT_NO");
            strQry.AppendLine(",EMPLOYEE_PASSPORT_COUNTRY_CODE");


            //User who added Record
            strQry.AppendLine(",USER_NO_NEW_RECORD");

            //Return strings for field that need to be Initialised to Zero
            clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE", ref strQry, ref strFieldNamesInitialised, parInt64CompanyNo);

            strQry.AppendLine(")");
            strQry.AppendLine(" VALUES ");

            strQry.AppendLine("(" + parDataSet.Tables["Employee"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine("," + intEmployeeNo);

            if (strEmployeeCode != "")
            {
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeCode));
            }
            else
            {
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_CODE"].ToString()));
            }

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_INITIALS"].ToString()));

            //2017-02-09
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_3RD_PARTY_CODE"].ToString()));

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_ID_NO"].ToString()));

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_UNIT_NUMBER"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_COMPLEX"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_STREET_NUMBER"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_STREET_NAME"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_SUBURB"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_CITY"].ToString()));

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_CODE"].ToString()));

            //ELR - 20150214
            if (parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_OPTION_IND"].ToString() == "")
            {
                //Same as Residential
                parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_OPTION_IND"] = "R";
            }
                     
            //ELR - 20150214
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_OPTION_IND"].ToString()));
            
            if (parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_OPTION_IND"].ToString() == "R")
            {
                //EMPLOYEE_POST_UNIT_NUMBER
                strQry.AppendLine(",NULL ");
                //EMPLOYEE_POST_COMPLEX
                strQry.AppendLine(",NULL ");
                //EMPLOYEE_POST_STREET_NUMBER
                strQry.AppendLine(",NULL ");
                //EMPLOYEE_POST_STREET_NAME
                strQry.AppendLine(",NULL ");
                //EMPLOYEE_POST_SUBURB
                strQry.AppendLine(",NULL ");
                //EMPLOYEE_POST_CITY
                strQry.AppendLine(",NULL ");
                //EMPLOYEE_POST_CODE
                strQry.AppendLine(",NULL ");
                //EMPLOYEE_POST_COUNTRY_CODE2
                strQry.AppendLine(",NULL ");
            }
            else
            {
                if (parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_OPTION_IND"].ToString() == "S")
                {
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_UNIT_NUMBER"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_COMPLEX"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_STREET_NUMBER"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_STREET_NAME"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_SUBURB"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_CITY"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_CODE"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_COUNTRY_CODE2"].ToString()));
                }
                else
                {
                    //P=PO Box B=Private Bag 
                    //EMPLOYEE_POST_UNIT_NUMBER
                    strQry.AppendLine(",NULL ");
                    //EMPLOYEE_POST_COMPLEX
                    strQry.AppendLine(",NULL ");
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_STREET_NUMBER"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_STREET_NAME"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_SUBURB"].ToString()));
                    //EMPLOYEE_POST_CITY
                    strQry.AppendLine(",NULL ");
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_CODE"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_POST_COUNTRY_CODE2"].ToString()));
                }
            }

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_HOME"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_WORK"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_CELL"].ToString()));

            strQry.AppendLine(",'" + Convert.ToDateTime(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_BIRTHDATE"]).ToString("yyyy-MM-dd") + "'");

            if (parstrPayrollType == "S")
            {
                strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["ANNUAL_SALARY"].ToString());
            }

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["TAX_DIRECTIVE_NO1"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["TAX_DIRECTIVE_NO2"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["TAX_DIRECTIVE_NO3"].ToString()));
            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["TAX_DIRECTIVE_PERCENTAGE"].ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_EMAIL"].ToString()));

            //			if (parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_ACCUM_TAX_PAYOUT_DATE"].ToString() == "")
            //			{
            //				strQry.AppendLine(",Null");
            //			}
            //			else
            //			{
            //				strQry.AppendLine(",'" + Convert.ToDateTime(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_ACCUM_TAX_PAYOUT_DATE"]).ToString("yyyy-MM-dd") + "'");
            //			}

            //strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["ACCUM_TAX_PAYOUT_AMOUNT"].ToString();
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TAX_NO"].ToString()));
            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TAX_OFFICE_NO"].ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["LEAVE_SHIFT_NO"].ToString());
            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NUMBER_CHEQUES"].ToString());
            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["NATURE_PERSON_NO"].ToString());

            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["DEPARTMENT_NO"].ToString());
            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_NO"].ToString());

            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["RACE_NO"].ToString());
            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["MARITAL_STATUS_NO"].ToString());
            
            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["NUMBER_MEDICAL_AID_DEPENDENTS"].ToString());

            //ELR 2014-05-01
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["MEDICAL_AID_DISABILITY_IND"].ToString()));

            //ELR 2017-09-23
            if (parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_EMAIL"].ToString().Trim() == "")
            {
                strQry.AppendLine(",'N'");
            }
            else
            {
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMAIL_VIA_PAYSLIP_IND"].ToString()));
            }
            
            //ELR 2017-09-23
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["USE_EMPLOYEE_NO_IND"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_PIN"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RFID_CARD_NO"].ToString()));
            
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["GENDER_IND"].ToString()));

            if (parDataSet.Tables["Employee"].Rows[0]["BANK_ACCOUNT_TYPE_NO"] != System.DBNull.Value)
            {
                strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["BANK_ACCOUNT_TYPE_NO"].ToString());
            }
            else
            {
                //None
                strQry.AppendLine(",0");
            }

            if (parDataSet.Tables["Employee"].Rows[0]["BANK_ACCOUNT_RELATIONSHIP_TYPE_NO"] != System.DBNull.Value)
            {
                strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["BANK_ACCOUNT_RELATIONSHIP_TYPE_NO"].ToString());
            }
            else
            {
                strQry.AppendLine(",NULL");
            }

            if (parDataSet.Tables["Employee"].Rows[0]["BRANCH_CODE"] != System.DBNull.Value)
            {
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["BRANCH_CODE"].ToString()));
            }
            else
            {
                strQry.AppendLine(",Null");
            }

            if (parDataSet.Tables["Employee"].Rows[0]["BRANCH_DESC"] != System.DBNull.Value)
            {
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["BRANCH_DESC"].ToString()));
            }
            else
            {
                strQry.AppendLine(",Null");
            }

            if (parDataSet.Tables["Employee"].Rows[0]["ACCOUNT_NO"] != System.DBNull.Value)
            {
                strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["ACCOUNT_NO"].ToString());
            }
            else
            {
                strQry.AppendLine(",Null");
            }


            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["ACCOUNT_NAME"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["TAX_TYPE_IND"].ToString()));


            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_LEVEL_NO"].ToString());
            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_CATEGORY_NO"].ToString());
            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_FUNCTION_NO"].ToString());

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["DISABLED_IND"].ToString()));

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["USE_RES_ADDR_COMPANY_IND"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["USE_WORK_TEL_IND"].ToString()));

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["NATURE_OF_DISABILITY"].ToString()));
            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["SKILLS_EQUITY_PROVINCE_NO"].ToString());

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_PASSPORT_NO"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_PASSPORT_COUNTRY_CODE"].ToString()));

            //User who added Record
            strQry.AppendLine("," + parint64CurrentUserNo);

            strQry.Append(strFieldNamesInitialised);
            strQry.AppendLine(")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            if (parDataSet.Tables["EmployeeEarning"].Rows.Count > 0)
            {
                Save_Employee_Earnings(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeeEarning"], intEmployeeNo);
            }

            if (parDataSet.Tables["EmployeeDeduction"].Rows.Count > 0)
            {
                Save_Employee_Deductions(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeeDeduction"], intEmployeeNo);
            }

            if (parDataSet.Tables["EmployeeLeave"].Rows.Count > 0)
            {
                Save_Employee_Leave(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeeLeave"], intEmployeeNo);
            }

            if (parDataSet.Tables["EmployeeLoan"].Rows.Count > 0)
            {
                Save_Employee_Loans(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeeLoan"], intEmployeeNo);
            }

            if (parDataSet.Tables["EmployeePayCategory"].Rows.Count > 0)
            {
                Save_Employee_PayCategory(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeePayCategory"], intEmployeeNo);
            }

            if (parDataSet.Tables["EmployeeFingerTemplate"].Rows.Count > 0)
            {
                Save_Employee_FingerTemplate(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeeFingerTemplate"], intEmployeeNo);
            }

            parDataSet.Dispose();
            parDataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return intEmployeeNo;
        }

        public int Insert_New_Record_TimeAttend(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, byte[] parbyteDataSet, string parstrPayrollType)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
            string strEmployeeCode = "";
            StringBuilder strFieldNamesInitialised = new StringBuilder();

            int intNumberOfEmployeePayCategoryFields = 0;
            int intEmployeeNo = -1;

            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT MAX(EMPLOYEE_NO) AS MAX_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo.ToString());

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
            {
                intEmployeeNo = 1;
            }
            else
            {
                intEmployeeNo = Convert.ToInt32(DataSet.Tables[0].Rows[0]["MAX_NO"]) + 1;
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT GENERATE_EMPLOYEE_NUMBER_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo.ToString());

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp1", parInt64CompanyNo);

            if (DataSet.Tables["Temp1"].Rows[0]["GENERATE_EMPLOYEE_NUMBER_IND"].ToString() == "N")
            {
                strEmployeeCode = intEmployeeNo.ToString("00000");
            }

            DataSet.Dispose();
            DataSet = null;

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE ");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",EMPLOYEE_CODE");
            strQry.AppendLine(",EMPLOYEE_NAME");
            strQry.AppendLine(",EMPLOYEE_SURNAME");
            
            strQry.AppendLine(",EMPLOYEE_TEL_HOME");
            strQry.AppendLine(",EMPLOYEE_TEL_WORK");
            strQry.AppendLine(",EMPLOYEE_TEL_CELL");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",OCCUPATION_NO");
            strQry.AppendLine(",DEPARTMENT_NO");
            strQry.AppendLine(",USE_WORK_TEL_IND");
            strQry.AppendLine(",EMPLOYEE_3RD_PARTY_CODE");

            //2017-09-29
            strQry.AppendLine(",USE_EMPLOYEE_NO_IND");
            strQry.AppendLine(",EMPLOYEE_PIN");
            strQry.AppendLine(",EMPLOYEE_RFID_CARD_NO");

            //User who added Record
            strQry.AppendLine(",USER_NO_NEW_RECORD");

            //Return strings for field that need to be Initialised to Zero
            clsDBConnectionObjects.Initialise_DataSet_Numeric_Fields("EMPLOYEE", ref strQry, ref strFieldNamesInitialised, parInt64CompanyNo);

            strQry.AppendLine(")");
            strQry.AppendLine(" VALUES ");

            strQry.AppendLine("(" + parDataSet.Tables["Employee"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine("," + intEmployeeNo);

            if (strEmployeeCode != "")
            {
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strEmployeeCode));
            }
            else
            {
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_CODE"].ToString()));
            }

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_HOME"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_WORK"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_CELL"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_NO"].ToString());
            strQry.AppendLine("," + parDataSet.Tables["Employee"].Rows[0]["DEPARTMENT_NO"].ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["USE_WORK_TEL_IND"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_3RD_PARTY_CODE"].ToString()));

            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["USE_EMPLOYEE_NO_IND"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_PIN"].ToString()));
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RFID_CARD_NO"].ToString()));
            
            //User who added Record
            strQry.AppendLine("," + parint64CurrentUserNo);

            strQry.Append(strFieldNamesInitialised);

            strQry.AppendLine(")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            if (parDataSet.Tables["EmployeePayCategory"].Rows.Count > 0)
            {
                Save_Employee_PayCategory(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeePayCategory"], intEmployeeNo);
            }

            if (parDataSet.Tables["EmployeeFingerTemplate"].Rows.Count > 0)
            {
                Save_Employee_FingerTemplate(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeeFingerTemplate"], intEmployeeNo);
            }

            parDataSet.Dispose();
            parDataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return intEmployeeNo;
        }

        public byte[] Save_Employee_Deductions(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, DataTable parDataTable, int parintEmployeeNo)
        {
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();
            DataRow drDataRow;

            DataSet = Get_Deduction(parInt64CompanyNo, -1,"W");

            int intTieBreaker = -1;

            for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
            {
                if (parDataTable.Rows[intRow].RowState == DataRowState.Deleted)
                {
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                    strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE() ");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE", DataRowVersion.Original].ToString()));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                    strQry.AppendLine(" AND DEDUCTION_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND TIE_BREAKER = " + Convert.ToInt32(parDataTable.Rows[intRow]["TIE_BREAKER", DataRowVersion.Original]));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                }
                else
                {
                    if (parDataTable.Rows[intRow].RowState == DataRowState.Added)
                    {
                        if (DataSet.Tables["Temp"] != null)
                        {
                            DataSet.Tables.Remove("Temp");
                        }

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" MAX(TIE_BREAKER) AS MAX_NO");
                        strQry.AppendLine(" FROM ");
                        strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                        strQry.AppendLine(" AND DEDUCTION_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_NO"]));
                        strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"]));

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", Convert.ToInt64(parDataTable.Rows[intRow]["COMPANY_NO"]));

                        if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
                        {
                            intTieBreaker = 1;
                        }
                        else
                        {
                            intTieBreaker = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
                        }

                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",DEDUCTION_NO");
                        strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                        strQry.AppendLine(",TIE_BREAKER");
                        strQry.AppendLine(",DEDUCTION_TYPE_IND");
                        strQry.AppendLine(",DEDUCTION_VALUE");
                        strQry.AppendLine(",DEDUCTION_MIN_VALUE");
                        strQry.AppendLine(",DEDUCTION_MAX_VALUE");
                        strQry.AppendLine(",DEDUCTION_PERIOD_IND");
                        strQry.AppendLine(",DEDUCTION_DAY_VALUE");
                        strQry.AppendLine(",DATETIME_NEW_RECORD");
                        strQry.AppendLine(",USER_NO_NEW_RECORD)");
                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("(" + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine("," + parintEmployeeNo);
                        strQry.AppendLine("," + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_NO"]));
                        strQry.AppendLine("," + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"]));
                        strQry.AppendLine("," + intTieBreaker);
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["DEDUCTION_TYPE_IND"].ToString()));
                        strQry.AppendLine("," + Convert.ToDouble(parDataTable.Rows[intRow]["DEDUCTION_VALUE"]));
                        strQry.AppendLine("," + Convert.ToDouble(parDataTable.Rows[intRow]["DEDUCTION_MIN_VALUE"]));
                        strQry.AppendLine("," + Convert.ToDouble(parDataTable.Rows[intRow]["DEDUCTION_MAX_VALUE"]));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["DEDUCTION_PERIOD_IND"].ToString()));
                        strQry.AppendLine("," + Convert.ToDouble(parDataTable.Rows[intRow]["DEDUCTION_DAY_VALUE"]));
                        strQry.AppendLine(",GETDATE()");
                        strQry.AppendLine("," + parint64CurrentUserNo + ")");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                        drDataRow = DataSet.Tables["EmployeeDeduction"].NewRow();

                        drDataRow["COMPANY_NO"] = +Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]);
                        drDataRow["PAY_CATEGORY_TYPE"] = parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString();
                        drDataRow["EMPLOYEE_NO"] = parintEmployeeNo;
                        drDataRow["DEDUCTION_NO"] = parDataTable.Rows[intRow]["DEDUCTION_NO"].ToString();
                        drDataRow["DEDUCTION_SUB_ACCOUNT_NO"] = parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString();
                        drDataRow["TIE_BREAKER"] = intTieBreaker.ToString();
                        drDataRow["DEDUCTION_TYPE_IND"] = parDataTable.Rows[intRow]["DEDUCTION_TYPE_IND"].ToString();
                        drDataRow["DEDUCTION_VALUE"] = parDataTable.Rows[intRow]["DEDUCTION_VALUE"].ToString();
                        drDataRow["DEDUCTION_MIN_VALUE"] = parDataTable.Rows[intRow]["DEDUCTION_MIN_VALUE"].ToString();
                        drDataRow["DEDUCTION_MAX_VALUE"] = parDataTable.Rows[intRow]["DEDUCTION_MAX_VALUE"].ToString();
                        drDataRow["DEDUCTION_PERIOD_IND"] = parDataTable.Rows[intRow]["DEDUCTION_PERIOD_IND"].ToString();
                        drDataRow["DEDUCTION_DAY_VALUE"] = parDataTable.Rows[intRow]["DEDUCTION_DAY_VALUE"].ToString();
                        drDataRow["DEDUCTION_LOAN_TYPE_IND"] = parDataTable.Rows[intRow]["DEDUCTION_LOAN_TYPE_IND"].ToString();
                        drDataRow["DEDUCTION_SUB_ACCOUNT_COUNT"] = parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_COUNT"].ToString();
                        drDataRow["LOAN_OUTSTANDING"] = parDataTable.Rows[intRow]["LOAN_OUTSTANDING"].ToString();

                        DataSet.Tables["EmployeeDeduction"].Rows.Add(drDataRow);
                    }
                    else
                    {
                        if (parDataTable.Rows[intRow].RowState == DataRowState.Modified)
                        {
                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" DEDUCTION_TYPE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["DEDUCTION_TYPE_IND"].ToString()));
                            strQry.AppendLine(",DEDUCTION_VALUE = " + Convert.ToDouble(parDataTable.Rows[intRow]["DEDUCTION_VALUE"]));
                            strQry.AppendLine(",DEDUCTION_MIN_VALUE = " + Convert.ToDouble(parDataTable.Rows[intRow]["DEDUCTION_MIN_VALUE"]));
                            strQry.AppendLine(",DEDUCTION_MAX_VALUE = " + Convert.ToDouble(parDataTable.Rows[intRow]["DEDUCTION_MAX_VALUE"]));
                            strQry.AppendLine(",DEDUCTION_PERIOD_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["DEDUCTION_PERIOD_IND"].ToString()));
                            strQry.AppendLine(",DEDUCTION_DAY_VALUE = " + Convert.ToDouble(parDataTable.Rows[intRow]["DEDUCTION_DAY_VALUE"]));
                            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                            strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                            strQry.AppendLine(" AND DEDUCTION_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_NO"]));
                            strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"]));
                            strQry.AppendLine(" AND TIE_BREAKER = " + parDataTable.Rows[intRow]["TIE_BREAKER"].ToString());

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                        }
                    }
                }
            }

            if (DataSet.Tables["Temp"] != null)
            {
                DataSet.Tables.Remove("Temp");
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Save_Deductions_Earning_Percentage(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, DataTable parDataTable, int parintEmployeeNo)
        {
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            int intTieBreaker = -1;

            for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
            {
                if (parDataTable.Rows[intRow].RowState == DataRowState.Deleted)
                {
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                    strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE() ");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE", DataRowVersion.Original].ToString()));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                    strQry.AppendLine(" AND DEDUCTION_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                }
                else
                {
                    if (parDataTable.Rows[intRow].RowState == DataRowState.Added)
                    {
                        if (DataSet.Tables["Temp"] != null)
                        {
                            DataSet.Tables.Remove("Temp");
                        }

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" MAX(TIE_BREAKER) AS MAX_NO");
                        strQry.AppendLine(" FROM ");
                        strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                        strQry.AppendLine(" AND DEDUCTION_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_NO"]));
                        strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"]));
                        strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_NO"]));

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", Convert.ToInt64(parDataTable.Rows[intRow]["COMPANY_NO"]));

                        if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
                        {
                            intTieBreaker = 1;
                        }
                        else
                        {
                            intTieBreaker = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
                        }

                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",DEDUCTION_NO");
                        strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                        strQry.AppendLine(",EARNING_NO");
                        strQry.AppendLine(",TIE_BREAKER");
                        strQry.AppendLine(",DATETIME_NEW_RECORD");
                        strQry.AppendLine(",USER_NO_NEW_RECORD)");
                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("(" + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine("," + parintEmployeeNo);
                        strQry.AppendLine("," + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_NO"]));
                        strQry.AppendLine("," + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"]));
                        strQry.AppendLine("," + Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_NO"]));
                        strQry.AppendLine("," + intTieBreaker);
                        strQry.AppendLine(",GETDATE()");
                        strQry.AppendLine("," + parint64CurrentUserNo + ")");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    }
                }
            }

            if (DataSet.Tables["Temp"] != null)
            {
                DataSet.Tables.Remove("Temp");
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }

        public byte[] Save_Employee_Earnings(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, DataTable parDataTable, int parintEmployeeNo)
        {
            StringBuilder strQry = new StringBuilder();
            int intTieBreaker = 0;
            DataSet DataSet = new DataSet();

            DataRow drDataRow;

            //Used For DownLoad
            DataSet = Get_Earning(parInt64CompanyNo, -1,"W");

            for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
            {
                if (parDataTable.Rows[intRow].RowState == DataRowState.Deleted)
                {
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                    strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE", DataRowVersion.Original].ToString()));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                    strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND TIE_BREAKER = " + Convert.ToInt32(parDataTable.Rows[intRow]["TIE_BREAKER", DataRowVersion.Original]));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                }
                else
                {
                    if (parDataTable.Rows[intRow].RowState == DataRowState.Added)
                    {
                        if (DataSet.Tables["Temp"] != null)
                        {
                            DataSet.Tables.Remove("Temp");
                        }

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" MAX(TIE_BREAKER) AS MAX_NO");
                        strQry.AppendLine(" FROM ");
                        strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                        strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_NO"]));

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", Convert.ToInt64(parDataTable.Rows[intRow]["COMPANY_NO"]));

                        if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
                        {
                            intTieBreaker = 1;
                        }
                        else
                        {
                            intTieBreaker = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
                        }


                        drDataRow = DataSet.Tables["EmployeeEarning"].NewRow();

                        drDataRow["COMPANY_NO"] = +Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]);
                        drDataRow["PAY_CATEGORY_TYPE"] = parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString();
                        drDataRow["EMPLOYEE_NO"] = parintEmployeeNo;
                        drDataRow["TIE_BREAKER"] = intTieBreaker.ToString();

                        if (parDataTable.Rows[intRow]["IRP5_CODE"].ToString() != "")
                        {
                            drDataRow["IRP5_CODE"] = Convert.ToInt32(parDataTable.Rows[intRow]["IRP5_CODE"]);
                        }

                        drDataRow["EARNING_NO"] = parDataTable.Rows[intRow]["EARNING_NO"].ToString();
                        drDataRow["EARNING_PERIOD_IND"] = parDataTable.Rows[intRow]["EARNING_PERIOD_IND"].ToString();
                        drDataRow["EARNING_TYPE_IND"] = parDataTable.Rows[intRow]["EARNING_TYPE_IND"].ToString();
                        drDataRow["EARNING_DAY_VALUE"] = parDataTable.Rows[intRow]["EARNING_DAY_VALUE"].ToString();
                        drDataRow["AMOUNT"] = parDataTable.Rows[intRow]["AMOUNT"].ToString();
                        
                        DataSet.Tables["EmployeeEarning"].Rows.Add(drDataRow);

                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",EARNING_NO");
                        strQry.AppendLine(",TIE_BREAKER");

                        strQry.AppendLine(",EARNING_TYPE_IND");
                        strQry.AppendLine(",EARNING_PERIOD_IND");
                        strQry.AppendLine(",AMOUNT");

                        strQry.AppendLine(",EARNING_DAY_VALUE");

                        strQry.AppendLine(",DATETIME_NEW_RECORD");
                        strQry.AppendLine(",USER_NO_NEW_RECORD)");
                        strQry.AppendLine(" VALUES");
                        strQry.AppendLine("(" + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine("," + parintEmployeeNo);
                        strQry.AppendLine("," + Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_NO"]));
                        strQry.AppendLine("," + intTieBreaker);

                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["EARNING_TYPE_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["EARNING_PERIOD_IND"].ToString()));
                        strQry.AppendLine("," + Convert.ToDouble(parDataTable.Rows[intRow]["AMOUNT"]));
                        strQry.AppendLine("," + parDataTable.Rows[intRow]["EARNING_DAY_VALUE"].ToString());

                        strQry.AppendLine(",GETDATE()");
                        strQry.AppendLine("," + parint64CurrentUserNo + ")");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    }
                    else
                    {
                        if (parDataTable.Rows[intRow].RowState == DataRowState.Modified)
                        {
                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" EARNING_TYPE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["EARNING_TYPE_IND"].ToString()));
                            strQry.AppendLine(",EARNING_PERIOD_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["EARNING_PERIOD_IND"].ToString()));
                            strQry.AppendLine(",EARNING_DAY_VALUE = " + Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_DAY_VALUE"]));
                            
                            strQry.AppendLine(",AMOUNT = " + Convert.ToDouble(parDataTable.Rows[intRow]["AMOUNT"]));
                            
                            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                            strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                            strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_NO"]));
                            strQry.AppendLine(" AND TIE_BREAKER = " + Convert.ToInt32(parDataTable.Rows[intRow]["TIE_BREAKER"]));

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                        }
                    }
                }
            }

            if (DataSet.Tables["Temp"] != null)
            {
                DataSet.Tables.Remove("Temp");
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Save_Employee_Loans(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, DataTable parDataTable, int parintEmployeeNo)
        {
            object[] objAdd = new object[3];
            DataSet DataSet = new DataSet();
            DataView DataView;

            DataTable myDataTable = parDataTable.Clone();
            DataSet.Tables.Add(myDataTable);

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DEDUCTION_NO ");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO ");
            strQry.AppendLine(",ISNULL(MAX(LOAN_REC_NO),0) + 1 AS MAX_LOAN_REC_NO");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.LOANS");

            if (parDataTable.Rows[0].RowState == DataRowState.Deleted)
            {
                strQry.AppendLine(" WHERE COMPANY_NO = " + parDataTable.Rows[0]["COMPANY_NO", System.Data.DataRowVersion.Original].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[0]["PAY_CATEGORY_TYPE", System.Data.DataRowVersion.Original].ToString()));
            }
            else
            {
                strQry.AppendLine(" WHERE COMPANY_NO = " + parDataTable.Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            }

            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" DEDUCTION_NO ");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
            {
                if (parDataTable.Rows[intRow].RowState == DataRowState.Deleted)
                {
                    //Delete
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LOANS");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                    strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO", System.Data.DataRowVersion.Original]));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                    strQry.AppendLine(" AND DEDUCTION_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_NO", System.Data.DataRowVersion.Original]));
                    strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO", System.Data.DataRowVersion.Original]));
                    strQry.AppendLine(" AND LOAN_REC_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["LOAN_REC_NO", System.Data.DataRowVersion.Original]));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                }
                else
                {
                    if (parDataTable.Rows[intRow].RowState == DataRowState.Added)
                    {
                    Save_Loan_Find_Row_Continue:

                        DataView = null;
                        DataView = new DataView(DataSet.Tables["Temp"],
                        "DEDUCTION_NO = " + parDataTable.Rows[intRow]["DEDUCTION_NO"].ToString() + " AND DEDUCTION_SUB_ACCOUNT_NO = " + parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"].ToString(),
                        "",
                        DataViewRowState.CurrentRows);

                        if (DataView.Count > 0)
                        {
                            goto Save_Loan_Continue;
                        }

                        objAdd[0] = Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_NO"]);
                        objAdd[1] = Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"]);
                        objAdd[2] = 1;

                        DataRow drvDataRow = DataSet.Tables["Temp"].Rows.Add(objAdd);

                        goto Save_Loan_Find_Row_Continue;

                    Save_Loan_Continue:

                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LOANS");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",DEDUCTION_NO");
                        strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                        strQry.AppendLine(",LOAN_REC_NO");
                        strQry.AppendLine(",LOAN_DESC");
                        strQry.AppendLine(",LOAN_AMOUNT_PAID");
                        strQry.AppendLine(",LOAN_AMOUNT_RECEIVED");
                        strQry.AppendLine(",PROCESS_NO");
                        strQry.AppendLine(",DATETIME_NEW_RECORD");
                        strQry.AppendLine(",USER_NO_NEW_RECORD)");
                        strQry.AppendLine(" VALUES");
                        strQry.AppendLine("(" + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine("," + parintEmployeeNo);
                        strQry.AppendLine("," + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_NO"]));
                        strQry.AppendLine("," + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"]));
                        strQry.AppendLine("," + DataView[0]["MAX_LOAN_REC_NO"].ToString());
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["LOAN_DESC"].ToString()));
                        strQry.AppendLine("," + Convert.ToDouble(parDataTable.Rows[intRow]["LOAN_AMOUNT_PAID"]));
                        strQry.AppendLine("," + Convert.ToDouble(parDataTable.Rows[intRow]["LOAN_AMOUNT_RECEIVED"]));
                        strQry.AppendLine("," + parDataTable.Rows[intRow]["PROCESS_NO"].ToString());
                        strQry.AppendLine(",GETDATE()");
                        strQry.AppendLine("," + parint64CurrentUserNo + ")");

                        //Get Row From Client - Update LOAN_REC_NO for Return to Client
                        DataSet.Tables["EmployeeLoan"].ImportRow(parDataTable.Rows[intRow]);
                        DataSet.Tables["EmployeeLoan"].Rows[DataSet.Tables["EmployeeLoan"].Rows.Count - 1]["LOAN_REC_NO"] = Convert.ToInt16(DataView[0]["MAX_LOAN_REC_NO"]);

                        DataView[0]["MAX_LOAN_REC_NO"] = Convert.ToInt16(DataView[0]["MAX_LOAN_REC_NO"]) + 1;

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    }
                    else
                    {
                        if (parDataTable.Rows[intRow].RowState == DataRowState.Modified)
                        {
                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LOANS");
                            strQry.AppendLine(" SET");
                            strQry.AppendLine(" LOAN_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["LOAN_DESC"].ToString()));
                            strQry.AppendLine(",LOAN_AMOUNT_PAID = " + Convert.ToDouble(parDataTable.Rows[intRow]["LOAN_AMOUNT_PAID"]));
                            strQry.AppendLine(",LOAN_AMOUNT_RECEIVED = " + Convert.ToDouble(parDataTable.Rows[intRow]["LOAN_AMOUNT_RECEIVED"]));
                            strQry.AppendLine(",PROCESS_NO = " + parDataTable.Rows[intRow]["PROCESS_NO"].ToString());
                            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                            strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                            strQry.AppendLine(" AND DEDUCTION_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_NO"]));
                            strQry.AppendLine(" AND DEDUCTION_SUB_ACCOUNT_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["DEDUCTION_SUB_ACCOUNT_NO"]));
                            strQry.AppendLine(" AND LOAN_REC_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["LOAN_REC_NO"]));

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                        }
                    }
                }
            }

            DataSet.Tables.Remove("Temp");

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Save_Employee_PayCategory(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, DataTable parDataTable, int parintEmployeeNo)
        {
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
            {
                if (parDataTable.Rows[intRow].RowState == DataRowState.Deleted)
                {
                    //Possible Delete
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                    strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO", System.Data.DataRowVersion.Original]));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["PAY_CATEGORY_NO", System.Data.DataRowVersion.Original]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE", System.Data.DataRowVersion.Original].ToString()));
                    strQry.AppendLine(" AND TIE_BREAKER = " + Convert.ToInt32(parDataTable.Rows[intRow]["TIE_BREAKER", System.Data.DataRowVersion.Original]));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                }
                else
                {
                    if (parDataTable.Rows[intRow].RowState == DataRowState.Added)
                    {
                        if (DataSet.Tables["Temp"] != null)
                        {
                            DataSet.Tables.Remove("Temp");
                        }

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" ISNULL(MAX(TIE_BREAKER),0) + 1 AS MAX_NO");
                        strQry.AppendLine(" FROM ");
                        strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataTable.Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",TIE_BREAKER");
                        strQry.AppendLine(",HOURLY_RATE");
                        strQry.AppendLine(",DEFAULT_IND");
                        strQry.AppendLine(",LEAVE_DAY_RATE_DECIMAL");
                        strQry.AppendLine(",DATETIME_NEW_RECORD");
                        strQry.AppendLine(",USER_NO_NEW_RECORD)");
                        strQry.AppendLine(" VALUES");
                        strQry.AppendLine("(" + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine("," + parintEmployeeNo);
                        strQry.AppendLine("," + Convert.ToInt32(parDataTable.Rows[intRow]["PAY_CATEGORY_NO"]));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine("," + Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]));

                        if (parDataTable.Rows[intRow]["HOURLY_RATE"] == System.DBNull.Value)
                        {
                            //From Time Attendance Internet Layer
                            strQry.AppendLine(",0");
                            strQry.AppendLine(",'Y'");
                            strQry.AppendLine(",0");
                        }
                        else
                        {

                            strQry.AppendLine("," + Convert.ToDouble(parDataTable.Rows[intRow]["HOURLY_RATE"]));
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["DEFAULT_IND"].ToString()));
                            strQry.AppendLine("," + Convert.ToDouble(parDataTable.Rows[intRow]["LEAVE_DAY_RATE_DECIMAL"]));
                        }
                        strQry.AppendLine(",GETDATE()");
                        strQry.AppendLine("," + parint64CurrentUserNo + ")");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    }
                    else
                    {
                        if (parDataTable.Rows[intRow].RowState == DataRowState.Modified)
                        {
                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY");
                            strQry.AppendLine(" SET");
                            strQry.AppendLine(" DEFAULT_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["DEFAULT_IND"].ToString()));
                            strQry.AppendLine(",HOURLY_RATE = " + Convert.ToDouble(parDataTable.Rows[intRow]["HOURLY_RATE"]));
                            strQry.AppendLine(",LEAVE_DAY_RATE_DECIMAL = " + Convert.ToDouble(parDataTable.Rows[intRow]["LEAVE_DAY_RATE_DECIMAL"]));
                            strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                            strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                            strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["PAY_CATEGORY_NO"]));
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(" AND TIE_BREAKER = " + Convert.ToInt32(parDataTable.Rows[intRow]["TIE_BREAKER"]));

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                        }
                    }
                }
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }

        public void Save_Employee_FingerTemplate(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, DataTable parDataTable, int parintEmployeeNo)
        {
            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
            {
                if (parDataTable.Rows[intRow].RowState == DataRowState.Added)
                {
                    //2017-04-29 - NB New Template Will Replace Old
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                    strQry.AppendLine(" AND FINGER_NO = " + parDataTable.Rows[intRow]["FINGER_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",FINGER_NO");
                    strQry.AppendLine(",FINGER_TEMPLATE");
                    strQry.AppendLine(",CREATION_DATETIME) ");

                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("(" + parInt64CompanyNo);
                    strQry.AppendLine("," + parintEmployeeNo);
                    strQry.AppendLine("," + parDataTable.Rows[intRow]["FINGER_NO"].ToString());
                    strQry.AppendLine(",@FINGER_TEMPLATE");
                    strQry.AppendLine(",GETDATE())");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), (byte[])parDataTable.Rows[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE", parInt64CompanyNo);
                }
                else
                {
                    strQry.Clear();

                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo.ToString());
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataTable.Rows[intRow]["EMPLOYEE_NO", DataRowVersion.Original].ToString());
                    strQry.AppendLine(" AND FINGER_NO = " + parDataTable.Rows[intRow]["FINGER_NO", DataRowVersion.Original].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                }
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }

        public byte[] Save_Employee_Leave(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, DataTable parDataTable, int parintEmployeeNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            object[] objAdd = new object[2];

            DataTable myDataTable = parDataTable.Clone();
            DataSet.Tables.Add(myDataTable);

            for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
            {
                if (parDataTable.Rows[intRow].RowState == DataRowState.Deleted)
                {
                    strQry.Clear();
                    strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT");
                    strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO", System.Data.DataRowVersion.Original]));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                    strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_NO", System.Data.DataRowVersion.Original]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE", System.Data.DataRowVersion.Original].ToString()));
                    strQry.AppendLine(" AND LEAVE_REC_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["LEAVE_REC_NO", System.Data.DataRowVersion.Original]));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                }
                else
                {
                    if (parDataTable.Rows[intRow].RowState == DataRowState.Added)
                    {
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT");
                        strQry.AppendLine("(COMPANY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",EARNING_NO");
                      
                        strQry.AppendLine(",LEAVE_DESC");
                        
                        strQry.AppendLine(",LEAVE_FROM_DATE");
                        strQry.AppendLine(",LEAVE_TO_DATE");
                        
                        strQry.AppendLine(",PROCESS_NO");
                        
                        strQry.AppendLine(",LEAVE_OPTION");

                        strQry.AppendLine(",LEAVE_DAYS_DECIMAL");
                        strQry.AppendLine(",LEAVE_HOURS_DECIMAL)");
                     
                        strQry.AppendLine(" VALUES");
                        strQry.AppendLine("(" + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine("," + parintEmployeeNo);
                        strQry.AppendLine("," + Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_NO"]));
                      
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["LEAVE_DESC"].ToString()));
                        
                        strQry.AppendLine(",'" + Convert.ToDateTime(parDataTable.Rows[intRow]["LEAVE_FROM_DATE"]).ToString("yyyy-MM-dd") + "'");
                        strQry.AppendLine(",'" + Convert.ToDateTime(parDataTable.Rows[intRow]["LEAVE_TO_DATE"]).ToString("yyyy-MM-dd") + "'");
                     
                        
                        strQry.AppendLine("," + parDataTable.Rows[intRow]["PROCESS_NO"].ToString());
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["LEAVE_OPTION"].ToString()));

                        strQry.AppendLine(",ROUND(" + parDataTable.Rows[intRow]["LEAVE_DAYS_DECIMAL"].ToString() + ",2)");
                        strQry.AppendLine(",ROUND(" + parDataTable.Rows[intRow]["LEAVE_HOURS_DECIMAL"].ToString() + ",2))");

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                        if (DataSet.Tables["Temp"] != null)
                        {
                            DataSet.Tables.Remove("Temp");
                        }

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" MAX(LEAVE_REC_NO) AS MAX_LEAVE_REC_NO");
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                        
                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);
                
                        //Get Row From Client - Update LOAN_REC_NO for Return to Client
                        DataSet.Tables["EmployeeLeave"].ImportRow(parDataTable.Rows[intRow]);
                        DataSet.Tables["EmployeeLeave"].Rows[DataSet.Tables["EmployeeLeave"].Rows.Count - 1]["LEAVE_REC_NO"] = Convert.ToInt16(DataSet.Tables["Temp"].Rows[0]["MAX_LEAVE_REC_NO"]);
                    }
                    else
                    {
                        if (parDataTable.Rows[intRow].RowState == DataRowState.Modified)
                        {
                            strQry.Clear();
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT");
                            strQry.AppendLine(" SET");
                           
                            strQry.AppendLine(" PROCESS_NO = " + parDataTable.Rows[intRow]["PROCESS_NO"].ToString());
                            strQry.AppendLine(",LEAVE_OPTION = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["LEAVE_OPTION"].ToString()));
                            strQry.AppendLine(",LEAVE_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["LEAVE_DESC"].ToString()));
                            strQry.AppendLine(",LEAVE_FROM_DATE = ' " + Convert.ToDateTime(parDataTable.Rows[intRow]["LEAVE_FROM_DATE"]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(",LEAVE_TO_DATE = ' " + Convert.ToDateTime(parDataTable.Rows[intRow]["LEAVE_TO_DATE"]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine(",LEAVE_HOURS_DECIMAL = ROUND(" + Convert.ToDouble(parDataTable.Rows[intRow]["LEAVE_HOURS_DECIMAL"]) + ",2)");
                            strQry.AppendLine(",LEAVE_DAYS_DECIMAL = ROUND(" + Convert.ToDouble(parDataTable.Rows[intRow]["LEAVE_DAYS_DECIMAL"]) + ",2)");

                            strQry.AppendLine(" WHERE COMPANY_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["COMPANY_NO"]));
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                            strQry.AppendLine(" AND EARNING_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["EARNING_NO"]));
                            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataTable.Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                            strQry.AppendLine(" AND LEAVE_REC_NO = " + Convert.ToInt32(parDataTable.Rows[intRow]["LEAVE_REC_NO"]));

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                        }
                    }
                }
            }

            if (DataSet.Tables["Temp"] != null)
            {
                DataSet.Tables.Remove("Temp");
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Update_Record(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, byte[] parbyteDataSet, string parstrPayrollType)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            DataSet TempDataSet;
            byte[] bytCompress;
            DataTable DataTable = new DataTable();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 0 AS RETURN_CODE");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.COMPANY ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Employee"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReturnCode", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT EMPLOYEE_NO");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Employee"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString());

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Check", parInt64CompanyNo);

            if (DataSet.Tables["Check"].Rows.Count == 0)
            {
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

                //2015-10-06
                strQry.AppendLine(",EMPLOYEE_RES_COUNTRY_CODE2 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RES_COUNTRY_CODE2"].ToString()));
                
                //ELR - 20150214
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

                strQry.AppendLine(",EMPLOYEE_TEL_HOME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_HOME"].ToString()));
                strQry.AppendLine(",EMPLOYEE_TEL_WORK = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_WORK"].ToString()));
                strQry.AppendLine(",EMPLOYEE_TEL_CELL = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_CELL"].ToString()));

                strQry.AppendLine(",EMPLOYEE_BIRTHDATE = '" + Convert.ToDateTime(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_BIRTHDATE"]).ToString("yyyy-MM-dd") + "'");

                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(",ANNUAL_SALARY = " + parDataSet.Tables["Employee"].Rows[0]["ANNUAL_SALARY"].ToString());
                }

                strQry.AppendLine(",TAX_DIRECTIVE_NO1 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["TAX_DIRECTIVE_NO1"].ToString()));

                strQry.AppendLine(",TAX_DIRECTIVE_NO2 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["TAX_DIRECTIVE_NO2"].ToString()));
                strQry.AppendLine(",TAX_DIRECTIVE_NO3 = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["TAX_DIRECTIVE_NO3"].ToString()));

                strQry.AppendLine(",TAX_DIRECTIVE_PERCENTAGE = " + parDataSet.Tables["Employee"].Rows[0]["TAX_DIRECTIVE_PERCENTAGE"].ToString());
                strQry.AppendLine(",EMPLOYEE_EMAIL = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_EMAIL"].ToString()));

                //if (parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_ACCUM_TAX_PAYOUT_DATE"] != System.DBNull.Value)
                //{
                //	strQry.AppendLine(",EMPLOYEE_ACCUM_TAX_PAYOUT_DATE = '" + Convert.ToDateTime(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_ACCUM_TAX_PAYOUT_DATE"]).ToString("yyyy-MM-dd") + "'");
                //}
                //else
                //{
                //	strQry.AppendLine(",EMPLOYEE_ACCUM_TAX_PAYOUT_DATE = Null");
                //}
                //strQry.AppendLine(",ACCUM_TAX_PAYOUT_AMOUNT = " + parDataSet.Tables["Employee"].Rows[0]["ACCUM_TAX_PAYOUT_AMOUNT"].ToString();

                strQry.AppendLine(",EMPLOYEE_TAX_NO = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TAX_NO"].ToString()));
                strQry.AppendLine(",EMPLOYEE_TAX_OFFICE_NO = " + parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TAX_OFFICE_NO"].ToString());
                strQry.AppendLine(",LEAVE_SHIFT_NO = " + parDataSet.Tables["Employee"].Rows[0]["LEAVE_SHIFT_NO"].ToString());
                strQry.AppendLine(",EMPLOYEE_NUMBER_CHEQUES = " + parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NUMBER_CHEQUES"].ToString());
                strQry.AppendLine(",NATURE_PERSON_NO = " + parDataSet.Tables["Employee"].Rows[0]["NATURE_PERSON_NO"].ToString());
                strQry.AppendLine(",DEPARTMENT_NO = " + parDataSet.Tables["Employee"].Rows[0]["DEPARTMENT_NO"].ToString());
                strQry.AppendLine(",OCCUPATION_NO = " + parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_NO"].ToString());

                strQry.AppendLine(",RACE_NO = " + parDataSet.Tables["Employee"].Rows[0]["RACE_NO"].ToString());
                strQry.AppendLine(",MARITAL_STATUS_NO = " + parDataSet.Tables["Employee"].Rows[0]["MARITAL_STATUS_NO"].ToString());
                strQry.AppendLine(",NUMBER_MEDICAL_AID_DEPENDENTS = " + parDataSet.Tables["Employee"].Rows[0]["NUMBER_MEDICAL_AID_DEPENDENTS"].ToString());

                //ELR 2014-05-01
                strQry.AppendLine(",MEDICAL_AID_DISABILITY_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["MEDICAL_AID_DISABILITY_IND"].ToString()));
                
                //ELR 2017-02-09
                strQry.AppendLine(",EMPLOYEE_3RD_PARTY_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_3RD_PARTY_CODE"].ToString()));

                //ELR 2017-09-23
                if (parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_EMAIL"].ToString().Trim() == "")
                {
                    strQry.AppendLine(",EMAIL_VIA_PAYSLIP_IND = 'N'");
                }
                else
                {
                    strQry.AppendLine(",EMAIL_VIA_PAYSLIP_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMAIL_VIA_PAYSLIP_IND"].ToString()));
                }

                //ELR 2017-09-29
                strQry.AppendLine(",USE_EMPLOYEE_NO_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["USE_EMPLOYEE_NO_IND"].ToString()));
                strQry.AppendLine(",EMPLOYEE_PIN = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_PIN"].ToString()));
                strQry.AppendLine(",EMPLOYEE_RFID_CARD_NO = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RFID_CARD_NO"].ToString()));

                strQry.AppendLine(",GENDER_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["GENDER_IND"].ToString()));

                if (parDataSet.Tables["Employee"].Rows[0]["BANK_ACCOUNT_TYPE_NO"] != System.DBNull.Value)
                {
                    strQry.AppendLine(",BANK_ACCOUNT_TYPE_NO = " + parDataSet.Tables["Employee"].Rows[0]["BANK_ACCOUNT_TYPE_NO"].ToString());
                }
                else
                {
                    strQry.AppendLine(",BANK_ACCOUNT_TYPE_NO = 0");
                }

                if (parDataSet.Tables["Employee"].Rows[0]["BANK_ACCOUNT_RELATIONSHIP_TYPE_NO"] != System.DBNull.Value)
                {
                    strQry.AppendLine(",BANK_ACCOUNT_RELATIONSHIP_TYPE_NO = " + parDataSet.Tables["Employee"].Rows[0]["BANK_ACCOUNT_RELATIONSHIP_TYPE_NO"].ToString());
                }
                else
                {
                    strQry.AppendLine(",BANK_ACCOUNT_RELATIONSHIP_TYPE_NO = NULL");
                }

                if (parDataSet.Tables["Employee"].Rows[0]["BRANCH_CODE"] != System.DBNull.Value)
                {
                    strQry.AppendLine(",BRANCH_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["BRANCH_CODE"].ToString()));
                }
                else
                {
                    strQry.AppendLine(",BRANCH_CODE = Null");
                }

                if (parDataSet.Tables["Employee"].Rows[0]["BRANCH_DESC"] != System.DBNull.Value)
                {
                    strQry.AppendLine(",BRANCH_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["BRANCH_DESC"].ToString()));
                }
                else
                {
                    strQry.AppendLine(",BRANCH_DESC = Null");
                }

                if (parDataSet.Tables["Employee"].Rows[0]["ACCOUNT_NO"] != System.DBNull.Value)
                {
                    strQry.AppendLine(",ACCOUNT_NO = " + parDataSet.Tables["Employee"].Rows[0]["ACCOUNT_NO"].ToString());
                }
                else
                {
                    strQry.AppendLine(",ACCOUNT_NO = Null");
                }

                if (parDataSet.Tables["Employee"].Rows[0]["ETI_START_DATE"] != System.DBNull.Value)
                {
                    strQry.AppendLine(",ETI_START_DATE = '" + Convert.ToDateTime(parDataSet.Tables["Employee"].Rows[0]["ETI_START_DATE"]).ToString("yyyy-MM-dd") + "'");
                }
                else
                {
                    strQry.AppendLine(",ETI_START_DATE = Null");
                }

                strQry.AppendLine(",ACCOUNT_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["ACCOUNT_NAME"].ToString()));
                strQry.AppendLine(",TAX_TYPE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["TAX_TYPE_IND"].ToString()));

                if (parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_LEVEL_NO"] == System.DBNull.Value)
                {
                    strQry.AppendLine(",OCCUPATION_LEVEL_NO = 0");
                }
                else
                {
                    strQry.AppendLine(",OCCUPATION_LEVEL_NO = " + parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_LEVEL_NO"].ToString());
                }

                if (parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_CATEGORY_NO"] == System.DBNull.Value)
                {
                    strQry.AppendLine(",OCCUPATION_CATEGORY_NO = 0");
                }
                else
                {
                    strQry.AppendLine(",OCCUPATION_CATEGORY_NO = " + parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_CATEGORY_NO"].ToString());
                }

                if (parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_FUNCTION_NO"] == System.DBNull.Value)
                {
                    strQry.AppendLine(",OCCUPATION_FUNCTION_NO = 0");
                }
                else
                {
                    strQry.AppendLine(",OCCUPATION_FUNCTION_NO = " + parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_FUNCTION_NO"].ToString());
                }

                strQry.AppendLine(",DISABLED_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["DISABLED_IND"].ToString()));

                strQry.AppendLine(",USE_RES_ADDR_COMPANY_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["USE_RES_ADDR_COMPANY_IND"].ToString()));
                strQry.AppendLine(",USE_WORK_TEL_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["USE_WORK_TEL_IND"].ToString()));

                strQry.AppendLine(",NATURE_OF_DISABILITY = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["NATURE_OF_DISABILITY"].ToString()));

                if (parDataSet.Tables["Employee"].Rows[0]["SKILLS_EQUITY_PROVINCE_NO"] == System.DBNull.Value)
                {
                    strQry.AppendLine(",SKILLS_EQUITY_PROVINCE_NO = 0");
                }
                else
                {
                    strQry.AppendLine(",SKILLS_EQUITY_PROVINCE_NO = " + parDataSet.Tables["Employee"].Rows[0]["SKILLS_EQUITY_PROVINCE_NO"].ToString());
                }

                strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);
                strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Employee"].Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString());

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                if (parDataSet.Tables["EmployeeDeduction"].Rows.Count > 0)
                {
                    bytCompress = Save_Employee_Deductions(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeeDeduction"], Convert.ToInt32(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"]));
                    TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytCompress);
                    DataSet.Merge(TempDataSet);
                }

                if (parDataSet.Tables["DeductionEarningPercentage"].Rows.Count > 0)
                {
                    Save_Deductions_Earning_Percentage(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["DeductionEarningPercentage"], Convert.ToInt32(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"]));
                }

                if (parDataSet.Tables["EmployeeEarning"].Rows.Count > 0)
                {
                    bytCompress = Save_Employee_Earnings(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeeEarning"], Convert.ToInt32(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"]));
                    TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytCompress);
                    DataSet.Merge(TempDataSet);
                }

                if (parDataSet.Tables["EmployeeLeave"].Rows.Count > 0)
                {
                    bytCompress = Save_Employee_Leave(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeeLeave"], Convert.ToInt32(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"]));
                    TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytCompress);
                    DataSet.Merge(TempDataSet);
                }

                if (parDataSet.Tables["EmployeeLoan"].Rows.Count > 0)
                {
                    bytCompress = Save_Employee_Loans(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeeLoan"], Convert.ToInt32(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"]));
                    TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytCompress);
                    DataSet.Merge(TempDataSet);
                }

                if (parDataSet.Tables["EmployeePayCategory"].Rows.Count > 0)
                {
                    Save_Employee_PayCategory(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeePayCategory"], Convert.ToInt32(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"]));
                }

                if (parDataSet.Tables["EmployeeFingerTemplate"].Rows.Count > 0)
                {
                    Save_Employee_FingerTemplate(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeeFingerTemplate"], Convert.ToInt32(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"]));
                }
            }
            else
            {
                DataSet.Tables["ReturnCode"].Rows[0]["RETURN_CODE"] = 9999;
            }

            DataSet.Tables.Remove("Check");

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;
            parDataSet.Dispose();
            parDataSet = null;

            return bytCompress;
        }

        public byte[] Update_Record_TimeAttend(Int64 parInt64CompanyNo, Int64 parint64CurrentUserNo, byte[] parbyteDataSet, string parstrPayrollType)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            byte[] bytCompress;
            DataTable DataTable = new DataTable();
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 0 AS RETURN_CODE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Employee"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReturnCode", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT EMPLOYEE_NO");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Employee"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString());

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Check", parInt64CompanyNo);

            if (DataSet.Tables["Check"].Rows.Count == 0)
            {
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" EMPLOYEE_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_CODE"].ToString()));
                strQry.AppendLine(",EMPLOYEE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString()));
                strQry.AppendLine(",EMPLOYEE_SURNAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString()));
               
                strQry.AppendLine(",EMPLOYEE_TEL_HOME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_HOME"].ToString()));
                strQry.AppendLine(",EMPLOYEE_TEL_WORK = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_WORK"].ToString()));
                strQry.AppendLine(",EMPLOYEE_TEL_CELL = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_CELL"].ToString()));

                strQry.AppendLine(",OCCUPATION_NO = " + parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_NO"].ToString());
                strQry.AppendLine(",DEPARTMENT_NO = " + parDataSet.Tables["Employee"].Rows[0]["DEPARTMENT_NO"].ToString());
                strQry.AppendLine(",USE_WORK_TEL_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["USE_WORK_TEL_IND"].ToString()));

                strQry.AppendLine(",EMPLOYEE_3RD_PARTY_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_3RD_PARTY_CODE"].ToString()));
                
                strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);

                if (parDataSet.Tables["Employee"].Rows[0]["CLOSE_IND"].ToString() == "Y")
                {
                    strQry.AppendLine(",EMPLOYEE_ENDDATE = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'");
                }
                
                strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Employee"].Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString());

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                if (parDataSet.Tables["EmployeePayCategory"].Rows.Count > 0)
                {
                    Save_Employee_PayCategory(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeePayCategory"], Convert.ToInt32(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"]));
                }
            }
            else
            {
                DataSet.Tables["ReturnCode"].Rows[0]["RETURN_CODE"] = 9999;
            }
            
            DataSet.Tables.Remove("Check");

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;
            parDataSet.Dispose();
            parDataSet = null;

            return bytCompress;
        }

        public byte[] Update_Record_TimeAttend_New(Int64 parint64CurrentUserNo, Int64 parInt64CompanyNo, int parintEmployeeNo,  byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            byte[] bytCompress;
            DataTable DataTable = new DataTable();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" 0 AS RETURN_CODE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Employee"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReturnCode", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT EMPLOYEE_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_CURRENT ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Employee"].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString());

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Check", parInt64CompanyNo);

            if (DataSet.Tables["Check"].Rows.Count == 0)
            {
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" EMPLOYEE_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_CODE"].ToString()));
                strQry.AppendLine(",EMPLOYEE_NAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NAME"].ToString()));
                strQry.AppendLine(",EMPLOYEE_SURNAME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_SURNAME"].ToString()));

                strQry.AppendLine(",EMPLOYEE_TEL_HOME = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_HOME"].ToString()));
                strQry.AppendLine(",EMPLOYEE_TEL_WORK = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_WORK"].ToString()));
                strQry.AppendLine(",EMPLOYEE_TEL_CELL = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_TEL_CELL"].ToString()));

                strQry.AppendLine(",OCCUPATION_NO = " + parDataSet.Tables["Employee"].Rows[0]["OCCUPATION_NO"].ToString());
                strQry.AppendLine(",DEPARTMENT_NO = " + parDataSet.Tables["Employee"].Rows[0]["DEPARTMENT_NO"].ToString());
                strQry.AppendLine(",USE_WORK_TEL_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["USE_WORK_TEL_IND"].ToString()));

                strQry.AppendLine(",EMPLOYEE_3RD_PARTY_CODE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_3RD_PARTY_CODE"].ToString()));

                //ELR 2017-09-29
                strQry.AppendLine(",USE_EMPLOYEE_NO_IND = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["USE_EMPLOYEE_NO_IND"].ToString()));
                strQry.AppendLine(",EMPLOYEE_PIN = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_PIN"].ToString()));
                strQry.AppendLine(",EMPLOYEE_RFID_CARD_NO = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_RFID_CARD_NO"].ToString()));

                strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo);

                if (parDataSet.Tables["Employee"].Rows[0]["CLOSE_IND"].ToString() == "Y")
                {
                    strQry.AppendLine(",EMPLOYEE_ENDDATE = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'");
                }

                strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables["Employee"].Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["Employee"].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"].ToString());

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                if (parDataSet.Tables["EmployeePayCategory"].Rows.Count > 0)
                {
                    Save_Employee_PayCategory(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeePayCategory"], Convert.ToInt32(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"]));
                }

                if (parDataSet.Tables["EmployeeFingerTemplate"].Rows.Count > 0)
                {
                    Save_Employee_FingerTemplate(parInt64CompanyNo, parint64CurrentUserNo, parDataSet.Tables["EmployeeFingerTemplate"], Convert.ToInt32(parDataSet.Tables["Employee"].Rows[0]["EMPLOYEE_NO"]));
                }
            }
            else
            {
                DataSet.Tables["ReturnCode"].Rows[0]["RETURN_CODE"] = 9999;
            }

            //2017-05-29
            //for (int intRow = 0; intRow < parDataSet.Tables["EmployeeFingerTemplate"].Rows.Count; intRow++)
            //{
            //    if (parDataSet.Tables["EmployeeFingerTemplate"].Rows[intRow].RowState == DataRowState.Added)
            //    {
            //        //2017-04-29 - NB New Template Will Replace Old
            //        strQry.Clear();

            //        strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
            //        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            //        strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
            //        strQry.AppendLine(" AND FINGER_NO = " + parDataSet.Tables["EmployeeFingerTemplate"].Rows[intRow]["FINGER_NO"].ToString());

            //        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            //        strQry.Clear();

            //        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
            //        strQry.AppendLine("(COMPANY_NO");
            //        strQry.AppendLine(",EMPLOYEE_NO");
            //        strQry.AppendLine(",FINGER_NO");
            //        strQry.AppendLine(",FINGER_TEMPLATE");
            //        strQry.AppendLine(",CREATION_DATETIME) ");

            //        strQry.AppendLine(" VALUES ");
            //        strQry.AppendLine("(" + parInt64CompanyNo);
            //        strQry.AppendLine("," + parintEmployeeNo);
            //        strQry.AppendLine("," + parDataSet.Tables["EmployeeFingerTemplate"].Rows[intRow]["FINGER_NO"].ToString());
            //        strQry.AppendLine(",@FINGER_TEMPLATE");
            //        strQry.AppendLine(",GETDATE())");

            //        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),(byte[]) parDataSet.Tables["EmployeeFingerTemplate"].Rows[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE", parInt64CompanyNo);
            //    }
            //    else
            //    {
            //        strQry.Clear();

            //        strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
            //        strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo.ToString());
            //        strQry.AppendLine(" AND EMPLOYEE_NO = " + parDataSet.Tables["EmployeeFingerTemplate"].Rows[intRow]["EMPLOYEE_NO", DataRowVersion.Original].ToString());
            //        strQry.AppendLine(" AND FINGER_NO = " + parDataSet.Tables["EmployeeFingerTemplate"].Rows[intRow]["FINGER_NO", DataRowVersion.Original].ToString());

            //        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            //    }
            //}
            
            DataSet.Tables.Remove("Check");

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;
            parDataSet.Dispose();
            parDataSet = null;

            return bytCompress;
        }

        public void Delete_Record(Int64 parint64CurrentUserNo, Int64 parInt64CompanyNo, int parintEmployeeNo)
        {
            DataSet DataSet = new DataSet();

            string strWhere = " WHERE COMPANY_NO = " + parInt64CompanyNo + " AND EMPLOYEE_NO = " + parintEmployeeNo;
           
            StringBuilder strQry = new StringBuilder();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" T.TABLE_NAME");
            strQry.AppendLine(",C1.COLUMN_NAME");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.TABLES T");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.INFORMATION_SCHEMA.COLUMNS C1");
            strQry.AppendLine(" ON T.TABLE_NAME = C1.TABLE_NAME");

            strQry.AppendLine(" WHERE T.TABLE_TYPE = 'BASE TABLE'");
            strQry.AppendLine(" AND C1.COLUMN_NAME = 'EMPLOYEE_NO'");
            strQry.AppendLine(" AND NOT T.TABLE_NAME LIKE '%_LOG%'");
            strQry.AppendLine(" AND NOT T.TABLE_NAME = 'SDL_REPORT'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "TableColumnName", parInt64CompanyNo);

            for (int intRow = 0; intRow < DataSet.Tables["TableColumnName"].Rows.Count; intRow++)
            {
                strQry.Clear();
                
                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo." + DataSet.Tables["TableColumnName"].Rows[intRow]["TABLE_NAME"].ToString() + strWhere);

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            DataSet.Dispose();
            DataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
