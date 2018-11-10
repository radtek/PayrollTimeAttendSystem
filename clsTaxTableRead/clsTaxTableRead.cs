using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class clsTaxTableRead
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public clsTaxTableRead()
        {
        }
        
        public DataSet Get_Tax_UIF_Tables(int intTaxYearEnd)
        {
            //ELR - Changed 2014-04-26
            int intPrevTaxYearEnd = intTaxYearEnd - 1;
            
            clsDBConnectionObjects = new clsDBConnectionObjects();

            string strQry = "";
            DataSet DataSet = new DataSet();

            strQry = "";
            strQry += " SELECT TOP 1";
            strQry += " UIF_PERCENTAGE";
            strQry += ",UIF_YEAR_AMOUNT";
            strQry += " FROM ";
            strQry += " InteractPayroll.dbo.UIF_THRESHOLD";

            strQry += " ORDER BY ";
            strQry += " DATETIME_UPATED DESC ";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "UIF", -1);

            strQry = "";
            strQry += " SELECT ";
            strQry += " TAX_YEAR_END";
            strQry += ",TAX_REBATE_TYPE";
            strQry += ",TAX_REBATE_VALUE";
            
            strQry += " FROM InteractPayroll.dbo.TAX_REBATE";

            strQry += " WHERE TAX_YEAR_END = " + intTaxYearEnd;

            strQry += " UNION ";
            
            //Get Previous Years If Current Year Does NOT Exist
            strQry += " SELECT ";
            strQry += " TR1.TAX_YEAR_END";
            strQry += ",TR1.TAX_REBATE_TYPE";
            strQry += ",TR1.TAX_REBATE_VALUE";

            strQry += " FROM InteractPayroll.dbo.TAX_REBATE TR1";

            strQry += " LEFT JOIN ";

            strQry += "(SELECT ";
            strQry += " TAX_YEAR_END";

            strQry += " FROM InteractPayroll.dbo.TAX_REBATE";
            strQry += " WHERE TAX_YEAR_END = " + intTaxYearEnd + ") AS TEMP_TABLE";

            strQry += " ON TEMP_TABLE.TAX_YEAR_END = TEMP_TABLE.TAX_YEAR_END";
           
            strQry += " WHERE TR1.TAX_YEAR_END = " + intPrevTaxYearEnd;
            strQry += " AND TEMP_TABLE.TAX_YEAR_END IS NULL";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "TaxRebate", -1);

            strQry = "";
            strQry += " SELECT ";
            strQry += " TAX_YEAR_END";
            strQry += ",TAX_CASUAL_PERCENT";
            
            strQry += " FROM InteractPayroll.dbo.TAX_CASUAL";

            strQry += " WHERE TAX_YEAR_END = " + intTaxYearEnd;

            strQry += " UNION ";

            //Get Previous Years If Current Year Does NOT Exist
            strQry += " SELECT ";
            strQry += " TC1.TAX_YEAR_END";
            strQry += ",TC1.TAX_CASUAL_PERCENT";

            strQry += " FROM InteractPayroll.dbo.TAX_CASUAL TC1";

            strQry += " LEFT JOIN ";

            strQry += "(SELECT ";
            strQry += " TAX_YEAR_END";

            strQry += " FROM InteractPayroll.dbo.TAX_CASUAL";
            strQry += " WHERE TAX_YEAR_END = " + intTaxYearEnd + ") AS TEMP_TABLE";

            strQry += " ON TEMP_TABLE.TAX_YEAR_END = TEMP_TABLE.TAX_YEAR_END";

            strQry += " WHERE TC1.TAX_YEAR_END = " + intPrevTaxYearEnd;
            strQry += " AND TEMP_TABLE.TAX_YEAR_END IS NULL";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "TaxCasual", -1);

            strQry = "";
            strQry += " SELECT ";
            strQry += " TAX_YEAR_END";
            strQry += ",TAX_DEDUCTION_NO";
            strQry += ",TAX_DEDUCTION_PERCENTAGE";
            strQry += ",TAX_DEDUCTION_FIXED_AMOUNT";
            strQry += ",TAX_DEDUCTION_PENSION_AMOUNT";
            strQry += ",TAX_DEDUCTION_AGE";
            strQry += ",TAX_DEDUCTION_ARREAR_AMOUNT";
            
            strQry += " FROM InteractPayroll.dbo.TAX_DEDUCTION_VALUES";

            strQry += " WHERE TAX_YEAR_END = " + intTaxYearEnd;

            strQry += " UNION ";

            //Get Previous Years If Current Year Does NOT Exist
            strQry += " SELECT ";
            strQry += " TDV1.TAX_YEAR_END";
            strQry += ",TDV1.TAX_DEDUCTION_NO";
            strQry += ",TDV1.TAX_DEDUCTION_PERCENTAGE";
            strQry += ",TDV1.TAX_DEDUCTION_FIXED_AMOUNT";
            strQry += ",TDV1.TAX_DEDUCTION_PENSION_AMOUNT";
            strQry += ",TDV1.TAX_DEDUCTION_AGE";
            strQry += ",TDV1.TAX_DEDUCTION_ARREAR_AMOUNT";

            strQry += " FROM InteractPayroll.dbo.TAX_DEDUCTION_VALUES TDV1";

            strQry += " LEFT JOIN ";

            strQry += "(SELECT ";
            strQry += " TAX_YEAR_END";

            strQry += " FROM InteractPayroll.dbo.TAX_DEDUCTION_VALUES";
            strQry += " WHERE TAX_YEAR_END = " + intTaxYearEnd + ") AS TEMP_TABLE";

            strQry += " ON TEMP_TABLE.TAX_YEAR_END = TEMP_TABLE.TAX_YEAR_END";

            strQry += " WHERE TDV1.TAX_YEAR_END = " + intPrevTaxYearEnd;
            strQry += " AND TEMP_TABLE.TAX_YEAR_END IS NULL";
       
            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "TaxDeduction", -1);

            strQry = "";
            strQry += " SELECT ";
            strQry += " TAX_YEAR_END";
            strQry += ",TAX_VALUE_RANGE";
            strQry += ",TAX_FIXED_AMOUNT";
            strQry += ",TAX_PERCENTAGE";
            strQry += ",' ' AS HIDDEN_CELL ";
            
            strQry += " FROM InteractPayroll.dbo.TAX";

            strQry += " WHERE TAX_YEAR_END = " + intTaxYearEnd;

            strQry += " UNION ";

            //Get Previous Years If Current Year Does NOT Exist
            strQry += " SELECT ";
            strQry += " T1.TAX_YEAR_END";
            strQry += ",T1.TAX_VALUE_RANGE";
            strQry += ",T1.TAX_FIXED_AMOUNT";
            strQry += ",T1.TAX_PERCENTAGE";
            strQry += ",' ' AS HIDDEN_CELL ";

            strQry += " FROM InteractPayroll.dbo.TAX T1";
            strQry += " LEFT JOIN ";

            strQry += "(SELECT ";
            strQry += " TAX_YEAR_END";

            strQry += " FROM InteractPayroll.dbo.TAX";
            strQry += " WHERE TAX_YEAR_END = " + intTaxYearEnd + ") AS TEMP_TABLE";

            strQry += " ON TEMP_TABLE.TAX_YEAR_END = TEMP_TABLE.TAX_YEAR_END";

            strQry += " WHERE T1.TAX_YEAR_END = " + intPrevTaxYearEnd;
            strQry += " AND TEMP_TABLE.TAX_YEAR_END IS NULL";
            
            strQry += " ORDER BY ";
            strQry += " 1";
            strQry += ",2";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "Tax", -1);

            strQry = "";
            strQry += " SELECT ";
            strQry += " TAX_YEAR_END";
            strQry += ",MAJOR_AMOUNT";
            strQry += ",MINOR_AMOUNT";
            
            strQry += " FROM InteractPayroll.dbo.TAX_MEDICAL_AID_CAPPED_AMOUNT";

            strQry += " WHERE TAX_YEAR_END = " + intTaxYearEnd;

            strQry += " UNION ";

            //Get Previous Years If Current Year Does NOT Exist
            strQry += " SELECT ";
            strQry += " T1.TAX_YEAR_END";
            strQry += ",T1.MAJOR_AMOUNT";
            strQry += ",T1.MINOR_AMOUNT";
           
            strQry += " FROM InteractPayroll.dbo.TAX_MEDICAL_AID_CAPPED_AMOUNT T1";
            strQry += " LEFT JOIN ";

            strQry += "(SELECT ";
            strQry += " TAX_YEAR_END";

            strQry += " FROM InteractPayroll.dbo.TAX_MEDICAL_AID_CAPPED_AMOUNT";
            strQry += " WHERE TAX_YEAR_END = " + intTaxYearEnd + ") AS TEMP_TABLE";

            strQry += " ON TEMP_TABLE.TAX_YEAR_END = TEMP_TABLE.TAX_YEAR_END";

            strQry += " WHERE T1.TAX_YEAR_END = " + intPrevTaxYearEnd;
            strQry += " AND TEMP_TABLE.TAX_YEAR_END IS NULL";
            
            strQry += " ORDER BY ";
            strQry += " 1";
            strQry += ",2";

            clsDBConnectionObjects.Create_DataTable(strQry, DataSet, "TaxMedicalCap", -1);

            return DataSet;
        }
    }
}
