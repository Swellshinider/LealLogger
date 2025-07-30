using System.Collections.Immutable;
using LealLogger.Enums;
using LealLogger.Handlers;

namespace LealLogger.Factory;

/// <summary>
/// A builder class for configuring and creating <see cref="BaseLogger"/> instances.
/// Allows chaining methods to specify various settings, such as log level, queue capacity,
/// and which <see cref="LogHandler"/> implementations to use.
/// </summary>
public sealed class LoggerBuilder
{
	private readonly List<LogHandler> _handlers;
	private int _queueCapacity;
	private LogLevel _logLevel;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerBuilder"/> class 
    /// with default settings. By default:
    /// <list type="number">
    ///   <item><description><c>QueueCapacity</c> is <c>1000</c>.</description></item>
    ///   <item><description><c>MinimumLogLevel</c> is <see cref="LogLevel.DEBUG"></see>.</description></item>
    ///   <item><description><c>Handlers</c> starts empty.</description></item>
    /// </list>
    /// </summary>
    public LoggerBuilder()
	{
		_handlers = [];
		_queueCapacity = 1_000;
		_logLevel = LogLevel.DEBUG;
	}
	
	/// <summary>
	/// Getter for the current _logLevel value
	/// </summary>
	public LogLevel MinimumLogLevel { get => _logLevel; }
	
	/// <summary>
	/// Getter for the _queueCapacity value
	/// </summary>
	public int QueueCapacity { get => _queueCapacity; }
	
	/// <summary>
	/// Getter for the _handlers value
	/// </summary>
	public List<LogHandler> Handlers { get => _handlers; }

	/// <summary>
	/// Adds a <see cref="ConsoleLogHandler"/> to the collection of log handlers.
	/// </summary>
	/// <param name="logLevel">The minimum log level for this log handler. Defaults to DEBUG</param>
	/// <returns>
	/// This <see cref="LoggerBuilder"/> instance for method chaining.
	/// </returns>
	public LoggerBuilder AddConsoleHandler(LogLevel logLevel = LogLevel.DEBUG)
		=> AddHandler(new ConsoleLogHandler(logLevel));

    /// <summary>
    /// Adds a <see cref="FileLogHandler"/> to the collection of log handlers.
    /// </summary>
    /// <param name="filePath">The path to the file where logs will be written.</param>
    /// <param name="finalFilePath">The path to the file where logs will be written in it's final version.</param>
    /// <param name="timeSpanPattern">The time span patter that will be appended into the filename. Defaults to "dd-MM-yyyy-HHmm"</param>
    /// <param name="fileRolling">Defines the strategies used to roll log files. Defaults to Daily</param>
    /// <param name="logLevel">The minimum log level for this log handler. Defaults to DEBUG</param>
    /// <returns>
    /// This <see cref="LoggerBuilder"/> instance for method chaining.
    /// </returns>
    public LoggerBuilder AddFileHandler(string filePath, out string finalFilePath, string timeSpanPattern = "dd-MM-yyyy-HHmm", FileRolling fileRolling = FileRolling.Daily, LogLevel logLevel = LogLevel.DEBUG)
		=> AddHandler(new FileLogHandler(filePath, logLevel, timeSpanPattern, fileRolling, out finalFilePath));

	/// <summary>
	/// Adds a custom <see cref="LogHandler"/> to the collection of log handlers.
	/// </summary>
	/// <typeparam name="T">A class derived from <see cref="LogHandler"/>.</typeparam>
	/// <param name="handler">An instance of <typeparamref name="T"/> to add.</param>
	/// <returns>
	/// This <see cref="LoggerBuilder"/> instance for method chaining.
	/// </returns>
	public LoggerBuilder AddHandler<T>(T handler) where T : LogHandler
	{
		_handlers.Add(handler);
		return this;
	}

	/// <summary>
	/// Sets the minimum <see cref="LogLevel"/> for the logger. Only messages at or above this level will be processed.
	/// </summary>
	/// <param name="logLevel">The desired <see cref="LogLevel"/> (e.g., <c>DEBUG</c>, <c>INFO</c>, <c>ERROR</c>, etc.).</param>
	/// <returns>
	/// This <see cref="LoggerBuilder"/> instance for method chaining.
	/// </returns>
	public LoggerBuilder SetMinimumLogLevel(LogLevel logLevel)
	{
		_logLevel = logLevel;
		return this;
	}

	/// <summary>
	/// Sets the queue capacity for log messages. When the internal queue reaches this capacity, 
	/// logging calls may block until space is available.
	/// </summary>
	/// <param name="queueCapacity">The maximum number of log messages that can be queued before blocking.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="queueCapacity"/> is less than 1.</exception>
	/// <returns>
	/// This <see cref="LoggerBuilder"/> instance for method chaining.
	/// </returns>
	public LoggerBuilder SetQueueCapacity(int queueCapacity)
	{
		if (queueCapacity < 1)
			throw new ArgumentException("Queue capacity must be greater than 0.", nameof(queueCapacity));

		_queueCapacity = queueCapacity;
		return this;
	}

	/// <summary>
	/// Builds and returns the default <see cref="Logger"/> implementation of <see cref="BaseLogger"/>.
	/// Uses the current builder configuration for log level, queue capacity, and handlers.
	/// </summary>
	/// <returns>
	/// A <see cref="Logger"/> (derived from <see cref="BaseLogger"/>), 
	/// configured with this builder's settings.
	/// </returns>
	public Logger Build() => new(_logLevel, _queueCapacity, [.. _handlers]);

	/// <summary>
	/// Builds and returns a user-defined class <typeparamref name="T"/> (derived from <see cref="BaseLogger"/>),
	/// by looking for a constructor with the signature 
	/// (<see cref="LogLevel"/>, <see cref="int"/>, <see cref="ImmutableArray{LogHandler}"/>).
	/// </summary>
	/// <typeparam name="T">A class that inherits from <see cref="BaseLogger"/>.</typeparam>
	/// <exception cref="MissingMethodException">
	/// Thrown if the <typeparamref name="T"/> type does not define a public constructor matching 
	/// <c>(LogLevel, int, ImmutableArray&lt;LogHandler&gt;)</c>.
	/// </exception>
	/// <returns>
	/// An instance of <typeparamref name="T"/>, constructed with this builder's current log level, 
	/// queue capacity, and handlers.
	/// </returns>
	public T Build<T>() where T : BaseLogger
	{
		// Extract expected constructor
		var ctor = typeof(T).GetConstructor([typeof(LogLevel), typeof(int), typeof(ImmutableArray<LogHandler>)])
			?? throw new MissingMethodException($"Constructor with signature (LogLevel, int, ImmutableArray<LogHandler>) not found in type {typeof(T).FullName}.");

		// Return the invoked constructor casting by T
		return (T)ctor.Invoke([_logLevel, _queueCapacity, _handlers.ToImmutableArray()]);
	}
}