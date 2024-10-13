using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics;
using Windows.Storage;
using WinRT.Interop;
using FireBrowserWinUi3MultiCore;

namespace FireBrowserWinUi3.Controls
{
    public sealed partial class RestoreBackUp : Window
    {
        private bool _isCancelled = false;
        private AppWindow appWindow;
        private AppWindowTitleBar titleBar;
        public RestoreBackUp()
        {
            this.InitializeComponent();
            InitializeWindow();
            StartRestoreProcess();
        }


        private async Task StartRestoreProcess()
        {
            try
            {
                string tempPath = Path.GetTempPath();
                string restoreFilePath = Path.Combine(tempPath, "restore.fireback");

                if (File.Exists(restoreFilePath))
                {
                    string backupFilePath = await File.ReadAllTextAsync(restoreFilePath);
                    await RestoreBackup(backupFilePath);
                }
                else
                {
                    await ShowErrorMessage("Restore file not found.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorMessage($"An error occurred: {ex.Message}");
            }
        }

        private async Task RestoreBackup(string backupFilePath)
        {
            StatusTextBlock.Text = "Restoring backup...";

            // Simulating backup restore process
            for (int i = 0; i <= 100; i++)
            {
                if (_isCancelled)
                {
                    StatusTextBlock.Text = "Restore cancelled.";
                    return;
                }

                // Update the progress bar and text for each step
                RestoreProgressBar.Value = i;
                PercentageTextBlock.Text = $"{i}%";

                // Simulate work being done
                await Task.Delay(25); // This gives time for the UI to update

                // Instead of updating backup manager in each iteration, you might want to do this after significant milestones
                if (i % 10 == 0)
                {
                    // Perform partial restore work every 10% or some other interval
                    BackupManager.RestoreBackup();
                }
            }

            // Delete the restore file once done
            string tempPath = Path.GetTempPath();
            string restoreFilePath = Path.Combine(tempPath, "restore.fireback");
            if (File.Exists(restoreFilePath))
            {
                File.Delete(restoreFilePath);
            }

            // Finalize the process
            StatusTextBlock.Text = "Backup restored successfully!";
            CancelButton.Content = "Close";
            Microsoft.Windows.AppLifecycle.AppInstance.Restart(""); // Optionally restart the app if needed
        }

      

        private void InitializeWindow()
        {
            var hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.MoveAndResize(new RectInt32(500, 500, 850, 500));
            FireBrowserWinUi3Core.Helpers.Windowing.Center(this);
            appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);
            appWindow.MoveInZOrderAtTop();
            appWindow.ShowOnceWithRequestedStartupState();
            appWindow.SetIcon("backup.ico");

            if (!AppWindowTitleBar.IsCustomizationSupported())
            {
                throw new Exception("Unsupported OS version.");
            }
            else
            {
                titleBar = appWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
                var btnColor = Colors.Transparent;
                titleBar.BackgroundColor = btnColor;
                titleBar.ButtonBackgroundColor = btnColor;
                titleBar.InactiveBackgroundColor = btnColor;
                titleBar.ButtonInactiveBackgroundColor = btnColor;
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (RestoreProgressBar.Value < 100)
            {
                _isCancelled = true;
            }
            else
            {
                this.Close();
            }
        }

        private async Task ShowErrorMessage(string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK"
            };

            await errorDialog.ShowAsync();
            this.Close();
        }
    }
}