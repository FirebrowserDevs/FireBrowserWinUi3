using FireBrowserWinUi3.Pages.Patch;
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
    public static SettingsHome Instance { get; set; }
    public bool IsPremium { get; set; }
    public SettingsHome()
    {
        SettingsService = App.GetService<SettingsService>();
        this.InitializeComponent();
        Instance = this;
        LoadUserDataAndSettings();
        LoadUsernames();
        IsPremium = false;
    }

    public void LoadUsernames()
    {
        List<string> usernames = AuthService.GetAllUsernames();
        string currentUsername = AuthService.CurrentUser?.Username;
        // reset first...
        UserListView.Items.Clear();

        if (currentUsername != null && currentUsername.Contains("Private"))
        {
            UserListView.IsEnabled = false;
            Add.IsEnabled = false;
        }
        else
        {
            UserListView.IsEnabled = true;

            Add.IsEnabled = true;

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
        AppService.IsAppNewUser = string.IsNullOrEmpty(AuthService.NewCreatedUser?.Username) ? true : false;
        Window window = new AddUserWindow();

        // do the settings now. 
        await AppService.ConfigureSettingsWindow(window);
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

            UserListView.Items.Clear();
            LoadUsernames();
            // var window = (Application.Current as App)?.m_window as MainWindow;
        }
    }


    private async void PatchBtn_Click(object sender, RoutedEventArgs e)
    {
        PatchUpdate dlg = new PatchUpdate();
        dlg.XamlRoot = this.XamlRoot;
        await dlg.ShowAsync();
    }

    private async void Reset_Click(object sender, RoutedEventArgs e)
    {
        SureReset dlg = new SureReset();
        dlg.XamlRoot = this.XamlRoot;
        await dlg.ShowAsync();
    }

    private async void BackUpNow_Click(object sender, RoutedEventArgs e)
    {
        BackUpDialog dlg = new BackUpDialog();
        dlg.XamlRoot = this.XamlRoot;
        await dlg.ShowAsync();
    }

    private async void RestoreNow_Click(object sender, RoutedEventArgs e)
    {
        RestoreBackupDialog dlg = new RestoreBackupDialog();
        dlg.XamlRoot = this.XamlRoot;
        await dlg.ShowAsync();
    }

    private void LogOutBtn_Click(object sender, RoutedEventArgs e)
    {
        AuthService.Logout();
        Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
    }
}