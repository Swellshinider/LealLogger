using System.Collections.Immutable;
using LealLogger.Handlers;

namespace LealLogger;

public abstract class BaseLogger : ILogger, IDisposable
{
	protected readonly LogLevel _minLogLevel;
	protected readonly int _queueCapacity;
	protected readonly ImmutableArray<LogHandler> _handlers;
	
	public BaseLogger(LogLevel minLogLevel, int queueCapacity = 10_000, params ImmutableArray<LogHandler> handlers) 
	{
		_minLogLevel = minLogLevel;
		_queueCapacity = queueCapacity;
		_handlers = handlers;
	}

	public abstract void Debug(string message, Exception? ex = null);
	public abstract void Error(string message, Exception? ex = null);
	public abstract void Fatal(string message, Exception? ex = null);
	public abstract void Info(string message, Exception? ex = null);
	public abstract void Warn(string message, Exception? ex = null);
	public abstract void Dispose();
}