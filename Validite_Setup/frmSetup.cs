using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Principal;
using System.IO;
using Microsoft.Win32;

namespace Setup
{
    public partial class frmSetup : Form
    {
        private int intCount = 0;

        public frmSetup()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.lblInfo.Visible == true)
            {
                this.lblInfo.Visible = false;
            }
            else
            {
                this.lblInfo.Visible = true;
            }

            intCount += 1;

            if (intCount == 1)
            {
#if(DEBUG)
#else
                if (IsAdministrator() == false)
                {
                    this.chkTimeAttendanceService.Enabled = false;
                    this.chkDatabase.Enabled = false;
                    this.chkInternetTimeAttendance.Enabled = false;
                    this.chkPayroll.Enabled = false;
                    this.chkClock.Enabled = false;
                    this.chkTimeAttendance.Enabled = false;

                    MessageBox.Show("This Program must be Run with Administrator Rights\n", this.Text,
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);

                   
                }
#endif
            }

            if (intCount == 4)
            {
                this.timer1.Enabled = false;
            }
        }

        private bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            bool bln64BitOperatingSystem = false;
            this.chkTimeAttendanceService.Enabled = false;
            this.chkPayroll.Enabled = false;
            this.chkInternetTimeAttendance.Enabled = false;

            this.btnInstall.Enabled = false;

            if (System.Environment.Is64BitOperatingSystem == true)
            {
                bln64BitOperatingSystem = true;
            }

            this.txtInfo.Text = "";

            string strStartupPath = AppDomain.CurrentDomain.BaseDirectory;

            Process pTimeAttendanceService = null;

            if (this.chkDatabase.Checked == true)
            {
                //2017-05-05
                //Check to See if DB Exists
                string strX86folder = Environment.GetEnvironmentVariable("ProgramFiles(x86)");

                if (strX86folder == "")
                {
                    strX86folder = Environment.GetEnvironmentVariable("ProgramFiles");

                    if (strX86folder == "")
                    {
                        MessageBox.Show("Cannot Find ProgramFiles(x86)\n\nSpeak to Administrator.",
                             "Error",
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Error);

                        return;
                    }
                }

                strX86folder += @"\Validite\Validite Services\DB\InteractPayrollClient.mdf";
#if (DEBUG)
                strX86folder = strX86folder.Replace("InteractPayrollClient", "InteractPayrollClient_Debug");
#endif
                FileInfo fiFileInfo = new FileInfo(strX86folder);

                if (fiFileInfo.Exists == true)
                {
                    DialogResult myDialogResult = MessageBox.Show("Database '" + strX86folder + "' Already Exists.\n\nAction Cancelled.",
                               "Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);

                    goto btnInstall_Click_Continue;
                }

                //Install Database Engine
                pTimeAttendanceService = null;
               
                bool blnSqlLocalDbInstalled = false;

                using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions\13.0"))
                {
                    if (registryKey != null)
                    {
                        string strInstanceApiPath = (string)registryKey.GetValue("InstanceApiPath");

                        if (strInstanceApiPath != "")
                        {
                            blnSqlLocalDbInstalled = true;
                        }
                    }
                }

                if (blnSqlLocalDbInstalled == false)
                {
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions\12.0"))
                    {
                        if (registryKey != null)
                        {
                            string strInstanceApiPath = (string)registryKey.GetValue("InstanceApiPath");

                            if (strInstanceApiPath != "")
                            {
                                blnSqlLocalDbInstalled = true;
                            }
                        }
                    }
                }

                if (blnSqlLocalDbInstalled == false)
                {
                    try
                    {
                        this.txtInfo.Text += "Starting SqlLocalDB.msi ..\r\n";

                        string strStartExePath = "/i " + @"SqlServerLocalDB\x64\SqlLocalDB.msi";

                        if (bln64BitOperatingSystem == false)
                        {
                            strStartExePath = "/i " + strStartupPath + @"SqlServerLocalDB\x86\SqlLocalDB.msi";
                        }

                        pTimeAttendanceService = Process.Start("msiexec.exe", strStartExePath);

                        pTimeAttendanceService.WaitForExit();

                        if (pTimeAttendanceService.ExitCode == 0)
                        {
                            this.txtInfo.Text += "SqlLocalDB Install Successful.\r\n";
                        }
                        else
                        {
                            this.txtInfo.Text += "SqlLocalDB Install UNSUCCESSFUL.\r\n";

                            DialogResult myDialogResult = MessageBox.Show("SqlLocalDB did NOT Complete Successfully.\n\nWould you like to Continue.",
                            "Error",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Error);

                            if (myDialogResult == System.Windows.Forms.DialogResult.No)
                            {
                                pTimeAttendanceService.Close();
                                goto btnInstall_Click_Continue;
                            }
                        }

                        pTimeAttendanceService.Close();
                        //Install the Database 
                        pTimeAttendanceService = null;
                    }
                    catch (Exception ex)
                    {
                        this.txtInfo.Text += "SqlLocalDB Install Failed.\r\n";
                        pTimeAttendanceService.Close();
                        this.txtInfo.Text += "SqlLocalDB Error = " + ex.Message + "\r\n";

                        if (ex.InnerException.Message != null)
                        {
                            this.txtInfo.Text += ex.InnerException.Message;
                        }

                        goto btnInstall_Click_Continue;
                    }
                }
                else
                {
                    this.txtInfo.Text += "SqlLocalDB Already Installed\r\n";
                }

                try
                {
                    this.txtInfo.Text += "Starting ISDatabaseUtility.exe Database Install ..\r\n";

                    pTimeAttendanceService = Process.Start("ISDatabaseUtility.exe");

                    pTimeAttendanceService.WaitForExit();

                    if (pTimeAttendanceService.ExitCode == 0)
                    {
                        this.txtInfo.Text += "ISDatabaseUtility Database Install Successful.\r\n";
                    }
                    else
                    {
                        this.txtInfo.Text += "ISDatabaseUtility Database Install UNSUCCESSFUL.\r\n";

                        DialogResult myDialogResult = MessageBox.Show("ISDatabaseUtility did NOT Complete Successfully.\n\nWould you like to Continue.",
                        "Error",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);

                        if (myDialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            pTimeAttendanceService.Close();
                            goto btnInstall_Click_Continue;
                        }
                    }

                    pTimeAttendanceService.Close();
                }
                catch (Exception ex)
                {
                    this.txtInfo.Text += "ISDatabaseUtility Database Install Failed.\r\n";
                    pTimeAttendanceService.Close();
                    this.txtInfo.Text += "ISDatabaseUtility Error = " + ex.Message + "\r\n";

                    if (ex.InnerException.Message != null)
                    {
                        this.txtInfo.Text += ex.InnerException.Message;
                    }

                    goto btnInstall_Click_Continue;
                }
            }
            
            if (this.chkTimeAttendanceService.Checked == true)
            {
                pTimeAttendanceService = null;
                
                try
                {
                    this.txtInfo.Text += "Starting ValiditeServices_Setup.msi ..\r\n";

                    pTimeAttendanceService = Process.Start("msiexec.exe", "/i ValiditeServices_Setup.msi");
                   
                    pTimeAttendanceService.WaitForExit();

                    if (pTimeAttendanceService.ExitCode == 0)
                    {
                        this.txtInfo.Text += "ValiditeServices_Setup Install Successful.\r\n";
                    }
                    else
                    {
                        this.txtInfo.Text += "ValiditeServices_Setup Install UNSUCCESSFUL.\r\n";

                        DialogResult myDialogResult = MessageBox.Show("ValiditeServices_Setup did NOT Complete Successfully.\n\nWould you like to Continue.",
                        "Error",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);

                        if (myDialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            pTimeAttendanceService.Close();
                            goto btnInstall_Click_Continue;
                        }
                    }

                    pTimeAttendanceService.Close();
                }
                catch (Exception ex)
                {
                    this.txtInfo.Text += "ValiditeServices_Setup Install Failed.\r\n";
                    pTimeAttendanceService.Close();
                    this.txtInfo.Text += "ValiditeServices_Setup Error = " + ex.Message + "\r\n";

                    if (ex.InnerException.Message != null)
                    {
                        this.txtInfo.Text += ex.InnerException.Message;
                    }

                    goto btnInstall_Click_Continue;
                }
            }
       
            if (this.chkTimeAttendance.Checked == true)
            {
                Process pTimeAttendance = null;

                try
                {
                    this.txtInfo.Text += "Starting ValiditeClient_Setup.msi ..\r\n";

                    pTimeAttendance = Process.Start("msiexec.exe", "/i ValiditeClient_Setup.msi");

                    pTimeAttendance.WaitForExit();

                    if (pTimeAttendance.ExitCode == 0)
                    {
                        this.txtInfo.Text += "ValiditeClient_Setup Install Successful.\r\n";
                    }
                    else
                    {
                        this.txtInfo.Text += "ValiditeClient_Setup Install UNSUCCESSFUL.\r\n";

                        DialogResult myDialogResult = MessageBox.Show("ValiditeClient_Setup did NOT Complete Successfully.\n\nWould you like to Continue.",
                        "Error",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);

                        if (myDialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            pTimeAttendance.Close();
                            goto btnInstall_Click_Continue;
                        }
                    }

                    pTimeAttendance.Close();
                }
                catch (Exception ex)
                {
                    this.txtInfo.Text += "ValiditeClient_Setup Install Failed.\r\n";
                    pTimeAttendance.Close();
                    this.txtInfo.Text += "ValiditeClient_Setup Error = " + ex.Message + "\r\n";

                    if (ex.InnerException.Message != null)
                    {
                        this.txtInfo.Text += ex.InnerException.Message;
                    }

                    goto btnInstall_Click_Continue;
                }
            }
            
            if (this.chkClock.Checked == true)
            {
                Process pTimeAttendance = null;

                try
                {
                    this.txtInfo.Text += "Starting ValiditeClock_Setup.msi ..\r\n";

                    pTimeAttendance = Process.Start("msiexec.exe", "/i ValiditeClock_Setup.msi");

                    pTimeAttendance.WaitForExit();

                    if (pTimeAttendance.ExitCode == 0)
                    {
                        this.txtInfo.Text += "ValiditeClock_Setup Install Successful.\r\n";
                    }
                    else
                    {
                        this.txtInfo.Text += "ValiditeClock_Setup Install UNSUCCESSFUL.\r\n";

                        DialogResult myDialogResult = MessageBox.Show("ValiditeClock_Setup did NOT Complete Successfully.\n\nWould you like to Continue.",
                        "Error",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);

                        if (myDialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            pTimeAttendance.Close();
                            goto btnInstall_Click_Continue;
                        }
                    }

                    pTimeAttendance.Close();
                }
                catch (Exception ex)
                {
                    this.txtInfo.Text += "ValiditeClock_Setup Install Failed.\r\n";
                    pTimeAttendance.Close();
                    this.txtInfo.Text += "ValiditeClock_Setup Error = " + ex.Message + "\r\n";

                    if (ex.InnerException.Message != null)
                    {
                        this.txtInfo.Text +=  ex.InnerException.Message;
                    }

                    goto btnInstall_Click_Continue;
                }
            }

            if (this.chkTimeAttendanceService.Checked == true
            || this.chkClock.Checked == true
            || this.chkTimeAttendance.Checked == true)
            {
                //Install Digital Persona
                pTimeAttendanceService = null;

                try
                {
                    this.txtInfo.Text += "Starting Digital Persona Setup.msi ..\r\n";

                    string strStartExePath = "/i " + @"DigitalPersona\x64\setup.msi";

                    if (bln64BitOperatingSystem == false)
                    {
                        strStartExePath = "/i " + strStartupPath + @"DigitalPersona\x86\setup.msi";
                    }

                    pTimeAttendanceService = Process.Start("msiexec.exe", strStartExePath);

                    pTimeAttendanceService.WaitForExit();

                    if (pTimeAttendanceService.ExitCode == 0)
                    {
                        this.txtInfo.Text += "Digital Persona Install Successful.\r\n";
                    }
                    else
                    {
                        this.txtInfo.Text += "Digital Persona Install UNSUCCESSFUL.\r\n";

                        DialogResult myDialogResult = MessageBox.Show("Digital Persona did NOT Complete Successfully.\n\nWould you like to Continue.",
                        "Error",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);

                        if (myDialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            pTimeAttendanceService.Close();
                            goto btnInstall_Click_Continue;
                        }
                    }

                    pTimeAttendanceService.Close();
                }
                catch (Exception ex)
                {
                    this.txtInfo.Text += "Digital Persona Install Failed.\r\n";
                    pTimeAttendanceService.Close();
                    this.txtInfo.Text += "Digital Persona Error = " + ex.Message + "\r\n";

                    if (ex.InnerException.Message != null)
                    {
                        this.txtInfo.Text += ex.InnerException.Message;
                    }

                    goto btnInstall_Click_Continue;
                }
            }
      
            if (this.chkPayroll.Checked == true)
            {
                Process pPayroll = null;
                 
                try
                {
                    this.txtInfo.Text += "Starting ValiditePayrollInternet_Setup.msi ..\r\n";

                    pPayroll = Process.Start("msiexec.exe", "/i ValiditePayrollInternet_Setup.msi");

                    pPayroll.WaitForExit();

                    if (pPayroll.ExitCode == 0)
                    {
                        this.txtInfo.Text += "ValiditePayrollInternet_Setup Install Successful.\r\n";
                    }
                    else
                    {
                        this.txtInfo.Text += "ValiditePayrollInternet_Setup Install UNSUCCESSFULL.\r\n";
                        DialogResult myDialogResult = MessageBox.Show("ValiditePayrollInternet_Setup did NOT Complete Successfully.\n\nWould you like to Continue.",
                        "Error",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);

                        if (myDialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            pPayroll.Close();
                            goto btnInstall_Click_Continue;
                        }
                    }

                    pPayroll.Close();
                }
                catch (Exception ex)
                {
                    this.txtInfo.Text += "ValiditePayrollInternet_Setup Install Failed.\r\n";
                    pPayroll.Close();
                    this.txtInfo.Text += "ValiditePayrollInternet_Setup Error = " + ex.Message + "\r\n";

                    if (ex.InnerException.Message != null)
                    {
                        this.txtInfo.Text += ex.InnerException.Message;
                    }

                    goto btnInstall_Click_Continue;
                }
            }

            if (this.chkInternetTimeAttendance.Checked == true)
            {
                Process pInternetTimeAttendance = null;

                try
                {
                    this.txtInfo.Text += "Starting ValiditeTimeAttendanceInternet_Setup.msi ..\r\n";

                    pInternetTimeAttendance = Process.Start("msiexec.exe", "/i ValiditeTimeAttendanceInternet_Setup.msi");

                    pInternetTimeAttendance.WaitForExit();

                    if (pInternetTimeAttendance.ExitCode == 0)
                    {
                        this.txtInfo.Text += "ValiditeTimeAttendanceInternet_Setup Install Successful.\r\n";
                    }
                    else
                    {
                        this.txtInfo.Text += "ValiditeTimeAttendanceInternet_Setup Install UNSUCCESSFULL.\r\n";
                        DialogResult myDialogResult = MessageBox.Show("ValiditeTimeAttendanceInternet_Setup did NOT Complete Successfully.\n\nWould you like to Continue.",
                        "Error",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);

                        if (myDialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            pInternetTimeAttendance.Close();
                            goto btnInstall_Click_Continue;
                        }
                    }

                    pInternetTimeAttendance.Close();
                }
                catch (Exception ex)
                {
                    this.txtInfo.Text += "ValiditeTimeAttendanceInternet_Setup Install Failed.\r\n";
                    pInternetTimeAttendance.Close();
                    this.txtInfo.Text += "ValiditeTimeAttendanceInternet_Setup Error = " + ex.Message + "\r\n";

                    if (ex.InnerException.Message != null)
                    {
                        this.txtInfo.Text += ex.InnerException.Message;
                    }

                    goto btnInstall_Click_Continue;
                }
            }
            
        btnInstall_Click_Continue:

            this.txtInfo.Text += "Exiting Install.\r\n";

            this.chkTimeAttendanceService.Enabled = true;
            this.chkPayroll.Enabled = true;
            this.chkInternetTimeAttendance.Enabled = true;

            this.btnInstall.Enabled = true;
        }

        private void chkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkTimeAttendanceService.Checked == false
            && this.chkPayroll.Checked == false
            && this.chkInternetTimeAttendance.Checked == false
            && this.chkClock.Checked == false
            && this.chkTimeAttendance.Checked == false)
            {
                this.btnInstall.Enabled = false;
            }
            else
            {
                this.btnInstall.Enabled = true;
            }
        }

        private void chkTimeAttendanceService_Click(object sender, EventArgs e)
        {
            if (this.chkTimeAttendanceService.Checked == true)
            {
                this.chkDatabase.Checked = true;
                this.chkDatabase.Visible = true;
            }
            else
            {
                this.chkDatabase.Visible = false;
            }
        }
    }
}
