namespace LealLogger.Handlers;

/// <summary>
/// Handles logging to a file by writing log entries to a specified file path.
/// </summary>
/// <remarks>
/// The FileLogHandler writes log entries to a file in a thread-safe manner.
/// On the first log entry, it writes header information including application details.
/// Subsequent entries include timestamp, log level, message and exception details if present.
/// </remarks>
public sealed class FileLogHandler : LogHandler
{
	private readonly string _filePath;
	private readonly Lock _lock = new();
	private StreamWriter? _writer;
	private bool _firstLog = true;
	
	internal FileLogHandler(string filePath, LogLevel logLevel) : base(logLevel)
	{
		_filePath = filePath;
		_writer = new StreamWriter(File.Open(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
		{
			AutoFlush = true
		};
	}

	/// <summary>
	/// Handles a log entry by writing it to the file.
	/// </summary>
	/// <param name="logEntry"> The log entry to handle. </param>
	public override void HandleLog(Log logEntry)
	{
		if (_writer == null || logEntry.LogLevel < MinimumLogLevel)
			return;

		lock (_lock)
		{
			if (_firstLog)
			{
				_writer.WriteLine($"===============================================================");
				_writer.WriteLine($"├─── Date: {DateTime.Now:dd-MM-yyyy}");
				_writer.WriteLine($"├─── ApplicationName: {logEntry.ApplicationName}");
				_writer.WriteLine($"├─── UserName       : {logEntry.UserName}");
				_writer.WriteLine($"├─── MachineName    : {logEntry.MachineName}");
				_writer.WriteLine($"├─── ProcessId      : {logEntry.ProcessId}");
				_writer.WriteLine($"└─── ThreadId       : {logEntry.ThreadId}\n");
			}
			
			_writer.WriteLine($"[({logEntry.Timestamp:dd-MM-yyyy HH:mm:ss.ffff}) {logEntry.LogLevel}]: {logEntry.Message}");
			
			if (logEntry.Exception != null)
				_writer.WriteLine(FormatException(logEntry.Exception));
		}
	}

	/// <inheritdoc />
	public override void Dispose()
	{
		lock (_lock)
		{
			_writer?.Dispose();
			_writer = null;
		}
	}
}
