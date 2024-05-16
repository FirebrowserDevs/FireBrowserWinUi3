using Microsoft.Web.WebView2.Core;
using System;
using System.Linq.Expressions;

namespace fluent_module
{
    public class core_modules
    {
        private CoreWebView2 webView;
        private object completedUrl;

        public core_modules(CoreWebView2 webView)
        {
            this.webView = webView;
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            // Initialize the WebView2 control
            var webViewEnvironment = await CoreWebView2Environment.CreateAsync();

            // Collect performance logs
            // TODO: Enable performance Logging to give a better idea of any 
            // potential issues that may accure when preforming tasks
            // webView.Settings.IsWebPerformanceLoggingEnabled = true;
            webView.Settings.AreDevToolsEnabled = true; // Enable developer tools for more insights

            // Enable icon caching
            webView.Settings.AreDefaultContextMenusEnabled = false;
            // TODO: enable caching for app icons and extension icons
            // webView.Settings.IsIconCachingEnabled = true;

            // Error handling
            // TODO: webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            // TODO  webView.CoreWebView2InitializationCompleted += WebView_ErrorOccurred;

            // Navigation events
            webView.NavigationStarting += WebView_NavigationStarting;
            webView.NavigationCompleted += WebView_NavigationCompleted;

            // Resource loading events
            webView.WebResourceRequested += WebView_WebResourceRequested;
        //TODO:    webView.WebResourceCompleted += WebView_WebResourceCompleted;
        }

        private void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                Console.WriteLine("WebView2 initialization completed successfully.");
            }
            else
            {
                // TODO: catch if theres an errer
            }
        }

        private void WebView_ErrorOccurred(object sender, CoreWebView2ErrorEventArgs e)
        {
            // Log the error details
            Console.WriteLine($"An error occurred: {e.ErrorMessage}");
        }

        private void WebView_ConsoleMessageReceived(object sender, CoreWebView2ConsoleMessageReceivedEventArgs e)
        {
            // Log console messages
            Console.WriteLine($"Console message: {e.Message}");
        }

        private void WebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // Log the URL being navigated to
            Console.WriteLine($"Navigating to URL: {e.Uri}");
        }

        private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // Log navigation completion
            Console.WriteLine($"Navigation completed with result: {e.IsSuccess}");
        }

        private void WebView_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            // Get the request details
            var request = e.Request;

            // Log the URL of the requested resource
            string requestedUrl = request.Uri;
            Console.WriteLine($"Requested URL: {requestedUrl}");

            // Log the time when the request was made
            DateTime requestTime = DateTime.Now;
            Console.WriteLine($"Request Time: {requestTime}");
        }

        private void WebView_WebResourceCompleted(object sender, CoreWebView2WebResourceCompletedEventArgs e)
        {
            // Get the response details
            var response = e.Response;

            // TODO: Log the URL of the completed resource
            // string completedUrl = response.Uri;
            Console.WriteLine($"Resource completed: {completedUrl}");

            // Log the completion status (success or failure)
            bool isSuccess = e.IsSuccess;
            Console.WriteLine($"Resource loading {(isSuccess ? "succeeded" : "failed")}");
        }
    }
}
