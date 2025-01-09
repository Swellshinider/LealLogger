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
		var customLogger = builder.Build<MyCustomLogger>();

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
		var logger = builder.Build();

		// Assert
		Assert.NotNull(logger);
		Assert.Contains(builder.Handlers, h => h is ConsoleLogHandler);
	}

	[Fact]
	public void AddFileHandler_ShouldIncludeFileHandler()
	{
		// Arrange
		var filePath = Path.GetTempFileName();
		var builder = new LoggerBuilder();

		// Act
		builder.AddFileHandler(filePath);
		var logger = builder.Build();

		// Assert
		Assert.NotNull(logger);
		Assert.Contains(builder.Handlers, h => h is FileLogHandler);
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
		var logger = builder.Build();

		// Assert
		Assert.NotNull(logger);
		Assert.Equal(level, builder.MinimumLogLevel);
	}
}