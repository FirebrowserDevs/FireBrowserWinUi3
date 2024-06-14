using FireBrowserWinUi3Assets;
using FireBrowserWinUi3DataCore.Models;
using FireBrowserWinUi3Migration;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserWinUi3
{

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
                string userImageName = ProfileImage.SelectedItem.ToString() + ".png";
                iImage = userImageName;
                ImageLoader imgLoader = new ImageLoader();
                var userProfilePicture = imgLoader.LoadImage(userImageName);
                Pimg.ProfilePicture = userProfilePicture;
            }
        }

        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UsrBox.Text = UserName.Text;
        }

        private async void Create_Click(object sender, RoutedEventArgs e)
        {
            await ShowBrowserSelectionDialog();
        }

     

        private async Task ShowBrowserSelectionDialog()
        {
           
                await CreateUserOnStartup();
                Frame.Navigate(typeof(SetupUi));
            
        }

      

        private async Task CreateUserOnStartup()
        {
            FireBrowserWinUi3MultiCore.User newUser = new FireBrowserWinUi3MultiCore.User
            {
                Username = UserName.Text,
            };

            List<FireBrowserWinUi3MultiCore.User> users = new List<FireBrowserWinUi3MultiCore.User> { newUser };
            UserFolderManager.CreateUserFolders(newUser);
            UserDataManager.SaveUsers(users);
            AuthService.Authenticate(newUser.Username);

            await CopyImageToUserDirectory();
        }

        string iImage = "";
        private async Task CopyImageToUserDirectory()
        {
            string imageName = $"{iImage}";
            string destinationFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username);

            try
            {
                StorageFolder destinationFolder = await StorageFolder.GetFolderFromPathAsync(destinationFolderPath);
                StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///FireBrowserWinUi3Assets/Assets/{imageName}"));
                await imageFile.CopyAsync(destinationFolder, "profile_image.jpg", NameCollisionOption.ReplaceExisting);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying image: {ex.Message}");
            }
        }

        private void BrowserSelectionDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Handle primary button click if needed
        }

        private void BrowserSelectionDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Handle secondary button click if needed
        }
    }
}
