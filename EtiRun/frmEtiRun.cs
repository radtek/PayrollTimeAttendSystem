using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InteractPayroll
{
    public partial class frmEtiRun : Form
    {
        clsISUtilities clsISUtilities;

        DataView pvtEmployeeDataView;

        private DataSet pvtDataSet;

        DataGridViewCellStyle EarnsTooMuchDataGridViewCellStyle;
        DataGridViewCellStyle BeyondPeriodDataGridViewCellStyle;
        DataGridViewCellStyle YoungerThanEighteenDataGridViewCellStyle;
        DataGridViewCellStyle OlderThanThirtyDataGridViewCellStyle;

        //SpreadSheet Columns
        int EmployeeNoCol = 0;
        int EmployeeCodeCol = 1;
        int EmployeeSurnameCol = 2;
        int EmployeeNameCol = 3;
        int EmployeeBirthDateCol = 4;
        int EmployeeEtiStartDateCol = 5;
        int EmployeeEtiMonthCol = 6;
        int EmployeeEtiPeriodCol = 7;
        int EmployeeTotalHoursCol = 8;
        int EmployeeFactorCol = 9;
        int EmployeeTotalAmountCol = 10;
        int EmployeeEtiAmountCol = 11;
        int EmployeeEtiCalculatedCol = 12;

        int intSelectedRowIndex = -1;

        public frmEtiRun()
        {
            InitializeComponent();
        }

        private void frmEtiRun_Load(object sender, EventArgs e)
        {
            try
            {
                clsISUtilities = new clsISUtilities(this, "busEtiRun");

                EarnsTooMuchDataGridViewCellStyle = new DataGridViewCellStyle();
                EarnsTooMuchDataGridViewCellStyle.BackColor = Color.Coral;
                EarnsTooMuchDataGridViewCellStyle.SelectionBackColor = Color.Coral;

                BeyondPeriodDataGridViewCellStyle = new DataGridViewCellStyle();
                BeyondPeriodDataGridViewCellStyle.BackColor = Color.MediumTurquoise;
                BeyondPeriodDataGridViewCellStyle.SelectionBackColor = Color.MediumTurquoise;

                YoungerThanEighteenDataGridViewCellStyle = new DataGridViewCellStyle();
                YoungerThanEighteenDataGridViewCellStyle.BackColor = Color.Lime;
                YoungerThanEighteenDataGridViewCellStyle.SelectionBackColor = Color.Lime;

                OlderThanThirtyDataGridViewCellStyle = new DataGridViewCellStyle();
                OlderThanThirtyDataGridViewCellStyle.BackColor = Color.Plum;
                OlderThanThirtyDataGridViewCellStyle.SelectionBackColor = Color.Plum;

                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);
                this.lblEmployeeExcluded.Paint += new System.Windows.Forms.PaintEventHandler(clsISUtilities.Label_Paint);

                object[] objParm = new object[3];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));

                byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Get_Form_Records", objParm);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);
                
                this.lblEarnAmount.Text = "Earns More than R" +  Convert.ToDecimal(this.pvtDataSet.Tables["EtiMaxAmount"].Rows[0]["MAX_ETI_AMOUNT"]).ToString("#####0.00");
                
                for (int intRow = 0; intRow < this.pvtDataSet.Tables["EtiRunDate"].Rows.Count; intRow++)
                {
                    cboEtiRunDate.Items.Add(Convert.ToDateTime(this.pvtDataSet.Tables["EtiRunDate"].Rows[intRow]["ETI_RUN_DATE"]).ToString("MMMM yyyy"));
                }
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }

        private void cboEtiRunDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnSave.Enabled = false;

            for (int intRow = 0; intRow < this.pvtDataSet.Tables["EtiRunDate"].Rows.Count; intRow++)
            {
                if (Convert.ToDateTime(this.pvtDataSet.Tables["EtiRunDate"].Rows[intRow]["ETI_RUN_DATE"]).ToString("MMMM yyyy") == this.cboEtiRunDate.SelectedItem.ToString())
                {
                    intSelectedRowIndex = intRow;
                    break;
                }
            }

            pvtEmployeeDataView = null;
            pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"],
                "ETI_RUN_DATE = " + Convert.ToDateTime(this.pvtDataSet.Tables["EtiRunDate"].Rows[intSelectedRowIndex]["ETI_RUN_DATE"]).ToString("yyyyMMdd"),
                "EMPLOYEE_CODE",
                DataViewRowState.CurrentRows);

            this.dgvEmployeeDataGridView.Rows.Clear();
            this.dgvEmployeeExcludedDataGridView.Rows.Clear();

            for (int intRow = 0; intRow < pvtEmployeeDataView.Count; intRow++)
            {
                if (pvtEmployeeDataView[intRow]["INCLUDED_IN_RUN"].ToString() == "Y")
                {
                    this.btnSave.Enabled = true;

                    this.dgvEmployeeDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                                          pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                           pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                          pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                         
                                                          Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_BIRTHDATE"]).ToString("dd MMMM yyyy"),
                                                          Convert.ToDateTime(pvtEmployeeDataView[intRow]["ETI_START_DATE"]).ToString("MMMM yyyy"),
                                                          pvtEmployeeDataView[intRow]["ETI_MONTH"].ToString(),
                                                          pvtEmployeeDataView[intRow]["ETI_PERIOD"].ToString(),
                                                          Convert.ToDecimal(pvtEmployeeDataView[intRow]["TOTAL_HOURS"]).ToString("###0.00"),
                                                          Convert.ToDecimal(pvtEmployeeDataView[intRow]["CALC_FACTOR"]).ToString("###0.0000"),
                                                          Convert.ToDecimal(pvtEmployeeDataView[intRow]["TOTAL_AMOUNT"]).ToString("####0.00"),
                                                          Convert.ToDecimal(pvtEmployeeDataView[intRow]["ETI_AMOUNT"]).ToString("####0.00"),
                                                          Convert.ToDecimal(pvtEmployeeDataView[intRow]["ETI_CALCULATED"]).ToString("####0.00"),
                                                          Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"]).ToString("00000"),
                                                          Convert.ToDateTime(pvtEmployeeDataView[intRow]["ETI_START_DATE"]).ToString("yyyyMM"),

                                                          Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_BIRTHDATE"]).ToString("yyyyMMdd"));
                }
                else
                {
                    this.dgvEmployeeExcludedDataGridView.Rows.Add(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"].ToString(),
                                                                  pvtEmployeeDataView[intRow]["EMPLOYEE_CODE"].ToString(),
                                                                    pvtEmployeeDataView[intRow]["EMPLOYEE_SURNAME"].ToString(),
                                                                  pvtEmployeeDataView[intRow]["EMPLOYEE_NAME"].ToString(),
                                                                
                                                                  Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_BIRTHDATE"]).ToString("dd MMMM yyyy"),
                                                                  Convert.ToDateTime(pvtEmployeeDataView[intRow]["ETI_START_DATE"]).ToString("MMMM yyyy"),
                                                                  pvtEmployeeDataView[intRow]["ETI_MONTH"].ToString(),
                                                                  pvtEmployeeDataView[intRow]["ETI_PERIOD"].ToString(),
                                                                  Convert.ToDecimal(pvtEmployeeDataView[intRow]["TOTAL_HOURS"]).ToString("###0.00"),
                                                                  Convert.ToDecimal(pvtEmployeeDataView[intRow]["CALC_FACTOR"]).ToString("###0.0000"),
                                                                  Convert.ToDecimal(pvtEmployeeDataView[intRow]["TOTAL_AMOUNT"]).ToString("####0.00"),
                                                                  Convert.ToDecimal(pvtEmployeeDataView[intRow]["ETI_AMOUNT"]).ToString("####0.00"),
                                                                  Convert.ToDecimal(pvtEmployeeDataView[intRow]["ETI_CALCULATED"]).ToString("####0.00"),
                                                                  Convert.ToInt32(pvtEmployeeDataView[intRow]["EMPLOYEE_NO"]).ToString("00000"),
                                                                  Convert.ToDateTime(pvtEmployeeDataView[intRow]["ETI_START_DATE"]).ToString("yyyyMM"),
                                                                  Convert.ToDateTime(pvtEmployeeDataView[intRow]["EMPLOYEE_BIRTHDATE"]).ToString("yyyyMMdd"));

                    string myValue = pvtEmployeeDataView[intRow]["INCLUDED_IN_RUN"].ToString();

                    if (pvtEmployeeDataView[intRow]["INCLUDED_IN_RUN"].ToString() == "N")
                    {
                        this.dgvEmployeeExcludedDataGridView.Rows[this.dgvEmployeeExcludedDataGridView.Rows.Count - 1].HeaderCell.Style = BeyondPeriodDataGridViewCellStyle;
                    }
                    else
                    {
                        if (pvtEmployeeDataView[intRow]["INCLUDED_IN_RUN"].ToString() == "E")
                        {
                            this.dgvEmployeeExcludedDataGridView.Rows[this.dgvEmployeeExcludedDataGridView.Rows.Count - 1].HeaderCell.Style = YoungerThanEighteenDataGridViewCellStyle;
                        }
                        else
                        {
                            if (pvtEmployeeDataView[intRow]["INCLUDED_IN_RUN"].ToString() == "T")
                            {
                                this.dgvEmployeeExcludedDataGridView.Rows[this.dgvEmployeeExcludedDataGridView.Rows.Count - 1].HeaderCell.Style = OlderThanThirtyDataGridViewCellStyle;
                            }
                            else
                            {
                                this.dgvEmployeeExcludedDataGridView.Rows[this.dgvEmployeeExcludedDataGridView.Rows.Count - 1].HeaderCell.Style = EarnsTooMuchDataGridViewCellStyle;
                            }
                        }
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet TempDataSet = new DataSet();

                TempDataSet.Tables.Add(this.pvtDataSet.Tables["EtiEmployeeSave"].Clone());
                
                for (int intRow = 0; intRow < this.dgvEmployeeDataGridView.Rows.Count; intRow++)
                {
                    DataRow DataRow = TempDataSet.Tables["EtiEmployeeSave"].NewRow();

                    DataRow["EMPLOYEE_NO"] = Convert.ToInt32(this.dgvEmployeeDataGridView[EmployeeNoCol, intRow].Value);
                    DataRow["ETI_MONTH"] = Convert.ToInt32(this.dgvEmployeeDataGridView[EmployeeEtiMonthCol, intRow].Value);
                    DataRow["TOTAL_HOURS"] = Convert.ToDecimal(this.dgvEmployeeDataGridView[EmployeeTotalHoursCol, intRow].Value);
                    DataRow["FACTOR"] = Convert.ToDecimal(this.dgvEmployeeDataGridView[EmployeeFactorCol, intRow].Value);
                    DataRow["TOTAL_EARNINGS"] = Convert.ToDecimal(this.dgvEmployeeDataGridView[EmployeeTotalAmountCol, intRow].Value);
                    DataRow["ETI_EARNINGS"] = Convert.ToDecimal(this.dgvEmployeeDataGridView[EmployeeEtiAmountCol, intRow].Value);
                    DataRow["ETI_VALUE"] = Convert.ToDecimal(this.dgvEmployeeDataGridView[EmployeeEtiCalculatedCol, intRow].Value);

                    TempDataSet.Tables["EtiEmployeeSave"].Rows.Add(DataRow);
                }

                byte[] pvtbytCompress = clsISUtilities.Compress_DataSet(TempDataSet);

                object[] objParm = new object[5];
                objParm[0] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("CompanyNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();
                objParm[2] = Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[3] = Convert.ToDateTime(this.pvtDataSet.Tables["EtiRunDate"].Rows[intSelectedRowIndex]["ETI_RUN_DATE"]).ToString("yyyy-MM-dd");
                objParm[4] = pvtbytCompress;

                byte[] bytCompress = (byte[])clsISUtilities.DynamicFunction("Insert_Eti_Records", objParm,true);

                pvtDataSet = clsISUtilities.DeCompress_Array_To_DataSet(bytCompress);
                
                cboEtiRunDate.Items.Clear();
                this.dgvEmployeeDataGridView.Rows.Clear();
                this.dgvEmployeeExcludedDataGridView.Rows.Clear();

                for (int intRow = 0; intRow < this.pvtDataSet.Tables["EtiRunDate"].Rows.Count; intRow++)
                {
                    cboEtiRunDate.Items.Add(Convert.ToDateTime(this.pvtDataSet.Tables["EtiRunDate"].Rows[intRow]["ETI_RUN_DATE"]).ToString("MMMM yyyy"));
                }

                this.btnSave.Enabled = false;
            }
            catch (Exception eException)
            {
                clsISUtilities.ErrorHandler(eException);
            }
        }
        
        private void dgvEmployeeExcludedDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 0)
            {
                if (double.Parse(dgvEmployeeExcludedDataGridView[12, e.RowIndex1].Value.ToString()) > double.Parse(dgvEmployeeExcludedDataGridView[12, e.RowIndex2].Value.ToString()))
                {
                    e.SortResult = 1;
                }
                else
                {
                    if (double.Parse(dgvEmployeeExcludedDataGridView[12, e.RowIndex1].Value.ToString()) < double.Parse(dgvEmployeeExcludedDataGridView[12, e.RowIndex2].Value.ToString()))
                    {
                        e.SortResult = -1;
                    }
                    else
                    {
                        e.SortResult = 0;
                    }
                }
         
                e.Handled = true;
            }
            else
            {
                if (e.Column.Index == 5)
                {
                    if (double.Parse(dgvEmployeeExcludedDataGridView[13, e.RowIndex1].Value.ToString()) > double.Parse(dgvEmployeeExcludedDataGridView[13, e.RowIndex2].Value.ToString()))
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvEmployeeExcludedDataGridView[13, e.RowIndex1].Value.ToString()) < double.Parse(dgvEmployeeExcludedDataGridView[13, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = -1;
                        }
                        else
                        {
                            e.SortResult = 0;
                        }
                    }

                    e.Handled = true;
                }
                else
                {
                    if (e.Column.Index == 4)
                    {
                        if (double.Parse(dgvEmployeeExcludedDataGridView[14, e.RowIndex1].Value.ToString()) > double.Parse(dgvEmployeeExcludedDataGridView[14, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvEmployeeExcludedDataGridView[14, e.RowIndex1].Value.ToString()) < double.Parse(dgvEmployeeExcludedDataGridView[14, e.RowIndex2].Value.ToString()))
                            {
                                e.SortResult = -1;
                            }
                            else
                            {
                                e.SortResult = 0;
                            }
                        }

                        e.Handled = true;
                    }

                }
            }
        }

        private void dgvEmployeeDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 0)
            {
                if (double.Parse(dgvEmployeeDataGridView[12, e.RowIndex1].Value.ToString()) > double.Parse(dgvEmployeeDataGridView[12, e.RowIndex2].Value.ToString()))
                {
                    e.SortResult = 1;
                }
                else
                {
                    if (double.Parse(dgvEmployeeDataGridView[12, e.RowIndex1].Value.ToString()) < double.Parse(dgvEmployeeDataGridView[12, e.RowIndex2].Value.ToString()))
                    {
                        e.SortResult = -1;
                    }
                    else
                    {
                        e.SortResult = 0;
                    }
                }

                e.Handled = true;
            }
            else
            {
                if (e.Column.Index == 5)
                {
                    if (double.Parse(dgvEmployeeDataGridView[13, e.RowIndex1].Value.ToString()) > double.Parse(dgvEmployeeDataGridView[13, e.RowIndex2].Value.ToString()))
                    {
                        e.SortResult = 1;
                    }
                    else
                    {
                        if (double.Parse(dgvEmployeeDataGridView[13, e.RowIndex1].Value.ToString()) < double.Parse(dgvEmployeeDataGridView[13, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = -1;
                        }
                        else
                        {
                            e.SortResult = 0;
                        }
                    }

                    e.Handled = true;
                }
                else
                {
                    if (e.Column.Index == 4)
                    {
                        if (double.Parse(dgvEmployeeDataGridView[14, e.RowIndex1].Value.ToString()) > double.Parse(dgvEmployeeDataGridView[14, e.RowIndex2].Value.ToString()))
                        {
                            e.SortResult = 1;
                        }
                        else
                        {
                            if (double.Parse(dgvEmployeeDataGridView[14, e.RowIndex1].Value.ToString()) < double.Parse(dgvEmployeeDataGridView[14, e.RowIndex2].Value.ToString()))
                            {
                                e.SortResult = -1;
                            }
                            else
                            {
                                e.SortResult = 0;
                            }
                        }

                        e.Handled = true;
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
