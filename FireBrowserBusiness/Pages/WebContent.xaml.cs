using CommunityToolkit.WinUI.Helpers;
using FireBrowserBusiness;
using FireBrowserBusiness.Controls;
using FireBrowserBusinessCore.Helpers;
using FireBrowserMultiCore;
using FireBrowserWinUi3.Controls;
using FireBrowserWinUi3Core.CoreUi;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using static FireBrowserBusiness.MainWindow;

namespace FireBrowserWinUi3.Pages;

public sealed partial class WebContent : Page
{
    Passer param;
    public static bool IsIncognitoModeEnabled { get; set; } = false;
    public BitmapImage PictureWebElement { get; set; }
    public WebView2 WebView { get; set; }
    private FireBrowserMultiCore.User GetUser()
    {
        // Check if the user is authenticated.
        if (AuthService.IsUserAuthenticated)
        {
            return AuthService.CurrentUser;
        }

        return null;

    }
    public WebContent()
    {
        this.InitializeComponent();
        Init();
        WebView = this.WebViewElement;
    }

    public void Init()
    {
        var currentUser = GetUser();

        if (currentUser is null) return;

        // Check if the user is authenticated
        if (!AuthService.IsUserAuthenticated && !AuthService.Authenticate(currentUser.Username)) return;

        // Get the path to the browser folder
        string browserFolderPath = Path.Combine(UserDataManager.CoreFolderPath, "Users", currentUser.Username, "Browser");

        Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", browserFolderPath);
        Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--enable-features=msSingleSignOnOSForPrimaryAccountIsShared");
    }

    public void AfterComplete()
    {
        if (!IsIncognitoModeEnabled)
        {
            var username = AuthService.CurrentUser;
            var url = WebViewElement.CoreWebView2.Source.ToString();
            var title = WebViewElement.CoreWebView2.DocumentTitle.ToString();

            SaveDb.InsertHistoryItem(username, url, title, visitCount: 0, typedCount: 0, hidden: 0);

            var isHttps = WebViewElement.CoreWebView2.Source.Contains("https");
            var isHttp = WebViewElement.CoreWebView2.Source.Contains("http");

            param.ViewModel.SecurityIcon = isHttps ? "\uE72E" : (isHttp ? "\uE785" : "");
            param.ViewModel.SecurityIcontext = isHttps ? "Https Secured Website" : (isHttp ? "Http UnSecured Website" : "");
        }
        else
        {
            // Handle incognito mode scenario
        }
    }

    public void LoadSettings()
    {
        //webview
        WebViewElement.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = (userSettings.BrowserKeys == "1") ? true : false;
        WebViewElement.CoreWebView2.Settings.IsStatusBarEnabled = (userSettings.StatusBar == "1") ? true : false;
        WebViewElement.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = (userSettings.BrowserScripts == "1") ? true : false;

        //privacy 
        WebViewElement.CoreWebView2.Settings.IsScriptEnabled = (userSettings.DisableJavaScript == "1") ? false : true;
        WebViewElement.CoreWebView2.Settings.IsPasswordAutosaveEnabled = (userSettings.DisablePassSave == "1") ? false : true;
        WebViewElement.CoreWebView2.Settings.IsGeneralAutofillEnabled = (userSettings.DisableGenAutoFill == "1") ? false : true;
        WebViewElement.CoreWebView2.Settings.IsWebMessageEnabled = (userSettings.DisableWebMess == "1") ? false : true;

        string disableWebMessSetting = userSettings.TrackPrevention ?? "2";

        // Map the numeric value to the corresponding tracking prevention level
        CoreWebView2TrackingPreventionLevel preventionLevel;
        switch (int.Parse(disableWebMessSetting))
        {
            case 0:
                preventionLevel = CoreWebView2TrackingPreventionLevel.None;
                break;
            case 1:
                preventionLevel = CoreWebView2TrackingPreventionLevel.Basic;
                break;
            case 2:
                preventionLevel = CoreWebView2TrackingPreventionLevel.Balanced;
                break;
            case 3:
                preventionLevel = CoreWebView2TrackingPreventionLevel.Strict;
                break;
            default:
                // You may want to handle unexpected values here
                preventionLevel = CoreWebView2TrackingPreventionLevel.Balanced;
                break;
        }

        // Set the PreferredTrackingPreventionLevel
        WebViewElement.CoreWebView2.Profile.PreferredTrackingPreventionLevel = preventionLevel;
    }

    Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);

    protected override async void OnNavigatedFrom(NavigationEventArgs e)
    {
        await WebViewElement.CoreWebView2?.ExecuteScriptAsync(@"(function() { 
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
    }

    public static class WebViewUtils
    {
        public static async Task EarlySync(WebView2 webView)
        {
            await webView.EnsureCoreWebView2Async();
            // Add any additional setup or event handling here
        }
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        param = e.Parameter as Passer;

        await WebViewUtils.EarlySync(WebViewElement);

        LoadSettings();
        WebView2 s = WebViewElement;

        if (param?.Param != null)
        {
            WebViewElement.CoreWebView2.Navigate(param.Param.ToString());
        }

        string useragent = userSettings.Useragent;

        useragent = string.IsNullOrEmpty(useragent) ? "1" : useragent;

        var userAgent = s?.CoreWebView2.Settings.UserAgent;
        if (!string.IsNullOrEmpty(userAgent))
        {
            var edgIndex = userAgent.IndexOf("Edg/");
            if (edgIndex >= 0)
            {
                if (!string.IsNullOrEmpty(userAgent))
                {
                    userAgent = userAgent.Substring(0, edgIndex);
                    s.CoreWebView2.Settings.UserAgent = userAgent;
                }
            }
        }
        s.DragLeave += (sender, args) =>
        {
            if (sender is WebView2 web)
            {
                OpenNewWindow(web.Source);
            }
        };
        s.CoreWebView2.ContainsFullScreenElementChanged += (sender, args) =>
        {
            var window = (Application.Current as App)?.m_window as MainWindow;
            window.GoFullScreenWeb(s.CoreWebView2.ContainsFullScreenElement);
        };
        s.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
        s.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
        s.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
        s.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
        s.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;
        s.CoreWebView2.ScriptDialogOpening += async (sender, args) =>
        {
            args.GetDeferral();
            var window = (Application.Current as App)?.m_window as MainWindow;
            await UIScript.ShowDialog($"{sender.DocumentTitle} says", args.Message, window.Content.XamlRoot);
        };
        s.CoreWebView2.DocumentTitleChanged += (sender, args) =>
        {
            if (!IsIncognitoModeEnabled)
            {
                param.Tab.Header = WebViewElement.CoreWebView2.DocumentTitle;
            }
            else
            {

            }
        };
        s.CoreWebView2.PermissionRequested += (sender, args) =>
        {
            try
            {
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        };
        s.CoreWebView2.FaviconChanged += async (sender, args) =>
        {
            try
            {
                if (!IsIncognitoModeEnabled)
                {
                    var bitmapImage = new BitmapImage();
                    var stream = await sender.GetFaviconAsync(0);

                    var iconSource = new ImageIconSource { ImageSource = bitmapImage };
                    if (stream != null)
                    {
                        await bitmapImage.SetSourceAsync(stream);
                    }
                    else
                    {
                        var bitmapImage2 = new BitmapImage();
                        await bitmapImage2.SetSourceAsync(await sender.GetFaviconAsync(CoreWebView2FaviconImageFormat.Jpeg));
                        iconSource.ImageSource = bitmapImage2;
                    }

                    param.Tab.IconSource = iconSource;
                }
            }
            catch
            {
                // Handle any exceptions that might occur during the process
            }
        };
        s.CoreWebView2.NavigationStarting += (sender, args) =>
        {
            Progress.IsIndeterminate = true;
            Progress.Visibility = Visibility.Visible;

            if ((TabViewItem)param.TabView.SelectedItem == param.Tab)
            {
                CheckNetworkStatus();
            }
        };

        s.CoreWebView2.NavigationCompleted += async (sender, args) =>
        {
            Progress.IsIndeterminate = false;
            Progress.Visibility = Visibility.Collapsed;


            if ((TabViewItem)param.TabView.SelectedItem == param.Tab)
            {
                CheckNetworkStatus();
                AfterComplete();
            }

            //thread safe
            s?.DispatcherQueue.TryEnqueue(async () =>
            {
                // allow webview to load the page
                await Task.Delay(2400);

                BitmapImage bitmap = new() { DecodePixelHeight = 512, DecodePixelWidth = 640 };

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await s?.CoreWebView2?.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Jpeg, memoryStream.AsRandomAccessStream());
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    bitmap.SetSource(memoryStream.AsRandomAccessStream());

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    Windows.Storage.StorageFile file = await ApplicationData.Current.RoamingFolder.CreateFileAsync("view.png", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                    await Windows.Storage.FileIO.WriteBytesAsync(file, memoryStream.GetBuffer());
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    PictureWebElement = bitmap;

                    // set the active tab with the current view
                    MainWindow win = (Window)(Application.Current as App).m_window as MainWindow;
                    if (win?.TabViewContainer.SelectedItem is FireBrowserTabViewItem tab)
                    {
                        if (win?.TabContent.Content is WebContent web)
                            tab.BitViewWebContent = web.PictureWebElement;
                    }
                }
            });

        };
        s.CoreWebView2.SourceChanged += (sender, args) =>
        {
            if ((TabViewItem)param.TabView.SelectedItem == param.Tab)
            {
                param.ViewModel.CurrentAddress = sender.Source;
            }
        };
        s.CoreWebView2.NewWindowRequested += (sender, args) =>
        {
            var window = (Application.Current as App)?.m_window as MainWindow;
            param?.TabView.TabItems.Add(window.CreateNewTab(typeof(WebContent), args.Uri));
            args.Handled = true;
        };
    }

    private void CoreWebView2_DownloadStarting(CoreWebView2 sender, CoreWebView2DownloadStartingEventArgs args)
    {
        var window = (Application.Current as App)?.m_window as MainWindow;

        window.DownloadFlyout.DownloadItemsListView.Items.Insert(0, new DownloadItem(args.DownloadOperation));
        window.DownloadFlyout.ShowAt(window.DownBtn);

        args.Handled = true;
    }

    string SelectionText;
    private void CoreWebView2_ContextMenuRequested(CoreWebView2 sender, CoreWebView2ContextMenuRequestedEventArgs args)
    {
        var flyout1 = (Microsoft.UI.Xaml.Controls.CommandBarFlyout)Resources["Ctx"];
        OpenLinks.Visibility = Visibility.Collapsed;
        var flyout = FlyoutBase.GetAttachedFlyout(WebViewElement);

        var options = new FlyoutShowOptions()
        {
            Position = args.Location,
            ShowMode = FlyoutShowMode.Standard
        };

        if (args.ContextMenuTarget.Kind == CoreWebView2ContextMenuTargetKind.SelectedText)
        {
            SelectionText = args.ContextMenuTarget.SelectionText;
        }
        else if (args.ContextMenuTarget.HasLinkUri)
        {
            SelectionText = args.ContextMenuTarget.LinkUri;
            OpenLinks.Visibility = Visibility.Visible;
        }

        flyout = flyout ?? (Microsoft.UI.Xaml.Controls.CommandBarFlyout)Resources["Ctx"];
        flyout.ShowAt(WebViewElement, options);
        args.Handled = true;
    }


    private async void ContextMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is AppBarButton button && button.Tag != null)
        {
            switch ((sender as AppBarButton).Tag)
            {
                case "MenuBack":
                    if (WebViewElement.CanGoBack == true)
                    {
                        WebViewElement.CoreWebView2.GoBack();
                    }
                    break;
                case "Forward":
                    if (WebViewElement.CanGoForward == true)
                    {
                        WebViewElement.CoreWebView2.GoForward();
                    }
                    break;
                case "Source":
                    WebViewElement.CoreWebView2.OpenDevToolsWindow();
                    break;
                case "Select":
                    await WebViewElement.CoreWebView2.ExecuteScriptAsync("document.execCommand('selectAll', false, null);");
                    break;
                case "Copy":
                    ClipBoard.WriteStringToClipboard(SelectionText);
                    break;
                case "Taskmgr":
                    WebViewElement.CoreWebView2.OpenTaskManagerWindow();
                    break;
                case "Save":

                    break;
                case "Share":

                    break;
                case "Print":
                    WebViewElement.CoreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.Browser);
                    break;
            }
        }
        Ctx.Hide();
    }


    SpeechSynthesizer synthesizer = new SpeechSynthesizer();

    private async void ConvertTextToSpeech(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        var synthesisStream = await new SpeechSynthesizer().SynthesizeSsmlToStreamAsync(
            $"<speak version='1.0' xml:lang='{userSettings.Lang}'><voice name='Microsoft Server Speech Text to Speech Voice ({userSettings.Lang}, HannaRUS)'>{text}</voice></speak>");

        var mediaPlayer = new MediaPlayer
        {
            Source = MediaSource.CreateFromStream(synthesisStream, synthesisStream.ContentType)
        };

        mediaPlayer.MediaEnded += (_, args) => mediaPlayer.Dispose();

        mediaPlayer.Play();
    }

    public static async void OpenNewWindow(Uri uri) => await Windows.System.Launcher.LaunchUriAsync(uri);

    private void ContextClicked_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem button && button.Tag != null)
        {
            var mainWindow = (Application.Current as App)?.m_window as MainWindow;

            switch ((sender as MenuFlyoutItem).Tag)
            {
                case "Read":
                    ConvertTextToSpeech(SelectionText);
                    break;
                case "WebApp":
                    break;
                case "OpenInTab":
                    var newTab = mainWindow?.CreateNewTab(typeof(WebContent), new Uri(SelectionText));
                    if (userSettings.OpenTabHandel == "1")
                    {
                        mainWindow?.Tabs.TabItems.Add(newTab);
                        select();
                    }
                    else
                    {
                        mainWindow?.Tabs.TabItems.Add(newTab);
                    }
                    break;
                case "OpenInWindow":
                    OpenNewWindow(new Uri(SelectionText));
                    break;
            }
        }
        Ctx.Hide();
    }

    public void select() => ((Application.Current as App)?.m_window as MainWindow)?.SelectNewTab();

    private bool isOffline = false;
    private async void CheckNetworkStatus()
    {
        while (true)
        {
            bool isInternetAvailable = NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable;
            if (isInternetAvailable && isOffline)
            {
                WebViewElement.Reload();
                Grid.Visibility = Visibility.Visible;
                offlinePage.Visibility = Visibility.Collapsed;
                isOffline = false;
            }
            else if (!isInternetAvailable)
            {
                offlinePage.Visibility = Visibility.Visible;
                Grid.Visibility = Visibility.Collapsed;
                isOffline = true;
                await Task.Delay(1000);
            }
            await Task.Delay(1000);
        }
    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        if (Grid.Children.Count == 0) Grid.Children.Add(WebViewElement);
    }
}