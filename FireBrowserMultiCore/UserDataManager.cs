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
            return new UserDataResult
            {
                Users = new List<User>(),
                CurrentUsername = string.Empty // You can set a default value if needed.
            };
        }

        string coreJson = File.ReadAllText(coreFilePath);
        var users = JsonSerializer.Deserialize<List<User>>(coreJson);

        // You would need to implement your logic to determine the current user's username.
        string currentUsername = GetCurrentUserUsername(); // Replace with your actual logic.

        return new UserDataResult
        {
            Users = users,
            CurrentUsername = currentUsername
        };
    }

    public static string GetCurrentUserUsername()
    {
        // Replace this example with your actual authentication logic.
        // You may have an authentication service or user session where you can access the username.
        // For simplicity, we'll assume you have a static property in an AuthService class.
        // AuthService.CurrentUser represents the currently authenticated user with a Username property.

        if (AuthService.IsUserAuthenticated)
        {
            return AuthService.CurrentUser.Username;
        }

        // Return a default or anonymous username if no user is authenticated.
        return "Guest";
    }


    public static void SaveUsers(List<User> users)
    {
        if (!Directory.Exists(CoreFolderPath))
        {
            Directory.CreateDirectory(CoreFolderPath);
        }

        string coreFilePath = Path.Combine(CoreFolderPath, CoreFileName);
        string coreJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(coreFilePath, coreJson);
    }
}