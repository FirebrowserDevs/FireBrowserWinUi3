using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FireBrowserWinUi3MultiCore;
public class AuthService
{
    private static readonly string UserDataFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FireBrowserUserCore", "UsrCore.json");
    private static readonly List<User> users = LoadUsersFromJson();

    private static List<User> LoadUsersFromJson()
    {
        try
        {
            string userDataJson = File.ReadAllText(UserDataFilePath);
            return JsonSerializer.Deserialize<List<User>>(userDataJson) ?? new List<User>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading user data: {ex.Message}");
            return new List<User>();
        }
    }

    public static User CurrentUser { get; private set; }

    public static bool IsUserAuthenticated => CurrentUser != null;

    public static bool SwitchUser(string username) => (CurrentUser = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase))) != null;

    public static bool Authenticate(string username) => (CurrentUser = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase))) != null;

    public static void AddUser(User newUser)
    {
        if (!users.Any(u => u.Username.Equals(newUser.Username, StringComparison.OrdinalIgnoreCase)))
        {
            users.Add(newUser);
            SaveUsers();
        }
    }

    private static void SaveUsers() => File.WriteAllText(UserDataFilePath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));

    public static List<string> GetAllUsernames() => users.Select(u => u.Username).ToList();

    public static void Logout() => CurrentUser = null;

    public static bool ChangeUsername(string oldUsername, string newUsername)
    {
        // Find the user with the old username
        User userToChange = users.FirstOrDefault(u => u.Username.Equals(oldUsername, StringComparison.OrdinalIgnoreCase));
        if (userToChange == null)
        {
            // User with old username not found
            return false;
        }

        // Check if the new username already exists
        if (users.Any(u => u.Username.Equals(newUsername, StringComparison.OrdinalIgnoreCase)))
        {
            // New username already exists, cannot change
            return false;
        }

        // Update the username
        userToChange.Username = newUsername;

        string tempFolderPath = Path.GetTempPath();
        string changeUsernameFilePath = Path.Combine(tempFolderPath, "changeusername.json");
        var changeUsernameData = new { OldUsername = oldUsername, NewUsername = newUsername, Changed = true };
        File.WriteAllText(changeUsernameFilePath, JsonSerializer.Serialize(changeUsernameData));

        // Save the updated user list
        SaveUsers();

        // If the current user is the one whose username was changed, update CurrentUser
        if (CurrentUser != null && CurrentUser.Username.Equals(oldUsername, StringComparison.OrdinalIgnoreCase))
        {
            CurrentUser = userToChange;
        }

        return true;
    }
}