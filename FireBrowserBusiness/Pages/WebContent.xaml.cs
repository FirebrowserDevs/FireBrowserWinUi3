using CommunityToolkit.WinUI.Helpers;
using FireBrowserBusinessCore.Helpers;
using FireBrowserMultiCore;
using FireBrowserWinUi3.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.Threading.Tasks;
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
        public WebContent()
        {
            this.InitializeComponent();
        }

        Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            param = e.Parameter as Passer;
            await WebViewElement.EnsureCoreWebView2Async();

            //LoadSettings();
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

            s.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            s.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            s.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;
            s.CoreWebView2.ScriptDialogOpening += async (sender, args) =>
            {

            };
            s.CoreWebView2.DocumentTitleChanged += (sender, args) =>
            {
                param.Tab.Header = WebViewElement.CoreWebView2.DocumentTitle;
            };
            s.CoreWebView2.PermissionRequested += (sender, args) =>
            {
                try
                {
                    return;
                }
                catch (Exception ex)
                {

                }
            };
            s.CoreWebView2.FaviconChanged += async (sender, args) =>
            {


                try
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
                catch
                {

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


                s.CoreWebView2.ContainsFullScreenElementChanged += (sender, args) =>
                {
                    FullSys sys = new();
                    sys.FullScreen = s.CoreWebView2.ContainsFullScreenElement;
                };

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

                        break;
                    case "WebApp":

                        break;
                    case "OpenInTab":

                        break;
                    case "OpenInWindow":
                        OpenNewWindow(new Uri(SelectionText));
                        break;
                }
            }
            Ctx.Hide();
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
