using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.Serialization;
using System.Reflection;
using System.Configuration;

namespace FingerPrintClockServer
{
    public partial class frmFingerPrintClockServer : Form
    {
        WebServiceHost FingerPrintClockServiceHost;
        TextMessageEncodingBindingElement TextMessageEncoder; 
        WebHttpBinding WebHttpBinding;

        FileInfo fiFileInfo;
        StreamReader srStreamReader;

        InteractPayroll.clsDBConnectionObjects clsDBConnectionObjects;
        
        //DB Sql Connection is Passed To Undelying Layers
        private string pvtstrConnection = @"Data Source=#Engine#;Initial Catalog=InteractPayrollClient;Integrated Security=true; MultipleActiveResultSets=True;";
        private string pvtstrIpAddress = "";
        private TextBox txtDateTime;
        private Label label4;
        private Label lblErrorMessage;
        private TextBox txtServiceName;
        private TextBox txtSqlName;
        private Label label2;
        private Button btnClose;
        private Label label1;
        private TextBox txtSoftware;
        private Label label5;
        private PictureBox picURU4000B;
        private PictureBox picURU4500;
        private IContainer components;
        private string pvtstrEndErrorMessage = "\n\nFix the Problem and Restart this Service.";

        public frmFingerPrintClockServer()
        {
            InitializeComponent();
        }

        private void frmFingerPrintClockServer_Load(object sender, EventArgs e)
        {
            try
            {
                //NB NB NB This Program Is Only Used for Testing = FingerPrintClockWindowsService.exe is the Release Program
                //NB NB NB This Program Is Only Used for Testing = FingerPrintClockWindowsService.exe is the Release Program
                //NB NB NB This Program Is Only Used for Testing = FingerPrintClockWindowsService.exe is the Release Program
                //NB NB NB This Program Is Only Used for Testing = FingerPrintClockWindowsService.exe is the Release Program

                //2013-10-07
                AppDomain.CurrentDomain.SetData("SoftwareToUse", "D");
              
                clsDBConnectionObjects = new InteractPayroll.clsDBConnectionObjects();

                string strApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
#if (DEBUG)
                //Put Here to Stop overwrite of New Compiled Programs is Debug Directory
                strApplicationPath = AppDomain.CurrentDomain.BaseDirectory + "bin\\";

                AppDomain.CurrentDomain.SetData("picURU4000B", this.picURU4000B);
                AppDomain.CurrentDomain.SetData("picURU4500", this.picURU4500);
#else
                this.Height = 262;    
                this.Width = 454;    
#endif
                bool blnDBConfigFound = false;
                string strDBEngine = System.Environment.MachineName + @"\SQLExpress";
                string strPortNo = "8000";

                AppDomain.CurrentDomain.SetData("DateTimeTextBox", this.txtDateTime);

                string[] strComponents = null;

                fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "DBConfig.txt");

                if (fiFileInfo.Exists == true)
                {
                    blnDBConfigFound = true;
                    srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "DBConfig.txt");
                    //strDBEngine = srStreamReader.ReadLine();

                    strComponents = srStreamReader.ReadLine().Split(';');

                    srStreamReader.Close();
                    srStreamReader.Dispose();
                }

                if (strComponents != null)
                {
                    if (strComponents.Length > 0)
                    {
                        strDBEngine = strComponents[0];
                    }

                    pvtstrConnection = pvtstrConnection.Replace("#Engine#", strDBEngine.Trim());

                    if (strComponents.Length > 1)
                    {
                        if (strComponents[1].IndexOf("AttachDBFilename") > -1)
                        {
                            //New AttachDBFilename
                            pvtstrConnection = pvtstrConnection + strComponents[1] + ";";
                        }
                    }
                }

                this.txtSqlName.Text = strDBEngine.Trim();
                //pvtstrConnection = pvtstrConnection.Replace("#Engine#", strDBEngine.Trim());

                AppDomain.CurrentDomain.SetData("SQLConnection", pvtstrConnection);

                string strLoggingSwitchedOn = "N";

                fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceServiceStartStop.exe.config");

                if (fiFileInfo.Exists == true)
                {
                    string line;
                    using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockTimeAttendanceServiceStartStop.exe.config"))
                    {
                        line = reader.ReadLine();

                        while (line != null)
                        {
                            line = reader.ReadLine();

                            if (line.IndexOf("LoggingSwitchedOn") > -1)
                            {
                                if (line.IndexOf("value=\"Y\"") > -1)
                                {
                                    strLoggingSwitchedOn = "Y";
                                }

                                break;
                            }
                        }
                    }
                }
                
                AppDomain.CurrentDomain.SetData("LoggingSwitchedOn", strLoggingSwitchedOn);

                fiFileInfo = null;
               
                int intReturnCode = Test_SQL_Connection();

                if (intReturnCode == 0)
                {
                    //NB FingerPrintClockService is LateBound Due to Download Mechanism
                    System.Reflection.Assembly pvtAssembly = System.Reflection.Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockService.dll");
                    Object FingerPrintClockService = pvtAssembly.CreateInstance("FingerPrintClockServer.FingerPrintClockService");
                                     
                    pvtstrIpAddress = @"http://localhost:" + strPortNo.Trim() + "/FingerPrintClockServer";

                    FingerPrintClockServiceHost = new WebServiceHost(FingerPrintClockService.GetType(), new Uri(pvtstrIpAddress));
                                
                    WebHttpBinding = new WebHttpBinding(); 

                    WebHttpBinding.MaxBufferPoolSize = 66665536;
                    WebHttpBinding.MaxBufferSize = 66665536;
                    WebHttpBinding.MaxReceivedMessageSize = 66665536;

                    WebHttpBinding.ReaderQuotas.MaxArrayLength = 100000000;

                    WebHttpBinding.ReaderQuotas.MaxStringContentLength = 66665536;
                    WebHttpBinding.ReaderQuotas.MaxBytesPerRead = 66665536;
                    WebHttpBinding.ReaderQuotas.MaxNameTableCharCount = 66665536;
                              
                    WebHttpBinding.TransferMode = TransferMode.Streamed;

                    FingerPrintClockServiceHost.AddServiceEndpoint(FingerPrintClockService.GetType().GetInterface("FingerPrintClockServer.IFingerPrintClockService"), WebHttpBinding, pvtstrIpAddress);
                                                            
                    FingerPrintClockServiceHost.Open();
                                                               
                    this.txtServiceName.Text = FingerPrintClockServiceHost.BaseAddresses[0].AbsoluteUri;
                }
                else
                {
                    if (intReturnCode == 9)
                    {
                        this.lblErrorMessage.Text = "Fingerprint Software NOT Setup";
                    }
                    else
                    {
                        if (blnDBConfigFound == false)
                        {
                            this.lblErrorMessage.Text = "File 'DBConfig.txt' Does NOT Exist.\n\nor Connection to SQL Server Database Failed." + pvtstrEndErrorMessage;
                        }
                        else
                        {
                            this.lblErrorMessage.Text = "Connection to SQL Server Database Failed" + pvtstrEndErrorMessage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string strExceptionError = "Date/Time  : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                   + "\r\n" + "Where      : " + ex.StackTrace
                   + "\r\n" + "Error Desc : " + ex.Message;

                FileInfo fiErrorFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "FingerPrintClockServerError.txt");

                StreamWriter swErrorStreamWriter = fiErrorFile.AppendText();

                swErrorStreamWriter.WriteLine("");
                swErrorStreamWriter.WriteLine(strExceptionError);

                swErrorStreamWriter.Close();

                MessageBox.Show(ex.Message);

            }
        }

        public int Test_SQL_Connection()
        {
            int intReturnCode = 0;
            string strQry = "";

            DataSet DataSet = new DataSet();

            try
            {
                strQry = "";
                strQry += " SELECT";
                strQry += " TABLE_NAME";
                strQry += " FROM InteractPayrollClient.INFORMATION_SCHEMA.TABLES";
                strQry += " WHERE TABLE_NAME = 'Errol'";

                clsDBConnectionObjects.Create_DataTable_Client(strQry,DataSet,"Temp");
            }
            catch (Exception ex)
            {
                intReturnCode = 1;
            }
            finally
            {
                DataSet.Dispose();
            }

            return intReturnCode;
        }

        private void frmFingerPrintClockServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                FingerPrintClockServiceHost.Close();
            }
            catch
            {
            }
        }
        
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFingerPrintClockServer));
            this.txtDateTime = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblErrorMessage = new System.Windows.Forms.Label();
            this.txtServiceName = new System.Windows.Forms.TextBox();
            this.txtSqlName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSoftware = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.picURU4000B = new System.Windows.Forms.PictureBox();
            this.picURU4500 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picURU4000B)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picURU4500)).BeginInit();
            this.SuspendLayout();
            // 
            // txtDateTime
            // 
            this.txtDateTime.Location = new System.Drawing.Point(14, 187);
            this.txtDateTime.Name = "txtDateTime";
            this.txtDateTime.ReadOnly = true;
            this.txtDateTime.Size = new System.Drawing.Size(132, 20);
            this.txtDateTime.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 172);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Last Message DateTime";
            // 
            // lblErrorMessage
            // 
            this.lblErrorMessage.ForeColor = System.Drawing.Color.Red;
            this.lblErrorMessage.Location = new System.Drawing.Point(120, 66);
            this.lblErrorMessage.Name = "lblErrorMessage";
            this.lblErrorMessage.Size = new System.Drawing.Size(313, 52);
            this.lblErrorMessage.TabIndex = 18;
            this.lblErrorMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtServiceName
            // 
            this.txtServiceName.Location = new System.Drawing.Point(13, 134);
            this.txtServiceName.Name = "txtServiceName";
            this.txtServiceName.ReadOnly = true;
            this.txtServiceName.Size = new System.Drawing.Size(420, 20);
            this.txtServiceName.TabIndex = 17;
            // 
            // txtSqlName
            // 
            this.txtSqlName.Location = new System.Drawing.Point(15, 34);
            this.txtSqlName.Name = "txtSqlName";
            this.txtSqlName.ReadOnly = true;
            this.txtSqlName.Size = new System.Drawing.Size(418, 20);
            this.txtSqlName.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(212, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "SQL Server Database Engine Server Name";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(347, 175);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 32);
            this.btnClose.TabIndex = 12;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Web Server Service Name";
            // 
            // txtSoftware
            // 
            this.txtSoftware.Location = new System.Drawing.Point(14, 83);
            this.txtSoftware.Name = "txtSoftware";
            this.txtSoftware.ReadOnly = true;
            this.txtSoftware.Size = new System.Drawing.Size(100, 20);
            this.txtSoftware.TabIndex = 22;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "FingerPrint Software";
            // 
            // picURU4000B
            // 
            this.picURU4000B.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picURU4000B.Location = new System.Drawing.Point(15, 227);
            this.picURU4000B.Name = "picURU4000B";
            this.picURU4000B.Size = new System.Drawing.Size(500, 550);
            this.picURU4000B.TabIndex = 23;
            this.picURU4000B.TabStop = false;
            // 
            // picURU4500
            // 
            this.picURU4500.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picURU4500.Location = new System.Drawing.Point(532, 227);
            this.picURU4500.Name = "picURU4500";
            this.picURU4500.Size = new System.Drawing.Size(500, 550);
            this.picURU4500.TabIndex = 24;
            this.picURU4500.TabStop = false;
            // 
            // frmFingerPrintClockServer
            // 
            this.ClientSize = new System.Drawing.Size(1095, 789);
            this.Controls.Add(this.picURU4500);
            this.Controls.Add(this.picURU4000B);
            this.Controls.Add(this.txtSoftware);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtDateTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblErrorMessage);
            this.Controls.Add(this.txtServiceName);
            this.Controls.Add(this.txtSqlName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmFingerPrintClockServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Interact\'s FingerPrint Web Server";
            this.Load += new System.EventHandler(this.frmFingerPrintClockServer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picURU4000B)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picURU4500)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
