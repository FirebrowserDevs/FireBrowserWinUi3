using FireBrowserDataCore.Actions;
using FireBrowserExceptions;
using FireBrowserMultiCore;
using FireBrowserWinUi3.Controls;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;

namespace FireBrowserWinUi3.Pages.TimeLinePages;
public sealed partial class DownloadsTimeLine : Page
{
    public ListView DownloadItemsList => DownloadItemsListView;

    public DownloadsTimeLine()
    {
        this.InitializeComponent();
        GetDownloadItems();
    }

    private async void GetDownloadItems()
    {
        try
        {
            DownloadActions downloadActions = new DownloadActions(AuthService.CurrentUser.Username);
            List<FireBrowserDataCore.Models.DownloadItem> items = await downloadActions.GetAllDownloadItems();

            if (items.Count > 0)
            {
                items.ForEach(t => {
                    DownloadItem downloadItem = new(t.current_path);
                    downloadItem.Handler_DownloadItem_Status += DownloadItem_Handler_DownloadItem_Status;
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

    private void DownloadItem_Handler_DownloadItem_Status(object sender, DownloadItem.DownloadItemStatusEventArgs e)
    {
        switch (e.Status)
        {
            case DownloadItem.DownloadItemStatusEventArgs.EnumStatus.Added:
                break;
            case DownloadItem.DownloadItemStatusEventArgs.EnumStatus.Removed:
                DownloadItemsListView.Items.Remove(e.DownloadedItem);
                break;
            case DownloadItem.DownloadItemStatusEventArgs.EnumStatus.Updated:
                break;
            default:
                break;
        }
    }
}