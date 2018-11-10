using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InteractPayroll
{
    public partial class frmRptEarningsDeductionsHorizontalPageSelection : Form
    {
        frmRptEarningsDeductionsSpreadSheet frmRptEarningsDeductionsSpreadSheet;
        clsISUtilities clsISUtilities;

        Form pvtParentForm;

        private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        private byte[] pvtbyteCompress;

        private int pvtintReturnCode = -1;

        private Int64 pvtint64CompanyNo = -1;
        private int pvtintNumberHorizontalPages = -1;
        private bool pvtblnConsolidate = false;
        private bool pvtblnByPayCategoryParameter = false;
        private string pvtstrReportHeader = "";
        private string pvtstrEmployeePrintOrder = "";

        public frmRptEarningsDeductionsHorizontalPageSelection(Form parParentForm,Int64 parint64CompanyNo,DataSet parDataSet, bool parblnConsolidate, bool parblnByPayCategoryParameter, string parstrReportHeader, int parintNumberHorizontalPages, string parstrEmployeePrintOrder)
        {
            pvtParentForm = parParentForm;
            pvtint64CompanyNo = parint64CompanyNo;
            pvtTempDataSet = parDataSet;
            pvtblnConsolidate = parblnConsolidate;
            pvtblnByPayCategoryParameter = parblnByPayCategoryParameter;
            pvtstrReportHeader = parstrReportHeader;
            pvtintNumberHorizontalPages = parintNumberHorizontalPages;
            pvtstrEmployeePrintOrder = parstrEmployeePrintOrder;

            InitializeComponent();
        }

        private void frmRptEarningsDeductionsHorizontalPageSelection_Load(object sender, System.EventArgs e)
        {
            clsISUtilities = new clsISUtilities(this,"busRptEarningsDeductions");
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void pbxHorzPage1_Click(object sender, System.EventArgs e)
        {
            try
            {
                object[] objParm = new object[6];
                objParm[0] = pvtint64CompanyNo;
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = 1;
                objParm[3] = pvtintNumberHorizontalPages;
                objParm[4] = pvtstrEmployeePrintOrder;
                objParm[5] = pvtblnByPayCategoryParameter;

                pvtbyteCompress = (byte[])clsISUtilities.DynamicFunction("Print_Horizontal_SpreadSheet_Page", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbyteCompress);

                //Company DataTable
                DataTable myCompanyDataTable = pvtTempDataSet.Tables["Company"].Copy();

                pvtDataSet.Tables.Add(myCompanyDataTable);

                //CompanyHeaderDetails DataTable
                DataTable myCompanyHeaderDetailsDataTable = pvtTempDataSet.Tables["CompanyHeaderDetails"].Copy();

                pvtDataSet.Tables.Add(myCompanyHeaderDetailsDataTable);

                frmRptEarningsDeductionsSpreadSheet = new frmRptEarningsDeductionsSpreadSheet(this, pvtDataSet, pvtblnConsolidate, pvtblnByPayCategoryParameter, 1, pvtintNumberHorizontalPages, pvtstrReportHeader, pvtstrEmployeePrintOrder, "S");
                frmRptEarningsDeductionsSpreadSheet.MdiParent = this.MdiParent;
                frmRptEarningsDeductionsSpreadSheet.Text = this.Text;
                frmRptEarningsDeductionsSpreadSheet.Show();

                this.Hide();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void pbxHorzPage2_Click(object sender, System.EventArgs e)
        {
            try
            {
                object[] objParm = new object[6];
                objParm[0] = pvtint64CompanyNo;
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = 2;
                objParm[3] = pvtintNumberHorizontalPages;
                objParm[4] = pvtstrEmployeePrintOrder;
                objParm[5] = pvtblnByPayCategoryParameter;

                pvtbyteCompress = (byte[])clsISUtilities.DynamicFunction("Print_Horizontal_SpreadSheet_Page", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbyteCompress);

                //Company DataTable
                DataTable myCompanyDataTable = pvtTempDataSet.Tables["Company"].Copy();

                pvtDataSet.Tables.Add(myCompanyDataTable);

                //CompanyHeaderDetails DataTable
                DataTable myCompanyHeaderDetailsDataTable = pvtTempDataSet.Tables["CompanyHeaderDetails"].Copy();

                pvtDataSet.Tables.Add(myCompanyHeaderDetailsDataTable);

                frmRptEarningsDeductionsSpreadSheet = new frmRptEarningsDeductionsSpreadSheet(this, pvtDataSet, pvtblnConsolidate, pvtblnByPayCategoryParameter, 2, pvtintNumberHorizontalPages, pvtstrReportHeader, pvtstrEmployeePrintOrder, "S");
                frmRptEarningsDeductionsSpreadSheet.MdiParent = this.MdiParent;
                frmRptEarningsDeductionsSpreadSheet.Text = this.Text;
                frmRptEarningsDeductionsSpreadSheet.Show();

                this.Hide();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void pbxHorzPage3_Click(object sender, System.EventArgs e)
        {
            try
            {
                object[] objParm = new object[6];
                objParm[0] = pvtint64CompanyNo;
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = 3;
                objParm[3] = pvtintNumberHorizontalPages;
                objParm[4] = pvtstrEmployeePrintOrder;
                objParm[5] = pvtblnByPayCategoryParameter;

                pvtbyteCompress = (byte[])clsISUtilities.DynamicFunction("Print_Horizontal_SpreadSheet_Page", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbyteCompress);

                //Company DataTable
                DataTable myCompanyDataTable = pvtTempDataSet.Tables["Company"].Copy();

                pvtDataSet.Tables.Add(myCompanyDataTable);

                //CompanyHeaderDetails DataTable
                DataTable myCompanyHeaderDetailsDataTable = pvtTempDataSet.Tables["CompanyHeaderDetails"].Copy();

                pvtDataSet.Tables.Add(myCompanyHeaderDetailsDataTable);

                frmRptEarningsDeductionsSpreadSheet = new frmRptEarningsDeductionsSpreadSheet(this, pvtDataSet, pvtblnConsolidate, pvtblnByPayCategoryParameter, 3, pvtintNumberHorizontalPages, pvtstrReportHeader, pvtstrEmployeePrintOrder, "S");
                frmRptEarningsDeductionsSpreadSheet.MdiParent = this.MdiParent;
                frmRptEarningsDeductionsSpreadSheet.Text = this.Text;
                frmRptEarningsDeductionsSpreadSheet.Show();

                this.Hide();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void pbxHorzPage4_Click(object sender, System.EventArgs e)
        {
            try
            {
                object[] objParm = new object[6];
                objParm[0] = pvtint64CompanyNo;
                objParm[1] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[2] = 4;
                objParm[3] = pvtintNumberHorizontalPages;
                objParm[4] = pvtstrEmployeePrintOrder;
                objParm[5] = pvtblnByPayCategoryParameter;

                pvtbyteCompress = (byte[])clsISUtilities.DynamicFunction("Print_Horizontal_SpreadSheet_Page", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbyteCompress);

                //Company DataTable
                DataTable myCompanyDataTable = pvtTempDataSet.Tables["Company"].Copy();

                pvtDataSet.Tables.Add(myCompanyDataTable);

                //CompanyHeaderDetails DataTable
                DataTable myCompanyHeaderDetailsDataTable = pvtTempDataSet.Tables["CompanyHeaderDetails"].Copy();

                pvtDataSet.Tables.Add(myCompanyHeaderDetailsDataTable);

                frmRptEarningsDeductionsSpreadSheet = new frmRptEarningsDeductionsSpreadSheet(this, pvtDataSet, pvtblnConsolidate, pvtblnByPayCategoryParameter, 4, pvtintNumberHorizontalPages, pvtstrReportHeader, pvtstrEmployeePrintOrder, "S");
                frmRptEarningsDeductionsSpreadSheet.MdiParent = this.MdiParent;
                frmRptEarningsDeductionsSpreadSheet.Text = this.Text;
                frmRptEarningsDeductionsSpreadSheet.Show();

                this.Hide();
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void frmRptEarningsDeductionsHorizontalPageSelection_FormClosing(object sender, FormClosingEventArgs e)
        {
            pvtParentForm.Show();
        }
    }
}
