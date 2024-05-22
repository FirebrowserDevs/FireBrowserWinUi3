using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireBrowserWinUiUpdate
{
    public partial class Patcher : Form
    {
        public Patcher()
        {
            InitializeComponent();
            PatchDLLs();
        }

        private async void PatchDLLs()
        {
            try
            {
                string startupPath = AppDomain.CurrentDomain.BaseDirectory;
                string patchFilePath = Path.Combine(startupPath, "PatchFile.p");

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

                await CreateUpdateDoneFile();
                await RestartApplication();
            }
            catch (Exception)
            {
                // Swallow exceptions to avoid displaying errors
            }
        }

        private async Task HandleDeletionFailure()
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            string patchFilePath = Path.Combine(startupPath, "PatchFile.p");

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

                string startupPath = AppDomain.CurrentDomain.BaseDirectory;
                string patchFilePath = Path.Combine(startupPath, "PatchFile.p");

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
                Application.Exit();
            }
            catch (Exception)
            {
                // Swallow exceptions to avoid displaying errors
            }
        }

        private async Task CreateUpdateDoneFile()
        {
            try
            {
                string startupPath = AppDomain.CurrentDomain.BaseDirectory;
                string updateDoneFilePath = Path.Combine(startupPath, "updatedone.txt");

                using (StreamWriter writer = new StreamWriter(updateDoneFilePath))
                {
                    await writer.WriteLineAsync("Update was successful.");
                }
            }
            catch (Exception)
            {
                // Swallow exceptions to avoid displaying errors
            }
        }
    }
}
