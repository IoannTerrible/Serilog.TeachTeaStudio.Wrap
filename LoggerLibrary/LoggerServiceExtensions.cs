using Microsoft.Extensions.DependencyInjection;

namespace LoggerLibrary
{
    public static class LoggerServiceExtensions
    {
        public static IServiceCollection AddLogger(this IServiceCollection services, Action<LoggerOptions>? configureOptions = null)
        {
            var options = new LoggerOptions();
            configureOptions?.Invoke(options);

            services.AddSingleton<ILoggerConfig>(provider =>
            {
                var loggerConfig = new LoggerConfig();
                loggerConfig.Configure(options);
                return loggerConfig;
            });

            services.AddSingleton<Logger>();

            return services;
        }
    }
}
