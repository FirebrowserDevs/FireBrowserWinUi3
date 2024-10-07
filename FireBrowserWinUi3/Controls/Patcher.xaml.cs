using FireBrowserWinUi3.Services;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Graphics;
using WinRT.Interop;

namespace FireBrowserWinUi3.Controls
{
    public sealed partial class Patcher : Window
    {
        private AppWindow appWindow;
        private AppWindowTitleBar titleBar;

        public Patcher()
        {
            this.InitializeComponent();
            InitializeWindow();
            PatchDLLsAsync();
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
            appWindow.SetIcon("logo.ico");

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

        private async void PatchDLLsAsync()
        {
            try
            {
                //string tempPath = Path.GetTempPath();
                //string patchFilePath = Path.Combine(tempPath, "Patch.ptc");
                string patchFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patch.core");

                if (!File.Exists(patchFilePath))
                {
                    return;
                }

                string[] dllNamesToUpdate = await File.ReadAllLinesAsync(patchFilePath);

                foreach (string dllName in dllNamesToUpdate)
                {
                    if (dllName.StartsWith("FireBrowserWinUi3") && dllName.EndsWith(".dll") && !dllName.Equals("FireBrowserWinUi3.dll", StringComparison.OrdinalIgnoreCase))
                    {
                        string localFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dllName);

                        if (!await TryDeleteFileAsync(localFilePath, 3, 500))
                        {
                            await HandleDeletionFailureAsync();
                            return;
                        }

                        string url = $"https://frcloud.000webhostapp.com/{dllName}";
                        await DownloadFileAsync(url, localFilePath);
                    }
                }

                await RestartApplicationAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in PatchDLLsAsync: {ex.Message}");
            }
        }

        private async Task<bool> TryDeleteFileAsync(string filePath, int maxRetries, int delayMs)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    // Check if the file is in use
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    return true;
                }
                catch (IOException) { await Task.Delay(delayMs); }
                catch (UnauthorizedAccessException) { await Task.Delay(delayMs); }
            }
            return false;
        }

        private async Task DownloadFileAsync(string url, string destinationPath)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                await using (var fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }
        }

        private async Task HandleDeletionFailureAsync()
        {
            //string tempPath = Path.GetTempPath();
            //string patchFilePath = Path.Combine(tempPath, "Patch.ptc");
            string patchFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patch.core");

            try
            {
                File.Delete(patchFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete patch file: {ex.Message}");
            }

            await RestartApplicationAsync();
        }

        private async Task RestartApplicationAsync()
        {
            try
            {
                string executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FireBrowserWinUi3.exe");

                if (!File.Exists(executablePath))
                {
                    Debug.WriteLine("Executable file not found for restart.");
                    return;
                }

                // Use the custom ProcessStarter to start the process
                ProcessStarter.StartProcess(executablePath, "", AppDomain.CurrentDomain.BaseDirectory);

                string tempPath = Path.GetTempPath();
                string patchFilePath = Path.Combine(tempPath, "Patch.ptc");

                // Ensure Patch.ptc is deleted
                try
                {
                    File.Delete(patchFilePath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error deleting patch file after restart: {ex.Message}");
                }

                await Task.Delay(2000); // Ensure the new process starts properly
                Application.Current.Exit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during restart: {ex.Message}");
            }
        }

    }
}
