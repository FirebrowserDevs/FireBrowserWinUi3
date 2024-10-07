using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FireBrowserWinUi3MultiCore;
public static class UserDataManager
{
    public static readonly string DocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public static readonly string CoreFolderPath = Path.Combine(DocumentsFolder, "FireBrowserUserCore");
    public static readonly string UsersFolderPath = "Users";
    public static readonly string CoreFileName = "UsrCore.json";
    public static readonly string CoreFilePath = Path.Combine(CoreFolderPath, CoreFileName);

    public static UserDataResult LoadUsers()
    {
        if (!File.Exists(CoreFilePath))
            return new UserDataResult { Users = new List<User>(), CurrentUsername = string.Empty };

        var users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(CoreFilePath));
        return new UserDataResult { Users = users, CurrentUsername = AuthService.IsUserAuthenticated ? AuthService.CurrentUser.Username : "Guest" };
    }

    public static void SaveUsers(List<User> users)
    {
        Directory.CreateDirectory(CoreFolderPath);
        File.WriteAllText(CoreFilePath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
    }

    public static void DeleteUser(string username)
    {
        var userData = LoadUsers();
        var userToDelete = userData.Users.FirstOrDefault(u => u.Username == username);

        if (userToDelete != null)
        {
            string userFolderPath = Path.Combine(CoreFolderPath, UsersFolderPath, username);

            if (Directory.Exists(userFolderPath))
                Directory.Delete(userFolderPath, true);

            userData.Users.Remove(userToDelete);
            SaveUsers(userData.Users);
            // never was updating settings timeline because user existed in AuthService.  DELETE. 
            AuthService.users.Remove(userToDelete);
        }
    }
}