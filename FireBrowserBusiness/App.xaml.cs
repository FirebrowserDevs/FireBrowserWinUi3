using FireBrowserBusinessCore.Helpers;
using FireBrowserMultiCore;
using FireBrowserWinUi3;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Path = System.IO.Path;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserBusiness
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 
        public App()
        {
            this.InitializeComponent();

            UrlHelperWinUi3.TLD.LoadKnownDomainsAsync();

            System.Environment.SetEnvironmentVariable("WEBVIEW2_USE_VISUAL_HOSTING_FOR_OWNED_WINDOWS", "1");
        }

        public static string GetUsernameFromCoreFolderPath(string coreFolderPath)
        {
            try
            {
                string usrCoreFilePath = Path.Combine(coreFolderPath, "UsrCore.json");

                // Check if UsrCore.json exists
                if (File.Exists(usrCoreFilePath))
                {
                    // Read the JSON content from UsrCore.json
                    string jsonContent = File.ReadAllText(usrCoreFilePath);

                    // Deserialize the JSON content into a list of user objects
                    var users = JsonSerializer.Deserialize<List<FireBrowserMultiCore.User>>(jsonContent);

                    if (users != null && users.Count > 0 && !string.IsNullOrWhiteSpace(users[0].Username))
                    {
                        return users[0].Username; // Assuming you want the first user's username
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during file reading or deserialization
                Console.WriteLine("Error reading UsrCore.json: " + ex.Message);
            }

            // Return null or an empty string if the username couldn't be retrieved
            return null;
        }

        public void checknormal()
        {
            string coreFolderPath = UserDataManager.CoreFolderPath;
            string username = GetUsernameFromCoreFolderPath(coreFolderPath);

            if (username != null)
                AuthService.Authenticate(username);
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            if (!Directory.Exists(UserDataManager.CoreFolderPath))
            {
                // The "FireBrowserUserCore" folder does not exist, so proceed with application setup behavior.
                m_window = new SetupWindow();
            }
            else
            {
                var evt = AppInstance.GetActivatedEventArgs();
                ProtocolActivatedEventArgs protocolArgs = evt as ProtocolActivatedEventArgs;

                if (protocolArgs != null && protocolArgs.Kind == ActivationKind.Protocol)
                {
                    string url = protocolArgs.Uri.ToString();

                    if (url.StartsWith("http") || url.StartsWith("https"))
                    {
                        AppArguments.UrlArgument = url; // Standard web URL
                        checknormal();
                    }
                    else if (url.StartsWith("firebrowserwinui://"))
                    {
                        AppArguments.FireBrowserArgument = url;
                        checknormal();
                    }
                    else if (url.StartsWith("firebrowseruser://"))
                    {
                        AppArguments.FireUser = url;

                        // Extract the username after 'firebrowserwinuifireuser://'
                        string usernameSegment = url.Replace("firebrowseruser://", ""); // Remove the prefix
                        string[] urlParts = usernameSegment.Split('/', StringSplitOptions.RemoveEmptyEntries);
                        string username = urlParts.FirstOrDefault(); // Retrieve the first segment as the username

                        // Authenticate the extracted username using your authentication service
                        if (!string.IsNullOrEmpty(username))
                        {
                            AuthService.Authenticate(username);
                        }

                        // No need to activate the window here
                    }
                    else if (url.StartsWith("firebrowserincog://"))
                    {
                        AppArguments.FireBrowserIncog = url;
                        checknormal(); // Custom protocol for FireBrowser
                    }
                    else if (url.Contains(".pdf"))
                    {
                        AppArguments.FireBrowserPdf = url;
                        checknormal();
                    }
                }
                else
                {
                    checknormal();
                }

                // Activate the window after evaluating the URL and handling respective cases
                m_window = new MainWindow();
            }

            // Activate the window outside of conditional blocks
            m_window.Activate();
        }

        public Window m_window;
    }
}
