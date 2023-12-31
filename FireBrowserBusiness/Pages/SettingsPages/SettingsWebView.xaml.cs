using FireBrowserMultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;

namespace FireBrowserWinUi3.Pages.SettingsPages;

public sealed partial class SettingsWebView : Page
{
    Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
    WebView2 Web = new WebView2();
    public SettingsWebView()
    {
        this.InitializeComponent();
        loadinit();
    }

    public void loadinit()
    {
        Agent.Text = userSettings.Useragent;
        StatusTog.IsOn = userSettings.StatusBar == "1" ? true : false;
        BrowserKeys.IsOn = userSettings.BrowserKeys == "1" ? true : false;
        BrowserScripts.IsOn = userSettings.BrowserScripts == "1" ? true : false;
    }

    private async void ClearCookies_Click(object sender, RoutedEventArgs e)
    {
        await Web.EnsureCoreWebView2Async();
        Web.CoreWebView2.CookieManager.DeleteAllCookies();
    }

    private async void ClearCache_Click(object sender, RoutedEventArgs e)
    {
        ClearAutofillData();
    }

    private async void ClearAutofillData()
    {
        CoreWebView2Profile profile;
        if (Web.CoreWebView2 != null)
        {
            profile = Web.CoreWebView2.Profile;
            // Get the current time, the time in which the browsing data will be cleared
            // until.
            System.DateTime endTime = DateTime.Now;
            System.DateTime startTime = DateTime.Now.AddHours(-1);
            // Offset the current time by one hour to clear the browsing data from the
            // last hour.
            CoreWebView2BrowsingDataKinds dataKinds = (CoreWebView2BrowsingDataKinds)
                                     (CoreWebView2BrowsingDataKinds.DiskCache |
                                      CoreWebView2BrowsingDataKinds.CacheStorage);
            await profile.ClearBrowsingDataAsync(dataKinds, startTime, endTime);
        }
    }

    private void Agent_TextChanged(object sender, TextChangedEventArgs e)
    {
        string autoSettingValue = Agent.Text.ToString();

        // Set the 'Auto' setting
        userSettings.Useragent = autoSettingValue;

        // Save the modified settings back to the user's settings file
        UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
    }

    private void StatusTog_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            // Assuming 'url' and 'selection' have been defined earlier
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            // Set the 'Auto' setting
            userSettings.StatusBar = autoSettingValue;

            // Save the modified settings back to the user's settings file
            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void BrowserKeys_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            // Assuming 'url' and 'selection' have been defined earlier
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            // Set the 'Auto' setting
            userSettings.BrowserKeys = autoSettingValue;

            // Save the modified settings back to the user's settings file
            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void BrowserScripts_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            // Assuming 'url' and 'selection' have been defined earlier
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            // Set the 'Auto' setting
            userSettings.BrowserScripts = autoSettingValue;

            // Save the modified settings back to the user's settings file
            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }
}
