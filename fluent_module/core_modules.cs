using Microsoft.Web.WebView2.Core;
using System;

//this module is just being developed and is not complete, there are emmense bugs and af of now most
//of the code isent being used in the main process yet as of right now its in the testing phase.

namespace fluent_module
{
    public class core_modules
    {
        private CoreWebView2 webView;

        public core_modules()
        {
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            // Initialize the WebView2 control
            var webViewEnvironment = await CoreWebView2Environment.CreateAsync();
            //webView.Settings.IsHardwareAccelerationEnabled = true;

            webView.Settings.AreDefaultContextMenusEnabled = false;

        }
    }
}
