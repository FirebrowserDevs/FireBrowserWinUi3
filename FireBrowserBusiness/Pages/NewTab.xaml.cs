using FireBrowserBusinessCore.ImagesBing;
using FireBrowserCore.Models;
using FireBrowserCore.ViewModel;
using FireBrowserExceptions;
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

    Passer param;
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
    private void HomeSync()
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
                try
                {
                    NtpTime.Visibility = NtpDate.Visibility = Visibility.Visible;
                    (NtpTime.Text, NtpDate.Text) = (DateTime.Now.ToString("H:mm"), $"{DateTime.Today.DayOfWeek}, {DateTime.Today.ToString("MMMM d")}");
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(ex);
                    break;
                }

            }
            else
            {
                break;
            }
        }

        if (NtpTime is not null && NtpDate is not null)
        {
            try
            {
                NtpTime.Visibility = NtpDate.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);

            }

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
            SetAndSaveBackgroundSettings(
                tag switch
                {
                    "None" => ("0", Settings.NewTabBackground.None, false, Visibility.Collapsed),
                    "Featured" => ("1", Settings.NewTabBackground.Featured, false, Visibility.Visible),
                    "Custom" => ("2", Settings.NewTabBackground.Costum, true, Visibility.Collapsed),
                    _ => throw new ArgumentException("Invalid selection.")
                });
        }
    }
    private void SetAndSaveBackgroundSettings((string, Settings.NewTabBackground, bool, Visibility) settings)
    {
        var (background, backgroundType, isNewColorEnabled, downloadVisibility) = settings;
        userSettings.Background = background;
        ViewModel.BackgroundType = backgroundType;
        NewColor.IsEnabled = isNewColorEnabled;
        Download.Visibility = downloadVisibility;

        UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
    }
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

                    if (images != null && images.images != null && images.images.Any())
                    {
                        BitmapImage btpImg = new BitmapImage(new Uri("https://bing.com" + images.images[0].url));

                        return new ImageBrush()
                        {
                            ImageSource = btpImg,
                            Stretch = Stretch.UniformToFill
                        };
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle the exception appropriately
                    Console.WriteLine($"Error fetching Bing image: {ex.Message}");
                }
                break;
        }

        return new SolidColorBrush();
    }
    private async Task DownloadImage()
    {
        try
        {
            var user = AuthService.CurrentUser;
            string username = user.Username;
            string databasePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username, "Database");
            string imagesFolderPath = Path.Combine(databasePath, "CacheImages");
            string storedDbPath = Path.Combine(databasePath, "StoredDb.json");

            if (!Directory.Exists(imagesFolderPath))
            {
                Directory.CreateDirectory(imagesFolderPath);
            }

            if (!File.Exists(storedDbPath))
            {
                File.WriteAllText(storedDbPath, "[]");

            }

            Guid gd = Guid.NewGuid();
            string imageName = $"{gd}.png";
            string savedImagePath = await new ImageDownloader().SaveGridAsImageAsync(GridImage, imageName, imagesFolderPath);

            var newImageData = new StoredImages
            {
                Name = imageName,
                Location = imagesFolderPath,
                Extension = ".png",
                Primary = false // Adjust this according to your logic
            };

            await new ImagesHelper().AppendToJsonAsync(storedDbPath, newImageData);
        }
        catch (Exception ex)
        {

        }
    }
    private void UpdateUserSettings(Action<FireBrowserMultiCore.Settings> updateAction)
    {
        if (AuthService.CurrentUser != null)
        {
            updateAction.Invoke(userSettings);
            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
        }
    }
    private void Type_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.Auto = Type.IsOn ? "1" : "0");
    private void Mode_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.LightMode = Mode.IsOn ? "1" : "0");
    private void NewColor_TextChanged(object sender, TextChangedEventArgs e) => UpdateUserSettings(userSettings => userSettings.ColorBackground = NewColor.Text);
    private void DateTimeToggle_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.NtpDateTime = DateTimeToggle.IsOn ? "1" : "0");
    private void NtpColorBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateUserSettings(userSettings => userSettings.NtpTextColor = NtpColorBox.Text);
    private void Download_Click(object sender, RoutedEventArgs e) => DownloadImage();

    private void NewTabSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (isAuto && Application.Current is App app && app.m_window is MainWindow window)
        {
            window.FocusUrlBox(NewTabSearchBox.Text);
        }
    }

    private void NewTabSearchBox_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (!isAuto && e.Key is Windows.System.VirtualKey.Enter && Application.Current is App app && app.m_window is MainWindow window)
        {
            window.FocusUrlBox(NewTabSearchBox.Text);
        }
    }
}