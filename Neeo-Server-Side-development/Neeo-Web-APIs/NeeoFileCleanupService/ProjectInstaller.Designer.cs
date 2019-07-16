namespace NeeoFileCleanupService
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
            this.NeeoFileCleanupProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.NeeoFileCleanupInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // NeeoFileCleanupProcessInstaller
            // 
            this.NeeoFileCleanupProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.NeeoFileCleanupProcessInstaller.Password = null;
            this.NeeoFileCleanupProcessInstaller.Username = null;
            // 
            // NeeoFileCleanupInstaller
            // 
            this.NeeoFileCleanupInstaller.Description = "It deletes the shared content that has expired.";
            this.NeeoFileCleanupInstaller.DisplayName = "Neeo File Cleanup Service";
            this.NeeoFileCleanupInstaller.ServiceName = "NeeoFileCleanupSvc";
            this.NeeoFileCleanupInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.NeeoFileCleanupInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller1_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.NeeoFileCleanupProcessInstaller,
            this.NeeoFileCleanupInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller NeeoFileCleanupProcessInstaller;
        private System.ServiceProcess.ServiceInstaller NeeoFileCleanupInstaller;
    }
}