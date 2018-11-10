using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busDataUpload
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busDataUpload()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            byte[] bytCompress = Get_Form_Records_New(parint64CompanyNo,"P");

            return bytCompress;
        }


        public byte[] Get_Form_Records_New(Int64 parint64CompanyNo,string parstrFromProgram)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            if (parstrFromProgram == "X")
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT DISTINCT");

                strQry.AppendLine(" 'Time Attendance' AS PAYROLL_TYPE ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO > 0");
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                strQry.AppendLine(" AND ISNULL(CLOSED_IND,'N') <> 'Y'");
                //strQry.AppendLine(" AND RUN_TYPE = 'P'");
            }
            else
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" 'Wages' AS PAYROLL_TYPE ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO > 0");
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND ISNULL(CLOSED_IND,'N') <> 'Y'");
                //strQry.AppendLine(" AND RUN_TYPE = 'P'");

                strQry.AppendLine(" UNION ");
                strQry.AppendLine(" SELECT DISTINCT");

                strQry.AppendLine(" 'Salaries' AS PAYROLL_TYPE ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO > 0");
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND ISNULL(CLOSED_IND,'N') <> 'Y'");
                //strQry.AppendLine(" AND RUN_TYPE = 'P'");
            }

            strQry.AppendLine(" ORDER BY 1 DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayrollType", parint64CompanyNo);
   
            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",COMPANY_DESC");
            strQry.AppendLine(",WAGE_RUN_IND");
            strQry.AppendLine(",SALARY_RUN_IND");
            strQry.AppendLine(",TIME_ATTENDANCE_RUN_IND");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" COMPANY_DESC");
           
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" PCPC.COMPANY_NO");
            strQry.AppendLine(",PCPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PCPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PCPC.PAY_PERIOD_DATE");
            strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");
            strQry.AppendLine(",PCPC.PAY_PERIOD_DATE AS TIMESHEET_UPLOAD_DATETIME");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PCPC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE IN ('W','S')");
            }
            
            strQry.AppendLine(" WHERE PCPC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            if (parstrFromProgram != "X")
            {
                for (int intRow = 0; intRow < DataSet.Tables["PayCategory"].Rows.Count; intRow++)
                {
                    if (DataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() == "S")
                    {
                        int intDateNow = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                        int intTimsSheetUploadDate = Convert.ToInt32(Convert.ToDateTime(DataSet.Tables["PayCategory"].Rows[intRow]["TIMESHEET_UPLOAD_DATETIME"]).ToString("yyyyMMdd"));

                        if (intDateNow < intTimsSheetUploadDate)
                        {
                            DataSet.Tables["PayCategory"].Rows[intRow]["TIMESHEET_UPLOAD_DATETIME"] = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                            DataSet.Tables["PayCategory"].AcceptChanges();
                        }

                        break;
                    }
                }
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            //Errol 2013-06-15
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            //Salaries
            strQry.AppendLine(" WHEN MAX(ESTBDH.TIMESHEET_DATE) < E.EMPLOYEE_LAST_RUNDATE AND ISNULL(PC.NO_EDIT_IND,'N') <> 'Y'");
            strQry.AppendLine(" THEN DATEADD(DD,1,MAX(ESTBDH.TIMESHEET_DATE)) ");

            strQry.AppendLine(" WHEN EIH.PAY_PERIOD_DATE = E.EMPLOYEE_LAST_RUNDATE ");
            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

            //Errol 2013-06-15
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EIH.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EIH.EMPLOYEE_NO ");
            //Take-On Record
            strQry.AppendLine(" AND EIH.RUN_TYPE = 'T' ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EIH.PAY_CATEGORY_TYPE");
            
            //Will Only Find Data For Salaries
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_HISTORY ESTBDH ");
            strQry.AppendLine(" ON ESTBDH.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ESTBDH.PAY_PERIOD_DATE > '" + DateTime.Now.AddMonths(-2).ToString("yyyy-MM") + "-01'");
            strQry.AppendLine(" AND ESTBDH.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ESTBDH.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
            
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");

            strQry.AppendLine(",PC.NO_EDIT_IND");
            strQry.AppendLine(",EIH.PAY_PERIOD_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.COMPANY_NO");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");
            
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Form_Records_For_User(Int64 parintCurrentUserNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            byte[] bytCompress = Get_Form_Records_For_User_New(parintCurrentUserNo, "P");

            return bytCompress;
        }

        public byte[] Get_Form_Records_For_User_New(Int64 parintCurrentUserNo, string parstrFromProgram)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            Int64 parint64CompanyNo = -1;

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_NO");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK C");

            if (parintCurrentUserNo != 0)
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA");
                strQry.AppendLine(" ON C.COMPANY_NO = UCA.COMPANY_NO");
                strQry.AppendLine(" AND UCA.USER_NO = " + parintCurrentUserNo);

                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL");
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyTemp", -1);

            for (int intRow = 0; intRow < DataSet.Tables["CompanyTemp"].Rows.Count; intRow++)
            {
                parint64CompanyNo = Convert.ToInt64(DataSet.Tables["CompanyTemp"].Rows[intRow]["COMPANY_NO"]);

#if(DEBUG)
                if (parint64CompanyNo == 11)
                {
                    string strStop = "";
                }
#endif

                //20170329
                strQry.Clear();

                strQry.AppendLine(" UPDATE C ");

                strQry.AppendLine(" SET FINGERPRINT_ENGINE = 'Digital Persona'");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

                strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);

                strQry.AppendLine(" AND C.FINGERPRINT_ENGINE IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",COMPANY_DESC");
                strQry.AppendLine(",WAGE_RUN_IND");
                strQry.AppendLine(",SALARY_RUN_IND");
                strQry.AppendLine(",TIME_ATTENDANCE_RUN_IND");
                //20170329
                strQry.AppendLine(",FINGERPRINT_ENGINE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" ORDER BY");
                strQry.AppendLine(" COMPANY_DESC");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT DISTINCT ");
                strQry.AppendLine(" C.COMPANY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",C.COMPANY_DESC");
                strQry.AppendLine(",C.WAGE_RUN_IND");
                strQry.AppendLine(",C.SALARY_RUN_IND");
                strQry.AppendLine(",C.TIME_ATTENDANCE_RUN_IND");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                strQry.AppendLine(" ON C.COMPANY_NO = PC.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

                strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" ORDER BY");
                strQry.AppendLine(" PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",C.COMPANY_DESC");

                //clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyNew", parint64CompanyNo);

                if (parstrFromProgram == "X"
                    | parstrFromProgram == "B")
                {
                    strQry.Clear();
                    strQry.AppendLine(" SELECT DISTINCT");

                    strQry.AppendLine(" 'Time Attendance' AS PAYROLL_TYPE ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO > 0");
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'T'");
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND ISNULL(CLOSED_IND,'N') <> 'Y'");
                }
                
                if (parstrFromProgram == "P"
                    | parstrFromProgram == "B")
                {
                    if (parstrFromProgram == "B")
                    {
                        strQry.AppendLine(" UNION ");
                    }
                    else
                    {
                        strQry.Clear();
                    }
                    strQry.AppendLine(" SELECT DISTINCT");
                    strQry.AppendLine(" 'Wages' AS PAYROLL_TYPE ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO > 0");
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'W'");
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND ISNULL(CLOSED_IND,'N') <> 'Y'");
                    
                    strQry.AppendLine(" UNION ");
                    strQry.AppendLine(" SELECT DISTINCT");

                    strQry.AppendLine(" 'Salaries' AS PAYROLL_TYPE ");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND PAY_CATEGORY_NO > 0");
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = 'S'");
                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                    strQry.AppendLine(" AND ISNULL(CLOSED_IND,'N') <> 'Y'");
                }

                strQry.AppendLine(" ORDER BY 1 DESC");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayrollType", parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
                strQry.AppendLine(",PCPC.PAY_PERIOD_DATE");
                strQry.AppendLine(",PC.LAST_UPLOAD_DATETIME");
                strQry.AppendLine(",PCPC.PAY_PERIOD_DATE AS TIMESHEET_UPLOAD_DATETIME");
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC");
                strQry.AppendLine(" ON PCPC.COMPANY_NO = PC.COMPANY_NO");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P'");
              
                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE = 'T'");
                }
                else
                {
                    if (parstrFromProgram == "P")
                    {
                        strQry.AppendLine(" AND PCPC.PAY_CATEGORY_TYPE IN ('W','S')");
                    }
                }

                strQry.AppendLine(" ORDER BY");
                strQry.AppendLine(" PC.PAY_CATEGORY_DESC");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

                if (parstrFromProgram != "X")
                {
                    for (int intRowNew = 0; intRowNew < DataSet.Tables["PayCategory"].Rows.Count; intRowNew++)
                    {
                        if (DataSet.Tables["PayCategory"].Rows[intRowNew]["PAY_CATEGORY_TYPE"].ToString() == "S")
                        {
                            if (DataSet.Tables["PayCategory"].Rows[intRowNew]["TIMESHEET_UPLOAD_DATETIME"] != System.DBNull.Value)
                            {
                                int intDateNow = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                                int intTimsSheetUploadDate = Convert.ToInt32(Convert.ToDateTime(DataSet.Tables["PayCategory"].Rows[intRowNew]["TIMESHEET_UPLOAD_DATETIME"]).ToString("yyyyMMdd"));

                                if (intDateNow < intTimsSheetUploadDate)
                                {
                                    DataSet.Tables["PayCategory"].Rows[intRowNew]["TIMESHEET_UPLOAD_DATETIME"] = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                                    DataSet.Tables["PayCategory"].AcceptChanges();
                                }
                            }
                        }
                    }
                }

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                //Errol 2013-06-15
                strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

                strQry.AppendLine(" CASE ");

                //Salaries
                strQry.AppendLine(" WHEN MAX(ESTBDH.TIMESHEET_DATE) < E.EMPLOYEE_LAST_RUNDATE AND ISNULL(PC.NO_EDIT_IND,'N') <> 'Y'");
                strQry.AppendLine(" THEN DATEADD(DD,1,MAX(ESTBDH.TIMESHEET_DATE)) ");

                strQry.AppendLine(" WHEN EIH.PAY_PERIOD_DATE = E.EMPLOYEE_LAST_RUNDATE ");
                strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

                strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

                strQry.AppendLine(" END ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

                //Errol 2013-06-15
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH ");
                strQry.AppendLine(" ON EPC.COMPANY_NO = EIH.COMPANY_NO ");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EIH.EMPLOYEE_NO ");
                //Take-On Record
                strQry.AppendLine(" AND EIH.RUN_TYPE = 'T' ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EIH.PAY_CATEGORY_TYPE");

                //Will Only Find Data For Salaries
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_HISTORY ESTBDH ");
                strQry.AppendLine(" ON ESTBDH.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND ESTBDH.PAY_PERIOD_DATE > '" + DateTime.Now.AddMonths(-2).ToString("yyyy-MM") + "-01'");
                strQry.AppendLine(" AND ESTBDH.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND ESTBDH.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");
                
                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
                }
                else
                {
                    if (parstrFromProgram == "P")
                    {
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");
                    }
                }

                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");

                strQry.AppendLine(",PC.NO_EDIT_IND");
                strQry.AppendLine(",EIH.PAY_PERIOD_DATE");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" EPC.COMPANY_NO");
                strQry.AppendLine(",EPC.EMPLOYEE_NO");
                strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",EPC.PAY_CATEGORY_NO");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");
                }
                else
                {
                    if (parstrFromProgram == "P")
                    {
                        strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE IN ('W','S')");
                    }
                }

                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parint64CompanyNo);
            }

            DataView myDataView = new DataView(DataSet.Tables["PayrollType"], "", "PAYROLL_TYPE", DataViewRowState.CurrentRows);

            for (int intRow = 1; intRow < myDataView.Count; intRow++)
            {
                if (myDataView[intRow]["PAYROLL_TYPE"].ToString() == myDataView[intRow - 1]["PAYROLL_TYPE"].ToString())
                {
                    myDataView[intRow].Delete();

                    intRow -= 1;
                }
            }

            DataSet.Tables.Remove("CompanyTemp");
            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public string Insert_TimeSheet_Records(Int64 parint64CompanyNo, DateTime parTimeSheetDateTime, byte[] parbyteDataSet,string strPayrollType,string parPayPeriodDate,string parTimeSheetUploadDate)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            int intTimeSheetSeqNo = -1;
            int intBreakSeqNo = -1;
            int intEmployeeNo = -1;

            StringBuilder strQry = new StringBuilder();
            
            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            DataView TimeSheetDataView;
            DataView BreakDataView;

            for (int intPayCategoryRow = 0; intPayCategoryRow < DataSet.Tables["PayCategoryUpload"].Rows.Count; intPayCategoryRow++)
            {
                intEmployeeNo = -1;

                TimeSheetDataView = null;
                TimeSheetDataView = new DataView(DataSet.Tables["TimeSheets"],
                    "PAY_CATEGORY_NO = " + DataSet.Tables["PayCategoryUpload"].Rows[intPayCategoryRow]["PAY_CATEGORY_NO"].ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < TimeSheetDataView.Count; intRow++)
                {
                    if (intEmployeeNo != Convert.ToInt32(TimeSheetDataView[intRow]["EMPLOYEE_NO"]))
                    {
                        intEmployeeNo = Convert.ToInt32(TimeSheetDataView[intRow]["EMPLOYEE_NO"]);

                        if (DataSet.Tables["Temp"] != null)
                        {
                            DataSet.Tables.Remove("Temp");
                        }

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ISNULL(MAX(TIMESHEET_SEQ),0) AS MAX_TIMESHEET_SEQ");

                        if (strPayrollType == "W")
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                        }
                        else
                        {
                            if (strPayrollType == "S")
                            {
                                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                            }
                            else
                            {
                                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                            }
                        }

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + TimeSheetDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + TimeSheetDataView[intRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND TIMESHEET_DATE = '" + parTimeSheetDateTime.ToString("yyyy-MM-dd") + "'");

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parint64CompanyNo);

                        intTimeSheetSeqNo = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_TIMESHEET_SEQ"]) + 1;
                    }

                    strQry.Clear();

                    if (strPayrollType == "W")
                    {
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                    }
                    else
                    {
                        if (strPayrollType == "S")
                        {
                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                        }
                        else
                        {
                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                        }
                    }

                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO ");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",TIMESHEET_DATE");
                    strQry.AppendLine(",TIMESHEET_SEQ");
                    strQry.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
                    strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");

                    strQry.AppendLine(",CLOCKED_TIME_IN_MINUTES");
                    strQry.AppendLine(",CLOCKED_TIME_OUT_MINUTES)");

                    strQry.AppendLine(" SELECT DISTINCT ");
                    strQry.AppendLine(parint64CompanyNo.ToString());
                    strQry.AppendLine("," + TimeSheetDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine("," + TimeSheetDataView[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(",'" + parTimeSheetDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + intTimeSheetSeqNo);
                    strQry.AppendLine("," + TimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"].ToString());
                    strQry.AppendLine("," + TimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"].ToString());

                    if (TimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + TimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"].ToString());
                    }

                    if (TimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {

                        strQry.AppendLine("," + TimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"].ToString());
                    }

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

                    strQry.AppendLine(" WHERE  C.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND C.COMPANY_NO NOT IN");
                    strQry.AppendLine("(SELECT COMPANY_NO");

                    if (strPayrollType == "W")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT");
                    }
                    else
                    {
                        if (strPayrollType == "S")
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT");
                        }
                        else
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT");
                        }
                    }

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + TimeSheetDataView[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(" AND TIMESHEET_DATE = '" + parTimeSheetDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + TimeSheetDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND TIMESHEET_TIME_IN_MINUTES = " + TimeSheetDataView[intRow]["TIMESHEET_TIME_IN_MINUTES"].ToString());
                    strQry.AppendLine(" AND TIMESHEET_TIME_OUT_MINUTES = " + TimeSheetDataView[intRow]["TIMESHEET_TIME_OUT_MINUTES"].ToString());

                    if (TimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(" AND CLOCKED_TIME_IN_MINUTES IS NULL");
                    }
                    else
                    {
                        strQry.AppendLine(" AND CLOCKED_TIME_IN_MINUTES = " + TimeSheetDataView[intRow]["CLOCKED_TIME_IN_MINUTES"].ToString());
                    }

                    if (TimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(" AND CLOCKED_TIME_OUT_MINUTES IS NULL)");
                    }
                    else
                    {
                        strQry.AppendLine(" AND CLOCKED_TIME_OUT_MINUTES = " + TimeSheetDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"].ToString() + ")");
                    }

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    intTimeSheetSeqNo += 1;
                }

                intEmployeeNo = -1;
          
                BreakDataView = null;
                BreakDataView = new DataView(DataSet.Tables["Breaks"],
                    "PAY_CATEGORY_NO = " + DataSet.Tables["PayCategoryUpload"].Rows[intPayCategoryRow]["PAY_CATEGORY_NO"].ToString(),
                    "",
                    DataViewRowState.CurrentRows);

                for (int intRow = 0; intRow < BreakDataView.Count; intRow++)
                {
                    if (intEmployeeNo != Convert.ToInt32(BreakDataView[intRow]["EMPLOYEE_NO"]))
                    {
                        intEmployeeNo = Convert.ToInt32(BreakDataView[intRow]["EMPLOYEE_NO"]);

                        if (DataSet.Tables["Temp"] != null)
                        {
                            DataSet.Tables.Remove("Temp");
                        }

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ISNULL(MAX(BREAK_SEQ),0) AS MAX_BREAK_SEQ");

                        if (strPayrollType == "W")
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT");
                        }
                        else
                        {
                            if (strPayrollType == "S")
                            {
                                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                            }
                            else
                            {
                                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                            }
                        }

                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + BreakDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + BreakDataView[intRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND BREAK_DATE = '" + parTimeSheetDateTime.ToString("yyyy-MM-dd") + "'");

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parint64CompanyNo);

                        intBreakSeqNo = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_BREAK_SEQ"]) + 1;
                    }

                    strQry.Clear();

                    if (strPayrollType == "W")
                    {
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT");
                    }
                    else
                    {
                        if (strPayrollType == "S")
                        {
                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                        }
                        else
                        {
                            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                        }
                    }

                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO ");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",BREAK_DATE");
                    strQry.AppendLine(",BREAK_SEQ");
                    strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
                    strQry.AppendLine(",BREAK_TIME_OUT_MINUTES");

                    strQry.AppendLine(",CLOCKED_TIME_IN_MINUTES");
                    strQry.AppendLine(",CLOCKED_TIME_OUT_MINUTES)");

                    strQry.AppendLine(" SELECT DISTINCT ");
                    strQry.AppendLine(parint64CompanyNo.ToString());
                    strQry.AppendLine("," + BreakDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine("," + BreakDataView[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(",'" + parTimeSheetDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + intBreakSeqNo);
                    strQry.AppendLine("," + BreakDataView[intRow]["BREAK_TIME_IN_MINUTES"].ToString());
                    strQry.AppendLine("," + BreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"].ToString());
               
                    if (BreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + BreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"].ToString());
                    }

                    if (BreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + BreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"].ToString());
                    }
                   
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

                    strQry.AppendLine(" WHERE  C.COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND C.COMPANY_NO NOT IN");
                    strQry.AppendLine("(SELECT COMPANY_NO");

                    if (strPayrollType == "W")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT");
                    }
                    else
                    {
                        if (strPayrollType == "S")
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT");
                        }
                        else
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT");
                        }
                    }

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + BreakDataView[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(" AND BREAK_DATE = '" + parTimeSheetDateTime.ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + BreakDataView[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND BREAK_TIME_IN_MINUTES = " + BreakDataView[intRow]["BREAK_TIME_IN_MINUTES"].ToString());
                    strQry.AppendLine(" AND BREAK_TIME_OUT_MINUTES = " + BreakDataView[intRow]["BREAK_TIME_OUT_MINUTES"].ToString());

                    if (BreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(" AND CLOCKED_TIME_IN_MINUTES IS NULL");
                    }
                    else
                    {
                        strQry.AppendLine(" AND CLOCKED_TIME_IN_MINUTES = " + BreakDataView[intRow]["CLOCKED_TIME_IN_MINUTES"].ToString());
                    }

                    if (BreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"] == System.DBNull.Value)
                    {
                        strQry.AppendLine(" AND CLOCKED_TIME_OUT_MINUTES IS NULL)");
                    }
                    else
                    {
                        strQry.AppendLine(" AND CLOCKED_TIME_OUT_MINUTES = " + BreakDataView[intRow]["CLOCKED_TIME_OUT_MINUTES"].ToString() + ")");
                    }
                   
                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                    intBreakSeqNo += 1;
                }

                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");
                strQry.AppendLine(" SET LAST_UPLOAD_DATETIME = GETDATE()");
               
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["PayCategoryUpload"].Rows[intPayCategoryRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategoryUpload"].Rows[intPayCategoryRow]["PAY_CATEGORY_TYPE"].ToString()));
           
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT");
                strQry.AppendLine(" SET SALARY_TIMESHEET_ENDDATE = '" + parTimeSheetUploadDate + "'");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND PAY_PERIOD_DATE = '" + parPayPeriodDate + "'");
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["PayCategoryUpload"].Rows[intPayCategoryRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategoryUpload"].Rows[intPayCategoryRow]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine(" AND RUN_TYPE = 'P'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            //Delete Any Timesheet Where Employee is Already Closed
            strQry.Clear();

            if (strPayrollType == "W")
            {
                strQry.AppendLine(" DELETE ETC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
            }
            else
            {
                if (strPayrollType == "S")
                {
                    strQry.AppendLine(" DELETE ETC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" DELETE ETC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND NOT E.EMPLOYEE_ENDDATE IS NULL ");
            strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            if (strPayrollType == "W")
            {
                strQry.AppendLine(" DELETE ETC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ETC");
            }
            else
            {
                if (strPayrollType == "S")
                {
                    strQry.AppendLine(" DELETE ETC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" DELETE ETC FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ETC");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND NOT E.EMPLOYEE_ENDDATE IS NULL ");
            strQry.AppendLine(" AND E.COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
