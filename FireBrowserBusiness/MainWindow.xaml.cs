using FireBrowserBusiness.Controls;
using FireBrowserBusiness.Pages;
using FireBrowserBusinessCore.Helpers;
using FireBrowserBusinessCore.Models;
using FireBrowserBusinessCore.ViewModel;
using FireBrowserDatabase;
using FireBrowserFavorites;
using FireBrowserMultiCore;
using FireBrowserQr;
using FireBrowserWinUi3.Controls;
using FireBrowserWinUi3.Pages;
using FireBrowserWinUi3Core.CoreUi;
using Microsoft.Data.Sqlite;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UrlHelperWinUi3;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;
using Settings = FireBrowserMultiCore.Settings;
using Windowing = FireBrowserBusinessCore.Helpers.Windowing;

namespace FireBrowserBusiness;
public sealed partial class MainWindow : Window
{
    private AppWindow appWindow;
    private AppWindowTitleBar titleBar;

    public DownloadFlyout DownloadFlyout { get; set; } = new DownloadFlyout();

    public MainWindow()
    {
        InitializeComponent();

        ArgsPassed();
        TitleTop();
        LoadUserDataAndSettings(); // Load data and settings for the new user
        LoadUserSettings();
        Init();

        appWindow.Closing += AppWindow_Closing;
    }

    public async void Init()
    {
        await FireBrowserBusinessCore.Models.Data.Init();
        FireBrowserSecureConnect.TwoFactorsAuthentification.Init();
    }

    private async void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (Tabs.TabItems?.Count > 1)
        {
            args.Cancel = true;

            var currentApp = Application.Current as App;
            var mainWindow = currentApp?.m_window as MainWindow;

            if (mainWindow != null)
            {
                ConfirmAppClose quickConfigurationDialog = new()
                {
                    XamlRoot = mainWindow.Content.XamlRoot
                };

                quickConfigurationDialog.PrimaryButtonClick += async (sender, e) =>
                {
                    // Close the application when the primary button is clicked
                    Application.Current.Exit();
                };

                await quickConfigurationDialog.ShowAsync();
            }
        }
        else
        {
            args.Cancel = false;
        }
    }

    bool incog = false;
    private async void ArgsPassed()
    {
        if (!string.IsNullOrEmpty(AppArguments.UrlArgument) &&
            Uri.TryCreate(AppArguments.UrlArgument, UriKind.Absolute, out Uri uri))
        {
            Tabs.TabItems.Add(CreateNewTab(typeof(WebContent), uri));
            return;
        }

        if (!string.IsNullOrEmpty(AppArguments.FireBrowserArgument) ||
            !string.IsNullOrEmpty(AppArguments.FireUser))
        {
            Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));
            return;
        }

        if (!string.IsNullOrEmpty(AppArguments.FireBrowserPdf))
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(AppArguments.FireBrowserPdf);
            var files = new List<IStorageItem> { file }.AsReadOnly();
            if (files.Count > 0)
            {
                Tabs.TabItems.Add(CreateNewTab(typeof(WebContent), files[0]));
            }
            return;
        }

        if (!string.IsNullOrEmpty(AppArguments.FireBrowserIncog))
        {
            Tabs.TabItems.Add(CreateNewIncog(typeof(InPrivate)));
            Fav.IsEnabled = false;
            His.IsEnabled = false;
            History.IsEnabled = false;
            NewTab.Visibility = Visibility.Collapsed;
            Down.IsEnabled = false;
            DownBtn.IsEnabled = false;
            FavoritesButton.IsEnabled = false;
            UserFrame.IsEnabled = false;
            NewWindow.Visibility = Visibility.Collapsed;
            WebContent.IsIncognitoModeEnabled = true;
            AuthService.DeleteUser("Private");
            InPrivateUser();
            incog = true;
            return;
        }

        Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));
    }


    private void InPrivateUser()
    {
        User newUser = new User
        {
            Id = Guid.NewGuid(), // Generate a new GUID for the user Id
            Username = "Private",
            IsFirstLaunch = true,
            UserSettings = null // You might want to initialize UserSettings based on your application logic
        };

        // Add the new user to your user collection or perform any other necessary logic
        // For demonstration purposes, let's assume 'users' is a List<User> in your AuthService
        AuthService.AddUser(newUser);
        UserFolderManager.CreateUserFolders(newUser);
        AuthService.CurrentUser.Username = newUser.Username;
        AuthService.Authenticate(newUser.Username);
    }

    private void LoadUsernames()
    {
        List<string> usernames = AuthService.GetAllUsernames();
        string currentUsername = AuthService.CurrentUser?.Username;

        foreach (string username in usernames.Where(username => username != currentUsername && !username.Contains("Private")))
        {
            UserListView.Items.Add(username);
        }
    }

    public void SmallUpdates()
    {
        string source = TabWebView.CoreWebView2.Source?.ToString() ?? string.Empty;
        UrlBox.Text = source;
        ViewModel.Securitytype = source;

        ViewModel.SecurityIcon = source.Contains("https") ? "\uE72E" :
                                  source.Contains("http") ? "\uE785" : "";

        ViewModel.SecurityIcontext = source.Contains("https") ? "Https Secured Website" :
                                     source.Contains("http") ? "Http UnSecured Website" : "";

        ViewModel.Securitytext = source.Contains("https")
            ? "This Page Is Secured By A Valid SSL Certificate, Trusted By Root Authorities"
            : source.Contains("http")
                ? "This Page Is Unsecured By A Non-Valid SSL Certificate, Please Be Careful"
                : "";
    }

    public void TitleTop()
    {
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        Microsoft.UI.WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        appWindow = AppWindow.GetFromWindowId(windowId);
        appWindow.SetIcon("Logo.ico");

        if (!AppWindowTitleBar.IsCustomizationSupported())
        {
            throw new Exception("Unsupported OS version.");
        }

        titleBar = appWindow.TitleBar;
        titleBar.ExtendsContentIntoTitleBar = true;
        var btnColor = Colors.Transparent;
        titleBar.BackgroundColor = titleBar.ButtonBackgroundColor = titleBar.InactiveBackgroundColor = titleBar.ButtonInactiveBackgroundColor = titleBar.ButtonHoverBackgroundColor = btnColor;

        ViewModel = new ToolbarViewModel
        {
            CurrentAddress = "",
            SecurityIcon = "\uE946",
            SecurityIcontext = "FireBrowser NewTab",
            Securitytext = "This The Default Home Page Of FireBrowser Internal Pages Secure",
            Securitytype = "Link - FireBrowser://NewTab"
        };
    }

    public static string launchurl { get; set; }
    public static string SearchUrl { get; set; }

    public bool isFull = false;

    public void GoFullScreenWeb(bool fullscreen)
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        Microsoft.UI.WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var view = AppWindow.GetFromWindowId(wndId);
        var margin = fullscreen ? new Thickness(0, -40, 0, 0) : new Thickness(0, 35, 0, 0);

        view.SetPresenter(fullscreen ? AppWindowPresenterKind.FullScreen : AppWindowPresenterKind.Default);

        ClassicToolbar.Height = fullscreen ? 0 : 40;

        TabContent.Margin = margin;
    }

    public void GoFullScreen(bool fullscreen)
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        Microsoft.UI.WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var view = AppWindow.GetFromWindowId(wndId);

        view.SetPresenter(fullscreen ? AppWindowPresenterKind.FullScreen : AppWindowPresenterKind.Default);
        isFull = fullscreen;
        TextFull.Text = fullscreen ? "Exit FullScreen" : "Full Screen";
    }

    private void LoadUserSettings()
    {
        LoadUsernames();
        UpdateUIBasedOnSettings();
    }

    private void LoadUserDataAndSettings()
    {
        FireBrowserMultiCore.User currentUser = AuthService.IsUserAuthenticated ? AuthService.CurrentUser : null;

        if (currentUser == null)
        {
            UserName.Text = Prof.Text = "DefaultUser";
            return;
        }

        if (!AuthService.IsUserAuthenticated && !AuthService.Authenticate(currentUser.Username))
        {
            return;
        }

        UserName.Text = Prof.Text = AuthService.CurrentUser?.Username ?? "DefaultUser";
    }

    private void UpdateUIBasedOnSettings()
    {
        Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);

        SetVisibility(AdBlock, userSettings.AdblockBtn != "0");
        SetVisibility(ReadBtn, userSettings.ReadButton != "0");
        SetVisibility(BtnTrans, userSettings.Translate != "0");
        SetVisibility(BtnDark, userSettings.DarkIcon != "0");
        SetVisibility(ToolBoxMore, userSettings.ToolIcon != "0");
        SetVisibility(AddFav, userSettings.FavoritesL != "0");
        SetVisibility(FavoritesButton, userSettings.Favorites != "0");
        SetVisibility(DownBtn, userSettings.Downloads != "0");
        SetVisibility(History, userSettings.Historybtn != "0");
        SetVisibility(QrBtn, userSettings.QrCode != "0");
    }

    private void SetVisibility(UIElement element, bool isVisible)
    {
        element.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    private int maxTabItems = 20;
    private void TabView_AddTabButtonClick(TabView sender, object args)
    {
        if (sender.TabItems.Count < maxTabItems)
        {
            sender.TabItems.Add(incog == true ? CreateNewIncog(typeof(InPrivate)) : CreateNewTab(typeof(NewTab)));
        }
    }

    #region toolbar

    public ToolbarViewModel ViewModel { get; set; }
    public class Passer
    {
        public FireBrowserTabViewItem Tab { get; set; }
        public FireBrowserTabViewContainer TabView { get; set; }
        public object Param { get; set; }
        public ToolbarViewModel ViewModel { get; set; }
    }

    #endregion

    public FireBrowserTabViewItem CreateNewTab(Type page = null, object param = null, int index = -1)
    {
        index = Tabs.TabItems.Count;

        var newItem = new FireBrowserTabViewItem
        {
            Header = "NewTab",
            IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource { Symbol = Symbol.Home },
            Style = (Style)Microsoft.UI.Xaml.Application.Current.Resources["FloatingTabViewItemStyle"]
        };

        Passer passer = new()
        {
            Tab = newItem,
            TabView = Tabs,
            ViewModel = new ToolbarViewModel(),
            Param = param,
        };

        passer.ViewModel.CurrentAddress = "";

        double margin = ClassicToolbar.Height;
        var frame = new Frame
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0, margin, 0, 0)
        };

        if (page != null)
        {
            frame.Navigate(page, passer);
        }

        var toolTip = new ToolTip
        {
            Content = new Grid
            {
                Children =
            {
                new Microsoft.UI.Xaml.Controls.Image(),
                new TextBlock()
            }
            }
        };
        ToolTipService.SetToolTip(newItem, toolTip);

        newItem.Content = frame;
        return newItem;
    }

    public Frame TabContent => (Tabs.SelectedItem as FireBrowserTabViewItem)?.Content as Frame;

    public WebView2 TabWebView => (TabContent?.Content as WebContent)?.WebViewElement;

    private double GetScaleAdjustment()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        Microsoft.UI.WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
        IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

        _ = Windowing.GetDpiForMonitor(hMonitor, Windowing.Monitor_DPI_Type.MDT_Default_DPI, out uint dpiX, out _);

        return dpiX / 96.0; // Simplified calculation
    }

    private void Tabs_Loaded(object sender, RoutedEventArgs e)
    {
        Apptitlebar.SizeChanged += Apptitlebar_SizeChanged;
    }

    private void Apptitlebar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        double scaleAdjustment = GetScaleAdjustment();
        Apptitlebar.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
        var customDragRegionPosition = Apptitlebar.TransformToVisual(null).TransformPoint(new Windows.Foundation.Point(0, 0));

        var dragRects = new Windows.Graphics.RectInt32[2];

        for (int i = 0; i < 2; i++)
        {
            dragRects[i] = new Windows.Graphics.RectInt32
            {
                X = (int)((customDragRegionPosition.X + (i * Apptitlebar.ActualWidth / 2)) * scaleAdjustment),
                Y = (int)(customDragRegionPosition.Y * scaleAdjustment),
                Height = (int)((Apptitlebar.ActualHeight - customDragRegionPosition.Y) * scaleAdjustment),
                Width = (int)((Apptitlebar.ActualWidth / 2) * scaleAdjustment)
            };
        }

        appWindow.TitleBar?.SetDragRectangles(dragRects);
    }

    private void Apptitlebar_LayoutUpdated(object sender, object e)
    {
        double scaleAdjustment = GetScaleAdjustment();
        Apptitlebar.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
        var customDragRegionPosition = Apptitlebar.TransformToVisual(null).TransformPoint(new Windows.Foundation.Point(0, 0));

        var dragRectsList = new List<Windows.Graphics.RectInt32>();

        for (int i = 0; i < 2; i++)
        {
            var dragRect = new Windows.Graphics.RectInt32
            {
                X = (int)((customDragRegionPosition.X + (i * Apptitlebar.ActualWidth / 2)) * scaleAdjustment),
                Y = (int)(customDragRegionPosition.Y * scaleAdjustment),
                Height = (int)((Apptitlebar.ActualHeight - customDragRegionPosition.Y) * scaleAdjustment),
                Width = (int)((Apptitlebar.ActualWidth / 2) * scaleAdjustment)
            };

            dragRectsList.Add(dragRect);
        }

        var dragRects = dragRectsList.ToArray();

        appWindow.TitleBar?.SetDragRectangles(dragRects);
    }


    private void Tabs_TabItemsChanged(TabView sender, IVectorChangedEventArgs args)
    {
        if (sender.TabItems.Count <= 0)
        {
            Application.Current.Exit();
        }
        else
        {
            sender.CanReorderTabs = sender.CanDragTabs = sender.TabItems.Count > 1;
        }
    }

    public Passer CreatePasser(object parameter = null)
    {
        return new()
        {
            Tab = Tabs.SelectedItem as FireBrowserTabViewItem,
            TabView = Tabs,
            ViewModel = ViewModel,
            Param = parameter,
        };
    }

    public void SelectNewTab()
    {
        Tabs.SelectedIndex = Tabs.TabItems.Count - 1;
    }

    public void FocusUrlBox(string text)
    {
        UrlBox.Text = text;
        UrlBox.Focus(FocusState.Programmatic);
    }

    public void NavigateToUrl(string uri)
    {
        if (TabContent.Content is not WebContent webContent)
        {
            launchurl ??= uri;
            TabContent.Navigate(typeof(WebContent), CreatePasser(uri));
            return;
        }

        webContent.WebViewElement.CoreWebView2.Navigate(uri.ToString());
    }

    private void UrlBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        string input = UrlBox.Text.ToString();
        string inputtype = UrlHelper.GetInputType(input);

        try
        {
            if (input.Contains("firebrowser://"))
            {
                switch (input)
                {
                    case "firebrowser://newtab":
                        Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));
                        SelectNewTab();
                        break;
                    case "firebrowser://settings":
                        Tabs.TabItems.Add(CreateNewTab(typeof(SettingsPage)));
                        SelectNewTab();
                        break;
                    default:
                        // default behavior
                        break;
                }
            }
            else if (inputtype is "url" or "urlNOProtocol")
            {
                string url = inputtype == "url" ? input.Trim() : "https://" + input.Trim();
                NavigateToUrl(url);
            }
            else
            {
                Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
                string searchurl = SearchUrl ?? $"{userSettings.SearchUrl}";
                string query = searchurl + input;
                NavigateToUrl(query);
            }
        }
        catch (Exception ex)
        {
            // Handle the exception, log it, or display an error message.
            Debug.WriteLine("Error during navigation: " + ex.Message);
        }
    }

    private async void ShowDisabledDialog()
    {
        var window = (Application.Current as App)?.m_window as MainWindow;
        UI quickConfigurationDialog = new()
        {
            XamlRoot = window.Content.XamlRoot,
            Content = "Disabled Until Fixed",
            Title = "Disabled",
            PrimaryButtonText = "OK",
        };

        await quickConfigurationDialog.ShowAsync();
    }

    #region cangochecks
    private bool CanGoBack()
    {
        ViewModel.CanGoBack = (TabContent?.Content is WebContent webContent)
            ? (bool)(TabWebView?.CoreWebView2.CanGoBack)
            : (bool)(TabContent?.CanGoBack);

        return ViewModel.CanGoBack;
    }

    private bool CanGoForward()
    {
        ViewModel.CanGoForward = (TabContent?.Content is WebContent webContent)
            ? (bool)(TabWebView?.CoreWebView2.CanGoForward)
            : (bool)(TabContent?.CanGoForward);

        return ViewModel.CanGoForward;
    }

    private void GoBack()
    {
        if (CanGoBack() && TabContent != null)
        {
            if (TabContent.Content is WebContent && TabWebView.CoreWebView2.CanGoBack)
            {
                TabWebView.CoreWebView2.GoBack();
            }
            else if (TabContent.CanGoBack)
            {
                TabContent.GoBack();
            }
            else
            {
                ViewModel.CanGoBack = false;
            }
        }
    }

    private void GoForward()
    {
        if (CanGoForward() && TabContent != null)
        {
            if (TabContent.Content is WebContent && TabWebView.CoreWebView2.CanGoForward)
            {
                TabWebView.CoreWebView2.GoForward();
            }
            else if (TabContent.CanGoForward)
            {
                TabContent.GoForward();
            }
            else
            {
                ViewModel.CanGoForward = false;
            }
        }
    }

    #endregion

    #region click

    private async void ToolbarButtonClick(object sender, RoutedEventArgs e)
    {
        Passer passer = new()
        {
            Tab = Tabs.SelectedItem as FireBrowserTabViewItem,
            TabView = Tabs,
            ViewModel = ViewModel
        };

        switch ((sender as Button).Tag)
        {
            case "Back":
                GoBack();
                break;
            case "Forward":
                GoForward();
                break;
            case "Refresh":
                if (TabContent.Content is WebContent) TabWebView.CoreWebView2.Reload();
                break;
            case "Home":
                if (TabContent.Content is WebContent)
                {
                    TabContent.Navigate(typeof(NewTab));
                    UrlBox.Text = "";
                    passer.Tab.Header = WebContent.IsIncognitoModeEnabled
                          ? "Incognito"
                          : "NewTab";

                    passer.Tab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource()
                    {
                        Symbol = WebContent.IsIncognitoModeEnabled
                              ? Symbol.BlockContact
                              : Symbol.Home
                    };

                    ViewModel.CurrentAddress = "";
                }

                break;
            case "Translate":
                if (TabContent.Content is WebContent)
                {
                    string url = (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Source.ToString();
                    (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Navigate("https://translate.google.com/translate?hl&u=" + url);
                }
                break;
            case "QRCode":
                try
                {
                    if (TabContent.Content is WebContent)
                    {
                        //Create raw qr code data
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode((TabContent.Content as WebContent).WebViewElement.CoreWebView2.Source.ToString(), QRCodeGenerator.ECCLevel.M);

                        //Create byte/raw bitmap qr code
                        BitmapByteQRCode qrCodeBmp = new BitmapByteQRCode(qrCodeData);
                        byte[] qrCodeImageBmp = qrCodeBmp.GetGraphic(20);
                        using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                        {
                            using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                            {
                                writer.WriteBytes(qrCodeImageBmp);
                                await writer.StoreAsync();
                            }
                            var image = new BitmapImage();
                            await image.SetSourceAsync(stream);

                            QRCodeImage.Source = image;
                        }
                    }
                    else
                    {
                        //await UI.ShowDialog("Information", "No Webcontent Detected ( Url )");
                        QRCodeFlyout.Hide();
                    }

                }
                catch
                {
                    //await UI.ShowDialog("Error", "An error occurred while trying to generate your qr code");
                    QRCodeFlyout.Hide();
                }
                break;
            case "ReadingMode":

                break;
            case "AdBlock":

                break;
            case "AddFavoriteFlyout":
                if (TabContent.Content is WebContent)
                {
                    FavoriteTitle.Text = TabWebView.CoreWebView2.DocumentTitle;
                    FavoriteUrl.Text = TabWebView.CoreWebView2.Source;
                }
                break;
            case "AddFavorite":
                FireBrowserMultiCore.User auth = AuthService.CurrentUser;
                FavManager fv = new FavManager();
                fv.SaveFav(auth, FavoriteTitle.Text.ToString(), FavoriteUrl.Text.ToString());
                break;
            case "Favorites":
                FireBrowserMultiCore.User user = AuthService.CurrentUser;
                FavManager fs = new FavManager();
                List<FavItem> favorites = fs.LoadFav(user);

                FavoritesListView.ItemsSource = favorites;
                break;
            case "DarkMode":
                if (TabContent.Content is WebContent)
                {

                }
                break;

            case "History":
                FetchBrowserHistory();
                break;
        }
    }

    #endregion

    private async void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TabContent?.Content is WebContent webContent)
        {
            TabWebView.NavigationStarting += (_, _) => ViewModel.CanRefresh = false;
            TabWebView.NavigationCompleted += (_, _) => ViewModel.CanRefresh = true;

            await TabWebView.EnsureCoreWebView2Async();
            SmallUpdates();
        }
        else
        {
            ViewModel.CanRefresh = false;
            ViewModel.CurrentAddress = null;
        }
    }

    public static async void OpenNewWindow(Uri uri)
    {
        await Windows.System.Launcher.LaunchUriAsync(uri);
    }

    private void TabMenuClick(object sender, RoutedEventArgs e)
    {
        switch ((sender as Button).Tag)
        {
            case "NewTab":
                Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));
                SelectNewTab();
                break;
            case "NewWindow":
                OpenNewWindow(new Uri("firebrowserwinui://"));
                break;
            case "Share":

                break;
            case "DevTools":
                if (TabContent.Content is WebContent)
                {
                    (TabContent.Content as WebContent).WebViewElement.CoreWebView2.OpenDevToolsWindow();
                }
                break;
            case "Settings":
                Tabs.TabItems.Add(CreateNewTab(typeof(SettingsPage)));
                SelectNewTab();
                break;
            case "FullScreen":
                GoFullScreen(isFull != true);
                break;
            case "Downloads":
                UrlBox.Text = "firebrowser://downloads";
                TabContent.Navigate(typeof(FireBrowserWinUi3.Pages.TimeLinePages.MainTimeLine));
                break;
            case "History":
                UrlBox.Text = "firebrowser://history";
                TabContent.Navigate(typeof(FireBrowserWinUi3.Pages.TimeLinePages.MainTimeLine));
                break;
            case "InPrivate":
                OpenNewWindow(new Uri("firebrowserincog://"));
                break;
            case "Favorites":
                UrlBox.Text = "firebrowser://favorites";
                TabContent.Navigate(typeof(FireBrowserWinUi3.Pages.TimeLinePages.MainTimeLine));
                break;
        }
    }

    #region database

    private async void ClearDb()
    {
        FireBrowserMultiCore.User user = AuthService.CurrentUser;
        string username = user.Username;
        string databasePath = Path.Combine(
            UserDataManager.CoreFolderPath,
            UserDataManager.UsersFolderPath,
            username,
            "Database",
            "History.db"
        );

        HistoryTemp.ItemsSource = null;
        await DbClear.ClearTable(databasePath, "urls");
    }

    private ObservableCollection<HistoryItem> browserHistory;

    private async void FetchBrowserHistory()
    {
        FireBrowserMultiCore.User user = AuthService.CurrentUser;

        Batteries.Init();
        try
        {
            string username = user.Username;
            string databasePath = Path.Combine(
                UserDataManager.CoreFolderPath,
                UserDataManager.UsersFolderPath,
                username,
                "Database",
                "History.db"
            );

            if (System.IO.File.Exists(databasePath))
            {
                using var connection = new SqliteConnection($"Data Source={databasePath};");
                await connection.OpenAsync();

                string sql = "SELECT url, title, visit_count, typed_count, hidden FROM urls ORDER BY id DESC";

                using var command = new SqliteCommand(sql, connection);
                using var reader = command.ExecuteReader();

                browserHistory = new ObservableCollection<HistoryItem>();

                while (reader.Read())
                {
                    HistoryItem historyItem = new HistoryItem
                    {
                        Url = reader.GetString(0),
                        Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                        VisitCount = reader.GetInt32(2),
                        TypedCount = reader.GetInt32(3),
                        Hidden = reader.GetInt32(4)
                    };

                    // Fetch the image source here
                    historyItem.ImageSource = new BitmapImage(new Uri($"https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url={historyItem.Url}&size=32"));

                    browserHistory.Add(historyItem);
                }

                // Bind the browser history items to the ListView
                HistoryTemp.ItemsSource = browserHistory;
            }
            else
            {
                Debug.WriteLine("Database file does not exist at the specified path.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }
    }

    #endregion

    public FireBrowserTabViewItem CreateNewIncog(Type page = null, object param = null, int index = -1)
    {
        if (index == -1) index = Tabs.TabItems.Count;

        UrlBox.Text = "";

        FireBrowserTabViewItem newItem = new()
        {
            Header = $"Incognito",
            IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.BlockContact }
        };

        Passer passer = new()
        {
            Tab = newItem,
            TabView = Tabs,
            ViewModel = new ToolbarViewModel(),
            Param = param
        };

        newItem.Style = (Style)Microsoft.UI.Xaml.Application.Current.Resources["FloatingTabViewItemStyle"];

        Frame frame = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0, 37, 0, 0)
        };

        if (page != null)
        {
            frame.Navigate(page, passer);
        }
        else
        {
            frame.Navigate(typeof(FireBrowserWinUi3.Pages.InPrivate), passer);
        }

        newItem.Content = frame;
        return newItem;
    }

    private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        TabViewItem selectedItem = args.Tab;
        var tabContent = (Frame)selectedItem.Content;

        if (tabContent.Content is WebContent webContent && webContent.WebViewElement != null)
        {
            webContent.WebViewElement.Close();
        }

        (sender as TabView)?.TabItems?.Remove(args.Tab);
    }

    private string selectedHistoryItem;
    private void Grid_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        HistoryItem historyItem = ((FrameworkElement)sender).DataContext as HistoryItem;
        selectedHistoryItem = historyItem.Url;

        var flyout = new MenuFlyout();

        var deleteMenuItem = new MenuFlyoutItem
        {
            Text = "Delete This Record",
        };

        deleteMenuItem.Icon = new FontIcon
        {
            Glyph = "\uE74D"
        };

        deleteMenuItem.Click += (s, args) =>
        {
            FireBrowserMultiCore.User user = AuthService.CurrentUser;
            string username = user.Username;
            string databasePath = Path.Combine(
                UserDataManager.CoreFolderPath,
                UserDataManager.UsersFolderPath,
                username,
                "Database",
                "History.db"
            );

            DbClearTableData db = new();
            db.DeleteTableData(databasePath, "urls", $"Url = '{selectedHistoryItem}'");
            if (HistoryTemp.ItemsSource is ObservableCollection<HistoryItem> historyItems)
            {
                var itemToRemove = historyItems.FirstOrDefault(item => item.Url == selectedHistoryItem);
                if (itemToRemove != null)
                {
                    historyItems.Remove(itemToRemove);
                }
            }
        };

        flyout.Items.Add(deleteMenuItem);

        flyout.ShowAt((FrameworkElement)sender, e.GetPosition((FrameworkElement)sender));
    }

    private void ClearHistoryDataMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ClearDb();
    }

    private void SearchHistoryMenuFlyout_Click(object sender, RoutedEventArgs e)
    {
        if (HistorySearchMenuItem.Visibility == Visibility.Collapsed)
        {
            HistorySearchMenuItem.Visibility = Visibility.Visible;
            HistorySmallTitle.Visibility = Visibility.Collapsed;
        }
        else
        {
            HistorySearchMenuItem.Visibility = Visibility.Collapsed;
            HistorySmallTitle.Visibility = Visibility.Visible;
        }
    }

    private void FilterBrowserHistory(string searchText)
    {
        if (browserHistory == null) return;

        HistoryTemp.ItemsSource = null;

        var filteredHistory = new ObservableCollection<HistoryItem>(browserHistory
            .Where(item => item.Url.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                           item.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true));

        HistoryTemp.ItemsSource = filteredHistory;
    }

    private void HistorySearchMenuItem_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = HistorySearchMenuItem.Text;
        FilterBrowserHistory(searchText);
    }

    private void FavoritesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ListView listView = sender as ListView;
        if (listView.ItemsSource != null)
        {
            FavItem item = (FavItem)listView.SelectedItem;
            string launchurlfav = item.Url;
            if (TabContent.Content is WebContent)
            {
                (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Navigate(launchurlfav);
            }
            else
            {
                TabContent.Navigate(typeof(WebContent), CreatePasser(launchurlfav));
            }

        }
        listView.ItemsSource = null;
        FavoritesFly.Hide();
    }

    private void HistoryTemp_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ListView listView = sender as ListView;
        if (listView.ItemsSource != null)
        {
            HistoryItem item = (HistoryItem)listView.SelectedItem;
            string launchurlfav = item.Url;
            if (TabContent.Content is WebContent)
            {
                (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Navigate(launchurlfav);
            }
            else
            {
                TabContent.Navigate(typeof(WebContent), CreatePasser(launchurlfav));
            }
        }
        listView.ItemsSource = null;
        HistoryFlyoutMenu.Hide();
    }

    private void DownBtn_Click(object sender, RoutedEventArgs e)
    {
        Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowOptions options = new Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowOptions() { Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom };
        DownloadFlyout.ShowAt(DownBtn, options);
    }

    private void OpenHistoryMenuItem_Click(object sender, RoutedEventArgs e)
    {
        UrlBox.Text = "firebrowser://history";
        TabContent.Navigate(typeof(FireBrowserWinUi3.Pages.TimeLinePages.MainTimeLine));
    }

    private void OpenFavoritesMenu_Click(object sender, RoutedEventArgs e)
    {
        UrlBox.Text = "firebrowser://favorites";
        TabContent.Navigate(typeof(FireBrowserWinUi3.Pages.TimeLinePages.MainTimeLine));
    }

    private void MainUser_Click(object sender, RoutedEventArgs e)
    {
        UserFrame.Visibility = UserFrame?.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
    }

    private void MoreTool_Click(object sender, RoutedEventArgs e)
    {
        UserFrame.Visibility = Visibility.Collapsed;
    }

    private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        FireBrowserSecureConnect.TwoFactorsAuthentification.ShowFlyout(Secure);
    }

    private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
    {
        FireBrowserBusinessCore.Helpers.FlyoutLoad.ShowFlyout(Secure);
    }

    private async void SaveQrImage_Click(object sender, RoutedEventArgs e)
    {
        if (TabContent.Content is WebContent)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode((TabContent.Content as WebContent).WebViewElement.CoreWebView2.Source.ToString(), QRCodeGenerator.ECCLevel.M);

            BitmapByteQRCode qrCodeBmp = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeImageBmp = qrCodeBmp.GetGraphic(20);

            FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
            var window = (Application.Current as App)?.m_window as MainWindow;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.FileTypeChoices.Add("PNG files", new List<string>() { ".png" });
            savePicker.DefaultFileExtension = ".png";
            savePicker.SuggestedFileName = "QrImage";

            StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                try
                {
                    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await stream.WriteAsync(qrCodeImageBmp.AsBuffer());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to save the image: " + ex.Message);
                }
            }
        }
    }

    private void SwitchName_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button switchButton && switchButton.DataContext is string clickedUserName)
        {
            OpenNewWindow(new Uri($"firebrowseruser://{clickedUserName}"));
            Shortcut ct = new();
            ct.CreateShortcut(clickedUserName);
        }
    }
}