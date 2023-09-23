using System;
using System.IO;
using System.Xml;
using System.Text.Json;

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
            string settingsFilePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username, "Settings", "settings.json");
            string settingsJson = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(settingsFilePath, settingsJson);
        }
    }
}
