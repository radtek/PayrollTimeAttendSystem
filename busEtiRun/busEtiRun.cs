using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace InteractPayroll
{
    public class busEtiRun
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busEtiRun()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parInt64CompanyNo, string parstrCurrentUserAccess, Int64 parint64CurrentUserNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();
            DataView EtiParametersDataView;

            DateTime dtCutoffDateTime = DateTime.ParseExact(DateTime.Now.AddMonths(-24).ToString("yyyy-MM-01"), "yyyy-MM-dd", null);
            DateTime dtFirstPeriodCutoffDateTime = DateTime.ParseExact(DateTime.Now.AddMonths(-12).ToString("yyyy-MM-01"), "yyyy-MM-dd", null);

            decimal dcmFirstPeriodHighValue = 0;
            decimal dcmSecondPeriodHighValue = 0;
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" MAX(ETI_HIGH_AMOUNT) AS MAX_ETI_AMOUNT");
         
            strQry.AppendLine(" FROM InteractPayroll.dbo.ETI_PARAMETERS");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EtiMaxAmount", parInt64CompanyNo);
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" EMPLOYEE_NO");
            strQry.AppendLine(",ETI_MONTH");
            strQry.AppendLine(",TOTAL_HOURS");
            strQry.AppendLine(",FACTOR");
            strQry.AppendLine(",TOTAL_EARNINGS");
            strQry.AppendLine(",ETI_EARNINGS");
            strQry.AppendLine(",ETI_VALUE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_ETI ");

            strQry.AppendLine(" WHERE COMPANY_NO = -1");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EtiEmployeeSave", parInt64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" ETI_TYPE");
            strQry.AppendLine(",ETI_HIGH_AMOUNT");
            strQry.AppendLine(",ETI_PERCENT");
            strQry.AppendLine(",ETI_VALUE");
            strQry.AppendLine(",ETI_PERCENT_CALCULATION");
            strQry.AppendLine(",ETI_VALUE_CALCULATION");
            strQry.AppendLine(",ETI_PREV_HIGH_AMOUNT_CALCULATION");

            strQry.AppendLine(" FROM InteractPayroll.dbo.ETI_PARAMETERS");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" ETI_TYPE");
            strQry.AppendLine(",ETI_HIGH_AMOUNT");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EtiParameters", parInt64CompanyNo);

            EtiParametersDataView = null;
            EtiParametersDataView = new DataView(DataSet.Tables["EtiParameters"],
            "ETI_TYPE = 1",
            "",
            DataViewRowState.CurrentRows);

            dcmFirstPeriodHighValue = Convert.ToDecimal(EtiParametersDataView[EtiParametersDataView.Count - 1]["ETI_HIGH_AMOUNT"]);

            EtiParametersDataView = null;
            EtiParametersDataView = new DataView(DataSet.Tables["EtiParameters"],
            "ETI_TYPE = 2",
            "",
            DataViewRowState.CurrentRows);

            dcmSecondPeriodHighValue = Convert.ToDecimal(EtiParametersDataView[EtiParametersDataView.Count - 1]["ETI_HIGH_AMOUNT"]);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" MAX(ETI_RUN_DATE) AS ETI_RUN_DATE");

            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_ETI ");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "EtiRunDate", parInt64CompanyNo);

            if (DataSet.Tables["EtiRunDate"].Rows[0]["ETI_RUN_DATE"] == System.DBNull.Value)
            {
                //Go Back 3 Months
                DateTime myDateTime = DateTime.ParseExact(DateTime.Now.AddMonths(-6).ToString("yyyy-MM-01"), "yyyy-MM-dd", null);

                DataSet.Tables["EtiRunDate"].Rows[0]["ETI_RUN_DATE"] = myDateTime;

                myDateTime = myDateTime.AddMonths(1);

                while (true)
                {
                    DataRow drDataRow = DataSet.Tables["EtiRunDate"].NewRow();

                    drDataRow["ETI_RUN_DATE"] = myDateTime;

                    DataSet.Tables["EtiRunDate"].Rows.Add(drDataRow);

                    if (DateTime.Now < myDateTime)
                    {
                        break;
                    }

                    myDateTime = myDateTime.AddMonths(1);
                }
            }
            else
            {
                //Add 1 to Month
                DateTime myDateTime = Convert.ToDateTime(DataSet.Tables["EtiRunDate"].Rows[0]["ETI_RUN_DATE"]).AddMonths(1);

                DataSet.Tables["EtiRunDate"].Rows[0]["ETI_RUN_DATE"] = myDateTime;

                myDateTime = myDateTime.AddMonths(1);

                while (true)
                {
                    DataRow drDataRow = DataSet.Tables["EtiRunDate"].NewRow();

                    drDataRow["ETI_RUN_DATE"] = myDateTime;

                    DataSet.Tables["EtiRunDate"].Rows.Add(drDataRow);

                    if (DateTime.Now < myDateTime)
                    {
                        break;
                    }

                    myDateTime = myDateTime.AddMonths(1);
                }
            }
            
            for (int intRow = 0; intRow < DataSet.Tables["EtiRunDate"].Rows.Count; intRow++)
            {
                DataSet DataSetTemp = new DataSet();

                strQry.Clear();
                strQry.AppendLine(" SELECT ");

                strQry.AppendLine(Convert.ToDateTime(DataSet.Tables["EtiRunDate"].Rows[intRow]["ETI_RUN_DATE"]).ToString("yyyyMMdd") + " AS ETI_RUN_DATE");

                strQry.AppendLine(",E.COMPANY_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");

                strQry.AppendLine(",E.EMPLOYEE_BIRTHDATE");

                strQry.AppendLine(",OLDER_THAN_EIGHTEEN = ");
                strQry.AppendLine(" CASE");
                strQry.AppendLine(" WHEN DATEADD(YY,18, E.EMPLOYEE_BIRTHDATE) < '" + Convert.ToDateTime(DataSet.Tables["EtiRunDate"].Rows[intRow]["ETI_RUN_DATE"]).AddMonths(1).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" THEN 'Y'");
                strQry.AppendLine(" ELSE 'N'");
                strQry.AppendLine(" END");

                strQry.AppendLine(",OLDER_THAN_THIRTY = ");
                strQry.AppendLine(" CASE");
                strQry.AppendLine(" WHEN DATEADD(YY,30, E.EMPLOYEE_BIRTHDATE) < '" + Convert.ToDateTime(DataSet.Tables["EtiRunDate"].Rows[intRow]["ETI_RUN_DATE"]).AddMonths(1).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" THEN 'Y'");
                strQry.AppendLine(" ELSE 'N'");
                strQry.AppendLine(" END");
                
                strQry.AppendLine(",E.ETI_START_DATE");
                strQry.AppendLine(",'Y' AS INCLUDED_IN_RUN");
                strQry.AppendLine(",'First' AS ETI_PERIOD");

                strQry.AppendLine(",SUM(EEH.HOURS_DECIMAL_ORIGINAL) AS TOTAL_HOURS");

                strQry.AppendLine(",CALC_FACTOR = ");

                strQry.AppendLine(" CASE");
                strQry.AppendLine(" WHEN SUM(EEH.HOURS_DECIMAL_ORIGINAL) < 160 ");
                strQry.AppendLine(" THEN ROUND((160 / SUM(EEH.HOURS_DECIMAL_ORIGINAL)),8) ");
                strQry.AppendLine(" ELSE 1");
                strQry.AppendLine(" END");

                strQry.AppendLine(",ETI_MONTH = DATEDIFF(mm, E.ETI_START_DATE, '" + Convert.ToDateTime(DataSet.Tables["EtiRunDate"].Rows[intRow]["ETI_RUN_DATE"]).ToString("yyyy-MM-dd") + "') + 1");

                strQry.AppendLine(",SUM(EEH.TOTAL) AS TOTAL_AMOUNT");
                strQry.AppendLine(",SUM(EEH.TOTAL) AS ETI_AMOUNT");
                strQry.AppendLine(",SUM(EEH.TOTAL) AS ETI_CALCULATED");

                strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E");

                strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_EARNING_HISTORY EEH");
                strQry.AppendLine(" ON E.COMPANY_NO = EEH.COMPANY_NO");
                strQry.AppendLine(" AND E.EMPLOYEE_NO = EEH.EMPLOYEE_NO");
                strQry.AppendLine(" AND EEH.RUN_TYPE = 'P'");
                strQry.AppendLine(" AND EEH.EARNING_NO >= 1");
                strQry.AppendLine(" AND EEH.EARNING_NO <= 5");

                strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE >= '" + Convert.ToDateTime(DataSet.Tables["EtiRunDate"].Rows[intRow]["ETI_RUN_DATE"]).ToString("yyyy-MM-dd") + "'");
                strQry.AppendLine(" AND EEH.PAY_PERIOD_DATE < '" + Convert.ToDateTime(DataSet.Tables["EtiRunDate"].Rows[intRow]["ETI_RUN_DATE"]).AddMonths(1).ToString("yyyy-MM-dd") + "'");

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
                strQry.AppendLine(" AND NOT E.ETI_START_DATE IS NULL");
                strQry.AppendLine(" AND E.ETI_START_DATE > '" + Convert.ToDateTime(DataSet.Tables["EtiRunDate"].Rows[intRow]["ETI_RUN_DATE"]).AddMonths(-26).ToString("yyyy-MM-dd") + "'");

                strQry.AppendLine(" GROUP BY ");
                strQry.AppendLine(" E.COMPANY_NO");
                strQry.AppendLine(",E.PAY_CATEGORY_TYPE");
                strQry.AppendLine(",E.EMPLOYEE_NO");
                strQry.AppendLine(",E.EMPLOYEE_CODE");
                strQry.AppendLine(",E.EMPLOYEE_NAME");
                strQry.AppendLine(",E.EMPLOYEE_SURNAME");
                strQry.AppendLine(",E.EMPLOYEE_BIRTHDATE");
                strQry.AppendLine(",E.ETI_START_DATE");

                clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parInt64CompanyNo);
            }

            for (int intRow = 0; intRow < DataSet.Tables["Employee"].Rows.Count; intRow++)
            {
                if (Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]) != 100)
                {
                    DataSet.Tables["Employee"].Rows[intRow]["ETI_AMOUNT"] = Math.Round(Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]) * Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["TOTAL_AMOUNT"]), 0);
                }
                else
                {
                    DataSet.Tables["Employee"].Rows[intRow]["ETI_AMOUNT"] = Math.Round(Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["TOTAL_AMOUNT"]), 0);
                }
                
                if (DataSet.Tables["Employee"].Rows[intRow]["OLDER_THAN_EIGHTEEN"].ToString() == "N")
                {
                    DataSet.Tables["Employee"].Rows[intRow]["INCLUDED_IN_RUN"] = "E";
                    DataSet.Tables["Employee"].Rows[intRow]["ETI_PERIOD"] = "";

                    DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = 0;

                    continue;
                }

                if (DataSet.Tables["Employee"].Rows[intRow]["OLDER_THAN_THIRTY"].ToString() == "Y")
                {
                    DataSet.Tables["Employee"].Rows[intRow]["INCLUDED_IN_RUN"] = "T";
                    DataSet.Tables["Employee"].Rows[intRow]["ETI_PERIOD"] = "";

                    DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = 0;

                    continue;
                }
                
                DateTime dtEtiRunDate = DateTime.ParseExact(DataSet.Tables["Employee"].Rows[intRow]["ETI_RUN_DATE"].ToString(), "yyyyMMdd", null);
                
                if (Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intRow]["ETI_START_DATE"]).AddMonths(24) <= dtEtiRunDate)
                {
                    DataSet.Tables["Employee"].Rows[intRow]["INCLUDED_IN_RUN"] = "N";
                    DataSet.Tables["Employee"].Rows[intRow]["ETI_PERIOD"] = "";

                    DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = 0;

                    continue;
                }
                else
                {
                    if (Convert.ToDateTime(DataSet.Tables["Employee"].Rows[intRow]["ETI_START_DATE"]).AddMonths(12) <= dtEtiRunDate)
                    {
                        DataSet.Tables["Employee"].Rows[intRow]["ETI_PERIOD"] = "Second";

                        EtiParametersDataView = null;
                        EtiParametersDataView = new DataView(DataSet.Tables["EtiParameters"],
                        "ETI_TYPE = 2 AND " + Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["ETI_AMOUNT"]).ToString("########0.00") + " <= ETI_HIGH_AMOUNT",
                        "",
                        DataViewRowState.CurrentRows);

                        if (EtiParametersDataView.Count > 0)
                        {
                            if (EtiParametersDataView[0]["ETI_PERCENT"] == System.DBNull.Value)
                            {
                                if (EtiParametersDataView[0]["ETI_VALUE"] == System.DBNull.Value)
                                {
                                    //Tested
                                    decimal dcmMonthlyPortion = Convert.ToDecimal(EtiParametersDataView[0]["ETI_PERCENT_CALCULATION"]) * (Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["ETI_AMOUNT"]) - Convert.ToDecimal(EtiParametersDataView[0]["ETI_PREV_HIGH_AMOUNT_CALCULATION"]));
                                    decimal dcmValueCalculated = Convert.ToDecimal(EtiParametersDataView[0]["ETI_VALUE_CALCULATION"]) - dcmMonthlyPortion;

                                    if (Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]) != 100)
                                    {
                                        DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = Math.Round(dcmValueCalculated / Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]), 0);
                                    }
                                    else
                                    {
                                        DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = Math.Round(dcmValueCalculated, 0);
                                    }
                                }
                                else
                                {
                                    //Tested
                                    if (Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]) != 100)
                                    {
                                        DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = Math.Round(Convert.ToDecimal(EtiParametersDataView[0]["ETI_VALUE"]) / Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]), 0);
                                    }
                                    else
                                    {
                                        DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = Convert.ToDecimal(EtiParametersDataView[0]["ETI_VALUE"]);
                                    }
                                }
                            }
                            else
                            {
                                //Tested
                                decimal dcmEtiValue = Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["ETI_AMOUNT"]) * Convert.ToDecimal(EtiParametersDataView[0]["ETI_PERCENT"]);

                                if (Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]) != 100)
                                {
                                    DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = Math.Round(dcmEtiValue / Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]), 0);
                                }
                                else
                                {
                                    DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = Math.Round(dcmEtiValue, 0);
                                }
                            }
                        }
                        else
                        {
                            DataSet.Tables["Employee"].Rows[intRow]["INCLUDED_IN_RUN"] = "H";
                            DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = 0;
                        }
                    }
                    else
                    {
                        EtiParametersDataView = null;
                        EtiParametersDataView = new DataView(DataSet.Tables["EtiParameters"],
                        "ETI_TYPE = 1 AND " + Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["ETI_AMOUNT"]).ToString("########0.00") + " <= ETI_HIGH_AMOUNT",
                        "",
                        DataViewRowState.CurrentRows);

                        if (EtiParametersDataView.Count > 0)
                        {
                            if (EtiParametersDataView[0]["ETI_PERCENT"] == System.DBNull.Value)
                            {
                                if (EtiParametersDataView[0]["ETI_VALUE"] == System.DBNull.Value)
                                {
                                    //Tested
                                    decimal dcmMonthlyPortion = Convert.ToDecimal(EtiParametersDataView[0]["ETI_PERCENT_CALCULATION"]) * (Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["ETI_AMOUNT"]) - Convert.ToDecimal(EtiParametersDataView[0]["ETI_PREV_HIGH_AMOUNT_CALCULATION"]));
                                    decimal dcmValueCalculated = Convert.ToDecimal(EtiParametersDataView[0]["ETI_VALUE_CALCULATION"]) - dcmMonthlyPortion;

                                    if (Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]) != 100)
                                    {
                                        DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = Math.Round(dcmValueCalculated / Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]), 0);
                                    }
                                    else
                                    {
                                        DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = Math.Round(dcmValueCalculated, 0);
                                    }
                                }
                                else
                                {
                                    //Tested
                                    if (Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]) != 100)
                                    {
                                        DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = Math.Round(Convert.ToDecimal(EtiParametersDataView[0]["ETI_VALUE"]) / Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]), 0);
                                    }
                                    else
                                    {
                                        DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = Convert.ToDecimal(EtiParametersDataView[0]["ETI_VALUE"]);
                                    }
                                }
                            }
                            else
                            {
                                //Tested
                                decimal dcmEtiValue = Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["ETI_AMOUNT"]) * Convert.ToDecimal(EtiParametersDataView[0]["ETI_PERCENT"]);

                                if (Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]) != 100)
                                {
                                    DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = Math.Round(dcmEtiValue / Convert.ToDecimal(DataSet.Tables["Employee"].Rows[intRow]["CALC_FACTOR"]), 0);
                                }
                                else
                                {
                                    DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = Math.Round(dcmEtiValue, 0);
                                }
                            }
                        }
                        else
                        {
                            DataSet.Tables["Employee"].Rows[intRow]["INCLUDED_IN_RUN"] = "H";
                            DataSet.Tables["Employee"].Rows[intRow]["ETI_CALCULATED"] = 0;
                        }
                    }
                }
            }

            DataSet.AcceptChanges();

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Insert_Eti_Records(Int64 parInt64CompanyNo, string parstrCurrentUserAccess, Int64 parint64CurrentUserNo, string parstrDateTime, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            StringBuilder strQry = new StringBuilder();

            for (int intRow = 0; intRow < parDataSet.Tables["EtiEmployeeSave"].Rows.Count; intRow++)
            {
                strQry.Clear();
            
                strQry.AppendLine(" INSERT INTO InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_ETI");

                strQry.AppendLine("(ETI_RUN_DATE");
                strQry.AppendLine(",COMPANY_NO");
                strQry.AppendLine(",EMPLOYEE_NO");
                strQry.AppendLine(",ETI_MONTH");
                strQry.AppendLine(",TOTAL_HOURS");
                strQry.AppendLine(",FACTOR");
                strQry.AppendLine(",TOTAL_EARNINGS");
                strQry.AppendLine(",ETI_EARNINGS");
                strQry.AppendLine(",ETI_VALUE)");

                strQry.AppendLine(" VALUES");

                strQry.AppendLine("('" + parstrDateTime + "'");
                strQry.AppendLine("," + parInt64CompanyNo);
                strQry.AppendLine("," + parDataSet.Tables["EtiEmployeeSave"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                strQry.AppendLine("," + parDataSet.Tables["EtiEmployeeSave"].Rows[intRow]["ETI_MONTH"].ToString());
                strQry.AppendLine("," + parDataSet.Tables["EtiEmployeeSave"].Rows[intRow]["TOTAL_HOURS"].ToString());
                strQry.AppendLine("," + parDataSet.Tables["EtiEmployeeSave"].Rows[intRow]["FACTOR"].ToString());
                strQry.AppendLine("," + parDataSet.Tables["EtiEmployeeSave"].Rows[intRow]["TOTAL_EARNINGS"].ToString());
                strQry.AppendLine("," + parDataSet.Tables["EtiEmployeeSave"].Rows[intRow]["ETI_EARNINGS"].ToString());
                strQry.AppendLine("," + parDataSet.Tables["EtiEmployeeSave"].Rows[intRow]["ETI_VALUE"].ToString() + ")");

                clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), parInt64CompanyNo);
            }

            strQry.Clear();

            strQry.AppendLine(" UPDATE InteractPayroll.dbo.COMPANY_LINK");
            strQry.AppendLine(" SET BACKUP_DB_IND = 1");
            strQry.AppendLine(" WHERE COMPANY_NO = " + parInt64CompanyNo);

            clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(), -1);

            byte[] bytCompress = Get_Form_Records(parInt64CompanyNo, parstrCurrentUserAccess, parint64CurrentUserNo);
            
            return bytCompress;
        }
    }
}
