using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FireBrowserDownloader.Objects
{
    public class FireBrowserDownloader : INotifyPropertyChanged
    {

        private string resultPath = string.Empty;
        private float progress = 0;
        private string totalFileSize = "0B";
        private string totalDownloadedDataSize = "0B";
        private string downloadSpeed = "0B";
        private string title = "Downloading...";
        private string description = string.Empty;
        private bool isProgressIntermeinate = true;
        private bool isProgressActive = true;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// The result path of the downloaded file. Returns an empry string if the file is not downloaded
        /// </summary>
        public string ResultPath
        {
            get { return resultPath; }
            set
            {
                if (value != resultPath)
                {
                    resultPath = value;
                    OnPropertyChanged();
                }
            }
        }
        public float Progress
        {
            get { return progress; }
            set
            {
                if (value != progress)
                {
                    progress = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TotalFileSize // 2nd match of the "downloadInfoRegex" regex
        {
            get { return totalFileSize; }
            set
            {
                if (value != totalFileSize)
                {
                    totalFileSize = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TotalDownloadedDataSize // 1st match
        {
            get { return totalDownloadedDataSize; }
            set
            {
                if (value != totalDownloadedDataSize)
                {
                    totalDownloadedDataSize = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DownloadSpeed // 3rd match
        {
            get { return downloadSpeed; }
            set
            {
                if (value != downloadSpeed)
                {
                    downloadSpeed = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                if (value != title)
                {
                    title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                if (value != description)
                {
                    description = value;
                    OnPropertyChanged();
                }
            }
        }


        public bool IsProgressActive
        {
            get
            {
                return isProgressActive;
            }
            set
            {
                if (value != isProgressActive)
                {
                    isProgressActive = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsProgressIntermeinate
        {
            get
            {
                return isProgressIntermeinate;
            }
            set
            {
                if (value != isProgressIntermeinate)
                {
                    isProgressIntermeinate = value;
                    OnPropertyChanged();
                }

            }
        }




        public FireBrowserDownloader(string url)
        {
            Download(url);
        }

        private async void Download(string url)
        {
            Process process = new();

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = FireDownloader.FireDownloaderPath,
                ArgumentList = { "-d " + FireDownloader.DlFolderPath, url },
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            process.StartInfo = startInfo;
            process.Start();




            while (!process.HasExited)
            {
                // Read a line from the StandardOutput stream
                string line = await process.StandardOutput.ReadLineAsync();

                if (line != null)
                {
                    // Process the line as needed
                    UpdateValues(line);
                }


            }

            IsProgressActive = false;
            Description = "Open file";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateValues(string data)
        {

            if (!string.IsNullOrEmpty(data))
            {
                IsProgressActive = true;

                Regex downloadInfoRegex = new("[0-9]{1,4}[a-zA-Z]{0,2}B"); // will match with the download info  (ex: with [#a139c3 4.9MiB/5.0MiB(98%) CN:1 DL:1.0MiB])
                var matches = downloadInfoRegex.Matches(data);

                if (matches.Count == 3) // Test if data match with the regex (will match with 3 part of the string)
                {
                    TotalDownloadedDataSize = matches[0].Value; // ex (in relation with the previous ex): 4.9MiB
                    TotalFileSize = matches[1].Value; // ex: 5.0MiB
                    DownloadSpeed = matches[2].Value; // ex: 1.0MiB (2 to 5 times faster than regular downloads on the network i tested) 

                    Regex percentageRegex = new(@"\d{1,3}%"); // will match only if a percentage is present on the string
                    var percentage = percentageRegex.Match(data);

                    Description = $"{TotalDownloadedDataSize}/{TotalFileSize} Speed: {downloadSpeed}/s";

                    if (percentage.Success)
                    {
                        Progress = float.Parse(percentage.Value.Replace("%", ""));
                        IsProgressIntermeinate = false;
                    }
                    else
                    {
                        IsProgressIntermeinate = true;
                    }
                }
                else
                {
                    Regex pathRegex = new("[A-Z]:/.*"); // will match with the result file path if the the string is a "Download completed" string
                    var path = pathRegex.Match(data);
                    if (path.Success)
                    {
                        ResultPath = path.Value;
                        isProgressActive = false;

                        FileInfo file = new(ResultPath);
                        Title = file.Name;
                    }
                }
            }
        }
    }
}
