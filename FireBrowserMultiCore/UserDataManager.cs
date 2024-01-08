using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FireBrowserMultiCore;
public static class UserDataManager
{
    public static readonly string DocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public static readonly string CoreFolderPath = Path.Combine(DocumentsFolder, "FireBrowserUserCore");
    public static readonly string UsersFolderPath = "Users";
    public static readonly string CoreFileName = "UsrCore.json";

    public static UserDataResult LoadUsers()
    {
        string coreFilePath = Path.Combine(CoreFolderPath, CoreFileName);

        if (!File.Exists(coreFilePath))
        {
            return new UserDataResult { Users = new List<User>(), CurrentUsername = string.Empty };
        }

        var users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(coreFilePath));
        string currentUsername = AuthService.IsUserAuthenticated ? AuthService.CurrentUser.Username : "Guest";

        return new UserDataResult { Users = users, CurrentUsername = currentUsername };
    }

    public static void SaveUsers(List<User> users)
    {
        if (!Directory.Exists(CoreFolderPath)) Directory.CreateDirectory(CoreFolderPath);

        string coreFilePath = Path.Combine(CoreFolderPath, CoreFileName);
        File.WriteAllText(coreFilePath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
    }
}