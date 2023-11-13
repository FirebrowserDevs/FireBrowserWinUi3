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
            // Read user data from the JSON file.
            string userDataJson = File.ReadAllText(UserDataFilePath);
            users = JsonSerializer.Deserialize<List<User>>(userDataJson);
        }
        catch (Exception ex)
        {
            // Handle any exceptions (e.g., file not found, JSON format error).
            Console.WriteLine($"Error loading user data: {ex.Message}");
            users = new List<User>();
            CurrentUser = null;
        }
    }

    public static User CurrentUser { get; private set; }

    public static bool IsUserAuthenticated => CurrentUser != null;

    public static bool SwitchUser(string username)
    {
        // Check if the provided username exists in the loaded user data.
        User switchedUser = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (switchedUser != null)
        {
            // Update the CurrentUser in AuthService.
            AuthService.CurrentUser = switchedUser;
            return true;
        }

        return false;
    }


    public static bool Authenticate(string username)
    {
        // Check if the provided username exists in the loaded user data.
        User authenticatedUser = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (authenticatedUser != null)
        {
            CurrentUser = authenticatedUser;
            return true;
        }

        return false;
    }

    public static void AddUser(User newUser)
    {
        // Check if the username already exists.
        if (users.Any(u => u.Username.Equals(newUser.Username, StringComparison.OrdinalIgnoreCase)))
        {
            // Username already exists, return without adding.
            return;
        }

        // Add the new user to the list.
        users.Add(newUser);


        SaveUsers();
    }

    private static void SaveUsers()
    {
        string coreFilePath = Path.Combine(UserDataFilePath);
        string coreJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(coreFilePath, coreJson);
    }

    public static List<string> GetAllUsernames()
    {
        return users.Select(u => u.Username).ToList();
    }

    public static void Logout()
    {
        // Log the user out by setting CurrentUser to null.
        CurrentUser = null;
    }
}