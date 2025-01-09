using System.Collections.Concurrent;
using System.Collections.Immutable;
using LealLogger.Handlers;

namespace LealLogger;

/// <summary>
/// Provides asynchronous, thread-safe logging with support for multiple log handlers and configurable log levels.
/// </summary>
public class Logger : BaseLogger
{
	private readonly BlockingCollection<Log> _logQueue;
	private readonly CancellationTokenSource _cts;
	private readonly Task _consumerTask;
	private bool _disposed = false;

	/// <summary>
	/// Initializes a new instance of the <see cref="Logger"/> class.
	/// </summary>
	/// <param name="minLogLevel">The minimum <see cref="LogLevel"/> required for logs to be processed.</param>
	/// <param name="queueCapacity">The maximum number of log entries that can be queued. Default is 10,000.</param>
	/// <param name="handlers">An array of <see cref="ILogHandler"/> instances responsible for handling logs.</param>
	internal Logger(LogLevel minLogLevel, int queueCapacity, params ImmutableArray<LogHandler> handlers)
		: base(minLogLevel, queueCapacity, handlers)
	{
		_cts = new CancellationTokenSource();
		_logQueue = new BlockingCollection<Log>(boundedCapacity: queueCapacity);

		// Start consumer task in background
		_consumerTask = Task.Run(() => ConsumeLogs(_cts.Token));
	}

	#region [ Public Methods ]
	public override void Debug(string message, Exception? ex = null) => EnqueueLog(LogLevel.DEBUG, message, ex);
	public override void Info(string message, Exception? ex = null) => EnqueueLog(LogLevel.INFO, message, ex);
	public override void Warn(string message, Exception? ex = null) => EnqueueLog(LogLevel.WARN, message, ex);
	public override void Error(string message, Exception? ex = null) => EnqueueLog(LogLevel.ERROR, message, ex);
	public override void Fatal(string message, Exception? ex = null) => EnqueueLog(LogLevel.FATAL, message, ex);
	#endregion

	#region [ Private Methods ]
	/// <summary>
	/// Adds a log entry to the processing queue if its level meets the minimum log level.
	/// </summary>
	/// <param name="level">The severity level of the log.</param>
	/// <param name="message">The message to log.</param>
	/// <param name="ex">An optional exception to include with the log.</param>
	private void EnqueueLog(LogLevel level, string message, Exception? ex)
	{
		if (level < _minLogLevel)
			return;

		var logEntry = new Log(level, message, ex);

		try
		{
			// If queue is full, blocks until there's space.
			_logQueue.Add(logEntry);
		}
		catch (InvalidOperationException) { } // Thrown if the collection has been marked as completed for adding.
	}
	
	/// <summary>
	/// Processes log entries from the queue and sends them to the configured handlers.
	/// </summary>
	/// <param name="token">The cancellation token to observe.</param>
	private void ConsumeLogs(CancellationToken token)
	{
		try
		{
			while (!token.IsCancellationRequested)
			{
				var logEntry = _logQueue.Take(token);

				foreach (var handler in _handlers)
					handler.HandleLog(logEntry);
			}
		}
		catch (OperationCanceledException) { } // Normal cancellation
		catch (Exception ex)
		{
			Console.WriteLine($"Exception in log consumer: {ex}");
		}
		finally
		{
			// Process remaining logs
			while (_logQueue.TryTake(out var leftoverLog))
				foreach (var handler in _handlers)
					handler.HandleLog(leftoverLog);
		}
	}
	#endregion

	#region [ Dispose ]
	public override void Dispose()
	{
		if (_disposed)
			return;

		_disposed = true;
		_cts.Cancel();

		_logQueue.CompleteAdding();

		// Wait task to finish
		_consumerTask.Wait();

		// Dispose handlers
		foreach (var handler in _handlers)
			handler.Dispose();

		_cts.Dispose();
		_logQueue.Dispose();
		GC.SuppressFinalize(this);
	}
	#endregion
}