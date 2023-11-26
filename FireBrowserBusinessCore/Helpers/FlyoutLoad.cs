using FireBrowserBusinessCore.CoreUi;
using Microsoft.UI.Xaml;

namespace FireBrowserBusinessCore.Helpers
{
    public static class FlyoutLoad
    {
        public static XamlRoot XamlRoot { get; set; }
        public static async void ShowFlyout(FrameworkElement element)
        {
            SecurityInfo flyout = new();
            flyout.ShowAt(element);
        }
    }
}
