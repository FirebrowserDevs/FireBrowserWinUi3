using FireBrowserBusiness;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DownloadItem : ListViewItem
    {
        private int Progress { get; set; } = 0;
        private string EstimatedEnd { get; set; } = string.Empty;

        private string _filepath;
        private CoreWebView2DownloadOperation _downloadOperation;
        public DownloadItem(CoreWebView2DownloadOperation downloadOperation)
        {
            this.InitializeComponent();

            _downloadOperation = downloadOperation;
            _filepath = _downloadOperation.ResultFilePath;

            _downloadOperation.BytesReceivedChanged += _downloadOperation_BytesReceivedChanged;
            _downloadOperation.StateChanged += _downloadOperation_StateChanged;
            _downloadOperation.EstimatedEndTimeChanged += _downloadOperation_EstimatedEndTimeChanged;

            fileName.Text = _downloadOperation.ResultFilePath.Substring(_filepath.LastIndexOf("\\") + 1);
        }

        public DownloadItem(string filepath)
        {
            this.InitializeComponent();

            _filepath = filepath;
            fileName.Text = _filepath.Substring(_filepath.LastIndexOf("\\") + 1);

            ResourceLoader resourceLoader = new();
            subtitle.Text = resourceLoader.GetString("OpenFile");

            progressRing.Visibility = Visibility.Collapsed;

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
                    subtitle.Text = resourceLoader.GetString("OpenFile");
                    SetIcon();


                    break;

                case CoreWebView2DownloadState.Interrupted: //[TODO] 
                    break;

                case CoreWebView2DownloadState.InProgress:
                    progressRing.Visibility = Visibility.Visible;
                    SetIcon();
                    break;



            }
        }

        private void _downloadOperation_BytesReceivedChanged(CoreWebView2DownloadOperation sender, object args)
        {
            try
            {
                long progress = 100 * sender.BytesReceived / sender.TotalBytesToReceive;
                subtitle.Text = ((int)progress).ToString() + "% - Downloading...";
                progressRing.Value = (int)progress;
                Progress = (int)progress;
            }
            catch { }
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

        private void SetIcon()
        {
            var window = (Application.Current as App)?.m_window as MainWindow;
            try
            {
                var icon = GetSmallFileIcon(_filepath);

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
            { window.DownloadFlyout.DownloadItemsListView.Items.Remove(this); }
        }
        private async new void Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Process.Start(_filepath);
            }
            catch
            {
                await Launcher.LaunchFileAsync(await StorageFile.GetFileFromPathAsync(_filepath));
            }

        }

        private new void RightTapped(object sender, RightTappedRoutedEventArgs e)
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

        private void DeleteMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(_filepath);
          
        }

        private void ShowMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "/select, " + _filepath);
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

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var window = (Application.Current as App)?.m_window as MainWindow;
            File.Delete(_filepath);
            window.DownloadFlyout.DownloadItemsListView.Items.Remove(this);
        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "/select, " + _filepath);
        }
    }
}
