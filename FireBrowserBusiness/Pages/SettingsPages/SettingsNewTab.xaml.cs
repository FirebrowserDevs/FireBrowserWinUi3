using FireBrowserMultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace FireBrowserWinUi3.Pages.SettingsPages;
public sealed partial class SettingsNewTab : Page
{
    Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
    public SettingsNewTab()
    {
        this.InitializeComponent();
        loadsets();
    }

    public void loadsets()
    {
        SearchengineSelection.SelectedItem = userSettings.EngineFriendlyName;

        OpenNew.IsOn = userSettings.OpenTabHandel == "1" ? true : false;
        Trbl.IsOn = userSettings.Translate == "1" ? true : false;
        Adbl.IsOn = userSettings.AdblockBtn == "1" ? true : false;
        Drbl.IsOn = userSettings.DarkIcon == "1" ? true : false;
        Read.IsOn = userSettings.ReadButton == "1" ? true : false;
        Dwbl.IsOn = userSettings.Downloads == "1" ? true : false;
        Frbl.IsOn = userSettings.FavoritesL == "1" ? true : false;
        FlAd.IsOn = userSettings.Favorites == "1" ? true : false;
        Hsbl.IsOn = userSettings.Historybtn == "1" ? true : false;
        Qrbl.IsOn = userSettings.QrCode == "1" ? true : false;
        Tlbl.IsOn = userSettings.ToolIcon == "1" ? true : false;
    }

    private void SearchengineSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            string selection = e.AddedItems[0].ToString();
            string url;

            switch (selection)
            {
                case "Ask":
                    url = "https://www.ask.com/web?q=";
                    break;
                case "Baidu":
                    url = "https://www.baidu.com/s?ie=utf-8&f=8&rsv_bp=1&rsv_idx=1&tn=baidu&wd=";
                    break;
                case "Bing":
                    url = "https://www.bing.com?q=";
                    break;
                case "DuckDuckGo":
                    url = "https://www.duckduckgo.com?q=";
                    break;
                case "Ecosia":
                    url = "https://www.ecosia.org/search?q=";
                    break;
                case "Google":
                    url = "https://www.google.com/search?q=";
                    break;
                case "Startpage":
                    url = "https://www.startpage.com/search?q=";
                    break;
                case "Qwant":
                    url = "https://www.qwant.com/?q=";
                    break;
                case "Qwant Lite":
                    url = "https://lite.qwant.com/?q=";
                    break;
                case "Yahoo!":
                    url = "https://search.yahoo.com/search?p=";
                    break;
                case "Presearch":
                    url = "https://presearch.com/search?q=";
                    break;
                // Add other cases for different search engines.
                default:
                    // Handle the case when selection doesn't match any of the predefined options.
                    url = "https://www.google.com/search?q=";
                    break;
            }

            if (!string.IsNullOrEmpty(url))
            {
                userSettings.EngineFriendlyName = selection;
                userSettings.SearchUrl = url;

                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    private void OpenNew_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.OpenTabHandel = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void Drbl_Toggled(object sender, RoutedEventArgs e)
    {

        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.DarkIcon = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void Trbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.Translate = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void Read_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.ReadButton = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void Adbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.AdblockBtn = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void Dwbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.Downloads = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void Frbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.FavoritesL = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void FlAd_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.Favorites = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void Hsbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.Historybtn = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void Qrbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.QrCode = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void Tlbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.ToolIcon = autoSettingValue;

            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }
}