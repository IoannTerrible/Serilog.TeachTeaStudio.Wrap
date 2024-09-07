using Serilog;
using Serilog.Events;

using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoggerLibrary
{
    public class Logger
    {
        private readonly ILoggerConfig _loggerConfig;

        public Logger(ILoggerConfig loggerConfig)
        {
            _loggerConfig = loggerConfig;
        }

        public async Task LogEventAsync(LogEventLevel logEventLevel, string message, Exception? ex = null)
        {
            var info = new StringBuilder($"Message: {message}");

            if (logEventLevel == LogEventLevel.Debug)
            {
                var assemblyVersion = GetAssemblyVersion();
                if (!string.IsNullOrEmpty(assemblyVersion))
                {
                    info.Append($" AssemblyVersion: {assemblyVersion}");
                }
            }

            if (ex != null)
            {
                info.AppendLine($" Exception: {ex.Message}");
                var stackTrace = new StackTrace(ex, true);
                var frame = GetRelevantFrame(stackTrace);
                if (frame != null)
                {
                    string fileName = Path.GetFileName(frame.GetFileName());
                    info.AppendLine($" File: {fileName}, Line: {frame.GetFileLineNumber()}, Column: {frame.GetFileColumnNumber()}, Method: {frame.GetMethod()}");
                }
            }

            await Task.Run(() => Log.Write(logEventLevel, info.ToString()));
        }

        private static StackFrame? GetRelevantFrame(StackTrace stackTrace)
        {
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                var frame = stackTrace.GetFrame(i);
                if (frame.GetFileLineNumber() != 0)
                {
                    return frame;
                }
            }
            return null;
        }

        private static string? GetAssemblyVersion()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly();
                var version = assembly?.GetName().Version;
                return version?.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get assembly version");
                return null;
            }
        }
    }
}
