using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FireBrowserWinUi3.Pages.SettingsPages;

public sealed partial class SettingsPrivacy : Page
{
    Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
    public SettingsPrivacy()
    {
        this.InitializeComponent();
        Stack();
    }

    public void Stack()
    {
        DisableJavaScriptToggle.IsOn = userSettings.DisableJavaScript;
        DisablWebMessFillToggle.IsOn = userSettings.DisableWebMess;
        DisableGenaralAutoFillToggle.IsOn = userSettings.DisableGenAutoFill;
        PasswordWebMessFillToggle.IsOn = userSettings.DisablePassSave;
    }

    private void DisableJavaScriptToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            userSettings.DisableJavaScript = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void DisableGenaralAutoFillToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            userSettings.DisableGenAutoFill = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void DisablWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            userSettings.DisableWebMess = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void PasswordWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            userSettings.DisablePassSave = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
    }

    private void CamPermission_Toggled(object sender, RoutedEventArgs e)
    {

    }
}