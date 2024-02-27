using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using Windows.System;

namespace FireBrowserWinUi3Core.Helpers
{
    public class KeyDownListernerWeb
    {
        public class KeyDownListener
        {
            public class KeyDownPressedEventArgs
            {
                public KeyDownPressedEventArgs(bool isControlKeyPressed, bool isAltKeyPressed, VirtualKey pressedKey)
                {
                    IsControlKeyPressed = isControlKeyPressed;
                    IsAltKeyPressed = isAltKeyPressed;
                    PressedKey = pressedKey;
                }

                public bool IsControlKeyPressed { get; }
                public bool IsAltKeyPressed { get; }
                public VirtualKey PressedKey { get; }
            }

            public delegate void KeyDownPressedEventHandler(object sender, KeyDownPressedEventArgs args);
            public event KeyDownPressedEventHandler KeyDown;

            private UIElement sender;

            public KeyDownListener(UIElement element)
            {
                sender = element;
                Init(element);
            }

            public void AttachToElement(UIElement element)
            {
                sender = element;
                Init(element);
            }

            private async void Init(UIElement element)
            {
                if (element is WebView2 webView)
                {
                    await webView.EnsureCoreWebView2Async();
                    await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
                        "window.chrome.webview.addEventListener('message', function (event) { window.chrome.webview.postMessage('[LEFTCLICK]'); });");
                    webView.WebMessageReceived += WebView_WebMessageReceived;
                }
            }

            private void WebView_WebMessageReceived(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
            {
                string message = args.WebMessageAsJson.Replace("\"", "");

                if (message.StartsWith("[LEFTCLICK]"))
                {
                    KeyDown?.Invoke(this, new KeyDownPressedEventArgs(false, false, VirtualKey.LeftButton));
                }
            }
        }
    }
}
