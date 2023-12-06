using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using webapi_serilog.Constants;
using webapi_serilog.Extensions;
using webapi_serilog.Services.Interface;
using webapi_serilog.Services;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

bool isUseDefaultLogger = configuration.GetValue<bool>(AppSettingsConstants.IsUseDefaultLogger);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (!isUseDefaultLogger)
{
    builder.Services.AddSingleton<ILoggingLevelSwitchService, LoggingLevelSwitchService>();

    bool isAppInsightSettingsConfigured =
        !string.IsNullOrEmpty(configuration[AppSettingsConstants.AppInsightConnectionStringEnvironmentVariable]) &&
        !string.IsNullOrEmpty(configuration[AppSettingsConstants.AppInsightInstrumentationKeyForWebSites]);

    builder.Host.AddSerilogLogging(configuration, isAppInsightSettingsConfigured);
}

var app = builder.Build();

if (!isUseDefaultLogger)
{
    app.UseSerilogRequestLogging(options =>
    {
        // Customize the message template
        options.MessageTemplate = "Handled {RequestPath} RequestBody {RequestBody} RequestHeaders {RequestHeaders} RequestScheme {RequestScheme}";

        // Emit debug-level events instead of the defaults
        options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;

        // Attach additional properties to the request completion event
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestBody", httpContext.Request.Body);
            diagnosticContext.Set("RequestHeaders", httpContext.Request.Headers);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
