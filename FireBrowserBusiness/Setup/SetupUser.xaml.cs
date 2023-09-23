using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using FireBrowserMultiCore;
using System.Text.Json;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Setup
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

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            CreateUserOnStartup();
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
        private void CreateUserOnStartup()
        {
            // Create a new user object with a unique username.
            User newUser = new User
            {
                Username = UserName.Text.ToString(), // Generate a unique username.                                                              // Add other user properties as needed.
            };

            // Create a list of users and add the new user to it.
            List<User> users = new List<User>();
            users.Add(newUser);

            // Create the user folders.
            UserFolderManager.CreateUserFolders(newUser);

            // Save the list of users to the JSON file.
            UserDataManager.SaveUsers(users);
            // Authenticate the new user (if needed).
            AuthService.Authenticate(newUser.Username);
            LoadUserSettings(newUser.Username);
        }
    }
}
