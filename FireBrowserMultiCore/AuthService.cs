using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FireBrowserMultiCore;
public class AuthService
{
    private static List<User> users;
    private static readonly string UserDataFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FireBrowserUserCore", "UsrCore.json");
    static AuthService()
    {
        LoadUsersFromJson();
    }

    private static void LoadUsersFromJson()
    {
        try
        {
            string userDataJson = File.ReadAllText(UserDataFilePath);
            users = JsonSerializer.Deserialize<List<User>>(userDataJson) ?? new List<User>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading user data: {ex.Message}");
            users = new List<User>();
            CurrentUser = null;
        }
    }

    public static User CurrentUser { get; private set; }

    public static bool IsUserAuthenticated => CurrentUser != null;

    public static bool SwitchUser(string username)
    {
        User switchedUser = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (switchedUser != null)
        {
            CurrentUser = switchedUser;
            return true;
        }

        return false;
    }

    public static bool Authenticate(string username)
    {
        User authenticatedUser = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (authenticatedUser != null)
        {
            CurrentUser = authenticatedUser;
            return true;
        }

        return false;
    }

    public static bool DeleteUser(string username)
    {
        User userToDelete = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (userToDelete != null)
        {
            try
            {
                string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username);
                if (Directory.Exists(userFolderPath))
                {
                    Directory.Delete(userFolderPath, true);
                }

                users.Remove(userToDelete);
                SaveUsers();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return false;
            }
        }

        return false;
    }

    public static void AddUser(User newUser)
    {
        if (users.Any(u => u.Username.Equals(newUser.Username, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        users.Add(newUser);
        SaveUsers();
    }

    private static void SaveUsers()
    {
        string coreJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(UserDataFilePath, coreJson);
    }

    public static List<string> GetAllUsernames()
    {
        return users.Select(u => u.Username).ToList();
    }

    public static void Logout()
    {
        CurrentUser = null;
    }
}