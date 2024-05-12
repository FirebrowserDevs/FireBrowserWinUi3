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
            // Additional initialization code...
        }

        private void PatchDLLs()
        {
            try
            {
                Thread.Sleep(1000);

                string patchFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patch.ptc");

                if (!File.Exists(patchFilePath))
                {
                    CloseAndExit();
                    return;
                }

                // Read the patch.ptc file
                string[] dllsToUpdate = File.ReadAllLines(patchFilePath);

                // Delete existing DLL files and download new ones
                foreach (var dllName in dllsToUpdate)
                {
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dllName);

                    // Delete existing DLL file
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    // Download new DLL file
                    string url = $"https://frcloud.000webhostapp.com/{dllName}";
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(url, filePath);
                    }
                }

                // Delete the patch.ptc file
                File.Delete(patchFilePath);

                // Restart FireBrowserWinUi3.exe
                CloseAndStartFireBrowser();
            }
            catch (Exception ex)
            {
                CloseAndExit();
            }
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

                // Close the current application
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
            // Close the current application
            Application.Exit();
        }
    }
}