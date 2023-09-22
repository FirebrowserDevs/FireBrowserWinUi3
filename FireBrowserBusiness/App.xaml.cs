using FireBrowserMultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public User CurrentUser { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 
        public App()
        {
            this.InitializeComponent();

        }

        public async Task<string> LoadUserAsync()
        {
            try
            {
                // Simulate a delay for loading users (replace this with your actual loading code)
                await Task.Delay(1000);

                // Load user information from users.json
                Sys multiuserSystem = new Sys();
                multiuserSystem.LoadUsers();

                // Define the username to use as the current user
                string desiredUsername = "Test";

                // Find the user with the desired username
                User user = multiuserSystem.Users.FirstOrDefault(u => u.Username == desiredUsername);

                if (user != null)
                {
                    // Set the current user based on the desiredUsername
                    CurrentUser = user;

                    // Return the username
                    return CurrentUser.Username;
                }
                else
                {
                    return "no user";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user: {ex.Message}");
                return "Error loading user";
            }
        }


     
        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
         
            m_window = new MainWindow();
            m_window.Activate();
        }

       
        private Window m_window;
    }
}
