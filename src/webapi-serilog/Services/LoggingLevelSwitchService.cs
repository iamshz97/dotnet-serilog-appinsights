using Serilog.Core;
using Serilog.Events;
using webapi_serilog.Services.Interface;

namespace webapi_serilog.Services;

public class LoggingLevelSwitchService : ILoggingLevelSwitchService
{
    private readonly LoggingLevelSwitch _levelSwitch = new LoggingLevelSwitch();

    public LoggingLevelSwitch LevelSwitch => _levelSwitch;

    public void SetLogLevel(LogEventLevel level)
    {
        _levelSwitch.MinimumLevel = level;
    }
}