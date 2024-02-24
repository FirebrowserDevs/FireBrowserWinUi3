using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;
using System.IO;

namespace FireBrowserMultiCore;
public static class UserFolderManager
{
    private static readonly string SettingsFolderName = "Settings";
    private static readonly string DatabaseFolderName = "Database";
    private static readonly string[] SubFolderNames = { SettingsFolderName, DatabaseFolderName, "Browser", "Modules" };

    public static void CreateUserFolders(User user)
    {
        string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username);

        foreach (var folderName in SubFolderNames)
        {
            Directory.CreateDirectory(Path.Combine(userFolderPath, folderName));
        }

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
        string settingsFilePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username, SettingsFolderName, "settings.json");

        // produces a default settings.
        var settings = new Settings(true).Self;

        File.WriteAllText(settingsFilePath, System.Text.Json.JsonSerializer.Serialize(settings));
    }

    private static void CreateDatabaseFile(string username, string dbName, string sql)
    {
        string databaseFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username, DatabaseFolderName);
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
        string settingsFilePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username, SettingsFolderName, "settings.json");

        if (File.Exists(settingsFilePath))
            return System.Text.Json.JsonSerializer.Deserialize<Settings>(File.ReadAllText(settingsFilePath)) ?? new Settings();

        // if someone deletes the settins file after a user exists, all fails so let's create a self image... 
        // return new Settings(): 
        var settings = new Settings(true).Self;
        CreateSettingsFile(user.Username);
        return settings;
    }

    public static Settings TempLoadPrivate(string user)
    {
        string settingsFilePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user, SettingsFolderName, "settings.json");

        if (File.Exists(settingsFilePath))
            return System.Text.Json.JsonSerializer.Deserialize<Settings>(File.ReadAllText(settingsFilePath)) ?? new Settings();

        return new Settings();
    }

    public static void TempSaveSettings(string user, Settings settings)
    {
        try
        {
            object objLock = new object();
            lock (objLock)
            {
                string settingsFilePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user, SettingsFolderName, "settings.json");
                File.WriteAllText(settingsFilePath, System.Text.Json.JsonSerializer.Serialize(settings));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error saving user settings: " + ex.Message);
        }
    }
    public static void SaveUserSettings(User user, Settings settings)
    {
        try
        {
            object objLock = new object();
            lock (objLock)
            {
                string settingsFilePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username, SettingsFolderName, "settings.json");
                File.WriteAllText(settingsFilePath, System.Text.Json.JsonSerializer.Serialize(settings));
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error saving user settings: " + ex.Message);
        }
    }
}