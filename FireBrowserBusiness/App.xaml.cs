﻿using FireBrowserMultiCore;
using FireBrowserWinUi3.Setup;
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
using System.Diagnostics;
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

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 
        public App()
        {
            this.InitializeComponent();
            CreateUserOnStartup();
        }


        private void CreateUserOnStartup()
        {
            // Create a new user object with a unique username.
            User newUser = new User
            {
                Username = "NewUser" + "." // Generate a unique username.                                                              // Add other user properties as needed.
            };

            // Create a list of users and add the new user to it.
            List<User> users = new List<User>();
            users.Add(newUser);

            // Create the user folders.
            UserFolderManager.CreateUserFolders(newUser);

            // Save the list of users to the JSON file.
            UserDataManager.SaveUsers(users);
            // Authenticate the new user (if needed).
            AuthService.Authenticate(newUser.Username);
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            if (!Directory.Exists(UserDataManager.CoreFolderPath))
            {
               
            }
            else
            {
                // The "FireBrowserUserCore" folder exists, so proceed with your application's normal behavior.
                m_window = new MainWindow();
                m_window.Activate();
            }

        }


        private Window m_window;
    }
}