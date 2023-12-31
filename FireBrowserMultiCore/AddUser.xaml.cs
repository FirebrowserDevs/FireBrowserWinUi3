using FireBrowserMultiCore.Helper;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserMultiCore;

public sealed partial class AddUser : ContentDialog
{
    public AddUser()
    {
        this.InitializeComponent();
    }

    private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
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

            await CopyImageToUserDirectory();
            // Close the ContentDialog
            Hide();
        }
        else
        {
            return;
        }
    }

    string iImage = "";
    private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        Hide();
    }

    private async Task CopyImageToUserDirectory()
    {
        string imageName = $"{iImage}"; // Replace this with the actual image name
        string destinationFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, Userbox.Text.ToString());

        try
        {
            StorageFolder destinationFolder = await StorageFolder.GetFolderFromPathAsync(destinationFolderPath);

            StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/Assets/{imageName}"));

            string destinationFilePath = Path.Combine(destinationFolderPath, "profile_image.jpg"); // Replace with desired file name
            StorageFile destinationFile = await imageFile.CopyAsync(destinationFolder, "profile_image.jpg", NameCollisionOption.ReplaceExisting);

            Console.WriteLine("Image copied successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error copying image: {ex.Message}");
        }
    }
    private void ProfileImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProfileImage.SelectedItem != null)
        {
            // Assuming 'userImageName' contains the name of the user's image file
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