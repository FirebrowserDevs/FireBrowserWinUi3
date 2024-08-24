using FireBrowserWinUi3AdBlockCore.AdBlocker;
using FireBrowserWinUi3MultiCore;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBrowserWinUi3AdBlockCore
{
    public class AdBlockCore // Instalnt and only compatible with host-based lists
    {
        private static string[] easylist;
        private static string[] easyprivacy;
        private static string[] malwaresList; // Spam404's main host blocklist
        private static string[] whitelist; // By me

        public static void Init()
        {
            if (easylist == null)
            {
                string assetsFile = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "FireBrowserWinUi3AdBlockCore.AdBlocker", "Assets");

                string easylistPath = Path.Combine(assetsFile, "Easylist.txt");
                string easyPrivacyPath = Path.Combine(assetsFile, "Easyprivacy.txt");
                string malwaresListPath = Path.Combine(assetsFile, "MalwaresList.txt");
                string whitelistListPath = Path.Combine(assetsFile, "Whitelist.txt");

                easylist = File.ReadAllLines(easylistPath);
                easyprivacy = File.ReadAllLines(easyPrivacyPath);
                malwaresList = File.ReadAllLines(malwaresListPath);
                whitelist = File.ReadAllLines(whitelistListPath);
            }

            if (FireBrowserWinUi3AdBlockCore.AdBlocker.EasyListAdBlocker.AdBlockList.Count() == 0 && AuthService.CurrentUser.UserSettings.AdBlockerType == 1)
            {
                FireBrowserWinUi3AdBlockCore.AdBlocker.EasyListAdBlocker.CreateRegexBasedFilterList();
            }
        }


        public AdBlockCore(CoreWebView2 coreWebView)
        {
            coreWebView.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);

            coreWebView.WebResourceRequested += CoreWebView_WebResourceRequested;
        }

        private async void CoreWebView_WebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs args)
        {
            if (AuthService.CurrentUser.UserSettings.IsAdBlockerEnabled)
            {
                if (AuthService.CurrentUser.UserSettings.AdBlockerType == 0) // host-based
                {
                    string host = new Uri(args.Request.Uri).Host;

                    if (ShouldBlockHost(host, sender))
                    {
                        sender.Stop(); // to have the time to block the request
                        args.Response = sender.Environment.CreateWebResourceResponse(null, 503, "Service Unavailable", "");
                        sender.Resume();
                    }
                }
                else
                {
                    if (await ShouldBlock(args.Request.Uri, args.ResourceContext, new Uri(sender.Source).Host))
                    {
                        sender.Stop(); // to have the time to block the request
                        args.Response = sender.Environment.CreateWebResourceResponse(null, 503, "Service Unavailable", "");
                        sender.Resume();
                    }
                }
            }
        }

        private async Task<bool> ShouldBlock(string request, CoreWebView2WebResourceContext context, string host)
        {

            return await EasyListAdBlocker.ShouldBlock(request, host, context);
        }

        private bool ShouldBlockHost(string host, CoreWebView2 sender)
        {
            if (!whitelist.Any(p => p == new Uri(sender.Source).Host))
            {
                if (easylist.Any(p => p == host)) return true;

                if (easyprivacy.Any(p => p == host)) return true;

                if (malwaresList.Any(p => p == host)) return true;
            }


            return false;
        }
    }
}
