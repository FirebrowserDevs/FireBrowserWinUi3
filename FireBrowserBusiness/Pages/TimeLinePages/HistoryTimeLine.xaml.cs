using FireBrowserDatabase;
using FireBrowserMultiCore;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using SQLitePCL;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace FireBrowserWinUi3.Pages.TimeLinePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HistoryTimeLine : Page
    {
        public HistoryTimeLine()
        {
            this.InitializeComponent();
            FetchBrowserHistory();
        }

        public FireBrowserMultiCore.User user = AuthService.CurrentUser;
        private ObservableCollection<HistoryItem> browserHistory;

        private async void FetchBrowserHistory()
        {
            Batteries.Init();
            try
            {
                string username = user.Username;
                string databasePath = Path.Combine(
                    UserDataManager.CoreFolderPath,
                    UserDataManager.UsersFolderPath,
                    username,
                    "Database",
                    "History.db"
                );

                if (File.Exists(databasePath))
                {
                    using (var connection = new SqliteConnection($"Data Source={databasePath};"))
                    {
                        await connection.OpenAsync();

                        string sql = "SELECT url, title, visit_count, last_visit_time FROM urls ORDER BY last_visit_time DESC";

                        using (SqliteCommand command = new SqliteCommand(sql, connection))
                        {
                            using (SqliteDataReader reader = command.ExecuteReader())
                            {
                                browserHistory = new ObservableCollection<HistoryItem>();

                                while (reader.Read())
                                {
                                    HistoryItem historyItem = new HistoryItem
                                    {
                                        Url = reader.GetString(0),
                                        Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                                        VisitCount = reader.GetInt32(2),
                                        LastVisitTime = reader.GetString(3),
                                    };

                                    // Fetch the image source here
                                    historyItem.ImageSource = new BitmapImage(new Uri("https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=" + historyItem.Url + "&size=32"));

                                    browserHistory.Add(historyItem);
                                }

                                // Bind the browser history items to the ListView
                                BigTemp.ItemsSource = browserHistory;
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Database file does not exist at the specified path.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }




        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FetchBrowserHistory();
        }

        private void FilterBrowserHistory(string searchText)
        {
            if (browserHistory == null) return;

            // Clear the collection to start fresh with filtered items
            BigTemp.ItemsSource = null;

            // Filter the browser history based on the search text
            var filteredHistory = new ObservableCollection<HistoryItem>(browserHistory
                .Where(item => item.Url.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                               item.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true));

            // Bind the filtered browser history items to the ListView
            BigTemp.ItemsSource = filteredHistory;
        }
        private void Ts_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = Ts.Text;
            FilterBrowserHistory(searchText);
        }

        private async void ClearDb()
        {
            string username = user.Username;
            string databasePath = Path.Combine(
                UserDataManager.CoreFolderPath,
                UserDataManager.UsersFolderPath,
                username,
                "Database",
                "History.db"
            );

            BigTemp.ItemsSource = null;
            await DbClear.ClearTable(databasePath, "urls");
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            ClearDb();
        }

        private string selectedHistoryItem;
        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // Get the selected HistoryItem object
            HistoryItem historyItem = ((FrameworkElement)sender).DataContext as HistoryItem;
            selectedHistoryItem = historyItem.Url;

            // Create a context menu flyout
            var flyout = new MenuFlyout();

            // Add a menu item for "Delete This Record" button
            var deleteMenuItem = new MenuFlyoutItem
            {
                Text = "Delete This Record",
            };

            // Set the icon for the menu item using the Unicode escape sequence
            deleteMenuItem.Icon = new FontIcon
            {
                Glyph = "\uE74D" // Replace this with the Unicode escape sequence for your desired icon
            };

            // Handle the click event directly within the right-tapped event handler
            deleteMenuItem.Click += (s, args) =>
            {
                string username = user.Username;
                string databasePath = Path.Combine(
                    UserDataManager.CoreFolderPath,
                    UserDataManager.UsersFolderPath,
                    username,
                    "Database",
                    "History.db"
                );
                // Perform the deletion logic here
                // Example: Delete data from the 'History' table where the 'Url' matches the selectedHistoryItem
                DbClearTableData db = new();
                db.DeleteTableData(databasePath, "urls", $"Url = '{selectedHistoryItem}'");
                if (BigTemp.ItemsSource is ObservableCollection<HistoryItem> historyItems)
                {
                    var itemToRemove = historyItems.FirstOrDefault(item => item.Url == selectedHistoryItem);
                    if (itemToRemove != null)
                    {
                        historyItems.Remove(itemToRemove);
                    }
                }
                // After deletion, you may want to update the UI or any other actions
            };

            flyout.Items.Add(deleteMenuItem);

            // Show the context menu flyout
            flyout.ShowAt((FrameworkElement)sender, e.GetPosition((FrameworkElement)sender));
        }
    }
}
