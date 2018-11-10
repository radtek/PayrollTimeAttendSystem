using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using C1.Win.C1FlexGrid;

namespace InteractPayroll
{
    public partial class frmCalenderControl : Form
    {
        private int pvtintCurrDay = 0;
        private int pvtintCurrMonth = 0;
        private int pvtintCurrYear = 0;
        private bool pvtblnBusyComboBox = false;

        private Control pvtControl;

        private Control pvtCtrlParent;
        private Control pvtCtrlChild;

        public MenuItem pubmniMenuItem;

        private string pvtstrDate;
        private string pvtstrDateFormat;

        private int pvtintLeft;
        private int pvtintTop;

        private bool pvtblnAbove;

        Form Form;

        DateTime dtDate;

        DataSet pvtDataSet;
        DataView pvtDataView;

        CellStyle CellStyle;
        CellRange CellRange;

        public frmCalenderControl()
        {
            InitializeComponent();

            for (int intYear = 2007; intYear < DateTime.Now.Year + 2; intYear++)
            {
                this.cboYear.Items.Add(intYear.ToString());
            }
            
            pvtDataSet = (DataSet)AppDomain.CurrentDomain.GetData("DataSet");

            CellStyle = this.flxgCalender.Styles.Add("SatSunStyle");
            CellStyle.ForeColor = Color.Blue;

            CellStyle = this.flxgCalender.Styles.Add("PublicHolidayStyle");
            CellStyle.ForeColor = Color.Red;

            CellStyle = this.flxgCalender.Styles.Add("Normal");
            CellStyle.ForeColor = Color.Black;

            this.flxgCalender.Cols[0].Style = this.flxgCalender.Styles["SatSunStyle"];
            this.flxgCalender.Cols[6].Style = this.flxgCalender.Styles["SatSunStyle"];

            try
            {
                pvtstrDateFormat = AppDomain.CurrentDomain.GetData("DateFormat").ToString();
            }
            catch
            {
            }
        }

        public void Show_Calender(Control parControl, bool parblnAbove)
        {
            pvtblnAbove = parblnAbove;

            try
            {
                pvtstrDateFormat = AppDomain.CurrentDomain.GetData("DateFormat").ToString();
            }
            catch
            {
            }

            pvtControl = (Control)parControl; 

            pvtCtrlChild = (Control)parControl; 

            int intBorderWidth = 0;
            int intTitlebarHeight = 0;
            pvtintLeft = pvtCtrlChild.Left;

            if (parblnAbove == true)
            {
                pvtintTop = pvtCtrlChild.Top;
            }
            else
            {
                pvtintTop = pvtCtrlChild.Top + pvtCtrlChild.Height;
            }

        Find_Window_Top_Position:

            pvtCtrlParent = pvtCtrlChild.Parent;

            try
            {
                Form = (Form)pvtCtrlParent;

                intBorderWidth = (Form.Width - Form.ClientSize.Width) / 2;
                intTitlebarHeight = Form.Height - Form.ClientSize.Height - 2 * intBorderWidth;

                //NB 2 Bits For Positioning Exact
                if (parblnAbove == true)
                {
                    pvtintTop += pvtCtrlParent.Top + intTitlebarHeight + 2;
                    pvtintLeft += pvtCtrlParent.Left + intBorderWidth - 2;
                }
                else
                {
                    pvtintTop += pvtCtrlParent.Top + intTitlebarHeight + 8;
                    pvtintLeft += pvtCtrlParent.Left + intBorderWidth;
                }
            }
            catch
            {
                pvtCtrlChild = pvtCtrlParent;
                pvtintTop += pvtCtrlChild.Top;
                pvtintLeft += pvtCtrlChild.Left;
                goto Find_Window_Top_Position;
            }

            this.Left = pvtintLeft;

            if (parblnAbove == true)
            {
                this.Top = (pvtintTop + Convert.ToInt32(AppDomain.CurrentDomain.GetData("TitleMenuAndToolbarHeight"))) - this.Height;
            }
            else
            {
                this.Top = (pvtintTop + Convert.ToInt32(AppDomain.CurrentDomain.GetData("TitleMenuAndToolbarHeight")));
            }

            this.Refresh();

            Set_Date(parControl.Text);
            Set_Date(parControl.Text);

            this.Show();
            this.Refresh();
        }

        private void Set_Date(string parstrDateTime)
        {
            if (parstrDateTime == "")
            {
                pvtstrDate = DateTime.Now.ToString(pvtstrDateFormat); 
            }
            else
            {
                pvtstrDate = parstrDateTime;
            }

            Load_Dates();
        }

        private void Load_Dates()
        {
            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtintCurrYear = Convert.ToInt32(pvtstrDate.ToString().Substring(0, 4));
                pvtintCurrMonth = Convert.ToInt32(pvtstrDate.ToString().Substring(5, 2));
                pvtintCurrDay = Convert.ToInt32(pvtstrDate.ToString().Substring(8, 2));
            }
            else
            {
                pvtintCurrDay = Convert.ToInt32(pvtstrDate.ToString().Substring(0, 2));
                pvtintCurrMonth = Convert.ToInt32(pvtstrDate.ToString().Substring(3, 2));
                pvtintCurrYear = Convert.ToInt32(pvtstrDate.ToString().Substring(6, 4));
            }

            DateTime dtCCCCYYMM01;

            dtCCCCYYMM01 = new DateTime(pvtintCurrYear, pvtintCurrMonth, 1);
            this.cboYear.SelectedItem = dtCCCCYYMM01.ToString("yyyy");
            
            int intDayOfWeek;
            int intColSaved;
            int intRowSaved;

            //Display Date
            this.lblDate.Text = pvtstrDate.ToString();
            
            this.lblMonth.Text = dtCCCCYYMM01.ToString("MMMM");
                        
            int intRow = 1;
            int intCol = 0;
            intDayOfWeek = Convert.ToInt32(dtCCCCYYMM01.DayOfWeek);

            if (intDayOfWeek != 0)
            {
                //Clear Old Dates - Before 1st day of Month
                for (int intColTemp = 0; intColTemp < intDayOfWeek; intColTemp++)
                {
                    this.flxgCalender[intRow, intColTemp] = "";
                    intCol = intColTemp;
                }

                intCol = intCol + 1;
            }

            intColSaved = intCol;
            intRowSaved = intRow;

            dtDate = new DateTime(pvtintCurrYear, pvtintCurrMonth, 1);

            CellRange = this.flxgCalender.GetCellRange(intRow, intCol, intRow, intCol);

            pvtDataView = null;
            pvtDataView = new DataView(pvtDataSet.Tables["PaidHoliday"]
                , "PUBLIC_HOLIDAY_DATE = '" + dtDate.ToString() + "'"
                , ""
                , DataViewRowState.CurrentRows);

            if (pvtDataView.Count > 0)
            {
                CellRange.Style = this.flxgCalender.Styles["PublicHolidayStyle"];
            }
            else
            {
                this.lblPaidDate.Text = "";

                if (Convert.ToInt32(dtDate.DayOfWeek) != 0
                    & Convert.ToInt32(dtDate.DayOfWeek) != 6)
                {
                    CellRange.Style = this.flxgCalender.Styles["Normal"];
                }
            }

            this.flxgCalender[intRow, intCol] = "1";
            this.flxgCalender.Row = intRow;
            this.flxgCalender.Col = intCol;

            for (int intCount = 2; intCount < 33; intCount++)
            {
                intCol = intCol + 1;

                if (intCol > 6)
                {
                    intRow = intRow + 1;
                    intCol = 0;
                }

                CellRange = this.flxgCalender.GetCellRange(intRow, intCol, intRow, intCol);

                if (intCount > 28)
                {
                    //Check If Valid Date
                    try
                    {
                        dtDate = new DateTime(pvtintCurrYear, pvtintCurrMonth, intCount);

                        pvtDataView = null;
                        pvtDataView = new DataView(pvtDataSet.Tables["PaidHoliday"]
                            , "PUBLIC_HOLIDAY_DATE = '" + dtDate.ToString() + "'"
                            , ""
                            , DataViewRowState.CurrentRows);

                        if (pvtDataView.Count > 0)
                        {
                            CellRange.Style = this.flxgCalender.Styles["PublicHolidayStyle"];
                        }
                        else
                        {
                            if (Convert.ToInt32(dtDate.DayOfWeek) != 0
                                & Convert.ToInt32(dtDate.DayOfWeek) != 6)
                            {
                                CellRange.Style = this.flxgCalender.Styles["Normal"];
                            }
                        }

                        this.flxgCalender[intRow, intCol] = intCount.ToString();

                        if (pvtintCurrDay == intCount)
                        {
                            intColSaved = intCol;
                            intRowSaved = intRow;
                        }
                    }
                    catch
                    {
                        this.flxgCalender[intRow, intCol] = "";

                        for (int intClearCount = intCount; intClearCount < 100; intClearCount++)
                        {
                            //Clear Cell
                            intCol = intCol + 1;

                            if (intCol > 6)
                            {
                                intRow = intRow + 1;
                                intCol = 0;
                            }

                            if (intRow > 6)
                            {
                                goto Load_Dates_Continue;
                            }

                            this.flxgCalender[intRow, intCol] = "";
                        }
                    }
                }
                else
                {
                    dtDate = new DateTime(pvtintCurrYear, pvtintCurrMonth, intCount);

                    pvtDataView = null;
                    pvtDataView = new DataView(pvtDataSet.Tables["PaidHoliday"]
                        , "PUBLIC_HOLIDAY_DATE = '" + dtDate.ToString() + "'"
                        , ""
                        , DataViewRowState.CurrentRows);

                    if (pvtDataView.Count > 0)
                    {
                        CellRange.Style = this.flxgCalender.Styles["PublicHolidayStyle"];
                    }
                    else
                    {
                        if (Convert.ToInt32(dtDate.DayOfWeek) != 0
                            & Convert.ToInt32(dtDate.DayOfWeek) != 6)
                        {
                            CellRange.Style = this.flxgCalender.Styles["Normal"];
                        }
                    }

                    this.flxgCalender[intRow, intCol] = intCount.ToString();

                    if (pvtintCurrDay == intCount)
                    {
                        intColSaved = intCol;
                        intRowSaved = intRow;
                    }
                }
            }

        Load_Dates_Continue:

            this.flxgCalender.Refresh();

            this.flxgCalender.Row = intRowSaved;
            this.flxgCalender.Col = intColSaved;

            if (pvtblnBusyComboBox == false)
            {
                this.flxgCalender.Focus();
            }
        }

        private void flxgCalender_BeforeRowColChange(object sender, C1.Win.C1FlexGrid.RangeEventArgs e)
        {
            if (e.NewRange.c1 >= 0
                & e.NewRange.r1 >= 0)
            {
                if (this.flxgCalender[e.NewRange.r1, e.NewRange.c1] == null)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    if (this.flxgCalender[e.NewRange.r1, e.NewRange.c1].ToString() == "")
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
            else
            {
                e.Cancel = true;
                return;
            }

            dtDate = new DateTime(pvtintCurrYear, pvtintCurrMonth, Convert.ToInt32(this.flxgCalender[e.NewRange.r1, e.NewRange.c1]));

            pvtDataView = null;
            pvtDataView = new DataView(pvtDataSet.Tables["PaidHoliday"]
                , "PUBLIC_HOLIDAY_DATE = '" + dtDate.ToString() + "'"
                , ""
                , DataViewRowState.CurrentRows);

            if (pvtDataView.Count > 0)
            {
                this.lblPaidDate.Text = pvtDataView[0]["PUBLIC_HOLIDAY_DESC"].ToString();
            }
            else
            {
                this.lblPaidDate.Text = "";
            }

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                this.lblDate.Text = pvtintCurrYear.ToString() + "/" + pvtintCurrMonth.ToString("00") + "/" + Convert.ToInt32(this.flxgCalender[e.NewRange.r1, e.NewRange.c1]).ToString("00");
            }
            else
            {
                this.lblDate.Text = Convert.ToInt32(this.flxgCalender[e.NewRange.r1, e.NewRange.c1]).ToString("00") + "/" + pvtintCurrMonth.ToString("00") + "/" + pvtintCurrYear.ToString();
            }
        }

        private void btnMonthDown_Click(object sender, System.EventArgs e)
        {
            if (pvtintCurrMonth == 1)
            {
                pvtintCurrMonth = 12;
                pvtintCurrYear = pvtintCurrYear - 1;
            }
            else
            {
                pvtintCurrMonth = pvtintCurrMonth - 1;
            }

            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/" + pvtintCurrMonth.ToString("00") + "/01";
            }
            else
            {
                pvtstrDate = "01/" + pvtintCurrMonth.ToString("00") + "/" + pvtintCurrYear.ToString();
            }

            Load_Dates();
        }

        private void btnMonthUp_Click(object sender, System.EventArgs e)
        {
            if (pvtintCurrMonth == 12)
            {
                pvtintCurrMonth = 1;
                pvtintCurrYear = pvtintCurrYear + 1;
            }
            else
            {
                pvtintCurrMonth = pvtintCurrMonth + 1;
            }

            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/" + pvtintCurrMonth.ToString("00") + "/01";
            }
            else
            {
                pvtstrDate = "01/" + pvtintCurrMonth.ToString("00") + "/" + pvtintCurrYear;
            }

            Load_Dates();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Hide();
        }

        private void btnJan_Click(object sender, System.EventArgs e)
        {
            pvtintCurrMonth = 1;
            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/01/01";
            }
            else
            {
                pvtstrDate = "01/01/" + pvtintCurrYear;
            }

            Load_Dates();
        }

        private void btnFeb_Click(object sender, System.EventArgs e)
        {
            pvtintCurrMonth = 2;
            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/02/01";
            }
            else
            {
                pvtstrDate = "01/02/" + pvtintCurrYear;
            }

            Load_Dates();
        }

        private void btnMar_Click(object sender, System.EventArgs e)
        {
            pvtintCurrMonth = 3;
            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/03/01";
            }
            else
            {
                pvtstrDate = "01/03/" + pvtintCurrYear;
            }

            Load_Dates();
        }

        private void btnApr_Click(object sender, System.EventArgs e)
        {
            pvtintCurrMonth = 4;
            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/04/01";
            }
            else
            {
                pvtstrDate = "01/04/" + pvtintCurrYear;
            }

            Load_Dates();
        }

        private void btnMay_Click(object sender, System.EventArgs e)
        {
            pvtintCurrMonth = 5;
            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/05/01";
            }
            else
            {
                pvtstrDate = "01/05/" + pvtintCurrYear;
            }

            Load_Dates();
        }

        private void btnJun_Click(object sender, System.EventArgs e)
        {
            pvtintCurrMonth = 6;
            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/06/01";
            }
            else
            {
                pvtstrDate = "01/06/" + pvtintCurrYear;
            }

            Load_Dates();
        }

        private void btnJul_Click(object sender, System.EventArgs e)
        {
            pvtintCurrMonth = 7;
            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/07/01";
            }
            else
            {
                pvtstrDate = "01/07/" + pvtintCurrYear;
            }

            Load_Dates();
        }

        private void btnAug_Click(object sender, System.EventArgs e)
        {
            pvtintCurrMonth = 8;
            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/08/01";
            }
            else
            {
                pvtstrDate = "01/08/" + pvtintCurrYear;
            }

            Load_Dates();
        }

        private void btnSep_Click(object sender, System.EventArgs e)
        {
            pvtintCurrMonth = 9;
            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/09/01";
            }
            else
            {
                pvtstrDate = "01/09/" + pvtintCurrYear;
            }

            Load_Dates();
        }

        private void btnOct_Click(object sender, System.EventArgs e)
        {
            pvtintCurrMonth = 10;
            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/10/01";
            }
            else
            {
                pvtstrDate = "01/10/" + pvtintCurrYear;
            }

            Load_Dates();
        }

        private void btnNov_Click(object sender, System.EventArgs e)
        {
            pvtintCurrMonth = 11;
            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/11/01";
            }
            else
            {
                pvtstrDate = "01/11/" + pvtintCurrYear;
            }

            Load_Dates();
        }

        private void btnDec_Click(object sender, System.EventArgs e)
        {
            pvtintCurrMonth = 12;
            pvtintCurrDay = 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/12/01";
            }
            else
            {
                pvtstrDate = "01/12/" + pvtintCurrYear;
            }

            Load_Dates();
        }

        private void btnToday_Click(object sender, System.EventArgs e)
        {
            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = DateTime.Now.ToString("yyyy/MM/dd");
            }
            else
            {
                pvtstrDate = DateTime.Now.ToString("dd/MM/yyyy");
            }

            pvtControl.Text = pvtstrDate;
            this.Hide();
        }

        private void frmCalender_Deactivate(object sender, System.EventArgs e)
        {
            this.Hide();
        }

        private void flxgCalender_DoubleClick(object sender, System.EventArgs e)
        {
            pvtControl.Text = this.lblDate.Text;
            this.Hide();
        }

        private void cboYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            pvtintCurrYear = Convert.ToInt32(cboYear.SelectedItem.ToString());

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/" + pvtintCurrMonth.ToString("00") + "/01";
            }
            else
            {
                pvtstrDate = "01/" + pvtintCurrMonth.ToString("00") + "/" + pvtintCurrYear;
            }

            pvtblnBusyComboBox = true;

            Load_Dates();

            pvtblnBusyComboBox = false;
        }

        private void btnBlank_Click(object sender, EventArgs e)
        {
            pvtControl.Text = "";
            this.Hide();
        }

        private void frmCalenderControl_Load(object sender, EventArgs e)
        {
            this.Left = pvtintLeft;

            if (pvtblnAbove == true)
            {
                this.Top = (pvtintTop + Convert.ToInt32(AppDomain.CurrentDomain.GetData("TitleMenuAndToolbarHeight"))) - this.Height;
            }
            else
            {
                this.Top = (pvtintTop + Convert.ToInt32(AppDomain.CurrentDomain.GetData("TitleMenuAndToolbarHeight")));
            }
        }
    }
}
