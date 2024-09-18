using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FireBrowserWinUi3.Services;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace FireBrowserWinUi3;

public class UserExtend : User
{

    public string PicturePath { get; set; }
    public User FireUser { get; set; }
    public UserExtend(User user) : base(user)
    {

        FireUser = user;
        var path = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username, "profile_image.jpg");

        if (File.Exists(path))
        {
            PicturePath = path;
        }
        else
        {
            PicturePath = new Uri("ms-appx:///FireBrowserWinUi3Assets/Assets/user.png").ToString();
        }

    }
}

public class UC_Viewmodel : ObservableRecipient
{

    public UC_Viewmodel() { }
    public List<UserExtend> Users { get; set; }
    public UserExtend User { get; set; }

    public Window ParentWindow { get; set; }

    ICommand _exitWindow;
    public ICommand ExitWindow => _exitWindow ?? (_exitWindow = new RelayCommand(
          () =>
          {
              AppService.IsAppGoingToClose = true;
              ParentWindow?.Close();

          }));
    public void RaisePropertyChanges([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(propertyName);
    }
}
public sealed partial class UserCentral : Window
{

    private AppWindow appWindow;
    private AppWindowTitleBar titleBar;
    public UC_Viewmodel ViewModel { get; set; }

    // Static property to keep track of whether UserCentral is already open
    public static bool IsOpen { get; private set; }

    public UserCentral()
    {
        this.InitializeComponent();
        // Set IsOpen to true
        ViewModel = new UC_Viewmodel();
        ViewModel.ParentWindow = this;
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
                    foreach (var user in users)
                    {
                        // Check if the username contains "private"
                        if (user.Username != null && !user.Username.Contains("Private"))
                        {
                            userExtends.Add(new UserExtend(user));
                        }
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

    private void UserListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var _user = UserListView.SelectedItem as UserExtend;
        if (_user != null)
        {

            // can remove because CheckNormal authenticates user but, maybe add something on ELSE;
            AuthService.Authenticate(_user.FireUser.Username);
            this.Close();

        }
    }

    

    private async void AppBarButton_Click(object sender, RoutedEventArgs e)
    {
       AddUserWindow usr = new AddUserWindow();
        usr.Activate();
    }
}