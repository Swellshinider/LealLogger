namespace LealLogger.Handlers;

public sealed class FileLogHandler : LogHandler
{
	private readonly string _filePath;
	private readonly Lock _lock = new();
	private StreamWriter? _writer;

	internal FileLogHandler(string filePath, LogLevel logLevel) : base(logLevel)
	{
		_filePath = filePath;
		_writer = new StreamWriter(File.Open(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
		{
			AutoFlush = true
		};
	}

	public override void HandleLog(Log logEntry)
	{
		if (_writer == null || logEntry.LogLevel < MinimumLogLevel)
			return;

		lock (_lock)
		{
			_writer.WriteLine($"[({logEntry.Timestamp:dd-MM-yyyy HH:mm:ss.ffff}) {logEntry.LogLevel}]: {logEntry.Message}");
			_writer.WriteLine($"├─── ApplicationName: {logEntry.ApplicationName}");
			_writer.WriteLine($"├─── UserName       : {logEntry.UserName}");
			_writer.WriteLine($"├─── MachineName    : {logEntry.MachineName}");
			_writer.WriteLine($"├─── ProcessId      : {logEntry.ProcessId}");
			_writer.WriteLine($"└─── ThreadId       : {logEntry.ThreadId}");

			if (logEntry.Exception != null)
				_writer.WriteLine(FormatException(logEntry.Exception));
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
