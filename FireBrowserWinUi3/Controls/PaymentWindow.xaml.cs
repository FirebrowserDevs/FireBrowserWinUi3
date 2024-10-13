using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using Windows.Graphics;
using WinRT.Interop;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Controls
{
    public sealed partial class PaymentWindow : Window
    {
        private string premiumLicensePath;
        private AppWindow appWindow;
        private AppWindowTitleBar titleBar;

        public PaymentWindow()
        {
            this.InitializeComponent();
            PaymentWebView.EnsureCoreWebView2Async();
            premiumLicensePath = Path.Combine(AppContext.BaseDirectory, "premium.license"); // Path for license file
            InitializeWindow();
        }

        private void InitializeWindow()
        {
            var hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.MoveAndResize(new RectInt32(500, 500, 1675, 1025));
            FireBrowserWinUi3Core.Helpers.Windowing.Center(this);
            appWindow.SetPresenter(AppWindowPresenterKind.Default);
            appWindow.MoveInZOrderAtTop();
            appWindow.ShowOnceWithRequestedStartupState();
            appWindow.SetIcon("buy.ico");
            AppWindow.Title = "Buy Premium";

            if (!AppWindowTitleBar.IsCustomizationSupported())
            {
                throw new Exception("Unsupported OS version.");
            }
            else
            {
                titleBar = appWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
                var btnColor = Colors.Transparent;
                titleBar.BackgroundColor = btnColor;
                titleBar.ButtonBackgroundColor = btnColor;
                titleBar.InactiveBackgroundColor = btnColor;
                titleBar.ButtonInactiveBackgroundColor = btnColor;
            }
        }

        private async void PaymentWebView_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            var currentUrl = sender.Source.ToString();

            // Inject JavaScript to check page content
            string pageContent = await sender.CoreWebView2.ExecuteScriptAsync("document.body.innerText");

            if (pageContent.Contains("Thanks for your payment", StringComparison.OrdinalIgnoreCase))
            {
                // Delay for a second asynchronously before unlocking premium
                await Task.Delay(1000);

                // Payment was successful
                UnlockPremium();
                Debug.WriteLine("License creation successful. Premium unlocked.");

                this.Close(); // Close the window after success
            }
            else if (currentUrl.Contains("payment-failure"))
            {
                // Handle payment failure
                Debug.WriteLine("Payment failed. License creation aborted.");
                this.Close(); // Close the window on failure
            }
        }

        private void UnlockPremium()
        {
            // Create a premium license file to unlock premium status
            using (FileStream fs = File.Create(premiumLicensePath))
            {
                Debug.WriteLine($"License file created at {premiumLicensePath}");
            }
        }
    }
}
