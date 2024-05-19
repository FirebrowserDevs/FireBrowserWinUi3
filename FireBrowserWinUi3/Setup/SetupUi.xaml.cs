using FireBrowserWinUi3.Services;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.AppService;
using Windows.UI.ApplicationSettings;
using static System.Net.Mime.MediaTypeNames;

namespace FireBrowserWinUi3;
public sealed partial class SetupUi : Page
{
    public SetupUi()
    {
        this.InitializeComponent();
        ColorTV.Text = "#000000";
        ColorTB.Text = "#000000";
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

    private void SetupUiBtn_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(SetupAlgemeen));
    }

    private void AutoTog_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            // Assuming 'url' and 'selection' have been defined earlier
            var autoSettingValue = toggleSwitch.IsOn;

            AppService.AppSettings.Auto = autoSettingValue;

            // Load the user's settings
            //Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());


            //// Set the 'Auto' setting
            //userSettings.Auto = autoSettingValue;

            //// Save the modified settings back to the user's settings file
            //UserFolderManager.SaveUserSettings(GetUser(), userSettings);
        }
    }

    private void ColorTB_TextChanged(object sender, TextChangedEventArgs e)
    {
        string color = ColorTB.Text.ToString();
        if (!string.IsNullOrEmpty(color))
        {

            AppService.AppSettings.ColorBackground = color;

            // Load the user's settings
            //Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());

            //userSettings.ColorBackground = color;

            //// Save the modified settings back to the user's settings file
            //UserFolderManager.SaveUserSettings(GetUser(), userSettings);
        }
    }

    private void ColorTV_TextChanged(object sender, TextChangedEventArgs e)
    {
        string color = ColorTV.Text.ToString();
        if (!string.IsNullOrEmpty(color))
        {
            AppService.AppSettings.ColorTool = color;
            //// Load the user's settings
            //Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());

            //userSettings.ColorTool = color;

            //// Save the modified settings back to the user's settings file
            //UserFolderManager.SaveUserSettings(GetUser(), userSettings);
        }
    }

    private void DateTime_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            // Assuming 'url' and 'selection' have been defined earlier
            var autoSettingValue = toggleSwitch.IsOn;

            AppService.AppSettings.NtpDateTime = autoSettingValue; ;
            // Load the user's settings
            //Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());


            //// Set the 'Auto' setting
            //userSettings.NtpDateTime = autoSettingValue;

            //// Save the modified settings back to the user's settings file
            //UserFolderManager.SaveUserSettings(GetUser(), userSettings);
        }
    }

    private void NtpColorText_TextChanged(object sender, TextChangedEventArgs e)
    {
        string color = NtpColorText.Text.ToString();
        if (!string.IsNullOrEmpty(color))
        {
            AppService.AppSettings.NtpTextColor = color; ;

            // Load the user's settings
            //Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());

            //userSettings.NtpTextColor = color;

            //// Save the modified settings back to the user's settings file
            //UserFolderManager.SaveUserSettings(GetUser(), userSettings);
        }
    }

    private void Color_TextChanged(object sender, TextChangedEventArgs e)
    {

        string color = Color.Text.ToString();
        if (!string.IsNullOrEmpty(color))
        {
            AppService.AppSettings.ColorTV = color;

            //// Load the user's settings
            //Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());

            //userSettings.ColorTV = color;

            //// Save the modified settings back to the user's settings file
            //UserFolderManager.SaveUserSettings(GetUser(), userSettings);
        }
    }

    private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selection = e.AddedItems[0].ToString();
        int type = default;


        switch (selection)
        {
            case "Default":
                type = 0;
                Color.IsEnabled = false;
                Image.Visibility = Visibility.Visible;
                Image2.Visibility = Visibility.Collapsed;
                Image3.Visibility = Visibility.Collapsed;
                break;
            case "Featured":
                type = 1;
                Color.IsEnabled = false;
                Image2.Visibility = Visibility.Visible;
                Image.Visibility = Visibility.Collapsed;
                Image3.Visibility = Visibility.Collapsed;
                break;
            case "Custom":
                type = 2;
                Color.IsEnabled = true;
                Image3.Visibility = Visibility.Visible;
                Image.Visibility = Visibility.Collapsed;
                Image2.Visibility = Visibility.Collapsed;
                break;

            // Add other cases for different search engines.
            default:
                // Handle the case when selection doesn't match any of the predefined options.
                type = 0;
                Color.IsEnabled = false;
                break;
        }

        AppService.AppSettings.Background = type;

        // Load the user's settings
        //Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());

        //userSettings.Background = type;

        //// Save the modified settings back to the user's settings file
        //UserFolderManager.SaveUserSettings(GetUser(), userSettings);

    }
}
