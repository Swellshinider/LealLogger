using System.Text;

namespace LealLogger.Handlers;

public abstract class LogHandler : ILogHandler
{	
	public abstract void HandleLog(Log logEntry);
	
	public abstract void Dispose();

	protected static string FormatException(Exception exception, string indentation = "    ", int depth = 0)
	{
		var stringBuilder = new StringBuilder();

		stringBuilder.AppendLine($"{indentation}{(depth == 0 ? "Exception" : "InnerException")}: {nameof(exception)}");
		stringBuilder.AppendLine($"{indentation}Message: {exception.Message}");
		stringBuilder.AppendLine($"{indentation} Source: {exception.Source}");

		if (exception.InnerException != null && depth < 3)
			stringBuilder.AppendLine(FormatException(exception.InnerException, indentation += "    ", depth++));

		return stringBuilder.ToString();
	}
}