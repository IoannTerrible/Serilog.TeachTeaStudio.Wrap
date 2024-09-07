using Serilog;
using Serilog.Events;

namespace LoggerLibrary
{
    public class LoggerOptions
    {
        public string LogDirectory { get; set; } = "logs";
        public LogEventLevel[] LogEventLevels { get; set; } = new[]
        {
            LogEventLevel.Information,
            LogEventLevel.Warning,
            LogEventLevel.Error,
            LogEventLevel.Fatal
        };
        public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;
        public string OutputTemplate { get; set; } = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message:lj} {NewLine}{Exception}";
    }
}
