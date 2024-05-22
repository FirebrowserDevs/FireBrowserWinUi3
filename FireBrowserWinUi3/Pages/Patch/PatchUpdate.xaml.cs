using FireBrowserWinUi3.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Pages.Patch
{
    public sealed partial class PatchUpdate : ContentDialog
    {
        private CancellationTokenSource cts = new CancellationTokenSource();

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

                string[] dllFiles = Directory.GetFiles(startupPath, "FireBrowserWinUi3*.dll");

                using (StreamWriter writer = new StreamWriter(versionsFilePath))
                {
                    foreach (var dllFile in dllFiles)
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(dllFile);
                        writer.WriteLine($"{Path.GetFileName(dllFile)}: {versionInfo.FileVersion}");
                    }
                }

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
                LoadingRing.IsActive = true;

                string serverVersionsUrl = "https://frcloud.000webhostapp.com/data.json";
                string serverVersionsJson;
                using (var client = new HttpClient())
                {
                    serverVersionsJson = await client.GetStringAsync(serverVersionsUrl);
                }

                var serverVersions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(serverVersionsJson);

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

                if (filesToUpdate.Count > 0)
                {
                    foreach (var dllName in filesToUpdate)
                    {
                        DllListTextBlock.Text += dllName + "\n";
                    }
                    PrimaryButtonText = "Update";
                }
                else
                {
                    DllListTextBlock.Text = "No updates are required.";
                    PrimaryButtonText = "OK";
                    SecondaryButtonText = "";
                }
            }
            catch (Exception ex)
            {
                TitleText.Text = "Server Or Local Error Occured";
                PrimaryButtonText = "OK";
                ShowErrorMessage($"An error occurred while comparing versions: {ex.Message}");
            }
            finally
            {
                LoadingRing.IsActive = false;
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
