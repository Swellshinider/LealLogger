using System.Collections.Immutable;
using LealLogger.Handlers;

namespace LealLogger;

/// <summary>
/// Base class for all loggers.
/// </summary>
public abstract class BaseLogger : ILogger, IDisposable
{
	/// <summary>
	/// Minimum log level to be logged.
	/// </summary>
	protected readonly LogLevel _minLogLevel;
	
	/// <summary>
	/// Maximum number of log entries that can be queued.
	/// </summary>
	protected readonly int _queueCapacity;
	
	/// <summary>
	/// Handlers to be used to log the messages.
	/// </summary>
	protected readonly ImmutableArray<LogHandler> _handlers;
	
	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="minLogLevel"> Minimum log level to be logged. </param>
	/// <param name="queueCapacity"> Maximum number of log entries that can be queued. </param>
	/// <param name="handlers"> Handlers to be used to log the messages. </param>
	public BaseLogger(LogLevel minLogLevel, int queueCapacity = 10_000, ImmutableArray<LogHandler> handlers = default) 
	{
		_minLogLevel = minLogLevel;
		_queueCapacity = queueCapacity;
		_handlers = handlers;
	}

	/// <inheritdoc/>
	public abstract void Debug(string message, Exception? ex = null);
	
	/// <inheritdoc/>
	public abstract void Error(string message, Exception? ex = null);
	
	/// <inheritdoc/>
	public abstract void Fatal(string message, Exception? ex = null);
	
	/// <inheritdoc/>
	public abstract void Info(string message, Exception? ex = null);
	
	/// <inheritdoc/>
	public abstract void Warn(string message, Exception? ex = null);
	
	/// <inheritdoc/>
	public abstract void Dispose();
}