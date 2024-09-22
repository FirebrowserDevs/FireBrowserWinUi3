using FireBrowserWinUi3.Pages.Patch;
using FireBrowserWinUi3.Services;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3Core.Models;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace FireBrowserWinUi3.Pages.SettingsPages;
public sealed partial class SettingsHome : Page
{
    SettingsService SettingsService { get; set; }
    public static SettingsHome Instance { get; set; }  
    public SettingsHome()
    {
        SettingsService = App.GetService<SettingsService>();
        this.InitializeComponent();
        Instance = this; 
        LoadUserDataAndSettings();
        LoadUsernames();
    }

    public void LoadUsernames()
    {
        List<string> usernames = AuthService.GetAllUsernames();
        string currentUsername = AuthService.CurrentUser?.Username;

        if (currentUsername != null && currentUsername.Contains("Private"))
        {
            UserListView.IsEnabled = false;
            Add.IsEnabled = false;
        }
        else
        {
            UserListView.IsEnabled = true;
            
            Add.IsEnabled = true ;

            // this isn't working every time i go to page add button is disabled 
            // I understand for private though.

<<<<<<< Updated upstream
            //int nonPrivateUserCount = usernames.Count(username => !username.Contains("Private"));

            //if (nonPrivateUserCount + (currentUsername != null && !currentUsername.Contains("Private") ? 1 : 0) >= 6)
            //{
            //    Add.IsEnabled = false; // Assuming AddButton is the name of your "Add" button
            //}
            //else
            //{
            //    Add.IsEnabled = false;
            //}
=======
            if (nonPrivateUserCount + (currentUsername != null && !currentUsername.Contains("Private") ? 1 : 0) >= 6)
            {
                Add.IsEnabled = false; // Assuming AddButton is the name of your "Add" button
            }
            else
            {
                Add.IsEnabled = true;
            }
>>>>>>> Stashed changes

            foreach (string username in usernames.Where(username => username != currentUsername && !username.Contains("Private")))
            {
                UserListView.Items.Add(username);
            }
            
        }
    }

    private FireBrowserWinUi3MultiCore.User GetUser() =>
      AuthService.IsUserAuthenticated ? AuthService.CurrentUser : null;

    private void LoadUserDataAndSettings()
    {
        User.Text = GetUser()?.Username ?? "DefaultUser";
    }

    private async void Add_Click(object sender, RoutedEventArgs e)
    {
        AppService.IsAppNewUser = string.IsNullOrEmpty(AuthService.NewCreatedUser?.Username) ? true : false; 
        Window window = new AddUserWindow();
        // add first then update ui
        
        // do the settings now. 
        await AppService.ConfigureSettingsWindow(window);

       

        
    }

    public static async void OpenNewWindow(Uri uri)
    {
        await Windows.System.Launcher.LaunchUriAsync(uri);
    }


    private async void Switch_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button switchButton && switchButton.DataContext is string clickedUserName)
        {
            OpenNewWindow(new Uri($"firebrowseruser://{clickedUserName}"));
            Shortcut ct = new();
            await ct.CreateShortcut(clickedUserName);
        }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {

        if (sender is Button switchButton && switchButton.DataContext is string clickedUserName)
        {
            UserDataManager.DeleteUser(clickedUserName);

            UserListView.Items.Clear();
            LoadUsernames(); 
           // var window = (Application.Current as App)?.m_window as MainWindow;
        }
    }



    public void Run()
    {
        // URL of the JSON file containing the server versions
        string jsonUrl = "https://frcloud.000webhostapp.com/data.json";

        // Local file names
        string[] localFileNames = {
                "FireBrowserWinUi3AdBlockCore.dll",
                "FireBrowserWinUi3Setup.dll",
                "FireBrowserWinUi3QrCore.dll",
                "FireBrowserWinUi3Navigator.dll",
                "FireBrowserWinUi3MultiCore.dll",
                "FireBrowserWinUi3Modules.dll",
                "FireBrowserWinUi3Favorites.dll",
                "FireBrowserWinUi3Exceptions.dll",
                "FireBrowserWinUi3DataCore.dll",
                "FireBrowserWinUi3Database.dll",
                "FireBrowserWinUi3Core.dll",
                "FireBrowserWinUi3Auth.dll",
                "FireBrowserWinUi3AuthCore.dll",
                "FireBrowserWinUi3Assets.dll"
            };

        try
        {
            // Download the JSON file
            WebClient webClient = new WebClient();
            string jsonContent = webClient.DownloadString(jsonUrl);

            System.Diagnostics.Debug.WriteLine("JSON content downloaded successfully.");

            // Parse JSON content
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent);

            System.Diagnostics.Debug.WriteLine("JSON content parsed successfully.");

            // Create a dictionary to store server versions
            Dictionary<string, string> serverVersions = new Dictionary<string, string>();

            // Add server versions from JSON data
            foreach (var item in data)
            {
                string dllFileName = item.Name + ".dll";
                if (Array.Exists(localFileNames, name => name.Equals(dllFileName, StringComparison.OrdinalIgnoreCase)))
                {
                    serverVersions[dllFileName] = item.Value;
                }
            }

            System.Diagnostics.Debug.WriteLine("Server versions added successfully.");

            // Get files in the startup folder
            string[] dllFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            System.Diagnostics.Debug.WriteLine("DLL files retrieved successfully.");

            // Create a list to store names of files to be written to patch.core
            List<string> filesToPatch = new List<string>();

            // Compare versions
            foreach (var serverVersion in serverVersions)
            {
                string dllFileName = serverVersion.Key;
                string serverFileVersion = serverVersion.Value;

                string dllFilePath = Array.Find(dllFiles, f => Path.GetFileName(f).Equals(dllFileName, StringComparison.OrdinalIgnoreCase));
                if (dllFilePath != null)
                {
                    // Get version of the DLL file
                    var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(dllFilePath);
                    Version versionFromDll = new Version(versionInfo.FileVersion);

                    // Compare versions
                    Version serverVersionParsed = new Version(serverFileVersion);
                    if (serverVersionParsed > versionFromDll)
                    {
                        // Add DLL name to the list
                        filesToPatch.Add(dllFileName);
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("Versions compared successfully.");

            // Write names of files to be patched to patch.core
            string patchCoreFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patch.core");
            File.WriteAllLines(patchCoreFilePath, filesToPatch);

            System.Diagnostics.Debug.WriteLine("Patch file created successfully.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"An error occurred: {ex.Message}");
        }
    }



    private async void PatchBtn_Click(object sender, RoutedEventArgs e)
    {
        PatchUpdate dlg = new PatchUpdate();
        dlg.XamlRoot = this.XamlRoot;
        await dlg.ShowAsync();
    }

    private async void Reset_Click(object sender, RoutedEventArgs e)
    {
        SureReset dlg = new SureReset();
        dlg.XamlRoot = this.XamlRoot;
        await dlg.ShowAsync();
    }
}