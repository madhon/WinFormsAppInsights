namespace DesktopApplicationInsights.ExampleApp
{
    using System;

    public partial class Form1 : TelemetryForm
    {
        public Form1()
        {
            InitializeComponent();

            this.TelemetryClientName = "DesktopApplicationInsights.ExampleApp";
            this.telemetryButton1.TelemetryClientName = "DesktopApplicationInsights.ExampleApp";
        }

        private void telemetryButton1_Click(object sender, System.EventArgs e)
        {
            try
            {
                throw new InvalidOperationException("Button1 fubar");
            }
            catch (Exception ex)
            {
                this.TelemetryClient.LogHandledException(ex);
                throw;
            }
        }
    }
}
