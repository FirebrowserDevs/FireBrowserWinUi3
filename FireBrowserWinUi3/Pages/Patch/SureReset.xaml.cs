using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserWinUi3.Pages.Patch
{
    public sealed partial class SureReset : ContentDialog
    {
        public SureReset()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string resetFilePath = Path.Combine(Path.GetTempPath(), "Reset.set");
            // Write some content to the file (or leave it empty)
            File.WriteAllText(resetFilePath, "True");
            // Restart the app
            Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
        }



        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }
    }
}
