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
    public partial class frmRptEarningsDeductionsSpreadSheet : InteractPayroll.frmISPrintDialogControl
    {
        private string pvtstrReportHeader = "";
        private string pvtstrEmployeePrintOrder = "";
        private string pvtstrPrintType = "";
        private string pvtstrDateFormat = "";
        private int pvtintCurrentHorizontalPage = 0;
        private int pvtintNumberOfHorizontalPages = 0;
        private int pvtintPageNo = 0;

        private double pvtdblCol1Total = 0;
        private double pvtdblCol2Total = 0;
        private double pvtdblCol3Total = 0;
        private double pvtdblCol4Total = 0;
        private double pvtdblCol5Total = 0;
        private double pvtdblCol6Total = 0;
        private double pvtdblCol7Total = 0;
        private double pvtdblCol8Total = 0;
        private double pvtdblCol9Total = 0;
        private double pvtdblCol10Total = 0;
        private double pvtdblCol11Total = 0;
        private double pvtdblCol12Total = 0;
        private double pvtdblCol13Total = 0;

        private double pvtdblGrossEarningsTotal = 0;
        private double pvtdblGrossDeductionsTotal = 0;
        private double pvtdblNettTotal = 0;

        private bool pvtblnConsolidate = false;
        private bool pvtblnByPayCategoryParameter = false;
        private Form pvtForm;
        private DataSet pvtDataSet;

        private int pvtintEmployeeTableRow = 0;
        private DataView pvtCompanyPrintHeaderDataView;

        Font printFont16;
        Font printFont8;
        Font printFont8Bold;
        Font printHeaderFont;

        public frmRptEarningsDeductionsSpreadSheet(Form parForm, DataSet parDataSet, bool parblnConsolidate, bool parblnByPayCategoryParameter,int parintCurrentHorizontalPage, int parintNumberOfHorizontalPages, string parstrReportHeader, string parstrEmployeePrintOrder, string parstrPrintType)
        {
            pvtForm = (System.Windows.Forms.Form)parForm;
            pvtDataSet = parDataSet;
            pvtblnConsolidate = parblnConsolidate;
            pvtblnByPayCategoryParameter = parblnByPayCategoryParameter;
            pvtintCurrentHorizontalPage = parintCurrentHorizontalPage;
            pvtintNumberOfHorizontalPages = parintNumberOfHorizontalPages;
            pvtstrReportHeader = parstrReportHeader;
            pvtstrEmployeePrintOrder = parstrEmployeePrintOrder;
            pvtstrPrintType = parstrPrintType;

            pvtstrDateFormat = AppDomain.CurrentDomain.GetData("DateFormat").ToString();
            
            InitializeComponent();
        }

        private void frmRptEarningsDeductionsSpreadSheet_Load(object sender, System.EventArgs e)
        {
            try
            {
                printFont16 = new Font("Sans Serif", 16);
                printFont8 = new Font("Sans Serif", 8);
                printFont8Bold = new Font("Sans Serif", 8, FontStyle.Bold);

                printDocument.DefaultPageSettings.Landscape = true;

                this.PrintPreviewControl.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        private void frmRptEarningsDeductionsSpreadSheet_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Refresh();

            if (pvtintNumberOfHorizontalPages > 1)
            {
                this.Visible = false;
                this.WindowState = FormWindowState.Minimized;

                pvtForm.Visible = true;
                pvtForm.Show();
            }
            else
            {
                pvtForm.Show();
            }
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //Set Graphics for ISPrintDialogControl
            this.Graphics = e.Graphics;

            string strPrevEmployeeCode = "";
            string strPrevPayCategoryDesc = "";
            bool blnPrintName = false;

            this.Print_Page_Header("Earnings / Deductions " + pvtstrReportHeader, false, pvtDataSet);

            //Print Column Headers
            e.Graphics.FillRectangle(Brushes.LightGray, this.pubintLeftBorderX, this.pubintColumnHeaderY, 1100, 32);

            int intColWidth = 78;

            this.DrawString("Employee\nCode", printFont8Bold, Brushes.Black, this.pubintLeftBorderX, this.pubintColumnHeaderTextY);

            int intFirstColOffset = 472;

            if (pvtintCurrentHorizontalPage == 1)
            {
                if (pvtstrEmployeePrintOrder == "N")
                {
                    this.DrawString("Name / Surname", printFont8Bold, Brushes.Black, this.pubintLeftBorderX + 80, this.pubintColumnHeaderTextY);
                }
                else
                {
                    this.DrawString("Surname / Name", printFont8Bold, Brushes.Black, this.pubintLeftBorderX + 80, this.pubintColumnHeaderTextY);
                }

                if (pvtblnByPayCategoryParameter == true)
                {
                    this.DrawString("Cost Centre", printFont8Bold, Brushes.Black, this.pubintLeftBorderX + 280, this.pubintColumnHeaderTextY);
                }

                if (pvtblnConsolidate == false)
                {
                    this.DrawString("Date", printFont8Bold, Brushes.Black, this.pubintLeftBorderX + 413, this.pubintColumnHeaderTextY);
                }
            }
            else
            {
                intFirstColOffset = 80;
            }

            for (int intColRow = 1; intColRow < 14; intColRow++)
            {
                if (pvtDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intColRow.ToString() + "_NAME"].ToString() != "")
                {
                    this.DrawStringRight(pvtDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intColRow.ToString() + "_NAME"].ToString(), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), this.pubintColumnHeaderTextY);
                }
            }

            this.DrawString(pvtDataSet.Tables["PrintHeader"].Rows[0]["HORIZONTAL_PAGE_NUMBER"].ToString(), printFont8, Brushes.Black, this.pubintLeftBorderX, this.pubintPageNumberLandscapeY);

            pvtintPageNo += 1;

            this.Print_Page_PageNumber("Page " + pvtintPageNo.ToString() + " of " + ((pvtDataSet.Tables["Print"].Rows.Count / pubintRowsInLandscapePage) + 1).ToString(), false);

            int intY = this.pubintFirstColumnY;

            for (int intRow = pvtintEmployeeTableRow; intRow < pvtDataSet.Tables["Print"].Rows.Count; intRow++)
            {
                blnPrintName = false;

                if (strPrevEmployeeCode != pvtDataSet.Tables["Print"].Rows[intRow]["EMPLOYEE_CODE"].ToString())
                {
                    this.DrawString(pvtDataSet.Tables["Print"].Rows[intRow]["EMPLOYEE_CODE"].ToString(), printFont8, Brushes.Black, this.pubintLeftBorderX, intY);

                    strPrevEmployeeCode = pvtDataSet.Tables["Print"].Rows[intRow]["EMPLOYEE_CODE"].ToString();
                    strPrevPayCategoryDesc = "";

                    blnPrintName = true;
                }

                if (pvtintCurrentHorizontalPage == 1)
                {
                    if (pvtstrEmployeePrintOrder == "N")
                    {
                        if (blnPrintName == true)
                        {
                            this.DrawString(pvtDataSet.Tables["Print"].Rows[intRow]["EMPLOYEE_NAME"].ToString() + ", " + pvtDataSet.Tables["Print"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString(), printFont8, Brushes.Black, this.pubintLeftBorderX + 80, intY);
                        }
                    }
                    else
                    {
                        if (blnPrintName == true)
                        {
                            this.DrawString(pvtDataSet.Tables["Print"].Rows[intRow]["EMPLOYEE_SURNAME"].ToString() + ", " + pvtDataSet.Tables["Print"].Rows[intRow]["EMPLOYEE_NAME"].ToString(), printFont8, Brushes.Black, this.pubintLeftBorderX + 80, intY);
                        }
                    }

                    if (pvtblnByPayCategoryParameter == true)
                    {
                        if (Convert.ToInt32(pvtDataSet.Tables["Print"].Rows[intRow]["SORT_IND"]) == 2)
                        {
                            e.Graphics.FillRectangle(Brushes.LightGray, this.pubintLeftBorderX + 470, intY - 3, 630, 20);
                        }

                        if (strPrevPayCategoryDesc != pvtDataSet.Tables["Print"].Rows[intRow]["PAY_CATEGORY_DESC"].ToString()
                            & Convert.ToInt32(pvtDataSet.Tables["Print"].Rows[intRow]["SORT_IND"]) == 1)
                        {
                            this.DrawString(pvtDataSet.Tables["Print"].Rows[intRow]["PAY_CATEGORY_DESC"].ToString(), printFont8, Brushes.Black, this.pubintLeftBorderX + 280, intY);
                            strPrevPayCategoryDesc = pvtDataSet.Tables["Print"].Rows[intRow]["PAY_CATEGORY_DESC"].ToString();
                        }
                    }

                    if (pvtblnConsolidate == false
                        & Convert.ToInt32(pvtDataSet.Tables["Print"].Rows[intRow]["SORT_IND"]) == 1)
                    {
                        this.DrawStringRight(Convert.ToDateTime(pvtDataSet.Tables["Print"].Rows[intRow]["PAY_PERIOD_DATE"]).ToString(pvtstrDateFormat), printFont8, Brushes.Black, this.pubintLeftBorderX + 470, intY);
                    }
                }
                else
                {
                    if (pvtblnByPayCategoryParameter == true)
                    {
                        if (Convert.ToInt32(pvtDataSet.Tables["Print"].Rows[intRow]["SORT_IND"]) == 2)
                        {
                            e.Graphics.FillRectangle(Brushes.LightGray, this.pubintLeftBorderX + 85, intY - 3, 1015, 20);
                        }
                    }

                    intFirstColOffset = 80;
                }

                for (int intColRow = 1; intColRow < 14; intColRow++)
                {
                    if (pvtDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intColRow.ToString() + "_NAME"].ToString() == "")
                    {
                        break;
                    }
                    else
                    {
                        if (pvtDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intColRow.ToString() + "_NAME"].ToString() == "Gross\nEarnings"
                            | pvtDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intColRow.ToString() + "_NAME"].ToString() == "Gross\nDeductions"
                            | pvtDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intColRow.ToString() + "_NAME"].ToString() == "Nett\nTotals")
                        {
                            this.DrawStringRight(Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]).ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                            if (pvtDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intColRow.ToString() + "_NAME"].ToString() == "Gross\nEarnings")
                            {
                                pvtdblGrossEarningsTotal += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);
                            }
                            else
                            {
                                if (pvtDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intColRow.ToString() + "_NAME"].ToString() == "Gross\nDeductions")
                                {
                                    pvtdblGrossDeductionsTotal += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);
                                }
                                else
                                {
                                    pvtdblNettTotal += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(pvtDataSet.Tables["Print"].Rows[intRow]["SORT_IND"]) == 1)
                            {
                                this.DrawStringRight(Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]).ToString("##,###,##0.00"), printFont8, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);
                            }
                            else
                            {
                                this.DrawStringRight(Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]).ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);
                            }
                        }

                        switch (intColRow)
                        {
                            case 1:

                                pvtdblCol1Total += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);

                                break;

                            case 2:

                                pvtdblCol2Total += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);

                                break;

                            case 3:

                                pvtdblCol3Total += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);

                                break;

                            case 4:

                                pvtdblCol4Total += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);

                                break;

                            case 5:

                                pvtdblCol5Total += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);

                                break;

                            case 6:

                                pvtdblCol6Total += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);

                                break;

                            case 7:

                                pvtdblCol7Total += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);

                                break;

                            case 8:

                                pvtdblCol8Total += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);

                                break;

                            case 9:

                                pvtdblCol9Total += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);

                                break;

                            case 10:

                                pvtdblCol10Total += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);

                                break;

                            case 11:

                                pvtdblCol11Total += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);

                                break;

                            case 12:

                                pvtdblCol12Total += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);

                                break;

                            case 13:

                                pvtdblCol13Total += Convert.ToDouble(pvtDataSet.Tables["Print"].Rows[intRow]["FIELD" + intColRow.ToString() + "_AMOUNT"]);

                                break;
                        }
                    }
                }

                intY += this.pubintColumnYHeight;

                if (intRow == pvtDataSet.Tables["Print"].Rows.Count - 1)
                {
                    //Print Column Headers
                    e.Graphics.FillRectangle(Brushes.Gray, this.pubintLeftBorderX, intY - 3, 1100, 20);

                    if (pvtintCurrentHorizontalPage == 1)
                    {
                        this.DrawString("Report Summary Totals", printFont8Bold, Brushes.Black, this.pubintLeftBorderX, intY);
                    }

                    for (int intColRow = 1; intColRow < 14; intColRow++)
                    {
                        if (pvtDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intColRow.ToString() + "_NAME"].ToString() == "")
                        {
                            break;
                        }
                        else
                        {
                            if (pvtDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intColRow.ToString() + "_NAME"].ToString() == "Gross\nEarnings")
                            {
                                this.DrawStringRight(pvtdblGrossEarningsTotal.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);
                            }
                            else
                            {
                                if (pvtDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intColRow.ToString() + "_NAME"].ToString() == "Gross\nDeductions")
                                {
                                    this.DrawStringRight(pvtdblGrossDeductionsTotal.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);
                                }
                                else
                                {
                                    if (pvtDataSet.Tables["PrintHeader"].Rows[0]["FIELD" + intColRow.ToString() + "_NAME"].ToString() == "Nett\nTotals")
                                    {
                                        this.DrawStringRight(pvtdblNettTotal.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);
                                    }
                                    else
                                    {
                                        switch (intColRow)
                                        {
                                            case 1:

                                                this.DrawStringRight(pvtdblCol1Total.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                                                break;

                                            case 2:

                                                this.DrawStringRight(pvtdblCol2Total.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                                                break;

                                            case 3:

                                                this.DrawStringRight(pvtdblCol3Total.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                                                break;

                                            case 4:

                                                this.DrawStringRight(pvtdblCol4Total.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                                                break;

                                            case 5:

                                                this.DrawStringRight(pvtdblCol5Total.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                                                break;

                                            case 6:

                                                this.DrawStringRight(pvtdblCol6Total.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                                                break;

                                            case 7:

                                                this.DrawStringRight(pvtdblCol7Total.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                                                break;

                                            case 8:

                                                this.DrawStringRight(pvtdblCol8Total.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                                                break;

                                            case 9:

                                                this.DrawStringRight(pvtdblCol9Total.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                                                break;

                                            case 10:

                                                this.DrawStringRight(pvtdblCol10Total.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                                                break;

                                            case 11:

                                                this.DrawStringRight(pvtdblCol11Total.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                                                break;

                                            case 12:

                                                this.DrawStringRight(pvtdblCol12Total.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                                                break;

                                            case 13:

                                                this.DrawStringRight(pvtdblCol13Total.ToString("##,###,##0.00"), printFont8Bold, Brushes.Black, this.pubintLeftBorderX + intFirstColOffset + (intColRow * intColWidth), intY);

                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //ReInitialise - No More Pages (Cater for Print)

                    pvtintEmployeeTableRow = 0;
                }
                else
                {
                    pvtintEmployeeTableRow = intRow + 1;

                    if (intRow != 0)
                    {
                        if (intRow % pubintRowsInLandscapePage == 0)
                        {
                            e.HasMorePages = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
