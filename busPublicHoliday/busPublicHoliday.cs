using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busPublicHoliday
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busPublicHoliday()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo, string parstrFromProgram)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();

            if (parInt64CompanyNo != 999999)
            {
                strQry.AppendLine(" SELECT ");

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" 'T' AS RUN_IND");
                }
                else
                {
                    strQry.AppendLine(" 'W' AS RUN_IND");
                }

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

                strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                if (parstrFromProgram == "X")
                {
                    strQry.AppendLine(" AND (TIME_ATTENDANCE_RUN_IND = 'Y'");
                    strQry.AppendLine(" OR TIME_ATTENDANCE_RUN_IND = 'N')");
                }
                else
                {
                    strQry.AppendLine(" AND WAGE_RUN_IND = 'Y'");

                    strQry.AppendLine(" UNION");

                    strQry.AppendLine(" SELECT");

                    strQry.AppendLine(" 'S' AS RUN_IND");

                    strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.COMPANY ");

                    strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

                    strQry.AppendLine(" AND SALARY_RUN_IND = 'Y'");
                }

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CurrentRunInd", parInt64CompanyNo);
            }
   
            strQry.Clear();

            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" PUBLIC_HOLIDAY_NO");
            strQry.AppendLine(",PUBLIC_HOLIDAY_DATE");
            strQry.AppendLine(",PUBLIC_HOLIDAY_DESC");

            if (parInt64CompanyNo == 999999)
            {
                strQry.AppendLine(" FROM InteractPayroll.dbo.PUBLIC_HOLIDAY ");
            }
            else
            {
                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY ");
            }

            strQry.AppendLine(" WHERE PUBLIC_HOLIDAY_DATE >= '" + DateTime.Now.ToString("yyyy") + "-01-01'");
            strQry.AppendLine(" ORDER BY PUBLIC_HOLIDAY_DATE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PaidHoliday", parInt64CompanyNo);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Update_Records(Int64 parInt64CompanyNo, byte[] parbyteDataSet, string parstrFromProgram)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            DataSet DataSet = new DataSet();
            
            StringBuilder strQry = new StringBuilder();

            if (parInt64CompanyNo == 999999)
            {
                 strQry.AppendLine(" SELECT ");
                 strQry.AppendLine(" COMPANY_NO");
                 
                 strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK ");

                 clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Companies", -1);
            }

            strQry.Clear();

            for (int intRow = 0; intRow < parDataSet.Tables["PaidHoliday"].Rows.Count; intRow++)
            {
                if (parDataSet.Tables["PaidHoliday"].Rows[intRow].RowState == DataRowState.Added)
                {
                    strQry.Clear();

                    if (parInt64CompanyNo == 999999)
                    {
                        strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.PUBLIC_HOLIDAY");
                    }
                    else
                    {
                        strQry.AppendLine(" INSERT INTO  InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY ");
                    }

                    strQry.AppendLine("(PUBLIC_HOLIDAY_DATE");
                    strQry.AppendLine(",PUBLIC_HOLIDAY_DESC)");
                    strQry.AppendLine(" VALUES ");
                    strQry.AppendLine("('" + Convert.ToDateTime(parDataSet.Tables["PaidHoliday"].Rows[intRow]["PUBLIC_HOLIDAY_DATE"]).ToString("yyyy-MM-dd") + "'");
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PaidHoliday"].Rows[intRow]["PUBLIC_HOLIDAY_DESC"].ToString()) + ")");

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);

                    if (parInt64CompanyNo == 999999)
                    {
                        for (int intCompanyRow = 0; intCompanyRow < DataSet.Tables["Companies"].Rows.Count; intCompanyRow++)
                        {
                            Int64 Int64CompanyNo = Convert.ToInt64(DataSet.Tables["Companies"].Rows[intCompanyRow]["COMPANY_NO"]);

                            strQry.Clear();
                            
                            strQry.AppendLine(" INSERT INTO  InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY ");

                            strQry.AppendLine("(PUBLIC_HOLIDAY_DATE");
                            strQry.AppendLine(",PUBLIC_HOLIDAY_DESC)");
                            strQry.AppendLine(" VALUES ");
                            strQry.AppendLine("('" + Convert.ToDateTime(parDataSet.Tables["PaidHoliday"].Rows[intRow]["PUBLIC_HOLIDAY_DATE"]).ToString("yyyy-MM-dd") + "'");
                            strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PaidHoliday"].Rows[intRow]["PUBLIC_HOLIDAY_DESC"].ToString()) + ")");

                            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), Int64CompanyNo);
                        }
                    }
                }
                else
                {
                    if (parDataSet.Tables["PaidHoliday"].Rows[intRow].RowState == DataRowState.Modified)
                    {
                        strQry.Clear();

                        if (parInt64CompanyNo == 999999)
                        {
                            strQry.AppendLine(" UPDATE InteractPayroll.dbo.PUBLIC_HOLIDAY");
                        }
                        else
                        {
                            strQry.AppendLine(" UPDATE InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY");
                        }

                        strQry.AppendLine(" SET ");
                        strQry.AppendLine(" PUBLIC_HOLIDAY_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["PaidHoliday"].Rows[intRow]["PUBLIC_HOLIDAY_DESC"].ToString()));
                        strQry.AppendLine(",PUBLIC_HOLIDAY_DATE = '" + Convert.ToDateTime(parDataSet.Tables["PaidHoliday"].Rows[intRow]["PUBLIC_HOLIDAY_DATE"]).ToString("yyyy-MM-dd") + "'");
                        strQry.AppendLine(" WHERE PUBLIC_HOLIDAY_NO = " + parDataSet.Tables["PaidHoliday"].Rows[intRow]["PUBLIC_HOLIDAY_NO"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    }
                    else
                    {
                        //Deleted
                        strQry.Clear();

                        if (parInt64CompanyNo == 999999)
                        {
                            strQry.AppendLine(" DELETE FROM InteractPayroll.dbo.PUBLIC_HOLIDAY");
                        }
                        else
                        {
                            strQry.AppendLine(" DELETE FROM InteractPayroll_#CompanyNo#.dbo.PUBLIC_HOLIDAY");
                        }

                        strQry.AppendLine(" WHERE PUBLIC_HOLIDAY_NO = " + parDataSet.Tables["PaidHoliday"].Rows[intRow]["PUBLIC_HOLIDAY_NO", System.Data.DataRowVersion.Original].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
                    }
                }
            }

            byte[] bytCompress = Get_Form_Records(parInt64CompanyNo, parstrFromProgram);
            DataSet.Dispose();
            DataSet = null;

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            return bytCompress;
        }
    }
}
