using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserDownloader
{
    public static class FireDownloader
    {

        public static string FireDownloaderPath { get; set; }
        public static string DlFolderPath { get; set; }
        public async static void Init()
        {
            FireDownloaderPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "FireBrowserWinUi3.FireDownloader", "Assets", "aria2c.exe");

            // Get the donwload folder path
            StorageFolder dlFolder = await (await (await DownloadsFolder.CreateFolderAsync(Guid.NewGuid().ToString())).GetParentAsync()).GetParentAsync();
            DlFolderPath = dlFolder.Path;

            // Start aria2 to avoid erors
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = FireDownloaderPath,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            Process process = new();

            process.StartInfo = startInfo;
            process.Start();
        }

        public static Process Downlaod(string url)
        {
            Process process = new();

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = FireDownloaderPath,
                ArgumentList = { "-d " + DlFolderPath, url },
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };


            process.StartInfo = startInfo;
            process.Start();
            process.BeginOutputReadLine();

            return process;
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Regex regex = new("[0-9]{1,4}[a-zA-Z]{0,2}B");
                var matches = regex.Matches(e.Data);

                if (matches.Count == 3)
                {
                    foreach (Match match in matches)
                    {
                        string s = match.Value;
                    }

                    Regex percentageRegex = new(@"\d{1,3}%");
                    var percentage = percentageRegex.Match(e.Data);

                    if (percentage.Success)
                    {
                        int pers = int.Parse(percentage.Value.Replace("%", ""));
                    }
                }
            }
        }

        private static void Process_Exited(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
