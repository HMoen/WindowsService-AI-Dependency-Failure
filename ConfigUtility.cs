namespace WindowsServiceAIFailure
{
    using System.Configuration;
    using System.Reflection;

    public static class ConfigUtility
    {
        private const string InstrumentationKey = "Azure.ApplicationInsights.InstrumentationKey";

        private static readonly Configuration configFile;

        private static readonly KeyValueConfigurationCollection settings;

        static ConfigUtility()
        {
            var assembly = Assembly.GetAssembly(typeof(TestService));
            assembly = assembly ?? Assembly.GetEntryAssembly();
            configFile = ConfigurationManager.OpenExeConfiguration(assembly?.Location);
            settings = configFile.AppSettings.Settings;
        }

        public static void WriteKeyValue(string value)
        {
            if (settings[InstrumentationKey] == null)
            {
                settings.Add(InstrumentationKey, value);
            }
            else
            {
                settings[InstrumentationKey].Value = value;
            }

            configFile.Save(ConfigurationSaveMode.Modified);
        }

        public static string GetKeyValue() => settings[InstrumentationKey].Value;
    }
}