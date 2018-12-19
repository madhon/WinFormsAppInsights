namespace DesktopApplicationInsights
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;

    /// <summary>Root class providing easy access to <c>TelemetryClient</c> and global Telemetry Configuration</summary>
    public static class Telemetry
    {
        private static readonly Dictionary<string, Tuple<TelemetryClient, TelemetryConfiguration>> _clientsAndConfigs = new Dictionary<string, Tuple<TelemetryClient, TelemetryConfiguration>>();
        /// <summary>
        /// Creates the client.
        /// </summary>
        /// <param name="clientName">Name of the client.</param>
        /// <param name="instrumentationKey">The instrumentation key.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// A client already exists with name &lt;name&gt;;clientName
        /// or
        /// A client already exists with the given instrumentation key.;instrumentationKey
        /// </exception>
        public static TelemetryClient CreateClient(string clientName, string instrumentationKey)
        {
            return CreateClient(clientName, instrumentationKey, Assembly.GetCallingAssembly());
        }

        private static TelemetryClient CreateClient(string clientName, string instrumentationKey,
            Assembly sourceAssembly)
        {
            if (_clientsAndConfigs.ContainsKey(clientName))
            {
                throw new ArgumentException(
                    $"A client already exists with name \"{clientName}\". Use GetClient() to retrieve it.",
                    nameof(clientName));
            }

            if (_clientsAndConfigs.Any(c => c.Value.Item1.InstrumentationKey.Equals(instrumentationKey, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException(
                    "A client already exists with the given instrumentation key.", nameof(instrumentationKey));
            }

            var config = TelemetryConfiguration.CreateDefault();
            var client = new TelemetryClient(config);
            ConfigureApplication(instrumentationKey, client, config,
                new TelemetryInitializer(sourceAssembly));

            _clientsAndConfigs.Add(clientName, Tuple.Create(client, config));

            return client;
        }

        /// <summary>
        /// Gets an existing Telemetry Client or creates a new one with the specified name &amp; instrumentation key
        /// </summary>
        /// <param name="clientName">Name of the client.</param>
        /// <param name="instrumentationKey">The instrumentation key.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// A client already exists with the given instrumentation key.;instrumentationKey
        /// </exception>
        public static TelemetryClient GetOrCreateClient(string clientName, string instrumentationKey)
        {
            Tuple<TelemetryClient, TelemetryConfiguration> clientWithConfig;
            if (!_clientsAndConfigs.TryGetValue(clientName, out clientWithConfig))
            {
                CreateClient(clientName, instrumentationKey, Assembly.GetCallingAssembly());
            }
            return _clientsAndConfigs[clientName].Item1;
        }

        /// <summary>
        /// Gets the specified telemetry client.
        /// </summary>
        /// <param name="clientName">Name of the client.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">No client has been crated with name &lt;name&gt;;clientName</exception>
        public static TelemetryClient GetClient(string clientName)
        {
            Tuple<TelemetryClient, TelemetryConfiguration> clientWithConfig;
            if (!_clientsAndConfigs.TryGetValue(clientName, out clientWithConfig))
            {
                throw new ArgumentException($"No client has been created with name \"{clientName}\"", nameof(clientName));
            }

            return clientWithConfig.Item1;
        }

        private static void ConfigureApplication(string instrumentationKey,
            TelemetryClient client, TelemetryConfiguration config, TelemetryInitializer initializer)
        {
            config.InstrumentationKey = instrumentationKey;
            config.TelemetryInitializers.Add(initializer);

            Application.ThreadException += (s, e) =>
            {
                client.TrackException(new ExceptionTelemetry(e.Exception)
                {
                    SeverityLevel = SeverityLevel.Critical,
                });

                throw e.Exception;
            };

            Application.ApplicationExit += (s, e) =>
            {
                client.Flush();
            };
        }

        /// <summary>
        /// Helper method providing easy access to logging a *handled* exception during program execution
        /// </summary>
        /// <param name="client">The telemetry client to use.</param>
        /// <param name="ex">The exception that has been handled</param>
        /// <param name="severityLevel">The severity level to assign to the logged event</param>
        /// <param name="message">The message.</param>
        /// <param name="data">Any extra data desired to be associated with the exception's log entry</param>
        public static void LogHandledException(this TelemetryClient client, Exception ex, SeverityLevel severityLevel = SeverityLevel.Error,
            string message = null, IDictionary<string, string> data = null)
        {
            var o = new ExceptionTelemetry(ex)
            {
                SeverityLevel = severityLevel
            };

            o.Properties.Add("LogMessage", message);
            foreach (var p in data ?? new Dictionary<string, string>())
            {
                o.Properties.Add(p);
            }

            client.TrackException(o);
        }

        /// <summary>
        /// Disables the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        public static void Disable(this TelemetryClient client) => _clientsAndConfigs.Single(c => c.Value.Item1 == client).Value.Item2.DisableTelemetry = true;

        /// <summary>
        /// Enables the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        public static void Enable(this TelemetryClient client) => _clientsAndConfigs.Single(c => c.Value.Item1 == client).Value.Item2.DisableTelemetry = false;
    }

    class TelemetryInitializer : ITelemetryInitializer
    {
        private readonly Assembly _sourceAssembly;

        private readonly string assemblyVersion;
        private readonly string languageTag;
        private readonly string operatingSystem;
        private readonly string screenResolutionData;
        private readonly string is64bitOS;
        private readonly string is64bitProcess;
        private readonly string machineName;
        private readonly string processorCount;
        private readonly string clrVersion;
        private readonly string sessionId;
        private readonly bool sessionIsFirst;
        private readonly string accountId;
        private readonly string userId;

        public TelemetryInitializer(Assembly sourceAssembly)
        {
            _sourceAssembly = sourceAssembly;
            assemblyVersion = _sourceAssembly.GetName().Version.ToString();
            languageTag = CultureInfo.CurrentUICulture.IetfLanguageTag;
            operatingSystem = Environment.OSVersion.ToString();
            screenResolutionData = GetScreenResolutionData();
            is64bitOS = Environment.Is64BitOperatingSystem.ToString();
            is64bitProcess = Environment.Is64BitProcess.ToString();
            machineName = Environment.MachineName;
            processorCount = Environment.ProcessorCount.ToString();
            clrVersion = Environment.Version.ToString();
            sessionId = DateTime.Now.ToFileTime().ToString();
            sessionIsFirst = true;
            accountId = Environment.UserDomainName;
            userId = Environment.UserName;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Component.Version = assemblyVersion;
            telemetry.Context.GlobalProperties.Add("Language", languageTag);
            telemetry.Context.Device.OperatingSystem = operatingSystem;
            telemetry.Context.GlobalProperties.Add("ScreenResolution", screenResolutionData);
            telemetry.Context.GlobalProperties.Add("64BitOS", is64bitOS);
            telemetry.Context.GlobalProperties.Add("64BitProcess", is64bitProcess);
            telemetry.Context.GlobalProperties.Add("Machine name", machineName);
            telemetry.Context.GlobalProperties.Add("ProcessorCount", processorCount);
            telemetry.Context.GlobalProperties.Add("ClrVersion", clrVersion);
            telemetry.Context.Session.Id = sessionId;
            telemetry.Context.Session.IsFirst = sessionIsFirst;
            telemetry.Context.User.AccountId = accountId;
            telemetry.Context.User.Id = userId;
        }

        private string GetScreenResolutionData()
        {
            if (Screen.AllScreens.Length > 1)
            {
                System.Text.StringBuilder screenData = new System.Text.StringBuilder();
                for (int i = 0; i < Screen.AllScreens.Length; i++)
                {
                    var screen = Screen.AllScreens[i];
                    screenData.AppendLine(
                        $"[{i.ToString()}] {screen.Bounds.Width.ToString()}x{screen.Bounds.Height.ToString()}");
                }
                return screenData.ToString();
            }
            else
            {
                return string.Concat(Screen.PrimaryScreen.Bounds.Width.ToString(), "x", Screen.PrimaryScreen.Bounds.Height.ToString());
            }
        }
    }
}
