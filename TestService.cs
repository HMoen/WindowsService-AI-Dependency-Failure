namespace WindowsServiceAIFailure
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.ServiceProcess;
    using System.Threading.Tasks;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;

    /// <inheritdoc />
    /// <summary>Service to test AI failure.</summary>
    public partial class TestService : ServiceBase
    {
        /// <summary>Telemetry client.</summary>
        private readonly TelemetryClient telemetryClient;

        /// <summary>HTTP client.</summary>
        private readonly HttpClient httpClient;

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="TestService" /> class.</summary>
        public TestService()
        {
            InitializeComponent();

            var key = ConfigUtility.GetKeyValue();
            this.telemetryClient = new TelemetryClient(new TelemetryConfiguration(key)) { InstrumentationKey = key };
            this.httpClient = new HttpClient();
        }

        /// <summary>Perform action causing AI failure.</summary>
        public void PerformFailure()
        {
            Task.Run(
                async () =>
                    {
                        while (true)
                        {
                            try
                            {
                                using (telemetryClient.StartOperation<RequestTelemetry>("operation"))
                                {
                                    await this.httpClient.GetAsync("https://bing.com").ConfigureAwait(false);
                                    telemetryClient.TrackEvent("Bing call event completed");
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e);
                                this.Stop();
                            }

                            await Task.Delay(1000).ConfigureAwait(false);
                        }
                    });
        }

        /// <inheritdoc />
        protected override void OnStart(string[] args)
        {
            this.PerformFailure();
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            this.telemetryClient.Flush();
        }
    }
}
