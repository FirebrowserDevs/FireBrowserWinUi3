using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.Graphics;
using WinRT.Interop;
using System.Diagnostics;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3.Services;
using Windows.Storage.Streams;
using Windows.Storage;
using FireBrowserWinUi3Exceptions;
using Newtonsoft.Json;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Xaml.Interactivity;

namespace FireBrowserWinUi3.Controls
{
    public sealed partial class CreateBackup : Window
    {
       private object _backupFilePath;
        //private string _backupPath;
        private AppWindow appWindow;
        private AppWindowTitleBar titleBar;
        public CreateBackup()
        {
            this.InitializeComponent();
            UpdateBack();
            InitializeWindow();
        }

        private async void UpdateBack()
        {
            await StartBackupProcess();
          
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
            appWindow.SetIcon("backup.ico");
            appWindow.ShowOnceWithRequestedStartupState();
            Windowing.Center(this);

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
        private async Task StartBackupProcess()
        {
            try
            {
                StatusTextBlock.Text = "Creating backup...";
                await Task.Delay(100);
                //_backupFilePath = await Task.Run(() => BackupManager.CreateBackup());
                _backupFilePath = await Task.Run(async () =>
                {
                    var fileName = BackupManager.CreateBackup();
                    this.DispatcherQueue.TryEnqueue(() =>
                    {
                        StatusTextBlock.Text = $"Backup is being uploaded to the cloud";
                    });
                    var json = await UploadFileToAzure(fileName);
                    this.DispatcherQueue.TryEnqueue(() =>
                    {
                        StatusTextBlock.Text = $"Successfully saved to the cloud Azure\\u2122\")";
                    });
                    return json;
                });

                Windows.Storage.ApplicationData.Current.LocalSettings.Values["FireCoreBackups"] += JsonConvert.SerializeObject(_backupFilePath);


                ExceptionLogger.LogInformation("File path is : " + JsonConvert.SerializeObject(_backupFilePath) + "\n"); 

                await Task.Delay(100);
                StatusTextBlock.Text = $"Backup created successfully at:\n{_backupFilePath}";

                string tempPath = Path.GetTempPath();
                string backupFilePath = Path.Combine(tempPath, "backup.fireback");

                File.Delete(backupFilePath);
                Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        private async Task<object> UploadFileToAzure(string fileName) {

            var connString = Windows.Storage.ApplicationData.Current.LocalSettings.Values["AzureStorageConnectionString"] as string;
            var az = new AzBackupService(connString, "storelean", "firebackups", AuthService.CurrentUser ?? new() { Id = Guid.NewGuid(), Username = "Admin", IsFirstLaunch = false});

            StorageFile file = await StorageFile.GetFileFromPathAsync(fileName); 
            IRandomAccessStream randomAccessStream = await file.OpenAsync(FileAccessMode.Read);
            return await az.UploadFileToBlobAsync(file.Name, randomAccessStream); 
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}