using FireBrowserMultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserSetup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupUser : Page
    {
        public SetupUser()
        {
            this.InitializeComponent();
        }

        private async void Create_Click(object sender, RoutedEventArgs e)
        {
            await CreateUserOnStartup();
            Thread.Sleep(500);
            Frame.Navigate(typeof(SetupUi));
        }

        public static Settings LoadUserSettings(string username)
        {
            string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username);
            string settingsFolderPath = Path.Combine(userFolderPath, "Settings");
            string settingsFilePath = Path.Combine(settingsFolderPath, "settings.json");

            try
            {
                if (File.Exists(settingsFilePath))
                {
                    // Read the JSON content from settings.json
                    string jsonContent = File.ReadAllText(settingsFilePath);

                    // Deserialize the JSON content into the UserSettings object
                    var userSettings = JsonSerializer.Deserialize<Settings>(jsonContent);

                    return userSettings;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during file reading or deserialization
                Console.WriteLine("Error reading settings.json: " + ex.Message);
            }

            // Return a default UserSettings object or handle the error as needed
            return new Settings();
        }

        private async Task CreateUserOnStartup()
        {
            // Create a new user object with a unique username.
            User newUser = new User
            {
                Username = UserName.Text, // Generate a unique username.                                                              // Add other user properties as needed.
            };

            // Create a list of users and add the new user to it.
            List<User> users = new List<User>();
            users.Add(newUser);

            // Create the user folders.
            UserFolderManager.CreateUserFolders(newUser);

            // Save the list of users to the JSON file.
            UserDataManager.SaveUsers(users);
            //AuthService.InitAuthService();
            // Authenticate the new user (if needed).
            AuthService.Authenticate(newUser.Username);
        }
    }
}
