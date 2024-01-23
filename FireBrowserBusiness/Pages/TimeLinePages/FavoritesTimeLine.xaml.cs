using FireBrowserBusinessCore.Helpers;
using FireBrowserFavorites;
using FireBrowserMultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Pages.TimeLinePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoritesTimeLine : Page
    {
        public FavoritesTimeLine()
        {
            this.InitializeComponent();
            LoadFavs();
        }

        public FireBrowserMultiCore.User user = AuthService.CurrentUser;
        public FavManager fs = new FavManager();

        string ctmtext;
        string ctmurl;

        public async void LoadFavs()
        {
            List<FavItem> favorites = fs.LoadFav(user);

            FavoritesListView.ItemsSource = favorites;
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;

            List<FavItem> favorites = fs.LoadFav(user);
            // Get all ListView items with the submitted search query
            var SearchResults = from s in favorites where s.Title.Contains(textbox.Text, StringComparison.OrdinalIgnoreCase) select s;
            // Set SearchResults as ItemSource for HistoryListView
            FavoritesListView.ItemsSource = SearchResults;
        }

        private void FavoritesListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = sender as ListView;
            var options = new FlyoutShowOptions()
            {
                Position = e.GetPosition(listView),
            };
            FavoritesContextMenu.ShowAt(listView, options);
            var item = ((FrameworkElement)e.OriginalSource).DataContext as FavItem;
            ctmtext = item.Title;
            ctmurl = item.Url;
        }

        private async void ClearFavs()
        {
            string username = user.Username;
            string databasePath = Path.Combine(
                UserDataManager.CoreFolderPath,
                UserDataManager.UsersFolderPath,
                username,
                "Database",
                "favorites.json"
            );

            FavoritesListView.ItemsSource = null;
            File.Delete(databasePath);
        }

        private async void RemoveFavorite(FavItem selectedItem)
        {
            string username = user.Username;
            string databasePath = Path.Combine(
                UserDataManager.CoreFolderPath,
                UserDataManager.UsersFolderPath,
                username,
                "Database",
                "favorites.json"
            );

            // Read the JSON content from the file
            string jsonContent = File.ReadAllText(databasePath);

            // Deserialize JSON content into a list of FavItem
            List<FavItem> favoritesList = JsonSerializer.Deserialize<List<FavItem>>(jsonContent);

            // Find and remove the selected item from the list
            if (selectedItem != null)
            {
                favoritesList.Remove(selectedItem);

                // Serialize the updated list back to JSON
                string updatedJsonContent = JsonSerializer.Serialize(favoritesList, new JsonSerializerOptions
                {
                    WriteIndented = true // Use this option if you want indented output
                });

                File.WriteAllText(databasePath, updatedJsonContent);

                // Update your UI or perform any other necessary actions
                FavoritesListView.ItemsSource = favoritesList;
            }
            else
            {
                // Handle the case where the selected item is null (optional)
                Console.WriteLine("Selected item is null.");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClearFavs();
        }
        private async void FavContextItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as AppBarButton).Tag)
            {
                case "OpenLnkInNewWindow":
                    await Launcher.LaunchUriAsync(new Uri($"{ctmurl}"));
                    break;
                case "Copy":
                    ClipBoard.WriteStringToClipboard(ctmurl);
                    break;
                case "CopyText":
                    ClipBoard.WriteStringToClipboard(ctmtext);
                    break;
                case "DeleteSingleRecord":
                    // Assuming ctmurl and ctmtext represent the URL and title of the selected item
                    //FavItem selectedItem = new FavItem { Url = ctmurl, Title = ctmtext };
                    //RemoveFavorite(selectedItem);
                    break;
                    // Add other cases as needed
            }
            FavoritesContextMenu.Hide();
        }


    }
}
