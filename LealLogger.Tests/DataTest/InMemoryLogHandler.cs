using System.Collections.Concurrent;
using LealLogger.Handlers;

namespace LealLogger.Tests.DataTest;

/// <summary>
/// A simple log handler that stores all logs in an in-memory list for verification.
/// </summary>
internal sealed class InMemoryLogHandler : LogHandler
{
	private readonly ConcurrentBag<Log> _logs = [];
	public IReadOnlyCollection<Log> Logs => _logs;

	public override void HandleLog(Log logEntry)
	{
		_logs.Add(logEntry);
	}

	public override void Dispose() { }
}