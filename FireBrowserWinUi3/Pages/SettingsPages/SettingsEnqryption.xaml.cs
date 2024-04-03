using FireBrowserWinUi3.Services;
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
        SettingsService SettingsService { get; set; }
        public SettingsEnqryption()
        {
            SettingsService = App.GetService<SettingsService>();
            this.InitializeComponent();
            LoadSets();
        }

        public void LoadSets()
        {
            EnqSets.IsOn = SettingsService.CoreSettings.Eqsets;
            Enq2fa.IsOn = SettingsService.CoreSettings.Eq2fa;
            EnqHis.IsOn = SettingsService.CoreSettings.EqHis;
            EnqFav.IsOn = SettingsService.CoreSettings.Eqfav;
        }




        private async void EnqSets_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                var autoSettingValue = toggleSwitch.IsOn;

                // Set the 'Auto' setting
                SettingsService.CoreSettings.Eqsets = autoSettingValue;

                // Save the modified settings back to the user's settings file
                await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);

            }
        }


        private async void Enq2fa_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                var autoSettingValue = toggleSwitch.IsOn;

                // Set the 'Auto' setting
                SettingsService.CoreSettings.Eq2fa = autoSettingValue;


                // Save the modified settings back to the user's settings file
                await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
            }
        }

        private async void EnqHis_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                var autoSettingValue = toggleSwitch.IsOn;

                // Set the 'Auto' setting
                SettingsService.CoreSettings.EqHis = autoSettingValue;

                // Save the modified settings back to the user's settings file
                await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
            }
        }

        private async void EnqFav_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                var autoSettingValue = toggleSwitch.IsOn;

                // Set the 'Auto' setting
                SettingsService.CoreSettings.Eqfav = autoSettingValue;

                // Save the modified settings back to the user's settings file
                await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
            }
        }
    }
}
