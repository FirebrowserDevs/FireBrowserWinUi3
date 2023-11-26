using Microsoft.UI.Xaml.Controls;
using System;

namespace FireBrowserMultiCore;

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
            return;
        }
    }

    private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        Hide();
    }
}