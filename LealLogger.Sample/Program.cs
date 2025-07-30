using LealLogger;
using LealLogger.Factory;
using LealLogger.Sample;

// Building a logger
var logger = new LoggerBuilder()
	.AddConsoleHandler()															// Add a logger to log to the console
	.AddFileHandler("errors.log", out var finalFilePath, logLevel: LogLevel.ERROR)	// Add a logger to log to a file only errors and above
	.AddHandler(new DatabaseLogHandler("database.sqlite3"))    						// Your Custom handler example
	.SetMinimumLogLevel(LogLevel.DEBUG)												// Log everything from DEBUG and above
	.SetQueueCapacity(5000)															// Large queue capacity for high throughput
	.Build();

// Log random messages
logger.Debug($"Final file path for logging: {finalFilePath}"); // Log debug information
logger.Debug("Debugging information to trace execution.");
logger.Error("User tried to divide by zero.", new DivideByZeroException("You cannot divide by zero"));
logger.Info("Application started successfully.");
logger.Warn("Low disk space warning.");
logger.Error("An error occurred while processing the request.", new InvalidOperationException("Invalid operation example"));
logger.Fatal("Fatal error - application shutting down!");

// Finalize and cleanup
logger.Dispose();