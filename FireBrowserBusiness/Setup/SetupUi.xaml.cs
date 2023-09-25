using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using FireBrowserMultiCore;
using System.Diagnostics;
using System.Text.Json;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupUi : Page
    {

        public SetupUi()
        {
            this.InitializeComponent();
        }

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

      
        private void SetupUiBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SetupAlgemeen));
        }

        private void SearchengineSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string selection = e.AddedItems[0].ToString();
                string url;

                switch (selection)
                {
                    case "Ask":
                        url = "https://www.ask.com/web?q=";
                        break;
                    case "Baidu":
                        url = "https://www.baidu.com/s?ie=utf-8&f=8&rsv_bp=1&rsv_idx=1&tn=baidu&wd=";
                        break;
                    case "Bing":
                        url = "https://www.bing.com?q=";
                        break;
                    case "DuckDuckGo":
                        url = "https://www.duckduckgo.com?q=";
                        break;
                    case "Ecosia":
                        url = "https://www.ecosia.org/search?q=";
                        break;
                    case "Google":
                        url = "https://www.google.com/search?q=";
                        break;
                    case "Startpage":
                        url = "https://www.startpage.com/search?q=";
                        break;
                    case "Qwant":
                        url = "https://www.qwant.com/?q=";
                        break;
                    case "Qwant Lite":
                        url = "https://lite.qwant.com/?q=";
                        break;
                    case "Yahoo!":
                        url = "https://search.yahoo.com/search?p=";
                        break;
                    case "Presearch":
                        url = "https://presearch.com/search?q=";
                        break;
                    // Add other cases for different search engines.
                    default:
                        // Handle the case when selection doesn't match any of the predefined options.
                        url = "https://www.google.com/search?q=";
                        break;
                }

                if (!string.IsNullOrEmpty(url))
                {
                    // Load the user's settings
                    Settings userSettings = UserFolderManager.LoadUserSettings(GetUser());

                    // Modify the settings based on user input
                    userSettings.EngineFriendlyName = selection;
                    userSettings.SearchUrl = url;



                    // Save the modified settings back to the user's settings file
                    UserFolderManager.SaveUserSettings(GetUser(), userSettings);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

        }
    }
}
