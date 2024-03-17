using CommunityToolkit.WinUI.UI;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChangeUsernameCore : Window
    {
        private AppWindow appWindow;
        private AppWindowTitleBar titleBar;
        private DispatcherTimer restartTimer;

        public ChangeUsernameCore()
        {
            this.InitializeComponent();
            title();
            ChangeUsername();
            SetupRestartTimer();
        }

        public void title()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

            appWindow = AppWindow.GetFromWindowId(windowId);

            if (!AppWindowTitleBar.IsCustomizationSupported())
            {
                // Why? Because I don't care
                throw new Exception("Unsupported OS version.");
            }
            else
            {
                titleBar = appWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
                var btnColor = Colors.Transparent;
            }
        }

        public void ChangeUsername()
        {
            try
            {
                // Read the JSON file
                string tempFolderPath = Path.GetTempPath();
                string jsonFilePath = Path.Combine(tempFolderPath, "changeusername.json");

                if (!File.Exists(jsonFilePath))
                {
                    Console.WriteLine("Change username JSON file not found.");
                    return;
                }

                string jsonContent = File.ReadAllText(jsonFilePath);

                // Deserialize the JSON content
                var changeUsernameData = JsonSerializer.Deserialize<ChangeUsernameData>(jsonContent);

                // Rename the folder
                string usersFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FireBrowserUserCore", "Users");
                string oldUserFolderPath = Path.Combine(usersFolderPath, changeUsernameData.OldUsername);
                string newUserFolderPath = Path.Combine(usersFolderPath, changeUsernameData.NewUsername);

                if (Directory.Exists(oldUserFolderPath))
                {
                    Directory.Move(oldUserFolderPath, newUserFolderPath);
                    Console.WriteLine($"Folder renamed from '{changeUsernameData.OldUsername}' to '{changeUsernameData.NewUsername}'.");
                }
                else
                {
                    Console.WriteLine($"Folder '{changeUsernameData.OldUsername}' not found.");
                }

                // Remove the JSON file
                File.Delete(jsonFilePath);
                Console.WriteLine("Change username JSON file deleted.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void SetupRestartTimer()
        {
            restartTimer = new DispatcherTimer();
            restartTimer.Tick += RestartTimer_Tick;
            restartTimer.Interval = TimeSpan.FromSeconds(2); // Set the interval to 2 seconds
            restartTimer.Start();
        }

        private void RestartTimer_Tick(object sender, object e)
        {
            restartTimer.Stop();

            // Delay the restart for a short duration to ensure the UI updates properly
            Task.Delay(100);

            // Restart the application
            Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
        }

        private class ChangeUsernameData
        {
            public string OldUsername { get; set; }
            public string NewUsername { get; set; }
        }

        private void ManaulRestart_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
        }
    }
}
