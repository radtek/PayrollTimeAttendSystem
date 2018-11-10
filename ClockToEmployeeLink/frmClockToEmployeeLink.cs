using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InteractPayrollClient
{
    public partial class frmClockToEmployeeLink : Form
    {
        clsISClientUtilities clsISClientUtilities;

        ToolStripMenuItem miLinkedMenuItem;

        private DataSet pvtDataSet;
        private DataView pvtEmployeeDataView;

        private Int64 pvtint64CompanyNo = -1;
        private int pvtintDeviceNo = -1;

        private bool pvtblnCompanyDataGridViewLoaded = false;
        private bool pvtblnDeviceDataGridViewLoaded = false;

        DataGridViewCellStyle NoTemplateDataGridViewCellStyle;
        DataGridViewCellStyle UseEmployeeNoDataGridViewCellStyle;
        DataGridViewCellStyle NormalDataGridViewCellStyle;
        DataGridViewCellStyle LocalTemplateDataGridViewCellStyle;
        DataGridViewCellStyle ServerTemplateDataGridViewCellStyle;

        public frmClockToEmployeeLink()
        {
            InitializeComponent();

            if (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 900
            && AppDomain.CurrentDomain.GetData("FormSmallest") == null)
            {
                this.Height += 114;

                this.dgvEmployeeDataGridView.Height += 114;

                this.grbRowLegend.Top += 114;
                this.grbRowFilter.Top += 114;
                this.grbFingerPrintsMissing.Top += 114;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmClockToEmployeeLink_Load(object sender, EventArgs e)
        {
            try
            {
                clsISClientUtilities = new clsISClientUtilities(this,"busClockToEmployeeLink");

                this.lblCompany.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblClockingDevice.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblEmployee.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);

                miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

                NoTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                NoTemplateDataGridViewCellStyle.BackColor = Color.Yellow;
                NoTemplateDataGridViewCellStyle.SelectionBackColor = Color.Yellow;

                UseEmployeeNoDataGridViewCellStyle = new DataGridViewCellStyle();
                UseEmployeeNoDataGridViewCellStyle.BackColor = Color.Orange;
                UseEmployeeNoDataGridViewCellStyle.SelectionBackColor = Color.Orange;

                NormalDataGridViewCellStyle = new DataGridViewCellStyle();
                NormalDataGridViewCellStyle.BackColor = SystemColors.Control;
                NormalDataGridViewCellStyle.SelectionBackColor = SystemColors.Control;

                LocalTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                LocalTemplateDataGridViewCellStyle.BackColor = Color.SeaGreen;
                LocalTemplateDataGridViewCellStyle.SelectionBackColor = Color.SeaGreen;

                ServerTemplateDataGridViewCellStyle = new DataGridViewCellStyle();
                ServerTemplateDataGridViewCellStyle.BackColor = Color.Aquamarine;
                ServerTemplateDataGridViewCellStyle.SelectionBackColor = Color.Aquamarine;

                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Form_Records", objParm,false);
                pvtDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);
#if(DEBUG)
                int intWidth = 0;
                int intHeight = 0;
                int intNewHeight = 0;

                DataGridView myDataGridView;

                foreach (Control myControl in this.Controls)
                {
                    if (myControl is DataGridView)
                    {
                        myDataGridView = null;
                        myDataGridView = (DataGridView)myControl;

                        intWidth = myDataGridView.RowHeadersWidth;

                        if (myDataGridView.ScrollBars == ScrollBars.Vertical
                            | myDataGridView.ScrollBars == ScrollBars.Both)
                        {
                            intWidth += 19;
                        }

                        for (int intCol = 0; intCol < myDataGridView.ColumnCount; intCol++)
                        {
                            if (myDataGridView.Columns[intCol].Visible == true)
                            {
                                intWidth += myDataGridView.Columns[intCol].Width;
                            }
                        }

                        if (intWidth != myDataGridView.Width)
                        {
                            System.Windows.Forms.MessageBox.Show(myDataGridView.Name + " Width should be " + intWidth.ToString());
                        }

                        intHeight = myDataGridView.ColumnHeadersHeight + 2;
                        intNewHeight = myDataGridView.RowTemplate.Height / 2;

                        for (int intRow = 0; intRow < 200; intRow++)
                        {
                            intHeight += myDataGridView.RowTemplate.Height;

                            if (intHeight == myDataGridView.Height)
                            {
                                break;
                            }
                            else
                            {
                                if (intHeight > myDataGridView.Height)
                                {
                                    System.Windows.Forms.MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                    break;
                                }
                                else
                                {

                                    if (intHeight + intNewHeight > myDataGridView.Height)
                                    {
                                        System.Windows.Forms.MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (myControl is GroupBox)
                        {
                            foreach (Control myControl1 in myControl.Controls)
                            {
                                if (myControl1 is DataGridView)
                                {
                                    myDataGridView = null;
                                    myDataGridView = (DataGridView)myControl1;

                                    intWidth = myDataGridView.RowHeadersWidth;

                                    if (myDataGridView.ScrollBars == ScrollBars.Vertical
                                        | myDataGridView.ScrollBars == ScrollBars.Both)
                                    {
                                        intWidth += 19;
                                    }

                                    for (int intCol = 0; intCol < myDataGridView.ColumnCount; intCol++)
                                    {
                                        if (myDataGridView.Columns[intCol].Visible == true)
                                        {
                                            intWidth += myDataGridView.Columns[intCol].Width;
                                        }
                                    }

                                    if (intWidth != myDataGridView.Width)
                                    {
                                        System.Windows.Forms.MessageBox.Show(myDataGridView.Name + " Width should be " + intWidth.ToString());
                                    }

                                    intHeight = myDataGridView.ColumnHeadersHeight + 2;
                                    intNewHeight = myDataGridView.RowTemplate.Height / 2;

                                    for (int intRow = 0; intRow < 200; intRow++)
                                    {
                                        intHeight += myDataGridView.RowTemplate.Height;

                                        if (intHeight == myDataGridView.Height)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            if (intHeight > myDataGridView.Height)
                                            {
                                                System.Windows.Forms.MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                                break;
                                            }
                                            else
                                            {

                                                if (intHeight + intNewHeight > myDataGridView.Height)
                                                {
                                                    System.Windows.Forms.MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
#endif
                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void Load_CurrentForm_Records()
        {
            Clear_DataGridView(dgvCompanyDataGridView);

            pvtblnCompanyDataGridViewLoaded = false;

            for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["Company"].Rows.Count; intRowCount++)
            {
                this.dgvCompanyDataGridView.Rows.Add(pvtDataSet.Tables["Company"].Rows[intRowCount]["COMPANY_DESC"].ToString(),
                                                     intRowCount.ToString());
            }

            pvtblnCompanyDataGridViewLoaded = true;

            if (this.dgvCompanyDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
            }
        }

        public int Get_DataGridView_SelectedRowIndex(DataGridView myDataGridView)
        {
            int intReturnIndex = -1;

            if (myDataGridView.SelectedRows.Count > 0)
            {
                if (myDataGridView.SelectedRows[0].Selected == true)
                {
                    intReturnIndex = myDataGridView.SelectedRows[0].Index;
                }
            }

            return intReturnIndex;
        }

        public void Set_DataGridView_SelectedRowIndex(DataGridView myDataGridView, int intRow)
        {
            //Fires DataGridView RowEnter Function
            if (myDataGridView.CurrentCell.RowIndex == intRow)
            {
                DataGridViewCellEventArgs myDataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, intRow);

                switch (myDataGridView.Name)
                {
                    case "dgvCompanyDataGridView":

                        this.dgvCompanyDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvDeviceDataGridView":

                        this.dgvDeviceDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    default:

                        System.Windows.Forms.MessageBox.Show("No Entry for " + myDataGridView.Name + " in Set_DataGridView_SelectedRowIndex", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                myDataGridView.CurrentCell = myDataGridView[0, intRow];
            }
        }

        private void Clear_DataGridView(DataGridView myDataGridView)
        {
            myDataGridView.Rows.Clear();

            if (myDataGridView.SortedColumn != null)
            {
                myDataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
        }

        private void frmClockToEmployeeLink_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }

        private void dgvCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvCompanyDataGridView.Rows.Count > 0
                    & pvtblnCompanyDataGridViewLoaded == true)
                {
                    grbFingerPrintsMissing.Visible = false;
                    
                    int intIndex = Convert.ToInt32(dgvCompanyDataGridView[1, e.RowIndex].Value);

                    pvtint64CompanyNo = Convert.ToInt64(pvtDataSet.Tables["Company"].Rows[intIndex]["COMPANY_NO"]);

                    DataView myCompanyDataView = new DataView(pvtDataSet.Tables["Employee"], "COMPANY_NO = " + pvtint64CompanyNo, "", DataViewRowState.CurrentRows);

                    if (myCompanyDataView.Count == 0)
                    {
                        object[] objParm = new object[1];
                        objParm[0] = pvtint64CompanyNo;

                        byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Company_Records", objParm, false);
                        DataSet TempDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                        pvtDataSet.Merge(TempDataSet);
                    }

                    Clear_DataGridView(dgvEmployeeDataGridView);
                    Clear_DataGridView(dgvDeviceDataGridView);

                    this.pvtblnDeviceDataGridViewLoaded = false;
                    string strDeviceType = "";

                    for (int intRowCount = 0; intRowCount < pvtDataSet.Tables["Device"].Rows.Count; intRowCount++)
                    {
                        DataView myNoTemplateDataView = new DataView(pvtDataSet.Tables["Employee"], "DEVICE_NO = " + pvtDataSet.Tables["Device"].Rows[intRowCount]["DEVICE_NO"].ToString() + " AND COMPANY_NO = " + pvtint64CompanyNo + " AND HAS_FINGERPRINTS_AT_SERVER_IND = 'N' AND HAS_FINGERPRINTS_AT_LOCAL_IND = 'N'", "", DataViewRowState.CurrentRows);
                        DataView myLocalDataView = new DataView(pvtDataSet.Tables["Employee"], "DEVICE_NO = " + pvtDataSet.Tables["Device"].Rows[intRowCount]["DEVICE_NO"].ToString() + " AND COMPANY_NO = " + pvtint64CompanyNo + " AND HAS_FINGERPRINTS_AT_LOCAL_IND = 'Y'", "", DataViewRowState.CurrentRows);
                        DataView myServerDataView = new DataView(pvtDataSet.Tables["Employee"], "DEVICE_NO = " + pvtDataSet.Tables["Device"].Rows[intRowCount]["DEVICE_NO"].ToString() + " AND COMPANY_NO = " + pvtint64CompanyNo + " AND HAS_FINGERPRINTS_AT_SERVER_IND = 'Y'", "", DataViewRowState.CurrentRows);
                        DataView myUseEmployeeNoDataView = new DataView(pvtDataSet.Tables["Employee"], "DEVICE_NO = " + pvtDataSet.Tables["Device"].Rows[intRowCount]["DEVICE_NO"].ToString() + " AND COMPANY_NO = " + pvtint64CompanyNo + "  AND USE_EMPLOYEE_NO_IND = 'Y'", "", DataViewRowState.CurrentRows);

                        if (this.rbnTemplateMissing.Checked == true)
                        {
                            if (myNoTemplateDataView.Count == 0)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (this.rbnLocal.Checked == true)
                            {
                                if (myLocalDataView.Count == 0)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (this.rbnServer.Checked == true)
                                {
                                    if (myServerDataView.Count == 0)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (this.rbnKeyEmployeeNo.Checked == true)
                                    {
                                        if (myUseEmployeeNoDataView.Count == 0)
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                        }

                        if (pvtDataSet.Tables["Device"].Rows[intRowCount]["DEVICE_USAGE"].ToString() == "T")
                        {
                            strDeviceType = "Time & Attendance";
                        }
                        else
                        {
                            if (pvtDataSet.Tables["Device"].Rows[intRowCount]["DEVICE_USAGE"].ToString() == "A")
                            {
                                strDeviceType = "Access";
                            }
                            else
                            {
                                strDeviceType = "Access / Time & Attendance";
                            }
                        }

                        this.dgvDeviceDataGridView.Rows.Add("",
                                                            "",
                                                            "",
                                                             "",
                                                            pvtDataSet.Tables["Device"].Rows[intRowCount]["DEVICE_DESC"].ToString(),
                                                            strDeviceType,
                                                            pvtDataSet.Tables["Device"].Rows[intRowCount]["DEVICE_NO"].ToString(),
                                                            intRowCount.ToString());

                        if (this.rbnAll.Checked == true
                        || this.rbnTemplateMissing.Checked == true)
                        {
                            if (myNoTemplateDataView.Count > 0)
                            {
                                dgvDeviceDataGridView[0,dgvDeviceDataGridView.Rows.Count - 1].Style = NoTemplateDataGridViewCellStyle;
                            }
                        }

                        if (this.rbnAll.Checked == true
                        || this.rbnServer.Checked == true)
                        {
                            if (myServerDataView.Count > 0)
                            {
                                dgvDeviceDataGridView[1, dgvDeviceDataGridView.Rows.Count - 1].Style = ServerTemplateDataGridViewCellStyle;
                            }
                        }

                        if (this.rbnAll.Checked == true
                        || this.rbnLocal.Checked == true)
                        {
                            if (myLocalDataView.Count > 0)
                            {
                                dgvDeviceDataGridView[2, dgvDeviceDataGridView.Rows.Count - 1].Style = LocalTemplateDataGridViewCellStyle;
                            }
                        }
                        
                        if (this.rbnAll.Checked == true
                        || this.rbnKeyEmployeeNo.Checked == true)
                        {
                            if (myUseEmployeeNoDataView.Count > 0)
                            {
                                dgvDeviceDataGridView[3, dgvDeviceDataGridView.Rows.Count - 1].Style = UseEmployeeNoDataGridViewCellStyle;
                            }
                        }
                    }

                    this.pvtblnDeviceDataGridViewLoaded = true;

                    if (dgvDeviceDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvDeviceDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvDeviceDataGridView));
                    }
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void dgvDeviceDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDeviceDataGridView.Rows.Count > 0
                & pvtblnDeviceDataGridViewLoaded == true)
            {
                grbFingerPrintsMissing.Visible = false;
                pvtintDeviceNo = Convert.ToInt32(dgvDeviceDataGridView[6, e.RowIndex].Value);

                string strFilter = "";

                if (this.rbnTemplateMissing.Checked == true)
                {
                    strFilter = " AND HAS_FINGERPRINTS_AT_SERVER_IND = 'N' AND HAS_FINGERPRINTS_AT_LOCAL_IND = 'N'";
                }
                else
                {
                    if (this.rbnLocal.Checked == true)
                    {
                        strFilter = " AND HAS_FINGERPRINTS_AT_LOCAL_IND = 'Y'";
                    }
                    else
                    {
                        if (this.rbnServer.Checked == true)
                        {
                            strFilter = " AND HAS_FINGERPRINTS_AT_SERVER_IND = 'Y'";
                        }
                        else
                        {
                            if (this.rbnKeyEmployeeNo.Checked == true)
                            {
                                strFilter = " AND USE_EMPLOYEE_NO_IND = 'Y'";
                            }
                        }
                    }
                }

                pvtEmployeeDataView = null;
                pvtEmployeeDataView = new DataView(pvtDataSet.Tables["Employee"], "DEVICE_NO = " + this.pvtintDeviceNo + " AND COMPANY_NO = " + pvtint64CompanyNo + strFilter, "", DataViewRowState.CurrentRows);

                Clear_DataGridView(dgvEmployeeDataGridView);

                for (int intRowCount = 0; intRowCount < pvtEmployeeDataView.Count; intRowCount++)
                {
                    dgvEmployeeDataGridView.Rows.Add("",
                                                     "",
                                                     "",
                                                     "",
                                                     pvtEmployeeDataView[intRowCount]["EMPLOYEE_CODE"].ToString(),
                                                     pvtEmployeeDataView[intRowCount]["EMPLOYEE_SURNAME"].ToString(),
                                                     pvtEmployeeDataView[intRowCount]["EMPLOYEE_NAME"].ToString(),
                                                     pvtEmployeeDataView[intRowCount]["PAY_CATEGORY_TYPE"].ToString().Substring(0, 1),
                                                     pvtEmployeeDataView[intRowCount]["PAY_CATEGORY_DESC"].ToString());
                                                    
                    if (pvtEmployeeDataView[intRowCount]["HAS_FINGERPRINTS_AT_SERVER_IND"].ToString() == "N"
                    && pvtEmployeeDataView[intRowCount]["HAS_FINGERPRINTS_AT_LOCAL_IND"].ToString() == "N")
                    {
                        dgvEmployeeDataGridView[0, intRowCount].Style = NoTemplateDataGridViewCellStyle;
                        grbFingerPrintsMissing.Visible = true;
                    }
                    else
                    {
                        if (pvtEmployeeDataView[intRowCount]["HAS_FINGERPRINTS_AT_SERVER_IND"].ToString() == "Y")
                        {
                            dgvEmployeeDataGridView[1, intRowCount].Style = ServerTemplateDataGridViewCellStyle;
                        }

                        if (pvtEmployeeDataView[intRowCount]["HAS_FINGERPRINTS_AT_LOCAL_IND"].ToString() == "Y")
                        {
                            dgvEmployeeDataGridView[2, intRowCount].Style = LocalTemplateDataGridViewCellStyle;
                        }
                    }

                    if (pvtEmployeeDataView[intRowCount]["USE_EMPLOYEE_NO_IND"].ToString() == "Y")
                    {
                        dgvEmployeeDataGridView[3, intRowCount].Style = UseEmployeeNoDataGridViewCellStyle;
                    }
                }
            }
        }

        private void DataGridView_Sorted(object sender, EventArgs e)
        {
            DataGridView myDataGridView = (DataGridView)sender;

            if (myDataGridView.Rows.Count > 0)
            {
                if (myDataGridView.SelectedRows.Count > 0)
                {
                    if (myDataGridView.SelectedRows[0].Selected == true)
                    {
                        myDataGridView.FirstDisplayedScrollingRowIndex = myDataGridView.SelectedRows[0].Index;
                    }
                }
            }
        }

        private void dgvDeviceDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 6)
            {
                if (double.Parse(e.CellValue1.ToString()) > double.Parse(e.CellValue2.ToString()))
                {
                    e.SortResult = 1;
                }
                else if (double.Parse(e.CellValue1.ToString()) < double.Parse(e.CellValue2.ToString()))
                {
                    e.SortResult = -1;
                }
                else
                {
                    e.SortResult = 0;
                }

                e.Handled = true;
            } 
        }

        private void Filter_Click(object sender, EventArgs e)
        {
            //Fires Row - Bug Doesn't Fire
            if (dgvCompanyDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView,this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
            }
        }
    }
}
