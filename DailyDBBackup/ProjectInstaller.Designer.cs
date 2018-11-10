namespace DailyDBBackup
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DailyDBBackupServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.DailyDBBackupServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // DailyDBBackupServiceProcessInstaller
            // 
            this.DailyDBBackupServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.DailyDBBackupServiceProcessInstaller.Password = null;
            this.DailyDBBackupServiceProcessInstaller.Username = null;
            // 
            // DailyDBBackupServiceInstaller
            // 
            this.DailyDBBackupServiceInstaller.ServiceName = "Daily DB Backup Service";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.DailyDBBackupServiceProcessInstaller,
            this.DailyDBBackupServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller DailyDBBackupServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller DailyDBBackupServiceInstaller;
    }
}