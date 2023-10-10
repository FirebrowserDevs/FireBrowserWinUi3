using CommunityToolkit.Mvvm.ComponentModel;
using FireBrowserBusiness.Controls;
using FireBrowserBusiness.Pages;
using FireBrowserMultiCore;
using FireBrowserQr;
using FireBrowserWinUi3.Controls;
using FireBrowserWinUi3.Pages;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UrlHelperWinUi3;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Appointments.DataProvider;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using WinRT.Interop;
using static System.Net.Mime.MediaTypeNames;
using Windowing = FireBrowserBusinessCore.Helpers.Windowing;

namespace FireBrowserBusiness;
public sealed partial class MainWindow : Window
{
    private AppWindow appWindow;
    private AppWindowTitleBar titleBar;
  
    public MainWindow()
    {
        InitializeComponent();

  
        Title();
        LoadUserDataAndSettings();              
        Launch();
    }

  
    public void SmallUpdates()
    {
        UrlBox.Text = TabWebView.CoreWebView2.Source.ToString();
        ViewModel.Securitytype = TabWebView.CoreWebView2.Source.ToString();

        if (TabWebView.CoreWebView2.Source.Contains("https"))
        {
            ViewModel.SecurityIcon = "\uE72E";
            ViewModel.SecurityIcontext = "Https Secured Website";
            ViewModel.Securitytext = "This Page Is Secured By A Valid SSL Certificate, Trusted By Root Authorities";
        }
        else if (TabWebView.CoreWebView2.Source.Contains("http"))
        {
            ViewModel.SecurityIcon = "\uE785";
            ViewModel.SecurityIcontext = "Http UnSecured Website";
            ViewModel.Securitytext = "This Page Is Unsecured By A Un-Valid SSL Certificate, Please Be Careful";
        }
    }
    public void Title()
    {
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        appWindow = AppWindow.GetFromWindowId(windowId);
        appWindow.SetIcon("Logo.ico");

        if (!AppWindowTitleBar.IsCustomizationSupported())
        {
            throw new Exception("Unsupported OS version.");
        }

        titleBar = appWindow.TitleBar;
        titleBar.ExtendsContentIntoTitleBar = true;
        var btnColor = Colors.Transparent;
        titleBar.BackgroundColor = btnColor;
        titleBar.ButtonBackgroundColor = btnColor;
        titleBar.InactiveBackgroundColor = btnColor;
        titleBar.ButtonInactiveBackgroundColor = btnColor;
        titleBar.ButtonHoverBackgroundColor = btnColor;

        ViewModel = new ToolbarViewModel
        {
           currentAddress = "",     
        };

        buttons();
    }

  
    public static string launchurl { get; set; }
    public static string SearchUrl { get; set; }

    public void HideToolbar(bool hide)
    {
        var visibility = hide ? Visibility.Collapsed : Visibility.Visible;
        var margin = hide ? new Thickness(0, -40, 0, 0) : new Thickness(0, 35, 0, 0);

        ClassicToolbar.Visibility = visibility;
        TabContent.Margin = margin;
    }

    Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
    private void LoadUserDataAndSettings()
    {
        if (GetUser() is not { } currentUser)
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


 

    public void buttons()
    {
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


    private FireBrowserMultiCore.User GetUser()
    {
        // Check if the user is authenticated.
        if (AuthService.IsUserAuthenticated)
        {
            // Return the authenticated user.
            return AuthService.CurrentUser;
        }

        // If no user is authenticated, return null or handle as needed.
        return null;
    }


    private async void Launch()
    {
        Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));     
    }



    private void TabView_AddTabButtonClick(TabView sender, object args)
    {
        sender.TabItems.Add(CreateNewTab(typeof(NewTab)));
    }

    #region toolbar
    public class Passer
    {
        public FireBrowserTabViewItem Tab { get; set; }
        public FireBrowserTabViewContainer TabView { get; set; }
        public object Param { get; set; }

        public ToolbarViewModel ViewModel { get; set; }
        public string UserName { get; set; }
    }

    public ToolbarViewModel ViewModel { get; set; }

    public partial class ToolbarViewModel : ObservableObject
    {
        [ObservableProperty]
        public bool canRefresh;
        [ObservableProperty]
        public bool canGoBack;
        [ObservableProperty]
        public bool canGoForward;
        [ObservableProperty]
        public string currentAddress;
        [ObservableProperty]
        public string securityIcon;
        [ObservableProperty]
        public string securityIcontext;
        [ObservableProperty]
        public string securitytext;
        [ObservableProperty]
        public string securitytype;
        [ObservableProperty]
        public Visibility homeButtonVisibility;

        private string _userName;

        public string UserName
        {
            get
            {
                if (_userName == "DefaultFireBrowserUser") return "DefaultFireBrowserUserName";
                else return _userName;
            }
            set { SetProperty(ref _userName, value); }
        }
    }

    #endregion

    private FireBrowserTabViewItem CreateNewTab(Type page = null, object param = null, int index = -1)
    {
        if (index == -1) index = Tabs.TabItems.Count;

        var newItem = new FireBrowserTabViewItem
        {
            Header = "FireBrowser HomePage",
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

        passer.ViewModel.CurrentAddress = null;

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

        var toolTip = new ToolTip();
        toolTip.Content = new Grid
        {
            Children =
        {
            new Microsoft.UI.Xaml.Controls.Image(),
            new TextBlock()
        }
        };
        ToolTipService.SetToolTip(newItem, toolTip);

        newItem.Content = frame;
        return newItem;
    }

    public Frame TabContent
    {
        get
        {
            FireBrowserTabViewItem selectedItem = (FireBrowserTabViewItem)Tabs.SelectedItem;
            if (selectedItem != null)
            {
                return (Frame)selectedItem.Content;
            }
            return null;
        }
    }

    public WebView2 TabWebView
    {
        get
        {
            if (TabContent.Content is WebContent)
            {
                return (TabContent.Content as WebContent).WebViewElement;
            }
            return null;
        }
    }


    private double GetScaleAdjustment()
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
        IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

        int result = Windowing.GetDpiForMonitor(hMonitor, Windowing.Monitor_DPI_Type.MDT_Default_DPI, out uint dpiX, out _);

        if (result != 0)
        {
            throw new Exception("Could not get DPI");
        }

        return dpiX / 96.0; // Simplified calculation
    }

    private void Tabs_Loaded(object sender, RoutedEventArgs e)
    {
        Apptitlebar.SizeChanged += Apptitlebar_SizeChanged;
    }

    private void Apptitlebar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        double scaleAdjustment = GetScaleAdjustment();
        Apptitlebar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var customDragRegionPosition = Apptitlebar.TransformToVisual(null).TransformPoint(new Point(0, 0));

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
        Apptitlebar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var customDragRegionPosition = Apptitlebar.TransformToVisual(null).TransformPoint(new Point(0, 0));

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

        if (appWindow.TitleBar != null)
        {
            appWindow.TitleBar.SetDragRectangles(dragRects);
        }
    }

    private int maxTabItems = 20;
    private void Tabs_TabItemsChanged(TabView sender, IVectorChangedEventArgs args)
    {
        Tabs.IsAddTabButtonVisible = Tabs.TabItems.Count < maxTabItems;
    }

    private Passer CreatePasser(object parameter = null)
    {
        Passer passer = new()
        {
            Tab = Tabs.SelectedItem as FireBrowserTabViewItem,
            TabView = Tabs,
            ViewModel = ViewModel,
            Param = parameter,
        };
        return passer;
    }

    public void SelectNewTab()
    {
        var tabToSelect = Tabs.TabItems.Count - 1;
        Tabs.SelectedIndex = tabToSelect;
    }

    public void FocusUrlBox(string text)
    {
        UrlBox.Text = text;
        UrlBox.Focus(FocusState.Programmatic);
    }
    public void NavigateToUrl(string uri)
    {
        if (TabContent.Content is WebContent webContent)
        {
            webContent.WebViewElement.CoreWebView2.Navigate(uri.ToString());
        }
        else
        {
            launchurl ??= uri;
            TabContent.Navigate(typeof(WebContent), CreatePasser(uri));
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
                    default:
                        // default behavior
                        break;
                }
            }
            else if (inputtype == "url")
            {
                NavigateToUrl(input.Trim());
            }
            else if (inputtype == "urlNOProtocol")
            {
                NavigateToUrl("https://" + input.Trim());
            }
            else
            {
                string searchurl;
                if (SearchUrl == null) searchurl = "https://www.google.nl/search?q=";
                else
                {
                    searchurl = SearchUrl;
                }
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

    #region cangochecks
    private bool CanGoBack()
    {
        ViewModel.CanGoBack = (bool)(TabContent?.Content is WebContent
            ? TabWebView?.CoreWebView2.CanGoBack
            : TabContent?.CanGoBack);

        return ViewModel.CanGoBack;
    }


    private bool CanGoForward()
    {
        ViewModel.CanGoForward = (bool)(TabContent?.Content is WebContent
            ? TabWebView?.CoreWebView2.CanGoForward
            : TabContent?.CanGoForward);
        return ViewModel.CanGoForward;
    }


    private void GoBack()
    {
        if (CanGoBack() && TabContent != null)
        {
            if (TabContent.Content is WebContent && TabWebView.CoreWebView2.CanGoBack) TabWebView.CoreWebView2.GoBack();
            else if (TabContent.CanGoBack) TabContent.GoBack();
            else ViewModel.CanGoBack = false;
        }
    }

    private void GoForward()
    {
        if (CanGoForward() && TabContent != null)
        {
            if (TabContent.Content is WebContent && TabWebView.CoreWebView2.CanGoForward) TabWebView.CoreWebView2.GoForward();
            else if (TabContent.CanGoForward) TabContent.GoForward();
            else ViewModel.CanGoForward = false;
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
                }
                break;
            case "DownloadFlyout":
                if (TabContent.Content is WebContent)
                {
                    if (TabWebView.CoreWebView2.IsDefaultDownloadDialogOpen == true)
                    {
                        (TabContent.Content as WebContent).WebViewElement.CoreWebView2.CloseDefaultDownloadDialog();
                    }
                    else
                    {
                        (TabContent.Content as WebContent).WebViewElement.CoreWebView2.OpenDefaultDownloadDialog();
                    }
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
              
                break;
            case "Favorites":
               
                break;
            case "DarkMode":
               
                break;
        }
    }

    #endregion

    private async void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TabContent?.Content is WebContent webContent)
        {
            TabWebView.NavigationStarting += (s, e) =>
            {
                ViewModel.CanRefresh = false;
            };
            TabWebView.NavigationCompleted += (s, e) =>
            {
                ViewModel.CanRefresh = true;
            };
            await TabWebView.EnsureCoreWebView2Async();
            SmallUpdates();
        }
        else
        {
            ViewModel.CanRefresh = false;
            ViewModel.CurrentAddress = null;
        }

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
                MainWindow newWindow = new();
                newWindow.Activate();
                break;
            case "Share":
                
                break;
            case "DevTools":
                if (TabContent.Content is WebContent) (TabContent.Content as WebContent).WebViewElement.CoreWebView2.OpenDevToolsWindow();
                break;
            case "Settings":
                Tabs.TabItems.Add(CreateNewTab(typeof(SettingsPage)));
                SelectNewTab();
                break;
            case "FullScreen":
              
                break;
            case "History":
                
                break;
            case "InPrivate":
             
                break;
            case "Favorites":
               
                break;
        }
    }

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

        // The content of the tab is often a frame that contains a page, though it could be any UIElement.

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

        if (tabContent.Content is WebContent webContent)
        {
            var webView = webContent.WebViewElement;

            if (webView != null)
            {
                webView.Close();
            }
        }

        var tabItems = (sender as TabView)?.TabItems;
        tabItems?.Remove(args.Tab);
    }
}
