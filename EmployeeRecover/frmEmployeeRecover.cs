using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace InteractPayroll
{
    public partial class frmEmployeeRecover : Form
    {
        clsISUtilities clsISUtilities;
        clsDBConnectionObjects clsDBConnectionObjects;

        public frmEmployeeRecover()
        {
            InitializeComponent();
        }

        private void frmEmployeeRecover_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities();
                clsDBConnectionObjects = new InteractPayroll.clsDBConnectionObjects();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                
                string strNewURL = "";
#if(DEBUG)
#else
                strNewURL = this.txtIP1.Text + "." + this.txtIP2.Text + "." + this.txtIP3.Text + "." + this.txtIP4.Text;
#endif
                AppDomain.CurrentDomain.SetData("URLPath", strNewURL);

                clsISUtilities = null;
                clsISUtilities = new clsISUtilities(null, "busPayrollLogon");

                clsISUtilities.Set_WebService_Timeout_Value(15000);

                string strOk = (string)clsISUtilities.DynamicFunction("Ping", null);

                if (strOk == "OK")
                {
                    MessageBox.Show("Communication Successful.", "Communication", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    clsISUtilities = null;
                    clsISUtilities = new clsISUtilities(null, "busEmployeeRecover");

                    this.btnRunFix.Enabled = true;
                }
                else
                {
                    this.btnRunFix.Enabled = true;
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void btnRunFix_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder strQry = new StringBuilder();
                DataSet DataSet = new DataSet();

                //Money Mine (Johnny)
                Int64 Int64CompanyNo = 11;
               
                object[] objParm = new object[1];
                objParm[0] = Int64CompanyNo;

                bool blnAllow = (Boolean)clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                if (blnAllow == true)
                {
                    DataSet = null;
                    DataSet = new DataSet();

                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");
                    strQry.AppendLine(",EMPLOYEE_CODE");
                    strQry.AppendLine(",EMPLOYEE_NAME");
                    strQry.AppendLine(",EMPLOYEE_SURNAME");
                    strQry.AppendLine(",EMPLOYEE_LAST_RUNDATE");
                    strQry.AppendLine(",DEPARTMENT_NO");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE ");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "Employee");

                    strQry.Clear();
                    strQry.AppendLine(" SELECT ");
                    strQry.AppendLine(" COMPANY_NO");
                    strQry.AppendLine(",EMPLOYEE_NO");
                    strQry.AppendLine(",PAY_CATEGORY_NO");
                    strQry.AppendLine(",PAY_CATEGORY_TYPE");

                    strQry.AppendLine(" FROM InteractPayrollClient.dbo.EMPLOYEE_PAY_CATEGORY ");

                    clsDBConnectionObjects.Create_DataTable_Client(strQry.ToString(), DataSet, "EmployeePayCategory");

                    byte[] bytCompress = clsDBConnectionObjects.Compress_DataSet(DataSet);

                    objParm = new object[2];
                    objParm[0] = Int64CompanyNo;
                    objParm[1] = bytCompress;

                    clsISUtilities.DynamicFunction("Upload_Records", objParm);

                    MessageBox.Show("Upload Records Successful", "Communication", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("You are NOT allowed to Upload Records", "Communication", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception eException)
            {
                MessageBox.Show("Error = " + eException.Message);
                clsISUtilities.ErrorHandler(eException);
            }
        }
    }
}
