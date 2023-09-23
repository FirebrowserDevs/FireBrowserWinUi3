using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FireBrowserMultiCore
{
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
            }
        }

        public static User CurrentUser { get; private set; }

        public static bool IsUserAuthenticated => CurrentUser != null;

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

        public static void Logout()
        {
            // Log the user out by setting CurrentUser to null.
            CurrentUser = null;
        }
    }
}
