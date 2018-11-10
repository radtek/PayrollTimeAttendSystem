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
    public partial class frmClockInBoundary : Form
    {
        clsISClientUtilities clsISClientUtilities;
        ToolStripMenuItem miLinkedMenuItem;

        Int64 pvtInt64CompanyNo = -1;
        int pvtintPayCategoryNo = -1;
        int pvtintDepartmentNo = -1;
        int pvtintGroupNo = -1;

        int pvtintPayCategoryNoSaved = -1;
        int pvtintDepartmentNoSaved = -1;
        int pvtintGroupNoSaved = -1;
        
        string pvtstrPayCategoryType = "";
        int pvtintDeviceNo = -1;

        bool blnNew = false;

        int pvtintLinkTypeBoundaryDataViewIndex = -1;

        private int pvtintReaderDataGridViewRowIndex = -1;
        private int pvtintCompanyDataGridViewRowIndex = -1;
        private int pvtintLinkTypeDataGridViewRowIndex = -1;
        private int pvtintPayCategoryDataGridViewRowIndex = -1;
        private int pvtintShiftDataGridViewRowIndex = -1;

        DataSet pvtDataSet;
        DataView pvtLinkTypeBoundaryDataView;
        DataView pvtLinkTypeDataView;
       
        DataView pvtPayCategoryDataView;
        DataView pvtPayCategoryDepartmentDataView;
        DataView pvtGroupDataView;

        private bool pvtblnCompanyDataGridViewLoaded = false;
        private bool pvtblnLinkTypeDataGridViewLoaded = false;
        private bool pvtblnReaderDataGridViewLoaded = false;
        private bool pvtblnPayCategoryDataGridViewLoaded = false;
        private bool pvtblnShiftDataGridViewLoaded = false;
       
        public frmClockInBoundary()
        {
            InitializeComponent();
        }

        private void MonHourMinuteIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourMonIn.SelectedIndex == 0)
            {
                this.cboMinMonIn.SelectedIndex = -1;
                this.cboMinMonIn.Enabled = false;
            }
            
            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourMonIn.SelectedIndex != 0)
                {
                    this.cboMinMonIn.Enabled = true;

                    if (this.cboMinMonIn.SelectedIndex == -1)
                    {
                        this.cboMinMonIn.SelectedIndex = 0;
                    }
                }

                if (this.cboHourMonIn.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["MON_CLOCK_IN"] = System.DBNull.Value;
                    this.chkMon.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourMonIn.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinMonIn.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["MON_CLOCK_IN"] = intMinutes;
                }
            }
        }

        private void TueHourMinuteIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourTueIn.SelectedIndex == 0)
            {
                this.cboMinTueIn.SelectedIndex = -1;
                this.cboMinTueIn.Enabled = false;
            }

            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourTueIn.SelectedIndex != 0)
                {
                    this.cboMinTueIn.Enabled = true;

                    if (this.cboMinTueIn.SelectedIndex == -1)
                    {
                        this.cboMinTueIn.SelectedIndex = 0;
                    }
                }

                if (this.cboHourTueIn.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TUE_CLOCK_IN"] = System.DBNull.Value;
                    this.chkTue.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourTueIn.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinTueIn.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TUE_CLOCK_IN"] = intMinutes;
                }
            }
        }

        private void WedHourMinuteIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourWedIn.SelectedIndex == 0)
            {
                this.cboMinWedIn.SelectedIndex = -1;
                this.cboMinWedIn.Enabled = false;
            }

            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourWedIn.SelectedIndex != 0)
                {
                    this.cboMinWedIn.Enabled = true;

                    if (this.cboMinWedIn.SelectedIndex == -1)
                    {
                        this.cboMinWedIn.SelectedIndex = 0;
                    }
                }

                if (this.cboHourWedIn.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["WED_CLOCK_IN"] = System.DBNull.Value;
                    this.chkWed.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourWedIn.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinWedIn.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["WED_CLOCK_IN"] = intMinutes;
                }
            }
        }

        private void ThuHourMinuteIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourThuIn.SelectedIndex == 0)
            {
                this.cboMinThuIn.SelectedIndex = -1;
                this.cboMinThuIn.Enabled = false;
            }

            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourThuIn.SelectedIndex != 0)
                {
                    this.cboMinThuIn.Enabled = true;

                    if (this.cboMinThuIn.SelectedIndex == -1)
                    {
                        this.cboMinThuIn.SelectedIndex = 0;
                    }
                }

                if (this.cboHourThuIn.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["THU_CLOCK_IN"] = System.DBNull.Value;
                    this.chkThu.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourThuIn.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinThuIn.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["THU_CLOCK_IN"] = intMinutes;
                }
            }
        }

        private void FriHourMinuteIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourFriIn.SelectedIndex == 0)
            {
                this.cboMinFriIn.SelectedIndex = -1;
                this.cboMinFriIn.Enabled = false;
            }

            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourFriIn.SelectedIndex != 0)
                {
                    this.cboMinFriIn.Enabled = true;

                    if (this.cboMinFriIn.SelectedIndex == -1)
                    {
                        this.cboMinFriIn.SelectedIndex = 0;
                    }
                }

                if (this.cboHourFriIn.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["FRI_CLOCK_IN"] = System.DBNull.Value;
                    this.chkFri.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourFriIn.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinFriIn.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["FRI_CLOCK_IN"] = intMinutes;
                }
            }
        }

        private void SatHourMinuteIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourSatIn.SelectedIndex == 0)
            {
                this.cboMinSatIn.SelectedIndex = -1;
                this.cboMinSatIn.Enabled = false;
            }

            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourSatIn.SelectedIndex != 0)
                {
                    this.cboMinSatIn.Enabled = true;

                    if (this.cboMinSatIn.SelectedIndex == -1)
                    {
                        this.cboMinSatIn.SelectedIndex = 0;
                    }
                }

                if (this.cboHourSatIn.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SAT_CLOCK_IN"] = System.DBNull.Value;
                    this.chkSat.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourSatIn.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinSatIn.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SAT_CLOCK_IN"] = intMinutes;
                }
            }
        }

        private void SunHourMinuteIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourSunIn.SelectedIndex == 0)
            {
                this.cboMinSunIn.SelectedIndex = -1;
                this.cboMinSunIn.Enabled = false;
            }

            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourSunIn.SelectedIndex != 0)
                {
                    this.cboMinSunIn.Enabled = true;

                    if (this.cboMinSunIn.SelectedIndex == -1)
                    {
                        this.cboMinSunIn.SelectedIndex = 0;
                    }
                }

                if (this.cboHourSunIn.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SUN_CLOCK_IN"] = System.DBNull.Value;
                    this.chkSun.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourSunIn.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinSunIn.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SUN_CLOCK_IN"] = intMinutes;
                }
            }
        }
      
        private void MonHourMinuteOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourMonOut.SelectedIndex == 0)
            {
                this.cboMinMonOut.SelectedIndex = -1;
                this.cboMinMonOut.Enabled = false;
            }

            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourMonOut.SelectedIndex != 0)
                {
                    this.cboMinMonOut.Enabled = true;

                    if (this.cboMinMonOut.SelectedIndex == -1)
                    {
                        this.cboMinMonOut.SelectedIndex = 0;
                    }
                }

                if (this.cboHourMonOut.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["MON_CLOCK_OUT"] = System.DBNull.Value;
                    this.chkMon.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourMonOut.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinMonOut.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["MON_CLOCK_OUT"] = intMinutes;
                }
            }
        }

        private void TueHourMinuteOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourTueOut.SelectedIndex == 0)
            {
                this.cboMinTueOut.SelectedIndex = -1;
                this.cboMinTueOut.Enabled = false;
            }

            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourTueOut.SelectedIndex != 0)
                {
                    this.cboMinTueOut.Enabled = true;

                    if (this.cboMinTueOut.SelectedIndex == -1)
                    {
                        this.cboMinTueOut.SelectedIndex = 0;
                    }
                }

                if (this.cboHourTueOut.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TUE_CLOCK_OUT"] = System.DBNull.Value;
                    this.chkTue.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourTueOut.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinTueOut.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TUE_CLOCK_OUT"] = intMinutes;
                }
            }
        }

        private void WedHourMinuteOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourWedOut.SelectedIndex == 0)
            {
                this.cboMinWedOut.SelectedIndex = -1;
                this.cboMinWedOut.Enabled = false;
            }

            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourWedOut.SelectedIndex != 0)
                {
                    this.cboMinWedOut.Enabled = true;

                    if (this.cboMinWedOut.SelectedIndex == -1)
                    {
                        this.cboMinWedOut.SelectedIndex = 0;
                    }
                }

                if (this.cboHourWedOut.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["WED_CLOCK_OUT"] = System.DBNull.Value;
                    this.chkWed.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourWedOut.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinWedOut.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["WED_CLOCK_OUT"] = intMinutes;
                }
            }
        }

        private void ThuHourMinuteOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourThuOut.SelectedIndex == 0)
            {
                this.cboMinThuOut.SelectedIndex = -1;
                this.cboMinThuOut.Enabled = false;
            }

            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourThuOut.SelectedIndex != 0)
                {
                    this.cboMinThuOut.Enabled = true;

                    if (this.cboMinThuOut.SelectedIndex == -1)
                    {
                        this.cboMinThuOut.SelectedIndex = 0;
                    }
                }

                if (this.cboHourThuOut.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["THU_CLOCK_OUT"] = System.DBNull.Value;
                    this.chkThu.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourThuOut.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinThuOut.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["THU_CLOCK_OUT"] = intMinutes;
                }
            }
        }

        private void FriHourMinuteOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourFriOut.SelectedIndex == 0)
            {
                this.cboMinFriOut.SelectedIndex = -1;
                this.cboMinFriOut.Enabled = false;
            }

            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourFriOut.SelectedIndex != 0)
                {
                    this.cboMinFriOut.Enabled = true;

                    if (this.cboMinFriOut.SelectedIndex == -1)
                    {
                        this.cboMinFriOut.SelectedIndex = 0;
                    }
                }

                if (this.cboHourFriOut.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["FRI_CLOCK_OUT"] = System.DBNull.Value;
                    this.chkFri.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourFriOut.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinFriOut.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["FRI_CLOCK_OUT"] = intMinutes;
                }
            }
        }

        private void SatHourMinuteOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourSatOut.SelectedIndex == 0)
            {
                this.cboMinSatOut.SelectedIndex = -1;
                this.cboMinSatOut.Enabled = false;
            }

            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourSatOut.SelectedIndex != 0)
                {
                    this.cboMinSatOut.Enabled = true;

                    if (this.cboMinSatOut.SelectedIndex == -1)
                    {
                        this.cboMinSatOut.SelectedIndex = 0;
                    }
                }

                if (this.cboHourSatOut.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SAT_CLOCK_OUT"] = System.DBNull.Value;
                    this.chkSat.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourSatOut.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinSatOut.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SAT_CLOCK_OUT"] = intMinutes;
                }
            }
        }

        private void SunHourMinuteOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboHourSunOut.SelectedIndex == 0)
            {
                this.cboMinSunOut.SelectedIndex = -1;
                this.cboMinSunOut.Enabled = false;
            }

            if (this.btnSave.Enabled == true)
            {
                if (this.cboHourSunOut.SelectedIndex != 0)
                {
                    this.cboMinSunOut.Enabled = true;

                    if (this.cboMinSunOut.SelectedIndex == -1)
                    {
                        this.cboMinSunOut.SelectedIndex = 0;
                    }
                }

                if (this.cboHourSunOut.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SUN_CLOCK_OUT"] = System.DBNull.Value;
                    this.chkSun.Checked = false;
                }
                else
                {
                    int intHourMinutes = 60 * Convert.ToInt32(this.cboHourSunOut.SelectedItem);
                    int intMinutes = intHourMinutes + Convert.ToInt32(this.cboMinSunOut.SelectedItem);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SUN_CLOCK_OUT"] = intMinutes;
                }
            }
        }
  
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmClockInBoundary_Load(object sender, EventArgs e)
        {
            miLinkedMenuItem = (ToolStripMenuItem)AppDomain.CurrentDomain.GetData("LinkedMenuItem");

            string strTag = "Start";
          
            try
            {
                clsISClientUtilities = new clsISClientUtilities(this,"busClockInBoundary");

                this.lblClock.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblCompany.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblCostCentre.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblLinkType.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
                this.lblShiftNameDesc.Paint += new System.Windows.Forms.PaintEventHandler(clsISClientUtilities.Label_Paint);
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

                this.dgvLinkTypeDataGridView.Rows.Add("Cost Centre");
                this.dgvLinkTypeDataGridView.Rows.Add("Department");
                this.dgvLinkTypeDataGridView.Rows.Add("Group");

                pvtblnLinkTypeDataGridViewLoaded = true;

                this.Set_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView, 0);
  
                object[] objParm = new object[2];
                objParm[0] = Convert.ToInt32(AppDomain.CurrentDomain.GetData("UserNo"));
                objParm[1] = AppDomain.CurrentDomain.GetData("AccessInd").ToString();

                byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Form_Records", objParm,false);
                pvtDataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);
              
                strTag = "After DataSet";

                Load_CurrentForm_Records();
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void Load_CurrentForm_Records()
        {
            Clear_DataGridView(dgvReaderDataGridView);

            pvtblnReaderDataGridViewLoaded = false;

            for (int intRow = 0; intRow < this.pvtDataSet.Tables["Clock"].Rows.Count; intRow++)
            {
                this.dgvReaderDataGridView.Rows.Add(this.pvtDataSet.Tables["Clock"].Rows[intRow]["DEVICE_DESC"].ToString(),
                                                    this.pvtDataSet.Tables["Clock"].Rows[intRow]["DEVICE_NO"].ToString());
            }

            pvtblnReaderDataGridViewLoaded = true;

            if (this.dgvReaderDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvReaderDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvReaderDataGridView));

                Clear_DataGridView(dgvCompanyDataGridView);

                pvtblnCompanyDataGridViewLoaded = false;

                for (int intRow = 0; intRow < this.pvtDataSet.Tables["Company"].Rows.Count; intRow++)
                {
                    dgvCompanyDataGridView.Rows.Add(this.pvtDataSet.Tables["Company"].Rows[intRow]["COMPANY_DESC"].ToString(),
                                                    this.pvtDataSet.Tables["Company"].Rows[intRow]["COMPANY_NO"].ToString());
                }

                pvtblnCompanyDataGridViewLoaded = true;
                
                if (this.dgvCompanyDataGridView.Rows.Count > 0)
                {
                    this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
                }
                else
                {
                    this.btnUpdate.Enabled = false;
                }
            }
            else
            {
                this.btnUpdate.Enabled = false;
            }
        }

        private void frmClockInBoundary_FormClosing(object sender, FormClosingEventArgs e)
        {
            miLinkedMenuItem.Enabled = true;
        }

        private void Clear_Parameter_Fields()
        {
            this.txtBoundaryName.Text = "";
            this.chkActive.Checked = false;

            this.cboHourMonIn.SelectedIndex = 0;
            this.cboHourTueIn.SelectedIndex = 0;
            this.cboHourWedIn.SelectedIndex = 0;
            this.cboHourThuIn.SelectedIndex = 0;
            this.cboHourFriIn.SelectedIndex = 0;
            this.cboHourSatIn.SelectedIndex = 0;
            this.cboHourSunIn.SelectedIndex = 0;

            this.cboHourMonOut.SelectedIndex = 0;
            this.cboHourTueOut.SelectedIndex = 0;
            this.cboHourWedOut.SelectedIndex = 0;
            this.cboHourThuOut.SelectedIndex = 0;
            this.cboHourFriOut.SelectedIndex = 0;
            this.cboHourSatOut.SelectedIndex = 0;
            this.cboHourSunOut.SelectedIndex = 0;

            this.chkMon.Checked = false;
            this.chkTue.Checked = false;
            this.chkWed.Checked = false;
            this.chkThu.Checked = false;
            this.chkFri.Checked = false;
            this.chkSat.Checked = false;
            this.chkSun.Checked = false;

            this.cboRoundingOption.SelectedIndex = 0;
            this.cboRoundingMinutes.SelectedIndex = -1;
        }
             

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.Text += " - Update";

            blnNew = false;

            Set_Form_For_Edit();
            
        }

        private void Set_Form_For_Edit()
        {
            this.btnUpdate.Enabled = false;

            this.btnSave.Enabled = true;
            this.btnCancel.Enabled = true;

            this.dgvReaderDataGridView.Enabled = false;
            this.dgvCompanyDataGridView.Enabled = false;
            this.dgvLinkTypeDataGridView.Enabled = false;
            this.dgvPayCategoryDataGridView.Enabled = false;
            this.dgvShiftDataGridView.Enabled = false;

            this.picReaderLock.Visible = true;
            this.picCompanyLock.Visible = true;
            this.picLinkType.Visible = true;
            this.picPayCategoryLock.Visible = true;
            this.picBoundaryLock.Visible = true;

            this.txtBoundaryName.Enabled = true;
            this.chkActive.Enabled = true;

            if (this.blnNew == true)
            {
                this.btnNew.Enabled = false;
                this.txtBoundaryName.Text = "";

                this.cboHourMonIn.SelectedIndex = 0;
                this.cboHourTueIn.SelectedIndex = 0;
                this.cboHourWedIn.SelectedIndex = 0;
                this.cboHourThuIn.SelectedIndex = 0;
                this.cboHourFriIn.SelectedIndex = 0;
                this.cboHourSatIn.SelectedIndex = 0;
                this.cboHourSunIn.SelectedIndex = 0;

                this.cboHourMonOut.SelectedIndex = 0;
                this.cboHourTueOut.SelectedIndex = 0;
                this.cboHourWedOut.SelectedIndex = 0;
                this.cboHourThuOut.SelectedIndex = 0;
                this.cboHourFriOut.SelectedIndex = 0;
                this.cboHourSatOut.SelectedIndex = 0;
                this.cboHourSunOut.SelectedIndex = 0;

                this.chkMon.Checked = false;
                this.chkTue.Checked = false;
                this.chkWed.Checked = false;
                this.chkThu.Checked = false;
                this.chkFri.Checked = false;
                this.chkSat.Checked = false;
                this.chkSun.Checked = false;

                this.cboRoundingOption.SelectedIndex = 0;
                this.cboRoundingMinutes.SelectedIndex = -1;
            }

            this.cboHourMonIn.Enabled = true;
            if (this.cboHourMonIn.SelectedIndex != 0)
            {
                this.cboMinMonIn.Enabled = true;
            }

            this.cboHourTueIn.Enabled = true;
            if (this.cboHourTueIn.SelectedIndex != 0)
            {
                this.cboMinTueIn.Enabled = true;
            }

            this.cboHourWedIn.Enabled = true;
            if (this.cboHourWedIn.SelectedIndex != 0)
            {
                this.cboMinWedIn.Enabled = true;
            }

            this.cboHourThuIn.Enabled = true;
            if (this.cboHourThuIn.SelectedIndex != 0)
            {
                this.cboMinThuIn.Enabled = true;
            }

            this.cboHourFriIn.Enabled = true;
            if (this.cboHourFriIn.SelectedIndex != 0)
            {
                this.cboMinFriIn.Enabled = true;
            }

            this.cboHourSatIn.Enabled = true;
            if (this.cboHourSatIn.SelectedIndex != 0)
            {
                this.cboMinSatIn.Enabled = true;
            }

            this.cboHourSunIn.Enabled = true;
            if (this.cboHourSunIn.SelectedIndex != 0)
            {
                this.cboMinSunIn.Enabled = true;
            }

            this.cboHourMonOut.Enabled = true;
            if (this.cboHourMonOut.SelectedIndex != 0)
            {
                this.cboMinMonOut.Enabled = true;
            }

            this.cboHourTueOut.Enabled = true;
            if (this.cboHourTueOut.SelectedIndex != 0)
            {
                this.cboMinTueOut.Enabled = true;
            }

            this.cboHourWedOut.Enabled = true;
            if (this.cboHourWedOut.SelectedIndex != 0)
            {
                this.cboMinWedOut.Enabled = true;
            }

            this.cboHourThuOut.Enabled = true;
            if (this.cboHourThuOut.SelectedIndex != 0)
            {
                this.cboMinThuOut.Enabled = true;
            }

            this.cboHourFriOut.Enabled = true;
            if (this.cboHourFriOut.SelectedIndex != 0)
            {
                this.cboMinFriOut.Enabled = true;
            }

            this.cboHourSatOut.Enabled = true;
            if (this.cboHourSatOut.SelectedIndex != 0)
            {
                this.cboMinSatOut.Enabled = true;
            }

            this.cboHourSunOut.Enabled = true;
            if (this.cboHourSunOut.SelectedIndex != 0)
            {
                this.cboMinSunOut.Enabled = true;
            }

            this.chkMon.Enabled = true;
            this.chkTue.Enabled = true;
            this.chkWed.Enabled = true;
            this.chkThu.Enabled = true;
            this.chkFri.Enabled = true;
            this.chkSat.Enabled = true;
            this.chkSun.Enabled = true;

            this.cboRoundingOption.Enabled = true;
            if (this.cboRoundingMinutes.SelectedIndex != -1)
            {
                this.cboRoundingMinutes.Enabled = true;
            }

            this.txtBoundaryName.Focus();
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
                        
                        pvtintCompanyDataGridViewRowIndex = -1;
                        this.dgvCompanyDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvLinkTypeDataGridView":

                        pvtintLinkTypeDataGridViewRowIndex = -1;
                        this.dgvLinkTypeDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvPayCategoryDataGridView":

                        pvtintPayCategoryDataGridViewRowIndex = -1;
                        this.dgvPayCategoryDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvReaderDataGridView":

                        pvtintReaderDataGridViewRowIndex = -1;
                        this.dgvReaderDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
                        break;

                    case "dgvShiftDataGridView":

                        pvtintShiftDataGridViewRowIndex = -1;
                        this.dgvShiftDataGridView_RowEnter(null, myDataGridViewCellEventArgs);
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.Text.LastIndexOf(" - ") > -1)
            {
                this.Text = this.Text.Substring(0, this.Text.LastIndexOf(" - "));
            }

            this.pvtDataSet.RejectChanges();

            this.btnNew.Enabled = true;

            this.btnUpdate.Enabled = false;
            this.btnDelete.Enabled = false;
            this.btnSave.Enabled = false;
            this.btnCancel.Enabled = false;

            this.dgvReaderDataGridView.Enabled = true;
            this.dgvCompanyDataGridView.Enabled = true;
            this.dgvLinkTypeDataGridView.Enabled = true;
            this.dgvPayCategoryDataGridView.Enabled = true;
            this.dgvShiftDataGridView.Enabled = true;

            this.picReaderLock.Visible = false;
            this.picCompanyLock.Visible = false;
            this.picLinkType.Visible = false;
            this.picPayCategoryLock.Visible = false;
            this.picBoundaryLock.Visible = false;

            this.txtBoundaryName.Enabled = false;
            this.chkActive.Enabled = false;

            this.cboHourMonIn.Enabled = false;
            this.cboMinMonIn.Enabled = false;

            this.cboHourTueIn.Enabled = false;
            this.cboMinTueIn.Enabled = false;

            this.cboHourWedIn.Enabled = false;
            this.cboMinWedIn.Enabled = false;

            this.cboHourThuIn.Enabled = false;
            this.cboMinThuIn.Enabled = false;

            this.cboHourFriIn.Enabled = false;
            this.cboMinFriIn.Enabled = false;

            this.cboHourSatIn.Enabled = false;
            this.cboMinSatIn.Enabled = false;

            this.cboHourSunIn.Enabled = false;
            this.cboMinSunIn.Enabled = false;

            this.cboHourMonOut.Enabled = false;
            this.cboMinMonOut.Enabled = false;

            this.cboHourTueOut.Enabled = false;
            this.cboMinTueOut.Enabled = false;

            this.cboHourWedOut.Enabled = false;
            this.cboMinWedOut.Enabled = false;

            this.cboHourThuOut.Enabled = false;
            this.cboMinThuOut.Enabled = false;

            this.cboHourFriOut.Enabled = false;
            this.cboMinFriOut.Enabled = false;

            this.cboHourSatOut.Enabled = false;
            this.cboMinSatOut.Enabled = false;

            this.cboHourSunOut.Enabled = false;
            this.cboMinSunOut.Enabled = false;

            this.chkMon.Enabled = false;
            this.chkTue.Enabled = false;
            this.chkWed.Enabled = false;
            this.chkThu.Enabled = false;
            this.chkFri.Enabled = false;
            this.chkSat.Enabled = false;
            this.chkSun.Enabled = false;

            this.cboRoundingOption.Enabled = false;
            this.cboRoundingMinutes.Enabled = false;

            if (this.dgvCompanyDataGridView.Rows.Count > 0)
            {
                this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtBoundaryName.Text.Trim() == "")
                {
                    CustomClientMessageBox.Show("Enter Shift Name.",
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    this.txtBoundaryName.Focus();
                    return;
                }

                DataSet TempDataSet = new DataSet();

                if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 0)
                {
                    TempDataSet.Tables.Add(pvtDataSet.Tables["PayCategoryActive"].Clone());
                    TempDataSet.Tables["PayCategoryActive"].ImportRow(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex].Row);
                }
                else
                {
                    if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 1)
                    {
                        TempDataSet.Tables.Add(pvtDataSet.Tables["PayCategoryDepartmentActive"].Clone());
                        TempDataSet.Tables["PayCategoryDepartmentActive"].ImportRow(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex].Row);
                    }
                    else
                    {
                        TempDataSet.Tables.Add(pvtDataSet.Tables["GroupActive"].Clone());
                        TempDataSet.Tables["GroupActive"].ImportRow(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex].Row);
                    }
                }
                
                pvtintPayCategoryNoSaved = pvtintPayCategoryNo;
                pvtintDepartmentNoSaved = pvtintDepartmentNo;
                pvtintGroupNoSaved = pvtintGroupNo;
                
                //Compress DataSet
                byte[] pvtbytCompress = clsISClientUtilities.Compress_DataSet(TempDataSet);

                object[] objParm = new object[3];
                objParm[0] = this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) + 1;
                objParm[1] = pvtbytCompress;
                objParm[2] = blnNew;

                int intKey = (int)clsISClientUtilities.DynamicFunction("Update_Link_Type", objParm, true);

                if (blnNew == true)
                {
                    if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 0)
                    {
                        pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO"] = intKey;
                    }
                    else
                    {
                        if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 1)
                        {
                            pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["DEVICE_DEPARTMENT_LINK_ACTIVE_NO"] = intKey;
                        }
                        else
                        {
                            pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["DEVICE_GROUP_LINK_ACTIVE_NO"] = intKey;

                        }

                    }
                }

                if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["ACTIVE_IND"].ToString() == "Y")
                {
                    //Reset other Active Ind
                    for (int intRow = 0; intRow < pvtLinkTypeBoundaryDataView.Count; intRow++)
                    {
                        if (intRow == pvtintLinkTypeBoundaryDataViewIndex)
                        {
                            continue;
                        }
                        else
                        {
                            pvtLinkTypeBoundaryDataView[intRow]["ACTIVE_IND"] = "N";
                        }
                    }

                }

                this.pvtDataSet.AcceptChanges();

                this.btnCancel_Click(sender, e);
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.Text += " - New";

            blnNew = true;

            DataRowView drvDataRowView = this.pvtLinkTypeBoundaryDataView.AddNew();
            //Set Key for Find
            drvDataRowView["DEVICE_NO"] = pvtintDeviceNo;
            drvDataRowView["COMPANY_NO"] = pvtInt64CompanyNo;

            if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 0
            | this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 1)
            {
                drvDataRowView["PAY_CATEGORY_NO"] = this.pvtintPayCategoryNo;
                drvDataRowView["PAY_CATEGORY_TYPE"] = this.pvtstrPayCategoryType;

                if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 0)
                {
                    drvDataRowView["DEVICE_PAY_CATEGORY_LINK_ACTIVE_NO"] = 0;
                }
                else
                {
                    if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 1)
                    {
                        drvDataRowView["DEPARTMENT_NO"] = this.pvtintDepartmentNo;
                        drvDataRowView["DEVICE_DEPARTMENT_LINK_ACTIVE_NO"] = 0;
                   }
                }
            }
            else
            {
                drvDataRowView["GROUP_NO"] = this.pvtintGroupNo;
                drvDataRowView["DEVICE_GROUP_LINK_ACTIVE_NO"] = 0;
            }

            drvDataRowView["TIME_ATTEND_ROUNDING_VALUE"] = 0;
             
            drvDataRowView.EndEdit();

            pvtintLinkTypeBoundaryDataViewIndex = 0; 

            if (pvtLinkTypeBoundaryDataView.Count == 1)
            {
                pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["ACTIVE_IND"] = "Y";
                this.chkActive.Checked = true;
            }
            else
            {
                pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["ACTIVE_IND"] = "N";
                this.chkActive.Checked = false;
            }
        
            Set_Form_For_Edit();
        }

        private void txtBoundaryName_Leave(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["DEVICE_PAY_CATEGORY_LINK_ACTIVE_DESC"] = this.txtBoundaryName.Text.Trim();

                    for (int intRow = 0; intRow < pvtLinkTypeBoundaryDataView.Count; intRow++)
                    {
                        if (this.txtBoundaryName.Text.Trim() == pvtLinkTypeBoundaryDataView[intRow]["DEVICE_PAY_CATEGORY_LINK_ACTIVE_DESC"].ToString())
                        {
                            pvtintLinkTypeBoundaryDataViewIndex = intRow;
                            
                            break;
                        }
                    }
                }
                else
                {
                    if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 1)
                    {
                        pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["DEVICE_DEPARTMENT_LINK_ACTIVE_DESC"] = this.txtBoundaryName.Text.Trim();

                        for (int intRow = 0; intRow < pvtLinkTypeBoundaryDataView.Count; intRow++)
                        {
                            if (this.txtBoundaryName.Text.Trim() == pvtLinkTypeBoundaryDataView[intRow]["DEVICE_DEPARTMENT_LINK_ACTIVE_DESC"].ToString())
                            {
                                pvtintLinkTypeBoundaryDataViewIndex = intRow;

                                break;
                            }
                        }
                    }
                    else
                    {
                        pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["DEVICE_GROUP_LINK_ACTIVE_DESC"] = this.txtBoundaryName.Text.Trim();

                        for (int intRow = 0; intRow < pvtLinkTypeBoundaryDataView.Count; intRow++)
                        {
                            if (this.txtBoundaryName.Text.Trim() == pvtLinkTypeBoundaryDataView[intRow]["DEVICE_GROUP_LINK_ACTIVE_DESC"].ToString())
                            {
                                pvtintLinkTypeBoundaryDataViewIndex = intRow;

                                break;
                            }
                        }
                    }
                }
            }
        }

        private void chkActive_CheckedChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (chkActive.Checked == true)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["ACTIVE_IND"] = "Y";
                }
                else
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["ACTIVE_IND"] = "N";
                }
            }
        }

        private void chkMon_CheckedChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (chkMon.Checked == true)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["MON_CLOCK_IN_APPLIES_IND"] = "Y";
                }
                else
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["MON_CLOCK_IN_APPLIES_IND"] = "N";
                }
            }
        }

        private void chkTue_CheckedChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (chkTue.Checked == true)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TUE_CLOCK_IN_APPLIES_IND"] = "Y";
                }
                else
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TUE_CLOCK_IN_APPLIES_IND"] = "N";
                }
            }
        }

        private void chkWed_CheckedChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (chkWed.Checked == true)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["WED_CLOCK_IN_APPLIES_IND"] = "Y";
                }
                else
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["WED_CLOCK_IN_APPLIES_IND"] = "N";
                }
            }
        }

        private void chkThu_CheckedChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (chkThu.Checked == true)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["THU_CLOCK_IN_APPLIES_IND"] = "Y";
                }
                else
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["THU_CLOCK_IN_APPLIES_IND"] = "N";
                }
            }
        }

        private void chkFri_CheckedChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (chkFri.Checked == true)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["FRI_CLOCK_IN_APPLIES_IND"] = "Y";
                }
                else
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["FRI_CLOCK_IN_APPLIES_IND"] = "N";
                }
            }
        }

        private void chkSat_CheckedChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (chkSat.Checked == true)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SAT_CLOCK_IN_APPLIES_IND"] = "Y";
                }
                else
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SAT_CLOCK_IN_APPLIES_IND"] = "N";
                }
            }
        }

        private void chkSun_CheckedChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (chkSun.Checked == true)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SUN_CLOCK_IN_APPLIES_IND"] = "Y";
                }
                else
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SUN_CLOCK_IN_APPLIES_IND"] = "N";
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlgResult = CustomClientMessageBox.Show("Delete Shift '" + this.dgvShiftDataGridView[0, this.Get_DataGridView_SelectedRowIndex(dgvShiftDataGridView)].Value.ToString() + "'.",
                    this.Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    DataSet TempDataSet = new DataSet();

                    if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 0)
                    {
                        TempDataSet.Tables.Add(pvtDataSet.Tables["PayCategoryActive"].Clone());
                        TempDataSet.Tables["PayCategoryActive"].ImportRow(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex].Row);
                    }
                    else
                    {
                        if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 1)
                        {
                            TempDataSet.Tables.Add(pvtDataSet.Tables["PayCategoryDepartmentActive"].Clone());
                            TempDataSet.Tables["PayCategoryDepartmentActive"].ImportRow(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex].Row);
                        }
                        else
                        {
                            TempDataSet.Tables.Add(pvtDataSet.Tables["GroupActive"].Clone());
                            TempDataSet.Tables["GroupActive"].ImportRow(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex].Row);
                        }
                    }

                    //Compress DataSet
                    byte[] pvtbytCompress = clsISClientUtilities.Compress_DataSet(TempDataSet);

                    object[] objParm = new object[2];
                    objParm[0] = this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) + 1;
                    objParm[1] = pvtbytCompress;
                    
                    clsISClientUtilities.DynamicFunction("Delete_Shift", objParm,true);

                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex].Delete();

                    pvtDataSet.AcceptChanges();

                    this.btnCancel_Click(sender,e);
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void cboRoundingOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (cboRoundingOption.SelectedIndex == 0)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TIME_ATTEND_ROUNDING_IND"] = "";
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TIME_ATTEND_ROUNDING_VALUE"] = 0;
                    this.cboRoundingMinutes.Enabled = false;
                    this.cboRoundingMinutes.SelectedIndex = -1;
                }
                else
                {
                    if (cboRoundingOption.SelectedIndex > 0)
                    {
                        pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TIME_ATTEND_ROUNDING_IND"] = this.cboRoundingOption.SelectedItem.ToString().Substring(0, 1);

                        this.cboRoundingMinutes.Enabled = true;

                        if (cboRoundingMinutes.SelectedIndex == -1)
                        {
                            cboRoundingMinutes.SelectedIndex = 0;
                        }
                    }
                }
            }
        }

        private void cboRoundingMinutes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.btnSave.Enabled == true)
            {
                if (this.cboRoundingMinutes.SelectedIndex != -1)
                {
                    pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TIME_ATTEND_ROUNDING_VALUE"] = Convert.ToInt32(this.cboRoundingMinutes.SelectedItem.ToString());
                }
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (this.ActiveControl.Name.Substring(0, 7) == "cboHour"
                | this.ActiveControl.Name.Substring(0, 6) == "cboMin")
            {

            }
            else
            {
                CustomClientMessageBox.Show("You need to put the Focus onto a valid Hour / Minute Field",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void dgvCompanyDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (pvtblnCompanyDataGridViewLoaded == true)
                {
                    if (pvtintCompanyDataGridViewRowIndex != e.RowIndex)
                    {
                        pvtintCompanyDataGridViewRowIndex = e.RowIndex;

                        pvtInt64CompanyNo = Convert.ToInt64(dgvCompanyDataGridView[1, e.RowIndex].Value);

                        pvtPayCategoryDataView = null;
                        pvtPayCategoryDataView = new DataView(pvtDataSet.Tables["PayCategory"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + this.pvtInt64CompanyNo, "", DataViewRowState.CurrentRows);

                        pvtPayCategoryDepartmentDataView = null;
                        pvtPayCategoryDepartmentDataView = new DataView(pvtDataSet.Tables["PayCategoryDepartment"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + this.pvtInt64CompanyNo, "", DataViewRowState.CurrentRows);

                        pvtGroupDataView = null;
                        pvtGroupDataView = new DataView(pvtDataSet.Tables["Group"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + this.pvtInt64CompanyNo, "", DataViewRowState.CurrentRows);

                        if (pvtPayCategoryDataView.Count == 0
                            & pvtPayCategoryDepartmentDataView.Count == 0
                            & pvtGroupDataView.Count == 0)
                        {
                            object[] objParm = new object[1];
                            objParm[0] = pvtInt64CompanyNo;

                            byte[] bytCompress = (byte[])clsISClientUtilities.DynamicFunction("Get_Company_Records", objParm, false);
                            DataSet DataSet = clsISClientUtilities.DeCompress_Array_To_DataSet(bytCompress);

                            pvtDataSet.Merge(DataSet);
                        }

                        Clear_DataGridView(dgvPayCategoryDataGridView);
                        Clear_DataGridView(dgvShiftDataGridView);

                        this.pvtblnPayCategoryDataGridViewLoaded = false;

                        if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 0)
                        {
                            pvtLinkTypeDataView = null;
                            pvtLinkTypeDataView = new DataView(pvtDataSet.Tables["PayCategory"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + this.pvtInt64CompanyNo, "", DataViewRowState.CurrentRows);
                        }
                        else
                        {
                            if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 1)
                            {
                                pvtLinkTypeDataView = null;
                                pvtLinkTypeDataView = new DataView(pvtDataSet.Tables["PayCategoryDepartment"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + this.pvtInt64CompanyNo, "", DataViewRowState.CurrentRows);
                            }
                            else
                            {
                                pvtLinkTypeDataView = null;
                                pvtLinkTypeDataView = new DataView(pvtDataSet.Tables["Group"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + this.pvtInt64CompanyNo, "", DataViewRowState.CurrentRows);
                            }
                        }

                        string strActiveLinkDesc = "";
                        int intSetRowIndex = 0;

                        for (int intRow = 0; intRow < pvtLinkTypeDataView.Count; intRow++)
                        {
                            strActiveLinkDesc = "";
                            
                            if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 0)
                            {
                                pvtLinkTypeBoundaryDataView = null;
                                pvtLinkTypeBoundaryDataView = new DataView(pvtDataSet.Tables["PayCategoryActive"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + this.pvtInt64CompanyNo + " AND PAY_CATEGORY_NO = " + pvtLinkTypeDataView[intRow]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + this.pvtLinkTypeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() + "' AND ACTIVE_IND = 'Y'"
                                    , ""
                                    , DataViewRowState.CurrentRows);

                                if (pvtLinkTypeBoundaryDataView.Count > 0)
                                {
                                    strActiveLinkDesc = pvtLinkTypeBoundaryDataView[0]["DEVICE_PAY_CATEGORY_LINK_ACTIVE_DESC"].ToString();
                                }

                                this.dgvPayCategoryDataGridView.Rows.Add(pvtLinkTypeDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                         pvtLinkTypeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString(),
                                                                         "",
                                                                         strActiveLinkDesc,
                                                                         intRow.ToString());

                                if (pvtintPayCategoryNoSaved.ToString() == pvtLinkTypeDataView[intRow]["PAY_CATEGORY_NO"].ToString())
                                {
                                    intSetRowIndex = intRow;
                                }
                            }
                            else
                            {
                                if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 1)
                                {
                                    pvtLinkTypeBoundaryDataView = null;
                                    pvtLinkTypeBoundaryDataView = new DataView(pvtDataSet.Tables["PayCategoryDepartmentActive"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + this.pvtInt64CompanyNo + " AND PAY_CATEGORY_NO = " + pvtLinkTypeDataView[intRow]["PAY_CATEGORY_NO"].ToString() + " AND PAY_CATEGORY_TYPE = '" + this.pvtLinkTypeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString() + "' AND DEPARTMENT_NO = " + this.pvtLinkTypeDataView[intRow]["DEPARTMENT_NO"].ToString() + " AND ACTIVE_IND = 'Y'"
                                           , ""
                                           , DataViewRowState.CurrentRows);

                                    if (pvtLinkTypeBoundaryDataView.Count > 0)
                                    {
                                        strActiveLinkDesc = pvtLinkTypeBoundaryDataView[0]["DEVICE_DEPARTMENT_LINK_ACTIVE_DESC"].ToString();

                                    }

                                    this.dgvPayCategoryDataGridView.Rows.Add(pvtLinkTypeDataView[intRow]["DEPARTMENT_DESC"].ToString(),
                                                                             pvtLinkTypeDataView[intRow]["PAY_CATEGORY_TYPE"].ToString(),
                                                                             pvtLinkTypeDataView[intRow]["PAY_CATEGORY_DESC"].ToString(),
                                                                             strActiveLinkDesc,
                                                                             intRow.ToString());

                                    if (pvtintDepartmentNoSaved.ToString() == this.pvtLinkTypeDataView[intRow]["DEPARTMENT_NO"].ToString())
                                    {
                                        intSetRowIndex = intRow;
                                    }
                                }
                                else
                                {
                                    pvtLinkTypeBoundaryDataView = null;
                                    pvtLinkTypeBoundaryDataView = new DataView(pvtDataSet.Tables["GroupActive"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + this.pvtInt64CompanyNo + " AND GROUP_NO = " + pvtintGroupNo + " AND ACTIVE_IND = 'Y'"
                                    , "DEVICE_GROUP_LINK_ACTIVE_DESC"
                                    , DataViewRowState.CurrentRows);

                                    if (pvtLinkTypeBoundaryDataView.Count > 0)
                                    {
                                        strActiveLinkDesc = pvtLinkTypeBoundaryDataView[0]["DEVICE_GROUP_LINK_ACTIVE_DESC"].ToString();

                                    }

                                    this.dgvPayCategoryDataGridView.Rows.Add(pvtLinkTypeDataView[intRow]["GROUP_DESC"].ToString(),
                                                                             "",
                                                                             "",
                                                                             strActiveLinkDesc,
                                                                             intRow.ToString());
                                    
                                    if (pvtintGroupNoSaved.ToString() == pvtintGroupNo.ToString())
                                    {
                                        intSetRowIndex = intRow;
                                    }
                                }
                            }
                        }

                        pvtblnPayCategoryDataGridViewLoaded = true;

                        this.btnUpdate.Enabled = false;
                        this.btnDelete.Enabled = false;

                        if (dgvPayCategoryDataGridView.Rows.Count > 0)
                        {
                            this.Set_DataGridView_SelectedRowIndex(dgvPayCategoryDataGridView, intSetRowIndex);

                            this.btnNew.Enabled = true;
                        }
                        else
                        {
                            this.btnNew.Enabled = false;

                            Clear_Parameter_Fields();
                        }
                    }
                }
            }
            catch (Exception eException)
            {
                clsISClientUtilities.ErrorHandler(eException);
            }
        }

        private void dgvLinkTypeDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.pvtblnLinkTypeDataGridViewLoaded == true)
            {
                if (pvtintLinkTypeDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintLinkTypeDataGridViewRowIndex = e.RowIndex;

                    if (e.RowIndex == 0)
                    {
                        dgvPayCategoryDataGridView.Columns[1].Visible = true;
                        dgvPayCategoryDataGridView.Columns[2].Visible = false;
                        dgvPayCategoryDataGridView.Columns[0].Width = 537;
                        dgvPayCategoryDataGridView.Columns[1].Width = 20;
                    }
                    else
                    {
                        if (e.RowIndex == 1)
                        {
                            dgvPayCategoryDataGridView.Columns[1].Visible = true;
                            dgvPayCategoryDataGridView.Columns[2].Visible = true;

                            dgvPayCategoryDataGridView.Columns[0].Width = 288;
                            dgvPayCategoryDataGridView.Columns[1].Width = 20;
                            dgvPayCategoryDataGridView.Columns[2].Width = 249;
                        }
                        else
                        {
                            dgvPayCategoryDataGridView.Columns[1].Visible = false;
                            dgvPayCategoryDataGridView.Columns[2].Visible = false;
                            dgvPayCategoryDataGridView.Columns[0].Width = 557;
                        }
                    }

                    lblCostCentre.Text = this.dgvLinkTypeDataGridView[0, e.RowIndex].Value.ToString();

                    if (this.dgvCompanyDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
                    }
                }
            }
        }

        private void dgvReaderDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnReaderDataGridViewLoaded == true)
            {
                if (pvtintReaderDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintReaderDataGridViewRowIndex = e.RowIndex;

                    pvtintDeviceNo = Convert.ToInt32(dgvReaderDataGridView[1, e.RowIndex].Value);

                    if (this.dgvCompanyDataGridView.Rows.Count > 0)
                    {
                        this.Set_DataGridView_SelectedRowIndex(dgvCompanyDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvCompanyDataGridView));
                    }
                }
            }
        }

        private void dgvReaderDataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 1)
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

        private void dgvPayCategoryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnPayCategoryDataGridViewLoaded == true)
            {
                if (pvtintPayCategoryDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintPayCategoryDataGridViewRowIndex = e.RowIndex;

                    Clear_DataGridView(dgvShiftDataGridView);

                    this.pvtblnShiftDataGridViewLoaded = false;

                    if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 0
                    | this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 1)
                    {
                        pvtintPayCategoryNo = Convert.ToInt32(pvtLinkTypeDataView[Convert.ToInt32(this.dgvPayCategoryDataGridView[4, e.RowIndex].Value)]["PAY_CATEGORY_NO"]);
                        pvtstrPayCategoryType = pvtLinkTypeDataView[Convert.ToInt32(this.dgvPayCategoryDataGridView[4, e.RowIndex].Value)]["PAY_CATEGORY_TYPE"].ToString();
                    }

                    if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 0)
                    {
                        pvtLinkTypeBoundaryDataView = null;
                        pvtLinkTypeBoundaryDataView = new DataView(pvtDataSet.Tables["PayCategoryActive"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + this.pvtInt64CompanyNo + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + this.pvtstrPayCategoryType + "'"
                        , "DEVICE_PAY_CATEGORY_LINK_ACTIVE_DESC"
                        , DataViewRowState.CurrentRows);
                    }
                    else
                    {
                        if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 1)
                        {
                            pvtintDepartmentNo = Convert.ToInt32(pvtLinkTypeDataView[Convert.ToInt32(this.dgvPayCategoryDataGridView[4, e.RowIndex].Value)]["DEPARTMENT_NO"]);

                            pvtLinkTypeBoundaryDataView = null;
                            pvtLinkTypeBoundaryDataView = new DataView(pvtDataSet.Tables["PayCategoryDepartmentActive"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + this.pvtInt64CompanyNo + " AND PAY_CATEGORY_NO = " + pvtintPayCategoryNo + " AND PAY_CATEGORY_TYPE = '" + this.pvtstrPayCategoryType + "' AND DEPARTMENT_NO = " + pvtintDepartmentNo
                           , "DEVICE_DEPARTMENT_LINK_ACTIVE_DESC"
                           , DataViewRowState.CurrentRows);
                        }
                        else
                        {
                            pvtintGroupNo = Convert.ToInt32(pvtLinkTypeDataView[Convert.ToInt32(this.dgvPayCategoryDataGridView[4, e.RowIndex].Value)]["GROUP_NO"]);

                            pvtLinkTypeBoundaryDataView = null;
                            pvtLinkTypeBoundaryDataView = new DataView(pvtDataSet.Tables["GroupActive"], "DEVICE_NO = " + pvtintDeviceNo + " AND COMPANY_NO = " + this.pvtInt64CompanyNo + " AND GROUP_NO = " + pvtintGroupNo
                            , "DEVICE_GROUP_LINK_ACTIVE_DESC"
                            , DataViewRowState.CurrentRows);
                        }
                    }

                    bool blnActive = false;

                    for (int intRow = 0; intRow < pvtLinkTypeBoundaryDataView.Count; intRow++)
                    {
                        blnActive = false;

                        if (pvtLinkTypeBoundaryDataView[intRow]["ACTIVE_IND"].ToString() == "Y")
                        {
                            blnActive = true;
                        }

                        if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 0)
                        {
                            this.dgvShiftDataGridView.Rows.Add(pvtLinkTypeBoundaryDataView[intRow]["DEVICE_PAY_CATEGORY_LINK_ACTIVE_DESC"].ToString(),
                                                               blnActive,
                                                               intRow.ToString());
                        }
                        else
                        {
                            if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 1)
                            {
                                this.dgvShiftDataGridView.Rows.Add(pvtLinkTypeBoundaryDataView[intRow]["DEVICE_DEPARTMENT_LINK_ACTIVE_DESC"].ToString(),
                                                              blnActive,
                                                              intRow.ToString());
                            }
                            else
                            {
                                this.dgvShiftDataGridView.Rows.Add(pvtLinkTypeBoundaryDataView[intRow]["DEVICE_GROUP_LINK_ACTIVE_DESC"].ToString(),
                                                              blnActive,
                                                              intRow.ToString());
                            }
                        }
                    }

                    this.pvtblnShiftDataGridViewLoaded = true;

                    if (dgvShiftDataGridView.Rows.Count > 0)
                    {
                        this.btnUpdate.Enabled = true;
                        this.btnDelete.Enabled = true;

                        this.Set_DataGridView_SelectedRowIndex(dgvShiftDataGridView, this.Get_DataGridView_SelectedRowIndex(dgvShiftDataGridView));
                    }
                    else
                    {
                        this.btnUpdate.Enabled = false;
                        this.btnDelete.Enabled = false;

                        Clear_Parameter_Fields();
                    }
                }
            }
        }

        private void dgvShiftDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (pvtblnShiftDataGridViewLoaded == true)
            {
                if (pvtintShiftDataGridViewRowIndex != e.RowIndex)
                {
                    pvtintShiftDataGridViewRowIndex = e.RowIndex;

                    int intHour = 0;
                    int intMinute = 0;

                    pvtintLinkTypeBoundaryDataViewIndex = Convert.ToInt32(dgvShiftDataGridView[2, e.RowIndex].Value);

                    if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 0)
                    {
                        this.txtBoundaryName.Text = pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["DEVICE_PAY_CATEGORY_LINK_ACTIVE_DESC"].ToString();
                    }
                    else
                    {
                        if (this.Get_DataGridView_SelectedRowIndex(dgvLinkTypeDataGridView) == 1)
                        {
                            this.txtBoundaryName.Text = pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["DEVICE_DEPARTMENT_LINK_ACTIVE_DESC"].ToString();
                        }
                        else
                        {
                            this.txtBoundaryName.Text = pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["DEVICE_GROUP_LINK_ACTIVE_DESC"].ToString();
                        }
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["ACTIVE_IND"].ToString() == "Y")
                    {
                        this.chkActive.Checked = true;
                    }
                    else
                    {
                        this.chkActive.Checked = false;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["MON_CLOCK_IN"] == System.DBNull.Value)
                    {
                        this.cboHourMonIn.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["MON_CLOCK_IN"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["MON_CLOCK_IN"]) % 60;

                        this.cboHourMonIn.SelectedIndex = intHour + 1;
                        this.cboMinMonIn.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TUE_CLOCK_IN"] == System.DBNull.Value)
                    {
                        this.cboHourTueIn.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TUE_CLOCK_IN"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TUE_CLOCK_IN"]) % 60;

                        this.cboHourTueIn.SelectedIndex = intHour + 1;
                        this.cboMinTueIn.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["WED_CLOCK_IN"] == System.DBNull.Value)
                    {
                        this.cboHourWedIn.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["WED_CLOCK_IN"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["WED_CLOCK_IN"]) % 60;

                        this.cboHourWedIn.SelectedIndex = intHour + 1;
                        this.cboMinWedIn.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["THU_CLOCK_IN"] == System.DBNull.Value)
                    {
                        this.cboHourThuIn.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["THU_CLOCK_IN"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["THU_CLOCK_IN"]) % 60;

                        this.cboHourThuIn.SelectedIndex = intHour + 1;
                        this.cboMinThuIn.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["FRI_CLOCK_IN"] == System.DBNull.Value)
                    {
                        this.cboHourFriIn.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["FRI_CLOCK_IN"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["FRI_CLOCK_IN"]) % 60;

                        this.cboHourFriIn.SelectedIndex = intHour + 1;
                        this.cboMinFriIn.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SAT_CLOCK_IN"] == System.DBNull.Value)
                    {
                        this.cboHourSatIn.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SAT_CLOCK_IN"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SAT_CLOCK_IN"]) % 60;

                        this.cboHourSatIn.SelectedIndex = intHour + 1;
                        this.cboMinSatIn.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SUN_CLOCK_IN"] == System.DBNull.Value)
                    {
                        this.cboHourSunIn.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SUN_CLOCK_IN"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SUN_CLOCK_IN"]) % 60;

                        this.cboHourSunIn.SelectedIndex = intHour + 1;
                        this.cboMinSunIn.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["MON_CLOCK_OUT"] == System.DBNull.Value)
                    {
                        this.cboHourMonOut.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["MON_CLOCK_OUT"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["MON_CLOCK_OUT"]) % 60;

                        this.cboHourMonOut.SelectedIndex = intHour + 1;
                        this.cboMinMonOut.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TUE_CLOCK_OUT"] == System.DBNull.Value)
                    {
                        this.cboHourTueOut.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TUE_CLOCK_OUT"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TUE_CLOCK_OUT"]) % 60;

                        this.cboHourTueOut.SelectedIndex = intHour + 1;
                        this.cboMinTueOut.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["WED_CLOCK_OUT"] == System.DBNull.Value)
                    {
                        this.cboHourWedOut.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["WED_CLOCK_OUT"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["WED_CLOCK_OUT"]) % 60;

                        this.cboHourWedOut.SelectedIndex = intHour + 1;
                        this.cboMinWedOut.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["THU_CLOCK_OUT"] == System.DBNull.Value)
                    {
                        this.cboHourThuOut.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["THU_CLOCK_OUT"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["THU_CLOCK_OUT"]) % 60;

                        this.cboHourThuOut.SelectedIndex = intHour + 1;
                        this.cboMinThuOut.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["FRI_CLOCK_OUT"] == System.DBNull.Value)
                    {
                        this.cboHourFriOut.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["FRI_CLOCK_OUT"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["FRI_CLOCK_OUT"]) % 60;

                        this.cboHourFriOut.SelectedIndex = intHour + 1;
                        this.cboMinFriOut.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SAT_CLOCK_OUT"] == System.DBNull.Value)
                    {
                        this.cboHourSatOut.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SAT_CLOCK_OUT"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SAT_CLOCK_OUT"]) % 60;

                        this.cboHourSatOut.SelectedIndex = intHour + 1;
                        this.cboMinSatOut.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SUN_CLOCK_OUT"] == System.DBNull.Value)
                    {
                        this.cboHourSunOut.SelectedIndex = 0;
                    }
                    else
                    {
                        intHour = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SUN_CLOCK_OUT"]) / 60;
                        intMinute = Convert.ToInt32(pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SUN_CLOCK_OUT"]) % 60;

                        this.cboHourSunOut.SelectedIndex = intHour + 1;
                        this.cboMinSunOut.SelectedIndex = intMinute;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["MON_CLOCK_IN_APPLIES_IND"].ToString() == "Y")
                    {
                        this.chkMon.Checked = true;
                    }
                    else
                    {
                        this.chkMon.Checked = false;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TUE_CLOCK_IN_APPLIES_IND"].ToString() == "Y")
                    {
                        this.chkTue.Checked = true;
                    }
                    else
                    {
                        this.chkTue.Checked = false;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["WED_CLOCK_IN_APPLIES_IND"].ToString() == "Y")
                    {
                        this.chkWed.Checked = true;
                    }
                    else
                    {
                        this.chkWed.Checked = false;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["THU_CLOCK_IN_APPLIES_IND"].ToString() == "Y")
                    {
                        this.chkThu.Checked = true;
                    }
                    else
                    {
                        this.chkThu.Checked = false;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["FRI_CLOCK_IN_APPLIES_IND"].ToString() == "Y")
                    {
                        this.chkFri.Checked = true;
                    }
                    else
                    {
                        this.chkFri.Checked = false;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SAT_CLOCK_IN_APPLIES_IND"].ToString() == "Y")
                    {
                        this.chkSat.Checked = true;
                    }
                    else
                    {
                        this.chkSat.Checked = false;
                    }

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["SUN_CLOCK_IN_APPLIES_IND"].ToString() == "Y")
                    {
                        this.chkSun.Checked = true;
                    }
                    else
                    {
                        this.chkSun.Checked = false;
                    }

                    this.cboRoundingOption.SelectedIndex = -1;
                    this.cboRoundingMinutes.SelectedIndex = -1;

                    if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TIME_ATTEND_ROUNDING_IND"].ToString() == "")
                    {
                        this.cboRoundingOption.SelectedIndex = 0;
                    }
                    else
                    {
                        for (int intRowCount = 1; intRowCount < this.cboRoundingOption.Items.Count; intRowCount++)
                        {
                            if (this.cboRoundingOption.Items[intRowCount].ToString().Substring(0, 1) == pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TIME_ATTEND_ROUNDING_IND"].ToString())
                            {
                                this.cboRoundingOption.SelectedIndex = intRowCount;

                                break;
                            }
                        }

                        if (pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TIME_ATTEND_ROUNDING_VALUE"] != System.DBNull.Value)
                        {
                            for (int intRowCount = 0; intRowCount < this.cboRoundingMinutes.Items.Count; intRowCount++)
                            {
                                if (this.cboRoundingMinutes.Items[intRowCount].ToString() == pvtLinkTypeBoundaryDataView[pvtintLinkTypeBoundaryDataViewIndex]["TIME_ATTEND_ROUNDING_VALUE"].ToString())
                                {
                                    this.cboRoundingMinutes.SelectedIndex = intRowCount;

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
