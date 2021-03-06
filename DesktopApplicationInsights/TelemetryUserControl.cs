﻿namespace DesktopApplicationInsights
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Microsoft.ApplicationInsights;

    /// <summary>
    /// Base UserControl class with built-in Telemetry wireup
    /// </summary>
    public class TelemetryUserControl : UserControl
    {
        private readonly Lazy<TelemetryClient> _telemetryClientFetcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryUserControl"/> class.
        /// </summary>
        public TelemetryUserControl() : base()
        {
            _telemetryClientFetcher = new Lazy<TelemetryClient>(() =>
            {
                System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(this.TelemetryClientName),
                    $"No Telemetry client name set on Telemetry Button \"{this.Name}\"");

                if (!string.IsNullOrWhiteSpace(this.TelemetryClientName))
                {
                    try
                    {
                        return Telemetry.GetClient(this.TelemetryClientName);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Fail(
                            $"Couldn't find telemetry client with name {this.TelemetryClientName}", ex.ToString());
                    }
                }

                return null;
            });
        }

        /// <summary>
        /// Gets or sets the name of the telemetry client used by the button
        /// </summary>
        [EditorBrowsable]
        public string TelemetryClientName { get; set; }

        /// <summary>
        /// Gets the telemetry client.
        /// </summary>
        protected TelemetryClient TelemetryClient => _telemetryClientFetcher.Value;
    }
}
