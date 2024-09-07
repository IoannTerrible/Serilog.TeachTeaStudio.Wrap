using Serilog;

namespace LoggerLibrary
{
    public class LoggerConfig : ILoggerConfig
    {
        public void Configure(LoggerOptions options)
        {
            if (!Directory.Exists(options.LogDirectory))
            {
                Directory.CreateDirectory(options.LogDirectory);
                Log.Information("Created logs directory: {LogDirectory}", options.LogDirectory);
            }

            var loggerConfig = new LoggerConfiguration().MinimumLevel.Verbose();
            foreach (var logEventLevel in options.LogEventLevels)
            {
                string logFileName = $"{logEventLevel.ToString().ToLower()}Log.txt";
                loggerConfig.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(evt => evt.Level == logEventLevel)
                    .WriteTo.File(Path.Combine(options.LogDirectory, logFileName),
                        rollingInterval: options.RollingInterval,
                        outputTemplate: options.OutputTemplate)
                );
            }

            Log.Logger = loggerConfig.CreateLogger();
        }
    }
}
