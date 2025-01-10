using System.Data.SQLite;
using LealLogger.Handlers;

namespace LealLogger.Sample
{
	public sealed class DatabaseLogHandler : LogHandler
	{
		private readonly SQLiteConnection _connection;

		public DatabaseLogHandler(string databasePath)
		{
			if (string.IsNullOrWhiteSpace(databasePath))
				throw new ArgumentException("Database file path must not be null or empty.", nameof(databasePath));

			_connection = new SQLiteConnection($"Data Source={databasePath};Version=3;");
			InitializeDatabase();
		}

		private void InitializeDatabase()
		{
			_connection.Open();

			var createTableQuery = @"
				CREATE TABLE IF NOT EXISTS Logs (
					Id INTEGER PRIMARY KEY AUTOINCREMENT,
					Timestamp TEXT NOT NULL,
					LogLevel TEXT NOT NULL,
					Message TEXT NOT NULL,
					Exception TEXT,
					ThreadId INTEGER,
					ApplicationName TEXT,
					MachineName TEXT,
					UserName TEXT,
					ProcessId INTEGER
				);
			";

			using var command = new SQLiteCommand(createTableQuery, _connection);
			command.ExecuteNonQuery();
		}

		public override void HandleLog(Log logEntry)
		{
			var insertQuery = @"
				INSERT INTO Logs (
					Timestamp, LogLevel, Message, Exception, ThreadId, 
					ApplicationName, MachineName, UserName, ProcessId
				) VALUES (
					@Timestamp, @LogLevel, @Message, @Exception, @ThreadId, 
					@ApplicationName, @MachineName, @UserName, @ProcessId
				);
			";

			using var command = new SQLiteCommand(insertQuery, _connection);

			command.Parameters.AddWithValue("@Timestamp", logEntry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));
			command.Parameters.AddWithValue("@LogLevel", logEntry.LogLevel.ToString());
			command.Parameters.AddWithValue("@Message", logEntry.Message);
			command.Parameters.AddWithValue("@Exception", logEntry.Exception != null ? logEntry.Exception.ToString() : DBNull.Value);
			command.Parameters.AddWithValue("@ThreadId", logEntry.ThreadId);
			command.Parameters.AddWithValue("@ApplicationName", logEntry.ApplicationName);
			command.Parameters.AddWithValue("@MachineName", logEntry.MachineName);
			command.Parameters.AddWithValue("@UserName", logEntry.UserName);
			command.Parameters.AddWithValue("@ProcessId", logEntry.ProcessId);

			command.ExecuteNonQuery();
		}

		public override void Dispose()
		{
			_connection.Close();
			_connection.Dispose();
		}
	}
}