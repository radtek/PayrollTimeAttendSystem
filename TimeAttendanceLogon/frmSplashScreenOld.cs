using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using System.Reflection;

namespace InteractPayrollClient
{
	public class frmSplashScreenOld : System.Windows.Forms.Form
	{
        clsTAUtilities clsTAUtilities;
		
		Int64 pvtint64UserNo = 0;
		string pvtstrAccessInd = "S";
		
		private DataSet pvtDataSet;
        private DataSet pvtTempDataSet;
        
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUserId;
		private System.Windows.Forms.PictureBox pibMainPicture;
        private Label label2;
        private Label lblUserEmployee;
        private Label lblVersion;
        private Button btnCancel;
        private Button btnOK;
		
		private System.ComponentModel.IContainer components;

		public frmSplashScreenOld()
		{
			InitializeComponent();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
	
		private void InitializeComponent()
		{
            this.txtUserId = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.pibMainPicture = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblUserEmployee = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pibMainPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // txtUserId
            // 
            this.txtUserId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserId.Enabled = false;
            this.txtUserId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserId.Location = new System.Drawing.Point(384, 385);
            this.txtUserId.MaxLength = 15;
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new System.Drawing.Size(112, 20);
            this.txtUserId.TabIndex = 1;
            // 
            // txtPassword
            // 
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.Enabled = false;
            this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(384, 424);
            this.txtPassword.MaxLength = 15;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(112, 20);
            this.txtPassword.TabIndex = 0;
            // 
            // pibMainPicture
            // 
            this.pibMainPicture.BackColor = System.Drawing.SystemColors.Control;
            this.pibMainPicture.Image = global::InteractPayrollClient.Properties.Resources.TimeAttendanceSplashScreen;
            this.pibMainPicture.Location = new System.Drawing.Point(-1, -1);
            this.pibMainPicture.Name = "pibMainPicture";
            this.pibMainPicture.Size = new System.Drawing.Size(705, 460);
            this.pibMainPicture.TabIndex = 14;
            this.pibMainPicture.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(290, 424);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 16);
            this.label2.TabIndex = 30;
            this.label2.Text = "Password";
            // 
            // lblUserEmployee
            // 
            this.lblUserEmployee.AutoSize = true;
            this.lblUserEmployee.BackColor = System.Drawing.Color.White;
            this.lblUserEmployee.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserEmployee.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.lblUserEmployee.Location = new System.Drawing.Point(290, 389);
            this.lblUserEmployee.Name = "lblUserEmployee";
            this.lblUserEmployee.Size = new System.Drawing.Size(37, 16);
            this.lblUserEmployee.TabIndex = 29;
            this.lblUserEmployee.Text = "User";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.White;
            this.lblVersion.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.lblVersion.Location = new System.Drawing.Point(613, 434);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(81, 13);
            this.lblVersion.TabIndex = 33;
            this.lblVersion.Text = "Version 2.3.4.5";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnCancel.Location = new System.Drawing.Point(622, 405);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(69, 24);
            this.btnCancel.TabIndex = 32;
            this.btnCancel.TabStop = false;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.White;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnOK.Location = new System.Drawing.Point(622, 377);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(69, 24);
            this.btnOK.TabIndex = 31;
            this.btnOK.TabStop = false;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmSplashScreen
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(702, 456);
            this.ControlBox = false;
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblUserEmployee);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUserId);
            this.Controls.Add(this.pibMainPicture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmSplashScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmSplashScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pibMainPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public void frmSplashScreen_Load(object sender, System.EventArgs e)
		{
			try
			{
                this.Show();
				this.Refresh();
				Application.DoEvents();

                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                lblVersion.Text = "Version " + fvi.ProductVersion;

				this.Cursor = Cursors.AppStarting;

				clsTAUtilities = new clsTAUtilities("busTimeAttendanceLogon");
				
				pvtDataSet = new DataSet();
				
#if(DEBUG)
{
                this.txtUserId.Text = "Interact";
                this.txtPassword.Text = "tcaretni";
                //this.txtUserId.Text = "TyroneS";
                //this.txtPassword.Text = "Test";
}
#endif
				
				this.txtUserId.Enabled = true;
				this.txtPassword.Enabled = true;

				this.btnOK.Enabled = true;

				this.txtUserId.Focus();
			
				this.Cursor = Cursors.Default;
			}
			catch (Exception eException)
			{
				clsTAUtilities.ErrorHandler(eException);
				this.Close();
			}
		}
		
		public void Load_SplashScreen(string parstring)
		{
			this.ShowDialog();
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
            try
            {
                if (this.txtUserId.Text.Trim() == "")
                {
                    MessageBox.Show("Enter User Id.", this.Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    this.txtUserId.Focus();
                    return;
                }
                else
                {
                    if (this.txtPassword.Text.Trim() == "")
                    {
                        MessageBox.Show("Enter Password.", this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        this.txtPassword.Focus();
                        return;
                    }
                }

                string strUserInformation = this.txtUserId.Text.Trim().ToUpper() + "|" + this.txtPassword.Text.Trim().ToUpper(); 
                
                object[] objParm = new object[1];
                objParm[0] = strUserInformation;
                
                byte[] bytCompress = (byte[])clsTAUtilities.DynamicFunction("Logon_User", objParm);

                pvtDataSet = clsTAUtilities.DeCompress_Array_To_DataSet(bytCompress);
               
                if (pvtDataSet.Tables["ReturnValues"].Rows[0]["USER_NO"].ToString() == "-1")
                {
                    //Not Found
                    MessageBox.Show("User Id / Password NOT Found.", this.Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                }
                else
                {
                    if (pvtDataSet.Tables["ReturnValues"].Rows[0]["RESET"].ToString() == "Y")
                    {
                        //Not Found
                        MessageBox.Show("User Needs To Logon To Interact Payroll (Internet) to Change Password.\nAn Administrator Needs to Download so that the User's Credentials are Passed down to the Client Database.'", this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        return;
                    }

                    pvtint64UserNo = Convert.ToInt64(pvtDataSet.Tables["ReturnValues"].Rows[0]["USER_NO"]);

                    pvtstrAccessInd = pvtDataSet.Tables["ReturnValues"].Rows[0]["ACCESS_IND"].ToString();

                    if (pvtDataSet.Tables["Trigger"].Rows.Count == 0)
                    {
                        MessageBox.Show("Trigger 'tgr_Create_Timesheets' Does NOT Exist.", this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    }
                }
#if(DEBUG)
#else
                FileInfo fiFileInfo;
                BinaryReader brBinaryReader;
                FileStream fsFileStream;
                BinaryWriter bwBinaryWriter;

                long lngFileStartOffset = 0;
                byte[] bytFileBytes;
                byte[] bytFileChunkBytes;
                byte[] bytDecompressedBytes;
                byte[] bytTempBytes;
                string strDownLoadFileName = "";
                bool blnRestartProgram = false;
                
                if (pvtDataSet.Tables["Files"].Rows.Count > 0)
                {
                    for (int intRow = 0;intRow < pvtDataSet.Tables["Files"].Rows.Count;intRow++)
			        {
                        fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + pvtDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString());
					
                        if (fiFileInfo.Exists == true)
                        {
                            if (fiFileInfo.LastWriteTime >= Convert.ToDateTime(pvtDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(-3)
						    & fiFileInfo.LastWriteTime <= Convert.ToDateTime(pvtDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]).AddSeconds(3))
					        {
                                continue;
                            }
                        }

                        strDownLoadFileName = pvtDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();

                        if (strDownLoadFileName == "TAUtilities.dll")
                        {
                            strDownLoadFileName = "TAUtilities.dll_";
                            blnRestartProgram = true;
                        }
                        else
                        {
                            if (strDownLoadFileName == "TimeAttendanceLogon.dll")
                            {
                                strDownLoadFileName = "TimeAttendanceLogon.dll_";
                                blnRestartProgram = true;
                            }
                        }

                        bytFileBytes = new byte[Convert.ToInt32(pvtDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE_COMPRESSED"])];
                        lngFileStartOffset = 0;
                        
                        for (int intChunkRow = 1;intChunkRow <= Convert.ToInt32(pvtDataSet.Tables["Files"].Rows[intRow]["MAX_FILE_CHUNK_NO"]);intChunkRow++)
						{
                            objParm = new object[2];
                            objParm[0] = pvtDataSet.Tables["Files"].Rows[intRow]["FILE_NAME"].ToString();
                            objParm[1] = intChunkRow;

                            bytTempBytes = (byte[])clsTAUtilities.DynamicFunction("Get_File_Chunk", objParm);

                            this.pvtTempDataSet = clsTAUtilities.DeCompress_Array_To_DataSet(bytTempBytes);

                            bytFileChunkBytes = (byte[])pvtTempDataSet.Tables["FileChunk"].Rows[0]["FILE_CHUNK"];
                           
							Array.Copy(bytFileChunkBytes,0,bytFileBytes,lngFileStartOffset,bytFileChunkBytes.Length);
                            lngFileStartOffset += bytFileChunkBytes.Length;
						}

                        bytDecompressedBytes = null;
                        bytDecompressedBytes = new byte[Convert.ToInt32(pvtDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE"])];

                        //Open Memory Stream with Compressed Data
                        MemoryStream msMemoryStream = new MemoryStream(bytFileBytes);

                        System.IO.Compression.GZipStream GZipStreamDecompress = new GZipStream(msMemoryStream, CompressionMode.Decompress);

                        //Decompress Bytes
                        brBinaryReader = new BinaryReader(GZipStreamDecompress);
                        bytDecompressedBytes = brBinaryReader.ReadBytes(Convert.ToInt32(pvtDataSet.Tables["Files"].Rows[intRow]["FILE_SIZE"]));

                        fsFileStream = null;
                        bwBinaryWriter = null;

                        fsFileStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + strDownLoadFileName, FileMode.Create);
                        bwBinaryWriter = new BinaryWriter(fsFileStream);

                        bwBinaryWriter.Write(bytDecompressedBytes);

                        //Write Memory Portion To Disk
                        bwBinaryWriter.Close();

                        File.SetLastWriteTime(strDownLoadFileName, Convert.ToDateTime(pvtDataSet.Tables["Files"].Rows[intRow]["FILE_LAST_UPDATED_DATE"]));
                    }
                }

                pvtDataSet.Tables.Remove(pvtDataSet.Tables["Files"]);

                if (blnRestartProgram == true)
                {
                    MessageBox.Show("Changes Have been Made to the Main Program.\nYou need to Restart the Program.",
                       "Program Changes",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Exclamation);
                    this.Close();
                }
#endif
                Close_SplasScreen_And_Show_Main();
            }
            catch (Exception eException)
            {
                clsTAUtilities.ErrorHandler(eException);
                this.Close();
            }
		}

		// Close the splash window and show the main window
		private void Close_SplasScreen_And_Show_Main()
		{
			try
			{
				//SET ALL GLOBAL VARIABLES HERE
				AppDomain.CurrentDomain.SetData("UserNo",pvtint64UserNo);
				AppDomain.CurrentDomain.SetData("AccessInd",pvtstrAccessInd);
			
				AppDomain.CurrentDomain.SetData("CurrentForm","");
				AppDomain.CurrentDomain.SetData("DataSet",this.pvtDataSet);

				try
				{
					string strPath = "";
					string strBasePath = "";
					
					strPath = AppDomain.CurrentDomain.BaseDirectory + "TimeAttendanceMain.dll";
					strBasePath = AppDomain.CurrentDomain.BaseDirectory;
					
					this.Hide();

					AppDomainSetup myDomainInfo = new AppDomainSetup();
					myDomainInfo.ApplicationBase = @strBasePath;

					// Creates the application domain.
					AppDomain mydomain = AppDomain.CreateDomain("TimeAttendanceMain", null, myDomainInfo);

					Assembly myAssembly = Assembly.LoadFrom(@strPath,mydomain.Evidence);
					System.Windows.Forms.Form frm = (System.Windows.Forms.Form)myAssembly.CreateInstance("InteractPayrollClient.frmTimeAttendanceMain");
					frm.ShowDialog();
					frm.Dispose();
					AppDomain.Unload(mydomain);
					this.Close();
				}
				catch(System.Exception ex)
				{
					MessageBox.Show("Late Binding Error " + ex.ToString());
				}
			}
			catch (Exception eException)
			{
				clsTAUtilities.ErrorHandler(eException);
				this.Close();
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
