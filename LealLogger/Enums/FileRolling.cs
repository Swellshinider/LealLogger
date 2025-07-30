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
    /// File is rolled every time the application is executed.
    /// </summary>
    ByExecution,

    /// <summary>
    /// Creates a new log file every hour.
    /// </summary>
    Hourly,

    /// <summary>
    /// Creates a new log file every two hours.
    /// </summary>
    EachTwoHours,

    /// <summary>
    /// Creates a new log file every three hours.
    /// </summary>
    EachThreeHours,

    /// <summary>
    /// Creates a new log file every four hours.
    /// </summary>
    EachFourHours,

    /// <summary>
    /// Creates a new log file every six hours.
    /// </summary>
    EachSixHours,

    /// <summary>
    /// Creates a new log file every twelve hours.
    /// </summary>
    EachTwelveHours,

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
    /// Creates a new log file every year.
    /// </summary>
    Yearly,
}
