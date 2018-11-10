using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

namespace InteractPayroll
{
    public class busRptTimeSheet
    {
        clsDBConnectionObjects clsDBConnectionObjects;

		public busRptTimeSheet()
		{
			clsDBConnectionObjects = new clsDBConnectionObjects();
		}

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, string parstrFromProgram, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
		{
            StringBuilder strQry = new StringBuilder();
			DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" E.COMPANY_NO ");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH");
            strQry.AppendLine(" ON E.COMPANY_NO = EPCH.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPCH.EMPLOYEE_NO");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE)  
            //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
            strQry.AppendLine(" ON E.COMPANY_NO = PCPH.COMPANY_NO");
            strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPCH.RUN_TYPE = PCPH.RUN_TYPE");
            strQry.AppendLine(" AND EPCH.RUN_NO = PCPH.RUN_NO");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" EPCH.COMPANY_NO");
            strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(",EPCH.EMPLOYEE_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH ");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
            strQry.AppendLine(" ON EPCH.COMPANY_NO = PCPH.COMPANY_NO");
            strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPCH.RUN_TYPE = PCPH.RUN_TYPE");
            strQry.AppendLine(" AND EPCH.RUN_NO = PCPH.RUN_NO");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" WHERE EPCH.COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategory", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EPC.COMPANY_NO");
            strQry.AppendLine(",EPC.EMPLOYEE_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EPC.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            
            strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parint64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EmployeePayCategoryCurrent", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCPH.COMPANY_NO ");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND PCPH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE PCPH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE IN ('W','S')");
            }
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCPH.COMPANY_NO ");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");
           
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PCPH.COMPANY_NO ");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_PERIOD_DATE DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayPeriodDate", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PUBLIC_HOLIDAY_DATE");
            strQry.AppendLine(",PUBLIC_HOLIDAY_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_HISTORY ");
      
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PUBLIC_HOLIDAY_DATE");
            strQry.AppendLine(",PUBLIC_HOLIDAY_DESC");


            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PUBLIC_HOLIDAY_DATE DESC");
            strQry.AppendLine(",PUBLIC_HOLIDAY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PublicHolidayDate", parint64CompanyNo);
	
			strQry.Clear();

			strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETH.COMPANY_NO ");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(",MIN(ETH.TIMESHEET_DATE) AS MIN_TIMESHEET_DATE");
            strQry.AppendLine(",MAX(ETH.TIMESHEET_DATE) AS MAX_TIMESHEET_DATE");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY ETH");
            }
            else
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_HISTORY ETH");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH ");
            strQry.AppendLine(" ON ETH.COMPANY_NO = EPCH.COMPANY_NO");
            strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND ETH.EMPLOYEE_NO = EPCH.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = EPCH.PAY_CATEGORY_NO");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = 'W'");
            }
           
            strQry.AppendLine(" AND EPCH.RUN_TYPE = 'P'");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
            strQry.AppendLine(" ON EPCH.COMPANY_NO = PCPH.COMPANY_NO");
            strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE");
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPCH.RUN_TYPE = PCPH.RUN_TYPE");
            strQry.AppendLine(" AND EPCH.RUN_NO = PCPH.RUN_NO");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'W'");
            }

            strQry.AppendLine(" WHERE ETH.COMPANY_NO = " + parint64CompanyNo);
			strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" ETH.COMPANY_NO ");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE ");

            if (parstrFromProgram != "X")
            {
                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ESTH.COMPANY_NO ");
                strQry.AppendLine(",'S' AS PAY_CATEGORY_TYPE ");
                strQry.AppendLine(",MIN(ESTH.TIMESHEET_DATE) AS MIN_TIMESHEET_DATE");
                strQry.AppendLine(",MAX(ESTH.TIMESHEET_DATE) AS MAX_TIMESHEET_DATE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_HISTORY ESTH");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH ");
                strQry.AppendLine(" ON ESTH.COMPANY_NO = EPCH.COMPANY_NO");
                strQry.AppendLine(" AND ESTH.PAY_PERIOD_DATE = EPCH.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND ESTH.EMPLOYEE_NO = EPCH.EMPLOYEE_NO");
                strQry.AppendLine(" AND ESTH.PAY_CATEGORY_NO = EPCH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND EPCH.RUN_TYPE = 'P'");

                //2013-08-30
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                    strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
                strQry.AppendLine(" ON EPCH.COMPANY_NO = PCPH.COMPANY_NO");
                strQry.AppendLine(" AND EPCH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EPCH.RUN_TYPE = PCPH.RUN_TYPE");
                strQry.AppendLine(" AND EPCH.RUN_NO = PCPH.RUN_NO");

                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = 'S'");

                strQry.AppendLine(" WHERE ESTH.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" GROUP BY");
                strQry.AppendLine(" ESTH.COMPANY_NO ");
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Date", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETH.COMPANY_NO ");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE ");

            strQry.AppendLine(",MIN(ETH.TIMESHEET_DATE) AS MIN_TIMESHEET_DATE");
            strQry.AppendLine(",MAX(ETH.TIMESHEET_DATE) AS MAX_TIMESHEET_DATE");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETH");
            }
            else
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETH");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPCH ");
            strQry.AppendLine(" ON ETH.COMPANY_NO = EPCH.COMPANY_NO");
            strQry.AppendLine(" AND ETH.EMPLOYEE_NO = EPCH.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = EPCH.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPCH.DATETIME_DELETE_RECORD IS NULL");

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = 'W'");
            }

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PCPH");
            strQry.AppendLine(" ON EPCH.COMPANY_NO = PCPH.COMPANY_NO");
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PCPH.DATETIME_DELETE_RECORD IS NULL");
            
            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'W'");
            }

            strQry.AppendLine(" WHERE ETH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" GROUP BY");
            strQry.AppendLine(" ETH.COMPANY_NO ");
            strQry.AppendLine(",EPCH.PAY_CATEGORY_TYPE ");

            if (parstrFromProgram != "X")
            {
                strQry.AppendLine(" UNION ");

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" ESTH.COMPANY_NO ");
                strQry.AppendLine(",'S' AS PAY_CATEGORY_TYPE ");
                strQry.AppendLine(",MIN(ESTH.TIMESHEET_DATE) AS MIN_TIMESHEET_DATE");
                strQry.AppendLine(",MAX(ESTH.TIMESHEET_DATE) AS MAX_TIMESHEET_DATE");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ESTH");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_CURRENT EPCH ");
                strQry.AppendLine(" ON ESTH.COMPANY_NO = EPCH.COMPANY_NO");
                strQry.AppendLine(" AND ESTH.EMPLOYEE_NO = EPCH.EMPLOYEE_NO");
                strQry.AppendLine(" AND ESTH.PAY_CATEGORY_NO = EPCH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = 'S'");
                
                //2013-08-30
                if (parstrCurrentUserAccess == "U")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                    strQry.AppendLine(" AND EPCH.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND EPCH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PCPH");
                strQry.AppendLine(" ON EPCH.COMPANY_NO = PCPH.COMPANY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EPCH.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PCPH.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'S'");

                strQry.AppendLine(" WHERE ESTH.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" GROUP BY");
                strQry.AppendLine(" ESTH.COMPANY_NO ");
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "DateCurrent", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCPH.COMPANY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND PCPH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON PCPH.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE PCPH.COMPANY_NO = " + parint64CompanyNo);

            if (parstrFromProgram == "X")
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = 'T'");
            }
            else
            {
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE IN ('W','S')");
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCPH.COMPANY_NO");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PCPH.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

			byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
			DataSet.Dispose();
			DataSet = null;

			return bytCompress;
		}

        public byte[] Print_Report_Detailed(Int64 parint64CompanyNo, string parstrPayrollType, string parstrTableTypeInd, string parstrReportType, string parstrEmployeeNoIN, string parstrPayCategoryNoIN, string parstrFromDate, string parstrToDate, string parstrFilterType, string parstrTimeRule, int parintTimeRule, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
			DataSet DataSet = new DataSet();

            string strTableType = "HISTORY";

            if (parstrTableTypeInd == "C")
            {
                strTableType = "CURRENT";
            }
                        
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_DESC");
            strQry.AppendLine(",CL.DATE_FORMAT");
            strQry.AppendLine(",'' AS REPORT_DATETIME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
            strQry.AppendLine(" ON C.COMPANY_NO = CL.COMPANY_NO ");

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportHeader", parint64CompanyNo);

            if (DataSet.Tables["ReportHeader"].Rows.Count > 0)
            {
                DataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"] = "Printed   " + DateTime.Now.ToString(DataSet.Tables["ReportHeader"].Rows[0]["DATE_FORMAT"].ToString() + "   HH:mm");
            }

			strQry.Clear();

			strQry.AppendLine(" SELECT ");
			strQry.AppendLine(" ETH.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETH.EMPLOYEE_NO");

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",ETH.PAY_PERIOD_DATE");
            }

			strQry.AppendLine(",ETH.TIMESHEET_DATE");
			strQry.AppendLine(",ETH.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETH.TIMESHEET_TIME_OUT_MINUTES");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_" + strTableType + " ETH");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_" + strTableType + " ETH");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_" + strTableType + " ETH");
                }
            }

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
                strQry.AppendLine(" ON ETH.COMPANY_NO = PCPH.COMPANY_NO");
                strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");
            }
            else
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PCPH");
                strQry.AppendLine(" ON ETH.COMPANY_NO = PCPH.COMPANY_NO");
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                strQry.AppendLine(" AND PCPH.DATETIME_DELETE_RECORD IS NULL");
            }

            //2015-10-14
            if (parstrFilterType == "I"
            || parstrFilterType == "O")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT ");

                strQry.AppendLine(" EMPLOYEE_NO ");
                strQry.AppendLine(",PAY_CATEGORY_NO ");
                strQry.AppendLine(",TIMESHEET_DATE ");
              
                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_" + strTableType + " ETH");
                }
                else
                {
                    if (parstrPayrollType == "S")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_" + strTableType + " ETH");
                    }
                    else
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_" + strTableType + " ETH");
                    }
                }

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                strQry.AppendLine(" GROUP BY ");

                strQry.AppendLine(" EMPLOYEE_NO ");
                strQry.AppendLine(",PAY_CATEGORY_NO ");
                strQry.AppendLine(",TIMESHEET_DATE ");


                string strTime = parintTimeRule.ToString();
                int intTime = 0;

                if (strTime.Length > 2)
                {
                    intTime = Convert.ToInt32(strTime.Substring(0, strTime.Length - 2)) * 60;
                    intTime += Convert.ToInt32(strTime.Substring(strTime.Length - 2, 2));

                }
                else
                {
                    intTime = parintTimeRule;
                }
                
                if (parstrFilterType == "I")
                {
                    strQry.AppendLine(" HAVING MIN(TIMESHEET_TIME_IN_MINUTES) ");
                }
                else
                {
                    strQry.AppendLine(" HAVING MAX(TIMESHEET_TIME_OUT_MINUTES) ");
                }

                if (parstrTimeRule == "G")
                {
                    strQry.Append(" > " + intTime + ") AS CALC_TABLE ");
                }
                else
                {
                    strQry.Append(" < " + intTime + ") AS CALC_TABLE ");
                }

                strQry.AppendLine(" ON ETH.EMPLOYEE_NO = CALC_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = CALC_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND ETH.TIMESHEET_DATE = CALC_TABLE.TIMESHEET_DATE");
            }

            if (parstrTableTypeInd == "C")
            {
                strQry.AppendLine(" LEFT JOIN ");

                //Get SQL For Errors in Timesheets / Breaks (Returns EMPLOYEE_NO,TIMESHEET_DATE,PAY_CATEGORY_NO)
                strQry.Append(Get_Current_Error_SQL(parint64CompanyNo, parstrPayrollType));

                strQry.AppendLine(" ON ETH.EMPLOYEE_NO = ERRORS_TABLE.EMPLOYEE_NO  ");
                strQry.AppendLine(" AND ETH.TIMESHEET_DATE = ERRORS_TABLE.TIMESHEET_DATE");
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = ERRORS_TABLE.PAY_CATEGORY_NO ");
            }

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND ETH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND ETH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE ETH.COMPANY_NO = " + parint64CompanyNo);

            if (parstrPayCategoryNoIN != "")
            {
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
            }

            if (parstrEmployeeNoIN != "")
            {
                strQry.AppendLine(" AND ETH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
            }

            //Pay Period
            if (parstrReportType == "P")
            {
                if (parstrTableTypeInd == "H")
                {
                    strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                }
            }
            else
            {
                //Week
                if (parstrReportType == "W")
                {
                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                }
                else
                {
                    if (parstrReportType == "M")
                    {
                        strQry.AppendLine(" AND YEAR(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(0, 4) + " AND MONTH(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(4));
                    }
                    else
                    {
                        if (parstrReportType == "E"
                        || parstrReportType == "H")
                        {
                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE = '" + parstrFromDate + "'");
                        }
                        else
                        {
                            if (parstrReportType == "G")
                            {
                                strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                            }
                            else
                            {
                                if (parstrReportType == "L")
                                {
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrFromDate + "'");
                                }
                                else
                                {
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrToDate + "'");
                                }
                            }
                        }
                    }
                }
            }

            //Current
            if (parstrTableTypeInd == "C")
            {
                //Errors
                strQry.AppendLine(" AND ERRORS_TABLE.EMPLOYEE_NO IS NULL  ");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" ETH.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETH.EMPLOYEE_NO");

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",ETH.PAY_PERIOD_DATE");
            }

            strQry.AppendLine(",ETH.TIMESHEET_DATE");
            strQry.AppendLine(",ETH.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETH.TIMESHEET_TIME_OUT_MINUTES");
			
            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportTimeSheet", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EBH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBH.EMPLOYEE_NO");

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",EBH.PAY_PERIOD_DATE");
            }

            strQry.AppendLine(",EBH.BREAK_DATE");
           
            strQry.AppendLine(",EBH.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBH.BREAK_TIME_OUT_MINUTES");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_" + strTableType + " EBH");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_" + strTableType + " EBH");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_" + strTableType + " EBH");
                }
            }


            if (parstrTableTypeInd == "C")
            {
                strQry.AppendLine(" LEFT JOIN ");

                //Get SQL For Errors in Timesheets / Breaks (Returns EMPLOYEE_NO,TIMESHEET_DATE,PAY_CATEGORY_NO)
                strQry.Append(Get_Current_Error_SQL(parint64CompanyNo, parstrPayrollType));

                strQry.AppendLine(" ON EBH.EMPLOYEE_NO = ERRORS_TABLE.EMPLOYEE_NO  ");
                strQry.AppendLine(" AND EBH.BREAK_DATE = ERRORS_TABLE.TIMESHEET_DATE");
                strQry.AppendLine(" AND EBH.PAY_CATEGORY_NO = ERRORS_TABLE.PAY_CATEGORY_NO ");
            }

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EBH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EBH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EBH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            }

            strQry.AppendLine(" WHERE EBH.COMPANY_NO = " + parint64CompanyNo);

            if (parstrPayCategoryNoIN != "")
            {
                strQry.AppendLine(" AND EBH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
            }

            if (parstrEmployeeNoIN != "")
            {
                strQry.AppendLine(" AND EBH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
            }
            
            //Pay Period
            if (parstrReportType == "P")
            {
                if (parstrTableTypeInd == "H")
                {
                    strQry.AppendLine(" AND EBH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                }
            }
            else
            {
                //Week
                if (parstrReportType == "W")
                {
                    strQry.AppendLine(" AND EBH.BREAK_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                    strQry.AppendLine(" AND EBH.BREAK_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                }
                else
                {
                    if (parstrReportType == "M")
                    {
                        strQry.AppendLine(" AND YEAR(EBH.BREAK_DATE) = " + parstrFromDate.Substring(0, 4) + " AND MONTH(EBH.BREAK_DATE) = " + parstrFromDate.Substring(4));
                    }
                    else
                    {
                        if (parstrReportType == "E"
                        || parstrReportType == "H")
                        {
                            strQry.AppendLine(" AND EBH.BREAK_DATE = '" + parstrFromDate + "'");
                        }
                        else
                        {
                            if (parstrReportType == "G")
                            {
                                strQry.AppendLine(" AND EBH.BREAK_DATE >= '" + parstrFromDate + "'");
                            }
                            else
                            {
                                if (parstrReportType == "L")
                                {
                                    strQry.AppendLine(" AND EBH.BREAK_DATE <= '" + parstrFromDate + "'");
                                }
                                else
                                {
                                    strQry.AppendLine(" AND EBH.BREAK_DATE >= '" + parstrFromDate + "'");
                                    strQry.AppendLine(" AND EBH.BREAK_DATE <= '" + parstrToDate + "'");
                                }
                            }
                        }
                    }
                }
            }

            //Current
            if (parstrTableTypeInd == "C")
            {
                //Errors
                strQry.AppendLine(" AND ERRORS_TABLE.EMPLOYEE_NO IS NULL  ");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EBH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBH.EMPLOYEE_NO");
            
            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",EBH.PAY_PERIOD_DATE");
            }
            
            strQry.AppendLine(",EBH.BREAK_DATE");
            strQry.AppendLine(",EBH.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBH.BREAK_TIME_OUT_MINUTES");
            
            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Break", parint64CompanyNo);
         
            DataSet.Tables["ReportTimeSheet"].Columns.Add("BREAK_TIME_IN_MINUTES", typeof(System.Int16));
            DataSet.Tables["ReportTimeSheet"].Columns.Add("BREAK_TIME_OUT_MINUTES", typeof(System.Int16));

            DataView pvtReportTimeSheetDataView = new DataView();
          
            //Merge Timesheets and Breaks
            for (int intRow = 0; intRow < DataSet.Tables["Break"].Rows.Count; intRow++)
            {
                pvtReportTimeSheetDataView = null;
                pvtReportTimeSheetDataView = new DataView(DataSet.Tables["ReportTimeSheet"],
                    "PAY_CATEGORY_NO = " + DataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString()
                     + " AND EMPLOYEE_NO = " + DataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO"].ToString()
                     + " AND TIMESHEET_DATE = '" + Convert.ToDateTime(DataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]).ToString("yyyy-MM-dd") + "'"
                     + " AND BREAK_TIME_OUT_MINUTES IS NULL",
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

                if (pvtReportTimeSheetDataView.Count > 0)
                {
                    if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"] != System.DBNull.Value)
                    {
                        pvtReportTimeSheetDataView[0]["BREAK_TIME_IN_MINUTES"] = Convert.ToInt16(DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"]);
                    }

                    if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"] != System.DBNull.Value)
                    {
                        pvtReportTimeSheetDataView[0]["BREAK_TIME_OUT_MINUTES"] = Convert.ToInt16(DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"]);
                    }
                }
                else
                {
                    DataRow drDataRow = DataSet.Tables["ReportTimeSheet"].NewRow();

                    drDataRow["PAY_CATEGORY_NO"] = DataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString();

                    drDataRow["EMPLOYEE_NO"] = DataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO"].ToString();

                    if (parstrTableTypeInd == "H")
                    {
                        drDataRow["PAY_PERIOD_DATE"] = Convert.ToDateTime(DataSet.Tables["Break"].Rows[intRow]["PAY_PERIOD_DATE"].ToString());
                    }

                    drDataRow["TIMESHEET_DATE"] = Convert.ToDateTime(DataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"].ToString());

                    drDataRow["TIMESHEET_TIME_IN_MINUTES"] = System.DBNull.Value;
                    drDataRow["TIMESHEET_TIME_OUT_MINUTES"] = System.DBNull.Value;

                    if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"] != System.DBNull.Value)
                    {
                        drDataRow["BREAK_TIME_IN_MINUTES"] = Convert.ToInt16(DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"]);
                    }

                    if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"] != System.DBNull.Value)
                    {
                        drDataRow["BREAK_TIME_OUT_MINUTES"] = Convert.ToInt16(DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"]);
                    }

                    DataSet.Tables["ReportTimeSheet"].Rows.Add(drDataRow);
                }
            }

            strQry.Clear();

            string strTablePrefix = "PCWH";

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCWH.PAY_CATEGORY_NO");

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");
                strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            }
            else
            {
                strQry.AppendLine(",PCWH.PAY_CATEGORY_DESC");
            }

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",PCWH.WEEK_DATE_FROM");
                strQry.AppendLine(",PCWH.WEEK_DATE");
            }
            
            strQry.AppendLine(",PCWH.EXCEPTION_SUN_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_SUN_BELOW_MINUTES");

            strQry.AppendLine(",PCWH.EXCEPTION_MON_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_MON_BELOW_MINUTES");

            strQry.AppendLine(",PCWH.EXCEPTION_TUE_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_TUE_BELOW_MINUTES");

            strQry.AppendLine(",PCWH.EXCEPTION_WED_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_WED_BELOW_MINUTES");

            strQry.AppendLine(",PCWH.EXCEPTION_THU_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_THU_BELOW_MINUTES");

            strQry.AppendLine(",PCWH.EXCEPTION_FRI_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_FRI_BELOW_MINUTES");

            strQry.AppendLine(",PCWH.EXCEPTION_SAT_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_SAT_BELOW_MINUTES");

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",PCPH.DAILY_ROUNDING_IND");
                strQry.AppendLine(",PCPH.DAILY_ROUNDING_MINUTES");

                strQry.AppendLine(",PCPH.PAY_PERIOD_ROUNDING_IND");
                strQry.AppendLine(",PCPH.PAY_PERIOD_ROUNDING_MINUTES");
            }
            else
            {
                strQry.AppendLine(",PCWH.DAILY_ROUNDING_IND");
                strQry.AppendLine(",PCWH.DAILY_ROUNDING_MINUTES");

                strQry.AppendLine(",PCWH.PAY_PERIOD_ROUNDING_IND");
                strQry.AppendLine(",PCWH.PAY_PERIOD_ROUNDING_MINUTES");
            }

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_HISTORY PCWH");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
                strQry.AppendLine(" ON PCWH.COMPANY_NO = PCPH.COMPANY_NO");

                if (parstrTableTypeInd == "H")
                {
                    strQry.AppendLine(" AND PCWH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE ");
                }

                strQry.AppendLine(" AND PCWH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCWH.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");

                if (parstrTableTypeInd == "H")
                {
                    strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");
                }
            }
            else
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PCWH");
            }

            if (parstrReportType != "P")
            {
                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_" + strTableType + " ETH");
                }
                else
                {
                    if (parstrPayrollType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_" + strTableType + " ETH");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_" + strTableType + " ETH");
                    }
                }

                strQry.AppendLine(" ON PCWH.COMPANY_NO = ETH.COMPANY_NO");

                if (parstrTableTypeInd == "H")
                {
                    strQry.AppendLine(" AND PCWH.PAY_PERIOD_DATE = ETH.PAY_PERIOD_DATE ");
                }

                strQry.AppendLine(" AND PCWH.PAY_CATEGORY_NO = ETH.PAY_CATEGORY_NO");
      
                if (parstrPayCategoryNoIN != "")
                {
                    strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                }

                if (parstrEmployeeNoIN != "")
                {
                    strQry.AppendLine(" AND ETH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
                }

                //Week
                if (parstrReportType == "W")
                {
                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                }
                else
                {
                    if (parstrReportType == "M")
                    {
                        strQry.AppendLine(" AND YEAR(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(0, 4) + " AND MONTH(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(4));
                    }
                    else
                    {
                        if (parstrReportType == "E"
                        || parstrReportType == "H")
                        {
                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE = '" + parstrFromDate + "'");
                        }
                        else
                        {
                            if (parstrReportType == "G")
                            {
                                strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                            }
                            else
                            {
                                if (parstrReportType == "L")
                                {
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrFromDate + "'");
                                }
                                else
                                {
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrToDate + "'");
                                }
                            }
                        }
                    }
                }
            }

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND PCPH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
                strQry.AppendLine(" ON PCWH.COMPANY_NO = PC.COMPANY_NO");
                strQry.AppendLine(" AND PCWH.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCWH.PAY_CATEGORY_TYPE = PC.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");
            }

            strQry.AppendLine(" WHERE PCWH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PCWH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

            if (parstrTableTypeInd == "C")
            {
                strQry.AppendLine(" AND PCWH.DATETIME_DELETE_RECORD IS NULL");
            }

            //Pay Period
            if (parstrReportType == "P")
            {
                if (parstrTableTypeInd == "H")
                {
                    strQry.AppendLine(" AND PCWH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                }
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCWH.PAY_CATEGORY_NO");

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");
                strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            }
            else
            {
                strQry.AppendLine(",PCWH.PAY_CATEGORY_DESC");
            }

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",PCWH.WEEK_DATE_FROM");
                strQry.AppendLine(",PCWH.WEEK_DATE");
            }

            strQry.AppendLine(",PCWH.EXCEPTION_SUN_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_SUN_BELOW_MINUTES");

            strQry.AppendLine(",PCWH.EXCEPTION_MON_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_MON_BELOW_MINUTES");

            strQry.AppendLine(",PCWH.EXCEPTION_TUE_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_TUE_BELOW_MINUTES");

            strQry.AppendLine(",PCWH.EXCEPTION_WED_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_WED_BELOW_MINUTES");

            strQry.AppendLine(",PCWH.EXCEPTION_THU_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_THU_BELOW_MINUTES");

            strQry.AppendLine(",PCWH.EXCEPTION_FRI_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_FRI_BELOW_MINUTES");

            strQry.AppendLine(",PCWH.EXCEPTION_SAT_ABOVE_MINUTES");
            strQry.AppendLine(",PCWH.EXCEPTION_SAT_BELOW_MINUTES");

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",PCPH.DAILY_ROUNDING_IND");
                strQry.AppendLine(",PCPH.DAILY_ROUNDING_MINUTES");

                strQry.AppendLine(",PCPH.PAY_PERIOD_ROUNDING_IND");
                strQry.AppendLine(",PCPH.PAY_PERIOD_ROUNDING_MINUTES");
            }
            else
            {
                strQry.AppendLine(",PCWH.DAILY_ROUNDING_IND");
                strQry.AppendLine(",PCWH.DAILY_ROUNDING_MINUTES");

                strQry.AppendLine(",PCWH.PAY_PERIOD_ROUNDING_IND");
                strQry.AppendLine(",PCWH.PAY_PERIOD_ROUNDING_MINUTES");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PCWH.PAY_CATEGORY_NO");

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");
                strQry.AppendLine(",PCWH.WEEK_DATE_FROM");
                strQry.AppendLine(",PCWH.WEEK_DATE");
            }

            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryException", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PCBH.PAY_CATEGORY_NO");

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",PCBH.PAY_PERIOD_DATE");
            }

            strQry.AppendLine(",PCBH.WORKED_TIME_MINUTES");
            strQry.AppendLine(",PCBH.BREAK_MINUTES");

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_HISTORY PCBH");
            }
            else
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK PCBH");
            }

            if (parstrReportType != "P")
            {
                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_" + strTableType + " ETH");
                }
                else
                {
                    if (parstrPayrollType == "S")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_" + strTableType + " ETH");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_" + strTableType + " ETH");
                    }
                }

                strQry.AppendLine(" ON PCBH.COMPANY_NO = ETH.COMPANY_NO");

                if (parstrTableTypeInd == "H")
                {
                    strQry.AppendLine(" AND PCBH.PAY_PERIOD_DATE = ETH.PAY_PERIOD_DATE ");
                }

                strQry.AppendLine(" AND PCBH.PAY_CATEGORY_NO = ETH.PAY_CATEGORY_NO");

                if (parstrPayCategoryNoIN != "")
                {
                    strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                }

                if (parstrEmployeeNoIN != "")
                {
                    strQry.AppendLine(" AND ETH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
                }

                //Week
                if (parstrReportType == "W")
                {
                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                }
                else
                {
                    if (parstrReportType == "M")
                    {
                        strQry.AppendLine(" AND YEAR(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(0, 4) + " AND MONTH(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(4));
                    }
                    else
                    {
                        if (parstrReportType == "E"
                        || parstrReportType == "H")
                        {
                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE = '" + parstrFromDate + "'");
                        }
                        else
                        {
                            if (parstrReportType == "G")
                            {
                                strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                            }
                            else
                            {
                                if (parstrReportType == "L")
                                {
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrFromDate + "'");
                                }
                                else
                                {
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrToDate + "'");
                                }
                            }
                        }
                    }
                }
            }

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND PCBH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND PCBH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCBH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }
           
            strQry.AppendLine(" WHERE PCBH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PCBH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

            //Pay Period
            if (parstrReportType == "P")
            {
                if (parstrTableTypeInd == "H")
                {
                    strQry.AppendLine(" AND PCBH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                }
            }

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PCBH.PAY_CATEGORY_NO");

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",PCBH.PAY_PERIOD_DATE");
            }

            strQry.AppendLine(",PCBH.WORKED_TIME_MINUTES");
            strQry.AppendLine(",PCBH.BREAK_MINUTES");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PCBH.PAY_CATEGORY_NO");

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(",PCBH.PAY_PERIOD_DATE");
            }

            strQry.AppendLine(",PCBH.WORKED_TIME_MINUTES");
            strQry.AppendLine(",PCBH.BREAK_MINUTES");

            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryBreak", parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" SELECT ");

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(" PCPH.PAY_CATEGORY_NO");
                strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");
                strQry.AppendLine(",PHH.PUBLIC_HOLIDAY_DATE");
            }
            else
            {
                strQry.AppendLine(" PHH.PUBLIC_HOLIDAY_DATE");
            }

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY_HISTORY PHH");
            }
            else
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY PHH");
            }

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
                strQry.AppendLine(" ON PHH.COMPANY_NO = PCPH.COMPANY_NO");
                strQry.AppendLine(" AND PHH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND PCPH.PAY_PUBLIC_HOLIDAY_IND = 'Y'");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");

                if (parstrReportType != "P")
                {
                    if (parstrPayrollType == "W")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_HISTORY ETH");
                    }
                    else
                    {
                        if (parstrPayrollType == "S")
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_HISTORY ETH");
                        }
                        else
                        {
                            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_HISTORY ETH");
                        }
                    }

                    strQry.AppendLine(" ON PCPH.COMPANY_NO = ETH.COMPANY_NO");
                    strQry.AppendLine(" AND PCPH.PAY_PERIOD_DATE = ETH.PAY_PERIOD_DATE ");
                    strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = ETH.PAY_CATEGORY_NO");

                    if (parstrPayCategoryNoIN != "")
                    {
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    }

                    if (parstrEmployeeNoIN != "")
                    {
                        strQry.AppendLine(" AND ETH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
                    }

                    //Week
                    if (parstrReportType == "W")
                    {
                        strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                        strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                    }
                    else
                    {
                        if (parstrReportType == "M")
                        {
                            strQry.AppendLine(" AND YEAR(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(0, 4) + " AND MONTH(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(4));
                        }
                        else
                        {
                            if (parstrReportType == "E"
                            || parstrReportType == "H")
                            {
                                strQry.AppendLine(" AND ETH.TIMESHEET_DATE = '" + parstrFromDate + "'");
                            }
                            else
                            {
                                if (parstrReportType == "G")
                                {
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                                }
                                else
                                {
                                    if (parstrReportType == "L")
                                    {
                                        strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrFromDate + "'");
                                    }
                                    else
                                    {
                                        strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                                        strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrToDate + "'");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                if (parstrTableTypeInd == "H")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                    strQry.AppendLine(" AND PCPH.COMPANY_NO = UEPCT.COMPANY_NO");
                    strQry.AppendLine(" AND PCPH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                    strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                }
            }

            if (parstrTableTypeInd == "H")
            {
                strQry.AppendLine(" WHERE PHH.COMPANY_NO = " + parint64CompanyNo);

                //Pay Period
                if (parstrReportType == "P")
                {
                    strQry.AppendLine(" AND PHH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                }

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" PCPH.PAY_CATEGORY_NO");
                strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");
                strQry.AppendLine(",PHH.PUBLIC_HOLIDAY_DATE");

                strQry.AppendLine(" ORDER BY ");
                strQry.AppendLine(" PCPH.PAY_CATEGORY_NO");
                strQry.AppendLine(",PCPH.PAY_PERIOD_DATE");
                strQry.AppendLine(",PHH.PUBLIC_HOLIDAY_DATE");
            }

            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategoryPublicHoliday", parint64CompanyNo);

			byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
			DataSet.Dispose();
			DataSet = null;

			return bytCompress;
		}

        public byte[] Print_Report_TimeType(Int64 parint64CompanyNo, string parstrPayrollType, string parstrTableTypeInd, string parstrReportType, string parstrEmployeeNoIN, string parstrPayCategoryNoIN, string parstrFromDate, string parstrToDate, string parstrFilterType, string parstrTimeInd, string parstrTimeTableInd,string  parstrTimeInOutInd, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            string strTableType = "HISTORY";

            if (parstrTableTypeInd == "C")
            {
                strTableType = "CURRENT";
            }

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_DESC");
            strQry.AppendLine(",CL.DATE_FORMAT");
            strQry.AppendLine(",'' AS REPORT_DATETIME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
            strQry.AppendLine(" ON C.COMPANY_NO = CL.COMPANY_NO ");

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportHeader", parint64CompanyNo);

            if (DataSet.Tables["ReportHeader"].Rows.Count > 0)
            {
                DataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"] = "Printed   " + DateTime.Now.ToString(DataSet.Tables["ReportHeader"].Rows[0]["DATE_FORMAT"].ToString() + "   HH:mm");
            }

            strQry.Clear();

            if (parstrTimeTableInd == "A"
            || parstrTimeTableInd == "T")
            {
                if (parstrTimeInOutInd == "A"
                || parstrTimeInOutInd == "I")
                {
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETH.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETH.EMPLOYEE_NO");
                    strQry.AppendLine(",ETH.TIMESHEET_DATE");

                    strQry.AppendLine(",'T' AS TABLE_TYPE");

                    strQry.AppendLine(",'I' AS CLOCK_TYPE");

                    strQry.AppendLine(",ETH.TIMESHEET_TIME_IN_MINUTES AS TIME_MINUTES");

                    strQry.AppendLine(",TIME_TYPE = ");
                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN NOT ETH.TIMESHEET_TIME_IN_MINUTES IS NULL AND NOT CLOCKED_TIME_IN_MINUTES IS NULL THEN 'Clock' ");

                    strQry.AppendLine(" WHEN NOT ETH.TIMESHEET_TIME_IN_MINUTES IS NULL AND NOT UI.FIRSTNAME IS NULL THEN UI.SURNAME + ', ' + UI.FIRSTNAME ");

                    strQry.AppendLine(" WHEN NOT ETH.TIMESHEET_TIME_IN_MINUTES IS NULL AND ETH.USER_NO_TIME_IN = 0 THEN 'Administrator' ");

                    strQry.AppendLine(" WHEN NOT ETH.TIMESHEET_TIME_IN_MINUTES IS NULL THEN 'User' ");

                    strQry.AppendLine(" ELSE '' ");

                    strQry.AppendLine(" END ");

                    if (parstrPayrollType == "W")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_" + strTableType + " ETH");
                    }
                    else
                    {
                        if (parstrPayrollType == "S")
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_" + strTableType + " ETH");
                        }
                        else
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_" + strTableType + " ETH");
                        }
                    }

                    if (parstrTableTypeInd == "H")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
                        strQry.AppendLine(" ON ETH.COMPANY_NO = PCPH.COMPANY_NO");
                        strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE");
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                        strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PCPH");
                        strQry.AppendLine(" ON ETH.COMPANY_NO = PCPH.COMPANY_NO");
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                        strQry.AppendLine(" AND PCPH.DATETIME_DELETE_RECORD IS NULL");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayroll.dbo.USER_ID UI");
                    strQry.AppendLine(" ON ETH.USER_NO_TIME_IN = UI.USER_NO");

                    //2013-08-30
                    if (parstrCurrentUserAccess == "U")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                        strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                        strQry.AppendLine(" AND ETH.COMPANY_NO = UEPCT.COMPANY_NO");
                        strQry.AppendLine(" AND ETH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                    }

                    strQry.AppendLine(" WHERE ETH.COMPANY_NO = " + parint64CompanyNo);

                    strQry.AppendLine(" AND NOT ETH.TIMESHEET_TIME_IN_MINUTES IS NULL ");

                    if (parstrPayCategoryNoIN != "")
                    {
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    }

                    if (parstrEmployeeNoIN != "")
                    {
                        strQry.AppendLine(" AND ETH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
                    }

                    //Pay Period
                    if (parstrReportType == "P")
                    {
                        if (parstrTableTypeInd == "H")
                        {
                            strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                        }
                    }
                    else
                    {
                        //Week
                        if (parstrReportType == "W")
                        {
                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                        }
                        else
                        {
                            if (parstrReportType == "M")
                            {
                                strQry.AppendLine(" AND YEAR(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(0, 4) + " AND MONTH(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(4));
                            }
                            else
                            {
                                if (parstrReportType == "E"
                                || parstrReportType == "H")
                                {
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE = '" + parstrFromDate + "'");
                                }
                                else
                                {
                                    if (parstrReportType == "G")
                                    {
                                        strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                                    }
                                    else
                                    {
                                        if (parstrReportType == "L")
                                        {
                                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrFromDate + "'");
                                        }
                                        else
                                        {
                                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrToDate + "'");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (parstrTimeInd == "A")
                    {
                    }
                    else
                    {
                        if (parstrTimeInd == "C")
                        {
                            strQry.AppendLine(" AND NOT CLOCKED_TIME_IN_MINUTES IS NULL ");
                        }
                        else
                        {
                            strQry.AppendLine(" AND CLOCKED_TIME_IN_MINUTES IS NULL ");
                        }
                    }
                }

                if (parstrTimeInOutInd == "A")
                {
                    strQry.AppendLine(" UNION ");
                }

                if (parstrTimeInOutInd == "A"
                || parstrTimeInOutInd == "O")
                {
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETH.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETH.EMPLOYEE_NO");
                    strQry.AppendLine(",ETH.TIMESHEET_DATE");

                    strQry.AppendLine(",'T' AS TABLE_TYPE");

                    strQry.AppendLine(",'O' AS CLOCK_TYPE");

                    strQry.AppendLine(",ETH.TIMESHEET_TIME_OUT_MINUTES AS TIME_MINUTES");

                    strQry.AppendLine(",TIME_TYPE = ");
                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN NOT ETH.TIMESHEET_TIME_OUT_MINUTES IS NULL AND NOT CLOCKED_TIME_OUT_MINUTES IS NULL THEN 'Clock' ");

                    strQry.AppendLine(" WHEN NOT ETH.TIMESHEET_TIME_OUT_MINUTES IS NULL AND NOT UI.FIRSTNAME IS NULL THEN UI.SURNAME + ', ' + UI.FIRSTNAME ");

                    strQry.AppendLine(" WHEN NOT ETH.TIMESHEET_TIME_OUT_MINUTES IS NULL AND ETH.USER_NO_TIME_OUT = 0 THEN 'Administrator' ");

                    strQry.AppendLine(" WHEN NOT ETH.TIMESHEET_TIME_OUT_MINUTES IS NULL THEN 'User' ");

                    strQry.AppendLine(" ELSE '' ");

                    strQry.AppendLine(" END ");

                    if (parstrPayrollType == "W")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_" + strTableType + " ETH");
                    }
                    else
                    {
                        if (parstrPayrollType == "S")
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_" + strTableType + " ETH");
                        }
                        else
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_" + strTableType + " ETH");
                        }
                    }

                    if (parstrTableTypeInd == "H")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
                        strQry.AppendLine(" ON ETH.COMPANY_NO = PCPH.COMPANY_NO");
                        strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE");
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                        strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PCPH");
                        strQry.AppendLine(" ON ETH.COMPANY_NO = PCPH.COMPANY_NO");
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                        strQry.AppendLine(" AND PCPH.DATETIME_DELETE_RECORD IS NULL");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayroll.dbo.USER_ID UI");
                    strQry.AppendLine(" ON ETH.USER_NO_TIME_OUT = UI.USER_NO");

                    //2013-08-30
                    if (parstrCurrentUserAccess == "U")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                        strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                        strQry.AppendLine(" AND ETH.COMPANY_NO = UEPCT.COMPANY_NO");
                        strQry.AppendLine(" AND ETH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                    }

                    strQry.AppendLine(" WHERE ETH.COMPANY_NO = " + parint64CompanyNo);

                    //2017-07-29
                    strQry.AppendLine(" AND NOT ETH.TIMESHEET_TIME_OUT_MINUTES IS NULL ");

                    if (parstrPayCategoryNoIN != "")
                    {
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    }

                    if (parstrEmployeeNoIN != "")
                    {
                        strQry.AppendLine(" AND ETH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
                    }

                    //Pay Period
                    if (parstrReportType == "P")
                    {
                        if (parstrTableTypeInd == "H")
                        {
                            strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                        }
                    }
                    else
                    {
                        //Week
                        if (parstrReportType == "W")
                        {
                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                        }
                        else
                        {
                            if (parstrReportType == "M")
                            {
                                strQry.AppendLine(" AND YEAR(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(0, 4) + " AND MONTH(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(4));
                            }
                            else
                            {
                                if (parstrReportType == "E"
                                || parstrReportType == "H")
                                {
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE = '" + parstrFromDate + "'");
                                }
                                else
                                {
                                    if (parstrReportType == "G")
                                    {
                                        strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                                    }
                                    else
                                    {
                                        if (parstrReportType == "L")
                                        {
                                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrFromDate + "'");
                                        }
                                        else
                                        {
                                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrToDate + "'");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (parstrTimeInd == "A")
                    {
                    }
                    else
                    {
                        if (parstrTimeInd == "C")
                        {
                            strQry.AppendLine(" AND NOT CLOCKED_TIME_OUT_MINUTES IS NULL ");
                        }
                        else
                        {
                            strQry.AppendLine(" AND CLOCKED_TIME_OUT_MINUTES IS NULL ");
                        }
                    }
                }
            }

            if (parstrTimeTableInd == "A")
            {
                strQry.AppendLine(" UNION ");
            }

            if (parstrTimeTableInd == "A"
            || parstrTimeTableInd == "B")
            {
                if (parstrTimeInOutInd == "A"
                || parstrTimeInOutInd == "I")
                {
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETH.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETH.EMPLOYEE_NO");
                    strQry.AppendLine(",ETH.BREAK_DATE AS TIMESHEET_DATE");

                    strQry.AppendLine(",'B' AS TABLE_TYPE");
                    strQry.AppendLine(",'I' AS CLOCK_TYPE");

                    strQry.AppendLine(",ETH.BREAK_TIME_IN_MINUTES AS TIME_MINUTES");

                    strQry.AppendLine(",TIME_TYPE = ");
                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN NOT ETH.BREAK_TIME_IN_MINUTES IS NULL AND NOT CLOCKED_TIME_IN_MINUTES IS NULL THEN 'Clock' ");

                    strQry.AppendLine(" WHEN NOT ETH.BREAK_TIME_IN_MINUTES IS NULL AND NOT UI.FIRSTNAME IS NULL THEN UI.SURNAME + ', ' + UI.FIRSTNAME ");

                    strQry.AppendLine(" WHEN NOT ETH.BREAK_TIME_IN_MINUTES IS NULL AND ETH.USER_NO_TIME_IN = 0 THEN 'Administrator' ");

                    strQry.AppendLine(" WHEN NOT ETH.BREAK_TIME_IN_MINUTES IS NULL THEN 'User' ");

                    strQry.AppendLine(" ELSE '' ");

                    strQry.AppendLine(" END ");

                    if (parstrPayrollType == "W")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_" + strTableType + " ETH");
                    }
                    else
                    {
                        if (parstrPayrollType == "S")
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_" + strTableType + " ETH");
                        }
                        else
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_" + strTableType + " ETH");
                        }
                    }

                    if (parstrTableTypeInd == "H")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
                        strQry.AppendLine(" ON ETH.COMPANY_NO = PCPH.COMPANY_NO");
                        strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE");
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                        strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PCPH");
                        strQry.AppendLine(" ON ETH.COMPANY_NO = PCPH.COMPANY_NO");
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                        strQry.AppendLine(" AND PCPH.DATETIME_DELETE_RECORD IS NULL");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayroll.dbo.USER_ID UI");
                    strQry.AppendLine(" ON ETH.USER_NO_TIME_IN = UI.USER_NO");

                    //2013-08-30
                    if (parstrCurrentUserAccess == "U")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                        strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                        strQry.AppendLine(" AND ETH.COMPANY_NO = UEPCT.COMPANY_NO");
                        strQry.AppendLine(" AND ETH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                    }

                    strQry.AppendLine(" WHERE ETH.COMPANY_NO = " + parint64CompanyNo);

                    strQry.AppendLine(" AND NOT ETH.BREAK_TIME_IN_MINUTES IS NULL ");

                    if (parstrPayCategoryNoIN != "")
                    {
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    }

                    if (parstrEmployeeNoIN != "")
                    {
                        strQry.AppendLine(" AND ETH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
                    }

                    //Pay Period
                    if (parstrReportType == "P")
                    {
                        if (parstrTableTypeInd == "H")
                        {
                            strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                        }
                    }
                    else
                    {
                        //Week
                        if (parstrReportType == "W")
                        {
                            strQry.AppendLine(" AND ETH.BREAK_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                            strQry.AppendLine(" AND ETH.BREAK_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                        }
                        else
                        {
                            if (parstrReportType == "M")
                            {
                                strQry.AppendLine(" AND YEAR(ETH.BREAK_DATE) = " + parstrFromDate.Substring(0, 4) + " AND MONTH(ETH.BREAK_DATE) = " + parstrFromDate.Substring(4));
                            }
                            else
                            {
                                if (parstrReportType == "E"
                                || parstrReportType == "H")
                                {
                                    strQry.AppendLine(" AND ETH.BREAK_DATE = '" + parstrFromDate + "'");
                                }
                                else
                                {
                                    if (parstrReportType == "G")
                                    {
                                        strQry.AppendLine(" AND ETH.BREAK_DATE >= '" + parstrFromDate + "'");
                                    }
                                    else
                                    {
                                        if (parstrReportType == "L")
                                        {
                                            strQry.AppendLine(" AND ETH.BREAK_DATE <= '" + parstrFromDate + "'");
                                        }
                                        else
                                        {
                                            strQry.AppendLine(" AND ETH.BREAK_DATE >= '" + parstrFromDate + "'");
                                            strQry.AppendLine(" AND ETH.BREAK_DATE <= '" + parstrToDate + "'");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (parstrTimeInd == "A")
                    {
                    }
                    else
                    {
                        if (parstrTimeInd == "C")
                        {
                            strQry.AppendLine(" AND NOT CLOCKED_TIME_IN_MINUTES IS NULL ");
                        }
                        else
                        {
                            strQry.AppendLine(" AND CLOCKED_TIME_IN_MINUTES IS NULL ");
                        }
                    }
                }

                if (parstrTimeInOutInd == "A")
                {
                    strQry.AppendLine(" UNION ");
                }

                if (parstrTimeInOutInd == "A"
                || parstrTimeInOutInd == "O")
                {
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ETH.PAY_CATEGORY_NO");
                    strQry.AppendLine(",ETH.EMPLOYEE_NO");
                    strQry.AppendLine(",ETH.BREAK_DATE AS TIMESHEET_DATE");

                    strQry.AppendLine(",'B' AS TABLE_TYPE");
                    strQry.AppendLine(",'O' AS CLOCK_TYPE");

                    strQry.AppendLine(",ETH.BREAK_TIME_OUT_MINUTES AS TIME_MINUTES");

                    strQry.AppendLine(",TIME_TYPE = ");
                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN NOT ETH.BREAK_TIME_OUT_MINUTES IS NULL AND NOT CLOCKED_TIME_OUT_MINUTES IS NULL THEN 'Clock' ");

                    strQry.AppendLine(" WHEN NOT ETH.BREAK_TIME_OUT_MINUTES IS NULL AND NOT UI.FIRSTNAME IS NULL THEN UI.SURNAME + ', ' + UI.FIRSTNAME ");

                    strQry.AppendLine(" WHEN NOT ETH.BREAK_TIME_OUT_MINUTES IS NULL AND ETH.USER_NO_TIME_OUT = 0 THEN 'Administrator' ");

                    strQry.AppendLine(" WHEN NOT ETH.BREAK_TIME_OUT_MINUTES IS NULL THEN 'User' ");

                    strQry.AppendLine(" ELSE '' ");

                    strQry.AppendLine(" END ");

                    if (parstrPayrollType == "W")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_" + strTableType + " ETH");
                    }
                    else
                    {
                        if (parstrPayrollType == "S")
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_" + strTableType + " ETH");
                        }
                        else
                        {
                            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_" + strTableType + " ETH");
                        }
                    }

                    if (parstrTableTypeInd == "H")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
                        strQry.AppendLine(" ON ETH.COMPANY_NO = PCPH.COMPANY_NO");
                        strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE");
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                        strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");
                    }
                    else
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PCPH");
                        strQry.AppendLine(" ON ETH.COMPANY_NO = PCPH.COMPANY_NO");
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

                        strQry.AppendLine(" AND PCPH.DATETIME_DELETE_RECORD IS NULL");
                    }

                    strQry.AppendLine(" LEFT JOIN InteractPayroll.dbo.USER_ID UI");
                    strQry.AppendLine(" ON ETH.USER_NO_TIME_OUT = UI.USER_NO");

                    //2013-08-30
                    if (parstrCurrentUserAccess == "U")
                    {
                        strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                        strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                        strQry.AppendLine(" AND ETH.COMPANY_NO = UEPCT.COMPANY_NO");
                        strQry.AppendLine(" AND ETH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                        strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
                    }

                    strQry.AppendLine(" WHERE ETH.COMPANY_NO = " + parint64CompanyNo);

                    strQry.AppendLine(" AND NOT ETH.BREAK_TIME_OUT_MINUTES IS NULL ");

                    if (parstrPayCategoryNoIN != "")
                    {
                        strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    }

                    if (parstrEmployeeNoIN != "")
                    {
                        strQry.AppendLine(" AND ETH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
                    }

                    //Pay Period
                    if (parstrReportType == "P")
                    {
                        if (parstrTableTypeInd == "H")
                        {
                            strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                        }
                    }
                    else
                    {
                        //Week
                        if (parstrReportType == "W")
                        {
                            strQry.AppendLine(" AND ETH.BREAK_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                            strQry.AppendLine(" AND ETH.BREAK_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                        }
                        else
                        {
                            if (parstrReportType == "M")
                            {
                                strQry.AppendLine(" AND YEAR(ETH.BREAK_DATE) = " + parstrFromDate.Substring(0, 4) + " AND MONTH(ETH.BREAK_DATE) = " + parstrFromDate.Substring(4));
                            }
                            else
                            {
                                if (parstrReportType == "E"
                                || parstrReportType == "H")
                                {
                                    strQry.AppendLine(" AND ETH.BREAK_DATE = '" + parstrFromDate + "'");
                                }
                                else
                                {
                                    if (parstrReportType == "G")
                                    {
                                        strQry.AppendLine(" AND ETH.BREAK_DATE >= '" + parstrFromDate + "'");
                                    }
                                    else
                                    {
                                        if (parstrReportType == "L")
                                        {
                                            strQry.AppendLine(" AND ETH.BREAK_DATE <= '" + parstrFromDate + "'");
                                        }
                                        else
                                        {
                                            strQry.AppendLine(" AND ETH.BREAK_DATE >= '" + parstrFromDate + "'");
                                            strQry.AppendLine(" AND ETH.BREAK_DATE <= '" + parstrToDate + "'");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (parstrTimeInd == "A")
                    {
                    }
                    else
                    {
                        if (parstrTimeInd == "C")
                        {
                            strQry.AppendLine(" AND NOT CLOCKED_TIME_OUT_MINUTES IS NULL ");
                        }
                        else
                        {
                            strQry.AppendLine(" AND CLOCKED_TIME_OUT_MINUTES IS NULL ");
                        }
                    }
                }
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",3");
            strQry.AppendLine(",6");
            //Table Type
            strQry.AppendLine(",4 DESC");

            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportTimeSheet", parint64CompanyNo);

            string strPayCategoryNos = "";

            //Get PAY_CATEGORY_NO in Report
            var groupQuery = from table in DataSet.Tables["ReportTimeSheet"].AsEnumerable()
                             group table by new
                             {
                                 PayCategoryNo = table.Field<Int16>("PAY_CATEGORY_NO")

                             } into grp
                             select new
                             {
                                 PayCategoryNo = grp.Key.PayCategoryNo
                             };

            foreach (var row in groupQuery)
            {
                if (strPayCategoryNos == "")
                {
                    strPayCategoryNos = row.PayCategoryNo.ToString();
                }
                else
                {

                    strPayCategoryNos += "," + row.PayCategoryNo.ToString();
                }
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

            //Pay Period
            if (parstrReportType == "P"
            && parstrTableTypeInd == "H")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_HISTORY PCWH");
                strQry.AppendLine(" ON PC.COMPANY_NO = PCWH.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCWH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCWH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND PCWH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
            }

            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);

            if (strPayCategoryNos != "")
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN (" + strPayCategoryNos + ")");
            }
            else
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN (-1)");
            }

            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategorySelected", parint64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
        
        public byte[] Print_Report_Simple_Errors(Int64 parint64CompanyNo, string parstrPayrollType, string parstrTableType, string parstrReportType, string parstrEmployeeNoIN, string parstrPayCategoryNoIN, string parstrFromDate, string parstrToDate, string parstrFilterType, string parstrTimeRule, int parintTime, string parstrPrintOrder, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_DESC");
            strQry.AppendLine(",CL.DATE_FORMAT");
            strQry.AppendLine(",'' AS REPORT_DATETIME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
            strQry.AppendLine(" ON C.COMPANY_NO = CL.COMPANY_NO ");

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportHeader", parint64CompanyNo);

            if (DataSet.Tables["ReportHeader"].Rows.Count > 0)
            {
                DataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"] = "Printed   " + DateTime.Now.ToString(DataSet.Tables["ReportHeader"].Rows[0]["DATE_FORMAT"].ToString() + "   HH:mm");
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",ETC.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(",NULL AS  BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",NULL AS BREAK_TIME_OUT_MINUTES");

            strQry.AppendLine(",TIMESHEET_TIME_IN_TYPE = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT ETC.TIMESHEET_TIME_IN_MINUTES IS NULL AND NOT CLOCKED_TIME_IN_MINUTES IS NULL THEN 'C' ");

            strQry.AppendLine(" WHEN NOT ETC.TIMESHEET_TIME_IN_MINUTES IS NULL THEN 'U' ");

            strQry.AppendLine(" ELSE '' ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",TIMESHEET_TIME_OUT_TYPE = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT ETC.TIMESHEET_TIME_OUT_MINUTES IS NULL AND NOT CLOCKED_TIME_OUT_MINUTES IS NULL THEN 'C' ");

            strQry.AppendLine(" WHEN NOT ETC.TIMESHEET_TIME_OUT_MINUTES IS NULL THEN 'U' ");

            strQry.AppendLine(" ELSE '' ");

            strQry.AppendLine(" END ");
       
            strQry.AppendLine(",'' AS  BREAK_TIME_IN_TYPE");
            strQry.AppendLine(",'' AS BREAK_TIME_OUT_TYPE");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                }
            }

            strQry.AppendLine(" INNER JOIN ");

            //Get SQL For Errors in Timesheets
            strQry.Append(Get_Current_Error_SQL(parint64CompanyNo, parstrPayrollType));

            strQry.AppendLine(" ON ETC.EMPLOYEE_NO = ERRORS_TABLE.EMPLOYEE_NO  ");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = ERRORS_TABLE.TIMESHEET_DATE");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ERRORS_TABLE.PAY_CATEGORY_NO ");
            
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON ETC.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = UEPCT.PAY_CATEGORY_TYPE");
            }

            strQry.AppendLine(" WHERE ETC.COMPANY_NO = " + parint64CompanyNo);

            if (parstrPayCategoryNoIN != "")
            {
                strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
            }

            if (parstrEmployeeNoIN != "")
            {
                strQry.AppendLine(" AND ETC.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
            }

            if (parstrReportType == "P")
            {
                //All for CURRENT
            }
            else
            {
                //Week
                if (parstrReportType == "W")
                {
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                }
                else
                {
                    if (parstrReportType == "M")
                    {
                        strQry.AppendLine(" AND YEAR(ETC.TIMESHEET_DATE) = " + parstrFromDate.Substring(0, 4) + " AND MONTH(ETC.TIMESHEET_DATE) = " + parstrFromDate.Substring(4));
                    }
                    else
                    {
                        if (parstrReportType == "E"
                        || parstrReportType == "H")
                        {
                            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = '" + parstrFromDate + "'");
                        }
                        else
                        {
                            if (parstrReportType == "G")
                            {
                                strQry.AppendLine(" AND ETC.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                            }
                            else
                            {
                                if (parstrReportType == "L")
                                {
                                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + parstrFromDate + "'");
                                }
                                else
                                {
                                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                                    strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= '" + parstrToDate + "'");
                                }
                            }
                        }
                    }
                }
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" 1");
            strQry.AppendLine(",2");
            strQry.AppendLine(",3");
            strQry.AppendLine(",4");
            strQry.AppendLine(",5");
            strQry.AppendLine(",6");
            strQry.AppendLine(",7");

            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportTimeSheet", parint64CompanyNo);
            
            string strPayCategoryNos = "";

            //Get PAY_CATEGORY_NO in Report
            var groupQuery = from table in DataSet.Tables["ReportTimeSheet"].AsEnumerable()
                             group table by new
                             {
                                 PayCategoryNo = table.Field<Int16>("PAY_CATEGORY_NO")

                             } into grp
                             select new
                             {
                                 PayCategoryNo = grp.Key.PayCategoryNo
                             };

            foreach (var row in groupQuery)
            {
                if (strPayCategoryNos == "")
                {
                    strPayCategoryNos = row.PayCategoryNo.ToString();
                }
                else
                {

                    strPayCategoryNos += "," + row.PayCategoryNo.ToString();
                }
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            //Pay Period
            if (parstrReportType == "P"
            && parstrTableType == "H")
            {
                strQry.AppendLine(",PCWH.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_SUN_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_MON_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_TUE_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_WED_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_THU_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_FRI_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_SAT_BELOW_MINUTES");
            }
            else
            {
                strQry.AppendLine(",PC.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_SUN_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_MON_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_TUE_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_WED_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_THU_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_FRI_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_SAT_BELOW_MINUTES");
            }

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

            //Pay Period
            if (parstrReportType == "P"
            && parstrTableType == "H")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_HISTORY PCWH");
                strQry.AppendLine(" ON PC.COMPANY_NO = PCWH.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCWH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCWH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND PCWH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
            }

            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);

            if (strPayCategoryNos != "")
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN (" + strPayCategoryNos + ")");
            }
            else
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN (-1)");
            }

            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategorySelected", parint64CompanyNo);
            
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");

            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");

            strQry.AppendLine(",BREAK_TIME_IN_TYPE = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT EBC.BREAK_TIME_IN_MINUTES IS NULL AND NOT CLOCKED_TIME_IN_MINUTES IS NULL THEN 'C' ");

            strQry.AppendLine(" WHEN NOT EBC.BREAK_TIME_IN_MINUTES IS NULL THEN 'U' ");

            strQry.AppendLine(" ELSE '' ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",BREAK_TIME_OUT_TYPE = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT EBC.BREAK_TIME_OUT_MINUTES IS NULL AND NOT CLOCKED_TIME_OUT_MINUTES IS NULL THEN 'C' ");

            strQry.AppendLine(" WHEN NOT EBC.BREAK_TIME_OUT_MINUTES IS NULL THEN 'U' ");

            strQry.AppendLine(" ELSE '' ");

            strQry.AppendLine(" END ");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT EBC");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC");
                }
            }

            strQry.AppendLine(" INNER JOIN ");

            //Get SQL For Errors in Timesheets
            strQry.Append(Get_Current_Error_SQL(parint64CompanyNo, parstrPayrollType));

            strQry.AppendLine(" ON EBC.EMPLOYEE_NO = ERRORS_TABLE.EMPLOYEE_NO  ");
            strQry.AppendLine(" AND EBC.BREAK_DATE = ERRORS_TABLE.TIMESHEET_DATE");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = ERRORS_TABLE.PAY_CATEGORY_NO ");

            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND EBC.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EBC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            }

            strQry.AppendLine(" WHERE EBC.COMPANY_NO = " + parint64CompanyNo);

            if (parstrPayCategoryNoIN != "")
            {
                strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
            }

            if (parstrEmployeeNoIN != "")
            {
                strQry.AppendLine(" AND EBC.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
            }

            if (parstrReportType == "P")
            {
                //All for CURRENT
            }
            else
            {
                //Week
                if (parstrReportType == "W")
                {
                    strQry.AppendLine(" AND EBC.BREAK_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                    strQry.AppendLine(" AND EBC.BREAK_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                }
                else
                {
                    if (parstrReportType == "M")
                    {
                        strQry.AppendLine(" AND YEAR(EBC.BREAK_DATE) = " + parstrFromDate.Substring(0, 4) + " AND MONTH(EBC.BREAK_DATE) = " + parstrFromDate.Substring(4));
                    }
                    else
                    {
                        if (parstrReportType == "E"
                        || parstrReportType == "H")
                        {
                            strQry.AppendLine(" AND EBC.BREAK_DATE = '" + parstrFromDate + "'");
                        }
                        else
                        {
                            if (parstrReportType == "G")
                            {
                                strQry.AppendLine(" AND EBC.BREAK_DATE >= '" + parstrFromDate + "'");
                            }
                            else
                            {
                                if (parstrReportType == "L")
                                {
                                    strQry.AppendLine(" AND EBC.BREAK_DATE <= '" + parstrFromDate + "'");
                                }
                                else
                                {
                                    strQry.AppendLine(" AND EBC.BREAK_DATE >= '" + parstrFromDate + "'");
                                    strQry.AppendLine(" AND EBC.BREAK_DATE <= '" + parstrToDate + "'");
                                }
                            }
                        }
                    }
                }
            }
           
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");

            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Break", parint64CompanyNo);
                      
            DataView pvtReportTimeSheetDataView = new DataView();

            //Merge Timesheets and Breaks
            for (int intRow = 0; intRow < DataSet.Tables["Break"].Rows.Count; intRow++)
            {
                pvtReportTimeSheetDataView = null;
                pvtReportTimeSheetDataView = new DataView(DataSet.Tables["ReportTimeSheet"],
                    "PAY_CATEGORY_NO = " + DataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString()
                     + " AND EMPLOYEE_NO = " + DataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO"].ToString()
                     + " AND TIMESHEET_DATE = '" + Convert.ToDateTime(DataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"]).ToString("yyyy-MM-dd") + "'"
                     + " AND BREAK_TIME_IN_MINUTES IS NULL AND BREAK_TIME_OUT_MINUTES IS NULL ",
                    "EMPLOYEE_NO",
                    DataViewRowState.CurrentRows);

                if (pvtReportTimeSheetDataView.Count > 0)
                {
                    pvtReportTimeSheetDataView[0]["BREAK_TIME_IN_TYPE"] = DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_TYPE"].ToString();
                    pvtReportTimeSheetDataView[0]["BREAK_TIME_OUT_TYPE"] = DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_TYPE"].ToString();

                    if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"] != System.DBNull.Value)
                    {
                        pvtReportTimeSheetDataView[0]["BREAK_TIME_IN_MINUTES"] = Convert.ToInt16(DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"]);

                        pvtReportTimeSheetDataView.RowFilter = pvtReportTimeSheetDataView.RowFilter.Replace("AND BREAK_TIME_IN_MINUTES IS NULL ","");
                    }

                    if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"] != System.DBNull.Value)
                    {
                        pvtReportTimeSheetDataView[0]["BREAK_TIME_OUT_MINUTES"] = Convert.ToInt16(DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"]);
                    }
                }
                else
                {
                    DataRow drDataRow = DataSet.Tables["ReportTimeSheet"].NewRow();

                    drDataRow["PAY_CATEGORY_NO"] = DataSet.Tables["Break"].Rows[intRow]["PAY_CATEGORY_NO"].ToString();

                    drDataRow["EMPLOYEE_NO"] = DataSet.Tables["Break"].Rows[intRow]["EMPLOYEE_NO"].ToString();

                    drDataRow["TIMESHEET_DATE"] = Convert.ToDateTime(DataSet.Tables["Break"].Rows[intRow]["BREAK_DATE"].ToString());

                    if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"] != System.DBNull.Value)
                    {
                        drDataRow["BREAK_TIME_IN_MINUTES"] = Convert.ToInt16(DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_MINUTES"]);
                    }

                    if (DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"] != System.DBNull.Value)
                    {
                        drDataRow["BREAK_TIME_OUT_MINUTES"] = Convert.ToInt16(DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_MINUTES"]);
                    }

                    drDataRow["BREAK_TIME_IN_TYPE"] = DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_IN_TYPE"].ToString();
                    drDataRow["BREAK_TIME_OUT_TYPE"] = DataSet.Tables["Break"].Rows[intRow]["BREAK_TIME_OUT_TYPE"].ToString();

                    DataSet.Tables["ReportTimeSheet"].Rows.Add(drDataRow);
                }
            }

            strQry.Clear();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public StringBuilder Get_Current_Error_SQL(Int64 parint64CompanyNo, string parstrPayrollType)
        {
            StringBuilder strQry = new StringBuilder();
            string strCurrenDate = DateTime.Now.ToString("yyyy-MM-dd");

            //TEMP
            strQry.Clear();

            string RunFieldName = "WAGE_RUN_IND";

            if (parstrPayrollType == "T")
            {
                RunFieldName = "TIME_ATTENDANCE_RUN_IND";
            }

            strQry.Clear();

            strQry.AppendLine("(SELECT ");

            strQry.AppendLine(" UNION_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",UNION_TABLE.TIMESHEET_DATE ");
            strQry.AppendLine(",UNION_TABLE.PAY_CATEGORY_NO ");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(SELECT ");
           
            strQry.AppendLine(" E.EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_SUMMARY_TABLE.DAY_DATE AS TIMESHEET_DATE");

            strQry.AppendLine(",BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO");
           
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");

            strQry.AppendLine(" INNER JOIN  ");

            strQry.AppendLine("(");

            strQry.AppendLine("");
            strQry.AppendLine("--2Start Break UNION 1");
            strQry.AppendLine("");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_TABLE.DAY_DATE");

            //ELR - 2015-05-30
            strQry.AppendLine(",INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN C." + RunFieldName + " = 'Y' AND BREAK_TABLE.DAY_DATE > BREAK_TABLE.EMPLOYEE_LAST_RUNDATE ");
            strQry.AppendLine(" THEN '' ");

            strQry.AppendLine(" ELSE 'X' ");

            strQry.AppendLine(" END ");
            
            strQry.AppendLine(" FROM ");

            //Removes Duplicates where BREAK_SEQ is Not Used in Join
            strQry.AppendLine("(");

            strQry.AppendLine("");
            strQry.AppendLine("--1Start Break UNION 1");
            strQry.AppendLine("");

            strQry.AppendLine(" SELECT DISTINCT");

            strQry.AppendLine(" EBC.COMPANY_NO");
            strQry.AppendLine(",'" + parstrPayrollType + "' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO ");
            strQry.AppendLine(",EBC.BREAK_DATE AS DAY_DATE");

            strQry.AppendLine(",EBC.BREAK_SEQ");

            //Errol 2015-07-01
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");

            strQry.AppendLine(" THEN DATEADD(DD,-40,'" + strCurrenDate + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN ");

            strQry.AppendLine("(SELECT ");

            //ELR - 2014-08-23 Sort Records By BREAK_TIME_IN_MINUTES,BREAK_TIME_OUT_MINUTES
            strQry.AppendLine(" ROW_NUMBER() OVER ");

            strQry.AppendLine(" (ORDER BY ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_DATE");
            strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES) AS SORTED_REC");

            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_DATE");
            strQry.AppendLine(",BREAK_SEQ");
            strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT) AS EBC");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT) AS EBC");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT) AS EBC");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine(" AND EBC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO ");

            //Errol - 2015-02-12
            strQry.AppendLine(" AND ((EBC.BREAK_DATE > E.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
            strQry.AppendLine(" OR (EBC.BREAK_DATE >= ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
            strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

            //Set Extra Days to 15
            strQry.AppendLine(" AND EBC.BREAK_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");
           

            strQry.AppendLine(" INNER JOIN ");

            strQry.AppendLine("(SELECT ");

            //ELR - 2014-08-23 Sort Records By BREAK_TIME_IN_MINUTES,BREAK_TIME_OUT_MINUTES
            strQry.AppendLine(" ROW_NUMBER() OVER ");

            strQry.AppendLine(" (ORDER BY ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_DATE");
            strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES) AS SORTED_REC");

            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_DATE");
            strQry.AppendLine(",BREAK_SEQ");
            strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT) AS EBC2");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT) AS EBC2");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT) AS EBC2");
                }
            }

            strQry.AppendLine(" ON EBC.COMPANY_NO = EBC2.COMPANY_NO");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EBC2.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EBC2.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EBC.BREAK_DATE = EBC2.BREAK_DATE");

            //if (parstrCurrentUserAccessInd == "U")
            //{
            //    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

            //    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
            //    strQry.AppendLine(" AND EBC.COMPANY_NO = UEPCT.COMPANY_NO ");
            //    strQry.AppendLine(" AND EBC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
            //    strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
            //    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            //}

            //Errol 2012-09-20 Fix Change of PAY_CATEGORY (Orphan Records)
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

            strQry.AppendLine(" ON EBC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");

            strQry.AppendLine(" ON EBC.COMPANY_NO = PC.COMPANY_NO ");

            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
            //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

            strQry.AppendLine("");
            strQry.AppendLine("--1End Break UNION 1");
            strQry.AppendLine("");

            strQry.AppendLine(" ) AS BREAK_TABLE ");

            //Errol 2015-06-06
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C ");

            strQry.AppendLine(" ON BREAK_TABLE.COMPANY_NO = C.COMPANY_NO ");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_TABLE.DAY_DATE");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(",C." + RunFieldName + "");

            strQry.AppendLine("");
            strQry.AppendLine("--2End Break UNION 1");
            strQry.AppendLine("");

            strQry.AppendLine(" ) AS BREAK_SUMMARY_TABLE");

            strQry.AppendLine(" ON E.COMPANY_NO = BREAK_SUMMARY_TABLE.COMPANY_NO");

            strQry.AppendLine(" AND E.EMPLOYEE_NO = BREAK_SUMMARY_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                }
            }

            strQry.AppendLine(" ON BREAK_SUMMARY_TABLE.COMPANY_NO = ETC.COMPANY_NO");

            strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO = ETC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND BREAK_SUMMARY_TABLE.DAY_DATE = ETC.TIMESHEET_DATE ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
            //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

            //No Timesheets for Day
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE IS NULL ");

            //goto Get_Current_Error_SQL_Continue;

            //strQry.Clear();


            


            //This Part is Where Timesheets Exist with or Without Break Records
            //This Part is Where Timesheets Exist with or Without Break Records
            strQry.AppendLine("");
            strQry.AppendLine(" UNION ");
            strQry.AppendLine("");

            strQry.AppendLine("");
            strQry.AppendLine("--3Start Major UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" SELECT ");
          
            strQry.AppendLine(" TEMP2_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TEMP2_TABLE.DAY_DATE AS TIMESHEET_DATE");
            strQry.AppendLine(",TEMP2_TABLE.PAY_CATEGORY_NO");
            
            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(");

            strQry.AppendLine("");
            strQry.AppendLine("--2Start Major UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TEMP1_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TEMP1_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TEMP1_TABLE.DAY_DATE");
            strQry.AppendLine(",TEMP1_TABLE.INDICATOR");
          
            strQry.AppendLine(" FROM ");

            strQry.AppendLine("( ");

            strQry.AppendLine("");
            strQry.AppendLine("--1Start Major UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TIMESHEET_TOTAL_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.DAY_DATE");

            strQry.AppendLine(",INDICATOR = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN BREAK_SUMMARY_TABLE.INDICATOR = 'X' OR TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES < BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES ");
            strQry.AppendLine(" THEN 'X'");

            strQry.AppendLine(" ELSE ISNULL(MAX(TIMESHEET_TOTAL_TABLE.INDICATOR),'')");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(");

            strQry.AppendLine("");
            strQry.AppendLine("--3Start Timesheet UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TIMESHEET_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.DAY_DATE");

            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES ");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.INDICATOR ");

            strQry.AppendLine(",ISNULL(MAX(PCB.BREAK_MINUTES),0) AS BREAK_MINUTES ");

            strQry.AppendLine(" FROM ");

            strQry.AppendLine("(");

            strQry.AppendLine("");
            strQry.AppendLine("--2Start Timesheet UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" TIMESHEET_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_TABLE.DAY_DATE");

            //ELR - 20150701
            strQry.AppendLine(",TIMESHEET_ACCUM_MINUTES = ");

            strQry.AppendLine(" SUM(CASE ");

            strQry.AppendLine(" WHEN C." + RunFieldName + " = 'Y' AND TIMESHEET_TABLE.DAY_DATE > TIMESHEET_TABLE.EMPLOYEE_LAST_RUNDATE AND TIMESHEET_TABLE.DAY_DATE <= PCPC.PAY_PERIOD_DATE ");
            strQry.AppendLine(" THEN 0 ");

            strQry.AppendLine(" ELSE TIMESHEET_ACCUM_MINUTES ");

            strQry.AppendLine(" END) ");
            //ELR - 20150701

            //ELR - 20150523
            strQry.AppendLine(",INDICATOR =");
            strQry.AppendLine(" CASE ");

            //Remove Error For NOT INCLUDED IN RUN
            strQry.AppendLine(" WHEN C." + RunFieldName + " = 'Y' AND TIMESHEET_TABLE.DAY_DATE > TIMESHEET_TABLE.EMPLOYEE_LAST_RUNDATE AND TIMESHEET_TABLE.DAY_DATE <= PCPC.PAY_PERIOD_DATE ");
            strQry.AppendLine(" THEN '' ");

            strQry.AppendLine(" ELSE MAX(ISNULL(INDICATOR,''))  ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM ");

            //Removes Duplicates where TIMESHEET_SEQ is Not Used in Join
            strQry.AppendLine("(");

            strQry.AppendLine("");
            strQry.AppendLine("--1Start Timesheet UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" SELECT DISTINCT");

            strQry.AppendLine(" ETC.COMPANY_NO");
            strQry.AppendLine(",'" + parstrPayrollType + "' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",ETC.PAY_CATEGORY_NO");
            strQry.AppendLine(",ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.TIMESHEET_DATE AS DAY_DATE");

            strQry.AppendLine(",ETC.TIMESHEET_SEQ");

            //Errol 2015-07-01
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");

            strQry.AppendLine(" THEN DATEADD(DD,-40,'" + strCurrenDate + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");
            //Errol 2015-02-12

            strQry.AppendLine(",TIMESHEET_ACCUM_MINUTES = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL THEN");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN ETC.TIMESHEET_TIME_OUT_MINUTES IS NULL OR ETC.TIMESHEET_TIME_IN_MINUTES IS NULL ");

            strQry.AppendLine(" THEN 0 ");

            strQry.AppendLine(" WHEN ETC.TIMESHEET_TIME_OUT_MINUTES > ETC.TIMESHEET_TIME_IN_MINUTES THEN ");

            strQry.AppendLine(" ETC.TIMESHEET_TIME_OUT_MINUTES - ETC.TIMESHEET_TIME_IN_MINUTES ");

            strQry.AppendLine(" ELSE 0 ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" ELSE ");

            strQry.AppendLine(" ETC.TIMESHEET_TIME_OUT_MINUTES - ETC.TIMESHEET_TIME_IN_MINUTES ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL");

            strQry.AppendLine(" THEN 'X' ");

            strQry.AppendLine(" ELSE '' ");

            strQry.AppendLine(" END ");
            //ELR - 20150509

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT ETC");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT ETC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT ETC");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
            //2013-06-15 >= Cater For Employee Take-On 
            //Errol - 2015-02-12
            strQry.AppendLine(" AND ((ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
            strQry.AppendLine(" OR (ETC.TIMESHEET_DATE >= ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
            strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

            strQry.AppendLine(" LEFT JOIN ");

            //NB DISTINCT Removes Duplicates Generated when //strQry.AppendLine(" AND ETC.TIMESHEET_SEQ = ETC2.TIMESHEET_SEQ" is Removed
            strQry.AppendLine("(SELECT DISTINCT ");
            strQry.AppendLine(" ETC.EMPLOYEE_NO ");
            strQry.AppendLine(",ETC.TIMESHEET_DATE");
            strQry.AppendLine(",ETC.TIMESHEET_SEQ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN ");

            strQry.AppendLine("(SELECT ");

            //ELR - 2014-08-23 Sort Records By BREAK_TIME_IN_MINUTES,BREAK_TIME_OUT_MINUTES
            strQry.AppendLine(" ROW_NUMBER() OVER ");
            strQry.AppendLine(" (ORDER BY ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_DATE");
            strQry.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES) AS SORTED_REC");

            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_DATE");
            strQry.AppendLine(",TIMESHEET_SEQ");
            strQry.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT) AS ETC");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT) AS ETC");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT) AS ETC");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = ETC.COMPANY_NO");
            strQry.AppendLine(" AND ETC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = ETC.EMPLOYEE_NO ");
            //2013-06-15 >= Cater For Employee Take-On 
            //Errol - 2015-02-12
            strQry.AppendLine(" AND ((ETC.TIMESHEET_DATE > E.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
            strQry.AppendLine(" OR (ETC.TIMESHEET_DATE >= ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
            strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

            //Set Extra Days to 15
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            strQry.AppendLine(" INNER JOIN ");

            strQry.AppendLine("(SELECT ");

            //ELR - 2014-08-23 Sort Records By BREAK_TIME_IN_MINUTES,BREAK_TIME_OUT_MINUTES
            strQry.AppendLine(" ROW_NUMBER() OVER ");

            strQry.AppendLine(" (ORDER BY ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_DATE");
            strQry.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES) AS SORTED_REC");

            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_DATE");
            strQry.AppendLine(",TIMESHEET_SEQ");
            strQry.AppendLine(",TIMESHEET_TIME_IN_MINUTES");
            strQry.AppendLine(",TIMESHEET_TIME_OUT_MINUTES");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_CURRENT) AS ETC2");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_CURRENT) AS ETC2");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_CURRENT) AS ETC2");
                }
            }

            strQry.AppendLine(" ON ETC.COMPANY_NO = ETC2.COMPANY_NO");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = ETC2.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = ETC2.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = ETC2.TIMESHEET_DATE");
            //strQry.AppendLine(" AND ETC.TIMESHEET_SEQ = ETC2.TIMESHEET_SEQ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
            //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

            //Errol Checked
            strQry.AppendLine(" AND ((ETC.TIMESHEET_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" OR ETC.TIMESHEET_TIME_OUT_MINUTES IS NULL)");
            //Same Row
            strQry.AppendLine(" OR (ETC.TIMESHEET_TIME_IN_MINUTES > ETC2.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.SORTED_REC <= ETC2.SORTED_REC)");

            //Different Rows
            strQry.AppendLine(" OR (ETC.TIMESHEET_TIME_IN_MINUTES < ETC2.TIMESHEET_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND ETC.SORTED_REC > ETC2.SORTED_REC))) AS TEMP");

            strQry.AppendLine(" ON ETC.EMPLOYEE_NO = TEMP.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.TIMESHEET_DATE = TEMP.TIMESHEET_DATE");
            strQry.AppendLine(" AND ETC.TIMESHEET_SEQ = TEMP.TIMESHEET_SEQ ");

            //if (parstrCurrentUserAccessInd == "U")
            //{
            //    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

            //    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
            //    strQry.AppendLine(" AND ETC.COMPANY_NO = UEPCT.COMPANY_NO ");
            //    strQry.AppendLine(" AND ETC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
            //    strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
            //    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            //}

            //Errol 2012-09-20 Fix Change of PAY_CATEGORY (Orphan Records)
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

            strQry.AppendLine(" ON ETC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON ETC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND ETC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
            //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

            strQry.AppendLine("");
            strQry.AppendLine("--1End Timesheet UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" ) AS TIMESHEET_TABLE ");

            //Errol 2015-06-06
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C ");

            strQry.AppendLine(" ON TIMESHEET_TABLE.COMPANY_NO = C.COMPANY_NO ");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
            strQry.AppendLine(" ON TIMESHEET_TABLE.COMPANY_NO = PCPC.COMPANY_NO ");
            strQry.AppendLine(" AND TIMESHEET_TABLE.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND TIMESHEET_TABLE.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P' ");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" TIMESHEET_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_TABLE.DAY_DATE");
            strQry.AppendLine(",TIMESHEET_TABLE.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(",PCPC.PAY_PERIOD_DATE");
            strQry.AppendLine(",C." + RunFieldName);

            strQry.AppendLine("");
            strQry.AppendLine("--2End Timesheet UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" ) AS TIMESHEET_SUMMARY_TABLE");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK PCB");

            strQry.AppendLine(" ON TIMESHEET_SUMMARY_TABLE.COMPANY_NO = PCB.COMPANY_NO");
            strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO = PCB.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE = PCB.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PCB.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES >= PCB.WORKED_TIME_MINUTES");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" TIMESHEET_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.DAY_DATE");

            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.TIMESHEET_ACCUM_MINUTES ");
            strQry.AppendLine(",TIMESHEET_SUMMARY_TABLE.INDICATOR ");
        
            strQry.AppendLine("");
            strQry.AppendLine("--3End Timesheet UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" ) AS TIMESHEET_TOTAL_TABLE ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C ");

            strQry.AppendLine(" ON TIMESHEET_TOTAL_TABLE.COMPANY_NO = C.COMPANY_NO ");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" LEFT JOIN  ");

            strQry.AppendLine("(");

            strQry.AppendLine("");
            strQry.AppendLine("--3Start Break UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_TABLE.DAY_DATE");

            //ELR - 20150523
            strQry.AppendLine(",BREAK_ACCUM_MINUTES = ");

            strQry.AppendLine(" SUM(CASE ");

            strQry.AppendLine(" WHEN C." + RunFieldName + " = 'Y' AND BREAK_TABLE.DAY_DATE > BREAK_TABLE.EMPLOYEE_LAST_RUNDATE AND BREAK_TABLE.DAY_DATE <= PCPC.PAY_PERIOD_DATE ");
            strQry.AppendLine(" THEN BREAK_TABLE.BREAK_ACCUM_MINUTES ");

            strQry.AppendLine(" WHEN C." + RunFieldName + " = 'Y' AND BREAK_TABLE.DAY_DATE > BREAK_TABLE.EMPLOYEE_LAST_RUNDATE AND BREAK_TABLE.DAY_DATE <= PCPC.PAY_PERIOD_DATE ");
            strQry.AppendLine(" THEN 0 ");

            strQry.AppendLine(" ELSE BREAK_TABLE.BREAK_ACCUM_MINUTES ");

            strQry.AppendLine(" END) ");

            //ELR - 20150523
            strQry.AppendLine(",INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN C." + RunFieldName + " = 'Y' AND BREAK_TABLE.DAY_DATE > BREAK_TABLE.EMPLOYEE_LAST_RUNDATE AND BREAK_TABLE.DAY_DATE <= PCPC.PAY_PERIOD_DATE ");
            strQry.AppendLine(" THEN '' ");

            strQry.AppendLine(" ELSE MAX(ISNULL(BREAK_TABLE.INDICATOR,'')) ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM ");

            //Removes Duplicates where BREAK_SEQ is Not Used in Join
            strQry.AppendLine("(");

            strQry.AppendLine("");
            strQry.AppendLine("--2Start Break UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" EBC.COMPANY_NO");
            strQry.AppendLine(",'" + parstrPayrollType + "' AS PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO ");
            strQry.AppendLine(",EBC.BREAK_DATE AS DAY_DATE");

            //ELR - 2014-04-23
            strQry.AppendLine(",EBC.BREAK_SEQ");

            //Errol 2015-07-01
            strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN E.EMPLOYEE_LAST_RUNDATE IS NULL ");

            strQry.AppendLine(" THEN DATEADD(DD,-40,'" + strCurrenDate + "')");

            strQry.AppendLine(" WHEN ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y' ");

            strQry.AppendLine(" THEN DATEADD(DD,-1,E.EMPLOYEE_LAST_RUNDATE) ");

            strQry.AppendLine(" ELSE E.EMPLOYEE_LAST_RUNDATE ");

            strQry.AppendLine(" END ");
            //Errol 2015-02-12

            strQry.AppendLine(",BREAK_ACCUM_MINUTES = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL THEN ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN EBC.BREAK_TIME_OUT_MINUTES IS NULL OR EBC.BREAK_TIME_IN_MINUTES IS NULL ");

            strQry.AppendLine(" THEN 0 ");

            strQry.AppendLine(" WHEN EBC.BREAK_TIME_OUT_MINUTES > EBC.BREAK_TIME_IN_MINUTES THEN ");

            strQry.AppendLine(" EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES ");

            strQry.AppendLine(" ELSE 0 ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" ELSE ");

            strQry.AppendLine(" EBC.BREAK_TIME_OUT_MINUTES - EBC.BREAK_TIME_IN_MINUTES ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",INDICATOR = ");

            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN NOT TEMP.EMPLOYEE_NO IS NULL");

            strQry.AppendLine(" THEN 'X' ");

            strQry.AppendLine(" ELSE '' ");

            strQry.AppendLine(" END ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT EBC");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT EBC");
                }
                else
                {
                    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT EBC");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine(" AND EBC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO ");
            //2013-06-15 >= Cater For Employee Take-On 
            //Errol - 2015-02-12
            strQry.AppendLine(" AND ((EBC.BREAK_DATE > E.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
            strQry.AppendLine(" OR (EBC.BREAK_DATE >= ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
            strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

            strQry.AppendLine(" AND EBC.BREAK_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            strQry.AppendLine(" LEFT JOIN ");

            strQry.AppendLine("(");

            strQry.AppendLine("");
            strQry.AppendLine("--1Start Break UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" SELECT ");

            strQry.AppendLine(" EBC.EMPLOYEE_NO ");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

            strQry.AppendLine(" INNER JOIN ");

            strQry.AppendLine("(SELECT ");

            //ELR - 2014-08-23 Sort Records By BREAK_TIME_IN_MINUTES,BREAK_TIME_OUT_MINUTES
            strQry.AppendLine(" ROW_NUMBER() OVER ");

            strQry.AppendLine(" (ORDER BY ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_DATE");
            strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES) AS SORTED_REC");

            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_DATE");
            strQry.AppendLine(",BREAK_SEQ");
            strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT) AS EBC");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT) AS  EBC");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT) AS  EBC");
                }
            }

            strQry.AppendLine(" ON E.COMPANY_NO = EBC.COMPANY_NO");
            strQry.AppendLine(" AND EBC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EBC.EMPLOYEE_NO ");
            //2013-06-15 >= Cater For Employee Take-On 
            //Errol - 2015-02-12
            strQry.AppendLine(" AND ((EBC.BREAK_DATE > E.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(" AND E.FIRST_RUN_COMPLETED_IND = 'Y')");
            strQry.AppendLine(" OR (EBC.BREAK_DATE >= ISNULL(E.EMPLOYEE_LAST_RUNDATE,DATEADD(DD,-40,'" + strCurrenDate + "'))");
            strQry.AppendLine(" AND ISNULL(E.FIRST_RUN_COMPLETED_IND,'') <> 'Y'))");

            //Set Extra Days to 15
            strQry.AppendLine(" AND EBC.BREAK_DATE <= DATEADD(DD,15,'" + strCurrenDate + "')");

            strQry.AppendLine("");
            strQry.AppendLine("--1End Break UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" INNER JOIN ");

            strQry.AppendLine("(SELECT ");

            //ELR - 2014-08-23 Sort Records By BREAK_TIME_IN_MINUTES,BREAK_TIME_OUT_MINUTES
            strQry.AppendLine(" ROW_NUMBER() OVER ");

            strQry.AppendLine(" (ORDER BY ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_DATE");
            strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES) AS SORTED_REC");

            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_DATE");
            strQry.AppendLine(",BREAK_SEQ");
            strQry.AppendLine(",BREAK_TIME_IN_MINUTES");
            strQry.AppendLine(",BREAK_TIME_OUT_MINUTES");

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_CURRENT) AS EBC2");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_CURRENT) AS EBC2");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_CURRENT) AS EBC2");
                }
            }

            strQry.AppendLine(" ON EBC.COMPANY_NO = EBC2.COMPANY_NO");
            strQry.AppendLine(" AND EBC2.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EBC2.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EBC2.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EBC.BREAK_DATE = EBC2.BREAK_DATE");
            //strQry.AppendLine(" AND EBC.BREAK_SEQ = EBC2.BREAK_SEQ");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
            //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

            strQry.AppendLine(" AND ((EBC.BREAK_TIME_IN_MINUTES IS NULL");
            strQry.AppendLine(" OR EBC.BREAK_TIME_OUT_MINUTES IS NULL)");
            //Same Row
            strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES > EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND EBC.SORTED_REC <= EBC2.SORTED_REC)");

            //Different Rows
            strQry.AppendLine(" OR (EBC.BREAK_TIME_IN_MINUTES < EBC2.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(" AND EBC.SORTED_REC > EBC2.SORTED_REC))) AS TEMP");
            //strQry.AppendLine(" AND EBC.BREAK_SEQ > EBC2.BREAK_SEQ))) AS TEMP");

            strQry.AppendLine(" ON EBC.EMPLOYEE_NO = TEMP.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EBC.BREAK_DATE = TEMP.BREAK_DATE");
            strQry.AppendLine(" AND EBC.BREAK_SEQ = TEMP.BREAK_SEQ ");

            //if (parstrCurrentUserAccessInd == "U")
            //{
            //    strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT ");

            //    strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
            //    strQry.AppendLine(" AND EBC.COMPANY_NO = UEPCT.COMPANY_NO ");
            //    strQry.AppendLine(" AND EBC.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO ");
            //    strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO ");
            //    strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            //}

            //Errol 2012-09-20 Fix Change of PAY_CATEGORY (Orphan Records)
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");

            strQry.AppendLine(" ON EBC.COMPANY_NO = EPC.COMPANY_NO ");
            strQry.AppendLine(" AND EBC.EMPLOYEE_NO = EPC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = EPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL");

            //Errol 2013-04-12
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON EBC.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND EBC.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
            strQry.AppendLine(" AND E.EMPLOYEE_TAKEON_IND = 'Y'");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
            //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

            strQry.AppendLine(" GROUP BY ");

            strQry.AppendLine(" EBC.COMPANY_NO ");
            strQry.AppendLine(",EBC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBC.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.BREAK_DATE");
            strQry.AppendLine(",EBC.BREAK_SEQ");
            strQry.AppendLine(",E.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(",E.FIRST_RUN_COMPLETED_IND");
            strQry.AppendLine(",TEMP.EMPLOYEE_NO");
            strQry.AppendLine(",EBC.BREAK_TIME_OUT_MINUTES");
            strQry.AppendLine(",EBC.BREAK_TIME_IN_MINUTES");

            strQry.AppendLine("");
            strQry.AppendLine("--2End Break UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" ) AS BREAK_TABLE ");

            //Errol 2015-06-06
            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.COMPANY C ");

            strQry.AppendLine(" ON BREAK_TABLE.COMPANY_NO = C.COMPANY_NO ");
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_CURRENT PCPC ");
            strQry.AppendLine(" ON BREAK_TABLE.COMPANY_NO = PCPC.COMPANY_NO ");
            strQry.AppendLine(" AND BREAK_TABLE.PAY_CATEGORY_NO = PCPC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND BREAK_TABLE.PAY_CATEGORY_TYPE = PCPC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND PCPC.RUN_TYPE = 'P' ");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" BREAK_TABLE.COMPANY_NO");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",BREAK_TABLE.DAY_DATE");
            strQry.AppendLine(",BREAK_TABLE.EMPLOYEE_LAST_RUNDATE");
            strQry.AppendLine(",PCPC.PAY_PERIOD_DATE");
            strQry.AppendLine(",C." + RunFieldName + "");

            strQry.AppendLine("");
            strQry.AppendLine("--3End Break UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" ) AS BREAK_SUMMARY_TABLE");

            strQry.AppendLine(" ON TIMESHEET_TOTAL_TABLE.COMPANY_NO = BREAK_SUMMARY_TABLE.COMPANY_NO");
            strQry.AppendLine(" AND TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_TYPE = BREAK_SUMMARY_TABLE.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_NO = BREAK_SUMMARY_TABLE.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND TIMESHEET_TOTAL_TABLE.EMPLOYEE_NO = BREAK_SUMMARY_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(" AND TIMESHEET_TOTAL_TABLE.DAY_DATE = BREAK_SUMMARY_TABLE.DAY_DATE");

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" TIMESHEET_TOTAL_TABLE.COMPANY_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.EMPLOYEE_NO ");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.DAY_DATE");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.TIMESHEET_ACCUM_MINUTES ");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.INDICATOR ");
            strQry.AppendLine(",TIMESHEET_TOTAL_TABLE.BREAK_MINUTES ");

            strQry.AppendLine(",BREAK_SUMMARY_TABLE.BREAK_ACCUM_MINUTES ");
            strQry.AppendLine(",BREAK_SUMMARY_TABLE.INDICATOR ");

            strQry.AppendLine(",BREAK_SUMMARY_TABLE.COMPANY_NO ");
            strQry.AppendLine(",C." + RunFieldName + " ");

            strQry.AppendLine("");
            strQry.AppendLine("--1End Major UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(") AS TEMP1_TABLE ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC1");

            strQry.AppendLine(" ON TEMP1_TABLE.COMPANY_NO = PC1.COMPANY_NO");
            strQry.AppendLine(" AND TEMP1_TABLE.PAY_CATEGORY_NO = PC1.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND TEMP1_TABLE.PAY_CATEGORY_TYPE = PC1.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND PC1.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");

            strQry.AppendLine("");
            strQry.AppendLine("--2End Major UNION 2");
            strQry.AppendLine("");

            strQry.AppendLine(" ) AS TEMP2_TABLE");

            strQry.AppendLine(" WHERE TEMP2_TABLE.INDICATOR = 'X') AS UNION_TABLE");

            strQry.AppendLine(") AS ERRORS_TABLE");
                                 
            strQry.AppendLine("");
            strQry.AppendLine("--3End Major UNION 2");
            strQry.AppendLine("");

        Get_Current_Error_SQL_Continue:

            int intCont = 0;

            return strQry;
        }

        public byte[] Print_Report_Simple(Int64 parint64CompanyNo, string parstrPayrollType,string parstrTableType, string parstrReportType, string parstrEmployeeNoIN, string parstrPayCategoryNoIN, string parstrFromDate, string parstrToDate, string parstrFilterType,string parstrTimeRule,int parintTimeRule, string parstrPrintOrder, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            string strCurrenDate = DateTime.Now.ToString("yyyy-MM-dd");
            string strTablePrefix = "";

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_DESC");
            strQry.AppendLine(",CL.DATE_FORMAT");
            strQry.AppendLine(",'' AS REPORT_DATETIME");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY C");

            strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.COMPANY_LINK CL");
            strQry.AppendLine(" ON C.COMPANY_NO = CL.COMPANY_NO ");

            strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportHeader", parint64CompanyNo);

            if (DataSet.Tables["ReportHeader"].Rows.Count > 0)
            {
                DataSet.Tables["ReportHeader"].Rows[0]["REPORT_DATETIME"] = "Printed   " + DateTime.Now.ToString(DataSet.Tables["ReportHeader"].Rows[0]["DATE_FORMAT"].ToString() + "   HH:mm");
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            if (parstrTableType == "C")
            {
                //Current - Get from PAY_CATEGORY table
                strQry.AppendLine(",PC.DAILY_ROUNDING_IND");
                strQry.AppendLine(",PC.DAILY_ROUNDING_MINUTES");
            }
            else
            {
                strQry.AppendLine(",PCPH.DAILY_ROUNDING_IND");
                strQry.AppendLine(",PCPH.DAILY_ROUNDING_MINUTES");
            }

            if (parstrPrintOrder == "C")
            {
                strQry.AppendLine(",E.EMPLOYEE_CODE");
            }
            else
            {
                if (parstrPrintOrder == "S")
                {
                    strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                }
                else
                {
                    //Name
                    strQry.AppendLine(",E.EMPLOYEE_NAME");
                }
            }
     
            strQry.AppendLine(",ETH.EMPLOYEE_NO");
            strQry.AppendLine(",ETH.TIMESHEET_DATE");

            if (parstrFilterType != "")
            {
                strQry.AppendLine(",D.DAY_NO");
            }

            //Pay Period
            if (parstrReportType == "P"
            && parstrTableType == "H")
            {
                strTablePrefix = "PCWH";

                strQry.AppendLine(",PCWH.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_SUN_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_MON_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_TUE_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_WED_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_THU_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_FRI_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_SAT_BELOW_MINUTES");
            }
            else
            {
                strTablePrefix = "PC";

                strQry.AppendLine(",PC.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_SUN_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_MON_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_TUE_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_WED_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_THU_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_FRI_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_SAT_BELOW_MINUTES");
            }
           
            strQry.AppendLine(",TIMESHEET_TOTAL = CONVERT(VARCHAR,SUM(ETH.TIMESHEET_TIME_OUT_MINUTES - ETH.TIMESHEET_TIME_IN_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,SUM(ETH.TIMESHEET_TIME_OUT_MINUTES - ETH.TIMESHEET_TIME_IN_MINUTES) % 60),2) ");

            strQry.AppendLine(",BREAK_TOTAL = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN PCBH.BREAK_MINUTES > ISNULL(BREAK_TABLE.TOTAL_BREAK_MINUTES,0) ");

            strQry.AppendLine(" THEN CONVERT(VARCHAR,PCBH.BREAK_MINUTES / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,PCBH.BREAK_MINUTES % 60),2)");

            strQry.AppendLine(" ELSE CONVERT(VARCHAR,ISNULL(BREAK_TABLE.TOTAL_BREAK_MINUTES,0) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,ISNULL(BREAK_TABLE.TOTAL_BREAK_MINUTES,0) % 60),2)");

            strQry.AppendLine(" END ");

            strQry.AppendLine(",TIMESHEET_TOTAL_AFTER_BREAK = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN PCBH.BREAK_MINUTES > ISNULL(BREAK_TABLE.TOTAL_BREAK_MINUTES,0) ");

            strQry.AppendLine(" THEN CONVERT(VARCHAR,(SUM(ETH.TIMESHEET_TIME_OUT_MINUTES - ETH.TIMESHEET_TIME_IN_MINUTES) - PCBH.BREAK_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(SUM(ETH.TIMESHEET_TIME_OUT_MINUTES - ETH.TIMESHEET_TIME_IN_MINUTES) - PCBH.BREAK_MINUTES) % 60),2)");
       
            strQry.AppendLine(" ELSE CONVERT(VARCHAR,(SUM(ETH.TIMESHEET_TIME_OUT_MINUTES - ETH.TIMESHEET_TIME_IN_MINUTES) - ISNULL(BREAK_TABLE.TOTAL_BREAK_MINUTES,0)) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(SUM(ETH.TIMESHEET_TIME_OUT_MINUTES - ETH.TIMESHEET_TIME_IN_MINUTES) - ISNULL(BREAK_TABLE.TOTAL_BREAK_MINUTES,0)) % 60),2)");
            
            strQry.AppendLine(" END ");

            strQry.AppendLine(",TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES = ");
            strQry.AppendLine(" CASE ");

            strQry.AppendLine(" WHEN PCBH.BREAK_MINUTES > ISNULL(BREAK_TABLE.TOTAL_BREAK_MINUTES,0) ");

            strQry.AppendLine(" THEN SUM(ETH.TIMESHEET_TIME_OUT_MINUTES - ETH.TIMESHEET_TIME_IN_MINUTES) - PCBH.BREAK_MINUTES ");
            
            strQry.AppendLine(" ELSE SUM(ETH.TIMESHEET_TIME_OUT_MINUTES - ETH.TIMESHEET_TIME_IN_MINUTES) - ISNULL(BREAK_TABLE.TOTAL_BREAK_MINUTES,0)");

            strQry.AppendLine(" END ");

            if (parstrFilterType == "N")
            {
                strQry.AppendLine(",TIMESHEET_APPLIED_PARAMETERS = ");
                strQry.AppendLine(" CASE ");

                strQry.AppendLine(" WHEN D.DAY_NO = 7 ");

                strQry.AppendLine(" THEN CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SUN_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SUN_BELOW_MINUTES % 60)),2) + ' - ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SUN_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SUN_ABOVE_MINUTES % 60)),2) ");

                strQry.AppendLine(" WHEN D.DAY_NO = 1 ");

                strQry.AppendLine(" THEN CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_MON_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_MON_BELOW_MINUTES % 60)),2) + ' - ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_MON_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_MON_ABOVE_MINUTES % 60)),2) ");

                strQry.AppendLine(" WHEN D.DAY_NO = 2 ");

                strQry.AppendLine(" THEN CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_TUE_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_TUE_BELOW_MINUTES % 60)),2) + ' - ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_TUE_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_TUE_ABOVE_MINUTES % 60)),2) ");

                strQry.AppendLine(" WHEN D.DAY_NO = 3 ");

                strQry.AppendLine(" THEN CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_WED_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_WED_BELOW_MINUTES % 60)),2) + ' - ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_WED_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_WED_ABOVE_MINUTES % 60)),2) ");

                strQry.AppendLine(" WHEN D.DAY_NO = 4 ");

                strQry.AppendLine(" THEN CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_THU_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_THU_BELOW_MINUTES % 60)),2) + ' - ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_THU_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_THU_ABOVE_MINUTES % 60)),2) ");

                strQry.AppendLine(" WHEN D.DAY_NO = 5 ");

                strQry.AppendLine(" THEN CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_FRI_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_FRI_BELOW_MINUTES % 60)),2) + ' - ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_FRI_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_FRI_ABOVE_MINUTES % 60)),2) ");

                strQry.AppendLine(" WHEN D.DAY_NO = 6 ");

                strQry.AppendLine(" THEN CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SAT_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SAT_BELOW_MINUTES % 60)),2) + ' - ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SAT_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SAT_ABOVE_MINUTES % 60)),2) ");

                strQry.AppendLine(" END ");
            }
            else
            {
                if (parstrFilterType == "E")
                {
                    strQry.AppendLine(",TIMESHEET_APPLIED_PARAMETERS = ");
                    strQry.AppendLine(" CASE ");

                    strQry.AppendLine(" WHEN D.DAY_NO = 7 AND " + strTablePrefix + ".EXCEPTION_SUN_BELOW_MINUTES = 0");

                    strQry.AppendLine(" THEN '> 0:00'");

                    strQry.AppendLine(" WHEN D.DAY_NO = 7 ");

                    strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SUN_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SUN_BELOW_MINUTES % 60)),2) + ' or > ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SUN_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SUN_ABOVE_MINUTES % 60)),2) ");

                    strQry.AppendLine(" WHEN D.DAY_NO = 1 AND " + strTablePrefix + ".EXCEPTION_MON_BELOW_MINUTES = 0");

                    strQry.AppendLine(" THEN '>0:00'");

                    strQry.AppendLine(" WHEN D.DAY_NO = 1 ");

                    strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_MON_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_MON_BELOW_MINUTES % 60)),2) + ' or > ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_MON_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_MON_ABOVE_MINUTES % 60)),2) ");

                    strQry.AppendLine(" WHEN D.DAY_NO = 2 AND " + strTablePrefix + ".EXCEPTION_TUE_BELOW_MINUTES = 0");

                    strQry.AppendLine(" THEN '> 0:00'");

                    strQry.AppendLine(" WHEN D.DAY_NO = 2 ");

                    strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_TUE_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_TUE_BELOW_MINUTES % 60)),2) + ' or > ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_TUE_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_TUE_ABOVE_MINUTES % 60)),2) ");

                    strQry.AppendLine(" WHEN D.DAY_NO = 3 AND " + strTablePrefix + ".EXCEPTION_WED_BELOW_MINUTES = 0");

                    strQry.AppendLine(" THEN '> 0:00'");

                    strQry.AppendLine(" WHEN D.DAY_NO = 3 ");

                    strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_WED_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_WED_BELOW_MINUTES % 60)),2) + ' or > ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_WED_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_WED_ABOVE_MINUTES % 60)),2) ");

                    strQry.AppendLine(" WHEN D.DAY_NO = 4 AND " + strTablePrefix + ".EXCEPTION_THU_BELOW_MINUTES = 0");

                    strQry.AppendLine(" THEN '> 0:00'");

                    strQry.AppendLine(" WHEN D.DAY_NO = 4 ");

                    strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_THU_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_THU_BELOW_MINUTES % 60)),2) + ' or > ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_THU_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_THU_ABOVE_MINUTES % 60)),2) ");

                    strQry.AppendLine(" WHEN D.DAY_NO = 5 AND " + strTablePrefix + ".EXCEPTION_FRI_BELOW_MINUTES = 0");

                    strQry.AppendLine(" THEN '> 0:00'");

                    strQry.AppendLine(" WHEN D.DAY_NO = 5 ");

                    strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_FRI_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_FRI_BELOW_MINUTES % 60)),2) + ' or > ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_FRI_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_FRI_ABOVE_MINUTES % 60)),2) ");

                    strQry.AppendLine(" WHEN D.DAY_NO = 6 AND " + strTablePrefix + ".EXCEPTION_SAT_BELOW_MINUTES = 0");

                    strQry.AppendLine(" THEN '> 0:00'");

                    strQry.AppendLine(" WHEN D.DAY_NO = 6 ");

                    strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SAT_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SAT_BELOW_MINUTES % 60)),2) + ' or > ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SAT_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SAT_ABOVE_MINUTES % 60)),2) ");

                    //2015-11-04
                    strQry.AppendLine(" ELSE '> 0:00'");

                    strQry.AppendLine(" END ");
                }
                else
                {
                    if (parstrFilterType == "L")
                    {
                        strQry.AppendLine(",TIMESHEET_APPLIED_PARAMETERS = ");
                        strQry.AppendLine(" CASE ");

                        strQry.AppendLine(" WHEN D.DAY_NO = 7 ");

                        strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SUN_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SUN_BELOW_MINUTES % 60)),2) ");

                        strQry.AppendLine(" WHEN D.DAY_NO = 1 ");

                        strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_MON_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_MON_BELOW_MINUTES % 60)),2) ");

                        strQry.AppendLine(" WHEN D.DAY_NO = 2 ");

                        strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_TUE_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_TUE_BELOW_MINUTES % 60)),2) ");

                        strQry.AppendLine(" WHEN D.DAY_NO = 3 ");

                        strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_WED_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_WED_BELOW_MINUTES % 60)),2) ");

                        strQry.AppendLine(" WHEN D.DAY_NO = 4 ");

                        strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_THU_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_THU_BELOW_MINUTES % 60)),2) ");

                        strQry.AppendLine(" WHEN D.DAY_NO = 5 ");

                        strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_FRI_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_FRI_BELOW_MINUTES % 60)),2) ");

                        strQry.AppendLine(" WHEN D.DAY_NO = 6 ");

                        strQry.AppendLine(" THEN '< ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SAT_BELOW_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SAT_BELOW_MINUTES % 60)),2) ");

                        strQry.AppendLine(" END ");
                    }
                    else
                    {
                        if (parstrFilterType == "H")
                        {
                            strQry.AppendLine(",TIMESHEET_APPLIED_PARAMETERS = ");
                            strQry.AppendLine(" CASE ");

                            strQry.AppendLine(" WHEN D.DAY_NO = 7 ");

                            strQry.AppendLine(" THEN '> ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SUN_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SUN_ABOVE_MINUTES % 60)),2) ");

                            strQry.AppendLine(" WHEN D.DAY_NO = 1 ");

                            strQry.AppendLine(" THEN '> ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_MON_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_MON_ABOVE_MINUTES % 60)),2) ");

                            strQry.AppendLine(" WHEN D.DAY_NO = 2 ");

                            strQry.AppendLine(" THEN '> ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_TUE_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_TUE_ABOVE_MINUTES % 60)),2) ");

                            strQry.AppendLine(" WHEN D.DAY_NO = 3 ");

                            strQry.AppendLine(" THEN '> ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_WED_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_WED_ABOVE_MINUTES % 60)),2) ");

                            strQry.AppendLine(" WHEN D.DAY_NO = 4 ");

                            strQry.AppendLine(" THEN '> ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_THU_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_THU_ABOVE_MINUTES % 60)),2) ");

                            strQry.AppendLine(" WHEN D.DAY_NO = 5 ");

                            strQry.AppendLine(" THEN '> ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_FRI_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_FRI_ABOVE_MINUTES % 60)),2) ");

                            strQry.AppendLine(" WHEN D.DAY_NO = 6 ");

                            strQry.AppendLine(" THEN '> ' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SAT_ABOVE_MINUTES) / 60) + ':' + RIGHT('00' + CONVERT(VARCHAR,(" + strTablePrefix + ".EXCEPTION_SAT_ABOVE_MINUTES % 60)),2) ");

                            strQry.AppendLine(" ELSE '> 0:00'");

                            strQry.AppendLine(" END ");
                        }
                    }
                }
            }

            strQry.AppendLine(",PC.PAY_CATEGORY_NO");

            string strTableType = "HISTORY";
            
            if (parstrTableType == "C")
            {
                strTableType = "CURRENT";
            }

            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_" + strTableType + " ETH");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_" + strTableType + " ETH");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_" + strTableType + " ETH");
                }
            }

            //2015-10-14
            if (parstrFilterType == "I"
            || parstrFilterType == "O")
            {
                strQry.AppendLine(" INNER JOIN ");

                strQry.AppendLine("(SELECT ");

                strQry.AppendLine(" EMPLOYEE_NO ");
                strQry.AppendLine(",PAY_CATEGORY_NO ");
                strQry.AppendLine(",TIMESHEET_DATE ");

                if (parstrPayrollType == "W")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIMESHEET_" + strTableType + " ETH");
                }
                else
                {
                    if (parstrPayrollType == "S")
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_TIMESHEET_" + strTableType + " ETH");
                    }
                    else
                    {
                        strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_TIMESHEET_" + strTableType + " ETH");
                    }
                }

                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                strQry.AppendLine(" GROUP BY ");

                strQry.AppendLine(" EMPLOYEE_NO ");
                strQry.AppendLine(",PAY_CATEGORY_NO ");
                strQry.AppendLine(",TIMESHEET_DATE ");


                string strTime = parintTimeRule.ToString();
                int intTime = 0;

                if (strTime.Length > 2)
                {
                    intTime = Convert.ToInt32(strTime.Substring(0, strTime.Length - 2)) * 60;
                    intTime += Convert.ToInt32(strTime.Substring(strTime.Length - 2, 2));

                }
                else
                {
                    intTime = parintTimeRule;
                }

                if (parstrFilterType == "I")
                {
                    strQry.AppendLine(" HAVING MIN(TIMESHEET_TIME_IN_MINUTES) ");
                }
                else
                {
                    strQry.AppendLine(" HAVING MAX(TIMESHEET_TIME_OUT_MINUTES) ");
                }

                if (parstrTimeRule == "G")
                {
                    strQry.Append(" > " + intTime + ") AS CALC_TABLE ");
                }
                else
                {
                    strQry.Append(" < " + intTime + ") AS CALC_TABLE ");
                }

                strQry.AppendLine(" ON ETH.EMPLOYEE_NO = CALC_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = CALC_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND ETH.TIMESHEET_DATE = CALC_TABLE.TIMESHEET_DATE");
            }

            if (parstrTableType == "C")
            {
                strQry.AppendLine(" LEFT JOIN ");
                
                //Get SQL For Errors in Timesheets / Breaks (Returns EMPLOYEE_NO,TIMESHEET_DATE,PAY_CATEGORY_NO)
                strQry.Append(Get_Current_Error_SQL(parint64CompanyNo, parstrPayrollType));

                strQry.AppendLine(" ON ETH.EMPLOYEE_NO = ERRORS_TABLE.EMPLOYEE_NO  ");
                strQry.AppendLine(" AND ETH.TIMESHEET_DATE = ERRORS_TABLE.TIMESHEET_DATE");
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = ERRORS_TABLE.PAY_CATEGORY_NO ");
            }

            if (parstrFilterType != "")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll.dbo.DATES D ");
                strQry.AppendLine(" ON ETH.TIMESHEET_DATE = D.DAY_DATE");
            }

            //History
            if (parstrTableType == "H")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH");
                strQry.AppendLine(" ON ETH.COMPANY_NO = PCPH.COMPANY_NO");
                strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = PCPH.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PCPH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCPH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND PCPH.RUN_TYPE = 'P'");
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");
            strQry.AppendLine(" ON ETH.COMPANY_NO = PC.COMPANY_NO");
            strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PC.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL ");

            //Pay Period
            if (parstrReportType == "P")
            {
                if (parstrTableType == "H")
                {
                    strQry.AppendLine(" INNER JOIN ");

                    strQry.AppendLine("(SELECT DISTINCT ");

                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");

                    strQry.AppendLine(",EXCEPTION_SUN_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_SUN_BELOW_MINUTES");

                    strQry.AppendLine(",EXCEPTION_MON_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_MON_BELOW_MINUTES");

                    strQry.AppendLine(",EXCEPTION_TUE_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_TUE_BELOW_MINUTES");

                    strQry.AppendLine(",EXCEPTION_WED_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_WED_BELOW_MINUTES");

                    strQry.AppendLine(",EXCEPTION_THU_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_THU_BELOW_MINUTES");

                    strQry.AppendLine(",EXCEPTION_FRI_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_FRI_BELOW_MINUTES");

                    strQry.AppendLine(",EXCEPTION_SAT_ABOVE_MINUTES");
                    strQry.AppendLine(",EXCEPTION_SAT_BELOW_MINUTES");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_HISTORY");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

                    if (parstrPayCategoryNoIN != "")
                    {
                        strQry.AppendLine(" AND PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
                    }

                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                    strQry.AppendLine(" AND PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate) + ") AS PCWH");

                    strQry.AppendLine(" ON PC.COMPANY_NO = PCWH.COMPANY_NO");
                    strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCWH.PAY_CATEGORY_NO");
                }
            }

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON ETH.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND ETH.EMPLOYEE_NO = E.EMPLOYEE_NO");
            //2017-01-28 (Allow Employee to Change PAY_CATEGORY_TYPE) 
            //strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");
          
            //2013-08-30
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND ETH.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND ETH.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = UEPCT.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND UEPCT.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            }

            if (parstrTableType == "C")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK PCBH");
                strQry.AppendLine(" ON ETH.COMPANY_NO = PCBH.COMPANY_NO");
                //strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = PCBH.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PCBH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCBH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND PCBH.DATETIME_DELETE_RECORD IS NULL");
            }
            else
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_BREAK_HISTORY PCBH");
                strQry.AppendLine(" ON ETH.COMPANY_NO = PCBH.COMPANY_NO");
                strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = PCBH.PAY_PERIOD_DATE");
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO = PCBH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCBH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            }

            strQry.AppendLine(" LEFT JOIN ");

            strQry.AppendLine("(SELECT ");

            strQry.AppendLine(" EBH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBH.EMPLOYEE_NO");
            strQry.AppendLine(",EBH.BREAK_DATE");

            strQry.AppendLine(",SUM(EBH.BREAK_TIME_OUT_MINUTES - EBH.BREAK_TIME_IN_MINUTES) AS TOTAL_BREAK_MINUTES ");
          
            if (parstrPayrollType == "W")
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_BREAK_" + strTableType + " EBH");
            }
            else
            {
                if (parstrPayrollType == "S")
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_SALARY_BREAK_" + strTableType + " EBH");
                }
                else
                {
                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_TIME_ATTEND_BREAK_" + strTableType + " EBH");
                }
            }

            strQry.AppendLine(" WHERE EBH.COMPANY_NO = " + parint64CompanyNo);

            if (parstrPayCategoryNoIN != "")
            {
                strQry.AppendLine(" AND EBH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
            }

            if (parstrEmployeeNoIN != "")
            {
                strQry.AppendLine(" AND EBH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
            }

            //Pay Period
            if (parstrReportType == "P")
            {
                if (parstrTableType == "H")
                {
                    strQry.AppendLine(" AND EBH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                }
            }
            else
            {
                //Week
                if (parstrReportType == "W")
                {
                    strQry.AppendLine(" AND EBH.BREAK_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                    strQry.AppendLine(" AND EBH.BREAK_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                }
                else
                {
                    if (parstrReportType == "M")
                    {
                        strQry.AppendLine(" AND YEAR(EBH.BREAK_DATE) = " + parstrFromDate.Substring(0, 4) + " AND MONTH(EBH.BREAK_DATE) = " + parstrFromDate.Substring(4));
                    }
                    else
                    {
                        if (parstrReportType == "E"
                        || parstrReportType == "H")
                        {
                            strQry.AppendLine(" AND EBH.BREAK_DATE = '" + parstrFromDate + "'");
                        }
                        else
                        {
                            if (parstrReportType == "G")
                            {
                                strQry.AppendLine(" AND EBH.BREAK_DATE >= '" + parstrFromDate + "'");
                            }
                            else
                            {
                                if (parstrReportType == "L")
                                {
                                    strQry.AppendLine(" AND EBH.BREAK_DATE <= '" + parstrFromDate + "'");
                                }
                                else
                                {
                                    strQry.AppendLine(" AND EBH.BREAK_DATE >= '" + parstrFromDate + "'");
                                    strQry.AppendLine(" AND EBH.BREAK_DATE <= '" + parstrToDate + "'");
                                }
                            }
                        }
                    }
                }
            }
        
            strQry.AppendLine(" GROUP BY ");

            strQry.AppendLine(" EBH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EBH.EMPLOYEE_NO");
            strQry.AppendLine(",EBH.BREAK_DATE) AS BREAK_TABLE");

            strQry.AppendLine(" ON ETH.PAY_CATEGORY_NO = BREAK_TABLE.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND ETH.EMPLOYEE_NO = BREAK_TABLE.EMPLOYEE_NO");
            strQry.AppendLine(" AND ETH.TIMESHEET_DATE = BREAK_TABLE.BREAK_DATE");

            strQry.AppendLine(" WHERE ETH.COMPANY_NO = " + parint64CompanyNo);

            if (parstrPayCategoryNoIN != "")
            {
                strQry.AppendLine(" AND ETH.PAY_CATEGORY_NO IN " + parstrPayCategoryNoIN);
            }

            if (parstrEmployeeNoIN != "")
            {
                strQry.AppendLine(" AND ETH.EMPLOYEE_NO IN " + parstrEmployeeNoIN);
            }

            //Current
            if (parstrTableType == "C")
            {
                //Errors
                strQry.AppendLine(" AND ERRORS_TABLE.EMPLOYEE_NO IS NULL  ");
            }

            //Pay Period
            if (parstrReportType == "P")
            {
                if (parstrTableType == "H")
                {
                    strQry.AppendLine(" AND ETH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                }
            }
            else
            {
                //Week
                if (parstrReportType == "W")
                {
                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= " + clsDBConnectionObjects.Text2DynamicSQL(parstrToDate));
                }
                else
                {
                    if (parstrReportType == "M")
                    {
                        strQry.AppendLine(" AND YEAR(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(0, 4) + " AND MONTH(ETH.TIMESHEET_DATE) = " + parstrFromDate.Substring(4));
                    }
                    else
                    {
                        if (parstrReportType == "E"
                        || parstrReportType == "H")
                        {
                            strQry.AppendLine(" AND ETH.TIMESHEET_DATE = '" + parstrFromDate + "'");
                        }
                        else
                        {
                            if (parstrReportType == "G")
                            {
                                strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                            }
                            else
                            {
                                if (parstrReportType == "L")
                                {
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrFromDate + "'");
                                }
                                else
                                {
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE >= '" + parstrFromDate + "'");
                                    strQry.AppendLine(" AND ETH.TIMESHEET_DATE <= '" + parstrToDate + "'");
                                }
                            }
                        }
                    }
                }
            }
           
            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            if (parstrTableType == "C")
            {
                strQry.AppendLine(",PC.DAILY_ROUNDING_IND");
                strQry.AppendLine(",PC.DAILY_ROUNDING_MINUTES");
            }
            else
            {
                strQry.AppendLine(",PCPH.DAILY_ROUNDING_IND");
                strQry.AppendLine(",PCPH.DAILY_ROUNDING_MINUTES");
            }

            if (parstrPrintOrder == "C")
            {
                strQry.AppendLine(",E.EMPLOYEE_CODE");
            }
            else
            {
                if (parstrPrintOrder == "S")
                {
                    strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                }
                else
                {
                    //Name
                    strQry.AppendLine(",E.EMPLOYEE_NAME");
                }
            }
    
            strQry.AppendLine(",ETH.EMPLOYEE_NO");
            strQry.AppendLine(",ETH.TIMESHEET_DATE");

            if (parstrFilterType != "")
            {
                strQry.AppendLine(",D.DAY_NO");
            }

            //Pay Period
            if (parstrReportType == "P"
            && parstrTableType == "H")
            {
                strQry.AppendLine(",PCWH.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_SUN_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_MON_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_TUE_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_WED_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_THU_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_FRI_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_SAT_BELOW_MINUTES");
            }
            else
            {
                strQry.AppendLine(",PC.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_SUN_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_MON_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_TUE_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_WED_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_THU_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_FRI_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_SAT_BELOW_MINUTES");
            }

            strQry.AppendLine(",PCBH.BREAK_MINUTES");
            strQry.AppendLine(",BREAK_TABLE.TOTAL_BREAK_MINUTES");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_DESC");

            if (parstrPrintOrder == "C")
            {
                strQry.AppendLine(",E.EMPLOYEE_CODE");
            }
            else
            {
                if (parstrPrintOrder == "S")
                {
                    strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                }
                else
                {
                    //Name
                    strQry.AppendLine(",E.EMPLOYEE_NAME");
                }
            }
            
            strQry.AppendLine(",ETH.TIMESHEET_DATE");
            
            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "ReportTimeSheet", parint64CompanyNo);

            string strPayCategoryNos = "";

            //Get PAY_CATEGORY_NO in Report
            var groupQuery = from table in DataSet.Tables["ReportTimeSheet"].AsEnumerable()
                                group table by new
                                {
                                    PayCategoryNo = table.Field<Int16>("PAY_CATEGORY_NO")

                                } into grp
                                select new
                                {
                                    PayCategoryNo = grp.Key.PayCategoryNo
                                };

            foreach (var row in groupQuery)
            {
                if (strPayCategoryNos == "")
                {
                    strPayCategoryNos = row.PayCategoryNo.ToString();
                }
                else
                {

                    strPayCategoryNos += "," + row.PayCategoryNo.ToString();
                }
            }

            strQry.Clear();

            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");

            //Pay Period
            if (parstrReportType == "P"
            && parstrTableType == "H")
            {
                strQry.AppendLine(",PCWH.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_SUN_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_MON_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_TUE_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_WED_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_THU_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_FRI_BELOW_MINUTES");

                strQry.AppendLine(",PCWH.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",PCWH.EXCEPTION_SAT_BELOW_MINUTES");
            }
            else
            {
                strQry.AppendLine(",PC.EXCEPTION_SUN_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_SUN_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_MON_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_MON_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_TUE_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_TUE_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_WED_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_WED_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_THU_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_THU_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_FRI_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_FRI_BELOW_MINUTES");

                strQry.AppendLine(",PC.EXCEPTION_SAT_ABOVE_MINUTES");
                strQry.AppendLine(",PC.EXCEPTION_SAT_BELOW_MINUTES");
            }

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC");

            //Pay Period
            if (parstrReportType == "P"
            && parstrTableType == "H")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_WEEK_HISTORY PCWH");
                strQry.AppendLine(" ON PC.COMPANY_NO = PCWH.COMPANY_NO");
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = PCWH.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND PCWH.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
                strQry.AppendLine(" AND PCWH.PAY_PERIOD_DATE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrFromDate));
            }

            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo);

            if (strPayCategoryNos != "")
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN (" + strPayCategoryNos + ")");
            }
            else
            {
                strQry.AppendLine(" AND PC.PAY_CATEGORY_NO IN (-1)");
            }

            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = '" + parstrPayrollType + "'");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL");

            this.clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategorySelected", parint64CompanyNo);

            //Rounding
            DataView ReportTimeSheetDataView = new DataView(DataSet.Tables["ReportTimeSheet"],
                    "DAILY_ROUNDING_IND <> 0 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES / 5 <> 0",
                    "",
                    DataViewRowState.CurrentRows);

            int intNormalTimeMinutesRounded = 0;

            for (int intRow = 0; intRow < ReportTimeSheetDataView.Count; intRow++)
            {
                intNormalTimeMinutesRounded = Convert.ToInt32(ReportTimeSheetDataView[intRow]["TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES"]);

                //if (ReportTimeSheetDataView[intRow]["EMPLOYEE_NO"].ToString() == "5")
                //{
                //    if (Convert.ToDateTime(ReportTimeSheetDataView[intRow]["TIMESHEET_DATE"]).ToString("yyyy-MM-dd") == "2015-01-22")
                //    {
                //        string strStop = "");
                //    }
                //}

                clsDBConnectionObjects.Round_For_Period(Convert.ToInt32(ReportTimeSheetDataView[intRow]["DAILY_ROUNDING_IND"]), Convert.ToInt32(ReportTimeSheetDataView[intRow]["DAILY_ROUNDING_MINUTES"]), ref intNormalTimeMinutesRounded);

                ReportTimeSheetDataView[intRow]["TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES"] = intNormalTimeMinutesRounded;
            }

            DataSet.Tables["ReportTimeSheet"].AcceptChanges();

            if (parstrFilterType != ""
            &&  parstrFilterType != "I"
            &&  parstrFilterType != "O"
            &&  parstrFilterType != "X")
            {
                for (int intRow = 0; intRow < DataSet.Tables["PayCategorySelected"].Rows.Count; intRow++)
                {
                    string strExceptionFilter = "PAY_CATEGORY_NO = " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["PAY_CATEGORY_NO"].ToString() + " AND (";
                    string strExceptionFilterInitial = strExceptionFilter;

                    if (parstrFilterType == "N")
                    {
                        if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SUN_BELOW_MINUTES"]) > 0)
                        {
                            strExceptionFilter += "(DAY_NO = 0 AND (TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES < " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SUN_BELOW_MINUTES"].ToString() + " OR TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES > " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SUN_ABOVE_MINUTES"].ToString() + "))";
                        }
                        else
                        {
                            //Remove The Day
                            strExceptionFilter += "(DAY_NO = 0)";
                        }

                        if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_BELOW_MINUTES"]) > 0)
                        {
                            if (strExceptionFilter == strExceptionFilterInitial)
                            {
                                strExceptionFilter += "(DAY_NO = 1 AND (TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES < " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_BELOW_MINUTES"].ToString() + " OR TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES > " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_ABOVE_MINUTES"].ToString() + "))";
                            }
                            else
                            {
                                strExceptionFilter += " OR (DAY_NO = 1 AND (TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES < " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_BELOW_MINUTES"].ToString() + " OR TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES > " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_ABOVE_MINUTES"].ToString() + "))";
                            }
                        }
                        else
                        {
                            //Remove the Day
                            if (strExceptionFilter == strExceptionFilterInitial)
                            {
                                strExceptionFilter += "(DAY_NO = 1)";
                            }
                            else
                            {
                                strExceptionFilter += " OR (DAY_NO = 1)";
                            }
                        }

                        if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_BELOW_MINUTES"]) > 0)
                        {
                            if (strExceptionFilter == strExceptionFilterInitial)
                            {
                                strExceptionFilter += "(DAY_NO = 2 AND (TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES < " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_BELOW_MINUTES"].ToString() + " OR TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES > " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_ABOVE_MINUTES"].ToString() + "))";
                            }
                            else
                            {
                                strExceptionFilter += " OR (DAY_NO = 2 AND (TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES < " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_BELOW_MINUTES"].ToString() + " OR TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES > " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_ABOVE_MINUTES"].ToString() + "))";
                            }
                        }
                        else
                        {
                            //Remove the Day
                            if (strExceptionFilter == strExceptionFilterInitial)
                            {
                                strExceptionFilter += "(DAY_NO = 2)";
                            }
                            else
                            {
                                strExceptionFilter += " OR (DAY_NO = 2)";
                            }
                        }

                        if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_BELOW_MINUTES"]) > 0)
                        {
                            if (strExceptionFilter == strExceptionFilterInitial)
                            {
                                strExceptionFilter += "(DAY_NO = 3 AND (TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES < " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_BELOW_MINUTES"].ToString() + " OR TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES > " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_ABOVE_MINUTES"].ToString() + "))";
                            }
                            else
                            {
                                strExceptionFilter += " OR (DAY_NO = 3 AND (TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES < " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_BELOW_MINUTES"].ToString() + " OR TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES > " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_ABOVE_MINUTES"].ToString() + "))";
                            }
                        }
                        else
                        {
                            //Remove the Day
                            if (strExceptionFilter == strExceptionFilterInitial)
                            {
                                strExceptionFilter += "(DAY_NO = 3)";
                            }
                            else
                            {
                                strExceptionFilter += " OR (DAY_NO = 3)";
                            }
                        }

                        if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_BELOW_MINUTES"]) > 0)
                        {
                            if (strExceptionFilter == strExceptionFilterInitial)
                            {
                                strExceptionFilter += "(DAY_NO = 4 AND (TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES < " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_BELOW_MINUTES"].ToString() + " OR TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES > " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_ABOVE_MINUTES"].ToString() + "))";
                            }
                            else
                            {
                                strExceptionFilter += " OR (DAY_NO = 4 AND (TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES < " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_BELOW_MINUTES"].ToString() + " OR TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES > " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_ABOVE_MINUTES"].ToString() + "))";
                            }
                        }
                        else
                        {
                            //Remove the Day
                            if (strExceptionFilter == strExceptionFilterInitial)
                            {
                                strExceptionFilter += "(DAY_NO = 4)";
                            }
                            else
                            {
                                strExceptionFilter += " OR (DAY_NO = 4)";
                            }
                        }

                        if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_BELOW_MINUTES"]) > 0)
                        {
                            if (strExceptionFilter == strExceptionFilterInitial)
                            {
                                strExceptionFilter += "(DAY_NO = 5 AND (TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES < " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_BELOW_MINUTES"].ToString() + " OR TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES > " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_ABOVE_MINUTES"].ToString() + "))";
                            }
                            else
                            {
                                strExceptionFilter += " OR (DAY_NO = 5 AND (TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES < " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_BELOW_MINUTES"].ToString() + " OR TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES > " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_ABOVE_MINUTES"].ToString() + "))";
                            }
                        }
                        else
                        {
                            //Remove the Day
                            if (strExceptionFilter == strExceptionFilterInitial)
                            {
                                strExceptionFilter += "(DAY_NO = 5)";
                            }
                            else
                            {
                                strExceptionFilter += " OR (DAY_NO = 5)";
                            }
                        }

                        if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_BELOW_MINUTES"]) > 0)
                        {
                            if (strExceptionFilter == strExceptionFilterInitial)
                            {
                                strExceptionFilter += "(DAY_NO = 6 AND (TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES < " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_BELOW_MINUTES"].ToString() + " OR TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES > " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_ABOVE_MINUTES"].ToString() + "))";
                            }
                            else
                            {
                                strExceptionFilter += " OR (DAY_NO = 6 AND (TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES < " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_BELOW_MINUTES"].ToString() + " OR TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES > " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_ABOVE_MINUTES"].ToString() + "))";
                            }
                        }
                        else
                        {
                            //Remove the Day
                            if (strExceptionFilter == strExceptionFilterInitial)
                            {
                                strExceptionFilter += "(DAY_NO = 6)";
                            }
                            else
                            {
                                strExceptionFilter += " OR (DAY_NO = 6)";
                            }
                        }
                    }
                    else
                    {
                        if (parstrFilterType == "E")
                        {
                            if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SUN_BELOW_MINUTES"]) > 0)
                            {
                                strExceptionFilter += "(DAY_NO = 0 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SUN_BELOW_MINUTES"].ToString() + " AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SUN_ABOVE_MINUTES"].ToString() + ")";
                            }

                            if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_BELOW_MINUTES"]) > 0)
                            {
                                if (strExceptionFilter == strExceptionFilterInitial)
                                {
                                    strExceptionFilter += "(DAY_NO = 1 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_BELOW_MINUTES"].ToString() + " AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_ABOVE_MINUTES"].ToString() + ")";
                                }
                                else
                                {
                                    strExceptionFilter += " OR (DAY_NO = 1 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_BELOW_MINUTES"].ToString() + " AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_ABOVE_MINUTES"].ToString() + ")";
                                }
                            }

                            if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_BELOW_MINUTES"]) > 0)
                            {
                                if (strExceptionFilter == strExceptionFilterInitial)
                                {
                                    strExceptionFilter += "(DAY_NO = 2 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_BELOW_MINUTES"].ToString() + " AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_ABOVE_MINUTES"].ToString() + ")";
                                }
                                else
                                {
                                    strExceptionFilter += " OR (DAY_NO = 2 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_BELOW_MINUTES"].ToString() + " AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_ABOVE_MINUTES"].ToString() + ")";
                                }
                            }

                            if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_BELOW_MINUTES"]) > 0)
                            {
                                if (strExceptionFilter == strExceptionFilterInitial)
                                {
                                    strExceptionFilter += "(DAY_NO = 3 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_BELOW_MINUTES"].ToString() + " AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_ABOVE_MINUTES"].ToString() + ")";
                                }
                                else
                                {
                                    strExceptionFilter += " OR (DAY_NO = 3 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_BELOW_MINUTES"].ToString() + " AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_ABOVE_MINUTES"].ToString() + ")";
                                }
                            }

                            if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_BELOW_MINUTES"]) > 0)
                            {
                                if (strExceptionFilter == strExceptionFilterInitial)
                                {
                                    strExceptionFilter += "(DAY_NO = 4 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_BELOW_MINUTES"].ToString() + " AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_ABOVE_MINUTES"].ToString() + ")";
                                }
                                else
                                {
                                    strExceptionFilter += " OR (DAY_NO = 4 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_BELOW_MINUTES"].ToString() + " AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_ABOVE_MINUTES"].ToString() + ")";
                                }
                            }

                            if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_BELOW_MINUTES"]) > 0)
                            {
                                if (strExceptionFilter == strExceptionFilterInitial)
                                {
                                    strExceptionFilter += "(DAY_NO = 5 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_BELOW_MINUTES"].ToString() + " AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_ABOVE_MINUTES"].ToString() + ")";
                                }
                                else
                                {
                                    strExceptionFilter += " OR (DAY_NO = 5 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_BELOW_MINUTES"].ToString() + " AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_ABOVE_MINUTES"].ToString() + ")";
                                }
                            }

                            if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_BELOW_MINUTES"]) > 0)
                            {
                                if (strExceptionFilter == strExceptionFilterInitial)
                                {
                                    strExceptionFilter += "(DAY_NO = 6 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_BELOW_MINUTES"].ToString() + " AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_ABOVE_MINUTES"].ToString() + ")";
                                }
                                else
                                {
                                    strExceptionFilter += " OR (DAY_NO = 6 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_BELOW_MINUTES"].ToString() + " AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_ABOVE_MINUTES"].ToString() + ")";
                                }
                            }
                        }
                        else
                        {
                            if (parstrFilterType == "L")
                            {
                                if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SUN_BELOW_MINUTES"]) > 0)
                                {
                                    strExceptionFilter += "(DAY_NO = 0 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SUN_BELOW_MINUTES"].ToString() + ")";
                                }
                                else
                                {
                                    //Remove The Day
                                    strExceptionFilter += "(DAY_NO = 0)";
                                }

                                if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_BELOW_MINUTES"]) > 0)
                                {
                                    if (strExceptionFilter == strExceptionFilterInitial)
                                    {
                                        strExceptionFilter += "(DAY_NO = 1 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_BELOW_MINUTES"].ToString() + ")";
                                    }
                                    else
                                    {
                                        strExceptionFilter += " OR (DAY_NO = 1 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_BELOW_MINUTES"].ToString() + ")";
                                    }
                                }
                                else
                                {
                                    //Remove the Day
                                    if (strExceptionFilter == strExceptionFilterInitial)
                                    {
                                        strExceptionFilter += "(DAY_NO = 1)";
                                    }
                                    else
                                    {
                                        strExceptionFilter += " OR (DAY_NO = 1)";
                                    }
                                }

                                if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_BELOW_MINUTES"]) > 0)
                                {
                                    if (strExceptionFilter == strExceptionFilterInitial)
                                    {
                                        strExceptionFilter += "(DAY_NO = 2 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_BELOW_MINUTES"].ToString() + ")";
                                    }
                                    else
                                    {
                                        strExceptionFilter += " OR (DAY_NO = 2 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_BELOW_MINUTES"].ToString() + ")";
                                    }
                                }
                                else
                                {
                                    //Remove the Day
                                    if (strExceptionFilter == strExceptionFilterInitial)
                                    {
                                        strExceptionFilter += "(DAY_NO = 2)";
                                    }
                                    else
                                    {
                                        strExceptionFilter += " OR (DAY_NO = 2)";
                                    }
                                }

                                if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_BELOW_MINUTES"]) > 0)
                                {
                                    if (strExceptionFilter == strExceptionFilterInitial)
                                    {
                                        strExceptionFilter += "(DAY_NO = 3 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_BELOW_MINUTES"].ToString() + ")";
                                    }
                                    else
                                    {
                                        strExceptionFilter += " OR (DAY_NO = 3 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_BELOW_MINUTES"].ToString() + ")";
                                    }
                                }
                                else
                                {
                                    //Remove the Day
                                    if (strExceptionFilter == strExceptionFilterInitial)
                                    {
                                        strExceptionFilter += "(DAY_NO = 3)";
                                    }
                                    else
                                    {
                                        strExceptionFilter += " OR (DAY_NO = 3)";
                                    }
                                }

                                if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_BELOW_MINUTES"]) > 0)
                                {
                                    if (strExceptionFilter == strExceptionFilterInitial)
                                    {
                                        strExceptionFilter += "(DAY_NO = 4 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_BELOW_MINUTES"].ToString() + ")";
                                    }
                                    else
                                    {
                                        strExceptionFilter += " OR (DAY_NO = 4 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_BELOW_MINUTES"].ToString() + ")";
                                    }
                                }
                                else
                                {
                                    //Remove the Day
                                    if (strExceptionFilter == strExceptionFilterInitial)
                                    {
                                        strExceptionFilter += "(DAY_NO = 4)";
                                    }
                                    else
                                    {
                                        strExceptionFilter += " OR (DAY_NO = 4)";
                                    }
                                }

                                if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_BELOW_MINUTES"]) > 0)
                                {
                                    if (strExceptionFilter == strExceptionFilterInitial)
                                    {
                                        strExceptionFilter += "(DAY_NO = 5 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_BELOW_MINUTES"].ToString() + ")";
                                    }
                                    else
                                    {
                                        strExceptionFilter += " OR (DAY_NO = 5 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_BELOW_MINUTES"].ToString() + ")";
                                    }
                                }
                                else
                                {
                                    //Remove the Day
                                    if (strExceptionFilter == strExceptionFilterInitial)
                                    {
                                        strExceptionFilter += "(DAY_NO = 5)";
                                    }
                                    else
                                    {
                                        strExceptionFilter += " OR (DAY_NO = 5)";
                                    }
                                }

                                if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_BELOW_MINUTES"]) > 0)
                                {
                                    if (strExceptionFilter == strExceptionFilterInitial)
                                    {
                                        strExceptionFilter += "(DAY_NO = 6 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_BELOW_MINUTES"].ToString() + ")";
                                    }
                                    else
                                    {
                                        strExceptionFilter += " OR (DAY_NO = 6 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES >= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_BELOW_MINUTES"].ToString() + ")";
                                    }
                                }
                                else
                                {
                                    //Remove the Day
                                    if (strExceptionFilter == strExceptionFilterInitial)
                                    {
                                        strExceptionFilter += "(DAY_NO = 6)";
                                    }
                                    else
                                    {
                                        strExceptionFilter += " OR (DAY_NO = 6)";
                                    }
                                }
                            }
                            else
                            {
                                if (parstrFilterType == "H")
                                {
                                    //Exceptions - High Side
                                    if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SUN_ABOVE_MINUTES"]) > 0)
                                    {
                                        strExceptionFilter += "(DAY_NO = 0 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SUN_ABOVE_MINUTES"].ToString() + ")";
                                    }
                                   
                                    if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_ABOVE_MINUTES"]) > 0)
                                    {
                                        if (strExceptionFilter == strExceptionFilterInitial)
                                        {
                                            strExceptionFilter += "(DAY_NO = 1 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_ABOVE_MINUTES"].ToString() + ")";
                                        }
                                        else
                                        {
                                            strExceptionFilter += " OR (DAY_NO = 1 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_MON_ABOVE_MINUTES"].ToString() + ")";
                                        }
                                    }
                                   
                                    if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_ABOVE_MINUTES"]) > 0)
                                    {
                                        if (strExceptionFilter == strExceptionFilterInitial)
                                        {
                                            strExceptionFilter += "(DAY_NO = 2 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_ABOVE_MINUTES"].ToString() + ")";
                                        }
                                        else
                                        {
                                            strExceptionFilter += " OR (DAY_NO = 2 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_TUE_ABOVE_MINUTES"].ToString() + ")";
                                        }
                                    }
                                   
                                    if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_ABOVE_MINUTES"]) > 0)
                                    {
                                        if (strExceptionFilter == strExceptionFilterInitial)
                                        {
                                            strExceptionFilter += "(DAY_NO = 3 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_ABOVE_MINUTES"].ToString() + ")";
                                        }
                                        else
                                        {
                                            strExceptionFilter += " OR (DAY_NO = 3 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_WED_ABOVE_MINUTES"].ToString() + ")";
                                        }
                                    }
                                   
                                    if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_ABOVE_MINUTES"]) > 0)
                                    {
                                        if (strExceptionFilter == strExceptionFilterInitial)
                                        {
                                            strExceptionFilter += "(DAY_NO = 4 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_ABOVE_MINUTES"].ToString() + ")";
                                        }
                                        else
                                        {
                                            strExceptionFilter += " OR (DAY_NO = 4 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_THU_ABOVE_MINUTES"].ToString() + ")";
                                        }
                                    }
                                   
                                    if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_ABOVE_MINUTES"]) > 0)
                                    {
                                        if (strExceptionFilter == strExceptionFilterInitial)
                                        {
                                            strExceptionFilter += "(DAY_NO = 5 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_ABOVE_MINUTES"].ToString() + ")";
                                        }
                                        else
                                        {
                                            strExceptionFilter += " OR (DAY_NO = 5 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_FRI_ABOVE_MINUTES"].ToString() + ")";
                                        }
                                    }
                                   
                                    if (Convert.ToInt16(DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_ABOVE_MINUTES"]) > 0)
                                    {
                                        if (strExceptionFilter == strExceptionFilterInitial)
                                        {
                                            strExceptionFilter += "(DAY_NO = 6 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_ABOVE_MINUTES"].ToString() + ")";
                                        }
                                        else
                                        {
                                            strExceptionFilter += " OR (DAY_NO = 6 AND TIMESHEET_TOTAL_AFTER_BREAK_ROUND_MINUTES <= " + DataSet.Tables["PayCategorySelected"].Rows[intRow]["EXCEPTION_SAT_ABOVE_MINUTES"].ToString() + ")";
                                        }
                                    }
                                }
                            }
                        }
                    }

                    strExceptionFilter += ")";

                    ReportTimeSheetDataView = new DataView(DataSet.Tables["ReportTimeSheet"],
                    strExceptionFilter,
                    "",
                    DataViewRowState.CurrentRows);

                    for (int intExceptioRow = 0; intExceptioRow < ReportTimeSheetDataView.Count; intExceptioRow++)
                    {
                        ReportTimeSheetDataView[intExceptioRow].Delete();
                        intExceptioRow -= 1;
                    }
                }
            }

            DataSet.Tables["ReportTimeSheet"].Columns.Remove("DAILY_ROUNDING_IND");
            DataSet.Tables["ReportTimeSheet"].Columns.Remove("DAILY_ROUNDING_MINUTES");
            DataSet.Tables["ReportTimeSheet"].Columns.Remove("PAY_CATEGORY_DESC");

            if (parstrPrintOrder == "C")
            {
                DataSet.Tables["ReportTimeSheet"].Columns.Remove("EMPLOYEE_CODE");
            }
            else
            {
                if (parstrPrintOrder == "S")
                {
                    DataSet.Tables["ReportTimeSheet"].Columns.Remove("EMPLOYEE_SURNAME");
                }
                else
                {
                    //Name
                    DataSet.Tables["ReportTimeSheet"].Columns.Remove("EMPLOYEE_NAME");
                }
            }

            DataSet.AcceptChanges();
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
	}
}
