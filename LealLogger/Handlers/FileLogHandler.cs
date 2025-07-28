using LealLogger.Enums;

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
    private readonly int _lineLength = 75;
    private readonly Guid _guid = Guid.NewGuid();
    private readonly FileRolling _fileRolling;
    private readonly string _pattern;

    private StreamWriter? _writer;
    private bool _firstLog = true;

    internal FileLogHandler(string filePath, LogLevel logLevel, string pattern, FileRolling fileRolling) : base(logLevel)
    {
        _filePath = filePath;
        _pattern = pattern;
        _fileRolling = fileRolling;

        if (string.IsNullOrWhiteSpace(_filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (!File.Exists(filePath))
        {
            _ = Directory.CreateDirectory(_filePath);
            File.Create(_filePath).Dispose();
        }

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
                _writer.WriteLine("╔" + new string('═', _lineLength - 2) + "╗");
                _writer.WriteLine(DrawBoxLine($"Log GUID       : {_guid}"));
                _writer.WriteLine(DrawBoxLine($"ApplicationName: {logEntry.ApplicationName}"));
                _writer.WriteLine(DrawBoxLine($"UserName       : {logEntry.UserName}"));
                _writer.WriteLine(DrawBoxLine($"MachineName    : {logEntry.MachineName}"));
                _writer.WriteLine("╚" + new string('═', _lineLength - 2) + "╝");
                _firstLog = false;
            }

            _writer.WriteLine($"[({logEntry.Timestamp:dd-MM-yyyy HH:mm:ss.ffff}) {logEntry.LogLevel}]: {logEntry.Message}");

            if (logEntry.Exception != null)
                _writer.WriteLine(FormatException(logEntry.Exception));
        }
    }

    private string DrawBoxLine(string text = "")
    {
        var padding = _lineLength - text.Length - 6;
        return $"║    {text}{new string(' ', padding)}║";
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        lock (_lock)
        {
            if (_writer != null)
            {
                _writer.WriteLine("╔" + new string('═', _lineLength - 2) + "╗");
                _writer.WriteLine(DrawBoxLine($"End GUID       : {_guid}"));
                _writer.WriteLine("╚" + new string('═', _lineLength - 2) + "╝");
            }

            _writer?.Dispose();
            _writer = null;
        }
    }
}
