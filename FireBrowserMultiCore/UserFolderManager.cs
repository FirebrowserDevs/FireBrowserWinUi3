using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;
using System.IO;

namespace FireBrowserMultiCore;
public static class UserFolderManager
{
    public static void CreateUserFolders(User user)
    {
        string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username);

        foreach (var folderName in new[] { "Settings", "Database", "Browser", "Modules" })
            Directory.CreateDirectory(Path.Combine(userFolderPath, folderName));

        CreateSettingsFile(user.Username);
        CreateDatabaseFile(user.Username, "History.db", @"CREATE TABLE IF NOT EXISTS urls (
                                        id INTEGER PRIMARY KEY,
                                        last_visit_time TEXT,
                                        url TEXT,
                                        title TEXT,
                                        visit_count INTEGER NOT NULL DEFAULT 0,
                                        typed_count INTEGER NOT NULL DEFAULT 0,
                                        hidden INTEGER NOT NULL DEFAULT 0
                                    )");
        CreateDatabaseFile(user.Username, "Downloads.db", @"CREATE TABLE IF NOT EXISTS downloads (
                                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        guid VARCHAR(50) NOT NULL,
                                        current_path TEXT NOT NULL,
                                        end_time INTEGER NOT NULL,
                                        start_time INTEGER NOT NULL
                                    )");
    }

    private static void CreateSettingsFile(string username)
    {
        string settingsFilePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username, "Settings", "settings.json");

        var settings = new Settings
        {
            DisableJavaScript = "0",
            DisablePassSave = "0",
            DisableWebMess = "0",
            DisableGenAutoFill = "0",
            ColorBackground = "#000000",
            StatusBar = "1",
            BrowserKeys = "1",
            BrowserScripts = "1",
            Useragent = "WebView",
            LightMode = "0",
            OpSw = "1",
            EngineFriendlyName = "Google",
            SearchUrl = "https://www.google.com/search?q=",
            ColorTool = "#000000",
            ColorTV = "#000000",
            Background = "1",
            Auto = "0",
            Lang = "nl-NL",
            ReadButton = "1",
            AdblockBtn = "1",
            Downloads = "1",
            Translate = "1",
            Favorites = "1",
            Historybtn = "1",
            QrCode = "1",
            FavoritesL = "1",
            ToolIcon = "1",
            DarkIcon = "1",
            OpenTabHandel = "0",
            NtpDateTime = "0",
            ExitDialog = "0",
            NtpTextColor = "#000000",
            ExceptionLog = "Low",
            Eq2fa = "1",
            Eqfav = "0",
            EqHis = "0",
            Eqsets = "0",
            TrackPrevention = "2",
        };

        File.WriteAllText(settingsFilePath, System.Text.Json.JsonSerializer.Serialize(settings));
    }

    private static void CreateDatabaseFile(string username, string dbName, string sql)
    {
        string databaseFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username, "Database");
        string databaseFilePath = Path.Combine(databaseFolderPath, dbName);

        using var connection = new SqliteConnection($"Data Source={databaseFilePath}");
        connection.Open();

        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.CommandText = sql;

        command.ExecuteNonQuery();
        transaction.Commit();

        Console.WriteLine($"SQLite database and '{dbName}' table created successfully.");
    }

    public static Settings LoadUserSettings(User user)
    {
        string settingsFilePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username, "Settings", "settings.json");

        if (File.Exists(settingsFilePath))
            return System.Text.Json.JsonSerializer.Deserialize<Settings>(File.ReadAllText(settingsFilePath)) ?? new Settings();

        return new Settings();
    }

    public static void SaveUserSettings(User user, Settings settings)
    {
        try
        {
            string settingsFilePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username, "Settings", "settings.json");
            File.WriteAllText(settingsFilePath, System.Text.Json.JsonSerializer.Serialize(settings));
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error saving user settings: " + ex.Message);
        }
    }
}