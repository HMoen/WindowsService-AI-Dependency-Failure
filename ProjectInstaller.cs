namespace WindowsServiceAIFailure
{
    using System.ComponentModel;
    using System.Configuration.Install;

    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        /// <inheritdoc />
        public ProjectInstaller()
        {
            InitializeComponent();
            this.BeforeInstall += this.ProjectInstallerBeforeInstall;
            this.AfterInstall += this.ProjectInstallerAfterInstall;
        }

        /// <summary>Validate parameters before install.</summary>
        /// <param name="sender">Sender <see cref="object"/>.</param>
        /// <param name="e"><see cref="InstallEventArgs"/> object.</param>
        private void ProjectInstallerBeforeInstall(object sender, InstallEventArgs e)
        {
            var key = this.Context.Parameters["KEY"].Trim();
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InstallException("Application Insights instrumentation key is missing.");
            }
        }

        /// <summary>Update app.config with values after install.</summary>
        /// <param name="sender">Sender <see cref="object"/>.</param>
        /// <param name="e"><see cref="InstallEventArgs"/> object.</param>
        private void ProjectInstallerAfterInstall(object sender, InstallEventArgs e)
        {
            var value = this.Context.Parameters["KEY"].Trim();
            ConfigUtility.WriteKeyValue(value);
        }
    }
}
