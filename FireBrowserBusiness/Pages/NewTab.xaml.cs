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

        var color = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), NtpColor);
        // ViewModel setup
        ViewModel = new HomeViewModel
        {
            BackgroundType = GetBackgroundType(backgroundSetting)
        };

        NewColor.IsEnabled = backgroundSetting == "2";
        NewColor.Text = colorBackgroundSetting;
        NtpColorBox.Text = NtpColor;
        NtpTime.Foreground = NtpDate.Foreground = new SolidColorBrush(color);

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
        while (isNtp)
        {
            await Task.Delay(100); // Introduce a delay before UI update

            // Check if UI elements are disposed or still accessible
            if (NtpTime == null || NtpDate == null)
            {
                break; // Exit the loop if UI elements are disposed
            }

            // Update UI only if the UI elements are available
            NtpTime.Visibility = NtpDate.Visibility = Visibility.Visible;
            NtpTime.Text = DateTime.Now.ToString("H:mm");
            NtpDate.Text = $"{DateTime.Today.DayOfWeek}, {DateTime.Today.ToString("MMMM d")}";
        }

        // Check UI elements again before modifying their visibility
        if (NtpTime != null && NtpDate != null)
        {
            NtpTime.Visibility = NtpDate.Visibility = Visibility.Collapsed;
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
                Download.Visibility = Visibility.Collapsed;
                break;
            case "Featured":
                userSettings.Background = "1";
                ViewModel.BackgroundType = Settings.NewTabBackground.Featured;
                NewColor.IsEnabled = false;
                Download.Visibility = Visibility.Visible;

                break;
            case "Custom":
                userSettings.Background = "2";
                ViewModel.BackgroundType = Settings.NewTabBackground.Costum;
                NewColor.IsEnabled = true;
                Download.Visibility = Visibility.Collapsed;
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
                try
                {
                    var client = new HttpClient();
                    var request = client.GetStringAsync(new Uri("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1")).Result;

                    try
                    {
                        var images = System.Text.Json.JsonSerializer.Deserialize<ImageRoot>(request);
                        BitmapImage btpImg = new BitmapImage(new Uri("https://bing.com" + images.images[0].url));

                        // Use the downloaded image as a background
                        return new ImageBrush()
                        {
                            ImageSource = btpImg,
                            Stretch = Stretch.UniformToFill
                        };
                    }
                    catch
                    {

                        //not for vidoe
                        string storedDbPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Database", "StoredDb.json");
                        string jsonData = File.ReadAllText(storedDbPath);

                        List<StoredImages> storedImages = System.Text.Json.JsonSerializer.Deserialize<List<StoredImages>>(jsonData);

                        StoredImages primaryImage = storedImages.FirstOrDefault(img => img.Primary);
                        if (primaryImage != null && primaryImage.Primary)
                        {
                            string imagesFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Database", "CacheImages");
                            string imagePath = Path.Combine(imagesFolderPath, $"{primaryImage.Name}{primaryImage.Extension}");

                            // Create a BitmapImage from the image path
                            BitmapImage primaryBitmapImage = new BitmapImage(new Uri(imagePath));

                            // Create an ImageBrush with the BitmapImage as the ImageSource
                            ImageBrush imageBrush = new ImageBrush
                            {
                                ImageSource = primaryBitmapImage,
                                Stretch = Stretch.UniformToFill
                            };

                            return imageBrush; // Return the created ImageBrush
                        }
                    }
                }
                catch
                {
                    // Handle exceptions or return a default value
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

    private void Download_Click(object sender, RoutedEventArgs e)
    {
        DownloadImage();
    }
}