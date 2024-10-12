using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FireBrowserWinUi3.Services;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using WinRT.Interop;

namespace FireBrowserWinUi3;

public class UserExtend : User
{
    public string PicturePath { get; }
    public User FireUser { get; }

    public UserExtend(User user) : base(user)
    {
        FireUser = user;
        string path = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username, "profile_image.jpg");
        PicturePath = File.Exists(path) ? path : "ms-appx:///FireBrowserWinUi3Assets/Assets/user.png";
    }
}

public class UC_Viewmodel : ObservableRecipient
{
    public UC_Viewmodel() => Users = new List<UserExtend>();
    public List<UserExtend> Users { get; set; }
    public UserExtend User { get; set; }
    public Window ParentWindow { get; set; }

    private ICommand _exitWindow;
    public ICommand ExitWindow => _exitWindow ??= new RelayCommand(() =>
    {
        AppService.IsAppGoingToClose = true;
        ParentWindow?.Close();
    });

    public void RaisePropertyChanges([CallerMemberName] string propertyName = null) => OnPropertyChanged(propertyName);
}

public sealed partial class UserCentral : Window
{
    private readonly AppWindow appWindow;
    private readonly AppWindowTitleBar titleBar;
    public static UserCentral Instance { get; private set; }
    public UC_Viewmodel ViewModel { get; set; }
    public static bool IsOpen { get; private set; }

    public UserCentral()
    {
        InitializeComponent();
        ViewModel = new UC_Viewmodel { ParentWindow = this };
        Instance = this;
        Activated += async (_, _) => await LoadDataGlobally();
    }

    public async Task LoadDataGlobally()
    {
        string coreFolderPath = UserDataManager.CoreFolderPath;
        ViewModel.Users = await GetUsernameFromCoreFolderPath(coreFolderPath);
        UserListView.ItemsSource = ViewModel.Users;
        ViewModel.RaisePropertyChanges(nameof(ViewModel.Users));
    }

    private async Task<List<UserExtend>> GetUsernameFromCoreFolderPath(string coreFolderPath, string userName = null)
    {
        try
        {
            string usrCoreFilePath = Path.Combine(coreFolderPath, "UsrCore.json");
            if (File.Exists(usrCoreFilePath))
            {
                string jsonContent = await File.ReadAllTextAsync(usrCoreFilePath);
                var users = JsonSerializer.Deserialize<List<User>>(jsonContent);

                return users?.FindAll(user => !user.Username.Contains("Private"))?.ConvertAll(user => new UserExtend(user));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading UsrCore.json: {ex.Message}");
        }
        return new List<UserExtend>();
    }

    private void UserListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (UserListView.SelectedItem is UserExtend selectedUser)
        {
            AuthService.Authenticate(selectedUser.FireUser.Username);
            Close();
        }
    }

    private async void AppBarButton_Click(object sender, RoutedEventArgs e)
    {
        var usr = new AddUserWindow();
        IntPtr hWnd = WindowNative.GetWindowHandle(this);
        if (hWnd != IntPtr.Zero)
        {
            Windowing.SetWindowPos(hWnd, Windowing.HWND_BOTTOM, 0, 0, 0, 0, Windowing.SWP_NOSIZE);
        }
        await AppService.ConfigureSettingsWindow(usr);
    }
}
