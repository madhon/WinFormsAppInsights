namespace DesktopApplicationInsights.ExampleApp
{
    using System;
    using System.Windows.Forms;

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var myClient = Telemetry.CreateClient("DesktopApplicationInsights.ExampleApp", "<instrumentation key from App Insights project>");

            Application.Run(new Form1());
        }
    }
}
