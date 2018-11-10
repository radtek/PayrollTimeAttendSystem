using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FileDownloadExe
{
    class Program
    {
        static void Main(string[] args)
        {
            bool blnContinue = true;

            FileInfo fiFileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\URLConfig.txt_");

            if (fiFileInfo.Exists == true)
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "URLConfig.txt");

                File.Move(AppDomain.CurrentDomain.BaseDirectory + "URLConfig.txt_", AppDomain.CurrentDomain.BaseDirectory + "\\URLConfig.txt");
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

            if (blnContinue == true)
            {
                AppDomain.CurrentDomain.SetData("URLClientPath", "");

                string strPath = AppDomain.CurrentDomain.BaseDirectory + "PayrollLogon.dll";
                System.Reflection.Assembly tmpAssembly = System.Reflection.Assembly.LoadFile(strPath);

                object lateBoundObj = tmpAssembly.CreateInstance("InteractPayroll.frmSplashScreen");

                Form Form = (Form)lateBoundObj;

                Form.ShowDialog();

                if (Convert.ToBoolean(AppDomain.CurrentDomain.GetData("Logoff")) == true)
                {
                    MessageBox.Show("Changes Have been Made to the Main Program.\nYou need to Restart the Program.",
                        "Program Changes",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    GC.Collect();
                }
                else
                {
                    if (Convert.ToInt64(AppDomain.CurrentDomain.GetData("UserNo")) > -1)
                    {
                        strPath = AppDomain.CurrentDomain.BaseDirectory + "FileDownload.dll";
                        tmpAssembly = System.Reflection.Assembly.LoadFile(strPath);

                        lateBoundObj = tmpAssembly.CreateInstance("InteractPayroll.frmFileDownload");

                        Form = (Form)lateBoundObj;
                        Form.ShowDialog();

                        GC.Collect();
                    }
                }
            }
        }
    }
}
