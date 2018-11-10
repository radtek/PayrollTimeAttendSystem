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
    public partial class frmRptEarningsDeductionsNormal : InteractPayroll.frmISPrintDialogControl
    {
        private string pvtstrDateFormat;
        private string pvtstrReportHeader;
        private string pvtstrSortColumn;
        private bool pvtblnConsolidate;
        private bool pvtblnByPayCategoryParameter;

        int pvtintPageNo = 0;
        int pvtintPrintTableRow = 0;

        Font printFont16;
        Font printFont8;
        Font printFont8Bold;
        Font printFont10Bold;

        Form pvtForm;

        DataSet pvtDataSet;
        DataView pvtEmployeeDetailDataView;
        DataView pvtPrintDataView;

        public frmRptEarningsDeductionsNormal(Form parForm, DataSet parDataSet, string parstrReportHeader, string parstrSortColumn, bool parblnConsolidate, bool parblnByPayCategoryParameter)
        {
            printFont16 = new Font("Sans Serif", 16);
            printFont8 = new Font("Sans Serif", 8);
            printFont8Bold = new Font("Sans Serif", 8, FontStyle.Bold);
            printFont10Bold = new Font("Sans Serif", 10, FontStyle.Bold);

            pvtForm = (System.Windows.Forms.Form)parForm;
            pvtDataSet = parDataSet;
            pvtstrReportHeader = parstrReportHeader;
            pvtstrSortColumn = parstrSortColumn;
            pvtblnConsolidate = parblnConsolidate;
            pvtblnByPayCategoryParameter = parblnByPayCategoryParameter;

            pvtstrDateFormat = AppDomain.CurrentDomain.GetData("DateFormat").ToString();

            InitializeComponent();
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //Set Graphics for ISPrintDialogControl
            this.Graphics = e.Graphics;

            DateTime dtDateTime = DateTime.Now.AddYears(-1000);

            this.Print_Page_Header("Earnings / Deductions " + pvtstrReportHeader, true, pvtDataSet);

            Print_Page_ColumnHeader_Strip(true);

            int intCategoryOffset = 260;

            this.DrawString("Employee\nCode", printFont8Bold, Brushes.Black, this.pubintLeftBorderX, this.pubintColumnHeaderTextY);

            if (pvtstrSortColumn == "N")
            {
                this.DrawString("Name / Surname", printFont8Bold, Brushes.Black, this.pubintLeftBorderX + 80, this.pubintColumnHeaderTextY);
            }
            else
            {
                this.DrawString("Surname / Name", printFont8Bold, Brushes.Black, this.pubintLeftBorderX + 80, this.pubintColumnHeaderTextY);
            }

            this.DrawString("Description", printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intCategoryOffset, this.pubintColumnHeaderTextY);

            if (pvtblnByPayCategoryParameter == true)
            {
                this.DrawString("Cost Centre", printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intCategoryOffset + 160, this.pubintColumnHeaderTextY);
            }

            if (pvtblnConsolidate == false)
            {
                this.DrawString("Date", printFont8Bold, Brushes.Black, this.pubintLeftBorderX + 532, this.pubintColumnHeaderTextY);
            }
            this.DrawStringRight("Earnings\nTotal", printFont8Bold, Brushes.Black, this.pubintLeftBorderX + 675, this.pubintColumnHeaderTextY);
            this.DrawStringRight("Deductions\nTotal", printFont8Bold, Brushes.Black, this.pubintLeftBorderX + 755, this.pubintColumnHeaderTextY);

            Int64 int64EmployeeNo = -1;
            string strCategoryDescPrev = "";
            string strPayCategoryDescPrev = "";

            if (pvtintPageNo == 0)
            {
                for (int intRow = 0; intRow < pvtDataSet.Tables["Print"].Rows.Count; intRow++)
                {
                    if (pvtDataSet.Tables["Print"].Rows[intRow]["PAY_CATEGORY_SORT_ORDER"].ToString() != "0")
                    {
                        if (int64EmployeeNo != Convert.ToInt64(pvtDataSet.Tables["Print"].Rows[intRow]["EMPLOYEE_NO"]))
                        {
                            pvtEmployeeDetailDataView = null;
                            pvtEmployeeDetailDataView = new DataView(pvtDataSet.Tables["EmployeeDetail"], "EMPLOYEE_NO = " + pvtDataSet.Tables["Print"].Rows[intRow]["EMPLOYEE_NO"].ToString(), "", DataViewRowState.CurrentRows);

                            int64EmployeeNo = Convert.ToInt64(pvtDataSet.Tables["Print"].Rows[intRow]["EMPLOYEE_NO"]);
                        }

                        pvtDataSet.Tables["Print"].Rows[intRow]["EMPLOYEE_CODE"] = pvtEmployeeDetailDataView[0]["EMPLOYEE_CODE"].ToString();
                        pvtDataSet.Tables["Print"].Rows[intRow]["EMPLOYEE_NAME"] = pvtEmployeeDetailDataView[0]["EMPLOYEE_NAME"].ToString();
                        pvtDataSet.Tables["Print"].Rows[intRow]["EMPLOYEE_SURNAME"] = pvtEmployeeDetailDataView[0]["EMPLOYEE_SURNAME"].ToString();
                    }
                }

                string strOrderColumn = "EMPLOYEE_CODE";

                if (pvtstrSortColumn == "S")
                {
                    strOrderColumn = "EMPLOYEE_SURNAME";
                }
                else
                {
                    if (pvtstrSortColumn == "N")
                    {
                        strOrderColumn = "EMPLOYEE_NAME";
                    }
                }

                pvtPrintDataView = null;
                pvtPrintDataView = new DataView(pvtDataSet.Tables["Print"], "", strOrderColumn, DataViewRowState.CurrentRows);
            }

            pvtintPageNo += 1;

            int64EmployeeNo = -1;

            this.Print_Page_PageNumber("Page " + pvtintPageNo.ToString() + " of " + ((pvtDataSet.Tables["Print"].Rows.Count / pubintRowsInPotraitPage) + 1).ToString(), true);

            int intY = this.pubintFirstColumnY;

            for (int intRow = pvtintPrintTableRow; intRow < pvtPrintDataView.Count; intRow++)
            {
                if (int64EmployeeNo != Convert.ToInt64(pvtPrintDataView[intRow]["EMPLOYEE_NO"]))
                {
                    pvtEmployeeDetailDataView = null;
                    pvtEmployeeDetailDataView = new DataView(pvtDataSet.Tables["EmployeeDetail"], "EMPLOYEE_NO = " + pvtPrintDataView[intRow]["EMPLOYEE_NO"].ToString(), "", DataViewRowState.CurrentRows);

                    this.DrawString(pvtEmployeeDetailDataView[0]["EMPLOYEE_CODE"].ToString(), printFont8, Brushes.Black, this.pubintLeftBorderX, intY);

                    if (pvtstrSortColumn == "N")
                    {
                        this.DrawString(pvtEmployeeDetailDataView[0]["EMPLOYEE_NAME"].ToString() + ", " + pvtEmployeeDetailDataView[0]["EMPLOYEE_SURNAME"].ToString(), printFont8, Brushes.Black, this.pubintLeftBorderX + 80, intY);
                    }
                    else
                    {
                        this.DrawString(pvtEmployeeDetailDataView[0]["EMPLOYEE_SURNAME"].ToString() + ", " + pvtEmployeeDetailDataView[0]["EMPLOYEE_NAME"].ToString(), printFont8, Brushes.Black, this.pubintLeftBorderX + 80, intY);
                    }

                    int64EmployeeNo = Convert.ToInt64(pvtPrintDataView[intRow]["EMPLOYEE_NO"]);
                }

                if (pvtPrintDataView[intRow]["SORT_ORDER"].ToString() == "2"
                    | pvtPrintDataView[intRow]["SORT_ORDER"].ToString() == "3")
                {
                    e.Graphics.FillRectangle(Brushes.LightGray, this.pubintLeftBorderX + 522, intY - 3, 240, 20);
                    dtDateTime = DateTime.Now.AddYears(-1000);
                    strCategoryDescPrev = "";
                    strPayCategoryDescPrev = "";
                }

                if (pvtPrintDataView[intRow]["SORT_ORDER"].ToString() == "1")
                {
                    if (strCategoryDescPrev != pvtPrintDataView[intRow]["CATEGORY_DESC"].ToString())
                    {
                        strCategoryDescPrev = pvtPrintDataView[intRow]["CATEGORY_DESC"].ToString();
                        this.DrawString(pvtPrintDataView[intRow]["CATEGORY_DESC"].ToString(), printFont8, Brushes.Black, this.pubintLeftBorderX + intCategoryOffset, intY);
                    }
                }
                else
                {
                    this.DrawStringRight(pvtPrintDataView[intRow]["CATEGORY_DESC"].ToString(), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + 590, intY);
                }

                if (pvtblnConsolidate == false)
                {
                    if (pvtPrintDataView[intRow]["SORT_ORDER"].ToString() == "1")
                    {
                        if (pvtPrintDataView[intRow]["PAY_PERIOD_DATE"] != System.DBNull.Value)
                        {
                            if (dtDateTime != Convert.ToDateTime(pvtPrintDataView[intRow]["PAY_PERIOD_DATE"]))
                            {
                                this.DrawStringRight(Convert.ToDateTime(pvtPrintDataView[intRow]["PAY_PERIOD_DATE"]).ToString(pvtstrDateFormat), printFont8, Brushes.Black, this.pubintLeftBorderX + 590, intY);
                                dtDateTime = Convert.ToDateTime(pvtPrintDataView[intRow]["PAY_PERIOD_DATE"]);
                            }


                        }
                    }
                }

                if (pvtPrintDataView[intRow]["SORT_ORDER"].ToString() == "1"
                     & pvtblnByPayCategoryParameter == true)
                {
                    if (strPayCategoryDescPrev != pvtPrintDataView[intRow]["PAY_CATEGORY_DESC"].ToString())
                    {
                        strPayCategoryDescPrev = pvtPrintDataView[intRow]["PAY_CATEGORY_DESC"].ToString();

                        this.DrawString(pvtPrintDataView[intRow]["PAY_CATEGORY_DESC"].ToString(), printFont8, Brushes.Black, this.pubintLeftBorderX + intCategoryOffset + 160, intY);
                    }
                }

                if (pvtPrintDataView[intRow]["EARNING_AMOUNT"] != System.DBNull.Value)
                {
                    if (pvtPrintDataView[intRow]["SORT_ORDER"].ToString() == "1")
                    {
                        this.DrawStringRight(Convert.ToDouble(pvtPrintDataView[intRow]["EARNING_AMOUNT"]).ToString("##,###,##0.00"), printFont8, Brushes.Black, this.pubintLeftBorderX + 675, intY);
                    }
                    else
                    {
                        this.DrawStringRight(Convert.ToDouble(pvtPrintDataView[intRow]["EARNING_AMOUNT"]).ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + 675, intY);
                    }
                }

                if (pvtPrintDataView[intRow]["DEDUCTION_AMOUNT"] != System.DBNull.Value)
                {
                    if (pvtPrintDataView[intRow]["SORT_ORDER"].ToString() == "1")
                    {
                        this.DrawStringRight(Convert.ToDouble(pvtPrintDataView[intRow]["DEDUCTION_AMOUNT"]).ToString("##,###,##0.00"), printFont8, Brushes.Black, this.pubintLeftBorderX + 755, intY);
                    }
                    else
                    {
                        this.DrawStringRight(Convert.ToDouble(pvtPrintDataView[intRow]["DEDUCTION_AMOUNT"]).ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + 755, intY);
                    }
                }

                pvtintPrintTableRow = intRow + 1;

                if (intRow != 0)
                {
                    if (intRow % this.pubintRowsInPotraitPage == 0)
                    {
                        e.HasMorePages = true;
                        break;
                    }
                }

                intY += this.pubintColumnYHeight;
            }
        }

        private void frmRptEarningsDeductionsNormal_Load(object sender, EventArgs e)
        {
            try
            {
                printFont16 = new Font("Sans Serif", 16);
                printFont8 = new Font("Sans Serif", 8);
                printFont8Bold = new Font("Sans Serif", 8, FontStyle.Bold);
                printFont10Bold = new Font("Sans Serif", 10, FontStyle.Bold);

                printDocument.DefaultPageSettings.Landscape = false;

                this.PrintPreviewControl.Show();

                pvtintPrintTableRow = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        private void frmRptEarningsDeductionsNormal_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Refresh();

            pvtForm.Visible = true;
            pvtForm.Show();
        }
    }
}
