namespace LealLogger.Handlers;

public class ConsoleLogHandler : LogHandler
{
	internal ConsoleLogHandler(LogLevel logLevel) : base(logLevel) { }

	public override void HandleLog(Log logEntry)
	{
		if (logEntry.LogLevel < MinimumLogLevel)
			return;
		
		var originalColor = Console.ForegroundColor;
		Console.ForegroundColor = GetColorForLevel(logEntry.LogLevel);
		Console.Write($"[({logEntry.Timestamp:dd-MM-yyyy HH:mm:ss.ffff}) {logEntry.LogLevel}]: ");
		Console.ForegroundColor = originalColor;
		Console.WriteLine(logEntry.Message);

		if (logEntry.Exception != null)
			Console.WriteLine(FormatException(logEntry.Exception!));
	}

	private static ConsoleColor GetColorForLevel(LogLevel level) => level switch
	{
		LogLevel.DEBUG => ConsoleColor.DarkGray,
		LogLevel.INFO => ConsoleColor.Green,
		LogLevel.WARN => ConsoleColor.Yellow,
		LogLevel.ERROR => ConsoleColor.Red,
		LogLevel.FATAL => ConsoleColor.Magenta,
		_ => ConsoleColor.White
	};

	public override void Dispose()
	{
		GC.SuppressFinalize(this);
	}
}