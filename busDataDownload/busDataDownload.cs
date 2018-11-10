using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busDataDownload
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busDataDownload()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, Int64 parint64UserNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            byte[] bytCompress = Get_Form_Records_New(parint64CompanyNo, "P");

            return bytCompress;
        }

        public byte[] Get_Form_Records_New(Int64 parint64CompanyNo, string parstrFromProgram)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            //20170329
            strQry.Clear();

            strQry.AppendLine(" UPDATE C ");

            strQry.AppendLine(" SET FINGERPRINT_ENGINE = 'Digital Persona'");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" AND C.FINGERPRINT_ENGINE IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",COMPANY_DESC");
            strQry.AppendLine(",POST_ADDR_LINE1");
            strQry.AppendLine(",POST_ADDR_LINE2");
            strQry.AppendLine(",POST_ADDR_LINE3");
            strQry.AppendLine(",POST_ADDR_LINE4");

            strQry.AppendLine(",POST_ADDR_CODE");

            strQry.AppendLine(",RES_UNIT_NUMBER");
            strQry.AppendLine(",RES_COMPLEX");
            strQry.AppendLine(",RES_STREET_NUMBER");
            strQry.AppendLine(",RES_STREET_NAME");
            strQry.AppendLine(",RES_SUBURB");
            strQry.AppendLine(",RES_CITY");
            strQry.AppendLine(",RES_ADDR_CODE");
            //20170329
            strQry.AppendLine(",FINGERPRINT_ENGINE");

            strQry.AppendLine(",'A' AS ACCESS_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",ISNULL(PC.DAILY_ROUNDING_IND,0) AS DAILY_ROUNDING_IND");
            strQry.AppendLine(",ISNULL(PC.DAILY_ROUNDING_MINUTES,0)AS DAILY_ROUNDING_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_SUN_ABOVE_MINUTES,0) AS EXCEPTION_SUN_ABOVE_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_SUN_BELOW_MINUTES,0) AS EXCEPTION_SUN_BELOW_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_MON_ABOVE_MINUTES,0) AS EXCEPTION_MON_ABOVE_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_MON_BELOW_MINUTES,0) AS EXCEPTION_MON_BELOW_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_TUE_ABOVE_MINUTES,0) AS EXCEPTION_TUE_ABOVE_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_TUE_BELOW_MINUTES,0) AS EXCEPTION_TUE_BELOW_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_WED_ABOVE_MINUTES,0) AS EXCEPTION_WED_ABOVE_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_WED_BELOW_MINUTES,0) AS EXCEPTION_WED_BELOW_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_THU_ABOVE_MINUTES,0) AS EXCEPTION_THU_ABOVE_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_THU_BELOW_MINUTES,0) AS EXCEPTION_THU_BELOW_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_FRI_ABOVE_MINUTES,0) AS EXCEPTION_FRI_ABOVE_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_FRI_BELOW_MINUTES,0) AS EXCEPTION_FRI_BELOW_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_SAT_ABOVE_MINUTES,0) AS EXCEPTION_SAT_ABOVE_MINUTES");
            strQry.AppendLine(",ISNULL(PC.EXCEPTION_SAT_BELOW_MINUTES,0) AS EXCEPTION_SAT_BELOW_MINUTES");

            strQry.AppendLine(",NO_EDIT_IND = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN CL.DYNAMIC_UPLOAD_KEY IS NULL");

            strQry.AppendLine(" THEN 'N' ");

            strQry.AppendLine(" ELSE PC.NO_EDIT_IND");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
            strQry.AppendLine(" ON E.COMPANY_NO = CL.COMPANY_NO");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

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

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" D.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",D.DEPARTMENT_NO");
            strQry.AppendLine(",D.DEPARTMENT_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEPARTMENT D");
            strQry.AppendLine(" ON E.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine(" AND E.DEPARTMENT_NO = D.DEPARTMENT_NO");
            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEPARTMENT_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Department", parint64CompanyNo);

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

            //B=Both
            byte[] bytCompress = Get_Form_Records_For_User_New(parintCurrentUserNo, "B");

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
            strQry.AppendLine(" SELECT DISTINCT");
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

                //20170329
                strQry.Clear();

                strQry.AppendLine(" UPDATE C ");

                strQry.AppendLine(" SET FINGERPRINT_ENGINE = 'Digital Persona'");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                
                strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);

                strQry.AppendLine(" AND C.FINGERPRINT_ENGINE IS NULL");
                
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

                strQry.Clear();

                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" C.COMPANY_NO");
                strQry.AppendLine(",C.COMPANY_DESC");
                strQry.AppendLine(",C.POST_ADDR_LINE1");
                strQry.AppendLine(",C.POST_ADDR_LINE2");
                strQry.AppendLine(",C.POST_ADDR_LINE3");
                strQry.AppendLine(",C.POST_ADDR_CODE");

                strQry.AppendLine(",C.RES_UNIT_NUMBER");
                strQry.AppendLine(",C.RES_COMPLEX");
                strQry.AppendLine(",C.RES_STREET_NUMBER");
                strQry.AppendLine(",C.RES_STREET_NAME");
                strQry.AppendLine(",C.RES_SUBURB");
                strQry.AppendLine(",C.RES_CITY");
                strQry.AppendLine(",C.RES_ADDR_CODE");
                //20170329
                strQry.AppendLine(",C.FINGERPRINT_ENGINE");
                
                if (parintCurrentUserNo == 0)
                {
                    strQry.AppendLine(",'A' AS COMPANY_ACCESS_IND");
                }
                else
                {
                    strQry.AppendLine(",UCA.COMPANY_ACCESS_IND");
                }

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

                if (parintCurrentUserNo != 0)
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA");
                    strQry.AppendLine(" ON C.COMPANY_NO = UCA.COMPANY_NO");
                    strQry.AppendLine(" AND UCA.USER_NO = " + parintCurrentUserNo);

                    strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL");
                }

                strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" ORDER BY");
                strQry.AppendLine(" COMPANY_DESC");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parint64CompanyNo);

                strQry.Clear();
                strQry.AppendLine(" SELECT DISTINCT ");
                strQry.AppendLine(" PC.COMPANY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
                strQry.AppendLine(",ISNULL(PC.DAILY_ROUNDING_IND,0) AS DAILY_ROUNDING_IND");
                strQry.AppendLine(",ISNULL(PC.DAILY_ROUNDING_MINUTES,0)AS DAILY_ROUNDING_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_SUN_ABOVE_MINUTES,0) AS EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_SUN_BELOW_MINUTES,0) AS EXCEPTION_SUN_BELOW_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_MON_ABOVE_MINUTES,0) AS EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_MON_BELOW_MINUTES,0) AS EXCEPTION_MON_BELOW_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_TUE_ABOVE_MINUTES,0) AS EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_TUE_BELOW_MINUTES,0) AS EXCEPTION_TUE_BELOW_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_WED_ABOVE_MINUTES,0) AS EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_WED_BELOW_MINUTES,0) AS EXCEPTION_WED_BELOW_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_THU_ABOVE_MINUTES,0) AS EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_THU_BELOW_MINUTES,0) AS EXCEPTION_THU_BELOW_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_FRI_ABOVE_MINUTES,0) AS EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_FRI_BELOW_MINUTES,0) AS EXCEPTION_FRI_BELOW_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_SAT_ABOVE_MINUTES,0) AS EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",ISNULL(PC.EXCEPTION_SAT_BELOW_MINUTES,0) AS EXCEPTION_SAT_BELOW_MINUTES");

                strQry.AppendLine(",NO_EDIT_IND = ");

                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN CL.DYNAMIC_UPLOAD_KEY IS NULL");

                strQry.AppendLine(" THEN 'N' ");

                strQry.AppendLine(" ELSE PC.NO_EDIT_IND");

                strQry.AppendLine(" END ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
                strQry.AppendLine(" ON E.COMPANY_NO = CL.COMPANY_NO");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                strQry.AppendLine(" ON E.COMPANY_NO = PC.COMPANY_NO");
                strQry.AppendLine(" AND PC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

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
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" PC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

                //2017-02-13
                strQry.Clear();
                strQry.AppendLine(" SELECT DISTINCT");
                strQry.AppendLine(" D.COMPANY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",D.DEPARTMENT_NO");
                strQry.AppendLine(",D.DEPARTMENT_DESC");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEPARTMENT D");
                strQry.AppendLine(" ON E.COMPANY_NO = D.COMPANY_NO");
                strQry.AppendLine(" AND E.DEPARTMENT_NO = D.DEPARTMENT_NO");
                strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
                strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
                strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

                strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" PC.PAY_CATEGORY_NO");
                strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",DEPARTMENT_NO");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Department", parint64CompanyNo);
            }

            DataSet.Tables.Remove("CompanyTemp");
            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Download_Records(Int64 parint64CompanyNo, string parstrArrayCostCentreWages, string parstrArrayCostCentreSalaries, Int64 parint64CurrentUserNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            string strArrayCostCentreTimeAttendance = "";

            byte[] bytCompress = Download_Records_New(parint64CompanyNo, parstrArrayCostCentreWages, parstrArrayCostCentreSalaries, strArrayCostCentreTimeAttendance);

            return bytCompress;
        }
        
        public byte[] Download_Records_Fix_Parameters(Int64 parint64CompanyNo, string parstrArrayCostCentreWages, string parstrArrayCostCentreSalaries, string parstrArrayCostCentreTimeAttendance, byte[] parbyteDataSet)
        {
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            //2017-07-10
            if (DataSet.Tables["EmployeeUploadParameters"] != null)
            {
                for (int intRow = 0; intRow < DataSet.Tables["EmployeeUploadParameters"].Rows.Count; intRow++)
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");

                    strQry.AppendLine(" SET ");

                    strQry.AppendLine(" USE_EMPLOYEE_NO_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["EmployeeUploadParameters"].Rows[intRow]["USE_EMPLOYEE_NO_IND"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_PIN = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["EmployeeUploadParameters"].Rows[intRow]["EMPLOYEE_PIN"].ToString()));
                    strQry.AppendLine(",EMPLOYEE_RFID_CARD_NO = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["EmployeeUploadParameters"].Rows[intRow]["EMPLOYEE_RFID_CARD_NO"].ToString()));

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["EmployeeUploadParameters"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["EmployeeUploadParameters"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }
            }
           
            byte[] bytCompress = Download_Records_New(parint64CompanyNo, parstrArrayCostCentreWages, parstrArrayCostCentreSalaries, parstrArrayCostCentreTimeAttendance);

            return bytCompress;
        }
        

        public byte[] Download_Records_New(Int64 parint64CompanyNo, string parstrArrayCostCentreWages, string parstrArrayCostCentreSalaries, string parstrArrayCostCentreTimeAttendance)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            string[] parstrCostCentreWages = parstrArrayCostCentreWages.Split('|');
            string[] parstrCostCentreSalaries = parstrArrayCostCentreSalaries.Split('|');
            string[] parstrCostCentreTimeAttendance = parstrArrayCostCentreTimeAttendance.Split('|');

            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" P.COMPANY_NO");
            strQry.AppendLine(",P.PAY_CATEGORY_NO");
            strQry.AppendLine(",P.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",P.PAY_CATEGORY_BREAK_NO");
            strQry.AppendLine(",P.WORKED_TIME_MINUTES");
            strQry.AppendLine(",P.BREAK_MINUTES");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK P");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON P.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND P.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND P.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

            strQry.AppendLine(" WHERE P.COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" AND ((P.PAY_CATEGORY_TYPE = 'W'");
            
            string strQryTemp = "";

            if (parstrArrayCostCentreWages == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreWages.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreWages[intCount].ToString();

                    if (intCount != parstrCostCentreWages.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND P.PAY_CATEGORY_NO IN (" + strQryTemp + "))");
            
            strQry.AppendLine(" OR (P.PAY_CATEGORY_TYPE = 'S'");
                     
            strQryTemp = "";

            if (parstrArrayCostCentreSalaries == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreSalaries.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreSalaries[intCount].ToString();

                    if (intCount != parstrCostCentreSalaries.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND P.PAY_CATEGORY_NO IN (" + strQryTemp + "))");
            
            strQry.AppendLine(" OR (P.PAY_CATEGORY_TYPE = 'T'");
            
            strQryTemp = "";

            if (parstrArrayCostCentreTimeAttendance == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreTimeAttendance.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreTimeAttendance[intCount].ToString();

                    if (intCount != parstrCostCentreTimeAttendance.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND P.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(") AND P.DATETIME_DELETE_RECORD IS NULL");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryBreak", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" UCA.COMPANY_NO");
            strQry.AppendLine(",UI.USER_NO");
            strQry.AppendLine(",UI.USER_ID");
            strQry.AppendLine(",UI.FIRSTNAME");
            strQry.AppendLine(",UI.SURNAME");
            strQry.AppendLine(",UI.SYSTEM_ADMINISTRATOR_IND");
            strQry.AppendLine(",UI.EMAIL");
            strQry.AppendLine(",UI.PASSWORD");
            strQry.AppendLine(",UI.RESET");
            strQry.AppendLine(",UI.DATETIME_DELETE_RECORD");
            //2017-05-10
            strQry.AppendLine(",UI.USER_CLOCK_PIN");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_ID UI");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_COMPANY_ACCESS UCA");
            strQry.AppendLine(" ON UI.USER_NO = UCA.USER_NO");
            strQry.AppendLine(" AND UCA.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL");
            //A = Administrator U = User 
            //strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A'");

            strQry.AppendLine(" WHERE UI.SYSTEM_ADMINISTRATOR_IND <> 'Y'");
            strQry.AppendLine(" AND UI.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserId", parint64CompanyNo);

            //NB DISTINCT was added to Fix a Problem on DB (Needs to be Investigated)
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" USER_NO");
            strQry.AppendLine(",COMPANY_NO");
            //A = Administrator U = User 
            strQry.AppendLine(",COMPANY_ACCESS_IND");
            //B = Internet and Client,I=Internet,C=Client
            strQry.AppendLine(",ISNULL(ACCESS_LAYER_IND,'C') AS ACCESS_LAYER_IND");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_COMPANY_ACCESS ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserCompany", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" UPC.USER_NO");
            strQry.AppendLine(",UPC.COMPANY_NO");
            strQry.AppendLine(",UPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",UPC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_PAY_CATEGORY UPC ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON UPC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND UPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND UPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

            strQry.AppendLine(" WHERE UPC.COMPANY_NO = " + parint64CompanyNo);
            
            strQry.AppendLine(" AND ((UPC.PAY_CATEGORY_TYPE = 'W'");

            strQryTemp = "";

            if (parstrArrayCostCentreWages == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreWages.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreWages[intCount].ToString();

                    if (intCount != parstrCostCentreWages.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND UPC.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(" OR (UPC.PAY_CATEGORY_TYPE = 'S'");

            strQryTemp = "";

            if (parstrArrayCostCentreSalaries == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreSalaries.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreSalaries[intCount].ToString();

                    if (intCount != parstrCostCentreSalaries.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND UPC.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(" OR (UPC.PAY_CATEGORY_TYPE = 'T'");

            strQryTemp = "";

            if (parstrArrayCostCentreTimeAttendance == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreTimeAttendance.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreTimeAttendance[intCount].ToString();

                    if (intCount != parstrCostCentreTimeAttendance.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND UPC.PAY_CATEGORY_NO IN (" + strQryTemp + "))");
            
            strQry.AppendLine(") AND UPC.DATETIME_DELETE_RECORD IS NULL");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserPayCategory", parint64CompanyNo);
            
            //2016-12-08 New Table Department within Cost Centre)
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" UPCD.USER_NO");
            strQry.AppendLine(",UPCD.COMPANY_NO");
            strQry.AppendLine(",UPCD.PAY_CATEGORY_NO");
            strQry.AppendLine(",UPCD.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",UPCD.DEPARTMENT_NO");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_PAY_CATEGORY_DEPARTMENT UPCD ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON UPCD.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND UPCD.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND UPCD.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEPARTMENT D");
            strQry.AppendLine(" ON UPCD.COMPANY_NO = D.COMPANY_NO");
            strQry.AppendLine(" AND UPCD.DEPARTMENT_NO = D.DEPARTMENT_NO");
            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE UPCD.COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" AND ((UPCD.PAY_CATEGORY_TYPE = 'W'");

            strQryTemp = "";

            if (parstrArrayCostCentreWages == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreWages.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreWages[intCount].ToString();

                    if (intCount != parstrCostCentreWages.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND UPCD.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(" OR (UPCD.PAY_CATEGORY_TYPE = 'S'");

            strQryTemp = "";

            if (parstrArrayCostCentreSalaries == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreSalaries.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreSalaries[intCount].ToString();

                    if (intCount != parstrCostCentreSalaries.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND UPCD.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(" OR (UPCD.PAY_CATEGORY_TYPE = 'T'");

            strQryTemp = "";

            if (parstrArrayCostCentreTimeAttendance == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreTimeAttendance.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreTimeAttendance[intCount].ToString();

                    if (intCount != parstrCostCentreTimeAttendance.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND UPCD.PAY_CATEGORY_NO IN (" + strQryTemp + "))");
            
            strQry.AppendLine(") AND UPCD.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserPayCategoryDepartment", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" UE.USER_NO");
            strQry.AppendLine(",UE.COMPANY_NO");
            strQry.AppendLine(",UE.EMPLOYEE_NO");
            strQry.AppendLine(",UE.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_EMPLOYEE UE ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
            strQry.AppendLine(" ON E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND UE.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND UE.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND UE.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            
            strQry.AppendLine(" AND ((EPC.PAY_CATEGORY_TYPE = 'W'");

            strQryTemp = "";

            if (parstrArrayCostCentreWages == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreWages.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreWages[intCount].ToString();

                    if (intCount != parstrCostCentreWages.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(" OR (EPC.PAY_CATEGORY_TYPE = 'S'");

            strQryTemp = "";

            if (parstrArrayCostCentreSalaries == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreSalaries.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreSalaries[intCount].ToString();

                    if (intCount != parstrCostCentreSalaries.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(" OR (EPC.PAY_CATEGORY_TYPE = 'T'");

            strQryTemp = "";

            if (parstrArrayCostCentreTimeAttendance == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreTimeAttendance.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreTimeAttendance[intCount].ToString();

                    if (intCount != parstrCostCentreTimeAttendance.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(") AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

            strQry.AppendLine(" WHERE UE.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND UE.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" UE.USER_NO");
            strQry.AppendLine(",UE.COMPANY_NO");
            strQry.AppendLine(",UE.EMPLOYEE_NO");
            strQry.AppendLine(",UE.PAY_CATEGORY_TYPE");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserEmployee", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");

            //Errol 2013-06-15
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN EIH.PAY_PERIOD_DATE = E.EMPLOYEE_LAST_RUNDATE ");
            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",E.EMPLOYEE_ENDDATE");
            strQry.AppendLine(",E.DEPARTMENT_NO");

            //2017-09-29
            strQry.AppendLine(",E.USE_EMPLOYEE_NO_IND");
            strQry.AppendLine(",E.EMPLOYEE_PIN");
            strQry.AppendLine(",E.EMPLOYEE_RFID_CARD_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");

            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (");

            strQryTemp = "";

            if (parstrArrayCostCentreWages == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreWages.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreWages[intCount].ToString();

                    if (intCount != parstrCostCentreWages.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(strQryTemp);

            strQry.AppendLine(") AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

            //Errol 2013-06-15
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EIH.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EIH.EMPLOYEE_NO ");
            //Take-On Record
            strQry.AppendLine(" AND EIH.RUN_TYPE = 'T' ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EIH.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            //Removed 2012-01-24 To Set NOT_ACTIVE_IND on Client Database
            //strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'W'");

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");

            //Errol 2013-06-15
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN MAX(ESTBDH.TIMESHEET_DATE) < E.EMPLOYEE_LAST_RUNDATE AND ISNULL(PC.NO_EDIT_IND,'N') <> 'Y'");
            strQry.AppendLine(" THEN DATEADD(DD,1,MAX(ESTBDH.TIMESHEET_DATE)) ");

            strQry.AppendLine(" WHEN EIH.PAY_PERIOD_DATE = E.EMPLOYEE_LAST_RUNDATE ");
            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",E.EMPLOYEE_ENDDATE");
            strQry.AppendLine(",E.DEPARTMENT_NO");

            //2017-09-29
            strQry.AppendLine(",E.USE_EMPLOYEE_NO_IND");
            strQry.AppendLine(",E.EMPLOYEE_PIN");
            strQry.AppendLine(",E.EMPLOYEE_RFID_CARD_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");

            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (");

            strQryTemp = "";

            if (parstrArrayCostCentreSalaries == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreSalaries.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreSalaries[intCount].ToString();

                    if (intCount != parstrCostCentreSalaries.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(strQryTemp);

            strQry.AppendLine(") AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
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

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_BREAK_DAY_HISTORY ESTBDH ");
            strQry.AppendLine(" ON ESTBDH.COMPANY_NO = E.COMPANY_NO ");
            strQry.AppendLine(" AND ESTBDH.PAY_PERIOD_DATE > '" + DateTime.Now.AddMonths(-2).ToString("yyyy-MM") + "-01'");
            strQry.AppendLine(" AND ESTBDH.EMPLOYEE_NO = E.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ESTBDH.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            //Removed 2012-01-24 To Set NOT_ACTIVE_IND on Client Database
            //strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'S'");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE ");
            strQry.AppendLine(",EIH.PAY_PERIOD_DATE");
            strQry.AppendLine(",E.EMPLOYEE_ENDDATE");
            strQry.AppendLine(",E.DEPARTMENT_NO");
            strQry.AppendLine(",PC.NO_EDIT_IND");
            //2017-09-29
            strQry.AppendLine(",E.USE_EMPLOYEE_NO_IND");
            strQry.AppendLine(",E.EMPLOYEE_PIN");
            strQry.AppendLine(",E.EMPLOYEE_RFID_CARD_NO");

            strQry.AppendLine(" UNION");

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");

            //Errol 2013-06-15
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN EIH.PAY_PERIOD_DATE = E.EMPLOYEE_LAST_RUNDATE ");
            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",E.EMPLOYEE_ENDDATE");
            strQry.AppendLine(",E.DEPARTMENT_NO");

            //2017-09-29
            strQry.AppendLine(",E.USE_EMPLOYEE_NO_IND");
            strQry.AppendLine(",E.EMPLOYEE_PIN");
            strQry.AppendLine(",E.EMPLOYEE_RFID_CARD_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");

            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (");

            strQryTemp = "";

            if (parstrArrayCostCentreTimeAttendance == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreTimeAttendance.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreTimeAttendance[intCount].ToString();

                    if (intCount != parstrCostCentreTimeAttendance.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(strQryTemp);

            strQry.AppendLine(") AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

            //Errol 2013-06-15
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH ");
            strQry.AppendLine(" ON EPC.COMPANY_NO = EIH.COMPANY_NO ");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = EIH.EMPLOYEE_NO ");
            //Take-On Record
            strQry.AppendLine(" AND EIH.RUN_TYPE = 'T' ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = EIH.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            //Removed 2012-01-24 To Set NOT_ACTIVE_IND on Client Database
            //strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = 'T'");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.COMPANY_NO");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
           
            strQry.AppendLine(" AND ((EPC.PAY_CATEGORY_TYPE = 'W'");

            strQryTemp = "";

            if (parstrArrayCostCentreWages == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreWages.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreWages[intCount].ToString();

                    if (intCount != parstrCostCentreWages.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(" OR (EPC.PAY_CATEGORY_TYPE = 'S'");

            strQryTemp = "";

            if (parstrArrayCostCentreSalaries == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreSalaries.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreSalaries[intCount].ToString();

                    if (intCount != parstrCostCentreSalaries.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(" OR (EPC.PAY_CATEGORY_TYPE = 'T'");

            strQryTemp = "";

            if (parstrArrayCostCentreTimeAttendance == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreTimeAttendance.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreTimeAttendance[intCount].ToString();

                    if (intCount != parstrCostCentreTimeAttendance.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(") AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parint64CompanyNo);

            //2017-03-21
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EFT.COMPANY_NO");
            strQry.AppendLine(",EFT.EMPLOYEE_NO");
            strQry.AppendLine(",EFT.FINGER_NO");
            strQry.AppendLine(",EFT.FINGER_TEMPLATE");
            strQry.AppendLine(",EFT.CREATION_DATETIME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");

            strQry.AppendLine(" AND ((EPC.PAY_CATEGORY_TYPE = 'W'");

            strQryTemp = "";

            if (parstrArrayCostCentreWages == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreWages.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreWages[intCount].ToString();

                    if (intCount != parstrCostCentreWages.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(" OR (EPC.PAY_CATEGORY_TYPE = 'S'");

            strQryTemp = "";

            if (parstrArrayCostCentreSalaries == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreSalaries.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreSalaries[intCount].ToString();

                    if (intCount != parstrCostCentreSalaries.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(" OR (EPC.PAY_CATEGORY_TYPE = 'T'");

            strQryTemp = "";

            if (parstrArrayCostCentreTimeAttendance == "")
            {
                strQryTemp += "-1";
            }
            else
            {
                for (int intCount = 0; intCount < parstrCostCentreTimeAttendance.Length; intCount++)
                {
                    strQryTemp += parstrCostCentreTimeAttendance[intCount].ToString();

                    if (intCount != parstrCostCentreTimeAttendance.Length - 1)
                    {
                        strQryTemp += ",";
                    }
                }
            }

            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO IN (" + strQryTemp + "))");

            strQry.AppendLine(") AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE EFT");
            strQry.AppendLine(" ON E.COMPANY_NO = EFT.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EFT.EMPLOYEE_NO");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeeFingerTemplate", parint64CompanyNo);
            
            //2017-04-20
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" UFT.USER_NO");
            strQry.AppendLine(",UFT.FINGER_NO");
            strQry.AppendLine(",UFT.FINGER_TEMPLATE");
            strQry.AppendLine(",UFT.CREATION_DATETIME");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_ID U ");
            strQry.AppendLine(" ON U.USER_NO = UCA.USER_NO  ");
            strQry.AppendLine(" AND U.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.USER_FINGERPRINT_TEMPLATE UFT ");
            strQry.AppendLine(" ON U.USER_NO = UFT.USER_NO  ");

            strQry.AppendLine(" WHERE UCA.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL");

            //B=Both C=Client NB I=Internet Only
            strQry.AppendLine(" AND ISNULL(UCA.ACCESS_LAYER_IND,'C') IN ('B','C')");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserFingerTemplate", parint64CompanyNo);
                      
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Maintain_Templates(Int64 parint64CompanyNo, byte[] parbyteDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            
            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            //2017-07-10
            if (DataSet.Tables["PayCategoryInteractInd"] != null)
            {
                for (int intRow = 0; intRow < DataSet.Tables["PayCategoryInteractInd"].Rows.Count; intRow++)
                {
                    strQry.Clear();

                    strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY");

                    strQry.AppendLine(" SET USER_INTERACT_IND = 'Y'");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables["PayCategoryInteractInd"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables["PayCategoryInteractInd"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                }
            }
            
            for (int intRow = 0; intRow < DataSet.Tables["EmployeeFingerPrintTemplateDelete"].Rows.Count; intRow++)
            {
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateDelete"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateDelete"].Rows[intRow]["FINGER_NO"].ToString());
                strQry.AppendLine(" AND CREATION_DATETIME = '" + Convert.ToDateTime(DataSet.Tables["EmployeeFingerPrintTemplateDelete"].Rows[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            for (int intRow = 0; intRow < DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows.Count; intRow++)
            {
                if (DataSet.Tables["Temp"] != null)
                {
                    DataSet.Tables.Remove("Temp");
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" CREATION_DATETIME");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["FINGER_NO"].ToString());

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parint64CompanyNo);

                if (DataSet.Tables["Temp"].Rows.Count == 0)
                {
                    strQry.Clear();

                    string strDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",FINGER_NO ");
                    strQry.AppendLine(",FINGER_TEMPLATE ");
                    strQry.AppendLine(",CREATION_DATETIME) ");
                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("(" + parint64CompanyNo);
                    strQry.AppendLine("," + DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                    strQry.AppendLine("," + DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["FINGER_NO"].ToString());
                    strQry.AppendLine(", @FINGER_TEMPLATE");

                    strQry.AppendLine(",'" + strDateTime + "')");
                    
                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), (byte[])DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE", parint64CompanyNo);

                    DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["CREATION_DATETIME"] = strDateTime;
                }
                else
                {
                    if (DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["CREATION_DATETIME"] == System.DBNull.Value)
                    {
                        strQry.Clear();

                        string strDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" CREATION_DATETIME = '" + strDateTime + "'");
                        strQry.AppendLine(",FINGER_TEMPLATE = @FINGER_TEMPLATE");
                        strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["FINGER_NO"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), (byte[])DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE", parint64CompanyNo);

                        DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["CREATION_DATETIME"] = strDateTime;
                    }
                    else
                    {
                        if (Convert.ToDateTime(DataSet.Tables["Temp"].Rows[0]["CREATION_DATETIME"]).ToString("yyyyMMddHHmmss") == Convert.ToDateTime(DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["CREATION_DATETIME"]).ToString("yyyyMMddHHmmss"))
                        {
                            //Same
                        }
                        else
                        {
                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_FINGERPRINT_TEMPLATE");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" CREATION_DATETIME = '" + Convert.ToDateTime(DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                            strQry.AppendLine(",FINGER_TEMPLATE = @FINGER_TEMPLATE");
                            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                            strQry.AppendLine(" AND EMPLOYEE_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                            strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["FINGER_NO"].ToString());

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), (byte[])DataSet.Tables["EmployeeFingerPrintTemplateUpload"].Rows[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE", parint64CompanyNo);
                        }
                    }
                }
            }

            for (int intRow = 0; intRow < DataSet.Tables["UserFingerPrintTemplateDelete"].Rows.Count; intRow++)
            {
                //2017-04-21 Tested
                strQry.Clear();

                strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.USER_FINGERPRINT_TEMPLATE");

                strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["UserFingerPrintTemplateDelete"].Rows[intRow]["USER_NO"].ToString());
                strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["UserFingerPrintTemplateDelete"].Rows[intRow]["FINGER_NO"].ToString());
                strQry.AppendLine(" AND CREATION_DATETIME = '" + Convert.ToDateTime(DataSet.Tables["UserFingerPrintTemplateDelete"].Rows[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
            }

            for (int intRow = 0; intRow < DataSet.Tables["UserFingerPrintTemplateUpload"].Rows.Count; intRow++)
            {
                if (DataSet.Tables["Temp"] != null)
                {
                    DataSet.Tables.Remove("Temp");
                }

                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" CREATION_DATETIME");

                strQry.AppendLine(" FROM InteractPayroll.dbo.USER_FINGERPRINT_TEMPLATE");

                strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["USER_NO"].ToString());
                strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["FINGER_NO"].ToString());

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parint64CompanyNo);

                if (DataSet.Tables["Temp"].Rows.Count == 0)
                {
                    //2017-04-21 Tested
                    strQry.Clear();

                    string strDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.USER_FINGERPRINT_TEMPLATE");
                    strQry.AppendLine("(USER_NO");
                    strQry.AppendLine(",FINGER_NO ");
                    strQry.AppendLine(",FINGER_TEMPLATE ");
                    strQry.AppendLine(",CREATION_DATETIME) ");
                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("(" + DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["USER_NO"].ToString());
                    strQry.AppendLine("," + DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["FINGER_NO"].ToString());
                    strQry.AppendLine(", @FINGER_TEMPLATE");

                    strQry.AppendLine(",'" + strDateTime + "')");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), (byte[])DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE", parint64CompanyNo);

                    DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["CREATION_DATETIME"] = strDateTime;
                }
                else
                {
                    if (DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["CREATION_DATETIME"] == System.DBNull.Value)
                    {
                        strQry.Clear();

                        string strDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_FINGERPRINT_TEMPLATE");
                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" CREATION_DATETIME = '" + strDateTime + "'");
                        strQry.AppendLine(",FINGER_TEMPLATE = @FINGER_TEMPLATE");
                        strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["USER_NO"].ToString());
                        strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["FINGER_NO"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), (byte[])DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE", parint64CompanyNo);

                        DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["CREATION_DATETIME"] = strDateTime;
                    }
                    else
                    {
                        if (Convert.ToDateTime(DataSet.Tables["Temp"].Rows[0]["CREATION_DATETIME"]).ToString("yyyyMMddHHmmss") == Convert.ToDateTime(DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["CREATION_DATETIME"]).ToString("yyyyMMddHHmmss"))
                        {
                            //Same
                        }
                        else
                        {
                            //2017-04-21 Tested

                            strQry.Clear();

                            strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_FINGERPRINT_TEMPLATE");
                            strQry.AppendLine(" SET ");
                            strQry.AppendLine(" CREATION_DATETIME = '" + Convert.ToDateTime(DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["CREATION_DATETIME"]).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                            strQry.AppendLine(",FINGER_TEMPLATE = @FINGER_TEMPLATE");
                            strQry.AppendLine(" WHERE USER_NO = " + DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["USER_NO"].ToString());
                            strQry.AppendLine(" AND FINGER_NO = " + DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["FINGER_NO"].ToString());

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), (byte[])DataSet.Tables["UserFingerPrintTemplateUpload"].Rows[intRow]["FINGER_TEMPLATE"], "@FINGER_TEMPLATE", parint64CompanyNo);
                        }
                    }
                }
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                      
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
