using CommunityToolkit.Mvvm.Messaging;
using FireBrowserWinUi3.Services;
using FireBrowserWinUi3.Services.Messages;
using FireBrowserWinUi3.Services.ViewModels;
using FireBrowserWinUi3Core.Helpers;
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Isolation;
using Windows.Storage;
using Windows.System;
using static System.Net.Mime.MediaTypeNames;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    

    public sealed partial class ProfileCommander : Flyout
    {
        private SettingsService SettingsService { get; set; }
        private MainWindowViewModel MainWindowViewModel { get; set; }   
        private IMessenger  Messenger { get; set; } 
        
        public ProfileCommander(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;  
            SettingsService = App.GetService<SettingsService>();    
            Messenger = App.GetService<IMessenger>();   

            this.InitializeComponent();
            LoadUserDataAndSettings();
            
        }

        
        private void LoadUserDataAndSettings()
        {
            if (!AuthService.IsUserAuthenticated)
                return;

           UsernameDisplay.Text = SettingsService.CurrentUser.Username ?? "DefaultUser"; 
          
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
                
                MainWindowViewModel.ProfileImage = userProfilePicture;

                // Assign the retrieved image to the ProfileImageControl
                RootImage.ProfilePicture = userProfilePicture;

                string destinationFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username);

                await CopyImageAsync(iImage.ToString(), destinationFolderPath);
               
                var pip = MainWindowViewModel.MainView?.TabViewContainer.TabStripHeader as PersonPicture;
                //var pipUser = MainWindowViewModel.MainView?.MainUserPicture as PersonPicture;

                if (pip is PersonPicture person) {
                    person.ProfilePicture = userProfilePicture;
                }
                
                
                //if (pipUser is PersonPicture person1) {
                //    person1.ProfilePicture = userProfilePicture;
                //}
                

                
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

            Console.WriteLine("Image copied successfully!");
        }


        private async void ChangeUsername_Click(object sender, RoutedEventArgs e)
        {
            
            Messenger.Send(new Message_Settings_Actions(EnumMessageStatus.Settings));

            string olduser = UsernameDisplay.Text;

            AuthService.ChangeUsername(olduser, username_box.Text.ToString());
            Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
        }
    }
}