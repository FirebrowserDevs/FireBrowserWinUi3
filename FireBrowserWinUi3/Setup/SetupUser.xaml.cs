using FireBrowserWinUi3Assets;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserWinUi3;

public sealed partial class SetupUser : Page
{
    public SetupUser()
    {
        this.InitializeComponent();
    }

    private void ProfileImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProfileImage.SelectedItem != null)
        {
            // Assuming 'userImageName' contains the name of the user's image file
            string userImageName = ProfileImage.SelectedItem.ToString() + ".png"; // Replace this with the actual user's image name

            iImage = userImageName;
            // Instantiate ImageLoader
            ImageLoader imgLoader = new ImageLoader();

            // Use the LoadImage method to get the image
            var userProfilePicture = imgLoader.LoadImage(userImageName);

            // Assign the retrieved image to the ProfileImageControl
            Pimg.ProfilePicture = userProfilePicture;

        }
    }

    private void UserName_TextChanged(object sender, TextChangedEventArgs e)
    {
        UsrBox.Text = UserName.Text;
    }

    private async void Create_Click(object sender, RoutedEventArgs e)
    {
        await CreateUserOnStartup();
        Thread.Sleep(500);
        Frame.Navigate(typeof(SetupUi));
    }

    #region startupclasses
    private async Task CreateUserOnStartup()
    {
        FireBrowserWinUi3MultiCore.User newUser = new FireBrowserWinUi3MultiCore.User
        {
            Username = UserName.Text,
        };

        // Create a list of users and add the new user to it.
        List<FireBrowserWinUi3MultiCore.User> users = new List<FireBrowserWinUi3MultiCore.User>();
        users.Add(newUser);

        // Create the user folders.
        UserFolderManager.CreateUserFolders(newUser);

        UserDataManager.SaveUsers(users);

        AuthService.Authenticate(newUser.Username);

        await CopyImageToUserDirectory();
    }

    string iImage = "";
    private async Task CopyImageToUserDirectory()
    {
        string imageName = $"{iImage}"; // Replace this with the actual image name
        string destinationFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username);

        try
        {
            StorageFolder destinationFolder = await StorageFolder.GetFolderFromPathAsync(destinationFolderPath);

            StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///FireBrowserWinUi3Assets/Assets/{imageName}"));

            string destinationFilePath = Path.Combine(destinationFolderPath, "profile_image.jpg"); // Replace with desired file name
            StorageFile destinationFile = await imageFile.CopyAsync(destinationFolder, "profile_image.jpg", NameCollisionOption.ReplaceExisting);

            Console.WriteLine("Image copied successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error copying image: {ex.Message}");
        }
    }

    #endregion
}