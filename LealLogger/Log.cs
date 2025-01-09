namespace LealLogger;

public sealed class Log 
{
	public Log(LogLevel logLevel, string message, Exception? exception = null)
	{
		LogLevel = logLevel;
		Message = message;
		Exception = exception;
	}
	
	public LogLevel LogLevel { get; }
	public string Message { get; }
	public Exception? Exception { get; }
	public DateTime Timestamp { get; } = DateTime.Now;
	public string MachineName { get; } = Environment.MachineName;
	public int ThreadId { get; }  = Environment.CurrentManagedThreadId;
	public string ApplicationName { get; } = AppDomain.CurrentDomain.FriendlyName;
	public string UserName { get; } = Environment.UserName;
	public int ProcessId { get; } = Environment.ProcessId;
}