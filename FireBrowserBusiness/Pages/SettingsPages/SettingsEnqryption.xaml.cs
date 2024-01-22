using FireBrowserBusinessCore.Helpers;
using FireBrowserBusinessCore.Models;
using FireBrowserMultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

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

        FireBrowserMultiCore.Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
        public void LoadSets()
        {
            EnqSets.IsOn = userSettings.Eqsets == "1" ? true : false;
            Enq2fa.IsOn = userSettings.Eq2fa == "1" ? true : false;
            EnqHis.IsOn = userSettings.EqHis == "1" ? true : false;
            EnqFav.IsOn = userSettings.Eqfav == "1" ? true : false;
        }

     


        private void EnqSets_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

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
                string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";              

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
                string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

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
                string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

                // Set the 'Auto' setting
                userSettings.Eqfav = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }
    }
}
