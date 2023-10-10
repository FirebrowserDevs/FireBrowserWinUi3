using FireBrowserMultiCore;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsHome : Page
    {
        public SettingsHome()
        {
            this.InitializeComponent();
            LoadUserDataAndSettings();
        }

        private FireBrowserMultiCore.User GetUser()
        {
            // Check if the user is authenticated.
            if (AuthService.IsUserAuthenticated)
            {
                // Return the authenticated user.
                return AuthService.CurrentUser;
            }

            // If no user is authenticated, return null or handle as needed.
            return null;
        }

        Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
        private void LoadUserDataAndSettings()
        {
            if (GetUser() is not { } currentUser)
            {
                User.Text = "DefaultUser";
                return;
            }

            if (!AuthService.IsUserAuthenticated && !AuthService.Authenticate(currentUser.Username))
            {
                return;
            }

            User.Text = AuthService.CurrentUser?.Username ?? "DefaultUser";
        }

    }
}
