using Microsoft.Data.Sqlite;

namespace FireBrowserDatabase;

public class DbClearTableData
{
    public void DeleteTableData(string databasePath, string tablename, string where)
    {
        using (SqliteConnection m_dbConnection = new SqliteConnection($"Data Source={databasePath};"))
        {
            m_dbConnection.Open();
            var wheret = "";
            if (!string.IsNullOrEmpty(where)) wheret = $" where {where}";
            SqliteCommand selectCommand = new SqliteCommand($"delete from {tablename}{wheret}", m_dbConnection);
            SqliteDataReader query = selectCommand.ExecuteReader();
            m_dbConnection.Close();
        }
    }
}
