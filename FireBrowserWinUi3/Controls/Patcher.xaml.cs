using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Windows.Graphics;
using WinRT.Interop;

namespace FireBrowserWinUi3.Controls;
public sealed partial class Patcher : Window
{
    private AppWindow appWindow;
    private AppWindowTitleBar titleBar;

    public Patcher()
    {
        this.InitializeComponent();
        InitializeWindow();
        PatchDLLs();
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

    private async void PatchDLLs()
    {
        try
        {
            string tempPath = Path.GetTempPath();
            string patchFilePath = Path.Combine(tempPath, "Patch.ptc");

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
                    bool fileDeleted = false;

                    for (int i = 0; i < 3; i++) // Retry deletion up to 3 times
                    {
                        try
                        {
                            // Check if the file is in use
                            using (var stream = new FileStream(localFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                            {
                                // If no exception is thrown, the file is not in use
                            }

                            if (File.Exists(localFilePath))
                            {
                                File.Delete(localFilePath);
                            }
                            fileDeleted = true;
                            break;
                        }
                        catch (IOException)
                        {
                            await Task.Delay(500); // Wait before retrying
                        }
                        catch (UnauthorizedAccessException)
                        {
                            await Task.Delay(500); // Wait before retrying
                        }
                    }

                    if (!fileDeleted)
                    {
                        await HandleDeletionFailure();
                        return;
                    }

                    string url = $"https://frcloud.000webhostapp.com/{dllName}";
                    using (WebClient client = new WebClient())
                    {
                        await client.DownloadFileTaskAsync(new Uri(url), localFilePath);
                    }
                }
            }


            await RestartApplication();
        }
        catch (Exception)
        {
            // Swallow exceptions to avoid displaying errors
        }
    }

    private async Task HandleDeletionFailure()
    {
        string tempPath = Path.GetTempPath();
        string patchFilePath = Path.Combine(tempPath, "Patch.ptc");

        try
        {
            File.Delete(patchFilePath);
        }
        catch (Exception)
        {
            // Swallow exceptions to avoid displaying errors
        }

        await RestartApplication();
    }

    private async Task RestartApplication()
    {
        try
        {
            string executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FireBrowserWinUi3.exe");

            if (!File.Exists(executablePath))
            {
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                UseShellExecute = true
            };
            Process.Start(startInfo);

            string tempPath = Path.GetTempPath();
            string patchFilePath = Path.Combine(tempPath, "Patch.ptc");

            // Ensure Patch.ptc is deleted
            try
            {
                File.Delete(patchFilePath);
            }
            catch (Exception)
            {
                // Swallow exceptions to avoid displaying errors
            }

            await Task.Delay(2000); // Ensure the new process starts properly
            Application.Current.Exit();
        }
        catch (Exception)
        {
            // Swallow exceptions to avoid displaying errors
        }
    }
}
