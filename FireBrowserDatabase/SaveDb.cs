using FireBrowserMultiCore;
using Microsoft.Data.Sqlite;
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

        if (File.Exists(databaseFilePath))
        {
            using (var connection = new SqliteConnection($"Data Source={databaseFilePath}"))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    string query = "INSERT INTO urls (url, title, visit_count, typed_count, hidden) " +
                                   "VALUES (@url, @title, @visitCount, @typedCount, @hidden)";

                    using (var command = new SqliteCommand(query, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@url", url);
                        command.Parameters.AddWithValue("@title", title);
                        command.Parameters.AddWithValue("@visitCount", visitCount);
                        command.Parameters.AddWithValue("@typedCount", typedCount);
                        command.Parameters.AddWithValue("@hidden", hidden);

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
