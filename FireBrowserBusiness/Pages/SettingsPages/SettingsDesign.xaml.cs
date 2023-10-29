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
    public sealed partial class SettingsDesign : Page
    {
        Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
        public SettingsDesign()
        {
            this.InitializeComponent();
            Init();
            Check();
        }

        public void Init()
        {
            AutoTog.IsOn = userSettings.Auto == "1" ? true : false;
            ColorTB.Text = userSettings.ColorTool;
            ColorTV.Text = userSettings.ColorTV;
            Color.Text = userSettings.ColorBackground;
        }

        public void Check()
        {
            Type.SelectedItem = userSettings.Background switch
            {
                "0" => "Default",
                "1" => "Featured",
                "2" => "Custom",
                _ => Type.SelectedItem
            };
        }

        private void AutoTog_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Assuming 'url' and 'selection' have been defined earlier
                string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

                // Set the 'Auto' setting
                userSettings.Auto = autoSettingValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }

        private void ColorTB_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (AuthService.CurrentUser != null)
            {
                // Update the "ColorBackground" setting for the current user
                userSettings.ColorTool = ColorTB.Text.ToString();

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
            else
            {
                // Handle the case when there is no authenticated user.
            }
        }

        private void ColorTV_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (AuthService.CurrentUser != null)
            {
                // Update the "ColorBackground" setting for the current user
                userSettings.ColorTV = ColorTV.Text.ToString();

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
            else
            {
                // Handle the case when there is no authenticated user.
            }
        }

        private void Color_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AuthService.CurrentUser != null)
            {
                // Update the "ColorBackground" setting for the current user
                userSettings.ColorBackground = Color.Text.ToString();

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
            else
            {
                // Handle the case when there is no authenticated user.
            }
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = e.AddedItems[0].ToString();
            if (selection == "Default")
            {
                Color.IsEnabled = false;
                userSettings.Background = "0";
            }
            if (selection == "Featured")
            {
                Color.IsEnabled = false;
                userSettings.Background = "1";
            }
            if (selection == "Custom")
            {
                Color.IsEnabled = true;
                userSettings.Background = "2";
            }

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }
}
