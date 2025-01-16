<div align="center">

# LealLogger

[![NuGet](https://img.shields.io/nuget/v/LealLogger.svg)](https://www.nuget.org/packages/LealLogger/)

</div>

**LealLogger** is a high-performance, extensible, and lightweight logging library for .NET applications. It supports multiple logging outputs (e.g., Console, File) and provides flexibility through a powerful builder pattern. Whether you're developing a small project or a high-load enterprise application, LealLogger adapts to your needs with ease.

---

## Table of Contents

- [LealLogger](#leallogger)
  - [Table of Contents](#table-of-contents)
  - [Getting Started](#getting-started)
    - [Installation](#installation)
    - [Usage](#usage)
      - [1. Basic Setup](#1-basic-setup)
      - [2. Adding File Logging](#2-adding-file-logging)
      - [3. Custom Logger](#3-custom-logger)
  - [API Reference](#api-reference)
    - [LoggerBuilder](#loggerbuilder)
    - [Log Levels](#log-levels)
    - [LogHandler](#loghandler)
  - [Customization Guide](#customization-guide)
    - [Custom Log Handlers](#custom-log-handlers)
  - [Contribution](#contribution)
    - [How to Contribute](#how-to-contribute)
  - [License](#license)

## Getting Started

### Installation

You can install LealLogger via terminal:

```bash
dotnet add package LealLogger
```

### Usage

For a practical example check out [Sample project](./LealLogger.Sample/Program.cs)

#### 1. Basic Setup

```csharp
using LealLogger.Factory;

using var logger = new LoggerBuilder()
    .AddConsoleHandler() // This adds a console handler to the logger, witch means that every time 
                        // you log something it will be automatically printed into the console
    .SetMinimumLogLevel(LogLevel.INFO)
    .Build();


logger.Debug("Application started."); // This won't be logged, because you setted the minimum LevelLog to INFO
logger.Info("Application started.");
logger.Warn("Low disk space detected.");
logger.Error("Unhandled exception occurred.", new Exception("Example exception"));
```

#### 2. Adding File Logging

```csharp
var logger = new LoggerBuilder()
    .AddConsoleHandler()
    .AddFileHandler("logs/app.log")
    .SetQueueCapacity(5000)
    .Build();

logger.Info("Logging to both console and file.");
```

#### 3. Custom Logger

Define a custom logger class by inheriting from `BaseLogger`:

```csharp
public sealed class MyCustomLogger : BaseLogger
{
    public MyCustomLogger(LogLevel logLevel, int queueCapacity, ImmutableArray<LogHandler> handlers)
        : base(logLevel, queueCapacity, handlers)
    {
    }

    public override void Debug(string message, Exception? ex = null) { /* Custom behavior */ }
    public override void Info(string message, Exception? ex = null) { /* Custom behavior */ }
    public override void Warn(string message, Exception? ex = null) { /* Custom behavior */ }
    public override void Error(string message, Exception? ex = null) { /* Custom behavior */ }
    public override void Fatal(string message, Exception? ex = null) { /* Custom behavior */ }
    public override void Dispose() { /* Cleanup resources */ }
}
```

Use the `LoggerBuilder` to instantiate it:

```csharp
var customLogger = new LoggerBuilder()
    .AddConsoleHandler()
    .Build<MyCustomLogger>(); // <--

customLogger.Info("Custom logger in action!");
```

---

## API Reference

### LoggerBuilder

The `LoggerBuilder` class simplifies logger configuration and instantiation.

- **`AddConsoleHandler()`**: Adds a console log handler.
- **`AddFileHandler(string filePath)`**: Adds a file log handler.
- **`AddHandler<T>(T handler)`**: Adds a custom log handler.
- **`SetMinimumLogLevel(LogLevel logLevel)`**: Sets the minimum log level for logs to be processed.
- **`SetQueueCapacity(int queueCapacity)`**: Sets the queue capacity for buffering logs.
- **`Build()`**: Creates a default `Logger`.
- **`Build<T>()`**: Creates a custom logger of type `T`.

### Log Levels

- `DEBUG`
- `INFO`
- `WARN`
- `ERROR`
- `FATAL`

### LogHandler

Implement `LogHandler` to create custom log output mechanisms. For example:

```csharp
public sealed class MyCustomHandler : LogHandler
{
    public override void HandleLog(Log logEntry)
    {
        // Custom handling logic
    }

    public override void Dispose()
    {
        // Cleanup resources
    }
}
```

---

## Customization Guide

### Custom Log Handlers

Create your own log handler by inheriting from `LogHandler`. Example:

```csharp
public sealed class DatabaseLogHandler : LogHandler
{
    public override void HandleLog(Log logEntry)
    {
        // Insert log entry into a database
    }

    public override void Dispose()
    {
        // Close database connections
    }
}
```

```csharp
var logger = new LoggerBuilder()
    .AddConsoleHandler()
    .AddFileHandler("logs/app.log")
    .AddHandler(new DatabaseLogHandler())
    .SetQueueCapacity(5000)
    .Build();

logger.Info("This log will be printed into the console, saved into the file and saved into the database");
```
---

## Contribution

Contributions are welcome! If you have ideas to improve LealLogger, feel free to open an issue or fork the repository.

### How to Contribute

- Fork the repository.
- Create a new branch for your feature or bugfix.
- Please, test your changes before committing.
- Submit a pull request with a detailed explanation of your changes.
- :)

## License

This project is licensed under the MIT License. See the LICENSE file for details.