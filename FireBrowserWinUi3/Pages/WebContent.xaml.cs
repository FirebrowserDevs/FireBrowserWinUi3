using CommunityToolkit.WinUI.Helpers;
using FireBrowserWinUi3.Controls;
using FireBrowserWinUi3.Services;
using FireBrowserWinUi3Core.CoreUi;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3Core.ShareHelper;
using FireBrowserWinUi3DataCore.Actions;
using FireBrowserWinUi3Exceptions;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.UI.WebUI;
using WinRT.Interop;
using static FireBrowserWinUi3.MainWindow;

namespace FireBrowserWinUi3.Pages;

public sealed partial class WebContent : Page
{
    Passer param;
    public static bool IsIncognitoModeEnabled { get; set; } = false;
    public BitmapImage PictureWebElement { get; set; }
    public WebView2 WebView { get; set; }
    private FireBrowserWinUi3MultiCore.User GetUser() => AuthService.IsUserAuthenticated ? AuthService.CurrentUser : null;
    SettingsService SettingsService { get; set; }

    public WebContent()
    {
        SettingsService = App.GetService<SettingsService>();
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

    public async Task AfterComplete()
    {
        if (!IsIncognitoModeEnabled)
        {
            await Task.Delay(500);
            var username = AuthService.CurrentUser;
            var source = WebViewElement.CoreWebView2.Source.ToString();
            var title = WebViewElement.CoreWebView2.DocumentTitle.ToString();

            var dbContext = new HistoryActions(username.Username);
            await dbContext.InsertHistoryItem(source, title, 0, 0, 0);

            var isSecure = source.Contains("https") ? "\uE72E" : (source.Contains("http") ? "\uE785" : "");
            param.ViewModel.SecurityIcon = isSecure;
            param.ViewModel.SecurityIcontext = isSecure == "\uE72E" ? "Https Secured Website" : (isSecure == "\uE785" ? "Http UnSecured Website" : "");
        }
        else
        {
            // Handle incognito mode scenario; do nothing here.
        }
    }

    public void LoadSettings()
    {
        if (SettingsService.CoreSettings.DisableJavaScript == true) { WebViewElement.CoreWebView2.Settings.IsScriptEnabled = false; } else { WebViewElement.CoreWebView2.Settings.IsScriptEnabled = true; }
        if (SettingsService.CoreSettings.DisablePassSave == true) { WebViewElement.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false; } else { WebViewElement.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true; }
        if (SettingsService.CoreSettings.DisableGenAutoFill == true) { WebViewElement.CoreWebView2.Settings.IsGeneralAutofillEnabled = false; } else { WebViewElement.CoreWebView2.Settings.IsGeneralAutofillEnabled = true; }
        if (SettingsService.CoreSettings.DisableWebMess == true) { WebViewElement.CoreWebView2.Settings.IsWebMessageEnabled = false; } else { WebViewElement.CoreWebView2.Settings.IsWebMessageEnabled = true; }
        if (SettingsService.CoreSettings.BrowserKeys == true) { WebViewElement.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true; } else { WebViewElement.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false; }
        if (SettingsService.CoreSettings.StatusBar == true) { WebViewElement.CoreWebView2.Settings.IsStatusBarEnabled = true; } else { WebViewElement.CoreWebView2.Settings.IsStatusBarEnabled = false; }
        if (SettingsService.CoreSettings.BrowserScripts == true) { WebViewElement.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true; } else { WebViewElement.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false; }
    }

    Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);

    public void ShareUi(string url, string title)
    {
        var hWnd = WindowNative.GetWindowHandle((Application.Current as App)?.m_window as MainWindow);
        ShareUIHelper.ShowShareUIURL(url, title, hWnd);
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        param = e.Parameter as Passer;

        await WebViewElement.EnsureCoreWebView2Async();

        LoadSettings();

        WebView2 s = WebViewElement;

        if (param?.Param != null) WebViewElement.CoreWebView2.Navigate(param.Param.ToString());

        string useragent = userSettings?.Useragent ?? "1";
        var userAgent = s?.CoreWebView2.Settings.UserAgent;

        if (!string.IsNullOrEmpty(userAgent) && userAgent.Contains("Edg/"))
        {
            s.CoreWebView2.Settings.UserAgent = userAgent.Substring(0, userAgent.IndexOf("Edg/"));
        }

        s.CoreWebView2.ContainsFullScreenElementChanged += (sender, args) =>
        {
            var window = (Application.Current as App)?.m_window as MainWindow;
            window.GoFullScreenWeb(s.CoreWebView2.ContainsFullScreenElement);
        };
        s.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
        s.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
        s.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
        s.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
        s.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;
        s.CoreWebView2.ScriptDialogOpening += async (sender, args) =>
        {
            args.GetDeferral();
            var window = (Application.Current as App)?.m_window as MainWindow;
            UIScript ui = new UIScript($"{sender.DocumentTitle} says", args.Message, window.Content.XamlRoot);
            var result = await ui.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                sender.Reload();
            }
        };
        s.CoreWebView2.DocumentTitleChanged += (sender, args) =>
        {
            if (!IsIncognitoModeEnabled) param.Tab.Header = WebViewElement.CoreWebView2.DocumentTitle;
        };
        s.CoreWebView2.PermissionRequested += (sender, args) =>
        {
            return;
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
                    await bitmapImage.SetSourceAsync(stream ?? await sender.GetFaviconAsync(CoreWebView2FaviconImageFormat.Jpeg));
                    param.Tab.IconSource = iconSource;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
            }
        };
        s.CoreWebView2.NavigationStarting += (sender, args) =>
        {
            ProgressLoading.IsIndeterminate = true;
            ProgressLoading.Visibility = Visibility.Visible;

            if ((TabViewItem)param.TabView.SelectedItem == param.Tab)
            {
                CheckNetworkStatus();
            }
        };
        s.CoreWebView2.HistoryChanged += async (sender, args) =>
        {
            if ((TabViewItem)param.TabView.SelectedItem == param.Tab) await AfterComplete();
        };
        s.CoreWebView2.NavigationCompleted += async (sender, args) =>
        {
            ProgressLoading.IsIndeterminate = false;
            ProgressLoading.Visibility = Visibility.Collapsed;
            //move history to history change event caputures all navigation now (thread safe)
            s?.DispatcherQueue.TryEnqueue(async () =>
            {
                await Task.Delay(1000);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    try
                    {
                        await s.CoreWebView2?.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Jpeg, memoryStream.AsRandomAccessStream());
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        BitmapImage bitmap = new BitmapImage { DecodePixelHeight = 512, DecodePixelWidth = 640 };
                        bitmap.SetSource(memoryStream.AsRandomAccessStream());
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        PictureWebElement = bitmap;

                        if ((Application.Current as App)?.m_window is MainWindow win &&
                            win.TabViewContainer.SelectedItem is FireBrowserTabViewItem tab &&
                            win.TabContent.Content is WebContent web)
                        {
                            tab.BitViewWebContent = web.PictureWebElement;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.LogException(ex);
                        Console.Write($"Error capturing preview of website:\n{ex.Message}");
                    }
                }
            });
        };
        s.CoreWebView2.SourceChanged += (sender, args) =>
        {
            if ((TabViewItem)param.TabView.SelectedItem == param.Tab) param.ViewModel.CurrentAddress = sender.Source;
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
        var mainWindow = (Application.Current as App)?.m_window as MainWindow;

        mainWindow.DownloadFlyout.DownloadItemsListView.Items.Insert(0, new DownloadItem(args.DownloadOperation));
        mainWindow.DownloadFlyout.ShowAt(mainWindow.DownBtn);

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
                    if (WebViewElement.CanGoBack) WebViewElement.CoreWebView2.GoBack();
                    break;
                case "Forward":
                    if (WebViewElement.CanGoForward) WebViewElement.CoreWebView2.GoForward();
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
                    ShareUi(WebViewElement.CoreWebView2.DocumentTitle, WebViewElement.CoreWebView2.Source);
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
                    if (IsIncognitoModeEnabled)
                    {
                        var newTab = mainWindow?.CreateNewIncog(typeof(WebContent), new Uri(SelectionText));
                        mainWindow?.Tabs.TabItems.Add(newTab);
                    }
                    else
                    {
                        var newTab = mainWindow?.CreateNewTab(typeof(WebContent), new Uri(SelectionText));
                        mainWindow?.Tabs.TabItems.Add(newTab);
                    }
                    if (userSettings.OpenTabHandel) select();
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
}