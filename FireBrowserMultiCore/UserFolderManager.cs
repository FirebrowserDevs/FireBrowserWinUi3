using System;
using System.IO;
using System.Xml;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace FireBrowserMultiCore
{
    public static class UserFolderManager
    {
        public static void CreateUserFolders(User user)
        {
            string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username);

            Directory.CreateDirectory(userFolderPath);
            Directory.CreateDirectory(Path.Combine(userFolderPath, "Settings"));
            Directory.CreateDirectory(Path.Combine(userFolderPath, "Database"));
            Directory.CreateDirectory(Path.Combine(userFolderPath, "Browser"));

            CreateSettingsFile(user.Username);
            CreateDatabaseFile(user.Username);
        }

        public static void CreateSettingsFile(string username)
        {
            string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username);
            string settingsFolderPath = Path.Combine(userFolderPath, "Settings");
            string settingsFilePath = Path.Combine(settingsFolderPath, "settings.json");

            // Ensure the "Settings" folder exists
            Directory.CreateDirectory(settingsFolderPath);

            // Create and populate the settings object
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
                // Add other settings properties here
            };

            // Serialize the settings object to JSON and save it to the file
            string jsonString = JsonSerializer.Serialize(settings);
            File.WriteAllText(settingsFilePath, jsonString);
        }

        public static void CreateDatabaseFile(string username)
        {
            string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username);
            string databaseFolderPath = Path.Combine(userFolderPath, "Database");
            string databaseFilePath = Path.Combine(databaseFolderPath, "History.db");

            // Ensure the "Database" folder exists
            Directory.CreateDirectory(databaseFolderPath);

            // Create the SQLite database file
            using (var connection = new SqliteConnection($"Data Source={databaseFilePath}"))
            {
                connection.Open();
                // You can create tables and perform other database operations as needed here
                // For example: Create tables if they don't exist
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS urlsDb (Url NVARCHAR(2583) PRIMARY KEY NOT NULL, " +
                                     "Title NVARCHAR(2548), " +
                                     "Visit_Count INTEGER, " +
                                     "Last_Visit_Time DATETIME)";
                    command.ExecuteNonQuery();
                }
            }
        }


        public static Settings LoadUserSettings(User user)
        {
            string settingsFilePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username, "Settings", "settings.json");

            if (File.Exists(settingsFilePath))
            {
                string settingsJson = File.ReadAllText(settingsFilePath);
                return JsonSerializer.Deserialize<Settings>(settingsJson);
            }

            // Return default settings if the file doesn't exist.
            return new Settings();
        }

        public static void SaveUserSettings(User user, Settings settings)
        {
            try
            {
                string settingsFilePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username, "Settings", "settings.json");
                string settingsJson = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(settingsFilePath, settingsJson);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error saving user settings: " + ex.Message);
                // You can handle the error or log it as needed.
            }
        }

    }
}
