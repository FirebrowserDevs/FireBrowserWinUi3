using FireBrowserBusinessCore.ImagesBing;
using FireBrowserCore.Models;
using FireBrowserCore.ViewModel;
using FireBrowserMultiCore;
using FireBrowserWinUi3.Controls;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static FireBrowserBusiness.MainWindow;
using Settings = FireBrowserBusinessCore.Models.Settings;

namespace FireBrowserBusiness.Pages;
public sealed partial class NewTab : Page
{
    bool isAuto;
    public HomeViewModel ViewModel { get; set; }
    public NewTab()
    {
        this.InitializeComponent();
        HomeSync();
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
        Type.IsOn = userSettings.Auto == "1";
        Mode.IsOn = userSettings.LightMode == "1";

        ViewModel = new HomeViewModel
        {
            BackgroundType = GetBackgroundType(userSettings.Background)
        };

        var color = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), userSettings.NtpTextColor);
        NewColor.IsEnabled = userSettings.Background == "2";
        NewColor.Text = userSettings.ColorBackground;
        NtpColorBox.Text = userSettings.NtpTextColor;
        NtpTime.Foreground = NtpDate.Foreground = new SolidColorBrush(color);

        GridSelect.SelectedValue = ViewModel.BackgroundType.ToString();
        SetVisibilityBasedOnLightMode(userSettings.LightMode == "1");
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
        while (isNtp)
        {
            await Task.Delay(100);

            if (NtpTime is not null && NtpDate is not null)
            {
                NtpTime.Visibility = NtpDate.Visibility = Visibility.Visible;
                (NtpTime.Text, NtpDate.Text) = (DateTime.Now.ToString("H:mm"), $"{DateTime.Today.DayOfWeek}, {DateTime.Today.ToString("MMMM d")}");
            }
            else
            {
                break;
            }
        }

        if (NtpTime is not null && NtpDate is not null)
        {
            NtpTime.Visibility = NtpDate.Visibility = Visibility.Collapsed;
        }
    }

    private void SetVisibilityBasedOnLightMode(bool isLightMode)
    {
        var visibility = isLightMode ? Visibility.Collapsed : Visibility.Visible;

        NtpGrid.Visibility = Edit.Visibility = SetTab.Visibility = BigGrid.Visibility = visibility;
    }

    private void GridSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selection = (sender as GridView)?.SelectedItem as GridViewItem;

        if (selection != null && selection.Tag is string tag)
        {
            switch (tag)
            {
                case "None":
                    SetAndSaveBackgroundSettings("0", Settings.NewTabBackground.None, false, Visibility.Collapsed);
                    break;
                case "Featured":
                    SetAndSaveBackgroundSettings("1", Settings.NewTabBackground.Featured, false, Visibility.Visible);
                    break;
                case "Custom":
                    SetAndSaveBackgroundSettings("2", Settings.NewTabBackground.Costum, true, Visibility.Collapsed);
                    break;
                default:
                    // Handle the case when selection doesn't match any of the predefined options.
                    break;
            }
        }
    }

    private void SetAndSaveBackgroundSettings(string background, Settings.NewTabBackground backgroundType, bool isNewColorEnabled, Visibility downloadVisibility)
    {
        userSettings.Background = background;
        ViewModel.BackgroundType = backgroundType;
        NewColor.IsEnabled = isNewColorEnabled;
        Download.Visibility = downloadVisibility;

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



    public static Brush GetGridBackgroundAsync(Settings.NewTabBackground backgroundType, FireBrowserMultiCore.Settings userSettings)
    {
        string colorString = userSettings.ColorBackground.ToString();
        var client = new HttpClient();

        switch (backgroundType)
        {
            case Settings.NewTabBackground.None:
                return new SolidColorBrush(Colors.Transparent);

            case Settings.NewTabBackground.Costum:
                var color = colorString == "#000000" ?
                                Colors.Transparent :
                                (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), colorString);
                return new SolidColorBrush(color);

            case Settings.NewTabBackground.Featured:
                try
                {
                    var images = System.Text.Json.JsonSerializer.Deserialize<ImageRoot>(client.GetStringAsync(new Uri("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1")).Result);
                    BitmapImage btpImg = new BitmapImage(new Uri("https://bing.com" + images.images[0].url));

                    return new ImageBrush()
                    {
                        ImageSource = btpImg,
                        Stretch = Stretch.UniformToFill
                    };
                }
                catch
                {
                    string storedDbPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Database", "StoredDb.json");
                    string jsonData = File.ReadAllText(storedDbPath);

                    List<StoredImages> storedImages = System.Text.Json.JsonSerializer.Deserialize<List<StoredImages>>(jsonData);

                    StoredImages primaryImage = storedImages.FirstOrDefault(img => img.Primary);
                    if (primaryImage != null && primaryImage.Primary)
                    {
                        string imagesFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Database", "CacheImages");
                        string imagePath = Path.Combine(imagesFolderPath, $"{primaryImage.Name}{primaryImage.Extension}");

                        BitmapImage primaryBitmapImage = new BitmapImage(new Uri(imagePath));

                        ImageBrush imageBrush = new ImageBrush
                        {
                            ImageSource = primaryBitmapImage,
                            Stretch = Stretch.UniformToFill
                        };

                        return imageBrush;
                    }
                }
                break;
        }

        return new SolidColorBrush();
    }




    private async Task DownloadImage()
    {
        try
        {
            FireBrowserMultiCore.User user = AuthService.CurrentUser;
            string username = user.Username;
            string databasePath = Path.Combine(
                UserDataManager.CoreFolderPath,
                UserDataManager.UsersFolderPath,
                username,
                "Database"
            );
            string imagesFolderPath = Path.Combine(databasePath, "CacheImages");
            string storedDbPath = Path.Combine(databasePath, "StoredDb.json");

            // Create the "CacheImages" folder if it doesn't exist
            if (!Directory.Exists(imagesFolderPath))
            {
                Directory.CreateDirectory(imagesFolderPath);
                ShowInfoBar("CacheImages folder created.", InfoBarSeverity.Success);
            }

            // Create the "StoredDb.json" file with default data if it doesn't exist
            if (!File.Exists(storedDbPath))
            {
                // Create an empty JSON file if it doesn't exist
                File.WriteAllText(storedDbPath, "[]");
                ShowInfoBar("Created empty StoredDb.json.", InfoBarSeverity.Success);
            }

            // Download the image using the specified details
            Guid gd = Guid.NewGuid();
            string imageName = $"{gd}.png"; // Set the image name
            ImageDownloader imageDownloader = new ImageDownloader();

            // Save the image using custom folder path
            string customFolderPath = imagesFolderPath; // Use the images folder path as custom folder path
            string savedImagePath = await imageDownloader.SaveGridAsImageAsync(GridImage, imageName, customFolderPath);

            // Append data to the JSON after saving the image
            StoredImages newImageData = new StoredImages
            {
                Name = imageName,
                Location = customFolderPath,
                Extension = ".png",
                Primary = false // Adjust this according to your logic
            };

            // Append new data to the JSON file
            ImagesHelper hp = new ImagesHelper();
            await hp.AppendToJsonAsync(storedDbPath, newImageData);

            ShowInfoBar($"Downloaded Bing Image To {imagesFolderPath} Success!", InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            ShowInfoBar($"Error downloading image: {ex.Message}", InfoBarSeverity.Error);
            // Handle the exception as needed: log, display to the user, etc.
        }
    }


    private async void ShowInfoBar(string message, InfoBarSeverity severity)
    {
        infoBar.IsOpen = true;
        infoBar.Message = message;
        infoBar.Severity = severity;

        await Task.Delay(TimeSpan.FromSeconds(1.5));

        infoBar.IsOpen = false;
    }

    private void Type_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoValue = toggleSwitch.IsOn ? "1" : "0";
            string autoColor = AuthService.CurrentUser != null ? "0" : autoValue;

            if (AuthService.CurrentUser != null)
            {
                userSettings.Auto = autoValue;
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }
    }

    private void Mode_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoValue = toggleSwitch.IsOn ? "1" : "0";
            string lightModeColor = AuthService.CurrentUser != null ? "0" : autoValue;

            if (AuthService.CurrentUser != null)
            {
                userSettings.LightMode = autoValue;
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
        }
    }

    private void NewColor_TextChanged(object sender, TextChangedEventArgs e)
    {
        string colorValue = AuthService.CurrentUser != null ? NewColor.Text : "#ffffff";

        if (AuthService.CurrentUser != null)
        {
            userSettings.ColorBackground = colorValue;
            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }

    private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoValue = toggleSwitch.IsOn ? "1" : "0";

            if (AuthService.CurrentUser != null)
            {
                userSettings.NtpDateTime = autoValue;
                UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            }
            else
            {
                autoValue = "0";
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
            userSettings.NtpTextColor = NtpColorBox.Text;
            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
        else
        {
            NtpColorBox.Text = "#ffffff";
        }
    }

    private void Download_Click(object sender, RoutedEventArgs e)
    {
        DownloadImage();
    }
}