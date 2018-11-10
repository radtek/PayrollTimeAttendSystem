using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace InteractPayrollClient
{
    public class TimeAttendanceClient
    {
        [STAThread]
        static void Main(string[] args)
        {
            //2018-11-03 
            //AppDomain.CurrentDomain.SetData("FormSmallest", "Y");

            bool blnResult;
            string strProgramName = "TimeAttendanceClientIS";

            var mutex = new System.Threading.Mutex(true, strProgramName, out blnResult);

            if (blnResult == false)
            {
                System.Windows.Forms.MessageBox.Show(strProgramName + ".exe is already running.", strProgramName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool blnContinue = true;
            //NB TimeAttendance.exe will Replace TimeAttendanceIS.exe_ to TimeAttendanceIS.exe if needs be 

            //P = Payroll
            //T = Time Attendance Client
            AppDomain.CurrentDomain.SetData("FromProgramInd", "T");

            string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

#if(DEBUG)
            strBaseDirectory += "bin\\";

            //Server Files with _ Renamed (Only in Debug Mode)
            string[] strFiles = Directory.GetFiles(strBaseDirectory, "*.*_");

            foreach (string strFile in strFiles)
            {
                if (strFile.Substring(strFile.Length - 25) == "TimeAttendanceClient.exe_"
                    | strFile.Substring(strFile.Length - 29) == "TimeSheetDynamicUploadIS.exe_")
                {
                    continue;
                }

                File.Copy(strFile, strFile.Substring(0, strFile.Length - 1), true);
                File.Delete(strFile);
            }
#else
            if (args.Length == 0)
            {
                MessageBox.Show("Click on TimeAttendanceClient.exe to Start this Program");
                return;
            }
            else
            {
                if (args[0].ToString() != "1")
                {
                    MessageBox.Show("Click on TimeAttendanceClient.exe to Start this Program");
                    return;
                }
            }
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FileInfo fiFileInfo = new FileInfo(strBaseDirectory + "TimeAttendanceLogon.dll_");

            if (fiFileInfo.Exists == true)
            {
                try
                {
                    File.Copy(strBaseDirectory + "TimeAttendanceLogon.dll_", strBaseDirectory + "TimeAttendanceLogon.dll", true);
                    File.Delete(strBaseDirectory + "TimeAttendanceLogon.dll_");
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Error Copying / Deleting TimeAttendanceLogon.dll_");
                }
            }

            fiFileInfo = new FileInfo(strBaseDirectory + "clsISClientUtilities.dll_");

            if (fiFileInfo.Exists == true)
            {
                try
                {
                    File.Copy(strBaseDirectory + "clsISClientUtilities.dll_", strBaseDirectory + "clsISClientUtilities.dll", true);
                    File.Delete(strBaseDirectory + "clsISClientUtilities.dll_");
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Error Copying / Deleting clsISClientUtilities.dll_");
                }
            }

            //Move TimeSheetDynamicUploadIS.dll_ Live
            fiFileInfo = new FileInfo(strBaseDirectory + "TimeSheetDynamicUploadIS.exe_");

            if (fiFileInfo.Exists == true)
            {
                try
                {
                    File.Copy(strBaseDirectory + "TimeSheetDynamicUploadIS.exe_", strBaseDirectory + "TimeSheetDynamicUploadIS.exe", true);
                    File.Delete(strBaseDirectory + "TimeSheetDynamicUploadIS.exe_");
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Error Copying / Deleting TimeSheetDynamicUploadIS.dll_");
                }
            }

            fiFileInfo = new FileInfo(strBaseDirectory + "clsISUtilities.dll_");

            if (fiFileInfo.Exists == true)
            {
                try
                {
                    File.Copy(strBaseDirectory + "clsISUtilities.dll_", strBaseDirectory + "clsISUtilities.dll", true);
                    File.Delete(strBaseDirectory + "clsISUtilities.dll_");
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Error Copying / Deleting clsISUtilities.dll_");
                }
            }

            fiFileInfo = new FileInfo(strBaseDirectory + "PasswordChange.dll_");

            if (fiFileInfo.Exists == true)
            {
                try
                {
                    File.Copy(strBaseDirectory + "PasswordChange.dll_", strBaseDirectory + "PasswordChange.dll", true);
                    File.Delete(strBaseDirectory + "PasswordChange.dll_");
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Error Copying / Deleting PasswordChange.dll_");
                }
            }

            fiFileInfo = new FileInfo(strBaseDirectory + "\\DownloadFiles.dll_");

            if (fiFileInfo.Exists == true)
            {
                try
                {
                    File.Copy(strBaseDirectory + "DownloadFiles.dll_", strBaseDirectory + "\\DownloadFiles.dll", true);
                    File.Delete(strBaseDirectory + "DownloadFiles.dll_");
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Error Copying / Deleting DownloadFiles.dll_");
                }
            }

            //2013-12-24
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
                DialogResult myDialogResult = System.Windows.Forms.MessageBox.Show("You are going to Use Internet Web Service Layer (Live System).\n\nWould you like to Continue.",
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
            
            fiFileInfo = new FileInfo("URLClientConfig.txt");

            if (fiFileInfo.Exists == false)
            {
#if(DEBUG)
                AppDomain.CurrentDomain.SetData("URLClientPath", "");
#else
                //Set To Read Local Machine
                string strFile = "127.0.0.1:8000";

                StreamWriter swStreamWriter = fiFileInfo.AppendText();

                swStreamWriter.WriteLine(strFile);

                swStreamWriter.Close();

                AppDomain.CurrentDomain.SetData("URLClientPath", "127.0.0.1:8000");
#endif
            }
            else
            {
#if(DEBUG)
                DialogResult myDialogResult = System.Windows.Forms.MessageBox.Show("You are going to Use the LOCAL Web Service Layer (Live System).\n\nWould you like to Continue.",
               "Question",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

                if (myDialogResult == System.Windows.Forms.DialogResult.No)
                {
                    blnContinue = false;
                }
#endif
                StreamReader srStreamReader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "URLClientConfig.txt");

                string strURLPath = srStreamReader.ReadLine();

                AppDomain.CurrentDomain.SetData("URLClientPath", strURLPath);

                srStreamReader.Close();
            }

            if (blnContinue == true)
            {
                try
                {
                    string strPath = AppDomain.CurrentDomain.BaseDirectory + "TimeAttendanceLogon.dll";

                    System.Reflection.Assembly tmpAssembly = System.Reflection.Assembly.LoadFile(strPath);

                    object lateBoundObj = tmpAssembly.CreateInstance("InteractPayrollClient.frmLogonScreen");

                    Form myForm = (System.Windows.Forms.Form)lateBoundObj;

                    myForm.ShowDialog();
                }
                catch (System.Exception ex)
                {
                }
                finally
                {
                    try
                    {
                        mutex.ReleaseMutex();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
