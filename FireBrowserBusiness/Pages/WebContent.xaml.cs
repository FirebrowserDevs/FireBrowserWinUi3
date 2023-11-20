using CommunityToolkit.WinUI.Helpers;
using FireBrowserBusiness;
using FireBrowserBusinessCore.Helpers;
using FireBrowserDatabase;
using FireBrowserMultiCore;
using FireBrowserWinUi3.Controls;
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
using static FireBrowserBusiness.MainWindow;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebContent : Page
    {
        Passer param;
        public static bool IsIncognitoModeEnabled { get; set; } = false;
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
        public WebContent()
        {
            this.InitializeComponent();
            Init();
        }

        public async Task Init()
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            param = e.Parameter as Passer;
            await WebViewElement.EnsureCoreWebView2Async();

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

            s.CoreWebView2.ContainsFullScreenElementChanged += (sender, args) =>
            {
                var window = (Application.Current as App)?.m_window as MainWindow;
                window.GoFullScreenWeb(s.CoreWebView2.ContainsFullScreenElement);
            };
            s.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            s.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;

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

                        if (stream != null)
                        {
                            await bitmapImage.SetSourceAsync(stream);
                            param.Tab.IconSource = new ImageIconSource { ImageSource = bitmapImage };
                        }
                        else
                        {
                            var bitmapImage2 = new BitmapImage();
                            await bitmapImage2.SetSourceAsync(await sender.GetFaviconAsync(CoreWebView2FaviconImageFormat.Jpeg));
                            param.Tab.IconSource = new ImageIconSource { ImageSource = bitmapImage2 };
                        }
                    }
                    else
                    {
                        // Handle incognito mode scenario
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

                CheckNetworkStatus();
            };
            s.CoreWebView2.NavigationCompleted += (sender, args) =>
            {
                Progress.IsIndeterminate = false;
                Progress.Visibility = Visibility.Collapsed;


                AfterComplete();
                CheckNetworkStatus();
            };
            s.CoreWebView2.SourceChanged += (sender, args) =>
            {
                if (param.TabView.SelectedItem == param.Tab)
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
            if (!string.IsNullOrWhiteSpace(text))
            {
                var synthesisStream = await synthesizer.SynthesizeSsmlToStreamAsync(
                    $"<speak version='1.0' xml:lang='{userSettings.Lang}'><voice name='Microsoft Server Speech Text to Speech Voice ({userSettings.Lang}, HannaRUS)'>{text}</voice></speak>");

                // Create a MediaPlayer element
                MediaPlayer mediaPlayer = new MediaPlayer();

                // Set the audio stream source
                mediaPlayer.Source = MediaSource.CreateFromStream(synthesisStream, synthesisStream.ContentType);

                // Subscribe to the MediaEnded event to clean up resources when playback completes
                mediaPlayer.MediaEnded += (sender, args) =>
                {
                    mediaPlayer.Dispose(); // Dispose of the MediaPlayer after playback is complete
                };

                // Start playback
                mediaPlayer.Play();
            }
        }

        public static async void OpenNewWindow(Uri uri)
        {
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private void ContextClicked_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem button && button.Tag != null)
            {
                switch ((sender as MenuFlyoutItem).Tag)
                {
                    case "Read":
                        ConvertTextToSpeech(SelectionText);
                        break;
                    case "WebApp":

                        break;
                    case "OpenInTab":
                        if (userSettings.OpenTabHandel == "1")
                        {
                            var window = (Application.Current as App)?.m_window as MainWindow;
                            window.Tabs.TabItems.Add(window.CreateNewTab(typeof(WebContent), new Uri(SelectionText)));
                            select();
                        }
                        else
                        {
                            var window = (Application.Current as App)?.m_window as MainWindow;
                            window.Tabs.TabItems.Add(window.CreateNewTab(typeof(WebContent), new Uri(SelectionText)));
                        }


                        break;
                    case "OpenInWindow":
                        OpenNewWindow(new Uri(SelectionText));
                        break;
                }

            }
            Ctx.Hide();
        }

        public void select()
        {
            var window = (Application.Current as App)?.m_window as MainWindow;
            window.SelectNewTab();
        }


        private bool isOffline = false;
        private async void CheckNetworkStatus()
        {
            while (true)
            {
                if (NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
                {
                    if (isOffline)
                    {
                        // Internet is back online
                        WebViewElement.Reload();
                        Grid.Visibility = Visibility.Visible;
                        offlinePage.Visibility = Visibility.Collapsed;
                        isOffline = false;

                    }
                }
                else
                {
                    // Internet is offline
                    offlinePage.Visibility = Visibility.Visible;
                    Grid.Visibility = Visibility.Collapsed;
                    isOffline = true;

                    // Wait for a second before checking again
                    await Task.Delay(1000);
                }

                // Wait for half a second before the next check
                await Task.Delay(500);
            }
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (Grid.Children.Count == 0) Grid.Children.Add(WebViewElement);
        }
    }
}