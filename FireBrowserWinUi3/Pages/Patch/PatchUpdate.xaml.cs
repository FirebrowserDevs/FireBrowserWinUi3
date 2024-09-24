using FireBrowserWinUi3.Services;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Pages.Patch
{
    public sealed partial class PatchUpdate : ContentDialog
    {
        private readonly UpdateService updateService = new UpdateService();

        public PatchUpdate()
        {
            this.InitializeComponent();
            RunUpdateServiceAsync();
        }

        public async Task RunUpdateServiceAsync()
        {
            try
            {
                await updateService.CheckUpdateAsync();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"An error occurred: {ex.Message}");
            }
        }

        private void ShowErrorMessage(string message)
        {
            DllListTextBlock.Text = message;
            SecondaryButtonText = "";
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (PrimaryButtonText == "Update")
            {
                try
                {
                    string data = DllListTextBlock.Text;

                    string patchFilePath = Path.Combine(Path.GetTempPath(), "Patch.ptc");
                    using (StreamWriter writer = new StreamWriter(patchFilePath))
                    {
                        foreach (var dllName in data.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            await writer.WriteLineAsync(dllName.Trim());
                        }
                    }

                    // Restart the app
                    Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"An error occurred while initiating the update process: {ex.Message}");
                }
            }
            else if (PrimaryButtonText == "OK")
            {
                Hide();
            }
        }
    }
}
