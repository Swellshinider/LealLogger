namespace LealLogger.Handlers;

public sealed class FileLogHandler : LogHandler
{
	private readonly string _filePath;
	private readonly Lock _lock = new();
	private StreamWriter? _writer;

	public FileLogHandler(string filePath)
	{
		_filePath = filePath;
		_writer = new StreamWriter(File.Open(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
		{
			AutoFlush = true
		};
	}

	public override void HandleLog(Log logEntry)
	{
		if (_writer == null) 
			return;

		lock (_lock)
		{
			// Minimal string manipulation to reduce overhead
			_writer.Write($"[{logEntry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}]");
			_writer.Write($" [{logEntry.LogLevel}]");
			_writer.Write($" [{logEntry.ThreadId}] ");
			_writer.WriteLine(logEntry.Message);

			if (logEntry.Exception != null)
			{
				_writer.WriteLine(logEntry.Exception.ToString());
			}
		}
	}

	public override void Dispose()
	{
		lock (_lock)
		{
			_writer?.Dispose();
			_writer = null;
		}
	}
}
