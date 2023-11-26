using FireBrowserBusiness;
using FireBrowserMultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsHome : Page
    {

        public SettingsHome()
        {
            this.InitializeComponent();
            LoadUserDataAndSettings();
            LoadUsernames();
        }

        private void LoadUsernames()
        {
            List<string> usernames = AuthService.GetAllUsernames();
            string currentUsername = AuthService.CurrentUser?.Username;

            foreach (string username in usernames)
            {
                // Exclude the current user's username
                if (username != currentUsername)
                {
                    UserListView.Items.Add(username);
                }
            }

            // Disable the "Add" button if the total count is equal to or greater than a limit
            if (usernames.Count + (currentUsername != null ? 1 : 0) >= 6)
            {
                Add.IsEnabled = false; // Assuming AddButton is the name of your "Add" button
            }
            else
            {
                Add.IsEnabled = true;
            }
        }
        private FireBrowserMultiCore.User GetUser()
        {
            // Check if the user is authenticated.
            if (AuthService.IsUserAuthenticated)
            {
                // Return the authenticated user.
                return AuthService.CurrentUser;
            }

            // If no user is authenticated, return null or handle as needed.
            return null;
        }

        Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
        private void LoadUserDataAndSettings()
        {
            if (GetUser() is not { } currentUser)
            {
                User.Text = "DefaultUser";
                return;
            }

            if (!AuthService.IsUserAuthenticated && !AuthService.Authenticate(currentUser.Username))
            {
                return;
            }

            User.Text = AuthService.CurrentUser?.Username ?? "DefaultUser";
        }


        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = (Application.Current as App)?.m_window as MainWindow;
            AddUser quickConfigurationDialog = new()
            {
                XamlRoot = window.Content.XamlRoot
            };

            quickConfigurationDialog.PrimaryButtonClick += async (sender, args) =>
            {
                // Handle the primary button click

                // Reload the ListView items
                UserListView.Items.Clear();
                LoadUsernames();

                // Optionally, you can await an asynchronous operation here if needed
            };

            await quickConfigurationDialog.ShowAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is string username)
            {
                MainWindow newWindow = new();
                newWindow.Activate();
            }
        }
    }
}
