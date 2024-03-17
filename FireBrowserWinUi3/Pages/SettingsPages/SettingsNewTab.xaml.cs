using FireBrowserWinUi3.Services;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

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
     //   BackSettings.IsOn = SettingsService.CoreSettings.BackButton;
       // ForwardSettings.IsOn = SettingsService.CoreSettings.ForwardButton;
       // ReloadSettings.IsOn = SettingsService.CoreSettings.RefreshButton;
       // HomeSettings.IsOn = SettingsService.CoreSettings.HomeButton;
        Confirm.IsOn = SettingsService.CoreSettings.ConfirmCloseDlg;
    }

    private async void SearchengineSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

    private async void OpenNew_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.OpenTabHandel = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void Drbl_Toggled(object sender, RoutedEventArgs e)
    {

        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.DarkIcon = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void Trbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.Translate = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void Read_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.ReadButton = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void Adbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.AdblockBtn = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void Dwbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.Downloads = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void Frbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.FavoritesL = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void FlAd_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.Favorites = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void Hsbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.Historybtn = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void Qrbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.QrCode = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void Tlbl_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.ToolIcon = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void Confirm_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.ConfirmCloseDlg = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void BackSettings_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            //SettingsService.CoreSettings.BackButton = autoSettingValue;

           // await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void ForwardSettings_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

          //  SettingsService.CoreSettings.ForwardButton = autoSettingValue;

           // await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void HomeSettings_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

           // SettingsService.CoreSettings.RefreshButton = autoSettingValue;

           // await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void ReloadSettings_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

           // SettingsService.CoreSettings.HomeButton = autoSettingValue;

           // await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }
}