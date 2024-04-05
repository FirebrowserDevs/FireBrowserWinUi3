using FireBrowserWinUi3MultiCore;
using FireBrowserWinUi3MultiCore.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserWinUi3Core.CoreUi;
public sealed partial class ProfileCommander : Flyout
{
    public ProfileCommander()
    {
        this.InitializeComponent();
        LoadUserDataAndSettings();
    }

    private void LoadUserDataAndSettings()
    {
        var currentUser = AuthService.IsUserAuthenticated && AuthService.Authenticate(AuthService.CurrentUser?.Username) ? AuthService.CurrentUser : null;
        UsernameDisplay.Text = currentUser?.Username ?? "DefaultUser";
    }


    string iImage = "";

    private async void pfpchanged_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (pfpchanged.SelectedItem != null)
        {
            // Assuming 'userImageName' contains the name of the user's image file
            string userImageName = pfpchanged.SelectedItem.ToString() + ".png"; // Replace this with the actual user's image name

            iImage = userImageName;
            // Instantiate ImageLoader
            ImageHelper imgLoader = new ImageHelper();

            // Use the LoadImage method to get the image
            var userProfilePicture = imgLoader.LoadImage(userImageName);

            // Assign the retrieved image to the ProfileImageControl
            RootImage.ProfilePicture = userProfilePicture;

            string destinationFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username);

            await CopyImageAsync(iImage.ToString(), destinationFolderPath);
        }
    }

    public async Task CopyImageAsync(string iImage, string destinationFolderPath)
    {
        ImageHelper imgLoader = new ImageHelper();
        imgLoader.ImageName = iImage;
        imgLoader.LoadImage($"{iImage}");

        StorageFolder destinationFolder = await StorageFolder.GetFolderFromPathAsync(destinationFolderPath);

        StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///FireBrowserWinUi3MultiCore/Assets/{iImage}"));
        StorageFile destinationFile = await imageFile.CopyAsync(destinationFolder, "profile_image.jpg", NameCollisionOption.ReplaceExisting);
    }


    private async void ChangeUsername_Click(object sender, RoutedEventArgs e)
    {
        string olduser = UsernameDisplay.Text;
        AuthService.ChangeUsername(olduser, username_box.Text.ToString());
        Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
    }
}