using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using static FireBrowserWinUi3.MainWindow;

namespace FireBrowserWinUi3.Pages.SettingsPages;

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
                url = "https://github.com/FirebrowserDevs/FireBrowserWinUi3";
                break;
            case "License":
                url = "https://github.com/FirebrowserDevs/FireBrowserWinUi3/blob/main/License.lic";
                break;
        }
        var window = (Application.Current as App)?.m_window as MainWindow;
        window.NavigateToUrl(url);
    }
}
