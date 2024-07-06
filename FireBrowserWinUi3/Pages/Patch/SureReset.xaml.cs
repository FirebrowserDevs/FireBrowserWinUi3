using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
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
            await ResetBrowserAppAsync();

            // Restart the app
            await Windows.ApplicationModel.Core.CoreApplication.RequestRestartAsync(string.Empty);
        }

        private async Task ResetBrowserAppAsync()
        {
            try
            {
                StorageFolder documentsFolder = KnownFolders.DocumentsLibrary;
                StorageFolder coreFolder = await documentsFolder.CreateFolderAsync("FireBrowserUserCore", CreationCollisionOption.OpenIfExists);

                if (coreFolder != null)
                {
                    await coreFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
            catch (Exception ex)
            {
                // Handle any errors during folder deletion
                // For example, show a message or log the error
                Console.WriteLine($"Error deleting FireBrowserUserCore folder: {ex.Message}");
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }
    }
}
