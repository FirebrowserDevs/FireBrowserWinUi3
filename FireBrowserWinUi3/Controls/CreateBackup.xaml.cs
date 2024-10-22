using FireBrowserWinUi3.Services;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3Exceptions;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics;
using Windows.Storage;
using Windows.Storage.Streams;
using WinRT.Interop;

namespace FireBrowserWinUi3.Controls
{
    public sealed partial class CreateBackup : Window
    {
        private object _backupFilePath;
        //private string _backupPath;
        private AppWindow appWindow;
        private AppWindowTitleBar titleBar;
        private AzBackupService AzBackup { get; }  
        public CreateBackup()
        {
            this.InitializeComponent();
            UpdateBack();
            InitializeWindow();
            AppService.Dispatcher = this.DispatcherQueue;
            var connString = Windows.Storage.ApplicationData.Current.LocalSettings.Values["AzureStorageConnectionString"] as string;
            AzBackup = new AzBackupService(connString, "storelean", "firebackups", AuthService.CurrentUser ?? new() { Id = Guid.NewGuid(), Username = "Admin", IsFirstLaunch = false });

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
            appWindow.Title = "CreateBackup";
            appWindow.MoveAndResize(new RectInt32(500, 500, 850, 500));
            FireBrowserWinUi3Core.Helpers.Windowing.Center(this);
            appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);
            appWindow.MoveInZOrderAtTop();
            appWindow.SetIcon("backup.ico");
            appWindow.ShowOnceWithRequestedStartupState();
            //            Windowing.Center(this);

            DispatcherQueue.TryEnqueue(() =>
            {
                IntPtr hWnd = Windowing.FindWindow(null, nameof(CreateBackup));
                if (hWnd != IntPtr.Zero)
                {
                    Windowing.SetWindowPos(hWnd, Windowing.HWND_BOTTOM, 0, 0, 0, 0, Windowing.SWP_NOSIZE | Windowing.SWP_NOMOVE | Windowing.SWP_SHOWWINDOW);
                }
            });



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
                var json = new object() ; 
                //_backupFilePath = await Task.Run(() => BackupManager.CreateBackup());
                _backupFilePath = await Task.Run(async () =>
                {
                    var fileName = BackupManager.CreateBackup();
                    
                    var fireUser = await AzBackup.GetUserInformationAsync();

                    if (fireUser is FireBrowserWinUi3MultiCore.User user) {
                        this.DispatcherQueue.TryEnqueue(() =>
                        {
                            StatusTextBlock.Text = $"Backup is being uploaded to the cloud";
                        });
                        json = await UploadFileToAzure(fileName, user);
                        if (json  is not null)
                        {
                            this.DispatcherQueue.TryEnqueue(async () =>
                            {
                                StatusTextBlock.Text = $"Successfully saved to the cloud of (FireBrowserDevs)";

                                await Task.Delay(100);

                            });
                        }
                    }

                    this.DispatcherQueue.TryEnqueue(async () =>
                    {
                        await Task.Delay(100);
                        StatusTextBlock.Text = $"Backup created successfully in your Document folder as:\n{fileName}";

                    });

                    return json;
                });

                ExceptionLogger.LogInformation("File path is : " + JsonConvert.SerializeObject(_backupFilePath) + "\n");

                await Task.Delay(2000);


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
        private async Task<object> UploadFileToAzure(string fileName, FireBrowserWinUi3MultiCore.User fireUser)
        {

            StorageFile file = await StorageFile.GetFileFromPathAsync(fileName);
            IRandomAccessStream randomAccessStream = await file.OpenAsync(FileAccessMode.Read);
            return await AzBackup.UploadAndStoreFile(file.Name, randomAccessStream, fireUser);
            
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}