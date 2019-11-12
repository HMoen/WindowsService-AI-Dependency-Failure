namespace WindowsServiceAIFailure
{
    using System;
    using System.Diagnostics;
    using System.ServiceProcess;
    using System.Threading;

    /// <summary>Windows service program.</summary>
    public static class Program
    {
        /// <summary>The main entry point for the application.</summary>
        public static void Main()
        {
            LaunchDebugger();

            var testService = new TestService();
            if (!Environment.UserInteractive)
            {
                ServiceBase.Run(new ServiceBase[] { testService });
                return;
            }

            testService.PerformFailure();
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(30));
            }
        }

        /// <summary>Launch the debugger if in debug mode and if it's not currently attached.</summary>
        [Conditional("DEBUG")]
        private static void LaunchDebugger()
        {
            if (Environment.UserInteractive)
            {
                return;
            }

            if (Debugger.IsAttached)
            {
                return;
            }

            Debugger.Launch();
        }
    }
}