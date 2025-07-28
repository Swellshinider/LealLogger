namespace LealLogger.Enums;

/// <summary>
/// Defines the strategies used to roll log files.
/// </summary>
public enum FileRolling
{
    /// <summary>
    /// No file rolling. All logs go into a single file.
    /// </summary>
    Infinite,

    /// <summary>
    /// Creates a new log file every hour.
    /// </summary>
    Hourly,

    /// <summary>
    /// Creates a new log file every day.
    /// </summary>
    Daily,

    /// <summary>
    /// Creates a new log file every week.
    /// </summary>
    Weekly,

    /// <summary>
    /// Creates a new log file every month.
    /// </summary>
    Monthly,

    /// <summary>
    /// Rolls over to a new log file when the current one exceeds a configured size in kilobytes.
    /// </summary>
    SizeKB,

    /// <summary>
    /// Rolls over to a new log file when the current one exceeds a configured size in megabytes.
    /// </summary>
    SizeMB,

    /// <summary>
    /// Rolls over to a new log file when the current one exceeds a configured size in gigabytes.
    /// </summary>
    SizeGB,

    /// <summary>
    /// Creates a new log file each time the application starts.
    /// </summary>
    Startup,

    /// <summary>
    /// Rolls logs based on a user-defined time interval.
    /// </summary>
    CustomTimeSpan,
}
