using Microsoft.Data.Sqlite;
using System.Threading.Tasks;

namespace FireBrowserDatabase;
public class DbClear
{
    public static async Task ClearTable(string databasePath, string tableName)
    {
        await Task.Run(() =>
        {
            using (var connection = new SqliteConnection($"Data Source={databasePath}"))
            {
                connection.Open();
                // Create a command that deletes all rows from the specified table
                var command = new SqliteCommand($"DELETE FROM {tableName}", connection);

                // Execute the command to clear the table
                command.ExecuteNonQuery();

                connection.Close();
            }
        });
    }
}
