using CommunityToolkit.WinUI.Controls;
using FireBrowserBusiness;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using static FireBrowserBusiness.MainWindow;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsAbout : Page
    {
        Passer param;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            param = e.Parameter as Passer;
        }
        public SettingsAbout()
        {
            this.InitializeComponent();
        }

        private void AboutCardClicked(object sender, RoutedEventArgs e)
        {
            string url = "https://example.com";
            switch ((sender as SettingsCard).Tag)
            {
                case "Discord":
                    url = "https://discord.gg/kYStRKBHwy";
                    break;
                case "GitHub":
                    url = "https://github.com/FirebrowserDevs/FireBrowser-Uwp";
                    break;
                case "License":
                    url = "https://github.com/FirebrowserDevs/FireBrowser-Uwp/TEXT/main/LICENSE";
                    break;
            }
            MainWindow mp = new();
        }
    }
}
