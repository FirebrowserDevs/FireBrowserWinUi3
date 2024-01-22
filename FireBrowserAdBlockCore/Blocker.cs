using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserAdBlockCore;

public class Blocker
{
    public static List<string> Blocklist { get; set; } = null;
    public static List<string> WhiteList { get; set; } = null;

    private WebView2 _webView;
    private bool _retryWithoutBlocker = false; // To prevent pages like outlook from not being loaded

    private bool _isLoaded = false;
    public bool IsEnabled { get; private set; } = false;
    public Blocker(WebView2 webView2)
    {
        _webView = webView2;
        _webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Script);
        _webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.XmlHttpRequest);
    }

    private async Task Init()
    {
        try
        {
            if (Blocklist == null)
            {
                string generalFilename = @"Assets\general.txt";
                string whitelistFilename = @"Assets\whitelist.txt";

                StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;

                StorageFile generalFile = await installationFolder.GetFileAsync(generalFilename);
                StorageFile whitelistFile = await installationFolder.GetFileAsync(whitelistFilename);

                string generalListString = File.ReadAllText(generalFile.Path);
                string whitelistListString = File.ReadAllText(whitelistFile.Path);

                Blocklist = generalListString.Split("\n").ToList();

                WhiteList = whitelistListString.Split("\n").ToList();
            }
        }
        catch { }


    }

    public async void Enable()
    {
        IsEnabled = true;

        if (!_isLoaded) { await Init(); _isLoaded = true; }
        _webView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
        _webView.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
        _webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;

        // Youtube ad-blocking
        await _webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("const clear = (() => {\r\n    const defined = v => v !== null && v !== undefined;\r\n    const timeout = setInterval(() => {\r\n        const ad = [...document.querySelectorAll('.ad-showing')][0];\r\n        if (defined(ad)) {\r\n            const video = document.querySelector('video');\r\n            if (defined(video)) {\r\n                video.currentTime = video.duration;\r\n            }\r\n        }\r\n    }, 500);\r\n    return function() {\r\n        clearTimeout(timeout);\r\n    }\r\n})();");
        await _webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("(function() {\r\n    // Select all elements with the class \"video-ads\" and hide them\r\n    var ads = document.querySelectorAll('.video-ads');\r\n    for (var i = 0; i < ads.length; i++) {\r\n        ads[i].style.display = 'none';\r\n    }\r\n\r\n    // Override the playVideo function to skip ads\r\n    var originalPlayVideo = window.YT.Player.prototype.playVideo;\r\n    window.YT.Player.prototype.playVideo = function() {\r\n        // Check if the video is an ad\r\n        if (this.getVideoData().title === 'Advertisement') {\r\n            // If it's an ad, skip to the next video\r\n            this.nextVideo();\r\n        } else {\r\n            // If it's not an ad, play the video\r\n            originalPlayVideo.apply(this, arguments);\r\n        }\r\n    }\r\n})();\r\n");
    }


    public void Disable()
    {
        IsEnabled = false;

        _webView.CoreWebView2.WebResourceRequested -= CoreWebView2_WebResourceRequested;
        _webView.CoreWebView2.DOMContentLoaded -= CoreWebView2_DOMContentLoaded;
        _webView.CoreWebView2.NavigationCompleted -= CoreWebView2_NavigationCompleted;

        _webView.CoreWebView2.RemoveScriptToExecuteOnDocumentCreated("const clear = (() => {\r\n    const defined = v => v !== null && v !== undefined;\r\n    const timeout = setInterval(() => {\r\n        const ad = [...document.querySelectorAll('.ad-showing')][0];\r\n        if (defined(ad)) {\r\n            const video = document.querySelector('video');\r\n            if (defined(video)) {\r\n                video.currentTime = video.duration;\r\n            }\r\n        }\r\n    }, 500);\r\n    return function() {\r\n        clearTimeout(timeout);\r\n    }\r\n})();");
        _webView.CoreWebView2.RemoveScriptToExecuteOnDocumentCreated("(function() {\r\n    // Select all elements with the class \"video-ads\" and hide them\r\n    var ads = document.querySelectorAll('.video-ads');\r\n    for (var i = 0; i < ads.length; i++) {\r\n        ads[i].style.display = 'none';\r\n    }\r\n\r\n    // Override the playVideo function to skip ads\r\n    var originalPlayVideo = window.YT.Player.prototype.playVideo;\r\n    window.YT.Player.prototype.playVideo = function() {\r\n        // Check if the video is an ad\r\n        if (this.getVideoData().title === 'Advertisement') {\r\n            // If it's an ad, skip to the next video\r\n            this.nextVideo();\r\n        } else {\r\n            // If it's not an ad, play the video\r\n            originalPlayVideo.apply(this, arguments);\r\n        }\r\n    }\r\n})();\r\n");
    }

    private async void CoreWebView2_NavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        string html = await sender.ExecuteScriptAsync("document.documentElement.outerHTML;");
        if (!_retryWithoutBlocker && !html.Contains("<body/>"))
        {
            _retryWithoutBlocker = true;
            _webView.Reload();
        }
        _retryWithoutBlocker = false;
    }


    private void CoreWebView2_WebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs args)
    {
        if (!_retryWithoutBlocker)
        {
            string request = new Uri(args.Request.Uri).Host;

            // Block any tracker/ad-related request (if the page isn't translated as it will cause issues)
            if (Blocklist.Contains(request) & !WhiteList.Contains(request) & !_webView.Source.Host.Contains("translate.goog"))
            {
                sender.Stop();
                args.Response = sender.Environment.CreateWebResourceResponse(null, 503, "Service Unavailable", "");
                sender.Resume();
            }
        }
    }

    private async void CoreWebView2_DOMContentLoaded(CoreWebView2 sender, CoreWebView2DOMContentLoadedEventArgs args)
    {
        // Youtube video ad-blocking
        if (_webView.Source.Host.Contains("www.youtube"))
            await sender.ExecuteScriptAsync("const skip_add = (clazz) => \r\n {\r\n  const buttons = document.getElementsByClassName(clazz);\r\n  for (const button of buttons) \r\n    {\r\n     button.click();\r\n     console.log(\"No More Ad\");\r\n    }\r\n }\r\n\r\n setInterval(() => \r\n {\r\n  skip_add(\"ytp-ad-skip-button-text\");\r\n  skip_add(\"ytp-ad-overlay-close-button\");\r\n }, 1000);");
    }
}