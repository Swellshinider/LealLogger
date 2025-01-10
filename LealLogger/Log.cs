namespace LealLogger;

/// <summary>
/// Represents a log entry with various details such as log level, message, exception, timestamp, and environment information.
/// </summary>
public sealed class Log
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Log"/> class.
	/// </summary>
	/// <param name="logLevel">The level of the log entry.</param>
	/// <param name="message">The message of the log entry.</param>
	/// <param name="exception">The exception associated with the log entry, if any.</param>
	public Log(LogLevel logLevel, string message, Exception? exception = null)
	{
		LogLevel = logLevel;
		Message = message;
		Exception = exception;
	}

	/// <summary>
	/// Gets the level of the log entry.
	/// </summary>
	public LogLevel LogLevel { get; }

	/// <summary>
	/// Gets the message of the log entry.
	/// </summary>
	public string Message { get; }

	/// <summary>
	/// Gets the exception associated with the log entry, if any.
	/// </summary>
	public Exception? Exception { get; }

	/// <summary>
	/// Gets the timestamp of when the log entry was created.
	/// </summary>
	public DateTime Timestamp { get; } = DateTime.Now;

	/// <summary>
	/// Gets the name of the machine where the log entry was created.
	/// </summary>
	public string MachineName { get; } = Environment.MachineName;

	/// <summary>
	/// Gets the ID of the thread that created the log entry.
	/// </summary>
	public int ThreadId { get; } = Environment.CurrentManagedThreadId;

	/// <summary>
	/// Gets the name of the application domain where the log entry was created.
	/// </summary>
	public string ApplicationName { get; } = AppDomain.CurrentDomain.FriendlyName;

	/// <summary>
	/// Gets the name of the user who created the log entry.
	/// </summary>
	public string UserName { get; } = Environment.UserName;

	/// <summary>
	/// Gets the ID of the process that created the log entry.
	/// </summary>
	public int ProcessId { get; } = Environment.ProcessId;
}