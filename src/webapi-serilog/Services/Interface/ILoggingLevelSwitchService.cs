using Serilog.Core;
using Serilog.Events;

namespace webapi_serilog.Services.Interface;

public interface ILoggingLevelSwitchService
{
    LoggingLevelSwitch LevelSwitch { get; }
    void SetLogLevel(LogEventLevel level);
}