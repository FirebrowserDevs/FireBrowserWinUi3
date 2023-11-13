using FireBrowserMultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Controls
{
    public sealed partial class AddUser : ContentDialog
    {
        public AddUser()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string enteredUsername = Userbox.Text;

            // Check if the username is not empty
            if (!string.IsNullOrWhiteSpace(enteredUsername))
            {
                // Create a new user
                User newUser = new User
                {
                    Id = Guid.NewGuid(), // Generate a new GUID for the user Id
                    Username = enteredUsername,
                    IsFirstLaunch = false,
                    UserSettings = null // You might want to initialize UserSettings based on your application logic
                };

                // Add the new user to your user collection or perform any other necessary logic
                // For demonstration purposes, let's assume 'users' is a List<User> in your AuthService
                AuthService.AddUser(newUser);
                UserFolderManager.CreateUserFolders(newUser);

                // Close the ContentDialog
                Hide();
            }
            else
            {
                // Show an error message or handle empty username case
                // For example, display a TextBlock with an error message
              
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }
    }
}
