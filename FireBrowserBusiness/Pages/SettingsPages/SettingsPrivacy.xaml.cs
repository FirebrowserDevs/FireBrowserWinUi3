using FireBrowserMultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
                // Assuming 'url' and 'selection' have been defined earlier
                string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

                // Set the 'Auto' setting
                userSettings.DisableJavaScript = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }

        private void DisableGenaralAutoFillToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

                // Set the 'Auto' setting
                userSettings.DisableGenAutoFill = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }

        private void DisablWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

                // Set the 'Auto' setting
                userSettings.DisableWebMess = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }

        private void PasswordWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

                // Set the 'Auto' setting
                userSettings.DisablePassSave = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
        }
    }
}
