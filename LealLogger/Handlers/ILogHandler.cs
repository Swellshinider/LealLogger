namespace LealLogger.Handlers;

internal interface ILogHandler : IDisposable
{
    void HandleLog(Log logEntry);
}