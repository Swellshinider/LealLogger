using LealLogger.Factory;
using LealLogger.Handlers;
using LealLogger.Tests.DataTest;

namespace LealLogger.Tests;

public class LoggerTests
{
	[Fact]
	public void LoggingBelowMinLevel_ShouldNotCallHandlers()
	{
		// Arrange
		var inMemoryHandler = new InMemoryLogHandler();
		var logger = new LoggerBuilder()
			.SetMinimumLogLevel(LogLevel.WARN)
			.SetQueueCapacity(1000)
			.AddHandler(inMemoryHandler)
			.Build();

		// Act
		logger.Debug("This is below WARN level.");  // Should be ignored
		logger.Info("This is also below WARN level."); // Should be ignored
		
		// Flush if needed
		logger.Dispose();

		// Assert
		Assert.Empty(inMemoryHandler.Logs);
	}

	[Fact]
	public void LoggingAtOrAboveMinLevel_ShouldCallHandlers()
	{
		// Arrange
		var inMemoryHandler = new InMemoryLogHandler();
		var logger = new LoggerBuilder()
			.SetMinimumLogLevel(LogLevel.INFO)
			.SetQueueCapacity(1000)
			.AddHandler(inMemoryHandler)
			.Build();

		// Act
		logger.Info("Info message");
		logger.Warn("Warn message");
		logger.Error("Error message");

		// Flush if needed
		logger.Dispose();

		// Assert
		Assert.Equal(3, inMemoryHandler.Logs.Count);
		Assert.Contains(inMemoryHandler.Logs, log => log.Message == "Info message");
		Assert.Contains(inMemoryHandler.Logs, log => log.Message == "Warn message");
		Assert.Contains(inMemoryHandler.Logs, log => log.Message == "Error message");
	}

	[Fact]
	public void Logging_ExceedQueueCapacity_ShouldEitherBlockOrDrop()
	{
		// Arrange
		var inMemoryHandler = new InMemoryLogHandler();
		var logger = new LoggerBuilder()
			.SetMinimumLogLevel(LogLevel.DEBUG)
			.SetQueueCapacity(1)
			.AddHandler(inMemoryHandler)
			.Build();

		// Act
		logger.Debug("First log - queue now full");
		
		logger.Debug("Second log - might block or drop");

		// Flush if needed
		logger.Dispose();
		Assert.True(inMemoryHandler.Logs.Count > 1);
	}

	[Fact]
	public void Disposal_ShouldFlushRemainingLogs()
	{
		// Arrange
		var inMemoryHandler = new InMemoryLogHandler();
		var logger = new LoggerBuilder()
			.SetMinimumLogLevel(LogLevel.DEBUG)
			.SetQueueCapacity(10)
			.AddHandler(inMemoryHandler)
			.Build();

		// Act
		logger.Debug("Log #1");
		logger.Info("Log #2");

		// Logs are queued and processed asynchronously, so disposing ensures the background thread flushes everything.
		logger.Dispose();

		// Assert
		Assert.Equal(2, inMemoryHandler.Logs.Count);
	}

	[Fact]
	public void LoggingWithException_ShouldStoreExceptionDetails()
	{
		// Arrange
		var inMemoryHandler = new InMemoryLogHandler();
		var logger = new LoggerBuilder()
			.SetMinimumLogLevel(LogLevel.ERROR)
			.SetQueueCapacity(1000)
			.AddHandler(inMemoryHandler)
			.Build();

		var exception = new InvalidOperationException("Test exception");
		// Act
		logger.Error("An error occurred", exception);
		logger.Dispose();

		Assert.Single(inMemoryHandler.Logs);
		var logEntry = inMemoryHandler.Logs.First();
		Assert.Equal("An error occurred", logEntry.Message);
		Assert.Equal(exception, logEntry.Exception);
    }

	[Fact]
	public async Task LoggingMultipleThreads_ShouldHandleConcurrency()
	{
		// Arrange
		var inMemoryHandler = new InMemoryLogHandler();
		var logger = new LoggerBuilder()
			.SetMinimumLogLevel(LogLevel.DEBUG)
			.SetQueueCapacity(1000)
			.AddHandler(inMemoryHandler)
			.Build();

		// Act
		var tasks = Enumerable.Range(0, 100).Select(i => Task.Run(() => logger.Info($"Log from task {i}"))).ToArray();
		await Task.WhenAll(tasks);

        logger.Dispose();
		// Assert
		Assert.Equal(100, inMemoryHandler.Logs.Count);
		Assert.All(inMemoryHandler.Logs, log => Assert.StartsWith("Log from task", log.Message));
    }
}