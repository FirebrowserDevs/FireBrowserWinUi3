using FireBrowserMultiCore;
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
        DisableJavaScriptToggle.IsOn = userSettings.DisableJavaScript == "1" ? true : false;
        DisablWebMessFillToggle.IsOn = userSettings.DisableWebMess == "1" ? true : false;
        DisableGenaralAutoFillToggle.IsOn = userSettings.DisableGenAutoFill == "1" ? true : false;
        PasswordWebMessFillToggle.IsOn = userSettings.DisablePassSave == "1" ? true : false;
    }

    private void DisableJavaScriptToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.DisableJavaScript = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void DisableGenaralAutoFillToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.DisableGenAutoFill = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void DisablWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.DisableWebMess = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void PasswordWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

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