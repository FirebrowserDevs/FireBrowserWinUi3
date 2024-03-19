using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FireBrowserWinUi3.Services.Messages;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI;
using Microsoft.UI.Windowing;
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
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;



// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    ///
    public class UserExtend : User {
            
            public string PicturePath { get; set; }
            public User FireUser { get; set; }  
            public UserExtend(User user) : base(user)  {

                FireUser = user;    
                var path = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username, "profile_image.jpg");

                if (File.Exists(path))
                {
                    PicturePath = path;
                }
                else {
                    PicturePath = new Uri("ms-appx:///FireBrowserWinUi3Assets/Assets/user.png").ToString();
                }
                
            } 
        }

    public class UC_Viewmodel : ObservableRecipient {

        public UC_Viewmodel() { }   
        public List<UserExtend> Users { get; set; }
        public UserExtend User { get; set; }

        public void RaisePropertyChanges([CallerMemberName] string? propertyName = null)
        {
            OnPropertyChanged(propertyName);
        }

    }
    public sealed partial class UserCentral : Window
    {
        
        public UC_Viewmodel ViewModel { get; set; } 
        
        public UserCentral()
        {
            
          
            this.InitializeComponent();
            
            

            ViewModel = new UC_Viewmodel();
            string coreFolderPath = UserDataManager.CoreFolderPath;
            ViewModel.Users = GetUsernameFromCoreFolderPath(coreFolderPath);
            ViewModel.RaisePropertyChanges(nameof(ViewModel.Users));
            UserListView.ItemsSource = ViewModel.Users;   

        }
        public List<UserExtend> GetUsernameFromCoreFolderPath(string coreFolderPath, string userName = null)
        {
            try
            {
                string usrCoreFilePath = Path.Combine(coreFolderPath, "UsrCore.json");

                if (File.Exists(usrCoreFilePath))
                {
                    string jsonContent = File.ReadAllText(usrCoreFilePath);
                    var users = JsonSerializer.Deserialize<List<FireBrowserWinUi3MultiCore.User>>(jsonContent);

                    if (users?.Count > 0 && !string.IsNullOrWhiteSpace(users[0].Username))
                    {

                        List<UserExtend> userExtends = new List<UserExtend>();
                        foreach(var user in  users ) {
                            userExtends.Add(new UserExtend(user));
                        }
                        return userExtends;
                    }   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading UsrCore.json: {ex.Message}");
            }

            return null;
        }

        private async void UserListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var _user = UserListView.SelectedItem as UserExtend;
            if (_user != null) {

                // can remove because CheckNormal authenticates user but, maybe add something on ELSE;
                if (AuthService.Authenticate(_user.FireUser.Username)) {
                    
                    App.Current.CheckNormal(_user.FireUser.Username);
                    App.Current.m_window = new MainWindow();
                    App.Current.m_window.AppWindow.MoveInZOrderAtTop();
                    Windowing.Center(App.Current.m_window);
                    IntPtr hWnd = WindowNative.GetWindowHandle(this);
                    Windowing.HideWindow(hWnd); 

                    App.Current.m_window.Activate();
                    
                    
                    if (AuthService.IsUserAuthenticated)
                    {
                        IMessenger messenger = App.GetService<IMessenger>();
                        messenger?.Send(new Message_Settings_Actions($"Welcome {AuthService.CurrentUser.Username} to our FireBrowser", EnumMessageStatus.Login));
                    }

                    await Task.Delay(500);
                    this.Close();
                }

            }
            

        }
    }
}
