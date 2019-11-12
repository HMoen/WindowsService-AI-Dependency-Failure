namespace WindowsServiceAIFailure
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.ServiceProcess;
    using System.Threading.Tasks;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc />
    /// <summary>Service to test AI failure.</summary>
    public partial class TestService : ServiceBase
    {
        /// <summary>Telemetry client.</summary>
        private readonly TelemetryClient telemetryClient;

        /// <summary>Service logger.</summary>
        private readonly ILogger<TestService> logger;

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="TestService" /> class.</summary>
        public TestService()
        {
            InitializeComponent();

            var key = ConfigUtility.GetKeyValue();
            var servicesCollection = new ServiceCollection();
            servicesCollection.AddApplicationInsightsTelemetryWorkerService(key);
            var serviceProvider = servicesCollection.BuildServiceProvider();

            ////this.telemetryClient = new TelemetryClient(new TelemetryConfiguration(key)) { InstrumentationKey = key };
            this.telemetryClient = serviceProvider.GetRequiredService<TelemetryClient>();
            this.logger = serviceProvider.GetRequiredService<ILogger<TestService>>();
        }

        /// <summary>Perform action causing AI failure.</summary>
        public void PerformFailure()
        {
            Task.Run(
                async () =>
                    {
                        var httpClient = new HttpClient();

                        while (true)
                        {
                            try
                            {
                                using (telemetryClient.StartOperation<RequestTelemetry>("operation"))
                                {
                                    logger.LogWarning("A sample warning message.");
                                    logger.LogInformation("Calling bing.com");
                                    var responseMessage = await httpClient.GetAsync("https://bing.com").ConfigureAwait(false);
                                    logger.LogInformation("Calling bing completed with status:" + responseMessage.StatusCode);
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
