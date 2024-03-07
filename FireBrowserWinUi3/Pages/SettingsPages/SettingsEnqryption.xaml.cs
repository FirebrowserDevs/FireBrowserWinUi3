using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsEnqryption : Page
    {
        public SettingsEnqryption()
        {
            this.InitializeComponent();
            LoadSets();
        }

        FireBrowserWinUi3MultiCore.Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
        public void LoadSets()
        {
            EnqSets.IsOn = userSettings.Eqsets;
            Enq2fa.IsOn = userSettings.Eq2fa;
            EnqHis.IsOn = userSettings.EqHis;
            EnqFav.IsOn = userSettings.Eqfav;
        }




        private void EnqSets_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                var autoSettingValue = toggleSwitch.IsOn;

                // Set the 'Auto' setting
                userSettings.Eqsets = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }


        private async void Enq2fa_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                var autoSettingValue = toggleSwitch.IsOn;

                // Set the 'Auto' setting
                userSettings.Eq2fa = autoSettingValue;



                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }

        private void EnqHis_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                var autoSettingValue = toggleSwitch.IsOn;

                // Set the 'Auto' setting
                userSettings.EqHis = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }

        private void EnqFav_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                var autoSettingValue = toggleSwitch.IsOn;

                // Set the 'Auto' setting
                userSettings.Eqfav = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }
    }
}
