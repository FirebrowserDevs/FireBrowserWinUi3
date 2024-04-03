using FireBrowserWinUi3.Services;
using FireBrowserWinUi3Core.Models;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FireBrowserWinUi3.Pages.SettingsPages;
public sealed partial class SettingsHome : Page
{
    SettingsService SettingsService { get; set; }
    public SettingsHome()
    {
        SettingsService = App.GetService<SettingsService>();
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

    private FireBrowserWinUi3MultiCore.User GetUser() =>
      AuthService.IsUserAuthenticated ? AuthService.CurrentUser : null;

    private void LoadUserDataAndSettings()
    {
        User.Text = GetUser()?.Username ?? "DefaultUser";
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
            UserListView.Items.Clear();
            LoadUsernames();
            window.LoadUsernames();
        };

        await quickConfigurationDialog.ShowAsync();
    }

    public static async void OpenNewWindow(Uri uri)
    {
        await Windows.System.Launcher.LaunchUriAsync(uri);
    }


    private async void Switch_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button switchButton && switchButton.DataContext is string clickedUserName)
        {
            OpenNewWindow(new Uri($"firebrowseruser://{clickedUserName}"));
            Shortcut ct = new();
            await ct.CreateShortcut(clickedUserName);
        }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {

        if (sender is Button switchButton && switchButton.DataContext is string clickedUserName)
        {
            UserDataManager.DeleteUser(clickedUserName);
            UserListView.Items.Remove(clickedUserName);
            var window = (Application.Current as App)?.m_window as MainWindow;
        }
    }
}