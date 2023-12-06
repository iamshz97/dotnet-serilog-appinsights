namespace webapi_serilog.Extensions;

using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using webapi_serilog.Constants;

public static class ConfigureSerilogLogging
{
    public static IHostBuilder AddSerilogLogging(this IHostBuilder builder, IConfiguration configuration, bool isAppInsightSettingsConfigured)
    {
        builder.UseSerilog((hostingContext, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .WriteTo.Console(new CompactJsonFormatter())
                .WriteTo.Debug();

            var logLevelConfigurations = configuration.GetSection(AppSettingsConstants.LogLevel).GetChildren();

            loggerConfiguration.MinimumLevel.Is(LogEventLevel.Warning);

            foreach (IConfigurationSection logLevelConfiguration in logLevelConfigurations)
            {
                var configKey = logLevelConfiguration.Key;

                // Azure App Service replaces . with _ in the configuration key so we need to put the . back
                if (configKey.Contains("_"))
                {
                    configKey = configKey.Replace("_", ".");
                }

                if (Enum.TryParse(logLevelConfiguration.Value, ignoreCase: true, out LogEventLevel result))
                {
                    if (configKey == AppSettingsConstants.Default)
                    {
                        loggerConfiguration.MinimumLevel.Is(result);
                        continue;
                    }

                    loggerConfiguration.MinimumLevel.Override(configKey, result);
                }
            }

            if (isAppInsightSettingsConfigured)
            {
                loggerConfiguration.WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces);
            }
        });

        return builder;
    }
}
