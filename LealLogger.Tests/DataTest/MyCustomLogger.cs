using System.Collections.Immutable;
using LealLogger.Handlers;

namespace LealLogger.Tests.DataTest;

internal class MyCustomLogger : BaseLogger
{
	public MyCustomLogger(LogLevel minLogLevel, int queueCapacity, params ImmutableArray<LogHandler> handlers)
		: base(minLogLevel, queueCapacity, handlers) { }

	public override void Debug(string message, Exception? ex = null) { }
	public override void Error(string message, Exception? ex = null) { }
	public override void Fatal(string message, Exception? ex = null) { }
	public override void Info(string message, Exception? ex = null) { }
	public override void Warn(string message, Exception? ex = null) { }

	public override void Dispose() { }
}