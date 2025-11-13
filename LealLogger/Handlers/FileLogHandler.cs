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
    
#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#else
    private readonly object _lock = new();
#endif

    private readonly int _lineLength = 75;
    private readonly Guid _guid = Guid.NewGuid();
    private readonly FileRolling _fileRolling;
    private readonly string _pattern;

    private StreamWriter? _writer;

    internal FileLogHandler(string filePath, LogLevel logLevel, string pattern, FileRolling fileRolling, out string finalFilePath) : base(logLevel)
    {
        _filePath = filePath;
        _pattern = pattern;
        _fileRolling = fileRolling;

        if (string.IsNullOrWhiteSpace(_filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        finalFilePath = CreateFile();

        if (_writer == null)
            throw new InvalidOperationException("Failed to create a StreamWriter for the log file.");

        _writer.WriteLine($"::.{_guid}.::");
        _writer.WriteLine($"Date & Time: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        _writer.WriteLine($"Application: {AppDomain.CurrentDomain.FriendlyName}");
        _writer.WriteLine($"UserName   : {Environment.UserName}");
        _writer.WriteLine($"MachineName: {Environment.MachineName}\n");
    }

    private string CreateFile()
    {
        var fileDirectoryInfo = Directory.CreateDirectory(_filePath);
        var directory = fileDirectoryInfo.Parent ?? throw new DirectoryNotFoundException($"The directory for the file path '{_filePath}' does not exist.");
        var fileExtension = Path.GetExtension(_filePath);
        var files = directory.GetFiles().Where(p => p.Extension == fileExtension).ToList();
        var hasAnyFiles = files.Count != 0;

        FileInfo? recentFile = null;

        if (hasAnyFiles)
        {
            recentFile = directory
                .GetFiles($"{Path.GetFileNameWithoutExtension(_filePath)}_*{fileExtension}")
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault();
        }

        var fileName = Path.GetFileNameWithoutExtension(_filePath);
        var timestamp = DateTime.Now.ToString(_pattern);

        // Create the first file
        if (!hasAnyFiles || recentFile == null)
        {
            var newFilePath = Path.Combine(directory.FullName, $"{fileName}_{timestamp}{fileExtension}");
            File.Create(newFilePath).Dispose();
            _writer = new StreamWriter(newFilePath, false) { AutoFlush = true };
            return newFilePath;
        }

        var createdNew = _fileRolling switch
        {
            FileRolling.Infinite => false,
            FileRolling.ByExecution => true,
            FileRolling.Hourly => DateTime.Now.Subtract(recentFile.LastWriteTime).TotalHours >= 1,
            FileRolling.EachTwoHours => DateTime.Now.Subtract(recentFile.LastWriteTime).TotalHours >= 2,
            FileRolling.EachThreeHours => DateTime.Now.Subtract(recentFile.LastWriteTime).TotalHours >= 3,
            FileRolling.EachFourHours => DateTime.Now.Subtract(recentFile.LastWriteTime).TotalHours >= 4,
            FileRolling.EachSixHours => DateTime.Now.Subtract(recentFile.LastWriteTime).TotalHours >= 6,
            FileRolling.EachTwelveHours => DateTime.Now.Subtract(recentFile.LastWriteTime).TotalHours >= 12,
            FileRolling.Daily => DateTime.Now.Date > recentFile.LastWriteTime.Date,
            FileRolling.Weekly => DateTime.Now.Date > recentFile.LastWriteTime.Date.AddDays(-(int)DateTime.Now.DayOfWeek),
            FileRolling.Monthly => DateTime.Now.Year != recentFile.LastWriteTime.Year || DateTime.Now.Month != recentFile.LastWriteTime.Month,
            FileRolling.Yearly => DateTime.Now.Year != recentFile.LastWriteTime.Year,
            _ => throw new NotSupportedException($"File rolling strategy '{_fileRolling}' is not supported."),
        };

        var finalFilePath = createdNew
            ? Path.Combine(fileDirectoryInfo.FullName, $"{fileName}_{timestamp}{fileExtension}")
            : recentFile.FullName;

        _writer = new StreamWriter(finalFilePath!, !createdNew) { AutoFlush = true };
        return finalFilePath;
    }

    private void LogHeader()
    {
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
            _writer?.WriteLine(new string('═', _lineLength));
            _writer?.Dispose();
            _writer = null;
        }
    }
}
