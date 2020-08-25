namespace DesktopApplicationInsights.ExampleApp
{
    using System;
    using System.Windows.Forms;
    using Microsoft.ApplicationInsights;

    public static class Program
    {

        public static TelemetryClient tc;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //tc = Telemetry.CreateClient("DesktopApplicationInsights.ExampleApp", "<instrumentation key from App Insights project>");
            tc = Telemetry.CreateClient("DesktopApplicationInsights.ExampleApp", "21d580e9-681a-49d7-9f2c-9e85547a17b4");

            Application.Run(new Form1());
        }
    }
}
