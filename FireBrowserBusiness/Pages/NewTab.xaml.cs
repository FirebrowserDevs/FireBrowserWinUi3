using FireBrowserBusinessCore.ImagesBing;
using FireBrowserCore.Models;
using FireBrowserCore.ViewModel;
using FireBrowserDatabase;
using FireBrowserDataCore.Actions;
using FireBrowserExceptions;
using FireBrowserFavorites;
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
    private HistoryActions HistoryActions { get; } = new HistoryActions(AuthService.CurrentUser.Username);
    Passer param;
    public NewTab()
    {
        ViewModel = new HomeViewModel();
        this.InitializeComponent();
        HomeSync();

    }

    private async void NewTab_Loaded(object sender, RoutedEventArgs e)
    {
        //NO need to set propertys here, although we need to referesh data on each load which is as you select the tab it will call the load event may we can refresh the page... 
        ViewModel.HistoryItems = await HistoryActions.GetAllHistoryItems();
        ViewModel.RaisePropertyChanges(nameof(ViewModel.HistoryItems));

        ViewModel.FavoriteItems = ViewModel.LoadFavorites();
        ViewModel.RaisePropertyChanges(nameof(ViewModel.FavoriteItems));

        SearchengineSelection.SelectedItem = userSettings.EngineFriendlyName;
        NewTabSearchBox.Text = string.Empty;
        NewTabSearchBox.Focus(FocusState.Programmatic);
        //bool isNtp = userSettings.NtpDateTime == "1";
        //DateTimeToggle.IsOn = isNtp;
        //NtpEnabled(isNtp);
    }

    FireBrowserMultiCore.Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
    private async void HomeSync()
    {
        Type.IsOn = userSettings.Auto == "1";
        Mode.IsOn = userSettings.LightMode == "1";

        //ViewModel = new HomeViewModel
        //{
        //    BackgroundType = GetBackgroundType(userSettings.Background)
        //};
        ViewModel.BackgroundType = GetBackgroundType(userSettings.Background);
        // set the ntpClock control visibility 
        await ViewModel.Intialize();

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

    //private async void NtpEnabled(bool isNtp)
    //{
    //    while (isNtp)
    //    {
    //        await Task.Delay(100);

    //        if (NtpTime is not null && NtpDate is not null)
    //        {
    //            try
    //            {
    //                NtpTime.Visibility = NtpDate.Visibility = Visibility.Visible;
    //                (NtpTime.Text, NtpDate.Text) = (DateTime.Now.ToString("H:mm"), $"{DateTime.Today.DayOfWeek}, {DateTime.Today.ToString("MMMM d")}");
    //            }
    //            catch (Exception ex)
    //            {
    //                ExceptionLogger.LogException(ex);
    //                break;
    //            }
    //        }
    //        else
    //        {
    //            break;
    //        }
    //    }

    //    if (NtpTime is not null && NtpDate is not null)
    //    {
    //        try
    //        {
    //            NtpTime.Visibility = NtpDate.Visibility = Visibility.Collapsed;
    //        }
    //        catch (Exception ex)
    //        {
    //            ExceptionLogger.LogException(ex);
    //        }
    //    }
    //}


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
            ExceptionLogger.LogException(ex);
        }
    }
    private void UpdateUserSettings(Action<FireBrowserMultiCore.Settings> updateAction)
    {
        if (AuthService.CurrentUser != null)
        {
            updateAction.Invoke(userSettings);
            UserFolderManager.SaveUserSettings(AuthService.CurrentUser, userSettings);
            UpdateNtpClock();
        }
    }

    private void UpdateNtpClock()
    {
        try
        {
            int answer = Int32.Parse(userSettings.NtpDateTime);
            switch (answer)
            {
                case 0:
                    ViewModel.NtpTimeEnabled = false;
                    break;
                case 1:
                    ViewModel.NtpTimeEnabled = true;
                    break;
            }
            ViewModel.Intialize();
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex);
        }

    }

    private void Type_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.Auto = Type.IsOn ? "1" : "0");
    private void Mode_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.LightMode = Mode.IsOn ? "1" : "0");
    private void NewColor_TextChanged(object sender, TextChangedEventArgs e) => UpdateUserSettings(userSettings => userSettings.ColorBackground = NewColor.Text);
    private void DateTimeToggle_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.NtpDateTime = DateTimeToggle.IsOn ? "1" : "0");
    private void FavoritesToggle_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.IsFavoritesToggled = FavoritesTimeToggle.IsOn ? "1" : "0");
    private void HistoryToggle_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.IsHistoryToggled = HistoryToggle.IsOn ? "1" : "0");

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


    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Application.Current is App app && app.m_window is MainWindow window)
        {
            if (e.AddedItems.Count > 0)
                window.NavigateToUrl((e.AddedItems.FirstOrDefault() as HistoryItem).Url);
        }
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
            NewTabSearchBox.Focus(FocusState.Programmatic);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    private void FavoritesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!(sender is ListView listView) || listView.ItemsSource == null) return;

        if (listView.SelectedItem is FavItem item)
        {
            if (Application.Current is App app && app.m_window is MainWindow window)
            {
                if (e.AddedItems.Count > 0)
                    window.NavigateToUrl(item.Url);
            }
        }

    }

}