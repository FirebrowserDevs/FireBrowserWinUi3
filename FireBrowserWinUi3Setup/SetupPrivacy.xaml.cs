using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupPrivacy : Page
    {
        public SetupPrivacy()
        {
            this.InitializeComponent();
        }

        private FireBrowserWinUi3MultiCore.User GetUser()
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
        private void DisableJavaScriptToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                var autoSettingValue = toggleSwitch.IsOn    ;

                // Load the user's settings
                Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());


                // Set the 'Auto' setting
                userSettings.DisableJavaScript = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(GetUser(), userSettings);
            }
        }

        private void DisableGenaralAutoFillToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                var autoSettingValue = toggleSwitch.IsOn;

                // Load the user's settings
                Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());


                // Set the 'Auto' setting
                userSettings.DisableGenAutoFill = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(GetUser(), userSettings);
            }
        }

        private void DisablWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                var autoSettingValue = toggleSwitch.IsOn;

                // Load the user's settings
                Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());


                // Set the 'Auto' setting
                userSettings.DisableWebMess = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(GetUser(), userSettings);
            }
        }

        private void PasswordWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                var autoSettingValue = toggleSwitch.IsOn;

                // Load the user's settings
                Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());


                // Set the 'Auto' setting
                userSettings.DisablePassSave = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(GetUser(), userSettings);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SetupAccess));
        }
    }
}
