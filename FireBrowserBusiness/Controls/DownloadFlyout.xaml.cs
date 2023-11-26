using FireBrowserBusiness;
using FireBrowserMultiCore;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
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

    private async void GetDownloadItems()
    {
        try
        {

            FireBrowserMultiCore.User user = AuthService.CurrentUser;
            string username = user.Username;
            string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username);
            string databaseFolderPath = Path.Combine(userFolderPath, "Database");
            string db = Path.Combine(databaseFolderPath, "Downloads.db");
            // Assuming _databaseFilePath is set correctly


            using (SqliteConnection connection = new SqliteConnection($"Filename={db}"))
            {
                connection.Open();

                SqliteCommand selectCommand = new SqliteCommand("SELECT current_path FROM downloads", connection);
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    string filePath = query.GetString(0);

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        DownloadItem downloadItem = new(filePath);
                        DownloadItemsListView.Items.Insert(0, downloadItem);
                    }
                }

                connection.Close();
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions, such as file access or database errors
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
