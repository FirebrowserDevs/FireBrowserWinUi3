using FireBrowserWinUi3MultiCore;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FireBrowserWinUi3Favorites;
public class FavManager
{
    private readonly string _favoritesDbPath;

    public FavManager()
    {
        _favoritesDbPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Database", "Favorites.db");
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        // Create the directory if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(_favoritesDbPath));

        // Create or open the SQLite database file
        using (var connection = new SqliteConnection($"Data Source={_favoritesDbPath}"))
        {
            connection.Open();

            // Create the Favorites table if it doesn't exist
            var createTableSql = @"
                CREATE TABLE IF NOT EXISTS Favorites (
                    Title TEXT,
                    Url TEXT,
                    IconUrlPath TEXT
                )";
            var createTableCommand = new SqliteCommand(createTableSql, connection);
            createTableCommand.ExecuteNonQuery();
        }

        // Check if the old favorites.json file exists
        string oldFavoritesJsonPath = Path.Combine(Path.GetDirectoryName(_favoritesDbPath), "favorites.json");
        if (File.Exists(oldFavoritesJsonPath))
        {
            // Read the JSON content from the file
            string jsonContent = File.ReadAllText(oldFavoritesJsonPath);

            // Deserialize JSON content into a list of FavItem
            List<FavItem> favoritesList = JsonSerializer.Deserialize<List<FavItem>>(jsonContent);

            // Insert each item into the SQLite database
            using (var connection = new SqliteConnection($"Data Source={_favoritesDbPath}"))
            {
                connection.Open();
                foreach (var favItem in favoritesList)
                {
                    var insertSql = "INSERT INTO Favorites (Title, Url, IconUrlPath) VALUES (@Title, @Url, @IconUrlPath)";
                    var insertCommand = new SqliteCommand(insertSql, connection);
                    insertCommand.Parameters.AddWithValue("@Title", favItem.Title);
                    insertCommand.Parameters.AddWithValue("@Url", favItem.Url);
                    insertCommand.Parameters.AddWithValue("@IconUrlPath", favItem.IconUrlPath);
                    insertCommand.ExecuteNonQuery();
                }
            }

            // Remove the old favorites.json file
            File.Delete(oldFavoritesJsonPath);
        }
    }

    public void SaveFav(string title, string url)
    {
        using (var connection = new SqliteConnection($"Data Source={_favoritesDbPath}"))
        {
            connection.Open();
            var insertSql = "INSERT INTO Favorites (Title, Url, IconUrlPath) VALUES (@Title, @Url, @IconUrlPath)";
            var insertCommand = new SqliteCommand(insertSql, connection);
            insertCommand.Parameters.AddWithValue("@Title", title);
            insertCommand.Parameters.AddWithValue("@Url", url);
            insertCommand.Parameters.AddWithValue("@IconUrlPath", $"https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url={url}&size=32");
            insertCommand.ExecuteNonQuery();
        }
    }

    public List<FavItem> LoadFav()
    {
        var favorites = new List<FavItem>();
        using (var connection = new SqliteConnection($"Data Source={_favoritesDbPath}"))
        {
            connection.Open();
            var selectSql = "SELECT * FROM Favorites";
            var selectCommand = new SqliteCommand(selectSql, connection);
            using (var reader = selectCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var favItem = new FavItem
                    {
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        Url = reader.GetString(reader.GetOrdinal("Url")),
                        IconUrlPath = reader.GetString(reader.GetOrdinal("IconUrlPath"))
                    };
                    favorites.Add(favItem);
                }
            }
        }
        return favorites;
    }

    public void ClearFavs()
    {
        using (var connection = new SqliteConnection($"Data Source={_favoritesDbPath}"))
        {
            connection.Open();
            var deleteSql = "DELETE FROM Favorites";
            var deleteCommand = new SqliteCommand(deleteSql, connection);
            deleteCommand.ExecuteNonQuery();
        }
    }

    public void RemoveFavorite(FavItem selectedItem)
    {
        if (selectedItem == null)
        {
            // Handle the case where the selected item is null (optional)
            Console.WriteLine("Selected item is null.");
            return;
        }

        using (var connection = new SqliteConnection($"Data Source={_favoritesDbPath}"))
        {
            connection.Open();
            var deleteSql = "DELETE FROM Favorites WHERE Title = @Title AND Url = @Url";
            var deleteCommand = new SqliteCommand(deleteSql, connection);
            deleteCommand.Parameters.AddWithValue("@Title", selectedItem.Title);
            deleteCommand.Parameters.AddWithValue("@Url", selectedItem.Url);
            deleteCommand.ExecuteNonQuery();
        }
    }
}
