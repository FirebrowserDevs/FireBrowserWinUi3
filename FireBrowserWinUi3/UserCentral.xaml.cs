using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FireBrowserWinUi3.Pages.Patch;
using FireBrowserWinUi3.Services;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics;
using WinRT;
using WinRT.Interop;

namespace FireBrowserWinUi3
{
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

    public partial class UC_Viewmodel : ObservableRecipient
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

        [RelayCommand]
        private void OpenWindowsWeather()
        {
            var options = new Windows.System.LauncherOptions();
            options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseMinimum;

            // Launch the URI
            Windows.System.Launcher.LaunchUriAsync(new("msnweather://forecast"), options).GetAwaiter().GetResult();
        }

        public void RaisePropertyChanges([CallerMemberName] string propertyName = null) => OnPropertyChanged(propertyName);
    }

    public sealed partial class UserCentral : Window
    {
        private readonly AppWindow appWindow;
        public static UserCentral Instance { get; private set; }
        public UC_Viewmodel ViewModel { get; set; }
        public static bool IsOpen { get; private set; }

        public UserCentral()
        {
            InitializeComponent();
            ViewModel = new UC_Viewmodel { ParentWindow = this };
            Instance = this;
            Activated += async (_, _) => await LoadDataGlobally();

            // Get the AppWindow for this window
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow != null)
            {
                // Remove default title bar
                appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                appWindow.TitleBar.ButtonForegroundColor = Colors.White;
                appWindow.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;

                // Set window size
                var size = new SizeInt32(700, 500);
                appWindow.Resize(size);
          

                // Remove default window chrome
                var presenter = appWindow.Presenter as OverlappedPresenter;
                if (presenter != null)
                {
                    presenter.IsResizable = false;
                    presenter.SetBorderAndTitleBar(true, false);
                }
            }
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
            if (e.AddedItems.Count > 0)
                if (UserListView.SelectedItem is UserExtend selectedUser)
                {
                    if (AuthService.users.Count == 0)
                        AuthService.users = AuthService.LoadUsersFromJson();

                    AuthService.Authenticate(selectedUser.FireUser.Username);
                    // close active window if not Usercentral, and then assign it as usercentral and close to give -> windowscontroller notification of close usercentral 
                    AppService.ActiveWindow?.Close();
                    AppService.ActiveWindow = this;
                    AppService.ActiveWindow?.Close();
                }
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var usr = new AddUserWindow();
            usr.Closed += (s, e) =>
            {
                AppService.ActiveWindow = this;
            };
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            if (hWnd != IntPtr.Zero)
            {
                Windowing.SetWindowPos(hWnd, Windowing.HWND_BOTTOM, 0, 0, 0, 0, Windowing.SWP_NOSIZE);
            }
            await AppService.ConfigureSettingsWindow(usr);
        }

        private async void RestoreNow_Click(object sender, RoutedEventArgs e)
        {
            var win = AppService.ActiveWindow = new Window();
            win.SystemBackdrop = new MicaBackdrop();
            var present = new ContentPresenter();
            win.Content = present;
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            if (hWnd != IntPtr.Zero)
            {
                Windowing.SetWindowPos(hWnd, Windowing.HWND_BOTTOM, 0, 0, 0, 0, Windowing.SWP_NOSIZE);
            }

            await AppService.ConfigureSettingsWindow(win);

            win.Closed += (s, e) =>
            {
                IntPtr ucHwnd = Windowing.FindWindow(null, nameof(UserCentral));
                if (ucHwnd != IntPtr.Zero)
                {
                    Windowing.Center(ucHwnd);
                    // close active window if not Usercentral, and then assign it as usercentral and close to give -> windowscontroller notification of close usercentral 
                    AppService.ActiveWindow?.Close();
                    AppService.ActiveWindow = this;
                    AppService.ActiveWindow?.Close();
                }
                else
                {
                    Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
                }
            };

            RestoreBackupDialog dlg = new RestoreBackupDialog();
            dlg.XamlRoot = present.XamlRoot;
            await dlg.ShowAsync();
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (appWindow != null)
            {
                var presenter = appWindow.Presenter as OverlappedPresenter;
                if (presenter != null)
                {
                    presenter.Minimize();
                }
            }
        }
    }
}