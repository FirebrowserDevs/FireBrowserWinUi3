using FireBrowserMultiCore;
using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace FireBrowserDatabase;
public class SaveDb
{
    public static void InsertHistoryItem(FireBrowserMultiCore.User user, string url, string title, int visitCount, int typedCount, int hidden)
    {
        string username = user.Username;
        string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username);
        string databaseFolderPath = Path.Combine(userFolderPath, "Database");
        string databaseFilePath = Path.Combine(databaseFolderPath, "History.db");
        var currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        if (File.Exists(databaseFilePath))
        {
            using (var connection = new SqliteConnection($"Data Source={databaseFilePath}"))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    string query = "INSERT INTO urls (url, title, visit_count, typed_count, hidden, last_visit_time) " +
                                   "VALUES (@url, @title, @visitCount, @typedCount, @hidden, @last_visit_time)";

                    using (var command = new SqliteCommand(query, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@url", url);
                        command.Parameters.AddWithValue("@title", title);
                        command.Parameters.AddWithValue("@visitCount", visitCount);
                        command.Parameters.AddWithValue("@typedCount", typedCount);
                        command.Parameters.AddWithValue("@hidden", hidden);
                        command.Parameters.AddWithValue("@last_visit_time", currentTime);

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
