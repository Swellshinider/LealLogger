namespace LealLogger;

/// <summary>
///	Defines a logger that can log messages at different levels.
/// </summary>
public interface ILogger
{
	/// <summary>
	/// Logs a debug-level message.
	/// </summary>
	/// <param name="message">The message to log.</param>
	/// <param name="ex">An optional exception to include with the log.</param>
	void Debug(string message, Exception? ex = null);

	/// <summary>
	/// Logs an informational message.
	/// </summary>
	/// <param name="message">The message to log.</param>
	/// <param name="ex">An optional exception to include with the log.</param>
	void Info(string message, Exception? ex = null);

	/// <summary>
	/// Logs a warning message.
	/// </summary>
	/// <param name="message">The message to log.</param>
	/// <param name="ex">An optional exception to include with the log.</param>
	void Warn(string message, Exception? ex = null);

	/// <summary>
	/// Logs an error message.
	/// </summary>
	/// <param name="message">The message to log.</param>
	/// <param name="ex">An optional exception to include with the log.</param>
	void Error(string message, Exception? ex = null);

	/// <summary>
	/// Logs a critical error or fatal message.
	/// </summary>
	/// <param name="message">The message to log.</param>
	/// <param name="ex">An optional exception to include with the log.</param>
	void Fatal(string message, Exception? ex = null);
}