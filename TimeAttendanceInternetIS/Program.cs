using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace InteractPayroll
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //2018-11-03 
            //AppDomain.CurrentDomain.SetData("FormSmallest", "Y");

            bool blnDebug = false;

            //P = Payroll
            //T = Time Attendance Client
            //X = Time Attendance Internet
            AppDomain.CurrentDomain.SetData("FromProgramInd", "X");

            //NB TimeAttendanceInternetInternet.exe will Replace TimeAttendanceInternetIS.exe_ to TimeAttendanceInternetIS.exe if needs be 

            //Group of Files Required to Start Run

            //clsDBConnectionObjects.dll
            //clsISUtilities.dll
            //PasswordChange.dll
            //TimeAttendanceInternetInternet.exe - This will Rename TimeAttendanceInternetIS.exe_ to TimeAttendanceInternetIS.exe (For Dot Net Change)
            //TimeAttendanceInternetIS.exe
            //TimeAttendanceLogon.dll
            //DownloadFiles.dll

            //URLConfig.txt If File Exist then will Link to Internet otherwise need Business objects eg. busTimeAttendanceLogon.dll etc.

            FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\Debug.txt");

            if (fiFileInfo.Exists == true)
            {
                blnDebug = true;
            }

            if (blnDebug == true)
            {
                MessageBox.Show("Entered TimeAttendanceInternetIS.exe");
            }
#if(DEBUG)
            string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory + "bin\\";

            //Server Files with _ Renamed (Only in Debug Mode)
            string[] strFiles = Directory.GetFiles(strBaseDirectory, "*.*_");

            foreach (string strFile in strFiles)
            {
                File.Delete(strFile.Substring(0, strFile.Length - 1));

                File.Move(strFile, strFile.Substring(0, strFile.Length - 1));
            }
#else
            if (args.Length == 0)
            {
                MessageBox.Show("Click on TimeAttendanceInternet.exe to Start this Program");
                return;
            }
            else
            {
                if (args[0].ToString() != "1")
                {
                    MessageBox.Show("Click on TimeAttendanceInternet.exe to Start this Program");
                    return;
                }
            }
#endif
            try
            {
                bool blnContinue = true;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                AppDomain.CurrentDomain.SetData("Logoff", false);

                string strApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
#if(DEBUG)
                //Put Here to Stop overwrite of New Compiled Programs is Debug Directory
                strApplicationPath = AppDomain.CurrentDomain.BaseDirectory + "bin\\";

                //NB TimeAttendanceInternetIS.exe_ gets Renamed in TimeSheetInternet.exe when Not DEBUG 
                fiFileInfo = new FileInfo(strApplicationPath + "\\TimeAttendanceInternetIS.exe_");

                if (fiFileInfo.Exists == true)
                {
                    File.Delete(strApplicationPath + "TimeAttendanceInternetIS.exe");

                    File.Move(strApplicationPath + "TimeAttendanceInternetIS.exe_", strApplicationPath + "\\TimeAttendanceInternetIS.exe");
                }
#endif

                fiFileInfo = new FileInfo(strApplicationPath + "\\TimeAttendanceInternetLogon.dll_");

                if (fiFileInfo.Exists == true)
                {
                    File.Delete(strApplicationPath + "TimeAttendanceInternetLogon.dll");

                    File.Move(strApplicationPath + "TimeAttendanceInternetLogon.dll_", strApplicationPath + "\\TimeAttendanceInternetLogon.dll");
                }

                //2013-02-04
                fiFileInfo = new FileInfo(strApplicationPath + "\\DownloadFiles.dll_");

                if (fiFileInfo.Exists == true)
                {
                    File.Delete(strApplicationPath + "DownloadFiles.dll");

                    File.Move(strApplicationPath + "DownloadFiles.dll_", strApplicationPath + "\\DownloadFiles.dll");
                }

                fiFileInfo = new FileInfo(strApplicationPath + "\\clsISUtilities.dll_");

                if (fiFileInfo.Exists == true)
                {
                    File.Delete(strApplicationPath + "clsISUtilities.dll");

                    File.Move(strApplicationPath + "clsISUtilities.dll_", strApplicationPath + "\\clsISUtilities.dll");
                }

                fiFileInfo = new FileInfo(strApplicationPath + "\\clsISClientUtilities.dll_");

                if (fiFileInfo.Exists == true)
                {
                    File.Delete(strApplicationPath + "clsISClientUtilities.dll");

                    File.Move(strApplicationPath + "clsISClientUtilities.dll_", strApplicationPath + "\\clsISClientUtilities.dll");
                }

                fiFileInfo = new FileInfo(strApplicationPath + "\\clsDBConnectionObjects.dll_");

                if (fiFileInfo.Exists == true)
                {
                    File.Delete(strApplicationPath + "clsDBConnectionObjects.dll");

                    File.Move(strApplicationPath + "clsDBConnectionObjects.dll_", strApplicationPath + "\\clsDBConnectionObjects.dll");
                }

                fiFileInfo = new FileInfo(strApplicationPath + "\\PasswordChange.dll_");

                if (fiFileInfo.Exists == true)
                {
                    File.Delete(strApplicationPath + "PasswordChange.dll");

                    File.Move(strApplicationPath + "PasswordChange.dll_", strApplicationPath + "\\PasswordChange.dll");
                }

                fiFileInfo = new FileInfo(strApplicationPath + "\\URLConfig.txt_");

                if (fiFileInfo.Exists == true)
                {
                    File.Delete(strApplicationPath + "URLConfig.txt");

                    File.Move(strApplicationPath + "URLConfig.txt_", strApplicationPath + "\\URLConfig.txt");
                }

                //Get URL Path From Config File 
                string strConfig = AppDomain.CurrentDomain.BaseDirectory + "URLConfig.txt";

                fiFileInfo = new FileInfo(strConfig);

                if (fiFileInfo.Exists == true)
                {
                    StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "URLConfig.txt");

                    string strURLPath = srStreamReader.ReadLine();

                    AppDomain.CurrentDomain.SetData("URLPath", strURLPath);

                    srStreamReader.Close();

#if(DEBUG)
                    DialogResult myDialogResult = MessageBox.Show("You are going to Use the Web Service Layer (Live System).\n\nWould you like to Continue.",
                    "Error",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error);

                    if (myDialogResult == System.Windows.Forms.DialogResult.No)
                    {
                        blnContinue = false;
                    }
#endif
                }
                else
                {
                    AppDomain.CurrentDomain.SetData("URLPath", "");
#if(DEBUG)
#else
                    MessageBox.Show("URLConfig.txt File Missing.\n\nSpeak to your System Administrator.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                    blnContinue = false;
#endif
                }

                //2013-01-24
                strConfig = AppDomain.CurrentDomain.BaseDirectory + "URLClientConfig.txt";

                fiFileInfo = new FileInfo(strConfig);

                if (fiFileInfo.Exists == true)
                {
                    StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "URLClientConfig.txt");

                    string strURLPath = srStreamReader.ReadLine();

                    srStreamReader.Close();

                    if (Check_IP(strURLPath) != 0)
                    {
                        blnContinue = false;

                        MessageBox.Show("Error in 'URLClientConfig.txt'\n\nSpeak To System Administrator.");
                    }
#if(DEBUG)
                    DialogResult myDialogResult = MessageBox.Show("Would you like to use the CLIENT Web Service Layer?",
                   "Error",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Warning);

                    if (myDialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        AppDomain.CurrentDomain.SetData("URLClientPath", strURLPath);
                    }
                    else
                    {
                        AppDomain.CurrentDomain.SetData("URLClientPath", "");
                    }
#else
                    AppDomain.CurrentDomain.SetData("URLClientPath", strURLPath);
#endif
                }
                else
                {
#if(DEBUG)
                    AppDomain.CurrentDomain.SetData("URLClientPath", "");
#else

                    //Set To Read Local - Local Machine Digital Persona
                    string strFile = "127.0.0.1:8000";

                    StreamWriter swStreamWriter = fiFileInfo.AppendText();

                    swStreamWriter.WriteLine(strFile);

                    swStreamWriter.Close();

                    AppDomain.CurrentDomain.SetData("URLClientPath", "127.0.0.1:8000");
#endif
                }

                if (blnDebug == true)
                {
                    MessageBox.Show("Before Load TimeAttendanceInternetLogon.dll (Reflection)");
                }

                if (blnContinue == true)
                {
                    string strPath = AppDomain.CurrentDomain.BaseDirectory + "TimeAttendanceInternetLogon.dll";
                    System.Reflection.Assembly tmpAssembly = System.Reflection.Assembly.LoadFile(strPath);

                    if (blnDebug == true)
                    {
                        MessageBox.Show("Create LateBound Object (InteractPayroll.frmSplashScreen)");
                    }

                    object lateBoundObj = tmpAssembly.CreateInstance("InteractPayroll.frmSplashScreen");

                    if (blnDebug == true)
                    {
                        MessageBox.Show("Before Cast to Form (InteractPayroll.frmSplashScreen)");
                    }

                    Form Form = (Form)lateBoundObj;

                    if (blnDebug == true)
                    {
                        MessageBox.Show("Before Form.ShowDialog() (InteractPayroll.frmSplashScreen)");
                    }

                    Form.ShowDialog();

                    if (blnDebug == true)
                    {
                        MessageBox.Show("After Form.ShowDialog() (InteractPayroll.frmSplashScreen)");
                    }

                    if (Convert.ToBoolean(AppDomain.CurrentDomain.GetData("Logoff")) == true)
                    {
                        MessageBox.Show("Changes Have been Made to the Main Program.\nYou need to Restart the Program.",
                            "Program Changes",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        if (Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo")) > -1)
                        {
                            strPath = AppDomain.CurrentDomain.GetData("StartUpFile").ToString();

                            if (blnDebug == true)
                            {
                                MessageBox.Show("Before Load TimeAttendanceMain.dll (Reflection)");
                            }

                            tmpAssembly = System.Reflection.Assembly.LoadFile(strPath);

                            if (blnDebug == true)
                            {
                                MessageBox.Show("Create LateBound Object (InteractPayroll.frmTimeAttendanceMain)");
                            }

                            lateBoundObj = tmpAssembly.CreateInstance("InteractPayroll.frmTimeAttendanceMain");

                            if (blnDebug == true)
                            {
                                MessageBox.Show("Before Cast to Form (InteractPayroll.frmTimeAttendanceMain)");
                            }

                            Form = (Form)lateBoundObj;

                            if (blnDebug == true)
                            {
                                MessageBox.Show("Before Form.ShowDialog() (InteractPayroll.frmPayrollMain)");
                            }

                            Form.ShowDialog();

                            GC.Collect();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error " + ex.ToString());
            }
        }

        static int Check_IP(string strIP)
        {
            int intReturn = 1;

            string[] strParts = strIP.Split('.');

            if (strParts.Length == 4)
            {
                string[] strLastPart = strParts[3].Split(':');

                int intResult = 0;

                if (strLastPart.Length == 2)
                {
                    if (int.TryParse(strParts[0], out intResult) == true
                        & int.TryParse(strParts[1], out intResult) == true
                        & int.TryParse(strParts[2], out intResult) == true
                        & int.TryParse(strLastPart[0], out intResult) == true
                         & int.TryParse(strLastPart[1], out intResult) == true)
                    {
                        intReturn = 0;
                    }
                }
            }

            return intReturn;
        }
    }
}
