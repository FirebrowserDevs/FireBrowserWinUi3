using FireBrowserWinUi3.Services;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FireBrowserWinUi3;
public sealed partial class SetupWebView : Page
{
    public SetupWebView()
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
    private void StatusTog_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            // Assuming 'url' and 'selection' have been defined earlier
            var autoSettingValue = toggleSwitch.IsOn;
            AppService.AppSettings.StatusBar = autoSettingValue;

        }
    }

    private void BrowserKeys_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            // Assuming 'url' and 'selection' have been defined earlier
            var autoSettingValue = toggleSwitch.IsOn;
            AppService.AppSettings.BrowserKeys = autoSettingValue; ;
        }
    }

    private void BrowserScripts_Toggled(object sender, RoutedEventArgs e)
    {

        if (sender is ToggleSwitch toggleSwitch)
        {
            // Assuming 'url' and 'selection' have been defined earlier
            var autoSettingValue = toggleSwitch.IsOn;
            AppService.AppSettings.BrowserScripts = autoSettingValue;
        }
    }

    private void userag_TextChanged(object sender, TextChangedEventArgs e)
    {
        string blob = userag.Text.ToString();
        if (!string.IsNullOrEmpty(blob))
        {
            AppService.AppSettings.Useragent = blob;
        }
    }

    private void SetupWebViewBtn_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(SetupFinish));
    }
}
