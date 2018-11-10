using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace InteractPayroll
{
    public class busRptEti
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busRptEti()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo, Int64 parint64CurrentUserNo, string parstrCurrentUserAccess)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETI_RUN_DATE");
          
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_ETI ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" ETI_RUN_DATE");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" ETI_RUN_DATE DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EtiRunDate", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EETI.ETI_RUN_DATE");
            strQry.AppendLine(",EETI.EMPLOYEE_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_ETI EETI ");

            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND EETI.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND EETI.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
            }

            strQry.AppendLine(" WHERE EETI.COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EtiEmployee", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NO");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_ETI EETI ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EETI.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND EETI.EMPLOYEE_NO = E.EMPLOYEE_NO");
            
            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND E.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
            }

            strQry.AppendLine(" WHERE EETI.COMPANY_NO = " + parint64CompanyNo);

            strQry.AppendLine(" GROUP BY ");
            strQry.AppendLine(" E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NO");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Print_EtiReport(Int64 parint64CurrentUserNo, string parstrCurrentUserAccess, Int64 parint64CompanyNo, string parstrEtiRunDate, string parstrEmployeeNoIN, string parstrPrintOrderInd)
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

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");

            strQry.AppendLine("," + parstrEtiRunDate.Replace("-","").Substring(0,6) + " AS ETI_RUN_MONTH");

            strQry.AppendLine(",EETI.ETI_MONTH");
            strQry.AppendLine(",EETI.ETI_EARNINGS");
            strQry.AppendLine(",EETI.ETI_VALUE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_ETI EETI ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EETI.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND EETI.EMPLOYEE_NO = E.EMPLOYEE_NO");

            if (parstrCurrentUserAccess == "U")
            {
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.USER_EMPLOYEE_PAY_CATEGORY_TEMP UEPCT");
                strQry.AppendLine(" ON UEPCT.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND E.COMPANY_NO = UEPCT.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = UEPCT.EMPLOYEE_NO");
            }

            strQry.AppendLine(" WHERE EETI.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EETI.ETI_RUN_DATE = '" + parstrEtiRunDate + "'");

            if (parstrEmployeeNoIN != "")
            {
                strQry.AppendLine(" AND EETI.EMPLOYEE_NO IN (" + parstrEmployeeNoIN + ")");
            }

            strQry.AppendLine(" ORDER BY ");

            if (parstrPrintOrderInd == "C")
            {
                strQry.AppendLine(" E.EMPLOYEE_CODE");
            }
            else
            {
                if (parstrPrintOrderInd == "S")
                {
                    strQry.AppendLine(" E.EMPLOYEE_SURNAME");
                }
                else
                {
                    strQry.AppendLine(" E.EMPLOYEE_NAME");
                }
            }

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Report", parint64CompanyNo);

            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }
    }
}
