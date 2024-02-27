using FireBrowserWinUi3;
using FireBrowserWinUi3.Services;
using FireBrowserWinUi3.Services.Events;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Controls;

public sealed partial class DownloadItem : ListViewItem
{
    private CoreWebView2DownloadOperation _downloadOperation;
    private string _filePath;
    private string _databaseFilePath;

    public event EventHandler<DownloadItemStatusEventArgs> Handler_DownloadItem_Status;
    private int Progress { get; set; } = 0;
    private string EstimatedEnd { get; set; } = string.Empty;
    /*  
     * this is set when items are created by pages or by the service
     * controls that don't inherit from UiElement like flyouts need extra creation steps because x:bind doesn't work.
    */
    public DownloadService ServiceDownloads { get; set; }

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
        _downloadOperation.BytesReceivedChanged += _downloadOperation_BytesReceivedChanged;
        _downloadOperation.StateChanged += _downloadOperation_StateChanged;
        _downloadOperation.EstimatedEndTimeChanged += _downloadOperation_EstimatedEndTimeChanged;

        fileName.Text = _downloadOperation.ResultFilePath.Substring(_filePath.LastIndexOf("\\") + 1);

        SetIcon();

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

        // add after completion to the database use dataCore; only if file exist on disk.
        if (ServiceDownloads is not DownloadService db && File.Exists(_filePath))
        {
            ServiceDownloads = App.GetService<DownloadService>();
            await ServiceDownloads.InsertAsync(_filePath, _downloadOperation.EstimatedEndTime.ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }
        else
        {
            await ServiceDownloads.InsertAsync(_filePath, _downloadOperation.EstimatedEndTime.ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

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

}
