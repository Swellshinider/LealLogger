using System.Text;

namespace LealLogger.Handlers;

/// <summary>
/// Abstract class that handles logging functionality.
/// </summary>
public abstract class LogHandler : ILogHandler
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LogHandler"/> class with the specified minimum log level.
	/// </summary>
	/// <param name="minimumLogLevel">The minimum log level for this log handler. Defaults to DEBUG</param>
	public LogHandler(LogLevel minimumLogLevel = LogLevel.DEBUG)
	{
		MinimumLogLevel = minimumLogLevel;
	}

	/// <summary>
	/// Gets or sets the minimum log level for this log handler.
	/// </summary>
	public LogLevel MinimumLogLevel { get; set; }

	/// <summary>
	/// Handles the log entry.
	/// </summary>
	/// <param name="logEntry">The log entry to handle.</param>
	public abstract void HandleLog(Log logEntry);

	/// <summary>
	/// Disposes the resources used by the log handler.
	/// </summary>
	public abstract void Dispose();

	/// <summary>
	/// Formats the exception details into a readable string.
	/// </summary>
	/// <param name="exception">The exception to format.</param>
	/// <param name="indentation">The indentation to use for formatting.</param>
	/// <param name="depth">The depth of the inner exception.</param>
	/// <returns>A formatted string representing the exception details.</returns>
	protected static string FormatException(Exception exception, string indentation = "    ", int depth = 0)
	{
		var stringBuilder = new StringBuilder();

		stringBuilder.AppendLine($"{indentation}{(depth == 0 ? "Exception" : "InnerException")}: {nameof(exception)}");
		stringBuilder.AppendLine($"{indentation}Message: {exception.Message}");
		stringBuilder.AppendLine($"{indentation} Source: {exception.Source}");

		if (exception.InnerException != null && depth < 3)
			stringBuilder.AppendLine(FormatException(exception.InnerException, indentation += "    ", ++depth));

		return stringBuilder.ToString();
	}
}