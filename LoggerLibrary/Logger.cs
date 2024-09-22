using System.Diagnostics;
using System.Reflection;
using System.Text;
using Serilog;
using Serilog.Events;

namespace LoggerLibrary;

public class Logger
{
	private readonly ILoggerConfig _loggerConfig;

	public Logger(ILoggerConfig loggerConfig)
	{
		_loggerConfig = loggerConfig ?? throw new ArgumentNullException(nameof(loggerConfig));
	}

	public async Task LogEventAsync(
		LogEventLevel logEventLevel,
		string message,
		Exception? ex = null
	)
	{
		var logMessage = BuildLogMessage(logEventLevel, message, ex);
		await Task.Run(() => Log.Write(logEventLevel, logMessage));
	}

	public void LogEvent(LogEventLevel logEventLevel, string message, Exception? ex = null)
	{
		var logMessage = BuildLogMessage(logEventLevel, message, ex);
		Log.Write(logEventLevel, logMessage);
	}

	private static string BuildLogMessage(
		LogEventLevel logEventLevel,
		string message,
		Exception? ex
	)
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
			var frame = GetRelevantFrame(new StackTrace(ex, true));
			if (frame != null)
			{
				string? fileName = Path.GetFileName(frame.GetFileName());
				info.AppendLine(
					$" File: {fileName}, Line: {frame.GetFileLineNumber()}, Column: {frame.GetFileColumnNumber()}, Method: {frame.GetMethod()}"
				);
			}
		}

		return info.ToString();
	}

	private static StackFrame? GetRelevantFrame(StackTrace stackTrace)
	{
		foreach (var frame in stackTrace.GetFrames() ?? Enumerable.Empty<StackFrame>())
		{
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
			return assembly?.GetName().Version?.ToString();
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Failed to get assembly version");
			return null;
		}
	}
}
