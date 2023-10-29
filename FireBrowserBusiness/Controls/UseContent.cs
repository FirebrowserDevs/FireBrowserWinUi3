using FireBrowserBusiness;
using FireBrowserBusiness.Pages;
using FireBrowserWinUi3.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FireBrowserWinUi3.Controls
{
    public class UseContent
    {
        public static WebContent WebContent => (Window.Current.Content as Frame)?.Content as WebContent;
        public static MainWindow MainPageContent => (Window.Current.Content as Frame)?.Content as MainWindow;
        public static SettingsPage SettingsContent => (Window.Current.Content as Frame)?.Content as SettingsPage;
        public static NewTab NewTabPage => (Window.Current.Content as Frame)?.Content as NewTab;
    }
}
