using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace InteractPayroll
{
    public class busConvertEmployeeType
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busConvertEmployeeType()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            Get_Employee_DataSet(DataSet, parint64CompanyNo, - 1);

            Get_CurrentPayCategory_DataSet(DataSet, parint64CompanyNo, -1);
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" P.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",P.PAY_CATEGORY_NO");
            strQry.AppendLine(",P.PAY_CATEGORY_DESC");
            //Used in Reply 
            strQry.AppendLine(",'' AS REC_RATE");
            strQry.AppendLine(",'' AS DEFAULT_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY P");

            strQry.AppendLine(" WHERE P.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND P.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND P.PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND ISNULL(P.CLOSED_IND,'N') <> 'Y'");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" P.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",P.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LEAVE_SHIFT_NO");
            strQry.AppendLine(",LEAVE_SHIFT_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LEAVE_SHIFT_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveShift", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        private void Get_Employee_DataSet(DataSet parDataSet, Int64 parint64CompanyNo,int parintEmployeeNo)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");

            strQry.AppendLine(",PAY_CATEGORY_TYPE_DESC = ");
            strQry.AppendLine(" CASE");
            strQry.AppendLine(" WHEN E.PAY_CATEGORY_TYPE = 'W'");
            strQry.AppendLine(" THEN 'Wages'");

            strQry.AppendLine(" WHEN E.PAY_CATEGORY_TYPE = 'S'");
            strQry.AppendLine(" THEN 'Salaries'");

            strQry.AppendLine(" WHEN E.PAY_CATEGORY_TYPE = 'T'");
            strQry.AppendLine(" THEN 'Time Attendance'");

            strQry.AppendLine(" ELSE 'Unknown'");

            strQry.AppendLine(" END");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

            if (parintEmployeeNo != -1)
            {
                strQry.AppendLine(" AND E.EMPLOYEE_NO = " + parintEmployeeNo);
            }

            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), parDataSet, "Employee", parint64CompanyNo);
        }

        private void Get_CurrentPayCategory_DataSet(DataSet parDataSet, Int64 parint64CompanyNo, int parintEmployeeNo)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
            strQry.AppendLine(" ON E.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON EPC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND ISNULL(PC.CLOSED_IND,'N') <> 'Y'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);

            if (parintEmployeeNo != -1)
            {
                strQry.AppendLine(" AND E.EMPLOYEE_NO = " + parintEmployeeNo);
            }

            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), parDataSet, "CurrentPayCategory", parint64CompanyNo);
        }

        public byte[] Update_Employee(Int64 parInt64CompanyNo, string parstrPayrollType, string parstrFromPayrollType, int parintEmployeeNo, int parintLeaveShiftNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();

            DataSet DataSet = new DataSet();

            //Remove Current EMPLOYEE_PAY_CATEGORY Link 
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY ");

            strQry.AppendLine(" SET DATETIME_DELETE_RECORD = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "'");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromPayrollType));
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            //Relink LEAVE_SHIFT_NO 
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE ");

            strQry.AppendLine(" SET ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(",LEAVE_SHIFT_NO = " + parintLeaveShiftNo);

            if (parstrPayrollType == "S")
            {
                strQry.AppendLine(",EMPLOYEE_NUMBER_CHEQUES = 12 ");
            }

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromPayrollType));
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND EMPLOYEE_ENDDATE IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            for (int intRow = 0; intRow < parDataSet.Tables["PayCategory"].Rows.Count; intRow++)
            {
                if (DataSet.Tables["Temp"] != null)
                {
                    DataSet.Tables.Remove("Temp");
                } 

                strQry.Clear();
                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ISNULL(MAX(TIE_BREAKER),0) + 1 AS MAX_NO");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + parDataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

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

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(parInt64CompanyNo.ToString());
                strQry.AppendLine("," + parintEmployeeNo);
                strQry.AppendLine("," + Convert.ToInt32(parDataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"]));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine("," + Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]));
                strQry.AppendLine("," + Convert.ToDouble(parDataSet.Tables["PayCategory"].Rows[intRow]["REC_RATE"]));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PayCategory"].Rows[intRow]["DEFAULT_IND"].ToString()));

                strQry.AppendLine(",LEAVE_DAY_RATE_DECIMAL = ");

                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN PC.PAY_CATEGORY_TYPE = 'W' THEN");
                
                strQry.AppendLine(" ROUND(SUM(PCTD.TIME_DECIMAL) / COUNT(*),2)");
                
                strQry.AppendLine(" ELSE ROUND(PC.SALARY_MINUTES_PAID_PER_DAY / 60,2)");

                strQry.AppendLine(" END ");
                
                strQry.AppendLine(",GETDATE()");
                //Current User
                strQry.AppendLine(",0");
                
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL PCTD");
                strQry.AppendLine(" ON PC.COMPANY_NO = PCTD.COMPANY_NO ");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCTD.PAY_CATEGORY_NO ");

                strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = " + parDataSet.Tables["PayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
                
                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" PC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(",PC.SALARY_MINUTES_PAID_PER_DAY");
                
                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }
            
            strQry.Clear();

            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EARNING ");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",EARNING_DESC");
            strQry.AppendLine(",SPREAD_SHEET_TYPE_IND");
            strQry.AppendLine(",LEAVE_PERCENTAGE");
            strQry.AppendLine(",EARNING_REPORT_HEADER1");
            strQry.AppendLine(",EARNING_REPORT_HEADER2");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",TAX_PERCENTAGE");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_RECORD");
            strQry.AppendLine(",DATETIME_DELETE_RECORD");
            strQry.AppendLine(",EARNING_DEL_IND)");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EARNING_NO");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(",E.TIE_BREAKER");
            strQry.AppendLine(",E.EARNING_DESC");
            strQry.AppendLine(",E.SPREAD_SHEET_TYPE_IND");
            strQry.AppendLine(",E.LEAVE_PERCENTAGE");
            strQry.AppendLine(",E.EARNING_REPORT_HEADER1");
            strQry.AppendLine(",E.EARNING_REPORT_HEADER2");
            strQry.AppendLine(",E.IRP5_CODE");
            strQry.AppendLine(",E.TAX_PERCENTAGE");
            strQry.AppendLine(",E.USER_NO_NEW_RECORD");
            strQry.AppendLine(",E.DATETIME_NEW_RECORD");
            strQry.AppendLine(",E.USER_NO_RECORD");
            strQry.AppendLine(",E.DATETIME_DELETE_RECORD");
            strQry.AppendLine(",E.EARNING_DEL_IND");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EARNING E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE ");
            strQry.AppendLine(" ON E.COMPANY_NO = EE.COMPANY_NO ");
            strQry.AppendLine(" AND EE.EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND E.EARNING_NO = EE.EARNING_NO ");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EE.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EARN");
            strQry.AppendLine(" ON E.COMPANY_NO = EARN.COMPANY_NO ");
            strQry.AppendLine(" AND E.EARNING_DESC = EARN.EARNING_DESC ");
            strQry.AppendLine(" AND EARN.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromPayrollType));
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            //Not Defaults and Not Leave
            strQry.AppendLine(" AND E.EARNING_NO > 9");
            strQry.AppendLine(" AND E.EARNING_NO < 200");
            
            //Record Does NOT Exist
            strQry.AppendLine(" AND EARN.COMPANY_NO IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            
            strQry.Clear();

            strQry.AppendLine(" UPDATE EE ");

            strQry.AppendLine(" SET ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                    
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING EE ");
            
            strQry.AppendLine(" WHERE EE.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EE.EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND EE.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromPayrollType));
            strQry.AppendLine(" AND EE.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            
            strQry.Clear();

            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.DEDUCTION ");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEDUCTION_DESC");
            strQry.AppendLine(",IRP5_CODE");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_RECORD");
            strQry.AppendLine(",DATETIME_DELETE_RECORD)");
            
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" D.COMPANY_NO");
            strQry.AppendLine(",D.DEDUCTION_NO");
            strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(",D.DEDUCTION_DESC");
            strQry.AppendLine(",D.IRP5_CODE");
            strQry.AppendLine(",D.DEDUCTION_SUB_ACCOUNT_COUNT");
            strQry.AppendLine(",D.DEDUCTION_REPORT_HEADER1");
            strQry.AppendLine(",D.DEDUCTION_REPORT_HEADER2");
            strQry.AppendLine(",D.DEDUCTION_LOAN_TYPE_IND");
            strQry.AppendLine(",D.USER_NO_NEW_RECORD");
            strQry.AppendLine(",D.DATETIME_NEW_RECORD");
            strQry.AppendLine(",D.USER_NO_RECORD");
            strQry.AppendLine(",D.DATETIME_DELETE_RECORD");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEDUCTION D");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED ");
            strQry.AppendLine(" ON D.COMPANY_NO = ED.COMPANY_NO ");
            strQry.AppendLine(" AND ED.EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND D.DEDUCTION_NO = ED.DEDUCTION_NO ");
            strQry.AppendLine(" AND D.DEDUCTION_SUB_ACCOUNT_NO = ED.DEDUCTION_SUB_ACCOUNT_NO ");
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = ED.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION DED");
            strQry.AppendLine(" ON D.COMPANY_NO = DED.COMPANY_NO ");
            strQry.AppendLine(" AND D.DEDUCTION_NO = DED.DEDUCTION_NO ");
            strQry.AppendLine(" AND DED.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            
            strQry.AppendLine(" WHERE D.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND D.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromPayrollType));
            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL");
            //Record Does NOT Exist
            strQry.AppendLine(" AND DED.COMPANY_NO IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIE_BREAKER");
            strQry.AppendLine(",DEDUCTION_TYPE_IND");
            strQry.AppendLine(",DEDUCTION_VALUE");
            strQry.AppendLine(",DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_RECORD");
            strQry.AppendLine(",DATETIME_DELETE_RECORD)");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ED.COMPANY_NO");
            strQry.AppendLine(",ED.EMPLOYEE_NO");
            strQry.AppendLine(",ED.DEDUCTION_NO");
            strQry.AppendLine(",ED.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(",ED.TIE_BREAKER");
            strQry.AppendLine(",ED.DEDUCTION_TYPE_IND");
            strQry.AppendLine(",ED.DEDUCTION_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_MIN_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_MAX_VALUE");
            strQry.AppendLine(",ED.DEDUCTION_PERIOD_IND");
            strQry.AppendLine(",ED.DEDUCTION_DAY_VALUE");
            strQry.AppendLine(",ED.USER_NO_NEW_RECORD");
            strQry.AppendLine(",ED.DATETIME_NEW_RECORD");
            strQry.AppendLine(",ED.USER_NO_RECORD");
            strQry.AppendLine(",ED.DATETIME_DELETE_RECORD");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED_Compare");
            strQry.AppendLine(" ON ED.COMPANY_NO = ED_Compare.COMPANY_NO");
            strQry.AppendLine(" AND ED.EMPLOYEE_NO = ED_Compare.EMPLOYEE_NO");
            strQry.AppendLine(" AND ED.DEDUCTION_NO = ED_Compare.DEDUCTION_NO");
            strQry.AppendLine(" AND ED.DEDUCTION_SUB_ACCOUNT_NO = ED_Compare.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(" AND ED_Compare.PAY_CATEGORY_TYPE  = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            
            strQry.AppendLine(" WHERE ED.COMPANY_NO = " + parInt64CompanyNo);
            //Default
            strQry.AppendLine(" AND ED.EMPLOYEE_NO = 0");
            strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromPayrollType));
            strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");
            
            //Record Does NOT Exist
            strQry.AppendLine(" AND ED_Compare.COMPANY_NO IS NULL ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE ED ");

            strQry.AppendLine(" SET PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
          
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION ED ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION DED");
            strQry.AppendLine(" ON ED.COMPANY_NO = DED.COMPANY_NO ");
            strQry.AppendLine(" AND ED.DEDUCTION_NO = DED.DEDUCTION_NO ");
            strQry.AppendLine(" AND ED.DEDUCTION_SUB_ACCOUNT_NO = DED.DEDUCTION_SUB_ACCOUNT_NO ");
            strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = DED.PAY_CATEGORY_TYPE ");
                      
            strQry.AppendLine(" WHERE ED.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND ED.EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND ED.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromPayrollType));
            strQry.AppendLine(" AND ED.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE EDEP ");

            strQry.AppendLine(" SET PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE EDEP ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EARNING EARN");
            strQry.AppendLine(" ON EDEP.COMPANY_NO = EARN.COMPANY_NO ");
            strQry.AppendLine(" AND EDEP.EARNING_NO = EARN.EARNING_NO ");
            strQry.AppendLine(" AND EDEP.PAY_CATEGORY_TYPE = EARN.PAY_CATEGORY_TYPE ");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION DED");
            strQry.AppendLine(" ON EDEP.COMPANY_NO = DED.COMPANY_NO ");
            strQry.AppendLine(" AND EDEP.DEDUCTION_NO = DED.DEDUCTION_NO ");
            strQry.AppendLine(" AND EDEP.DEDUCTION_SUB_ACCOUNT_NO = DED.DEDUCTION_SUB_ACCOUNT_NO ");
            strQry.AppendLine(" AND EDEP.PAY_CATEGORY_TYPE = DED.PAY_CATEGORY_TYPE ");
            
            strQry.AppendLine(" WHERE EDEP.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EDEP.EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND EDEP.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromPayrollType));
            strQry.AppendLine(" AND EDEP.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            
            if (parstrPayrollType == "W")
            {
                strQry.Clear();

                strQry.AppendLine(" UPDATE EDEP ");

                //2=Normal Time
                strQry.AppendLine(" SET EARNING_NO = 2 ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE EDEP ");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE EDEP_Check ");
                strQry.AppendLine(" ON EDEP.COMPANY_NO = EDEP_Check.COMPANY_NO  ");
                strQry.AppendLine(" AND EDEP.EMPLOYEE_NO = EDEP_Check.EMPLOYEE_NO  ");
                strQry.AppendLine(" AND EDEP_Check.EARNING_NO = 2  ");
                strQry.AppendLine(" AND EDEP.PAY_CATEGORY_TYPE = EDEP_Check.PAY_CATEGORY_TYPE  ");

                strQry.AppendLine(" WHERE EDEP.COMPANY_NO = " + parInt64CompanyNo);
                strQry.AppendLine(" AND EDEP.EMPLOYEE_NO = " + parintEmployeeNo);
                //PAY_CATEGORY_TYPE already Changed
                strQry.AppendLine(" AND EDEP.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                //1=Income
                strQry.AppendLine(" AND EDEP.EARNING_NO = 1 ");

                strQry.AppendLine(" AND EDEP.DATETIME_DELETE_RECORD IS NULL");

                //Records do NOT already Exist
                strQry.AppendLine(" AND EDEP_Check.COMPANY_NO IS NULL  ");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.Clear();

                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE ");
                    strQry.AppendLine("(COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",DEDUCTION_NO");
                    strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                    strQry.AppendLine(",EARNING_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",TIE_BREAKER");
                    strQry.AppendLine(",USER_NO_NEW_RECORD");
                    strQry.AppendLine(",DATETIME_NEW_RECORD");
                    strQry.AppendLine(",USER_NO_RECORD");
                    strQry.AppendLine(",DATETIME_DELETE_RECORD)");

                    strQry.AppendLine(" SELECT ");

                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",DEDUCTION_NO");
                    strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
                    //Income
                    strQry.AppendLine(",1");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",TIE_BREAKER");
                    strQry.AppendLine(",USER_NO_NEW_RECORD");
                    strQry.AppendLine(",DATETIME_NEW_RECORD");
                    strQry.AppendLine(",USER_NO_RECORD");
                    strQry.AppendLine(",DATETIME_DELETE_RECORD");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_EARNING_PERCENTAGE  ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
                    //PAY_CATEGORY_TYPE already Changed
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                    //2=Normal Time
                    strQry.AppendLine(" AND EARNING_NO = 2 ");

                    strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                }
            }
            
            strQry.Clear();

            strQry.AppendLine(" UPDATE L ");

            strQry.AppendLine(" SET PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LOANS L ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.DEDUCTION DED");
            strQry.AppendLine(" ON L.COMPANY_NO = DED.COMPANY_NO ");
            strQry.AppendLine(" AND L.DEDUCTION_NO = DED.DEDUCTION_NO ");
            strQry.AppendLine(" AND L.DEDUCTION_SUB_ACCOUNT_NO = DED.DEDUCTION_SUB_ACCOUNT_NO ");
            strQry.AppendLine(" AND L.PAY_CATEGORY_TYPE = DED.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(" WHERE L.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND L.EMPLOYEE_NO = " + parintEmployeeNo);
            strQry.AppendLine(" AND L.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromPayrollType));
            strQry.AppendLine(" AND L.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            
            strQry.Clear();

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ");
                }
                else
                {
                    if (parstrPayrollType == "T")
                    {
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                    }
                }
            }

            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_DATE");
            strQry.AppendLine(",TIMESHEET_SEQ");
            strQry.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_OUT_MINUTES");
            strQry.AppendLine(",INDICATOR");
            strQry.AppendLine(",TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine(",USER_NO_TIME_IN");
            strQry.AppendLine(",USER_NO_TIME_OUT)");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EBC.COMPANY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            //New PAY_CATEGORY_NO
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.TIMESHEET_DATE");
            strQry.AppendLine(",EBC.TIMESHEET_SEQ");
            strQry.AppendLine(",EBC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(",EBC.CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.CLOCKED_TIME_OUT_MINUTES");
            strQry.AppendLine(",EBC.INDICATOR");
            strQry.AppendLine(",EBC.TIMESHEET_ACCUM_MINUTES");
            strQry.AppendLine(",EBC.USER_NO_TIME_IN");
            strQry.AppendLine(",EBC.USER_NO_TIME_OUT");

            if (parstrFromPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT EBC ");
            }
            else
            {
                if (parstrFromPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT EBC ");
                }
                else
                {
                    if (parstrFromPayrollType == "T")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT EBC ");
                    }
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON EBC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            //New EMPLOYEE_PAY_CATEGORY
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            strQry.AppendLine(" WHERE EBC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = " + parintEmployeeNo);
           
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            if (parstrFromPayrollType == "W")
            {
                strQry.AppendLine("DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ");
            }
            else
            {
                if (parstrFromPayrollType == "S")
                {
                    strQry.AppendLine("DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ");
                }
                else
                {
                    if (parstrFromPayrollType == "T")
                    {
                        strQry.AppendLine("DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ");
                    }
                }
            }
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ");
                }
                else
                {
                    if (parstrPayrollType == "T")
                    {
                        strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                    }
                }
            }

            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_DATE");
            strQry.AppendLine(",BREAK_SEQ");
            strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",CLOCKED_TIME_OUT_MINUTES");
            strQry.AppendLine(",INDICATOR");
            strQry.AppendLine(",BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",USER_NO_TIME_IN");
            strQry.AppendLine(",USER_NO_TIME_OUT)");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EBC.COMPANY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            //New PAY_CATEGORY_NO
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(",EBC.CLOCKED_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.CLOCKED_TIME_OUT_MINUTES");
            strQry.AppendLine(",EBC.INDICATOR");
            strQry.AppendLine(",EBC.BREAK_ACCUM_MINUTES");
            strQry.AppendLine(",EBC.USER_NO_TIME_IN");
            strQry.AppendLine(",EBC.USER_NO_TIME_OUT");

            if (parstrFromPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT EBC ");
            }
            else
            {
                if (parstrFromPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC ");
                }
                else
                {
                    if (parstrFromPayrollType == "T")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC ");
                    }
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            strQry.AppendLine(" ON EBC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            //New EMPLOYEE_PAY_CATEGORY
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            strQry.AppendLine(" WHERE EBC.COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = " + parintEmployeeNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            strQry.Clear();

            if (parstrFromPayrollType == "W")
            {
                strQry.AppendLine("DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT ");
            }
            else
            {
                if (parstrFromPayrollType == "S")
                {
                    strQry.AppendLine("DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT ");
                }
                else
                {
                    if (parstrFromPayrollType == "T")
                    {
                        strQry.AppendLine("DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT ");
                    }
                }
            }

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LEAVE_CURRENT ");

            //1=Income
            strQry.AppendLine(" SET PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
            //PAY_CATEGORY_TYPE already Changed
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromPayrollType));

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            
            strQry.Clear();

            //Batch Processing Earning
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_BATCH_TEMP ");

            //1=Income
            strQry.AppendLine(" SET PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
            //PAY_CATEGORY_TYPE already Changed
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromPayrollType));

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            
            strQry.Clear();

            //Batch Processing Deduction
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_BATCH_TEMP ");

            //1=Income
            strQry.AppendLine(" SET PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO = " + parintEmployeeNo);
            //PAY_CATEGORY_TYPE already Changed
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromPayrollType));

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            Get_Employee_DataSet(DataSet, parInt64CompanyNo, parintEmployeeNo);

            Get_CurrentPayCategory_DataSet(DataSet, parInt64CompanyNo, parintEmployeeNo);

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
    }
}
