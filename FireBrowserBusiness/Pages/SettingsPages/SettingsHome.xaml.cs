using FireBrowserBusiness;
using FireBrowserBusinessCore.Models;
using FireBrowserMultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FireBrowserWinUi3.Pages.SettingsPages;

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

        if (currentUsername != null && currentUsername.Contains("Private"))
        {
            UserListView.IsEnabled = false;
            Add.IsEnabled = false;
        }
        else
        {
            UserListView.IsEnabled = true;

            int nonPrivateUserCount = usernames.Count(username => !username.Contains("Private"));

            if (nonPrivateUserCount + (currentUsername != null && !currentUsername.Contains("Private") ? 1 : 0) >= 6)
            {
                Add.IsEnabled = false; // Assuming AddButton is the name of your "Add" button
            }
            else
            {
                Add.IsEnabled = true;
            }

            foreach (string username in usernames.Where(username => username != currentUsername && !username.Contains("Private")))
            {
                UserListView.Items.Add(username);
            }
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

    FireBrowserMultiCore.Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
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

    public static async void OpenNewWindow(Uri uri)
    {
        await Windows.System.Launcher.LaunchUriAsync(uri);
    }


    private void Switch_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button switchButton && switchButton.DataContext is string clickedUserName)
        {
            OpenNewWindow(new Uri($"firebrowseruser://{clickedUserName}"));
            Shortcut ct = new();
            ct.CreateShortcut(clickedUserName);
        }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {

        if (sender is Button switchButton && switchButton.DataContext is string clickedUserName)
        {
            UserDataManager.DeleteUser(clickedUserName);
            UserListView.Items.Remove(clickedUserName);
        }
    }
}