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
        antitracklevel();

    }

    public void antitracklevel()
    {
        // Assuming userSettings.TrackPrevention is a string that may be null or contain a valid number (0, 1, 2, 3)
        string trackPreventionSetting = userSettings.TrackPrevention;

        // Default to "2" if the setting is null or empty
        trackPreventionSetting = string.IsNullOrEmpty(trackPreventionSetting) ? "2" : trackPreventionSetting;

        // Map the numeric value to the corresponding text value
        string selectedText;
        switch (int.Parse(trackPreventionSetting))
        {
            case 0:
                selectedText = "None";
                break;
            case 1:
                selectedText = "Basic";
                break;
            case 2:
                selectedText = "Balanced";
                break;
            case 3:
                selectedText = "Strict";
                break;
            default:
                // You may want to handle unexpected values here
                selectedText = "Balanced";
                break;
        }

        // Assuming PreventionLevel.ItemsSource contains the text values ("None", "Basic", "Balanced", "Strict")
        PreventionLevel.SelectedItem = selectedText;

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

    private void PreventionLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            string selection = e.AddedItems[0].ToString();
            string antitrack;

            switch (selection)
            {
                case "None":
                    antitrack = "0";
                    Info.Text = "No Privacy Anti Tracking No Effect On Websites";
                    break;
                case "Basic":
                    antitrack = "1";
                    Info.Text = "Basic Privacy Anti Tracking Small Effect On Websites";
                    break;
                case "Balanced":
                    antitrack = "2";
                    Info.Text = "Balanced Privacy Anti Tracking High Level Works With Most Sites";
                    break;
                case "Strict":
                    antitrack = "3";
                    Info.Text = "Strict Privacy Anti Tracking Can Break Some Websites";
                    break;

                default:
                    // Handle the case when selection doesn't match any of the predefined options.
                    antitrack = "2";
                    break;
            }

            if (!string.IsNullOrEmpty(antitrack))
            {

                userSettings.TrackPrevention = antitrack;
                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}
