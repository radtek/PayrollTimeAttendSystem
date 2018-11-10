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
    public partial class frmCalender : Form
    {
        clsISUtilities clsISUtilities;

        private int pvtintCurrDay = 0;
        private int pvtintCurrMonth = 0;
        private int pvtintCurrYear = 0;
        private bool pvtblnCalenderDataGridViewLoaded = true;

        public MenuItem pubmniMenuItem;

        private string pvtstrDate;
        private string pvtstrDateFormat;

        DateTime dtDate;

        DataSet pvtDataSet;
        DataView pvtDataView;

        DataGridViewCellStyle SatSunDataGridViewCellStyle;
        DataGridViewCellStyle PublicHolidayDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;
        DataGridViewCellStyle LightDataGridViewCellStyle;

        private int pvtintSavedRowIndex = -1;
        private int pvtintSavedColumnIndex = -1;

        public frmCalender()
        {
            InitializeComponent();

            pvtstrDateFormat = AppDomain.CurrentDomain.GetData("DateFormat").ToString();
        }

        private void frmCalender_Load(object sender, System.EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busCalender");

                object[] objParm = new object[1];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));

                byte[] pvtbytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(pvtbytCompress);

                for (int intYear = 2007; intYear < DateTime.Now.Year + 2; intYear++)
                {
                    this.cboYear.Items.Add(intYear.ToString());
                }
                
                for (int intRow = 0; intRow < 6; intRow++)
                {
                    this.dgvCalenderDataGridView.Rows.Add("",
                                                          "",
                                                          "",
                                                          "",
                                                          "",
                                                          "",
                                                          "",
                                                          "",
                                                          "");
                }

                pvtblnCalenderDataGridViewLoaded = true;

                this.btnToday.Text = DateTime.Now.ToString(pvtstrDateFormat);

                SatSunDataGridViewCellStyle = new DataGridViewCellStyle();
                SatSunDataGridViewCellStyle.ForeColor = Color.Navy;
                SatSunDataGridViewCellStyle.SelectionForeColor = Color.Navy;

                PublicHolidayDataGridViewCellStyle = new DataGridViewCellStyle();
                PublicHolidayDataGridViewCellStyle.ForeColor = Color.Red;
                PublicHolidayDataGridViewCellStyle.SelectionForeColor = Color.Red;

                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.ForeColor = Color.Black;
                NormalDataGridViewCellStyle.SelectionForeColor = Color.Black;

                LightDataGridViewCellStyle = new DataGridViewCellStyle();
                LightDataGridViewCellStyle.ForeColor = Color.Gray;
                LightDataGridViewCellStyle.SelectionForeColor = Color.Gray;

                Set_Date(DateTime.Now.ToString(pvtstrDateFormat));
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        public void Set_Date(string parstrDateTime)
        {
            pvtstrDate = parstrDateTime;

            Load_Dates();
        }

        private void Load_Dates()
        {
            int intCurrSavedCol = 0;
            int intCurrSavedRow = 0;

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

            DateTime dtCCCCYYMM01 = new DateTime(pvtintCurrYear, pvtintCurrMonth, 1);
            int intDayOfWeek = Convert.ToInt32(dtCCCCYYMM01.DayOfWeek);

            int intCol = 0;
            int intRow = 0;

            this.lblPaidDate.Text = "";

            dtDate = new DateTime(pvtintCurrYear, pvtintCurrMonth, 1).AddDays(-1);

            for (intCol = intDayOfWeek; intCol > 0; intCol--)
            {
                this.dgvCalenderDataGridView[intCol, 0].Value = dtDate.Day.ToString();

                dtDate = dtDate.AddDays(-1);

                this.dgvCalenderDataGridView[intCol, intRow].Style = this.LightDataGridViewCellStyle;
            }
           
            dtDate = new DateTime(pvtintCurrYear, pvtintCurrMonth, 1);

            while (true)
            {
                this.dgvCalenderDataGridView[Convert.ToInt32(dtDate.DayOfWeek) + 1, intRow].Value = dtDate.Day.ToString();

                if (dtDate.Day == pvtintCurrDay)
                {
                    intCurrSavedCol = Convert.ToInt32(dtDate.DayOfWeek + 1);
                    intCurrSavedRow = intRow;
                }

                pvtDataView = null;
                pvtDataView = new DataView(pvtDataSet.Tables["PaidHoliday"]
                       , "PUBLIC_HOLIDAY_DATE = '" + dtDate.ToString("yyyy-MM-dd") + "'"
                        , ""
                        , DataViewRowState.CurrentRows);

                if (pvtDataView.Count > 0)
                {
                    this.dgvCalenderDataGridView[Convert.ToInt32(dtDate.DayOfWeek) + 1, intRow].Style = this.PublicHolidayDataGridViewCellStyle;
                }
                else
                {
                    if (Convert.ToInt32(dtDate.DayOfWeek) != 0
                        & Convert.ToInt32(dtDate.DayOfWeek) != 6)
                    {
                        this.dgvCalenderDataGridView[Convert.ToInt32(dtDate.DayOfWeek) + 1, intRow].Style = this.NormalDataGridViewCellStyle;
                    }
                    else
                    {
                        this.dgvCalenderDataGridView[Convert.ToInt32(dtDate.DayOfWeek) + 1, intRow].Style = this.SatSunDataGridViewCellStyle;
                    }
                }

                if (Convert.ToInt32(dtDate.DayOfWeek) == 6)
                {
                    intRow = intRow + 1;
                }

                dtDate = dtDate.AddDays(1);

                if (dtDate.Day == 1)
                {
                    break;
                }
            }

            intCol = Convert.ToInt32(dtDate.DayOfWeek + 1);

            for (intRow = intRow; intRow < 6; intRow++)
            {
                for (intCol = intCol; intCol < 8; intCol++)
                {
                    this.dgvCalenderDataGridView[intCol, intRow].Value = dtDate.Day.ToString();

                    pvtDataView = null;
                    pvtDataView = new DataView(pvtDataSet.Tables["PaidHoliday"]
                       , "PUBLIC_HOLIDAY_DATE = '" + dtDate.ToString("yyyy-MM-dd") + "'"
                        , ""
                        , DataViewRowState.CurrentRows);

                    if (pvtDataView.Count > 0)
                    {
                        this.dgvCalenderDataGridView[intCol, intRow].Style = this.PublicHolidayDataGridViewCellStyle;
                    }
                    else
                    {
                        this.dgvCalenderDataGridView[intCol, intRow].Style = this.LightDataGridViewCellStyle;
                    }

                    dtDate = dtDate.AddDays(1);
                }

                intCol = 1;
            }

            this.lblMonth.Text = dtCCCCYYMM01.ToString("MMMM");

            this.cboYear.SelectedItem = dtCCCCYYMM01.ToString("yyyy");

            dgvCalenderDataGridView.CurrentCell = dgvCalenderDataGridView[intCurrSavedCol, intCurrSavedRow];

            dgvCalenderDataGridView.Focus();
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

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnYearDown_Click(object sender, System.EventArgs e)
        {
            pvtintCurrYear = pvtintCurrYear - 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/" + pvtintCurrMonth.ToString("00") + "/01";
            }
            else
            {
                pvtstrDate = "01/" + pvtintCurrMonth.ToString("00") + "/" + pvtintCurrYear;
            }

            pvtintCurrDay = 1;

            Load_Dates();
        }

        private void btnYearUp_Click(object sender, System.EventArgs e)
        {
            pvtintCurrYear = pvtintCurrYear + 1;

            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
            {
                pvtstrDate = pvtintCurrYear + "/" + pvtintCurrMonth.ToString("00") + "/01";
            }
            else
            {
                pvtstrDate = "01/" + pvtintCurrMonth.ToString("00") + "/" + pvtintCurrYear;
            }

            pvtintCurrDay = 1;

            Load_Dates();
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
       
        private void cboYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            pvtintCurrYear = Convert.ToInt32(cboYear.SelectedItem.ToString());

            if (pvtblnCalenderDataGridViewLoaded == true)
            {
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
        }

        private void dgvCalenderDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCalenderDataGridView.Rows.Count > 0
                & pvtblnCalenderDataGridViewLoaded == true)
            {
                if (e.RowIndex > -1
                    & e.ColumnIndex > -1)
                {
                    if (this.dgvCalenderDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString() != "")
                    {
                        pvtintSavedRowIndex = e.RowIndex;
                        pvtintSavedColumnIndex = e.ColumnIndex;

                        pvtintCurrDay = Convert.ToInt32(this.dgvCalenderDataGridView[e.ColumnIndex, e.RowIndex].Value);

                        if (pvtintCurrDay > 20
                            & e.RowIndex == 0)
                        {
                            pvtintCurrMonth -= 1;

                            if (pvtintCurrMonth == 0)
                            {
                                pvtintCurrMonth = 12;
                                this.pvtintCurrYear -= 1;
                            }

                            if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
                            {
                                pvtstrDate = pvtintCurrYear + "/" + pvtintCurrMonth.ToString("00") + "/" + pvtintCurrDay.ToString("00");
                            }
                            else
                            {
                                pvtstrDate = pvtintCurrDay.ToString("00") + "/" + pvtintCurrMonth.ToString("00") + "/" + pvtintCurrYear;
                            }

                            tmrTimer.Enabled = true;
                        }
                        else
                        {
                            if (pvtintCurrDay < 16
                            & e.RowIndex > 3)
                            {
                                pvtintCurrMonth += 1;

                                if (pvtintCurrMonth == 13)
                                {
                                    pvtintCurrMonth = 1;
                                    pvtintCurrYear += 1;
                                }

                                if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
                                {
                                    pvtstrDate = pvtintCurrYear + "/" + pvtintCurrMonth.ToString("00") + "/" + pvtintCurrDay.ToString("00");
                                }
                                else
                                {
                                    pvtstrDate = pvtintCurrDay.ToString("00") + "/" + pvtintCurrMonth.ToString("00") + "/" + pvtintCurrYear;
                                }

                                tmrTimer.Enabled = true;
                            }
                            else
                            {
                                if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
                                {
                                    pvtstrDate = pvtintCurrYear.ToString("0000") + "/" + pvtintCurrMonth.ToString("00") + "/" + pvtintCurrDay.ToString("00");
                                }
                                else
                                {
                                    pvtstrDate = pvtintCurrDay.ToString("00") + "/" + pvtintCurrMonth.ToString("00") + "/" + pvtintCurrYear.ToString("0000");
                                }

                                dtDate = new DateTime(pvtintCurrYear, pvtintCurrMonth, pvtintCurrDay);

                                pvtDataView = null;
                                pvtDataView = new DataView(pvtDataSet.Tables["PaidHoliday"]
                                       , "PUBLIC_HOLIDAY_DATE = '" + dtDate.ToString("yyyy-MM-dd") + "'"
                                        , ""
                                        , DataViewRowState.CurrentRows);

                                if (pvtDataView.Count > 0)
                                {
                                    this.lblPaidDate.Text = this.pvtDataView[0]["PUBLIC_HOLIDAY_DESC"].ToString();
                                }
                                else
                                {
                                    this.lblPaidDate.Text = "";
                                }

                                this.lblDate.Text = pvtstrDate.ToString();
                            }
                        }
                    }
                    else
                    {
                        if (e.ColumnIndex == 0)
                        {
                            if (this.dgvCalenderDataGridView[1, e.RowIndex].Value.ToString() != "")
                            {
                                pvtintCurrDay = Convert.ToInt32(this.dgvCalenderDataGridView[1, e.RowIndex].Value);

                                dtDate = new DateTime(pvtintCurrYear, pvtintCurrMonth, pvtintCurrDay).AddDays(-1);

                                if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
                                {
                                    pvtstrDate = dtDate.Year.ToString("0000") + "/" + dtDate.Month.ToString("00") + "/" + dtDate.Day.ToString("00");
                                }
                                else
                                {
                                    pvtstrDate = dtDate.Day.ToString("00") + "/" + dtDate.Month.ToString("00") + "/" + dtDate.Year.ToString("0000");
                                }

                                this.tmrTimer.Enabled = true;
                            }
                        }
                        else
                        {
                            if (e.ColumnIndex == 8)
                            {
                                if (this.dgvCalenderDataGridView[7, e.RowIndex].Value.ToString() != "")
                                {
                                    pvtintCurrDay = Convert.ToInt32(this.dgvCalenderDataGridView[7, e.RowIndex].Value);

                                    dtDate = new DateTime(pvtintCurrYear, pvtintCurrMonth, pvtintCurrDay).AddDays(1);

                                    if (pvtstrDateFormat.Substring(0, 4).ToUpper() == "YYYY")
                                    {
                                        pvtstrDate = dtDate.Year.ToString("0000") + "/" + dtDate.Month.ToString("00") + "/" + dtDate.Day.ToString("00");
                                    }
                                    else
                                    {
                                        pvtstrDate = dtDate.Day.ToString("00") + "/" + dtDate.Month.ToString("00") + "/" + dtDate.Year.ToString("0000");
                                    }

                                    this.tmrTimer.Enabled = true;
                                }
                            }

                        }
                    }
                }
            }
        }

        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            tmrTimer.Enabled = false;

            Load_Dates();
        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            pvtstrDate = this.btnToday.Text;

            Load_Dates();
        }
    }
}
