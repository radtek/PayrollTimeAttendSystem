using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace InteractPayroll
{
    public class busEmployeeActivate
    {
        clsDBConnectionObjects clsDBConnectionObjects;
       
        public busEmployeeActivate()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
     
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_TAX_STARTDATE");
           
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EEC.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND EEC.EMPLOYEE_NO = E.EMPLOYEE_NO");
            strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE IN ('W','S') ");

            strQry.AppendLine(" AND EEC.RUN_TYPE = 'T'");
            strQry.AppendLine(" AND EEC.TOTAL <> 0");

            strQry.AppendLine(" UNION ");

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_TAX_STARTDATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");
            strQry.AppendLine(" ON EDC.COMPANY_NO = E.COMPANY_NO");
            strQry.AppendLine(" AND EDC.EMPLOYEE_NO = E.EMPLOYEE_NO");
            strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = E.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL");

            strQry.AppendLine(" WHERE EDC.COMPANY_NO = " + parint64CompanyNo);

            //2013-06-21 Exclude T=Time Attendance
            strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE IN ('W','S') ");

            strQry.AppendLine(" AND EDC.RUN_TYPE = 'T'");
            strQry.AppendLine(" AND EDC.TOTAL <> 0");
      
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            //Create Empty Table
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PAY_CATEGORY_TYPE AS PAYROLL_TYPE ");
            strQry.AppendLine(",PAY_PERIOD_DATE AS LAST_PAY_PERIOD_DATE ");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = -1");
            
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayrollType", parint64CompanyNo);
            
            DataView myDataView = new DataView(DataSet.Tables["Employee"],
                                               "PAY_CATEGORY_TYPE = 'W'",
                                               "",
                                               DataViewRowState.CurrentRows);

            if (myDataView.Count > 0)
            {
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" 'Wages' AS PAYROLL_TYPE ");
                strQry.AppendLine(",MAX(ISNULL(PCPH.PAY_PERIOD_DATE,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')) AS LAST_PAY_PERIOD_DATE ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH ");
                strQry.AppendLine(" ON EEC.COMPANY_NO = PCPH.COMPANY_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");

                strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = 'W'");
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'T'");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayrollType", parint64CompanyNo);
            }

            myDataView = null;
            myDataView = new DataView(DataSet.Tables["Employee"],
                                               "PAY_CATEGORY_TYPE = 'S'",
                                               "",
                                               DataViewRowState.CurrentRows);

            if (myDataView.Count > 0)
            {
                strQry.Clear();

                strQry.AppendLine(" SELECT ");
                strQry.AppendLine(" 'Salaries' AS PAYROLL_TYPE ");
                strQry.AppendLine(",MAX(ISNULL(PCPH.PAY_PERIOD_DATE,'" + DateTime.Now.ToString("yyyy-MM-dd") + "')) AS LAST_PAY_PERIOD_DATE ");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");

                strQry.AppendLine(" LEFT JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY_PERIOD_HISTORY PCPH ");
                strQry.AppendLine(" ON EEC.COMPANY_NO = PCPH.COMPANY_NO ");
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = PCPH.PAY_CATEGORY_TYPE");

                strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parint64CompanyNo);
                strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = 'S'");
                strQry.AppendLine(" AND EEC.RUN_TYPE = 'T'");

                strQry.AppendLine(" ORDER BY 1 DESC");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayrollType", parint64CompanyNo);
            }
          
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public int Backup_DataBase(Int64 parInt64CompanyNo,string parstrPayrollType,string parstrOption)
        {
            int intReturnCode = 9;

            try
            {
                StringBuilder strQry = new StringBuilder();

                DataSet pvtDataSet = new DataSet();

                strQry.Clear();

                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" BACKUP_DATABASE_PATH");
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_PATH");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), pvtDataSet, "Directory", -1);

                string strFileDirectory = pvtDataSet.Tables["Directory"].Rows[0]["BACKUP_DATABASE_PATH"].ToString();

                string strDataBaseName = "InteractPayroll_" + parInt64CompanyNo.ToString("00000");

                string strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_BeforeEmployeeActivation.bak";

                if (parstrOption == "A")
                {
                    strBackupFileName = strDataBaseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_AfterEmployeeActivation.bak";
                }

                strQry.Clear();

                strQry.AppendLine("BACKUP DATABASE " + strDataBaseName + " TO DISK = '" + strFileDirectory + "\\" + strBackupFileName + "' WITH CHECKSUM");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

                strQry.Clear();

                strQry.AppendLine("INSERT INTO InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");
                strQry.AppendLine("(BACKUP_DATABASE_NO");
                strQry.AppendLine(",PAY_CATEGORY_TYPE");
                strQry.AppendLine(",BACKUP_DATABASE_NAME");
                strQry.AppendLine(",PAYROLL_RUN_DATETIME");
                strQry.AppendLine(",BACKUP_DATETIME");
                strQry.AppendLine(",BACKUP_FILE_NAME)");
                strQry.AppendLine(" SELECT");
                strQry.AppendLine(" ISNULL(MAX(BACKUP_DATABASE_NO),0) + 1");
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
                strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(strDataBaseName));
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",GETDATE()");
                strQry.AppendLine(",'" + strFileDirectory + "\\" + strBackupFileName + "'");
                strQry.AppendLine(" FROM InteractPayroll.dbo.BACKUP_DATABASE_DATETIME");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
                            
                intReturnCode = 0;
            }
            catch (Exception ex)
            {
                string strStop = "";
            }

            return intReturnCode;
        }

        public void Activate_Employees(Int64 parint64CurrentUserNo, Int64 parint64CompanyNo, DateTime parDateTime, string parstrPayrollType, string parstrEmployeeNoIn)
        {
            StringBuilder strQry = new StringBuilder();
            StringBuilder strFieldNamesInitialised = new StringBuilder();
  
            //Delete All Blank Records
            strQry.Clear();

            strQry.AppendLine(" DELETE EEH ");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC");
            strQry.AppendLine(" ON EEH.COMPANY_NO = EEC.COMPANY_NO ");
            strQry.AppendLine(" AND EEH.EMPLOYEE_NO = EEC.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EEH.EARNING_NO = EEC.EARNING_NO ");
            //Maybe Employee has changed his Default PAY_CATEGORY 
            //strQry.AppendLine(" AND EEH.PAY_CATEGORY_NO = EEC.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = EEC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EEH.RUN_TYPE = EEC.RUN_TYPE ");
            strQry.AppendLine(" AND EEH.RUN_NO = EEC.RUN_NO ");

            //Only Records witrh Balnace
            strQry.AppendLine(" AND EEC.TOTAL <> 0 ");
            
            strQry.AppendLine(" WHERE EEH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EEH.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND EEH.RUN_TYPE = 'T' ");
            strQry.AppendLine(" AND EEH.EMPLOYEE_NO IN (" + parstrEmployeeNoIn + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY ");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_PERIOD_DATE");

            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",EARNING_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",RUN_TYPE");
            strQry.AppendLine(",RUN_NO");

            strQry.AppendLine(",TOTAL");
            
            //ELR 2014-05-24
            strQry.AppendLine(",EARNING_TYPE_IND");

            clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_EARNING_HISTORY", ref strQry, ref strFieldNamesInitialised, "EEC", parint64CompanyNo);

            strQry.AppendLine(")");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EEC.COMPANY_NO");
            //Get Initial Take-On Date from EMPLOYEE_PAY_CATEGORY_HISTORY
            strQry.AppendLine(",EPCH.PAY_PERIOD_DATE");
            strQry.AppendLine(",EEC.EMPLOYEE_NO");
            strQry.AppendLine(",EEC.EARNING_NO");
            //Get Initial PAY_CATEGORY_NOe from EMPLOYEE_PAY_CATEGORY_HISTORY
            strQry.AppendLine(",EPCH.PAY_CATEGORY_NO");
            strQry.AppendLine(",EEC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EEC.RUN_TYPE");
            strQry.AppendLine(",EEC.RUN_NO");

            strQry.AppendLine(",EEC.TOTAL");
            
            //ELR 2014-05-24
            strQry.AppendLine(",EEC.EARNING_TYPE_IND");

            //Append From Fields
            strQry.Append(strFieldNamesInitialised);

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT EEC ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY_HISTORY EPCH ");
            strQry.AppendLine(" ON EEC.COMPANY_NO = EPCH.COMPANY_NO ");
            strQry.AppendLine(" AND EEC.EMPLOYEE_NO = EPCH.EMPLOYEE_NO ");
            
             //Maybe Employee has changed his Default PAY_CATEGORY 
            //strQry.AppendLine(" AND EEC.PAY_CATEGORY_NO = EPCH.PAY_CATEGORY_NO ");
            strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = EPCH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EEC.RUN_TYPE = EPCH.RUN_TYPE ");
            strQry.AppendLine(" AND EEC.RUN_NO = EPCH.RUN_NO ");
            
            strQry.AppendLine(" WHERE EEC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EEC.EMPLOYEE_NO IN (" + parstrEmployeeNoIn + ")");
            strQry.AppendLine(" AND EEC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND EEC.RUN_TYPE = 'T' ");

            //Only Records witrh Balnace
            strQry.AppendLine(" AND EEC.TOTAL <> 0 ");
           
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" DELETE EDH");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY EDH");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC");
            strQry.AppendLine(" ON EDH.COMPANY_NO = EDC.COMPANY_NO ");
            strQry.AppendLine(" AND EDH.EMPLOYEE_NO = EDC.EMPLOYEE_NO ");

            strQry.AppendLine(" AND EDH.DEDUCTION_NO = EDC.DEDUCTION_NO ");
            strQry.AppendLine(" AND EDH.DEDUCTION_SUB_ACCOUNT_NO = EDC.DEDUCTION_SUB_ACCOUNT_NO ");
            strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = EDC.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EDH.RUN_TYPE = EDC.RUN_TYPE ");
            strQry.AppendLine(" AND EDH.RUN_NO = EDC.RUN_NO ");
             //Only Records witrh Balance
            strQry.AppendLine(" AND EDC.TOTAL <> 0 ");
            
            strQry.AppendLine(" WHERE EDH.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EDH.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND EDH.RUN_TYPE = 'T' ");
            strQry.AppendLine(" AND EDH.EMPLOYEE_NO IN (" + parstrEmployeeNoIn + ")");
            
            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
                       
            strQry.Clear();

            strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_HISTORY ");
            strQry.AppendLine("(COMPANY_NO");
            strQry.AppendLine(",PAY_PERIOD_DATE");
            
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",DEDUCTION_NO");
            strQry.AppendLine(",DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",RUN_TYPE");
            strQry.AppendLine(",RUN_NO");

            strQry.AppendLine(",TOTAL");

            clsDBConnectionObjects.Initialise_DataSet_Name_Fields("EMPLOYEE_DEDUCTION_CURRENT", ref strQry, ref strFieldNamesInitialised, "EDC", parint64CompanyNo);

            strQry.AppendLine(")");
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EDC.COMPANY_NO");
            //Get Initial Take-On Date from EMPLOYEE_INFO_HISTORY
            strQry.AppendLine(",EIH.PAY_PERIOD_DATE");

            strQry.AppendLine(",EDC.EMPLOYEE_NO");
            strQry.AppendLine(",EDC.DEDUCTION_NO");
            strQry.AppendLine(",EDC.DEDUCTION_SUB_ACCOUNT_NO");
            strQry.AppendLine(",EDC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",EDC.RUN_TYPE");
            strQry.AppendLine(",EDC.RUN_NO");

            strQry.AppendLine(",EDC.TOTAL");

            //Append From Fields
            strQry.Append(strFieldNamesInitialised);

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT EDC ");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_INFO_HISTORY EIH ");
            strQry.AppendLine(" ON EDC.COMPANY_NO = EIH.COMPANY_NO ");
            strQry.AppendLine(" AND EDC.EMPLOYEE_NO = EIH.EMPLOYEE_NO ");
            strQry.AppendLine(" AND EDC.RUN_TYPE = EIH.RUN_TYPE ");
            strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = EIH.PAY_CATEGORY_TYPE ");
            strQry.AppendLine(" AND EDC.RUN_NO = EIH.RUN_NO ");
            
            strQry.AppendLine(" WHERE EDC.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EDC.PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND EDC.RUN_TYPE = 'T' ");
            strQry.AppendLine(" AND EDC.EMPLOYEE_NO IN (" + parstrEmployeeNoIn + ")");

            //Only Records witrh Balance
            strQry.AppendLine(" AND EDC.TOTAL <> 0 ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            //Delete Records
            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_CURRENT");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND EMPLOYEE_NO IN (" + parstrEmployeeNoIn + ")");
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND RUN_TYPE = 'T' ");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_DEDUCTION_CURRENT");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND RUN_TYPE = 'T' ");
            strQry.AppendLine(" AND EMPLOYEE_NO IN (" + parstrEmployeeNoIn + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);
          
            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.EMPLOYEE");
            strQry.AppendLine(" SET EMPLOYEE_TAX_STARTDATE = '" + parDateTime.ToString("yyyy-MM-dd") + "'");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parstrPayrollType));
            strQry.AppendLine(" AND EMPLOYEE_NO IN (" + parstrEmployeeNoIn + ")");

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parint64CompanyNo);

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);
        }
    }
}
