using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using FireBrowserMultiCore;
using System.Diagnostics;
using System.Text.Json;
using ABI.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupUi : Page
    {

        public SetupUi()
        {
            this.InitializeComponent();
        }

        private FireBrowserMultiCore.User GetUser()
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

      
        private void SetupUiBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SetupAlgemeen));
        }

      

        private void AutoTog_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

                // Load the user's settings
                Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());


                // Set the 'Auto' setting
                userSettings.Auto = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(GetUser(), userSettings);
            }

        }

        private void ColorTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string color = ColorTB.Text.ToString();
            if (!string.IsNullOrEmpty(color))
            {
                // Load the user's settings
                Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());

                userSettings.ColorBackground = color;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(GetUser(), userSettings);
            }
        }

        private void ColorTV_TextChanged(object sender, TextChangedEventArgs e)
        {
            string color = ColorTV.Text.ToString();
            if (!string.IsNullOrEmpty(color))
            {
                // Load the user's settings
                Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());

                userSettings.ColorTool = color;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(GetUser(), userSettings);
            }
        }

        private void Color_TextChanged(object sender, TextChangedEventArgs e)
        {
            string color = Color.Text.ToString();
            if (!string.IsNullOrEmpty(color))
            {
                // Load the user's settings
                Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());

                userSettings.ColorTV = color;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(GetUser(), userSettings);
            }
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = e.AddedItems[0].ToString();
            string type;

            switch (selection)
            {
                case "Default":
                    type = "0";
                    Color.IsEnabled = false;
                    break;
                case "Featured":
                    type = "1";
                    Color.IsEnabled = false;
                    break;
                case "Custom":
                    type = "2";
                    Color.IsEnabled = true;
                    break;
               
                // Add other cases for different search engines.
                default:
                    // Handle the case when selection doesn't match any of the predefined options.
                    type = "0";
                    Color.IsEnabled = false;
                    break;
            }

            if (!string.IsNullOrEmpty(type))
            {
                // Load the user's settings
                Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());

                userSettings.Background = type;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(GetUser(), userSettings);
            }
        }
    }
}
