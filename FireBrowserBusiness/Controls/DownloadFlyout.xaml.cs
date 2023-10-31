using FireBrowserMultiCore;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DownloadFlyout : Flyout
    {
        public DownloadFlyout()
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string downloadsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
            Process.Start("explorer.exe", downloadsFolderPath);


        }
    }
}
