using FireBrowserDatabase;
using FireBrowserWinUi3.Controls;
using FireBrowserWinUi3.Services;
using FireBrowserWinUi3.ViewModels;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3Core.ImagesBing;
using FireBrowserWinUi3DataCore.Actions;
using FireBrowserWinUi3Exceptions;
using FireBrowserWinUi3Favorites;
using FireBrowserWinUi3MultiCore;
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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using Windows.UI;
using static FireBrowserWinUi3.MainWindow;
using Settings = FireBrowserWinUi3Core.Models.Settings;

namespace FireBrowserWinUi3.Pages;

public sealed partial class NewTab : Page
{
    bool isAuto;

    public List<TrendingItem> trendings = new List<TrendingItem>();
    public HomeViewModel ViewModel { get; set; }
    private HistoryActions HistoryActions { get; } = new HistoryActions(AuthService.CurrentUser.Username);
    FireBrowserWinUi3MultiCore.Settings userSettings { get; set; }
    SettingsService SettingsService { get; }

    Passer param;
    public NewTab()
    {
        ViewModel = App.GetService<HomeViewModel>();
        // init to load controls from settings, and start clock . 
        _ = ViewModel.Intialize().GetAwaiter();
        // assign to ViewModel, and or new instance.  
        ViewModel.SettingsService.Initialize();
        userSettings = ViewModel.SettingsService.CoreSettings;

        this.InitializeComponent();

        if (userSettings.IsTrendingVisible) _ = UpdateTrending().ConfigureAwait(false);
    }
    public class TrendingItem
    {
        public string webSearchUrl { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string text { get; set; }
        public TrendingItem() { }
        public TrendingItem(string _webSearchUrl, string _name, string _url, string _text)
        {
            webSearchUrl = _webSearchUrl;
            name = _name;
            url = _url;
            text = _text;
        }

    }
    private async Task UpdateTrending()
    {
        var bing = new BingSearchApi();
        var topics = bing.TrendingListTask("calico cats").GetAwaiter().GetResult();
        // fixed treding errors. 
        if (topics is not null)
        {
            var list = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>(topics).ToList();

            trendings.Clear(); ;

            foreach (var item in list)
            {

                trendings.Add(new TrendingItem(item["webSearchUrl"].ToString(), item["name"].ToString(), item["image"]["url"].ToString(), item["query"]["text"].ToString()));
            }
        }
        else
        {
            await Task.Delay(1000);
            await UpdateTrending();
        }

    }
    public record TrendingListItem(string webSearchUrl, string name, string url, string text);
    private async void NewTab_Loaded(object sender, RoutedEventArgs e)
    {
        // round-robin if one or more newTab's are open apply settings. 
        await ViewModel.Intialize();
        userSettings = ViewModel.SettingsService.CoreSettings;

        //NO need to load because property is attached to viewModel, and also if you select the tab it will call the load event may we can refresh the page... 
        ViewModel.HistoryItems = await HistoryActions.GetAllHistoryItems();
        ViewModel.RaisePropertyChanges(nameof(ViewModel.HistoryItems));

        ViewModel.FavoriteItems = ViewModel.LoadFavorites();
        ViewModel.RaisePropertyChanges(nameof(ViewModel.FavoriteItems));

        SearchengineSelection.SelectedItem = userSettings.EngineFriendlyName;
        NewTabSearchBox.Text = string.Empty;
        NewTabSearchBox.Focus(FocusState.Programmatic);

        if (userSettings.IsTrendingVisible) await UpdateTrending();

        HomeSync();
    }


    private async void HomeSync()
    {
        Type.IsOn = userSettings.Auto is true;
        Mode.IsOn = userSettings.LightMode is true;

        ViewModel.BackgroundType = GetBackgroundType(userSettings.Background);
        ViewModel.RaisePropertyChanges(nameof(ViewModel.BackgroundType));

        //var color = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), userSettings.NtpTextColor);
        NewColor.IsEnabled = userSettings.Background is 2;
        // If userSettings.ColorBackground is null, default to a specific color (e.g., Windows.UI.Colors.White)
        // Use Microsoft.UI.Colors instead of Windows.UI.Colors
        NewColorPicker.Color = (Color)XamlBindingHelper.ConvertValue(
       typeof(Color),
       string.IsNullOrWhiteSpace(userSettings.ColorBackground)
           ? Microsoft.UI.Colors.White // Default color
           : userSettings.ColorBackground
         );

        // Convert the NtpTextColor setting from a string (if not null) or use a default color
        NtpColorPicker.Color = (Color)XamlBindingHelper.ConvertValue(
            typeof(Color),
            string.IsNullOrWhiteSpace(userSettings.NtpTextColor)
                ? Microsoft.UI.Colors.Black // Default color
                : userSettings.NtpTextColor
        );

        //NtpTime.Foreground = NtpDate.Foreground = new SolidColorBrush(color);
        GridSelect.SelectedIndex = userSettings.Background;
        SetVisibilityBasedOnLightMode(userSettings.LightMode is true);
        await Task.CompletedTask;
    }

    private Settings.NewTabBackground GetBackgroundType(int setting)
    {
        return setting switch
        {
            2 => Settings.NewTabBackground.Costum,
            1 => Settings.NewTabBackground.Featured,
            _ => Settings.NewTabBackground.None
        };
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
                    "None" => (0, Settings.NewTabBackground.None, false, Visibility.Collapsed),
                    "Featured" => (1, Settings.NewTabBackground.Featured, false, Visibility.Visible),
                    "Custom" => (2, Settings.NewTabBackground.Costum, true, Visibility.Collapsed),
                    _ => throw new ArgumentException("Invalid selection.")
                });
        }
    }
    private async void SetAndSaveBackgroundSettings((int, Settings.NewTabBackground, bool, Visibility) settings)
    {
        var (background, backgroundType, isNewColorEnabled, downloadVisibility) = settings;
        userSettings.Background = background;
        ViewModel.BackgroundType = backgroundType;
        NewColor.IsEnabled = isNewColorEnabled;
        Download.Visibility = downloadVisibility;
        await ViewModel.SettingsService?.SaveChangesToSettings(AuthService.CurrentUser, userSettings);
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        param = e.Parameter as Passer;
    }

    public static Brush GetGridBackgroundAsync(Settings.NewTabBackground backgroundType, FireBrowserWinUi3MultiCore.Settings userSettings)
    {
        switch (backgroundType)
        {
            case Settings.NewTabBackground.None:
                return new SolidColorBrush(Colors.Transparent);

            case Settings.NewTabBackground.Costum:
                string colorString = userSettings.ColorBackground?.ToString() ?? "#000000";

                var color = colorString == "#000000" ?
                 Colors.Transparent :
                 (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), colorString);
                return new SolidColorBrush(color);


            case Settings.NewTabBackground.Featured:
                var handler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true, // Optional but recommended
                    SslOptions = new SslClientAuthenticationOptions
                    {
                        ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 }
                    }
                };

                var client = new HttpClient(handler);

                // Static field to cache the ImageBrush so it's fetched only once
                ImageBrush cachedImageBrush = null;

                // Check if the image has already been fetched and cached
                if (cachedImageBrush != null)
                {
                    return cachedImageBrush;
                }

                try
                {
                    var request = client.GetStringAsync(new Uri("https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1")).Result;

                    try
                    {
                        // Deserialize JSON response into ImageRoot object
                        var images = System.Text.Json.JsonSerializer.Deserialize<ImageRoot>(request);

                        // Construct the image URL using data from the API
                        Uri imageUrl = new Uri("https://bing.com" + images.images[0].url);

                        // Create BitmapImage from the URL
                        BitmapImage btpImg = new BitmapImage(imageUrl);

                        // Create and return an ImageBrush for WPF
                        cachedImageBrush = new ImageBrush()
                        {
                            ImageSource = btpImg,
                            Stretch = Stretch.UniformToFill
                        };

                        return cachedImageBrush;
                    }
                    catch (System.Text.Json.JsonException jsonEx)
                    {
                        // Handle JSON parsing errors
                        Console.WriteLine($"Error parsing JSON: {jsonEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        // Handle other exceptions
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    // Handle HTTP request exceptions
                    Console.WriteLine($"HTTP request error: {httpEx.Message}");
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    Console.WriteLine($"Error: {ex.Message}");
                }
                break;


        }

        return new SolidColorBrush();
    }

    private class ImageRoot
    {
        public Image[] images { get; set; }
    }
    private class Image
    {
        public string url { get; set; }
        public string copyright { get; set; }
        public string copyrightlink { get; set; }
        public string title { get; set; }
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
    private void UpdateUserSettings(Action<FireBrowserWinUi3MultiCore.Settings> updateAction)
    {
        if (AuthService.CurrentUser != null)
        {
            updateAction.Invoke(userSettings);
            ViewModel.SettingsService.CoreSettings = userSettings;
            UpdateNtpClock();
        }
    }

    private void UpdateNtpClock()
    {
        try
        {
            ViewModel.NtpTimeEnabled = userSettings.NtpDateTime;
            ViewModel.Intialize();
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex);
        }
    }

    private void Type_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.Auto = Type.IsOn);
    private void Mode_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.LightMode = Mode.IsOn);
    private void NewColor_TextChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        var newColor = userSettings.ColorBackground = XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), NewColorPicker.Color).ToString();
        UpdateUserSettings(userSettings => userSettings.ColorBackground = newColor);
        // raise a change to backgroundtype so that the x:Bind on GridMain will show new backgroundColor {x:Bind local:NewTab.GetGridBackgroundAsync(ViewModel.BackgroundType, userSettings), Mode=OneWay}
        ViewModel.RaisePropertyChanges(nameof(ViewModel.BackgroundType));
    }
    private void DateTimeToggle_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.NtpDateTime = DateTimeToggle.IsOn);
    private void FavoritesToggle_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.IsFavoritesToggled = FavoritesTimeToggle.IsOn);
    private void HistoryToggle_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.IsHistoryToggled = HistoryToggle.IsOn);
    private void SearchVisible_Toggled(Object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.IsSearchVisible = SearchVisible.IsOn);
    private void FavsVisible_Toggled(Object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.IsFavoritesVisible = FavsVisible.IsOn);
    private void HistoryVisible_Toggled(Object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.IsHistoryVisible = HistoryVisible.IsOn);
    private void NtpColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        var newColor = userSettings.NtpTextColor = XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), NtpColorPicker.Color).ToString();
        UpdateUserSettings(userSettings => userSettings.NtpTextColor = newColor);
        ViewModel.BrushNtp = new SolidColorBrush(NtpColorPicker.Color);
    }
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
    private async void SearchengineSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.AddedItems.Count == 0) return;

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
                case "Swisscows":
                    url = "https://swisscows.com/web?query=";
                    break;
                case "Dogpile":
                    url = "https://www.dogpile.com/serp?q=";
                    break;
                case "Webcrawler":
                    url = "https://www.webcrawler.com/serp?q=";
                    break;
                case "You":
                    url = "https://you.com/search?q=";
                    break;
                case "Excite":
                    url = "https://results.excite.com/serp?q=";
                    break;
                case "Lycos":
                    url = "https://search20.lycos.com/web/?q=";
                    break;
                case "Metacrawler":
                    url = "https://www.metacrawler.com/serp?q=";
                    break;
                case "Mojeek":
                    url = "https://www.mojeek.com/search?q=";
                    break;
                case "BraveSearch":
                    url = "https://search.brave.com/search?q=";
                    break;
                default:
                    url = "https://www.google.com/search?q=";
                    break;
            }

            if (!string.IsNullOrEmpty(url))
            {
                userSettings.EngineFriendlyName = selection;
                userSettings.SearchUrl = url;
                await ViewModel.SettingsService?.SaveChangesToSettings(AuthService.CurrentUser, userSettings);
            }
            NewTabSearchBox.Focus(FocusState.Programmatic);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }


    private void TrendingSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Application.Current is App app && app.m_window is MainWindow window)
        {
            if (e.AddedItems.Count > 0)
            {
                ViewModel.TrendingItem = (e.AddedItems.FirstOrDefault() as TrendingItem);
                window.NavigateToUrl(ViewModel.TrendingItem.webSearchUrl);

            }
        }
    }
    private void TrendingVisible_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.IsTrendingVisible = TrendingVisible.IsOn);
    private void FloatingBox_Toggled(object sender, RoutedEventArgs e) => UpdateUserSettings(userSettings => userSettings.IsLogoVisible = FloatingBox.IsOn);

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