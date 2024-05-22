using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace FireBrowserUpdate
{
    public partial class Patcher : Form
    {
        public Patcher()
        {
            InitializeComponent();
            PatchDLLs();
        }

        private void PatchDLLs()
        {
            try
            {
                string patchFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patch.ptc");

                if (!File.Exists(patchFilePath))
                {
                    CloseAndExit();
                    return;
                }

                string[] dllNamesToUpdate = File.ReadAllLines(patchFilePath);

                foreach (string dllName in dllNamesToUpdate)
                {
                    if (dllName.StartsWith("FireBrowserWinUi3") && dllName.EndsWith(".dll"))
                    {
                        string localFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dllName);
                        string tempFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{dllName}.tmp");

                        string url = $"https://frcloud.000webhostapp.com/{dllName}";
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(url, tempFilePath);
                        }

                        if (IsUpdateRequired(localFilePath, tempFilePath))
                        {
                            if (File.Exists(localFilePath))
                            {
                                File.Delete(localFilePath);
                            }
                            File.Move(tempFilePath, localFilePath);
                        }
                        else
                        {
                            File.Delete(tempFilePath);
                        }
                    }
                }

                File.Delete(patchFilePath);
                File.Create("Patch");
                CloseAndStartFireBrowser();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseAndExit();
            }
        }

        private bool IsUpdateRequired(string localFilePath, string tempFilePath)
        {
            if (!File.Exists(localFilePath))
                return true;

            try
            {
                Version localVersion = GetFileVersion(localFilePath);
                Version remoteVersion = GetFileVersion(tempFilePath);

                return remoteVersion > localVersion;
            }
            catch
            {
                return false;
            }
        }

        private Version GetFileVersion(string filePath)
        {
            return new Version(FileVersionInfo.GetVersionInfo(filePath).FileVersion);
        }

        private void CloseAndStartFireBrowser()
        {
            try
            {
                string executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FireBrowserWinUi3.exe");

                if (!File.Exists(executablePath))
                {
                    MessageBox.Show("FireBrowserWinUi3.exe not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CloseAndExit();
                    return;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    UseShellExecute = true
                };
                Process.Start(startInfo);

                Thread.Sleep(2000);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while restarting FireBrowserWinUi3.exe: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseAndExit();
            }
        }

        private void CloseAndExit()
        {
            Application.Exit();
        }
    }
}
