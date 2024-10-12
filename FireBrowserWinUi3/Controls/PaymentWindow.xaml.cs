using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using Windows.Graphics;
using WinRT.Interop;

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

            appWindow.MoveAndResize(new RectInt32(500, 500, 1650, 1000));
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

            if (currentUrl.Contains("payment-success"))
            {
                // Payment was successful
                UnlockPremium();
                this.Close(); // Close the window after success
                await new ContentDialog
                {
                    Title = "Payment Successful",
                    Content = "Premium features unlocked. You can now create up to 50 backups.",
                    CloseButtonText = "OK"
                }.ShowAsync();
            }
            else if (currentUrl.Contains("payment-failure"))
            {
                // Handle payment failure
                this.Close(); // Close the window on failure
                await new ContentDialog
                {
                    Title = "Payment Failed",
                    Content = "Payment could not be processed. Please try again.",
                    CloseButtonText = "OK"
                }.ShowAsync();
            }
        }

        private void UnlockPremium()
        {
            // Create a premium license file to unlock premium status
            using (FileStream fs = File.Create(premiumLicensePath)) ;
        }
    }
}
