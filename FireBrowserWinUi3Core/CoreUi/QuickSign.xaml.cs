using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;

namespace FireBrowserWinUi3Core.CoreUi;
public sealed partial class QuickSign : Window
{

    public QuickSign(string site)
    {
        this.InitializeComponent();
        LoadWeb();
        NavigateToSite(site);
    }


    public async void LoadWeb()
    {
        await webView.EnsureCoreWebView2Async(null);
    }

    private void NavigateToSite(string site)
    {
        switch (site.ToLower())
        {
            case "microsoft":
                webView.Source = new Uri("https://login.microsoftonline.com/");
                break;
            case "google":
                webView.Source = new Uri("https://accounts.google.com/");
                break;
            default:
                webView.Source = new Uri("about:blank");
                break;
        }
    }
}