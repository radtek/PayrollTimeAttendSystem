using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace InteractPayroll
{
    public class busUserEmployeeLink
    {
        clsDBConnectionObjects clsDBConnectionObjects;

        public busUserEmployeeLink()
        {
            clsDBConnectionObjects = new clsDBConnectionObjects();
        }

        public byte[] Get_Form_Records(Int64 parint64CurrentUserNo, string parstrAccessInd)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            //DISTINCT Because bug caused Duplicates
            strQry.Clear();
            strQry.AppendLine(" SELECT DISTINCT");
            strQry.AppendLine(" U.USER_NO");
            strQry.AppendLine(",UCA.COMPANY_NO");
            strQry.AppendLine(",U.USER_ID");
            strQry.AppendLine(",U.FIRSTNAME");
            strQry.AppendLine(",U.SURNAME");
           
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.USER_ID U");
            strQry.AppendLine(",InteractPayroll.dbo.USER_COMPANY_ACCESS UCA");

            if (parstrAccessInd == "A")
            {
                strQry.AppendLine(",InteractPayroll.dbo.USER_COMPANY_ACCESS UCA1");
            }

            strQry.AppendLine(" WHERE UCA.COMPANY_ACCESS_IND <> 'A' ");
            strQry.AppendLine(" AND U.USER_NO = UCA.USER_NO");
            strQry.AppendLine(" AND U.USER_NO <> " + parint64CurrentUserNo);
            strQry.AppendLine(" AND U.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" AND UCA.DATETIME_DELETE_RECORD IS NULL ");

            if (parstrAccessInd == "A")
            {
                strQry.AppendLine(" AND UCA1.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND UCA1.COMPANY_NO = UCA.COMPANY_NO ");
                strQry.AppendLine(" AND UCA1.DATETIME_DELETE_RECORD IS NULL ");
            }

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" USER_ID");
                                
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "User",-1);

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" C.COMPANY_NO");
            strQry.AppendLine(",C.COMPANY_DESC");
            strQry.AppendLine(" FROM ");
            strQry.AppendLine(" InteractPayroll.dbo.COMPANY_LINK C");

            if (parstrAccessInd == "A")
            {
                strQry.AppendLine(",InteractPayroll.dbo.USER_COMPANY_ACCESS UCA");
           
                strQry.AppendLine(" WHERE UCA.COMPANY_ACCESS_IND = 'A' ");
                strQry.AppendLine(" AND UCA.USER_NO = " + parint64CurrentUserNo);
                strQry.AppendLine(" AND UCA.COMPANY_NO = C.COMPANY_NO ");
            }

            strQry.AppendLine(" ORDER BY");
            strQry.AppendLine(" C.COMPANY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Company",-1);

            if (DataSet.Tables["User"].Rows.Count > 0)
            {
                byte[] bytTempCompress = Get_Company_CostCentres_Departments_Employees(Convert.ToInt32(DataSet.Tables["User"].Rows[0]["COMPANY_NO"]));
                DataSet pvtTempDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(bytTempCompress);
                DataSet.Merge(pvtTempDataSet);
            }

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Get_Company_CostCentres_Departments_Employees(Int64 parint64CompanyNo)
        {
            StringBuilder strQry = new StringBuilder();
            DataSet DataSet = new DataSet();

            strQry.Clear();
            strQry.AppendLine(" SELECT");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(" FROM InteractPayroll.dbo.COMPANY_LINK C");

            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
           
            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "CompanyLoaded", -1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" E.COMPANY_NO");
            strQry.AppendLine(",E.EMPLOYEE_NO");
            strQry.AppendLine(",E.EMPLOYEE_CODE");
            strQry.AppendLine(",E.EMPLOYEE_SURNAME");
            strQry.AppendLine(",E.EMPLOYEE_NAME");
            strQry.AppendLine(",E.DEPARTMENT_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_NO");
            strQry.AppendLine(",EPC.PAY_CATEGORY_TYPE");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.EMPLOYEE E ");
            strQry.AppendLine(",InteractPayroll_#CompanyNo#.dbo.EMPLOYEE_PAY_CATEGORY EPC ");
            
            strQry.AppendLine(" WHERE E.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND E.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" AND E.EMPLOYEE_ENDDATE IS NULL");
            strQry.AppendLine(" AND E.COMPANY_NO = EPC.COMPANY_NO");
            strQry.AppendLine(" AND E.EMPLOYEE_NO = EPC.EMPLOYEE_NO");
            strQry.AppendLine(" AND E.PAY_CATEGORY_TYPE = EPC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(" AND EPC.DEFAULT_IND = 'Y'");
            strQry.AppendLine(" AND EPC.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" E.EMPLOYEE_CODE");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Employee", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",PAY_CATEGORY_DESC");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND PAY_CATEGORY_NO > 0");
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PAY_CATEGORY_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "PayCategory", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" D.COMPANY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_TYPE");
            strQry.AppendLine(",D.DEPARTMENT_NO");
            strQry.AppendLine(",PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",D.DEPARTMENT_DESC");
            
            strQry.AppendLine(" FROM InteractPayroll_#CompanyNo#.dbo.DEPARTMENT D");

            strQry.AppendLine(" INNER JOIN InteractPayroll_#CompanyNo#.dbo.PAY_CATEGORY PC ");
            strQry.AppendLine(" ON D.COMPANY_NO = PC.COMPANY_NO ");
            strQry.AppendLine(" AND PC.PAY_CATEGORY_NO > 0 ");
            strQry.AppendLine(" AND PC.DATETIME_DELETE_RECORD IS NULL ");

            strQry.AppendLine(" WHERE D.COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND D.DATETIME_DELETE_RECORD IS NULL ");
            strQry.AppendLine(" ORDER BY ");
            strQry.AppendLine(" PC.PAY_CATEGORY_DESC");
            strQry.AppendLine(",D.DEPARTMENT_DESC");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Department", parint64CompanyNo);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",USER_NO");
            strQry.AppendLine(",EMPLOYEE_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIE_BREAKER");
           
            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_EMPLOYEE ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserEmployee",-1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",USER_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",TIE_BREAKER");
            
            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_PAY_CATEGORY ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserPayCategory",-1);

            strQry.Clear();
            strQry.AppendLine(" SELECT ");
            strQry.AppendLine(" COMPANY_NO");
            strQry.AppendLine(",USER_NO");
            strQry.AppendLine(",PAY_CATEGORY_NO");
            strQry.AppendLine(",PAY_CATEGORY_TYPE");
            strQry.AppendLine(",DEPARTMENT_NO");
            strQry.AppendLine(",TIE_BREAKER");

            strQry.AppendLine(" FROM InteractPayroll.dbo.USER_PAY_CATEGORY_DEPARTMENT ");
            
            strQry.AppendLine(" WHERE COMPANY_NO = " + parint64CompanyNo);
            strQry.AppendLine(" AND DATETIME_DELETE_RECORD IS NULL ");

            clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "UserDepartment",-1);

            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;

            return bytCompress;
        }

        public byte[] Update_User_PayCategory_Employee(Int64 parint64CurrentUserNo, byte[] parbyteDataSet)
        {
            DataSet parDataSet = clsDBConnectionObjects.DeCompress_Array_To_DataSet(parbyteDataSet);
            DataSet DataSet = new DataSet();

            DataTable myDataTable = parDataSet.Tables["UserPayCategory"].Clone();
            DataSet.Tables.Add(myDataTable);

            myDataTable = parDataSet.Tables["UserDepartment"].Clone();
            DataSet.Tables.Add(myDataTable);

            myDataTable = parDataSet.Tables["UserEmployee"].Clone();
            DataSet.Tables.Add(myDataTable);

            StringBuilder strQry = new StringBuilder();
            int intTieBreaker = -1;
            object[] objAddOther = new object[6];
            object[] objAdd = new object[5];

            string[] strQryArray = new string[parDataSet.Tables["UserPayCategory"].Rows.Count + parDataSet.Tables["UserDepartment"].Rows.Count + parDataSet.Tables["UserEmployee"].Rows.Count];
            
            for (int intRow = 0; intRow < parDataSet.Tables["UserPayCategory"].Rows.Count; intRow++)
            {
                if (parDataSet.Tables["UserPayCategory"].Rows[intRow].RowState == DataRowState.Deleted)
                {
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_PAY_CATEGORY");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                    strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                    
                    strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt32(parDataSet.Tables["UserPayCategory"].Rows[intRow]["USER_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND COMPANY_NO = " + Convert.ToInt32(parDataSet.Tables["UserPayCategory"].Rows[intRow]["COMPANY_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["UserPayCategory"].Rows[intRow]["PAY_CATEGORY_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parDataSet.Tables["UserPayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE", DataRowVersion.Original].ToString() + "'");
                    strQry.AppendLine(" AND TIE_BREAKER = " + Convert.ToInt32(parDataSet.Tables["UserPayCategory"].Rows[intRow]["TIE_BREAKER", DataRowVersion.Original]));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);
                }
                else
                {
                    if (parDataSet.Tables["UserPayCategory"].Rows[intRow].RowState == DataRowState.Added)
                    {
                        if (DataSet.Tables["Temp"] != null)
                        {
                            DataSet.Tables.Remove("Temp");
                        }

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" MAX(TIE_BREAKER) AS MAX_NO");
                        
                        strQry.AppendLine(" FROM ");
                        strQry.AppendLine(" InteractPayroll.dbo.USER_PAY_CATEGORY");
                        
                        strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt32(parDataSet.Tables["UserPayCategory"].Rows[intRow]["USER_NO"]));
                        strQry.AppendLine(" AND COMPANY_NO = " + Convert.ToInt32(parDataSet.Tables["UserPayCategory"].Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["UserPayCategory"].Rows[intRow]["PAY_CATEGORY_NO"]));
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parDataSet.Tables["UserPayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp",-1);

                        if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
                        {
                            intTieBreaker = 1;
                        }
                        else
                        {
                            intTieBreaker = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
                        }

                        //Insert if Doesn't Exist
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.USER_PAY_CATEGORY");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",COMPANY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",TIE_BREAKER");
                        strQry.AppendLine(",DATETIME_NEW_RECORD");
                        strQry.AppendLine(",USER_NO_NEW_RECORD)");
                        
                        strQry.AppendLine(" VALUES ");
                        
                        strQry.AppendLine("(" + parDataSet.Tables["UserPayCategory"].Rows[intRow]["USER_NO"].ToString());
                        strQry.AppendLine("," + parDataSet.Tables["UserPayCategory"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine("," + parDataSet.Tables["UserPayCategory"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine(",'" + parDataSet.Tables["UserPayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                        strQry.AppendLine("," + intTieBreaker);
                        strQry.AppendLine(",GETDATE()");
                        strQry.AppendLine("," + parint64CurrentUserNo + ")");

                        objAdd[0] = Convert.ToInt32(parDataSet.Tables["UserPayCategory"].Rows[intRow]["COMPANY_NO"]);
                        objAdd[1] = Convert.ToInt32(parDataSet.Tables["UserPayCategory"].Rows[intRow]["USER_NO"]);
                        objAdd[2] = Convert.ToInt32(parDataSet.Tables["UserPayCategory"].Rows[intRow]["PAY_CATEGORY_NO"]);
                        objAdd[3] = parDataSet.Tables["UserPayCategory"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString();
                        objAdd[4] = intTieBreaker;

                        DataSet.Tables["UserPayCategory"].Rows.Add(objAdd);

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);
                    }
                }
            }

            for (int intRow = 0; intRow < parDataSet.Tables["UserDepartment"].Rows.Count; intRow++)
            {
                if (parDataSet.Tables["UserDepartment"].Rows[intRow].RowState == DataRowState.Deleted)
                {
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_PAY_CATEGORY_DEPARTMENT");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                    strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                    strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt32(parDataSet.Tables["UserDepartment"].Rows[intRow]["USER_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND COMPANY_NO = " + Convert.ToInt32(parDataSet.Tables["UserDepartment"].Rows[intRow]["COMPANY_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["UserDepartment"].Rows[intRow]["PAY_CATEGORY_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["UserDepartment"].Rows[intRow]["PAY_CATEGORY_TYPE", DataRowVersion.Original].ToString()));
                    strQry.AppendLine(" AND DEPARTMENT_NO = " + Convert.ToInt32(parDataSet.Tables["UserDepartment"].Rows[intRow]["DEPARTMENT_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND TIE_BREAKER = " + Convert.ToInt32(parDataSet.Tables["UserDepartment"].Rows[intRow]["TIE_BREAKER", DataRowVersion.Original]));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);
                }
                else
                {
                    if (parDataSet.Tables["UserDepartment"].Rows[intRow].RowState == DataRowState.Added)
                    {
                        if (DataSet.Tables["Temp"] != null)
                        {
                            DataSet.Tables.Remove("Temp");
                        }

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" MAX(TIE_BREAKER) AS MAX_NO");
                        
                        strQry.AppendLine(" FROM InteractPayroll.dbo.USER_PAY_CATEGORY_DEPARTMENT");
                        
                        strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt32(parDataSet.Tables["UserDepartment"].Rows[intRow]["USER_NO"]));
                        strQry.AppendLine(" AND COMPANY_NO = " + Convert.ToInt32(parDataSet.Tables["UserDepartment"].Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine(" AND PAY_CATEGORY_NO = " + Convert.ToInt32(parDataSet.Tables["UserDepartment"].Rows[intRow]["PAY_CATEGORY_NO"]));
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = " + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["UserDepartment"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine(" AND DEPARTMENT_NO = " + Convert.ToInt32(parDataSet.Tables["UserDepartment"].Rows[intRow]["DEPARTMENT_NO"]));

                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp",-1);

                        if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
                        {
                            intTieBreaker = 1;
                        }
                        else
                        {
                            intTieBreaker = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
                        }

                        //Insert if Doesn't Exist
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.USER_PAY_CATEGORY_DEPARTMENT");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",COMPANY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",DEPARTMENT_NO");
                        strQry.AppendLine(",TIE_BREAKER");
                        strQry.AppendLine(",DATETIME_NEW_RECORD");
                        strQry.AppendLine(",USER_NO_NEW_RECORD)");
                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("(" + parDataSet.Tables["UserDepartment"].Rows[intRow]["USER_NO"].ToString());
                        strQry.AppendLine("," + parDataSet.Tables["UserDepartment"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine("," + parDataSet.Tables["UserDepartment"].Rows[intRow]["PAY_CATEGORY_NO"].ToString());
                        strQry.AppendLine("," + clsDBConnectionObjects.Text2DynamicSQL(parDataSet.Tables["UserDepartment"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString()));
                        strQry.AppendLine("," + parDataSet.Tables["UserDepartment"].Rows[intRow]["DEPARTMENT_NO"].ToString());
                        strQry.AppendLine("," + intTieBreaker);
                        strQry.AppendLine(",GETDATE()");
                        strQry.AppendLine("," + parint64CurrentUserNo + ")");

                        objAddOther[0] = Convert.ToInt32(parDataSet.Tables["UserDepartment"].Rows[intRow]["COMPANY_NO"]);
                        objAddOther[1] = Convert.ToInt32(parDataSet.Tables["UserDepartment"].Rows[intRow]["USER_NO"]);
                        objAddOther[2] = Convert.ToInt32(parDataSet.Tables["UserDepartment"].Rows[intRow]["PAY_CATEGORY_NO"]);
                        objAddOther[3] = parDataSet.Tables["UserDepartment"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString();
                        objAddOther[4] = Convert.ToInt32(parDataSet.Tables["UserDepartment"].Rows[intRow]["DEPARTMENT_NO"]);
                        objAddOther[5] = intTieBreaker;

                        DataSet.Tables["UserDepartment"].Rows.Add(objAddOther);

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);
                    }
                }
            }

            for (int intRow = 0; intRow < parDataSet.Tables["UserEmployee"].Rows.Count; intRow++)
            {
                if (parDataSet.Tables["UserEmployee"].Rows[intRow].RowState == DataRowState.Deleted)
                {
                    strQry.Clear();
                    strQry.AppendLine(" UPDATE InteractPayroll.dbo.USER_EMPLOYEE");
                    strQry.AppendLine(" SET ");
                    strQry.AppendLine(" USER_NO_RECORD = " + parint64CurrentUserNo);
                    strQry.AppendLine(",DATETIME_DELETE_RECORD = GETDATE()");
                    
                    strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt32(parDataSet.Tables["UserEmployee"].Rows[intRow]["USER_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND COMPANY_NO = " + Convert.ToInt32(parDataSet.Tables["UserEmployee"].Rows[intRow]["COMPANY_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToInt32(parDataSet.Tables["UserEmployee"].Rows[intRow]["EMPLOYEE_NO", DataRowVersion.Original]));
                    strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parDataSet.Tables["UserEmployee"].Rows[intRow]["PAY_CATEGORY_TYPE", DataRowVersion.Original].ToString() + "'");
                    strQry.AppendLine(" AND TIE_BREAKER = " + Convert.ToInt32(parDataSet.Tables["UserEmployee"].Rows[intRow]["TIE_BREAKER", DataRowVersion.Original]));

                    clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);
                }
                else
                {
                    if (parDataSet.Tables["UserEmployee"].Rows[intRow].RowState == DataRowState.Added)
                    {
                        if (DataSet.Tables["Temp"] != null)
                        {
                            DataSet.Tables.Remove("Temp");
                        }

                        strQry.Clear();
                        strQry.AppendLine(" SELECT ");
                        strQry.AppendLine(" MAX(TIE_BREAKER) AS MAX_NO");
                        
                        strQry.AppendLine(" FROM ");
                        strQry.AppendLine(" InteractPayroll.dbo.USER_EMPLOYEE");
                        
                        strQry.AppendLine(" WHERE USER_NO = " + Convert.ToInt32(parDataSet.Tables["UserEmployee"].Rows[intRow]["USER_NO"]));
                        strQry.AppendLine(" AND COMPANY_NO = " + Convert.ToInt32(parDataSet.Tables["UserEmployee"].Rows[intRow]["COMPANY_NO"]));
                        strQry.AppendLine(" AND EMPLOYEE_NO = " + Convert.ToInt32(parDataSet.Tables["UserEmployee"].Rows[intRow]["EMPLOYEE_NO"]));
                        strQry.AppendLine(" AND PAY_CATEGORY_TYPE = '" + parDataSet.Tables["UserEmployee"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                        
                        clsDBConnectionObjects.Create_DataTable(strQry.ToString(), DataSet, "Temp",-1);

                        if (DataSet.Tables["Temp"].Rows[0].IsNull("MAX_NO") == true)
                        {
                            intTieBreaker = 1;
                        }
                        else
                        {
                            intTieBreaker = Convert.ToInt32(DataSet.Tables["Temp"].Rows[0]["MAX_NO"]) + 1;
                        }

                        //Insert if Doesn't Exist
                        strQry.Clear();
                        strQry.AppendLine(" INSERT INTO InteractPayroll.dbo.USER_EMPLOYEE");
                        strQry.AppendLine("(USER_NO");
                        strQry.AppendLine(",COMPANY_NO");
                        strQry.AppendLine(",EMPLOYEE_NO");
                        strQry.AppendLine(",PAY_CATEGORY_TYPE");
                        strQry.AppendLine(",TIE_BREAKER");
                        strQry.AppendLine(",DATETIME_NEW_RECORD");
                        strQry.AppendLine(",USER_NO_NEW_RECORD)");
                        strQry.AppendLine(" VALUES ");
                        strQry.AppendLine("(" + parDataSet.Tables["UserEmployee"].Rows[intRow]["USER_NO"].ToString());
                        strQry.AppendLine("," + parDataSet.Tables["UserEmployee"].Rows[intRow]["COMPANY_NO"].ToString());
                        strQry.AppendLine("," + parDataSet.Tables["UserEmployee"].Rows[intRow]["EMPLOYEE_NO"].ToString());
                        strQry.AppendLine(",'" + parDataSet.Tables["UserEmployee"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString() + "'");
                        strQry.AppendLine("," + intTieBreaker);
                        strQry.AppendLine(",GETDATE()");
                        strQry.AppendLine("," + parint64CurrentUserNo + ")");

                        objAdd[0] = Convert.ToInt32(parDataSet.Tables["UserEmployee"].Rows[intRow]["COMPANY_NO"]);
                        objAdd[1] = Convert.ToInt32(parDataSet.Tables["UserEmployee"].Rows[intRow]["USER_NO"]);
                        objAdd[2] = Convert.ToInt32(parDataSet.Tables["UserEmployee"].Rows[intRow]["EMPLOYEE_NO"]);
                        objAdd[3] = parDataSet.Tables["UserEmployee"].Rows[intRow]["PAY_CATEGORY_TYPE"].ToString();
                        objAdd[4] = intTieBreaker;

                        DataSet.Tables["UserEmployee"].Rows.Add(objAdd);

                        clsDBConnectionObjects.Execute_SQLCommand(strQry.ToString(),-1);
                    }
                }
            }
   
            byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);
            DataSet.Dispose();
            DataSet = null;
            parDataSet.Dispose();
            parDataSet = null;

            return bytCompress;
        }
    }
}
