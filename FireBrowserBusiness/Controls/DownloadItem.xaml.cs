using FireBrowserBusiness;
using FireBrowserBusiness.Controls;
using FireBrowserMultiCore;
using FireBrowserWinUi3.Pages;
using FireBrowserWinUi3.Pages.TimeLinePages;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Controls;

public sealed partial class DownloadItem : ListViewItem
{
    private CoreWebView2DownloadOperation _downloadOperation;
    private string _filePath;
    private readonly string _databaseFilePath = "";

    private static DownloadsTimeLine downloadsTimeLineInstance;

    private int Progress { get; set; } = 0;
    private string EstimatedEnd { get; set; } = string.Empty;

    public DownloadItem(string filepath)
    {
        this.InitializeComponent();

        _filePath = filepath;
        fileName.Text = _filePath.Substring(_filePath.LastIndexOf("\\") + 1);

        progressRing.Visibility = Visibility.Collapsed;
        ResourceLoader resourceLoader = new();
        subtitle.Text = resourceLoader.GetString("OpenDownload");

        SetIcon();
    }


    public DownloadItem(CoreWebView2DownloadOperation downloadOperation)
    {
        this.InitializeComponent();

        _downloadOperation = downloadOperation;
        _filePath = _downloadOperation.ResultFilePath;

        FireBrowserMultiCore.User user = AuthService.CurrentUser;
        string username = user.Username;
        string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username);
        string databaseFolderPath = Path.Combine(userFolderPath, "Database");
        _databaseFilePath = Path.Combine(databaseFolderPath, "Downloads.db");

        _downloadOperation.BytesReceivedChanged += _downloadOperation_BytesReceivedChanged;
        _downloadOperation.StateChanged += _downloadOperation_StateChanged;
        _downloadOperation.EstimatedEndTimeChanged += _downloadOperation_EstimatedEndTimeChanged;

        fileName.Text = _downloadOperation.ResultFilePath.Substring(_filePath.LastIndexOf("\\") + 1);  

        SetIcon();

        InsertDownloadIntoDatabase();
    }

    private void _downloadOperation_EstimatedEndTimeChanged(CoreWebView2DownloadOperation sender, object args)
    {
        EstimatedEnd = sender.EstimatedEndTime;
    }

    private void _downloadOperation_StateChanged(CoreWebView2DownloadOperation sender, object args)
    {
        switch (sender.State)
        {
            case CoreWebView2DownloadState.Completed:
                progressRing.Visibility = Visibility.Collapsed;
                ResourceLoader resourceLoader = new();
                subtitle.Text = resourceLoader.GetString("OpenDownload");
              
                SetIcon();
                break;

            case CoreWebView2DownloadState.Interrupted: //[TODO] 
                break;

            case CoreWebView2DownloadState.InProgress:
                progressRing.Visibility = Visibility.Visible;
                break;



        }
    }

    private void _downloadOperation_BytesReceivedChanged(CoreWebView2DownloadOperation sender, object args)
    {
        try
        {
            long progress = 100 * sender.BytesReceived / sender.TotalBytesToReceive;
            subtitle.Text = ((int)progress).ToString() + "%";
            progressRing.Value = (int)progress;

            if (progress >= 100)
            {
                progressRing.Visibility = Visibility.Collapsed;
                SetIcon();
                ResourceLoader resourceLoader = new();
                subtitle.Text = resourceLoader.GetString("OpenDownload");

                // Simulate the download completion state
                SimulateDownloadCompletion(sender);
            }
        }
        catch { }
    }

    private async void SimulateDownloadCompletion(CoreWebView2DownloadOperation downloadOperation)
    {
        // Simulate completion by delaying and changing the state
        await Task.Delay(1000); // Adjust the delay as needed

        // Change the state to Completed
        downloadOperation.GetType().GetMethod("OnDownloadStateChange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(downloadOperation, new object[] { CoreWebView2DownloadState.Completed });
    }

    private void SetIcon()
    {
        try
        {
            var icon = GetSmallFileIcon(_filePath);

            if (icon != null)
            {
                var image = new Microsoft.UI.Xaml.Controls.Image();

                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream stream = new MemoryStream())
                {
                    icon.Save(stream);
                    stream.Position = 0;
                    bitmapImage.SetSource(stream.AsRandomAccessStream());
                }

                iconImage.Source = bitmapImage;
            }

        }
        catch (FileNotFoundException)
        { }
    }



    private Icon GetSmallFileIcon(string filePath)
    {
        SHFILEINFO shinfo = new SHFILEINFO();
        IntPtr hImgSmall = Win32.SHGetFileInfo(filePath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);

        if (hImgSmall != IntPtr.Zero)
        {
            Icon icon = (Icon)Icon.FromHandle(shinfo.hIcon).Clone();
            Win32.DestroyIcon(shinfo.hIcon);
            return icon;
        }

        return null;
    }

    class Win32
    {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_SMALLICON = 0x1;

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    private async void InsertDownloadIntoDatabase()
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={_databaseFilePath}"))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    INSERT INTO downloads (guid, current_path, end_time, start_time)
                    VALUES (@guid, @currentPath, @endTime, @startTime)";

                    // Parameters for the SQL command
                    command.Parameters.Add("@guid", SqliteType.Text).Value = Guid.NewGuid().ToString();
                    command.Parameters.Add("@currentPath", SqliteType.Text).Value = _filePath;
                    command.Parameters.Add("@endTime", SqliteType.Text).Value = _downloadOperation.EstimatedEndTime.ToString();
                    command.Parameters.Add("@startTime", SqliteType.Integer).Value = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (SqliteException ex)
        {
            // Handle database insertion error
            Debug.WriteLine($"Database insertion error: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Debug.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (_downloadOperation != null)
        {
            if (_downloadOperation.State == CoreWebView2DownloadState.Completed)
            {
                FlyoutShowOptions flyoutShowOptions = new FlyoutShowOptions();
                flyoutShowOptions.Position = e.GetPosition(this);
                contextMenu.ShowAt(this, flyoutShowOptions);
            }
        }
        else
        {
            FlyoutShowOptions flyoutShowOptions = new FlyoutShowOptions();
            flyoutShowOptions.Position = e.GetPosition(this);
            contextMenu.ShowAt(this, flyoutShowOptions);
        }
    }


    private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Delete from the database
            DeleteDownloadFromDatabase();

            // Delete the file
            File.Delete(_filePath); // Replace with the actual property name

            // Remove the item from the current ListView in MainWindow
            var window = (Application.Current as App)?.m_window as MainWindow;
            window.DownloadFlyout.DownloadItemsListView.Items.Remove(this);
        }
        catch (SqliteException ex)
        {
            // Handle SQLite exceptions
            Debug.WriteLine($"SQLite Exception: {ex.Message}");
            // You might want to show a user-friendly message here
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Debug.WriteLine($"An error occurred: {ex.Message}");
            // You might want to show a user-friendly message here
        }
    }


    private async void DeleteDownloadFromDatabase()
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={_databaseFilePath}"))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM downloads WHERE current_path = @currentPath";
                    command.Parameters.AddWithValue("@currentPath", _filePath);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            // Handle database deletion error
            Debug.WriteLine($"Database deletion error: {ex.Message}");
        }
    }

    private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
    {
        Process.Start("explorer.exe", "/select, " + _filePath);
    }
}
