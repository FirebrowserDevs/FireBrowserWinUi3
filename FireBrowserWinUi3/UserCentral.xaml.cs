using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FireBrowserWinUi3.Services.Messages;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using WinRT.Interop;

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

    public void RaisePropertyChanges([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(propertyName);
    }
}
public sealed partial class UserCentral : Window
{
    public UC_Viewmodel ViewModel { get; set; }

    // Static property to keep track of whether UserCentral is already open
    public static bool IsOpen { get; private set; }

    public UserCentral()
    {
        // Check if UserCentral is already open
        if (IsOpen)
        {
            // If it is, close this instance
            this.Close();
            return;
        }

        this.InitializeComponent();

        // Set IsOpen to true
        IsOpen = true;

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
            AuthService.Authenticate(_user.FireUser.Username);
            // Set IsOpen to false before closing
            IsOpen = false;
            this.Close();
        }
    }
}