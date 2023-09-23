using FireBrowserBusinessCore.Models;
using FireBrowserCore.Models;
using FireBrowserCore.ViewModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using static FireBrowserBusiness.MainWindow;

namespace FireBrowserBusiness.Pages;
public sealed partial class NewTab : Page
{
    bool isAuto;
    bool isMode;
    public HomeViewModel ViewModel { get; set; }
    public NewTab()
    {
        this.InitializeComponent();
        HomeSync();
    }

    public void HomeSync()
    {
        isAuto = FireBrowserBusinessCore.Helpers.SettingsHelper.GetSetting("Auto") == "1";
        Type.IsOn = isAuto;

        isMode = FireBrowserBusinessCore.Helpers.SettingsHelper.GetSetting("LightMode") == "1";
        Mode.IsOn = isMode;

        var backgroundSetting = FireBrowserBusinessCore.Helpers.SettingsHelper.GetSetting("Background");
        var colorBackgroundSetting = FireBrowserBusinessCore.Helpers.SettingsHelper.GetSetting("ColorBackground");

        // ViewModel setup
        ViewModel = new HomeViewModel
        {
            BackgroundType = GetBackgroundType(backgroundSetting)
        };

        NewColor.IsEnabled = backgroundSetting == "2";
        NewColor.Text = colorBackgroundSetting;
        GridSelect.SelectedValue = ViewModel.BackgroundType.ToString();

        // Visibility setup based on LightMode setting
        SetVisibilityBasedOnLightMode(isMode);
    }

    private Settings.NewTabBackground GetBackgroundType(string setting)
    {
        return setting switch
        {
            "2" => Settings.NewTabBackground.Costum,
            "1" => Settings.NewTabBackground.Featured,
            _ => Settings.NewTabBackground.None
        };
    }

    private void SetVisibilityBasedOnLightMode(bool isLightMode)
    {
        Edit.Visibility = isLightMode ? Visibility.Collapsed : Visibility.Visible;
        SetTab.Visibility = isLightMode ? Visibility.Collapsed : Visibility.Visible;
        BigGrid.Visibility = isLightMode ? Visibility.Collapsed : Visibility.Visible;
    }

    private void GridSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selection = (sender as GridView).SelectedItem as GridViewItem;

        switch (selection.Tag)
        {
            case "None":
                FireBrowserBusinessCore.Helpers.SettingsHelper.SetSetting("Background", "0");
                ViewModel.BackgroundType = Settings.NewTabBackground.None;
                NewColor.IsEnabled = false;
                break;
            case "Featured":
                FireBrowserBusinessCore.Helpers.SettingsHelper.SetSetting("Background", "1");
                ViewModel.BackgroundType = Settings.NewTabBackground.Featured;
                NewColor.IsEnabled = false;
                break;
            case "Custom":
                FireBrowserBusinessCore.Helpers.SettingsHelper.SetSetting("Background", "2");
                ViewModel.BackgroundType = Settings.NewTabBackground.Costum;
                NewColor.IsEnabled = true;
                break;
            default:

                break;
        }
    }

    Passer param;
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        param = e.Parameter as Passer;
    }

    private class ImageRoot
    {
        public ImageTab[] images { get; set; }
    }


    public static Brush GetGridBackgroundAsync(Settings.NewTabBackground backgroundType)
    {
        FireBrowserBusinessCore.Helpers.SettingsHelper.SetSetting("ColorBackground", "#000000");
        string colorString = FireBrowserBusinessCore.Helpers.SettingsHelper.GetSetting("ColorBackground");
   
        switch (backgroundType)
        {
            case Settings.NewTabBackground.None:
                return new SolidColorBrush(Colors.Transparent);

            case Settings.NewTabBackground.Costum:

                if (colorString == "")
                {
                    return new SolidColorBrush(Colors.Transparent);
                }
                else
                {                        
                    var color = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), colorString);

                    return new SolidColorBrush();
                }

            case Settings.NewTabBackground.Featured:

                var client = new HttpClient();
                try
                {
                    var request = client.GetStringAsync(new Uri("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1")).Result;
                    try
                    {
                        var images = System.Text.Json.JsonSerializer.Deserialize<ImageRoot>(request);


                        BitmapImage btpImg = new()
                        {
                            UriSource = new Uri("https://bing.com" + images.images[0].url)
                        };
                        return new ImageBrush()
                        {
                            ImageSource = btpImg,
                            Stretch = Stretch.UniformToFill
                        };
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                catch
                {
                    return new SolidColorBrush(Colors.Transparent);
                }

        }
        return new SolidColorBrush();
    }

    private void Type_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            isAuto = toggleSwitch.IsOn;
            string autoValue = isAuto ? "1" : "0";
            FireBrowserBusinessCore.Helpers.SettingsHelper.SetSetting("Auto", autoValue);
        }
    }

    private void Mode_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string lightModeValue = toggleSwitch.IsOn ? "1" : "0";
            FireBrowserBusinessCore.Helpers.SettingsHelper.SetSetting("LightMode", lightModeValue);
        }
    }

    private void NewColor_TextChanged(object sender, TextChangedEventArgs e)
    {
        FireBrowserBusinessCore.Helpers.SettingsHelper.SetSetting("ColorBackground", $"{NewColor.Text.ToString()}");
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      
    }
}
