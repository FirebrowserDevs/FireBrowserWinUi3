using CommunityToolkit.WinUI.UI;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;


namespace FireBrowserWinUi3;

public sealed partial class ChangeUsernameCore : Window
{
    private AppWindow appWindow;
    private AppWindowTitleBar titleBar;
    private DispatcherTimer restartTimer;

    public ChangeUsernameCore()
    {
        this.InitializeComponent();
        this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(850,850,850,850));
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
            titleBar.BackgroundColor = btnColor;
            titleBar.ButtonBackgroundColor = btnColor;
            titleBar.InactiveBackgroundColor = btnColor;
            titleBar.ButtonInactiveBackgroundColor = btnColor;
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

            // Deserialize the JSON content
            string jsonContent = File.ReadAllText(jsonFilePath);
            var changeUsernameData = JsonSerializer.Deserialize<ChangeUsernameData>(jsonContent);

            // Update the UI with old and new usernames
            Username.Text = $"{changeUsernameData.OldUsername} -> {changeUsernameData.NewUsername}";

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
        Task.Delay(500);

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
        // Read the JSON file
        string tempFolderPath = Path.GetTempPath();
        string jsonFilePath = Path.Combine(tempFolderPath, "changeusername.json");
        File.Delete(jsonFilePath);
        Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
    }
}