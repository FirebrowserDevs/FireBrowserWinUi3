using FireBrowserWinUi3.Services;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace FireBrowserWinUi3.Pages.SettingsPages;
public sealed partial class SettingsNewTab : Page
{
    SettingsService SettingsService { get; set; }
    public SettingsNewTab()
    {
        SettingsService = App.GetService<SettingsService>();
        this.InitializeComponent();
        loadsets();
    }

    public void loadsets()
    {
        SearchengineSelection.SelectedItem = SettingsService.CoreSettings.EngineFriendlyName;

        OpenNew.IsOn = SettingsService.CoreSettings.OpenTabHandel;
        Trbl.IsOn = SettingsService.CoreSettings.Translate;
        Adbl.IsOn = SettingsService.CoreSettings.AdblockBtn;
        Drbl.IsOn = SettingsService.CoreSettings.DarkIcon;
        Read.IsOn = SettingsService.CoreSettings.ReadButton;
        Dwbl.IsOn = SettingsService.CoreSettings.Downloads;
        Frbl.IsOn = SettingsService.CoreSettings.FavoritesL;
        FlAd.IsOn = SettingsService.CoreSettings.Favorites;
        Hsbl.IsOn = SettingsService.CoreSettings.Historybtn;
        Qrbl.IsOn = SettingsService.CoreSettings.QrCode;
        Tlbl.IsOn = SettingsService.CoreSettings.ToolIcon;
        BackSettings.IsOn = SettingsService.CoreSettings.BackButton;
        ForwardSettings.IsOn = SettingsService.CoreSettings.ForwardButton;
        ReloadSettings.IsOn = SettingsService.CoreSettings.RefreshButton;
        HomeSettings.IsOn = SettingsService.CoreSettings.HomeButton;
        Confirm.IsOn = SettingsService.CoreSettings.ConfirmCloseDlg;
    }

    private async void SearchengineSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            string selection = e.AddedItems[0].ToString();

            // Dictionary to map search engine names to their respective URLs
            Dictionary<string, string> searchEngines = new Dictionary<string, string>
        {
            { "Ask", "https://www.ask.com/web?q=" },
            { "Baidu", "https://www.baidu.com/s?ie=utf-8&f=8&rsv_bp=1&rsv_idx=1&tn=baidu&wd=" },
            { "Bing", "https://www.bing.com?q=" },
            { "DuckDuckGo", "https://www.duckduckgo.com?q=" },
            { "Ecosia", "https://www.ecosia.org/search?q=" },
            { "Google", "https://www.google.com/search?q=" },
            { "Startpage", "https://www.startpage.com/search?q=" },
            { "Qwant", "https://www.qwant.com/?q=" },
            { "Qwant Lite", "https://lite.qwant.com/?q=" },
            { "Yahoo!", "https://search.yahoo.com/search?p=" },
            { "Presearch", "https://presearch.com/search?q=" },
            { "Swisscows", "https://swisscows.com/web?query=" },
            { "Dogpile", "https://www.dogpile.com/serp?q=" },
            { "Webcrawler", "https://www.webcrawler.com/serp?q=" },
            { "You", "https://you.com/search?q=" },
            { "Excite", "https://results.excite.com/serp?q=" },
            { "Lycos", "https://search20.lycos.com/web/?q=" },
            { "Metacrawler", "https://www.metacrawler.com/serp?q=" },
            { "Mojeek", "https://www.mojeek.com/search?q=" },
            { "BraveSearch", "https://search.brave.com/search?q=" },
            // Add other cases for different search engines.
        };

            string url = searchEngines.ContainsKey(selection) ? searchEngines[selection] : "https://www.google.com/search?q=";

            if (!string.IsNullOrEmpty(url))
            {
                SettingsService.CoreSettings.EngineFriendlyName = selection;
                SettingsService.CoreSettings.SearchUrl = url;

                await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }


    private async void ToggleSetting(string settingName, bool value)
    {
        // Set the specified setting
        switch (settingName)
        {
            case "OpenTabHandel":
                SettingsService.CoreSettings.OpenTabHandel = value;
                break;
            case "DarkIcon":
                SettingsService.CoreSettings.DarkIcon = value;
                break;
            case "Translate":
                SettingsService.CoreSettings.Translate = value;
                break;
            case "ReadButton":
                SettingsService.CoreSettings.ReadButton = value;
                break;
            case "AdblockBtn":
                SettingsService.CoreSettings.AdblockBtn = value;
                break;
            case "Downloads":
                SettingsService.CoreSettings.Downloads = value;
                break;
            case "FavoritesL":
                SettingsService.CoreSettings.FavoritesL = value;
                break;
            case "Favorites":
                SettingsService.CoreSettings.Favorites = value;
                break;
            case "Historybtn":
                SettingsService.CoreSettings.Historybtn = value;
                break;
            case "QrCode":
                SettingsService.CoreSettings.QrCode = value;
                break;
            case "ToolIcon":
                SettingsService.CoreSettings.ToolIcon = value;
                break;
            case "ConfirmCloseDlg":
                SettingsService.CoreSettings.ConfirmCloseDlg = value;
                break;
            case "BackButton":
                SettingsService.CoreSettings.BackButton = value;
                break;
            case "ForwardButton":
                SettingsService.CoreSettings.ForwardButton = value;
                break;
            case "HomeButton":
                SettingsService.CoreSettings.HomeButton = value;
                break;
            case "RefreshButton":
                SettingsService.CoreSettings.RefreshButton = value;
                break;
            // Add other cases for different settings.
            default:
                throw new ArgumentException("Invalid setting name");
        }

        // Save the modified settings back to the user's settings file
        await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
    }

    private async void ToggleSetting_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;
            var settingName = toggleSwitch.Tag as string;
            ToggleSetting(settingName, autoSettingValue);
        }
    }

}