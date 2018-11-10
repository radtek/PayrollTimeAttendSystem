using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
//using System.Web.Services;
using System.Reflection;
using System.Drawing;

namespace InteractPayroll
{
    public class clsISUtilities
    {
        Assembly asAssembly;
        System.Type typObjectType;
        object busDynamicService;

        localhost.busWebDynamicServices busWebDynamicServices;
        bool pvtblnCallBackComplete = false;

        Graphics Graphics;
        object pvtReturnObject;
        private string pvtstrBusinessObjectName = "";

        private bool pvtblnFormInEditMode = false;
        private bool pvtblnRemovingFormatting = false;

        private bool pvtblnCommunicationError = false;
        private bool pvtblnCommunicationTimeOutError = false;
        private bool pvtblnOtherError = false;

        private bool pvtblnCloseFormSent = false;

        private string pvtstrDataViewTableUniqueKeyName = "";

        Panel myPanelGlobe;
        Form pvtParentForm;

        DataSet pvtFormDataSet;
        DataSet pvtDataSet;
       
        IntPtr[] ControlPtr;
        int intControlPtrIndex = 0;

        private string pvtsrFunctionNameSaved = "";
        private object[] pvtobjParmSaved;

        System.Windows.Forms.PaintEventArgs myPaintEventArgs;

        DataView pvtFormDataView;
        public int DataViewIndex = -1;
        public bool pubintReloadSpreadsheet = false;
        
        //Communication Picture when accessing the Internet
        PictureBox picInternet;

        int intNumberCalendersCreated = 0;

        IntPtr[] IntPtrDateTextBox = new IntPtr[4];

        Button btnDate1;
        Button btnDate2;
        Button btnDate3;
        Button btnDate4;

        DateTimePicker dtpCalender1;
        DateTimePicker dtpCalender2;
        DateTimePicker dtpCalender3;
        DateTimePicker dtpCalender4;

        //Used In DataGridView
        DateTimePicker myDataGridViewDateTimePicker;
        DataGridView myDataGridView;

        int pvtintColumnIndex = -1;
        int pvtintRowIndex = -1;

        private int pvtintWebServiceMilliSecondTimeOut = -1;

        private bool pvtblnEFiling = false;

        string[] strTextBoxName = new string[4];

        bool blnClearCalenderClicked = false;

        public clsISUtilities()
        {
            AppDomain.CurrentDomain.SetData("KillApp", "N");

            Initialise_Classes_DataSet();
        }

        public clsISUtilities(Form parParentForm,string BusinessObjectName)
        {
            AppDomain.CurrentDomain.SetData("KillApp", "N");

            if (parParentForm != null)
            {
                pvtParentForm = parParentForm;

                int intWidth = 0;
                int intHeight = 0;
                int intNewHeight = 0;

                DataGridView myDataGridView;

                foreach (Control myControl in pvtParentForm.Controls)
                {
                    if (myControl is DataGridView)
                    {
                        myDataGridView = null;
                        myDataGridView = (DataGridView)myControl;

                        foreach (DataGridViewColumn myColumn in myDataGridView.Columns)
                        {
                            if (myColumn is DataGridViewCalendarColumn)
                            {
                                if (myDataGridViewDateTimePicker == null)
                                {
                                    myDataGridViewDateTimePicker = new DateTimePicker();
                                    myDataGridViewDateTimePicker.Name = "myDataGridViewDateTimePicker";

                                    myDataGridViewDateTimePicker.Format = DateTimePickerFormat.Custom;

                                    try
                                    {
                                        myDataGridViewDateTimePicker.CustomFormat = AppDomain.CurrentDomain.GetData("DateFormat").ToString();
                                    }
                                    catch
                                    {
                                        MessageBox.Show("AppDomain.CurrentDomain.GetData - DateFormat ERROR\nFix To Continue.");
                                        return;
                                    }

                                    myDataGridView.Parent.Controls.Add(myDataGridViewDateTimePicker);

                                    myDataGridViewDateTimePicker.Visible = false;

                                    //DateTimePicker Events
                                    myDataGridViewDateTimePicker.CloseUp += new System.EventHandler(myDataGridViewDateTimePicker_CloseUp);
                                    myDataGridViewDateTimePicker.KeyPress += new System.Windows.Forms.KeyPressEventHandler(myDataGridViewDateTimePicker_KeyPress);

                                    //DataGridView Events
                                    myDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(DataGridView_CellBeginEdit);
                                    myDataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(DataGridView_CellEnter);
                                    myDataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(DataGridView_ColumnHeaderMouseClick);
                                    myDataGridView.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(DataGridView_RowHeaderMouseClick);
                                    myDataGridView.Scroll += new System.Windows.Forms.ScrollEventHandler(DataGridView_Scroll);
                                    myDataGridView.MouseLeave += new System.EventHandler(DataGridView_MouseLeave);
                                    
                                    break;
                                }
                            }
                        }

                        if (myDataGridView.RowHeadersVisible == true)
                        {
                            intWidth = myDataGridView.RowHeadersWidth;
                        }
                        else
                        {
                            intWidth = 0;
                        }

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
                            MessageBox.Show(myDataGridView.Name + " Width should be " + intWidth.ToString());
                        }

                        if (myDataGridView.ColumnHeadersVisible == true)
                        {
                            intHeight = myDataGridView.ColumnHeadersHeight + 2;
                        }
                        else
                        {
                            intHeight = 0;
                        }

                        intNewHeight = myDataGridView.RowTemplate.Height / 2;

                        for (int intRow = 0; intRow < 200; intRow++)
                        {
                            intHeight += myDataGridView.RowTemplate.Height;

                            if (intHeight == myDataGridView.Height
                            ||  myDataGridView.Height == 22)
                            {
                                break;
                            }
                            else
                            {
                                if (intHeight > myDataGridView.Height)
                                {
                                    MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                    break;
                                }
                                else
                                {

                                    if (intHeight + intNewHeight > myDataGridView.Height)
                                    {
                                        MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
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

                                    string myName = myDataGridView.Name;

                                    if (myDataGridView.RowHeadersVisible == true)
                                    {
                                        intWidth = myDataGridView.RowHeadersWidth;
                                    }
                                    else
                                    {
                                        intWidth = 0;
                                    }

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
                                        MessageBox.Show(myDataGridView.Name + " Width should be " + intWidth.ToString());
                                    }

                                    if (myDataGridView.ColumnHeadersVisible == true)
                                    {
                                        intHeight = myDataGridView.ColumnHeadersHeight + 2;
                                    }
                                    else
                                    {
                                        intHeight = 0;
                                    }

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
                                                MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                                break;
                                            }
                                            else
                                            {

                                                if (intHeight + intNewHeight > myDataGridView.Height)
                                                {
                                                    MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (myControl is TabControl)
                            {
                                foreach (Control myControl2 in myControl.Controls)
                                {
                                    if (myControl2 is TabPage)
                                    {
                                        foreach (Control myControl3 in myControl2.Controls)
                                        {
                                            if (myControl3 is DataGridView)
                                            {
                                                myDataGridView = null;
                                                myDataGridView = (DataGridView)myControl3;

                                                foreach (DataGridViewColumn myColumn in myDataGridView.Columns)
                                                {
                                                    if (myColumn is DataGridViewCalendarColumn)
                                                    {
                                                        if (myDataGridViewDateTimePicker == null)
                                                        {
                                                            myDataGridViewDateTimePicker = new DateTimePicker();

                                                            myDataGridViewDateTimePicker.Format = DateTimePickerFormat.Custom;

                                                            try
                                                            {
                                                                myDataGridViewDateTimePicker.CustomFormat = AppDomain.CurrentDomain.GetData("DateFormat").ToString();
                                                            }
                                                            catch
                                                            {
                                                                MessageBox.Show("AppDomain.CurrentDomain.GetData - DateFormat ERROR\nFix To Continue.");
                                                                return;
                                                            }

                                                            myDataGridView.Parent.Controls.Add(myDataGridViewDateTimePicker);

                                                            myDataGridViewDateTimePicker.Visible = false;

                                                            //DateTimePicker Events
                                                            myDataGridViewDateTimePicker.CloseUp += new System.EventHandler(myDataGridViewDateTimePicker_CloseUp);
                                                            myDataGridViewDateTimePicker.KeyPress += new System.Windows.Forms.KeyPressEventHandler(myDataGridViewDateTimePicker_KeyPress);

                                                            //DataGridView Events
                                                            myDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(DataGridView_CellBeginEdit);
                                                            myDataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(DataGridView_CellEnter);
                                                            myDataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(DataGridView_ColumnHeaderMouseClick);
                                                            myDataGridView.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(DataGridView_RowHeaderMouseClick);
                                                            myDataGridView.Scroll += new System.Windows.Forms.ScrollEventHandler(DataGridView_Scroll);
                                                            myDataGridView.MouseLeave += new System.EventHandler(DataGridView_MouseLeave);
                                    
                                                            break;
                                                        }
                                                    }
                                                }

                                                if (myDataGridView.RowHeadersVisible == true)
                                                {
                                                    intWidth = myDataGridView.RowHeadersWidth;
                                                }
                                                else
                                                {
                                                    intWidth = 0;
                                                }

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
                                                    MessageBox.Show(myDataGridView.Name + " Width should be " + intWidth.ToString());
                                                }

                                                if (myDataGridView.ColumnHeadersVisible == true)
                                                {
                                                    intHeight = myDataGridView.ColumnHeadersHeight + 2;
                                                }
                                                else
                                                {
                                                    intHeight = 0;
                                                }

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
                                                            MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
                                                            break;
                                                        }
                                                        else
                                                        {

                                                            if (intHeight + intNewHeight > myDataGridView.Height)
                                                            {
                                                                MessageBox.Show(myDataGridView.Name + " Height should be " + intHeight.ToString());
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
                        }
                    }
                }
            }
      
            Initialise_Classes_DataSet();

            pvtstrBusinessObjectName = BusinessObjectName;

            if (AppDomain.CurrentDomain.GetData("URLPath").ToString() != "")
            {
                busWebDynamicServices = new localhost.busWebDynamicServices();

                int intReturnCode = WebService_Settings(busWebDynamicServices, "busWebDynamicServices");

                if (intReturnCode == 0)
                {
                    busWebDynamicServices.DynamicFunctionCompleted += new InteractPayroll.localhost.DynamicFunctionCompletedEventHandler(DynamicFunctionCompleted);
                }
                else
                {
                    MessageBox.Show("Error Creating URL - WebService Settings");
                }
            }
            else
            {
                FileInfo fiFileInfo = new FileInfo(BusinessObjectName + ".dll");

                if (fiFileInfo.Exists == true)
                {
                    asAssembly = Assembly.LoadFrom(BusinessObjectName + ".dll");
                    typObjectType = asAssembly.GetType("InteractPayroll." + BusinessObjectName);
                    busDynamicService = Activator.CreateInstance(typObjectType);
                }
                else
                {
                    MessageBox.Show(BusinessObjectName + ".dll does Not Exist\nSpeak to System Administrator.","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }
              
        private void myDataGridViewDateTimePicker_CloseUp(object sender, EventArgs e)
        {
            myDataGridView[pvtintColumnIndex,pvtintRowIndex].Value = myDataGridViewDateTimePicker.Value.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());

            myDataGridView.EndEdit();

            myDataGridViewDateTimePicker.Visible = false;
        }

        public void Set_WebService_Timeout_Value(int TimeoutValueInMilliSeconds)
        {
            if (AppDomain.CurrentDomain.GetData("URLPath").ToString() != "")
            {
                if (busWebDynamicServices != null)
                {
                    pvtintWebServiceMilliSecondTimeOut = TimeoutValueInMilliSeconds;
                }
                else
                {
                    MessageBox.Show("WebService_Timeout Value Cannot be set because Web busDynamicService IS NULL.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void Set_eFiling_Indicator(bool blnValue)
        {
            this.pvtblnEFiling = blnValue;
        }

        public int eFiling_PAYE_Ref_No(string parstrPAYERefNO)
        {
            int intReturnCode = 1;

            if (parstrPAYERefNO.Length == 0)
            {
                intReturnCode = 0;
            }
            else
            {
                if (parstrPAYERefNO.Length == 10)
                {
                    if (parstrPAYERefNO.Substring(0, 1) == "7")
                    {
                        //Modulus 10 Check
                        intReturnCode = Modulus_10_Check(parstrPAYERefNO,false);
                    }
                }
            }

            return intReturnCode;
        }

        public int eFiling_Income_Tax_Ref_No(string parstrIncomeTaxRefNo)
        {
            int intReturnCode = 1;

            if (parstrIncomeTaxRefNo.Length == 0)
            {
                //intReturnCode = 0;
            }
            else
            {
                if (parstrIncomeTaxRefNo.Length == 10)
                {
                    if (parstrIncomeTaxRefNo.Substring(0, 1) == "0"
                        | parstrIncomeTaxRefNo.Substring(0, 1) == "1"
                        | parstrIncomeTaxRefNo.Substring(0, 1) == "2"
                        | parstrIncomeTaxRefNo.Substring(0, 1) == "3"
                        | parstrIncomeTaxRefNo.Substring(0, 1) == "9")
                    {
                        if (parstrIncomeTaxRefNo != "0000000000")
                        {
                            //Modulus 10 Check
                            decimal dblResult = 0;

                            if (Decimal.TryParse(parstrIncomeTaxRefNo, out dblResult) == true)
                            {
                                intReturnCode = Modulus_10_Check(parstrIncomeTaxRefNo, true);
                            }
                        }
                        else
                        {
                            intReturnCode = 0;
                        }
                    }
                }
            }

            return intReturnCode;
        }
    
        public int eFiling_SDL_Ref_No(string parstrSDLRefNO)
        {
            int intReturnCode = 1;

            if (parstrSDLRefNO.Length == 0)
            {
                intReturnCode = 0;
            }
            else
            {
                if (parstrSDLRefNO.Length == 9)
                {
                    decimal dblResult = 0;

                    if (Decimal.TryParse(parstrSDLRefNO, out dblResult) == true)
                    {
                        intReturnCode = Modulus_10_Check("L" + parstrSDLRefNO, false);
                    }
                }
            }

            return intReturnCode;
        }

        public int eFiling_UIF_Ref_No(string parstrUIFRefNO)
        {
            int intReturnCode = 1;

            if (parstrUIFRefNO.Length == 0)
            {
                intReturnCode = 0;
            }
            else
            {
                if (parstrUIFRefNO.Length == 9)
                {
                    decimal dblResult = 0;

                    if (Decimal.TryParse(parstrUIFRefNO, out dblResult) == true)
                    {
                        intReturnCode = Modulus_10_Check("U" + parstrUIFRefNO, false);
                    }
                }
            }

            return intReturnCode;
        }

        private int Modulus_10_Check(string parstrNO,bool parblnIncomeTaxNo)
        {
            int intReturnCode = 1;
            int intNumber = 0;
            int intTotal = 0;
            string strNumber = "";
           
            try
            {
                //NB Last Digit is Check Digit
                for (int intDigit = 0; intDigit < parstrNO.Length - 1; intDigit++)
                {
                    if (intDigit % 2 == 0)
                    {
                        //Even Number
                        if (intDigit == 0
                        & parblnIncomeTaxNo == false)
                        {
                            //Replace 1st Character With 4 and Multiply by 2
                            parstrNO = "4" + parstrNO.Substring(1);

                            intNumber = 8;
                        }
                        else
                        {
                            intNumber = Convert.ToInt32(parstrNO.Substring(intDigit, 1)) * 2;
                        }

                        if (intNumber > 9)
                        {
                            strNumber = intNumber.ToString();

                            intNumber = Convert.ToInt32(strNumber.Substring(0, 1)) + Convert.ToInt32(strNumber.Substring(1));
                        }

                        intTotal += intNumber;
                    }
                    else
                    {
                        //Odd Number
                        intTotal += Convert.ToInt32(parstrNO.Substring(intDigit, 1)); 
                    }
                }
               
                if ((intTotal + Convert.ToInt32(parstrNO.Substring(parstrNO.Length - 1))) % 10 == 0)
                {
                    intReturnCode = 0;
                }
            }
            catch
            {
            }

            return intReturnCode;
        }

        private void myDataGridViewDateTimePicker_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                myDataGridView[pvtintColumnIndex, pvtintRowIndex].Value = myDataGridViewDateTimePicker.Value.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());

                myDataGridView.EndEdit();

                myDataGridViewDateTimePicker.Visible = false;
            }
        }

        private void DataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            myDataGridView = (DataGridView)sender;

            myDataGridViewDateTimePicker.Visible = false;

            if (myDataGridView.Rows[e.RowIndex].ReadOnly == false)
            {
                if (myDataGridView[e.ColumnIndex, e.RowIndex].ReadOnly == false)
                {
                    if (myDataGridView[e.ColumnIndex, e.RowIndex].GetType() == typeof(DataGridViewCalendarCell))
                    {
                        pvtintColumnIndex = e.ColumnIndex;
                        pvtintRowIndex = e.RowIndex;

                        Show_DateTimePicker(e.ColumnIndex, e.RowIndex);
                    }
                }
            }
        }

        private void DataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            myDataGridView = (DataGridView)sender;
            myDataGridView.EndEdit();

            myDataGridViewDateTimePicker.Visible = false;
        }

        private void DataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            myDataGridView = (DataGridView)sender;
            myDataGridView.EndEdit();
            myDataGridViewDateTimePicker.Visible = false;
        }

        private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            myDataGridView = (DataGridView)sender;
            myDataGridView.EndEdit();
            myDataGridViewDateTimePicker.Visible = false;
        }

        private void DataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            myDataGridView = (DataGridView)sender;
            myDataGridView.EndEdit();
            myDataGridViewDateTimePicker.Visible = false;
        }

        private void DataGridView_MouseLeave(object sender, EventArgs e)
        {
            myDataGridView = (DataGridView)sender;
            myDataGridView.EndEdit();
            myDataGridViewDateTimePicker.Visible = false;
        }
        
        private void Show_DateTimePicker(int ColumnIndex, int RowIndex)
        {
            if (myDataGridView[ColumnIndex, RowIndex].Value == null)
            {
                myDataGridViewDateTimePicker.Value = DateTime.Now;
            }
            else
            {
                if (myDataGridView[ColumnIndex, RowIndex].Value.ToString() == "")
                {
                    myDataGridViewDateTimePicker.Value = DateTime.Now;
                }
                else
                {
                    try
                    {
                        myDataGridViewDateTimePicker.Value = DateTime.ParseExact(myDataGridView[ColumnIndex, RowIndex].Value.ToString(), AppDomain.CurrentDomain.GetData("DateFormat").ToString(), null);
                    }
                    catch
                    {
                        int intError = 0;

                    }

                }
            }

            var cellRectangle = myDataGridView.GetCellDisplayRectangle(ColumnIndex, RowIndex, true);

            myDataGridViewDateTimePicker.Left = myDataGridView.Left + cellRectangle.Left;
            myDataGridViewDateTimePicker.Top = myDataGridView.Top + cellRectangle.Top - 1;

            myDataGridViewDateTimePicker.Width = myDataGridView.Columns[ColumnIndex].Width - 1;
            myDataGridViewDateTimePicker.BringToFront();
            myDataGridViewDateTimePicker.Show();
            myDataGridViewDateTimePicker.Refresh();
        }

        private void PingCompleted(object sender, localhost.PingCompletedEventArgs e)
        {
            try
            {
                pvtReturnObject = e.Result;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException.Message.IndexOf("The page cannot be displayed") > -1
                | ex.InnerException.Message.IndexOf("Server Unavailable") > -1)
                {
                    frmConnectionFailure frmConnectionFailure = new frmConnectionFailure();
                    frmConnectionFailure.ShowDialog();

                    pvtblnCommunicationError = true;
                }
                else
                {
                    if (ex.InnerException.Message.IndexOf("Runtime Error") > -1
                        | ex.InnerException.Message.IndexOf("connection timed out") > -1)
                    {
                        frmTimeoutFailure frmTimeoutFailure = new frmTimeoutFailure();
                        frmTimeoutFailure.ShowDialog();

                        pvtblnCommunicationTimeOutError = true;
                    }
                    else
                    {
                        Show_Write_Errors(ex.ToString());
                    }
                }
            }
            finally
            {
                pvtblnCallBackComplete = true;
            }
        }

        private void Initialise_Classes_DataSet()
        {
            pvtDataSet = new DataSet();

            DataTable DataTable = new DataTable("Control");
            
            DataTable.Columns.Add("CONTROL_NAME", typeof(String));
            DataTable.Columns.Add("CONTROL_HANDLE", typeof(IntPtr));
            DataTable.Columns.Add("OTHER_CONTROL_HANDLE", typeof(IntPtr));

            DataTable.Columns.Add("TABINDEX1", typeof(int));
            DataTable.Columns.Add("TABINDEX2", typeof(int));
            DataTable.Columns.Add("TABINDEX3", typeof(int));
            DataTable.Columns.Add("TABINDEX4", typeof(int));
            DataTable.Columns.Add("TABINDEX5", typeof(int));
            DataTable.Columns.Add("TABINDEX6", typeof(int));
            //T=TextBox,D=Date,C=ComboBox,G=GroupBox
            DataTable.Columns.Add("CONTROL_TYPE", typeof(String));

            DataTable.Columns.Add("DATABOUND", typeof(String));
           
            DataTable.Columns.Add("DATAVIEW_VALUEMEMBER", typeof(String));
            DataTable.Columns.Add("REQUIRED_IND", typeof(String));
            DataTable.Columns.Add("EFILING_IND", typeof(String));
            DataTable.Columns.Add("EITHER_OR_IND", typeof(String));

            DataTable.Columns.Add("REQUIRED_MESSAGE", typeof(String));
            DataTable.Columns.Add("CONTROL_PARENT_NAME", typeof(String));

            DataTable.Columns.Add("CONTROL_IS_DATAVIEW_SORT_COLUMN_IND", typeof(String));

            DataTable.Columns.Add("ENABLE_IN_EDIT_MODE", typeof(String));

            //ComboBox 
            DataTable.Columns.Add("DISPLAYMEMBER", typeof(String));
            DataTable.Columns.Add("VALUEMEMBER", typeof(String));
            DataTable.Columns.Add("TABLENAME", typeof(String));

            //TextBox
            DataTable.Columns.Add("NUMBER_DECIMALS", typeof(String));
            DataTable.Columns.Add("MIN_LENGTH", typeof(String));
            DataTable.Columns.Add("MAX_NUMBER", typeof(Double));
            DataTable.Columns.Add("NUMERIC_FIELD_FORMAT", typeof(String));
            DataTable.Columns.Add("NUMERIC_FIELD_NULLABLE", typeof(String));

            DataTable.Columns.Add("SA_ID_NUMBER_IND", typeof(String));
            DataTable.Columns.Add("SPECIAL_ADDR_FIELD_IND", typeof(String));
            DataTable.Columns.Add("SPECIAL_FIELD_GROUP", typeof(String));

            //RadioButton - Set to This Control on new Record
            DataTable.Columns.Add("DEFAULT_IND", typeof(String));
            DataTable.Columns.Add("DB_VALUE", typeof(String));
            DataTable.Columns.Add("FIRST_MEMBER_IND", typeof(String));
  
            DataTable.Columns.Add("TAB_CONTROL_HANDLE", typeof(IntPtr));
            DataTable.Columns.Add("TABPAGE_INDEX", typeof(Int32));

            pvtDataSet.Tables.Add(DataTable);

            //Table For Holding Control that has had a Paint Event attached
            DataTable = new DataTable("ControlPaintEvent");

            DataTable.Columns.Add("CONTROL_NAME", typeof(String));
            DataTable.Columns.Add("CONTROL_HANDLE", typeof(IntPtr));
           // DataTable.Columns.Add("CONTROL_OTHER_HANDLE", typeof(IntPtr));
            
            pvtDataSet.Tables.Add(DataTable);
        }

        public void Set_New_BusinessObjectName(string BusinessObjectName)
        {
            pvtstrBusinessObjectName = BusinessObjectName;

            if (AppDomain.CurrentDomain.GetData("URLPath").ToString() == "")
            {
                if (asAssembly != null)
                {
                    asAssembly = null;
                }

                asAssembly = Assembly.LoadFrom(BusinessObjectName + ".dll");
                typObjectType = asAssembly.GetType("InteractPayroll." + BusinessObjectName);
                busDynamicService = Activator.CreateInstance(typObjectType);
            }
        }

        public void ErrorHandler(Exception parException)
        {
            string strExceptionError = "";

            if (pvtblnCommunicationError == false
                & pvtblnCommunicationTimeOutError == false
                & pvtblnOtherError == false)
            {
                if (parException.Message.IndexOf("Server Unavailable") > -1)
                {
                    strExceptionError = "Connection to Web Server Could NOT be established";
                }
                else
                {
                    strExceptionError = parException.ToString();
                }
            }

            Show_Write_Errors(strExceptionError);
        }

        private void Show_Write_Errors(string parstrExceptionError)
        {
            if (pvtblnCommunicationError == false
            & pvtblnCommunicationTimeOutError == false
            & pvtblnOtherError == false
            & AppDomain.CurrentDomain.GetData("KillApp").ToString() != "Y")
            {
                string strExceptionError = "Date/Time  : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "\n\n" + "Error Desc : " + parstrExceptionError;

                if (pvtParentForm != null)
                {
                    MessageBox.Show(parstrExceptionError,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                }

                string strFileName = "";

                //P = Payroll
                //T = Time Attendance Client
                if (AppDomain.CurrentDomain.GetData("FromProgramInd") == null)
                {
                    strFileName = "PayrollIS_Error";
                }
                else
                {
                    if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "P")
                    {
                        strFileName = "PayrollIS_Error";
                    }
                    else
                    {
                        if (AppDomain.CurrentDomain.GetData("FromProgramInd").ToString() == "X")
                        {
                            strFileName = "TimeAttendanceInternetIS_Error";
                        }
                        else
                        {
                            strFileName = "TimeAttendanceClientIS_Error";
                        }
                    }
                }

                FileInfo fiErrorFile = new FileInfo(strFileName + ".txt");

                StreamWriter swErrorStreamWriter = fiErrorFile.AppendText();

                swErrorStreamWriter.WriteLine("");
                swErrorStreamWriter.WriteLine(parstrExceptionError);

                swErrorStreamWriter.Close();

                pvtblnOtherError = true;
            }
           
            //Close Current Form
            if (AppDomain.CurrentDomain.GetData("TimerCloseCurrentForm") != null)
            {
                if (pvtblnCloseFormSent == false)
                {
                    if (pvtParentForm != null)
                    {
                        pvtblnCloseFormSent = true;

                        AppDomain.CurrentDomain.SetData("FormToClose", pvtParentForm);

                        Timer tmrCloseCurrentForm = (Timer)AppDomain.CurrentDomain.GetData("TimerCloseCurrentForm");
                        tmrCloseCurrentForm.Enabled = true;
                    }
                }
            }
        }
      
        public void Round_For_Period(int parintRoundInd, int parintRoundValue, ref int parintTotal)
        {
            if (parintRoundInd == 0)
            {
            }
            else
            {
                if (parintTotal % parintRoundValue == 0)
                {
                }
                else
                {
                    //Up
                    if (parintRoundInd == 1)
                    {
                        parintTotal = parintTotal + (parintRoundValue - (parintTotal % parintRoundValue));
                    }
                    else
                    {
                        //Down
                        if (parintRoundInd == 2)
                        {
                            parintTotal = parintTotal - (parintTotal % parintRoundValue);
                        }
                        else
                        {
                            //Closest
                            if (parintTotal % parintRoundValue >= Convert.ToDecimal(parintRoundValue) / 2)
                            {
                                //Up
                                parintTotal = parintTotal + (parintRoundValue - (parintTotal % parintRoundValue));
                            }
                            else
                            {
                                //Down
                                parintTotal = parintTotal - (parintTotal % parintRoundValue);
                            }
                        }
                    }
                }
            }
        }

        public decimal Convert_Time_To_Decimal(int parintTimeInMinutes)
        {
            int intFieldHH;
            decimal dblFieldMM;
            
            intFieldHH = Convert.ToInt32(parintTimeInMinutes / 60);
            dblFieldMM = Convert.ToInt32(parintTimeInMinutes % 60);
            
            dblFieldMM = Math.Round(dblFieldMM / 60,2);

            decimal dblReturnValue = Convert.ToDecimal(intFieldHH) + dblFieldMM;

            return dblReturnValue;
        }

        public void Numeric_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)8
                | (e.KeyChar > (char)47
                & e.KeyChar < (char)58))
            {
            }
            else
            {
                e.Handled = true;
                System.Console.Beep();
                return;
            }
        }
       
        public void Numeric_2_Decimal_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            try
            {
                TextBox myTextBox = (TextBox)sender;
                int intNumberDecimals = 2;
 
                if (e.KeyChar == (char)8
                    | e.KeyChar == (char)46
                    | (e.KeyChar > (char)47
                    & e.KeyChar < (char)58))
                {
                    if (intNumberDecimals > 0)
                    {
                        if (e.KeyChar == 46
                            & myTextBox.Text.IndexOf(".") > -1)
                        {
                            e.Handled = true;
                            System.Console.Beep();
                        }
                        else
                        {
                            string strNewTextField = "";

                            //Build TextBox Field
                            if (myTextBox.Text != "")
                            {
                                if (myTextBox.Text.Length == myTextBox.SelectionStart)
                                {
                                    if (e.KeyChar == 8)
                                    {
                                        strNewTextField = myTextBox.Text.Substring(0, myTextBox.Text.Length - 1);
                                    }
                                    else
                                    {
                                        strNewTextField = myTextBox.Text + e.KeyChar;
                                    }
                                }
                                else
                                {
                                    if (e.KeyChar == 8)
                                    {
                                        if (myTextBox.SelectionStart == 0)
                                        {
                                            //Nothing Changed
                                            strNewTextField = myTextBox.Text;
                                        }
                                        else
                                        {
                                            strNewTextField = myTextBox.Text.Substring(0, myTextBox.SelectionStart - 1) + myTextBox.Text.Substring(myTextBox.SelectionStart);
                                        }
                                    }
                                    else
                                    {
                                        strNewTextField = myTextBox.Text.Substring(0, myTextBox.SelectionStart) + e.KeyChar + myTextBox.Text.Substring(myTextBox.SelectionStart);
                                    }
                                }
                            }
                            else
                            {
                                strNewTextField = e.KeyChar.ToString();
                            }

                            string[] strParts = strNewTextField.Split('.');

                            if (strParts.Length > 1)
                            {
                                if (strParts[strParts.Length - 1].Length > intNumberDecimals)
                                {
                                    //Errol Added 2012-065-30
                                    if (myTextBox.SelectionLength == 0)
                                    {
                                        e.Handled = true;
                                        System.Console.Beep();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (e.KeyChar == 46)
                        {
                            e.Handled = true;
                            System.Console.Beep();
                        }
                    }
                }
                else
                {
                    if (e.KeyChar == (char)13)
                    {
                        //Enter
                        string strFormat = "########0.00";
                       
                        if (myTextBox.Text.Replace(".", "").Trim() == "")
                        {
                            myTextBox.Text = Convert.ToDecimal(0).ToString(strFormat);

                        }
                        else
                        {
                            myTextBox.Text = Convert.ToDecimal(myTextBox.Text).ToString(strFormat);
                        }
                    }
                    else
                    {
                        e.Handled = true;
                        System.Console.Beep();
                    }
                }
            }
            catch
            {
            }
        }

        public void Numeric_4_Decimal_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            try
            {
                TextBox myTextBox = (TextBox)sender;
                int intNumberDecimals = 4;

                if (e.KeyChar == (char)8
                    | e.KeyChar == (char)46
                    | (e.KeyChar > (char)47
                    & e.KeyChar < (char)58))
                {
                    if (intNumberDecimals > 0)
                    {
                        if (e.KeyChar == 46
                            & myTextBox.Text.IndexOf(".") > -1)
                        {
                            e.Handled = true;
                            System.Console.Beep();
                        }
                        else
                        {
                            string strNewTextField = "";

                            //Build TextBox Field
                            if (myTextBox.Text != "")
                            {
                                if (myTextBox.Text.Length == myTextBox.SelectionStart)
                                {
                                    if (e.KeyChar == 8)
                                    {
                                        strNewTextField = myTextBox.Text.Substring(0, myTextBox.Text.Length - 1);
                                    }
                                    else
                                    {
                                        strNewTextField = myTextBox.Text + e.KeyChar;
                                    }
                                }
                                else
                                {
                                    if (e.KeyChar == 8)
                                    {
                                        if (myTextBox.SelectionStart == 0)
                                        {
                                            //Nothing Changed
                                            strNewTextField = myTextBox.Text;
                                        }
                                        else
                                        {
                                            strNewTextField = myTextBox.Text.Substring(0, myTextBox.SelectionStart - 1) + myTextBox.Text.Substring(myTextBox.SelectionStart);
                                        }
                                    }
                                    else
                                    {
                                        strNewTextField = myTextBox.Text.Substring(0, myTextBox.SelectionStart) + e.KeyChar + myTextBox.Text.Substring(myTextBox.SelectionStart);
                                    }
                                }
                            }
                            else
                            {
                                strNewTextField = e.KeyChar.ToString();
                            }

                            string[] strParts = strNewTextField.Split('.');

                            if (strParts.Length > 1)
                            {
                                if (strParts[strParts.Length - 1].Length > intNumberDecimals)
                                {
                                    //Errol Added 2012-065-30
                                    if (myTextBox.SelectionLength == 0)
                                    {
                                        e.Handled = true;
                                        System.Console.Beep();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (e.KeyChar == 46)
                        {
                            e.Handled = true;
                            System.Console.Beep();
                        }
                    }
                }
                else
                {
                    e.Handled = true;
                    System.Console.Beep();
                }
            }
            catch
            {
            }
        }
         
        public void Set_Form_For_Edit(bool blnNew)
        {
            //Used In Update Mode to know whether to Reload Spreadsheet that drives the Records
            pubintReloadSpreadsheet = false;

            if (blnNew == true)
            {
                pubintReloadSpreadsheet = true;
                DataBind_Initialise_Numeric_Fields();
            }

            //Allow Change Events from Updating DataView
            pvtblnFormInEditMode = true;

            //NB CONTROL_TYPE = 'O' is Controlled From within the Program
            //Attach Paint Events FOR NON DataBound Controls
            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_TYPE <> 'O'", "TABINDEX1,TABINDEX2,TABINDEX3,TABINDEX4,TABINDEX5,TABINDEX6", DataViewRowState.CurrentRows);
           
            for (int intRow = 0; intRow < DataView.Count; intRow++)
            {
                Control myControl = Control.FromHandle((IntPtr)DataView[intRow]["CONTROL_HANDLE"]);
                    
                DataView PaintDataView = new System.Data.DataView(pvtDataSet.Tables["ControlPaintEvent"], "CONTROL_NAME = '" + myControl.Parent.Name + "'", "", DataViewRowState.CurrentRows);
#if(DEBUG)
                string strName = myControl.Name;

                if (strName == "txtRate1")
                {
                    string strStop = "";
                }

                if (intRow == 13)
                {
                    string strStop = "";
                }

#endif
                //Attach Paint Event
                if (PaintDataView.Count == 0)
                {
                    //Add Record For ControlPaintEvent and Attach Event
                    DataRow DataRow = pvtDataSet.Tables["ControlPaintEvent"].NewRow();
                    DataRow["CONTROL_NAME"] = myControl.Parent.Name;
                    DataRow["CONTROL_HANDLE"] = myControl.Parent.Handle;
                    pvtDataSet.Tables["ControlPaintEvent"].Rows.Add(DataRow);

                    myControl.Parent.Paint += new PaintEventHandler(this.Control_Paint);
                }

                //NB This Must Be Here so That Paint Event Fires in Change Events
                if (DataView[intRow]["ENABLE_IN_EDIT_MODE"].ToString() == "Y")
                {
                    myControl.Enabled = true;
                }

                if (DataView[intRow]["DATABOUND"].ToString() == "Y")
                {
                    if (DataView[intRow]["CONTROL_TYPE"].ToString() == "D")
                    {
                        //Date LinkED To DateTimePicker
                        for (int intCount = 0; intCount < strTextBoxName.Length; intCount++)
                        {
                            if (strTextBoxName[intCount] == null)
                            {
                                break;
                            }
                            else
                            {
                                if (strTextBoxName[intCount] == myControl.Name)
                                {
                                    if (intCount == 0)
                                    {
                                        btnDate1.Enabled = true;
                                        dtpCalender1.Enabled = true;
                                    }
                                    else
                                    {
                                        if (intCount == 1)
                                        {
                                            btnDate2.Enabled = true;
                                            dtpCalender2.Enabled = true;
                                        }
                                        else
                                        {
                                            if (intCount == 2)
                                            {
                                                btnDate3.Enabled = true;
                                                dtpCalender3.Enabled = true;
                                            }
                                            else
                                            {
                                                if (intCount == 3)
                                                {
                                                    btnDate4.Enabled = true;
                                                    dtpCalender4.Enabled = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (blnNew == true)
                    {
                        if (DataView[intRow]["CONTROL_TYPE"].ToString() == "T")
                        {
                            if (DataView[intRow]["NUMBER_DECIMALS"].ToString() == "")
                            {
                                //TextBox
                                myControl.Text = "";
                            }
                            else
                            {
                                //Numeric Field
                                if (pvtFormDataView.Table.Columns[DataView[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].DataType.ToString() == "System.String")
                                {
                                    //DataType is NOT Numeric
                                    myControl.Text = "";
                                }
                                else
                                {
                                    if (DataView[intRow]["NUMERIC_FIELD_NULLABLE"].ToString() == "Y")
                                    {
                                        //DataType Numeric but NULLABLE
                                        myControl.Text = "";
                                    }
                                    else
                                    {
                                        if (DataView[intRow]["NUMBER_DECIMALS"].ToString() == "0")
                                        {
                                            myControl.Text = "0";
                                        }
                                        else
                                        {
                                            string strDecimals = new string('0', Convert.ToInt32(DataView[intRow]["NUMBER_DECIMALS"]));
                                            myControl.Text = "0." + strDecimals;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (DataView[intRow]["CONTROL_TYPE"].ToString() == "C")
                            {
                                ComboBox myComboBox = (ComboBox)myControl;

                                myComboBox.SelectedIndex = -1;
                            }
                            else
                            {
                                if (DataView[intRow]["CONTROL_TYPE"].ToString() == "D")
                                {
                                    //Date
                                    myControl.Text = "";
                                }
                                else
                                {
                                    if (DataView[intRow]["CONTROL_TYPE"].ToString() == "R")
                                    {
                                        //RadioButton
#if(DEBUG)
                                        string strRadioButtonName = myControl.Name;

                                        if (strRadioButtonName == "txtTelCell")
                                        {
                                            string strRadioButtonStop = "";
                                        }
#endif

                                        if (DataView[intRow]["DEFAULT_IND"].ToString() == "Y")
                                        {
                                            RadioButton myRadioButton = (RadioButton)Control.FromHandle((IntPtr)DataView[intRow]["CONTROL_HANDLE"]);
                                            myRadioButton.Checked = true;

                                            //Set Here Because CheckChanged Does Not Fire If Same
                                            pvtFormDataView[DataViewIndex][DataView[intRow]["DATAVIEW_VALUEMEMBER"].ToString()] = DataView[intRow]["DB_VALUE"].ToString();
                                        }
                                        else
                                        {
                                            if (DataView[intRow]["FIRST_MEMBER_IND"].ToString() == "Y")
                                            {
                                                DataView RadioButtonDataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "DATAVIEW_VALUEMEMBER = '" + DataView[intRow]["DATAVIEW_VALUEMEMBER"].ToString() + "' AND DEFAULT_IND = 'Y'", "", DataViewRowState.CurrentRows);

                                                if (RadioButtonDataView.Count == 0)
                                                {
                                                    //No Default Set for RadioButton - Take First RadioButton
                                                    RadioButton myRadioButton = (RadioButton)Control.FromHandle((IntPtr)DataView[intRow]["CONTROL_HANDLE"]);
                                                    myRadioButton.Checked = true;

                                                    //Set Here Because CheckChanged Does Not Fire If Same
                                                    pvtFormDataView[DataViewIndex][DataView[intRow]["DATAVIEW_VALUEMEMBER"].ToString()] = DataView[intRow]["DB_VALUE"].ToString();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("ERROR");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {

                        if (DataView[intRow]["NUMERIC_FIELD_FORMAT"].ToString() != ""
                            & pvtFormDataView.Table.Columns[DataView[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].DataType.ToString() == "System.String")
                        {
                            //Reset to DataLayer Value - Removee Formatting
                            myControl.Text = pvtFormDataView[DataViewIndex][DataView[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].ToString();
                        }
                    }
                }
                else
                {
                    if (DataView[intRow]["CONTROL_TYPE"].ToString() == "D")
                    {
                        if (myControl.Visible == true)
                        {
                            //Date Linked To DateTimePicker
                            for (int intCount = 0; intCount < strTextBoxName.Length; intCount++)
                            {
                                if (strTextBoxName[intCount] == null)
                                {
                                    break;
                                }
                                else
                                {
                                    if (strTextBoxName[intCount] == myControl.Name)
                                    {
                                        if (intCount == 0)
                                        {
                                            btnDate1.Enabled = true;
                                            dtpCalender1.Enabled = true;
                                        }
                                        else
                                        {
                                            if (intCount == 1)
                                            {
                                                btnDate2.Enabled = true;
                                                dtpCalender2.Enabled = true;
                                            }
                                            else
                                            {
                                                if (intCount == 2)
                                                {
                                                    btnDate3.Enabled = true;
                                                    dtpCalender3.Enabled = true;
                                                }
                                                else
                                                {
                                                    if (intCount == 3)
                                                    {
                                                        btnDate4.Enabled = true;
                                                        dtpCalender4.Enabled = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Set_Form_For_Read()
        {
            //Stop Change Events from Updating DataView
            pvtblnFormInEditMode = false;
                 
            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_TYPE = 'O'", "", DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < DataView.Count; intRow++)
            {
                Control parControl = Control.FromHandle((IntPtr)DataView[intRow]["CONTROL_HANDLE"]);

                DataView DataViewTest = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_PARENT_NAME = '" + parControl.Parent.Name + "'", "", DataViewRowState.CurrentRows);

                if (DataViewTest.Count == 1)
                {
                    Paint_Parent_Marker(parControl, false);
                    parControl.Parent.Paint -= new PaintEventHandler(Control_Paint);
                }

                DataView[intRow].Delete();

                intRow -= 1;
            }

            pvtDataSet.Tables["Control"].AcceptChanges();
                 
            DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "", "", DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < DataView.Count; intRow++)
            {
                Control myControl = Control.FromHandle((IntPtr)DataView[intRow]["CONTROL_HANDLE"]);

#if(DEBUG)
                string strName = myControl.Name;

                if (strName == "cboMinShiftHours")
                {
                    string strStop = "";
                }
#endif

                //Remove Red Edit Marker
                Paint_Parent_Marker(myControl, false);

                if (DataView[intRow]["CONTROL_TYPE"].ToString() == "T"
                    | DataView[intRow]["CONTROL_TYPE"].ToString() == "D")
                {
                    myControl.Text = "";

                    if (DataView[intRow]["CONTROL_TYPE"].ToString() == "D")
                    {
                        //Date LinkED To DateTimePicker
                        for (int intCount = 0; intCount < strTextBoxName.Length; intCount++)
                        {
                            if (strTextBoxName[intCount] == null)
                            {
                                break;
                            }
                            else
                            {
                                if (strTextBoxName[intCount] == myControl.Name)
                                {
                                    if (intCount == 0)
                                    {
                                        btnDate1.Enabled = false;
                                        dtpCalender1.Enabled = false;
                                    }
                                    else
                                    {
                                        if (intCount == 1)
                                        {
                                            btnDate2.Enabled = false;
                                            dtpCalender2.Enabled = false;
                                        }
                                        else
                                        {
                                            if (intCount == 2)
                                            {
                                                btnDate3.Enabled = false;
                                                dtpCalender3.Enabled = false;
                                            }
                                            else
                                            {
                                                if (intCount == 3)
                                                {
                                                    btnDate4.Enabled = false;
                                                    dtpCalender4.Enabled = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (DataView[intRow]["CONTROL_TYPE"].ToString() == "C")
                    {
                        ComboBox myComboBox = (ComboBox)myControl;
                        myComboBox.SelectedIndex = -1;
                    }
                }

                myControl.Enabled = false;
            }

            DataView PaintEventDataView = new System.Data.DataView(pvtDataSet.Tables["ControlPaintEvent"], "", "", DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < PaintEventDataView.Count; intRow++)
            {
                Control myControl = Control.FromHandle((IntPtr)PaintEventDataView[intRow]["CONTROL_HANDLE"]);

                //This is The Parent
                myControl.Paint -= new PaintEventHandler(Control_Paint);

                PaintEventDataView[intRow].Row.Delete();

                intRow -= 1;
            }

            pvtDataSet.Tables["ControlPaintEvent"].AcceptChanges();
        }
     
        public void DataBind_ComboBox_Load(ComboBox parComboBox,DataTable parDataTable,string DisplayMember,string ValueMember)
        {
            if (pvtFormDataSet == null)
            {
                pvtFormDataSet = parDataTable.DataSet;
            }
                              
            DataRow DataRow = pvtDataSet.Tables["Control"].NewRow();

            DataRow["TABLENAME"] = parDataTable.TableName;
            DataRow["CONTROL_NAME"] = parComboBox.Name;
            DataRow["CONTROL_HANDLE"] = parComboBox.Handle;
            DataRow["CONTROL_TYPE"] = "C";
            DataRow["DATABOUND"] = "Y";
            DataRow["DISPLAYMEMBER"] = DisplayMember;
            DataRow["VALUEMEMBER"] = ValueMember;
            DataRow["CONTROL_PARENT_NAME"] = parComboBox.Parent.Name;

            pvtDataSet.Tables["Control"].Rows.Add(DataRow);
                
            for (int intRow = 0; intRow < parDataTable.Rows.Count; intRow++)
            {
                parComboBox.Items.Add(parDataTable.Rows[intRow][DisplayMember].ToString());
            }
        }

        public int DataBind_Save_Check()
        {
            int intReturnCode = 0;

            string strFilter = "";

            if (this.pvtblnEFiling == true)
            {
                strFilter = " OR EFILING_IND = 'Y'";
            }

            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "REQUIRED_IND IN ('Y','E') OR NOT NUMBER_DECIMALS IS NULL OR SPECIAL_ADDR_FIELD_IND = 'Y' OR  EITHER_OR_IND = 'Y'" + strFilter, "TABINDEX1,TABINDEX2,TABINDEX3,TABINDEX4,TABINDEX5,TABINDEX6", DataViewRowState.CurrentRows);
           
            for (int intRow = 0; intRow < DataView.Count; intRow++)
            {
                Control myControl = Control.FromHandle((IntPtr)DataView[intRow]["CONTROL_HANDLE"]);
#if(DEBUG)
                string strName = myControl.Name;

                if (strName == "txtCompanyStreetName")
                {
                    string strStop = "";
                }
#endif
                if (DataView[intRow]["SPECIAL_ADDR_FIELD_IND"].ToString() == "Y")
                {
                    if (myControl.Enabled == true)
                    {
                        DataView myDataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "SPECIAL_ADDR_FIELD_IND = 'Y' AND SPECIAL_FIELD_GROUP = '" + DataView[intRow]["SPECIAL_FIELD_GROUP"].ToString() + "'", "TABINDEX1,TABINDEX2,TABINDEX3,TABINDEX4,TABINDEX5,TABINDEX6,CONTROL_NAME", DataViewRowState.CurrentRows);

                        Control myLinkedControl1 = new Control();
                        Control myLinkedControl2 = new Control();
                        Control myLinkedControl3 = new Control();
                        Control myLinkedControl4 = new Control();

                        for (int intRow1 = 0; intRow1 < myDataView.Count; intRow1++)
                        {
                            switch (intRow1)
                            {
                                case 0:

                                    myLinkedControl1 = Control.FromHandle((IntPtr)myDataView[intRow1]["CONTROL_HANDLE"]);
                                    break;

                                case 1:

                                    myLinkedControl2 = Control.FromHandle((IntPtr)myDataView[intRow1]["CONTROL_HANDLE"]);
                                    break;

                                case 2:

                                    myLinkedControl3 = Control.FromHandle((IntPtr)myDataView[intRow1]["CONTROL_HANDLE"]);
                                    break;

                                case 3:

                                    myLinkedControl4 = Control.FromHandle((IntPtr)myDataView[intRow1]["CONTROL_HANDLE"]);
                                    break;
                            }
                        }

                        if (myLinkedControl1.Text == ""
                            & myLinkedControl2.Text == ""
                            & myLinkedControl3.Text == ""
                            & myLinkedControl4.Text == ""
                            & (this.pvtblnEFiling == false
                            | DataView[intRow]["SPECIAL_FIELD_GROUP"].ToString() == "2"))
                        {
                            if (myControl.Name == myLinkedControl1.Name)
                            {
                                if (DataView[intRow]["REQUIRED_IND"].ToString() == "Y")
                                {
                                    //Other - Added Dynamically (If Row Exists then Error)
                                    if (DataView[intRow]["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                                    {
                                        TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)DataView[intRow]["TAB_CONTROL_HANDLE"]);

                                        myTabControl.SelectedIndex = Convert.ToInt32(DataView[intRow]["TABPAGE_INDEX"]);
                                    }

                                    MessageBox.Show(DataView[intRow]["REQUIRED_MESSAGE"].ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    
                                    myControl.Focus();

                                    intReturnCode = 1;
                                }
                            }
                        }
                        else
                        {
                            if (DataView[intRow]["SPECIAL_FIELD_GROUP"].ToString() == "1"
                            || DataView[intRow]["SPECIAL_FIELD_GROUP"].ToString() == "3"
                            || DataView[intRow]["SPECIAL_FIELD_GROUP"].ToString() == "4")
                            {
                                //StreetName
                                if (myLinkedControl1.Text == "")
                                {
                                    if (myControl.Name == myLinkedControl1.Name)
                                    {
                                        //Other - Added Dynamically (If Row Exists then Error)
                                        if (DataView[intRow]["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                                        {
                                            TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)DataView[intRow]["TAB_CONTROL_HANDLE"]);

                                            myTabControl.SelectedIndex = Convert.ToInt32(DataView[intRow]["TABPAGE_INDEX"]);
                                        }

                                        MessageBox.Show("Enter Street Name / Farm Name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                        myControl.Focus();

                                        intReturnCode = 1;
                                    }
                                }

                                if (myLinkedControl2.Text == ""
                                & myLinkedControl3.Text == "")
                                {
                                    if (myControl.Name == myLinkedControl2.Name)
                                    {
                                        //Other - Added Dynamically (If Row Exists then Error)
                                        if (DataView[intRow]["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                                        {
                                            TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)DataView[intRow]["TAB_CONTROL_HANDLE"]);

                                            myTabControl.SelectedIndex = Convert.ToInt32(DataView[intRow]["TABPAGE_INDEX"]);
                                        }

                                        MessageBox.Show("Enter Suburb / District or City / Town.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                        myControl.Focus();

                                        intReturnCode = 1;
                                    }
                                    else
                                    {
                                        if (myControl.Name == myLinkedControl3.Name)
                                        {
                                            //Other - Added Dynamically (If Row Exists then Error)
                                            if (DataView[intRow]["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                                            {
                                                TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)DataView[intRow]["TAB_CONTROL_HANDLE"]);

                                                myTabControl.SelectedIndex = Convert.ToInt32(DataView[intRow]["TABPAGE_INDEX"]);
                                            }

                                            MessageBox.Show("Enter Suburb / District or City / Town.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                            myControl.Focus();

                                            intReturnCode = 1;
                                        }
                                    }
                                }

                                //txtPhysicalCode
                                if (myLinkedControl4.Text.Length == 4
                                    & myLinkedControl4.Text != "0000")
                                {
                                }
                                else
                                {
                                    if (myControl.Name == myLinkedControl4.Name)
                                    {
                                        //Other - Added Dynamically (If Row Exists then Error)
                                        if (DataView[intRow]["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                                        {
                                            TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)DataView[intRow]["TAB_CONTROL_HANDLE"]);

                                            myTabControl.SelectedIndex = Convert.ToInt32(DataView[intRow]["TABPAGE_INDEX"]);
                                        }

                                        MessageBox.Show("Enter a Valid Physical Address Code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                        myControl.Focus();

                                        intReturnCode = 1;
                                    }
                                }
                            }
                            else
                            {
                                //Post Addr Line 1
                                if (myLinkedControl1.Text == "")
                                {
                                    if (myControl.Name == myLinkedControl1.Name)
                                    {
                                        //Other - Added Dynamically (If Row Exists then Error)
                                        if (DataView[intRow]["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                                        {
                                            TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)DataView[intRow]["TAB_CONTROL_HANDLE"]);

                                            myTabControl.SelectedIndex = Convert.ToInt32(DataView[intRow]["TABPAGE_INDEX"]);
                                        }

                                        MessageBox.Show("Enter Postal Address Line 1.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                        myControl.Focus();

                                        intReturnCode = 1;
                                    }
                                }

                                //txtPhysicalCode
                                if (myLinkedControl4.Text.Length == 4
                                    & myLinkedControl4.Text != "0000")
                                {
                                }
                                else
                                {
                                    if (myControl.Name == myLinkedControl4.Name)
                                    {
                                        //Other - Added Dynamically (If Row Exists then Error)
                                        if (DataView[intRow]["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                                        {
                                            TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)DataView[intRow]["TAB_CONTROL_HANDLE"]);

                                            myTabControl.SelectedIndex = Convert.ToInt32(DataView[intRow]["TABPAGE_INDEX"]);
                                        }

                                        MessageBox.Show("Enter a Valid Postal Address Code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                        myControl.Focus();

                                        intReturnCode = 1;
                                    }
                                }
                            }
                        }

                        if (intReturnCode == 1)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (DataView[intRow]["CONTROL_TYPE"].ToString() == "D")
                    {
                        intReturnCode = DataBound_Date_TextBox_Check((TextBox)myControl, DataView[intRow]);

                        if (intReturnCode == 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (DataView[intRow]["CONTROL_TYPE"].ToString() == "O")
                        {
                            //Other - Added Dynamically (If Row Exists then Error)
                            if (DataView[intRow]["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                            {
                                TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)DataView[intRow]["TAB_CONTROL_HANDLE"]);

                                myTabControl.SelectedIndex = Convert.ToInt32(DataView[intRow]["TABPAGE_INDEX"]);
                            }

                            MessageBox.Show(DataView[intRow]["REQUIRED_MESSAGE"].ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            intReturnCode = 1;
                            break;
                        }
                        else
                        {
                            if (DataView[intRow]["CONTROL_TYPE"].ToString() == "C")
                            {
                                intReturnCode = DataBound_ComboBox_Check((ComboBox)myControl, DataView[intRow], true);

                                if (intReturnCode == 1)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (DataView[intRow]["NUMBER_DECIMALS"].ToString() != "")
                                {
                                    //Numeric Field
                                    intReturnCode = DataBound_Numeric_TextBox_Check((TextBox)myControl, DataView[intRow], true);

                                    if (intReturnCode == 1)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    //TextBox

                                    intReturnCode = DataBound_TextBox_Check((TextBox)myControl, DataView[intRow], true);

                                    if (intReturnCode == 1)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
           
            return intReturnCode;
        }

        private int DataBound_ComboBox_Check(ComboBox parComboBox,DataRowView parDataViewRow,bool ShowErrorMessage)
        {
            int intReturnCode = 0;

#if(DEBUG)
            string strName = parComboBox.Name;

            if (strName == "cboAddress")
            {
                string strStop = "";
            }
#endif
            if (parComboBox.SelectedIndex == -1
            & parComboBox.Enabled == true)
            {
                if (ShowErrorMessage == true)
                {
                    if (parDataViewRow["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                    {
                        TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)parDataViewRow["TAB_CONTROL_HANDLE"]);

                        myTabControl.SelectedIndex = Convert.ToInt32(parDataViewRow["TABPAGE_INDEX"]);
                    }

                    MessageBox.Show(parDataViewRow["REQUIRED_MESSAGE"].ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    parComboBox.Focus();
                }


                intReturnCode = 1;
            }

            return intReturnCode; 
        }

        private int DataBound_Numeric_TextBox_Check(TextBox parTextBox,DataRowView parDataViewRow,bool ShowErrorMessage)
        {
            int intReturnCode = 0;
#if(DEBUG)
            string strName = parTextBox.Name;

            if (strName == "txtTaxRefNo")
            {
                string strStop = "";
            }
#endif
            if (parTextBox.Enabled == true)
            {
                if (parDataViewRow["REQUIRED_IND"].ToString() == "Y"
                    | parDataViewRow["REQUIRED_IND"].ToString() == "E")
                {
                    if (parTextBox.Text == ""
                        | parTextBox.Text == ".")
                    {
                        intReturnCode = 1;
                    }
                    else
                    {
                        decimal dblResult = 0;

                        if (Decimal.TryParse(parTextBox.Text.Trim(), out dblResult) == false)
                        {
                            //Error From Data Layer
                            intReturnCode = 1;
                        }
                        else
                        {
                            if (Convert.ToDecimal(parTextBox.Text.Trim()) == 0)
                            {
                                //ELR 2014-05-03
                                if (parTextBox.Name != "txtNumberMedicalDependents")
                                {
                                    intReturnCode = 1;
                                }
                            }
                            else
                            {
                                if (parDataViewRow["NUMERIC_FIELD_FORMAT"].ToString() != "")
                                {
                                    if (parDataViewRow["NUMERIC_FIELD_FORMAT"].ToString().Replace("-", "").Length != parTextBox.Text.Trim().Length)
                                    {
                                        intReturnCode = 1;
                                    }
                                }
                                else
                                {
                                    if (parDataViewRow["SA_ID_NUMBER_IND"].ToString() == "Y")
                                    {
                                        intReturnCode = SA_Identity_Number_Check(parTextBox.Text.Trim());
                                    }
                                    else
                                    {
                                        if (parDataViewRow["MIN_LENGTH"].ToString() != "")
                                        {
                                            if (parTextBox.Text.Trim().Length < Convert.ToInt32(parDataViewRow["MIN_LENGTH"]))
                                            {
                                                intReturnCode = 1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Numeric Format
                    if (parDataViewRow["NUMERIC_FIELD_FORMAT"].ToString() != "")
                    {
                        if (parTextBox.Text != "")
                        {
                            if (parDataViewRow["NUMERIC_FIELD_FORMAT"].ToString().Replace("-", "").Length != parTextBox.Text.Trim().Length)
                            {
                                intReturnCode = 1;
                            }
                        }
                    }
                    else
                    {
                        if (parDataViewRow["SA_ID_NUMBER_IND"].ToString() == "Y")
                        {
                            if (parDataViewRow["REQUIRED_IND"].ToString() == "N"
                                & parTextBox.Text.Trim() == "")
                            {
                            }
                            else
                            {
                                intReturnCode = SA_Identity_Number_Check(parTextBox.Text.Trim());
                            }
                        }
                        else
                        {
                            if (parDataViewRow["MIN_LENGTH"].ToString() != "")
                            {
                                if (parTextBox.Text.Trim().Length != 0
                                & parTextBox.Text.Trim().Length < Convert.ToInt32(parDataViewRow["MIN_LENGTH"]))
                                {
                                    intReturnCode = 1;
                                }
                                else
                                {
                                    if (parDataViewRow["EFILING_IND"].ToString() == "Y"
                                     & this.pvtblnEFiling == true)
                                    {
                                        if (parTextBox.Text.Trim().Length == 0)
                                        {
                                            intReturnCode = 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (parTextBox.Name == "txtTaxRefNo")
                                {
                                    intReturnCode = eFiling_Income_Tax_Ref_No(parTextBox.Text);
                                }
                            }
                        }
                    }
                }

                if (intReturnCode == 1 
                    & ShowErrorMessage == true)
                {
                    if (parDataViewRow["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                    {
                        TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)parDataViewRow["TAB_CONTROL_HANDLE"]);

                        myTabControl.SelectedIndex = Convert.ToInt32(parDataViewRow["TABPAGE_INDEX"]);
                    }

                    if (parDataViewRow["REQUIRED_MESSAGE"].ToString() == "")
                    {
                        if (parDataViewRow["NUMERIC_FIELD_FORMAT"].ToString() != "")
                        {
                            MessageBox.Show("Field Must be Numeric.\n\nLength = " + parDataViewRow["NUMERIC_FIELD_FORMAT"].ToString().Replace("-", "").Length.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            if (parDataViewRow["MIN_LENGTH"].ToString() != "")
                            {
                                MessageBox.Show(parDataViewRow["REQUIRED_MESSAGE"].ToString() + "\n\nField Must be Numeric.\nMinimum Length = " + parDataViewRow["MIN_LENGTH"].ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show("Field Must be Numeric.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        if (parDataViewRow["NUMERIC_FIELD_FORMAT"].ToString() != "")
                        {
                            MessageBox.Show(parDataViewRow["REQUIRED_MESSAGE"].ToString() + "\n\nField Must be Numeric.\n\nLength = " + parDataViewRow["NUMERIC_FIELD_FORMAT"].ToString().Replace("-", "").Length.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            if (parDataViewRow["MIN_LENGTH"].ToString() != "")
                            {
                                MessageBox.Show(parDataViewRow["REQUIRED_MESSAGE"].ToString() + "\n\nField Must be Numeric.\nMinimum Length = " + parDataViewRow["MIN_LENGTH"].ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show(parDataViewRow["REQUIRED_MESSAGE"].ToString() + "\n\nField Must be Numeric.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    parTextBox.Focus();
                
                }
            }

            return intReturnCode; 
        }

        private int DataBound_TextBox_Check(TextBox parTextBox, DataRowView parDataViewRow, bool ShowErrorMessage)
        {
           int intReturnCode = 0;
#if(DEBUG)
            string strName = parTextBox.Name;

            if (strName == "txtPostStreetName")
            {
               string strStop = "";
            }
#endif
            if (parTextBox.Enabled == true)
            {
                if (parDataViewRow["SPECIAL_ADDR_FIELD_IND"].ToString() == "Y")
                {
                    DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "SPECIAL_ADDR_FIELD_IND = 'Y' AND SPECIAL_FIELD_GROUP = '" + parDataViewRow["SPECIAL_FIELD_GROUP"].ToString() + "'", "TABINDEX1,TABINDEX2,TABINDEX3,TABINDEX4,TABINDEX5,TABINDEX6,CONTROL_NAME", DataViewRowState.CurrentRows);

                    Control myLinkedControl1 = new Control();
                    Control myLinkedControl2 = new Control();
                    Control myLinkedControl3 = new Control();
                    Control myLinkedControl4 = new Control();

                    for (int intRow = 0; intRow < DataView.Count; intRow++)
                    {
                        switch (intRow)
                        {
                            case 0:

                                myLinkedControl1 = Control.FromHandle((IntPtr)DataView[intRow]["CONTROL_HANDLE"]);
                                break;

                            case 1:

                                myLinkedControl2 = Control.FromHandle((IntPtr)DataView[intRow]["CONTROL_HANDLE"]);
                                break;

                            case 2:

                                myLinkedControl3 = Control.FromHandle((IntPtr)DataView[intRow]["CONTROL_HANDLE"]);
                                break;

                            case 3:

                                myLinkedControl4 = Control.FromHandle((IntPtr)DataView[intRow]["CONTROL_HANDLE"]);
                                break;
                        }
                    }

                    if (myLinkedControl1.Text == ""
                       & myLinkedControl2.Text == ""
                       & myLinkedControl3.Text == ""
                       & myLinkedControl4.Text == ""
                       & (this.pvtblnEFiling == false
                       | parDataViewRow["SPECIAL_FIELD_GROUP"].ToString() == "2"))
                    {
                        if (parDataViewRow["SPECIAL_FIELD_GROUP"].ToString() == "1"
                        || parDataViewRow["SPECIAL_FIELD_GROUP"].ToString() == "3"
                        || parDataViewRow["SPECIAL_FIELD_GROUP"].ToString() == "4")
                        {
                            if (myLinkedControl1.Name == "txtCompanyStreetName"
                            || myLinkedControl1.Name == "txtCostCentreStreetName")
                            {
                                Paint_Parent_Marker((TextBox)myLinkedControl1, false);
                            }
                            else
                            {
                                Paint_Parent_Marker((TextBox)myLinkedControl1, true);

                            }
                        }
                        else
                        {
                            Paint_Parent_Marker((TextBox)myLinkedControl1, false);
                        }
                        
                        Paint_Parent_Marker((TextBox)myLinkedControl2, false);
                        Paint_Parent_Marker((TextBox)myLinkedControl3, false);
                        Paint_Parent_Marker((TextBox)myLinkedControl4, false);
                    }
                    else
                    {
                        if (parDataViewRow["SPECIAL_FIELD_GROUP"].ToString() == "1"
                        || parDataViewRow["SPECIAL_FIELD_GROUP"].ToString() == "3"
                        || parDataViewRow["SPECIAL_FIELD_GROUP"].ToString() == "4")
                        {
                            //StreetName
                            if (myLinkedControl1.Text == "")
                            {
                                if (parTextBox.Name == myLinkedControl1.Name)
                                {
                                    intReturnCode = 1;
                                }
                                else
                                {
                                    Paint_Parent_Marker((TextBox)myLinkedControl1, true);
                                }
                            }
                            else
                            {
                                Paint_Parent_Marker((TextBox)myLinkedControl1, false);
                            }

                            if (myLinkedControl2.Text == ""
                            & myLinkedControl3.Text == "")
                            {
                                if (parTextBox.Name == myLinkedControl2.Name)
                                {
                                    Paint_Parent_Marker((TextBox)myLinkedControl3, true);
                                    intReturnCode = 1;
                                }
                                else
                                {
                                    if (parTextBox.Name == myLinkedControl3.Name)
                                    {
                                        Paint_Parent_Marker((TextBox)myLinkedControl2, true);
                                        intReturnCode = 1;
                                    }
                                    else
                                    {
                                        Paint_Parent_Marker((TextBox)myLinkedControl3, true);
                                        Paint_Parent_Marker((TextBox)myLinkedControl2, true);
                                    }
                                }
                            }
                            else
                            {
                                Paint_Parent_Marker((TextBox)myLinkedControl2, false);
                                Paint_Parent_Marker((TextBox)myLinkedControl3, false);
                            }

                            //txtPhysicalCode
                            if (myLinkedControl4.Text.Length == 4
                                & myLinkedControl4.Text != "0000")
                            {
                                Paint_Parent_Marker((TextBox)myLinkedControl4, false);
                            }
                            else
                            {
                                if (parTextBox.Name == myLinkedControl4.Name)
                                {
                                    intReturnCode = 1;
                                }
                                else
                                {
                                    Paint_Parent_Marker((TextBox)myLinkedControl4, true);
                                }
                            }
                        }
                        else
                        {
                            //Group 2
                            //txtPostAddr1
                            if (myLinkedControl1.Text != "")
                            {
                                Paint_Parent_Marker((TextBox)myLinkedControl1, false);
                            }
                            else
                            {
                                if (parTextBox.Name == myLinkedControl1.Name)
                                {
                                    intReturnCode = 1;
                                }
                                else
                                {
                                    Paint_Parent_Marker((TextBox)myLinkedControl1, true);
                                }
                            }

                            Paint_Parent_Marker((TextBox)myLinkedControl2, false);
                            Paint_Parent_Marker((TextBox)myLinkedControl3, false);

                            //txtPostAddrCode
                            if (myLinkedControl4.Text.Length == 4
                                & myLinkedControl4.Text != "0000")
                            {
                                Paint_Parent_Marker((TextBox)myLinkedControl4, false);
                            }
                            else
                            {
                                if (parTextBox.Name == myLinkedControl4.Name)
                                {
                                    intReturnCode = 1;
                                }
                                else
                                {
                                    Paint_Parent_Marker((TextBox)myLinkedControl4, true);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (parDataViewRow["REQUIRED_IND"].ToString() == "Y"
                        | parDataViewRow["REQUIRED_IND"].ToString() == "E")
                    {
                        if (parTextBox.Text.Trim() == "")
                        {
                            if (ShowErrorMessage == true)
                            {
                                if (parDataViewRow["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                                {
                                    TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)parDataViewRow["TAB_CONTROL_HANDLE"]);

                                    myTabControl.SelectedIndex = Convert.ToInt32(parDataViewRow["TABPAGE_INDEX"]);
                                }

                                MessageBox.Show(parDataViewRow["REQUIRED_MESSAGE"].ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                parTextBox.Focus();
                            }

                            intReturnCode = 1;
                        }
                    }
                    else
                    {
                        if (parDataViewRow["EFILING_IND"].ToString() == "Y"
                            & this.pvtblnEFiling == true)
                        {
                            if (parTextBox.Text.Trim() == "")
                            {
                                if (parDataViewRow["EITHER_OR_IND"].ToString() == "Y")
                                {
                                    Control myLinkedControl = Control.FromHandle((IntPtr)parDataViewRow["OTHER_CONTROL_HANDLE"]);

                                    if (myLinkedControl.Text == "")
                                    {
                                        Paint_Parent_Marker((TextBox)myLinkedControl, true);

                                        intReturnCode = 1;
                                    }
                                    else
                                    {
                                        Paint_Parent_Marker((TextBox)myLinkedControl, false);
                                    }
                                }
                                else
                                {
                                    if (parTextBox.Name == "txtUIFRefNo")
                                    {
                                        intReturnCode = eFiling_UIF_Ref_No(parTextBox.Text);
                                    }
                                    else
                                    {
                                        if (parTextBox.Name == "txtSDLRefNo")
                                        {
                                            intReturnCode = eFiling_SDL_Ref_No(parTextBox.Text);
                                        }
                                        else
                                        {
                                            intReturnCode = 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (parDataViewRow["EITHER_OR_IND"].ToString() == "Y")
                                {
                                    Control myLinkedControl = Control.FromHandle((IntPtr)parDataViewRow["OTHER_CONTROL_HANDLE"]);

                                    if (myLinkedControl.Name == "txtPAYERefNo")
                                    {
                                        intReturnCode = eFiling_PAYE_Ref_No(myLinkedControl.Text);

                                        if (intReturnCode == 1)
                                        {
                                            Paint_Parent_Marker((TextBox)myLinkedControl, true);
                                        }
                                        else
                                        {
                                            Paint_Parent_Marker((TextBox)myLinkedControl, false);

                                            intReturnCode = eFiling_Income_Tax_Ref_No(parTextBox.Text);
                                        }
                                    }
                                    else
                                    {
                                        if (myLinkedControl.Name == "txtTaxRefNo")
                                        {
                                            intReturnCode = eFiling_Income_Tax_Ref_No(myLinkedControl.Text);

                                            if (intReturnCode == 1)
                                            {
                                                Paint_Parent_Marker((TextBox)myLinkedControl, true);
                                            }
                                            else
                                            {
                                                Paint_Parent_Marker((TextBox)myLinkedControl, false);

                                                intReturnCode = eFiling_PAYE_Ref_No(parTextBox.Text);
                                            }
                                        }
                                        else
                                        {
                                            Paint_Parent_Marker((TextBox)myLinkedControl, false);
                                        }
                                    }
                                }
                                else
                                {
                                    if (parTextBox.Name == "txtUIFRefNo")
                                    {
                                        intReturnCode = eFiling_UIF_Ref_No(parTextBox.Text);
                                    }
                                    else
                                    {
                                        if (parTextBox.Name == "txtSDLRefNo")
                                        {
                                            intReturnCode = eFiling_SDL_Ref_No(parTextBox.Text);
                                        }
                                        else
                                        {
                                            if (parTextBox.Name == "txtEFileContactNo")
                                            {
                                                //Check Field is NUmeric

                                                decimal dblResult = 0;

                                                if (Decimal.TryParse(parTextBox.Text.Trim(), out dblResult) == false)
                                                {
                                                    //Error From Data Layer
                                                    intReturnCode = 1;
                                                }
                                                else
                                                {
                                                    if (parTextBox.Text.Length < 10)
                                                    {
                                                        intReturnCode = 1;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (parTextBox.Name == "txtEFileEmail")
                                                {
                                                    if (parTextBox.Text.IndexOf("@") == -1)
                                                    {
                                                        intReturnCode = 1;
                                                    }
                                                    else
                                                    {
                                                        if (parTextBox.Text.IndexOf(".") == -1)
                                                        {
                                                            intReturnCode = 1;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (intReturnCode == 1)
                            {
                                if (ShowErrorMessage == true)
                                {
                                    if (parDataViewRow["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                                    {
                                        TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)parDataViewRow["TAB_CONTROL_HANDLE"]);

                                        myTabControl.SelectedIndex = Convert.ToInt32(parDataViewRow["TABPAGE_INDEX"]);
                                    }

                                    MessageBox.Show(parDataViewRow["REQUIRED_MESSAGE"].ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                    parTextBox.Focus();
                                }
                            }
                        }
                        else
                        {
                            //OutSide of Efiling Check
                            if (parTextBox.Name == "txtPAYERefNo")
                            {
                                intReturnCode = eFiling_PAYE_Ref_No(parTextBox.Text);
                            }
                            else
                            {
                                if (parTextBox.Name == "txtTaxRefNo")
                                {
                                    intReturnCode = eFiling_Income_Tax_Ref_No(parTextBox.Text);
                                }
                                else
                                {
                                    if (parTextBox.Name == "txtUIFRefNo")
                                    {
                                        intReturnCode = eFiling_UIF_Ref_No(parTextBox.Text);
                                    }
                                    else
                                    {
                                        if (parTextBox.Name == "txtSDLRefNo")
                                        {
                                            intReturnCode = eFiling_SDL_Ref_No(parTextBox.Text);
                                        }
                                    }
                                }
                            }

                            if (intReturnCode == 1)
                            {
                                if (ShowErrorMessage == true)
                                {
                                    if (parDataViewRow["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                                    {
                                        TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)parDataViewRow["TAB_CONTROL_HANDLE"]);

                                        myTabControl.SelectedIndex = Convert.ToInt32(parDataViewRow["TABPAGE_INDEX"]);
                                    }

                                    MessageBox.Show(parDataViewRow["REQUIRED_MESSAGE"].ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                    parTextBox.Focus();
                                }
                            }
                        }
                    }
                }
            }

            return intReturnCode; 
        }

        private int DataBound_Date_TextBox_Check(TextBox parTextBox,DataRowView parDataViewRow)
        {
            int intReturnCode = 0;

            //Date - Disable Button
            if (parTextBox.Text == "")
            {
                if (parDataViewRow["TAB_CONTROL_HANDLE"] != System.DBNull.Value)
                {
                    TabControl myTabControl = (TabControl)Control.FromHandle((IntPtr)parDataViewRow["TAB_CONTROL_HANDLE"]);

                    myTabControl.SelectedIndex = Convert.ToInt32(parDataViewRow["TABPAGE_INDEX"]);
                }

                MessageBox.Show(parDataViewRow["REQUIRED_MESSAGE"].ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                parTextBox.Focus();

                intReturnCode = 1;
            }

            return intReturnCode;
        }
       
        public void NotDataBound_ComboBox(ComboBox parComboBox, string RequiredMessage)
        {
            DataRow myDataRow = pvtDataSet.Tables["Control"].NewRow();

            myDataRow["CONTROL_NAME"] = parComboBox.Name;
            myDataRow["CONTROL_HANDLE"] = parComboBox.Handle;
            myDataRow["DATABOUND"] = "N";

            TabPage myControlTabPage = Create_Control_TabIndex_For_DataRow(myDataRow, parComboBox);

            if (myControlTabPage != null)
            {
                //Used to Gat Correct Tab Page on Save_Check
                TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                myDataRow["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                myDataRow["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
            }
            
            //ComboBox
            myDataRow["CONTROL_TYPE"] = "C";
            myDataRow["REQUIRED_IND"] = "E";
            myDataRow["REQUIRED_MESSAGE"] = RequiredMessage;
            myDataRow["CONTROL_PARENT_NAME"] = parComboBox.Parent.Name;

            pvtDataSet.Tables["Control"].Rows.Add(myDataRow);

            parComboBox.SelectedIndexChanged += new System.EventHandler(ComboBox_SelectedIndexChanged);
        }

        public void NotDataBound_TextBox(TextBox parTextBox, string RequiredMessage)
        {
            DataRow myDataRow = pvtDataSet.Tables["Control"].NewRow();

            myDataRow["CONTROL_NAME"] = parTextBox.Name;
            myDataRow["CONTROL_HANDLE"] = parTextBox.Handle;
            myDataRow["DATABOUND"] = "N";

            TabPage myControlTabPage = Create_Control_TabIndex_For_DataRow(myDataRow, parTextBox);

            if (myControlTabPage != null)
            {
                //Used to Gat Correct Tab Page on Save_Check
                TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                myDataRow["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                myDataRow["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
            }

            //TextBox
            myDataRow["CONTROL_TYPE"] = "T";
            myDataRow["REQUIRED_IND"] = "E";
            myDataRow["REQUIRED_MESSAGE"] = RequiredMessage;
            myDataRow["CONTROL_PARENT_NAME"] = parTextBox.Parent.Name;

            pvtDataSet.Tables["Control"].Rows.Add(myDataRow);

            parTextBox.TextChanged += new System.EventHandler(TextBox_TextChanged);
        }

        public void NotDataBound_Date_TextBox(TextBox parTextBox, string RequiredMessage)
        {
            DataRow myDataRow = pvtDataSet.Tables["Control"].NewRow();

            myDataRow["CONTROL_NAME"] = parTextBox.Name;
            myDataRow["CONTROL_HANDLE"] = parTextBox.Handle;
            myDataRow["DATABOUND"] = "N";

            TabPage myControlTabPage = Create_Control_TabIndex_For_DataRow(myDataRow, parTextBox);

            if (myControlTabPage != null)
            {
                //Used to Gat Correct Tab Page on Save_Check
                TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                myDataRow["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                myDataRow["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
            }

            //TextBox (Date)
            myDataRow["CONTROL_TYPE"] = "D";
            myDataRow["REQUIRED_IND"] = "E";
            myDataRow["REQUIRED_MESSAGE"] = RequiredMessage;
            myDataRow["CONTROL_PARENT_NAME"] = parTextBox.Parent.Name;

            myDataRow["ENABLE_IN_EDIT_MODE"] = "Y";

            pvtDataSet.Tables["Control"].Rows.Add(myDataRow);

            parTextBox.TextChanged += new System.EventHandler(TextBox_TextChanged);
            //parTextBox.Enabled += new System.EventHandler(TextBox_Enabled);
        }
   
        public void NotDataBound_Control_Paint_Remove(Control parControl)
        {
            string strParentName = parControl.Parent.Name;

            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + parControl.Name + "'", "", DataViewRowState.CurrentRows);

            if (DataView.Count > 0)
            {
                DataView[0].Delete();
                pvtDataSet.Tables["Control"].AcceptChanges();
            }

            Paint_Parent_Marker(parControl, false);

            DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_PARENT_NAME = '" + strParentName + "'", "", DataViewRowState.CurrentRows);

            if (DataView.Count == 0)
            {
                parControl.Parent.Paint -= new PaintEventHandler(Control_Paint);
            }
        }

        public void NotDataBound_Control_Paint(Control parControl, string RequiredMessage)
        {
            DataRow myDataRow = pvtDataSet.Tables["Control"].NewRow();

            myDataRow["CONTROL_NAME"] = parControl.Name;
            myDataRow["CONTROL_HANDLE"] = parControl.Handle;
            myDataRow["DATABOUND"] = "N";

            TabPage myControlTabPage = Create_Control_TabIndex_For_DataRow(myDataRow, parControl);

            if (myControlTabPage != null)
            {
                //Used to Gat Correct Tab Page on Save_Check
                TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                myDataRow["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                myDataRow["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
            }

            //Other
            myDataRow["CONTROL_TYPE"] = "O";
            myDataRow["REQUIRED_IND"] = "E";
            myDataRow["REQUIRED_MESSAGE"] = RequiredMessage;
            myDataRow["CONTROL_PARENT_NAME"] = parControl.Parent.Name;

            pvtDataSet.Tables["Control"].Rows.Add(myDataRow);

            Paint_Parent_Marker(parControl, true);
            parControl.Parent.Paint += new PaintEventHandler(Control_Paint);
        }
      
        public void NotDataBound_Numeric_TextBox(TextBox parTextBox, string RequiredMessage, int NumberDecimals, double MaxNumber)
        {
            DataRow myDataRow = pvtDataSet.Tables["Control"].NewRow();

            myDataRow["CONTROL_NAME"] = parTextBox.Name;
            myDataRow["CONTROL_HANDLE"] = parTextBox.Handle;
            myDataRow["DATABOUND"] = "N";

            TabPage myControlTabPage = Create_Control_TabIndex_For_DataRow(myDataRow, parTextBox);

            if (myControlTabPage != null)
            {
                //Used to Gat Correct Tab Page on Save_Check
                TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                myDataRow["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                myDataRow["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
            }

            //Numeric TextBox
            myDataRow["CONTROL_TYPE"] = "N";

            myDataRow["NUMBER_DECIMALS"] = NumberDecimals;
            myDataRow["MAX_NUMBER"] = MaxNumber;
       
            myDataRow["REQUIRED_IND"] = "E";
            myDataRow["REQUIRED_MESSAGE"] = RequiredMessage;
            myDataRow["CONTROL_PARENT_NAME"] = parTextBox.Parent.Name;

            pvtDataSet.Tables["Control"].Rows.Add(myDataRow);

            //Events
            parTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(DataBind_Numeric_KeyPress);
            parTextBox.TextChanged += new System.EventHandler(TextBox_TextChanged);
        }

        public void DataBind_DataView_To_ComboBox(ComboBox parComboBox,string ValueMember,bool blnRequired,string RequiredMessage,bool blnEnableInEditMode)
        {
            if (pvtFormDataView == null)
            {
                MessageBox.Show("Form DataView has Not been set");
                return;
            }

            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + parComboBox.Name + "'", "", DataViewRowState.CurrentRows);

            if (DataView.Count == 0)
            {
                MessageBox.Show("ComboBox DataBind_ComboBox_Load Has Not Been Set for " + parComboBox.Name);
            }
            else
            {
                bool blnValueMember = false;

                for (int intRow = 0; intRow < pvtFormDataView.Table.Columns.Count; intRow++)
                {
                    if (pvtFormDataView.Table.Columns[intRow].ColumnName == ValueMember)
                    {
                        blnValueMember = true;
                        break;
                    }
                }

                if (blnValueMember == false)
                {
                    MessageBox.Show(ValueMember + " does Not Exist int pvtFormDataView");
                }
                else
                {
                    DataView[0]["DATAVIEW_VALUEMEMBER"] = ValueMember;

                    TabPage myControlTabPage = Create_Control_TabIndex_For_DataView(DataView, parComboBox);

                    if (myControlTabPage != null)
                    {
                        //Used to Gat Correct Tab Page on Save_Check
                        TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                        DataView[0]["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                        DataView[0]["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
                    }

                    if (blnRequired == true)
                    {
                        DataView[0]["REQUIRED_IND"] = "Y";
                    }
                    else
                    {
                        DataView[0]["REQUIRED_IND"] = "N";
                    }

                    if (blnEnableInEditMode == true)
                    {
                        DataView[0]["ENABLE_IN_EDIT_MODE"] = "Y";
                    }
                    else
                    {
                        DataView[0]["ENABLE_IN_EDIT_MODE"] = "N";
                    }

                    if (blnRequired == true)
                    {
                        if (RequiredMessage.Trim() == "")
                        {
                            MessageBox.Show("ComboBox " + parComboBox.Name + "Required Ind and Message Error");
                        }
                        else
                        {
                            if (blnEnableInEditMode == false)
                            {
                                MessageBox.Show("ComboBox " + parComboBox.Name + "Required=true and EnableInEditMode=false");
                            }
                            else
                            {
                                DataView[0]["REQUIRED_MESSAGE"] = RequiredMessage;
                            }
                        }
                    }
                    else
                    {
                        if (RequiredMessage.Trim() != "")
                        {
                            MessageBox.Show("ComboBox " + parComboBox.Name + "Required Ind and Message Error");
                        }
                    }

                    parComboBox.SelectedIndexChanged += new System.EventHandler(ComboBox_SelectedIndexChanged);

                    parComboBox.Enabled = false;
                }
            }
        }

        private TabPage Create_Control_TabIndex_For_DataView(DataView DataView,Control myControl)
        {
            TabPage ControlTabPage = null;
            string strTabIndex = myControl.TabIndex.ToString();

            Control ctlParent = myControl.Parent;

            Get_Control_TabIndex_Continue:
            
            strTabIndex = ctlParent.TabIndex.ToString() + "." + strTabIndex;

            if (ctlParent is TabPage)
            {
                ControlTabPage = (TabPage)ctlParent;
            }
  
            ctlParent = ctlParent.Parent;
          
            if (ctlParent is Form)
            {
                string[] strPieces = strTabIndex.Split('.');

                if (strPieces.Length > 6)
                {
                    MessageBox.Show("Error in Create_Control_TabIndex_For_DataView for Control - " + myControl.Name);
                }
                else
                {
                    int intIndex = 0;

                    for (int intCount = 0; intCount < strPieces.Length; intCount++)
                    {
                        intIndex = intCount + 1;

                        DataView[0]["TABINDEX" + intIndex.ToString()] = strPieces[intCount].ToString();
                    }
                }
            }
            else
            {
                
                goto Get_Control_TabIndex_Continue;
            }

            return ControlTabPage;
        }

        private TabPage Create_Control_TabIndex_For_DataRow(DataRow DataRow, Control myControl)
        {
            TabPage ControlTabPage = null;
            string strTabIndex = myControl.TabIndex.ToString();

            Control ctlParent = myControl.Parent;

        Create_Control_TabIndex_For_DataRow_Continue:

            strTabIndex = ctlParent.TabIndex.ToString() + "." + strTabIndex;

            if (ctlParent is TabPage)
            {
                ControlTabPage = (TabPage)ctlParent;
            }

            ctlParent = ctlParent.Parent;

            if (ctlParent is Form)
            {
                string[] strPieces = strTabIndex.Split('.');

                if (strPieces.Length > 6)
                {
                    MessageBox.Show("Error in Create_Control_TabIndex_For_DataView for Control - " + myControl.Name);
                }
                else
                {
                    int intIndex = 0;

                    for (int intCount = 0; intCount < strPieces.Length; intCount++)
                    {
                        intIndex = intCount + 1;

                        DataRow["TABINDEX" + intIndex.ToString()] = strPieces[intCount].ToString();
                    }
                }
            }
            else
            {
                goto Create_Control_TabIndex_For_DataRow_Continue;
            }

            return ControlTabPage;
        }

        public void DataBind_Control_Required_If_Enabled(Control parControl, string RequiredMessage)
        {
            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + parControl.Name + "'", "", DataViewRowState.CurrentRows);

            if (DataView.Count == 0)
            {
                MessageBox.Show("ComboBox DataBind_Control_Required_If_Enabled Does Not Exist - " + parControl.Name);
            }
            else
            {
                if (DataView[0]["REQUIRED_IND"].ToString() == "Y")
                {
                    MessageBox.Show("ComboBox DataBind_Control_Required_If_Enabled CONFLICT - " + parControl.Name);
                }
                else
                {
                    DataView[0]["REQUIRED_IND"] = "E";
                    DataView[0]["REQUIRED_MESSAGE"] = RequiredMessage;
                }
            }
        }

        public void DataBind_Numeric_Field_Formatting(Control parControl, string FieldFormat)
        {
            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + parControl.Name + "'", "", DataViewRowState.CurrentRows);

            if (DataView.Count == 0)
            {
                MessageBox.Show(parControl.Name + " Must be DataBound before DataBind_Numeric_Field_Length_Or_Formatting can Be Proceessed.");
            }
            else
            {
                if (DataView[0]["NUMBER_DECIMALS"] == System.DBNull.Value)
                {
                    MessageBox.Show(parControl.Name + " Needs to be Setup as Numeric.");
                }
                else
                {
                    if (FieldFormat.IndexOf("0") == -1)
                    {
                        MessageBox.Show(parControl.Name + " Field Format " + FieldFormat + " does NOT Have '9'");

                    }
                    else
                    {

                        DataView[0]["NUMERIC_FIELD_FORMAT"] = FieldFormat;
                    }
                }
            }
        }

        public void DataBind_Special_Field_Remove(int parLinkedGroup)
        {
            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "SPECIAL_ADDR_FIELD_IND = 'Y' AND SPECIAL_FIELD_GROUP = '" + parLinkedGroup.ToString() + "'","", DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < DataView.Count; intRow++)
            {
                Control myControl = Control.FromHandle((IntPtr)DataView[intRow]["CONTROL_HANDLE"]);

                myControl.TextChanged -= new System.EventHandler(TextBox_TextChanged);
         
                if (myControl.Name == "txtPhysicalCode"
                    | myControl.Name == "txtPostAddrCode")
                {
                    myControl.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(DataBind_Numeric_KeyPress);
                }
         
                DataView[intRow].Delete();

                Paint_Parent_Marker(myControl, false);

                intRow -= 1;
            }

            pvtDataSet.Tables["Control"].AcceptChanges();
        }

        public void DataBind_Field_Remove(Control parControl)
        {
            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + parControl.Name + "'", "", DataViewRowState.CurrentRows);

            if (DataView.Count > 0)
            {
                if (DataView[0]["CONTROL_TYPE"].ToString() == "T")
                {
                    if (DataView[0]["CONTROL_HANDLE"] != System.DBNull.Value)
                    {
                        parControl.TextChanged -= new System.EventHandler(TextBox_TextChanged);
                    }

                    if (DataView[0]["NUMBER_DECIMALS"].ToString() != "")
                    {
                        //Numeric
                        parControl.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(DataBind_Numeric_KeyPress);
                    }

                    DataView[0].Delete();
                }
                else
                {
                    if (DataView[0]["CONTROL_TYPE"].ToString() == "C")
                    {
                        DataView[0]["DATAVIEW_VALUEMEMBER"] = System.DBNull.Value;
                        DataView[0]["TAB_CONTROL_HANDLE"] = System.DBNull.Value;
                        DataView[0]["TABPAGE_INDEX"] = System.DBNull.Value;

                        ComboBox parComboBox = (ComboBox)parControl;

                        parComboBox.SelectedIndexChanged -= new System.EventHandler(ComboBox_SelectedIndexChanged);
                    }
                }

                Paint_Parent_Marker(parControl, false);

                pvtDataSet.Tables["Control"].AcceptChanges();
           }
        }

        public void DataBind_Special_Field(Control parControl, int parLinkedGroup)
        {
            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + parControl.Name + "'", "", DataViewRowState.CurrentRows);

            if (DataView.Count == 0)
            {
                MessageBox.Show(parControl.Name + " Must be DataBound before DataBind_Special_Field_Formatting can Be Proceessed.");
            }
            else
            {
                DataView[0]["SPECIAL_ADDR_FIELD_IND"] = "Y";
                DataView[0]["SPECIAL_FIELD_GROUP"] = parLinkedGroup.ToString();

                if (parControl.Name == "txtPhysicalCode"
                    | parControl.Name == "txtPostAddrCode")
                {
                    DataView[0]["NUMBER_DECIMALS"] = "0";
                    DataView[0]["MAX_NUMBER"] = 0;
                    //Special Field
                    parControl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(DataBind_Numeric_KeyPress);
                }
            }
        }

        public void DataBind_Numeric_Field_SA_ID_Number(Control parControl)
        {
            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + parControl.Name + "'", "", DataViewRowState.CurrentRows);

            if (DataView.Count == 0)
            {
                MessageBox.Show(parControl.Name + " Must be DataBound before DataBind_Numeric_Field_SA_ID_Number can Be Proceessed.");
            }
            else
            {
                if (DataView[0]["NUMBER_DECIMALS"] == System.DBNull.Value)
                {
                    MessageBox.Show(parControl.Name + " Needs to be Setup as Numeric.");
                }
                else
                {
                    DataView[0]["SA_ID_NUMBER_IND"] = "Y";
                }
            }
        }

        public int SA_Identity_Number_Check(string ID)
        {
            int intReturnCode = 1;

            if (ID.Length == 13)
            {
                try
                {
                    decimal dblResult = 0;

                    if (Decimal.TryParse(ID.Trim(), out dblResult) == true)
                    {
                        string strYear = "20" + ID.Substring(0, 2);
                        string strOddCharacters = "";
                        int intEvenCharacters = 0;
                        int intOddCharacters = 0;
                        int intNewOddCharacters = 0;

                        //Check First 6 Characters is a Valid Date
                        DateTime dtDateTime = new DateTime(Convert.ToInt32(strYear), Convert.ToInt32(ID.Substring(2, 2)), Convert.ToInt32(ID.Substring(4, 2)));

                        //1) - Add all Digits in Odd Positions (Except last Character)
                        for (int intCount = 0; intCount < 12; intCount = intCount + 2)
                        {
                            intEvenCharacters += Convert.ToInt32(ID.Substring(intCount, 1));
                        }

                        //2) a) Move all digits in Even Positions to New Field 
                        for (int intCount = 1; intCount < 13; intCount = intCount + 2)
                        {
                            strOddCharacters += ID.Substring(intCount, 1);
                        }

                        //2) b) Multiply by 2
                        intOddCharacters = Convert.ToInt32(strOddCharacters) * 2;

                        //2) c) Add Individual Digits Together
                        for (int intCount = 0; intCount < intOddCharacters.ToString().Length; intCount++)
                        {
                            intNewOddCharacters += Convert.ToInt32(intOddCharacters.ToString().Substring(intCount, 1));
                        }

                        //3) - Check for Valid ID Number
                        if ((intEvenCharacters + intNewOddCharacters + Convert.ToInt32(ID.Substring(12, 1))) % 10 == 0)
                        {
                            intReturnCode = 0;
                        }
                    }
                }
                catch
                {
                }
            }

            return intReturnCode;
        }

        private void Control_Paint(object sender, PaintEventArgs e)
        {
            Control myControl = (Control)sender;

            bool blnPaintInd = false;
            int intReturnCode = 0;
            string strFilter = "";

            if (this.pvtblnEFiling == true)
            {
                strFilter = " OR EFILING_IND = 'Y'";
            }

            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_PARENT_NAME = '" + myControl.Name + "' AND REQUIRED_IND IN ('Y','E') OR NOT NUMBER_DECIMALS IS NULL OR SPECIAL_ADDR_FIELD_IND = 'Y'" + strFilter, "", DataViewRowState.CurrentRows);

            for (int intRow = 0; intRow < DataView.Count; intRow++)
            {
                blnPaintInd = false;
                              
                Control myControlPaint = Control.FromHandle((IntPtr)DataView[intRow]["CONTROL_HANDLE"]);

                //2012-09-10
                if (myControlPaint.Visible == false)
                {
                    continue;
                }
#if(DEBUG)
                string strName = myControlPaint.Name;

                if (strName == "txtCompanyStreetName")
                {
                    if (myControlPaint.Enabled == true)
                    {
                        string strStop = "";
                    }
                }
#endif
                if (myControlPaint.Enabled == true)
                {
                    if (DataView[intRow]["CONTROL_TYPE"].ToString() == "C")
                    {
                        intReturnCode = DataBound_ComboBox_Check((ComboBox)myControlPaint, DataView[intRow], false);

                        if (intReturnCode == 1)
                        {
                            blnPaintInd = true;
                        }
                    }
                    else
                    {
                        if (DataView[intRow]["NUMBER_DECIMALS"].ToString() != ""
                         & DataView[intRow]["SPECIAL_ADDR_FIELD_IND"].ToString() != "Y")
                        {
                            //Numeric Field
                            intReturnCode = DataBound_Numeric_TextBox_Check((TextBox)myControlPaint, DataView[intRow], false);

                            if (intReturnCode == 1)
                            {
                                blnPaintInd = true;
                            }
                        }
                        else
                        {
                            //TextBox
                            if (DataView[intRow]["CONTROL_TYPE"].ToString() == "T")
                            {
                                intReturnCode = DataBound_TextBox_Check((TextBox)myControlPaint, DataView[intRow], false);

                                if (intReturnCode == 1)
                                {
                                    blnPaintInd = true;
                                }
                            }
                            else
                            {
                                //Other
                                if (DataView[intRow]["CONTROL_TYPE"].ToString() == "O")
                                {
                                    blnPaintInd = true;
                                }
                                else
                                {
                                    //2012-09-10
                                    if (DataView[intRow]["CONTROL_TYPE"].ToString() == "D"
                                    & myControlPaint.Text.Trim() == "")
                                    {
                                        blnPaintInd = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Date
                    if (DataView[intRow]["CONTROL_TYPE"].ToString() == "D"
                        & myControlPaint.Text.Trim() == "")
                    {
                        blnPaintInd = true;
                    }
                    else
                    {
                        if (DataView[intRow]["CONTROL_TYPE"].ToString() == "O")
                        {
                            blnPaintInd = true;
                        }
                    }
                }
                    
                Paint_Parent_Marker(myControlPaint, blnPaintInd);
         
            }
        }

        public void Label_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            myPaintEventArgs = e;

            System.Windows.Forms.Label lbl = (System.Windows.Forms.Label)sender;

            Paint_Label(lbl);
        }

        private void Paint_Label(System.Windows.Forms.Label lbl)
        {
            Paint_Label_Background_Area(lbl);

            Paint_Label_Text(lbl);
        }
         
        private void Paint_Label_Text(System.Windows.Forms.Label lbl)
        {
            System.Drawing.Rectangle rect = new Rectangle(lbl.Left, lbl.Top, lbl.Width, lbl.Height);
            System.Drawing.SizeF size = myPaintEventArgs.Graphics.MeasureString(lbl.Text, lbl.Font);

            System.Drawing.Point pt = new System.Drawing.Point((rect.Width - Convert.ToInt32(size.Width)) / 2, (rect.Height - Convert.ToInt32(size.Height)) / 2);

            myPaintEventArgs.Graphics.DrawString(lbl.Text.Replace("&", ""), lbl.Font, new SolidBrush(lbl.ForeColor), pt.X, pt.Y);
        }

        private void Paint_Label_Background_Area(System.Windows.Forms.Label lbl)
        {
            System.Drawing.Color color = lbl.BackColor;

            Color[] ColorArray = null;
            float[] PositionArray = null;

            ColorArray = new Color[]{ColourBlend(color,System.Drawing.SystemColors.HighlightText,20),
                                        ColourBlend(color,System.Drawing.SystemColors.HighlightText,30),
                                        ColourBlend(color,System.Drawing.SystemColors.HighlightText,20),
                                        ColourBlend(color,System.Drawing.SystemColors.HighlightText,00),               
                                        ColourBlend(color,System.Drawing.SystemColors.MenuText,20),
                                        ColourBlend(color,System.Drawing.SystemColors.MenuText,10),};

            PositionArray = new float[] { 0.0f, .15f, .40f, .65f, .80f, 1.0f };
         
            System.Drawing.Drawing2D.ColorBlend blend = new System.Drawing.Drawing2D.ColorBlend();
            blend.Colors = ColorArray;
            blend.Positions = PositionArray;

            System.Drawing.Rectangle rect = new Rectangle(0, 0, lbl.Width, lbl.Height);

            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, System.Drawing.SystemColors.MenuText, ColourBlend(System.Drawing.SystemColors.MenuText, System.Drawing.SystemColors.MenuText, 10), System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            brush.InterpolationColors = blend;

            myPaintEventArgs.Graphics.FillRectangle(brush, rect);

            brush.Dispose();
        }

        private static Color ColourBlend(Color SColor, Color DColor, int Percentage)
        {
            int r = SColor.R + ((DColor.R - SColor.R) * Percentage) / 100;
            int g = SColor.G + ((DColor.G - SColor.G) * Percentage) / 100;
            int b = SColor.B + ((DColor.B - SColor.B) * Percentage) / 100;
            return Color.FromArgb(r, g, b);
        }
        
        public void DataBind_DataView_To_TextBox(TextBox parTextBox, string ValueMember, bool blnRequired, string RequiredMessage, bool blnEnableInEditMode)
        {
            if (pvtFormDataView == null)
            {
                MessageBox.Show("Form DataView has Not been set");
                return;
            }

            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + parTextBox.Name + "'", "", DataViewRowState.CurrentRows);

            if (DataView.Count > 0)
            {
                string strStop = "";
            }

            DataRow DataRow = pvtDataSet.Tables["Control"].NewRow();

            DataRow["CONTROL_NAME"] = parTextBox.Name;
            DataRow["CONTROL_HANDLE"] = parTextBox.Handle;
                  
            TabPage myControlTabPage = Create_Control_TabIndex_For_DataRow(DataRow, parTextBox);

            if (myControlTabPage != null)
            {
                //Used to Gat Correct Tab Page on Save_Check
                TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                DataRow["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                DataRow["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
            }
                
            DataRow["CONTROL_TYPE"] = "T";
            DataRow["DATABOUND"] = "Y";
            DataRow["DATAVIEW_VALUEMEMBER"] = ValueMember;

            if (ValueMember == this.pvtFormDataView.Sort)
            {
                DataRow["CONTROL_IS_DATAVIEW_SORT_COLUMN_IND"] = "Y";
            }
                 
            if (blnRequired == true)
            {
                DataRow["REQUIRED_IND"] = "Y";
            }
            else
            {
                DataRow["REQUIRED_IND"] = "N";
            }

            DataRow["CONTROL_PARENT_NAME"] = parTextBox.Parent.Name;

            if (blnEnableInEditMode == true)
            {
                DataRow["ENABLE_IN_EDIT_MODE"] = "Y";
            }
            else
            {
                DataRow["ENABLE_IN_EDIT_MODE"] = "N";
            }

            if (blnRequired == true)
            {
                if (RequiredMessage.Trim() == "")
                {
                    MessageBox.Show("TextBox " + parTextBox.Name + "Required Ind and Message Error");
                }
                else
                {
                    if (blnEnableInEditMode == false)
                    {
                        MessageBox.Show("TextBox " + parTextBox.Name + "Required=true and EnableInEditMode=false");
                    }
                    else
                    {
                        DataRow["REQUIRED_MESSAGE"] = RequiredMessage;
                    }
                }
            }
            else
            {
                if (RequiredMessage.Trim() != "")
                {
                    MessageBox.Show("TextBox " + parTextBox.Name + "Required Ind and Message Error");
                }
            }

            //Add Row
            pvtDataSet.Tables["Control"].Rows.Add(DataRow);
               
            parTextBox.TextChanged += new System.EventHandler(TextBox_TextChanged);

            parTextBox.Enabled = false;
        }

        public void DataBind_DataView_Field_EFiling(TextBox parTextBox)
        {
            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + parTextBox.Name + "'", "", DataViewRowState.CurrentRows);

            if (DataView.Count == 0)
            {
                MessageBox.Show(parTextBox.Name + " Must be DataBound before DataBind_DataView_Field_EFiling can Be Proceessed.");
            }
            else
            {
                DataView[0]["EFILING_IND"] = "Y";
            }
        }

        public void NotDataBound_ComboBox_EFiling(ComboBox parComboBox, string RequiredMessage)
        {
            DataRow myDataRow = pvtDataSet.Tables["Control"].NewRow();

            myDataRow["CONTROL_NAME"] = parComboBox.Name;
            myDataRow["CONTROL_HANDLE"] = parComboBox.Handle;
            myDataRow["DATABOUND"] = "N";

            TabPage myControlTabPage = Create_Control_TabIndex_For_DataRow(myDataRow, parComboBox);

            if (myControlTabPage != null)
            {
                //Used to Gat Correct Tab Page on Save_Check
                TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                myDataRow["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                myDataRow["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
            }

            //ComboBox
            myDataRow["CONTROL_TYPE"] = "C";
            myDataRow["REQUIRED_IND"] = "N";
            myDataRow["EFILING_IND"] = "Y";
            myDataRow["REQUIRED_MESSAGE"] = RequiredMessage;
            myDataRow["CONTROL_PARENT_NAME"] = parComboBox.Parent.Name;

            pvtDataSet.Tables["Control"].Rows.Add(myDataRow);

            parComboBox.SelectedIndexChanged += new System.EventHandler(ComboBox_SelectedIndexChanged);
        }

        public void DataBind_DataView_To_TextBox_EFiling(TextBox parTextBox, string ValueMember,bool patblnNumeric, string RequiredMessage)
        {
            if (pvtFormDataView == null)
            {
                MessageBox.Show("Form DataView has Not been set");
                return;
            }

            DataRow DataRow = pvtDataSet.Tables["Control"].NewRow();

            DataRow["CONTROL_NAME"] = parTextBox.Name;
            DataRow["CONTROL_HANDLE"] = parTextBox.Handle;

            TabPage myControlTabPage = Create_Control_TabIndex_For_DataRow(DataRow, parTextBox);

            if (myControlTabPage != null)
            {
                //Used to Gat Correct Tab Page on Save_Check
                TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                DataRow["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                DataRow["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
            }

            DataRow["CONTROL_TYPE"] = "T";
            DataRow["DATABOUND"] = "Y";
            DataRow["DATAVIEW_VALUEMEMBER"] = ValueMember;

            if (ValueMember == this.pvtFormDataView.Sort)
            {
                DataRow["CONTROL_IS_DATAVIEW_SORT_COLUMN_IND"] = "Y";
            }

            DataRow["EFILING_IND"] = "Y";

            DataRow["REQUIRED_IND"] = "N";

            DataRow["CONTROL_PARENT_NAME"] = parTextBox.Parent.Name;

            DataRow["ENABLE_IN_EDIT_MODE"] = "Y";

            if (RequiredMessage.Trim() == "")
            {
                MessageBox.Show("TextBox " + parTextBox.Name + "Required Ind and Message Error");
            }
            else
            {
                DataRow["REQUIRED_MESSAGE"] = RequiredMessage;
            }

            //if (patblnNumeric == true)
            //{
            //    DataRow["MAX_NUMBER"] = 0;
            //    DataRow["NUMBER_DECIMALS"] = 0;
            //}

            //Add Row
            pvtDataSet.Tables["Control"].Rows.Add(DataRow);

            parTextBox.TextChanged += new System.EventHandler(TextBox_TextChanged);

            if (patblnNumeric == true)
            {
                parTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(Numeric_KeyPress);
            }

            parTextBox.Enabled = false;
        }


        public void DataBind_DataView_To_TextBox_EFiling_Either_Or(TextBox parTextBox,TextBox parOtherTextBox, string ValueMember, bool patblnNumeric, string RequiredMessage)
        {
            if (pvtFormDataView == null)
            {
                MessageBox.Show("Form DataView has Not been set");
                return;
            }

            DataRow DataRow = pvtDataSet.Tables["Control"].NewRow();

            DataRow["CONTROL_NAME"] = parTextBox.Name;
            DataRow["CONTROL_HANDLE"] = parTextBox.Handle;
            DataRow["OTHER_CONTROL_HANDLE"] = parOtherTextBox.Handle;

            DataRow["EITHER_OR_IND"] = "Y";
       
            TabPage myControlTabPage = Create_Control_TabIndex_For_DataRow(DataRow, parTextBox);

            if (myControlTabPage != null)
            {
                //Used to Gat Correct Tab Page on Save_Check
                TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                DataRow["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                DataRow["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
            }

            DataRow["CONTROL_TYPE"] = "T";
            DataRow["DATABOUND"] = "Y";
            DataRow["DATAVIEW_VALUEMEMBER"] = ValueMember;

            if (ValueMember == this.pvtFormDataView.Sort)
            {
                DataRow["CONTROL_IS_DATAVIEW_SORT_COLUMN_IND"] = "Y";
            }

            DataRow["EFILING_IND"] = "Y";

            DataRow["REQUIRED_IND"] = "N";

            DataRow["CONTROL_PARENT_NAME"] = parTextBox.Parent.Name;

            DataRow["ENABLE_IN_EDIT_MODE"] = "Y";

            if (RequiredMessage.Trim() == "")
            {
                MessageBox.Show("TextBox " + parTextBox.Name + "Required Ind and Message Error");
            }
            else
            {
                DataRow["REQUIRED_MESSAGE"] = RequiredMessage;
            }

            //if (patblnNumeric == true)
            //{
            //    DataRow["MAX_NUMBER"] = 0;
            //    DataRow["NUMBER_DECIMALS"] = 0;
            //}

            //Add Row
            pvtDataSet.Tables["Control"].Rows.Add(DataRow);

            parTextBox.TextChanged += new System.EventHandler(TextBox_TextChanged);

            if (patblnNumeric == true)
            {
                parTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(Numeric_KeyPress);
            }

            parTextBox.Enabled = false;
        }



        public void DataBind_DataView_To_RadioButton(RadioButton parRadioButton, string ValueMember,string DBValue)
        {
            if (pvtFormDataView == null)
            {
                MessageBox.Show("Form DataView has Not been set");
                return;
            }

            bool blnFirstMember = true;

            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "DATAVIEW_VALUEMEMBER = '" + ValueMember + "'", "", DataViewRowState.CurrentRows);

            if (DataView.Count > 0)
            {
                blnFirstMember = false;
            }

            DataRow DataRow = pvtDataSet.Tables["Control"].NewRow();

            DataRow["CONTROL_NAME"] = parRadioButton.Name;
            DataRow["CONTROL_HANDLE"] = parRadioButton.Handle;

            //RadioButton
            DataRow["CONTROL_TYPE"] = "R";
            DataRow["DATABOUND"] = "Y";
            DataRow["DATAVIEW_VALUEMEMBER"] = ValueMember;
            //Used to Tie Up DB Value and RadioButton Checked
            DataRow["DB_VALUE"] = DBValue;

            DataRow["ENABLE_IN_EDIT_MODE"] = "Y";

            if (blnFirstMember == true)
            {
                DataRow["FIRST_MEMBER_IND"] = "Y";
            }
            
            //Add Row
            pvtDataSet.Tables["Control"].Rows.Add(DataRow);

            parRadioButton.CheckedChanged += new System.EventHandler(RadioButton_CheckedChanged);

            parRadioButton.Enabled = false;
        }

        public void DataBind_RadioButton_Default(RadioButton parRadioButton)
        {
            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + parRadioButton.Name + "'", "", DataViewRowState.CurrentRows);

            if (DataView.Count == 0)
            {
                MessageBox.Show("RadioButton '" + parRadioButton.Name + "' Needs to be Initialised in DataBind_DataView_To_RadioButton");
            }
            else
            {
                DataView[0]["DEFAULT_IND"] = "Y";
            }
        }
               
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pvtblnFormInEditMode == true)
            {
                ComboBox myComboBox = (ComboBox)sender;
#if(DEBUG)
                string strComboName = myComboBox.Name;

                if (strComboName == "cboMinShiftHours")
                {
                    string strStop = "";
                }
#endif
                bool blnPaintInd = false;
                
                DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + myComboBox.Name + "'", "", DataViewRowState.CurrentRows);

                if (DataView.Count > 0)
                {
                    if (DataView[0]["DATABOUND"].ToString() == "Y")
                    {
                        if (myComboBox.SelectedIndex == -1)
                        {
                            if (pvtFormDataView.Table.Columns[DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()].DataType.ToString() == "System.String")
                            {
                                pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = "";
                            }
                            else
                            {
                                pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = 0;
                            }
                        }
                        else
                        {
                            if (pvtFormDataView.Table.Columns[DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()].DataType.ToString() == "System.String")
                            {
                                pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = pvtFormDataSet.Tables[DataView[0]["TABLENAME"].ToString()].Rows[myComboBox.SelectedIndex][DataView[0]["VALUEMEMBER"].ToString()].ToString();
                            }
                            else
                            {
                                pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = Convert.ToInt32(pvtFormDataSet.Tables[DataView[0]["TABLENAME"].ToString()].Rows[myComboBox.SelectedIndex][DataView[0]["VALUEMEMBER"].ToString()].ToString());
                            }
                        }
                    }
                    
                    if (myComboBox.Enabled == true
                    & myComboBox.Text.Trim() == "")
                    {
                        blnPaintInd = true;
                    }

                    Paint_Parent_Marker(myComboBox, blnPaintInd);
                }
                else
                {
                    //ComboBox is Attached to GroupBox
                    GroupBox myGroupBox = (GroupBox)myComboBox.Parent;

                    DataView = new System.Data.DataView(pvtDataSet.Tables["Paint"], "CONTROL_NAME = '" + myGroupBox.Name + "'", "", DataViewRowState.CurrentRows);

                    Control myControlGroup = Control.FromHandle((IntPtr)DataView[0]["CONTROL_HANDLE"]);

                    foreach (Control myChildControl in myControlGroup.Controls)
                    {
                        blnPaintInd = true;

                        if (myChildControl is TextBox
                            | myChildControl is ComboBox)
                        {
                            if (myChildControl.Text.Trim() != "")
                            {
                                blnPaintInd = false;

                                break;
                            }
                        }
                    }

                    Paint_Parent_Marker(myControlGroup, blnPaintInd);
                }
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (pvtblnFormInEditMode == true)
            {
                RadioButton myRadioButton = (RadioButton)sender;

                if (myRadioButton.Checked == true)
                {
#if(DEBUG)
                    string strName = myRadioButton.Name;

                    if (strName == "txtTelHome")
                    {
                        string strStop = "";
                    }
#endif
                    DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + myRadioButton.Name + "'", "", DataViewRowState.CurrentRows);

                    pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = DataView[0]["DB_VALUE"].ToString();
                }
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (pvtblnFormInEditMode == true)
            {
                if (pvtblnRemovingFormatting == false)
                {
                    TextBox myTextBox = (TextBox)sender;

#if(DEBUG)
                    string strName = myTextBox.Name;

                    if (strName == "txtRate1")
                    {
                        string strStop = "";
                    }
#endif
                    bool blnPaintInd = false;
                    int intReturnCode = 0;

                    DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + myTextBox.Name + "'", "", DataViewRowState.CurrentRows);

                    if (DataView[0]["SPECIAL_ADDR_FIELD_IND"].ToString() == "Y")
                    {
                        DataView myDataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "SPECIAL_ADDR_FIELD_IND = 'Y' AND SPECIAL_FIELD_GROUP = '" + DataView[0]["SPECIAL_FIELD_GROUP"].ToString() + "'", "TABINDEX1,TABINDEX2,TABINDEX3,TABINDEX4,TABINDEX5,TABINDEX6,CONTROL_NAME", DataViewRowState.CurrentRows);

                        Control myLinkedControl1 = new Control();
                        Control myLinkedControl2 = new Control();
                        Control myLinkedControl3 = new Control();
                        Control myLinkedControl4 = new Control();

                        for (int intRow = 0; intRow < myDataView.Count; intRow++)
                        {
                            switch (intRow)
                            {
                                case 0:

                                    myLinkedControl1 = Control.FromHandle((IntPtr)myDataView[intRow]["CONTROL_HANDLE"]);
                                    break;

                                case 1:

                                    myLinkedControl2 = Control.FromHandle((IntPtr)myDataView[intRow]["CONTROL_HANDLE"]);
                                    break;

                                case 2:

                                    myLinkedControl3 = Control.FromHandle((IntPtr)myDataView[intRow]["CONTROL_HANDLE"]);
                                    break;

                                case 3:

                                    myLinkedControl4 = Control.FromHandle((IntPtr)myDataView[intRow]["CONTROL_HANDLE"]);
                                    break;
                            }
                        }

                      if (myLinkedControl1.Text == ""
                            & myLinkedControl2.Text == ""
                            & myLinkedControl3.Text == ""
                            & myLinkedControl4.Text == ""
                            & (this.pvtblnEFiling == false
                            | DataView[0]["SPECIAL_FIELD_GROUP"].ToString() == "2"))
                        {
                            Paint_Parent_Marker(myLinkedControl1, false);
                            Paint_Parent_Marker(myLinkedControl2, false);
                            Paint_Parent_Marker(myLinkedControl3, false);
                            Paint_Parent_Marker(myLinkedControl4, false);
                        }
                        else
                        {
                            if (DataView[0]["SPECIAL_FIELD_GROUP"].ToString() == "1"
                            || DataView[0]["SPECIAL_FIELD_GROUP"].ToString() == "3"
                            || DataView[0]["SPECIAL_FIELD_GROUP"].ToString() == "4")
                            {
                                //StreetName
                                if (myLinkedControl1.Name == myTextBox.Name)
                                {
                                    if (myLinkedControl1.Text == "")
                                    {
                                        blnPaintInd = true;
                                    }
                                }
                                else
                                {
                                    if (myLinkedControl1.Text.Trim() == "")
                                    {
                                        Paint_Parent_Marker(myLinkedControl1, true);
                                    }
                                    else
                                    {
                                        Paint_Parent_Marker(myLinkedControl1, false);
                                    }
                                }


                                if (myLinkedControl2.Name == myTextBox.Name
                                    | myLinkedControl3.Name == myTextBox.Name)
                                {
                                    if (myLinkedControl2.Text == ""
                                    & myLinkedControl3.Text == "")
                                    {
                                        if (myLinkedControl2.Name == myTextBox.Name)
                                        {
                                            Paint_Parent_Marker(myLinkedControl3, true);
                                        }
                                        else
                                        {
                                            Paint_Parent_Marker(myLinkedControl2, true);
                                        }

                                        blnPaintInd = true;
                                    }
                                    else
                                    {
                                        if (myLinkedControl2.Name == myTextBox.Name)
                                        {
                                            Paint_Parent_Marker(myLinkedControl3, false);
                                        }
                                        else
                                        {
                                            Paint_Parent_Marker(myLinkedControl2, false);
                                        }

                                    }
                                }
                                else
                                {
                                    if (myLinkedControl2.Text == ""
                                    & myLinkedControl3.Text == "")
                                    {
                                        Paint_Parent_Marker(myLinkedControl2, true);
                                        Paint_Parent_Marker(myLinkedControl3, true);
                                    }
                                    else
                                    {
                                        Paint_Parent_Marker(myLinkedControl2, false);
                                        Paint_Parent_Marker(myLinkedControl3, false);
                                    }
                                }

                                if (myLinkedControl4.Name == myTextBox.Name)
                                {
                                    //txtPhysicalCode
                                    if (myLinkedControl4.Text.Length == 4
                                    & myLinkedControl4.Text != "0000")
                                    {
                                    }
                                    else
                                    {
                                        blnPaintInd = true;
                                    }
                                }
                                else
                                {
                                    //txtPhysicalCode
                                    if (myLinkedControl4.Text.Length == 4
                                    & myLinkedControl4.Text != "0000")
                                    {
                                        Paint_Parent_Marker(myLinkedControl4, false);
                                    }
                                    else
                                    {
                                        Paint_Parent_Marker(myLinkedControl4, true);
                                    }
                                }
                            }
                            else
                            {
                                //Group 2
                                //txtPostAddr1
                                if (myLinkedControl1.Text != "")
                                {
                                    Paint_Parent_Marker((TextBox)myLinkedControl1, false);
                                }
                                else
                                {
                                    if (myLinkedControl1.Name == myTextBox.Name)
                                    {
                                        blnPaintInd = true;
                                    }
                                    else
                                    {
                                        Paint_Parent_Marker((TextBox)myLinkedControl1, true);
                                    }
                                }

                                Paint_Parent_Marker((TextBox)myLinkedControl2, false);
                                Paint_Parent_Marker((TextBox)myLinkedControl3, false);

                                //txtPostAddrCode
                                if (myLinkedControl4.Text.Length == 4
                                    & myLinkedControl4.Text != "0000")
                                {
                                    Paint_Parent_Marker((TextBox)myLinkedControl4, false);
                                }
                                else
                                {
                                    if (myLinkedControl4.Name == myTextBox.Name)
                                    {
                                        blnPaintInd = true;
                                    }
                                    else
                                    {
                                        Paint_Parent_Marker((TextBox)myLinkedControl4, true);
                                    }
                                }
                            }
                        }

                        if (myTextBox.Text.Trim() != "")
                        {
                            pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = myTextBox.Text.Trim();
                        }
                        else
                        {
                            pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = System.DBNull.Value;
                        }
                    }
                    else
                    {
                        if (DataView[0]["CONTROL_TYPE"].ToString() == "D")
                        {
                            //Date
                            if (DataView[0]["DATABOUND"].ToString() == "Y")
                            {
                                if (myTextBox.Text.Trim() != "")
                                {
                                    if (AppDomain.CurrentDomain.GetData("DateFormat").ToString() == "dd-MM-yyyy")
                                    {
                                        string strDate = myTextBox.Text.Trim().Substring(6) + "-" + myTextBox.Text.Trim().Substring(3, 2) + "-" + myTextBox.Text.Trim().Substring(0, 2);

                                        pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = strDate;
                                    }
                                    else
                                    {
                                        pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = myTextBox.Text.Trim();
                                    }
                                }
                                else
                                {
                                    pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = System.DBNull.Value;

                                    blnPaintInd = true;
                                }
                            }
                            else
                            {
                                if (myTextBox.Text.Trim() == "")
                                {
                                    blnPaintInd = true;
                                }
                            }
                        }
                        else
                        {
                            //Data Bound
                            if (DataView[0]["NUMBER_DECIMALS"].ToString() != "")
                            {
                                //Numeric
                                if (DataView[0]["DATABOUND"].ToString() == "Y")
                                {
                                    if (myTextBox.Text.Trim().Replace(".", "") == "")
                                    {
                                        if (pvtFormDataView.Table.Columns[DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()].DataType.ToString() == "System.String")
                                        {
                                            pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = myTextBox.Text.Trim();
                                        }
                                        else
                                        {
                                            if (DataView[0]["NUMERIC_FIELD_NULLABLE"].ToString() == "Y")
                                            {
                                                pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = System.DBNull.Value;
                                            }
                                            else
                                            {
                                                pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //20170418 - Loses Leading Zeroes
                                        if (pvtFormDataView.Table.Columns[DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()].DataType.ToString() == "System.Decimal")
                                        {
                                            pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = Convert.ToDecimal(myTextBox.Text.Trim());
                                        }
                                        else
                                        {
                                            pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = myTextBox.Text.Trim();
                                        }
                                    }
                                }

                                intReturnCode = DataBound_Numeric_TextBox_Check(myTextBox, DataView[0], false);

                                if (intReturnCode == 1)
                                {
                                    blnPaintInd = true;
                                }
                            }
                            else
                            {
                                if (DataView[0]["CONTROL_TYPE"].ToString() == "T")
                                {
                                    //TextBox
                                    if (DataView[0]["CONTROL_IS_DATAVIEW_SORT_COLUMN_IND"].ToString() == "Y")
                                    {
                                        string strKey = pvtFormDataView[DataViewIndex][pvtstrDataViewTableUniqueKeyName].ToString();

                                        pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = myTextBox.Text.Trim();

                                        for (int intRow = 0; intRow < pvtFormDataView.Count; intRow++)
                                        {
                                            if (strKey == pvtFormDataView[intRow][pvtstrDataViewTableUniqueKeyName].ToString())
                                            {
                                                if (DataViewIndex != intRow)
                                                {
                                                    pubintReloadSpreadsheet = true;

                                                    DataViewIndex = intRow;
                                                }

                                                break;
                                            }
                                        }
                                    }

                                    if (DataView[0]["DATABOUND"].ToString() == "Y")
                                    {
                                        pvtFormDataView[DataViewIndex][DataView[0]["DATAVIEW_VALUEMEMBER"].ToString()] = myTextBox.Text.Trim();
                                    }

                                    intReturnCode = DataBound_TextBox_Check(myTextBox, DataView[0], false);

                                    if (intReturnCode == 1)
                                    {
                                        blnPaintInd = true;
                                    }
                                }
                                else
                                {

                                }
                            }
                        }
                    }

                    Paint_Parent_Marker(myTextBox, blnPaintInd);
                }
            }
        }

        public void Update_Paint_Parent_Marker(Control parControl)
        {
            TextBox myTextBox = (TextBox) parControl;

            bool blnPaint = false;

            if (myTextBox.Enabled == true)
            {
                DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + myTextBox.Name + "'", "", DataViewRowState.CurrentRows);

                if (DataView[0]["CONTROL_TYPE"].ToString() == "T")
                {
                    if (DataView[0]["NUMBER_DECIMALS"].ToString() != "")
                    {
                        if (myTextBox.Text.Trim().Replace(".", "") == "")
                        {
                            blnPaint = true;
                        }
                        else
                        {
                            if (Convert.ToDecimal(myTextBox.Text.Trim()) == 0)
                            {
                                blnPaint = true;
                            }
                        }
                    }
                    else
                    {
                        if (myTextBox.Text.Trim() == "")
                        {
                            blnPaint = true;
                        }
                    }
                }
            }

            Paint_Parent_Marker(parControl, blnPaint);
        }

        public void Paint_Parent_Marker(Control parControl, bool parPaintControl)
        {
#if(DEBUG)
            string strName = parControl.Name;

            if (strName == "txtPostAddrCode")
            {
                string strStop = "";
            }
#endif
            if (parControl is Form)
            {
            }
            else
            {
                int intXOffset = 4;
                int intYOffset = 5;

                Graphics = Graphics.FromHwnd(parControl.Parent.Handle);

                if (parControl is GroupBox)
                {
                    intXOffset = 5;
                    intYOffset = 0;
                }
                else
                {
                    if (parControl is TextBox)
                    {
                        //Link To DateTimePicker
                        for (int intRow = 0; intRow < strTextBoxName.Length; intRow++)
                        {
                            if (strTextBoxName[intRow] == null)
                            {
                                break;
                            }
                            else
                            {
                                if (strTextBoxName[intRow] == parControl.Name)
                                {
                                    if (intRow == 0)
                                    {
                                        parControl = dtpCalender1;
                                    }
                                    else
                                    {
                                        if (intRow == 1)
                                        {
                                            parControl = dtpCalender2;

                                        }
                                        else
                                        {
                                            if (intRow == 2)
                                            {
                                                parControl = dtpCalender3;

                                            }
                                            else
                                            {
                                                if (intRow == 3)
                                                {
                                                    parControl = dtpCalender3;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (parPaintControl == true
                    & pvtblnFormInEditMode == true)
                {
                    Graphics.FillEllipse(System.Drawing.Brushes.Red, parControl.Left - intXOffset, parControl.Top - intYOffset, 5, 5);
#if(DEBUG)
                    if (strName == "txtPhysicalCode")
                    {
                        string strStop = "";
                    }
#endif
                }
                else
                {
                    System.Drawing.Brush myBrush = new SolidBrush(parControl.Parent.BackColor);

                    Graphics.FillEllipse(myBrush, parControl.Left - intXOffset, parControl.Top - intYOffset, 5, 5);

                    myBrush.Dispose();
                }
            }
        }

        public void DataBind_DataView_To_Numeric_TextBox(TextBox parTextBox, string ValueMember, int intNumberDecimals, bool blnRequired, string RequiredMessage, bool blnEnableInEditMode,double MaxNumber,bool blnFieldNullable)
        {
            if (pvtFormDataView == null)
            {
                MessageBox.Show("Form DataView has Not been set");
                return;
            }

#if(DEBUG)
            if (parTextBox.Name == "txtTaxRefNo")
            {
                string strStop = "";
            }
#endif

            DataRow DataRow = pvtDataSet.Tables["Control"].NewRow();

            DataRow["CONTROL_NAME"] = parTextBox.Name;
            DataRow["CONTROL_HANDLE"] = parTextBox.Handle;

            TabPage myControlTabPage = Create_Control_TabIndex_For_DataRow(DataRow, parTextBox);

            if (myControlTabPage != null)
            {
                //Used to Gat Correct Tab Page on Save_Check
                TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                DataRow["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                DataRow["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
            }

            DataRow["CONTROL_TYPE"] = "T";
            DataRow["DATABOUND"] = "Y";
            DataRow["DATAVIEW_VALUEMEMBER"] = ValueMember;
            DataRow["NUMBER_DECIMALS"] = intNumberDecimals.ToString();
            DataRow["MAX_NUMBER"] = MaxNumber;
                 
            if (blnRequired == true)
            {
                DataRow["REQUIRED_IND"] = "Y";
            }
            else
            {
                DataRow["REQUIRED_IND"] = "N";

                if (parTextBox.Name == "txtTaxRefNo")
                {
                    DataRow["REQUIRED_MESSAGE"] = "Enter Tax Reference No. (If None Enter 0000000000)";
                }
                else
                {
                    if (parTextBox.Name == "txtIDNo")
                    {
                        DataRow["REQUIRED_MESSAGE"] = "Enter Valid Employee ID. Number.";
                    }
                }
            }

            if (blnFieldNullable == true)
            {
                DataRow["NUMERIC_FIELD_NULLABLE"] = "Y";
            }
            else
            {
                DataRow["NUMERIC_FIELD_NULLABLE"] = "N";
            }

            if (blnEnableInEditMode == true)
            {
                DataRow["ENABLE_IN_EDIT_MODE"] = "Y";
            }
            else
            {
                DataRow["ENABLE_IN_EDIT_MODE"] = "N";
            }

            DataRow["CONTROL_PARENT_NAME"] = parTextBox.Parent.Name;

            if (blnRequired == true)
            {
                if (RequiredMessage.Trim() == "")
                {
                    MessageBox.Show("TextBox " + parTextBox.Name + "Required Ind and Message Error");
                }
                else
                {
                    if (blnEnableInEditMode == false)
                    {
                        MessageBox.Show("TextBox " + parTextBox.Name + "Required=true and EnableInEditMode=false");
                    }
                    else
                    {
                        DataRow["REQUIRED_MESSAGE"] = RequiredMessage;
                    }
                }
            }
            else
            {
                if (RequiredMessage.Trim() != "")
                {
                    MessageBox.Show("TextBox " + parTextBox.Name + "Required Ind and Message Error");
                }
            }

            //Add Row
            pvtDataSet.Tables["Control"].Rows.Add(DataRow);
                
            //Events
            parTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(DataBind_Numeric_KeyPress);
            parTextBox.TextChanged += new System.EventHandler(TextBox_TextChanged);

            parTextBox.Enabled = false;
        }

        public void DataBind_DataView_To_Numeric_TextBox_Min_Length(TextBox parTextBox, string ValueMember, int intNumberDecimals,int intMinLength, bool blnRequired, string RequiredMessage, bool blnEnableInEditMode, double MaxNumber, bool blnFieldNullable)
        {
            if (pvtFormDataView == null)
            {
                MessageBox.Show("Form DataView has Not been set");
                return;
            }

#if(DEBUG)
            if (parTextBox.Name == "txtTelHome")
            {
                string strStop = "";
            }
#endif

            DataRow DataRow = pvtDataSet.Tables["Control"].NewRow();

            DataRow["CONTROL_NAME"] = parTextBox.Name;
            DataRow["CONTROL_HANDLE"] = parTextBox.Handle;

            TabPage myControlTabPage = Create_Control_TabIndex_For_DataRow(DataRow, parTextBox);

            if (myControlTabPage != null)
            {
                //Used to Gat Correct Tab Page on Save_Check
                TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                DataRow["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                DataRow["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
            }

            DataRow["CONTROL_TYPE"] = "T";
            DataRow["DATABOUND"] = "Y";
            DataRow["DATAVIEW_VALUEMEMBER"] = ValueMember;
            DataRow["NUMBER_DECIMALS"] = intNumberDecimals.ToString();
            DataRow["MAX_NUMBER"] = MaxNumber;
            DataRow["MIN_LENGTH"] = intMinLength;

            if (blnRequired == true)
            {
                DataRow["REQUIRED_IND"] = "Y";
            }
            else
            {
                DataRow["REQUIRED_IND"] = "N";
            }

            if (blnFieldNullable == true)
            {
                DataRow["NUMERIC_FIELD_NULLABLE"] = "Y";
            }
            else
            {
                DataRow["NUMERIC_FIELD_NULLABLE"] = "N";
            }

            if (blnEnableInEditMode == true)
            {
                DataRow["ENABLE_IN_EDIT_MODE"] = "Y";
            }
            else
            {
                DataRow["ENABLE_IN_EDIT_MODE"] = "N";
            }

            DataRow["CONTROL_PARENT_NAME"] = parTextBox.Parent.Name;

            if (blnRequired == true)
            {
                if (RequiredMessage.Trim() == "")
                {
                    MessageBox.Show("TextBox " + parTextBox.Name + "Required Ind and Message Error");
                }
                else
                {
                    if (blnEnableInEditMode == false)
                    {
                        MessageBox.Show("TextBox " + parTextBox.Name + "Required=true and EnableInEditMode=false");
                    }
                    else
                    {
                        DataRow["REQUIRED_MESSAGE"] = RequiredMessage;
                    }
                }
            }
            else
            {
                if (RequiredMessage.Trim() != "")
                {
                    if (intMinLength == 0)
                    {
                        MessageBox.Show("TextBox " + parTextBox.Name + "Required Ind and Message Error");
                    }
                    else
                    {
                        DataRow["REQUIRED_MESSAGE"] = RequiredMessage;
                    }
                }
            }

            //Add Row
            pvtDataSet.Tables["Control"].Rows.Add(DataRow);

            //Events
            parTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(DataBind_Numeric_KeyPress);
            parTextBox.TextChanged += new System.EventHandler(TextBox_TextChanged);

            parTextBox.Enabled = false;
        }

        public void DataBind_DataView_To_Date_TextBox(TextBox parTextBox, string ValueMember, bool blnRequired, string RequiredMessage)
        {
            if (pvtFormDataView == null)
            {
                MessageBox.Show("Form DataView has Not been set");
                return;
            }

            Create_Calender_Control_From_TextBox(parTextBox);

            DataRow DataRow = pvtDataSet.Tables["Control"].NewRow();

            DataRow["CONTROL_NAME"] = parTextBox.Name;
            DataRow["CONTROL_HANDLE"] = parTextBox.Handle;

            TabPage myControlTabPage = Create_Control_TabIndex_For_DataRow(DataRow, parTextBox);

            if (myControlTabPage != null)
            {
                //Used to Gat Correct Tab Page on Save_Check
                TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                DataRow["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                DataRow["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
            }

            DataRow["CONTROL_TYPE"] = "D";
            DataRow["DATABOUND"] = "Y";
            DataRow["DATAVIEW_VALUEMEMBER"] = ValueMember;
            
            if (blnRequired == true)
            {
                DataRow["REQUIRED_IND"] = "Y";
            }
            else
            {
                DataRow["REQUIRED_IND"] = "N";
            }

            DataRow["ENABLE_IN_EDIT_MODE"] = "N";
            DataRow["CONTROL_PARENT_NAME"] = parTextBox.Parent.Name;

            if (blnRequired == true)
            {
                if (RequiredMessage.Trim() == "")
                {
                    MessageBox.Show("TextBox " + parTextBox.Name + "Required Ind and Message Error");
                }
                else
                {
                   DataRow["REQUIRED_MESSAGE"] = RequiredMessage;
                }
            }
            else
            {
                if (RequiredMessage.Trim() != "")
                {
                    MessageBox.Show("TextBox " + parTextBox.Name + "Required Ind and Message Error");
                }
            }
           
            //Add Row
            pvtDataSet.Tables["Control"].Rows.Add(DataRow);
                
            parTextBox.TextChanged += new System.EventHandler(TextBox_TextChanged);

            parTextBox.Enabled = false;
        }

        public void DataBind_DataView_To_Date_TextBox_ReadOnly(TextBox parTextBox, string ValueMember)
        {
            if (pvtFormDataView == null)
            {
                MessageBox.Show("Form DataView has Not been set");
                return;
            }

            DataRow DataRow = pvtDataSet.Tables["Control"].NewRow();

            DataRow["CONTROL_NAME"] = parTextBox.Name;
            DataRow["CONTROL_HANDLE"] = parTextBox.Handle;

            TabPage myControlTabPage = Create_Control_TabIndex_For_DataRow(DataRow, parTextBox);

            if (myControlTabPage != null)
            {
                //Used to Gat Correct Tab Page on Save_Check
                TabControl pvtTabControl = (TabControl)myControlTabPage.Parent;

                DataRow["TAB_CONTROL_HANDLE"] = pvtTabControl.Handle;
                DataRow["TABPAGE_INDEX"] = pvtTabControl.TabPages.IndexOf(myControlTabPage);
            }

            DataRow["CONTROL_TYPE"] = "D";
            DataRow["DATABOUND"] = "Y";
            DataRow["DATAVIEW_VALUEMEMBER"] = ValueMember;

            DataRow["REQUIRED_IND"] = "N";
            
            DataRow["ENABLE_IN_EDIT_MODE"] = "N";
            DataRow["CONTROL_PARENT_NAME"] = parTextBox.Parent.Name;

            //Add Row
            pvtDataSet.Tables["Control"].Rows.Add(DataRow);

            //parTextBox.TextChanged += new System.EventHandler(TextBox_TextChanged);

            parTextBox.Enabled = false;
        }

        private void DataBind_Numeric_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            TextBox myTextBox = (TextBox)sender;
         
            DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "CONTROL_NAME = '" + myTextBox.Name + "'", "", DataViewRowState.CurrentRows);

            int intNumberDecimals = Convert.ToInt32(DataView[0]["NUMBER_DECIMALS"]);
            decimal dblgMaxNumber = Convert.ToDecimal(DataView[0]["MAX_NUMBER"]);
           
            if (e.KeyChar == (char)8
                | e.KeyChar == (char)46
                | (e.KeyChar > (char)47
                & e.KeyChar < (char)58))
            {
                if (intNumberDecimals > 0)
                {
                    if (e.KeyChar == 46
                        & myTextBox.Text.IndexOf(".") > -1)
                    {
                        e.Handled = true;
                        System.Console.Beep();
                        return;
                    }
                    else
                    {
                        string strNewTextField = "";

                        //Build TextBox Field
                        if (myTextBox.Text != "")
                        {
                            if (myTextBox.Text.Length == myTextBox.SelectionStart)
                            {
                                if (e.KeyChar == 8)
                                {
                                    strNewTextField = myTextBox.Text.Substring(0, myTextBox.Text.Length - 1);
                                }
                                else
                                {
                                    strNewTextField = myTextBox.Text + e.KeyChar;
                                }
                            }
                            else
                            {
                                if (e.KeyChar == 8)
                                {
                                    if (myTextBox.SelectionStart == 0)
                                    {
                                        //Nothing Changed
                                        strNewTextField = myTextBox.Text;
                                    }
                                    else
                                    {
                                        strNewTextField = myTextBox.Text.Substring(0, myTextBox.SelectionStart - 1) + myTextBox.Text.Substring(myTextBox.SelectionStart);
                                    }
                                }
                                else
                                {
                                    strNewTextField = myTextBox.Text.Substring(0, myTextBox.SelectionStart) + e.KeyChar + myTextBox.Text.Substring(myTextBox.SelectionStart);
                                }
                            }
                        }
                        else
                        {
                            strNewTextField = e.KeyChar.ToString();
                        }

                        string[] strParts = strNewTextField.Split('.');

                        if (strParts.Length > 1)
                        {
                            if (strParts[strParts.Length -1].Length > intNumberDecimals)
                            {
                                //Errol Added 2012-08-06
                                if (myTextBox.SelectionLength == 0)
                                {
                                    e.Handled = true;
                                    System.Console.Beep();
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (e.KeyChar == 46) 
                    {
                        e.Handled = true;
                        System.Console.Beep();
                        return;
                    }
                }

                if (dblgMaxNumber != 0)
                {
                    string strNewNumericTextField = "";

                    if (myTextBox.Text != "")
                    {
                        if (myTextBox.SelectionLength == myTextBox.Text.Length)
                        {
                            //Replace All Characters with 1 New Numeric Character 
                            strNewNumericTextField = e.KeyChar.ToString();
                        }
                        else
                        {
                            if (myTextBox.SelectionLength > 0)
                            {
                                //Replace 
                                strNewNumericTextField = myTextBox.Text.Remove(myTextBox.SelectionStart, myTextBox.SelectionLength);

                                if (myTextBox.SelectionStart == 0)
                                {
                                    strNewNumericTextField = e.KeyChar + strNewNumericTextField;
                                }
                                else
                                {

                                    if (myTextBox.SelectionStart == strNewNumericTextField.Length)
                                    {
                                        //Add to End
                                        strNewNumericTextField = strNewNumericTextField + e.KeyChar;
                                    }
                                    else
                                    {
                                        //Middle
                                        strNewNumericTextField = strNewNumericTextField.Substring(0, myTextBox.SelectionStart) + e.KeyChar + strNewNumericTextField.Substring(myTextBox.SelectionStart);
                                    }
                                }
                            }
                            else
                            {
                                if (myTextBox.Text.Length == myTextBox.SelectionStart)
                                {
                                    if (e.KeyChar == 8)
                                    {
                                        strNewNumericTextField = myTextBox.Text.Substring(0, myTextBox.Text.Length - 1);
                                    }
                                    else
                                    {
                                        strNewNumericTextField = myTextBox.Text + e.KeyChar;
                                    }
                                }
                                else
                                {
                                    if (e.KeyChar == 8)
                                    {
                                        if (myTextBox.SelectionStart == 0)
                                        {
                                            //Nothing Changed
                                            strNewNumericTextField = myTextBox.Text;
                                        }
                                        else
                                        {

                                            strNewNumericTextField = myTextBox.Text.Substring(0, myTextBox.SelectionStart - 1) + myTextBox.Text.Substring(myTextBox.SelectionStart);
                                        }
                                    }
                                    else
                                    {
                                        strNewNumericTextField = myTextBox.Text.Substring(0, myTextBox.SelectionStart) + e.KeyChar + myTextBox.Text.Substring(myTextBox.SelectionStart);
                                    }
                                }
                            }
                        }
                    }

                    if (strNewNumericTextField.Replace(".", "") != "")
                    {
                        if (dblgMaxNumber < Convert.ToDecimal(strNewNumericTextField))
                        {
                            e.Handled = true;
                            System.Console.Beep();
                            return;
                        }
                    }
                }
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    //Enter
                    string strFormat = "########0";
                    string strLowerFormat = new string('0',intNumberDecimals);

                    if (intNumberDecimals > 0)
                    {
                        strFormat += "." + strLowerFormat;
                    }
                    
                    if (myTextBox.Text.Replace(".", "").Trim() == "")
                    {
                        myTextBox.Text = Convert.ToDecimal(0).ToString(strFormat);

                    }
                    else
                    {
                        myTextBox.Text = Convert.ToDecimal(myTextBox.Text).ToString(strFormat);
                    }
                }
                else
                {
                    e.Handled = true;
                    System.Console.Beep();
                }
            }
        }

        private void DataBind_Initialise_Numeric_Fields()
        {
            if (pvtFormDataView != null)
            {
                //NB DataView is passed by Reference
                for (int intRow = 0; intRow < pvtFormDataView.Table.Columns.Count; intRow++)
                {
                    if (pvtFormDataView.Table.Columns[intRow].DataType.ToString() == "System.Int16"
                        | pvtFormDataView.Table.Columns[intRow].DataType.ToString() == "System.Int32"
                        | pvtFormDataView.Table.Columns[intRow].DataType.ToString() == "System.Int64"
                        | pvtFormDataView.Table.Columns[intRow].DataType.ToString() == "System.Double"
                        | pvtFormDataView.Table.Columns[intRow].DataType.ToString() == "System.Decimal"
                        | pvtFormDataView.Table.Columns[intRow].DataType.ToString() == "System.SByte"
                        | pvtFormDataView.Table.Columns[intRow].DataType.ToString() == "System.Byte"
                        | pvtFormDataView.Table.Columns[intRow].DataType.ToString() == "System.UInt16"
                        | pvtFormDataView.Table.Columns[intRow].DataType.ToString() == "System.UInt32"
                        | pvtFormDataView.Table.Columns[intRow].DataType.ToString() == "System.UInt64")
                    {
                        if (pvtFormDataView.Table.Columns[intRow].ColumnName == "COMPANY_NO"
                        | pvtFormDataView.Table.Columns[intRow].ColumnName == pvtstrDataViewTableUniqueKeyName)
                        {
                            //Key Would have been Set
                            continue;
                        }

                        DataView myDataView = new DataView(pvtDataSet.Tables["Control"], "DATAVIEW_VALUEMEMBER = '" + pvtFormDataView.Table.Columns[intRow].ColumnName + "'", "", DataViewRowState.CurrentRows);

                        if (myDataView.Count > 0)
                        {
                            if (myDataView[0]["NUMERIC_FIELD_NULLABLE"].ToString() == "Y")
                            {
                                pvtFormDataView[DataViewIndex][pvtFormDataView.Table.Columns[intRow].ColumnName] = System.DBNull.Value;
                            }
                            else
                            {
                                pvtFormDataView[DataViewIndex][pvtFormDataView.Table.Columns[intRow].ColumnName] = 0;
                            }
                        }
                        else
                        {
                            pvtFormDataView[DataViewIndex][pvtFormDataView.Table.Columns[intRow].ColumnName] = 0;
                        }
                    }
                }
            }
        }

        public bool DataBind_Form_And_DataView_To_Class()
        {
            bool blnBound = false;

            if (pvtFormDataView != null)
            {
                blnBound = true;
            }

            return blnBound;
        }

        public void Re_DataBind_DataView(DataView parDataView)
        {
            DataView myDataView = new DataView(pvtDataSet.Tables["Control"], "NOT DATAVIEW_VALUEMEMBER IS NULL", "", DataViewRowState.CurrentRows);

            if (myDataView.Count > 0)
            {
                pvtFormDataView = null;
                pvtFormDataView = parDataView;
            }
            else
            {
                MessageBox.Show("ReBind Error");
                return;
            }
        }

        public void DataBind_DataView_And_Index(Form myForm,DataView parDataView,string DataViewKeyFieldName)
        {
            DataView myDataView = new DataView(pvtDataSet.Tables["Control"], "NOT DATAVIEW_VALUEMEMBER IS NULL", "", DataViewRowState.CurrentRows);

            if (myDataView.Count > 0)
            {
                MessageBox.Show("This Feature has Not been Included - Unhooking of Events need to Be Programmed");
                return;
            }

            if (parDataView.Sort != "")
            {
                pvtFormDataView = parDataView;

                pvtstrDataViewTableUniqueKeyName = DataViewKeyFieldName;
            }
            else
            {
                MessageBox.Show("DataView needs to have a Sort Field Set"); 
            }
        }

        public void DataBind_DataView_Record_Show()
        {

#if(DEBUG)
            if (pvtblnFormInEditMode == true)
            {
                MessageBox.Show("DataBind_DataView_Record_Show - Still In Edit Mode\nAction Canceled.");
                return;
            }
#endif
            //pvtDataSet.Tables["Control"].AcceptChanges();


            //Load Control Fields
            for (int intRow = 0; intRow < pvtDataSet.Tables["Control"].Rows.Count; intRow++)
            {
                if (pvtDataSet.Tables["Control"].Rows[intRow]["DATABOUND"].ToString() == "Y")
                {
#if(DEBUG)
                    if (intRow == 50)
                    {
                        string strStop = "";

                        
                    }


                    string strControlName = pvtDataSet.Tables["Control"].Rows[intRow]["CONTROL_NAME"].ToString(); 
                    string strValueMember = pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString();

                    if (strControlName == "txtPostCity")
                    {
                        string stop = "";
                    }

                    string strValue = "";

                    try
                    {
                        strValue = pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].ToString();
                    }
                    catch
                    {
                        string A = "";
                    }

                    if (strValueMember == "")
                    {
                        MessageBox.Show("Control '" + strControlName + "' VALUEMEMBER HAS NOT Been Initialised\nAction Cancelled.");
                        return;
                    }
#endif
                    //TextBox (Normal or Numeric) and Date
                    if (pvtDataSet.Tables["Control"].Rows[intRow]["CONTROL_TYPE"].ToString() == "T"
                        | pvtDataSet.Tables["Control"].Rows[intRow]["CONTROL_TYPE"].ToString() == "D")
                    {
                        TextBox myTextBox = (TextBox)Control.FromHandle((IntPtr)pvtDataSet.Tables["Control"].Rows[intRow]["CONTROL_HANDLE"]);

                        if (pvtDataSet.Tables["Control"].Rows[intRow]["NUMBER_DECIMALS"].ToString() == ""
                            & pvtDataSet.Tables["Control"].Rows[intRow]["CONTROL_TYPE"].ToString() == "T")
                        {
                            //TextBox
                            myTextBox.Text = pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].ToString();
                        }
                        else
                        {
                            if (pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()] == System.DBNull.Value
                                | pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].ToString() == "")
                            {
                                myTextBox.Text = "";
                            }
                            else
                            {
                                if (pvtDataSet.Tables["Control"].Rows[intRow]["CONTROL_TYPE"].ToString() == "T")
                                {
                                    //Numeric
                                    if (pvtFormDataView.Table.Columns[pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].DataType.ToString() == "System.String")
                                    {
                                        //Numeric Field Held in String
                                        if (pvtDataSet.Tables["Control"].Rows[intRow]["NUMERIC_FIELD_FORMAT"].ToString() == "")
                                        {
                                            myTextBox.Text = pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].ToString();
                                        }
                                        else
                                        {
                                            decimal dblResult = 0;

                                            if (Decimal.TryParse(pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].ToString(), out dblResult) == false)
                                            {
                                                //Not Number
                                                myTextBox.Text = pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].ToString();
                                            }
                                            else
                                            {
                                                if (pvtDataSet.Tables["Control"].Rows[intRow]["NUMERIC_FIELD_FORMAT"].ToString().Replace("-","").Length == pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].ToString().Length)
                                                {
                                                    //Format
                                                    myTextBox.Text = Convert.ToDecimal(pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()]).ToString(pvtDataSet.Tables["Control"].Rows[intRow]["NUMERIC_FIELD_FORMAT"].ToString());
                                                }
                                                else
                                                {
                                                    myTextBox.Text = pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].ToString();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string strDecimals = new string('0', Convert.ToInt32(pvtDataSet.Tables["Control"].Rows[intRow]["NUMBER_DECIMALS"]));
                                        string strFormat = "#############0";

                                        if (strDecimals != "")
                                        {
                                            strFormat += "." + strDecimals;
                                        }
                                        
                                        myTextBox.Text = Convert.ToDecimal(pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()]).ToString(strFormat);
                                    }
                                }
                                else
                                {
                                    //Date
                                    myTextBox.Text = Convert.ToDateTime(pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()]).ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());
                                }
                            }
                        }

                        //2013-09-21
                        myTextBox.Refresh();
                    }
                    else
                    {
                        if (pvtDataSet.Tables["Control"].Rows[intRow]["CONTROL_TYPE"].ToString() == "C")
                        {
                            int intSelectedIndex = -1;
                            ComboBox myComboBox = (ComboBox)Control.FromHandle((IntPtr)pvtDataSet.Tables["Control"].Rows[intRow]["CONTROL_HANDLE"]);
#if(DEBUG)
                            string strComboName = myComboBox.Name;

                            if (pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"] == System.DBNull.Value)
                            {
                                if (pvtDataSet.Tables["Control"].Rows[intRow]["DATABOUND"].ToString() == "Y")
                                {
                                    MessageBox.Show("There is No DataView Field Link to the ComboBox '" + strComboName + "'");
                                    return;
                                }
                            }
#endif
                            if (pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()] != System.DBNull.Value)
                            {
                                for (int intComboRow = 0; intComboRow < myComboBox.Items.Count; intComboRow++)
                                {
                                    if (pvtFormDataSet.Tables[pvtDataSet.Tables["Control"].Rows[intRow]["TABLENAME"].ToString()].Rows[intComboRow][pvtDataSet.Tables["Control"].Rows[intRow]["VALUEMEMBER"].ToString()].ToString() == pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].ToString())
                                    {
                                        intSelectedIndex = intComboRow;

                                        break;
                                    }
                                }
                            }

                            myComboBox.SelectedIndex = intSelectedIndex;
                            //2013-09-21
                            myComboBox.Refresh();
                        }
                        else
                        {
                            if (pvtDataSet.Tables["Control"].Rows[intRow]["CONTROL_TYPE"].ToString() == "R")
                            {
                                if (pvtDataSet.Tables["Control"].Rows[intRow]["DB_VALUE"].ToString() == pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].ToString())
                                {
                                    RadioButton myRadioButton = (RadioButton)Control.FromHandle((IntPtr)pvtDataSet.Tables["Control"].Rows[intRow]["CONTROL_HANDLE"]);
                                    myRadioButton.Checked = true;
                                    //2013-09-21
                                    myRadioButton.Refresh();
                                }
                                else
                                {
                                    //Check if it will 
                                    if (pvtDataSet.Tables["Control"].Rows[intRow]["FIRST_MEMBER_IND"].ToString() == "Y")
                                    {
                                        DataView DataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "DB_VALUE = '" + pvtFormDataView[DataViewIndex][pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString()].ToString() + "'", "", DataViewRowState.CurrentRows);

                                        if (DataView.Count == 0)
                                        {
                                            DataView RadioButtonsDataView = new System.Data.DataView(pvtDataSet.Tables["Control"], "DATAVIEW_VALUEMEMBER = '" + pvtDataSet.Tables["Control"].Rows[intRow]["DATAVIEW_VALUEMEMBER"].ToString() + "'", "", DataViewRowState.CurrentRows);

                                            for (int intRadioButtonCount = 0; intRadioButtonCount < RadioButtonsDataView.Count; intRadioButtonCount++)
                                            {
                                                RadioButton myRadioButton = (RadioButton)Control.FromHandle((IntPtr)RadioButtonsDataView[intRadioButtonCount]["CONTROL_HANDLE"]);

                                                myRadioButton.Checked = false;
                                                //2013-09-21
                                                myRadioButton.Refresh();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Calender_Clear_Click(object sender, EventArgs e)
        {
            Button myDateClearButton = (Button)sender;

            TextBox myTextBox;

            blnClearCalenderClicked = true;

            if (myDateClearButton.Name == "btnDate1")
            {
                dtpCalender1.Value = DateTime.Now;

                myTextBox = (TextBox)Control.FromHandle((IntPtr)IntPtrDateTextBox[0]);
                myTextBox.Text = "";
            }
            else
            {
                if (myDateClearButton.Name == "btnDate2")
                {
                    dtpCalender2.Value = DateTime.Now;
                   
                    myTextBox = (TextBox)Control.FromHandle((IntPtr)IntPtrDateTextBox[1]);
                    myTextBox.Text = "";
                }
                else
                {
                    if (myDateClearButton.Name == "btnDate3")
                    {
                        dtpCalender3.Value = DateTime.Now;
                       
                        myTextBox = (TextBox)Control.FromHandle((IntPtr)IntPtrDateTextBox[2]);
                        myTextBox.Text = "";
                    }
                    else
                    {
                        if (myDateClearButton.Name == "btnDate4")
                        {
                            dtpCalender4.Value = DateTime.Now;
                        
                            myTextBox = (TextBox)Control.FromHandle((IntPtr)IntPtrDateTextBox[3]);
                            myTextBox.Text = "";
                        }
                    }
                }
            }

            blnClearCalenderClicked = false;
        }

        private void Calender_CloseUp(object sender, EventArgs e)
        {
            DateTimePicker myDateTimePicker = (DateTimePicker)sender;
            TextBox myTextBox;

            if (myDateTimePicker.Name == "dtpCalender1")
            {
                myTextBox = (TextBox)Control.FromHandle((IntPtr)IntPtrDateTextBox[0]);
                myTextBox.Text = myDateTimePicker.Value.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());
            }
            else
            {
                if (myDateTimePicker.Name == "dtpCalender2")
                {
                    myTextBox = (TextBox)Control.FromHandle((IntPtr)IntPtrDateTextBox[1]);
                    myTextBox.Text = myDateTimePicker.Value.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());
                }
                else
                {
                    if (myDateTimePicker.Name == "dtpCalender3")
                    {
                        myTextBox = (TextBox)Control.FromHandle((IntPtr)IntPtrDateTextBox[2]);
                        myTextBox.Text = myDateTimePicker.Value.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());
                    }
                    else
                    {
                        if (myDateTimePicker.Name == "dtpCalender4")
                        {
                            myTextBox = (TextBox)Control.FromHandle((IntPtr)IntPtrDateTextBox[3]);
                            myTextBox.Text = myDateTimePicker.Value.ToString(AppDomain.CurrentDomain.GetData("DateFormat").ToString());
                        }
                    }
                }
            }
        }

        private void Calender_TextChanged(object sender, EventArgs e)
        {
            if (blnClearCalenderClicked == false)
            {
                TextBox myTextBox = (TextBox)sender;
                DateTime dtDateTime = DateTime.Now;

                if (myTextBox.Text != "")
                {
                    int intYear = 0;
                    int intMonth = 0;
                    int intDay = 0;

                    //MessageBox.Show("Date Value = '" + myTextBox.Text + "'"); 

                    if (myTextBox.Text.IndexOf("-") == 4)
                    {
                        intYear = Convert.ToInt32(myTextBox.Text.Substring(0, 4));
                        intMonth = Convert.ToInt32(myTextBox.Text.Substring(5, 2));
                        intDay = Convert.ToInt32(myTextBox.Text.Substring(8, 2));
                    }
                    else
                    {
                        intDay = Convert.ToInt32(myTextBox.Text.Substring(0, 2));
                        intMonth = Convert.ToInt32(myTextBox.Text.Substring(3, 2));
                        intYear = Convert.ToInt32(myTextBox.Text.Substring(6, 4));
                    }

                    dtDateTime = new DateTime(intYear, intMonth, intDay);
                }

                for (int intRow = 0; intRow < strTextBoxName.Length; intRow++)
                {
                    if (strTextBoxName[intRow] == myTextBox.Name)
                    {
                        if (intRow == 0)
                        {
                            dtpCalender1.Value = dtDateTime;
                        }
                        else
                        {
                            if (intRow == 1)
                            {
                                dtpCalender2.Value = dtDateTime;
                            }
                            else
                            {
                                if (intRow == 2)
                                {
                                    dtpCalender3.Value = dtDateTime;
                                }
                                else
                                {
                                    if (intRow == 3)
                                    {
                                        dtpCalender4.Value = dtDateTime;
                                    }
                                    else
                                    {

                                        MessageBox.Show("Calender_TextChanged Error");
                                    }
                                }
                            }
                        }

                        break;
                    }
                }
            }
        }

        public void Calender_Control_From_TextBox_Enable(TextBox myTextBox)
        {
            for (int intRow = 0; intRow < strTextBoxName.Length; intRow++)
            {
                if (strTextBoxName[intRow] == myTextBox.Name)
                {
                    if (intRow == 0)
                    {
                        this.dtpCalender1.Enabled = true;
                        this.btnDate1.Enabled = true;
                    }
                    else
                    {
                        if (intRow == 1)
                        {
                            this.dtpCalender2.Enabled = true;
                            this.btnDate2.Enabled = true;
                        }
                        else
                        {
                            if (intRow == 2)
                            {
                                this.dtpCalender3.Enabled = true;
                                this.btnDate3.Enabled = true;
                            }
                            else
                            {
                                if (intRow == 3)
                                {
                                    this.dtpCalender4.Enabled = true;
                                    this.btnDate4.Enabled = true;
                                }
                                else
                                {
                                    MessageBox.Show("Calender_Control_From_TextBox_Enable Error");
                                }
                            }
                        }
                    }

                    break;
                }
            }
        }

        public void Calender_Control_From_TextBox_Disable(TextBox myTextBox)
        {
            for (int intRow = 0; intRow < strTextBoxName.Length; intRow++)
            {
                if (strTextBoxName[intRow] == myTextBox.Name)
                {
                    if (intRow == 0)
                    {
                        this.dtpCalender1.Enabled = false;
                        this.btnDate1.Enabled = false;
                    }
                    else
                    {
                        if (intRow == 1)
                        {
                            this.dtpCalender2.Enabled = false;
                            this.btnDate2.Enabled = false;
                        }
                        else
                        {
                            if (intRow == 2)
                            {
                                this.dtpCalender3.Enabled = false;
                                this.btnDate3.Enabled = false;
                            }
                            else
                            {
                                if (intRow == 3)
                                {
                                    this.dtpCalender4.Enabled = false;
                                    this.btnDate4.Enabled = false;
                                }
                                else
                                {
                                    MessageBox.Show("Calender_Control_From_TextBox_Disable Error");
                                }
                            }
                        }
                    }

                    break;
                }
            }
        }

        public void Calender_Control_From_TextBox_SetVisible(TextBox myTextBox)
        {
            for (int intRow = 0; intRow < strTextBoxName.Length; intRow++)
            {
                if (strTextBoxName[intRow] == myTextBox.Name)
                {
                    if (intRow == 0)
                    {
                        this.dtpCalender1.Visible = true;
                        this.btnDate1.Visible = true;
                    }
                    else
                    {
                        if (intRow == 1)
                        {
                            this.btnDate2.Visible = true;
                            this.dtpCalender2.Visible = true;
                        }
                        else
                        {
                            if (intRow == 2)
                            {
                                this.dtpCalender3.Visible = true;
                                this.btnDate3.Visible = true;
                            }
                            else
                            {
                                if (intRow == 3)
                                {
                                    this.dtpCalender4.Visible = true;
                                    this.btnDate4.Visible = true;
                                }
                                else
                                {
                                    MessageBox.Show("Calender_Control_From_TextBox_SetVisible Error");
                                }
                            }
                        }
                    }

                    myTextBox.Visible = true;
                    myTextBox.BringToFront();

                    break;
                }
            }
        }

        public void Calender_Control_From_TextBox_SetInvisible(TextBox myTextBox)
        {
            for (int intRow = 0; intRow < strTextBoxName.Length; intRow++)
            {
                if (strTextBoxName[intRow] == myTextBox.Name)
                {
                    if (intRow == 0)
                    {
                        this.dtpCalender1.Visible = false;
                        this.btnDate1.Visible = false;
                    }
                    else
                    {
                        if (intRow == 1)
                        {
                            this.dtpCalender2.Visible = false;
                            this.btnDate2.Visible = false;
                        }
                        else
                        {
                            if (intRow == 2)
                            {
                                this.dtpCalender3.Visible = false;
                                this.btnDate3.Visible = false;
                            }
                            else
                            {
                                if (intRow == 3)
                                {
                                    this.dtpCalender4.Visible = false;
                                    this.btnDate4.Visible = false;
                                }
                                else
                                {
                                    MessageBox.Show("Calender_Control_From_TextBox_SetInvisible Error");
                                }
                            }
                        }
                    }

                    myTextBox.Visible = false;

                    break;
                }
            }
        }

        public void Create_Calender_Control_From_TextBox(TextBox myTextBox)
        {
            if (intNumberCalendersCreated == 0)
            {
                strTextBoxName[0] = myTextBox.Name;

                //Errol 2012-08-31
                myTextBox.TextChanged -= new System.EventHandler(this.Calender_TextChanged);
                myTextBox.TextChanged += new System.EventHandler(this.Calender_TextChanged);

                dtpCalender1 = new DateTimePicker();
                dtpCalender1.Name = "dtpCalender1";

                dtpCalender1.Format = DateTimePickerFormat.Custom;
                dtpCalender1.Height = 20;
                dtpCalender1.Width = 107;

                dtpCalender1.Left = myTextBox.Left;
                dtpCalender1.Top = myTextBox.Top;
                dtpCalender1.Enabled = false;

                myTextBox.Parent.Controls.Add(this.dtpCalender1);

                btnDate1 = new Button();
                btnDate1.Name = "btnDate1";
                btnDate1.Height = 18;
                btnDate1.Width = 17;
                btnDate1.Image = (Image)Properties.Resources.Clear;

                btnDate1.BackColor = System.Drawing.Color.White;
                btnDate1.FlatStyle = FlatStyle.Flat;
                btnDate1.FlatAppearance.BorderSize = 0;

                btnDate1.Left = myTextBox.Left + 1;
                btnDate1.Top = myTextBox.Top + 1;
                btnDate1.TabStop = false;
                btnDate1.Enabled = false;

                myTextBox.Parent.Controls.Add(this.btnDate1);

                //Errol 2012-08-31
                this.btnDate1.Click -= new System.EventHandler(this.Calender_Clear_Click);
                this.btnDate1.Click += new System.EventHandler(this.Calender_Clear_Click);

                myTextBox.Left += 17;

                myTextBox.Top += 3;
                myTextBox.Left += 1;
                myTextBox.BorderStyle = BorderStyle.None;
                myTextBox.Width = 54;
                myTextBox.ReadOnly = true;

                //Errol 2012-08-31
                this.dtpCalender1.CloseUp -= new System.EventHandler(this.Calender_CloseUp);
                this.dtpCalender1.CloseUp += new System.EventHandler(this.Calender_CloseUp);

                IntPtrDateTextBox[0] = myTextBox.Handle;
                this.btnDate1.BringToFront();
            }
            else
            {
                if (intNumberCalendersCreated == 1)
                {
                    strTextBoxName[1] = myTextBox.Name;

                    //Errol 2012-08-31
                    myTextBox.TextChanged -= new System.EventHandler(this.Calender_TextChanged);
                    myTextBox.TextChanged += new System.EventHandler(this.Calender_TextChanged);

                    dtpCalender2 = new DateTimePicker();
                    dtpCalender2.Name = "dtpCalender2";

                    dtpCalender2.Format = DateTimePickerFormat.Custom;
                    dtpCalender2.Height = 20;
                    dtpCalender2.Width = 107;

                    dtpCalender2.Left = myTextBox.Left;
                    dtpCalender2.Top = myTextBox.Top;
                    dtpCalender2.Enabled = false;

                    myTextBox.Parent.Controls.Add(this.dtpCalender2);

                    btnDate2 = new Button();
                    btnDate2.Name = "btnDate2";
                    btnDate2.Height = 18;
                    btnDate2.Width = 17;
                    btnDate2.Image = (Image)Properties.Resources.Clear;

                    btnDate2.BackColor = System.Drawing.Color.White;
                    btnDate2.FlatStyle = FlatStyle.Flat;
                    btnDate2.FlatAppearance.BorderSize = 0;

                    btnDate2.Left = myTextBox.Left + 1;
                    btnDate2.Top = myTextBox.Top + 1;
                    btnDate2.TabStop = false;
                    btnDate2.Enabled = false;

                    myTextBox.Parent.Controls.Add(this.btnDate2);

                    //Errol 2012-08-31
                    this.btnDate2.Click -= new System.EventHandler(this.Calender_Clear_Click);
                    this.btnDate2.Click += new System.EventHandler(this.Calender_Clear_Click);

                    myTextBox.Left += 17;

                    myTextBox.Top += 3;
                    myTextBox.Left += 1;
                    myTextBox.BorderStyle = BorderStyle.None;
                    myTextBox.Width = 54;
                    myTextBox.ReadOnly = true;

                    this.dtpCalender2.CloseUp += new System.EventHandler(this.Calender_CloseUp);

                    IntPtrDateTextBox[1] = myTextBox.Handle;

                    this.btnDate2.BringToFront();
                }
                else
                {
                    if (intNumberCalendersCreated == 2)
                    {
                        strTextBoxName[2] = myTextBox.Name;

                        //Errol 2012-08-31
                        myTextBox.TextChanged -= new System.EventHandler(this.Calender_TextChanged);
                        myTextBox.TextChanged += new System.EventHandler(this.Calender_TextChanged);

                        dtpCalender3 = new DateTimePicker();
                        dtpCalender3.Name = "dtpCalender3";

                        dtpCalender3.Format = DateTimePickerFormat.Custom;
                        dtpCalender3.Height = 20;
                        dtpCalender3.Width = 107;

                        dtpCalender3.Left = myTextBox.Left;
                        dtpCalender3.Top = myTextBox.Top;
                        dtpCalender3.Enabled = false;

                        myTextBox.Parent.Controls.Add(this.dtpCalender3);

                        btnDate3 = new Button();
                        btnDate3.Name = "btnDate3";
                        btnDate3.Height = 18;
                        btnDate3.Width = 17;
                        btnDate3.Image = (Image)Properties.Resources.Clear;

                        btnDate3.BackColor = System.Drawing.Color.White;
                        btnDate3.FlatStyle = FlatStyle.Flat;
                        btnDate3.FlatAppearance.BorderSize = 0;

                        btnDate3.Left = myTextBox.Left + 1;
                        btnDate3.Top = myTextBox.Top + 1;
                        btnDate3.TabStop = false;
                        btnDate3.Enabled = false;

                        myTextBox.Parent.Controls.Add(this.btnDate3);

                        //Errol 2012-08-31
                        this.btnDate3.Click -= new System.EventHandler(this.Calender_Clear_Click);
                        this.btnDate3.Click += new System.EventHandler(this.Calender_Clear_Click);

                        myTextBox.Left += 17;

                        myTextBox.Top += 3;
                        myTextBox.Left += 1;
                        myTextBox.BorderStyle = BorderStyle.None;
                        myTextBox.Width = 54;
                        myTextBox.ReadOnly = true;

                        //Errol 2012-08-31
                        this.dtpCalender3.CloseUp -= new System.EventHandler(this.Calender_CloseUp);
                        this.dtpCalender3.CloseUp += new System.EventHandler(this.Calender_CloseUp);

                        IntPtrDateTextBox[2] = myTextBox.Handle;

                        this.btnDate3.BringToFront();
                    }
                    else
                    {
                        if (intNumberCalendersCreated == 3)
                        {
                            strTextBoxName[3] = myTextBox.Name;

                            //Errol 2012-08-31
                            myTextBox.TextChanged -= new System.EventHandler(this.Calender_TextChanged);
                            myTextBox.TextChanged += new System.EventHandler(this.Calender_TextChanged);

                            dtpCalender4 = new DateTimePicker();
                            dtpCalender4.Name = "dtpCalender4";

                            dtpCalender4.Format = DateTimePickerFormat.Custom;
                            dtpCalender4.Height = 20;
                            dtpCalender4.Width = 107;

                            dtpCalender4.Left = myTextBox.Left;
                            dtpCalender4.Top = myTextBox.Top;
                            dtpCalender4.Enabled = false;

                            myTextBox.Parent.Controls.Add(this.dtpCalender4);

                            btnDate4 = new Button();
                            btnDate4.Name = "btnDate4";
                            btnDate4.Height = 18;
                            btnDate4.Width = 17;
                            btnDate4.Image = (Image)Properties.Resources.Clear;

                            btnDate4.BackColor = System.Drawing.Color.White;
                            btnDate4.FlatStyle = FlatStyle.Flat;
                            btnDate4.FlatAppearance.BorderSize = 0;

                            btnDate4.Left = myTextBox.Left + 1;
                            btnDate4.Top = myTextBox.Top + 1;
                            btnDate4.TabStop = false;
                            btnDate4.Enabled = false;

                            myTextBox.Parent.Controls.Add(this.btnDate4);

                            //Errol 2012-08-31
                            this.btnDate4.Click -= new System.EventHandler(this.Calender_Clear_Click);
                            this.btnDate4.Click += new System.EventHandler(this.Calender_Clear_Click);

                            myTextBox.Left += 17;

                            myTextBox.Top += 3;
                            myTextBox.Left += 1;
                            myTextBox.BorderStyle = BorderStyle.None;
                            myTextBox.Width = 54;
                            myTextBox.ReadOnly = true;

                            //Errol 2012-08-31
                            this.dtpCalender4.CloseUp -= new System.EventHandler(this.Calender_CloseUp);
                            this.dtpCalender4.CloseUp += new System.EventHandler(this.Calender_CloseUp);

                            IntPtrDateTextBox[3] = myTextBox.Handle;

                            this.btnDate4.BringToFront();
                        }
                        else
                        {
                            MessageBox.Show("Error Creating");
                        }
                    }
                }
            }

            intNumberCalendersCreated += 1;
        }
       
        public void Calculate_Wage_Time_Breakdown(DataView parDataView, int parintRow, int intDayNo,
            int parintTotalDayHours, int parintNormalTimeTotalBoundary,
            ref int parintOverTime1TotalBoundary, ref int parintOverTime2TotalBoundary,
            ref int parintOverTime3TotalBoundary,
            ref int parintNormalTime, ref int parintOverTime1, ref int parintOverTime2,
            ref int parintOverTime3,
            ref int parintNormalTimeWeekTotal, ref int parintOverTime1WeekTotal,
            ref int parintOverTime2WeekTotal, ref int parintOverTime3WeekTotal)
        {
            int intTimeBreakDown = 0;
            int intOverTimeExceed = 0;

            parintNormalTime = 0;
            parintOverTime1 = 0;
            parintOverTime2 = 0;
            parintOverTime3 = 0;

            if (intDayNo > 0
                & intDayNo < 6)
            {
                //Week Day
                intTimeBreakDown = parintNormalTimeWeekTotal + parintTotalDayHours;

                if (parintTotalDayHours > Convert.ToDecimal(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"])
                    & Convert.ToDecimal(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"]) != 0)
                {
                    intOverTimeExceed = parintTotalDayHours - Convert.ToInt32(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"]);
                }
            }
            else
            {
                if (intDayNo == 6)
                {
                    //Break Time into Pay Time Range Brackets
                    //Saturday's Breakdown
                    if (parDataView[parintRow]["SATURDAY_PAY_RATE_IND"].ToString() == ""
                        | Convert.ToDecimal(parDataView[parintRow]["SATURDAY_PAY_RATE"]) == 0)
                    {
                        intTimeBreakDown = parintNormalTimeWeekTotal + parintTotalDayHours;

                        if (parintTotalDayHours > Convert.ToDecimal(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"])
                            & Convert.ToDecimal(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"]) != 0)
                        {
                            intOverTimeExceed = parintTotalDayHours - Convert.ToInt32(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"]);
                        }
                    }
                    else
                    {
                        if (parDataView[parintRow]["SATURDAY_PAY_RATE_IND"].ToString() == "A"
                            | parintNormalTimeWeekTotal > parintNormalTimeTotalBoundary)
                        {
                            //Append to relevant OverTime Rate
                            if (Convert.ToDecimal(parDataView[parintRow]["SATURDAY_PAY_RATE"]) == Convert.ToDecimal(parDataView[parintRow]["OVERTIME1_RATE"]))
                            {
                                parintOverTime1 = parintTotalDayHours;
                                goto Calculate_Wage_Time_Breakdown_Continue;
                            }
                            else
                            {
                                if (Convert.ToDecimal(parDataView[parintRow]["SATURDAY_PAY_RATE"]) == Convert.ToDecimal(parDataView[parintRow]["OVERTIME2_RATE"]))
                                {
                                    parintOverTime2 = parintTotalDayHours;
                                    goto Calculate_Wage_Time_Breakdown_Continue;
                                }
                                else
                                {
                                    if (Convert.ToDecimal(parDataView[parintRow]["SATURDAY_PAY_RATE"]) == Convert.ToDecimal(parDataView[parintRow]["OVERTIME3_RATE"]))
                                    {
                                        parintOverTime3 = parintTotalDayHours;
                                        goto Calculate_Wage_Time_Breakdown_Continue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            intTimeBreakDown = parintNormalTimeWeekTotal + parintTotalDayHours;

                            //Time Required to Fill Normal Time
                            if (parintTotalDayHours > Convert.ToDecimal(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"])
                                & Convert.ToDecimal(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"]) != 0)
                            {
                                intOverTimeExceed = parintTotalDayHours - Convert.ToInt32(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"]);
                            }

                            if (intTimeBreakDown > parintNormalTimeTotalBoundary)
                            {
                                //Boundary Already Exceeded
                                if (parintNormalTimeWeekTotal != parintNormalTimeTotalBoundary)
                                {
                                    parintTotalDayHours = parintNormalTimeTotalBoundary - parintNormalTimeWeekTotal;
                                    parintNormalTime = parintTotalDayHours;
                                }

                                parintTotalDayHours = intTimeBreakDown - parintNormalTimeTotalBoundary;

                                //Time For Relevant Overtime
                                if (Convert.ToDecimal(parDataView[parintRow]["SATURDAY_PAY_RATE"]) == Convert.ToDecimal(parDataView[parintRow]["OVERTIME1_RATE"]))
                                {
                                    parintOverTime1 = parintTotalDayHours;
                                    goto Calculate_Wage_Time_Breakdown_Continue;
                                }
                                else
                                {
                                    if (Convert.ToDecimal(parDataView[parintRow]["SATURDAY_PAY_RATE"]) == Convert.ToDecimal(parDataView[parintRow]["OVERTIME2_RATE"]))
                                    {
                                        parintOverTime2 = parintTotalDayHours;
                                        goto Calculate_Wage_Time_Breakdown_Continue;
                                    }
                                    else
                                    {
                                        if (Convert.ToDecimal(parDataView[parintRow]["SATURDAY_PAY_RATE"]) == Convert.ToDecimal(parDataView[parintRow]["OVERTIME3_RATE"]))
                                        {
                                            parintOverTime3 = parintTotalDayHours;
                                            goto Calculate_Wage_Time_Breakdown_Continue;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //All Goes to Normal Hours
                                parintNormalTime = parintTotalDayHours;
                            }
                        }
                    }
                }
                else
                {
                    //Break Time into Pay Time Range Brackets
                    //Sunday's Breakdown
                    if (parDataView[parintRow]["SUNDAY_PAY_RATE_IND"].ToString() == ""
                        | Convert.ToDecimal(parDataView[parintRow]["SUNDAY_PAY_RATE"]) == 0)
                    {
                        intTimeBreakDown = parintNormalTimeWeekTotal + parintTotalDayHours;

                        if (parintTotalDayHours > Convert.ToDecimal(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"])
                            & Convert.ToDecimal(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"]) != 0)
                        {
                            intOverTimeExceed = parintTotalDayHours - Convert.ToInt32(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"]);
                        }
                    }
                    else
                    {
                        if (parDataView[parintRow]["SUNDAY_PAY_RATE_IND"].ToString() == "A"
                            | parintNormalTimeWeekTotal > parintNormalTimeTotalBoundary)
                        {
                            //Append to relevant OverTime Rate
                            if (Convert.ToDecimal(parDataView[parintRow]["SUNDAY_PAY_RATE"]) == Convert.ToDecimal(parDataView[parintRow]["OVERTIME1_RATE"]))
                            {
                                parintOverTime1 = parintTotalDayHours;
                                goto Calculate_Wage_Time_Breakdown_Continue;
                            }
                            else
                            {
                                if (Convert.ToDecimal(parDataView[parintRow]["SUNDAY_PAY_RATE"]) == Convert.ToDecimal(parDataView[parintRow]["OVERTIME2_RATE"]))
                                {
                                    parintOverTime2 = parintTotalDayHours;
                                    goto Calculate_Wage_Time_Breakdown_Continue;
                                }
                                else
                                {
                                    if (Convert.ToDecimal(parDataView[parintRow]["SUNDAY_PAY_RATE"]) == Convert.ToDecimal(parDataView[parintRow]["OVERTIME3_RATE"]))
                                    {
                                        parintOverTime3 = parintTotalDayHours;
                                        goto Calculate_Wage_Time_Breakdown_Continue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Time Required to Fill Normal Time
                            intTimeBreakDown = parintNormalTimeWeekTotal + parintTotalDayHours;

                            if (parintTotalDayHours > Convert.ToDecimal(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"])
                                & Convert.ToDecimal(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"]) != 0)
                            {
                                intOverTimeExceed = parintTotalDayHours - Convert.ToInt32(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"]);
                            }

                            if (intTimeBreakDown > parintNormalTimeTotalBoundary)
                            {
                                //Boundary Already Exceeded
                                if (parintNormalTimeWeekTotal != parintNormalTimeTotalBoundary)
                                {
                                    parintTotalDayHours = parintNormalTimeTotalBoundary - parintNormalTimeWeekTotal;
                                    parintNormalTime = parintTotalDayHours;
                                }

                                parintTotalDayHours = intTimeBreakDown - parintNormalTimeTotalBoundary;

                                //Time For Relevant Overtime
                                if (Convert.ToDecimal(parDataView[parintRow]["SUNDAY_PAY_RATE"]) == Convert.ToDecimal(parDataView[parintRow]["OVERTIME1_RATE"]))
                                {
                                    parintOverTime1 = parintTotalDayHours;
                                    goto Calculate_Wage_Time_Breakdown_Continue;
                                }
                                else
                                {
                                    if (Convert.ToDecimal(parDataView[parintRow]["SUNDAY_PAY_RATE"]) == Convert.ToDecimal(parDataView[parintRow]["OVERTIME2_RATE"]))
                                    {
                                        parintOverTime2 = parintTotalDayHours;
                                        goto Calculate_Wage_Time_Breakdown_Continue;
                                    }
                                    else
                                    {
                                        if (Convert.ToDecimal(parDataView[parintRow]["SUNDAY_PAY_RATE"]) == Convert.ToDecimal(parDataView[parintRow]["OVERTIME3_RATE"]))
                                        {
                                            parintOverTime3 = parintTotalDayHours;
                                            goto Calculate_Wage_Time_Breakdown_Continue;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //All Goes to Normal Hours
                                parintNormalTime = parintTotalDayHours;
                            }
                        }
                    }
                }
            }

            if (intTimeBreakDown > parintNormalTimeTotalBoundary)
            {
                //Add All OverTime Hours To BreakDown
                if (parintOverTime1TotalBoundary > 0)
                {
                    intTimeBreakDown += parintOverTime1TotalBoundary;
                }

                if (parintOverTime2TotalBoundary > 0)
                {
                    intTimeBreakDown += parintOverTime2TotalBoundary;
                }

                if (parintOverTime3TotalBoundary > 0)
                {
                    intTimeBreakDown += parintOverTime3TotalBoundary;
                }
            }

            int intOverTime1 = Convert.ToInt32(parDataView[parintRow]["OVERTIME1_MINUTES"]);
            int intOverTime2 = Convert.ToInt32(parDataView[parintRow]["OVERTIME2_MINUTES"]);
            int intOverTime3 = Convert.ToInt32(parDataView[parintRow]["OVERTIME3_MINUTES"]);

            //Brought Forward Value From Previous Run has already Exceeded Normal Boundary
            if (parintNormalTimeTotalBoundary < 0)
            {
                int intValue = parintNormalTimeTotalBoundary * -1;

                if (intValue > intOverTime1)
                {
                    intValue = intValue - intOverTime1;

                    intOverTime1 = 0;

                    if (intValue > intOverTime2)
                    {
                        intValue = intValue - intOverTime2;

                        intOverTime2 = 0;

                        if (intValue > intOverTime3)
                        {
                            intOverTime3 = 0;
                        }
                        else
                        {
                            intOverTime3 = intOverTime3 - intValue;
                        }
                    }
                    else
                    {
                        intOverTime2 = intOverTime2 - intValue;
                    }
                }
                else
                {
                    intOverTime1 = intOverTime1 - intValue;
                }
            }

            if (intTimeBreakDown > parintNormalTimeTotalBoundary)
            {
                if (parintNormalTimeTotalBoundary < 0)
                {
                }
                else
                {
                    if (parintNormalTimeTotalBoundary != parintNormalTimeWeekTotal)
                    {
                        parintNormalTime = parintNormalTimeTotalBoundary - parintNormalTimeWeekTotal;
                    }

                    intTimeBreakDown = intTimeBreakDown - parintNormalTimeTotalBoundary;
                }

                if (intTimeBreakDown > intOverTime1)
                {
                    if (intOverTime1 != parintOverTime1TotalBoundary)
                    {
                        parintOverTime1 = intOverTime1 - parintOverTime1TotalBoundary;
                    }

                    intTimeBreakDown = intTimeBreakDown - intOverTime1;

                    if (intTimeBreakDown > intOverTime2)
                    {
                        if (intOverTime2 != parintOverTime2TotalBoundary)
                        {
                            parintOverTime2 = intOverTime2 - parintOverTime2TotalBoundary;
                        }

                        intTimeBreakDown = intTimeBreakDown - intOverTime2;

                        parintOverTime3 = intTimeBreakDown - parintOverTime3TotalBoundary;
                    }
                    else
                    {
                        parintOverTime2 = intTimeBreakDown - parintOverTime2TotalBoundary;
                    }
                }
                else
                {
                    parintOverTime1 = intTimeBreakDown - parintOverTime1TotalBoundary;
                }
            }
            else
            {
                if (intOverTimeExceed == 0)
                {
                    parintNormalTime = parintTotalDayHours;
                }
                else
                {
                    parintNormalTime = Convert.ToInt32(parDataView[parintRow]["TOTAL_DAILY_TIME_OVERTIME"]);

                    intTimeBreakDown = intOverTimeExceed;

                    if (intTimeBreakDown > intOverTime1)
                    {
                        if (intOverTime1 != parintOverTime1TotalBoundary)
                        {
                            parintOverTime1 = intOverTime1 - parintOverTime1TotalBoundary;

                            intTimeBreakDown = intTimeBreakDown - intOverTime1;
                        }

                        if (intTimeBreakDown > intOverTime2)
                        {
                            if (intOverTime2 != parintOverTime2TotalBoundary)
                            {
                                parintOverTime2 = intOverTime2 - parintOverTime2TotalBoundary;

                                intTimeBreakDown = intTimeBreakDown - intOverTime2;

                                parintOverTime3 = intTimeBreakDown;
                            }
                        }
                        else
                        {
                            parintOverTime2 = intTimeBreakDown;
                        }
                    }
                    else
                    {
                        parintOverTime1 = intTimeBreakDown;
                    }
                }
            }

            //Calculate OverTime Boundary Values
            if (parintOverTime1 > 0)
            {
                parintOverTime1TotalBoundary += parintOverTime1;
            }

            if (parintOverTime2 > 0)
            {
                parintOverTime2TotalBoundary += parintOverTime2;
            }

            if (parintOverTime3 > 0)
            {
                parintOverTime3TotalBoundary += parintOverTime3;
            }


        Calculate_Wage_Time_Breakdown_Continue:

            //Final Values To Appear on SpreadSheet
            if (parintNormalTime > 0)
            {
                parintNormalTimeWeekTotal += parintNormalTime;
            }

            if (parintOverTime1 > 0)
            {
                parintOverTime1WeekTotal += parintOverTime1;
            }

            if (parintOverTime2 > 0)
            {
                parintOverTime2WeekTotal += parintOverTime2;
            }

            if (parintOverTime3 > 0)
            {
                parintOverTime3WeekTotal += parintOverTime3;
            }
        }

        public string Get_Day_From_DayofWeek(int parintDay)
        {
            string strDayOfWeek = "";

            switch (parintDay)
            {
                case 0:

                    strDayOfWeek = "SUN";

                    break;

                case 1:

                    strDayOfWeek = "MON";

                    break;

                case 2:

                    strDayOfWeek = "TUE";

                    break;

                case 3:

                    strDayOfWeek = "WED";

                    break;

                case 4:

                    strDayOfWeek = "THU";

                    break;

                case 5:

                    strDayOfWeek = "FRI";

                    break;

                case 6:

                    strDayOfWeek = "SAT";

                    break;
            }

            return strDayOfWeek;
        }
        
        public byte[] Compress_DataSet(DataSet parDataSet)
        {
            parDataSet.RemotingFormat = SerializationFormat.Binary;

            MemoryStream msMemoryStream = new MemoryStream();
            MemoryStream msMemoryStreamCompressed = new MemoryStream();

            BinaryFormatter bfBinaryFormatter = new BinaryFormatter();
            bfBinaryFormatter.Serialize(msMemoryStream, parDataSet);

            System.IO.Compression.GZipStream GZipStreamCompressed = new GZipStream(msMemoryStreamCompressed, CompressionMode.Compress, true);
            GZipStreamCompressed.Write(msMemoryStream.ToArray(), 0, (int)msMemoryStream.Length);
            GZipStreamCompressed.Flush();
            GZipStreamCompressed.Close();

            return msMemoryStreamCompressed.ToArray();
        }

        public DataSet DeCompress_Array_To_DataSet(byte[] parbytArray)
        {
            DataSet DataSet = new DataSet();
            DataSet.RemotingFormat = SerializationFormat.Binary;

            MemoryStream msMemoryStreamCompressed = new MemoryStream(parbytArray);
            System.IO.Compression.GZipStream GZipStreamCompressed = new GZipStream(msMemoryStreamCompressed, CompressionMode.Decompress, true);
            
            byte[] byteDecompressed = ReadFullStream(GZipStreamCompressed);
            GZipStreamCompressed.Flush();
            GZipStreamCompressed.Close();

            MemoryStream msMemoryStreamDecompressed = new MemoryStream(byteDecompressed);
           
            BinaryFormatter bf = new BinaryFormatter();
            DataSet = (DataSet)bf.Deserialize(msMemoryStreamDecompressed, null);
                      
            return DataSet;
        }

        private byte[] ReadFullStream(Stream stream)
        {
            byte[] bytBuffer = new byte[32768];

            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int intCharCount = stream.Read(bytBuffer, 0, bytBuffer.Length);

                    if (intCharCount <= 0)
                    {
                        return ms.ToArray();
                    }
                    else
                    {
                        ms.Write(bytBuffer, 0, intCharCount);
                    }
                }
            }
        }
     
        public object DynamicFunction(string FunctionName, object[] objParm, bool ShowDBUpdateScreen)
        {
            try
            {
                AppDomain.CurrentDomain.SetData("KillApp", "N");

                DateTime dtNewDatetime = DateTime.Now.AddSeconds(1000);
                pvtsrFunctionNameSaved = FunctionName;
                pvtobjParmSaved = objParm;
                
                pvtblnCommunicationError = false;
                pvtblnCommunicationTimeOutError = false;
                pvtblnOtherError = false;

                if (AppDomain.CurrentDomain.GetData("Globe") != null)
                {
                    if (myPanelGlobe == null)
                    {
                        myPanelGlobe = (Panel)AppDomain.CurrentDomain.GetData("Globe");
                    }

                    myPanelGlobe.Visible = true;
                    myPanelGlobe.Refresh();
                    Application.DoEvents();
                }

//2012-09-03 (Temp)
#if(DEBUG)
#else
                if (pvtParentForm != null)
                {
                    if (pvtParentForm.Name != "frmPayrollMain"
                    & pvtParentForm.Name != "frmSplashScreen"
                    & pvtParentForm.Name != "frmReadWriteFile"
                    & pvtParentForm.Name != "frmTimeAttendanceMain")
                    {
                        pvtParentForm.Cursor = Cursors.WaitCursor;
                        //Disable_Forms_Controls();
                    }
                }
#endif
                if (AppDomain.CurrentDomain.GetData("URLPath").ToString() != "")
                {
                    busWebDynamicServices.DynamicFunctionAsync(pvtstrBusinessObjectName, FunctionName, objParm);

                    if (pvtintWebServiceMilliSecondTimeOut != -1)
                    {
                        dtNewDatetime = DateTime.Now.AddMilliseconds(pvtintWebServiceMilliSecondTimeOut);
                    }

                    pvtblnCallBackComplete = false;
                    //Loop Until Call Completed From Web Server
                    while (pvtblnCallBackComplete == false
                    & AppDomain.CurrentDomain.GetData("KillApp").ToString() != "Y")
                    {
                        Application.DoEvents();
                        Application.DoEvents();

                        if (pvtintWebServiceMilliSecondTimeOut != -1)
                        {
                            if (dtNewDatetime < DateTime.Now)
                            {
                                busWebDynamicServices.Abort();
                            }
                        }
                    }

                    if (AppDomain.CurrentDomain.GetData("KillApp").ToString() == "Y")
                    {
                        busWebDynamicServices.Abort();
                        goto DynamicFunction_Return;
                    }
                }
                else
                {
                    pvtReturnObject = null;
                    MethodInfo mi = typObjectType.GetMethod(FunctionName);
                    pvtReturnObject = mi.Invoke(busDynamicService, objParm);
                }
#if(DEBUG)
#else
                if (pvtParentForm != null)
                {
                    if (pvtParentForm.Name != "frmPayrollMain"
                    & pvtParentForm.Name != "frmSplashScreen"
                    & pvtParentForm.Name != "frmReadWriteFile"
                    & pvtParentForm.Name != "frmTimeAttendanceMain")
                    {
                        //Re_Enable_Forms_Controls();
                    }
                }
#endif
                pvtParentForm.Cursor = Cursors.Default;

                if (ShowDBUpdateScreen == true)
                {
                    frmDatabaseUpdated frmDatabaseUpdated = new frmDatabaseUpdated();
                    frmDatabaseUpdated.Show();

                    clsFadeForm.FadeForm(frmDatabaseUpdated);

                    frmDatabaseUpdated.Close();
                }

                DynamicFunction_Return:

                if (myPanelGlobe != null)
                {
                    myPanelGlobe.Visible = false;
                    myPanelGlobe.Refresh();
                    Application.DoEvents();
                }

                return pvtReturnObject;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                pvtParentForm.Cursor = Cursors.Default;
                MessageBox.Show("Connection Failure", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }
            catch (Exception ex)
            {
                FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "ShowError.txt");

                if (fiFileInfo.Exists == true)
                {
                    string strError = ex.ToString();
                                      
                    MessageBox.Show("clsISUtilities Call " + pvtstrBusinessObjectName + "/" + FunctionName + " " + strError);
                }

                pvtParentForm.Cursor = Cursors.Default;
                string strExceptionError = "";

                if (pvtblnCommunicationError == false)
                {
                    if (ex.Message.IndexOf("Server Unavailable") > -1)
                    {
                        strExceptionError = "Connection to Web Server Could NOT be established";
                    }
                    else
                    {
                        strExceptionError = ex.ToString();
                        
                        strExceptionError += "\n\n" + "Where : " + FunctionName;
                        strExceptionError += "\n\n" + "Object : " + pvtstrBusinessObjectName + ".dll";
                    }
                }

                Show_Write_Errors(strExceptionError);

                return null;
            }
        }

        public object DynamicFunction(string FunctionName, object[] objParm)
        {
            string strWhere = "";

            AppDomain.CurrentDomain.SetData("KillApp", "N");

            DateTime dtNewDatetime = DateTime.Now.AddSeconds(100);

            bool blnShowGlobeAndWaitCursor = false;

            try
            {
                pvtblnCommunicationError = false;
                pvtblnCommunicationTimeOutError = false;
                pvtblnOtherError = false;

                if (AppDomain.CurrentDomain.GetData("Globe") != null)
                {
                    if (myPanelGlobe == null)
                    {
                        myPanelGlobe = (Panel)AppDomain.CurrentDomain.GetData("Globe");
                    }

                    myPanelGlobe.Visible = true;
                    myPanelGlobe.Refresh();
                    Application.DoEvents();
  
                }
//2012-09-03
//#if(DEBUG)
//#else
                if (pvtParentForm != null)
                {
                    if (pvtParentForm.Name != "frmPayrollMain"
                    &  pvtParentForm.Name != "frmSplashScreen"
                    &  pvtParentForm.Name != "frmReadWriteFile")
                    {
#if(DEBUG)
                        strWhere = "Disable_Forms_Controls";
#endif
                        Disable_Forms_Controls();
                    }

                    if (pvtParentForm.Name != "frmReadWriteFile")
                    {
                        pvtParentForm.Cursor = Cursors.WaitCursor;
                    }
                }
               
                if (AppDomain.CurrentDomain.GetData("URLPath").ToString() != "")
                {
                    busWebDynamicServices.DynamicFunctionAsync(pvtstrBusinessObjectName, FunctionName, objParm);

                    if (pvtintWebServiceMilliSecondTimeOut != -1)
                    {
                        dtNewDatetime = DateTime.Now.AddMilliseconds(pvtintWebServiceMilliSecondTimeOut);
                    }

                    pvtblnCallBackComplete = false;
                    //Loop Until Call Completed From Web Server
                    while (pvtblnCallBackComplete == false
                    & AppDomain.CurrentDomain.GetData("KillApp").ToString() != "Y")
                    {
                        Application.DoEvents();

                        if (pvtintWebServiceMilliSecondTimeOut != -1)
                        {
                            if (dtNewDatetime < DateTime.Now)
                            {
                                busWebDynamicServices.Abort();
                            }
                        }
                    }

                    if (AppDomain.CurrentDomain.GetData("KillApp").ToString() == "Y")
                    {
                        busWebDynamicServices.Abort();
                        goto DynamicFunction_Return;
                    }
                }
                else
                {
                    pvtReturnObject = null;
                    MethodInfo mi = typObjectType.GetMethod(FunctionName);
                    pvtReturnObject = mi.Invoke(busDynamicService, objParm);
                }
                //2012-09-03
                //#if(DEBUG)
                //#else
             
                if (pvtParentForm != null)
                {
                    if (pvtParentForm.Name != "frmPayrollMain"
                    &  pvtParentForm.Name != "frmSplashScreen"
                    &  pvtParentForm.Name != "frmReadWriteFile")
                    {
#if(DEBUG)
                        strWhere = "Re_Enable_Forms_Controls";
#endif
                        if (pvtParentForm.Name == "frmTimeAttendanceRun"
                        && FunctionName == "Check_Queue")
                        {
                            if (pvtReturnObject != null)
                            {
                                if (pvtReturnObject.ToString() == "S")
                                {
                                    blnShowGlobeAndWaitCursor = true;
                                }
                                else
                                {
                                    Re_Enable_Forms_Controls();
                                }
                            }
                            else
                            {
                                Re_Enable_Forms_Controls();
                            }
                        }
                        else
                        { 
                            Re_Enable_Forms_Controls();
                        }
                    }

                    if (blnShowGlobeAndWaitCursor == false)
                    {
                        pvtParentForm.Cursor = Cursors.Default;
                    }
                }
//#endif

                DynamicFunction_Return:

                if (myPanelGlobe != null
                && blnShowGlobeAndWaitCursor == false)
                {
                    myPanelGlobe.Visible = false;
                    myPanelGlobe.Refresh();
                    Application.DoEvents();
                }

                return pvtReturnObject;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                MessageBox.Show("Connection Failure", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }
            catch (Exception ex)
            {
                FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "ShowError.txt");

                if (fiFileInfo.Exists == true)
                {
                    string strError = ex.ToString();

                    MessageBox.Show("clsISUtilities Call " + pvtstrBusinessObjectName + "/" + FunctionName + " " + strError);
                }

                string strExceptionError = "";

                if (pvtblnCommunicationError == false)
                {
                    if (ex.Message.IndexOf("Server Unavailable") > -1)
                    {
                        strExceptionError = "Connection to Web Server Could NOT be established";
                    }
                    else
                    {
                        strExceptionError = ex.ToString();
                        
                        strExceptionError += "\n\n" + "Where : " + FunctionName;
                        strExceptionError += "\n\n" + "Object : " + pvtstrBusinessObjectName + ".dll";
                    }
                }

                Show_Write_Errors(strExceptionError);

                return null;
            }
        }

        private void Re_Enable_Forms_Controls()
        {
            if (this.pvtParentForm != null)
            {
                for (int intCount = 0; intCount < ControlPtr.Length; intCount++)
                {
                    if (ControlPtr[intCount] != (IntPtr)0)
                    {
                        Control MyControl = Control.FromHandle((IntPtr)ControlPtr[intCount]);
                        MyControl.Enabled = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void Disable_Forms_Controls()
        {
            if (this.pvtParentForm != null)
            {
                ControlPtr = null;
                ControlPtr = new IntPtr[1000];
                intControlPtrIndex = -1;

                foreach (Control myControl in pvtParentForm.Controls)
                {
                    if (myControl is Label
                    | myControl is PictureBox
                    | myControl is MdiClient)
                    {
                        continue;
                    }
                    else
                    {
                        if (myControl is TextBox
                        | myControl is ComboBox
                        | myControl is ListBox
                        | myControl is RadioButton
                        | myControl is DateTimePicker
                        | myControl is CheckBox)
                        {
                            if (myControl.Enabled == true)
                            {
                               intControlPtrIndex += 1;
                               ControlPtr[intControlPtrIndex] = myControl.Handle;

                               myControl.Enabled = false;
                            }
                        }
                        else
                        {
                            if (myControl is Button)
                            {
                                if (myControl.Text == "Close")
                                {
                                }
                                else
                                {
                                    if (myControl.Text == "New"
                                        | myControl.Text == "Update"
                                        | myControl.Text == "Delete"
                                        | myControl.Text == "Save"
                                        | myControl.Text == "Cancel")
                                    {
                                        if (myControl.Enabled == true)
                                        {
                                            if (pvtParentForm.Name == "frmLogonScreen"
                                            &  myControl.Text == "Cancel")
                                            {
                                            }
                                            else
                                            {
                                                intControlPtrIndex += 1;
                                                ControlPtr[intControlPtrIndex] = myControl.Handle;

                                                myControl.Enabled = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (myControl.Enabled == true)
                                        {
                                            intControlPtrIndex += 1;
                                            ControlPtr[intControlPtrIndex] = myControl.Handle;

                                            myControl.Enabled = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (myControl.ProductName == "ComponentOne C1FlexGrid")
                                {
                                    if (myControl.Enabled == true)
                                    {
                                        intControlPtrIndex += 1;
                                        ControlPtr[intControlPtrIndex] = myControl.Handle;

                                        myControl.Enabled = false;
                                    }
                                }
                                else
                                {
                                    if (myControl is GroupBox
                                        | myControl is TabControl
                                        | myControl is TabPage
                                        | myControl is ToolStrip
                                        | myControl is ProgressBar
                                        | myControl is TreeView
                                        | myControl is Panel)
                                    {
                                        if (myControl.Enabled == true)
                                        {
                                            if (myControl.Name == "grbTimesheetError"
                                            || myControl.Name == "grbStatus"
                                            || myControl.Name == "grbLeaveReminder"
                                            || myControl.Name == "grbActivationProcess"
                                            || myControl.Name == "grbPublicHolidayInfo")
                                            {
                                            }
                                            else
                                            {
                                                intControlPtrIndex += 1;
                                                ControlPtr[intControlPtrIndex] = myControl.Handle;

                                                myControl.Enabled = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (myControl is DataGridView)
                                        {
                                            if (myControl.Name == "dgvPayrollTypeDataGridView")
                                            {
                                                string strStop = "";

                                                string a = pvtsrFunctionNameSaved;
                                                object[] myObj;

                                                if (pvtobjParmSaved != null)
                                                {
                                                    myObj = pvtobjParmSaved;
                                                }
                                            }

                                            if (myControl.Enabled == true)
                                            {
                                                intControlPtrIndex += 1;
                                                ControlPtr[intControlPtrIndex] = myControl.Handle;

                                                myControl.Enabled = false;
                                            }
                                        }
                                        else
                                        {
                                            if (myControl.Name != "reportViewer")
                                            {
                                                MessageBox.Show("Control Not Catered for = " + myControl.Name);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public int WebService_Settings(System.Web.Services.Protocols.WebClientProtocol ws, string parWebServiceName)
        {
            try
            {
                ws.Url = @"http://" + AppDomain.CurrentDomain.GetData("URLPath").ToString() + @"/InteractPayroll/" + parWebServiceName + ".asmx";

                ws.Timeout = 120000;

                ws.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Web Service '" + parWebServiceName + "' URL Path '" + AppDomain.CurrentDomain.GetData("URLPath").ToString() + "' is Invalid.\n\nContact System Administrator.",
                    "Web Service Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                if (AppDomain.CurrentDomain.GetData("TimerCloseCurrentForm") != null)
                {
                    if (pvtblnCloseFormSent == false)
                    {
                        if (pvtParentForm != null)
                        {
                            pvtblnCloseFormSent = true;

                            AppDomain.CurrentDomain.SetData("FormToClose", pvtParentForm);

                            Timer tmrCloseCurrentForm = (Timer)AppDomain.CurrentDomain.GetData("TimerCloseCurrentForm");
                            tmrCloseCurrentForm.Enabled = true;
                        }
                    }
                }
                
                return -1;
            }

            return 0;
        }

        public DataTable Get_Countries()
        {
            DataTable DataTable = new DataTable("Country");

            DataTable.Columns.Add("COUNTRY_CODE", typeof(String));
            DataTable.Columns.Add("COUNTRY_CODE2", typeof(String));
            DataTable.Columns.Add("COUNTRY_DESC", typeof(String));

            DataRow DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ZNC";
            DataRow["COUNTRY_CODE2"] = "ZN";
            DataRow["COUNTRY_DESC"] = "[Any country not on this list]";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "AFG";
            DataRow["COUNTRY_CODE2"] = "AF";
            DataRow["COUNTRY_DESC"] = "Afghanistan";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ALA";
            DataRow["COUNTRY_CODE2"] = "AX";
            DataRow["COUNTRY_DESC"] = "Åland Islands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();
            DataRow["COUNTRY_CODE"] = "ALB";
            DataRow["COUNTRY_CODE2"] = "AL";
            DataRow["COUNTRY_DESC"] = "Albania";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "DZA";
            DataRow["COUNTRY_CODE2"] = "DZ";
            DataRow["COUNTRY_DESC"] = "Algeria";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ASM";
            DataRow["COUNTRY_CODE2"] = "AS";
            DataRow["COUNTRY_DESC"] = "American Samoa";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "AND";
            DataRow["COUNTRY_CODE2"] = "AD";
            DataRow["COUNTRY_DESC"] = "Andorra";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "AGO";
            DataRow["COUNTRY_CODE2"] = "AO";
            DataRow["COUNTRY_DESC"] = "Angola";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "AIA";
            DataRow["COUNTRY_CODE2"] = "AI";
            DataRow["COUNTRY_DESC"] = "Anguilla";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ATA";
            DataRow["COUNTRY_CODE2"] = "AQ";
            DataRow["COUNTRY_DESC"] = "Antarctica";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ATG";
            DataRow["COUNTRY_CODE2"] = "AG";
            DataRow["COUNTRY_DESC"] = "Antigua and Barbuda";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ARG";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Argentina";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ARM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Armenia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ABW";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Aruba";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "AUS";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Australia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "AUT";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Austria";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "AZE";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Azerbaijan";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BHS";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Bahamas";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BHR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Bahrain";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BGD";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Bangladesh";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BRB";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Barbados";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BLR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Belarus";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BEL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Belgium";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BLZ";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Belize";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BEN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Benin";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BMU";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Bermuda";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BTN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Bhutan";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BOL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Bolivia";


            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            //Fix
            DataRow["COUNTRY_CODE"] = "";
            DataRow["COUNTRY_CODE2"] = "BQ";
            DataRow["COUNTRY_DESC"] = "Bonair,Sint Eustatius and Saba";
          
            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BIH";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Bosnia and Herzegovina";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BWA";
            DataRow["COUNTRY_CODE2"] = "BW";
            DataRow["COUNTRY_DESC"] = "Botswana";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BVT";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Bouvet Island";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BRA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Brazil";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "IOT";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "British Indian Ocean Territory";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "VGB";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "British Virgin Islands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BRN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Brunei Darussalam";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BGR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Bulgaria";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BFA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Burkina Faso";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "BDI";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Burundi";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "KHM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Cambodia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CMR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Cameroon";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CAN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Canada";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CPV";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Cape Verde";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CYM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Cayman Islands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CAF";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Central African Republic";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TCD";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Chad";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CHL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Chile";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CHN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "China";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CXR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Christmas Island";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CCK";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Cocos (Keeling) Island";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "COL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Colombia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "COM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Comoros";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "COG";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Congo";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "COK";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Cook Islands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CRI";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Costa Rica";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CIV";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Côte d'Ivoire";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "HRV";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Croatia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CUB";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Cuba";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CYP";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Cyprus";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CZE";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Czech Republic";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "PRK";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Democratic People's Republic of Korea";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "COD";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Democratic Republic of the Congo";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "DNK";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Denmark";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "DJI";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Djibouti";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "DMA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Dominica";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "DOM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Dominican Republic";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ECU";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Ecuador";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "EGY";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Egypt";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SLV";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "El Salvador";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GNQ";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Equatorial Guinea";

            DataRow["COUNTRY_CODE"] = "ERI";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Eritrea";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "EST";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Estonia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ETH";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Ethiopia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "EUU";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "European Union";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "FRO";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Faeroe Islands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "FLK";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Falkland Islands (Malvinas)";

            DataRow["COUNTRY_CODE"] = "FJI";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Fiji";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "FIN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Finland";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "FRA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "France";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GUF";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "French Guiana";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "PYF";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "French Polynesia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ATF";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "French Southern Territories - TF";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GAB";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Gabon";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GMB";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Gambia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GEO";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Georgia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "DEU";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Germany";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GHA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Ghana";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GIB";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Gibraltar";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GRC";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Greece";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GRL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Greenland";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GRD";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Grenada";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GLP";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Guadeloupe";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GUM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Guam";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GTM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Guatemala";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GGY";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Guernsey";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GIN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Guinea";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GNB";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Guinea-Bissau";

            DataRow["COUNTRY_CODE"] = "GUY";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Guyana";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "HTI";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Haiti";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "HMD";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Heard and McDonald Islands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "VAT";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Holy See";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "HND";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Honduras";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "HKG";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Hong Kong Special Administrative Region of China";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "HUN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Hungary";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ISL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Iceland";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "IND";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "India";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "IDN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Indonesia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "IRN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Iran, Islamic Republic of";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "IRQ";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Iraq";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "IRL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Ireland";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "IMN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Isle of Man";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ISR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Israel";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ITA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Italy";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "JAM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Jamaica";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "JPN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Japan";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "JEY";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Jersey";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "JOR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Jordan";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "KAZ";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Kazakhstan";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "KEN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Kenya";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "KIR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Kiribati";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "KWT";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Kuwait";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "KGZ";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Kyrgyzstan";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "LAO";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Lao People's Democratic Republic";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "LVA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Latvia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "LBN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Lebanon";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "LSO";
            DataRow["COUNTRY_CODE2"] = "LS";
            DataRow["COUNTRY_DESC"] = "Lesotho";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "LBR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Liberia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "LBY";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Libyan Arab Jamahiriya";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "LIE";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Liechtenstein";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "LTU";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Lithuania";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "LUX";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Luxembourg";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MAC";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Macao Special Administrative Region of China";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MDG";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Madagascar";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MWI";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Malawi";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MYS";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Malaysia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MDV";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Maldives";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MLI";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Mali";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MLT";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Malta";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MHL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Marshall Islands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MTQ";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Martinique";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MRT";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Mauritania";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MUS";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Mauritius";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MYT";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Mayotte";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MEX";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Mexico";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MDA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Moldova";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MCO";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Monaco";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MNG";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Mongolia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MNE";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Montenegro";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MSR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Montserrat";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MAR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Morocco";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MOZ";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Mozambique";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MMR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Myanmar";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "NAM";
            DataRow["COUNTRY_CODE2"] = "NA";
            DataRow["COUNTRY_DESC"] = "Namibia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "NRU";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Nauru";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "NPL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Nepal";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "NLD";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Netherlands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ANT";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Netherlands Antilles";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "NCL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "New Caledonia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "NZL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "New Zealand";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "NIC";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Nicaragua";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "NER";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Niger";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "NGA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Nigeria";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "NIU";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Niue";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "NFK";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Norfolk Island";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MNP";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Northern Mariana Islands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "NOR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Norway";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "PSE";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Occupied Palestinian Territory";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "OMN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Oman";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "PAK";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Pakistan";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "PLW";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Palau";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "PAN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Panama";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "PNG";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Papua New Guinea";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "PRY";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Paraguay";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "PER";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Peru";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "PHL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Philippines";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "PCN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Pitcairn";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "POL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Poland";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "PRT";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Portugal";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "PRI";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Puerto Rico";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "QAT";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Qatar";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "KOR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Republic of Korea";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "REU";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Réunion";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ROU";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Romania";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "RUS";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Russian Federation";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "RWA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Rwanda";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SGS";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "S. Georgia and S. Sandwich Is.";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SHN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Saint Helena";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "KNA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Saint Kitts and Nevis";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "LCA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Saint Lucia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SPM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Saint Pierre and Miquelon";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "VCT";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Saint Vincent and the Grenadines";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SMR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "San Marino";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "STP";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Sao Tome and Principe";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SAU";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Saudi Arabia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SEN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Senegal";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SRB";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Serbia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SYC";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Seychelles";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SLE";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Sierra Leone";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SGP";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Singapore";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SVK";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Slovakia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SVN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Slovenia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SLB";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Solomon Islands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SOM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Somalia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ZAF";
            DataRow["COUNTRY_CODE2"] = "ZA";
            DataRow["COUNTRY_DESC"] = "South Africa";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ESP";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Spain";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "LKA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Sri Lanka";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SDN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Sudan";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SUR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Suriname";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SJM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Svalbard and Jan Mayen Islands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SWZ";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Swaziland";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SWE";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Sweden";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "CHE";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Switzerland";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "SYR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Syrian Arab Republic";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TWN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Taiwan, city of China";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TJK";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Tajikistan";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "THA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Thailand";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "MKD";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "The former Yugoslav Republic of Macedonia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TLS";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Timor-Leste";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TGO";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Togo";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TKL";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Tokelau";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TON";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Tonga";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TTO";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Trinidad and Tobago";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TUN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Tunisia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TUR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Turkey";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TKM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Turkmenistan";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TCA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Turks and Caicos Islands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TUV";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Tuvalu";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "UGA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Uganda";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "UKR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Ukraine";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ARE";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "United Arab Emirates";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "GBR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "United Kingdom of Great Britain and Northern Ireland";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "TZA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "United Republic of Tanzania";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "USA";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "United States of America";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "VIR";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "United States Virgin Islands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "URY";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Uruguay";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "UZB";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Uzbekistan";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "VUT";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Vanuatu";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "VEN";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Venezuela";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "VNM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Vietnam";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "WLF";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Wallis and Futuna Islands";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ESH";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Western Sahara";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "YEM";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Yemen";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ZMB";
            DataRow["COUNTRY_CODE2"] = "AR";
            DataRow["COUNTRY_DESC"] = "Zambia";

            DataTable.Rows.Add(DataRow);
            DataRow = DataTable.NewRow();

            DataRow["COUNTRY_CODE"] = "ZWE";
            DataRow["COUNTRY_CODE2"] = "ZW";
            DataRow["COUNTRY_DESC"] = "Zimbabwe";

            DataTable.Rows.Add(DataRow);

            return DataTable;
        }

        private void DynamicFunctionCompleted(object sender, localhost.DynamicFunctionCompletedEventArgs e)
        {
            try
            {
                pvtReturnObject = e.Result;
            }
            catch (TargetInvocationException ex)
            {
                if (myPanelGlobe != null)
                {
                    myPanelGlobe.Visible = false;
                    myPanelGlobe.Refresh();
                    Application.DoEvents();
                }

                if (ex.InnerException.Message.IndexOf("The page cannot be displayed") > -1
                    | ex.InnerException.Message.IndexOf("The request was canceled") > -1
                    | ex.InnerException.Message.IndexOf("Unable to connect to the remote server") > -1
                    | ex.InnerException.Message.IndexOf("Login.aspx") > -1
                    | ex.InnerException.Message.IndexOf("Server Unavailable") > -1)
                {
                    frmConnectionFailure frmConnectionFailure = new frmConnectionFailure();

                    frmConnectionFailure.ShowDialog();

                    pvtblnCommunicationError = true;
                }
                else
                {
                    if (ex.InnerException.Message.IndexOf("Runtime Error") > -1
                        | ex.InnerException.Message.IndexOf("connection timed out") > -1
                        | ex.InnerException.Message.IndexOf("Timeout") > -1)
                    {
                        frmTimeoutFailure frmTimeoutFailure = new frmTimeoutFailure();
                        frmTimeoutFailure.ShowDialog();

                        pvtblnCommunicationTimeOutError = true;
                    }
                    else
                    {
                        Show_Write_Errors(ex.ToString());
                    }
                }
            }
            finally
            {
                pvtblnCallBackComplete = true;
            }
        }
    }
}
