using FireBrowserWinUi3.Services.ViewModels;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3Favorites;
using FireBrowserWinUi3MultiCore;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Windows.System;

namespace FireBrowserWinUi3.Pages.TimeLinePages;
public sealed partial class FavoritesTimeLine : Page
{
    public FavoritesTimeLine()
    {
        this.InitializeComponent();
        LoadFavs();
    }

    public FireBrowserWinUi3MultiCore.User user = AuthService.CurrentUser;
    public FavManager fs = new FavManager();

    string ctmtext;
    string ctmurl;

    public async void LoadFavs()
    {
        List<FavItem> favorites = fs.LoadFav();
        FavoritesListView.ItemsSource = favorites;
    }

    private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        TextBox textbox = sender as TextBox;

        List<FavItem> favorites = fs.LoadFav();
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
  

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        FavManager fs = new FavManager();
        fs.ClearFavs();
        LoadFavs();
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
                FavManager fs = new FavManager();
                FavItem selectedItem = new FavItem { Url = ctmurl, Title = ctmtext };
                fs.RemoveFavorite(selectedItem);
                LoadFavs();
                break;
                // Add other cases as needed
        }
        FavoritesContextMenu.Hide();
    }
}