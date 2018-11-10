using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busNormalSickLeave
    {
        clsDBConnectionObjects clsDBConnectionObjects;
      
        public busNormalSickLeave()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo,string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            
            if (parstrCurrentUserAccess == "S")
            {
                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" COMPANY_NO");
                strQry.AppendLine(",COMPANY_DESC");
                strQry.AppendLine(",'A' AS ACCESS_IND");
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY");
                strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" ORDER BY");
                strQry.AppendLine(" COMPANY_DESC");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parint64CompanyNo);
            }
            else
            {
                //Administrator
                strQry.Clear();
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" C.COMPANY_NO");
                strQry.AppendLine(",C.COMPANY_DESC");
                strQry.AppendLine(",'A' AS ACCESS_IND");
                strQry.AppendLine(" FROM ");
                strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.COMPANY C");
                strQry.AppendLine(",InteractPayroll.dbo.USER_COMPANY_ACCESS UCA ");
                strQry.AppendLine(" WHERE C.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND C.COMPANY_NO = UCA.COMPANY_NO ");
                strQry.AppendLine(" AND C.DATETIME_DELETE_RECORD IS NULL");
                strQry.AppendLine(" AND UCA.USER_NO = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" AND UCA.COMPANY_ACCESS_IND = 'A' ");
                strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" ORDER BY");
                strQry.AppendLine(" C.COMPANY_DESC");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company", parint64CompanyNo);
            }

            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT ");
            strQry.AppendLine(" LS.COMPANY_NO");
            strQry.AppendLine(",LS.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LS.LEAVE_SHIFT_NO");
            strQry.AppendLine(",LS.LEAVE_SHIFT_DESC");
            strQry.AppendLine(",LS.MIN_VALID_SHIFT_MINUTES");
            strQry.AppendLine(",LS.MAX_SHIFTS_YEAR");
            strQry.AppendLine(",LS.NORM_PAID_DAYS");
            strQry.AppendLine(",LS.SICK_PAID_DAYS");
            strQry.AppendLine(",LS.NORM_PAID_PER_PERIOD");
            strQry.AppendLine(",LS.SICK_PAID_PER_PERIOD");
            strQry.AppendLine(",LS.LEAVE_PAID_ACCUMULATOR_IND");
            strQry.AppendLine(",LS.LEAVE_SHIFT_DEL_IND");
            strQry.AppendLine(",LSC.LEAVE_SHIFT_NO AS PAYROLL_LINK");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT LS ");
            
            strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT LSC");
            strQry.AppendLine(" ON LS.COMPANY_NO = LSC.COMPANY_NO ");
            strQry.AppendLine(" AND LS.PAY_CATEGORY_TYPE = LSC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND LS.LEAVE_SHIFT_NO = LSC.LEAVE_SHIFT_NO ");
            
            strQry.AppendLine(" WHERE LS.COMPANY_NO = " + parint64CompanyNo);
            //Record has Not been Deleted
            strQry.AppendLine(" AND LS.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" LS.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LS.LEAVE_SHIFT_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Leave", parint64CompanyNo);
            
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet = null;
            return bytCompress;
        }

        public int Insert_New_Record(Int64 parInt64CompanyNo,Int64 parint64CurrentUserNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
            int intLeaveNo = -1;
           
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" MAX(LEAVE_SHIFT_NO) AS MAX_NO");
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables[0].Rows[0]["COMPANY_NO"].ToString());

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp", parInt64CompanyNo);

            if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
            {
                intLeaveNo = 1;
            }
            else
            {
                intLeaveNo = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
            }

            DataSet.Dispose();
            DataSet = null;

            strQry.Clear();
            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",LEAVE_SHIFT_NO");
            strQry.AppendLine(",LEAVE_SHIFT_DESC");
            strQry.AppendLine(",MIN_VALID_SHIFT_MINUTES");
            strQry.AppendLine(",MAX_SHIFTS_YEAR");
            strQry.AppendLine(",NORM_PAID_DAYS");
            strQry.AppendLine(",SICK_PAID_DAYS");
            strQry.AppendLine(",NORM_PAID_PER_PERIOD");
            strQry.AppendLine(",SICK_PAID_PER_PERIOD");
            strQry.AppendLine(",LEAVE_PAID_ACCUMULATOR_IND");
            strQry.AppendLine(",DATETIME_NEW_RECORD");
            strQry.AppendLine(",USER_NO_NEW_RECORD");
            strQry.AppendLine(",LEAVE_SHIFT_DEL_IND)");
            strQry.AppendLine(" VALUES ");
            strQry.AppendLine("(" + parDataSet.Tables[0].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine("," + intLeaveNo);
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["LEAVE_SHIFT_DESC"].ToString()));
            strQry.AppendLine("," + parDataSet.Tables[0].Rows[0]["MIN_VALID_SHIFT_MINUTES"].ToString());
            strQry.AppendLine("," + parDataSet.Tables[0].Rows[0]["MAX_SHIFTS_YEAR"].ToString());
            strQry.AppendLine("," + parDataSet.Tables[0].Rows[0]["NORM_PAID_DAYS"].ToString());
            strQry.AppendLine("," + parDataSet.Tables[0].Rows[0]["SICK_PAID_DAYS"].ToString());
            strQry.AppendLine("," + parDataSet.Tables[0].Rows[0]["NORM_PAID_PER_PERIOD"].ToString());
            strQry.AppendLine("," + parDataSet.Tables[0].Rows[0]["SICK_PAID_PER_PERIOD"].ToString());
            strQry.AppendLine("," + parDataSet.Tables[0].Rows[0]["LEAVE_PAID_ACCUMULATOR_IND"].ToString());
            strQry.AppendLine(",GETDATE()");
            strQry.AppendLine("," + parint64CurrentUserNo);
            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["LEAVE_SHIFT_DEL_IND"].ToString()) + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

            parDataSet.Dispose();
            parDataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return intLeaveNo;
        }
        
        public int Update_Record(Int64 parInt64CompanyNo,Int64 parint64CurrentUserNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);

            StringBuilder strQry = new StringBuilder();
            int intReturnCode = 0;
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" LEAVE_SHIFT_NO ");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT_CURRENT ");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables[0].Rows[0]["COMPANY_NO"].ToString());
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
            strQry.AppendLine(" AND LEAVE_SHIFT_NO = " + parDataSet.Tables[0].Rows[0]["LEAVE_SHIFT_NO"].ToString());

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "LeaveShift", parInt64CompanyNo);

            if (DataSet.Tables["LeaveShift"].Rows.Count == 0)
            {
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT");
                strQry.AppendLine(" SET ");
                strQry.AppendLine(" LEAVE_SHIFT_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["LEAVE_SHIFT_DESC"].ToString()));
                strQry.AppendLine(",MIN_VALID_SHIFT_MINUTES = " + parDataSet.Tables[0].Rows[0]["MIN_VALID_SHIFT_MINUTES"].ToString());
                strQry.AppendLine(",MAX_SHIFTS_YEAR = " + parDataSet.Tables[0].Rows[0]["MAX_SHIFTS_YEAR"].ToString());
                strQry.AppendLine(",NORM_PAID_DAYS = " + parDataSet.Tables[0].Rows[0]["NORM_PAID_DAYS"].ToString());
                strQry.AppendLine(",SICK_PAID_DAYS = " + parDataSet.Tables[0].Rows[0]["SICK_PAID_DAYS"].ToString());
                strQry.AppendLine(",NORM_PAID_PER_PERIOD = " + parDataSet.Tables[0].Rows[0]["NORM_PAID_PER_PERIOD"].ToString());
                strQry.AppendLine(",SICK_PAID_PER_PERIOD = " + parDataSet.Tables[0].Rows[0]["SICK_PAID_PER_PERIOD"].ToString());
                strQry.AppendLine(",LEAVE_PAID_ACCUMULATOR_IND = " + parDataSet.Tables[0].Rows[0]["LEAVE_PAID_ACCUMULATOR_IND"].ToString());
                strQry.AppendLine(",USER_NO_RECORD = " + parint64CurrentUserNo.ToString());
                strQry.AppendLine(" WHERE COMPANY_NO = " + parDataSet.Tables[0].Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine(" AND LEAVE_SHIFT_NO = " + parDataSet.Tables[0].Rows[0]["LEAVE_SHIFT_NO"].ToString());
                strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                
                strQry.Clear();
                strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY");

                strQry.AppendLine(" SET LEAVE_DAY_RATE_DECIMAL = TEMP_TABLE.LEAVE_DAY_RATE_DECIMAL");

                strQry.AppendLine(" FROM ");

                strQry.AppendLine("(SELECT");
                strQry.AppendLine(" EPC.COMPANY_NO ");
                strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(",EPC.TIE_BREAKER ");

                strQry.AppendLine(",ROUND(SUM(PCTD.TIME_DECIMAL) / COUNT(*),2) AS LEAVE_DAY_RATE_DECIMAL");
         
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC");
              
                strQry.AppendLine(" INNER JOIN  InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
                strQry.AppendLine(" ON EPC.COMPANY_NO = E.COMPANY_NO ");
                strQry.AppendLine(" AND EPC.EMPLOYEE_NO = E.EMPLOYEE_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = " + parDataSet.Tables[0].Rows[0]["LEAVE_SHIFT_NO"].ToString());
                strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_TIME_DECIMAL PCTD");
                strQry.AppendLine(" ON EPC.COMPANY_NO = PCTD.COMPANY_NO ");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_NO = PCTD.PAY_CATEGORY_NO ");
 
                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT LS ");
                strQry.AppendLine(" ON EPC.COMPANY_NO = LS.COMPANY_NO");
                strQry.AppendLine(" AND E.LEAVE_SHIFT_NO = LS.LEAVE_SHIFT_NO");
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = LS.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND (((LS.LEAVE_PAID_ACCUMULATOR_IND = 1");
                strQry.AppendLine(" AND PCTD.DAY_NO IN (1,2,3,4,5))");
                //Saturday Included
                strQry.AppendLine(" OR (LS.LEAVE_PAID_ACCUMULATOR_IND = 2");
                strQry.AppendLine(" AND PCTD.DAY_NO IN (1,2,3,4,5,6)))");
                //Sunday Included
                strQry.AppendLine(" OR (LS.LEAVE_PAID_ACCUMULATOR_IND = 3");
                strQry.AppendLine(" AND PCTD.DAY_NO IN (0,1,2,3,4,5,6)))");

                strQry.AppendLine(" AND LS.DATETIME_DELETE_RECORD IS NULL");
                
                strQry.AppendLine(" WHERE EPC.COMPANY_NO = " + parDataSet.Tables[0].Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND EPC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables[0].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                //Leave Related
                strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y' ");
                strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");

                strQry.AppendLine(" GROUP BY");
                strQry.AppendLine(" EPC.COMPANY_NO ");
                strQry.AppendLine(",EPC.EMPLOYEE_NO ");
                strQry.AppendLine(",EPC.PAY_CATEGORY_NO ");
                strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE ");
                strQry.AppendLine(",EPC.TIE_BREAKER) AS TEMP_TABLE ");

                strQry.AppendLine(" WHERE EMPLOYEE_PAY_CATEGORY.COMPANY_NO = TEMP_TABLE.COMPANY_NO");
                strQry.AppendLine(" AND EMPLOYEE_PAY_CATEGORY.EMPLOYEE_NO = TEMP_TABLE.EMPLOYEE_NO");
                strQry.AppendLine(" AND EMPLOYEE_PAY_CATEGORY.PAY_CATEGORY_NO = TEMP_TABLE.PAY_CATEGORY_NO");
                strQry.AppendLine(" AND EMPLOYEE_PAY_CATEGORY.PAY_CATEGORY_TYPE = TEMP_TABLE.PAY_CATEGORY_TYPE");
                strQry.AppendLine(" AND EMPLOYEE_PAY_CATEGORY.TIE_BREAKER = TEMP_TABLE.TIE_BREAKER");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }
            else
            {
                intReturnCode = 9999;
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            DataSet.Dispose();
            DataSet = null;
            parDataSet.Dispose();
            parDataSet = null;

            return intReturnCode;
        }

        public void Delete_Record(Int64 parint64CurrentUserNo, Int64 parint64CompanyNo, int parintLeaveNo, string parstrPayrollType)
        {
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");
            
            strQry.AppendLine(" SET LEAVE_SHIFT_NO = 0");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND LEAVE_SHIFT_NO = " + parintLeaveNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.LEAVE_SHIFT");
            strQry.AppendLine(" SET ");
            strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo.ToString());
            strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE() ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND LEAVE_SHIFT_NO = " + parintLeaveNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
 }
