using FireBrowserBusiness.Controls;
using FireBrowserBusiness.Pages;
using FireBrowserBusiness.Services;
using FireBrowserBusinessCore.Helpers;
using FireBrowserBusinessCore.Models;
using FireBrowserBusinessCore.ShareHelper;
using FireBrowserBusinessCore.ViewModel;
using FireBrowserDatabase;
using FireBrowserDataCore.Actions;
using FireBrowserExceptions;
using FireBrowserFavorites;
using FireBrowserMultiCore;
using FireBrowserQr;
using FireBrowserWinUi3.Controls;
using FireBrowserWinUi3.Pages;
using FireBrowserWinUi3Core.CoreUi;
using FireBrowserWinUiModules.Darkmode;
using FireBrowserWinUiModules.Read;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

    public DownloadFlyout DownloadFlyout { get; set; } = new DownloadFlyout();
    public DownloadService ServiceDownloads { get; set; }

    public MainWindow()
    {
        ServiceDownloads = App.GetService<DownloadService>();

        InitializeComponent();

        ArgsPassed();
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

    public void setColorsTool()
    {
        var toolbar = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
        if (toolbar.ColorTV == "#000000")
        {
            Tabs.Background = new SolidColorBrush(Colors.Transparent);
        }
        else
        {
            string colorw = toolbar.ColorTV;
            var color = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), colorw);
            var brush = new SolidColorBrush(color);
            Tabs.Background = brush;
        }
        if (toolbar.ColorTool == "#000000")
        {
            ClassicToolbar.Background = new SolidColorBrush(Colors.Transparent);
        }
        else
        {
            string colorw = toolbar.ColorTool;
            var color = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), colorw);
            var brush = new SolidColorBrush(color);
            ClassicToolbar.Background = brush;
        }
    }

    private async void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (Tabs.TabItems?.Count > 1)
        {
            Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
            if (userSettings.ConfirmCloseDlg == "1")
            {
                try
                {
                    args.Cancel = true;

                    if (!(Application.Current is App currentApp) || !(currentApp.m_window is MainWindow mainWindow))
                        return;

                    ConfirmAppClose quickConfigurationDialog = new()
                    {
                        XamlRoot = mainWindow.Content.XamlRoot
                    };

                    quickConfigurationDialog.PrimaryButtonClick += async (_, _) =>
                    {
                        quickConfigurationDialog.Hide();
                        await Task.Delay(250);
                        Application.Current.Exit();
                    };
                    await quickConfigurationDialog.ShowAsync();
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(ex);
                }
            }
            return;
        }
        args.Cancel = false;
    }

    bool incog = false;

    private async void ArgsPassed()
    {
        TitleTop();

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
                Tabs.TabItems.Add(CreateNewTab(typeof(WebContent), files[1]));
            }
            return;
        }

        if (!string.IsNullOrEmpty(AppArguments.FireBrowserIncog))
        {
            Tabs.TabItems.Add(CreateNewIncog(typeof(InPrivate)));
            var controlsToDisable = new Control[] { Fav, His, History, Down, DownBtn, FavoritesButton, UserFrame };
            foreach (var control in controlsToDisable) control.IsEnabled = false;
            NewTab.Visibility = Visibility.Collapsed;
            NewWindow.Visibility = Visibility.Collapsed;
            WebContent.IsIncognitoModeEnabled = true;
            UserDataManager.DeleteUser("Private");
            InPrivateUser();
            incog = true;
            return;
        }

        Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));
    }


    private void InPrivateUser()
    {
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "Private",
            IsFirstLaunch = true,
            UserSettings = null
        };

        AuthService.AddUser(newUser);
        UserFolderManager.CreateUserFolders(newUser);
        AuthService.CurrentUser.Username = newUser.Username;
        AuthService.Authenticate(newUser.Username);
    }
    public void LoadUsernames()
    {
        var currentUsername = AuthService.CurrentUser?.Username;
        UserListView.Items.Clear();
        foreach (var username in AuthService.GetAllUsernames().Where(u => u != currentUsername && !u.Contains("Private")))
        {
            UserListView.Items.Add(username);
        }
    }

    public void SmallUpdates()
    {
        string source = TabWebView.CoreWebView2.Source?.ToString() ?? string.Empty;
        UrlBox.Text = source;
        ViewModel.Securitytype = source;

        ViewModel.SecurityIcon = source switch
        {
            string s when s.Contains("https") => "\uE72E",
            string s when s.Contains("http") => "\uE785",
            _ => ""
        };

        ViewModel.SecurityIcontext = source switch
        {
            string s when s.Contains("https") => "Https Secured Website",
            string s when s.Contains("http") => "Http UnSecured Website",
            _ => ""
        };

        ViewModel.Securitytext = source switch
        {
            string s when s.Contains("https") => "This Page Is Secured By A Valid SSL Certificate, Trusted By Root Authorities",
            string s when s.Contains("http") => "This Page Is Unsecured By A Non-Valid SSL Certificate, Please Be Careful",
            _ => ""
        };
    }


    public void TitleTop()
    {
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        appWindow = AppWindow.GetFromWindowId(windowId);
        appWindow.SetIcon("Logo.ico");

        if (!AppWindowTitleBar.IsCustomizationSupported())
            throw new Exception("Unsupported OS version.");

        var titleBar = appWindow.TitleBar;
        titleBar.ExtendsContentIntoTitleBar = true;
        var btnColor = Colors.Transparent;
        titleBar.BackgroundColor = titleBar.ButtonBackgroundColor =
            titleBar.InactiveBackgroundColor = titleBar.ButtonInactiveBackgroundColor =
            titleBar.ButtonHoverBackgroundColor = btnColor;

        ViewModel = new() { CurrentAddress = "", SecurityIcon = "\uE946", SecurityIcontext = "FireBrowser NewTab", Securitytext = "This The Default Home Page Of FireBrowser Internal Pages Secure", Securitytype = "Link - FireBrowser://NewTab" };
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

        ClassicToolbar.Height = fullscreen ? 0 : 36;

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
        setColorsTool();
    }

    private void LoadUserDataAndSettings()
    {
        FireBrowserMultiCore.User currentUser = AuthService.IsUserAuthenticated ? AuthService.CurrentUser : null;

        if (currentUser == null || (!AuthService.IsUserAuthenticated && !AuthService.Authenticate(currentUser?.Username)))
        {
            UserName.Text = "DefaultUser";
            return;
        }

        UserName.Text = currentUser.Username ?? "DefaultUser";
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
    public FireBrowserTabViewItem CreateNewTab(Type? page = null, object param = null, int index = -1)
    {
        index = Tabs.TabItems.Count;

        var newItem = new FireBrowserTabViewItem
        {
            Header = "NewTab",
            IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource { Symbol = Symbol.Home },
            Style = (Style)Microsoft.UI.Xaml.Application.Current.Resources["FloatingTabViewItemStyle"]
        };

        var passer = new Passer
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

        newItem.Content = frame;

        return newItem;
    }

    public Frame TabContent => (Tabs.SelectedItem as FireBrowserTabViewItem)?.Content as Frame;
    public WebView2 TabWebView => (TabContent?.Content as WebContent)?.WebViewElement;
    public FireBrowserTabViewContainer TabViewContainer => Tabs;
    private double GetScaleAdjustment()
    {
        var hWnd = WindowNative.GetWindowHandle(this);
        var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
        var hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

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
        if (sender.TabItems.Count <= 0) Application.Current.Exit();
        else sender.CanReorderTabs = sender.CanDragTabs = sender.TabItems.Count > 1;
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
    public void SelectNewTab() => Tabs.SelectedIndex = Tabs.TabItems.Count - 1;
    public void FocusUrlBox(string text)
    {
        UrlBox.Text = text;
        UrlBox.Focus(FocusState.Programmatic);
    }
    public void NavigateToUrl(string uri)
    {
        try
        {
            if (!(TabContent.Content is WebContent webContent))
            {
                launchurl ??= uri;
                TabContent.Navigate(typeof(WebContent), CreatePasser(uri));

                return;
            }

            webContent.WebViewElement.CoreWebView2.Navigate(uri.ToString());
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex);
        }
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
                    case "firebrowser://vault":
                        //Tabs.TabItems.Add(CreateNewTab(typeof(SecureVault)));
                        //SelectNewTab();
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
            ExceptionLogger.LogException(ex);
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
            else
            {
                TabContent.GoBack();
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
            else
            {
                TabContent.GoForward();
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
            case "Refresh" when TabContent.Content is WebContent:
                TabWebView.CoreWebView2.Reload();
                break;
            case "Home" when TabContent.Content is WebContent:
                if (incog == true)
                {
                    TabContent.Navigate(typeof(InPrivate));
                }
                else
                {
                    TabContent.Navigate(typeof(NewTab));
                }
                UrlBox.Text = "";
                passer.Tab.Header = WebContent.IsIncognitoModeEnabled ? "Incognito" : "NewTab";
                passer.Tab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource()
                {
                    Symbol = WebContent.IsIncognitoModeEnabled ? Symbol.BlockContact : Symbol.Home
                };
                ViewModel.CurrentAddress = "";
                break;
            case "Translate" when TabContent.Content is WebContent:
                string url = (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Source.ToString();
                (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Navigate($"https://translate.google.com/translate?hl&u={url}");
                break;
            case "QRCode" when TabContent.Content is WebContent:
                try
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode((TabContent.Content as WebContent).WebViewElement.CoreWebView2.Source.ToString(), QRCodeGenerator.ECCLevel.M);
                    BitmapByteQRCode qrCodeBmp = new BitmapByteQRCode(qrCodeData);
                    byte[] qrCodeImageBmp = qrCodeBmp.GetGraphic(20);

                    using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                    using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes(qrCodeImageBmp);
                        await writer.StoreAsync();

                        var image = new BitmapImage();
                        await image.SetSourceAsync(stream);

                        QRCodeImage.Source = image;
                    }
                }
                catch
                {
                    QRCodeFlyout.Hide();
                }
                break;
            case "ReadingMode" when TabContent.Content is WebContent:
                string jscriptread = await ReadabilityHelper.GetReadabilityScriptAsync();
                await (TabContent.Content as WebContent).WebViewElement.CoreWebView2.ExecuteScriptAsync(jscriptread);
                break;
            case "AdBlock":
                // Handle AdBlock
                break;
            case "AddFavoriteFlyout" when TabContent.Content is WebContent:
                FavoriteTitle.Text = TabWebView.CoreWebView2.DocumentTitle;
                FavoriteUrl.Text = TabWebView.CoreWebView2.Source;
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
            case "DarkMode" when TabContent.Content is WebContent:
                string jscriptdark = await ForceDarkHelper.GetForceDarkScriptAsync();
                await (TabContent.Content as WebContent).WebViewElement.CoreWebView2.ExecuteScriptAsync(jscriptdark);
                break;
            case "History":
                FetchBrowserHistory();
                break;
        }
    }


    #endregion
    private async void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);

        if (TabContent?.Content is WebContent webContent)
        {
            TabWebView.NavigationStarting += (_, _) => ViewModel.CanRefresh = false;
            TabWebView.NavigationCompleted += (_, _) => ViewModel.CanRefresh = true;

            await TabWebView.EnsureCoreWebView2Async();
            // 2014-02-04 added to stop a video from playing when selection is made to a different tab / save on memory resources. 
            if (e.RemovedItems.Count > 0)
            {
                e.RemovedItems.All((tab) =>
                {

                    if (tab is FireBrowserTabViewItem viewedItem)
                    {
                        if (viewedItem.Content is Frame frame)
                        {
                            if (userSettings.ResourceSave != null && userSettings.ResourceSave.Equals("1"))
                            {
                                if (frame.Content is WebContent web)
                                {
                                    frame.DispatcherQueue.TryEnqueue(async () =>
                                    {
                                        await web.WebView.CoreWebView2?.ExecuteScriptAsync(@"(function() { 
                                        try
                                        {
                                            const videos = document.querySelectorAll('video');
                                            videos.forEach((video) => { video.pause();});
                                            console.log('WINUI3_CoreWebView2: YES_VIDEOS_CLOSED');
                                            return true; 

                                        }
                                        catch(error) {
                                          console.log('WINUI3_CoreWebView2: NO_VIDEOS_CLOSED');
                                          return error.message; 
                                        }
                                    })();");
                                    });
                                }
                            }
                            else
                            {
                                // do nothing
                            }
                        }
                        return true;
                    }
                    return false;
                });
            }

            SmallUpdates();
        }
        else
        {
            ViewModel.CanRefresh = false;
            ViewModel.CurrentAddress = null;
        }
    }

    public static async void OpenNewWindow(Uri uri) => await Windows.System.Launcher.LaunchUriAsync(uri);

    public void ShareUi(string Url, string Title)
    {
        var hWnd = WindowNative.GetWindowHandle(this);
        ShareUIHelper.ShowShareUIURL(Url, Title, hWnd);
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
                OpenNewWindow(new Uri($"firebrowseruser://{UserName.Text}"));
                break;
            case "Share" when TabContent.Content is WebContent:
                ShareUi(TabWebView.CoreWebView2.DocumentTitle, TabWebView.CoreWebView2.Source);
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
        HistoryActions historyActions = new HistoryActions(AuthService.CurrentUser.Username);
        await historyActions.DeleteAllHistoryItems();

        HistoryTemp.ItemsSource = null;
        //FireBrowserMultiCore.User user = AuthService.CurrentUser;
        //string username = user.Username;
        //string databasePath = Path.Combine(
        //    UserDataManager.CoreFolderPath,
        //    UserDataManager.UsersFolderPath,
        //    username,
        //    "Database",
        //    "History.db"
        //);

        //HistoryTemp.ItemsSource = null;
        //await DbClear.ClearTable(databasePath, "urls");
    }

    private ObservableCollection<HistoryItem> browserHistory;

    private async void FetchBrowserHistory()
    {
        //FireBrowserMultiCore.User user = AuthService.CurrentUser;
        try
        {
            HistoryActions historyActions = new HistoryActions(AuthService.CurrentUser.Username);
            var items = await historyActions.GetAllHistoryItems();
            HistoryTemp.ItemsSource = items; // browserHistory;
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex);
        }

    }

    #endregion

    public FireBrowserTabViewItem CreateNewIncog(Type? page = null, object? param = null, int index = -1)
    {
        index = Tabs.TabItems.Count;
        UrlBox.Text = "";

        var newItem = new FireBrowserTabViewItem
        {
            Header = $"Incognito",
            IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource { Symbol = Symbol.BlockContact },
            Style = (Style)Microsoft.UI.Xaml.Application.Current.Resources["FloatingTabViewItemStyle"]
        };

        var passer = new Passer
        {
            Tab = newItem,
            TabView = Tabs,
            ViewModel = new ToolbarViewModel(),
            Param = param
        };

        var frame = new Frame
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0, 37, 0, 0)
        };

        _ = page != null ? frame.Navigate(page, passer) : frame.Navigate(typeof(FireBrowserWinUi3.Pages.InPrivate), passer);

        newItem.Content = frame;
        return newItem;
    }


    private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        if (!(args.Tab?.Content is Frame tabContent)) return;

        if (tabContent.Content is WebContent webContent && webContent.WebViewElement != null)
        {
            webContent.WebViewElement.Close();
        }

        (sender as TabView)?.TabItems?.Remove(args.Tab);
    }


    private string selectedHistoryItem;
    private void Grid_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        if (((FrameworkElement)sender).DataContext is not HistoryItem historyItem) return;

        selectedHistoryItem = historyItem.Url;

        var flyout = new MenuFlyout();

        var deleteMenuItem = new MenuFlyoutItem
        {
            Text = "Delete This Record",
            Icon = new FontIcon { Glyph = "\uE74D" }
        };

        deleteMenuItem.Click += async (s, args) =>
        {
            if (!(AuthService.CurrentUser is FireBrowserMultiCore.User user)) return;

            string username = user.Username;
            string databasePath = Path.Combine(
                UserDataManager.CoreFolderPath,
                UserDataManager.UsersFolderPath,
                username,
                "Database",
                "History.db"
            );

            HistoryActions historyActions = new HistoryActions(AuthService.CurrentUser.Username);
            await historyActions.DeleteHistoryItem(selectedHistoryItem);
            await Task.Delay(1000);

            if (HistoryTemp.ItemsSource is ObservableCollection<HistoryItem> historyItems)
            {
                var itemToRemove = historyItems.FirstOrDefault(item => item.Url == selectedHistoryItem);
                if (itemToRemove != null)
                    historyItems.Remove(itemToRemove);
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
        HistorySearchMenuItem.Visibility = HistorySearchMenuItem.Visibility == Visibility.Collapsed
            ? Visibility.Visible
            : Visibility.Collapsed;

        HistorySmallTitle.Visibility = HistorySmallTitle.Visibility == Visibility.Collapsed
            ? Visibility.Visible
            : Visibility.Collapsed;
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
        if (!(sender is ListView listView) || listView.ItemsSource == null) return;

        if (listView.SelectedItem is FavItem item)
        {
            string launchurlfav = item.Url;
            if (TabContent.Content is WebContent webContent)
            {
                webContent.WebViewElement.CoreWebView2.Navigate(launchurlfav);
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
        if (!(sender is ListView listView) || listView.ItemsSource == null) return;

        if (!(listView.SelectedItem is HistoryItem item)) return;
        string launchurlfav = item.Url;
        if (TabContent.Content is WebContent webContent)
        {
            webContent.WebViewElement.CoreWebView2.Navigate(launchurlfav);
        }
        else
        {
            TabContent.Navigate(typeof(WebContent), CreatePasser(launchurlfav));
        }

        listView.ItemsSource = null;
        HistoryFlyoutMenu.Hide();
    }

    private void DownBtn_Click(object sender, RoutedEventArgs e)
    {
        var options = new Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowOptions { Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom };
        DownloadFlyout.ShowAt(DownBtn, options);
    }

    private void OpenHistoryMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _ = (UrlBox.Text = "firebrowser://history") + (TabContent.Navigate(typeof(FireBrowserWinUi3.Pages.TimeLinePages.MainTimeLine)));
    }

    private void OpenFavoritesMenu_Click(object sender, RoutedEventArgs e)
    {
        _ = (UrlBox.Text = "firebrowser://favorites") + (TabContent.Navigate(typeof(FireBrowserWinUi3.Pages.TimeLinePages.MainTimeLine)));
    }

    private void MainUser_Click(object sender, RoutedEventArgs e)
    {
        UserFrame.Visibility = UserFrame?.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        LoadUsernames();
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
        if (TabContent.Content is WebContent webContent)
        {
            var qrCodeData = new QRCodeGenerator().CreateQrCode(webContent.WebViewElement.CoreWebView2.Source.ToString(), QRCodeGenerator.ECCLevel.M);
            var qrCodeBmp = new BitmapByteQRCode(qrCodeData).GetGraphic(20);

            var savePicker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                DefaultFileExtension = ".png",
                SuggestedFileName = "QrImage"
            };

            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, WinRT.Interop.WindowNative.GetWindowHandle((Application.Current as App)?.m_window as MainWindow));
            savePicker.FileTypeChoices.Add("PNG files", new List<string>() { ".png" });

            if (await savePicker.PickSaveFileAsync() is StorageFile file)
            {
                try
                {
                    await (await file.OpenAsync(FileAccessMode.ReadWrite)).WriteAsync(qrCodeBmp.AsBuffer());
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(ex);
                }
            }
        }
    }

    private void SwitchName_Click(object sender, RoutedEventArgs e)
    {
        if (!(sender is Button switchButton && switchButton.DataContext is string clickedUserName)) return;
        OpenNewWindow(new Uri($"firebrowseruser://{clickedUserName}")); new Shortcut().CreateShortcut(clickedUserName);
    }
}