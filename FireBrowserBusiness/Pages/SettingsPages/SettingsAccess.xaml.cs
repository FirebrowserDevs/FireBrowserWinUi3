using FireBrowserMultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace FireBrowserWinUi3.Pages.SettingsPages;
public sealed partial class SettingsAccess : Page
{
    bool isMode;

    FireBrowserMultiCore.Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
    public SettingsAccess()
    {
        this.InitializeComponent();
        LoadUserDataAndSettings();
        id();
    }

    private void LoadUserDataAndSettings()
    {
        Langue.SelectedValue = userSettings.Lang;
        Logger.SelectedValue = userSettings.ExceptionLog;
        bool isMode = userSettings.LightMode == "1";
        LiteMode.IsOn = isMode;
    }
    private void LiteMode_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            isMode = toggleSwitch.IsOn;
            string autoValue = isMode ? "1" : "0";


            if (AuthService.CurrentUser != null)
            {
                // Update the "Auto" setting for the current user
                userSettings.LightMode = autoValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
            else
            {
                // Handle the case when there is no authenticated user.
            }
        }
    }

    private void Langue_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selection = e.AddedItems[0].ToString();
        string langSetting = selection switch
        {
            "nl-NL" => "nl-NL",
            "en-US" => "en-US",
            _ => throw new ArgumentException("Invalid selection")
        };

        // Update the "Auto" setting for the current user
        userSettings.Lang = langSetting;

        // Save the modified settings back to the user's settings file
        UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
    }

    private async void id()
    {
        var startup = await StartupTask.GetAsync("FireBrowserWinUiStartUp");
        UpdateToggleState(startup.State);
    }

    private void UpdateToggleState(StartupTaskState state)
    {
        LaunchOnStartupToggle.IsEnabled = true;
        LaunchOnStartupToggle.IsChecked = state switch
        {
            StartupTaskState.Enabled => true,
            StartupTaskState.Disabled => false,
            StartupTaskState.DisabledByUser => false,
            _ => LaunchOnStartupToggle.IsEnabled = false
        };
    }
    private async Task ToggleLaunchOnStartup(bool enable)
    {
        var startup = await StartupTask.GetAsync("FireBrowserWinUiStartUp");

        switch (startup.State)
        {
            case StartupTaskState.Enabled when !enable:
                startup.Disable();
                break;
            case StartupTaskState.Disabled when enable:
                var updatedState = await startup.RequestEnableAsync();
                UpdateToggleState(updatedState);
                break;
            case StartupTaskState.DisabledByUser when enable:
                ContentDialog cs = new ContentDialog();
                cs.Title = "Unable to change state of startup task via the application";
                cs.Content = "Enable via Startup tab on Task Manager (Ctrl+Shift+Esc)";
                cs.PrimaryButtonText = "OK";
                await cs.ShowAsync();
                break;
            default:
                ContentDialog cs2 = new ContentDialog();
                cs2.Title = "Unable to change state of startup task";
                cs2.PrimaryButtonText = "OK";
                await cs2.ShowAsync();
                break;
        }
    }

    private async void LaunchOnStartupToggle_Click(object sender, RoutedEventArgs e)
    {
        await ToggleLaunchOnStartup(LaunchOnStartupToggle.IsChecked ?? false);
    }

    private void Logger_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selection = e.AddedItems[0].ToString();
        string langSetting = selection switch
        {
            "Low" => "Low",
            "High" => "High",
            _ => throw new ArgumentException("Invalid selection")
        };

        // Update the "Auto" setting for the current user
        userSettings.ExceptionLog = langSetting;

        // Save the modified settings back to the user's settings file
        UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
    }
}
