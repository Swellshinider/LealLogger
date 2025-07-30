using LealLogger.Factory;
using LealLogger.Handlers;
using LealLogger.Tests.DataTest;

namespace LealLogger.Tests;

public class LoggerBuilderTests
{
	[Fact]
	public void Build_DefaultLogger_ShouldUseDefaultValues()
	{
		// Arrange
		// In a Debug build, log level = DEBUG; in Release, log level = INFO.
		var builder = new LoggerBuilder();

		// Act
		var logger = builder.Build();

		// Assert
		Assert.NotNull(logger);
		Assert.IsType<Logger>(logger);
		Assert.Equal(LogLevel.DEBUG, builder.MinimumLogLevel);
		Assert.Equal(1000, builder.QueueCapacity);
		Assert.Empty(builder.Handlers);
	}

	[Fact]
	public void Build_CustomLogger_ShouldInstantiateCustomClass()
	{
		// Arrange
		var builder = new LoggerBuilder();

		// Act
		using var customLogger = builder.Build<MyCustomLogger>();

		// Assert
		Assert.NotNull(customLogger);
		Assert.IsType<MyCustomLogger>(customLogger);
	}

	[Fact]
	public void AddConsoleHandler_ShouldIncludeConsoleHandler()
	{
		// Arrange
		var builder = new LoggerBuilder();

		// Act
		builder.AddConsoleHandler();
        using var logger = builder.Build();

		// Assert
		Assert.NotNull(logger);
		Assert.Contains(builder.Handlers, h => h is ConsoleLogHandler);
    }

	[Fact]
	public void AddFileHandler_ShouldIncludeFileHandler()
	{
		// Arrange
		var fileDirectory = Path.GetTempPath();
		var filePath = Path.Combine(fileDirectory, "test_log.txt");
        var builder = new LoggerBuilder();

		// Act
		builder.AddFileHandler(filePath, out var finalFilePath);
        using var logger = builder.Build();

		// Assert
		Assert.NotNull(logger);
		Assert.Contains(builder.Handlers, h => h is FileLogHandler);
		Assert.NotEqual(filePath, finalFilePath);
		Assert.True(File.Exists(finalFilePath));
    }

	[Fact]
	public void SetQueueCapacity_InvalidValue_ShouldThrow()
	{
		// Arrange
		var builder = new LoggerBuilder();

		// Act & Assert
		Assert.Throws<ArgumentException>(() => builder.SetQueueCapacity(0));
		Assert.Throws<ArgumentException>(() => builder.SetQueueCapacity(-100));
	}

	[Theory]
	[InlineData(LogLevel.DEBUG)]
	[InlineData(LogLevel.WARN)]
	[InlineData(LogLevel.ERROR)]
	public void SetMinimumLogLevel_ShouldAssignProperly(LogLevel level)
	{
		// Arrange
		var builder = new LoggerBuilder();

		// Act
		builder.SetMinimumLogLevel(level);
		using var logger = builder.Build();

		// Assert
		Assert.NotNull(logger);
		Assert.Equal(level, builder.MinimumLogLevel);
	}
}