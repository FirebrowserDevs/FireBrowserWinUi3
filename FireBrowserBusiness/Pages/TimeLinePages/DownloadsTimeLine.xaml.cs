using FireBrowserMultiCore;
using FireBrowserWinUi3.Controls;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;

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
    }
}
