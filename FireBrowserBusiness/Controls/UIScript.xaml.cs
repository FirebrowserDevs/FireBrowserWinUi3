using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UIScript : ContentDialog
    {
        public UIScript()
        {
            this.InitializeComponent();
        }

        public static async Task ShowDialog(string title, string content, XamlRoot root)
        {
            ContentDialog dialog = new()
            {
                Title = title,
                XamlRoot = root,
                Content = content,
                PrimaryButtonText = "Okay",
                DefaultButton = ContentDialogButton.Primary
            };

            await dialog.ShowAsync();
        }

    }
}
