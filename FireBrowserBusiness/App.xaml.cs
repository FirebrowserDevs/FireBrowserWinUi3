using FireBrowserMultiCore;
using FireBrowserWinUi3;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.ViewManagement;
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

            string coreFolderPath = UserDataManager.CoreFolderPath;
            string username = GetUsernameFromCoreFolderPath(coreFolderPath);

            if (username != null)
                AuthService.Authenticate(username);

            UrlHelperWinUi3.TLD.LoadKnownDomainsAsync();
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




        public static string Args { get; set; }
        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {

            if (!Directory.Exists(UserDataManager.CoreFolderPath))
            {
                // The "FireBrowserUserCore" folder does not exist, so proceed with  application's setup behavior.
                m_window = new SetupWindow();
                m_window.Activate();
            }
            else
            {
                var evt = AppInstance.GetActivatedEventArgs();
                ProtocolActivatedEventArgs protocolArgs = evt as ProtocolActivatedEventArgs;

                if (protocolArgs != null)
                {
                    try
                    {
                        Args = protocolArgs.Uri.ToString();
                    }
                    catch { }
                }

            

                // The "FireBrowserUserCore" folder exists, so proceed with your application's normal behavior.
                m_window = new MainWindow();
                m_window.Activate();
            }
        }


        public Window m_window;
    }
}
