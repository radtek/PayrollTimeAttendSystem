using System;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Xml;

namespace InteractPayrollClient
{
	public class clsISClientUtilities
	{
        System.Windows.Forms.PaintEventArgs myPaintEventArgs;

        Assembly asAssembly;
        System.Type typObjectType;
        object busDynamicService;

        private bool pvtblnCloseFormSent = false;

        private object pvtReturnObject;
        private string pvtstrBusinessObjectName = "";
      
        public DataSet pvtDataSet;
        public DataView pvtDataView;

        public bool pubblnErrorHasBeenHandled = false;
       
        Form myCallingForm;
        Panel pnlInternetGlobe;
       
        HttpClient HttpClient;
        public Uri UriHeader;
        Uri UriRequest;
        System.ServiceModel.Channels.Message respondMessage;

        private bool _dragging = false;
        private Point _start_point = new Point(0, 0);

        public clsISClientUtilities()
        {
        }

        public clsISClientUtilities(Form CallingForm,string BusinessObjectName)
		{
            if (CallingForm != null)
            {
                myCallingForm = CallingForm;
            }

            pvtstrBusinessObjectName = BusinessObjectName;
#if(DEBUG)

            if (myCallingForm != null)
            {
                int intWidth = 0;
                int intHeight = 0;
                int intNewHeight = 0;

                DataGridView myDataGridView;

                foreach (Control myControl in CallingForm.Controls)
                {
                    if (myControl is DataGridView)
                    {
                        myDataGridView = null;
                        myDataGridView = (DataGridView)myControl;

                        if (myDataGridView.RowHeadersVisible == true)
                        {
                            intWidth = myDataGridView.RowHeadersWidth;
                        }
                        else
                        {
                            intWidth = 0;
                        }

                        if (myDataGridView.ScrollBars == ScrollBars.Vertical
                        || myDataGridView.ScrollBars == ScrollBars.Both)
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
                                        || myDataGridView.ScrollBars == ScrollBars.Both)
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

                                                if (myDataGridView.RowHeadersVisible == true)
                                                {
                                                    intWidth = myDataGridView.RowHeadersWidth;
                                                }
                                                else
                                                {
                                                    intWidth = 0;
                                                }

                                                if (myDataGridView.ScrollBars == ScrollBars.Vertical
                                                    || myDataGridView.ScrollBars == ScrollBars.Both)
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
                        }
                    }
                }
            }
         
            if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() == "")
            {
                asAssembly = Assembly.LoadFrom(BusinessObjectName + ".dll");
                typObjectType = asAssembly.GetType("InteractPayrollClient." + BusinessObjectName);
                busDynamicService = Activator.CreateInstance(typObjectType);
            }
            else
            {
                UriHeader = new Uri("http://" + AppDomain.CurrentDomain.GetData("URLClientPath").ToString() + "/FingerPrintClockServer/");

                HttpClient = new HttpClient(UriHeader, true);
            }
#else
            //2013-01-24
            UriHeader = new Uri("http://" + AppDomain.CurrentDomain.GetData("URLClientPath").ToString()  + "/FingerPrintClockServer/");

            HttpClient = new HttpClient(UriHeader, true);
#endif
        }

        public int WebService_Ping_Test()
        {
            int intReturnCode = 1;

            string strReturnCode = (string)DynamicFunction("Ping", null,false);

            if (strReturnCode == "OK")
            {
                intReturnCode = 0;
            }

            return intReturnCode;
        }

        public int WebService_Settings(System.Web.Services.Protocols.WebClientProtocol ws, string parWebServiceName)
        {
            try
            {
                ws.Url = AppDomain.CurrentDomain.GetData("URLClientPath").ToString() + "/" + parWebServiceName + ".asmx";
                ws.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }
            catch
            {
                if (myCallingForm != null)
                {
                    System.Windows.Forms.MessageBox.Show("Web Service '" + parWebServiceName + "' URL Path '" + AppDomain.CurrentDomain.GetData("URLClientPath").ToString() + "' is Invalid.\n\nContact System Administrator.",
                        "Web Service Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                if (AppDomain.CurrentDomain.GetData("TimerCloseCurrentForm") != null)
                {
                    if (pvtblnCloseFormSent == false)
                    {
                        if (myCallingForm != null)
                        {
                            pvtblnCloseFormSent = true;

                            AppDomain.CurrentDomain.SetData("FormToClose", myCallingForm);

                            Timer tmrCloseCurrentForm = (Timer)AppDomain.CurrentDomain.GetData("TimerCloseCurrentForm");
                            tmrCloseCurrentForm.Enabled = true;
                        }
                    }
                }
                return -1;
            }

            return 0;
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
            pvtDataSet = new DataSet();
            pvtDataSet.RemotingFormat = SerializationFormat.Binary;

            MemoryStream msMemoryStreamCompressed = new MemoryStream(parbytArray);
            System.IO.Compression.GZipStream GZipStreamCompressed = new GZipStream(msMemoryStreamCompressed, CompressionMode.Decompress, true);

            byte[] byteDecompressed = ReadFullStream(GZipStreamCompressed);
            GZipStreamCompressed.Flush();
            GZipStreamCompressed.Close();

            MemoryStream msMemoryStreamDecompressed = new MemoryStream(byteDecompressed);

            BinaryFormatter bf = new BinaryFormatter();
            pvtDataSet = (DataSet)bf.Deserialize(msMemoryStreamDecompressed, null);

            return pvtDataSet;
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

        public void lblHeader_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _start_point = new Point(e.X, e.Y);
        }

        public void lblHeader_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        public void Form_Paint(object sender, PaintEventArgs e)
        {
            Form frm = (Form)sender;
            Rectangle myRectangle = new Rectangle(frm.ClientRectangle.X - 2, frm.ClientRectangle.Y - 2, frm.ClientRectangle.Width - 2, frm.ClientRectangle.Height - 2);

            ControlPaint.DrawBorder(e.Graphics, frm.ClientRectangle,
            System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
            System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
            System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid,
            System.Drawing.Color.Black, 1, ButtonBorderStyle.Solid);

            Pen blackPen = new Pen(Color.Black, 1);
            e.Graphics.DrawLine(blackPen, 0, 31, frm.Width, 31);
        }

        public void lblHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Form myForm = (Form)((Label)sender).Parent;
                Label lblHeader = (Label)sender;

                //Cursor Position relative to lblHeader (On Screen)
                Point p = myForm.PointToScreen(new Point(Cursor.Position.X - myForm.PointToScreen(lblHeader.Location).X, Cursor.Position.Y - myForm.PointToScreen(lblHeader.Location).Y));

                myForm.Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);
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

        public object DynamicFunction(string FunctionName, object[] objParm,bool ShowUpdateForm)
        {
            try
            {
                pvtReturnObject = null;
#if(DEBUG)
                if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() == "")
                {
                    MethodInfo mi = typObjectType.GetMethod(FunctionName);
                    pvtReturnObject = mi.Invoke(busDynamicService, objParm);

                    goto DynamicFunction_Continue;
                }
#endif
                if (AppDomain.CurrentDomain.GetData("InternetGlobe") != null)
                {
                    if (pnlInternetGlobe == null)
                    {
                        pnlInternetGlobe = (Panel)AppDomain.CurrentDomain.GetData("InternetGlobe");
                    }

                    pnlInternetGlobe.Visible = true;
                    pnlInternetGlobe.Refresh();
                }

                if (myCallingForm != null)
                {
                    myCallingForm.Cursor = Cursors.WaitCursor;
                }

                DateTime dtTimeWait = DateTime.Now.AddMilliseconds(500);

                if (objParm != null)
                {
                    UriRequest = new Uri(UriHeader, "DynamicFunction/" + pvtstrBusinessObjectName + "/" + FunctionName);

                    ObjectParameters myObjectParameters = new ObjectParameters();

                    MemoryStream stmStreamObjects = new MemoryStream();

                    BinaryFormatter bFormatter = new BinaryFormatter();
                    bFormatter.Serialize(stmStreamObjects, objParm);

                    stmStreamObjects.Position = 0;

                    byte[] byteParametersAsBytes = new byte[stmStreamObjects.Length];

                    stmStreamObjects.Read(byteParametersAsBytes, 0, byteParametersAsBytes.Length);

                    myObjectParameters.bytParameter = byteParametersAsBytes;

                    respondMessage = null;
                    respondMessage = HttpClient.Post(UriRequest, myObjectParameters);
                }
                else
                {
                    UriRequest = new Uri(UriHeader, "DynamicProcedure/" + pvtstrBusinessObjectName + "/" + FunctionName);

                    respondMessage = null;
                    respondMessage = HttpClient.Get(UriRequest);
                }

                while (DateTime.Now < dtTimeWait)
                {
                    System.Threading.Thread.Sleep(50);
                }

                if (pnlInternetGlobe != null)
                {
                    pnlInternetGlobe.Visible = false;
                    pnlInternetGlobe.Refresh();
                    Application.DoEvents();
                }

                if (HttpClient.blnConnectionFailure == true
                || HttpClient.blnTimeoutFailure == true
                || HttpClient.blnOtherFailure == true)
                {
                    if (HttpClient.blnConnectionFailure == true)
                    {
                        if (myCallingForm != null)
                        {
                            if (myCallingForm.Name == "frmSplashScreen"
                            && FunctionName == "Logon_Client_DataBase")
                            {
                            }
                            else
                            {
                                frmConnectionFailure frmConnectionFailure = new frmConnectionFailure();
                                frmConnectionFailure.ShowDialog();
                            }
                        }

                        pubblnErrorHasBeenHandled = true;
                    }
                    else
                    {
                        if (HttpClient.blnTimeoutFailure == true)
                        {
                            if (myCallingForm != null)
                            {
                                if (pvtstrBusinessObjectName == "busClientPayrollLogon"
                                && FunctionName == "Logon_Client_DataBase")
                                {
                                }
                                else
                                {

                                    frmTimeOutFailure frmTimeOutFailure = new frmTimeOutFailure();
                                    frmTimeOutFailure.ShowDialog();
                                }
                            }

                            pubblnErrorHasBeenHandled = true;
                        }
                        else
                        {
                            if (myCallingForm != null)
                            {
                                System.Windows.Forms.MessageBox.Show("Bad Request.\n\nSpeak To System Administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            pubblnErrorHasBeenHandled = true;
                        }
                    }

                    //Errol To Check
                    if (myCallingForm != null)
                    {
                        if (HttpClient.blnConnectionFailure == true)
                        {
                            throw new System.ArgumentException("Communication Error", "Communication");
                        }
                        else
                        {
                            if (HttpClient.blnTimeoutFailure == true)
                            {
                                throw new System.ArgumentException("Timeout Error", "Communication");
                            }
                            else
                            {
                                throw new System.ArgumentException("Bad Request", "Communication");
                            }
                        }
                    }
                }
                else
                {
                    if (ShowUpdateForm == true)
                    {
                        frmDatabaseUpdated frmDatabaseUpdated = new InteractPayrollClient.frmDatabaseUpdated();
                        frmDatabaseUpdated.Show();
                        clsFadeForm.FadeForm(frmDatabaseUpdated);
                        frmDatabaseUpdated.Close();
                    }

                    ReturnObject myReturnObject = respondMessage.GetBody<ReturnObject>();

                    pvtReturnObject = myReturnObject.obj;
                }

                if (myCallingForm != null)
                {
                    myCallingForm.Cursor = Cursors.Default;
                }

                return pvtReturnObject;
            }
            catch (Exception ex)
            {
                FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "ShowError.txt");

                if (fiFileInfo.Exists == true)
                {
                    string strError = ex.ToString();

                    System.Windows.Forms.MessageBox.Show("clsISClientUtilities Call " + pvtstrBusinessObjectName + "/" + FunctionName + " " + strError);
                }

                if (myCallingForm != null)
                {
                    myCallingForm.Cursor = Cursors.Default;
                }

                if (pnlInternetGlobe != null)
                {
                    pnlInternetGlobe.Visible = false;
                    pnlInternetGlobe.Refresh();
                }

                if (ex.Message.IndexOf("Communication Error") > -1
                    || ex.Message.IndexOf("Timeout Error") > -1
                     || ex.Message.IndexOf("Bad Request") > -1)
                {
                    throw ex;
                }
                else
                {
                    if (ex.Message.IndexOf("has exceeded the allotted timeout") > -1)
                    {
                        if (myCallingForm != null)
                        {
                            if (pvtstrBusinessObjectName == "busClientPayrollLogon"
                            && FunctionName == "Logon_Client_DataBase")
                            {
                            }
                            else
                            {
                                frmTimeOutFailure frmTimeOutFailure = new frmTimeOutFailure();
                                frmTimeOutFailure.ShowDialog();
                            }
                        }

                        pubblnErrorHasBeenHandled = true;

                        //Errol To Check
                        if (myCallingForm != null)
                        {
                            throw new System.ArgumentException("Timeout Error", "Communication");
                        }
                    }
                    else
                    {
                        if (ex.Message.IndexOf("was no endpoint listening") > -1)
                        {
                            if (myCallingForm != null)
                            {
                                frmConnectionFailure frmConnectionFailure = new frmConnectionFailure();
                                frmConnectionFailure.ShowDialog();
                            }

                            pubblnErrorHasBeenHandled = true;

                            //Errol To Check
                            if (myCallingForm != null)
                            {
                                throw new System.ArgumentException("Communication Error", "Communication");
                            }
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Bad Request for " + pvtstrBusinessObjectName + " " + FunctionName + ".\n\nSpeak To System Administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            string strExceptionError = "";
                            string strAreaFrom = pvtstrBusinessObjectName + " " + FunctionName;

                            String strDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                            strExceptionError = "Date/Time  : " + strDateTime
                            + "\r\n" + "Error Desc : " + ex.ToString();
                                
                            strExceptionError += "\n\nSpeak to System Administrator";

                            string strFileName = "";

                            //P = Payroll
                            //T = Time Attendance Client
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

                            FileInfo fiErrorFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + strFileName + ".txt");

                            StreamWriter swErrorStreamWriter = fiErrorFile.AppendText();

                            swErrorStreamWriter.WriteLine("");
                            swErrorStreamWriter.WriteLine(strExceptionError);

                            swErrorStreamWriter.Close();
                           
                            pubblnErrorHasBeenHandled = true;
                        }
                    }
                }
            }
            
            DynamicFunction_Continue:

            if (myCallingForm != null)
            {
                myCallingForm.Cursor = Cursors.Default;
            }
      
            return pvtReturnObject;
        }

        public void Set_New_BusinessObjectName(string BusinessObjectName)
        {
            pvtstrBusinessObjectName = BusinessObjectName;

            if (AppDomain.CurrentDomain.GetData("URLClientPath").ToString() == "")
            {
                if (asAssembly != null)
                {
                    asAssembly = null;
                }

                asAssembly = Assembly.LoadFrom(BusinessObjectName + ".dll");
                typObjectType = asAssembly.GetType("InteractPayrollClient." + BusinessObjectName);
                busDynamicService = Activator.CreateInstance(typObjectType);
            }
        }

        public string Get_BusinessObjectName()
        {
            return pvtstrBusinessObjectName;
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
                            if (parintTotal % parintRoundValue >= Convert.ToDouble(parintRoundValue) / 2)
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

        public static int Subtract_Two_Time_Fields(int parintFirstField, int parintSecondField)
        {
            int intFirstFieldMM;
            int intFirstFieldHH;
            int intSecondFieldMM;
            int intSecondFieldHH;

            if (parintFirstField < 0
                || parintSecondField < 0)
            {
                int intAddValue = Add_Two_Time_Fields(parintFirstField, parintSecondField);
                return intAddValue;
            }

            if (parintFirstField.ToString().Length >= 3)
            {
                intFirstFieldMM = Convert.ToInt32(parintFirstField.ToString().Substring(parintFirstField.ToString().Length - 2, 2));
                intFirstFieldHH = Convert.ToInt32(parintFirstField.ToString().Substring(0, parintFirstField.ToString().Length - 2));
            }
            else
            {
                intFirstFieldMM = Convert.ToInt32(parintFirstField.ToString());
                intFirstFieldHH = 0;
            }

            if (parintSecondField.ToString().Length >= 3)
            {
                intSecondFieldMM = Convert.ToInt32(parintSecondField.ToString().Substring(parintSecondField.ToString().Length - 2, 2));
                intSecondFieldHH = Convert.ToInt32(parintSecondField.ToString().Substring(0, parintSecondField.ToString().Length - 2));
            }
            else
            {
                intSecondFieldMM = Convert.ToInt32(parintSecondField.ToString());
                intSecondFieldHH = 0;
            }

            if (intFirstFieldMM < intSecondFieldMM)
            {
                intFirstFieldMM = intFirstFieldMM + 60;
                intFirstFieldHH = intFirstFieldHH - 1;
            }

            intFirstFieldMM = intFirstFieldMM - intSecondFieldMM;
            intFirstFieldHH = intFirstFieldHH - intSecondFieldHH;

            int intReturnValue = Convert.ToInt32(intFirstFieldHH.ToString() + intFirstFieldMM.ToString("00"));

            return intReturnValue;
        }

        public static int Add_Two_Time_Fields(int parintFirstField, int parintSecondField)
        {
            if (parintFirstField < 0
                || parintSecondField < 0)
            {
                if (parintFirstField < 0
                && parintSecondField < 0)
                {
                    parintFirstField = parintFirstField * -1;
                    parintSecondField = parintSecondField * -1;

                    int intFinalReturnValue = Add_Two_Time_Fields(parintFirstField, parintSecondField);

                    return intFinalReturnValue * -1;
                }
                else
                {
                    int intSubtractValue = 0;

                    if (parintFirstField > parintSecondField)
                    {
                        parintSecondField *= -1;

                        if (parintFirstField > parintSecondField)
                        {
                            intSubtractValue = Subtract_Two_Time_Fields(parintFirstField, parintSecondField);
                        }
                        else
                        {
                            intSubtractValue = Subtract_Two_Time_Fields(parintSecondField, parintFirstField);
                            intSubtractValue = intSubtractValue * -1;
                        }
                    }
                    else
                    {
                        parintFirstField *= -1;

                        if (parintFirstField > parintSecondField)
                        {
                            intSubtractValue = Subtract_Two_Time_Fields(parintFirstField, parintSecondField);
                            intSubtractValue = intSubtractValue * -1;
                        }
                        else
                        {
                            intSubtractValue = Subtract_Two_Time_Fields(parintSecondField, parintFirstField);
                        }
                    }

                    return intSubtractValue;
                }
            }

            int intFirstFieldMM;
            int intFirstFieldHH;
            int intSecondFieldMM;
            int intSecondFieldHH;

            if (parintFirstField.ToString().Length >= 3)
            {
                intFirstFieldMM = Convert.ToInt32(parintFirstField.ToString().Substring(parintFirstField.ToString().Length - 2, 2));
                intFirstFieldHH = Convert.ToInt32(parintFirstField.ToString().Substring(0, parintFirstField.ToString().Length - 2));
            }
            else
            {
                intFirstFieldMM = Convert.ToInt32(parintFirstField.ToString());
                intFirstFieldHH = 0;
            }

            if (parintSecondField.ToString().Length >= 3)
            {
                intSecondFieldMM = Convert.ToInt32(parintSecondField.ToString().Substring(parintSecondField.ToString().Length - 2, 2));
                intSecondFieldHH = Convert.ToInt32(parintSecondField.ToString().Substring(0, parintSecondField.ToString().Length - 2));
            }
            else
            {
                intSecondFieldMM = Convert.ToInt32(parintSecondField.ToString());
                intSecondFieldHH = 0;
            }

            intFirstFieldMM = intFirstFieldMM + intSecondFieldMM;
            intFirstFieldHH = intFirstFieldHH + intSecondFieldHH;

            if (intFirstFieldMM > 59)
            {
                intFirstFieldMM = intFirstFieldMM - 60;
                intFirstFieldHH = intFirstFieldHH + 1;
            }

            int intReturnValue = Convert.ToInt32(intFirstFieldHH.ToString() + intFirstFieldMM.ToString("00"));

            return intReturnValue;
        }
        
		public void ErrorHandler(Exception parException)
		{
            if (pnlInternetGlobe != null)
            {
                pnlInternetGlobe.Visible = false;
            }

            if (pubblnErrorHasBeenHandled == true)
            {
                //Web Server Error
            }
            else
            {
                string strExceptionError = "";
                string strAreaFrom = "";
                String strDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                //strAreaFrom = parException.StackTrace;

                strExceptionError = "Date/Time  : " + strDateTime
                    + "\r\n" + "Where      : " + strAreaFrom
                    + "\r\n" + "Error Desc : " + parException.Message;


                string strFileName = "";

                //P = Payroll
                //T = Time Attendance Client
                if (AppDomain.CurrentDomain.GetData("FromProgramInd") == null)
                {
                    strFileName = "TimeAttendanceClientIS_Error";
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

                FileInfo fiErrorFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + strFileName + ".txt");

                StreamWriter swErrorStreamWriter = fiErrorFile.AppendText();

                swErrorStreamWriter.WriteLine("");
                swErrorStreamWriter.WriteLine(strExceptionError);

                swErrorStreamWriter.Close();

                if (myCallingForm != null)
                {
                    System.Windows.Forms.MessageBox.Show(strExceptionError,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
            }

            if (AppDomain.CurrentDomain.GetData("TimerCloseCurrentForm") != null)
            {
                if (pvtblnCloseFormSent == false)
                {
                    if (myCallingForm != null)
                    {
                        pvtblnCloseFormSent = true;

                        AppDomain.CurrentDomain.SetData("FormToClose", myCallingForm);

                        Timer tmrCloseCurrentForm = (Timer)AppDomain.CurrentDomain.GetData("TimerCloseCurrentForm");
                        tmrCloseCurrentForm.Enabled = true;
                    }
                }
            }
		}
    }
}