using FireBrowserWinUi3.Services;
using FireBrowserWinUi3MultiCore;
using FireBrowserWinUi3MultiCore.Helper;
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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
using static System.Net.Mime.MediaTypeNames;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3
{
   
    public sealed partial class AddUserWindow : Window
    {

        public AddUserWindow()
        {
            this.InitializeComponent();
        }

        string iImage = "";
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


        private async void Button_Click(object sender, RoutedEventArgs e)
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

            AuthService.NewCreatedUser = newUser;

            AppService.CreateNewUsersSettings();
        }
    }
}
