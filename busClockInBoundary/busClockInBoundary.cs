using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using InteractPayroll;

namespace InteractPayrollClient
{
    public class busClockInBoundary
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busClockInBoundary()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(int parintCurrentUserNo, string parstrAccessInd)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();

            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" D.DEVICE_NO");
            strQry.AppendLine(",D.DEVICE_DESC");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE D");

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" D.DEVICE_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Clock");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" C.COMPANY_NO");
            strQry.AppendLine(",C.COMPANY_DESC");
            
            strQry.AppendLine(" FROM InteractPayrollClient.dbo.COMPANY C ");

            if (parstrAccessInd != "S")
            {
                strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.USER_COMPANY_ACCESS  UC ");
                strQry.AppendLine(" ON UC.USER_NO = " + parintCurrentUserNo);
                strQry.AppendLine(" AND UC.COMPANY_NO = C.COMPANY_NO ");
                
                //2013-07-10
                strQry.AppendLine(" AND UC.COMPANY_ACCESS_IND = 'A'");
            }

            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" C.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Company");

            if (DataSet.Tables["Company"].Rows.Count > 0)
            {
                byte[] bytTempCompress = Get_Company_Records(Convert.ToInt64(DataSet.Tables["Company"].Rows[0]["COMPANY_NO"]));

                DataSet TempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);
                DataSet.Merge(TempDataSet);
            }
           
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Company_Records(Int64 parint64CompanyNo)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = new DataSet();
            StringBuilder strQry = new StringBuilder();

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DPCL.DEVICE_NO");
            strQry.AppendLine(",PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY PC");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK DPCL");
            strQry.AppendLine(" ON PC.COMPANY_NO = DPCL.COMPANY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = DPCL.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = DPCL.PAY_CATEGORY_TYPE");
           
            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo.ToString());
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" DPCL.DEVICE_NO");
            strQry.AppendLine(",PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategory");
          
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DDL.DEVICE_NO");
            strQry.AppendLine(",PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",D.DEPARTMENT_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",D.DEPARTMENT_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.PAY_CATEGORY PC");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEPARTMENT D");
            strQry.AppendLine(" ON PC.COMPANY_NO = D.COMPANY_NO");
            //2017-02-13
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = D.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = D.PAY_CATEGORY_TYPE");
            
            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK DDL");
            strQry.AppendLine(" ON D.COMPANY_NO = DDL.COMPANY_NO");

            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO = DDL.PAY_CATEGORY_NO");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_TYPE = DDL.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND D.DEPARTMENT_NO = DDL.DEPARTMENT_NO");

            strQry.AppendLine(" WHERE PC.COMPANY_NO = " + parint64CompanyNo.ToString());
           
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" DDL.DEVICE_NO");
            strQry.AppendLine(",PC.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",D.DEPARTMENT_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategoryDepartment");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DGL.DEVICE_NO");
            strQry.AppendLine(",G.COMPANY_NO");
            strQry.AppendLine(",G.GROUP_DESC");
            strQry.AppendLine(",G.GROUP_NO");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.GROUPS G");

            strQry.AppendLine(" INNER JOIN InteractPayrollClient.dbo.DEVICE_GROUP_LINK DGL");
            strQry.AppendLine(" ON G.COMPANY_NO = DGL.COMPANY_NO");
            strQry.AppendLine(" AND G.GROUP_NO = DGL.GROUP_NO");
    
            strQry.AppendLine(" WHERE G.COMPANY_NO = " + parint64CompanyNo.ToString());
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" DGL.DEVICE_NO");
            strQry.AppendLine(",G.COMPANY_NO");
            strQry.AppendLine(",G.GROUP_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Group");
             
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DEVICE_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO");
            
            strQry.AppendLine(",DEVICE_PAY_CATEGORY_LINK_ACTIVE_DESC");

            strQry.AppendLine(",MON_CLOCK_IN");
            strQry.AppendLine(",TUE_CLOCK_IN");
            strQry.AppendLine(",WED_CLOCK_IN");
            strQry.AppendLine(",THU_CLOCK_IN");
            strQry.AppendLine(",FRI_CLOCK_IN");
            strQry.AppendLine(",SAT_CLOCK_IN");
            strQry.AppendLine(",SUN_CLOCK_IN");

            strQry.AppendLine(",MON_CLOCK_OUT");
            strQry.AppendLine(",TUE_CLOCK_OUT");
            strQry.AppendLine(",WED_CLOCK_OUT");
            strQry.AppendLine(",THU_CLOCK_OUT");
            strQry.AppendLine(",FRI_CLOCK_OUT");
            strQry.AppendLine(",SAT_CLOCK_OUT");
            strQry.AppendLine(",SUN_CLOCK_OUT");
 
            strQry.AppendLine(",MON_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",TUE_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",WED_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",THU_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",FRI_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",SAT_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",SUN_CLOCK_IN_APPLIES_IND");

            strQry.AppendLine(",TIME_ATTEND_ROUNDING_IND");
            strQry.AppendLine(",TIME_ATTEND_ROUNDING_VALUE");
 
            strQry.AppendLine(",ACTIVE_IND");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE ");
           
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" DEVICE_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEVICE_PAY_CATEGORY_LINK_ACTIVE_DESC");
            
            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategoryActive");

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DEVICE_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEPARTMENT_NO");
            strQry.AppendLine(",DEVICE_DEPARTMENT_LINK_ACTIVE_NO");
    
            strQry.AppendLine(",DEVICE_DEPARTMENT_LINK_ACTIVE_DESC");

            strQry.AppendLine(",MON_CLOCK_IN");
            strQry.AppendLine(",TUE_CLOCK_IN");
            strQry.AppendLine(",WED_CLOCK_IN");
            strQry.AppendLine(",THU_CLOCK_IN");
            strQry.AppendLine(",FRI_CLOCK_IN");
            strQry.AppendLine(",SAT_CLOCK_IN");
            strQry.AppendLine(",SUN_CLOCK_IN");

            strQry.AppendLine(",MON_CLOCK_OUT");
            strQry.AppendLine(",TUE_CLOCK_OUT");
            strQry.AppendLine(",WED_CLOCK_OUT");
            strQry.AppendLine(",THU_CLOCK_OUT");
            strQry.AppendLine(",FRI_CLOCK_OUT");
            strQry.AppendLine(",SAT_CLOCK_OUT");
            strQry.AppendLine(",SUN_CLOCK_OUT");
 
            strQry.AppendLine(",MON_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",TUE_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",WED_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",THU_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",FRI_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",SAT_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",SUN_CLOCK_IN_APPLIES_IND");

            strQry.AppendLine(",TIME_ATTEND_ROUNDING_IND");
            strQry.AppendLine(",TIME_ATTEND_ROUNDING_VALUE");
  
            strQry.AppendLine(",ACTIVE_IND");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE ");
           
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" DEVICE_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEPARTMENT_NO");
            strQry.AppendLine(",DEVICE_DEPARTMENT_LINK_ACTIVE_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "PayCategoryDepartmentActive");
            
            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" DEVICE_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",GROUP_NO");
            strQry.AppendLine(",DEVICE_GROUP_LINK_ACTIVE_NO");
    
            strQry.AppendLine(",DEVICE_GROUP_LINK_ACTIVE_DESC");
            
            strQry.AppendLine(",MON_CLOCK_IN");
            strQry.AppendLine(",TUE_CLOCK_IN");
            strQry.AppendLine(",WED_CLOCK_IN");
            strQry.AppendLine(",THU_CLOCK_IN");
            strQry.AppendLine(",FRI_CLOCK_IN");
            strQry.AppendLine(",SAT_CLOCK_IN");
            strQry.AppendLine(",SUN_CLOCK_IN");

            strQry.AppendLine(",MON_CLOCK_OUT");
            strQry.AppendLine(",TUE_CLOCK_OUT");
            strQry.AppendLine(",WED_CLOCK_OUT");
            strQry.AppendLine(",THU_CLOCK_OUT");
            strQry.AppendLine(",FRI_CLOCK_OUT");
            strQry.AppendLine(",SAT_CLOCK_OUT");
            strQry.AppendLine(",SUN_CLOCK_OUT");
 
            strQry.AppendLine(",MON_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",TUE_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",WED_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",THU_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",FRI_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",SAT_CLOCK_IN_APPLIES_IND");
            strQry.AppendLine(",SUN_CLOCK_IN_APPLIES_IND");

            strQry.AppendLine(",TIME_ATTEND_ROUNDING_IND");
            strQry.AppendLine(",TIME_ATTEND_ROUNDING_VALUE");
  
            strQry.AppendLine(",ACTIVE_IND");

            strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE ");
           
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo.ToString());
            
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" DEVICE_NO");
            strQry.AppendLine(",COMPANY_NO");
            strQry.AppendLine(",GROUP_NO");
            strQry.AppendLine(",DEVICE_GROUP_LINK_ACTIVE_DESC");

            clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "GroupActive");
           
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public void Delete_Shift(int parintLinkType, byte[] byteCompressedDataSet)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteCompressedDataSet);

            string strTableName = "";
            StringBuilder strQry = new StringBuilder();

            if (parintLinkType == 1)
            {
                strTableName = "PayCategoryActive";
            }
            else
            {
                if (parintLinkType == 2)
                {
                    strTableName = "PayCategoryDepartmentActive";
                }
                else
                {
                    strTableName = "GroupActive";
                }
            }
           
            if (parintLinkType == 1)
            {
                strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE");

                strQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                strQry.AppendLine(" AND COMPANY_NO = " + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_NO"].ToString());
                strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                strQry.AppendLine(" AND DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO"].ToString());
            }
            else
            {
                if (parintLinkType == 2)
                {
                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE");

                    strQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                    strQry.AppendLine(" AND COMPANY_NO = " + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND DEPARTMENT_NO = " + DataSet.Tables[strTableName].Rows[0]["DEPARTMENT_NO"].ToString());
                    strQry.AppendLine(" AND DEVICE_DEPARTMENT_LINK_ACTIVE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_DEPARTMENT_LINK_ACTIVE_NO"].ToString());
                }
                else
                {
                    strQry.AppendLine(" DELETE FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE");

                    strQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                    strQry.AppendLine(" AND COMPANY_NO = " + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND GROUP_NO = " + DataSet.Tables[strTableName].Rows[0]["GROUP_NO"].ToString());
                   
                    strQry.AppendLine(" AND DEVICE_GROUP_LINK_ACTIVE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_GROUP_LINK_ACTIVE_NO"].ToString());
                }
            }
       
            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());
        }

        public int Update_Link_Type(int parintLinkType, byte[] byteCompressedDataSet,bool parblnNew)
        {
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 
            //Stop Make Sure That Interface is Backward Compatible. Create New Function / Procedure if Parameter List Changes 

            DataSet DataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(byteCompressedDataSet);

            string strTableName = "";

            if (parintLinkType == 1)
            {
                strTableName = "PayCategoryActive";
            }
            else
            {
                if (parintLinkType == 2)
                {
                    strTableName = "PayCategoryDepartmentActive";
                }
                else
                {
                    strTableName = "GroupActive";
                }
            }
             
            int intReturnKey = 0;
            StringBuilder strQry = new StringBuilder();
            StringBuilder strUpdateQry = new StringBuilder();
            
            if (parintLinkType == 1)
            {
                if (parblnNew == true)
                {
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" ISNULL(MAX(DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO),0) + 1 AS  MAX_DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO");
                    
                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE");
                    
                    strQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                    strQry.AppendLine(" AND COMPANY_NO = " + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Temp");

                    intReturnKey = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO"]);

                    DataSet.Tables[strTableName].Rows[0]["DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO"] = intReturnKey; 

                    strQry.Clear();
                    strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_PAY_CATEGORY_LINK_ACTIVE");
                    strQry.AppendLine("(DEVICE_NO");
                    strQry.AppendLine(",COMPANY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO");
                    strQry.AppendLine(",DEVICE_PAY_CATEGORY_LINK_ACTIVE_DESC");
                    strQry.AppendLine(",MON_CLOCK_IN");
                    strQry.AppendLine(",TUE_CLOCK_IN");
                    strQry.AppendLine(",WED_CLOCK_IN");
                    strQry.AppendLine(",THU_CLOCK_IN");
                    strQry.AppendLine(",FRI_CLOCK_IN");
                    strQry.AppendLine(",SAT_CLOCK_IN");
                    strQry.AppendLine(",SUN_CLOCK_IN");
                    strQry.AppendLine(",MON_CLOCK_OUT");
                    strQry.AppendLine(",TUE_CLOCK_OUT");
                    strQry.AppendLine(",WED_CLOCK_OUT");
                    strQry.AppendLine(",THU_CLOCK_OUT");
                    strQry.AppendLine(",FRI_CLOCK_OUT");
                    strQry.AppendLine(",SAT_CLOCK_OUT");
                    strQry.AppendLine(",SUN_CLOCK_OUT");
                    strQry.AppendLine(",MON_CLOCK_IN_APPLIES_IND");
                    strQry.AppendLine(",TUE_CLOCK_IN_APPLIES_IND");
                    strQry.AppendLine(",WED_CLOCK_IN_APPLIES_IND");
                    strQry.AppendLine(",THU_CLOCK_IN_APPLIES_IND");
                    strQry.AppendLine(",FRI_CLOCK_IN_APPLIES_IND");
                    strQry.AppendLine(",SAT_CLOCK_IN_APPLIES_IND");
                    strQry.AppendLine(",SUN_CLOCK_IN_APPLIES_IND");
                    strQry.AppendLine(",TIME_ATTEND_ROUNDING_IND");
                    strQry.AppendLine(",TIME_ATTEND_ROUNDING_VALUE");
                    strQry.AppendLine(",ACTIVE_IND)");
                    strQry.AppendLine(" VALUES");

                    strQry.AppendLine("(" + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                    strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                    strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine("," + intReturnKey);

                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["DEVICE_PAY_CATEGORY_LINK_ACTIVE_DESC"].ToString()));

                    if (DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_OUT"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_OUT"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_OUT"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_OUT"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_OUT"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_OUT"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",NULL");
                    }
                    else
                    {
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_OUT"].ToString());
                    }

                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN_APPLIES_IND"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN_APPLIES_IND"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN_APPLIES_IND"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN_APPLIES_IND"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN_APPLIES_IND"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN_APPLIES_IND"].ToString()));
                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN_APPLIES_IND"].ToString()));

                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString()));
                    strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"].ToString());

                    strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["ACTIVE_IND"].ToString()) + ")");
                }
                else
                {
                    strQry.AppendLine(" UPDATE DEVICE_PAY_CATEGORY_LINK_ACTIVE");
                    strQry.AppendLine(" SET ");

                    strQry.AppendLine(" DEVICE_PAY_CATEGORY_LINK_ACTIVE_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["DEVICE_PAY_CATEGORY_LINK_ACTIVE_DESC"].ToString()));

                    if (DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",MON_CLOCK_IN = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",MON_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",TUE_CLOCK_IN = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",TUE_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",WED_CLOCK_IN = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",WED_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",THU_CLOCK_IN = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",THU_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",FRI_CLOCK_IN = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",FRI_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",SAT_CLOCK_IN = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",SAT_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN"].ToString() == "")
                    {
                        strQry.AppendLine(",SUN_CLOCK_IN = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",SUN_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN"].ToString());
                    }
                 
                    if (DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",MON_CLOCK_OUT = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",MON_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_OUT"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",TUE_CLOCK_OUT = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",TUE_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_OUT"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",WED_CLOCK_OUT = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",WED_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_OUT"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",THU_CLOCK_OUT = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",THU_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_OUT"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",FRI_CLOCK_OUT = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",FRI_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_OUT"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",SAT_CLOCK_OUT = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",SAT_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_OUT"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_OUT"].ToString() == "")
                    {
                        strQry.AppendLine(",SUN_CLOCK_OUT = NULL");
                    }
                    else
                    {
                        strQry.AppendLine(",SUN_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_OUT"].ToString());
                    }

                    strQry.AppendLine(",MON_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN_APPLIES_IND"].ToString()));
                    strQry.AppendLine(",TUE_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN_APPLIES_IND"].ToString()));
                    strQry.AppendLine(",WED_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN_APPLIES_IND"].ToString()));
                    strQry.AppendLine(",THU_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN_APPLIES_IND"].ToString()));
                    strQry.AppendLine(",FRI_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN_APPLIES_IND"].ToString()));
                    strQry.AppendLine(",SAT_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN_APPLIES_IND"].ToString()));
                    strQry.AppendLine(",SUN_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN_APPLIES_IND"].ToString()));

                    strQry.AppendLine(",TIME_ATTEND_ROUNDING_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString()));
                    strQry.AppendLine(",TIME_ATTEND_ROUNDING_VALUE = " + DataSet.Tables[strTableName].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"].ToString());

                    strQry.AppendLine(",ACTIVE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["ACTIVE_IND"].ToString()));
                   
                    strQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                    strQry.AppendLine(" AND COMPANY_NO = " + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_NO"].ToString());
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                    strQry.AppendLine(" AND DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO"].ToString());
                }

                if (DataSet.Tables[strTableName].Rows[0]["ACTIVE_IND"].ToString() == "Y")
                {
                    strUpdateQry.AppendLine(" UPDATE DEVICE_PAY_CATEGORY_LINK_ACTIVE");
                    strUpdateQry.AppendLine(" SET ACTIVE_IND = 'N'");
                    strUpdateQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                    strUpdateQry.AppendLine(" AND COMPANY_NO = " + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                    strUpdateQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_NO"].ToString());
                    strUpdateQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                    strUpdateQry.AppendLine(" AND DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO <> " + DataSet.Tables[strTableName].Rows[0]["DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO"].ToString());

                    clsDBConnectionObjects.Execute_SQLCommand_Client(strUpdateQry.ToString());
                }
            }
            else
            {
                if (parintLinkType == 2)
                {
                    if (parblnNew == true)
                    {
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" ISNULL(MAX(DEVICE_DEPARTMENT_LINK_ACTIVE_NO),0) + 1 AS  MAX_DEVICE_DEPARTMENT_LINK_ACTIVE_NO");
                        
                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE");
                        
                        strQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                        strQry.AppendLine(" AND COMPANY_NO = " + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine(" AND DEPARTMENT_NO = " + DataSet.Tables[strTableName].Rows[0]["DEPARTMENT_NO"].ToString());

                        clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Temp");

                        intReturnKey = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_DEVICE_DEPARTMENT_LINK_ACTIVE_NO"]);

                        DataSet.Tables[strTableName].Rows[0]["DEVICE_DEPARTMENT_LINK_ACTIVE_NO"] = intReturnKey; 

                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_DEPARTMENT_LINK_ACTIVE");
                        strQry.AppendLine("(DEVICE_NO");
                        strQry.AppendLine(",COMPANY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",DEPARTMENT_NO");
                        strQry.AppendLine(",DEVICE_DEPARTMENT_LINK_ACTIVE_NO");
                        strQry.AppendLine(",DEVICE_DEPARTMENT_LINK_ACTIVE_DESC");

                        strQry.AppendLine(",MON_CLOCK_IN");
                        strQry.AppendLine(",TUE_CLOCK_IN");
                        strQry.AppendLine(",WED_CLOCK_IN");
                        strQry.AppendLine(",THU_CLOCK_IN");
                        strQry.AppendLine(",FRI_CLOCK_IN");
                        strQry.AppendLine(",SAT_CLOCK_IN");
                        strQry.AppendLine(",SUN_CLOCK_IN");
                        strQry.AppendLine(",MON_CLOCK_OUT");
                        strQry.AppendLine(",TUE_CLOCK_OUT");
                        strQry.AppendLine(",WED_CLOCK_OUT");
                        strQry.AppendLine(",THU_CLOCK_OUT");
                        strQry.AppendLine(",FRI_CLOCK_OUT");
                        strQry.AppendLine(",SAT_CLOCK_OUT");
                        strQry.AppendLine(",SUN_CLOCK_OUT");
                        strQry.AppendLine(",MON_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",TUE_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",WED_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",THU_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",FRI_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",SAT_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",SUN_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",TIME_ATTEND_ROUNDING_VALUE");
                        strQry.AppendLine(",ACTIVE_IND)");
                        strQry.AppendLine(" VALUES");

                        strQry.AppendLine("(" + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["DEPARTMENT_NO"].ToString());

                        strQry.AppendLine("," + intReturnKey);

                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["DEVICE_DEPARTMENT_LINK_ACTIVE_DESC"].ToString()));

                        if (DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_OUT"].ToString());
                        }

                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN_APPLIES_IND"].ToString()));

                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString()));
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"].ToString());

                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["ACTIVE_IND"].ToString()) + ")");
                    }
                    else
                    {
                        strQry.AppendLine(" UPDATE DEVICE_DEPARTMENT_LINK_ACTIVE");
                        strQry.AppendLine(" SET ");

                        strQry.AppendLine(" DEVICE_DEPARTMENT_LINK_ACTIVE_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["DEVICE_DEPARTMENT_LINK_ACTIVE_DESC"].ToString()));

                        if (DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",MON_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",MON_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",TUE_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",TUE_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",WED_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",WED_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",THU_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",THU_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",FRI_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",FRI_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",SAT_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",SAT_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",SUN_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",SUN_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",MON_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",MON_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",TUE_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",TUE_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",WED_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",WED_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",THU_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",THU_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",FRI_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",FRI_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",SAT_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",SAT_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",SUN_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",SUN_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_OUT"].ToString());
                        }

                        strQry.AppendLine(",MON_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine(",TUE_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine(",WED_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine(",THU_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine(",FRI_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine(",SAT_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine(",SUN_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN_APPLIES_IND"].ToString()));

                        strQry.AppendLine(",TIME_ATTEND_ROUNDING_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString()));
                        strQry.AppendLine(",TIME_ATTEND_ROUNDING_VALUE = " + DataSet.Tables[strTableName].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"].ToString());

                        strQry.AppendLine(",ACTIVE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["ACTIVE_IND"].ToString()));

                        strQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                        strQry.AppendLine(" AND COMPANY_NO = " + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));

                        strQry.AppendLine(" AND DEPARTMENT_NO = " + DataSet.Tables[strTableName].Rows[0]["DEPARTMENT_NO"].ToString());


                        strQry.AppendLine(" AND DEVICE_DEPARTMENT_LINK_ACTIVE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_DEPARTMENT_LINK_ACTIVE_NO"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["ACTIVE_IND"].ToString() == "Y")
                    {
                        strUpdateQry.AppendLine(" UPDATE DEVICE_DEPARTMENT_LINK_ACTIVE");
                        strUpdateQry.AppendLine(" SET ACTIVE_IND = 'N'");
                        strUpdateQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                        strUpdateQry.AppendLine(" AND COMPANY_NO = " + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                        strUpdateQry.AppendLine(" AND PAY_CATEGORY_NO = " + DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_NO"].ToString());
                        strUpdateQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["PAY_CATEGORY_TYPE"].ToString()));
                        strUpdateQry.AppendLine(" AND DEPARTMENT_NO = " + DataSet.Tables[strTableName].Rows[0]["DEPARTMENT_NO"].ToString());
                        strUpdateQry.AppendLine(" AND DEVICE_DEPARTMENT_LINK_ACTIVE_NO <> " + DataSet.Tables[strTableName].Rows[0]["DEVICE_DEPARTMENT_LINK_ACTIVE_NO"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strUpdateQry.ToString());
                    }
                }
                else
                {
                    if (parblnNew == true)
                    {
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" ISNULL(MAX(DEVICE_GROUP_LINK_ACTIVE_NO),0) + 1 AS  MAX_DEVICE_GROUP_LINK_ACTIVE_NO");
                        
                        strQry.AppendLine(" FROM InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE");
                        
                        strQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                        strQry.AppendLine(" AND COMPANY_NO = " + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND GROUP_NO = " + DataSet.Tables[strTableName].Rows[0]["GROUP_NO"].ToString());
                       
                        clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Temp");

                        intReturnKey = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_DEVICE_GROUP_LINK_ACTIVE_NO"]);

                        DataSet.Tables[strTableName].Rows[0]["DEVICE_GROUP_LINK_ACTIVE_NO"] = intReturnKey;

                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayrollClient.dbo.DEVICE_GROUP_LINK_ACTIVE");
                        strQry.AppendLine("(DEVICE_NO");
                        strQry.AppendLine(",COMPANY_NO");
                        strQry.AppendLine(",GROUP_NO");
                        strQry.AppendLine(",DEVICE_GROUP_LINK_ACTIVE_NO");
                        strQry.AppendLine(",DEVICE_GROUP_LINK_ACTIVE_DESC");
                        strQry.AppendLine(",MON_CLOCK_IN");
                        strQry.AppendLine(",TUE_CLOCK_IN");
                        strQry.AppendLine(",WED_CLOCK_IN");
                        strQry.AppendLine(",THU_CLOCK_IN");
                        strQry.AppendLine(",FRI_CLOCK_IN");
                        strQry.AppendLine(",SAT_CLOCK_IN");
                        strQry.AppendLine(",SUN_CLOCK_IN");
                        strQry.AppendLine(",MON_CLOCK_OUT");
                        strQry.AppendLine(",TUE_CLOCK_OUT");
                        strQry.AppendLine(",WED_CLOCK_OUT");
                        strQry.AppendLine(",THU_CLOCK_OUT");
                        strQry.AppendLine(",FRI_CLOCK_OUT");
                        strQry.AppendLine(",SAT_CLOCK_OUT");
                        strQry.AppendLine(",SUN_CLOCK_OUT");
                        strQry.AppendLine(",MON_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",TUE_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",WED_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",THU_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",FRI_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",SAT_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",SUN_CLOCK_IN_APPLIES_IND");
                        strQry.AppendLine(",TIME_ATTEND_ROUNDING_IND");
                        strQry.AppendLine(",TIME_ATTEND_ROUNDING_VALUE");
                        strQry.AppendLine(",ACTIVE_IND)");
                        strQry.AppendLine(" VALUES");

                        strQry.AppendLine("(" + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["GROUP_NO"].ToString());
                     
                        strQry.AppendLine("," + intReturnKey);

                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["DEVICE_GROUP_LINK_ACTIVE_DESC"].ToString()));

                        if (DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",NULL");
                        }
                        else
                        {
                            strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_OUT"].ToString());
                        }

                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN_APPLIES_IND"].ToString()));

                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString()));
                        strQry.AppendLine("," + DataSet.Tables[strTableName].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"].ToString());

                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["ACTIVE_IND"].ToString()) + ")");
                    }
                    else
                    {
                        strQry.AppendLine(" UPDATE DEVICE_GROUP_LINK_ACTIVE");
                        strQry.AppendLine(" SET ");

                        strQry.AppendLine(" DEVICE_GROUP_LINK_ACTIVE_DESC = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["DEVICE_GROUP_LINK_ACTIVE_DESC"].ToString()));

                        if (DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",MON_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",MON_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",TUE_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",TUE_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",WED_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",WED_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",THU_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",THU_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",FRI_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",FRI_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",SAT_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",SAT_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN"].ToString() == "")
                        {
                            strQry.AppendLine(",SUN_CLOCK_IN = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",SUN_CLOCK_IN = " + DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",MON_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",MON_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",TUE_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",TUE_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",WED_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",WED_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",THU_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",THU_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",FRI_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",FRI_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",SAT_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",SAT_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_OUT"].ToString());
                        }

                        if (DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_OUT"].ToString() == "")
                        {
                            strQry.AppendLine(",SUN_CLOCK_OUT = NULL");
                        }
                        else
                        {
                            strQry.AppendLine(",SUN_CLOCK_OUT = " + DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_OUT"].ToString());
                        }

                        strQry.AppendLine(",MON_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["MON_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine(",TUE_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["TUE_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine(",WED_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["WED_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine(",THU_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["THU_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine(",FRI_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["FRI_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine(",SAT_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["SAT_CLOCK_IN_APPLIES_IND"].ToString()));
                        strQry.AppendLine(",SUN_CLOCK_IN_APPLIES_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["SUN_CLOCK_IN_APPLIES_IND"].ToString()));

                        strQry.AppendLine(",TIME_ATTEND_ROUNDING_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["TIME_ATTEND_ROUNDING_IND"].ToString()));
                        strQry.AppendLine(",TIME_ATTEND_ROUNDING_VALUE = " + DataSet.Tables[strTableName].Rows[0]["TIME_ATTEND_ROUNDING_VALUE"].ToString());

                        strQry.AppendLine(",ACTIVE_IND = " + clsDBConnectionObjects.Text2DynamicSQL(DataSet.Tables[strTableName].Rows[0]["ACTIVE_IND"].ToString()));

                        strQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                        strQry.AppendLine(" AND COMPANY_NO = " + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                        strQry.AppendLine(" AND GROUP_NO = " + DataSet.Tables[strTableName].Rows[0]["GROUP_NO"].ToString());
                        strQry.AppendLine(" AND DEVICE_GROUP_LINK_ACTIVE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_GROUP_LINK_ACTIVE_NO"].ToString());
                    }

                    if (DataSet.Tables[strTableName].Rows[0]["ACTIVE_IND"].ToString() == "Y")
                    {
                        strUpdateQry.AppendLine(" UPDATE DEVICE_GROUP_LINK_ACTIVE");
                        strUpdateQry.AppendLine(" SET ACTIVE_IND = 'N'");
                        strUpdateQry.AppendLine(" WHERE DEVICE_NO = " + DataSet.Tables[strTableName].Rows[0]["DEVICE_NO"].ToString());
                        strUpdateQry.AppendLine(" AND COMPANY_NO = " + DataSet.Tables[strTableName].Rows[0]["COMPANY_NO"].ToString());
                        strUpdateQry.AppendLine(" AND GROUP_NO = " + DataSet.Tables[strTableName].Rows[0]["GROUP_NO"].ToString());

                        strUpdateQry.AppendLine(" AND DEVICE_GROUP_LINK_ACTIVE_NO <> " + DataSet.Tables[strTableName].Rows[0]["DEVICE_GROUP_LINK_ACTIVE_NO"].ToString());

                        clsDBConnectionObjects.Execute_SQLCommand_Client(strUpdateQry.ToString());
                    }
                }
            }

            clsDBConnectionObjects.Execute_SQLCommand_Client(strQry.ToString());

            return intReturnKey;
        }
    }
}
