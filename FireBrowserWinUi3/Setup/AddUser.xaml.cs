using FireBrowserWinUi3.Services;
using FireBrowserWinUi3MultiCore;
using FireBrowserWinUi3MultiCore.Helper;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserWinUi3.Setup;

public sealed partial class AddUser : ContentDialog
{
    public AddUser()
    {
        this.InitializeComponent();
    }

    private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        string enteredUsername = Userbox.Text;

        if (string.IsNullOrWhiteSpace(enteredUsername)) return;

        User newUser = new User
        {
            Id = Guid.NewGuid(),
            Username = enteredUsername,
            IsFirstLaunch = false,
            UserSettings = null // You might want to initialize UserSettings based on your application logic
        };

        List<FireBrowserWinUi3MultiCore.User> users = new List<FireBrowserWinUi3MultiCore.User>();
        users.Add(newUser);

        AuthService.AddUser(newUser);

        UserFolderManager.CreateUserFolders(newUser);

        string destinationFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, Userbox.Text.ToString());

        await CopyImageAsync(iImage.ToString(), destinationFolderPath);

        AppService.CreateNewUsersSettings();

        Hide();



    }


    string iImage = "";
    private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        Hide();
    }

    public async Task CopyImageAsync(string iImage, string destinationFolderPath)
    {
        ImageHelper imgLoader = new ImageHelper();
        imgLoader.ImageName = iImage;
        imgLoader.LoadImage($"{iImage}");

        StorageFolder destinationFolder = await StorageFolder.GetFolderFromPathAsync(destinationFolderPath);

        StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///FireBrowserWinUi3MultiCore/Assets/{iImage}"));
        StorageFile destinationFile = await imageFile.CopyAsync(destinationFolder, "profile_image.jpg", NameCollisionOption.ReplaceExisting);

        Console.WriteLine("Image copied successfully!");
    }

    private void ProfileImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        if (ProfileImage.SelectedItem != null)
        { // Assuming 'userImageName' contains the name of the user's image file
            string userImageName = ProfileImage.SelectedItem.ToString() + ".png"; // Replace this with the actual user's image name

            iImage = userImageName;
            // Instantiate ImageLoader
            ImageHelper imgLoader = new ImageHelper();

            // Use the LoadImage method to get the image
            var userProfilePicture = imgLoader.LoadImage(userImageName);

            // Assign the retrieved image to the ProfileImageControl
            Pimg.ProfilePicture = userProfilePicture;



        }
    }

    private void Userbox_TextChanged(object sender, TextChangedEventArgs e)
    {
        UsrBox.Text = Userbox.Text;
    }
}