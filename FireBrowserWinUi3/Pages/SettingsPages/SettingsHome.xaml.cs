using CommunityToolkit.Mvvm.Messaging;
using FireBrowserWinUi3.Pages.Patch;
using FireBrowserWinUi3.Services;
using FireBrowserWinUi3.Services.Messages;
using FireBrowserWinUi3Core.Models;
using FireBrowserWinUi3Exceptions;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Pages.SettingsPages;
public sealed partial class SettingsHome : Page
{
    SettingsService SettingsService { get; set; }
    public static SettingsHome Instance { get; set; }

    IMessenger Messenger { get; set; }
    public bool IsPremium { get; set; }
    public SettingsHome()
    {
        SettingsService = App.GetService<SettingsService>();
        Messenger = App.GetService<IMessenger>();
        this.InitializeComponent();
        Instance = this;
        LoadUserDataAndSettings();
        LoadUsernames();
        IsPremium = false;
    }

    public Task LoadUsernames()
    {
        List<string> usernames = AuthService.GetAllUsernames();
        string currentUsername = AuthService.CurrentUser?.Username;
        // reset first...
        //UserListView.Items.Clear();

        if (currentUsername != null && currentUsername.Contains("Private"))
        {
            UserListView.IsEnabled = false;
            Add.IsEnabled = false;
        }
        else
        {
            UserListView.IsEnabled = true;

            Add.IsEnabled = true;

            UserListView.ItemsSource = usernames.Where(username => username != currentUsername && !username.Contains("Private")).ToList();

        }
        return Task.CompletedTask;
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

    private async void Delete_Click(object sender, RoutedEventArgs e)
    {

        try
        {
            if (sender is Button switchButton && switchButton.DataContext is string clickedUserName)
            {
                UserDataManager.DeleteUser(clickedUserName);

                UserListView.ItemsSource = null;
                // allow ui to updated
                await LoadUsernames();

                Messenger?.Send(new Message_Settings_Actions($"User:  {clickedUserName} has been removed from FireBrowser", EnumMessageStatus.Removed));
                // var window = (Application.Current as App)?.m_window as MainWindow;
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex!);
            Messenger?.Send(new Message_Settings_Actions($"You may not remove a User that has an Active Session !", EnumMessageStatus.XorError));

        }

    }

    private void UpdateApp()
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = $"upgrade --name \"FireBrowserWinUi\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    string msg2 = Regex.Replace(result, @"[\r*\-\\]", "");
                    Messenger?.Send(new Message_Settings_Actions($"Application update status\n\n{msg2.Trim()} !", EnumMessageStatus.Informational));
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex!);
            Messenger?.Send(new Message_Settings_Actions($"Application update failed !", EnumMessageStatus.XorError));

        }


    }
    private async void PatchBtn_Click(object sender, RoutedEventArgs e)
    {
        //https://www.microsoft.com/en-us/videoplayer/embed/RE3i5DH
        //StoreContext storeContext = StoreContext.GetDefault();"9PCN40XXVCVB"
        //StoreProductResult result = await storeContext.GetStoreProductForCurrentAppAsync();

        // here is how we were discussing winget for updates. 
        UpdateApp();

        //PatchUpdate dlg = new PatchUpdate();
        //dlg.XamlRoot = this.XamlRoot;
        //await dlg.ShowAsync();
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