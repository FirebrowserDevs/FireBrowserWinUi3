using FireBrowserMultiCore;
using FireBrowserWinUi3.Controls;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Pages.TimeLinePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DownloadsTimeLine : Page
    {
        public DownloadsTimeLine()
        {
            this.InitializeComponent();
            GetDownloadItems();
        }

        private async void GetDownloadItems()
        {
            FireBrowserMultiCore.User user = AuthService.CurrentUser;
            string path = null;
            string username = user.Username;
            string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username);
            string browserFolderPath = Path.Combine(userFolderPath, "Browser");
            string ebWebViewFolderPath = Path.Combine(browserFolderPath, "EBWebView");
            string defaultFolderPath = Path.Combine(ebWebViewFolderPath, "Default");


            try
            {
                StorageFolder defaultFolder = await StorageFolder.GetFolderFromPathAsync(defaultFolderPath);
                StorageFile file = await defaultFolder.GetFileAsync("History");

                path = file.Path;

                using (SqliteConnection connection = new SqliteConnection($"Filename={path}"))
                {
                    connection.Open();

                    SqliteCommand selectCommand = new SqliteCommand("SELECT current_path FROM downloads", connection);
                    SqliteDataReader query = selectCommand.ExecuteReader();

                    while (query.Read())
                    {
                        string filePath = query.GetString(0);

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            DownloadItem downloadItem = new DownloadItem(filePath);

                            // Assuming that 'DownloadItemsListView' is the name of your ListView control.
                            DownloadItemsListView.Items.Add(downloadItem);
                        }
                    }


                    connection.Close();
                }

            }
            catch (FileNotFoundException)
            {
                path = "History file not found.";
            }
            catch (UnauthorizedAccessException)
            {
                path = "Permission denied to access the History file.";
            }
            catch (Exception ex)
            {
                path = "An error occurred: " + ex.Message;
            }


        }

    }
}
