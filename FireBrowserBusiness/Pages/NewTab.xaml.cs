using FireBrowserCore.Models;
using FireBrowserCore.ViewModel;
using FireBrowserMultiCore;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static FireBrowserBusiness.MainWindow;
using Settings = FireBrowserBusinessCore.Models.Settings;

namespace FireBrowserBusiness.Pages;
public sealed partial class NewTab : Page
{
    bool isAuto;
    bool isMode;
    bool isNtp;
    public HomeViewModel ViewModel { get; set; }
    public NewTab()
    {
        this.InitializeComponent();
        HomeSync();
        this.Loaded += NewTab_Loaded;
    }

    private void NewTab_Loaded(object sender, RoutedEventArgs e)
    {
        bool isNtp = userSettings.NtpDateTime == "1";
        DateTimeToggle.IsOn = isNtp;
        NtpEnabled(isNtp);
    }

    FireBrowserMultiCore.Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
    public void HomeSync()
    {
        bool isAuto = userSettings.Auto == "1";
        Type.IsOn = isAuto;

        // Update the LightMode setting
        bool isMode = userSettings.LightMode == "1";
        Mode.IsOn = isMode;


        // Get Background and ColorBackground settings
        string backgroundSetting = userSettings.Background;
        string colorBackgroundSetting = userSettings.ColorBackground;
        string NtpColor = userSettings.NtpTextColor;

        // ViewModel setup
        ViewModel = new HomeViewModel
        {
            BackgroundType = GetBackgroundType(backgroundSetting)
        };

        NewColor.IsEnabled = backgroundSetting == "2";
        NewColor.Text = colorBackgroundSetting;
        NtpColorBox.Text = NtpColor;

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

    private async void NtpEnabled(bool isNtp)
    {
        if (isNtp == true)
        {
            while (isNtp == true)
            {

                NtpTime.Visibility = Visibility.Visible;
                NtpDate.Visibility = Visibility.Visible;
                NtpTime.Text = System.DateTime.Now.ToString("H:mm");
                NtpDate.Text = System.DateTime.Today.DayOfWeek.ToString() + ", " + System.DateTime.Today.ToString("MMMM d");
                await Task.Delay(1000);
            }
        }
        else
        {
            NtpTime.Visibility = Visibility.Collapsed;
            NtpDate.Visibility = Visibility.Collapsed;
            return;
        }
    }

    private void SetVisibilityBasedOnLightMode(bool isLightMode)
    {
        NtpGrid.Visibility = isLightMode ? Visibility.Collapsed : Visibility.Visible;
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
                userSettings.Background = "0";
                ViewModel.BackgroundType = Settings.NewTabBackground.None;
                NewColor.IsEnabled = false;

                break;
            case "Featured":
                userSettings.Background = "1";
                ViewModel.BackgroundType = Settings.NewTabBackground.Featured;
                NewColor.IsEnabled = false;


                break;
            case "Custom":
                userSettings.Background = "2";
                ViewModel.BackgroundType = Settings.NewTabBackground.Costum;
                NewColor.IsEnabled = true;

                break;
            default:
                // Handle the case when selection doesn't match any of the predefined options.
                break;
        }

        // Save the modified settings back to the user's settings file
        UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
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


    public static Brush GetGridBackgroundAsync(Settings.NewTabBackground backgroundType, FireBrowserMultiCore.Settings usersettings)
    {
        string colorString = usersettings.ColorBackground.ToString();

        switch (backgroundType)
        {
            case Settings.NewTabBackground.None:

                return new SolidColorBrush(Colors.Transparent);


            case Settings.NewTabBackground.Costum:

                if (colorString == "#000000")
                {
                    return new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    var color = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), colorString);
                    return new SolidColorBrush(color);
                }

            case Settings.NewTabBackground.Featured:
                var client = new HttpClient();

                try
                {
                    var request = client.GetStringAsync(new Uri("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1")).Result;
                    try
                    {
                        var images = System.Text.Json.JsonSerializer.Deserialize<ImageRoot>(request);
                        BitmapImage btpImg = new BitmapImage(new Uri("https://bing.com" + images.images[0].url));

                        // Extract copyright information from the response
                        string copyright = images.images[0].copyright;



                        // Now, you can use the 'copyright' string on your page
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


            if (AuthService.CurrentUser != null)
            {
                // Update the "Auto" setting for the current user
                userSettings.Auto = autoValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
            else
            {
                // Handle the case when there is no authenticated user.
            }
        }

    }

    private void Mode_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            isMode = toggleSwitch.IsOn;
            string autoValue = isMode ? "1" : "0";


            if (AuthService.CurrentUser != null)
            {
                // Update the "Auto" setting for the current user
                userSettings.LightMode = autoValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
            else
            {
                // Handle the case when there is no authenticated user.
            }
        }
    }

    private void NewColor_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Get the current user


        if (AuthService.CurrentUser != null)
        {
            // Update the "ColorBackground" setting for the current user
            userSettings.ColorBackground = NewColor.Text.ToString();

            // Save the modified settings back to the user's settings file
            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
        else
        {
            // Handle the case when there is no authenticated user.
        }

    }

    private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            isNtp = toggleSwitch.IsOn;
            string autoValue = isNtp ? "1" : "0";


            if (AuthService.CurrentUser != null)
            {
                // Update the "Auto" setting for the current user
                userSettings.NtpDateTime = autoValue;

                // Save the modified settings back to the user's settings file
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
            else
            {
                // Handle the case when there is no authenticated user.
            }
        }
    }

    private void NewTabSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (isAuto)
        {
            var window = (Application.Current as App)?.m_window as MainWindow;
            window.FocusUrlBox((NewTabSearchBox.Text));
        }
    }

    private void NewTabSearchBox_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (!isAuto && e.Key is Windows.System.VirtualKey.Enter)
        {
            var window = (Application.Current as App)?.m_window as MainWindow;
            window.FocusUrlBox((NewTabSearchBox.Text));
        }
    }

    private void NtpColorBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (AuthService.CurrentUser != null)
        {
            // Update the "ColorBackground" setting for the current user
            userSettings.NtpTextColor = NtpColorBox.Text.ToString();

            // Save the modified settings back to the user's settings file
            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
        else
        {
            // Handle the case when there is no authenticated user.
        }
    }
}
