using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace FireBrowserWinUi3.Pages.Patch;

public sealed partial class PatchUpdate : ContentDialog
{
    public PatchUpdate()
    {
        this.InitializeComponent();
        CheckForUpdates();
    }

    private void CheckForUpdates()
    {
        try
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            string versionsFilePath = Path.Combine(startupPath, "versions.txt");

            // Get files in the startup folder
            string[] dllFiles = Directory.GetFiles(startupPath, "FireBrowserWinUi3*.dll");

            // Output DLL names and versions to versions.txt
            using (StreamWriter writer = new StreamWriter(versionsFilePath))
            {
                foreach (var dllFile in dllFiles)
                {
                    var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(dllFile);
                    writer.WriteLine($"{Path.GetFileName(dllFile)}: {versionInfo.FileVersion}");
                }
            }

            // Compare versions with server versions
            CompareVersions(versionsFilePath);
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"An error occurred while checking for updates: {ex.Message}");
        }
    }

    private async void CompareVersions(string versionsFilePath)
    {
        try
        {
            // Show the loading ring
            LoadingRing.IsActive = true;

            // Read server versions from the server
            string serverVersionsUrl = "https://frcloud.000webhostapp.com/data.json";
            string serverVersionsJson;
            using (var client = new HttpClient())
            {
                serverVersionsJson = await client.GetStringAsync(serverVersionsUrl);
            }

            // Parse server version JSON data
            // Assuming serverVersionsJson is an array of objects [{ "name": "FireBrowserWinUi3AdBlockCore.dll", "version": "1.0.0.1" }, ...]
            var serverVersions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(serverVersionsJson);

            // Read local versions from the versions.txt file
            Dictionary<string, string> localVersions = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(versionsFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        localVersions[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }

            // Compare versions
            List<string> filesToUpdate = new List<string>();
            foreach (var serverVersion in serverVersions)
            {
                string name = serverVersion.name;
                string version = serverVersion.version;

                if (localVersions.ContainsKey(name))
                {
                    Version localVersion = new Version(localVersions[name]);
                    Version serverVer = new Version(version);

                    if (serverVer > localVersion)
                    {
                        filesToUpdate.Add(name);
                    }
                }
            }

            // Display information about which DLLs need to be updated
            if (filesToUpdate.Count > 0)
            {
                foreach (var dllName in filesToUpdate)
                {
                    DllListTextBlock.Text += dllName + "\n";
                }
            }
            else
            {
                DllListTextBlock.Text = "No updates are required.";
                PrimaryButtonText = "OK"; // Change the button text to OK
                SecondaryButtonText = ""; // Hide the secondary button
            }
        }
        catch (Exception ex)
        {
            TitleText.Text = "Server Or Local Error Occured";
            PrimaryButtonText = "OK"; // Change the button text to OK
            ShowErrorMessage($"An error occurred while comparing versions: {ex.Message}");
        }
        finally
        {
            // Hide the loading ring
            LoadingRing.IsActive = false;
        }
    }

    string filesToUpdate;
    private void ShowErrorMessage(string message)
    {
        DllListTextBlock.Text = message;
        SecondaryButtonText = ""; // Hide the secondary button if an error occurs
    }

    private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        try
        {
            if (PrimaryButtonText == "Update")
            {
                try
                {
                    string DATA = DllListTextBlock.Text.ToString();

                    // Save the string to patch.ptc file
                    string patchFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patch.ptc");
                    using (StreamWriter writer = new StreamWriter(patchFilePath))
                    {
                        // Write each DLL name on a new line
                        foreach (var dllName in DATA.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            await writer.WriteLineAsync(dllName.Trim());
                        }
                    }

                    // Start FireBrowserUpdate.exe process
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "FireBrowserUpdate.exe",
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);

                    // Close the application
                    Application.Current.Exit();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"An error occurred while initiating the update process: {ex.Message}");
                }
            }
            else if (PrimaryButtonText == "OK")
            {
                // Close the dialog
                Hide();
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"An error occurred while initiating the update process: {ex.Message}");
        }
    }
}