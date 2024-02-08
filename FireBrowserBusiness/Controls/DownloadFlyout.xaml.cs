using FireBrowserBusiness;
using FireBrowserDataCore.Actions;
using FireBrowserExceptions;
using FireBrowserMultiCore;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace FireBrowserWinUi3.Controls;
public sealed partial class DownloadFlyout : Flyout
{
    public DownloadFlyout()
    {
        this.InitializeComponent();

        GetDownloadItems();
    }

    public void TriggerFlyoutUpdate()
    {
        DownloadItemsListView.Items.Clear();
        GetDownloadItems();
    }

    public async void GetDownloadItems()
    {
        try
        {
            DownloadActions downloadActions =  new DownloadActions(AuthService.CurrentUser.Username);
            List<FireBrowserDataCore.Models.DownloadItem> items = await downloadActions.GetAllDownloadItems(); 

            if (items.Count > 0) {
                items.ForEach(t => {
                    DownloadItem downloadItem = new(t.current_path);
                    DownloadItemsListView.Items.Insert(0, downloadItem);
                });
            };
            
        }
        catch (Exception ex)
        {
            // Handle any exceptions, such as file access or database errors
            ExceptionLogger.LogException(ex);
            Console.WriteLine($"Error accessing database: {ex.Message}");
        }
    }

    private void ShowDownloads_Click(object sender, RoutedEventArgs e)
    {
        var downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
        Process.Start("explorer.exe", downloadPath);
    }

    private void OpenDownloadsItem_Click(object sender, RoutedEventArgs e)
    {
        var window = (Application.Current as App)?.m_window as MainWindow;
        window.UrlBox.Text = "firebrowser://downloads";
        window.TabContent.Navigate(typeof(FireBrowserWinUi3.Pages.TimeLinePages.MainTimeLine));
    }
}
